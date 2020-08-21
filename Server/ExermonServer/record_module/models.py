from django.db.models.query import QuerySet

from item_module.models import BaseItem, BaseContainer
from question_module.models import *

from .types import *

# from player_module.models import Player
from utils.calc_utils import *
from utils.model_utils import BaseModel, CacheableModel

import jsonfield

# Create your models here.


# ===================================================
#  基本玩家题目关系表
# ===================================================
class BasePlayerQuestion(CacheableModel):
	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "基本玩家题目关系"

	# 前后端时间差最大值（毫秒）
	MAX_DELTATIME = 10*1000

	# 缓存键配置
	STARTTIME_CACHE_KEY = 'start_time'
	CORRECT_CACHE_KEY = 'correct'
	SCORE_CACHE_KEY = 'score'

	# 类设定
	QUESTION_CLASS: BaseQuestion = None
	QUESTION_SET_CLASS: 'QuesSetRecord' = None

	# 奖励计算类
	REWARD_CALC = QuesSetSingleRewardCalc

	# 来源
	SOURCE = RecordSource.Others

	# DO_NOT_AUTO_CONVERT_FIELDS = ['question_set_id', 'exp_incr', 'slot_exp_incr', 'gold_incr']

	# 题目
	question: BaseQuestion = None
	question_id: int = None

	# 题目集
	question_set: 'QuesSetRecord' = None
	question_set_id: int = None

	# 是否作答
	answered = models.BooleanField(default=False, verbose_name="作答标志")

	# 用时（毫秒数，0为未作答）
	timespan = models.PositiveIntegerField(default=0, verbose_name="用时")

	# # 得分
	# score = models.PositiveSmallIntegerField(null=True, verbose_name="得分")

	# 经验增加
	exp_incr = models.SmallIntegerField(null=True, verbose_name="经验增加")
	exp_incr.type_filter = ['result']

	# 槽经验增加
	slot_exp_incr = models.SmallIntegerField(null=True, verbose_name="槽经验增加")
	slot_exp_incr.type_filter = ['result']

	# 金币增加
	gold_incr = models.SmallIntegerField(null=True, verbose_name="金币增加")
	gold_incr.type_filter = ['result']

	# 是否新题
	is_new = models.BooleanField(default=False, verbose_name="新题标志")

	# region 读取转换配置

	# TYPE_FIELD_EXCLUDE_MAP = {
	# 	'any': ['question_set']
	# }

	# endregion

	# 所属题目集（用于子类继承）
	# question_set = None

	def __init__(self, *args, **kwargs):
		super().__init__(*args, **kwargs)

		self._setupCachePool()

	def __str__(self):
		return "%s %s" % (self.question_set, self.question)

	# region 配置项

	@classmethod
	def questionClass(cls): return cls.QUESTION_CLASS

	@classmethod
	def questionType(cls): return cls.questionClass().TYPE

	@classmethod
	def quesRecordClass(cls):
		return cls.questionClass().recordClass()

	@classmethod
	def rewardCalculator(cls) -> QuesSetSingleRewardCalc:
		"""
		获取对应的奖励计算类
		Returns:
			对应奖励计算类本身（继承自 QuesSetSingleRewardCalc）
		"""
		return cls.REWARD_CALC

	@classmethod
	def source(cls) -> RecordSource:
		"""
		题目来源
		Returns:
			题目来源枚举成员
		"""
		return cls.SOURCE

	# endregion

	@classmethod
	def create(cls, question_set: 'QuesSetRecord',
			   question_id: int, **kwargs) -> 'SelectingPlayerQuestion':
		"""
		创建对象
		Args:
			question_set (QuesSetRecord): 题目集记录
			question_id (int): 题目ID
			**kwargs (**dict): 子类重载参数
		Returns:
			创建的玩家题目关系对象
		"""
		ques = cls()
		ques.question_id = question_id
		ques.question_set = question_set
		ques._updateIsNew(question_set)
		ques._create(**kwargs)

		ques.save()

		return ques

	def _create(self, **kwargs):
		"""
		内部创建函数（用于子类重载）
		Args:
			**kwargs (**dict): 子类重载参数
		"""
		pass

	# region 转化

	def _convertCustomAttrs(self, res, type=None, **kwargs):
		super()._convertCustomAttrs(res, type, **kwargs)

		question_type = self.questionType().value

		res['question_type'] = question_type

	def _convertResultData(self, res, **kwargs):
		res['correct'] = self.correct()
		res['score'] = self.score()

	# def convert(self, type: str = None) -> dict:
	# 	"""
	# 	转化为字典
	# 	Args:
	# 		type (str): 转化类型
	# 	Returns:
	# 		转化后的字典数据
	# 	"""
	# 	res = {}
	#
	# 	self._convertBaseInfo(res, type)
	#
	# 	if type == 'result':
	# 		self._convertResultInfo(res, type)
	#
	# 	return res
	#
	# def _convertBaseInfo(self, res, type):
	# 	"""
	# 	转化基本信息
	# 	Args:
	# 		res (dict): 转化结果
	# 		type (str): 转化类型
	# 	"""
	# 	question_type = self.questionType().value
	#
	# 	res['id'] = self.id
	# 	res['question_type'] = question_type
	# 	res['question_id'] = self.question_id
	#
	# 	res['timespan'] = self.timespan
	# 	res['answered'] = self.answered
	# 	res['is_new'] = self.is_new
	#
	# def _convertResultInfo(self, res, type):
	#
	# 	res['exp_incr'] = self.exp_incr
	# 	res['slot_exp_incr'] = self.slot_exp_incr
	# 	res['gold_incr'] = self.gold_incr
	# 	res['correct'] = self.correct()
	# 	res['score'] = self.score()

	# endregion

	def _updateIsNew(self, question_set: 'QuesSetRecord'):
		"""
		检测并设置题目是否新题目
		Args:
			question_set (QuesSetRecord): 题目集记录
		"""
		player = question_set.player

		rec = player.quesRecord(
			self.quesRecordClass(), self.question_id)

		self.is_new = rec is None

	def _answerDict(self) -> dict:
		"""
		作答字典
		Returns:
			返回作答结果（字典形式）
		"""
		return {}

	# region 统计

	def correct(self) -> bool:
		"""
		是否正确（带缓存）
		Returns:
			返回作答是否正确
		"""
		if not self.answered: return False
		return self._getOrSetCache(self.CORRECT_CACHE_KEY,
			lambda: self.question.calcCorrect(**self._answerDict()))

	def score(self) -> int:
		"""
		计算得分（带缓存）
		Returns:
			返回作答得分
		"""
		if not self.answered: return 0
		return self._getOrSetCache(self.SCORE_CACHE_KEY,
			lambda: self.question.calcScore(**self._answerDict()))

	# endregion

	# region 操作

	def start(self):
		"""
		开始答题
		"""
		self._setCache(self.STARTTIME_CACHE_KEY, datetime.datetime.now())

	def answer(self, answer: dict, timespan: int, record: 'QuesSetRecord'):
		"""
		作答题目
		Args:
			answer (dict): 作答内容
			timespan (int): 用时
			record (QuesSetRecord): 题目集记录
		"""
		start_time = self._getCache(self.STARTTIME_CACHE_KEY)

		if start_time is None:
			raise GameException(ErrorType.QuestionNotStarted)

		now = datetime.datetime.now()
		if now <= start_time:
			raise GameException(ErrorType.InvalidTimeSpan)

		backend_timespan = (now - start_time).total_seconds()
		backend_timespan = int(backend_timespan*1000)

		if timespan >= 0:
			self.timespan = self._realTimespan(timespan, backend_timespan)
		else:
			self.timespan = self.question.stdTime()*1000

		self.answered = True

		self._processAnswer(answer)
		self._calcRewards(record)

		# self.save()

	def _processAnswer(self, answer):
		"""
		处理题目回答
		Args:
			answer (dict): 作答内容
		"""
		raise NotImplementedError

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

	def _calcRewards(self, record: 'QuesSetRecord'):
		"""
		计算奖励
		Args:
			record (QuesSetRecord): 题目集记录
		"""
		calc = self.rewardCalculator()
		if calc is None: return

		calc = calc.calc(self, record)

		self.exp_incr = calc.exer_exp_incr
		self.slot_exp_incr = calc.slot_exp_incr
		self.gold_incr = calc.gold_incr

	# endregion

	def save(self, judge=True, **kwargs):
		if judge and self.question_set is None:
			self.delete_save = False
			if self.id is not None: self.delete()
		else: super().save(**kwargs)


