from item_module.models import *
from item_module.manager import *

from player_module.item_system.cont_items import ItemPackItem
from player_module.item_system.containers import ItemPack


# ===================================================
#  对战物资槽
# ===================================================
@ItemManager.registerSlotContainer("对战物资槽")
class BattleItemSlot(SlotContainer):

	# 最大物资槽数
	MAX_ITEM_COUNT = 3

	# 玩家
	player = models.OneToOneField('player_module.Player',
								  on_delete=models.CASCADE, verbose_name="玩家")

	# 获取容器容量（0为无限）
	@classmethod
	def defaultCapacity(cls): return cls.MAX_ITEM_COUNT

	def _equipContainer(self, index: int) -> ItemPack:
		"""
		获取指定装备ID的所属容器
		Args:
			index (int): 装备ID
		Returns:
			指定装备ID的所属容器项
		"""
		return self.exactlyPlayer().humanPack()

	def _create(self, player: 'Player'):
		"""
		创建对战物资容器（创建角色时候执行）
		Args:
			player (Player): 玩家
		"""
		self.player = player
		super()._create()

	def owner(self) -> 'Player':
		"""
		获取容器的持有者
		Returns:
			持有玩家
		"""
		return self.player

	def ensureItemEquipable(self, equip_item: ItemPackItem):
		"""
		保证物品可以装备（战斗中使用）
		Args:
			equip_item (ItemPackItem): 装备项
		Raises:
			ErrorType.IncorrectItemType: 不正确的物品类型
		"""
		if equip_item.count != 1:
			raise GameException(ErrorType.IncorrectItemType)

		if not equip_item.item.battle_use:
			raise GameException(ErrorType.IncorrectItemType)

	def ensureEquipCondition(self, slot_item, equip_item: ItemPackItem):
		"""
		确保满足装备条件
		Args:
			slot_item (BattleItemSlotItem): 装备槽项
			equip_item (ItemPackItem): 装备项
		"""
		super().ensureEquipCondition(slot_item, equip_item)

		self.ensureItemEquipable(equip_item)

		return True

	def setPackItem(self, pack_item: ItemPackItem = None,
					index: int = None, force: bool = False):
		"""
		设置物资槽物品
		Args:
			pack_item (ItemPackItem): 人类物品容器项
			index (int): 槽编号
			force (bool): 是否强制设置（不损失背包物品）
		"""
		self.setEquip(equip_index=0, equip_item=pack_item,
					  index=index, force=force)
