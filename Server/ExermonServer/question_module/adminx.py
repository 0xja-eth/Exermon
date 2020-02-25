from xadmin.layout import Fieldset
# from xadmin.plugins.inline import Inline
from game_module.adminx import ParamsInline
from item_module.adminx import *
from .models import *
import xadmin

# Register your models here.


# class QuesSugarPriceInline(object):
#
# 	model = QuesSugarPrice
# 	min_num = 1
# 	max_num = 1
# 	validate_min = 1
# 	validate_max = 1
# 	style = "one"


class QuesChoicesInline(object):

	model = QuesChoice
	style = "table"


class QuesPicturesInline(object):

	model = QuesPicture
	style = "table"


class QuesSugarParamsInline(ParamsInline): model = QuesSugarParam


class QuesSugarPackItemsInline(BaseContItemsInline): model = QuesSugarPackItem


class QuesSugarPriceInline(CurrencyInline): model = QuesSugarPrice


# @xadmin.sites.register(QuesSugarPrice)
# class QuesSugarPriceAdmin(object):
#
# 	list_display = ['id', 'sugar', 'gold', 'ticket', 'bound_ticket']
#
# 	list_editable = ['sugar', 'gold', 'ticket', 'bound_ticket']


@xadmin.sites.register(Question)
class QuestionAdmin(object):

	list_display = ['id', 'title', 'star', 'score', 'subject',
					'create_time', 'type', 'for_test', 'status', 'is_deleted']

	list_editable = ['star', 'score', 'subject',
					 'type', 'for_test', 'status', 'is_deleted']

	# form_layout = [Fieldset('题目主体', 'name', 'description')]

	inlines = [QuesChoicesInline, QuesPicturesInline]


@xadmin.sites.register(QuesReport)
class QuesReportAdmin(object):

	list_display = ['id', 'player', 'question', 'type', 'description']


@xadmin.sites.register(QuesSugar)
class QuesSugarAdmin(BaseItemAdmin):

	list_display = BaseItemAdmin.list_display + \
				   ['question', 'sell_price',
					'get_rate', 'get_count']

	list_editable = BaseItemAdmin.list_editable + \
				   ['question', 'sell_price',
					'get_rate', 'get_count']

	field_set = [Fieldset('题目糖属性', 'question',
						  'sell_price', 'get_rate', 'get_count')]

	form_layout = BaseItemAdmin.form_layout + field_set

	inlines = [QuesSugarPriceInline, QuesSugarParamsInline]


@xadmin.sites.register(QuesSugarPack)
class QuesSugarPackAdmin(PackContainerAdmin):

	list_display = PackContainerAdmin.list_display + \
				   ['player']

	field_set = [Fieldset('题目糖背包属性', 'player')]

	form_layout = PackContainerAdmin.form_layout + field_set

	inlines = [QuesSugarPackItemsInline]


@xadmin.sites.register(QuesSugarPackItem)
class QuesSugarPackItemAdmin(PackContItemAdmin):
	pass


xadmin.site.register(QuesSugarPrice, CurrencyAdmin)
