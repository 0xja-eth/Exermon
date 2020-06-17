# -*-coding:GBK -*-

from django.db import models
from django.conf import settings
from game_module.models import GroupConfigure
from item_module.models import *
from question_module.models import BaseQuestion, BaseQuesChoice, GroupQuestion
from utils.model_utils import QuestionAudioUpload, Common as ModelUtils
from utils.exception import ErrorType, GameException
import os, base64, datetime, jsonfield, random
from enum import Enum


# Create your models here.

# region ��Ŀ


# ===================================================
#  Ӣ����Ŀ����ö��
# ===================================================
class QuestionType(Enum):
    Listening = 1  # ������
    Phrase = 2  # ����ʽ��
    Correction = 3  # �Ĵ���


# ===================================================
#  ������Ŀѡ���
# ===================================================
class ListeningQuesChoice(BaseQuesChoice):
    class Meta:
        verbose_name = verbose_name_plural = "������Ŀѡ��"

    # ��������
    question = models.ForeignKey('ListeningSubQuestion', null=False, on_delete=models.CASCADE,
                                 verbose_name="��������")


# ===================================================
#  ����С��
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

    TYPE = QuestionType.Listening

	# �ظ�����
	times = models.PositiveSmallIntegerField(default=2, verbose_name="�ظ�����")

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

		res['times'] = self.times
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
#  �Ķ���Ŀѡ���
# ===================================================
class ReadingQuesChoice(BaseQuesChoice):
    class Meta:
        verbose_name = verbose_name_plural = "�Ķ���Ŀѡ��"

    # ��������
    question = models.ForeignKey('ReadingSubQuestion', null=False, on_delete=models.CASCADE,
                                 verbose_name="��������")


# ===================================================
#  �Ķ�С��
# ===================================================
class ReadingSubQuestion(BaseQuestion):
    class Meta:
        verbose_name = verbose_name_plural = "�Ķ�С��"

    # �Ķ���Ŀ
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
#  ������Ŀ����ö��
# ===================================================
class PhraseType(Enum):
    SB = 1  # [sb. sth. ��ͷ�Ķ���ѡ��]
    Do = 2  # [to do, doing ��ͷ�Ķ���ѡ��]
    Prep = 3  # [��ʶ���ѡ��]


# ===================================================
#  ������
# ===================================================
class PhraseQuestion(models.Model):
    class Meta:
        verbose_name = verbose_name_plural = "������"

    TYPES = [
        (PhraseType.SB.value, '[���� sb. �Ķ���ѡ��]'),
        (PhraseType.Do.value, '[do ��ʽ�Ķ���ѡ��]'),
        (PhraseType.Prep.value, '[��ʶ���ѡ��]'),
    ]

    TYPE = QuestionType.Phrase

    # ����
    word = models.CharField(max_length=64, verbose_name="����")

    # ���ķ���
    chinese = models.CharField(max_length=64, verbose_name="����")

    # ����ʽ��
    phrase = models.CharField(max_length=64, verbose_name="����ʽ��")

    # ����ʽ�������
    type = models.PositiveSmallIntegerField(default=PhraseType.Do.value,
                                            choices=TYPES, verbose_name="�޸�����")

    def convertToDict(self):
        """
        ת��Ϊ�ֵ�
        Returns:
            ����ת������ֵ�
        """
        return {
            'id': self.id,
            'word': self.word,
            'chinese': self.chinese,
            'phrase': self.phrase,
            'type': self.type
        }


# ===================================================
#  �Ĵ���
# ===================================================
class CorrectionQuestion(models.Model):
    class Meta:
        verbose_name = verbose_name_plural = "�Ĵ���"

    TYPE = QuestionType.Correction

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
    word = models.TextField(verbose_name="��ȷ����", null=True, blank=True)

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
		}


