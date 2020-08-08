
from item_module.adminx import *
from .models import *
import xadmin

# Register your models here.

@AdminXHelper.registerBaseInline(ExerSkill)
class ExerSkillsInline(object):

	model = ExerSkill
	extra = 0
	max_num = 3
	validate_max = 3
	style = "accordion"
	fk_name = 'o_exermon'


# @xadmin.sites.register(Exermon)
@AdminXHelper.relatedModel(Exermon)
class ExermonAdmin(BaseItemAdmin): pass


# @xadmin.sites.register(ExerFrag)
@AdminXHelper.relatedModel(ExerFrag)
class ExerFragAdmin(BaseItemAdmin): pass


@AdminXHelper.relatedModel(ExerGift)
class ExerGiftAdmin(BaseItemAdmin): pass


@AdminXHelper.relatedModel(ExerSkill)
class ExerSkillAdmin(BaseItemAdmin): pass


@AdminXHelper.relatedModel(GameEquip)
class ExerEquipAdmin(EquipableItemAdmin): pass


@AdminXHelper.relatedModel(ExerHub)
class ExerHubAdmin(PackContainerAdmin): pass


@AdminXHelper.relatedModel(EquipPack)
class ExerPackAdmin(PackContainerAdmin): pass


@AdminXHelper.relatedModel(ExerSlot)
class ExerSlotAdmin(SlotContainerAdmin): pass


@AdminXHelper.relatedModel(ExerEquipSlot)
class ExerEquipSlotAdmin(SlotContainerAdmin): pass


@AdminXHelper.relatedModel(ExerFragPack)
class ExerFragPackAdmin(PackContainerAdmin): pass


@AdminXHelper.relatedModel(ExerGiftPool)
class ExerGiftPoolAdmin(PackContainerAdmin): pass


@AdminXHelper.relatedModel(PlayerExermon)
class PlayerExermonAdmin(PackContItemAdmin): pass


@AdminXHelper.relatedModel(ExerSkillSlot)
class ExerSkillSlotAdmin(SlotContainerAdmin): pass


@AdminXHelper.relatedModel(EquipPackItem)
class ExerPackEquipAdmin(PackContItemAdmin): pass


@AdminXHelper.relatedModel(ExerSlotItem)
class ExerSlotItemAdmin(SlotContItemAdmin): pass


@AdminXHelper.relatedModel(ExerEquipSlotItem)
class ExerEquipSlotItemAdmin(SlotContItemAdmin): pass


@AdminXHelper.relatedModel(ExerSkillSlotItem)
class ExerSkillSlotItemAdmin(SlotContItemAdmin): pass


@AdminXHelper.relatedModel(ExerFragPackItem)
class ExerFragPackItemAdmin(PackContItemAdmin): pass


@AdminXHelper.relatedModel(PlayerExerGift)
class PlayerExerGiftAdmin(PackContItemAdmin): pass


xadmin.site.register(GameEquipPrice, CurrencyAdmin)
xadmin.site.register(ExerSkillEffect, BaseEffectAdmin)

