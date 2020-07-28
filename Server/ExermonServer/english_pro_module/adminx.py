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


# class ListeningQuesChoicesInline(BaseQuesChoicesInline):
# 	model = ListeningQuesChoice
#
#
# class PlotQuesChoicesInline(BaseQuesChoicesInline):
# 	model = PlotQuesChoice


# class ReadingQuesChoicesInline(BaseQuesChoicesInline):
# 	model = ReadingQuesChoice


# class ListeningSubQuestionsInline(BaseQuestionsInline):
# 	model = ListeningSubQuestion


# class ReadingSubQuestionsInline(BaseQuestionsInline):
# 	model = ReadingSubQuestion


@AdminXHelper.registerBaseInline(WrongItem)
class WrongItemsInline(object):

	model = WrongItem
	style = "table"

# class ExerProPlotEffectsInline(BaseEffectsInline):
# 	model = ExerProPlotEffect
#
#
# class ExerProItemTraitsInline(BaseTraitsInline):
# 	model = ExerProItemTrait
#
#
# class ExerProStateTraitsInline(BaseTraitsInline):
# 	model = ExerProStateTrait
#
#
# class ExerProPotionEffectsInline(BaseEffectsInline):
# 	model = ExerProPotionEffect
#
#
# class ExerProCardEffectsInline(BaseEffectsInline):
# 	model = ExerProCardEffect
#
#
# class EnemyEffectsInline(BaseEffectsInline):
# 	model = EnemyEffect
#
#
# class EnemyActionsInline(BaseEffectsInline):
# 	model = EnemyAction


@AdminXHelper.registerBaseInline(ExerProMapStage)
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


@AdminXHelper.relatedModel(ListeningQuesChoice)
class ListeningQuesChoiceAdmin(object):

	# list_display = ['id', 'order', 'text', 'answer']
	#
	# list_editable = ['order', 'text', 'answer']
	pass


@AdminXHelper.relatedModel(ListeningSubQuestion)
class ListeningSubQuestionAdmin(BaseQuestionAdmin):

	# inlines = [ListeningQuesChoicesInline]
	pass

@AdminXHelper.relatedModel(PlotQuestion)
class PlotQuestionAdmin(BaseQuestionAdmin):

	# list_display = ['id', 'title', 'event_name']
	#
	# list_editable = ['title', 'event_name']
	#
	# inlines = [PlotQuesChoicesInline]
	pass


@AdminXHelper.relatedModel(PlotQuesChoice)
class PlotQuesChoiceAdmin(object):

	# list_display = ['question', 'order', 'text', 'result_text', 'gold']
	#
	# list_editable = ['question', 'order', 'text', 'result_text', 'gold']
	#
	# inlines = [ExerProPlotEffectsInline]
	pass


# @AdminXHelper.relatedModel(ReadingSubQuestion)
# class ReadingSubQuestionAdmin(BaseQuestionAdmin):
# 	inlines = [ReadingQuesChoicesInline]


@AdminXHelper.relatedModel(ListeningQuestion)
class ListeningQuestionAdmin(GroupQuestionAdmin):

	# list_display = GroupQuestionAdmin.list_display + \
	# 			   ['times']
	#
	# list_editable = GroupQuestionAdmin.list_editable + \
	# 			   ['times']
	#
	# inlines = [ListeningSubQuestionsInline]
	pass


# @AdminXHelper.relatedModel(ReadingQuestion)
# class ReadingQuestionAdmin(GroupQuestionAdmin):
# 	inlines = [ReadingSubQuestionsInline]


@AdminXHelper.relatedModel(PhraseQuestion)
class PhraseQuestionAdmin(object):

	# list_display = ['id', 'word', 'chinese', 'phrase']
	#
	# list_editable = ['word', 'chinese', 'phrase']
	pass


@AdminXHelper.relatedModel(CorrectionQuestion)
class CorrectionQuestionAdmin(object):

	# list_display = ['id', 'article', 'description']
	#
	# list_editable = ['article', 'description']
	#
	# inlines = [WrongItemsInline]
	pass


@AdminXHelper.relatedModel(WrongItem)
class WrongItemAdmin(object):

	# list_display = ['id', 'sentence_index', 'word_index', 'type', 'word']
	#
	# list_editable = ['sentence_index', 'word_index', 'type', 'word']
	pass


@AdminXHelper.relatedModel(Word)
class WordAdmin(object):

	# list_display = ['id', 'english', 'chinese', 'type',
	# 				'level', 'is_middle', 'is_high']
	#
	# list_editable = ['english', 'chinese', 'type',
	# 				 'level', 'is_middle', 'is_high']
	pass


@AdminXHelper.relatedModel(WordRecord)
class WordRecordAdmin(object):

	# list_display = ['id', 'word', 'record', 'count', 'correct',
	# 				'last_date', 'first_date', 'collected', 'wrong']
	#
	# list_editable = ['count', 'correct', 'last_date',
	# 				 'first_date', 'collected', 'wrong']
	pass


@AdminXHelper.relatedModel(Antonym)
class AntonymAdmin(object):
	# list_display = ['id', 'card_word', 'enemy_word', 'hurt_rate']
	#
	# list_editable = ['card_word', 'enemy_word', 'hurt_rate']
	pass


