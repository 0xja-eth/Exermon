from django.db import models
from django.conf import settings

from .item_system.items import *
from .item_system.containers import *
from .item_system.cont_items import *

from utils.model_utils import QuestionImageUpload, Common as ModelUtils
from utils.exception import ErrorType, GameException

from enum import Enum
import os, base64


# ===================================================
#  题目选项表
# ===================================================
class BaseQuesChoice(models.Model):
	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "题目选项"

	# 编号
	order = models.PositiveSmallIntegerField(verbose_name="编号")

	# 文本
	text = models.TextField(verbose_name="文本")

	# 正误
	answer = models.BooleanField(default=False, verbose_name="正误")

	def __str__(self):
		return self.text

	# def adminQuestionId(self):
	# 	if self.question is None: return ''
	# 	return self.question.id
	#
	# adminColor.short_description = "星级颜色"

	def convert(self):
		return {
			'order': self.order,
			'text': self.text,
			'answer': self.answer
		}


# ===================================================
#  题目类型枚举
# ===================================================
class QuestionType(Enum):
	Single = 0  # 单选题
	Multiple = 1  # 多选题
	Judge = 2  # 判断题
	Other = -1  # 其他


# ===================================================
#  基本题目表
# ===================================================
class BaseQuestion(models.Model):
	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "题目"

	TYPES = [
		(QuestionType.Single.value, '单选题'),
		(QuestionType.Multiple.value, '多选题'),
		(QuestionType.Judge.value, '判断题'),
		(QuestionType.Other.value, '其他题'),
	]

	# 题干
	title = models.TextField(verbose_name="题干")

	# 题解
	description = models.TextField(null=True, blank=True, verbose_name="题解")

	# 类型ID
	type = models.PositiveSmallIntegerField(default=QuestionType.Single.value,
											choices=TYPES, verbose_name="类型")

	# 测试题目
	for_test = models.BooleanField(default=False, verbose_name="测试")

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

	def convert(self, type=None):

		if type == 'info':
			return {
				'id': self.id,
			}

		choices = ModelUtils.objectsToDict(self.choices())

		return {
			'id': self.id,
			'number': self.number(),
			'title': self.title,
			'description': self.description,
			'type': self.type,

			'choices': choices,
		}

	# 正确答案（编号）
	def correctAnswer(self):

		answers = []
		for choice in self.correctChoices():
			answers.append(choice.order)

		return answers

	# 选项
	def choices(self):
		raise NotImplementedError

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


# ===================================================
#  题目选项表
# ===================================================
class QuesChoice(BaseQuesChoice):
	class Meta:
		verbose_name = verbose_name_plural = "题目选项"

	# 所属问题
	question = models.ForeignKey('Question', null=False, on_delete=models.CASCADE,
								 verbose_name="所属问题")


# ===================================================
#  题目图片表
# ===================================================
class QuesPicture(models.Model):
	class Meta:

		verbose_name = verbose_name_plural = "题目图片"

	# 序号
	number = models.PositiveSmallIntegerField(verbose_name="序号")

	# 解析图片
	desc_pic = models.BooleanField(default=False, verbose_name="解析图片")

	# 图片文件名
	file = models.ImageField(upload_to=QuestionImageUpload(), verbose_name="图片文件")

	# 图片所属题目
	question = models.ForeignKey('Question', null=False, on_delete=models.CASCADE,
								 verbose_name="所属题目")

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


# ===================================================
#  题目状态枚举
# ===================================================
class QuestionStatus(Enum):
	Normal = 0  # 正常
	Abnormal = 1  # 异常
	Other = -1  # 其他


