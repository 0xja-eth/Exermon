from django.db import models
from django.db.models.query import QuerySet

from .item_system.containers import *
from .item_system.cont_items import *

from game_module.models import GroupConfigure, Subject, QuestionStar

from player_module.models import Player
from exermon_module.models import ExerSkill, HitType, TargetType
from record_module.models import QuesSetRecord, SelectingPlayerQuestion, RecordSource

from utils.calc_utils import ExerciseSingleRewardCalc, BattleResultRewardCalc
from utils.model_utils import CacheableModel, Common as ModelUtils
from utils.exception import ErrorType, GameException

from enum import Enum
import random, datetime

# Create your models here.


# =======================
# 对战评价表，记录对战评价所需分数以及增加/扣除星星数的关系
# =======================
class BattleResultJudge(GroupConfigure):
	"""
	对战评价表，记录对战评价所需分数以及增加/扣除星星数的关系
	"""

	class Meta:
		verbose_name = verbose_name_plural = "对战评价表"

	NOT_EXIST_ERROR = ErrorType.ResultJudgeNotExist

	# 评价要求分数
	score = models.PositiveSmallIntegerField(default=0, verbose_name="评价分数")

	# 胜利增加星星
	win = models.SmallIntegerField(default=0, verbose_name="胜利增星数")

	# 失败增加星星（负数为减少）
	lose = models.SmallIntegerField(default=0, verbose_name="失败增星数")

	def convert(self, type: str = None, **kwargs) -> dict:
		"""
		转化为字典
		Args:
			type (str): 转化类型
			**kwargs (**dict): 子类重载参数
		Returns:
			转化后的字典数据
		"""
		res = super().convert()

		res['score'] = self.score
		res['win'] = self.win
		res['lose'] = self.lose

		return res


# ===================================================
#  对战类型枚举
# ===================================================
class BattleMode(Enum):
	Normal = 0  # 经典模式


