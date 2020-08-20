from item_module.manager import *

# import player_module.item_system.cont_items as ContItems
# from . import *


# ===================================================
#  卡牌使用效果表
# ===================================================
class GameCardEffect(BaseEffect):

	class Meta:
		verbose_name = verbose_name_plural = "卡牌使用效果"

	# 物品
	item = models.ForeignKey('card_module.GameCard', on_delete=models.CASCADE,
							 verbose_name="物品")


# ===================================================
#  游戏卡牌
# ===================================================
@ItemManager.registerItem("游戏卡牌")
class GameCard(UsableItem):

	# 获取所有的效果
	@CacheHelper.staticCache
	def effects(self):
		return self.gamecardeffect_set.all()

	# 不可直接购买
	def buyPrice(self):
		return None
