from django.db import models
from utils.model_utils import CacheableModel, Common as ModelUtils
from utils.exception import ErrorType, ErrorException
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
	note = models.CharField(blank=True, null=True, max_length=128, verbose_name="备注")

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
		record = cls()
		record.player = player
		record.question_id = question_id

		return record

	# 更新已有记录
	def updateRecord(self, player_ques, collect=False):

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

		if collect: self.collected = True

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

	def convertToDict(self, type=None):

		if type == 'answer':

			return {
				'score': self.score(),
				'correct_selection': self.question.correctAnswer()
			}

		return {
			'question_id': self.question.id,
			'selection': self.selection,
			'timespan': self.timespan,
			'exp_incr': self.exp_incr,
			'slot_exp_incr': self.slot_exp_incr,
			'gold_incr': self.gold_incr,
			'is_new': self.is_new,
		}

	# 创建题目
	@classmethod
	def create(cls, question_set, question_id):
		ques = cls()
		ques.question_id = question_id
		ques.setQuestionSet(question_set)
		ques._updateNew()

		return ques

	# 奖励计算类
	@classmethod
	def rewardCalculator(cls):
		from utils.calc_utils import QuestionSetSingleRewardCalc
		return QuestionSetSingleRewardCalc

	# 更新是否新题目
	def _updateNew(self):
		player = self.questionSet().player
		res = player.questionrecord_set.filter(
			question_id=self.question_id)

		self.is_new = res.exists()

	# 来源
	def source(self):
		return RecordSource.Others

	# 设置题目集
	def setQuestionSet(self, question_set):
		raise NotImplementedError
		# self.question_set = question_set

	# 获取题目集
	def questionSet(self):
		raise NotImplementedError
		# return self.question_set

	# 是否正确
	def correct(self):
		return self.getOrSetCache(self.CORRECT_CACHE_KEY,
			lambda: self.question.calcCorrect(self.selection))

	# 计算得分
	def score(self):
		return self.getOrSetCache(self.SCORE_CACHE_KEY,
			lambda: self.question.calcScore(self.selection))

	# 开始做题
	def start(self):
		self.cache(self.STARTTIME_CACHE_KEY, datetime.datetime.now())

	# 作答
	def answer(self, selection, record):

		start_time = self.getCache(self.STARTTIME_CACHE_KEY)

		if start_time is None:
			raise ErrorException(ErrorType.QuestionNotStarted)

		now = datetime.datetime.now()
		if now <= start_time:
			raise ErrorException(ErrorType.InvalidTimeSpan)

		self.timespan = (now - start_time).microseconds
		self.selection = selection
		self.answered = True

		self._calcRewards(record)

		# self.save()

	# 计算奖励
	def _calcRewards(self, record):

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

	# 奖励计算类
	@classmethod
	def rewardCalculator(cls):
		from utils.calc_utils import ExerciseSingleRewardCalc
		return ExerciseSingleRewardCalc

	# 来源
	def source(self):
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

	Unset = 0 # 未设置


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

	def __str__(self):
		return "%s %s" % (self.player, self.generateName())

	def convertToDict(self, type=None):

		create_time = ModelUtils.timeToStr(self.create_time)
		player_questions = ModelUtils.objectsToDict(self.playerQuestions())

		return {
			'id': self.id,
			'exp_incr': self.exp_incr,
			'slot_exp_incr': self.slot_exp_incr,
			'gold_incr': self.gold_incr,
			'player_questions': player_questions,
			'create_time': create_time,
		}

	# 创建一个题目集
	@classmethod
	def create(cls, player, **kwargs):
		record = cls()
		record.player = player
		record.start(**kwargs)

		return record

	# 奖励计算类
	@classmethod
	def rewardCalculator(cls):
		from utils.calc_utils import QuestionSetResultRewardCalc
		return QuestionSetResultRewardCalc

	# 生成名字
	def generateName(self): return ''

	# 获取所有题目关系
	def playerQuestions(self):
		return self.getOrSetCache(self.QUES_CACHE_KEY,
			lambda: self._playerQuestions())

	# 获取所有题目关系
	def _playerQuestions(self):
		raise NotImplementedError

	# 获取题目关系
	def playerQuestion(self, question_id):
		res = self.playerQuestions().filter(question_id=question_id)
		if res.exists(): return res.first()
		return None

	# 获取缓存的题目关系
	def cachedPlayerQuestion(self, question_id):

		cache = self._getQuestionCache()
		for player_ques in cache:
			if player_ques.question_id == question_id:
				return player_ques

		return None

	# 实际玩家（获取在线信息）
	def exactlyPlayer(self):
		from player_module.views import Common

		player = self.player
		# 如果获取不到实际的玩家，该对象释放时需要自动保存
		player.delete_save = True

		online_player = Common.getOnlinePlayer(player.id)

		return online_player or player

	# 开始一个题目集（生成题目）
	def start(self, **kwargs):

		self._initQuestionCache()

		self.exactlyPlayer().setExercise(self)

		self._generateQuestions(**kwargs)

	# 生成题目
	def _generateQuestions(self, **kwargs):
		pass

	# 结束答题
	def terminate(self, **kwargs):

		self.finished = True

		self._applyResult(self._calcResult())

		# self._saveQuestionCache()

		# 会自动保存
		self.exactlyPlayer().clearExercise()

	def _calcResult(self):

		calc = self.rewardCalculator().calc(self.player_questions)

		self.exp_incr = calc.exer_exp_incr
		self.slot_exp_incr = calc.exerslot_exp_incr
		self.gold_incr = calc.gold_incr

		return calc

	def _applyResult(self, calc):

		self.exactlyPlayer().gainMoney(self.gold_incr)
		self.exactlyPlayer().gainExp(self.slot_exp_incr,
							calc.exer_exp_incrs,
							calc.exerslot_exp_incrs)

	# 增加题目
	def addQuestion(self, question_id):

		player_ques = ExerciseQuestion.create(self, question_id)

		if self.player_questions is None:
			self.player_questions = []

		self._addQuestionToCache(player_ques)

	# 回答题目
	def answerQuestion(self, question_id, selection, collect=False):

		# 从缓存中读取
		player_ques: PlayerQuestion = self.cachedPlayerQuestion(question_id)

		rec = QuestionRecord.create(self.player, player_ques.question_id)

		player_ques.answer(selection, rec)

		rec.updateRecord(player_ques, collect)

	# 初始化题目缓存
	def _initQuestionCache(self):
		self.cache(self.QUES_CACHE_KEY, [])

	# 获取题目缓存
	def _getQuestionCache(self):
		cache = self.getCache(self.QUES_CACHE_KEY)
		if cache is None: return []
		return cache

	# 往缓存中添加题目
	def _addQuestionToCache(self, player_ques):
		self._getQuestionCache().append(player_ques)

	# # 初始化题目缓存
	# def _saveQuestionCache(self):
	# 	self.saveCache(self.QUES_CACHE_KEY)

	# region 统计数据

	# 统计数据
	def _totalData(self, key, func, player_queses=None):
		if not self.finished: return []

		return self.getOrSetCache('total_' + key,
			lambda: self.__generTotalData(func, player_queses))

	def __generTotalData(self, func, player_queses=None):
		res = []

		if player_queses is None:
			player_queses = self.playerQuestions()

		for data in player_queses:
			res.append(func(data))

		return res

	# 求和数据
	def _sumData(self, key, func, player_queses=None):
		if not self.finished: return -1

		return self.getOrSetCache('sum_' + key,
			lambda: self.__generSumData(func, player_queses))

	def __generSumData(self, func, player_queses=None):
		res = 0

		data = func(player_queses)
		for d in data: res += int(d)

		return res

	# 全部选择
	def selections(self, player_queses=None):
		return self._totalData('selections',
							   lambda d: d.selection, player_queses)

	# 全部用时
	def timespans(self, player_queses=None):
		return self._totalData('timespans',
							   lambda d: d.timespans, player_queses)

	# 全部得分
	def scores(self, player_queses=None):
		return self._totalData('scores',
							   lambda d: d.score(), player_queses)

	# 全部正误
	def results(self, player_queses=None):
		return self._totalData('results',
							   lambda d: d.correct(), player_queses)

	# 总用时
	def sumTimespan(self, player_queses=None):
		return self._sumData('timespans', self.timespans, player_queses)

	# 总得分
	def sumScore(self, player_queses=None):
		return self._sumData('scores', self.scores, player_queses)

	# 正确数量
	def corrCount(self, player_queses=None):
		return self._sumData('correct', self.results, player_queses)

	# 正确率
	def corrRate(self, player_queses=None):
		if not self.finished: return -1

		corr_cnt = 0
		results = self.results(player_queses)
		for result in results: corr_cnt += int(result)

		return corr_cnt / len(results)

	# endregion


