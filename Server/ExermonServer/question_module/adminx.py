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


@AdminXHelper.registerBaseInline(SelectingQuestion)
class BaseQuestionsInline(object):

	style = "accordion"


@AdminXHelper.registerBaseInline(GeneralQuesPicture)
class QuesPicturesInline(object):

	model = GeneralQuesPicture
	style = "table"


@AdminXHelper.relatedModel(SelectingQuestion)
class BaseQuestionAdmin(object): pass


@AdminXHelper.relatedModel(GroupQuestion)
class GroupQuestionAdmin(object): pass


@AdminXHelper.relatedModel(GeneralQuestion)
class QuestionAdmin(BaseQuestionAdmin): pass


@AdminXHelper.relatedModel(BaseQuesReport)
class QuesReportAdmin(object): pass