# ===================================================
#  对战记录表
# ===================================================
class BattleRecord(CacheableModel):
	class Meta:
		verbose_name = verbose_name_plural = "对战记录"

	# 常量声明
	MODES = [
		(BattleMode.Normal.value, '经典模式'),
	]

	# 对战玩家缓存键
	# BATTLE_PLAYERS_CACHE_KEY = "players"

	# 对战玩家缓存键
	# BATTLE_ROUNDS_CACHE_KEY = "rounds"

	# 对战模式
	mode = models.PositiveSmallIntegerField(default=BattleMode.Normal.value,
											choices=MODES, verbose_name="对战模式")

	# 赛季
	season = models.ForeignKey('season_module.CompSeason', on_delete=models.CASCADE,
							   verbose_name="赛季")

	# 对战时间
	create_time = models.DateTimeField(auto_now_add=True, verbose_name="对战时间")

	# 结算时间
	result_time = models.DateTimeField(null=True, verbose_name="结算时间")

	def __str__(self):
		return "%s. %s" % (str(self.id), self.generateName())

	# def __init__(self, *args, **kwargs):
	# 	super().__init__(*args, **kwargs)

	def generateName(self):
		"""
		生成名称
		Returns:
			返回对战记录名称
		"""
		return "%s VS %s" % (self.adminPlayer1(), self.adminPlayer2())

	# admin 显示玩家1
	def adminPlayer1(self):
		player = self.firstPlayer()
		return '-' if player is None else str(player)

	adminPlayer1.short_description = "玩家1"

	# admin 显示玩家2
	def adminPlayer2(self):
		player = self.secondPlayer()
		return '-' if player is None else str(player)

	adminPlayer2.short_description = "玩家2"

	@classmethod
	def _cacheForeignKeyModels(cls):
		return [BattlePlayer, BattleRound]

	@classmethod
	def create(cls, player1: Player, player2: Player, mode: int) -> 'BattleRecord':
		"""
		创建对战记录实例
		Args:
			player1 (Player): 玩家1
			player2 (Player): 玩家2
			mode (int): 对战模式
		Returns:
			本对战记录实例
		"""
		from season_module.runtimes import SeasonManager

		rec = cls()
		rec.mode = mode
		rec.season_id = SeasonManager.getCurrentSeason().id

		rec.save()

		rec.start(player1, player2)

		return rec

	def convert(self, type: str = None, **kwargs) -> dict:
		"""
		转化为字典
		Args:
			type (str): 转化类型
			**kwargs (**dict): 子类重载参数
		Returns:
			转化后的字典数据
		"""
		create_time = ModelUtils.timeToStr(self.create_time)
		result_time = ModelUtils.timeToStr(self.result_time)

		players = ModelUtils.objectsToDict(self.battlePlayers(), type=type)
		rounds = ModelUtils.objectsToDict(self.battleRounds(), type=type)

		res = {
			'id': self.id,
			'mode': self.mode,
			'season_id': self.season_id,
			'create_time': create_time,
			'result_time': result_time,

			'players': players,
		}

		if type == "record" or type == "result":
			res['rounds'] = rounds

		return res

	def start(self, player1: Player, player2: Player):
		"""
		对战开始
		Args:
			player1 (Player): 玩家1
			player2 (Player): 玩家2
		"""
		self._setupCachePool()

		self.addPlayer(player1)
		self.addPlayer(player2)

	def terminate(self, battle):
		"""
		结束对战
		Args:
			battle (RuntimeBattle): 运行时对战
		"""
		# self.save()
		# self._saveCache(self.BATTLE_ROUNDS_CACHE_KEY)

		self.firstPlayer().terminate(battle=battle)
		self.secondPlayer().terminate(battle=battle)

		self.result_time = datetime.datetime.now()

		self.save()

	# region 玩家操作

	def battlePlayers(self) -> QuerySet or list:
		"""
		获取战斗玩家
		Returns:
			返回战斗玩家集
		"""
		return self._queryModelCache(BattlePlayer)

	def firstPlayer(self) -> 'BattlePlayer':
		"""
		获取第一个玩家实例
		Returns:
			如果有第一个玩家，则返回其实例，否则返回 None
		"""
		players = self.battlePlayers()
		if len(players) >= 1: return players[0]

		return None

	def secondPlayer(self) -> 'BattlePlayer':
		"""
		获取第二个玩家实例
		Returns:
			如果有第二个玩家，则返回其实例，否则返回 None
		"""
		players = self.battlePlayers()
		if len(players) >= 2: return players[1]

		return None

	def getBattlePlayer(self, player: Player = None, battle_player: 'BattlePlayer' = None):
		"""
		获取对战玩家
		Args:
			player (Player): 玩家实例
			battle_player (BattlePlayer): 对战玩家实例
		Returns:
			返回自身对战玩家
		"""
		battle_player1 = self.firstPlayer()
		battle_player2 = self.secondPlayer()

		if player and player.id == battle_player1.player_id:
			return battle_player1
		if player and player.id == battle_player2.player_id:
			return battle_player2

		if battle_player and battle_player == battle_player1:
			return battle_player1
		if battle_player and battle_player == battle_player2:
			return battle_player2

		return None

	def getOppoBattlePlayer(self, player: Player = None, battle_player: 'BattlePlayer' = None):
		"""
		获取对方对战玩家
		Args:
			player (Player): 玩家实例
			battle_player (BattlePlayer): 对战玩家实例
		Returns:
			返回自身对战玩家
		"""
		battle_player1 = self.firstPlayer()
		battle_player2 = self.secondPlayer()

		if player and player.id == battle_player1.player_id:
			return battle_player2
		if player and player.id == battle_player2.player_id:
			return battle_player1

		if battle_player and battle_player == battle_player1:
			return battle_player2
		if battle_player and battle_player == battle_player2:
			return battle_player1

		return None

	def addPlayer(self, player: Player) -> 'BattlePlayer':
		"""
		添加一个对战玩家
		Args:
			player (Player): 玩家
		"""
		player = BattlePlayer.create(player, record=self)
		self._addBattlePlayer(player)

		return player

	def _addBattlePlayer(self, player: 'BattlePlayer'):
		"""
		添加对战玩家到缓存中
		Args:
			player (BattlePlayer): 对战玩家
		"""
		self._appendModelCache(BattlePlayer, player)

	# endregion

	# region 回合操作

	def battleRounds(self) -> QuerySet or list:
		"""
		获取对战回合数据
		Returns:
			返回对战回合数据
		"""
		return self._queryModelCache(BattleRound)

	def currentRound(self) -> 'BattleRound':
		"""
		获取当前回合
		Returns:
			若对战未结束，返回最后一个回合（当前回合），否则返回空
		"""
		if self.result_time is not None: return None
		rounds = list(self.battleRounds())

		if len(rounds) > 0: return rounds[-1]
		return None

	def addRound(self) -> 'BattleRound':
		"""
		添加一个对战回合
		"""
		rounds = self.battleRounds()
		round = BattleRound.create(self, len(rounds))

		players = self.battlePlayers()
		for player in players: player.addRound(round)

		self._addBattleRound(round)

		return round

	def startCurrentRound(self):
		"""
		开始当前回合（答题用）
		"""
		players = self.battlePlayers()
		for player in players: player.startCurrentRound()

	def _addBattleRound(self, round: 'BattleRound'):
		"""
		添加对战回合到缓存中
		Args:
			round (BattleRound): 对战回合
		"""
		self._appendModelCache(BattleRound, round)

	# endregion

	def subjects(self) -> set:
		"""
		获取对战玩家所选的科目数据
		Returns:
			所选科目数组
		"""
		player = self.firstPlayer()
		if player is None: return []
		player = player.exactlyPlayer()

		return player.subjects()


