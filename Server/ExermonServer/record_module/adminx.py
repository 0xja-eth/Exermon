from utils.admin_utils import AdminXHelper
from .models import *


# Register your models here.


@AdminXHelper.registerBaseInline(BasePlayerQuestion)
class PlayerQuestionsInline(object):

	style = "table"

# region QuesSetRecord


@AdminXHelper.relatedModel(QuesSetRecord)
class QuesSetRecordAdmin(object): pass


@AdminXHelper.relatedModel(BaseExerciseRecord)
class BaseExerciseRecordAdmin(QuesSetRecordAdmin): pass


@AdminXHelper.relatedModel(GeneralExerciseRecord)
class GeneralExerciseRecordAdmin(BaseExerciseRecordAdmin): pass


@AdminXHelper.relatedModel(ListeningExerciseRecord)
class ListeningExerciseRecordAdmin(BaseExerciseRecordAdmin): pass


@AdminXHelper.relatedModel(ReadingExerciseRecord)
class ReadingExerciseRecordAdmin(BaseExerciseRecordAdmin): pass


@AdminXHelper.relatedModel(CollectingExerciseRecord)
class CollectingExerciseRecordAdmin(BaseExerciseRecordAdmin): pass


@AdminXHelper.relatedModel(WordExerciseRecord)
class WordExerciseRecordAdmin(QuesSetRecordAdmin): pass


@AdminXHelper.relatedModel(PhraseExerciseRecord)
class PhraseExerciseRecordAdmin(QuesSetRecordAdmin): pass


# endregion


# region PlayerQuestion


@AdminXHelper.relatedModel(BasePlayerQuestion)
class BasePlayerQuestionAdmin(object): pass


@AdminXHelper.relatedModel(SelectingPlayerQuestion)
class SelectingPlayerQuestionAdmin(BasePlayerQuestionAdmin): pass


@AdminXHelper.relatedModel(GroupPlayerQuestion)
class GroupPlayerQuestionAdmin(BasePlayerQuestionAdmin): pass


@AdminXHelper.relatedModel(ElementPlayerQuestion)
class ElementPlayerQuestionAdmin(BasePlayerQuestionAdmin): pass


@AdminXHelper.relatedModel(GeneralExerciseQuestion)
class GeneralExerciseQuestionAdmin(SelectingPlayerQuestionAdmin): pass


@AdminXHelper.relatedModel(ListeningExerciseQuestion)
class ListeningExerciseQuestionAdmin(GroupPlayerQuestionAdmin): pass


@AdminXHelper.relatedModel(ReadingExerciseQuestion)
class ReadingExerciseQuestionAdmin(GroupPlayerQuestionAdmin): pass


@AdminXHelper.relatedModel(CorrectingExerciseQuestion)
class CorrectingExerciseQuestionAdmin(BasePlayerQuestionAdmin): pass


@AdminXHelper.relatedModel(WordExerciseQuestion)
class WordExerciseQuestionAdmin(ElementPlayerQuestionAdmin): pass


@AdminXHelper.relatedModel(PhraseExerciseQuestion)
class PhraseExerciseQuestionAdmin(ElementPlayerQuestionAdmin): pass


# endregion
