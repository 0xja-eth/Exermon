from .models import *
from player_module.models import Player
from item_module.views import Service as ItemService, Common as ItemCommon
from utils.exception import ErrorType, GameException

# Create your views here.


# =======================
# 艾瑟萌服务类，封装管理艾瑟萌模块的业务处理函数
# =======================
class Service:

	# 艾瑟萌槽装备（艾瑟萌/艾瑟萌天赋）
	@classmethod
	async def equipPlayerExer(cls, consumer, player: Player, sid: int, peid: int):
		# 返回数据：无

		Subject.ensure(id=sid)

		exer_slot = Common.getExerSlot(player)

		player_exer = Common.getPlayerExer(player, id=peid) if peid > 0 else None

		return ItemService.slotContainerEquip(exer_slot, player_exer, subject_id=sid)

	# 艾瑟萌槽装备（艾瑟萌/艾瑟萌天赋）
	@classmethod
	async def equipPlayerGift(cls, consumer, player: Player, sid: int, pgid: int):
		# 返回数据：无

		Subject.ensure(id=sid)

		exer_slot = Common.getExerSlot(player)

		player_gift = Common.getPlayerGift(player, id=pgid) if pgid > 0 else None

		return ItemService.slotContainerEquip(exer_slot, player_gift, subject_id=sid)

	# 艾瑟萌装备槽装备
	@classmethod
	async def equipExerEquip(cls, consumer, player: Player, sid: int, eeid: int):
		# 返回数据：无

		equip_slot = Common.getExerEquipSlot(player, subject_id=sid)

		pack_equip = Common.getPackEquip(player, id=eeid)

		return ItemService.slotContainerEquip(equip_slot, pack_equip, e_type_id=pack_equip.item.e_type_id)

	# 艾瑟萌装备槽卸下装备
	@classmethod
	async def dequipExerEquip(cls, consumer, player: Player, sid: int, type: int):
		# 返回数据：无

		equip_slot = Common.getExerEquipSlot(player, subject_id=sid)

		return ItemService.slotContainerEquip(equip_slot, None, type_=ExerPackEquip, e_type_id=type)

	# 艾瑟萌改名
	@classmethod
	async def editNickname(cls, consumer, player: Player, peid: int, name: str):
		# 返回数据：

		Check.ensureNameFormat(name)

		player_exer = Common.getPlayerExer(player, include_equipped=True, id=peid)

		ItemCommon.ensureContItemOwnerPlayer(player_exer, player)

		player_exer.nickname = name


# =======================
# 艾瑟萌校验类，封装艾瑟萌业务数据格式校验的函数
# =======================
class Check:

	# 校验艾瑟萌数量
	@classmethod
	def ensureExermonCount(cls, val: list):
		if len(val) != Subject.MAX_SELECTED:
			raise GameException(ErrorType.InvalidExermonCount)

	# 校验名字格式
	@classmethod
	def ensureNameFormat(cls, val: str):
		if len(val) > Exermon.NAME_LEN:
			raise GameException(ErrorType.InvalidExermonName)