# ===================================================
#  ����
# ===================================================
class Word(models.Model):
    class Meta:
        verbose_name = verbose_name_plural = "����"

    # Ӣ��
    english = models.CharField(unique=True, max_length=64, verbose_name="Ӣ��")

    # ����
    chinese = models.CharField(max_length=256, verbose_name="����")

    # ����
    type = models.CharField(max_length=64, verbose_name="����", null=True, blank=True)

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
    word = models.ForeignKey('Word', on_delete=models.CASCADE, verbose_name="����")

    # ��Ӧ����ѵ��¼
    record = models.ForeignKey('ExerProRecord', on_delete=models.CASCADE, verbose_name="��ѵ��¼")

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

    # ��ǰ�ֵ���
    current = models.BooleanField(default=False, verbose_name="�Ƿ��ǵ�ǰ��")

    # ��ǰ���Ƿ���
    current_correct = models.BooleanField(default=None, null=True, verbose_name="��ǰ���Ƿ���")

    # ת��Ϊ�ַ���
    def __str__(self):
        return '%s (%s)' % (self.word, self.record)

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

            'current': self.current,
            'current_correct': self.current_correct,
            # ����ǰ���޷��ж� None����Ҫ����һ�����ӵ��ֶ�
            'current_done': self.current_correct is not None,
        }

    # �����¼�¼
    @classmethod
    def create(cls, pro_record, word_id):
        record = pro_record.wordRecord(word_id)

        if record is None:
            record = cls()
            record.record = pro_record
            record.word_id = word_id

        record.current = True
        record.save()

        return record

    def updateRecord(self, correct):
        """
        �������м�¼
        Args:
            correct (bool): �Ƿ���ȷ
        """
        self.current_correct = correct

        if correct:
            self.correct += 1
        else:
            self.wrong = True

        if self.count <= 0:
            self.first_date = datetime.datetime.now()

        self.last_date = datetime.datetime.now()
        self.count += 1

        self.save()

    def answer(self, chinese):
        """
        ��������
        Args:
            chinese (str): ����
        Returns::
            ���ش��Ƿ���ȷ
        """
        correct = chinese == self.word.chinese
        self.updateRecord(correct)

        return correct

    # ��ȷ��
    def corrRate(self):
        if self.count is None or self.count == 0:
            return 0
        return self.correct / self.count


# endregion

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
	AddMHP = 201  # ���MHP
	AddPower = 202  # �������
	AddDefense = 203  # ��ø�
	AddAgile = 204  # �������
	AddParamUrgent = 205  # ��������ֵ��������ť��

	TempAddParam = 210  # ��ʱ��������ֵ
	TempAddMHP = 211  # ��ʱ���MHP
	TempAddPower = 212  # ��ʱ�������
	TempAddDefense = 213  # ��ʱ��ø�
	TempAddAgile = 214  # ��ʱ�������

	AddState = 220  # ����״̬
	RemoveState = 221  # �Ƴ�״̬
	RemoveNegaState = 222  # �Ƴ�����״̬

	AddEnergy = 230  # �ظ�����

	DrawCards = 300  # ��ȡ����
	ConsumeCards = 310  # ���Ŀ���

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
		(ExerProEffectCode.AddMHP.value, '���MHP'),
		(ExerProEffectCode.AddPower.value, '�������'),
		(ExerProEffectCode.AddDefense.value, '��ø�'),
		(ExerProEffectCode.AddAgile.value, '�������'),
		(ExerProEffectCode.AddParamUrgent.value, '��������ֵ��������ť��'),

		(ExerProEffectCode.TempAddParam.value, '��ʱ��������ֵ'),
		(ExerProEffectCode.TempAddMHP.value, '��ʱ���MHP'),
		(ExerProEffectCode.TempAddPower.value, '��ʱ�������'),
		(ExerProEffectCode.TempAddDefense.value, '��ʱ��ø�'),
		(ExerProEffectCode.TempAddAgile.value, '��ʱ�������'),

		(ExerProEffectCode.AddState.value, '����״̬'),
		(ExerProEffectCode.RemoveState.value, '�Ƴ�״̬'),
		(ExerProEffectCode.RemoveNegaState.value, '�Ƴ�����״̬'),

		(ExerProEffectCode.AddEnergy.value, '�ظ�����'),

		(ExerProEffectCode.DrawCards.value, '��ȡ����'),
		(ExerProEffectCode.ConsumeCards.value, '���Ŀ���'),

        (ExerProEffectCode.ChangeCost.value, '���ĺ���'),
        (ExerProEffectCode.ChangeCostDisc.value, '���ĺ��ܣ����֣�'),
        (ExerProEffectCode.ChangeCostCrazy.value, '���ĺ��ܣ����'),

		(ExerProEffectCode.PlotAddMoney.value, '��ý��'),
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
#  ��ѵ��Ʒ�Ǽ���
# ===================================================
class ExerProItemStar(GroupConfigure):
    class Meta:
        verbose_name = verbose_name_plural = "��ѵ��Ʒ�Ǽ�"

    # �Ǽ���ɫ��#ABCDEF��
    color = models.CharField(max_length=7, null=False, default='#000000', verbose_name="�Ǽ���ɫ")

    def __str__(self):
        return self.name

    # ���������ã���ʾ�Ǽ���ɫ
    def adminColor(self):
        from django.utils.html import format_html

        res = '<div style="background: %s; width: 48px; height: 24px;"></div>' % self.color

        return format_html(res)

    adminColor.short_description = "�Ǽ���ɫ"

    def convertToDict(self):
        return {
            'id': self.id,
            'name': self.name,
            'color': self.color,
        }


