from django.contrib import admin
from item_module.admin import PackContItemsInline, ParamsInline
from .models import *

# Register your models here.


@admin.register(LoginInfo)
class LoginInfoAdmin(admin.ModelAdmin):

	list_display = ['id', 'player', 'time', 'logout', 'ip_address']


@admin.register(PasswordRecord)
class PasswordRecordAdmin(admin.ModelAdmin):

	list_display = ['id', 'player', 'time', 'password', 'ip_address']


@admin.register(Character)
class CharacterAdmin(admin.ModelAdmin):

	list_display = ['id', 'name', 'description', 'gender', 'bust', 'face', 'battle']

	list_editable = ['name', 'description', 'gender', 'bust', 'face', 'battle']


@admin.register(Player)
class PlayerAdmin(admin.ModelAdmin):

	list_display = ['id', 'username', 'password', 'phone', 'email', 'name',
					'character', 'grade', 'create_time', 'last_refresh_time',
					'status', 'type', 'birth', 'school', 'city', 'contact',
					'description', 'is_deleted']

	list_editable = ['username', 'phone', 'email', 'name', 'grade', 'birth',
					 'school', 'city', 'contact', 'description']


@admin.register(HumanItem)
class HumanItemAdmin(admin.ModelAdmin):

	list_display = ['id', 'name', 'description', 'type',
					'itemprice', 'sell_price', 'discardable', 'tradable', 'icon',
					'max_count', 'battle_use', 'menu_use', 'adventure_use',
					'consumable', 'freeze', 'i_type']

	list_editable = ['name', 'description',
					 'sell_price', 'discardable', 'tradable', 'icon',
					 'max_count', 'battle_use', 'menu_use', 'adventure_use',
					 'consumable', 'freeze', 'i_type']

	exclude = ['type']

	# inlines = [PriceInline]


@admin.register(HumanEquip)
class HumanEquipAdmin(admin.ModelAdmin):

	list_display = ['id', 'name', 'description', 'type',
					'itemprice', 'sell_price', 'discardable', 'tradable', 'icon',
					'e_type']

	list_editable = ['name', 'description',
					 'sell_price', 'discardable', 'tradable', 'icon',
					 'e_type']

	exclude = ['type']

	inlines = [ParamsInline]


@admin.register(HumanPack)
class HumanPackAdmin(admin.ModelAdmin):

	list_display = ['id', 'type', 'capacity', 'player']

	list_editable = ['capacity']

	inlines = [PackContItemsInline]


@admin.register(HumanPackItem)
class HumanPackItemAdmin(admin.ModelAdmin):

	list_display = ['id', 'container', 'type', 'item', 'count']

	list_editable = ['item', 'count']


@admin.register(HumanPackEquip)
class HumanPackEquipAdmin(admin.ModelAdmin):

	list_display = ['id', 'container', 'type', 'item', 'count']

	list_editable = ['item', 'count']

