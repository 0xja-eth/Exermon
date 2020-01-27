from django.contrib import admin

from .models import *

# Register your models here.


class ExerParamBaseRangesInline(admin.TabularInline):

	model = ExerParamBaseRange
	min_num = 6
	max_num = 6


class ExerParamRateRangesInline(admin.TabularInline):

	model = ExerParamRateRange
	min_num = 6
	max_num = 6


class ExerGiftParamRateRangesInline(admin.TabularInline):

	model = ExerGiftParamRateRange
	min_num = 6
	max_num = 6


@admin.register(Subject)
class SubjectAdmin(admin.ModelAdmin):
	list_display = ['id', 'configure', 'name', 'description',
					'max_score', 'force']

	list_editable = ['configure', 'name', 'description',
					 'max_score', 'force']


@admin.register(BaseParam)
class BaseParamAdmin(admin.ModelAdmin):
	list_display = ['id', 'configure', 'name', 'description', 'attr']

	list_editable = ['configure', 'name', 'description', 'attr']


@admin.register(UsableItemType)
class UsableItemTypeAdmin(admin.ModelAdmin):
	list_display = ['id', 'configure', 'name', 'description']

	list_editable = ['configure', 'name', 'description']


@admin.register(HumanEquipType)
class HumanEquipTypeAdmin(admin.ModelAdmin):
	list_display = ['id', 'configure', 'name', 'description']

	list_editable = ['configure', 'name', 'description']


@admin.register(ExerEquipType)
class ExerEquipTypeAdmin(admin.ModelAdmin):
	list_display = ['id', 'configure', 'name', 'description']

	list_editable = ['configure', 'name', 'description']


@admin.register(ExerStar)
class ExerStarAdmin(admin.ModelAdmin):
	list_display = ['id', 'name',
					'adminColor', 'max_level', 'adminLevelExpFactors',
					'adminParamBaseRanges', 'adminParamRateRanges', 'configure']

	list_editable = ['name', 'max_level', 'configure']

	inlines = [ExerParamBaseRangesInline, ExerParamRateRangesInline]


@admin.register(ExerGiftStar)
class ExerGiftStarAdmin(admin.ModelAdmin):
	list_display = ['id', 'name', 'adminColor', 'configure']

	list_editable = ['name', 'configure']

	inlines = [ExerGiftParamRateRangesInline]


@admin.register(GameVersion)
class GameVersionAdmin(admin.ModelAdmin):
	list_display = ['id', 'main_version', 'sub_version', 'update_time',
					'update_note', 'description', 'configure', 'is_used']

	list_editable = ['main_version', 'sub_version', 'configure', 'is_used']


@admin.register(GameConfigure)
class GameConfigureAdmin(admin.ModelAdmin):
	list_display = ['id', 'name', 'eng_name', 'gold', 'ticket', 'bound_ticket']

	list_editable = ['name', 'eng_name', 'gold', 'ticket', 'bound_ticket']
