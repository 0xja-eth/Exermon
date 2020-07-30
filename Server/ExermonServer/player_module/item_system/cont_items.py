from item_module.models import *

import player_module.item_system.items as Items
import player_module.item_system.containers as Containers
# from . import *


# ===================================================
#  人类背包物品
# ===================================================
@ItemManager.registerPackContItem("人类背包物品",
	Containers.HumanPack, Items.HumanItem)
class HumanPackItem(PackContItem):

	def isContItemUsable(self, occasion: ItemUseOccasion,
						 target=None, count=1) -> bool:
		"""
		配置当前物品是否可用
		Args:
			occasion (ItemUseOccasion): 使用场合枚举
			target (PlayerExermon): 目标
			count (int): 使用次数
		Returns:
			返回当前物品是否可用
		"""
		return self.item.isUsable(occasion, target, count)


# ===================================================
#  人类背包装备
# ===================================================
@ItemManager.registerPackContItem("人类背包装备",
	Containers.HumanPack, Items.HumanItem)
class HumanPackEquip(PackContItem, EquipParamsObject):

	@classmethod
	def baseParamClass(cls):
		return cls.acceptedItemClass().baseParamClass()

	@classmethod
	def levelParamClass(cls):
		return cls.acceptedItemClass().levelParamClass()

	# 获取等级属性值
	def _levelParam(self, **kwargs):
		return self.item.levelParam(**kwargs)

	# 获取属性值
	def _baseParam(self, **kwargs):
		return self.item.baseParam(**kwargs)

	def refresh(self):
		super().refresh()
		self._clearParamsCache()


# ===================================================
#  人类装备槽项
# ===================================================
@ItemManager.registerSlotContItem("人类装备槽项",
	Containers.HumanEquipSlot, pack_equip=HumanPackEquip)
class HumanEquipSlotItem(SlotContItem, ParamsObject):

	# 装备槽类型
	e_type = models.ForeignKey('game_module.HumanEquipType', on_delete=models.CASCADE,
							   verbose_name="装备槽类型")

	@classmethod
	def paramValueClass(cls):
		cla = cls.acceptedEquipItemClasses()[0]
		return cla.baseParamClass()

	# 转化为 dict
	def convert(self, **kwargs):
		res = super().convert(**kwargs)

		res['e_type'] = self.e_type_id

		return res

	# 装备
	def packEquip(self) -> HumanPackEquip:
		return self.equipItem(0)

	# 配置索引
	def setupIndex(self, index, **kwargs):
		super().setupIndex(index, **kwargs)
		self.e_type_id = index

	def refresh(self):
		super().refresh()
		self._clearParamsCache()