# ===================================================
#  对战回合
# ===================================================
class BattleRound(models.Model):

	class Meta:

		verbose_name = verbose_name_plural = "对战回合"

	# 回合序号
	order = models.PositiveSmallIntegerField(default=0, verbose_name="回合号")

	# 关联的对战记录
	record = models.ForeignKey('BattleRecord', on_delete=models.CASCADE, verbose_name="对战记录")

	# 题目
	question = models.ForeignKey('question_module.GeneralQuestion',
								 null=True, on_delete=models.CASCADE, verbose_name="题目")

	def __str__(self):
		return str(self.record)+" 回合 "+str(self.order)

	def convert(self, type: str = None) -> dict:
		"""
		转化为字典
		Args:
			type (str): 转化类型
		Returns:
			转化后的字典数据
		"""
		return {
			'order': self.order,
			'subject_id': self.question.subject_id,
			'star_id': self.question.star_id,
			'question_id': self.question_id
		}

	# 创建对象
	@classmethod
	def create(cls, battle: BattleRecord, order: int) -> 'BattleRound':
		"""
		创建一个对战回合
		Args:
			battle (BattleRecord): 对战记录
			order (int): 回合序号（从0开始）
		Returns:
			新创建的对战回合对象
		"""
		round = cls()
		round.order = order
		round.record = battle

		round.save()

		round.generateQuestion()

		return round

	# 生成科目和星级
	def _generateSubjectAndStar(self) -> (Subject, QuestionStar):
		"""
		生成随机科目和星级
		Returns:
			随机生成的科目, 题目星级
		"""
		return random.choice(list(self.record.subjects())), \
			   random.choice(QuestionStar.objs())

	def _generateConfigurePlayer(self) -> Player:
		"""
		生成本回合题目生成配置时所需的玩家
		Returns:
			题目生成配置所需的玩家对象
		"""
		battler = self.record.firstPlayer() if self.order % 2 == 1 \
			else self.record.secondPlayer()

		return battler.exactlyPlayer()

	def generateQuestion(self):
		"""
		生成题目，赋值到 question 中
		"""

		from utils.calc_utils import BaseQuestionGenerateConfigure, QuestionGenerateType, GeneralQuestionGenerator

		player = self._generateConfigurePlayer()
		subject, star = self._generateSubjectAndStar()

		configure = BaseQuestionGenerateConfigure(self, player, subject, ques_star=star, count=1,
												  gen_type=QuestionGenerateType.NotOccurFirst.value)

		gen = GeneralQuestionGenerator.generate(configure, True)
		result = gen.result

		if len(result) > 0: self.question_id = result[0]
		# 没有题目生成
		else: raise GameException(ErrorType.GenerateError)


