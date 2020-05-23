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
#  ��Ŀѡ���
# ===================================================
class ListeningQuesChoice(BaseQuesChoice):
	class Meta:
		verbose_name = verbose_name_plural = "������Ŀѡ��"

	# ��������
	question = models.ForeignKey('ListeningSubQuestion', null=False, on_delete=models.CASCADE,
								 verbose_name="��������")


# ===================================================
#  ������
# ===================================================
class ListeningSubQuestion(BaseQuestion):

	class Meta:
		verbose_name = verbose_name_plural = "����С��"

	# ������Ŀ
	question = models.ForeignKey('ListeningQuestion', on_delete=models.CASCADE,
								 verbose_name="������Ŀ")

	def choices(self):
		return self.listeningqueschoice_set.all()


# ===================================================
#  ������
# ===================================================
class ListeningQuestion(GroupQuestion):

	class Meta:
		verbose_name = verbose_name_plural = "������"

	# ��Ƶ�ļ�
	audio = models.FileField(upload_to=QuestionAudioUpload(), verbose_name="��Ƶ�ļ�")

	# ��ȡ����·��
	def getExactlyPath(self):
		base = settings.STATIC_URL
		path = os.path.join(base, str(self.audio))
		if os.path.exists(path):
			return path
		else:
			raise GameException(ErrorType.PictureFileNotFound)

	# ��ȡ��Ƶbase64����
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
		����Ŀ
		Returns:
			���ظ�������Ŀ������Ŀ
		"""
		return self.listeningsubquestion_set.all()


# ===================================================
#  ��Ŀѡ���
# ===================================================
class ReadingQuesChoice(BaseQuesChoice):
	class Meta:
		verbose_name = verbose_name_plural = "�Ķ���Ŀѡ��"

	# ��������
	question = models.ForeignKey('ReadingSubQuestion', null=False, on_delete=models.CASCADE,
								 verbose_name="��������")


# ===================================================
#  ������
# ===================================================
class ReadingSubQuestion(BaseQuestion):

	class Meta:
		verbose_name = verbose_name_plural = "�Ķ�С��"

	# ������Ŀ
	question = models.ForeignKey('ReadingQuestion', on_delete=models.CASCADE,
								 verbose_name="�Ķ���Ŀ")

	def choices(self):
		return self.readingqueschoice_set.all()


# ===================================================
#  �Ķ���
# ===================================================
class ReadingQuestion(GroupQuestion):

	class Meta:
		verbose_name = verbose_name_plural = "�Ķ���"

	def subQuestions(self) -> QuerySet:
		"""
		����Ŀ
		Returns:
			���ظ�������Ŀ������Ŀ
		"""
		return self.readingsubquestion_set.all()


# ===================================================
#  �Ĵ���
# ===================================================
class CorrectionQuestion(models.Model):

	class Meta:
		verbose_name = verbose_name_plural = "�Ĵ���"

	# ����
	article = models.TextField(verbose_name="����")

	# ����
	description = models.TextField(null=True, blank=True, verbose_name="����")

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
		������
		Returns:
			���ظøĴ���Ŀ�Ĵ�����
		"""
		return self.wrongitem_set.all()


# ===================================================
#  ��������
# ===================================================
class CorrectType(Enum):

	Add = 1  # ����
	Edit = 2  # �޸�
	Delete = 3  # ɾ��


