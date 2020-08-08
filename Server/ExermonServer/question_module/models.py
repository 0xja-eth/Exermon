from django.db import models
from django.db.models import QuerySet

from .types import *

from utils.cache_utils import CacheHelper
from utils.model_utils import QuestionImageUpload

from enum import Enum
import datetime

# region 题目


# ===================================================
#  题目状态枚举
# ===================================================
class QuestionStatus(Enum):
	Normal = 0  # 正常
	Abnormal = 1  # 异常
	Other = -1  # 其他


# ===================================================
#  基本题目表
# ===================================================
class BaseQuestion(models.Model):

	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "题目"

	TYPES = QUES_TYPES

	STATUSES = [
		(QuestionStatus.Normal.value, '正常'),
		(QuestionStatus.Abnormal.value, '异常'),
		(QuestionStatus.Other.value, '其他')
	]

	TYPE = QuestionType.Others

	# 默认分值
	DEFAULT_SCORE = 1

	# 标准用时（秒，为0则不限时）
	STD_TIME = 0

	REPORT_CLASS: 'BaseQuesReport' = None
	RECORD_CLASS: 'BaseQuesRecord' = None
	PICTURE_CLASS: 'BaseQuesPicture' = None

	LIST_EDITABLE_EXCLUDE = ['create_time']

	# 科目
	subject = models.ForeignKey('game_module.Subject', default=1, on_delete=models.CASCADE, verbose_name="科目")

	# 创建时间
	create_time = models.DateTimeField(auto_now_add=True, verbose_name="收录时间")

	# 状态
	status = models.PositiveSmallIntegerField(default=QuestionStatus.Normal.value,
											  choices=STATUSES, verbose_name="状态")

	# 分值
	score = models.PositiveSmallIntegerField(default=None, null=True,
											 blank=True, verbose_name="分值")

	# 来源
	source = models.TextField(null=True, blank=True, verbose_name="来源")

	# 题解
	description = models.TextField(null=True, blank=True, verbose_name="题解")

	# 是否小学题目
	is_primary = models.BooleanField(default=True, verbose_name="是否小学题目")

	# 是否初中题目
	is_middle = models.BooleanField(default=True, verbose_name="是否初中题目")

	# 是否高中题目
	is_high = models.BooleanField(default=True, verbose_name="是否高中题目")

	# 测试题目
	for_test = models.BooleanField(default=False, verbose_name="测试")

	@classmethod
	def reportClass(cls): return cls.REPORT_CLASS

	@classmethod
	def recordClass(cls): return cls.RECORD_CLASS

	def stdTime(self): return self.STD_TIME

	# 生成随机编号
	def number(self): return self.id

	def convert(self, type=None):

		res = {}

		self._convertIndexInfo(res, type)

		if type == 'info': return res

		self._convertBaseInfo(res, type)

		if type == 'answer':
			self._convertAnswerInfo(res)

		return res

	# 转化索引信息
	def _convertIndexInfo(self, res, type):

		res['id'] = self.id
		res['type'] = self.TYPE.value
		res['subject_id'] = self.subject_id

	# 转化基本信息
	def _convertBaseInfo(self, res, type):

		create_time = ModelUtils.timeToStr(self.create_time)

		pictures = ModelUtils.objectsToDict(self.pictures())

		res['number'] = self.number()
		res['score'] = self.quesScore()
		res['create_time'] = create_time
		res['status'] = self.status

		res['pictures'] = pictures

		res['is_primary'] = self.is_primary
		res['is_middle'] = self.is_middle
		res['is_high'] = self.is_high

	# 转化答案信息
	def _convertAnswerInfo(self, res):

		res['source'] = self.source
		res['description'] = self.description

	def quesScore(self):
		return self.score or self.DEFAULT_SCORE

	# 选项
	@CacheHelper.staticCache
	def pictures(self):
		if self.PICTURE_CLASS is None: return []

		class_name = self.PICTURE_CLASS.__name__
		attr_name = class_name.lower() + '_set'

		return getattr(self, attr_name).all()

	# 计算选择是否正确
	def calcCorrect(self, **kwargs):
		raise NotImplementedError

	# 计算分数
	def calcScore(self, **kwargs):
		raise NotImplementedError


