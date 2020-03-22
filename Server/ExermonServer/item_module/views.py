from django.shortcuts import render
from .models import *
from player_module.models import *
from exermon_module.models import *
from question_module.models import *
from utils.interface_manager import Common as InterfaceCommon
from utils.exception import ErrorType, GameException

# Create your views here.


# =======================
# 物品服务类，封装管理物品模块的业务处理函数
# =======================
class Service:
	# 获取背包类容器项数据
	@classmethod
	async def packContainerGet(cls, consumer, player: Player, type: int, cid=None):
		# 返回数据：
		# items: 背包容器项数据（数组） => 人类背包项

		if cid:
			cid = InterfaceCommon.convertDataType(cid, 'int')
			container: PackContainer = Common.getContainer(type, id=cid)
		else:
			container: PackContainer = Common.getContainer(type, player=player)

		ViewUtils.ensureObjectType(container, PackContainer, ErrorType.IncorrectContainerType)

		return cls._getContainerItems(container)

	# 获取背包类容器项数据
	@classmethod
	async def slotContainerGet(cls, consumer, player: Player, type: int, cid=None):
		# 返回数据：
		# items: 槽容器项数据（数组） => 人类背包项

		if cid:
			cid = InterfaceCommon.convertDataType(cid, 'int')
			container: SlotContainer = Common.getContainer(type, id=cid)
		else:
			container: SlotContainer = Common.getContainer(type, player=player)

		ViewUtils.ensureObjectType(container, SlotContainer, ErrorType.IncorrectContainerType)

		return cls._getContainerItems(container)

	# 背包类容器获得物品
	@classmethod
	async def packContainerGain(cls, consumer, player: Player, type: int, i_type: int,
								item_id: int, count: int, refresh: bool):
		# 返回数据：无

		container: PackContainer = Common.getContainer(type, player=player)

		ViewUtils.ensureObjectType(container, PackContainer, ErrorType.IncorrectContainerType)

		item = Common.getItem(i_type, id=item_id)

		container.gainItems(item, count)

		if refresh: return cls._getContainerItems(container)

	# 背包类容器转移
	@classmethod
	async def packContainerTransfer(cls, consumer, player: Player, type: int, target_cid: int,
									ci_types: list, contitem_ids: list, counts: list):
		# 返回数据：无

		container: PackContainer = Common.getContainer(type, player=player)
		target: PackContainer = Common.getContainer(type, id=target_cid)

		ViewUtils.ensureObjectType(container, PackContainer, ErrorType.IncorrectContainerType)
		ViewUtils.ensureObjectType(target, PackContainer, ErrorType.IncorrectContainerType)

		cont_items = Common.getContItems(contitem_ids, types=ci_types, player=player)

		container.transferContItems(target, cont_items, counts)

		return cls._getContainerItems(container)

	# 背包类容器拆分
	@classmethod
	async def packContainerSplit(cls, consumer, player: Player, type: int, ci_type: int,
								 contitem_id: int, count: int):
		# 返回数据：无

		container: PackContainer = Common.getContainer(type, player=player)

		ViewUtils.ensureObjectType(container, PackContainer, ErrorType.IncorrectContainerType)

		cont_item = Common.getContItem(type_=ci_type, player=player, id=contitem_id)

		container.splitItem(cont_item, count)

		return cls._getContainerItems(container)

	# 背包类容器组合
	@classmethod
	async def packContainerMerge(cls, consumer, player: Player, type: int,
								 ci_type: int, contitem_ids: list):
		# 返回数据：无

		container: PackContainer = Common.getContainer(type, player=player)

		ViewUtils.ensureObjectType(container, PackContainer, ErrorType.IncorrectContainerType)

		cont_items = Common.getSameContItems(contitem_ids, type_=ci_type, player=player)

		container.mergeItem(cont_items)

		return cls._getContainerItems(container)

	# 槽容器装备
	@classmethod
	def slotContainerEquip(cls, container: SlotContainer, equip_items: list, **kwargs):
		"""
		槽容器装备
		Args:
			container (SlotContainer): 对应槽容器
			equip_items (list): 装备项数组
			**kwargs (**dict): 其他装备参数（详见 SlotContainer.setEquip）
		"""

		ViewUtils.ensureObjectType(container, SlotContainer, ErrorType.IncorrectContainerType)

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
			raise GameException(ErrorType.IncorrectItemType)
		ViewUtils.ensureEnumData(val, ItemType, ErrorType.IncorrectItemType, True)

	# 校验物品类型
	@classmethod
	def ensureContainerType(cls, val: int):
		if val == 0:
			raise GameException(ErrorType.IncorrectContainerType)
		ViewUtils.ensureEnumData(val, ContainerType, ErrorType.IncorrectContainerType, True)

	# 校验容器项类型
	@classmethod
	def ensureContItemType(cls, val: int):
		if val == 0:
			raise GameException(ErrorType.IncorrectContItemType)
		ViewUtils.ensureEnumData(val, ContItemType, ErrorType.IncorrectContItemType, True)