# =======================
# 艾瑟萌公用类，封装关于艾瑟萌模块的公用函数
# =======================
class Common:

	# 获取艾瑟萌
	@classmethod
	def getExermon(cls, error: ErrorType = ErrorType.ExermonNotExist, **kwargs) -> Exermon:
		"""
		获取艾瑟萌
		Args:
			error (ErrorType): 错误时抛出的异常类型
			**kwargs (**dict): 查询参数
		Returns:
			返回一定条件下查找到的艾瑟萌对象，若不存在抛出设置好的异常
		"""
		return ViewUtils.getObject(Exermon, error, **kwargs)

	# 获取艾瑟萌天赋
	@classmethod
	def getExerGift(cls, error: ErrorType = ErrorType.ExerGiftNotExist, **kwargs) -> ExerGift:
		"""
		获取艾瑟萌天赋
		Args:
			error (ErrorType): 错误时抛出的异常类型
			**kwargs (**dict): 查询参数
		Returns:
			返回一定条件下查找到的艾瑟萌天赋对象，若不存在抛出设置好的异常
		"""
		return ViewUtils.getObject(ExerGift, error, **kwargs)

	# 获取艾瑟萌背包
	@classmethod
	def getExerPack(cls, player: Player, error: ErrorType = ErrorType.ContainerNotExist) -> ExerPack:
		"""
		获取艾瑟萌背包
		Args:
			player (Player): 所属玩家
			error (ErrorType): 错误时抛出的异常类型
		Returns:
			返回对应玩家的艾瑟萌背包对象
		"""
		return ItemCommon.getContainer(cla=ExerPack, player=player, error=error)

	# 获取艾瑟萌仓库
	@classmethod
	def getExerHub(cls, player: Player, error: ErrorType = ErrorType.ContainerNotExist) -> ExerHub:
		"""
		获取艾瑟萌仓库
		Args:
			player (Player): 所属玩家
			error (ErrorType): 错误时抛出的异常类型
		Returns:
			返回对应玩家的艾瑟萌仓库对象
		"""
		return ItemCommon.getContainer(cla=ExerHub, player=player, error=error)

	# 获取艾瑟萌天赋池
	@classmethod
	def getExerGiftPool(cls, player: Player, error: ErrorType = ErrorType.ContainerNotExist) -> ExerHub:
		"""
		获取艾瑟萌天赋池
		Args:
			player (Player): 所属玩家
			error (ErrorType): 错误时抛出的异常类型
		Returns:
			返回对应玩家的艾瑟萌天赋池对象
		"""
		return ItemCommon.getContainer(cla=ExerGiftPool, player=player, error=error)

	# 获取艾瑟萌槽
	@classmethod
	def getExerSlot(cls, player: Player, error: ErrorType = ErrorType.ContainerNotExist) -> ExerSlot:
		return ItemCommon.getContainer(cla=ExerSlot, player=player, error=error)

	# 获取艾瑟萌槽项
	@classmethod
	def getExerSlotItem(cls, player: Player, error: ErrorType = ErrorType.ExerSlotItemNotExist,
						**kwargs) -> ExerSlotItem:
		return ItemCommon.getContItem(cla=ExerSlotItem, player=player, error=error, **kwargs)

	# 获取艾瑟萌装备槽
	@classmethod
	def getExerEquipSlot(cls, player: Player, error: ErrorType = ErrorType.ContainerNotExist,
						 **kwargs) -> ExerEquipSlot:
		res = cls.getExerSlotItem(player, **kwargs).exerEquipSlot()
		if res is None: raise GameException(error)
		return res

	# 获取玩家艾瑟萌关系
	@classmethod
	def getPlayerExer(cls, player: Player, error: ErrorType = ErrorType.PlayerExermonNotExist,
					  **kwargs) -> PlayerExermon:
		return ItemCommon.getContItem(cla=PlayerExermon, player=player, error=error, **kwargs)

	# 获取玩家艾瑟萌天赋关系
	@classmethod
	def getPlayerGift(cls, player: Player, error: ErrorType = ErrorType.PlayerExerGiftNotExist,
					  **kwargs) -> PlayerExerGift:
		return ItemCommon.getContItem(cla=PlayerExerGift, player=player, error=error, **kwargs)

	# 获取玩家持有艾瑟萌装备
	@classmethod
	def getPackEquip(cls, player: Player, error: ErrorType = ErrorType.ContItemNotExist,
					 **kwargs) -> ExerPackEquip:
		return ItemCommon.getContItem(cla=ExerPackEquip, player=player, error=error, **kwargs)

	# 获取多个艾瑟萌
	@classmethod
	def getExermons(cls, ids, error: ErrorType = ErrorType.ExermonNotExist) -> Exermon:

		unique_ids = list(set(ids))

		res = ViewUtils.getObjects(Exermon, id__in=ids)

		# 数量不一致，说明获取出现问题
		if res.count() != len(unique_ids): raise GameException(error)

		# 如果本身给的 ids 没有重复，直接返回
		# if len(unique_ids) == len(ids): return res

		# 否则，需要按顺序抽取
		real_res = []
		for id in ids: real_res.append(res.get(id=id))

		return real_res

	# 获取多个艾瑟萌天赋
	@classmethod
	def getExerGifts(cls, ids, error: ErrorType = ErrorType.ExerGiftNotExist) -> ExerGift:

		unique_ids = list(set(ids))

		res = ViewUtils.getObjects(ExerGift, id__in=unique_ids)

		# 数量不一致，说明获取出现问题
		if res.count() != len(unique_ids): raise GameException(error)

		# 如果本身给的 ids 没有重复，直接返回
		# if len(unique_ids) == len(ids): return res

		# 否则，需要按顺序抽取
		real_res = []
		for id in ids: real_res.append(res.get(id=id))

		return real_res

	# 确保艾瑟萌存在
	@classmethod
	def ensureExermonExist(cls, error: ErrorType = ErrorType.ExermonNotExist, **kwargs):
		return ViewUtils.ensureObjectExist(Exermon, error, **kwargs)

	# 确保艾瑟萌天赋存在
	@classmethod
	def ensureExerGiftExist(cls, error: ErrorType = ErrorType.ExerGiftNotExist, **kwargs):
		return ViewUtils.ensureObjectExist(ExerGift, error, **kwargs)

	# 确保艾瑟萌科目合法
	@classmethod
	def ensureExermonSubject(cls, exers):
		sbjs = []

		for exer in exers:
			sid = exer.subject_id
			if sid in sbjs:
				raise GameException(ErrorType.InvalidExermonSubject)

			sbjs.append(sid)

		force_sbjs = Subject.objs(force=True)

		for sbj in force_sbjs:
			if sbj.id not in sbjs:
				raise GameException(ErrorType.InvalidExermonSubject)

	# 确保艾瑟萌类型
	@classmethod
	def ensureExermonType(cls, exers=None, exer=None,
						  e_type=ExermonType.Initial):

		if exer is not None:
			if exer.e_type != e_type.value:
				raise GameException(ErrorType.InvalidExermonType)

		if exers is not None:
			for exer in exers:
				cls.ensureExermonType(exer=exer, e_type=e_type)

	# 确保艾瑟萌天赋类型
	@classmethod
	def ensureExerGiftType(cls, gifts=None, gift=None, g_type=ExerGiftType.Initial):

		if gift is not None:
			if gift.g_type != g_type.value:
				raise GameException(ErrorType.InvalidExerGiftType)

		if gifts is not None:
			for gift in gifts:
				cls.ensureExerGiftType(gift=gift, g_type=g_type)
