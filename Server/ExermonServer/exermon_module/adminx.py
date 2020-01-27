from xadmin.layout import Fieldset
# from xadmin.plugins.inline import Inline
from game_module.adminx import ParamsInline
from item_module.adminx import BaseItemAdmin, \
	UsableItemAdmin, EquipableItemAdmin, PackContainerAdmin, \
	SlotContainerAdmin, PackContItemAdmin, SlotContItemAdmin
from .models import *
import xadmin

# Register your models here.


class ExerParamBasesInline(ParamsInline):
	model = ExerParamBase


class ExerParamRatesInline(ParamsInline):
	model = ExerParamRate


class GiftParamRatesInline(ParamsInline):
	model = GiftParamRate


class ExerSkillsInline(object):

	model = ExerSkill
	extra = 0
	max_num = 3
	validate_max = 3
	style = "accordion"
	fk_name = 'o_exermon'


class ExerEquipSlotInline(object):

	model = ExerEquipSlot
	min_num = 1
	max_num = 1
	validate_min = 1
	validate_max = 1
	style = "one"


@xadmin.sites.register(Exermon)
class ExermonAdmin(BaseItemAdmin):

	list_display = BaseItemAdmin().list_display + \
				   ['star', 'subject', 'adminParamBases', 'adminParamRates',
					'e_type']

	list_editable = BaseItemAdmin().list_editable + \
					['star', 'subject', 'e_type']

	field_set = [Fieldset('艾瑟萌属性', 'star', 'subject', 'full', 'icon',
						  'battle', 'e_type')]

	form_layout = BaseItemAdmin().form_layout + field_set

	inlines = [ExerParamBasesInline, ExerParamRatesInline, ExerSkillsInline]


@xadmin.sites.register(ExerFrag)
class ExerFragAdmin(BaseItemAdmin):

	list_display = BaseItemAdmin().list_display + \
				   ['o_exermon', 'count', 'sell_price']

	list_editable = BaseItemAdmin().list_editable + \
				   ['o_exermon', 'count', 'sell_price']

	field_set = [Fieldset('艾瑟萌碎片属性', 'o_exermon', 'count', 'sell_price')]

	form_layout = BaseItemAdmin().form_layout + field_set


@xadmin.sites.register(ExerGift)
class ExerGiftAdmin(BaseItemAdmin):

	list_display = BaseItemAdmin().list_display + \
				   ['star', 'adminParamRates', 'g_type']

	list_editable = BaseItemAdmin().list_editable + \
				   ['star', 'g_type']

	field_set = [Fieldset('艾瑟萌天赋属性', 'star', 'g_type')]

	form_layout = BaseItemAdmin().form_layout + field_set

	inlines = [GiftParamRatesInline]


@xadmin.sites.register(ExerSkill)
class ExerSkillAdmin(BaseItemAdmin):

	list_display = BaseItemAdmin().list_display + \
				   ['o_exermon', 'passive', 'next_skill', 'need_count',
					'mp_cost', 'rate', 'freeze', 'max_use_count', 'target',
					'hit_type', 'atk_rate', 'def_rate', 'plus_formula',
					'icon', 'ani', 'target_ani']

	list_editable = BaseItemAdmin().list_editable + \
					['o_exermon', 'passive', 'next_skill', 'need_count',
					 'mp_cost', 'rate', 'freeze', 'max_use_count', 'target',
					 'hit_type', 'atk_rate', 'def_rate', 'plus_formula',
					 'icon', 'ani', 'target_ani']

	field_set = [Fieldset('艾瑟萌技能属性', 'o_exermon', 'passive', 'next_skill', 'need_count',
						  'mp_cost', 'rate', 'freeze', 'max_use_count', 'target',
						  'hit_type', 'atk_rate', 'def_rate', 'plus_formula',
						  'icon', 'ani', 'target_ani')]

	form_layout = BaseItemAdmin().form_layout + field_set


