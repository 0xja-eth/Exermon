from django.db import models
from django.db.models.query import QuerySet

from game_module.models import GroupConfigure, Subject, QuestionStar
from item_module.models import *
from exermon_module.models import ExerSkill, HitType, TargetType
from player_module.models import Player, HumanPackItem, HumanPack
from record_module.models import QuestionSetRecord, PlayerQuestion, RecordSource
from utils.calc_utils import ExerciseSingleRewardCalc
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

	def convertToDict(self, type: str = None, **kwargs) -> dict:
		"""
		转化为字典
		Args:
			type (str): 转化类型
			**kwargs (**dict): 子类重载参数
		Returns:
			转化后的字典数据
		"""
		res = super().convertToDict()

		res['score'] = self.score
		res['win'] = self.win
		res['lose'] = self.lose

		return res


# ===================================================
#  对战物资槽
# ===================================================
class BattleItemSlot(SlotContainer):

	class Meta:
		verbose_name = verbose_name_plural = "对战物资槽"

	# 容器类型
	TYPE = ContainerType.BattleItemSlot

	# 最大物资槽数
	MAX_ITEM_COUNT = 3

	# 玩家
	player = models.OneToOneField('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 所接受的容器项类
	@classmethod
	def baseContItemClass(cls): return HumanPackItem

	# 所接受的槽项类
	@classmethod
	def acceptedSlotItemClass(cls): return BattleItemSlotItem

	# 获取容器容量（0为无限）
	@classmethod
	def defaultCapacity(cls): return cls.MAX_ITEM_COUNT

	def _equipContainer(self, index: int) -> HumanPack:
		"""
		获取指定装备ID的所属容器
		Args:
			index (int): 装备ID
		Returns:
			指定装备ID的所属容器项
		"""
		return self.exactlyPlayer().humanPack()

	def _create(self, player: Player):
		"""
		创建对战物资容器（创建角色时候执行）
		Args:
			player (Player): 玩家
		"""
		super()._create()
		self.player = player

	def owner(self) -> Player:
		"""
		获取容器的持有者
		Returns:
			持有玩家
		"""
		return self.player

	def ensureItemEquipable(self, equip_item: HumanPackItem):
		"""
		保证物品可以装备（战斗中使用）
		Args:
			equip_item (HumanPackItem): 装备项
		Raises:
			ErrorType.IncorrectItemType: 不正确的物品类型
		"""
		if equip_item.count != 1:
			raise GameException(ErrorType.IncorrectItemType)

		if not equip_item.item.battle_use:
			raise GameException(ErrorType.IncorrectItemType)

	def ensureEquipCondition(self, slot_item: 'BattleItemSlotItem',
							 equip_item: HumanPackItem):
		"""
		确保满足装备条件
		Args:
			slot_item (BattleItemSlotItem): 装备槽项
			equip_item (HumanPackItem): 装备项
		"""
		super().ensureEquipCondition(slot_item, equip_item)

		self.ensureItemEquipable(equip_item)

		return True

	def setPackItem(self, pack_item: HumanPackItem = None, index: int = None, force: bool = False):
		"""
		设置物资槽物品
		Args:
			pack_item (HumanPackItem): 人类物品容器项
			index (int): 槽编号
			force (bool): 是否强制设置（不损失背包物品）
		"""
		self.setEquip(equip_index=0, equip_item=pack_item, index=index, force=force)


# ===================================================
#  对战物资槽项
# ===================================================
class BattleItemSlotItem(SlotContItem):

	class Meta:
		verbose_name = verbose_name_plural = "对战物资槽项"

	# 容器项类型
	TYPE = ContItemType.BattleItemSlotItem

	# 容器
	container = models.ForeignKey('BattleItemSlot', on_delete=models.CASCADE,
							   null=True, verbose_name="容器")

	# 装备项
	pack_item = models.OneToOneField('player_module.HumanPackItem', null=True, blank=True,
									  on_delete=models.SET_NULL, verbose_name="装备")

	# 所属容器的类
	@classmethod
	def containerClass(cls): return BattleItemSlot

	# 所接受的装备项类（可多个）
	@classmethod
	def acceptedEquipItemClass(cls): return (HumanPackItem, )

	# 所接受的装备项属性名（可多个）
	@classmethod
	def acceptedEquipItemAttr(cls): return ('pack_item', )

	def isUsable(self) -> bool:
		"""
		能否使用
		Returns:
			返回能否使用
		"""
		return True

	def useItem(self, **kwargs):
		"""
		使用物品
		Args:
			**kwargs (**dict): 拓展参数
		"""
		super().useItem(**kwargs)
		self.dequip(index=0)


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
	BATTLE_PLAYERS_CACHE_KEY = "players"

	# 对战玩家缓存键
	BATTLE_ROUNDS_CACHE_KEY = "rounds"

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
		return "%s. %s VS %s" % (str(self.id), self.adminPlayer1(), self.adminPlayer2())

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
		rec = cls()
		rec.mode = mode
		rec.start(player1, player2)
		return rec

	def convertToDict(self, type: str = None, **kwargs) -> dict:
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

		players = ModelUtils.objectsToDict(self.battlePlayers())
		rounds = ModelUtils.objectsToDict(self.battleRounds())

		res = {
			'id': self.id,
			'mode': self.mode,
			'season_id': self.season_id,
			'create_time': create_time,
			'result_time': result_time,

			'players': players,
		}

		if type == "record":
			res['rounds'] = rounds

		return res

	def start(self, player1: Player, player2: Player):
		"""
		对战开始
		Args:
			player1 (Player): 玩家1
			player2 (Player): 玩家2
		"""
		self._initCaches()

		self.addPlayer(player1)
		self.addPlayer(player2)

	def terminate(self):
		"""
		结束对战
		"""
		self._generateResult()
		self._processResult()

		self.result_time = datetime.datetime.now()

		self.save()

	def _generateResult(self):
		"""
		生成对战结果
		"""
		pass

	def _processResult(self):
		"""
		处理对战结果
		"""
		pass

	def _initCaches(self):
		"""
		初始化所有缓存数据
		"""
		self._cache(self.BATTLE_PLAYERS_CACHE_KEY, [])
		self._cache(self.BATTLE_ROUNDS_CACHE_KEY, [])

	# region 玩家操作

	def battlePlayers(self) -> QuerySet:
		"""
		获取对战玩家
		Returns:
			对战玩家 QuerySet 对象
		"""
		# 结算时间为空，表示正在对战中
		if self.result_time is None:
			return self._getCachedBattlePlayers()

		return self.battleplayer_set.all()

	def firstPlayer(self) -> 'BattlePlayer':
		"""
		获取第一个玩家实例
		Returns:
			如果有第一个玩家，则返回其实例，否则返回 None
		"""
		# 结算时间为空，表示正在对战中
		if self.result_time is None:
			return self._getCachedBattlePlayers()[0]

		players = self.battlePlayers()
		if players.count() >= 1:
			return players[0]
		return None

	def secondPlayer(self) -> 'BattlePlayer':
		"""
		获取第二个玩家实例
		Returns:
			如果有第二个玩家，则返回其实例，否则返回 None
		"""
		if self.result_time is None:
			return self._getCachedBattlePlayers()[1]

		players = self.battlePlayers()
		if players.count() >= 2:
			return players[1]
		return None

	def addPlayer(self, player: Player) -> 'BattlePlayer':
		"""
		添加一个对战玩家
		Args:
			player (Player): 玩家
		"""
		player = BattlePlayer.create(player, record=self)
		self._addBattlePlayerCache(player)

		return player

	def _addBattlePlayerCache(self, player: 'BattlePlayer'):
		"""
		添加对战玩家到缓存中
		Args:
			player (BattlePlayer): 对战玩家
		"""
		cache = self._getCachedBattlePlayers()
		cache.append(player)

	def _getCachedBattlePlayers(self) -> list:
		"""
		获取缓存对战玩家数组
		Returns:
			返回当前缓存对战玩家数组
		"""
		return self._getCache(self.BATTLE_PLAYERS_CACHE_KEY)

	# endregion

	# region 回合操作

	def battleRounds(self) -> QuerySet:
		"""
		获取所有对战回合数据
		Returns:
			对战回合 QuerySet 对象
		"""
		return self.battleround_set.all()

	def currentRound(self) -> 'BattleRound':
		"""
		获取当前回合
		Returns:
			若对战未结束，返回最后一个回合（当前回合），否则返回空
		"""
		if self.result_time is not None: return None
		cache = self._getCachedBattleRounds()

		if len(cache) > 0: return cache[-1]
		return None

	def addRound(self) -> 'BattleRound':
		"""
		添加一个对战回合
		"""
		cache = self._getCachedBattleRounds()
		round = BattleRound.create(self, len(cache))

		players = self._getCachedBattlePlayers()
		for player in players: player.addRound(round)

		self._addBattleRoundCache(round)

		return round

	def startCurrentRound(self):
		"""
		开始当前回合（答题用）
		"""
		players = self._getCachedBattlePlayers()
		for player in players: player.startCurrentRound()

	def _addBattleRoundCache(self, round: 'BattleRound'):
		"""
		添加对战回合到缓存中
		Args:
			round (BattleRound): 对战回合
		"""
		cache = self._getCachedBattleRounds()
		cache.append(round)

	def _getCachedBattleRounds(self) -> list:
		"""
		获取缓存对战回合数组
		Returns:
			返回当前缓存对战回合数组
		"""
		return self._getCache(self.BATTLE_ROUNDS_CACHE_KEY)

	# endregion

	def subjects(self) -> set:
		"""
		获取对战玩家所选的科目数据
		Returns:
			所选科目数组
		"""
		player = self.firstPlayer()
		if player is None: return []
		player = player.player

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
	question = models.ForeignKey('question_module.Question', null=True, on_delete=models.CASCADE, verbose_name="题目")

	def __str__(self):
		return str(self.record)+" 回合 "+str(self.order)

	def convertToDict(self, type: str = None) -> dict:
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

		return battler.player

	def generateQuestion(self):
		"""
		生成题目，赋值到 question 中
		"""

		from utils.calc_utils import QuestionGenerateConfigure, QuestionGenerateType, QuestionGenerator

		player = self._generateConfigurePlayer()
		subject, star = self._generateSubjectAndStar()

		configure = QuestionGenerateConfigure(self, player, subject, ques_star=star, count=1,
											  gen_type=QuestionGenerateType.NotOccurFirst.value)

		gen = QuestionGenerator.generate(configure, True)
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
class BattlePlayer(QuestionSetRecord):

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

	# 行动评分（*100）
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
			  "行动：%.2f，附加：%.2f<br>" \
			  "总分：%.2f" % \
			  (self.time_score, self.hurt_score, self.damage_score,
			   self.recovery_score, self.correct_score, self.plus_score,
			   self.battleScore())

		return format_html(res)

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

	def generateName(self) -> str:
		"""
		生成题目集记录的名字
		Returns:
			生成的名字
		"""
		return '%s对战记录' % str(self.record)

	def _create(self, record: 'BattleRecord'):
		"""
		创建实例后用于配置具体属性
		Args:
			record (BattleRecord): 子类中定义参数
		"""
		self.record = record

	def convertToDict(self, type: str = None) -> dict:
		"""
		转化为字典
		Args:
			type (str): 转化类型
		Returns:
			转化后的字典数据
		"""
		res = super().convertToDict(type)

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

		cache = self._getQuestionsCache()
		if len(cache) > 0: return cache[-1]
		return None

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

	def sumRecover(self, player_queses: QuerySet = None) -> int:
		"""
		获取对战总回复
		Args:
			player_queses (QuerySet): 玩家题目关系集合，默认情况下为所有题目关系
		Returns:
			返回对战总回复
		"""
		return self._sumData('recovery', lambda d: d.recovery, player_queses)

	# endregion


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
class BattleRoundResult(PlayerQuestion):
	class Meta:
		verbose_name = verbose_name_plural = "对战回合结果"

	RESULT_TYPES = [
		(HitResultType.Unknown.value, "未知"),
		(HitResultType.Hit.value, "命中"),
		(HitResultType.Critical.value, "暴击"),
		(HitResultType.Miss.value, "回避"),
	]

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

	def convertToDict(self, type: str = None,
					  runtime_battler: 'RuntimeBattlePlayer' = None) -> dict:
		"""
		转化为字典
		Args:
			type (str): 转化类型
			runtime_battler (RuntimeBattlePlayer): 运行时对战玩家对象
		Returns:
			转化后的字典数据
		"""
		res = super().convertToDict(type)

		res['order'] = self.round.order
		res['attack'] = self.attack
		res['skill_id'] = self.skill_id
		res['target_type'] = self.target_type
		res['result_type'] = self.result_type
		res['hurt'] = self.hurt
		res['damage'] = self.damage
		res['recovery'] = self.recovery

		if runtime_battler is not None:
			runtime_battler.convertToDict(res)

		return res

	def _create(self, round: BattleRound):
		"""
		内部创建函数
		Args:
			round (BattleRound): 站都回合对象
		"""
		self.round = round

	def setQuestionSet(self, question_set: BattlePlayer):
		"""
		设置题目集（对战玩家）
		Args:
			question_set (BattlePlayer): 对战玩家
		"""
		self.battle_player = question_set

	def questionSet(self) -> BattlePlayer:
		"""
		获取题目集记录（对战玩家）
		Returns:
			对战玩家
		"""
		return self.battle_player

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

		print("self.hurt: "+str(self.hurt))
		print("self.damage: "+str(self.damage))

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
