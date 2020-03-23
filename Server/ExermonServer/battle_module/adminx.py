import xadmin

from item_module.adminx import *
from record_module.adminx import *
from .models import *


class BattleItemSlotItemsInline(BaseContItemsInline): model = BattleItemSlotItem


@xadmin.sites.register(BattleResultJudge)
class SeasonRecordAdmin(object):
	list_display = ['id', 'score', 'win', 'lose']

	list_editable = ['score', 'win', 'lose']


@xadmin.sites.register(BattleItemSlot)
class HumanEquipSlotAdmin(SlotContainerAdmin):

	list_display = SlotContainerAdmin.list_display + \
				   ['score', 'win', 'lose']

	field_set = [Fieldset('对战物资槽属性', 'player')]

	form_layout = SlotContainerAdmin.form_layout + field_set

	inlines = [BattleItemSlotItemsInline]


@xadmin.sites.register(BattleItemSlotItem)
class BattleItemSlotItemAdmin(SlotContItemAdmin):

	list_display = SlotContItemAdmin.list_display + \
				   ['pack_item']

	list_editable = SlotContItemAdmin.list_display + \
				   ['pack_equip']

	field_set = [Fieldset('对战物资槽项属性', 'pack_item')]

	form_layout = SlotContItemAdmin().form_layout + field_set


@xadmin.sites.register(BattleRecord)
class BattleRecordAdmin(object):
	list_display = ['id', 'mode', 'season', 'create_time', 'result_time']

	list_editable = ['mode', 'season', 'create_time', 'result_time']


@xadmin.sites.register(BattleRound)
class BattleRoundAdmin(object):
	list_display = ['id', 'order', 'record', 'question']

	list_editable = ['name', 'start_time', 'question']


@xadmin.sites.register(BattlePlayer)
class BattlePlayerAdmin(QuestionSetRecordAdmin):
	list_display = QuestionSetRecordAdmin.list_display + \
					['record', 'adminScores', 'score_incr', 'result', 'status']

	list_editable = QuestionSetRecordAdmin.list_editable + \
					 ['record', 'score_incr', 'result', 'status']


@xadmin.sites.register(BattleRoundResult)
class BattleRoundResultAdmin(PlayerQuestionAdmin):
	list_display = PlayerQuestionAdmin.list_display + \
				   ['round', 'battle_player', 'attack', 'skill', 'target_type'
					   'result_type', 'hurt', 'damage', 'recover']

	list_editable = PlayerQuestionAdmin.list_editable + \
					['attack', 'skill', 'target_type'
					 'result_type', 'hurt', 'damage', 'recover']