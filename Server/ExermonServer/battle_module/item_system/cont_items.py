from item_module.models import *

from player_module.item_system.cont_items import HumanPackItem

import battle_module.item_system.containers as Containers


# ===================================================
#  对战物资槽项
# ===================================================
@ItemManager.registerSlotContItem("对战物资槽项",
	Containers.BattleItemSlot, pack_item=HumanPackItem)
class BattleItemSlotItem(SlotContItem):

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


