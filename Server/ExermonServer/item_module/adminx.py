from xadmin.layout import Fieldset

from .models import *

from utils.model_utils import AdminXHelper

# Register your models here.


@AdminXHelper.registerBaseInline(Currency)
class CurrencyInline(object):

	min_num = 1
	max_num = 1
	validate_min = 1
	validate_max = 1
	style = "one"


@AdminXHelper.registerBaseInline(BaseContItem)
class BaseContItemsInline(object):

	extra = 0
	style = "accordion"


@AdminXHelper.registerBaseInline(BaseEffect)
class BaseEffectsInline(object):

	extra = 1
	style = "table"


@AdminXHelper.registerBaseInline(BaseTrait)
class BaseTraitsInline(object):

	extra = 1
	style = "table"


@AdminXHelper.relatedModel(Currency)
class CurrencyAdmin(object):

	# list_display = ['id', 'gold', 'ticket', 'bound_ticket', 'item']
	#
	# list_editable = ['gold', 'ticket', 'bound_ticket', 'item']
	pass


@AdminXHelper.relatedModel(BaseItem)
class BaseItemAdmin(object):

	# list_display = ['id', 'name', 'description']
	#
	# list_editable = ['name', 'description']

	# field_set = [Fieldset('基本物品属性', 'name', 'description')]
	#
	# form_layout = field_set

	# form_layout = field_set
	pass


@AdminXHelper.relatedModel(LimitedItem)
class LimitedItemAdmin(BaseItemAdmin):

	# 'adminBuyPrice',
	# list_display = BaseItemAdmin.list_display + \
	# 			   ['icon_index', 'adminBuyPrice', 'sell_price', 'discardable', 'tradable']
	#
	# list_editable = BaseItemAdmin.list_editable + \
	# 				['icon_index', 'sell_price', 'discardable', 'tradable']
	#
	# field_set = [Fieldset('有限物品属性', 'sell_price', 'discardable',
	# 					 'tradable', 'icon')]
	#
	# form_layout = BaseItemAdmin.form_layout + field_se
	pass


@AdminXHelper.relatedModel(UsableItem)
class UsableItemAdmin(LimitedItemAdmin):

	# list_display = LimitedItemAdmin.list_display + \
	# 			   ['max_count', 'battle_use', 'menu_use', 'adventure_use',
	# 				'consumable', 'target', 'batch_count', 'freeze', 'i_type', 'adminEffects']
	#
	# list_editable = LimitedItemAdmin.list_editable + \
	# 				['max_count', 'battle_use', 'menu_use', 'adventure_use',
	# 				'consumable', 'target', 'batch_count', 'freeze', 'i_type']

	# field_set = [Fieldset('可用物品属性', 'max_count', 'battle_use', 'menu_use',
	# 					  'adventure_use', 'consumable', 'target',
	# 					  'batch_count', 'freeze', 'i_type')]
	#
	# form_layout = LimitedItemAdmin.form_layout + field_set

	# effect_inlines = None

	# inlines = LimitedItemAdmin.inlines + [effect_inlines]
	pass


@AdminXHelper.relatedModel(EquipableItem)
class EquipableItemAdmin(LimitedItemAdmin):

	# list_display = LimitedItemAdmin.list_display + \
	# 			   ['min_level', 'adminLevelParams', 'adminBaseParams']
	#
	# list_editable = LimitedItemAdmin.list_editable + \
	# 				['min_level']

	# param_inlines = None

	# inlines = LimitedItemAdmin.inlines + [param_inlines]
	pass


@AdminXHelper.relatedModel(BaseContainer)
class BaseContainerAdmin(object):

	# list_display = ['id', 'adminOwnerPlayer']
	#
	# list_editable = []
	#
	# field_set = [Fieldset('基本容器属性')]
	#
	# form_layout = field_set

	# cont_item_inlines = None
	#
	# inlines = [cont_item_inlines]
	pass


@AdminXHelper.relatedModel(PackContainer)
class PackContainerAdmin(BaseContainerAdmin):

	# list_display = BaseContainerAdmin.list_display + \
	# 			   ['capacity']
	#
	# list_editable = BaseContainerAdmin.list_editable + \
	# 				['capacity']
	#
	# field_set = [Fieldset('背包类容器属性', 'capacity')]
	#
	# form_layout = BaseContainerAdmin.form_layout + field_set
	pass


@AdminXHelper.relatedModel(SlotContainer)
class SlotContainerAdmin(BaseContainerAdmin):

	# list_display = BaseContainerAdmin.list_display + \
	# 			   ['equip_container1', 'equip_container2']

	# list_editable = BaseContainerAdmin).list_editable + \
	# 				['equip_container1', 'equip_container2']

	# field_set = [Fieldset('槽类容器属性')]

	# exclude = BaseContainerAdmin.exclude + \
	# 		  ['equip_container1', 'equip_container2']

	# form_layout = BaseContainerAdmin.form_layout + field_set
	pass


@AdminXHelper.relatedModel(BaseContItem)
class BaseContItemAdmin(object):
	#
	# list_display = ['id', 'container']
	#
	# list_editable = ['container']
	#
	# field_set = Fieldset('基本容器项属性', 'container')
	#
	# form_layout = [field_set]
	pass


@AdminXHelper.relatedModel(PackContItem)
class PackContItemAdmin(BaseContItemAdmin):

	# list_display = BaseContItemAdmin.list_display + \
	# 			   ['item', 'count', 'equiped']
	#
	# list_editable = BaseContItemAdmin.list_editable + \
	# 			   ['item', 'count', 'equiped']
	#
	# field_set = [Fieldset('背包类容器项属性', 'item', 'count', 'equiped')]
	#
	# form_layout = BaseContItemAdmin.form_layout + field_set
	pass


@AdminXHelper.relatedModel(SlotContItem)
class SlotContItemAdmin(BaseContItemAdmin):

	# list_display = BaseContItemAdmin.list_display + \
	# 			   ['index']
	#
	# list_editable = BaseContItemAdmin.list_editable + \
	# 			   ['index']
	#
	# field_set = [Fieldset('槽类容器项属性', 'index')]
	#
	# form_layout = BaseContItemAdmin.form_layout + field_set
	pass


@AdminXHelper.relatedModel(BaseEffect)
class BaseEffectAdmin(object):

	# list_display = ['id', 'item', 'code', 'params', 'adminDescribe']
	#
	# list_editable = ['code', 'params']
	pass