@AdminXHelper.relatedModel(BaseItem)
class BaseExerProItemAdmin(BaseItemAdmin):

	# list_display = BaseItemAdmin.list_display + \
	# 			   ['icon_index', 'start_ani_index',
	# 				'target_ani_index', 'star']
	#
	# list_editable = BaseItemAdmin.list_editable + \
	# 				['icon_index', 'start_ani_index',
	# 				'target_ani_index', 'star']
	#
	# field_set = [Fieldset('基本特训物品属性',
	# 					  'icon_index', 'start_ani_index',
	# 					  'target_ani_index', 'star')]
	#
	# form_layout = BaseItemAdmin.form_layout + field_set

	# form_layout = field_set
	pass


@AdminXHelper.relatedModel(ExerProItem)
class ExerProItemAdmin(BaseExerProItemAdmin):

	# list_display = BaseExerProItemAdmin.list_display + \
	# 			   []
	#
	# list_editable = BaseExerProItemAdmin.list_editable + \
	# 			   []
	#
	# field_set = [Fieldset('特训物品属性')]
	#
	# form_layout = BaseExerProItemAdmin.form_layout + field_set
	#
	# inlines = [ExerProItemTraitsInline]
	pass


@AdminXHelper.relatedModel(ExerProPotion)
class ExerProPotionAdmin(BaseExerProItemAdmin):

	# list_display = BaseExerProItemAdmin.list_display + \
	# 			   []
	#
	# list_editable = BaseExerProItemAdmin.list_editable + \
	# 			   []
	#
	# field_set = [Fieldset('特训物品属性')]
	#
	# form_layout = BaseItemAdmin.form_layout + field_set
	#
	# inlines = [ExerProPotionEffectsInline]
	pass


@AdminXHelper.relatedModel(ExerProCard)
class ExerProCardAdmin(BaseItemAdmin):

	# list_display = BaseExerProItemAdmin.list_display + \
	# 			   ['cost', 'card_type', 'inherent', 'disposable',
	# 				'character', 'target']
	#
	# list_editable = BaseExerProItemAdmin.list_editable + \
	# 			   ['cost', 'card_type', 'inherent', 'disposable',
	# 				'character', 'target']
	#
	# field_set = [Fieldset('特训卡片属性', 'cost', 'card_type',
	# 					  'inherent', 'disposable', 'character', 'target')]
	#
	# form_layout = BaseExerProItemAdmin.form_layout + field_set
	#
	# inlines = [ExerProCardEffectsInline]
	pass


@AdminXHelper.relatedModel(FirstCardGroup)
class FirstCardGroupAdmin(object):

	# list_display = ['id', 'name', 'adminCards']
	#
	# list_editable = ['name']
	pass


@AdminXHelper.relatedModel(ExerProEnemy)
class ExerProEnemyAdmin(BaseItemAdmin):

	# list_display = BaseItemAdmin.list_display + \
	# 			   ['type', 'mhp', 'power', 'defense', 'character']
	#
	# list_editable = BaseItemAdmin.list_editable + \
	# 			   ['type', 'mhp', 'power', 'defense', 'character']
	#
	# field_set = [Fieldset('特训敌人属性', 'type', 'mhp', 'power',
	# 					  'defense', 'character')]
	#
	# form_layout = BaseItemAdmin.form_layout + field_set
	#
	# inlines = [EnemyActionsInline, EnemyEffectsInline]
	pass


@AdminXHelper.relatedModel(ExerProState)
class ExerProStateAdmin(BaseItemAdmin):

	# list_display = BaseItemAdmin.list_display + \
	# 			   ['icon_index', 'max_turns', 'is_nega']
	#
	# list_editable = BaseItemAdmin.list_editable + \
	# 			   ['icon_index', 'max_turns', 'is_nega']
	#
	# field_set = [Fieldset('特训状态属性', 'icon_index', 'max_turns', 'is_nega')]
	#
	# form_layout = BaseItemAdmin.form_layout + field_set
	#
	# inlines = [ExerProStateTraitsInline]
	pass


@AdminXHelper.relatedModel(ExerProMap)
class ExerProMapAdmin(object):

	# list_display = ['id', 'name', 'description', 'level', 'min_level']
	#
	# list_editable = ['name', 'description', 'level', 'min_level']
	#
	# inlines = [MapStagesInline]
	pass


@AdminXHelper.relatedModel(ExerProMapStage)
class ExerProMapStageAdmin(object):

	# list_display = ['id', 'order', 'map', 'enemies', 'max_battle_enemies',
	# 				'steps', 'max_fork_node', 'max_fork', 'node_rate']
	#
	# list_editable = ['order', 'enemies', 'max_battle_enemies',
	# 				 'steps', 'max_fork_node', 'max_fork', 'node_rate']

	# inlines = [EnemiesInline]
	pass


@AdminXHelper.relatedModel(ExerProItemStar)
class ExerProItemStarAdmin(object):
	# list_display = ['id', 'name', 'color', 'adminColor']
	#
	# list_editable = ['name', 'color']
	pass


@AdminXHelper.relatedModel(NodeType)
class NodeTypeAdmin(object):
	# list_display = ['id', 'name', 'ques_types', 'configure']
	#
	# list_editable = ['name', 'ques_types', 'configure']
	pass


@AdminXHelper.relatedModel(ExerProRecord)
class ExerProRecordAdmin(object):
	# list_display = ['id', 'player', 'stage', 'started', 'generated',
	# 				'cur_index', 'node_flag', 'word_level', 'gold']
	#
	# list_editable = ['stage', 'started', 'generated',
	# 				'cur_index', 'node_flag', 'word_level', 'gold']
	pass