# ===================================================
#  基本题目图片表
# ===================================================
class BaseQuesPicture(models.Model):
	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "题目图片"

	QUESTION_CLASS: BaseQuestion = None

	# 序号
	number = models.PositiveSmallIntegerField(verbose_name="序号")

	# 解析图片
	desc_pic = models.BooleanField(default=False, verbose_name="解析图片")

	# 图片文件名
	file = models.ImageField(upload_to=QuestionImageUpload(), verbose_name="图片文件")

	question: BaseQuestion = None

	# # 图片所属题目
	# question = models.ForeignKey('GeneralQuestion', null=False, on_delete=models.CASCADE,
	# 							 verbose_name="所属题目")

	def __str__(self):
		return self.file.url

	# 获取完整路径
	def getExactlyPath(self):
		base = settings.STATIC_URL
		path = os.path.join(base, str(self.file))
		if os.path.exists(path):
			return path
		else:
			raise GameException(ErrorType.PictureFileNotFound)

	# 获取视频base64编码
	def convertToBase64(self):

		with open(self.getExactlyPath(), 'rb') as f:
			data = base64.b64encode(f.read())

		return data.decode()

	def convert(self):
		return {
			'number': self.number,
			'desc_pic': self.desc_pic,
			'data': self.convertToBase64()
		}


# region 选择题


# ===================================================
#  题目类型枚举
# ===================================================
class SelectingQuestionType(Enum):
	Single = 0  # 单选题
	Multiple = 1  # 多选题
	Judge = 2  # 判断题


# ===================================================
#  选择题表
# ===================================================
class SelectingQuestion(BaseQuestion):
	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "选择题"

	SEL_TYPES = [
		(SelectingQuestionType.Single.value, '单选题'),
		(SelectingQuestionType.Multiple.value, '多选题'),
		(SelectingQuestionType.Judge.value, '判断题'),
	]

	TYPE = QuestionType.Selecting

	CHOICE_CLASS: 'BaseQuesChoice' = None

	# 题干
	title = models.TextField(verbose_name="题干")

	# 选择类型
	sel_type = models.PositiveSmallIntegerField(
		default=SelectingQuestionType.Single.value,
		choices=SEL_TYPES, verbose_name="类型")

	def __str__(self):

		return self.title[:32]

	# 生成随机编号
	def number(self): return self.id

	# 正确答案（编号）文本
	def adminCorrectAnswer(self):

		text = ""
		answers = self.correctAnswer()

		for answer in answers:
			text += chr(65 + answer) + ' '

		return text

	adminCorrectAnswer.short_description = "正确选项"

	def _convertBaseInfo(self, res, type):
		super()._convertBaseInfo(res, type)

		choices = ModelUtils.objectsToDict(self.choices(), type=type)

		res['title'] = self.title
		res['choices'] = choices
		res['sel_type'] = self.sel_type

	# 正确答案（编号）
	def correctAnswer(self):

		answers = []
		for choice in self.correctChoices():
			answers.append(choice.order)

		return answers

	# 选项
	@CacheHelper.staticCache
	def choices(self):
		if self.CHOICE_CLASS is None: return []

		class_name = self.CHOICE_CLASS.__name__
		attr_name = class_name.lower() + '_set'

		return getattr(self, attr_name).all()

	# 正确选项
	def correctChoices(self):
		return self.choices().filter(answer=True)

	# 计算选择是否正确
	def calcCorrect(self, selection):

		count = 0
		answers = self.correctAnswer()

		for select in selection:
			if select not in answers: return False
			else: count += 1

		return count == len(answers)

	# 计算分数
	def calcScore(self, selection):

		count = 0
		answers = self.correctAnswer()

		for select in selection:
			if select not in answers: return 0
			else: count += 1

		if count == len(answers): return self.quesScore()
		return 0


# ===================================================
#  题目选项表
# ===================================================
class BaseQuesChoice(models.Model):
	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "题目选项"

	QUESTION_CLASS: SelectingQuestion = None

	# 编号
	order = models.PositiveSmallIntegerField(verbose_name="编号")

	# 文本
	text = models.TextField(verbose_name="文本")

	# 正误
	answer = models.BooleanField(default=False, verbose_name="正误")

	question: SelectingQuestion = None

	def __str__(self):
		return self.text

	def convert(self):
		return {
			'order': self.order,
			'text': self.text,
			'answer': self.answer
		}

# endregion

# region 填空题

# endregion

# region 改错题


# ===================================================
#  改错题
# ===================================================
class CorrectingQuestion(BaseQuestion):
	class Meta:
		verbose_name = verbose_name_plural = "改错题"

	TYPE = QuestionType.Correcting

	# 文章
	article = models.TextField(verbose_name="文章")

	def _convertBaseInfo(self, res, type):
		super()._convertBaseInfo(res, type)

		res['article'] = self.article

	def _convertAnswerInfo(self, res):
		super()._convertAnswerInfo(res)

		wrong_items = ModelUtils.objectsToDict(self.wrongItems())

		res['wrong_items'] = wrong_items

	@CacheHelper.staticCache
	def wrongItems(self) -> QuerySet:
		"""
		错误项
		Returns:
			返回该改错题目的错误项
		"""
		return self.wrongitem_set.all()