# ===================================================
#  ������ѵ��Ʒ��
# ===================================================
class BaseExerProItem(BaseItem):
    class Meta:
        abstract = True
        verbose_name = verbose_name_plural = "��ѵ��Ʒ"

	# ͼ������
	icon_index = models.PositiveSmallIntegerField(default=0, verbose_name="ͼ������")

	# ��Ʒ�Ǽ���ϡ���ȣ�
	star = models.ForeignKey("ExerProItemStar", on_delete=models.CASCADE, verbose_name="�Ǽ�")

    # ��ң�0��ʾ���ɹ���
    gold = models.PositiveSmallIntegerField(default=0, verbose_name="���")

    def convertToDict(self, **kwargs):
        """
        ת��Ϊ�ֵ�
        Returns:
            ����ת������ֵ�
        """
        res = super().convertToDict(**kwargs)

        effects = ModelUtils.objectsToDict(self.effects())

        res['star_id'] = self.star_id
        res['gold'] = self.gold
        res['effects'] = effects

        return res

    def effects(self):
        raise NotImplementedError


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
class ExerProItem(BaseExerProItem):
    class Meta:
        verbose_name = verbose_name_plural = "��ѵ��Ʒ"

    # ��������
    TYPE = ItemType.ExerProItem

    def effects(self):
        return self.exerproitemeffect_set.all()


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
class ExerProPotion(BaseExerProItem):
    class Meta:
        verbose_name = verbose_name_plural = "��ѵҩˮ"

    # ��������
    TYPE = ItemType.ExerProPotion

    def convertToDict(self):
        """
        ת��Ϊ�ֵ�
        Returns:
            ����ת������ֵ�
        """
        res = super().convertToDict()

        return res

    def effects(self):
        return self.exerpropotioneffect_set.all()


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
#  ����ʱ�
# ===================================================
class Antonym(GroupConfigure):
    class Meta:
        verbose_name = verbose_name_plural = "�����"

    # ���ƴ�
    card_word = models.CharField(max_length=32, verbose_name="���ƴ�")

    # ���˴�
    enemy_word = models.CharField(max_length=32, verbose_name="���˴�")

    # �˺����ʣ�*100��
    hurt_rate = models.SmallIntegerField(default=100, verbose_name="�˺�����")

    def convertToDict(self):
        """
        ת��Ϊ�ֵ�
        Returns:
            ����ת������ֵ�
        """
        return {
            'card_word': self.card_word,
            'enemy_word': self.enemy_word,
            'hurt_rate': self.hurt_rate / 100,
        }


# ===================================================
#  ��Ƭ����ö��
# ===================================================
class ExerProCardType(Enum):
    Attack = 1  # ����
    Skill = 2  # ����
    Ability = 3  # ����
    Evil = 4  # ����


