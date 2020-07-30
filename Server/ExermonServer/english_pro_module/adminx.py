#-*-coding:GBK -*-

from question_module.adminx import *
from item_module.adminx import *
from .models import *

# Register your models here.


@AdminXHelper.registerBaseInline(WrongItem)
class WrongItemsInline(object):

	model = WrongItem
	style = "table"


@AdminXHelper.registerBaseInline(ExerProMapStage)
class MapStagesInline(object):

	model = ExerProMapStage
	style = "accordion"


@AdminXHelper.relatedModel(ListeningQuesChoice)
class ListeningQuesChoiceAdmin(object): pass


@AdminXHelper.relatedModel(ListeningSubQuestion)
class ListeningSubQuestionAdmin(BaseQuestionAdmin): pass


@AdminXHelper.relatedModel(PlotQuestion)
class PlotQuestionAdmin(BaseQuestionAdmin): pass


@AdminXHelper.relatedModel(PlotQuesChoice)
class PlotQuesChoiceAdmin(object): pass


@AdminXHelper.relatedModel(ListeningQuestion)
class ListeningQuestionAdmin(GroupQuestionAdmin): pass


@AdminXHelper.relatedModel(PhraseQuestion)
class PhraseQuestionAdmin(object): pass


@AdminXHelper.relatedModel(CorrectionQuestion)
class CorrectionQuestionAdmin(object): pass


@AdminXHelper.relatedModel(WrongItem)
class WrongItemAdmin(object): pass


@AdminXHelper.relatedModel(Word)
class WordAdmin(object): pass


@AdminXHelper.relatedModel(WordRecord)
class WordRecordAdmin(object): pass


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

