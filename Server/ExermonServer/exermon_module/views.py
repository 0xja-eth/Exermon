from django.shortcuts import render
from .models import *
from player_module.models import Player
from utils.exception import ErrorType, ErrorException

# Create your views here.


# =======================
# 艾瑟萌服务类，封装管理艾瑟萌模块的业务处理函数
# =======================
class Service:

	# 艾瑟萌槽装备
	@classmethod
	async def exerSlotEquip(cls, consumer, player: Player, cid: int, sid: int, peid: int, pgid: int):
		# 返回数据：无
		from item_module.views import Service as ItemService

		Subject.ensure(id=sid)

		ItemService.slotContainerEquip(player, cid, peid, pgid, subject_id=sid)

	# 艾瑟萌装备槽装备
	@classmethod
	async def equipSlotEquip(cls, consumer, player: Player, cid: int, eid: int, eeid: int):
		# 返回数据：无
		from item_module.views import Service as ItemService

		ExerEquipType.ensure(id=eid)

		ItemService.slotContainerEquip(player, cid, eeid, e_type_id=eid)


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
		ViewUtils.ensureRegexp(val, Exermon.NAME_REG, ErrorType.InvalidExermonName)


# =======================
# 艾瑟萌公用类，封装关于艾瑟萌模块的公用函数
# =======================
class Common:

	# 获取艾瑟萌
	@classmethod
	def getExermon(cls, return_type='object', error: ErrorType = ErrorType.ExermonNotExist, **args) -> Exermon:

		return ViewUtils.getObject(Exermon, error, return_type=return_type, **args)

	# 获取艾瑟萌天赋
	@classmethod
	def getExerGift(cls, return_type='object', error: ErrorType = ErrorType.ExerGiftNotExist, **args) -> ExerGift:

		return ViewUtils.getObject(ExerGift, error, return_type=return_type, **args)

	# 获取多个艾瑟萌
	@classmethod
	def getExermons(cls, ids, return_type='object', error: ErrorType = ErrorType.ExermonNotExist) -> Exermon:

		res = ViewUtils.getObjects(Exermon, error, return_type=return_type, id__in=ids)

		if res.count() != len(ids): raise ErrorException(error)

		return res

	# 获取多个艾瑟萌天赋
	@classmethod
	def getExerGifts(cls, ids, return_type='object', error: ErrorType = ErrorType.ExerGiftNotExist) -> ExerGift:

		res = ViewUtils.getObjects(ExerGift, error, return_type=return_type, id__in=ids)

		if res.count() != len(ids): raise ErrorException(error)

		return res

	# 确保艾瑟萌存在
	@classmethod
	def ensureExermonExist(cls, error: ErrorType = ErrorType.ExermonNotExist, **args):
		return ViewUtils.ensureObjectExist(Exermon, error, **args)

	# 确保艾瑟萌天赋存在
	@classmethod
	def ensureExerGiftExist(cls, error: ErrorType = ErrorType.ExerGiftNotExist, **args):
		return ViewUtils.ensureObjectExist(ExerGift, error, **args)

	# 确保艾瑟萌科目合法
	@classmethod
	def ensureExermonSubject(cls, exers):
		sbjs = []

		for exer in exers:
			sid = exer.subject_id
			if sid in sbjs:
				raise ErrorException(ErrorType.InvalidExermonSubject)

			sbjs[sid] = True

		force_sbjs = Subject.get(force=True)

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
			if gift.g_type != g_type:
				raise ErrorException(ErrorType.InvalidExermonType)

		if gifts is not None:
			for gift in gifts:
				cls.ensureExerGiftType(gift=gift, g_type=g_type)
