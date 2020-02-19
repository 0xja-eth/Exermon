from django.shortcuts import render
from .models import *
from player_module.models import Player
from utils.exception import ErrorType, ErrorException

# Create your views here.


# =======================
# 艾瑟萌服务类，封装管理艾瑟萌模块的业务处理函数
# =======================
class Service:

	# 艾瑟萌槽装备（艾瑟萌/艾瑟萌天赋）
	@classmethod
	async def exerSlotEquip(cls, consumer, player: Player, sid: int, peid: int, pgid: int):
		# 返回数据：无
		from item_module.views import Service as ItemService

		Subject.ensure(id=sid)

		exer_slot = player.exerSlot()

		player_exer = Common.getPlayerExer(id=peid)
		player_gift = Common.getPlayerGift(id=pgid)

		ItemService.slotContainerEquip(player, exer_slot, [player_exer, player_gift], subject_id=sid)

	# 艾瑟萌装备槽装备
	@classmethod
	async def equipSlotEquip(cls, consumer, player: Player, cid: int, eid: int, eeid: int):
		# 返回数据：无
		from item_module.views import Service as ItemService

		ExerEquipType.ensure(id=eid)

		equip_slot = Common.getExerEquipSlot(id=cid)

		pack_equip = Common.getPackEquip(id=eeid)

		ItemService.slotContainerEquip(player, equip_slot, pack_equip, e_type_id=eid)


# =======================
# 用户校验类，封装用户业务数据格式校验的函数
# =======================
class Check:

	# 校验艾瑟萌数量
	@classmethod
	def ensureExermonCount(cls, val: list):
		if len(val) != Subject.MAX_SELECTED:
			raise ErrorException(ErrorType.InvalidExermonCount)

	# 校验名字格式
	@classmethod
	def ensureExermonNameFormat(cls, val: str):
		if len(val) > Exermon.NAME_LEN:
			raise ErrorException(ErrorType.InvalidExermonName)


# =======================
# 艾瑟萌公用类，封装关于艾瑟萌模块的公用函数
# =======================
class Common:

	# 获取艾瑟萌
	@classmethod
	def getExermon(cls, return_type='object', error: ErrorType = ErrorType.ExermonNotExist,
				   **kwargs) -> Exermon:

		return ViewUtils.getObject(Exermon, error, return_type=return_type, **kwargs)

	# 获取艾瑟萌天赋
	@classmethod
	def getExerGift(cls, return_type='object', error: ErrorType = ErrorType.ExerGiftNotExist,
					**kwargs) -> ExerGift:

		return ViewUtils.getObject(ExerGift, error, return_type=return_type, **kwargs)

	# 获取艾瑟萌槽
	@classmethod
	def getExerEquipSlot(cls, return_type='object', error: ErrorType = ErrorType.ContainerNotExist,
						 **kwargs) -> ExerEquipSlot:

		return ViewUtils.getObject(ExerEquipSlot, error, return_type=return_type, **kwargs)

	# 获取玩家艾瑟萌关系
	@classmethod
	def getPlayerExer(cls, return_type='object', error: ErrorType = ErrorType.PlayerExermonNotExist,
					  **kwargs) -> PlayerExermon:

		return ViewUtils.getObject(PlayerExermon, error, return_type=return_type, **kwargs)

	# 获取玩家艾瑟萌天赋关系
	@classmethod
	def getPlayerGift(cls, return_type='object', error: ErrorType = ErrorType.PlayerExerGiftNotExist,
					  **kwargs) -> PlayerExerGift:

		return ViewUtils.getObject(PlayerExerGift, error, return_type=return_type, **kwargs)

	# 获取玩家持有艾瑟萌装备
	@classmethod
	def getPackEquip(cls, return_type='object', error: ErrorType = ErrorType.ContItemNotExist,
					 **kwargs) -> ExerPackEquip:

		return ViewUtils.getObject(ExerPackEquip, error, return_type=return_type, **kwargs)

	# 获取多个艾瑟萌
	@classmethod
	def getExermons(cls, ids, error: ErrorType = ErrorType.ExermonNotExist) -> Exermon:

		unique_ids = list(set(ids))

		res = ViewUtils.getObjects(Exermon, id__in=ids)

		# 数量不一致，说明获取出现问题
		if res.count() != len(unique_ids): raise ErrorException(error)

		# 如果本身给的 ids 没有重复，直接返回
		if len(unique_ids) == len(ids): return res

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
		if res.count() != len(unique_ids): raise ErrorException(error)

		# 如果本身给的 ids 没有重复，直接返回
		if len(unique_ids) == len(ids): return res

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
				raise ErrorException(ErrorType.InvalidExermonSubject)

			sbjs.append(sid)

		force_sbjs = Subject.objs(force=True)

		for sbj in force_sbjs:
			if sbj.id not in sbjs:
				raise ErrorException(ErrorType.InvalidExermonSubject)

	# 确保艾瑟萌类型
	@classmethod
	def ensureExermonType(cls, exers=None, exer=None,
						  e_type=ExermonType.Initial):

		if exer is not None:
			if exer.e_type != e_type.value:
				raise ErrorException(ErrorType.InvalidExermonType)

		if exers is not None:
			for exer in exers:
				cls.ensureExermonType(exer=exer, e_type=e_type)

	# 确保艾瑟萌天赋类型
	@classmethod
	def ensureExerGiftType(cls, gifts=None, gift=None, g_type=ExerGiftType.Initial):

		if gift is not None:
			if gift.g_type != g_type.value:
				raise ErrorException(ErrorType.InvalidExerGiftType)

		if gifts is not None:
			for gift in gifts:
				cls.ensureExerGiftType(gift=gift, g_type=g_type)
