
from item_module.adminx import *
from .models import *
import xadmin

# Register your models here.


@AdminXHelper.relatedModel(LoginInfo)
class LoginInfoAdmin(object): pass


@AdminXHelper.relatedModel(PasswordRecord)
class PasswordRecordAdmin(object): pass


@AdminXHelper.relatedModel(Character)
class CharacterAdmin(object): pass


@AdminXHelper.relatedModel(PlayerMoney)
class PlayerMoneyAdmin(object): pass


@AdminXHelper.relatedModel(Player)
class PlayerAdmin(object): pass


@AdminXHelper.relatedModel(HumanItem)
class HumanItemAdmin(UsableItemAdmin): pass


@AdminXHelper.relatedModel(HumanEquip)
class HumanEquipAdmin(EquipableItemAdmin): pass


@AdminXHelper.relatedModel(HumanPack)
class HumanPackAdmin(PackContainerAdmin): pass


@AdminXHelper.relatedModel(HumanEquipSlot)
class HumanEquipSlotAdmin(SlotContainerAdmin): pass


@AdminXHelper.relatedModel(HumanPackItem)
class HumanPackItemAdmin(PackContItemAdmin): pass


@AdminXHelper.relatedModel(HumanPackEquip)
class HumanPackEquipAdmin(PackContItemAdmin): pass


@AdminXHelper.relatedModel(HumanEquipSlotItem)
class HumanEquipSlotItemAdmin(SlotContItemAdmin): pass


xadmin.site.register(HumanItemEffect, BaseEffectAdmin)

xadmin.site.register(HumanItemPrice, CurrencyAdmin)
xadmin.site.register(HumanEquipPrice, CurrencyAdmin)
