from xadmin.layout import Fieldset
# from xadmin.plugins.inline import Inline
from game_module.adminx import ParamsInline
from question_module.adminx import *
from item_module.adminx import *
from .models import *
import xadmin

# Register your models here.


# class QuesSugarPriceInline(object):
#
# 	model = QuesSugarPrice
# 	min_num = 1
# 	max_num = 1
# 	validate_min = 1
# 	validate_max = 1
# 	style = "one"


class ListeningQuesChoicesInline(BaseQuesChoicesInline):
	model = ListeningQuesChoice


class ReadingQuesChoicesInline(BaseQuesChoicesInline):
	model = ReadingQuesChoice


class ListeningSubQuestionsInline(BaseQuestionsInline):
	model = ListeningSubQuestion


class ReadingSubQuestionsInline(BaseQuestionsInline):
	model = ReadingSubQuestion


class WrongItemsInline(object):
	model = WrongItem
	style = "table"


# @xadmin.sites.register(QuesSugarPrice)
# class QuesSugarPriceAdmin(object):
#
# 	list_display = ['id', 'sugar', 'gold', 'ticket', 'bound_ticket']
#
# 	list_editable = ['sugar', 'gold', 'ticket', 'bound_ticket']


@xadmin.sites.register(ListeningSubQuestion)
class ListeningSubQuestionAdmin(BaseQuestionAdmin):
	inlines = [ListeningQuesChoicesInline]


@xadmin.sites.register(ReadingSubQuestion)
class ListeningSubQuestionAdmin(BaseQuestionAdmin):
	inlines = [ReadingQuesChoicesInline]


@xadmin.sites.register(ListeningQuestion)
class ListeningQuestionAdmin(GroupQuestionAdmin):
	inlines = [ListeningSubQuestionsInline]


@xadmin.sites.register(ReadingQuestion)
class ListeningQuestionAdmin(GroupQuestionAdmin):
	inlines = [ReadingSubQuestionsInline]


@xadmin.sites.register(QuesReport)
class QuesReportAdmin(object):

	list_display = ['id', 'player', 'question', 'type', 'description']


@xadmin.sites.register(CorrectionQuestion)
class CorrectionQuestionAdmin(object):

	list_display = ['id', 'article', 'description']

	list_editable = ['article', 'description']

	inlines = [WrongItemsInline]


@xadmin.sites.register(WrongItem)
class WrongItemAdmin(object):

	list_display = ['id', 'sentence_index', 'word_index', 'type', 'word']

	list_editable = ['sentence_index', 'word_index', 'type', 'word']


@xadmin.sites.register(Word)
class WordAdmin(object):

	list_display = ['id', 'english', 'chinese', 'type',
					'level', 'is_middle', 'is_high']

	list_editable = ['english', 'chinese', 'type',
					 'level', 'is_middle', 'is_high']
