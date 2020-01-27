from django.contrib import admin
from .models import *

# Register your models here.


class ParamsInline(admin.TabularInline):

	model = EquipParam
	extra = 6


class PriceInline(admin.StackedInline):
	model = ItemPrice


class PackContItemsInline(admin.TabularInline):

	model = PackContItem


class SlotContItemsInline(admin.TabularInline):

	model = PackContItem


@admin.register(ItemPrice)
class ItemPriceAdmin(admin.ModelAdmin):

	list_display = ['id', 'gold', 'ticket', 'bound_ticket']

	list_editable = ['gold', 'ticket', 'bound_ticket']


@admin.register(BaseItem)
class BaseItemAdmin(admin.ModelAdmin):

	list_display = ['id', 'name', 'description', 'type', 'target']

	list_editable = ['name', 'description']


@admin.register(LimitedItem)
class LimitedItemAdmin(admin.ModelAdmin):

	list_display = ['id', 'name', 'description', 'type', 'target',
					'itemprice', 'sell_price', 'discardable', 'tradable', 'icon']

	list_editable = ['name', 'description', 'sell_price', 'discardable', 'tradable', 'icon']

	# inlines = [PriceInline]


@admin.register(UsableItem)
class UsableItemAdmin(admin.ModelAdmin):

	list_display = ['id', 'name', 'description', 'type', 'target',
					'itemprice', 'sell_price', 'discardable', 'tradable', 'icon',
					'max_count', 'battle_use', 'menu_use', 'adventure_use',
					'consumable', 'freeze', 'i_type']

	list_editable = ['name', 'description', 'sell_price', 'discardable', 'tradable', 'icon',
					'max_count', 'battle_use', 'menu_use', 'adventure_use',
					'consumable', 'freeze', 'i_type']


@admin.register(EquipParam)
class EquipParamAdmin(admin.ModelAdmin):

	list_display = ['id', 'param', 'value', 'getValue']

	list_editable = ['param', 'value']


@admin.register(EquipableItem)
class EquipableItemAdmin(admin.ModelAdmin):

	list_display = ['id', 'name', 'description', 'type', 'target',
					'itemprice', 'sell_price', 'discardable', 'tradable', 'icon']

	list_editable = ['name', 'description', 'sell_price', 'discardable', 'tradable', 'icon']

	inlines = [ParamsInline]