# ===================================================
#  对战玩家状态枚举
# ===================================================
class BattlePlayerStatus(Enum):
	Normal = 0
	Disconnected = 1
	Cancelled = 2


# ===================================================
#  玩家对战结果枚举
# ===================================================
class BattlePlayerResult(Enum):
	Win = 1  # 胜利
	Lose = 2  # 失败
	Tie = 3  # 平局


# ===================================================
#  对战玩家表
# ===================================================
class BattlePlayer(QuesSetRecord):

	class Meta:
		verbose_name = verbose_name_plural = "对战玩家"

	STATUSES = [
		(BattlePlayerStatus.Normal.value, "正常"),
		(BattlePlayerStatus.Disconnected.value, "掉线"),
		(BattlePlayerStatus.Cancelled.value, "退出"),
	]

	RESULT_TYPES = [
		(BattlePlayerResult.Win.value, "胜利"),
		(BattlePlayerResult.Lose.value, "失败"),
		(BattlePlayerResult.Tie.value, "平局"),
	]

	LIST_DISPLAY_APPEND = ['adminScores']

	# 关联的记录
	record = models.ForeignKey('BattleRecord', on_delete=models.CASCADE, verbose_name="对战记录")

	# 积分变更
	score_incr = models.SmallIntegerField(null=True, verbose_name="积分变更")

	# 用时评分（*100）
	time_score = models.PositiveSmallIntegerField(null=True, verbose_name="用时评分")

	# 伤害评分（*100）
	hurt_score = models.PositiveSmallIntegerField(null=True, verbose_name="伤害评分")

	# 承伤评分（*100）
	damage_score = models.PositiveSmallIntegerField(null=True, verbose_name="承伤评分")

	# 恢复评分（*100）
	recovery_score = models.PositiveSmallIntegerField(null=True, verbose_name="恢复评分")

	# 正确评分（*100）
	correct_score = models.PositiveSmallIntegerField(null=True, verbose_name="行动评分")

	# 奖励分数（*100）
	plus_score = models.PositiveSmallIntegerField(null=True, verbose_name="奖励分数")

	# 战斗结果
	result = models.PositiveSmallIntegerField(null=True, choices=RESULT_TYPES, verbose_name="战斗结果")

	# 战斗标志
	status = models.PositiveSmallIntegerField(null=True, choices=STATUSES, verbose_name="战斗状态标志")

	def __str__(self):
		return str(self.player)

	# admin 用
	def adminScores(self):
		from django.utils.html import format_html

		res = "用时：%.2f，伤害：%.2f<br>" \
			  "承伤：%.2f，回复：%.2f<br>" \
			  "正确：%.2f，附加：%.2f<br>" \
			  "总分：%.2f" % \
			  (self.time_score, self.hurt_score, self.damage_score,
			   self.recovery_score, self.correct_score, self.plus_score,
			   self.battleScore())

		return format_html(res)

	# region 配置

	@classmethod
	def rewardCalculator(cls) -> BattleResultRewardCalc:
		"""
		奖励计算类
		Returns:
			返回对应的奖励计算类类对象
		"""
		return BattleResultRewardCalc

	@classmethod
	def playerQuesClass(cls) -> 'BattleRoundResult':
		"""
		该类对应的玩家题目关系类，用于 addQuestion 中创建一个题目关系
		Returns:
			返回 BattleRoundResult 本身
		"""
		return BattleRoundResult

	@classmethod
	def rewardClass(cls):
		"""
		该类对应的奖励记录类
		Returns:
			返回为空
		"""
		return None

	def _playerQuestions(self) -> QuerySet:
		"""
		获取所有题目关系（数据库）
		Returns:
			题目关系 QuerySet 对象
		"""
		return self.battleroundresult_set.all()

	def _rewards(self) -> QuerySet:
		"""
		获取所有奖励（数据库）
		Returns:
			题目集奖励 QuerySet 对象
		"""
		return []

	# endregion

	def generateName(self) -> str:
		"""
		生成题目集记录的名字
		Returns:
			生成的名字
		"""
		return self.record.generateName()

	def _create(self, record: 'BattleRecord'):
		"""
		创建实例后用于配置具体属性
		Args:
			record (BattleRecord): 子类中定义参数
		"""
		self.record = record

	def convert(self, type: str = None) -> dict:
		"""
		转化为字典
		Args:
			type (str): 转化类型
		Returns:
			转化后的字典数据
		"""
		res = super().convert(type)

		res['pid'] = self.player_id
		res['score_incr'] = self.score_incr

		# res['sum_hurt'] = self.sumHurt()
		# res['sum_damage'] = self.sumDamage()
		# res['sum_recover'] = self.sumRecover()

		res['time_score'] = self.time_score/100
		res['hurt_score'] = self.hurt_score/100
		res['damage_score'] = self.damage_score/100
		res['recovery_score'] = self.recovery_score/100
		res['correct_score'] = self.correct_score/100
		res['plus_score'] = self.plus_score/100

		res['result'] = self.result
		res['status'] = self.status

		return res

	def battleScore(self) -> int:
		"""
		获取最终对战评分
		Returns:
			对战评分
		"""
		return (self.time_score + self.hurt_score + self.damage_score +
				self.recovery_score + self.correct_score) / 5 + self.plus_score

	def currentRound(self) -> 'BattleRoundResult':
		"""
		获取当前回合对象
		Returns:
			返回当前回合对象（BattleRoundResult）（从缓存）
		"""
		if self.finished: return None

		rounds = list(self.playerQuestions())
		if len(rounds) > 0: return rounds[-1]
		return None

	# region 回合操作

	def addRound(self, round: BattleRound):
		"""
		添加回合
		Args:
			round (BattleRound): 对战回合
		"""
		self.addQuestion(round.question_id, round=round)

	def startCurrentRound(self):
		"""
		开始当前回合
		"""
		cur_round = self.currentRound()
		if cur_round is None: return

		self.startQuestion(player_ques=cur_round)

	def answerCurrentRound(self, selection: list, timespan: int):
		"""
		作答当前回合
		Args:
			selection (list): 选择情况
			timespan (int): 作答时长
		"""
		cur_round = self.currentRound()
		if cur_round is None: return

		self.answerQuestion(selection, timespan, player_ques=cur_round)

	# endregion

	# region 对战结束

	def _applyBaseResult(self, calc: BattleResultRewardCalc):
		"""
		应用基本结果
		Args:
			calc (BattleResultRewardCalc): 结果
		"""
		super()._applyBaseResult(calc)

		self.result = calc.result.value
		self.status = calc.status.value
		self.score_incr = calc.score_incr

		self.time_score = calc.battle_scores.time_score*100
		self.hurt_score = calc.battle_scores.hurt_score*100
		self.damage_score = calc.battle_scores.damage_score*100
		self.recovery_score = calc.battle_scores.recovery_score*100
		self.correct_score = calc.battle_scores.correct_score*100
		self.plus_score = calc.battle_scores.plus_score*100

	def _applyPlayerResult(self, calc: BattleResultRewardCalc):
		"""
		应用玩家结果
		Args:
			calc (BattleResultRewardCalc): 结果
		"""
		super()._applyPlayerResult(calc)

		player = self.exactlyPlayer()

		season_record = player.currentSeasonRecord()

		season_record.adjustCredit(calc.credit_incr)
		season_record.adjustPoint(calc.score_incr)
		season_record.adjustStarNum(calc.star_incr)

		season_record.save()

	# endregion

	# region 统计数据

	def sumHurt(self, player_queses: QuerySet = None) -> int:
		"""
		获取对战总伤害
		Args:
			player_queses (QuerySet): 玩家题目关系集合，默认情况下为所有题目关系
		Returns:
			返回对战总伤害
		"""
		return self._sumData('hurt', lambda d: d.hurtPoint(), player_queses)

	def sumDamage(self, player_queses: QuerySet = None) -> int:
		"""
		获取对战总承伤
		Args:
			player_queses (QuerySet): 玩家题目关系集合，默认情况下为所有题目关系
		Returns:
			返回对战总承伤
		"""
		return self._sumData('damage', lambda d: d.damagePoint(), player_queses)

	def sumRecovery(self, player_queses: QuerySet = None) -> int:
		"""
		获取对战总回复
		Args:
			player_queses (QuerySet): 玩家题目关系集合，默认情况下为所有题目关系
		Returns:
			返回对战总回复
		"""
		return self._sumData('recovery', lambda d: d.recovery, player_queses)

	# endregion

	"""占位符"""


