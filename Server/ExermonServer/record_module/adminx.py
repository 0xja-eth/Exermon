from xadmin.layout import Fieldset
# from xadmin.plugins.inline import Inline
from game_module.adminx import ParamsInline
from item_module.adminx import *
from .models import *
import xadmin

# Register your models here.


@AdminXHelper.registerBaseInline(ExerciseQuestion)
class ExerciseQuestionsInline(object):

	model = ExerciseQuestion
	style = "table"


@AdminXHelper.relatedModel(GeneralQuesRecord)
class QuestionRecordAdmin(object): pass


@AdminXHelper.relatedModel(SelectingPlayerQuestion)
class PlayerQuestionAdmin(object): pass


@AdminXHelper.relatedModel(ExerciseQuestion)
class ExerciseQuestionAdmin(PlayerQuestionAdmin): pass


@AdminXHelper.relatedModel(QuestionSetRecord)
class QuestionSetRecordAdmin(object): pass


@AdminXHelper.relatedModel(ExerciseRecord)
class ExerciseRecordAdmin(QuestionSetRecordAdmin): pass