# ===================================================
#  ��Ƭ��Ŀ���
# ===================================================
class ExerProCardTarget(Enum):
    Default = 0  # Ĭ��
    One = 1  # ����
    All = 2  # Ⱥ��


# ===================================================
#  ��ѵ��Ƭ��
# ===================================================
class ExerProCard(BaseExerProItem):
    class Meta:
        verbose_name = verbose_name_plural = "��ѵ��Ƭ"

    # ��������
    TYPE = ItemType.ExerProCard

    CARD_TYPES = [
        (ExerProCardType.Attack.value, '����'),
        (ExerProCardType.Skill.value, '����'),
        (ExerProCardType.Ability.value, '����'),
        (ExerProCardType.Evil.value, '����'),
    ]

    TARGETS = [
        (ExerProCardTarget.Default.value, 'Ĭ��'),
        (ExerProCardTarget.One.value, '����'),
        (ExerProCardTarget.All.value, 'Ⱥ��'),
    ]

    # ��������
    cost = models.PositiveSmallIntegerField(default=1, verbose_name="��������")

    # ��Ƭ����
    card_type = models.PositiveSmallIntegerField(default=ExerProCardType.Attack.value,
                                                 choices=CARD_TYPES, verbose_name="��Ƭ����")

    # ����
    inherent = models.BooleanField(default=False, verbose_name="����")

    # ���ģ�һ���Եģ�
    disposable = models.BooleanField(default=False, verbose_name="����")

    # ����
    character = models.CharField(default="", blank=True, max_length=32, verbose_name="����")

    # Ŀ��
    target = models.PositiveSmallIntegerField(default=ExerProCardTarget.Default.value,
                                              choices=TARGETS, verbose_name="Ŀ��")

    def convertToDict(self):
        """
        ת��Ϊ�ֵ�
        Returns:
            ����ת������ֵ�
        """
        res = super().convertToDict()

        res['cost'] = self.cost
        res['card_type'] = self.card_type
        res['inherent'] = self.inherent
        res['disposable'] = self.disposable
        res['character'] = self.character
        res['target'] = self.target

        return res

    def effects(self):
        return self.exerprocardeffect_set.all()


# ===================================================
#  ��ѵ���˹���Ч����
# ===================================================
class EnemyEffect(ExerProEffect):
    class Meta:
        verbose_name = verbose_name_plural = "��ѵ���˹���Ч��"

    # ����
    enemy = models.ForeignKey('ExerProEnemy', on_delete=models.CASCADE,
                              verbose_name="����")


# ===================================================
#  �����ж�����ö��
# ===================================================
class EnemyActionType(Enum):
	Attack = 1  # ����
	PowerUp = 2  # ��������
	PosStates = 3  # ״̬����
	PowerDown = 4  # ��������
	NegStates = 5  # ״̬����
	Escape = 6  # ����
	Unset = 7  # ʲô������


# ===================================================
#  �����ж���
# ===================================================
class EnemyAction(models.Model):
	class Meta:
		verbose_name = verbose_name_plural = "�����ж�"

	TYPES = [
		(EnemyActionType.Attack.value, '����'),
		(EnemyActionType.PowerUp.value, '��������'),
		(EnemyActionType.PosStates.value, '״̬����'),
		(EnemyActionType.PowerDown.value, '��������'),
		(EnemyActionType.NegStates.value, '״̬����'),
		(EnemyActionType.Escape.value, '����'),
		(EnemyActionType.Unset.value, '��'),
	]

	# �غ�
	rounds = jsonfield.JSONField(default=[], verbose_name="�غ�")

	# ����
	type = models.PositiveSmallIntegerField(default=EnemyActionType.Unset.value,
											choices=TYPES, verbose_name="����")

	# ����
	params = jsonfield.JSONField(default=[], verbose_name="����")

	# Ȩ��
	rate = models.PositiveSmallIntegerField(default=10, verbose_name="Ȩ��")

	# ����
	enemy = models.ForeignKey("ExerProEnemy", on_delete=models.CASCADE, verbose_name="����")

	def convertToDict(self):
		"""
		ת��Ϊ�ֵ�
		Returns:
			����ת������ֵ�
		"""
		return {
			'rounds': self.rounds,
			'type': self.type,
			'params': self.params,
			'rate': self.rate,
		}


