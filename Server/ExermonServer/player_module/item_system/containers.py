from item_module.models import *
from game_module.models import HumanEquipType


# ===================================================
#  人类背包
# ===================================================
@ItemManager.registerPackContainer("人类背包")
class HumanPack(PackContainer):

	# 默认容量
	DEFAULT_CAPACITY = 0

	# 玩家
	player = models.OneToOneField('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 获取容器容量（0为无限）
	@classmethod
	def defaultCapacity(cls): return cls.DEFAULT_CAPACITY

	# 创建一个背包（创建角色时候执行）
	def _create(self, player):
		super()._create()
		self.player = player

	# 持有者
	def owner(self): return self.player


# ===================================================
#  人类装备槽
# ===================================================
@ItemManager.registerSlotContainer("人类装备槽")
class HumanEquipSlot(SlotContainer, ParamsObject):

	# 玩家
	player = models.OneToOneField('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 默认容器容量（0为无限）
	@classmethod
	def defaultCapacity(cls): return HumanEquipType.count()

	@classmethod
	def paramValueClass(cls):
		cla = cls.acceptedSlotItemClass()
		return cla.paramValueClass()

	def _create(self, player: 'Player'):
		"""
		创建对战物资容器（创建角色时候执行）
		Args:
			player (Player): 玩家
		"""
		super()._create()
		self.player = player

	# 保证装备类型与槽一致
	def ensureEquipType(self, slot_item, equip):
		if slot_item.e_type_id != equip.item.e_type_id:
			raise GameException(ErrorType.IncorrectEquipType)

		return True

	# 保证满足装备条件
	def ensureEquipCondition(self, slot_item, equip_item):
		super().ensureEquipCondition(slot_item, equip_item)

		self.ensureEquipType(slot_item, equip_item)

		return True

	# 设置艾瑟萌装备
	def setPackEquip(self, pack_equip=None, e_type_id=None, force=False):

		if pack_equip is not None:
			e_type_id = pack_equip.item.e_type_id

		self.setEquip(equip_index=0, equip_item=pack_equip, e_type_id=e_type_id, force=force)

	# 获取属性值
	def _paramVal(self, **kwargs):
		sum = 0
		slot_items = self.contItems()

		for slot_item in slot_items:
			sum += slot_item.paramVal(**kwargs)

		return sum

	def owner(self) -> 'Player':
		"""
		获取容器的持有者
		Returns:
			持有玩家
		"""
		return self.player

	def refresh(self):
		super().refresh()
		self._clearParamsCache()
