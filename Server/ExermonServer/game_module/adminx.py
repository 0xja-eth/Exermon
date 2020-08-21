
from .models import *
from utils.admin_utils import AdminXHelper

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


@AdminXHelper.relatedModel(GameData)
class GameDataAdmin(object): pass


@AdminXHelper.relatedModel(StaticData)
class StaticDataAdmin(object): pass


@AdminXHelper.relatedModel(DynamicData)
class DynamicDataAdmin(object): pass


@AdminXHelper.relatedModel(GameTip)
class GameTipAdmin(DynamicDataAdmin): pass


@AdminXHelper.relatedModel(Subject)
class SubjectAdmin(StaticDataAdmin): pass


@AdminXHelper.relatedModel(BaseParam)
class BaseParamAdmin(StaticDataAdmin): pass


@AdminXHelper.relatedModel(GameItemType)
class UsableItemTypeAdmin(StaticDataAdmin): pass


@AdminXHelper.relatedModel(GameEquipType)
class ExerEquipTypeAdmin(StaticDataAdmin): pass


@AdminXHelper.relatedModel(ExerStar)
class ExerStarAdmin(StaticDataAdmin): pass


@AdminXHelper.relatedModel(ExerGiftStar)
class ExerGiftStarAdmin(StaticDataAdmin): pass


@AdminXHelper.relatedModel(ItemStar)
class ItemStarAdmin(StaticDataAdmin): pass


@AdminXHelper.relatedModel(QuestionStar)
class QuestionStarAdmin(StaticDataAdmin): pass


@AdminXHelper.relatedModel(GameVersion)
class GameVersionAdmin(object): pass


@AdminXHelper.relatedModel(GameConfigure)
class GameConfigureAdmin(object): pass
