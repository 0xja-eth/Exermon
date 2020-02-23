from xadmin.layout import Fieldset
# from xadmin.plugins.inline import Inline
from game_module.adminx import ParamsInline, EquipParamsInline
from item_module.adminx import *
from .models import *
import xadmin

# Register your models here.


class ExerParamBasesInline(ParamsInline): model = ExerParamBase


class ExerParamRatesInline(ParamsInline): model = ExerParamRate


class GiftParamRatesInline(ParamsInline): model = GiftParamRate


class ExerItemEffectsInline(BaseEffectsInline): model = ExerItemEffect


class ExerSkillEffectsInline(BaseEffectsInline): model = ExerSkillEffect


class ExerEquipParamsInline(EquipParamsInline): model = ExerEquipParam


class ExerEquipSlotItemsInline(BaseContItemsInline): model = ExerEquipSlotItem


class PlayerExerGiftsInline(BaseContItemsInline): model = PlayerExerGift


class ExerFragPackItemsInline(BaseContItemsInline): model = ExerFragPackItem


class ExerSlotItemsInline(BaseContItemsInline): model = ExerSlotItem


class ExerPackItemsInline(BaseContItemsInline): model = ExerPackItem


class ExerPackEquipsInline(BaseContItemsInline): model = ExerPackEquip


class PlayerExermonsInline(BaseContItemsInline): model = PlayerExermon


class ExerSkillSlotItemsInline(BaseContItemsInline): model = ExerSkillSlotItem


class ExerSkillsInline(object):

	model = ExerSkill
	extra = 0
	max_num = 3
	validate_max = 3
	style = "accordion"
	fk_name = 'o_exermon'


# class ExerEquipSlotInline(object):
#
# 	model = ExerEquipSlot
# 	min_num = 1
# 	max_num = 1
# 	validate_min = 1
# 	validate_max = 1
# 	style = "one"


@xadmin.sites.register(Exermon)
class ExermonAdmin(BaseItemAdmin):

	list_display = BaseItemAdmin.list_display + \
				   ['animal', 'star', 'subject', 'adminParamBases',
					'adminParamRates', 'e_type']

	list_editable = BaseItemAdmin.list_editable + \
					['animal', 'star', 'subject', 'e_type']

	field_set = [Fieldset('艾瑟萌属性', 'star', 'subject', 'full', 'icon',
						  'battle', 'e_type')]

	form_layout = BaseItemAdmin.form_layout + field_set

	inlines = [ExerParamBasesInline, ExerParamRatesInline, ExerSkillsInline]


@xadmin.sites.register(ExerFrag)
class ExerFragAdmin(BaseItemAdmin):

	list_display = BaseItemAdmin.list_display + \
				   ['o_exermon', 'count', 'sell_price']

	list_editable = BaseItemAdmin.list_editable + \
				   ['o_exermon', 'count', 'sell_price']

	field_set = [Fieldset('艾瑟萌碎片属性', 'o_exermon', 'count', 'sell_price')]

	form_layout = BaseItemAdmin.form_layout + field_set


@xadmin.sites.register(ExerGift)
class ExerGiftAdmin(BaseItemAdmin):

	list_display = BaseItemAdmin.list_display + \
				   ['star', 'color', 'adminColor', 'adminParamRates', 'g_type']

	list_editable = BaseItemAdmin.list_editable + \
				   ['star', 'color', 'g_type']

	field_set = [Fieldset('艾瑟萌天赋属性', 'star', 'color', 'g_type')]

	form_layout = BaseItemAdmin.form_layout + field_set

	inlines = [GiftParamRatesInline]


@xadmin.sites.register(ExerSkill)
class ExerSkillAdmin(BaseItemAdmin):

	list_display = BaseItemAdmin.list_display + \
				   ['o_exermon', 'passive', 'next_skill', 'need_count',
					'mp_cost', 'rate', 'freeze', 'max_use_count', 'target_type',
					'hit_type', 'atk_rate', 'def_rate', 'plus_formula', 'adminEffects']

	list_editable = BaseItemAdmin.list_editable + \
					['o_exermon', 'passive', 'next_skill', 'need_count',
					 'mp_cost', 'rate', 'freeze', 'max_use_count', 'target_type',
					 'hit_type', 'atk_rate', 'def_rate', 'plus_formula']

	field_set = [Fieldset('艾瑟萌技能属性', 'o_exermon', 'passive', 'next_skill', 'need_count',
						  'mp_cost', 'rate', 'freeze', 'max_use_count', 'target_type',
						  'hit_type', 'atk_rate', 'def_rate', 'plus_formula',
						  'icon', 'ani', 'target_ani')]

	form_layout = BaseItemAdmin.form_layout + field_set

	inlines = [ExerSkillEffectsInline]


@xadmin.sites.register(ExerItem)
class ExerItemAdmin(UsableItemAdmin):
	inlines = UsableItemAdmin.inlines + [ExerItemEffectsInline]


@xadmin.sites.register(ExerEquip)
class ExerEquipAdmin(EquipableItemAdmin):
	inlines = EquipableItemAdmin.inlines + [ExerEquipParamsInline]


@xadmin.sites.register(ExerHub)
class ExerHubAdmin(PackContainerAdmin):

	list_display = PackContainerAdmin.list_display + \
				   ['player']

	field_set = [Fieldset('艾瑟萌仓库属性', 'player')]

	form_layout = PackContainerAdmin.form_layout + field_set

	inlines = [PlayerExermonsInline]


