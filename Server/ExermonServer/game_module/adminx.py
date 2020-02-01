import xadmin

from .models import *

# Register your models here.

DEFAULT_PARAM_COUNT = 6


class ParamsInline(object):

	min_num = DEFAULT_PARAM_COUNT
	max_num = DEFAULT_PARAM_COUNT
	validate_min = DEFAULT_PARAM_COUNT
	validate_max = DEFAULT_PARAM_COUNT
	style = "table"


# class BaseParamsInline(object):
# 	model = BaseParam
# 	style = "table"
#
#
# class SubjectsInline(object):
# 	model = Subject
# 	style = "table"
#
#
# class UsableItemTypesInline(object):
# 	model = UsableItemType
# 	style = "table"
#
#
# class HumanEquipTypesInline(object):
# 	model = HumanEquipType
# 	style = "table"
#
#
# class ExerEquipTypesInline(object):
# 	model = ExerEquipType
# 	style = "table"
#
#
# class ExerStarsInline(object):
# 	model = ExerStar
# 	style = "accordion"
#
#
# class ExerGiftStarsInline(object):
# 	model = ExerGiftStar
# 	style = "accordion"


class ExerParamBaseRangesInline(ParamsInline):
	model = ExerParamBaseRange


class ExerParamRateRangesInline(ParamsInline):
	model = ExerParamRateRange


class ExerGiftParamRateRangesInline(ParamsInline):
	model = ExerGiftParamRateRange


@xadmin.sites.register(Subject)
class SubjectAdmin(object):
	list_display = ['id', 'name', 'max_score', 'force', 'adminColor', 'configure']

	list_editable = ['name', 'max_score', 'force', 'configure']


@xadmin.sites.register(BaseParam)
class BaseParamAdmin(object):
	list_display = ['id', 'name', 'description', 'attr', 'adminColor', 'configure']

	list_editable = ['name', 'description', 'attr', 'configure']


@xadmin.sites.register(UsableItemType)
class UsableItemTypeAdmin(object):
	list_display = ['id', 'name', 'description', 'configure']

	list_editable = ['name', 'description', 'configure']


@xadmin.sites.register(HumanEquipType)
class HumanEquipTypeAdmin(object):
	list_display = ['id', 'name', 'description', 'configure']

	list_editable = ['name', 'description', 'configure']


@xadmin.sites.register(ExerEquipType)
class ExerEquipTypeAdmin(object):
	list_display = ['id', 'name', 'description', 'configure']

	list_editable = ['name', 'description', 'configure']


@xadmin.sites.register(ExerStar)
class ExerStarAdmin(object):
	list_display = ['id', 'name',
					'adminColor', 'max_level', 'adminLevelExpFactors',
					'adminParamBaseRanges', 'adminParamRateRanges', 'configure']

	list_editable = ['name', 'max_level', 'configure']

	inlines = [ExerParamBaseRangesInline, ExerParamRateRangesInline]


@xadmin.sites.register(ExerGiftStar)
class ExerGiftStarAdmin(object):
	list_display = ['id', 'name', 'adminColor', 'configure']

	list_editable = ['name', 'configure']

	inlines = [ExerGiftParamRateRangesInline]


@xadmin.sites.register(GameVersion)
class GameVersionAdmin(object):
	list_display = ['id', 'main_version', 'sub_version', 'update_time',
					'update_note', 'description', 'is_used', 'configure']

	list_editable = ['main_version', 'sub_version', 'is_used', 'configure']


@xadmin.sites.register(GameConfigure)
class GameConfigureAdmin(object):
	list_display = ['id', 'name', 'eng_name', 'gold', 'ticket', 'bound_ticket']

	list_editable = ['name', 'eng_name', 'gold', 'ticket', 'bound_ticket']

	# inlines = [BaseParamsInline, SubjectsInline, UsableItemTypesInline,
	# 		   HumanEquipTypesInline, ExerEquipTypesInline,
	# 		   ExerStarsInline, ExerGiftStarsInline]