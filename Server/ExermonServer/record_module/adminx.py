from xadmin.layout import Fieldset
# from xadmin.plugins.inline import Inline
from game_module.adminx import ParamsInline
from item_module.adminx import *
from .models import *
import xadmin

# Register your models here.


@AdminXHelper.registerBaseInline(GeneralExerciseQuestion)
class ExerciseQuestionsInline(object):

	model = GeneralExerciseQuestion
	style = "table"


@AdminXHelper.relatedModel(SelectingPlayerQuestion)
class PlayerQuestionAdmin(object): pass


@AdminXHelper.relatedModel(GeneralExerciseQuestion)
class ExerciseQuestionAdmin(PlayerQuestionAdmin): pass


@AdminXHelper.relatedModel(QuesSetRecord)
class QuestionSetRecordAdmin(object): pass


@AdminXHelper.relatedModel(GeneralExerciseRecord)
class ExerciseRecordAdmin(QuestionSetRecordAdmin): pass

