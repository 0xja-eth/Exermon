from django.db import models

from .question_sets import *

from ..manager import RecordManager

from question_module.models import *

from ..models import *

# import record_module.models as Models

from utils.calc_utils.question_calc import *

import jsonfield


# ===================================================
#  刷题题目关系表
# ===================================================
@RecordManager.registerPlayerQuestion(
	GeneralQuestion, GeneralExerciseRecord,
	RecordSource.Exercise, ExerciseSingleRewardCalc)
class GeneralExerciseQuestion(SelectingPlayerQuestion): pass


# ===================================================
#  刷题题目关系表
# ===================================================
@RecordManager.registerPlayerQuestion(
	ListeningQuestion, ListeningExerciseRecord, RecordSource.Exercise)
class ListeningExerciseQuestion(GroupPlayerQuestion): pass


# ===================================================
#  刷题题目关系表
# ===================================================
@RecordManager.registerPlayerQuestion(
	ReadingQuestion, ReadingExerciseRecord, RecordSource.Exercise)
class ReadingExerciseQuestion(GroupPlayerQuestion): pass


# ===================================================
#  刷题题目关系表
# ===================================================
@RecordManager.registerPlayerQuestion(
	CorrectingQuestion, CollectingExerciseRecord, RecordSource.Exercise)
class CorrectingExerciseQuestion(BasePlayerQuestion):

	# 回答错误项
	wrong_items = jsonfield.JSONField(default=[], verbose_name="回答错误项")

	def _answerDict(self) -> dict:
		return {'wrong_items': self.wrong_items}

	def _processAnswer(self, answer):
		self.wrong_items = answer


# ===================================================
#  单词题目关系表
# ===================================================
@RecordManager.registerPlayerQuestion(
	Word, WordExerciseRecord, RecordSource.Exercise)
class WordExerciseQuestion(ElementPlayerQuestion):

	# 是否默写
	dictation = models.BooleanField(default=False, verbose_name="默写")

	# def _convertBaseInfo(self, res, type):
	# 	super()._convertBaseInfo(res, type)
	#
	# 	res['dictation'] = self.dictation

	def _create(self, dictation=False):
		self.dictation = dictation

		super()._create()

	def _generateChoices(self):
		if self.dictation: return
		# TODO: 生成单词题目
		pass

	def title(self):
		question: Word = self.question

		return question.title(self.dictation)

	def correctAnswer(self):
		question: Word = self.question

		return question.answer(self.dictation)

	def _answerDict(self) -> dict:
		res = super()._answerDict()

		res['dictation'] = self.dictation

		return res


# ===================================================
#  单词题目关系表
# ===================================================
@RecordManager.registerPlayerQuestion(
	Phrase, PhraseExerciseRecord, RecordSource.Exercise)
class PhraseExerciseQuestion(ElementPlayerQuestion):

	def _generateChoices(self):
		# TODO: 生成短语题目
		pass
