from django.shortcuts import render
from .models import *
from player_module.models import *
from exermon_module.models import *
from question_module.models import *
from utils.exception import ErrorType, ErrorException

# Create your views here.


# =======================
# 物品服务类，封装管理物品模块的业务处理函数
# =======================
class Service:
	# 获取背包类容器项数据
	@classmethod
	async def packContainerGet(cls, consumer, player: Player, type: int, cid: int):
		# 返回数据：
		# items: 背包容器项数据（数组） => 人类背包项
		container: PackContainer = Common.getContainer(type, id=cid)

		ViewUtils.ensureObjectType(container, PackContainer, ErrorType.IncorrectContainerType)

		Common.ensureContainerOwnerPlayer(container, player)

		return cls._getContainerItems(container)

	# 获取背包类容器项数据
	@classmethod
	async def slotContainerGet(cls, consumer, player: Player, type: int, cid: int):
		# 返回数据：
		# items: 槽容器项数据（数组） => 人类背包项
		container: SlotContainer = Common.getContainer(type, id=cid)

		ViewUtils.ensureObjectType(container, SlotContainer, ErrorType.IncorrectContainerType)

		Common.ensureContainerOwnerPlayer(container, player)

		return cls._getContainerItems(container)

	# 背包类容器获得物品
	@classmethod
	async def packContainerGain(cls, consumer, player: Player, type: int, cid: int,
								i_type: int, item_id: int, count: int, refresh: bool):
		# 返回数据：无
		container: PackContainer = Common.getContainer(type, id=cid)

		ViewUtils.ensureObjectType(container, PackContainer, ErrorType.IncorrectContainerType)

		Common.ensureContainerOwnerPlayer(container, player)

		item = Common.getItem(i_type, id=item_id)

		container.gainItems(item, count)

		if refresh: return cls._getContainerItems(container)

	# 背包类容器转移
	@classmethod
	async def packContainerTransfer(cls, consumer, player: Player, type: int, cid: int, target_cid: int,
									ci_types: list, contitem_ids: list, counts: list):
		# 返回数据：无
		container: PackContainer = Common.getContainer(type, id=cid)
		target: PackContainer = Common.getContainer(type, id=target_cid)

		ViewUtils.ensureObjectType(container, PackContainer, ErrorType.IncorrectContainerType)
		ViewUtils.ensureObjectType(target, PackContainer, ErrorType.IncorrectContainerType)

		Common.ensureContainerOwnerPlayer(container, player)

		cont_items = Common.getContItems(ci_types, ids=contitem_ids)

		container.transferContItems(target, cont_items, counts)

		return cls._getContainerItems(container)

	# 背包类容器拆分
	@classmethod
	async def packContainerSplit(cls, consumer, player: Player, type: int, cid: int,
								 ci_type: int, contitem_id: int, count: int, ):
		# 返回数据：无
		container: PackContainer = Common.getContainer(type, id=cid)

		ViewUtils.ensureObjectType(container, PackContainer, ErrorType.IncorrectContainerType)

		Common.ensureContainerOwnerPlayer(container, player)

		cont_item = Common.getContItem(ci_type, id=contitem_id)

		container.splitItem(cont_item, count)

		return cls._getContainerItems(container)

	# 背包类容器组合
	@classmethod
	async def packContainerMerge(cls, consumer, player: Player, type: int, cid: int,
								 ci_type: int, contitem_ids: list, ):
		# 返回数据：无
		container: PackContainer = Common.getContainer(type, id=cid)

		ViewUtils.ensureObjectType(container, PackContainer, ErrorType.IncorrectContainerType)

		Common.ensureContainerOwnerPlayer(container, player)

		cont_items = Common.getSameContItems(ci_type, ids=contitem_ids)

		container.mergeItem(cont_items)

		return cls._getContainerItems(container)

	# 槽容器装备
	@classmethod
	def slotContainerEquip(cls, player: Player, container: SlotContainer, equip_items: list, **kwargs):

		ViewUtils.ensureObjectType(container, SlotContainer, ErrorType.IncorrectContainerType)

		Common.ensureContainerOwnerPlayer(container, player)

		for equip_item in equip_items:

			container.setEquip(equip_item=equip_item, **kwargs)

	# 获取容器项
	@classmethod
	def _getContainerItems(cls, container):
		return {'container': container.convertToDict(type='items')}


