#-*-coding:GBK -*-

from django.db import models
from django.conf import settings
from item_module.models import *
from question_module.models import BaseQuestion, BaseQuesChoice, GroupQuestion
from utils.model_utils import QuestionAudioUpload, Common as ModelUtils
from utils.exception import ErrorType, GameException
import os, base64, datetime, jsonfield
from enum import Enum

# Create your models here.

# region ��Ŀ


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

	# ����
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

# endregion

# region ��Ʒ


# ===================================================
#  ʹ��Ч�����ö��
# ===================================================
class ExerProEffectCode(Enum):
	Unset = 0  # ��

	Attack = 1  # ����˺�
	AttackSlash = 2  # ����˺�������ն����
	AttackBlack = 3  # ����˺��������磩
	AttackWave = 4  # ����˺�������ȭ��
	AttackRite = 5  # ����˺�����ʽذ�ף�

	Recover = 100  # �ظ�����ֵ

	AddParam = 200  # ��������ֵ
	AddParamUrgent = 201  # ��������ֵ��������ť��
	TempAddParam = 210  # ��ʱ��������ֵ
	AddStatus = 220  # ����״̬

	GetCards = 300  # ��ȡ����
	RemoveCards = 310  # �Ƴ�����

	ChangeCost = 400  # ���ĺ���
	ChangeCostDisc = 401  # ���ĺ��ܣ����֣�
	ChangeCostCrazy = 402  # ���ĺ��ܣ����

	Sadistic = 500  # ��Ű����
	ForceAddStatus = 600  # ���Ӽ���״̬


# ===================================================
#  ��ѵʹ��Ч����
# ===================================================
class ExerProEffect(models.Model):

	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "��ѵʹ��Ч��"

	CODES = [
		(ExerProEffectCode.Unset.value, '��'),

		(ExerProEffectCode.Attack.value, '����˺�'),
		(ExerProEffectCode.AttackSlash.value, '����˺�������ն����'),
		(ExerProEffectCode.AttackBlack.value, '����˺��������磩'),
		(ExerProEffectCode.AttackWave.value, '����˺�������ȭ��'),
		(ExerProEffectCode.AttackRite.value, '����˺�����ʽذ�ף�'),

		(ExerProEffectCode.Recover.value, '�ظ�����ֵ'),

		(ExerProEffectCode.AddParam.value, '��������ֵ'),
		(ExerProEffectCode.AddParamUrgent.value, '��������ֵ��������ť��'),
		(ExerProEffectCode.TempAddParam.value, '��ʱ��������ֵ'),
		(ExerProEffectCode.AddStatus.value, '����״̬'),

		(ExerProEffectCode.GetCards.value, '��ȡ����'),
		(ExerProEffectCode.RemoveCards.value, '�Ƴ�����'),

		(ExerProEffectCode.ChangeCost.value, '���ĺ���'),
		(ExerProEffectCode.ChangeCostDisc.value, '���ĺ��ܣ����֣�'),
		(ExerProEffectCode.ChangeCostCrazy.value, '���ĺ��ܣ����'),

		(ExerProEffectCode.Sadistic.value, '��Ű����'),
		(ExerProEffectCode.ForceAddStatus.value, '���Ӽ���״̬'),
	]

	# Ч�����
	code = models.PositiveSmallIntegerField(default=0, choices=CODES, verbose_name="Ч�����")

	# Ч������
	params = jsonfield.JSONField(default=[], verbose_name="Ч������")

	# ת��Ϊ�ֵ�
	def convertToDict(self):

		return {
			'code': self.code,
			'params': self.params,
		}


# ===================================================
#  ��ѵ��Ʒʹ��Ч����
# ===================================================
class ExerProItemEffect(ExerProEffect):

	class Meta:
		verbose_name = verbose_name_plural = "��ѵ��Ʒʹ��Ч��"

	# ��Ʒ
	item = models.ForeignKey('ExerProItem', on_delete=models.CASCADE,
							 verbose_name="��Ʒ")


# ===================================================
#  ��ѵ��Ʒ��
# ===================================================
class ExerProItem(BaseItem):

	class Meta:

		verbose_name = verbose_name_plural = "��ѵ��Ʒ"

	# ��������
	TYPE = ItemType.ExerProItem


