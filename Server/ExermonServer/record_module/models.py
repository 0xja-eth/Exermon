from django.db import models
from django.db.models.query import QuerySet

from item_module.models import BaseItem, BaseContainer
# from player_module.models import Player
from utils.model_utils import CacheableModel, Common as ModelUtils
from utils.calc_utils import QuestionGenerateType, QuestionGenerateConfigure, \
	QuestionSetResultRewardCalc, QuestionSetSingleRewardCalc, \
	ExerciseSingleRewardCalc
from utils.exception import ErrorType, GameException
from enum import Enum
import jsonfield, datetime

# Create your models here.


# ===================================================
#  记录来源
# ===================================================
class RecordSource(Enum):
	Exercise = 1  # 刷题
	Exam = 2  # 考核
	Battle = 3  # 对战
	Adventure = 4  # 冒险
	Others = 0  # 其他


# ===================================================
#  题目记录表
# ===================================================
class QuestionRecord(models.Model):

	class Meta:

		verbose_name = verbose_name_plural = "做题记录"

	SOURCES = [
		(RecordSource.Exercise.value, '刷题'),
		(RecordSource.Exam.value, '考核'),
		(RecordSource.Battle.value, '对战'),
		(RecordSource.Adventure.value, '冒险'),
		(RecordSource.Others.value, '其他'),
	]

	MAX_NOTE_LEN = 128

	# 题目
	question = models.ForeignKey('question_module.Question', null=False,
								 on_delete=models.CASCADE, verbose_name="题目")

	# 玩家
	player = models.ForeignKey('player_module.Player', null=False,
							   on_delete=models.CASCADE, verbose_name="玩家")

	# 做题次数
	count = models.PositiveSmallIntegerField(default=0, verbose_name="次数")

	# 正确次数
	correct = models.PositiveSmallIntegerField(default=0, verbose_name="正确数")

	# 上次做题日期
	last_date = models.DateTimeField(null=True, verbose_name="上次做题日期")

	# 初次做题日期
	first_date = models.DateTimeField(null=True, verbose_name="初次做题日期")

	# 初次用时（毫秒数）
	first_time = models.PositiveIntegerField(default=0, verbose_name="初次用时")

	# 平均用时（毫秒数）
	avg_time = models.PositiveIntegerField(default=0, verbose_name="平均用时")

	# 首次正确用时（毫秒数）
	corr_time = models.PositiveIntegerField(null=True, verbose_name="首次正确用时")

	# 累计获得经验
	sum_exp = models.PositiveSmallIntegerField(default=0, verbose_name="上次得分")

	# 累计获得金币
	sum_gold = models.PositiveSmallIntegerField(default=0, verbose_name="平均得分")

	# 记录来源（初次）
	source = models.PositiveSmallIntegerField(default=RecordSource.Others.value,
											  choices=SOURCES, verbose_name="记录来源")

	# 收藏标志
	collected = models.BooleanField(default=False, verbose_name="收藏标志")

	# 错题标志
	wrong = models.BooleanField(default=False, verbose_name="错题标志")

	# 备注
	note = models.CharField(blank=True, null=True, max_length=MAX_NOTE_LEN, verbose_name="备注")

	# 转化为字符串
	def __str__(self):
		return '%s (%s)' % (self.question.number(), self.player)

	# 转化为字典
	def convertToDict(self, type=None):

		last_date = ModelUtils.timeToStr(self.last_date)
		first_date = ModelUtils.dateToStr(self.first_date)

		return {
			'id': self.id,
			'question_id': self.question_id,
			'count': self.count,
			'correct': self.correct,
			'first_date': first_date,
			'last_date': last_date,
			'first_time': self.first_time,
			'avg_time': self.avg_time,
			'corr_time': self.corr_time,
			'sum_exp': self.sum_exp,
			'sum_gold': self.sum_gold,
			'source': self.source,
			'collected': self.collected,
			'wrong': self.wrong,
			'note': self.note
		}

	# 创建新记录
	@classmethod
	def create(cls, player, question_id):
		record = player.questionRecord(question_id)

		if record is None:
			record = cls()
			record.player = player
			record.question_id = question_id
			record.save()

		return record

	# 更新已有记录
	def updateRecord(self, player_ques):

		timespan = player_ques.timespan

		if player_ques.correct():
			if self.corr_time <= 0:
				self.corr_time = timespan
			self.correct += 1

		if self.count <= 0:
			self.source = player_ques.source().value

			self.first_date = datetime.datetime.now()
			self.first_time = timespan

		sum_time = self.avg_time*self.count+timespan
		self.avg_time = round(sum_time/(self.count+1))

		self.sum_exp += player_ques.exp_incr
		self.sum_gold += player_ques.gold_incr

		self.last_date = datetime.datetime.now()
		self.count += 1

		self.save()

	# 正确率
	def corrRate(self):
		return self.correct / self.count