# ===================================================
#  对战回合结果类型枚举
# ===================================================
class HitResultType(Enum):
	Unknown = 0  # 未知
	Hit = 1  # 命中
	Critical = 2  # 命中
	Miss = 3  # 回避


# ===================================================
#  对战回合结果表
# ===================================================
class BattleRoundResult(SelectingPlayerQuestion):
	class Meta:
		verbose_name = verbose_name_plural = "对战回合结果"

	RESULT_TYPES = [
		(HitResultType.Unknown.value, "未知"),
		(HitResultType.Hit.value, "命中"),
		(HitResultType.Critical.value, "暴击"),
		(HitResultType.Miss.value, "回避"),
	]

	LIST_EDITABLE_EXCLUDE = ['round', 'battle_player']

	# 关联的回合
	round = models.ForeignKey('BattleRound', on_delete=models.CASCADE, verbose_name="回合")

	# 对战玩家
	battle_player = models.ForeignKey('BattlePlayer', on_delete=models.CASCADE, verbose_name="对战玩家")

	# 是否进攻
	attack = models.BooleanField(null=True, verbose_name="是否进攻")

	# 使用技能（为 None 则是普通攻击）（本回合攻击方的技能）
	skill = models.ForeignKey("exermon_module.ExerSkill", null=True,
							  on_delete=models.SET_NULL, verbose_name="使用技能")

	# 目标（本回合攻击方的目标）
	target_type = models.PositiveSmallIntegerField(default=TargetType.Enemy.value,
												   choices=ExerSkill.TARGET_TYPES, verbose_name="目标")

	# 回合结果（本回合攻击方的结果）
	result_type = models.PositiveSmallIntegerField(default=HitResultType.Unknown.value,
												   choices=RESULT_TYPES, verbose_name="回合结果")

	# 伤害点数（自己对目标造成的HP伤害，小于0为恢复）
	hurt = models.SmallIntegerField(default=0, verbose_name="伤害点数")

	# 承伤点数（任何自己遭受的HP伤害，小于0为恢复）
	damage = models.SmallIntegerField(default=0, verbose_name="承伤点数")

	# 回复点数（通过物品的HP回复，若物品需要扣除HP则不算入内）
	recovery = models.PositiveSmallIntegerField(default=0, verbose_name="回复点数")

	@classmethod
	def rewardCalculator(cls) -> ExerciseSingleRewardCalc:
		"""
		获取对应的奖励计算类
		Returns:
			对应奖励计算类本身（继承自 QuestionSetSingleRewardCalc）
		"""
		return ExerciseSingleRewardCalc

	@classmethod
	def source(cls) -> RecordSource:
		"""
		题目来源
		Returns:
			题目来源枚举成员
		"""
		return RecordSource.Battle

	def convert(self, type: str = None,
					  runtime_battler: 'RuntimeBattlePlayer' = None) -> dict:
		"""
		转化为字典
		Args:
			type (str): 转化类型
			runtime_battler (RuntimeBattlePlayer): 运行时对战玩家对象
		Returns:
			转化后的字典数据
		"""
		res = super().convert(type)

		res['order'] = self.round.order
		res['attack'] = self.attack
		res['skill_id'] = self.skill_id
		res['target_type'] = self.target_type
		res['result_type'] = self.result_type
		res['hurt'] = self.hurt
		res['damage'] = self.damage
		res['recovery'] = self.recovery

		if runtime_battler is not None:
			runtime_battler.convert(res)

		return res

	def _create(self, round: BattleRound):
		"""
		内部创建函数
		Args:
			round (BattleRound): 站都回合对象
		"""
		self.round = round

	# def setQuestionSet(self, question_set: BattlePlayer):
	# 	"""
	# 	设置题目集（对战玩家）
	# 	Args:
	# 		question_set (BattlePlayer): 对战玩家
	# 	"""
	# 	self.battle_player = question_set
	#
	# def questionSet(self) -> BattlePlayer:
	# 	"""
	# 	获取题目集记录（对战玩家）
	# 	Returns:
	# 		对战玩家
	# 	"""
	# 	return self.battle_player

	def start(self):
		super().start()
		self.hurt = self.damage = self.recovery = 0

	def processRecovery(self, recovery: int):
		"""
		处理道具回复
		Args:
			recovery (int): 道具回复量
		"""
		if recovery > 0: self.recovery += recovery

	def processAttack(self, skill: ExerSkill, target_type: TargetType,
				  result_type: HitResultType, hurt: int, attacker: bool):
		"""
		处理回合攻击
		Args:
			skill (ExerSkill): 技能（为 None 则为普通攻击）
			target_type (TargetType): 实际目标类型（有可能与技能的目标类型不一致）
			result_type (HitResultType): 命中结果类型
			hurt (int): 伤害点数
			attacker (bool): 自己是否为攻击方
		"""
		self.skill = skill
		self.target_type = target_type.value
		self.result_type = result_type.value

		self._processHurt(hurt, attacker)

	def _processHurt(self, hurt: int, attacker: bool):
		"""
		处理伤害（保存记录）
		Args:
			hurt (int): 伤害值
			attacker (bool): 自己是否为攻击方
		"""
		if self.skill is None:
			hit_type = HitType.HPDamage
		else:
			hit_type = HitType(self.skill.hit_type)

		if hit_type == HitType.MPDamage or \
			hit_type == HitType.HPRecover or \
			hit_type == HitType.MPRecover or \
			hit_type == HitType.MPDrain: return

		target_type = TargetType(self.target_type)

		# 对敌攻击/双方攻击，如果是攻击方则计入伤害点数，否则计入承伤点数
		if target_type == TargetType.Enemy or target_type == TargetType.Both:
			if attacker: self.hurt += hurt
			else: self.damage += hurt

		# 对己攻击，如果是攻击方计入承伤点数，不计入伤害点数
		if target_type == TargetType.Self:
			if attacker: self.damage += hurt

	def hurtPoint(self) -> int:
		"""
		实际对敌伤害值
		Returns:
			返回实际的对敌伤害值
		"""
		skill = self.skill

		if skill is None: return self.hurt
		if skill.hit_type == HitType.HPDamage and \
			self.target_type == TargetType.Both or \
			self.target_type == TargetType.Enemy:
			return self.hurt

	def damagePoint(self) -> int:
		"""
		实际受到的伤害点数
		Returns:
			获取实际受到的伤害
		"""
		skill = self.skill

		if skill is None: return self.damage
		if skill.hit_type == HitType.HPDamage and \
			self.target_type == TargetType.Both or \
			self.target_type == TargetType.Enemy:
			return self.damage
