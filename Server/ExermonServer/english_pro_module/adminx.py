#-*-coding:GBK -*-

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


class MapStagesInline(object):
	model = ExerProMapStage
	style = "accordion"


# class EnemiesInline(object):
# 	model = ExerProEnemy
# 	style = "table"


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


@xadmin.sites.register(WordRecord)
class WordRecordAdmin(object):

	list_display = ['id', 'word', 'player', 'count', 'correct',
					'last_date', 'first_date', 'collected', 'wrong']

	list_editable = ['count', 'correct',  'last_date',
					 'first_date', 'collected', 'wrong']


@xadmin.sites.register(ExerProItem)
class ExerProItemAdmin(BaseItemAdmin):

	list_display = BaseItemAdmin.list_display + \
				   []

	list_editable = BaseItemAdmin.list_editable + \
				   []

	field_set = [Fieldset('特训物品属性')]

	form_layout = BaseItemAdmin.form_layout + field_set

	inlines = []


@xadmin.sites.register(ExerProPotion)
class ExerProPotionAdmin(BaseItemAdmin):

	list_display = BaseItemAdmin.list_display + \
				   []

	list_editable = BaseItemAdmin.list_editable + \
				   []

	field_set = [Fieldset('特训物品属性')]

	form_layout = BaseItemAdmin.form_layout + field_set

	inlines = []


@xadmin.sites.register(ExerProCard)
class ExerProCardAdmin(BaseItemAdmin):

	list_display = BaseItemAdmin.list_display + \
				   ['cost', 'card_type']

	list_editable = BaseItemAdmin.list_editable + \
				   ['cost', 'card_type']

	field_set = [Fieldset('特训卡片属性', 'cost', 'card_type')]

	form_layout = BaseItemAdmin.form_layout + field_set

	inlines = []


@xadmin.sites.register(ExerProEnemy)
class ExerProEnemyAdmin(BaseItemAdmin):

	list_display = BaseItemAdmin.list_display + \
				   ['mhp', 'power', 'level']

	list_editable = BaseItemAdmin.list_editable + \
				   ['mhp', 'power', 'level']

	field_set = [Fieldset('特训敌人属性', 'mhp', 'power', 'level')]

	form_layout = BaseItemAdmin.form_layout + field_set

	inlines = []


@xadmin.sites.register(ExerProMap)
class ExerProMapAdmin(object):

	list_display = ['id', 'name', 'description', 'level', 'min_level']

	list_editable = ['name', 'description', 'level', 'min_level']

	inlines = [MapStagesInline]


@xadmin.sites.register(ExerProMapStage)
class ExerProMapStageAdmin(object):

	list_display = ['id', 'order', 'map', 'enemies', 'max_battle_enemies',
					'steps', 'max_fork_node', 'max_fork', 'node_rate']

	list_editable = ['order', 'enemies', 'max_battle_enemies',
					 'steps', 'max_fork_node', 'max_fork', 'node_rate']

	# inlines = [EnemiesInline]