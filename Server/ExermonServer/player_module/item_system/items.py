from game_module.models import EquipParamValue, EquipParamRate
from item_module.models import *

# import player_module.item_system.cont_items as ContItems
# from . import *


# ===================================================
#  人类物品使用效果表
# ===================================================
class HumanItemEffect(BaseEffect):

	class Meta:
		verbose_name = verbose_name_plural = "人类物品使用效果"

	# 物品
	item = models.ForeignKey('HumanItem', on_delete=models.CASCADE,
							 verbose_name="物品")


# ===================================================
#  人类物品价格
# ===================================================
class HumanItemPrice(Currency):

	class Meta:
		verbose_name = verbose_name_plural = "人类物品价格"

	LIST_DISPLAY_APPEND = ['item']

	# 物品
	item = models.OneToOneField('HumanItem', on_delete=models.CASCADE,
							 verbose_name="物品")


# ===================================================
#  人类物品表
# ===================================================
@ItemManager.registerItem("人类物品")
class HumanItem(UsableItem):

	# 获取所有的效果
	@CacheHelper.staticCache
	def effects(self):
		return self.humanitemeffect_set.all()

	# 购买价格
	@CacheHelper.staticCache
	def buyPrice(self):
		try: return self.humanitemprice
		except HumanItemPrice.DoesNotExist: return None


# ===================================================
#  人类装备等级属性值表
# ===================================================
class HumanEquipLevelParam(EquipParamRate):
	class Meta:
		verbose_name = verbose_name_plural = "人类装备等级属性值"

	# 装备
	equip = models.ForeignKey("HumanEquip", on_delete=models.CASCADE, verbose_name="装备")


# ===================================================
#  人类装备属性值表
# ===================================================
class HumanEquipBaseParam(EquipParamValue):
	class Meta:
		verbose_name = verbose_name_plural = "人类装备属性值"

	# 装备
	equip = models.ForeignKey("HumanEquip", on_delete=models.CASCADE, verbose_name="装备")

	# 最大值
	def maxVal(self):
		return None

	# 最小值
	def minVal(self):
		return None


# ===================================================
#  人类装备价格
# ===================================================
class HumanEquipPrice(Currency):
	class Meta:
		verbose_name = verbose_name_plural = "人类装备价格"

	LIST_DISPLAY_APPEND = ['item']

	# 物品
	item = models.OneToOneField('HumanEquip', on_delete=models.CASCADE,
								verbose_name="物品")


# ===================================================
#  人类装备
# ===================================================
@ItemManager.registerItem("人类装备")
class HumanEquip(EquipableItem):

	# 装备类型
	e_type = models.ForeignKey("game_module.HumanEquipType",
							   on_delete=models.CASCADE, verbose_name="装备类型")

	# 转化为 dict
	def convert(self, **kwargs):
		res = super().convert(**kwargs)

		res['e_type'] = self.e_type_id

		return res

	@classmethod
	def levelParamClass(cls):
		return HumanEquipLevelParam

	@classmethod
	def baseParamClass(cls):
		return HumanEquipBaseParam

	# 获取所有的属性基本值
	@CacheHelper.staticCache
	def _levelParams(self):
		return self.humanequiplevelparam_set.all()

	# 获取所有的属性基本值
	@CacheHelper.staticCache
	def _baseParams(self):
		return self.humanequipbaseparam_set.all()

	# 购买价格
	@CacheHelper.staticCache
	def buyPrice(self):
		try:
			return self.humanequipprice
		except HumanEquipPrice.DoesNotExist:
			return None