# ===================================================
#  纠正类型
# ===================================================
class CorrectType(Enum):
	Add = 1  # 增加
	Edit = 2  # 修改
	Delete = 3  # 删除


# ===================================================
#  改错题错误项
# ===================================================
class WrongItem(models.Model):
	class Meta:
		verbose_name = verbose_name_plural = "改错题错误项"

	TYPES = [
		(CorrectType.Add.value, '增加'),
		(CorrectType.Edit.value, '修改'),
		(CorrectType.Delete.value, '删除'),
	]

	# 句子编号
	sentence_index = models.PositiveSmallIntegerField(verbose_name="句子编号")

	# 单词编号
	word_index = models.PositiveSmallIntegerField(verbose_name="单词编号")

	# 修改类型
	type = models.PositiveSmallIntegerField(default=CorrectType.Edit.value,
											choices=TYPES, verbose_name="修改类型")

	# 正确单词
	word = models.TextField(verbose_name="正确单词", null=True, blank=True)

	# 对应题目
	question = models.ForeignKey('CorrectingQuestion', on_delete=models.CASCADE,
								 verbose_name="改错题目")

	def convert(self):
		return {
			'id': self.id,
			'sentence_index': self.sentence_index,
			'word_index': self.word_index,
			'type': self.type,
			'word': self.word,
		}

# endregion

# region 简答题

# endregion

# region 元素


# ===================================================
#  元素题目
# ===================================================
class ElementQuestion(BaseQuestion):
	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "元素"

	TYPE = QuestionType.Element


# endregion

# region 组合


# ===================================================
#  组合题
# ===================================================
class GroupQuestion(BaseQuestion):
	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "组合题"

	TYPE = QuestionType.Group

	SUB_QUES_CLASS: BaseQuestion = None

	# 文章
	article = models.TextField(null=True, blank=True, verbose_name="文章")

	def __str__(self):
		return "%s. %s" % (self.id, self.article)

	def _convertBaseInfo(self, res, type):
		super()._convertBaseInfo(res, type)

		sub_questions = ModelUtils.objectsToDict(
			self.subQuestions(), type=type)

		res['article'] = self.article
		res['sub_questions'] = sub_questions

	@CacheHelper.staticCache
	def subQuestions(self) -> QuerySet:
		if self.SUB_QUES_CLASS is None: return []

		class_name = self.SUB_QUES_CLASS.__name__
		attr_name = class_name.lower() + '_set'

		return getattr(self, attr_name).all()


# endregion

# endregion


# region 题目记录


# ===================================================
#  记录来源
# ===================================================
class RecordSource(Enum):
	Exercise = 1  # 刷题
	Exam = 2  # 考核
	Battle = 3  # 对战
	Adventure = 4  # 副本
	ExerPro = 5  # 特训
	Others = 0  # 其他


