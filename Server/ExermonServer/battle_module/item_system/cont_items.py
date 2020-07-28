from item_module.models import *

from player_module.item_system.cont_items import HumanPackItem

import battle_module.item_system.containers as Containers


# ===================================================
#  对战物资槽项
# ===================================================
@ItemManager.registerSlotContItem("对战物资槽项",
	Containers.BattleItemSlot, pack_item=HumanPackItem)
class BattleItemSlotItem(SlotContItem):

	# # 容器
	# container = models.ForeignKey('BattleItemSlot', on_delete=models.CASCADE,
	# 						   null=True, verbose_name="容器")
	#
	# # 装备项
	# pack_item = models.OneToOneField('player_module.HumanPackItem', null=True, blank=True,
	# 								  on_delete=models.SET_NULL, verbose_name="装备")

	# # 所属容器的类
	# @classmethod
	# def containerClass(cls):
	# 	return Containers.BattleItemSlot
	#
	# # 所接受的装备项类（可多个）
	# @classmethod
	# def acceptedEquipItemClasses(cls): return (HumanPackItem,)
	#
	# # 所接受的装备项属性名（可多个）
	# @classmethod
	# def acceptedEquipItemAttrs(cls): return ('pack_item',)

	def isUsable(self) -> bool:
		"""
		能否使用
		Returns:
			返回能否使用
		"""
		return True

	def useItem(self, **kwargs):
		"""
		使用物品
		Args:
			**kwargs (**dict): 拓展参数
		"""
		super().useItem(ItemUseOccasion.Battle, **kwargs)
		self.dequip(index=0)


