from django.db import models

from ..manager import QuesManager
from ..models import *

# import question_module.models as Models

from enum import Enum


# ===================================================
#  单词
# ===================================================
@QuesManager.registerQuestion("单词")
class Word(ElementQuestion):

	# 标准用时（秒）
	STD_TIME = 15

	# 英文
	english = models.CharField(max_length=64, verbose_name="英文")

	# 中文
	chinese = models.CharField(max_length=256, verbose_name="中文")
	chinese.type_filter = ['answer']

	# 词性
	type = models.CharField(max_length=64, verbose_name="词性", null=True, blank=True)

	# 等级（出现该单词的最低等级）
	level = models.PositiveSmallIntegerField(default=1, verbose_name="等级")

	def __str__(self):
		return "%d. %s" % (self.id, self.english)

	def title(self, dictation=False):
		return self.chinese if dictation else self.english

	def answer(self, dictation=False):
		return self.english if dictation else self.chinese


# ===================================================
#  单词记录表
# ===================================================
@QuesManager.registerQuesRecord(Word)
class WordRecord(BaseQuesRecord):

	# 当前轮单词
	current = models.BooleanField(default=False, verbose_name="是否是当前轮")

	# 当前轮是否答对
	current_correct = models.BooleanField(default=None, null=True, verbose_name="当前轮是否答对")

	# 转化为字符串
	def __str__(self):
		return '%s (%s)' % (self.word, self.record)

	def _convertCustomAttrs(self, res, type=None, **kwargs):
		super()._convertCustomAttrs(res, type, **kwargs)

		res['current_done'] = self.current_correct is not None

	def _create(self):
		super(WordRecord, self)._create()

		self.current = True


# ===================================================
#  单词记录表
# ===================================================
@QuesManager.registerQuesReport(Word)
class WordReport(BaseQuesReport): pass


# ===================================================
#  短语题目类型枚举
# ===================================================
class PhraseType(Enum):
	SB = 1  # [sb. sth. 开头的短语选项]
	Do = 2  # [to do, doing 开头的短语选项]
	Prep = 3  # [介词短语选项]


# ===================================================
#  短语
# ===================================================
@QuesManager.registerQuestion("短语")
class Phrase(ElementQuestion):

	PHRASE_TYPES = [
		(PhraseType.SB.value, '包含 sb. 的短语选项'),
		(PhraseType.Do.value, 'do 形式的短语选项'),
		(PhraseType.Prep.value, '介词短语选项'),
	]

	# 标准用时（秒）
	STD_TIME = 30

	# 单词
	word = models.CharField(max_length=64, verbose_name="单词")

	# 中文翻译
	chinese = models.CharField(max_length=64, verbose_name="中文")

	# 不定式项
	phrase = models.CharField(max_length=64, verbose_name="不定式项")
	phrase.type_filter = ['answer']

	# 不定式项的类型
	type = models.PositiveSmallIntegerField(default=PhraseType.Do.value,
											choices=PHRASE_TYPES, verbose_name="修改类型")

	def title(self):
		return [self.word, self.chinese]

	def answer(self): return self.phrase


# ===================================================
#  单词记录表
# ===================================================
@QuesManager.registerQuesRecord(Phrase)
class PhraseRecord(BaseQuesRecord): pass


# ===================================================
#  单词记录表
# ===================================================
@QuesManager.registerQuesReport(Phrase)
class PhraseReport(BaseQuesReport): pass
