from item_module.adminx import *
from utils.admin_utils import AdminXHelper
from .models import *

# Register your models here.


@AdminXHelper.registerBaseInline(ExerProMapStage)
class MapStagesInline(object):

	model = ExerProMapStage
	style = "accordion"


@AdminXHelper.relatedModel(Antonym)
class AntonymAdmin(object): pass


@AdminXHelper.relatedModel(BaseExerProItem)
class BaseExerProItemAdmin(BaseItemAdmin): pass


@AdminXHelper.relatedModel(ExerProItemStar)
class ExerProItemStarAdmin(object): pass


@AdminXHelper.relatedModel(ExerProItem)
class ExerProItemAdmin(BaseExerProItemAdmin): pass


@AdminXHelper.relatedModel(ExerProPotion)
class ExerProPotionAdmin(BaseExerProItemAdmin): pass


@AdminXHelper.relatedModel(ExerProCard)
class ExerProCardAdmin(BaseItemAdmin): pass


@AdminXHelper.relatedModel(ExerProEnemy)
class ExerProEnemyAdmin(BaseItemAdmin): pass


@AdminXHelper.relatedModel(ExerProState)
class ExerProStateAdmin(BaseItemAdmin): pass


@AdminXHelper.relatedModel(FirstCardGroup)
class FirstCardGroupAdmin(object): pass


@AdminXHelper.relatedModel(ExerProMap)
class ExerProMapAdmin(object): pass


@AdminXHelper.relatedModel(NodeType)
class NodeTypeAdmin(object): pass


@AdminXHelper.relatedModel(ExerProMapStage)
class ExerProMapStageAdmin(object): pass


@AdminXHelper.relatedModel(ExerProRecord)
class ExerProRecordAdmin(object): pass

