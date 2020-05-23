from django.db import models
from django.conf import settings
from item_module.models import *
from question_module.models import BaseQuestion, BaseQuesChoice, GroupQuestion
from utils.model_utils import QuestionAudioUpload, Common as ModelUtils
from utils.exception import ErrorType, GameException
import os, base64, datetime
from enum import Enum

# Create your models here.


# ===================================================
#  题目选项表
# ===================================================
class ListeningQuesChoice(BaseQuesChoice):
	class Meta:
		verbose_name = verbose_name_plural = "听力题目选项"

	# 所属问题
	question = models.ForeignKey('ListeningSubQuestion', null=False, on_delete=models.CASCADE,
								 verbose_name="所属问题")


# ===================================================
#  听力题
# ===================================================
class ListeningSubQuestion(BaseQuestion):

	class Meta:
		verbose_name = verbose_name_plural = "听力小题"

	# 听力题目
	question = models.ForeignKey('ListeningQuestion', on_delete=models.CASCADE,
								 verbose_name="听力题目")

	def choices(self):
		return self.listeningqueschoice_set.all()


# ===================================================
#  听力题
# ===================================================
class ListeningQuestion(GroupQuestion):

	class Meta:
		verbose_name = verbose_name_plural = "听力题"

	# 音频文件
	audio = models.FileField(upload_to=QuestionAudioUpload(), verbose_name="音频文件")

	# 获取完整路径
	def getExactlyPath(self):
		base = settings.STATIC_URL
		path = os.path.join(base, str(self.audio))
		if os.path.exists(path):
			return path
		else:
			raise GameException(ErrorType.PictureFileNotFound)

	# 获取视频base64编码
	def convertToBase64(self):

		with open(self.getExactlyPath(), 'rb') as f:
			data = base64.b64encode(f.read())

		return data.decode()

	def convertToDict(self):
		res = super().convertToDict()

		res['audio'] = self.convertToBase64()

		return res

	def subQuestions(self) -> QuerySet:
		"""
		子题目
		Returns:
			返回该听力题目的子题目
		"""
		return self.listeningsubquestion_set.all()


# ===================================================
#  题目选项表
# ===================================================
class ReadingQuesChoice(BaseQuesChoice):
	class Meta:
		verbose_name = verbose_name_plural = "阅读题目选项"

	# 所属问题
	question = models.ForeignKey('ReadingSubQuestion', null=False, on_delete=models.CASCADE,
								 verbose_name="所属问题")


# ===================================================
#  听力题
# ===================================================
class ReadingSubQuestion(BaseQuestion):

	class Meta:
		verbose_name = verbose_name_plural = "阅读小题"

	# 听力题目
	question = models.ForeignKey('ReadingQuestion', on_delete=models.CASCADE,
								 verbose_name="阅读题目")

	def choices(self):
		return self.readingqueschoice_set.all()


# ===================================================
#  阅读题
# ===================================================
class ReadingQuestion(GroupQuestion):

	class Meta:
		verbose_name = verbose_name_plural = "阅读题"

	def subQuestions(self) -> QuerySet:
		"""
		子题目
		Returns:
			返回该听力题目的子题目
		"""
		return self.readingsubquestion_set.all()


# ===================================================
#  改错题
# ===================================================
class CorrectionQuestion(models.Model):

	class Meta:
		verbose_name = verbose_name_plural = "改错题"

	# 文章
	article = models.TextField(verbose_name="文章")

	# 解析
	description = models.TextField(null=True, blank=True, verbose_name="解析")

	def convertToDict(self):
		wrong_items = ModelUtils.objectsToDict(self.wrongItems())

		return {
			'id': self.id,
			'article': self.article,
			'description': self.description,

			'wrong_items': wrong_items
		}

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
	word = models.TextField(verbose_name="正确单词")

	# 对应题目
	question = models.ForeignKey('CorrectionQuestion', on_delete=models.CASCADE,
								 verbose_name="改错题目")

	def convertToDict(self):
		return {
			'id': self.id,
			'sentence_index': self.sentence_index,
			'word_index': self.word_index,
			'type': self.type,
			'word': self.word,
			'question': self.question,
		}


