from django.contrib import admin

from .models import *

# Register your models here.


class BaseParamAdmin(admin.ModelAdmin):
	list_display = ['id', 'name', 'attr', 'description']

	list_editable = ['name', 'attr', 'description']


class GameVersionAdmin(admin.ModelAdmin):
	list_display = ['id', 'main_version', 'sub_version', 'update_time',
					'update_note', 'description', 'is_used']

	list_editable = ['main_version', 'sub_version', 'is_used']


class GameTermAdmin(admin.ModelAdmin):
	list_display = ['id', 'name', 'eng_name']

	list_editable = ['name', 'eng_name']


admin.site.register(BaseParam, BaseParamAdmin)

admin.site.register(GameVersion, GameVersionAdmin)

admin.site.register(GameTerm, GameTermAdmin)
