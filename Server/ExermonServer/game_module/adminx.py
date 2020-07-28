import xadmin

from .models import *
from utils.model_utils import AdminXHelper

# Register your models here.

DEFAULT_PARAM_COUNT = 6


@AdminXHelper.registerBaseInline(ParamValue)
class ParamsInline(object):

	min_num = DEFAULT_PARAM_COUNT
	max_num = DEFAULT_PARAM_COUNT
	validate_min = DEFAULT_PARAM_COUNT
	validate_max = DEFAULT_PARAM_COUNT
	style = "table"


@AdminXHelper.registerBaseInline(EquipParamValue)
class EquipParamsInline(ParamsInline):

	min_num = 0
	validate_min = 0


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

#
# class ExerParamBaseRangesInline(ParamsInline):
# 	model = ExerParamBaseRange
#
#
# class ExerParamRateRangesInline(ParamsInline):
# 	model = ExerParamRateRange
#
#
# class ExerGiftParamRateRangesInline(ParamsInline):
# 	model = ExerGiftParamRateRange


@AdminXHelper.relatedModel(GameTip)
class GameTipAdmin(object):
	# list_display = ['id', 'name', 'description']
	#
	# list_editable = ['name', 'description']
	pass


@AdminXHelper.relatedModel(Subject)
class SubjectAdmin(object):
	# list_display = ['id', 'name', 'max_score', 'force', 'color', 'adminColor', 'configure']
	#
	# list_editable = ['name', 'max_score', 'force', 'color', 'configure']
	pass


@AdminXHelper.relatedModel(BaseParam)
class BaseParamAdmin(object):
	# list_display = ['id', 'name', 'description', 'attr', 'color', 'adminColor', 'configure']
	#
	# list_editable = ['name', 'description', 'attr', 'color', 'configure']
	pass


@AdminXHelper.relatedModel(UsableItemType)
class UsableItemTypeAdmin(object):
	# list_display = ['id', 'name', 'description', 'configure']
	#
	# list_editable = ['name', 'description', 'configure']
	pass


@AdminXHelper.relatedModel(HumanEquipType)
class HumanEquipTypeAdmin(object):
	# list_display = ['id', 'name', 'description', 'configure']
	#
	# list_editable = ['name', 'description', 'configure']
	pass


@AdminXHelper.relatedModel(ExerEquipType)
class ExerEquipTypeAdmin(object):
	# list_display = ['id', 'name', 'description', 'configure']
	#
	# list_editable = ['name', 'description', 'configure']
	pass


@AdminXHelper.relatedModel(ExerStar)
class ExerStarAdmin(object):
	# list_display = ['id', 'name', 'color',
	# 				'adminColor', 'max_level', 'adminLevelExpFactors',
	# 				'adminParamBaseRanges', 'adminParamRateRanges', 'configure']
	#
	# list_editable = ['name', 'color', 'max_level', 'configure']
	#
	# inlines = [ExerParamBaseRangesInline, ExerParamRateRangesInline]
	pass


@AdminXHelper.relatedModel(ExerGiftStar)
class ExerGiftStarAdmin(object):
	# list_display = ['id', 'name', 'color', 'adminColor', 'configure']
	#
	# list_editable = ['name', 'color', 'configure']
	#
	# inlines = [ExerGiftParamRateRangesInline]
	pass


@AdminXHelper.relatedModel(ItemStar)
class ItemStarAdmin(object):
	# list_display = ['id', 'name', 'color', 'adminColor', 'configure']
	#
	# list_editable = ['name', 'color', 'configure']
	pass


@AdminXHelper.relatedModel(QuestionStar)
class QuestionStarAdmin(object):
	# list_display = ['id', 'name', 'color', 'adminColor', 'level', 'weight',
	# 				'exp_incr', 'gold_incr', 'std_time', 'min_time']
	#
	# list_editable = ['name', 'color', 'level', 'weight', 'exp_incr', 'gold_incr',
	# 				 'std_time', 'min_time', 'configure']
	pass


@AdminXHelper.relatedModel(GameVersion)
class GameVersionAdmin(object):
	# list_display = ['id', 'main_version', 'sub_version', 'update_time',
	# 				'update_note', 'description', 'is_used', 'configure']
	#
	# list_editable = ['main_version', 'sub_version', 'is_used', 'configure']
	pass


@AdminXHelper.relatedModel(GameConfigure)
class GameConfigureAdmin(object):
	# list_display = ['id', 'name', 'eng_name', 'gold', 'ticket', 'bound_ticket']
	#
	# list_editable = ['name', 'eng_name', 'gold', 'ticket', 'bound_ticket']

	# inlines = [BaseParamsInline, SubjectsInline, UsableItemTypesInline,
	# 		   HumanEquipTypesInline, ExerEquipTypesInline,
	# 		   ExerStarsInline, ExerGiftStarsInline]
	pass