# ===================================================
#  选择题目关系表
# ===================================================
class SelectingPlayerQuestion(BasePlayerQuestion):
	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "选择题目关系"

	# 选择情况
	selection = jsonfield.JSONField(default=[], verbose_name="选择情况")

	# region Adminx 配置

	# 用于admin显示
	def adminSelection(self):

		result = ''
		for s in self.selection:
			result += chr(ord('A')+s)

		return result

	adminSelection.short_description = "选择情况"

	# 用于admin显示
	def adminAnswer(self):
		question: SelectingQuestion = self.question
		return question.adminCorrectAnswer()

	adminAnswer.short_description = "正确答案"

	LIST_DISPLAY_APPEND = ['adminSelection', 'adminAnswer']

	# endregion

	def _answerDict(self):
		return {'selection': self.selection}

	def _processAnswer(self, answer):
		self.selection = answer


# ===================================================
#  选择组合题目关系表
# ===================================================
class GroupPlayerQuestion(BasePlayerQuestion):

	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "选择组合题目关系"

	# 回答情况
	answers = jsonfield.JSONField(default=[], verbose_name="回答情况")

	def _answerDict(self) -> dict:
		return {'answers': self.answers}

	def _processAnswer(self, answer):
		self.answers = answer


# ===================================================
#  元素题目关系表
# ===================================================
class ElementPlayerQuestion(BasePlayerQuestion):

	# 回答
	answer = models.CharField(null=True, blank=True,
							  max_length=64, verbose_name="回答")
	answer.type_filter = ['result']

	# 选项（字符串数组）
	choices = jsonfield.JSONField(default=[], verbose_name="选项")

	def _convertCustomAttrs(self, res, type=None, **kwargs):
		super()._convertCustomAttrs(res, type, **kwargs)

		res['title'] = self.title()

	def _convertResultData(self, res, **kwargs):
		super()._convertResultData(res, **kwargs)

		res['correct_answer'] = self.correctAnswer()

	def _create(self):
		super()._create()

		self._generateChoices()

	def _generateChoices(self):
		raise NotImplementedError

	def title(self):
		question: ElementQuestion = self.question

		return question.title()

	def correctAnswer(self):
		question: ElementQuestion = self.question

		return question.answer()

	def _answerDict(self) -> dict:
		return {'answer': self.answer}

	def _processAnswer(self, answer):
		self.answer = answer