# ===================================================
#  单词
# ===================================================
class Word(models.Model):

	class Meta:
		verbose_name = verbose_name_plural = "单词"

	# 英文
	english = models.CharField(max_length=64, verbose_name="英文")

	# 中文
	chinese = models.CharField(max_length=64, verbose_name="中文")

	# 词性
	type = models.CharField(max_length=32, verbose_name="词性")

	# 等级
	level = models.PositiveSmallIntegerField(default=1, verbose_name="等级")

	# 是否初中题目
	is_middle = models.BooleanField(default=True, verbose_name="是否初中题目")

	# 是否高中题目
	is_high = models.BooleanField(default=True, verbose_name="是否高中题目")

	def __str__(self):
		return "%d. %s" % (self.id, self.english)

	def convertToDict(self):
		return {
			'id': self.id,
			'english': self.english,
			'chinese': self.chinese,
			'type': self.type,
			'level': self.level,

			'is_middle': self.is_middle,
			'is_high': self.is_high,
		}


# ===================================================
#  单词记录表
# ===================================================
class WordRecord(models.Model):

	class Meta:

		verbose_name = verbose_name_plural = "单词记录"

	# 题目
	word = models.ForeignKey('Word', null=False,
							 on_delete=models.CASCADE, verbose_name="单词")

	# 玩家
	player = models.ForeignKey('player_module.Player', null=False,
							   on_delete=models.CASCADE, verbose_name="玩家")

	# 回答次数
	count = models.PositiveSmallIntegerField(default=0, verbose_name="回答次数")

	# 正确次数
	correct = models.PositiveSmallIntegerField(default=0, verbose_name="正确数")

	# 上次回答日期
	last_date = models.DateTimeField(null=True, verbose_name="上次回答日期")

	# 初次回答日期
	first_date = models.DateTimeField(null=True, verbose_name="初次回答日期")

	# 收藏标志
	collected = models.BooleanField(default=False, verbose_name="收藏标志")

	# 错题标志
	wrong = models.BooleanField(default=False, verbose_name="错题标志")

	# 转化为字符串
	def __str__(self):
		return '%s (%s)' % (self.word, self.player)

	# 转化为字典
	def convertToDict(self, type=None):

		last_date = ModelUtils.timeToStr(self.last_date)
		first_date = ModelUtils.timeToStr(self.first_date)

		return {
			'id': self.id,
			'word_id': self.word_id,
			'count': self.count,
			'correct': self.correct,
			'first_date': first_date,
			'last_date': last_date,
			'collected': self.collected,
			'wrong': self.wrong,
		}

	# 创建新记录
	@classmethod
	def create(cls, player, word_id):
		record = player.wordRecord(word_id)

		if record is None:
			record = cls()
			record.player = player
			record.word_id = word_id
			record.save()

		return record

	def updateRecord(self, correct):
		"""
		更新已有记录
		Args:
			correct (bool): 是否正确
		"""

		if correct: self.correct += 1
		else: self.wrong = True

		if self.count <= 0:
			self.first_date = datetime.datetime.now()

		self.last_date = datetime.datetime.now()
		self.count += 1

		self.save()

	# 正确率
	def corrRate(self):
		if self.count is None or self.count == 0:
			return 0
		return self.correct / self.count


# # ===================================================
# #  英语单词来源枚举
# # ===================================================
# class EnglishWordSourceType(Enum):
#
# 	MiddleSchool = 1  # 初中
# 	HighSchool = 2  # 高中
# 	CET4 = 3  # 四级
# 	CET6 = 4  # 六级
# 	Postgraduate = 5  # 考研
#
# 	Unknown = 0  # 未知
#
#
# # ===================================================
# #  英语单词来源表
# # ===================================================
# class EnglishWordSource(models.Model):
#
# 	class Meta:
# 		verbose_name = verbose_name_plural = "英语单词来源"
#
# 	TYPES = [
# 		(EnglishWordSourceType.MiddleSchool.value, '初中'),
# 		(EnglishWordSourceType.HighSchool.value, '高中'),
# 		(EnglishWordSourceType.CET4.value, '四级'),
# 		(EnglishWordSourceType.CET6.value, '六级'),
# 		(EnglishWordSourceType.Postgraduate.value, '考研'),
#
# 		(EnglishWordSourceType.Unknown.value, '未知'),
# 	]
#
# 	# 来源
# 	source = models.PositiveSmallIntegerField(default=EnglishWordSourceType.Unknown.value,
# 											choices=TYPES, verbose_name="来源")
#
# 	# 单词
# 	word = models.ForeignKey('EnglishWord', on_delete=models.CASCADE, verbose_name="单词")
#
# 	def convertToDict(self):
# 		return self.source
#
