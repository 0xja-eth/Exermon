from item_module.models import *


# ===================================================
#  题目糖背包
# ===================================================
@ItemManager.registerSlotContainer("题目糖背包")  #, ContItems.QuesSugarPackItem)
class QuesSugarPack(PackContainer):

	# 玩家
	player = models.OneToOneField('player_module.Player',
								  on_delete=models.CASCADE, verbose_name="玩家")

	# # 所接受的容器项类（单个，基类）
	# @classmethod
	# def baseContItemClass(cls):
	# 	return ContItems.QuesSugarPackItem

	# 创建一个背包（创建角色时候执行）
	def _create(self, player):
		super()._create()
		self.player = player

	# 持有者
	def owner(self): return self.player