@xadmin.sites.register(ExerHub)
class ExerHubAdmin(PackContainerAdmin):

	list_display = PackContainerAdmin().list_display + \
				   ['player']

	field_set = [Fieldset('艾瑟萌仓库属性', 'player')]

	form_layout = PackContainerAdmin().form_layout + field_set


@xadmin.sites.register(ExerPack)
class ExerPackAdmin(PackContainerAdmin):

	list_display = PackContainerAdmin().list_display + \
				   ['player']

	field_set = [Fieldset('艾瑟萌背包属性', 'player')]

	form_layout = PackContainerAdmin().form_layout + field_set


@xadmin.sites.register(ExerSlot)
class ExerSlotAdmin(SlotContainerAdmin):

	list_display = SlotContainerAdmin().list_display + \
				   ['player']

	field_set = [Fieldset('艾瑟萌槽属性', 'player')]

	form_layout = SlotContainerAdmin().form_layout + field_set


@xadmin.sites.register(ExerEquipSlot)
class ExerSlotAdmin(SlotContainerAdmin):

	list_display = SlotContainerAdmin().list_display + \
				   ['player']

	field_set = [Fieldset('艾瑟萌槽属性', 'player')]

	form_layout = SlotContainerAdmin().form_layout + field_set


@xadmin.sites.register(ExerFragPack)
class ExerFragPackAdmin(PackContainerAdmin):

	list_display = PackContainerAdmin().list_display + \
				   ['player']

	field_set = [Fieldset('艾瑟萌碎片背包属性', 'player')]

	form_layout = PackContainerAdmin().form_layout + field_set


@xadmin.sites.register(ExerGiftPool)
class ExerGiftPoolAdmin(PackContainerAdmin):

	list_display = PackContainerAdmin().list_display + \
				   ['player']

	field_set = [Fieldset('艾瑟萌天赋池属性', 'player')]

	form_layout = PackContainerAdmin().form_layout + field_set


@xadmin.sites.register(PlayerExermon)
class PlayerExermonAdmin(PackContItemAdmin):

	list_display = PackContItemAdmin().list_display + \
				   ['player', 'nickname', 'exp', 'level']

	list_editable = PackContItemAdmin().list_editable + \
					['nickname', 'exp', 'level']

	field_set = [Fieldset('玩家艾瑟萌属性', 'nickname', 'exp', 'level')]

	exclude = PackContItemAdmin().exclude + ['player']

	form_layout = PackContItemAdmin().form_layout + field_set


@xadmin.sites.register(ExerPackItem)
class ExerPackItemAdmin(PackContItemAdmin):
	pass


@xadmin.sites.register(ExerPackEquip)
class ExerPackEquipAdmin(PackContItemAdmin):
	pass


@xadmin.sites.register(ExerSlotItem)
class ExerSlotItemAdmin(SlotContItemAdmin):

	list_display = SlotContItemAdmin().list_display + \
				   ['player', 'subject', 'exp']

	list_editable = SlotContItemAdmin().list_editable + \
					['subject', 'exp']

	field_set = [Fieldset('艾瑟萌槽项属性', 'subject', 'exp')]

	exclude = SlotContItemAdmin().exclude + ['player']

	form_layout = SlotContItemAdmin().form_layout + field_set

	inlines = [ExerEquipSlotInline]


@xadmin.sites.register(ExerEquipSlotItem)
class ExerEquipSlotItemAdmin(SlotContItemAdmin):

	list_display = SlotContItemAdmin().list_display + \
				   ['e_type']

	list_editable = SlotContItemAdmin().list_editable + \
					['e_type']

	field_set = [Fieldset('艾瑟萌装备槽项', 'e_type')]

	form_layout = SlotContItemAdmin().form_layout + field_set


@xadmin.sites.register(ExerFragPackItem)
class ExerFragPackItemAdmin(PackContItemAdmin):
	pass


@xadmin.sites.register(PlayerExerGift)
class PlayerExerGiftAdmin(PackContItemAdmin):
	pass
