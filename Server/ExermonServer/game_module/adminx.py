
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


@AdminXHelper.relatedModel(GameTip)
class GameTipAdmin(object): pass


@AdminXHelper.relatedModel(Subject)
class SubjectAdmin(object): pass


@AdminXHelper.relatedModel(BaseParam)
class BaseParamAdmin(object): pass


@AdminXHelper.relatedModel(UsableItemType)
class UsableItemTypeAdmin(object): pass


@AdminXHelper.relatedModel(HumanEquipType)
class HumanEquipTypeAdmin(object): pass


@AdminXHelper.relatedModel(ExerEquipType)
class ExerEquipTypeAdmin(object): pass


@AdminXHelper.relatedModel(ExerStar)
class ExerStarAdmin(object): pass


@AdminXHelper.relatedModel(ExerGiftStar)
class ExerGiftStarAdmin(object): pass


@AdminXHelper.relatedModel(ItemStar)
class ItemStarAdmin(object): pass


@AdminXHelper.relatedModel(QuestionStar)
class QuestionStarAdmin(object): pass


@AdminXHelper.relatedModel(GameVersion)
class GameVersionAdmin(object): pass


@AdminXHelper.relatedModel(GameConfigure)
class GameConfigureAdmin(object): pass
