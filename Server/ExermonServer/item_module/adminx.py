from xadmin.layout import Fieldset
# from xadmin.plugins.inline import Inline
from game_module.adminx import ParamsInline
from .models import *
import xadmin

# Register your models here.


class EquipParamsInline(ParamsInline):

	model = EquipParam


class ItemPriceInline(object):

	model = ItemPrice
	min_num = 1
	max_num = 1
	validate_min = 1
	validate_max = 1
	style = "one"


class PackContItemsInline(object):

	model = PackContItem
	extra = 0
	style = "accordion"


class SlotContItemsInline(object):

	model = SlotContItem
	extra = 0
	style = "accordion"


class ItemEffectsInline(object):

	model = ItemEffect
	extra = 1
	style = "table"


@xadmin.sites.register(ItemPrice)
class ItemPriceAdmin(object):

	list_display = ['id', 'item', 'gold', 'ticket', 'bound_ticket']

	list_editable = ['item', 'gold', 'ticket', 'bound_ticket']


@xadmin.sites.register(BaseItem)
class BaseItemAdmin(object):

	list_display = ['id', 'name', 'description', 'type']

	list_editable = ['name', 'description']

	field_set = [Fieldset('基本物品属性', 'name', 'description')]

	form_layout = field_set

	exclude = ['type']

	# form_layout = field_set


@xadmin.sites.register(LimitedItem)
class LimitedItemAdmin(BaseItemAdmin):

	list_display = BaseItemAdmin().list_display + \
				   ['adminBuyPrice', 'sell_price', 'discardable', 'tradable', 'icon']

	list_editable = BaseItemAdmin().list_editable + \
					['sell_price', 'discardable', 'tradable', 'icon']

	field_set = [Fieldset('有限物品属性', 'sell_price', 'discardable',
						 'tradable', 'icon')]

	form_layout = BaseItemAdmin().form_layout + field_set

	inlines = [ItemPriceInline]


@xadmin.sites.register(UsableItem)
class UsableItemAdmin(LimitedItemAdmin):

	list_display = LimitedItemAdmin().list_display + \
				   ['max_count', 'battle_use', 'menu_use', 'adventure_use',
					'consumable', 'freeze', 'i_type', 'adminEffects']

	list_editable = LimitedItemAdmin().list_display + \
					['max_count', 'battle_use', 'menu_use', 'adventure_use',
					'consumable', 'freeze', 'i_type']

	field_set = [Fieldset('可用物品属性', 'max_count', 'battle_use', 'menu_use',
						 'adventure_use', 'consumable', 'freeze', 'i_type')]

	form_layout = LimitedItemAdmin().form_layout + field_set

	inlines = LimitedItemAdmin().inlines + [ItemEffectsInline]


@xadmin.sites.register(EquipParam)
class EquipParamAdmin(object):

	list_display = ['id', 'param', 'value', 'getValue']

	list_editable = ['param', 'value']


@xadmin.sites.register(EquipableItem)
class EquipableItemAdmin(LimitedItemAdmin):

	inlines = LimitedItemAdmin().inlines + [EquipParamsInline]


@xadmin.sites.register(BaseContainer)
class BaseContainerAdmin(object):

	list_display = ['id', 'type']

	list_editable = []

	field_set = [Fieldset('基本容器属性')]

	form_layout = field_set

	exclude = ['type']


@xadmin.sites.register(PackContainer)
class PackContainerAdmin(BaseContainerAdmin):

	list_display = BaseContainerAdmin().list_display + \
				   ['capacity']

	list_editable = BaseContainerAdmin().list_editable + \
					['capacity']

	field_set = [Fieldset('背包类容器属性', 'capacity')]

	form_layout = BaseContainerAdmin().form_layout + field_set

	inlines = [PackContItemsInline]


@xadmin.sites.register(SlotContainer)
class SlotContainerAdmin(BaseContainerAdmin):

	list_display = BaseContainerAdmin().list_display + \
				   ['equip_container1', 'equip_container2']

	# list_editable = BaseContainerAdmin().list_editable + \
	# 				['equip_container1', 'equip_container2']

	field_set = [Fieldset('槽类容器属性')]

	exclude = BaseContainerAdmin().exclude + \
			  ['equip_container1', 'equip_container2']

	form_layout = BaseContainerAdmin().form_layout + field_set

	inlines = [SlotContItemsInline]


@xadmin.sites.register(BaseContItem)
class BaseContItemAdmin(object):

	list_display = ['id', 'container', 'type']

	list_editable = ['container']

	field_set = Fieldset('基本容器项属性', 'container')

	form_layout = [field_set]

	exclude = ['type']


@xadmin.sites.register(PackContItem)
class PackContItemAdmin(BaseContItemAdmin):

	list_display = BaseContItemAdmin().list_display + \
				   ['item', 'count']

	list_editable = BaseContItemAdmin().list_editable + \
				   ['item', 'count']

	field_set = [Fieldset('背包类容器项属性', 'item', 'count')]

	form_layout = BaseContItemAdmin().form_layout + field_set


@xadmin.sites.register(SlotContItem)
class SlotContItemAdmin(BaseContItemAdmin):

	list_display = BaseContItemAdmin().list_display + \
				   ['index', 'equip_item1', 'equip_item2']

	list_editable = BaseContItemAdmin().list_editable + \
				   ['index', 'equip_item1', 'equip_item2']

	field_set = [Fieldset('槽类容器项属性', 'index', 'equip_item1', 'equip_item2')]

	form_layout = BaseContItemAdmin().form_layout + field_set


class EffectAdmin(object):

	list_display = ['id', 'item', 'code', 'params']

	list_editable = ['code', 'params']


@xadmin.sites.register(ItemEffect)
class ItemEffectAdmin(EffectAdmin):
	pass