# ===================================================
#  ���˵ȼ�ö��
# ===================================================
class ExerProEnemyType(Enum):
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

    ENEMY_TYPES = [
        (ExerProEnemyType.Normal.value, '��ͨ'),
        (ExerProEnemyType.Elite.value, '��Ӣ'),
        (ExerProEnemyType.Boss.value, 'BOSS'),
    ]

    # �ȼ�
    type = models.PositiveSmallIntegerField(default=ExerProEnemyType.Normal.value,
                                            choices=ENEMY_TYPES, verbose_name="�ȼ�")

    # �������ֵ
    mhp = models.PositiveSmallIntegerField(default=100, verbose_name="�������ֵ")

    # ����
    power = models.PositiveSmallIntegerField(default=10, verbose_name="����")

    # ��
    defense = models.PositiveSmallIntegerField(default=10, verbose_name="��")

    # ��
    character = models.CharField(default="", blank=True, max_length=32, verbose_name="�Ը�")

    def convertToDict(self):
        """
        ת��Ϊ�ֵ�
        Returns:
            ����ת������ֵ�
        """
        res = super().convertToDict()

        actions = ModelUtils.objectsToDict(self.actions())
        effects = ModelUtils.objectsToDict(self.effects())

		res['mhp'] = self.mhp
		res['power'] = self.power
		res['mhp'] = self.mhp
		res['power'] = self.power
		res['defense'] = self.defense
		res['character'] = self.character
		res['type_'] = self.type

        res['actions'] = actions
        res['effects'] = effects

        return res

    def actions(self):
        """
        ��ȡ���˵��ж��ƻ�
        Returns:
            ���ص����ж��ƻ�
        """
        return self.enemyaction_set.all()

    def effects(self):
        """
        ��ȡ���˵Ĺ���Ч��
        Returns:
            ���ص��˹���Ч��
        """
        return self.enemyeffect_set.all()


# ===================================================
#  ��ѵ״̬��
# ===================================================
class ExerProState(BaseItem):
	class Meta:
		verbose_name = verbose_name_plural = "��ѵ״̬"

	# ��������
	TYPE = ItemType.ExerProState

	# ���״̬�غ���
	max_turns = models.PositiveSmallIntegerField(default=0, verbose_name="���״̬�غ���")

	# �Ƿ���״̬
	is_nega = models.BooleanField(default=False, verbose_name="�Ƿ���״̬")

	def convertToDict(self):
		"""
		ת��Ϊ�ֵ�
		Returns:
			����ת������ֵ�
		"""
		res = super().convertToDict()

		res['max_turns'] = self.max_turns
		res['is_nega'] = self.is_nega

		return res


# endregion

# region ��ͼ


# ===================================================
#  �ݵ����ͱ�
# ===================================================
class NodeType(GroupConfigure):
    class Meta:
        verbose_name = verbose_name_plural = "�ݵ�����"

    # ����
    ques_types = models.CharField(max_length=32, verbose_name="����")

    def convertToDict(self):
        """
        ת��Ϊ�ֵ�
        Returns:
            ����ת������ֵ�
        """
        res = super().convertToDict()

        res['ques_types'] = self.ques_types

        return res


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

    def stage(self, order) -> 'ExerProMapStage':
        """
        ��ȡָ����ŵĹؿ�
        Args:
            order (int): �ؿ����
        Returns:
            ����ָ����ŵĹؿ�����
        """
        stage = self.stages().filter(order=order)

        if stage.exists(): return stage.first()

        return None


# # ===================================================
# #  �ݵ����ͱ�
# # ===================================================
# class NodeType(Enum):
# 	Rest = 0  # ��Ϣ�ݵ�
# 	Treasure = 1  # �ر��ݵ�
# 	Shop = 2  # ���˾ݵ�
# 	Enemy = 3  # ���˾ݵ�
# 	Elite = 4  # ��Ӣ�ݵ�
# 	Unknown = 5  # δ֪�ݵ�
# 	Boss = 6  # ��Ӣ�ݵ�


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