# ===================================================
#  ��ѵҩˮʹ��Ч����
# ===================================================
class ExerProPotionEffect(ExerProEffect):

	class Meta:
		verbose_name = verbose_name_plural = "��ѵҩˮʹ��Ч��"

	# ��Ʒ
	item = models.ForeignKey('ExerProPotion', on_delete=models.CASCADE,
							 verbose_name="��Ʒ")


# ===================================================
#  ��ѵҩˮ��
# ===================================================
class ExerProPotion(BaseItem):

	class Meta:

		verbose_name = verbose_name_plural = "��ѵ��Ʒ"

	# ��������
	TYPE = ItemType.ExerProPotion

	# HP�ظ�����
	hp_recover = models.SmallIntegerField(default=0, verbose_name="HP�ظ�����")

	# HP�ظ��ʣ�*100��
	hp_rate = models.IntegerField(default=0, verbose_name="HP�ظ���")

	# ������������
	power_add = models.SmallIntegerField(default=0, verbose_name="������������")

	# ���������ʣ�*100���������ĸ��ʣ���������Ҫ+1��
	power_rate = models.IntegerField(default=0, verbose_name="����������")

	def convertToDict(self):
		"""
		ת��Ϊ�ֵ�
		Returns:
			����ת������ֵ�
		"""
		res = super().convertToDict()

		res['hp_recover'] = self.hp_recover
		res['hp_rate'] = self.hp_rate / 100
		res['power_add'] = self.power_add
		res['power_rate'] = self.power_rate / 100

		return res


# ===================================================
#  ��ѵ��Ƭʹ��Ч����
# ===================================================
class ExerProCardEffect(ExerProEffect):

	class Meta:
		verbose_name = verbose_name_plural = "��ѵ��Ƭʹ��Ч��"

	# ��Ʒ
	item = models.ForeignKey('ExerProCard', on_delete=models.CASCADE,
							 verbose_name="��Ʒ")


# ===================================================
#  ��Ƭ����ö��
# ===================================================
class ExerProCardType(Enum):

	Normal = 1  # ��ͨ
	Evil = 2  # ����


# ===================================================
#  ��ѵ��Ƭ��
# ===================================================
class ExerProCard(BaseItem):

	class Meta:

		verbose_name = verbose_name_plural = "��ѵ��Ƭ"

	# ��������
	TYPE = ItemType.ExerProCard

	CARD_TYPES = [
		(ExerProCardType.Normal.value, '��ͨ'),
		(ExerProCardType.Evil.value, '����'),
	]

	# ��������
	cost = models.PositiveSmallIntegerField(default=1, verbose_name="��������")

	# ��Ƭ����
	card_type = models.PositiveSmallIntegerField(
		default=ExerProCardType.Normal.value, choices=CARD_TYPES, verbose_name="��Ƭ����")

	def convertToDict(self):
		"""
		ת��Ϊ�ֵ�
		Returns:
			����ת������ֵ�
		"""
		res = super().convertToDict()

		res['cost'] = self.cost
		res['card_type'] = self.card_type

		return res


# ===================================================
#  ���˵ȼ�ö��
# ===================================================
class ExerProEnemyLevel(Enum):

	Normal = 1  # ��ͨ
	Elite = 2  # ��Ӣ
	Boss = 3  # BOSS


# ===================================================
#  ��ѵ���˱�
# ===================================================
class ExerProEnemy(BaseItem):

	class Meta:

		verbose_name = verbose_name_plural = "��ѵ����"

	# ��������
	TYPE = ItemType.ExerProEnemy

	LEVELS = [
		(ExerProEnemyLevel.Normal.value, '��ͨ'),
		(ExerProEnemyLevel.Elite.value, '��Ӣ'),
		(ExerProEnemyLevel.Boss.value, 'BOSS'),
	]

	# �������ֵ
	mhp = models.PositiveSmallIntegerField(default=100, verbose_name="�������ֵ")

	# ����
	power = models.PositiveSmallIntegerField(default=10, verbose_name="����")

	# �ȼ�
	level = models.PositiveSmallIntegerField(
		default=ExerProEnemyLevel.Normal.value, choices=LEVELS, verbose_name="�ȼ�")

	def convertToDict(self):
		"""
		ת��Ϊ�ֵ�
		Returns:
			����ת������ֵ�
		"""
		res = super().convertToDict()

		res['mhp'] = self.mhp
		res['power'] = self.power
		res['level'] = self.level

		return res