# =======================
# 用户校验类，封装用户业务数据格式校验的函数
# =======================
class Check:

	# 校验物品类型
	@classmethod
	def ensureItemType(cls, val: int):
		if val == 0:
			raise ErrorException(ErrorType.IncorrectItemType)
		ViewUtils.ensureEnumData(val, ItemType, ErrorType.IncorrectItemType, True)

	# 校验物品类型
	@classmethod
	def ensureContainerType(cls, val: int):
		if val == 0:
			raise ErrorException(ErrorType.IncorrectContainerType)
		ViewUtils.ensureEnumData(val, ContainerType, ErrorType.IncorrectContainerType, True)

	# 校验容器项类型
	@classmethod
	def ensureContItemType(cls, val: int):
		if val == 0:
			raise ErrorException(ErrorType.IncorrectContItemType)
		ViewUtils.ensureEnumData(val, ContItemType, ErrorType.IncorrectContItemType, True)


# =======================
# 物品公用类，封装关于物品模块的公用函数
# =======================
class Common:

	# 获取物品
	@classmethod
	def getItem(cls, type, return_type='object', error: ErrorType = ErrorType.ItemNotExist, **kwargs) -> BaseItem:

		Check.ensureItemType(type)

		cla = eval(ItemType(type).name)

		return ViewUtils.getObject(cla, error, return_type=return_type, **kwargs)

	# 获取容器
	@classmethod
	def getContainer(cls, type, return_type='object', error: ErrorType = ErrorType.ContainerNotExist,
					 **kwargs) -> BaseContainer:

		Check.ensureContainerType(type)

		cla = eval(ContainerType(type).name)

		return ViewUtils.getObject(cla, error, return_type=return_type, **kwargs)

	# 获取容器项
	@classmethod
	def getContItem(cls, type, return_type='object', error: ErrorType = ErrorType.ContItemNotExist,
					 **kwargs) -> BaseContItem:

		Check.ensureContItemType(type)

		cla = eval(ContItemType(type).name)

		return ViewUtils.getObject(cla, error, return_type=return_type, **kwargs)

	# 获取容器项（多个）
	@classmethod
	def getContItems(cls, types, ids, return_type='object', error: ErrorType = ErrorType.ContItemNotExist,
					 **kwargs) -> BaseContItem:
		res = []

		for i in len(range(ids)):
			res.append(cls.getContItem(types[i], return_type=return_type,
									   error=error, id=ids[i], **kwargs))
		return res

	# 获取容器项（同种类型，多个）
	@classmethod
	def getSameContItems(cls, type, ids, return_type='object', error: ErrorType = ErrorType.ContItemNotExist,
					 **kwargs) -> BaseContItem:

		Check.ensureItemType(type)

		cla = eval(ContainerType(type).name)

		res = ViewUtils.getObjects(cla, return_type=return_type, id__in=ids, **kwargs)

		# 数量不一致，说明获取出现问题
		if len(ids) != res.count(): raise ErrorException(error)

		return res

	# 确保容器的持有者
	@classmethod
	def ensureContainerOwnerPlayer(cls, container: BaseContainer, player):
		if container.exactlyPlayer() != player:
			raise ErrorException(ErrorType.ContainerNotOwner)

	# 确保容器项容器正确
	@classmethod
	def ensureContainerItem(cls, container: BaseContainer, cont_item: BaseContItem):
		if cont_item.container_id != container.id:
			raise ErrorException(ErrorType.ContItemNotHold)
