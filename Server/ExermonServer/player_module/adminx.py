from xadmin.layout import Fieldset
# from xadmin.plugins.inline import Inline
from game_module.adminx import ParamsInline
from item_module.adminx import *
from .models import *
import xadmin

# Register your models here.


class HumanItemEffectsInline(BaseEffectsInline):
	model = HumanItemEffect


class HumanEquipParamsInline(ParamsInline):
	model = HumanEquipParam


class HumanPackItemsInline(BaseContItemsInline):
	model = HumanPackItem


class HumanPackEquipsInline(BaseContItemsInline):
	model = HumanPackEquip


class HumanEquipSlotItemsInline(BaseContItemsInline):
	model = HumanEquipSlotItem


@xadmin.sites.register(LoginInfo)
class LoginInfoAdmin(object):

	list_display = ['id', 'player', 'time', 'logout', 'ip_address']


@xadmin.sites.register(PasswordRecord)
class PasswordRecordAdmin(object):

	list_display = ['id', 'player', 'time', 'password', 'ip_address']


@xadmin.sites.register(Character)
class CharacterAdmin(object):

	list_display = ['id', 'name', 'description', 'gender']

	list_editable = ['name', 'description', 'gender']


@xadmin.sites.register(Player)
class PlayerAdmin(object):

	list_display = ['id', 'username', 'password', 'phone', 'email', 'name',
					'character', 'exp', 'online', 'create_time', 'last_refresh_time',
					'status', 'type', 'grade', 'birth', 'school', 'city', 'contact',
					'description', 'is_deleted']

	list_editable = ['username', 'phone', 'email', 'name', 'character', 'exp',
					 'status', 'type', 'online', 'grade', 'birth', 'school',
					 'city', 'contact', 'description']


@xadmin.sites.register(HumanItem)
class HumanItemAdmin(UsableItemAdmin):
	effect_inlines = HumanItemEffectsInline


@xadmin.sites.register(HumanEquip)
class HumanEquipAdmin(EquipableItemAdmin):
	param_inlines = HumanEquipParamsInline


@xadmin.sites.register(HumanPack)
class HumanPackAdmin(PackContainerAdmin):

	list_display = PackContainerAdmin.list_display + \
				   ['player']

	field_set = [Fieldset('人类背包属性', 'player')]

	form_layout = PackContainerAdmin.form_layout + field_set

	inlines = [HumanPackItemsInline, HumanPackEquipsInline]


@xadmin.sites.register(HumanEquipSlot)
class HumanEquipSlotAdmin(SlotContainerAdmin):

	list_display = SlotContainerAdmin.list_display + \
				   ['pack_equip', 'player']

	field_set = [Fieldset('人类装备槽属性', 'pack_equip', 'player')]

	form_layout = SlotContainerAdmin.form_layout + field_set

	cont_item_inlines = HumanEquipSlotItemsInline


@xadmin.sites.register(HumanPackItem)
class HumanPackItemAdmin(PackContItemAdmin):
	pass


@xadmin.sites.register(HumanPackEquip)
class HumanPackEquipAdmin(PackContItemAdmin):
	pass


@xadmin.sites.register(HumanEquipSlotItem)
class HumanEquipSlotItemAdmin(SlotContItemAdmin):

	list_display = SlotContItemAdmin.list_display + \
				   ['pack_equip', 'e_type']

	list_editable = SlotContItemAdmin.list_display + \
				   ['pack_equip']

	field_set = [Fieldset('人类装备槽项属性', 'pack_equip')]

	exclude = ['e_type']

	form_layout = SlotContItemAdmin().form_layout + field_set


@xadmin.sites.register(HumanItemEffect)
class HumanItemEffectAdmin(BaseEffectAdmin):
	pass