# ===================================================
#  ��ѵ״̬��
# ===================================================
class ExerProStatus(BaseItem):

	class Meta:

		verbose_name = verbose_name_plural = "��ѵ״̬"

	# ��������
	TYPE = ItemType.ExerProStatus


# endregion

# region ��ͼ


# ===================================================
#  ��ͼ��
# ===================================================
class ExerProMap(models.Model):
	class Meta:
		verbose_name = verbose_name_plural = "��ѵ��ͼ"

	# ��ͼ����
	name = models.CharField(max_length=24, verbose_name="��ͼ����")

	# ��������
	description = models.CharField(max_length=512, verbose_name="��������")

	# ��ͼ�Ѷ�
	level = models.PositiveSmallIntegerField(default=1, verbose_name="��ͼ�Ѷ�")

	# �ȼ�Ҫ��
	min_level = models.PositiveSmallIntegerField(default=1, verbose_name="�ȼ�Ҫ��")

	def __str__(self):
		return "%d. %s" % (self.id, self.name)

	def convertToDict(self):
		"""
		ת��Ϊ�ֵ�
		Returns:
			����ת������ֵ�
		"""
		stages = ModelUtils.objectsToDict(self.stages())

		return {
			'id': self.id,
			'name': self.name,
			'description': self.description,
			'level': self.level,
			'min_level': self.min_level,

			'stages': stages
		}

	def stages(self):
		"""
		��ȡ���йؿ�
		Returns:
			���عؿ� QuerySet
		"""
		return self.exerpromapstage_set.all()


# ===================================================
#  �ݵ����ͱ�
# ===================================================
class NodeType(Enum):
	Rest = 0  # ��Ϣ�ݵ�
	Treasure = 1  # �ر��ݵ�
	Shop = 2  # ���˾ݵ�
	Enemy = 3  # ���˾ݵ�
	Elite = 4  # ��Ӣ�ݵ�
	Unknown = 5  # δ֪�ݵ�
	Boss = 6  # ��Ӣ�ݵ�


# ===================================================
#  ��ͼ�ؿ���
# ===================================================
class ExerProMapStage(models.Model):
	class Meta:
		verbose_name = verbose_name_plural = "��ͼ�ؿ�"

	# ���
	order = models.PositiveSmallIntegerField(default=1, verbose_name="���")

	# ��ͼ
	map = models.ForeignKey("english_pro_module.ExerProMap", on_delete=models.CASCADE, verbose_name="��ͼ")

	# ���˼��ϣ����׶λ�ˢ�ĵ��ˣ�
	enemies = models.ManyToManyField("ExerProEnemy", verbose_name="���˼���")

	# ս������������
	max_battle_enemies = models.PositiveSmallIntegerField(default=1, verbose_name="ս������������")

	# ÿ���ݵ���������һ����Ϊ1��
	steps = jsonfield.JSONField(default=[3, 4, 5, 2, 1], verbose_name="ÿ���ݵ����")

	# ���ֲ�ݵ�����
	max_fork_node = models.PositiveSmallIntegerField(default=5, verbose_name="���ֲ�ݵ�����")

	# ���ֲ�ѡ����
	max_fork = models.PositiveSmallIntegerField(default=3, verbose_name="���ֲ�ѡ����")

	# �ݵ������һ��6�־ݵ㣬���ոñ����ļ��ʽ������ɣ�ʵ�ʲ�һ��Ϊ�ñ�����
	node_rate = jsonfield.JSONField(default=[1, 1, 1, 1, 1, 1], verbose_name="�ݵ����")

	def __str__(self):
		return "%s �� %s ��" % (self.map, self.order)

	def convertToDict(self):
		"""
		ת��Ϊ�ֵ�
		Returns:
			����ת������ֵ�
		"""
		enemies = list(e.id for e in self.enemies.all())

		return {
			'order': self.order,
			'max_battle_enemies': self.max_battle_enemies,
			'steps': self.steps,
			'max_fork_node': self.max_fork_node,
			'max_fork': self.max_fork,
			'node_rate': self.node_rate,

			'enemies': enemies,
		}

# endregion

