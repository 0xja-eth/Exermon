from item_module.models import *


# ===================================================
#  题目糖背包
# ===================================================
@ItemManager.registerSlotContainer("题目糖背包")
class QuesSugarPack(PackContainer):

	# 玩家
	player = models.OneToOneField('player_module.Player',
								  on_delete=models.CASCADE, verbose_name="玩家")

	# 创建一个背包（创建角色时候执行）
	def _create(self, player):
		self.player = player
		super()._create()

	# 持有者
	def owner(self): return self.player
