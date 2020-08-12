from django.db import models

from .question_sets import *

from ..manager import RecordManager

from question_module.models import *

import record_module.models as Models

from utils.calc_utils.question_calc import *

import jsonfield


# ===================================================
#  刷题题目关系表
# ===================================================
@RecordManager.registerPlayerQuestion(
	GeneralQuestion, GeneralExerciseRecord,
	RecordSource.Exercise, ExerciseSingleRewardCalc)
class GeneralExerciseQuestion(Models.SelectingPlayerQuestion): pass


# ===================================================
#  刷题题目关系表
# ===================================================
@RecordManager.registerPlayerQuestion(
	ListeningQuestion, ListeningExerciseRecord, RecordSource.Exercise)
class ListeningExerciseQuestion(Models.GroupPlayerQuestion): pass


# ===================================================
#  刷题题目关系表
# ===================================================
@RecordManager.registerPlayerQuestion(
	ReadingQuestion, ReadingExerciseRecord, RecordSource.Exercise)
class ReadingExerciseQuestion(Models.GroupPlayerQuestion): pass


# ===================================================
#  单词题目关系表
# ===================================================
@RecordManager.registerPlayerQuestion(
	Word, WordExerciseRecord, RecordSource.Exercise)
class WordExerciseQuestion(Models.ElementPlayerQuestion):

	# 是否默写
	dictation = models.BooleanField(default=False, verbose_name="默写")

	def _convertBaseInfo(self, res, type):
		super()._convertBaseInfo(res, type)

		res['dictation'] = self.dictation

	def _create(self, dictation=False):
		self.dictation = dictation

		super()._create()

	def _generateChoices(self):
		if self.dictation: return
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
class PhraseExerciseQuestion(Models.ElementPlayerQuestion):

	def _generateChoices(self):
		pass