# ===================================================
#  题目表
# ===================================================
class Question(BaseQuestion):
	class Meta:

		verbose_name = verbose_name_plural = "题目"

	# 常量声明
	DEFAULT_SCORE = 6
	UNDER_SELECT_SCORE_RATE = 0.5

	# 起始编号
	NUMBER_BASE = 10000

	STATUSES = [
		(QuestionStatus.Normal.value, '正常'),
		(QuestionStatus.Abnormal.value, '异常'),
		(QuestionStatus.Other.value, '其他')
	]

	LIST_EDITABLE_EXCLUDE = ['create_time']

	# 来源
	source = models.TextField(null=True, blank=True, verbose_name="来源")

	# 星数
	star = models.ForeignKey('game_module.QuestionStar', default=1, on_delete=models.CASCADE,
							 verbose_name="星级")

	# 题目附加等级（计算用）
	level = models.SmallIntegerField(default=0, verbose_name="附加等级")

	# 分值
	score = models.PositiveSmallIntegerField(default=DEFAULT_SCORE, verbose_name="分值")

	# 科目
	subject = models.ForeignKey('game_module.Subject', default=1, on_delete=models.CASCADE, verbose_name="科目")

	# 创建时间
	create_time = models.DateTimeField(auto_now_add=True, verbose_name="收录时间")

	# 状态
	status = models.PositiveSmallIntegerField(default=QuestionStatus.Normal.value,
											  choices=STATUSES, verbose_name="状态")

	# 删除标记
	is_deleted = models.BooleanField(default=False, verbose_name="删除标志")

	def convert(self, type=None):

		res = super().convert(type)

		if type == 'info':
			res['star_id'] = self.star_id
			res['subject_id'] = self.subject_id

			return res

		create_time = ModelUtils.timeToStr(self.create_time)
		pictures = ModelUtils.objectsToDict(self.pictures())

		res['source'] = self.source
		res['level'] = self.level
		res['score'] = self.score
		res['status'] = self.status

		res['star_id'] = self.star_id
		res['subject_id'] = self.subject_id

		res['create_time'] = create_time
		res['pictures'] = pictures

		return res

		# return {
		# 	'id': self.id,
		# 	'number': self.number(),
		# 	'title': self.title,
		# 	'description': self.description,
		# 	'source': self.source,
		# 	'star_id': self.star.id,
		# 	'level': self.level,
		# 	'score': self.score,
		# 	'subject_id': self.subject_id,
		# 	'type': self.type,
		# 	'status': self.status,
		#
		# 	'create_time': create_time,
		#
		# 	'choices': choices,
		# 	'pictures': pictures
		# }
	#
	# # 正确答案（编号）
	# def correctAnswer(self):
	#
	# 	answers = []
	# 	for choice in self.correctChoices():
	# 		answers.append(choice.order)
	#
	# 	return answers

	# 选项
	def choices(self):
		return self.queschoice_set.all()

	# # 正确选项
	# def correctChoices(self):
	# 	return self.choices().filter(answer=True)
	#
	# # 计算选择是否正确
	# def calcCorrect(self, selection):
	#
	# 	count = 0
	# 	answers = self.correctAnswer()
	#
	# 	for select in selection:
	# 		if select not in answers: return False
	# 		else: count += 1
	#
	# 	return count == len(answers)

	# 计算分数
	def calcScore(self, selection):

		count = 0
		answers = self.correctAnswer()

		for select in selection:
			if select not in answers: return 0
			else: count += 1

		if count == len(answers): return self.score
		elif count > 0: return round(self.score * self.UNDER_SELECT_SCORE_RATE)
		else: return 0

	# 图片
	def pictures(self):
		return self.quespicture_set.all()

	# 基础经验值增量
	def expIncr(self):
		return self.star.exp_incr

	# 基础金币增量
	def goldIncr(self):
		return self.star.gold_incr

	# 总等级
	def sumLevel(self):
		return self.star.level + self.level


# ===================================================
#  组合题
# ===================================================
class GroupQuestion(models.Model):

	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "组合题"

	# 文章
	article = models.TextField(null=True, blank=True, verbose_name="文章")

	# 来源
	source = models.TextField(null=True, blank=True, verbose_name="来源")

	def __str__(self):
		return "%s. %s" % (self.id, self.article)

	def convert(self):
		sub_questions = ModelUtils.objectsToDict(self.subQuestions())

		return {
			'id': self.id,
			'article': self.article,
			'source': self.source,

			'sub_questions': sub_questions,
		}

	def subQuestions(self) -> QuerySet:
		raise NotImplementedError


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
class QuesReport(models.Model):

	class Meta:
		verbose_name = verbose_name_plural = "题目反馈"

	# 常量定义
	MAX_DESC_LEN = 256

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
	question = models.ForeignKey('Question', on_delete=models.CASCADE, verbose_name="题目")

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

		return {
			'question_id': self.question_id,
			'type': self.type,
			'description': self.description,
			'create_time': create_time,
			'result': self.result,
			'result_time': result_time,
		}


