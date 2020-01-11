from django.db import models
from django.conf import settings
from utils.model_utils import SystemImageUpload
from utils.exception import ErrorType, ErrorException
import os, base64
from enum import Enum


# ===================================================
#  题目等级表
# ===================================================
class QuestionLevel(models.Model):
	class Meta:
		verbose_name = verbose_name_plural = "题目等级"

	# 题目等级名
	text = models.CharField(max_length=4, verbose_name="名称")

	# 等级压力增加
	pressure = models.PositiveSmallIntegerField(default=0, verbose_name="压力增加")

	# 等级权重（用于生成分数/生成初始科目点数）
	weight = models.PositiveSmallIntegerField(default=0, verbose_name="等级权重")

	# 等级增益基数（用于计算增益点数）
	incre = models.PositiveSmallIntegerField(default=0, verbose_name="增益基数")

	# 等级点数限制
	entry = models.PositiveSmallIntegerField(default=0, verbose_name="点数限制")

	# 等级标准时间（单位：分钟）
	std_time = models.PositiveSmallIntegerField(default=0, verbose_name="标准时间（分）")

	# 等级最短时间（单位：秒）
	min_time = models.PositiveSmallIntegerField(default=0, verbose_name="最短时间（秒）")

	# 等级基础威力
	power = models.PositiveSmallIntegerField(default=0, verbose_name="对战威力")

	def __str__(self):
		return self.text

	def convertToDict(self):
		return {
			'id': self.id,
			'text': self.text,
			'pressure': self.pressure,
			'weight': self.weight,
			'incre': self.incre,
			'entry': self.entry,
			'power': self.power,
			'std_time': self.std_time,
			'min_time': self.min_time
		}


# ===================================================
#  题目选项表
# ===================================================
class QuestionChoice(models.Model):
	class Meta:
		verbose_name = verbose_name_plural = "题目选项"

	# 文本
	text = models.TextField(verbose_name="文本")

	# 正误
	answer = models.BooleanField(default=False, verbose_name="正误")

	# 所属问题
	question = models.ForeignKey('Question', null=False, on_delete=models.CASCADE,
								 verbose_name="所属问题")

	def __str__(self):
		return self.text

	def questionId(self):
		if self.question == None:
			return ''
		return self.question.id

	def convertToDict(self):
		return {
			'text': self.text,
			'answer': self.answer
		}


# ===================================================
#  题目图片表
# ===================================================
class QuestionPicture(models.Model):
	class Meta:

		verbose_name = verbose_name_plural = "题目图片"

	# 图片文件名
	file = models.ImageField(upload_to=SystemImageUpload('pictures'), verbose_name="图片文件")

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
			raise ErrorException(ErrorType.PictureFileNotFound)

	# 获取视频base64编码
	def convertToBase64(self):

		with open(self.getExactlyPath(), 'rb') as f:
			data = base64.b64encode(f.read())

		return data.decode()


# ===================================================
#  题目反馈类型枚举
# ===================================================
class QuestionReportType(Enum):
	QuestionError = 0  # 题目错误（题目显示错误/题目内容错误/空白题目）
	PictureError = 1  # 图片错误（图片显示错误/图片不对应/图片不显示）
	AnswerError = 2  # 答案错误（无正确答案/正确答案错误/答案题目不匹配）
	SubjectError = 3  # 科目错误（题目科目不匹配）
	DifficultyError = 4  # 难度分配错误（题目难度等级不合适）
	Other = -1  # 其他错误


