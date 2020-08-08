from item_module.manager import *
from item_module.models import *

import player_module.item_system.items as Items
import player_module.item_system.containers as Containers
# from . import *


# ===================================================
#  人类背包物品
# ===================================================
@ItemManager.registerPackContItem("道具背包项",
								  Containers.ItemPack, Items.GameItem)
class ItemPackItem(PackContItem):

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