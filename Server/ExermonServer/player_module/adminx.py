from xadmin.layout import Fieldset
# from xadmin.plugins.inline import Inline
from game_module.adminx import EquipParamsInline
from item_module.adminx import *
from .models import *
import xadmin

# Register your models here.

#
# class HumanItemEffectsInline(BaseEffectsInline): model = HumanItemEffect
#
#
# class HumanEquipLevelParamsInline(EquipParamsInline): model = HumanEquipLevelParam
#
#
# class HumanEquipBaseParamsInline(EquipParamsInline): model = HumanEquipBaseParam
#
#
# class HumanPackItemsInline(BaseContItemsInline): model = HumanPackItem
#
#
# class HumanPackEquipsInline(BaseContItemsInline): model = HumanPackEquip
#
#
# class HumanEquipSlotItemsInline(BaseContItemsInline): model = HumanEquipSlotItem
#
#
# class PlayerMoneyInline(CurrencyInline): model = PlayerMoney
#
#
# class HumanItemPriceInline(CurrencyInline): model = HumanItemPrice
#
#
# class HumanEquipPriceInline(CurrencyInline): model = HumanEquipPrice


@AdminXHelper.relatedModel(LoginInfo)
class LoginInfoAdmin(object):

	# list_display = ['id', 'player', 'time', 'logout', 'ip_address']
	pass


@AdminXHelper.relatedModel(PasswordRecord)
class PasswordRecordAdmin(object):

	# list_display = ['id', 'player', 'time', 'password', 'ip_address']
	pass


@AdminXHelper.relatedModel(Character)
class CharacterAdmin(object):

	# list_display = ['id', 'name', 'description', 'gender']
	#
	# list_editable = ['name', 'description', 'gender']
	pass


@AdminXHelper.relatedModel(PlayerMoney)
class PlayerMoneyAdmin(object):

	# list_display = ['id', 'gold', 'ticket', 'bound_ticket', 'player']
	#
	# list_editable = ['gold', 'ticket', 'bound_ticket', 'player']
	pass


@AdminXHelper.relatedModel(Player)
class PlayerAdmin(object):

	# list_display = ['id', 'username', 'phone', 'email', 'name', 'status',
	# 				'character', 'exp', 'adminLevel', 'adminMoney', 'online',
	# 				'type', 'grade', 'create_time', 'last_refresh_time']
	#
	# list_editable = ['username', 'phone', 'email', 'name', 'character', 'exp',
	# 				 'status', 'type', 'online', 'grade']
	#
	# inlines = [PlayerMoneyInline]
	pass


@AdminXHelper.relatedModel(HumanItem)
class HumanItemAdmin(UsableItemAdmin):

	# inlines = [HumanItemPriceInline, HumanItemEffectsInline]
	pass


@AdminXHelper.relatedModel(HumanEquip)
class HumanEquipAdmin(EquipableItemAdmin):

	# inlines = [HumanEquipPriceInline,
	# 		   HumanEquipLevelParamsInline,
	# 		   HumanEquipBaseParamsInline]
	pass


@AdminXHelper.relatedModel(HumanPack)
class HumanPackAdmin(PackContainerAdmin):

	# list_display = PackContainerAdmin.list_display + \
	# 			   ['player']
	#
	# field_set = [Fieldset('人类背包属性', 'player')]
	#
	# form_layout = PackContainerAdmin.form_layout + field_set
	#
	# inlines = [HumanPackItemsInline, HumanPackEquipsInline]
	pass


@AdminXHelper.relatedModel(HumanEquipSlot)
class HumanEquipSlotAdmin(SlotContainerAdmin):

	# list_display = SlotContainerAdmin.list_display + \
	# 			   ['player']
	#
	# field_set = [Fieldset('人类装备槽属性', 'player')]
	#
	# form_layout = SlotContainerAdmin.form_layout + field_set
	#
	# inlines = [HumanEquipSlotItemsInline]
	pass


@AdminXHelper.relatedModel(HumanPackItem)
class HumanPackItemAdmin(PackContItemAdmin):
	pass


@AdminXHelper.relatedModel(HumanPackEquip)
class HumanPackEquipAdmin(PackContItemAdmin):
	pass


@AdminXHelper.relatedModel(HumanEquipSlotItem)
class HumanEquipSlotItemAdmin(SlotContItemAdmin):

	# list_display = SlotContItemAdmin.list_display + \
	# 			   ['pack_equip', 'e_type']
	#
	# list_editable = SlotContItemAdmin.list_display + \
	# 			   ['pack_equip']
	#
	# field_set = [Fieldset('人类装备槽项属性', 'pack_equip')]
	#
	# exclude = ['e_type']
	#
	# form_layout = SlotContItemAdmin().form_layout + field_set
	pass


xadmin.site.register(HumanItemEffect, BaseEffectAdmin)

xadmin.site.register(HumanItemPrice, CurrencyAdmin)
xadmin.site.register(HumanEquipPrice, CurrencyAdmin)