# ===================================================
#  题目反馈表
# ===================================================
class QuestionReport(models.Model):
	class Meta:
		verbose_name = verbose_name_plural = "题目反馈"

	# 常量定义
	MAX_DESC_LEN = 256

	TYPE_CHOICES = [
		(QuestionReportType.QuestionError.value, '题目错误'),
		(QuestionReportType.PictureError.value, '图片错误'),
		(QuestionReportType.AnswerError.value, '答案错误'),
		(QuestionReportType.SubjectError.value, '科目错误'),
		(QuestionReportType.DifficultyError.value, '难度分配错误'),
		(QuestionReportType.Other.value, '其他')
	]

	# 玩家
	player = models.ForeignKey('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 题目
	question = models.ForeignKey('Question', on_delete=models.CASCADE, verbose_name="题目")

	# 反馈类型
	type = models.PositiveSmallIntegerField(choices=TYPE_CHOICES, verbose_name="类型")

	# 反馈描述
	description = models.CharField(max_length=MAX_DESC_LEN, verbose_name="描述")


# ===================================================
#  题目类型枚举
# ===================================================
class QuestionType(Enum):
	Single = 0  # 单选题
	Multiple = 1  # 多选题
	Judge = 2  # 判断题
	Other = -1  # 其他


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
class Question(models.Model):
	class Meta:

		verbose_name = verbose_name_plural = "题目"

	# 常量声明
	DEFAULT_SCORE = 6
	CORRECT_PRESSURE_RATE = 0.25
	UNDER_SELECT_SCORE_FACTOR = 0.5

	QUESTION_TYPES = [
		(QuestionType.Single.value, '单选题'),
		(QuestionType.Multiple.value, '多选题'),
		(QuestionType.Judge.value, '判断题'),
		(QuestionType.Other.value, '其他题'),
	]

	QUESTION_STATUSES = [
		(QuestionStatus.Normal.value, '正常'),
		(QuestionStatus.Abnormal.value, '异常'),
		(QuestionStatus.Other.value, '其他')
	]

	# 题干
	title = models.TextField(verbose_name="题干")

	# 题解
	description = models.TextField(default="无", verbose_name="题解")

	# 星数
	level = models.ForeignKey('QuestionLevel', default=1, on_delete=models.CASCADE,
							  verbose_name="星级")

	# 分值
	score = models.PositiveSmallIntegerField(default=DEFAULT_SCORE, verbose_name="分值")

	# 科目
	subject = models.ForeignKey('game_module.Subject', default=1, on_delete=models.CASCADE, verbose_name="科目")

	# 创建时间
	create_time = models.DateTimeField(auto_now_add=True, verbose_name="收录时间")

	# 类型ID
	type = models.PositiveSmallIntegerField(default=1, choices=QUESTION_TYPES, verbose_name="类型")

	# 测试题目
	for_test = models.BooleanField(default=False, verbose_name="测试")

	# 状态
	status = models.PositiveSmallIntegerField(default=0, choices=QUESTION_STATUSES, verbose_name="状态")

	# 删除标记
	is_deleted = models.BooleanField(default=False, verbose_name="删除标志")

	def __str__(self):

		return self.title

	def subjectName(self):

		if self.subject is None:
			return ''
		return self.subject.name

	def typeText(self):

		if self.type is None:
			return ''
		return self.QUESTION_TYPES[self.type]

	def correctAnswerText(self):

		text = ""
		answers = self.correctAnswer()

		for answer in answers:
			text += chr(65 + answer) + ' '

		return text

	correctAnswerText.short_description = "正确答案"

	def correctAnswer(self):

		index = 0
		answers = []

		choice_set = self.questionchoice_set.all()
		for choice in choice_set:
			if choice.answer:
				answers.append(index)
			index += 1

		return answers

	def calcCorrect(self, selection):

		count = 0
		answers = self.correctAnswer()

		for select in selection:
			if select not in answers:
				return False
			else:
				count += 1

		return count == len(answers)

	def calcScore(self, selection):

		count = 0
		answers = self.correctAnswer()

		for select in selection:
			if select not in answers:
				return 0
			else:
				count += 1

		if count == len(answers):
			return self.score
		elif count > 0:
			return round(self.score * self.UNDER_SELECT_SCORE_FACTOR)
		else:
			return 0

	"""
		def convertToDict(self, filter_type=None):

			if filter_type == 'record':

				return {
					'id' : self.id,
					'title' : self.title,
					'level' : self.level.id,
					'subject_id' : self.subject.id,
					'type' : self.type.id,
				}

			if not filter_type:
				choices = []
				pictures = []

				choice_set = self.questionchoice_set.all()
				for choice in choice_set:
					choices.append(choice.convertToDict())

				picture_set = self.questionpicture_set.all()
				for picture in picture_set:
					pictures.append(picture.convertToBase64())

				return {
					'id' : self.id,
					'title' : self.title,
					'description' : self.description,
					'level' : self.level.id,
					'score' : self.score, 
					'subject_id' : self.subject.id,
					'type' : self.type.id,
					'status' : self.status,

					'choices' : choices,
					'pictures' : pictures
				}

			return {}

	def queryDetail(self, choice_detail=True):

		from django.db.models import Sum, Avg

		all_question_rec = self.questionrecord_set.all()
		all_collected_rec = all_question_rec.exclude(collectedquestionrecord__isnull=True)

		all_question_rec = all_question_rec.filter(count__gt=0)

		people = all_question_rec.count()
		collected_people = all_collected_rec.count()

		choices_detail = [0] * self.questionchoice_set.count()

		if people > 0:
			aggregate = all_question_rec.aggregate(Sum('count'), Sum('correct'), Avg('avg_time'))
			sum_count = aggregate['count__sum']
			sum_corr = aggregate['correct__sum']
			avg_time = aggregate['avg_time__avg']
			avg_corr = sum_corr / sum_count

			if choice_detail:
				exam_questions = self.examquestion_set.all()
				exercise_questions = self.exercisequestion_set.all()
				# battle_questions = self.battleroundresult_set.all()

				for exam_q in exam_questions:
					selection = exam_q.selection
					for sel in selection: choices_detail[sel] += 1

				for exercise_q in exercise_questions:
					selection = exercise_q.selection
					for sel in selection: choices_detail[sel] += 1


		else:
			sum_count = 0
			avg_time = avg_corr = 0.0

		return {
			'people': people,
			'collected_people': collected_people,
			'sum_count': sum_count,
			'all_corr_rate': avg_corr,
			'all_avg_time': avg_time,
			'choices_detail': choices_detail
		}

	"""

	def convertToDict(self, filter_type=None):

		if filter_type == 'info':
			return {
				'id': self.id,
				'level': self.level.id,
				'subject_id': self.subject.id,
			}

		create_time = self.create_time.strftime('%Y-%m-%d %H:%M:%S')

		if not filter_type:

			choices = []
			pictures = []

			choice_set = self.questionchoice_set.all()
			for choice in choice_set:
				choices.append(choice.convertToDict())

			picture_set = self.questionpicture_set.all()
			for picture in picture_set:
				pictures.append(picture.convertToBase64())

			return {
				'id': self.id,
				'title': self.title,
				'description': self.description,
				'level': self.level.id,
				'score': self.score,
				'subject_id': self.subject.id,
				'type': self.type,
				'status': self.status,

				'create_time': create_time,

				'choices': choices,
				'pictures': pictures
			}


