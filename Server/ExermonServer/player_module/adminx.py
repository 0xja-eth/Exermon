from xadmin.layout import Fieldset
# from xadmin.plugins.inline import Inline
from item_module.adminx import PackContItemsInline, \
	UsableItemAdmin, EquipableItemAdmin, PackContainerAdmin, \
	SlotContainerAdmin, PackContItemAdmin, SlotContItemAdmin
from .models import *
import xadmin

# Register your models here.


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
					'character', 'grade', 'create_time', 'last_refresh_time',
					'status', 'type', 'online', 'birth', 'school', 'city', 'contact',
					'description', 'is_deleted']

	list_editable = ['username', 'phone', 'email', 'name', 'character', 'grade', 'birth',
					 'school', 'city', 'contact', 'description', 'status', 'type', 'online']


@xadmin.sites.register(HumanItem)
class HumanItemAdmin(UsableItemAdmin):
	pass


@xadmin.sites.register(HumanEquip)
class HumanEquipAdmin(EquipableItemAdmin):
	pass


@xadmin.sites.register(HumanPack)
class HumanPackAdmin(PackContainerAdmin):

	list_display = PackContainerAdmin().list_display + \
				   ['player']

	field_set = [Fieldset('人类背包属性', 'player')]

	form_layout = PackContainerAdmin().form_layout + field_set


@xadmin.sites.register(HumanPackItem)
class HumanPackItemAdmin(PackContItemAdmin):
	pass


@xadmin.sites.register(HumanPackEquip)
class HumanPackEquipAdmin(PackContItemAdmin):
	pass


@xadmin.sites.register(HumanEquipSlot)
class HumanEquipSlotAdmin(SlotContainerAdmin):

	list_display = SlotContainerAdmin().list_display + \
				   ['player']

	field_set = [Fieldset('人类装备槽属性', 'player')]

	form_layout = SlotContainerAdmin().form_layout + field_set


@xadmin.sites.register(HumanEquipSlotItem)
class HumanEquipSlotItemAdmin(SlotContItemAdmin):

	list_display = SlotContItemAdmin().list_display + \
				   ['e_type']

	field_set = [Fieldset('人类装备槽项属性', 'e_type')]

	form_layout = SlotContItemAdmin().form_layout + field_set

