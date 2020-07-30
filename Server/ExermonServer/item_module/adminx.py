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
class CurrencyAdmin(object): pass


@AdminXHelper.relatedModel(BaseItem)
class BaseItemAdmin(object): pass


@AdminXHelper.relatedModel(LimitedItem)
class LimitedItemAdmin(BaseItemAdmin): pass


@AdminXHelper.relatedModel(UsableItem)
class UsableItemAdmin(LimitedItemAdmin): pass


@AdminXHelper.relatedModel(EquipableItem)
class EquipableItemAdmin(LimitedItemAdmin): pass


@AdminXHelper.relatedModel(BaseContainer)
class BaseContainerAdmin(object): pass


@AdminXHelper.relatedModel(PackContainer)
class PackContainerAdmin(BaseContainerAdmin): pass


@AdminXHelper.relatedModel(SlotContainer)
class SlotContainerAdmin(BaseContainerAdmin): pass


@AdminXHelper.relatedModel(BaseContItem)
class BaseContItemAdmin(object): pass


@AdminXHelper.relatedModel(PackContItem)
class PackContItemAdmin(BaseContItemAdmin): pass


@AdminXHelper.relatedModel(SlotContItem)
class SlotContItemAdmin(BaseContItemAdmin): pass


@AdminXHelper.relatedModel(BaseEffect)
class BaseEffectAdmin(object): pass