# ===================================================
#  玩家题目关系表
# ===================================================
class PlayerQuestion(CacheableModel):
	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "玩家题目关系"

	# 前后端时间差最大值（毫秒）
	MAX_DELTATIME = 10*1000

	# 缓存键配置
	STARTTIME_CACHE_KEY = 'start_time'
	CORRECT_CACHE_KEY = 'correct'
	SCORE_CACHE_KEY = 'score'

	# 题目
	question = models.ForeignKey('question_module.Question',
								 on_delete=models.CASCADE, verbose_name="题目")

	# 选择情况
	selection = jsonfield.JSONField(default=[], verbose_name="选择情况")

	# 是否作答
	answered = models.BooleanField(default=False, verbose_name="作答标志")

	# 用时（毫秒数，0为未作答）
	timespan = models.PositiveIntegerField(default=0, verbose_name="用时")

	# 经验增加
	exp_incr = models.SmallIntegerField(null=True, verbose_name="经验增加")

	# 槽经验增加
	slot_exp_incr = models.SmallIntegerField(null=True, verbose_name="槽经验增加")

	# 金币增加
	gold_incr = models.SmallIntegerField(null=True, verbose_name="金币增加")

	# 是否新题
	is_new = models.BooleanField(default=False, verbose_name="新题标志")

	# 所属题目集（用于子类继承）
	# question_set = None

	def __str__(self):
		return "%s %s" % (self.questionSet(), self.question)

	# 用于admin显示
	def adminSelection(self):

		result = ''
		for s in self.selection:
			result += chr(ord('A')+s)

		return result

	adminSelection.short_description = "选择情况"

	# 用于admin显示
	def adminAnswer(self):
		return self.question.adminCorrectAnswer()

	adminAnswer.short_description = "正确答案"

	def convertToDict(self, type: str = None) -> dict:
		"""
		转化为字典
		Args:
			type (str): 转化类型
		Returns:
			转化后的字典数据
		"""
		base = {
			'id': self.id,
			'question_id': self.question.id,
			'selection': self.selection,
			'timespan': self.timespan,
			'answered': self.answered,
			'correct': self.correct(),
			'is_new': self.is_new,
		}

		if type == 'result':

			base['exp_incr'] = self.exp_incr
			base['slot_exp_incr'] = self.slot_exp_incr
			base['gold_incr'] = self.gold_incr
			base['score'] = self.score()
			base['correct_selection'] = self.question.correctAnswer()

		return base

	@classmethod
	def create(cls, question_set: 'QuestionSetRecord',
			   question_id: int, **kwargs) -> 'PlayerQuestion':
		"""
		创建对象
		Args:
			question_set (QuestionSetRecord): 题目集记录
			question_id (int): 题目ID
			**kwargs (**dict): 子类重载参数
		Returns:
			创建的玩家题目关系对象
		"""
		ques = cls()
		ques.question_id = question_id
		ques.setQuestionSet(question_set)
		ques._updateIsNew(question_set)
		ques._create(**kwargs)

		return ques

	@classmethod
	def rewardCalculator(cls) -> QuestionSetSingleRewardCalc:
		"""
		获取对应的奖励计算类
		Returns:
			对应奖励计算类本身（继承自 QuestionSetSingleRewardCalc）
		"""
		return QuestionSetSingleRewardCalc

	@classmethod
	def source(cls) -> RecordSource:
		"""
		题目来源
		Returns:
			题目来源枚举成员
		"""
		return RecordSource.Others

	def _create(self, **kwargs):
		"""
		内部创建函数（用于子类重载）
		Args:
			**kwargs (**dict): 子类重载参数
		"""
		pass

	def _updateIsNew(self, question_set: 'QuestionSetRecord'):
		"""
		检测并设置题目是否新题目
		Args:
			question_set (QuestionSetRecord): 题目集记录
		"""
		player = question_set.player
		rec = player.questionRecord(self.question_id)
		self.is_new = rec is None

	# 设置题目集
	def setQuestionSet(self, question_set):
		raise NotImplementedError
		# self.question_set = question_set

	# 获取题目集
	def questionSet(self):
		raise NotImplementedError
		# return self.question_set

	def correct(self) -> bool:
		"""
		是否正确（带缓存）
		Returns:
			返回作答是否正确
		"""
		return self._getOrSetCache(self.CORRECT_CACHE_KEY,
								   lambda: self.question.calcCorrect(self.selection))

	def score(self) -> int:
		"""
		计算得分（带缓存）
		Returns:
			返回作答得分
		"""
		return self._getOrSetCache(self.SCORE_CACHE_KEY,
								   lambda: self.question.calcScore(self.selection))

	def start(self):
		"""
		开始答题
		"""
		self._cache(self.STARTTIME_CACHE_KEY, datetime.datetime.now())

	def answer(self, selection: list, timespan: int, record: 'QuestionSetRecord'):
		"""
		作答题目
		Args:
			selection (list): 选择情况
			timespan (int): 用时
			record (QuestionSetRecord): 题目集记录
		"""
		start_time = self._getCache(self.STARTTIME_CACHE_KEY)

		if start_time is None:
			raise GameException(ErrorType.QuestionNotStarted)

		now = datetime.datetime.now()
		if now <= start_time:
			raise GameException(ErrorType.InvalidTimeSpan)

		backend_timespan = (now - start_time).microseconds

		self.timespan = self._realTimespan(timespan, backend_timespan)
		self.selection = selection
		self.answered = True

		self._calcRewards(record)

		# self.save()

	def _realTimespan(self, frontend: int, backend: int) -> int:
		"""
		实际用时
		Args:
			frontend (int): 前端用时
			backend (int): 后端用时
		Returns:
			返回实际做题用时
		"""
		delta = backend - frontend
		if delta > self.MAX_DELTATIME: return backend
		return min(frontend, backend)

	def _calcRewards(self, record: 'QuestionSetRecord'):
		"""
		计算奖励
		Args:
			record (QuestionSetRecord): 题目集记录
		"""
		calc = self.rewardCalculator().calc(self, record)

		self.exp_incr = calc.exer_exp_incr
		self.slot_exp_incr = calc.exerslot_exp_incr
		self.gold_incr = calc.gold_incr