# ===================================================
#  题目集奖励表
# ===================================================
class QuesSetReward(BaseModel):

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
	def create(cls, record: 'QuesSetRecord', item: BaseItem, count: int) -> 'QuesSetReward':
		"""
		创建实例
		Args:
			record (QuesSetRecord): 题目集记录
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
		return self.item().containerClass()


# ===================================================
#  题目集记录表
# ===================================================
class QuesSetRecord(CacheableModel):

	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "题目集记录"

	TYPE = QuesSetType.Unset

	# 可接受的题目玩家关系类
	ACCEPT_PLAYER_QUES_CLASSES: list = []

	# 题目生成类
	QUES_GEN_CLASS: BaseQuestionGenerator = None

	# 奖励
	REWARD_CALC: QuesSetResultRewardCalc = None
	REWARD_CLASS: QuesSetReward = None

	# 名称格式
	NAME_STRING_FMT = "%s\n%s"

	# DO_NOT_AUTO_CONVERT_FIELDS = ['exer_exp_incrs',
	# 							  'slot_exp_incrs',
	# 							  'gold_incr']

	# 玩家
	player = models.ForeignKey("player_module.Player", on_delete=models.CASCADE,
							   verbose_name="玩家")

	# 开始时间
	create_time = models.DateTimeField(auto_now_add=True, verbose_name="开始时间")

	# 艾瑟萌经验增加（{subject_id: value}）
	exer_exp_incrs = jsonfield.JSONField(default={}, null=True, verbose_name="经验增加")
	exer_exp_incrs.type_filter = ['result']

	# 槽经验增加（{subject_id: value}）
	slot_exp_incrs = jsonfield.JSONField(default={}, null=True, verbose_name="槽经验增加")
	slot_exp_incrs.type_filter = ['result']

	# 金币增加（总）
	gold_incr = models.SmallIntegerField(null=True, verbose_name="金币增加")
	gold_incr.type_filter = ['result']

	# 是否完成
	finished = models.BooleanField(default=False, verbose_name="完成标志")

	# region 读取转化配置
	
	TYPE_RELATED_FILTER_MAP = {
		'any': [BasePlayerQuestion],
		'result': [QuesSetReward]
	}

	def _convertCustomAttrs(self, res, type=None, **kwargs):
		super()._convertCustomAttrs(res, type, **kwargs)

		res['name'] = self.generateName()

	def _convertResultData(self, res, **kwargs): pass

	# 	rewards = ModelUtils.objectsToDict(self.rewards())
	#
	# 	self._convertCustomFields(res, 'exer_exp_incrs',
	# 					 'slot_exp_incrs', 'gold_incr')
	#
	# 	res['rewards'] = rewards

	# def convert(self, type: str = None) -> dict:
	# 	"""
	# 	转化为字典
	# 	Args:
	# 		type (str): 转化类型
	# 	Returns:
	# 		转化后的字典数据
	# 	"""
	# 	res = {}
	#
	# 	self._convertBaseInfo(res, type)
	#
	# 	if type == 'result':
	# 		self._convertResultInfo(res)
	#
	# 	return res
	#
	# def _convertBaseInfo(self, res, type):
	#
	# 	create_time = ModelUtils.timeToStr(self.create_time)
	# 	player_questions = ModelUtils.objectsToDict(
	# 		self.playerQuestions(), type=type)
	#
	# 	res['id'] = self.id
	# 	res['name'] = self.generateName()
	# 	res['questions'] = player_questions
	# 	res['create_time'] = create_time
	# 	res['finished'] = self.finished
	#
	# def _convertResultInfo(self, res):
	# 	rewards = ModelUtils.objectsToDict(self.rewards())
	#
	# 	res['exer_exp_incrs'] = self.exer_exp_incrs
	# 	res['slot_exp_incrs'] = self.slot_exp_incrs
	# 	res['gold_incr'] = self.gold_incr
	# 	res['rewards'] = rewards

	# endregion

	# region Admin 配置

	# region 管理显示

	def adminExerExpIncrs(self):
		"""
		Admin 用显示艾瑟萌经验奖励
		"""
		from django.utils.html import format_html
		from game_module.models import Subject

		res = ""
		sbjs = Subject.objs()

		for sid in self.exer_exp_incrs:
			res += "%s + %d<br>" % \
				   (sbjs.get(id=sid).name,
					self.exer_exp_incrs[sid])

		return format_html(res)

	adminExerExpIncrs.short_description = "艾瑟萌经验奖励"

	def adminSlotExpIncrs(self):
		"""
		Admin 用显示艾瑟萌槽经验奖励
		"""
		from django.utils.html import format_html
		from game_module.models import Subject

		res = ""
		sbjs = Subject.objs()

		for sid in self.slot_exp_incrs:
			res += "%s + %d<br>" % \
				   (sbjs.get(id=sid).name,
					self.exer_exp_incrs[sid])

		return format_html(res)

	adminSlotExpIncrs.short_description = "艾瑟萌槽经验奖励"

	def adminExpIncrs(self):
		"""
		Admin 用显示人物经验奖励
		"""
		res = 0

		for sid in self.slot_exp_incrs:
			res += self.slot_exp_incrs[sid]

		return res

	adminExpIncrs.short_description = "人物经验奖励"

	# endregion

	LIST_DISPLAY_APPEND = ['adminExerExpIncrs',
						   'adminSlotExpIncrs', 'adminExpIncrs']

	# endregion

	def __str__(self):
		return "%s %s" % (self.player, self.generateName())

	def __init__(self, *args, **kwargs):
		super().__init__(*args, **kwargs)

		self._setupCachePool()
	
	# region 配置项

	# 奖励计算类
	@classmethod
	def rewardCalculator(cls) -> 'QuesSetResultRewardCalc':
		"""
		获取奖励计算的类
		Returns:
			奖励计算类或其子类本身
		"""
		return cls.REWARD_CALC

	@classmethod
	def playerQuesClasses(cls) -> list:
		"""
		该类对应的玩家题目关系类集合，用于 addQuestion 中创建一个题目关系
		Returns:
			返回一个 PlayerQuestion 子类列表
		"""
		return cls.ACCEPT_PLAYER_QUES_CLASSES

	@classmethod
	def playerQuesClass(cls) -> 'SelectingPlayerQuestion':
		"""
		该类对应的玩家题目关系类，用于 addQuestion 中创建一个题目关系
		Returns:
			返回一个 PlayerQuestion 子类本身
		"""
		clas = cls.playerQuesClasses()
		if len(clas) <= 0: return None

		return clas[0]

	@classmethod
	def rewardClass(cls) -> 'QuesSetReward':
		"""
		该类对应的奖励记录类
		Returns:
			返回某个 QuesSetReward 子类本身
		"""
		return cls.REWARD_CLASS

	@classmethod
	def _questionGenerator(cls):
		return cls.QUES_GEN_CLASS

	@classmethod
	def _cacheForeignKeyModels(cls):
		res = cls.playerQuesClasses().copy()
		res.append(cls.rewardClass())

		return res

	# endregion

	# region 创建和转化

	# 创建一个题目集
	@classmethod
	def create(cls, player: 'Player', **kwargs) -> 'QuesSetRecord':
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
		record.save()

		record.start(**kwargs)

		return record

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
		return self.NAME_STRING_FMT % self._nameParams()

	def _nameParams(self):
		create_time = ModelUtils.timeToStr(self.create_time)
		verbose_name = type(self)._meta.verbose_name

		return create_time, verbose_name

	# endregion

	def exactlyPlayer(self):  # -> Player:
		"""
		获取实际玩家在线信息
		Returns:
			实际玩家在线信息（OnlinePlayer）
		"""
		from player_module.views import Common

		player = self.player

		online_player = Common.getOnlinePlayer(player.id)

		return online_player.player or player

	# region 题目操作

	def playerQuestions(self, cla: BasePlayerQuestion = None) -> list:
		"""
		获取所有题目关系（缓存）
		执行该函数后，对应缓存项将改变为 QuerySet 类型，故需要在题目集结束后调用
		Args:
			cla (BasePlayerQuestion): 类型
		Returns:
			题目关系 list 对象
		"""
		if cla is not None:
			return self._queryModelCache(cla, listed=True)

		player_queses = []

		for cla in self.playerQuesClasses():
			player_queses += self._queryModelCache(cla, listed=True)

		return player_queses

	def playerQuestion(self, question_id: int,
					   cla: BasePlayerQuestion = None) -> BasePlayerQuestion:
		"""
		获取单个题目关系（缓存/数据库）
		Args:
			question_id (int): 题目ID
			cla (BasePlayerQuestion): 类型
		Returns:
			对应ID的题目关系对象
		"""
		if cla is None: cla = self.playerQuesClass()

		if cla is not None:
			return self._getModelCache(cla, question_id=question_id)

		return None

	def _initQuestionCache(self):
		"""
		初始化题目缓存
		"""
		for cla in self.playerQuesClasses():
			self._clearModelCache(cla)

	def _addQuestion(self, player_ques: BasePlayerQuestion):
		"""
		往缓存中添加题目
		Args:
			player_ques (BasePlayerQuestion): 题目关系
		"""
		self._appendModelCache(type(player_ques), player_ques)

	def _removeQuestion(self, player_ques: BasePlayerQuestion):
		"""
		往缓存中移除题目
		Args:
			player_ques (BasePlayerQuestion): 题目关系
		"""
		self._removeModelCache(type(player_ques), player_ques)

	# endregion

	# region 奖励操作

	def rewards(self) -> list:
		"""
		获取所有奖励（缓存）
		Returns:
			题目集奖励列表
		"""
		return self._queryModelCache(self.rewardClass())

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

		self._addReward(reward)

	def _addReward(self, reward: QuesSetReward):
		"""
		添加奖励到缓存中
		Args:
			reward (QuesSetReward): 奖励对象
		"""
		self._appendModelCache(self.rewardClass(), reward)

	# endregion

	# region 开始题目集

	def start(self, **kwargs):
		"""
		开始一个题目集，在调用 create() 的时候会调用，进行缓存的初始化、当前题目集设置以及题目生成
		Args:
			**kwargs (**dict): 子类重载参数
		"""
		self._initQuestionCache()

		# 设置当前的题目集
		self.exactlyPlayer().setCurrentQuesSet(self)

		self._generateQuestions(**kwargs)

	def _generateQuestions(self, **kwargs):
		"""
		生成题目
		Args:
			**kwargs (**dict): 子类重载参数
		"""
		configure = self._makeGenerateConfigure(**kwargs)

		if configure is None: return
		
		gen = self._questionGenerator().generate(configure, True)

		for q in gen.result: self.addQuestion(*q)

	def _makeGenerateConfigure(self, **kwargs) -> BaseQuestionGenerateConfigure:
		"""
		获取题目生成配置信息，用于 _generateQuestions() 中进行题目生成
		Args:
			**kwargs (**dict): 子类重载参数
		Returns:
			配置类对象
		"""
		return None

	def addQuestion(self, cla: BasePlayerQuestion, 
					question_id: int, **kwargs) -> BasePlayerQuestion:
		"""
		增加题目（临时加入到内存）
		Args:
			cla (BasePlayerQuestion): 题目关系类型
			question_id (int): 题目ID
			**kwargs (**dict): 子类重载参数
		Returns:
			新增的玩家题目关系对象
		"""
		player_ques = cla.create(self, question_id, **kwargs)

		self._addQuestion(player_ques)

		return player_ques

	# endregion

	# region 题目集过程

	def startQuestion(self, cla: BasePlayerQuestion = None, question_id: int = None, 
					  player_ques: BasePlayerQuestion = None):
		"""
		开始作答题目
		Args:
			cla (type): 题目类
			question_id (int): 题目ID
			player_ques (SelectingPlayerQuestion): 玩家题目关系对象
		"""

		# 从缓存中读取
		if player_ques is None:
			player_ques = self.playerQuestion(cla, question_id)

		player_ques.start()

	def answerQuestion(self, answer: dict, timespan: int,
					   cla: BasePlayerQuestion = None, question_id: int = None,
					   player_ques: SelectingPlayerQuestion = None):
		"""
		回答指定题目
		Args:
			answer (dict): 作答内容
			timespan (int): 用时
			cla (BasePlayerQuestion): 题目关系类型
			question_id (int): 题目ID
			player_ques (SelectingPlayerQuestion): 玩家题目关系对象
		"""

		# 从缓存中读取
		if player_ques is None:
			player_ques = self.playerQuestion(cla, question_id)

		if question_id is None:
			question_id = player_ques.question_id

		record_cla = cla.questionClass().recordClass()
		rec = record_cla.create(self.player, question_id)

		player_ques.answer(answer, timespan, rec)

		rec.updateRecord(player_ques)

	# endregion

	# region 结束题目集
	
	def terminate(self, **kwargs):
		"""
		结束刷题
		Args:
			**kwargs (**dict): 子类重载参数（用于奖励计算）
		"""

		self.finished = True

		self._applyResult(self._calcResult(**kwargs))

		# 会自动保存
		self.exactlyPlayer().clearCurrentQuesSet()

	def _calcResult(self, **kwargs) -> QuesSetResultRewardCalc:
		"""
		计算题目集结果
		Returns:
			结果对象
		"""
		calc = self.rewardCalculator()

		if calc is None: return None

		return calc.calc(self, self.playerQuestions(), **kwargs)

	def _applyResult(self, calc: QuesSetResultRewardCalc):
		"""
		应用题目集结果
		Args:
			calc (QuesSetResultRewardCalc): 结果
		"""
		if calc is None: return

		self._applyBaseResult(calc)
		self._applyPlayerResult(calc)
		self._applyRewardsResult(calc)

	def _applyBaseResult(self, calc: QuesSetResultRewardCalc):
		"""
		应用基本结果
		Args:
			calc (QuesSetResultRewardCalc): 结果
		"""
		self.exer_exp_incrs = calc.exer_exp_incrs
		self.slot_exp_incrs = calc.slot_exp_incrs
		self.gold_incr = calc.gold_incr

	def _applyPlayerResult(self, calc: QuesSetResultRewardCalc):
		"""
		应用玩家结果
		Args:
			calc (QuesSetResultRewardCalc): 结果
		"""
		player = self.exactlyPlayer()

		player.gainMoney(gold=calc.gold_incr)
		player.gainExp(calc.slot_exp_incr, calc.exer_exp_incrs,
					   calc.slot_exp_incrs)

	def _applyRewardsResult(self, calc: QuesSetResultRewardCalc):
		"""
		应用物品奖励结果
		Args:
			calc (QuesSetResultRewardCalc): 结果
		"""
		from item_module.models import PackContainer

		for reward in calc.item_rewards: self.addReward(**reward)

		player = self.exactlyPlayer()

		for reward in self.rewards():
			cla = reward.containerType()
			container: PackContainer = player.getContainer(cla)
			container.gainItems(reward.item(), reward.count)

	# endregion

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

	def sumStdTime(self, player_queses: QuerySet = None) -> int:
		"""
		获取题目集总标准用时
		Args:
			player_queses (QuerySet): 玩家题目关系集合，默认情况下为所有题目关系
		Returns:
			题目集总标准用时
		"""
		return self._sumData('std_time', lambda d: d.question.stdTime(),
							 player_queses)

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

	"""占位符"""
	

from .question_system.question_sets import *
from .question_system.player_questions import *