# ===================================================
#  分配类型枚举
# ===================================================
class ExerciseDistributionType(Enum):
	Normal = 0  # 普通模式
	OccurFirst = 1  # 已做优先
	NotOccurFirst = 2  # 未做优先
	WorngFirst = 3  # 错题优先
	CorrFirst = 4  # 对题优先
	SimpleFirst = 5  # 简单题优先
	MiddleFirst = 6  # 中等题优先
	DifficultFirst = 7  # 难题优先


# ===================================================
#  刷题记录表
# ===================================================
class ExerciseRecord(QuestionSetRecord):

	class Meta:
		verbose_name = verbose_name_plural = "刷题记录"

	TYPE = QuestionSetType.Exercise

	DTB_CHOICES = [
		(ExerciseDistributionType.Normal.value, '普通模式'),
		(ExerciseDistributionType.OccurFirst.value, '已做优先'),
		(ExerciseDistributionType.NotOccurFirst.value, '未做优先'),
		(ExerciseDistributionType.WorngFirst.value, '错题优先'),
		(ExerciseDistributionType.CorrFirst.value, '对题优先'),
		(ExerciseDistributionType.SimpleFirst.value, '简单题优先'),
		(ExerciseDistributionType.MiddleFirst.value, '中等题优先'),
		(ExerciseDistributionType.DifficultFirst.value, '难题优先'),
	]

	# 常量定义
	NAME_STRING_FMT = "%s\n%s 刷题记录"

	# 科目
	subject = models.ForeignKey('game_module.Subject', default=1,
								on_delete=models.CASCADE, verbose_name="科目")

	# 题量（用于生成题目）
	count = models.PositiveSmallIntegerField(default=1, verbose_name="题量")

	# 题目分配类型
	dtb_type = models.PositiveSmallIntegerField(choices=DTB_CHOICES, default=0,
												verbose_name="分配类型")

	# 所属赛季
	# season = models.ForeignKey('rank_module.GraduationSeason', on_delete=models.CASCADE,
	#						   verbose_name="赛季")

	# 奖励计算类
	@classmethod
	def rewardCalculator(cls):
		from utils.calc_utils import ExerciseResultRewardCalc
		return ExerciseResultRewardCalc

	# 生成名字
	def generateName(self):

		return self.NAME_STRING_FMT % (self.create_time.strftime("%Y-%m-%d %H:%M:%S"),
									   self.subject.name)

	# 获取所有题目关系
	def _playerQuestions(self):
		return self.exercisequestion_set.all()