# ===================================================
#  刷题题目关系表
# ===================================================
class ExerciseQuestion(PlayerQuestion):
	class Meta:
		verbose_name = verbose_name_plural = "刷题题目关系"

	# 刷题记录
	exercise = models.ForeignKey('ExerciseRecord',
								 on_delete=models.CASCADE, verbose_name="刷题记录")

	@classmethod
	def rewardCalculator(cls) -> QuestionSetSingleRewardCalc:
		"""
		获取对应的奖励计算类
		Returns:
			对应奖励计算类本身（继承自 QuestionSetSingleRewardCalc）
		"""
		from utils.calc_utils import ExerciseSingleRewardCalc
		return ExerciseSingleRewardCalc

	@classmethod
	def source(cls):
		return RecordSource.Exercise

	# 设置题目集
	def setQuestionSet(self, question_set):
		self.exercise = question_set

	# 获取题目集
	def questionSet(self):
		return self.exercise


# ===================================================
#  题目集类型枚举
# ===================================================
class QuestionSetType(Enum):
	Exercise = 1  # 刷题
	Exam = 2  # 考核
	Battle = 3  # 对战

	Unset = 0  # 未设置


# ===================================================
#  题目集奖励表
# ===================================================
class QuestionSetReward(models.Model):

	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "题目集奖励"

	# 奖励物品类型
	type = models.PositiveSmallIntegerField(choices=BaseItem.TYPES, verbose_name="物品类型")

	# 奖励物品ID
	item_id = models.PositiveSmallIntegerField(verbose_name="物品ID")

	# 奖励物品数量
	count = models.PositiveSmallIntegerField(verbose_name="数量")

	# 题目集记录
	record = None

	def __str__(self):
		return "%s *%d" % (self.item(), self.count)

	@classmethod
	def create(cls, record: 'QuestionSetRecord', item: BaseItem, count: int) -> 'QuestionSetReward':
		"""
		创建实例
		Args:
			record (QuestionSetRecord): 题目集记录
			item (BaseItem): 物品
			count (int): 数目
		Returns:
			创建的题目集奖励对象
		"""
		reward = cls()
		reward.record = record
		reward.type = item.TYPE
		reward.item_id = item.id
		reward.count = count

		return reward

	# 转化为字典
	def convertToDict(self) -> dict:
		"""
		转化为字典
		Returns:
			转化后的字典数据
		"""
		return {
			'type': self.type,
			'item_id': self.item_id,
			'count': self.count
		}

	# 获取物品
	def item(self) -> BaseItem:
		"""
		获取奖励的物品
		Returns:
			物品对象
		"""
		from item_module.views import Common as ItemCommon
		return ItemCommon.getItem(self.type, id=self.item_id)

	# 获取容器类型
	def containerType(self) -> BaseContainer:
		"""
		获取奖励物品所属的容器类型
		Returns:
			容器类型
		"""
		return self.item().contItemClass().containerClass()


