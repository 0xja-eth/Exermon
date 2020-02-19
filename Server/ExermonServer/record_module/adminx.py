from xadmin.layout import Fieldset
# from xadmin.plugins.inline import Inline
from game_module.adminx import ParamsInline
from item_module.adminx import *
from .models import *
import xadmin

# Register your models here.


class ExerciseQuestionsInline(object):

	model = ExerciseQuestion
	style = "table"


@xadmin.sites.register(QuestionRecord)
class QuestionRecordAdmin(object):

	list_display = ['id', 'question', 'player', 'count', 'correct',
					'last_date', 'first_date', 'first_time', 'avg_time',
					'corr_time', 'sum_exp', 'sum_gold', 'source',
					'collected', 'wrong', 'note']

	list_editable = ['question', 'player', 'count', 'correct',
					'last_date', 'first_date', 'first_time', 'avg_time',
					'corr_time', 'sum_exp', 'sum_gold', 'source',
					'collected', 'wrong', 'note']


class PlayerQuestionAdmin(object):

	list_display = ['id', 'question', 'answered', 'adminSelection',
					'adminAnswer', 'timespan', 'exp_incr',
					'slot_exp_incr', 'gold_incr', 'is_new']

	list_editable = ['question', 'answered', 'timespan', 'exp_incr',
					 'slot_exp_incr', 'gold_incr', 'is_new']


@xadmin.sites.register(ExerciseQuestion)
class ExerciseQuestionAdmin(PlayerQuestionAdmin):

	list_display = PlayerQuestionAdmin.list_display + \
				   ['exercise']

	list_editable = PlayerQuestionAdmin.list_editable + \
				   ['exercise']


class QuestionSetRecordAdmin(object):

	list_display = ['id', 'player', 'create_time', 'finished',
					'exp_incr', 'slot_exp_incr', 'gold_incr']

	list_editable = ['player', 'create_time', 'finished',
					 'exp_incr', 'slot_exp_incr', 'gold_incr']


@xadmin.sites.register(ExerciseRecord)
class ExerciseRecordAdmin(QuestionSetRecordAdmin):

	list_display = QuestionSetRecordAdmin.list_display + \
				   ['subject', 'count', 'dtb_type']

	list_editable = BaseItemAdmin.list_editable + \
				   ['subject', 'count', 'dtb_type']

	inlines = [ExerciseQuestionsInline]
