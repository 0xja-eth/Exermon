from django.shortcuts import render
from .models import *
from player_module.models import Player
from utils.exception import ErrorType, ErrorException

# Create your views here.


# =======================
# 物品服务类，封装管理物品模块的业务处理函数
# =======================
class Service:
	# 获取背包类容器项数据
	@classmethod
	async def packContainerGet(cls, consumer, player: Player, cid: int):
		# 返回数据：
		# items: 背包容器项数据（数组） => 人类背包项
		container: PackContainer = Common.getContainer(id=cid)

		ViewUtils.ensureObjectType(container, PackContainer, ErrorType.IncorrectContainerType)

		Common.ensureContainerOwner(container, player)

		return {'container': container.convertToDict(type='items')}

	# 获取背包类容器项数据
	@classmethod
	async def slotContainerGet(cls, consumer, player: Player, cid: int):
		# 返回数据：
		# items: 槽容器项数据（数组） => 人类背包项
		container: SlotContainer = Common.getContainer(id=cid)

		ViewUtils.ensureObjectType(container, SlotContainer, ErrorType.IncorrectContainerType)

		Common.ensureContainerOwner(container, player)

		return {'container': container.convertToDict(type='items')}

	# 背包类容器获得物品
	@classmethod
	async def packContainerGain(cls, consumer, player: Player, cid: int, item_id: int, count: int, ):
		# 返回数据：无
		container: PackContainer = Common.getContainer(id=cid)

		ViewUtils.ensureObjectType(container, PackContainer, ErrorType.IncorrectContainerType)

		Common.ensureContainerOwner(container, player)

		item = Common.getItem(id= item_id)

		container.gainItems(item, count)

	# 背包类容器转移
	@classmethod
	async def packContainerTransfer(cls, consumer, player: Player, cid: int, target_cid: int,
									contitem_ids: list, counts: list):
		# 返回数据：无
		container: PackContainer = Common.getContainer(id=cid)
		target: PackContainer = Common.getContainer(id=target_cid)

		ViewUtils.ensureObjectType(container, PackContainer, ErrorType.IncorrectContainerType)
		ViewUtils.ensureObjectType(target, PackContainer, ErrorType.IncorrectContainerType)

		Common.ensureContainerOwner(container, player)

		cont_items = Common.getContItems(ids=contitem_ids)

		container.transferContItems(target, cont_items, counts)

	# 背包类容器拆分
	@classmethod
	async def packContainerSplit(cls, consumer, player: Player, cid: int, contitem_id: int, count: int, ):
		# 返回数据：无
		container: PackContainer = Common.getContainer(id=cid)

		ViewUtils.ensureObjectType(container, PackContainer, ErrorType.IncorrectContainerType)

		Common.ensureContainerOwner(container, player)

		cont_item = Common.getContItem(id=contitem_id)

		container.splitItem(cont_item, count)

	# 背包类容器组合
	@classmethod
	async def packContainerMerge(cls, consumer, player: Player, cid: int, contitem_ids: list, ):
		# 返回数据：无
		container: PackContainer = Common.getContainer(id=cid)

		ViewUtils.ensureObjectType(container, PackContainer, ErrorType.IncorrectContainerType)

		Common.ensureContainerOwner(container, player)

		cont_items = Common.getContItems(ids=contitem_ids)

		container.mergeItem(cont_items)

	# 槽容器装备
	@classmethod
	def slotContainerEquip(cls, player: Player, cid: int, equip_item1_id: int = None,
								 equip_item2_id: int = None, **kwargs):

		container: SlotContainer = Common.getContainer(id=cid)

		ViewUtils.ensureObjectType(container, SlotContainer, ErrorType.IncorrectContainerType)

		Common.ensureContainerOwner(container, player)

		equip_item1 = equip_item2 = None
		if equip_item1_id > 0:
			equip_item1 = Common.getContItem(id=equip_item1_id)
		if equip_item2_id > 0:
			equip_item2 = Common.getContItem(id=equip_item2_id)

		container.setEquip(equip_item=equip_item1, index=1, **kwargs)
		container.setEquip(equip_item=equip_item2, index=2, **kwargs)


# =======================
# 物品公用类，封装关于物品模块的公用函数
# =======================
class Common:

	# 获取物品
	@classmethod
	def getItem(cls, return_type='object', error: ErrorType = ErrorType.ItemNotExist, **args) -> BaseItem:

		return ViewUtils.getObject(BaseItem, error, return_type=return_type, **args).target()

	# 获取容器
	@classmethod
	def getContainer(cls, return_type='object', error: ErrorType = ErrorType.ContainerNotExist,
					 **args) -> BaseContainer:
		return ViewUtils.getObject(BaseContainer, error, return_type=return_type, **args).target()

	# 获取容器项
	@classmethod
	def getContItem(cls, return_type='object', error: ErrorType = ErrorType.ContItemNotExist,
					 **args) -> BaseContItem:
		return ViewUtils.getObject(BaseContItem, error, return_type=return_type, **args).target()

	# 获取容器项（多个）
	@classmethod
	def getContItems(cls, ids, return_type='object', error: ErrorType = ErrorType.ContItemNotExist,
					 **args) -> BaseContItem:

		cont_item = cls.getContItem(id=ids[0], error=error)

		res = ViewUtils.getObjects(type(cont_item), error, return_type=return_type, id__in=ids)

		if res.count() != len(ids): raise ErrorException(error)

		return res

	# 确保容器的持有者
	@classmethod
	def ensureContainerOwner(cls, container: BaseContainer, player):
		if container.ownerPlayer() != player:
			raise ErrorException(ErrorType.ContainerNotOwner)

	# 确保容器项容器正确
	@classmethod
	def ensureContainerItem(cls, container: BaseContainer, cont_item: BaseContItem):
		if cont_item.container_id != container.id:
			raise ErrorException(ErrorType.ContItemNotHold)