# =======================
# 物品公用类，封装关于物品模块的公用函数
# =======================
class Common:

	@classmethod
	def getItem(cls, type_: int = None, cla: type = None,
				error: ErrorType = ErrorType.ItemNotExist, **kwargs) -> BaseItem:
		"""
		获取一定条件的物品
		Args:
			type_ (int): 类型（枚举值）
			cla (type): 类型（类）
			error (ErrorType): 不存在时抛出异常类型
			**kwargs (**dict): 查询参数
		Returns:
			返回符合条件的物品（若有多个返回第一个）
		"""
		if cla is None:
			Check.ensureItemType(type_)
			cla = eval(ItemType(type_).name)

		return ViewUtils.getObject(cla, error, **kwargs)

	@classmethod
	def getContainer(cls, type_: int = None, cla: type = None, player: Player = None,
					 error: ErrorType = ErrorType.ContainerNotExist, **kwargs) -> BaseContainer:
		"""
		获取一定条件的容器
		若传入 player，将会在指定玩家的缓存内进行搜索（返回的是玩家内缓存的数据）
		Args:
			type_ (int): 类型（枚举值）
			cla (type): 类型（类）
			player (Player): 所属玩家
			error (ErrorType): 不存在时抛出异常类型
			**kwargs (**dict): 查询参数
		Returns:
			返回符合条件的容器（若有多个返回第一个）
		"""
		if cla is None:
			Check.ensureContainerType(type_)
			cla = eval(ContainerType(type_).name)

		if player is None:
			return ViewUtils.getObject(cla, error, **kwargs)

		return player.getContainer(cla)

	@classmethod
	def getContItem(cls, type_: int = None, cla: type = None,
					player: Player = None, container: BaseContainer = None,
					error: ErrorType = ErrorType.ContItemNotExist, **kwargs) -> BaseContItem:
		"""
		获取一定条件的容器项
		若传入 player，将会在指定玩家的对应容器内进行搜索（返回的是玩家内缓存的数据）
		若传入 container 亦同理
		Args:
			type_ (int): 类型（枚举值）
			cla (type): 类型（类）
			player (Player): 所属玩家
			container (BaseContainer): 所属容器
			error (ErrorType): 不存在时抛出异常类型
			**kwargs (**dict): 查询参数
		Returns:
			返回符合条件的容器项（若有多个返回第一个）
		"""
		if container is None:

			if cla is None:
				Check.ensureContItemType(type_)
				cla = eval(ContItemType(type_).name)

			if player is None:
				return ViewUtils.getObject(cla, error, **kwargs)

			container_cla = cla.containerClass()

			container = cls.getContainer(cla=container_cla, player=player)

		cont_item = container.contItem(cla=cla, **kwargs)

		if cont_item is None: raise GameException(error)

		return cont_item

	# 获取容器项（多个）
	@classmethod
	def getContItems(cls, ids: list, types: list = None, clas: list = None,
					 player: Player = None, container: BaseContainer = None,
					 error: ErrorType = ErrorType.ContItemNotExist) -> list:
		"""
		获取多个容器项（根据ID）
		Args:
			ids (list): 容器项ID列表
			types (list): 容器项类型列表（枚举值）
			clas (list): 容器项类型列表（类）
			player (Player): 所属玩家
			container (BaseContainer): 所属容器
			error (ErrorType): 不存在时抛出异常类型
		Returns:
			返回指定ID集合的容器项数组
		"""
		res = []

		for i in len(range(ids)):
			cont_item = None

			if types is not None:
				cont_item = cls.getContItem(
					type_=types[i], player=player, container=container,
					error=error, id=ids[i])

			elif clas is not None:
				cont_item = cls.getContItem(
					cla=clas[i], player=player, container=container,
					error=error, id=ids[i])

			if cont_item is None: raise GameException(error)

			res.append(cont_item)

		return res

	# 获取容器项（同种类型，多个）
	@classmethod
	def getSameContItems(cls, ids: list, type_: int = None, cla: type = None,
						 player: Player = None, container: BaseContainer = None,
						 error: ErrorType = ErrorType.ContItemNotExist, **kwargs) -> list:
		"""
		获取多个相同类型的容器项（根据ID）
		Args:
			ids (list): 容器项ID列表
			type_ (int): 类型（枚举值）
			cla (type): 类型（类）
			player (Player): 所属玩家
			container (BaseContainer): 所属容器
			error (ErrorType): 不存在时抛出异常类型
		Returns:
			返回指定ID集合的容器项数组
		"""
		if container is None:

			if cla is None:
				Check.ensureContItemType(type_)
				cla = eval(ContItemType(type_).name)

			if player is None:
				return ViewUtils.getObject(cla, error, **kwargs)

			container_cla = cla.containerClass()

			container = cls.getContainer(cla=container_cla, player=player)

		cont_items = container.contItems(
			cla=cla, cond=lambda x: x.id in ids)

		if len(cont_items) != len(ids): raise GameException(error)

		return cont_items

	# 确保容器的持有者
	@classmethod
	def ensureContainerOwnerPlayer(cls, container: BaseContainer, player,
								   error=ErrorType.ContainerNotOwner):
		if container.exactlyPlayer() != player: raise GameException(error)

	# 确保容器项的持有者
	@classmethod
	def ensureContItemOwnerPlayer(cls, cont_item: BaseContItem, player,
								  error=ErrorType.ContItemNotHold):
		container = cont_item.container

		if container is None: raise GameException(error)

		cls.ensureContainerOwnerPlayer(container, player, error)

	# 确保容器项容器正确
	@classmethod
	def ensureContainerItem(cls, container: BaseContainer, cont_item: BaseContItem,
							error=ErrorType.ContItemNotHold):
		if cont_item.container_id != container.id: raise GameException(error)