@xadmin.sites.register(ExerPack)
class ExerPackAdmin(PackContainerAdmin):

	list_display = PackContainerAdmin.list_display + \
				   ['player']

	field_set = [Fieldset('艾瑟萌背包属性', 'player')]

	form_layout = PackContainerAdmin.form_layout + field_set

	inlines = [ExerPackItemsInline, ExerPackEquipsInline]


@xadmin.sites.register(ExerSlot)
class ExerSlotAdmin(SlotContainerAdmin):

	list_display = SlotContainerAdmin.list_display + \
				   ['player']

	field_set = [Fieldset('艾瑟萌槽属性', 'player')]

	form_layout = SlotContainerAdmin.form_layout + field_set

	inlines = [ExerSlotItemsInline]


@xadmin.sites.register(ExerEquipSlot)
class ExerEquipSlotAdmin(SlotContainerAdmin):

	list_display = SlotContainerAdmin.list_display + \
				   ['exer_slot']

	field_set = [Fieldset('艾瑟萌槽属性', 'exer_slot')]

	form_layout = SlotContainerAdmin.form_layout + field_set

	inlines = [ExerEquipSlotItemsInline]


@xadmin.sites.register(ExerFragPack)
class ExerFragPackAdmin(PackContainerAdmin):

	list_display = PackContainerAdmin.list_display + \
				   ['player']

	field_set = [Fieldset('艾瑟萌碎片背包属性', 'player')]

	form_layout = PackContainerAdmin.form_layout + field_set

	inlines = [ExerFragPackItemsInline]


@xadmin.sites.register(ExerGiftPool)
class ExerGiftPoolAdmin(PackContainerAdmin):

	list_display = PackContainerAdmin.list_display + \
				   ['player']

	field_set = [Fieldset('艾瑟萌天赋池属性', 'player')]

	form_layout = PackContainerAdmin.form_layout + field_set

	inlines = [PlayerExerGiftsInline]


@xadmin.sites.register(PlayerExermon)
class PlayerExermonAdmin(PackContItemAdmin):

	list_display = PackContItemAdmin.list_display + \
				   ['nickname', 'exp', 'level']

	list_editable = PackContItemAdmin.list_editable + \
					['nickname', 'exp', 'level']

	field_set = [Fieldset('玩家艾瑟萌属性', 'nickname', 'exp', 'level')]

	exclude = ['player']

	form_layout = PackContItemAdmin.form_layout + field_set


@xadmin.sites.register(ExerSkillSlot)
class ExerSkillSlotAdmin(SlotContainerAdmin):

	list_display = SlotContainerAdmin.list_display + \
				   ['player_exer']

	field_set = [Fieldset('艾瑟萌技能槽属性', 'player_exer')]

	form_layout = SlotContainerAdmin.form_layout + field_set

	inlines = [ExerSkillSlotItemsInline]


@xadmin.sites.register(ExerPackItem)
class ExerPackItemAdmin(PackContItemAdmin):
	pass


@xadmin.sites.register(ExerPackEquip)
class ExerPackEquipAdmin(PackContItemAdmin):
	pass


@xadmin.sites.register(ExerSlotItem)
class ExerSlotItemAdmin(SlotContItemAdmin):

	list_display = SlotContItemAdmin.list_display + \
				   ['player', 'player_exer', 'player_gift', 'subject', 'exp']

	list_editable = SlotContItemAdmin.list_editable + \
					['player_exer', 'player_gift', 'subject', 'exp']

	field_set = [Fieldset('艾瑟萌槽项属性', 'player_exer', 'player_gift',
						  'subject', 'exp')]

	exclude = ['player']

	form_layout = SlotContItemAdmin.form_layout + field_set

	# inlines = [ExerEquipSlotInline]


@xadmin.sites.register(ExerEquipSlotItem)
class ExerEquipSlotItemAdmin(SlotContItemAdmin):

	list_display = SlotContItemAdmin.list_display + \
				   ['pack_equip', 'e_type']

	list_editable = SlotContItemAdmin.list_editable + \
					['pack_equip']

	field_set = [Fieldset('艾瑟萌装备槽项', 'pack_equip')]

	exclude = ['e_type']

	form_layout = SlotContItemAdmin.form_layout + field_set


@xadmin.sites.register(ExerSkillSlotItem)
class ExerSkillSlotItemAdmin(SlotContItemAdmin):

	list_display = SlotContItemAdmin.list_display + \
				   ['skill', 'use_count']

	list_editable = SlotContItemAdmin.list_editable + \
					['skill', 'use_count']

	field_set = [Fieldset('艾瑟萌装备槽项', 'skill', 'use_count')]

	form_layout = SlotContItemAdmin.form_layout + field_set


@xadmin.sites.register(ExerFragPackItem)
class ExerFragPackItemAdmin(PackContItemAdmin):
	pass


@xadmin.sites.register(PlayerExerGift)
class PlayerExerGiftAdmin(PackContItemAdmin):
	pass


@xadmin.sites.register(ExerItemEffect)
class ExerItemEffectAdmin(BaseEffectAdmin):
	pass


@xadmin.sites.register(ExerSkillEffect)
class ExerSkillEffectAdmin(BaseEffectAdmin):
	pass