# ===================================================
#  ��ѵ��¼��
# ===================================================
class ExerProRecord(CacheableModel):
	class Meta:

		verbose_name = verbose_name_plural = "��ѵ��¼"

	# ��ǰ���ʻ����
	CUR_WORDS_CACHE_KEY = 'cur_words'

	# �ؿ�
	stage = models.ForeignKey('ExerProMapStage', null=True,
							  on_delete=models.CASCADE, verbose_name="�ؿ�")

	# ��ʼ��־
	started = models.BooleanField(default=False, verbose_name="��ʼ��־")

	# ���ɱ�־
	generated = models.BooleanField(default=False, verbose_name="���ɱ�־")

	# ��ǰ�ݵ�����
	cur_index = models.PositiveSmallIntegerField(default=None, null=True, verbose_name="��ǰ�ݵ�����")

	# �Ƿ���ɾݵ��¼�
	node_flag = models.BooleanField(default=False, verbose_name="�Ƿ���ɾݵ��¼�")

	# ���ʵȼ���ͬʱҲ�������Ӣ��ģ��ĵȼ���
	word_level = models.PositiveSmallIntegerField(default=1, verbose_name="���ʵȼ�")

	# # ��һ����
	# next = models.ForeignKey('Word', null=True, blank=True,
	# 						 on_delete=models.CASCADE, verbose_name="��һ����")

	# �ݵ�����
	nodes = jsonfield.JSONField(default=None, null=True, blank=True,
								verbose_name="�ݵ�����")

	# ��ɫ����
	actor = jsonfield.JSONField(default=None, null=True, blank=True,
								verbose_name="��ɫ����")

	# ���
	player = models.OneToOneField('player_module.Player', null=False,
								  on_delete=models.CASCADE, verbose_name="���")

	def __str__(self):
		return "%d. %s ��ѵ��¼" % (self.id, self.player)

	# �����¼�¼
	@classmethod
	def create(cls, player):
		"""
		������ѵ��¼
		Args:
			player (Player): �缲
			wids (list): ����ID����
		"""
		record = cls()
		record.player = player
		record.save()

		record._generateWordRecords()

		return record

	def convertToDict(self, type: str = None, **kwargs):
		"""
		ת��Ϊ�ֵ�
		Args:
			type (str): ����
			**kwargs (**dict): ��չ����
		Returns:
			����ת������ֵ�
		"""

		if type == "records":
			return ModelUtils.objectsToDict(self.wordRecords())

		word_records = self.currentWordRecords()
		words = [record.word for record in word_records]

		if type == "words":

			return {
				'word_level': self.word_level,
				'words': ModelUtils.objectsToDict(words),
				'word_records': ModelUtils.objectsToDict(word_records),
			}

		# if type == "status":
		# 	records = self.currentWordRecords()
		#
		# 	corr_recs = [record for record in records
		# 				 if record.current_correct is True]
		# 	wrong_recs = [record for record in records
		# 				  if record.current_correct is False]
		#
		# 	sum = len(records)
		# 	correct = len(corr_recs)
		# 	wrong = len(wrong_recs)
		#
		# 	return {
		# 		'level': self.word_level,
		# 		'sum': sum,
		# 		'correct': correct,
		# 		'wrong': wrong
		# 	}

		cur_index = self.cur_index
		if cur_index is None: cur_index = -1

		return {
			'id': self.id,
			'map_id': self.stage.map_id,
			'stage_order': self.stage.order,
			'started': self.started,
			'generated': self.generated,
			'cur_index': cur_index,
			'word_level': self.word_level,

			'nodes': self.nodes,
			'actor': self.actor,

			'words': ModelUtils.objectsToDict(words),
			'word_records': ModelUtils.objectsToDict(word_records),
		}

	def loadFromDict(self, data: dict):
		"""
		���ֵ��ж�ȡ
		Args:
			data (dict): �ֵ�
		"""
		from .views import Common

		map_id = ModelUtils.loadKey(data, 'map_id')
		stage_order = ModelUtils.loadKey(data, 'stage_order')

		self.stage = Common.getMapStage(mid=map_id, order=stage_order)

		ModelUtils.loadKey(data, 'started', self)
		ModelUtils.loadKey(data, 'generated', self)
		ModelUtils.loadKey(data, 'cur_index', self)
		ModelUtils.loadKey(data, 'nodes', self)
		ModelUtils.loadKey(data, 'actor', self)

	# region ���̿���

	def setupMap(self, map: 'ExerProMap'):
		"""
		���õ�ͼ
		Args:
			map (ExerProMap): ��ͼ
		"""
		self.reset()
		self.stage = map.stage(1)
		self.started = True

	def reset(self):
		"""
		���ã�������ѵ��¼״̬
		"""
		self.started = self.generated = False
		self.cur_index = self.nodes = self.actor = None

	def upgrade(self):
		"""
		��������
		"""
		# ��ʼ״̬��Ϊ None
		if self.word_level is None:
			self.word_level = 1
		else:
			self.word_level += 1

		self._generateWordRecords()
		self.save()

	def _generateWordRecords(self):
		"""
		���ɵ��ʺͼ�¼
		"""
		word_recs = self.wordRecords()

		wids = self._generateWords()
		self.clearCurrentWords()

		for wid in wids:
			word_rec = WordRecord.create(self, wid)
			word_recs.append(word_rec)

	def _generateWords(self) -> list:
		"""
		���ɵ���
		Returns:
			�������ɵ���ID����
		"""
		from utils.calc_utils import NewWordsGenerator

		# ��ȡ�ɵ��ʵ�ID����
		old_words = self.currentWordRecords()
		old_words = [record.word_id for record in old_words]

		return NewWordsGenerator.generate(self.word_level, old_words)

	def terminate(self):
		"""
		������ѵ
		"""
		self.started = self.generated = False
		self.save()

	# endregion

	# region ���ʼ�¼����

	def isFinished(self):
		"""
		���ֵ����Ƿ����
		Returns:
			���ر��ֵ����Ƿ����
		"""
		if self.word_level is None: return True

		word_recs = self.currentWordRecords()

		for word_rec in word_recs:
			if not word_rec.current_correct: return False

		return True

	def nextWord(self) -> WordRecord:
		"""
		������һ������
		Returns:
			������һ�����ʼ�¼
		"""
		word_recs = self.currentWordRecords()
		word_recs = [word_rec for word_rec in word_recs
					 if word_rec.current_correct is None]

		if len(word_recs) <= 0: return None

		return random.choice(word_recs)

	def clearCurrentWords(self):
		"""
		�����ǰ����
		"""
		word_recs = self.currentWordRecords()

		for word_rec in word_recs:
			word_rec.current = False
			word_rec.current_correct = None

	def wordRecords(self):
		"""
		ȫ�����ʼ�¼�����棩
		Returns:
			���ػ����ȫ�����ʼ�¼�б�
		"""
		return self._getOrSetCache(self.CUR_WORDS_CACHE_KEY,
								   lambda: list(self._wordRecords()))

	def _wordRecords(self):
		"""
		ȫ�����ʼ�¼�����ݿ⣩
		Returns:
			����ȫ�����ʼ�¼�б�
		"""
		return self.wordrecord_set.all()

	def currentWordRecords(self):
		"""
		��ǰ���ʼ�¼
		Returns:
			���ص�ǰ���ʼ�¼�б�
		"""
		return ModelUtils.query(self.wordRecords(), current=True)

	def wordRecord(self, word_id: int, **kwargs) -> 'WordRecord':
		"""
		ͨ������ID���ҵ��ʼ�¼
		Args:
			word_id (int): ����ID
			**kwargs (**dict): ������ѯ����
		Returns:
			�����ڵ��ʼ�¼������֮�����򷵻� None
		"""
		return ModelUtils.get(self.wordRecords(), word_id=word_id, **kwargs)

# endregion