# ===================================================
#  �Ĵ��������
# ===================================================
class WrongItem(models.Model):
	class Meta:
		verbose_name = verbose_name_plural = "�Ĵ��������"

	TYPES = [
		(CorrectType.Add.value, '����'),
		(CorrectType.Edit.value, '�޸�'),
		(CorrectType.Delete.value, 'ɾ��'),
	]

	# ���ӱ��
	sentence_index = models.PositiveSmallIntegerField(verbose_name="���ӱ��")

	# ���ʱ��
	word_index = models.PositiveSmallIntegerField(verbose_name="���ʱ��")

	# �޸�����
	type = models.PositiveSmallIntegerField(default=CorrectType.Edit.value,
		choices=TYPES, verbose_name="�޸�����")

	# ��ȷ����
	word = models.TextField(verbose_name="��ȷ����")

	# ��Ӧ��Ŀ
	question = models.ForeignKey('CorrectionQuestion', on_delete=models.CASCADE,
								 verbose_name="�Ĵ���Ŀ")

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
#  ����
# ===================================================
class Word(models.Model):

	class Meta:
		verbose_name = verbose_name_plural = "����"

	# Ӣ��
	english = models.CharField(max_length=64, verbose_name="Ӣ��")

	# ����
	chinese = models.CharField(max_length=64, verbose_name="����")

	# ����
	type = models.CharField(max_length=32, verbose_name="����")

	# �ȼ�
	level = models.PositiveSmallIntegerField(default=1, verbose_name="�ȼ�")

	# �Ƿ������Ŀ
	is_middle = models.BooleanField(default=True, verbose_name="�Ƿ������Ŀ")

	# �Ƿ������Ŀ
	is_high = models.BooleanField(default=True, verbose_name="�Ƿ������Ŀ")

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
#  ���ʼ�¼��
# ===================================================
class WordRecord(models.Model):

	class Meta:

		verbose_name = verbose_name_plural = "���ʼ�¼"

	# ��Ŀ
	word = models.ForeignKey('Word', null=False,
							 on_delete=models.CASCADE, verbose_name="����")

	# ���
	player = models.ForeignKey('player_module.Player', null=False,
							   on_delete=models.CASCADE, verbose_name="���")

	# �ش����
	count = models.PositiveSmallIntegerField(default=0, verbose_name="�ش����")

	# ��ȷ����
	correct = models.PositiveSmallIntegerField(default=0, verbose_name="��ȷ��")

	# �ϴλش�����
	last_date = models.DateTimeField(null=True, verbose_name="�ϴλش�����")

	# ���λش�����
	first_date = models.DateTimeField(null=True, verbose_name="���λش�����")

	# �ղر�־
	collected = models.BooleanField(default=False, verbose_name="�ղر�־")

	# �����־
	wrong = models.BooleanField(default=False, verbose_name="�����־")

	# ת��Ϊ�ַ���
	def __str__(self):
		return '%s (%s)' % (self.word, self.player)

	# ת��Ϊ�ֵ�
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

	# �����¼�¼
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
		�������м�¼
		Args:
			correct (bool): �Ƿ���ȷ
		"""

		if correct: self.correct += 1
		else: self.wrong = True

		if self.count <= 0:
			self.first_date = datetime.datetime.now()

		self.last_date = datetime.datetime.now()
		self.count += 1

		self.save()

	# ��ȷ��
	def corrRate(self):
		if self.count is None or self.count == 0:
			return 0
		return self.correct / self.count


# # ===================================================
# #  Ӣ�ﵥ����Դö��
# # ===================================================
# class EnglishWordSourceType(Enum):
#
# 	MiddleSchool = 1  # ����
# 	HighSchool = 2  # ����
# 	CET4 = 3  # �ļ�
# 	CET6 = 4  # ����
# 	Postgraduate = 5  # ����
#
# 	Unknown = 0  # δ֪
#
#
# # ===================================================
# #  Ӣ�ﵥ����Դ��
# # ===================================================
# class EnglishWordSource(models.Model):
#
# 	class Meta:
# 		verbose_name = verbose_name_plural = "Ӣ�ﵥ����Դ"
#
# 	TYPES = [
# 		(EnglishWordSourceType.MiddleSchool.value, '����'),
# 		(EnglishWordSourceType.HighSchool.value, '����'),
# 		(EnglishWordSourceType.CET4.value, '�ļ�'),
# 		(EnglishWordSourceType.CET6.value, '����'),
# 		(EnglishWordSourceType.Postgraduate.value, '����'),
#
# 		(EnglishWordSourceType.Unknown.value, 'δ֪'),
# 	]
#
# 	# ��Դ
# 	source = models.PositiveSmallIntegerField(default=EnglishWordSourceType.Unknown.value,
# 											choices=TYPES, verbose_name="��Դ")
#
# 	# ����
# 	word = models.ForeignKey('EnglishWord', on_delete=models.CASCADE, verbose_name="����")
#
# 	def convertToDict(self):
# 		return self.source
#
