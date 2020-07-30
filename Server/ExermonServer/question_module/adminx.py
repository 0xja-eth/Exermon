from xadmin.layout import Fieldset
# from xadmin.plugins.inline import Inline
from game_module.adminx import ParamsInline
from item_module.adminx import *
from .models import *
import xadmin

# Register your models here.


@AdminXHelper.registerBaseInline(BaseQuesChoice)
class BaseQuesChoicesInline(object):

	style = "table"


@AdminXHelper.registerBaseInline(BaseQuestion)
class BaseQuestionsInline(object):

	style = "accordion"


@AdminXHelper.registerBaseInline(QuesPicture)
class QuesPicturesInline(object):

	model = QuesPicture
	style = "table"


@AdminXHelper.relatedModel(BaseQuestion)
class BaseQuestionAdmin(object): pass


@AdminXHelper.relatedModel(GroupQuestion)
class GroupQuestionAdmin(object): pass


@AdminXHelper.relatedModel(Question)
class QuestionAdmin(BaseQuestionAdmin): pass


@AdminXHelper.relatedModel(QuesReport)
class QuesReportAdmin(object): pass


@AdminXHelper.relatedModel(QuesSugar)
class QuesSugarAdmin(BaseItemAdmin): pass


@AdminXHelper.relatedModel(QuesSugarPack)
class QuesSugarPackAdmin(PackContainerAdmin): pass


@AdminXHelper.relatedModel(QuesSugarPackItem)
class QuesSugarPackItemAdmin(PackContItemAdmin): pass


xadmin.site.register(QuesSugarPrice, CurrencyAdmin)