# ===================================================
#  刷题奖励表
# ===================================================
class ExerciseReward(models.Model):

	class Meta:
		verbose_name = verbose_name_plural = "刷题奖励"

	# 刷题记录
	record = models.ForeignKey("ExerciseRecord", on_delete=models.CASCADE, verbose_name="刷题记录")


# ===================================================
#  题目集记录表
# ===================================================
class QuestionSetRecord(CacheableModel):

	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "题目集记录"

	TYPE = QuestionSetType.Unset

	# 题目缓存键
	QUES_CACHE_KEY = 'player_ques'

	# 奖励缓存键
	REWARD_CACHE_KEY = 'rewards'

	# 玩家
	player = models.ForeignKey("player_module.Player", on_delete=models.CASCADE,
							   verbose_name="玩家")

	# 开始时间
	create_time = models.DateTimeField(auto_now_add=True, verbose_name="开始时间")

	# 经验增加（总）
	exp_incr = models.SmallIntegerField(null=True, verbose_name="经验增加")

	# 金币增加（总）
	gold_incr = models.SmallIntegerField(null=True, verbose_name="金币增加")

	# 槽经验增加（总）
	slot_exp_incr = models.SmallIntegerField(null=True, verbose_name="槽经验增加")

	# 是否完成
	finished = models.BooleanField(default=False, verbose_name="完成标志")

	def __init__(self, *args, **kwargs):
		super().__init__(*args, **kwargs)
		self._cache(self.REWARD_CACHE_KEY, [])

	def __str__(self):
		return "%s %s" % (self.player, self.generateName())

	# 创建一个题目集
	@classmethod
	def create(cls, player: 'Player', **kwargs) -> 'QuestionSetRecord':
		"""
		创建一个题目集记录
		Args:
			player (Player): 玩家
			**kwargs (**dict): 附加参数
		Returns:
			新创建的题目集记录
		"""
		record = cls()
		record.player = player
		record._create(**kwargs)
		record.start(**kwargs)

		return record

	# 奖励计算类
	@classmethod
	def rewardCalculator(cls) -> 'QuestionSetResultRewardCalc':
		"""
		获取奖励计算的类
		Returns:
			奖励计算类或其子类本身
		"""
		return QuestionSetResultRewardCalc

	@classmethod
	def playerQuesClass(cls) -> 'PlayerQuestion':
		"""
		该类对应的玩家题目关系类，用于 addQuestion 中创建一个题目关系
		Returns:
			返回一个 PlayerQuestion 子类本身
		"""
		raise NotImplementedError

	@classmethod
	def rewardClass(cls) -> 'QuestionSetReward':
		"""
		该类对应的奖励记录类
		Returns:
			返回某个 QuestionSetReward 子类本身
		"""
		raise NotImplementedError

	def _create(self, **kwargs):
		"""
		创建实例后用于配置具体属性
		Args:
			**kwargs (**dict): 子类中定义参数
		"""
		pass

	def generateName(self) -> str:
		"""
		生成题目集记录的名字
		Returns:
			生成的名字
		"""
		return '题目集记录'

	def convertToDict(self, type: str = None) -> dict:
		"""
		转化为字典
		Args:
			type (str): 转化类型
		Returns:
			转化后的字典数据
		"""
		create_time = ModelUtils.timeToStr(self.create_time)
		player_questions = ModelUtils.objectsToDict(self.playerQuestions(), type=type)

		base = {
			'id': self.id,
			'name': self.generateName(),
			'questions': player_questions,
			'create_time': create_time,
			'finished': self.finished,
		}

		if type == 'result':
			rewards = ModelUtils.objectsToDict(self.rewards())

			base['exp_incr'] = self.exp_incr
			base['slot_exp_incr'] = self.slot_exp_incr
			base['gold_incr'] = self.gold_incr
			base['rewards'] = rewards

		return base

	def playerQuestions(self) -> QuerySet:
		"""
		获取所有题目关系（缓存）
		执行该函数后，对应缓存项将改变为 QuerySet 类型，故需要在题目集结束后调用
		Returns:
			题目关系 QuerySet 对象
		"""
		cache = self._getCache(self.QUES_CACHE_KEY)

		if isinstance(cache, list):
			self._deleteCache(self.QUES_CACHE_KEY)
		
		return self._getOrSetCache(self.QUES_CACHE_KEY,
								   lambda: self._playerQuestions())

	def _playerQuestions(self) -> QuerySet:
		"""
		获取所有题目关系（数据库）
		Returns:
			题目关系 QuerySet 对象
		"""
		raise NotImplementedError

	def playerQuestion(self, question_id: int) -> PlayerQuestion:
		"""
		获取单个题目关系（缓存/数据库）
		Args:
			question_id (int): 题目ID
		Returns:
			对应ID的题目关系对象
		"""
		res = self.playerQuestions().filter(question_id=question_id)
		if res.exists(): return res.first()
		return None

	def cachedPlayerQuestion(self, question_id: int) -> PlayerQuestion:
		"""
		获取单个题目关系（仅查询缓存）
		Args:
			question_id (int): 题目ID
		Returns:
			对应ID的题目关系对象
		"""
		cache = self._getQuestionsCache()
		for player_ques in cache:
			if player_ques.question_id == question_id:
				return player_ques

		return None

	def _initQuestionCache(self):
		"""
		初始化题目缓存
		"""
		self._cache(self.QUES_CACHE_KEY, [])

	def _getQuestionsCache(self) -> list:
		"""
		获取题目缓存
		Returns:
			题目关系数组
		"""
		cache = self._getCache(self.QUES_CACHE_KEY)
		if cache is None: return []
		return cache

	def _addQuestionToCache(self, player_ques: PlayerQuestion):
		"""
		往缓存中添加题目
		Args:
			player_ques (PlayerQuestion): 题目关系
		"""
		self._getQuestionsCache().append(player_ques)

	def _removeQuestionFromCache(self, player_ques: PlayerQuestion):
		"""
		往缓存中移除题目
		Args:
			player_ques (PlayerQuestion): 题目关系
		"""
		cache = self._getQuestionsCache()
		if player_ques in cache: cache.remove(player_ques)

	def exactlyPlayer(self): # -> Player:
		"""
		获取实际玩家在线信息
		Returns:
			实际玩家在线信息（OnlinePlayer）
		"""
		from player_module.views import Common

		player = self.player
		# 如果获取不到实际的玩家，该对象释放时需要自动保存
		player.delete_save = True

		online_player = Common.getOnlinePlayer(player.id)

		return online_player.player or player

	def start(self, **kwargs):
		"""
		开始一个题目集，在调用 create() 的时候会调用，进行缓存的初始化、当前题目集设置以及题目生成
		Args:
			**kwargs (**dict): 子类重载参数
		"""
		self._initQuestionCache()

		# 设置当前的题目集
		self.exactlyPlayer().setCurrentQuestionSet(self)

		self._generateQuestions(**kwargs)

	def _generateQuestions(self, **kwargs):
		"""
		生成题目
		Args:
			**kwargs (**dict): 子类重载参数
		"""
		from utils.calc_utils import QuestionGenerator

		configure = self.__makeGenerateConfigure(**kwargs)

		if configure is None: return

		gen = QuestionGenerator.generate(configure, True)

		for qid in gen.result: self.addQuestion(qid)

	def __makeGenerateConfigure(self, **kwargs) -> QuestionGenerateConfigure:
		"""
		获取题目生成配置信息，用于 _generateQuestions() 中进行题目生成
		Args:
			**kwargs (**dict): 子类重载参数
		Returns:
			配置类对象
		"""
		raise NotImplementedError

	def terminate(self, **kwargs):
		"""
		结束刷题
		Args:
			**kwargs (**dict): 子类重载参数
		"""

		self.finished = True

		self._applyResult(self._calcResult())

		# 会自动保存
		self.exactlyPlayer().clearCurrentQuestionSet()

	def shrinkQuestions(self):
		"""
		压缩题目，用于排除未作答的题目，通过移除缓存题目的方式实现
		"""

		player_queses = self._getQuestionsCache()

		for player_ques in player_queses:
			if not player_ques.answer:
				self._removeQuestionFromCache(player_ques)

	def _calcResult(self) -> QuestionSetResultRewardCalc:
		"""
		计算题目集结果
		Returns:
			结果对象
		"""
		calc = self.rewardCalculator()

		if calc is None: return None

		calc = calc.calc(self.player_questions)

		self.exp_incr = calc.exer_exp_incr
		self.slot_exp_incr = calc.exerslot_exp_incr
		self.gold_incr = calc.gold_incr

		return calc

	def _applyResult(self, calc: QuestionSetResultRewardCalc):
		"""
		应用题目集结果
		Args:
			calc (QuestionSetResultRewardCalc): 结果
		"""
		if calc is None: return

		player = self.exactlyPlayer()

		player.gainMoney(self.gold_incr)
		player.gainExp(self.slot_exp_incr,
							calc.exer_exp_incrs,
							calc.exerslot_exp_incrs)

		rewards = self.rewards()

		for reward in rewards:
			cla = reward.containerType()
			container = player.getContainer(cla)
			container.gainItems(reward.item(), reward.count)

	def addQuestion(self, question_id: int, **kwargs) -> PlayerQuestion:
		"""
		增加题目（临时加入到内存）
		Args:
			question_id (int): 题目ID
			**kwargs (**dict): 子类重载参数
		Returns:
			新增的玩家题目关系对象
		"""

		cla = self.playerQuesClass()

		player_ques = cla.create(self, question_id, **kwargs)

		if self.player_questions is None:
			self.player_questions = []

		self._addQuestionToCache(player_ques)

		return player_ques

	def startQuestion(self, question_id: int = None, player_ques: PlayerQuestion = None):
		"""
		开始作答题目
		Args:
			question_id (int): 题目ID
			player_ques (PlayerQuestion): 玩家题目关系对象
		"""

		# 从缓存中读取
		if player_ques is None:
			player_ques = self.cachedPlayerQuestion(question_id)

		player_ques.start()

	def answerQuestion(self, selection: list, timespan: int,
					   question_id: int = None, player_ques: PlayerQuestion = None):
		"""
		回答题目
		Args:
			question_id (int): 题目ID
			player_ques (PlayerQuestion): 玩家题目关系对象
			selection (list): 选择情况
			timespan (int): 用时
		"""

		# 从缓存中读取
		if player_ques is None:
			player_ques = self.cachedPlayerQuestion(question_id)

		rec = QuestionRecord.create(self.player, question_id)

		player_ques.answer(selection, timespan, rec)

		rec.updateRecord(player_ques)

	# region 统计数据

	def _totalData(self, key: str, func: callable, player_queses: QuerySet = None) -> list:
		"""
		获取统计数据（获取后自动根据键名存入缓存）
		Args:
			key (str): 键名（统计类型）
			func (callable): 计算函数
			player_queses (QuerySet): 玩家题目关系集合
		Returns:
			统计的数据结果
		"""
		if not self.finished: return []

		return self._getOrSetCache('total_' + key,
								   lambda: self.__generTotalData(func, player_queses))

	def __generTotalData(self, func: callable, player_queses: QuerySet = None) -> list:
		"""
		生成统计数据
		Args:
			func (callable): 计算函数
			player_queses (QuerySet): 玩家题目关系集合，默认情况下为所有题目关系
		Returns:
			统计的数据结果
		"""
		res = []

		if player_queses is None:
			player_queses = self.playerQuestions()

		for data in player_queses:
			res.append(func(data))

		return res

	def _sumData(self, key: str, func: callable, player_queses: QuerySet = None) -> int:
		"""
		获取求和数据（获取后自动根据键名存入缓存）
		Args:
			key (str): 键名（统计类型）
			func (callable): 计算函数
			player_queses (QuerySet): 玩家题目关系集合
		Returns:
			统计的数据结果
		"""
		if not self.finished: return -1

		return self._getOrSetCache('sum_' + key,
								   lambda: self.__generSumData(func, player_queses))

	def __generSumData(self, func: callable, player_queses: QuerySet = None) -> int:
		"""
		生成求和数据
		Args:
			func (callable): 计算函数
			player_queses (QuerySet): 玩家题目关系集合，默认情况下为所有题目关系
		Returns:
			统计的数据结果
		"""
		res = 0

		if player_queses is None:
			player_queses = self.playerQuestions()

		for data in player_queses:
			res += int(func(data))

		# data = func(player_queses)
		# for d in data: res += int(d)

		return res

	def selections(self, player_queses: QuerySet = None) -> list:
		"""
		获取每道题目的选择情况
		Args:
			player_queses (QuerySet): 玩家题目关系集合，默认情况下为所有题目关系
		Returns:
			题目选择情况列表
		"""
		return self._totalData('selections',
							   lambda d: d.selection, player_queses)

	def timespans(self, player_queses: QuerySet = None) -> list:
		"""
		获取每道题目的用时
		Args:
			player_queses (QuerySet): 玩家题目关系集合，默认情况下为所有题目关系
		Returns:
			题目用时列表
		"""
		return self._totalData('timespans', lambda d: d.timespan, player_queses)

	def scores(self, player_queses: QuerySet = None) -> list:
		"""
		获取每道题目的得分
		Args:
			player_queses (QuerySet): 玩家题目关系集合，默认情况下为所有题目关系
		Returns:
			题目得分列表
		"""
		return self._totalData('scores', lambda d: d.score(), player_queses)

	def results(self, player_queses: QuerySet = None) -> list:
		"""
		获取每道题目的正误
		Args:
			player_queses (QuerySet): 玩家题目关系集合，默认情况下为所有题目关系
		Returns:
			题目用时列表
		"""
		return self._totalData('results', lambda d: d.correct(), player_queses)

	def sumTimespan(self, player_queses: QuerySet = None) -> int:
		"""
		获取题目集总用时
		Args:
			player_queses (QuerySet): 玩家题目关系集合，默认情况下为所有题目关系
		Returns:
			题目集总用时
		"""
		return self._sumData('timespans', lambda d: d.timespan, player_queses)

	def sumScore(self, player_queses: QuerySet = None) -> int:
		"""
		获取题目集总得分
		Args:
			player_queses (QuerySet): 玩家题目关系集合，默认情况下为所有题目关系
		Returns:
			题目集总得分
		"""
		return self._sumData('scores', lambda d: d.score(), player_queses)

	def corrCount(self, player_queses: QuerySet = None) -> int:
		"""
		获取题目集中的正确数量
		Args:
			player_queses (QuerySet): 玩家题目关系集合，默认情况下为所有题目关系
		Returns:
			题目集正确数量
		"""
		return self._sumData('correct', lambda d: d.correct(), player_queses)

	def corrRate(self, player_queses: QuerySet = None) -> float:
		"""
		获取题目集正确率
		Args:
			player_queses (QuerySet): 玩家题目关系集合，默认情况下为所有题目关系
		Returns:
			题目集正确率
		"""
		if not self.finished: return -1

		corr_cnt = 0
		results = self.results(player_queses)
		for result in results: corr_cnt += int(result)

		return corr_cnt / len(results)

	# endregion

	# 添加奖励
	def addReward(self, item: BaseItem, count: int):
		"""
		添加结算奖励
		Args:
			item (BaseItem): 物品
			count (int): 数目
		"""
		cla = self.rewardClass()
		if cla is None: return

		reward = cla.create(self, item, count)

		self._addCachedReward(reward)

	# 添加一个缓存奖励
	def _addCachedReward(self, reward: QuestionSetReward):
		"""
		添加奖励到缓存中
		Args:
			reward (QuestionSetReward): 奖励对象
		"""
		self._getCache(self.REWARD_CACHE_KEY).append(reward)

	# 获取奖励
	def rewards(self) -> list:
		"""
		获取所有奖励（缓存）
		Returns:
			题目集奖励列表
		"""
		return self._getOrSetCache(self.REWARD_CACHE_KEY,
								   lambda: list(self._rewards()))

	# 实际获取奖励
	def _rewards(self) -> QuerySet:
		"""
		获取所有奖励（数据库）
		Returns:
			题目集奖励 QuerySet 对象
		"""
		raise NotImplementedError


