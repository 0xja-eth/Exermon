
from item_module.adminx import *
from utils.admin_utils import AdminXHelper
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


@AdminXHelper.relatedModel(GameItem)
class HumanItemAdmin(UsableItemAdmin): pass


@AdminXHelper.relatedModel(ItemPack)
class HumanPackAdmin(PackContainerAdmin): pass


@AdminXHelper.relatedModel(ItemPackItem)
class HumanPackItemAdmin(PackContItemAdmin): pass


xadmin.site.register(GameItemEffect, BaseEffectAdmin)

xadmin.site.register(GameItemPrice, CurrencyAdmin)
