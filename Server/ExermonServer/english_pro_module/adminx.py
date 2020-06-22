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


# class ReadingQuesChoicesInline(BaseQuesChoicesInline):
# 	model = ReadingQuesChoice


class ListeningSubQuestionsInline(BaseQuestionsInline):
	model = ListeningSubQuestion


# class ReadingSubQuestionsInline(BaseQuestionsInline):
# 	model = ReadingSubQuestion


class WrongItemsInline(object):
	model = WrongItem
	style = "table"


class ExerProItemTraitsInline(BaseTraitsInline):
	model = ExerProItemTrait


class ExerProStateTraitsInline(BaseTraitsInline):
	model = ExerProStateTrait


class ExerProPotionEffectsInline(BaseEffectsInline):
	model = ExerProPotionEffect


class ExerProCardEffectsInline(BaseEffectsInline):
	model = ExerProCardEffect


class EnemyEffectsInline(BaseEffectsInline):
	model = EnemyEffect


class EnemyActionsInline(BaseEffectsInline):
	model = EnemyAction


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


# @xadmin.sites.register(ReadingSubQuestion)
# class ReadingSubQuestionAdmin(BaseQuestionAdmin):
# 	inlines = [ReadingQuesChoicesInline]


@xadmin.sites.register(ListeningQuestion)
class ListeningQuestionAdmin(GroupQuestionAdmin):

	list_display = GroupQuestionAdmin.list_display + \
				   ['times']

	list_editable = GroupQuestionAdmin.list_editable + \
				   ['times']

	inlines = [ListeningSubQuestionsInline]


# @xadmin.sites.register(ReadingQuestion)
# class ReadingQuestionAdmin(GroupQuestionAdmin):
# 	inlines = [ReadingSubQuestionsInline]


@xadmin.sites.register(PhraseQuestion)
class InfinitiveQuestionAdmin(object):

	list_display = ['id', 'word', 'chinese', 'phrase']

	list_editable = ['word', 'chinese', 'phrase']


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

	list_display = ['id', 'word', 'record', 'count', 'correct',
					'last_date', 'first_date', 'collected', 'wrong']

	list_editable = ['count', 'correct',  'last_date',
					 'first_date', 'collected', 'wrong']


@xadmin.sites.register(Antonym)
class AntonymAdmin(object):
	list_display = ['id', 'card_word', 'enemy_word', 'hurt_rate']

	list_editable = ['card_word', 'enemy_word', 'hurt_rate']


class BaseExerProItemAdmin(BaseItemAdmin):

	list_display = BaseItemAdmin.list_display + \
				   ['icon_index', 'start_ani_index',
					'target_ani_index', 'star']

	list_editable = BaseItemAdmin.list_editable + \
					['icon_index', 'start_ani_index',
					'target_ani_index', 'star']

	field_set = [Fieldset('基本特训物品属性',
						  'icon_index', 'start_ani_index',
						  'target_ani_index', 'star')]

	form_layout = BaseItemAdmin.form_layout + field_set

	# form_layout = field_set


@xadmin.sites.register(ExerProItem)
class ExerProItemAdmin(BaseExerProItemAdmin):

	list_display = BaseExerProItemAdmin.list_display + \
				   []

	list_editable = BaseExerProItemAdmin.list_editable + \
				   []

	field_set = [Fieldset('特训物品属性')]

	form_layout = BaseExerProItemAdmin.form_layout + field_set

	inlines = [ExerProItemTraitsInline]


@xadmin.sites.register(ExerProPotion)
class ExerProPotionAdmin(BaseExerProItemAdmin):

	list_display = BaseExerProItemAdmin.list_display + \
				   []

	list_editable = BaseExerProItemAdmin.list_editable + \
				   []

	field_set = [Fieldset('特训物品属性')]

	form_layout = BaseItemAdmin.form_layout + field_set

	inlines = [ExerProPotionEffectsInline]


@xadmin.sites.register(ExerProCard)
class ExerProCardAdmin(BaseItemAdmin):

	list_display = BaseExerProItemAdmin.list_display + \
				   ['cost', 'card_type', 'inherent', 'disposable',
					'character', 'target']

	list_editable = BaseExerProItemAdmin.list_editable + \
				   ['cost', 'card_type', 'inherent', 'disposable',
					'character', 'target']

	field_set = [Fieldset('特训卡片属性', 'cost', 'card_type',
						  'inherent', 'disposable', 'character', 'target')]

	form_layout = BaseExerProItemAdmin.form_layout + field_set

	inlines = [ExerProCardEffectsInline]


@xadmin.sites.register(FirstCardGroup)
class FirstCardGroupAdmin(object):

	list_display = ['id', 'name', 'adminCards']

	list_editable = ['name']


@xadmin.sites.register(ExerProEnemy)
class ExerProEnemyAdmin(BaseItemAdmin):

	list_display = BaseItemAdmin.list_display + \
				   ['type', 'mhp', 'power', 'defense', 'character']

	list_editable = BaseItemAdmin.list_editable + \
				   ['type', 'mhp', 'power', 'defense', 'character']

	field_set = [Fieldset('特训敌人属性', 'type', 'mhp', 'power',
						  'defense', 'character')]

	form_layout = BaseItemAdmin.form_layout + field_set

	inlines = [EnemyActionsInline, EnemyEffectsInline]


@xadmin.sites.register(ExerProState)
class ExerProStateAdmin(BaseItemAdmin):

	list_display = BaseItemAdmin.list_display + \
				   []

	list_editable = BaseItemAdmin.list_editable + \
				   []

	field_set = [Fieldset('特训状态属性')]

	form_layout = BaseItemAdmin.form_layout + field_set

	inlines = [ExerProStateTraitsInline]


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


@xadmin.sites.register(ExerProItemStar)
class ExerProItemStarAdmin(object):
	list_display = ['id', 'name', 'color', 'adminColor']

	list_editable = ['name', 'color']


@xadmin.sites.register(NodeType)
class NodeTypeAdmin(object):
	list_display = ['id', 'name', 'ques_types', 'configure']

	list_editable = ['name', 'ques_types', 'configure']


@xadmin.sites.register(ExerProRecord)
class ExerProRecordAdmin(object):
	list_display = ['id', 'player', 'stage', 'started', 'generated',
					'cur_index', 'node_flag', 'word_level']

	list_editable = ['stage', 'started', 'generated',
					'cur_index', 'node_flag', 'word_level']

