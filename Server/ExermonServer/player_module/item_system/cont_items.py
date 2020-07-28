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

	# # 容器
	# container = models.ForeignKey('HumanPack', on_delete=models.CASCADE,
	# 						   null=True, verbose_name="容器")
	#
	# # 物品
	# item = models.ForeignKey('HumanItem', on_delete=models.CASCADE,
	# 						 null=True, verbose_name="物品")

	# # 所属容器的类
	# @classmethod
	# def containerClass(cls):
	# 	return Containers.HumanPack
	#
	# # 所接受的物品类
	# @classmethod
	# def acceptedItemClass(cls):
	# 	return Items.HumanItem

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
class HumanPackEquip(PackContItem):

	# # 容器
	# container = models.ForeignKey('HumanPack', on_delete=models.CASCADE,
	# 						   null=True, verbose_name="容器")
	#
	# # 物品
	# item = models.ForeignKey('HumanEquip', on_delete=models.CASCADE,
	# 						 null=True, verbose_name="物品")

	# # 所属容器的类
	# @classmethod
	# def containerClass(cls): return Containers.HumanPack
	#
	# # 所接受的物品类
	# @classmethod
	# def acceptedItemClass(cls): return Items.HumanEquip

	# 获取等级属性值
	def levelParam(self, param_id=None, attr=None):
		return self.item.levelParam(param_id, attr)

	# 获取属性值
	def baseParam(self, param_id=None, attr=None):
		return self.item.baseParam(param_id, attr)


# ===================================================
#  人类装备槽项
# ===================================================
@ItemManager.registerSlotContItem("人类装备槽项",
	Containers.HumanEquipSlot, pack_equip=HumanPackEquip)
class HumanEquipSlotItem(SlotContItem):

	# 容器
	# container = models.ForeignKey('HumanEquipSlot', on_delete=models.CASCADE,
	# 						   null=True, verbose_name="容器")

	# 装备项
	# pack_equip = models.OneToOneField('HumanPackEquip', null=True, blank=True,
	# 								  on_delete=models.SET_NULL, verbose_name="装备")

	# 装备槽类型
	e_type = models.ForeignKey('game_module.HumanEquipType', on_delete=models.CASCADE,
							   verbose_name="装备槽类型")

	# # 所属容器的类
	# @classmethod
	# def containerClass(cls): return Containers.HumanEquipSlot
	#
	# # 所接受的装备项类（可多个）
	# @classmethod
	# def acceptedEquipItemClasses(cls): return (HumanPackEquip,)
	#
	# # 所接受的装备项属性名（可多个）
	# @classmethod
	# def acceptedEquipItemAttrs(cls): return ('pack_equip',)

	# 转化为 dict
	def convert(self, **kwargs):
		res = super().convert(**kwargs)

		res['e_type'] = self.e_type_id

		return res

	# def _equipItem(self, index):
	# 	if index == 0: return self.pack_equip

	# 配置索引
	def setupIndex(self, index, **kwargs):
		super().setupIndex(index, **kwargs)
		self.e_type_id = index

	# 获取属性值
	def param(self, param_id=None, attr=None):
		return self.pack_equip.param(param_id, attr)

	# # 装备
	# def equip(self, pack_equip=None, **kwargs):
	# 	self.pack_equip: HumanPackEquip = pack_equip
	# 	self.pack_equip.transfer(self.container)
	#
	# # 卸下
	# def dequip(self, **kwargs):
	# 	self.pack_equip.remove()
	# 	pack_equip = self.pack_equip
	# 	self.pack_equip = None
	# 	return pack_equip
