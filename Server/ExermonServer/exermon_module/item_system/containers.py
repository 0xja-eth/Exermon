from item_module.models import *
from game_module.models import Subject, ExerEquipType


# ===================================================
#  艾瑟萌仓库
# ===================================================
@ItemManager.registerPackContainer("艾瑟萌仓库")  # , ContItems.PlayerExermon)
class ExerHub(PackContainer):

	# 玩家
	player = models.OneToOneField('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 创建一个背包（创建角色时候执行）
	def _create(self, player):
		super()._create()
		self.player = player

	# 持有玩家
	def owner(self): return self.player


# ===================================================
#  艾瑟萌天赋池
# ===================================================
@ItemManager.registerPackContainer("艾瑟萌天赋池")  # , ContItems.PlayerExerGift)
class ExerGiftPool(PackContainer):

	# 玩家
	player = models.OneToOneField('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 创建一个背包（创建角色时候执行）
	def _create(self, player):
		super()._create()
		self.player = player

	# 持有玩家
	def owner(self): return self.player


# ===================================================
#  艾瑟萌碎片背包
# ===================================================
@ItemManager.registerPackContainer("艾瑟萌碎片背包")  # , ContItems.ExerFragPackItem)
class ExerFragPack(PackContainer):

	# 玩家
	player = models.OneToOneField('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 创建一个背包（创建角色时候执行）
	def _create(self, player):
		super()._create()
		self.player = player

	# 持有玩家
	def owner(self): return self.player


# ===================================================
#  艾瑟萌槽表
# ===================================================
@ItemManager.registerSlotContainer("艾瑟萌槽")  # , ContItems.ExerSlotItem)
class ExerSlot(SlotContainer):

	# 玩家
	player = models.OneToOneField('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	init_exers = None

	# 默认容器容量（0为无限）
	@classmethod
	def defaultCapacity(cls): return Subject.MAX_SELECTED

	# 创建一个艾瑟萌槽（选择艾瑟萌时候执行）
	def _create(self, player, player_exers):
		super()._create()
		self.player = player
		self.init_exers = player_exers

	def _equipContainer(self, index):
		if index == 0: return self.exactlyPlayer().exerHub()
		if index == 1: return self.exactlyPlayer().exerGiftPool()

	# 创建一个槽
	def _createSlot(self, cla, index, **kwargs):
		return super()._createSlot(cla, index,
			init_exer=self.init_exers[index], **kwargs)

	# 持有玩家
	def owner(self): return self.player

	# 保证科目与槽一致
	def ensureSubject(self, slot_item, exermon):
		if slot_item.subject_id != exermon.exermon().subject_id:
			raise GameException(ErrorType.IncorrectSubject)

		return True

	# 保证满足装备条件
	def ensureEquipCondition(self, slot_item, equip_item):
		super().ensureEquipCondition(slot_item, equip_item)
		from .cont_items import PlayerExermon

		if isinstance(equip_item, PlayerExermon):
			self.ensureSubject(slot_item, equip_item)

	# 设置艾瑟萌
	def setPlayerExer(self, slot_item: SlotContItem = None,
					  player_exer=None, subject_id=None, force=False):

		if player_exer is not None:
			subject_id = player_exer.exermon().subject_id

		self.setEquip(slot_item=slot_item, equip_index=0, equip_item=player_exer, subject_id=subject_id, force=force)

	# 设置艾瑟萌天赋
	def setPlayerGift(self, slot_item: SlotContItem = None,
					  player_gift=None, subject_id=None, slot_index=None, force=False):

		if slot_index is not None:
			self.setEquip(slot_item=slot_item, equip_index=1, equip_item=player_gift,
						  index=slot_index, force=force)
		elif subject_id is not None:
			self.setEquip(slot_item=slot_item, equip_index=1, equip_item=player_gift,
						  subject_id=subject_id, force=force)
		else:
			self.setEquip(slot_item=slot_item, equip_index=1, equip_item=player_gift,
						  force=force)

	def gainExp(self, slot_exps: dict, exer_exps: dict):
		"""
		获得经验
		Args:
			slot_exps (dict): 槽经验（字典类型，键为科目ID，值为变更经验值）
			exer_exps (dict): 艾瑟萌经验（字典类型，键为科目ID，值为变更经验值）
		"""

		for sid in slot_exps:
			slot_exp = slot_exps[sid]
			exer_exp = exer_exps[sid]

			slot_item = self.contItem(subject_id=sid)
			if slot_item is None: continue

			slot_item.gainExp(slot_exp, exer_exp)

	def battlePoint(self) -> int:
		"""
		获取战斗力
		Returns:
			返回战斗力点数
		"""
		sum = 0

		slot_items = self.contItems()
		for slot_item in slot_items:
			sum += slot_item.battlePoint()

		return sum


# ===================================================
#  艾瑟萌技能槽
# ===================================================
@ItemManager.registerSlotContainer("艾瑟萌技能槽")  # , ContItems.ExerSkillSlotItem)
class ExerSkillSlot(SlotContainer):

	# 最大技能数量
	MAX_SKILL_COUNT = 3

	# 艾瑟萌
	player_exer = models.OneToOneField('exermon_module.PlayerExermon',
								   on_delete=models.CASCADE, verbose_name="艾瑟萌")

	skills = None

	# # 所接受的装备项基类（由于重载了 contItemClass，该函数意义有改变）
	# @classmethod
	# def baseContItemClass(cls): return None
	#
	# # 所接受的容器项类
	# @classmethod
	# def acceptedContItemClasses(cls): return ()

	# 默认容器容量（0为无限）
	@classmethod
	def defaultCapacity(cls): return cls.MAX_SKILL_COUNT

	# 创建一个背包（创建角色时候执行）
	def _create(self, player_exer):
		super()._create()
		self.player_exer = player_exer
		self.skills = player_exer.exermon().skills()

	# 创建一个槽
	def _createSlot(self, cla, index, **kwargs):
		if index >= self.skills.count(): skill = None
		else: skill = self.skills[index]

		return super()._createSlot(cla, index, skill=skill, **kwargs)

	# 持有者
	def owner(self): return self.player_exer

	# 持有玩家
	def ownerPlayer(self): return self.owner().player


# ===================================================
#  艾瑟萌背包
# ===================================================
@ItemManager.registerPackContainer("艾瑟萌背包")  # ContItems.ExerPackItem, ContItems.ExerPackEquip)
class ExerPack(PackContainer):

	# 默认容量
	DEFAULT_CAPACITY = 0

	# 玩家
	player = models.OneToOneField('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# # 所接受的容器项类
	# @classmethod
	# def acceptedContItemClasses(cls):
	# 	return ContItems.ExerPackItem, ContItems.ExerPackEquip

	# 获取容器容量（0为无限）
	@classmethod
	def defaultCapacity(cls): return cls.DEFAULT_CAPACITY

	# 创建一个背包（创建角色时候执行）
	def _create(self, player):
		super()._create()
		self.player = player

	# 持有玩家
	def owner(self): return self.player


# ===================================================
#  艾瑟萌装备槽
#  有 ExermonEquipType.Count 个固定的槽
# ===================================================
@ItemManager.registerSlotContainer("艾瑟萌装备槽")  # , ContItems.ExerEquipSlotItem)
class ExerEquipSlot(SlotContainer, ParamsObject):

	# 艾瑟萌
	exer_slot_item = models.OneToOneField('exermon_module.ExerSlotItem',
										  on_delete=models.CASCADE, verbose_name="艾瑟萌")

	# # 所接受的槽项类
	# @classmethod
	# def acceptedSlotItemClass(cls):
	# 	return ContItems.ExerEquipSlotItem
	#
	# # 所接受的装备项基类（由于重载了 contItemClass，该函数意义有改变）
	# @classmethod
	# def baseContItemClass(cls):
	# 	return ContItems.ExerPackEquip

	# 默认容器容量（0为无限）
	@classmethod
	def defaultCapacity(cls):
		return ExerEquipType.count()

	@classmethod
	def paramValueClass(cls):
		cla = cls.acceptedSlotItemClass()
		return cla.paramValueClass()

	# 创建一个槽（创建角色时候执行）
	def _create(self, exer_slot_item):
		super()._create()
		self.exer_slot_item = exer_slot_item

	def exerSlotItem(self):
		return self.exactlyPlayer().getContItem(
			cont_item=self.exer_slot_item)

	def ensureEquipCondition(self, slot_item, equip_item):
		"""
		确保装备条件满足
		Args:
			slot_item (ExerEquipSlotItem): 槽项
			equip_item (ExerPackEquip): 装备项
		"""
		super().ensureEquipCondition(slot_item, equip_item)

		self.ensureEquipType(slot_item, equip_item)

		self.ensureEquipLevel(equip_item)

	def ensureEquipType(self, slot_item, equip_item):
		"""
		确保装备类型与槽一致
		Args:
			slot_item (ExerEquipSlotItem): 槽项
			equip_item (ExerPackEquip): 装备项
		Raises:
			ErrorType.IncorrectEquipType: 装备类型不正确
		"""
		if slot_item.e_type_id != equip_item.item.e_type_id:
			raise GameException(ErrorType.IncorrectEquipType)

	def ensureEquipLevel(self, equip_item):
		"""
		确保装备等级足够
		Args:
			equip_item (ExerPackEquip): 装备项
		Raises:
			ErrorType.InsufficientLevel: 等级不足
		"""
		# player_exer = self.exer_slot.player_exer
		if self.exerSlotItem().slotLevel() < equip_item.item.min_level:
			raise GameException(ErrorType.InsufficientLevel)

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

	# 持有者
	def owner(self): return self.exer_slot_item

	# 持有玩家
	def ownerPlayer(self): return self.owner().player

	def refresh(self):
		super().refresh()
		self._clearParamsCache()
