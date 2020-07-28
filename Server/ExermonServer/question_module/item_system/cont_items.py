from item_module.models import *

import question_module.item_system.items as Items
import question_module.item_system.containers as Containers


# ===================================================
#  题目糖背包物品
# ===================================================
@ItemManager.registerPackContItem("人类背包装备",
	Containers.QuesSugarPack, Items.QuesSugar)
class QuesSugarPackItem(PackContItem):

	# # 容器
	# container = models.ForeignKey('QuesSugarPack', on_delete=models.CASCADE,
	# 						   null=True, verbose_name="容器")
	#
	# # 物品
	# item = models.ForeignKey('QuesSugar', on_delete=models.CASCADE,
	# 						 null=True, verbose_name="物品")
	#
	# # 所属容器的类
	# @classmethod
	# def containerClass(cls): return Containers.QuesSugarPack
	#
	# # 所接受的物品类
	# @classmethod
	# def acceptedItemClass(cls): return Items.QuesSugar

	def isContItemUsable(self, occasion: ItemUseOccasion, **kwargs) -> bool:
		"""
		配置当前物品是否可用
		Args:
			occasion (ItemUseOccasion): 使用场合枚举
			**kwargs (**dict): 拓展参数
		Returns:
			返回当前物品是否可用
		"""
		return occasion == ItemUseOccasion.Battle