# ===================================================
#  基本题目记录表
# ===================================================
class BaseQuesRecord(models.Model):

	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "做题记录"

	SOURCES = [
		(RecordSource.Exercise.value, '刷题'),
		(RecordSource.Exam.value, '考核'),
		(RecordSource.Battle.value, '对战'),
		(RecordSource.Adventure.value, '冒险'),
		(RecordSource.ExerPro.value, '特训'),
		(RecordSource.Others.value, '其他'),
	]

	MAX_NOTE_LEN = 128

	QUESTION_CLASS: BaseQuestion = None

	# 题目
	question: BaseQuestion = None
	# question = models.ForeignKey('question_module.models.GeneralQuestion', null=False,
	# 	on_delete=models.CASCADE, verbose_name="题目")

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

	@classmethod
	def questionClass(cls): return cls.QUESTION_CLASS

	@classmethod
	def questionType(cls): return cls.questionClass().TYPE

	# 转化为字典
	def convert(self):

		last_date = ModelUtils.timeToStr(self.last_date)
		first_date = ModelUtils.timeToStr(self.first_date)

		question_id = ModelUtils.objectToId(self.question)
		question_type = self.questionType().value

		return {
			'id': self.id,
			'question_id': question_id,
			'question_type': question_type,

			'count': self.count,
			'correct': self.correct,
			'first_date': first_date,
			'last_date': last_date,
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

		self._updateRecord(player_ques)

		self.save()

	def _updateRecord(self, player_ques):

		if player_ques.correct():
			self.correct += 1
		else:
			self.wrong = True

		if self.count <= 0:
			self.source = player_ques.source().value

			self.first_date = datetime.datetime.now()

		self.last_date = datetime.datetime.now()
		self.count += 1

	# 正确率
	def corrRate(self):
		if self.count is None or self.count == 0:
			return 0
		return self.correct / self.count


# endregion


# region 反馈


# ===================================================
#  题目反馈类型枚举
# ===================================================
class QuesReportType(Enum):
	Other = 0  # 其他错误

	QuestionError = 1  # 题目错误（题目显示错误/题目内容错误/空白题目）
	PictureError = 2  # 图片错误（图片显示错误/图片不对应/图片不显示）
	AnswerError = 3  # 答案错误（无正确答案/正确答案错误）
	DescError = 4  # 解析错误（解析内容错误/解析与答案不匹配/无解析）
	SubjectError = 5  # 科目错误（题目科目不匹配）
	DifficultyError = 6  # 难度分配错误（题目难度等级不合适）
	MultError = 7  # 多个错误（请在描述中说明）


# ===================================================
#  题目反馈表
# ===================================================
class BaseQuesReport(models.Model):

	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "题目反馈"

	# 常量定义
	MAX_DESC_LEN = 256

	QUESTION_CLASS: BaseQuestion = None

	TYPES = [
		(QuesReportType.QuestionError.value, '题目错误'),
		(QuesReportType.PictureError.value, '图片错误'),
		(QuesReportType.AnswerError.value, '答案错误'),
		(QuesReportType.DescError.value, '解析错误'),
		(QuesReportType.SubjectError.value, '科目错误'),
		(QuesReportType.DifficultyError.value, '难度分配错误'),
		(QuesReportType.MultError.value, '多个错误'),

		(QuesReportType.Other.value, '其他错误'),
	]

	TYPES_WITH_DESC = [
		(QuesReportType.QuestionError.value, '题目错误（题目显示错误/题目内容错误/空白题目）'),
		(QuesReportType.PictureError.value, '图片错误（图片显示错误/图片不对应/图片不显示）'),
		(QuesReportType.AnswerError.value, '答案错误（无正确答案/正确答案错误）'),
		(QuesReportType.DescError.value, '解析错误（解析内容错误/解析与答案不匹配/无解析）'),
		(QuesReportType.SubjectError.value, '科目错误（题目科目不匹配）'),
		(QuesReportType.DifficultyError.value, '难度分配错误（题目难度等级不合适）'),
		(QuesReportType.MultError.value, '多个错误（请在描述中说明）'),

		(QuesReportType.Other.value, '其他错误（请在描述中说明）'),
	]

	LIST_EDITABLE = []

	# 玩家
	player = models.ForeignKey('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 题目
	question: BaseQuestion = None
	# question = models.ForeignKey('GeneralQuestion', on_delete=models.CASCADE, verbose_name="题目")

	# 反馈类型
	type = models.PositiveSmallIntegerField(choices=TYPES, verbose_name="类型")

	# 反馈描述
	description = models.CharField(max_length=MAX_DESC_LEN, verbose_name="描述")

	# 反馈时间
	create_time = models.DateTimeField(auto_now_add=True, verbose_name="反馈时间")

	# 处理结果
	result = models.CharField(max_length=MAX_DESC_LEN, null=True, blank=True, verbose_name="描述")

	# 处理时间
	result_time = models.DateTimeField(null=True, blank=True, verbose_name="处理时间")

	def __str__(self):
		return "%d. %s %s %s" % (self.id, self.player, self.question.number(), self.type)

	@classmethod
	def questionClass(cls): return cls.QUESTION_CLASS

	@classmethod
	def questionType(cls): return cls.questionClass().TYPE

	# 创建
	@classmethod
	def create(cls, player, question_id, type, description):
		report = cls()
		report.player = player
		report.question_id = question_id
		report.type = type
		report.description = description
		report.save()

		return report

	def convert(self):

		create_time = ModelUtils.timeToStr(self.create_time)
		result_time = ModelUtils.timeToStr(self.result_time)

		question_id = ModelUtils.objectToId(self.question)
		question_type = self.questionType().value

		return {
			'id': self.id,
			'question_id': question_id,
			'question_type': question_type,

			'type': self.type,
			'description': self.description,
			'create_time': create_time,
			'result': self.result,
			'result_time': result_time,
		}

# endregion

from .question_system.element_questions import *
from .question_system.selecting_questions import *
from .question_system.group_questions import *

