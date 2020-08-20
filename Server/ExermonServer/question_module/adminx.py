# from xadmin.plugins.inline import Inline
from utils.admin_utils import AdminXHelper
from .models import *

# Register your models here.


@AdminXHelper.registerBaseInline(BaseQuesChoice)
class BaseQuesChoicesInline(object):

	style = "table"


@AdminXHelper.registerBaseInline(BaseQuesPicture)
class QuesPicturesInline(object):

	style = "table"


@AdminXHelper.registerBaseInline(BaseQuestion)
class BaseQuestionsInline(object):

	style = "accordion"


@AdminXHelper.registerBaseInline(WrongItem)
class WrongItemsInline(object):

	model = WrongItem
	style = "table"


@AdminXHelper.relatedModel(BaseQuestion)
class BaseQuestionAdmin(object): pass


@AdminXHelper.relatedModel(SelectingQuestion)
class SelectingQuestionAdmin(BaseQuestionAdmin): pass


@AdminXHelper.relatedModel(CorrectingQuestion)
class CorrectingQuestionAdmin(BaseQuestionAdmin): pass


@AdminXHelper.relatedModel(ElementQuestion)
class ElementQuestionAdmin(BaseQuestionAdmin): pass


@AdminXHelper.relatedModel(GroupQuestion)
class GroupQuestionAdmin(BaseQuestionAdmin): pass


@AdminXHelper.relatedModel(BaseQuesRecord)
class BaseQuesRecordAdmin(object): pass


@AdminXHelper.relatedModel(BaseQuesReport)
class BaseQuesReportAdmin(object): pass


# region SelectingQuestion


@AdminXHelper.relatedModel(GeneralQuestion)
class GeneralQuestionAdmin(SelectingQuestionAdmin): pass


@AdminXHelper.relatedModel(GeneralQuesRecord)
class GeneralQuesRecordAdmin(BaseQuesRecordAdmin): pass


@AdminXHelper.relatedModel(GeneralQuesReport)
class GeneralQuesReportAdmin(BaseQuesReportAdmin): pass


@AdminXHelper.relatedModel(PlotQuestion)
class PlotQuestionAdmin(SelectingQuestionAdmin): pass


# endregion


# region GroupQuestion


@AdminXHelper.relatedModel(ListeningQuestion)
class ListeningQuestionAdmin(GroupQuestionAdmin): pass


@AdminXHelper.relatedModel(ListeningQuesRecord)
class ListeningQuesRecordAdmin(BaseQuesRecordAdmin): pass


@AdminXHelper.relatedModel(ListeningQuesReport)
class ListeningQuesReportAdmin(BaseQuesReportAdmin): pass


@AdminXHelper.relatedModel(ReadingQuestion)
class ReadingQuestionAdmin(GroupQuestionAdmin): pass


@AdminXHelper.relatedModel(ReadingQuesRecord)
class ReadingQuesRecordAdmin(BaseQuesRecordAdmin): pass


@AdminXHelper.relatedModel(ReadingQuesReport)
class ReadingQuesReportAdmin(BaseQuesReportAdmin): pass


# endregion


# region GroupQuestion


@AdminXHelper.relatedModel(Word)
class WordQuestionAdmin(GroupQuestionAdmin): pass


@AdminXHelper.relatedModel(WordRecord)
class WordRecordAdmin(BaseQuesRecordAdmin): pass


@AdminXHelper.relatedModel(WordReport)
class WordReportAdmin(BaseQuesReportAdmin): pass


@AdminXHelper.relatedModel(Phrase)
class PhraseQuestionAdmin(GroupQuestionAdmin): pass


@AdminXHelper.relatedModel(PhraseRecord)
class PhraseRecordAdmin(BaseQuesRecordAdmin): pass


@AdminXHelper.relatedModel(PhraseReport)
class PhraseReportAdmin(BaseQuesReportAdmin): pass


# endregion


