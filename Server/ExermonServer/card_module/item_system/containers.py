from item_module.models import *


# ===================================================
#  人类背包
# ===================================================
@ItemManager.registerPackContainer("卡组")
class CardGroup(PackContainer):

	# 默认容量
	DEFAULT_CAPACITY = 0

	# 玩家
	player = models.OneToOneField('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 获取容器容量（0为无限）
	@classmethod
	def defaultCapacity(cls): return cls.DEFAULT_CAPACITY

	# 创建一个背包（创建角色时候执行）
	def _create(self, player):
		self.player = player
		super()._create()

	# 持有者
	def owner(self): return self.player
