from item_module.adminx import *
from record_module.adminx import *
from utils.admin_utils import AdminXHelper
from .models import *


# class BattleItemSlotItemsInline(BaseContItemsInline): model = BattleItemSlotItem


@AdminXHelper.relatedModel(BattleResultJudge)
class SeasonRecordAdmin(object): pass


@AdminXHelper.relatedModel(BattleItemSlot)
class HumanEquipSlotAdmin(SlotContainerAdmin): pass


@AdminXHelper.relatedModel(BattleItemSlotItem)
class BattleItemSlotItemAdmin(SlotContItemAdmin): pass


@AdminXHelper.relatedModel(BattleRecord)
class BattleRecordAdmin(object): pass


@AdminXHelper.relatedModel(BattleRound)
class BattleRoundAdmin(object): pass


@AdminXHelper.relatedModel(BattlePlayer)
class BattlePlayerAdmin(QuesSetRecordAdmin): pass


@AdminXHelper.relatedModel(BattleRoundResult)
class BattleRoundResultAdmin(BasePlayerQuestionAdmin): pass