# ===================================================
#  刷题记录表
# ===================================================
class ExerciseRecord(QuestionSetRecord):

	class Meta:
		verbose_name = verbose_name_plural = "刷题记录"

	TYPE = QuestionSetType.Exercise

	# 最大刷题数
	MAX_COUNT = 10

	GEN_TYPES = [
		(QuestionGenerateType.Normal.value, '普通模式'),
		(QuestionGenerateType.OccurFirst.value, '已做优先'),
		(QuestionGenerateType.NotOccurFirst.value, '未做优先'),
		(QuestionGenerateType.WrongFirst.value, '错题优先'),
		(QuestionGenerateType.CollectedFirst.value, '收藏优先'),
		(QuestionGenerateType.SimpleFirst.value, '简单题优先'),
		(QuestionGenerateType.MiddleFirst.value, '中等题优先'),
		(QuestionGenerateType.DifficultFirst.value, '难题优先'),
	]

	# 常量定义
	NAME_STRING_FMT = "%s\n%s 刷题记录"

	# 科目
	subject = models.ForeignKey('game_module.Subject', on_delete=models.CASCADE,
								verbose_name="科目")

	# 题量（用于生成题目）
	count = models.PositiveSmallIntegerField(default=1, verbose_name="题量")

	# 题目分配模式
	gen_type = models.PositiveSmallIntegerField(choices=GEN_TYPES, default=0,
												verbose_name="生成模式")

	# 所属赛季
	season = models.ForeignKey('season_module.CompSeason', on_delete=models.CASCADE,
							   verbose_name="所属赛季")

	# 奖励计算类
	@classmethod
	def rewardCalculator(cls):
		from utils.calc_utils import ExerciseResultRewardCalc
		return ExerciseResultRewardCalc

	# 玩家题目关系类
	@classmethod
	def playerQuesClass(cls): return ExerciseQuestion

	# 奖励记录类
	@classmethod
	def rewardClass(cls): return ExerciseReward

	# 生成名字
	def generateName(self) -> str:

		return self.NAME_STRING_FMT % (self.create_time.strftime("%Y-%m-%d %H:%M:%S"),
									   self.subject.name)

	# 获取所有题目关系
	def _playerQuestions(self) -> 'QuerySet':
		"""
		获取所有题目关系（所有题目）
		Returns:
			返回通过 _set.all() 获得的 QuerySet 对象
		"""
		return self.exercisequestion_set.all()

	def convertToDict(self, type=None):

		res = super().convertToDict(type)

		res['season_id'] = self.season_id
		res['subject_id'] = self.subject_id
		res['count'] = self.count
		res['gen_type'] = self.gen_type

		return res

	def _rewards(self): return self.exercisereward_set

	# 开始刷题
	def _create(self, subject, count, gen_type):
		self.subject = subject
		self.count = count
		self.gen_type = gen_type

	# 生成题目生成配置信息
	def __makeGenerateConfigure(self):
		from utils.calc_utils import QuestionGenerateConfigure

		return QuestionGenerateConfigure(self, self.player, self.subject,
										 gen_type=self.gen_type, count=self.count)

	# 终止答题
	def terminate(self, **kwargs):
		self.shrinkQuestions()
		super().terminate(**kwargs)