from django.db import models
from django.db.models import Sum, F
from django.conf import settings
from game_module.models import ParamValue, GameConfigure, ExerEquipType, HumanEquipType
from utils.model_utils import ItemIconUpload, Common as ModelUtils
from utils.view_utils import Common as ViewUtils
from utils.exception import ErrorType, ErrorException
from enum import Enum
import jsonfield, os, math

# region 基本物品


# ===================================================
#  物品类型枚举
# ===================================================
class ItemType(Enum):
	Unset = 0  # 未设置

	# ===！！！！不能轻易修改序号！！！！===
	# LimitedItem 1~100
	# UsableItem
	HumanItem = 1  # 人类物品
	ExerItem = 2  # 艾瑟萌物品

	# EquipableItem
	HumanEquip = 11  # 人类装备
	ExerEquip = 12  # 艾瑟萌装备

	# InfiniteItem 101+
	QuesSugar = 101  # 题目糖

	Exermon = 201  # 艾瑟萌
	ExerSkill = 202  # 艾瑟萌技能
	ExerGift = 203  # 艾瑟萌天赋
	ExerFrag = 204  # 艾瑟萌碎片


# ===================================================
#  容器类型枚举
# ===================================================
class ContainerType(Enum):
	Unset = 0  # 未设置

	# ===！！！！不能轻易修改序号！！！！===
	# Pack 1~100
	# 可转移
	HumanPack = 1  # 人类背包
	ExerPack = 2  # 艾瑟萌背包

	# 可转移，可叠加
	ExerFragPack = 11  # 艾瑟萌碎片背包

	# 可转移，可叠加
	QuesSugarPack = 21  # 题目糖背包

	# Slot 101~200
	# 可转移
	HumanEquipSlot = 101  # 人类装备槽
	ExerEquipSlot = 102  # 艾瑟萌装备槽

	# 可转移
	ExerSlot = 111  # 艾瑟萌槽

	# 可转移
	ExerSkillSlot = 121  # 艾瑟萌技能槽

	# Others 201+
	# 可转移
	ExerGiftPool = 201  # 艾瑟萌天赋池
	# 可转移
	ExerHub = 202  # 艾瑟萌仓库


# ===================================================
#  容器物品枚举
# ===================================================
class ContItemType(Enum):
	Unset = 0  # 未设置

	# ===！！！！不能轻易修改序号！！！！===
	# Pack 1~100
	HumanPackItem = 1  # 人类背包物品
	ExerPackItem = 2  # 艾瑟萌背包物品
	HumanPackEquip = 3  # 人类背包装备
	ExerPackEquip = 4  # 艾瑟萌背包装备

	# 可叠加
	ExerFragPackItem = 11  # 艾瑟萌碎片背包物品

	# 可叠加
	QuesSugarPackItem = 21  # 题目糖背包物品

	# Slot 101~200
	HumanEquipSlotItem = 101  # 人类装备槽项
	ExerEquipSlotItem = 102  # 艾瑟萌装备槽项

	ExerSlotItem = 111  # 艾瑟萌槽项

	ExerSkillSlotItem = 121  # 艾瑟萌技能槽物品

	# Others 201+
	PlayerExerGift = 201  # 玩家艾瑟萌天赋关系

	PlayerExermon = 202  # 玩家艾瑟萌关系


# ===================================================
#  货币表
# ===================================================
class Currency(models.Model):
	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "货币"

	# 默认金币
	DEFAULT_GOLD = 0

	# 金币
	gold = models.PositiveIntegerField(default=DEFAULT_GOLD, verbose_name="金币")

	# 点券
	ticket = models.PositiveIntegerField(default=0, verbose_name="点券")

	# 绑定点券
	bound_ticket = models.PositiveIntegerField(default=0, verbose_name="金币")

	def __str__(self):
		term: GameConfigure = GameConfigure.get()
		return '%s：%d %s：%d %s：%d' % (
			term.gold, self.gold,
			term.ticket, self.ticket,
			term.bound_ticket, self.bound_ticket)

	def convertToDict(self):
		return {
			'gold': self.gold,
			'ticket': self.ticket,
			'bound_ticket': self.bound_ticket
		}


# region 物品


# ===================================================
#  基础物品表
# ===================================================
class BaseItem(models.Model):
	class Meta:

		verbose_name = verbose_name_plural = "基础物品"

	TYPES = [
		(ItemType.Unset.value, '未知物品'),

		# LimitedItem
		# UsableItem
		(ItemType.HumanItem.value, '人类物品'),
		(ItemType.ExerItem.value, '艾瑟萌物品'),

		# EquipableItem
		(ItemType.HumanEquip.value, '人类装备'),
		(ItemType.ExerEquip.value, '艾瑟萌装备'),

		# InfiniteItem
		(ItemType.QuesSugar.value, '题目糖'),

		(ItemType.Exermon.value, '艾瑟萌'),
		(ItemType.ExerSkill.value, '艾瑟萌技能'),
		(ItemType.ExerGift.value, '艾瑟萌天赋'),
		(ItemType.ExerFrag.value, '艾瑟萌碎片'),
	]

	# 道具类型
	TYPE = ItemType.Unset

	# 名称
	name = models.CharField(max_length=24, verbose_name="名称")

	# 描述
	description = models.CharField(max_length=128, blank=True, verbose_name="描述")

	# 物品 标识类型
	type = models.PositiveSmallIntegerField(default=ItemType.Unset.value,
											choices=TYPES, verbose_name="物品类型")

	# region 配置项

	# 对应的容器项类
	@classmethod
	def contItemClass(cls): return BaseContItem

	# endregion

	# region 类型配置

	def __init__(self, *args, **kwargs):
		super().__init__(*args, **kwargs)
		if self.type == ItemType.Unset.value:
			self.type = self.TYPE.value

	def __str__(self):
		type_name = self.getTypeName()

		return '%d %s（%s）' % (self.id, self.name, type_name)

	def __getattr__(self, item):
		raise AttributeError(item)

	# 获取类型名称
	def getTypeName(self):
		for type in self.TYPES:
			if self.type == type[0]:
				return type[1]

		return self.TYPES[0][1]

	# 获取目标的物品
	# noinspection PyUnresolvedReferences
	def target(self):

		from exermon_module.models import Exermon, ExerGift, \
			ExerFrag, ExerSkill, ExerEquip, ExerItem
		from player_module.models import HumanItem, HumanEquip

		target = BaseItem

		type_ = ItemType(self.type)
		if type_ != ItemType.Unset:
			target = eval(type_.name)

		if type(self) == target: return self

		return ViewUtils.getObject(target, ErrorType.ItemNotExist,
								   return_type='object', id=self.id)

	target.short_description = "目标物品"

	# 转化为 dict
	def _convertToDict(self, **kwargs):
		return {
			'id': self.id,
			'name': self.name,
			'description': self.description,
			'type': self.type
		}

	# 转化为 dict
	def convertToDict(self, **kwargs):
		return self.target()._convertToDict(**kwargs)

	# endregion

	# 最大叠加数量（为0则不限）
	def _maxCount(self): return 1

	# 最大叠加数量（为0则不限）
	def maxCount(self):
		return self.target()._maxCount()


# ===================================================
#  物品价格表
# ===================================================
class ItemPrice(Currency):
	class Meta:
		verbose_name = verbose_name_plural = "物品价格"

	# 对应物品
	item = models.OneToOneField("LimitedItem", on_delete=models.CASCADE,
									 null=True, verbose_name="物品")

	# def __str__(self):
	# 	return '%s：%s' % (self.limiteditem, super().__str__())


# ===================================================
#  有限物品表（一般为背包的物品）
# ===================================================
class LimitedItem(BaseItem):

	class Meta:
		# abstract = True
		verbose_name = verbose_name_plural = "有限物品"

	# 物品星级
	star = models.ForeignKey("game_module.ItemStar", on_delete=models.CASCADE, verbose_name="星级")

	# 出售价格（出售固定为金币，为0则不可出售）
	sell_price = models.PositiveIntegerField(default=0, verbose_name="出售价格")

	# 是否可丢弃
	discardable = models.BooleanField(default=True, verbose_name="可丢弃")

	# 能否交易
	tradable = models.BooleanField(default=True, verbose_name="可交易")

	# 物品图标
	icon = models.ImageField(upload_to=ItemIconUpload(),
							 null=True, blank=True, verbose_name="图标")

	# 管理界面用：显示购入价格
	def adminBuyPrice(self):
		return self.itemprice

	adminBuyPrice.short_description = "购入价格"

	# 获取购买价格
	def buyPrice(self):
		try: return self.itemprice
		except ItemPrice.DoesNotExist: return None

	# 获取完整路径
	def getExactlyIconPath(self):
		base = settings.STATIC_URL
		path = os.path.join(base, str(self.icon))
		if os.path.exists(path):
			return path
		else:
			raise ErrorException(ErrorType.PictureFileNotFound)

	# 获取图标base64编码
	def convertIconToBase64(self):
		import base64

		with open(self.getExactlyIconPath(), 'rb') as f:
			data = base64.b64encode(f.read())

		return data.decode()

	# 转化为 dict
	def _convertToDict(self, **kwargs):
		res = super()._convertToDict(**kwargs)

		buy_price = ModelUtils.objectToDict(self.buyPrice())

		res['star_id'] = self.star_id
		res['buy_price'] = buy_price
		res['sell_price'] = self.sell_price
		res['discardable'] = self.discardable
		# res['icon'] = self.convertIconToBase64()

		return res


# ===================================================
#  可用物品表
# ===================================================
class UsableItem(LimitedItem):

	class Meta:
		verbose_name = verbose_name_plural = "可用物品"

	# 叠加数量
	max_count = models.PositiveSmallIntegerField(default=99, verbose_name="叠加数量")

	# 对战道具
	battle_use = models.BooleanField(default=True, verbose_name="对战道具")

	# 背包道具
	menu_use = models.BooleanField(default=True, verbose_name="背包道具")

	# 冒险道具
	adventure_use = models.BooleanField(default=True, verbose_name="冒险道具")

	# 消耗品
	consumable = models.BooleanField(default=False, verbose_name="消耗品")

	# 冻结回合
	freeze = models.PositiveSmallIntegerField(default=0, verbose_name="冻结回合")

	# 物品类型
	i_type = models.ForeignKey("game_module.UsableItemType",
							   on_delete=models.CASCADE, verbose_name="物品类型")

	# 后台管理：显示使用效果
	def adminEffects(self):
		from django.utils.html import format_html

		effects = self.effects()

		res = ''
		for e in effects:
			res += e.describe() + "<br>"

		return format_html(res)

	adminEffects.short_description = "使用效果"

	# 转化为 dict
	def _convertToDict(self, **kwargs):
		res = super()._convertToDict(**kwargs)

		effects = ModelUtils.objectsToDict(self.effects())

		res['max_count'] = self.max_count
		res['consumable'] = self.consumable
		res['battle_use'] = self.battle_use
		res['menu_use'] = self.menu_use
		res['adventure_use'] = self.adventure_use
		res['freeze'] = self.freeze
		res['i_type'] = self.i_type_id
		res['effects'] = effects

		return res

	# 最大叠加数量（为0则不限）
	def _maxCount(self): return self.max_count

	# 获取所有的效果
	def effects(self):
		return self.itemeffect_set.all()


# ===================================================
#  装备属性值表
# ===================================================
class EquipParam(ParamValue):

	class Meta:
		verbose_name = verbose_name_plural = "装备属性值"

	# 装备
	equip = models.ForeignKey("EquipableItem", on_delete=models.CASCADE, verbose_name="装备")

	# 最大值
	def maxVal(self):
		return None

	# 最小值
	def minVal(self):
		return None


# ===================================================
#  可装备物品
# ===================================================
class EquipableItem(LimitedItem):

	class Meta:
		verbose_name = verbose_name_plural = "可装备物品"

	# 转化为 dict
	def _convertToDict(self, **kwargs):
		res = super()._convertToDict(**kwargs)

		res['params'] = ModelUtils.objectsToDict(self.params())

		return res

	# 获取所有的属性基本值
	def params(self):
		return self.equipparam_set.all()

	# # 用于获取属性值
	# def __getattr__(self, item):
	# 	param = self.param(attr=item)
	# 	if param is None: return super().__getattr__(item)
	# 	return param

	# 获取属性值
	def param(self, param_id=None, attr=None):
		param = None
		if param_id is not None:
			param = self.params().filter(param_id=param_id)
		if attr is not None:
			param = self.params().filter(param__attr=attr)

		if param is None or not param.exists(): return None

		return param.first().getValue()


#
# # ===================================================
# #  无限物品表
# # ===================================================
# class InfiniteItem(BaseItem):
#
# 	class Meta:
# 		abstract = True
# 		verbose_name = verbose_name_plural = "无限物品"
#
# 	# 转化为 dict
# 	def convertToDict(self):
# 		res = BaseItem.convertToDict(self)
#
# 		return res

# endregion

# region 容器


# ===================================================
#  基本容器
# ===================================================
class BaseContainer(models.Model):

	class Meta:
		verbose_name = verbose_name_plural = "基本容器"

	TYPES = [
		(ContainerType.Unset.value, '未知容器'),

		# Pack
		(ContainerType.HumanPack.value, '人类背包'),
		(ContainerType.ExerPack.value, '艾瑟萌背包'),

		(ContainerType.ExerFragPack.value, '艾瑟萌碎片背包'),

		(ContainerType.QuesSugarPack.value, '题目糖背包'),

		# Slot
		(ContainerType.HumanEquipSlot.value, '人类装备槽'),
		(ContainerType.ExerEquipSlot.value, '艾瑟萌装备槽'),

		(ContainerType.ExerSlot.value, '艾瑟萌槽'),

		(ContainerType.ExerSkillSlot.value, '艾瑟萌技能槽'),

		# Others
		(ContainerType.ExerGiftPool.value, '艾瑟萌天赋池'),
		(ContainerType.ExerHub.value, '艾瑟萌仓库'),
	]

	# 容器类型
	TYPE = ContainerType.Unset

	# 玩家
	# player = models.ForeignKey('player_module.Player', on_delete=models.CASCADE,
	#						   verbose_name="玩家")

	# 容器标识类型
	type = models.PositiveSmallIntegerField(default=ContainerType.Unset.value,
											choices=TYPES, verbose_name="容器类型")

	# 缓存容器内的所有 cont_items
	# {'args': **kwargs, 'res': 容器项[]}
	cache_cont_items = None

	# region 配置项

	# 所接受的容器项类
	@classmethod
	def acceptedContItemClass(cls): return BaseContItem

	# 默认容器容量（0为无限）
	@classmethod
	def defaultCapacity(cls): return 0

	# endregion

	# region 创建函数

	# 创建实例
	@classmethod
	def create(cls, **kwargs):
		container = cls._create(**kwargs)
		container.save()
		return container

	# 创建实例
	@classmethod
	def _create(cls, **kwargs):
		container = cls()
		return container

	# endregion

	# region 类型配置

	def __init__(self, *args, **kwargs):
		super().__init__(*args, **kwargs)
		if self.type == ContainerType.Unset.value:
			self.type = self.TYPE.value

	def __str__(self):
		type_name = self.getTypeName()

		return '%d %s' % (self.id, type_name)

	def __getattr__(self, item):
		raise AttributeError(item)

	# 获取类型名称
	def getTypeName(self):
		for type in self.TYPES:
			if self.type == type[0]:
				return type[1]

		return self.TYPES[0][1]

	# 获取目标的容器
	# noinspection PyUnresolvedReferences
	def target(self):

		from exermon_module.models import ExerHub, \
			ExerGiftPool, ExerFragPack, ExerPack, \
			ExerSlot, ExerSkillSlot, ExerEquipSlot
		from player_module.models import HumanPack, HumanEquipSlot

		target = BaseItem

		type_ = ContainerType(self.type)
		if type_ != ContainerType.Unset:
			target = eval(type_.name)

		if type(self) == target: return self

		return ViewUtils.getObject(target, ErrorType.ContainerNotExist,
								   return_type='object', id=self.id)

	# 转化容器项为 dict
	def _convertItemsToDict(self):
		return ModelUtils.objectsToDict(self.contItems())

	# 转化为 dict
	def _convertToDict(self, **kwargs):

		type = None

		res = {
			'id': self.id,
			'type': self.type,
			'capacity': self.getCapacity(),
		}

		if 'type' in kwargs: type = kwargs['type']

		if type == 'items':
			res['items'] = self._convertItemsToDict()

		return res

	def convertToDict(self, **kwargs):
		return self.target()._convertToDict(**kwargs)

	# endregion

	# 持有玩家
	def ownerPlayer(self): pass

	# 获取对应的容器项（创建容器项时候调用）
	def contItemClass(self, **kwargs):
		return self.acceptedContItemClass()

	# 获取容器容量（0为无限）
	def getCapacity(self): return self.defaultCapacity()

	# 物品类型是否接受
	def isItemAcceptable(self, cont_item=None, **kwargs):
		if cont_item is not None:
			# cont_item = cont_item.target()
			return isinstance(cont_item, self.acceptedContItemClass())

		return False

	# 获取所有的容器物品关系
	def contItems(self, **kwargs):
		# 查询缓存，如果有相同的参数，直接返回
		cache = self.cache_cont_items
		if cache is not None and cache['params'] == kwargs:
			return cache['res']

		res = self._contItems(**kwargs)

		# 配置缓存
		self.cache_cont_items = {'params': kwargs, 'res': res}

		return res

	# 获取所有的容器物品关系
	def _contItems(self, **kwargs):
		cla = self.contItemClass(**kwargs)

		if 'item' in kwargs and kwargs['item'] is not None:
			kwargs['item_id'] = kwargs['item'].id
			kwargs.pop('item')

		return ViewUtils.getObjects(cla, container_id=self.id, **kwargs)

	# 以人类可理解的形式展示该容器
	def show(self, name):
		self.clearCache()
		cont_items = self.contItems()

		print("%s(%s) ======================" % (name, self))
		print("Capacity: %d" % self.getCapacity())
		print("Used: %d" % self.contItemCnt())
		print("ContItems:")
		for cont_item in cont_items:
			print(cont_item)

	# 清空缓存
	def clearCache(self):
		self.cache_cont_items = None

	# 获取指定物品数量或所有物品总数（占用格子数）
	def contItemCnt(self, **kwargs):
		return self.contItems(**kwargs).count()

	# 获取指定物品数量或所有物品总数（总数）
	def itemCnt(self, **kwargs):
		return self.contItemCnt(**kwargs)

	# 容器是否存在指定物品或任意物品指定数量
	def hasItem(self, count, **kwargs):
		return self.itemCnt(**kwargs) >= count

	# 获取一个容器项
	def getContItem(self, **kwargs):
		cont_items = self.contItems(**kwargs)
		if cont_items.exists():
			return cont_items.first().target()

		return None

	# 创建一个容器项
	# cla: 直接提供一个容器项类型
	# cont_item: 从提供的已有容器项中复制
	# save: 创建后是否立刻 save
	def _createContItem(self, cla=None, cont_item=None, **kwargs):
		# 如果没有提供 cla
		if cla is None:
			# 如果有提供已有容器项，直接求出 cla
			if cont_item is not None: cla = type(cont_item)
			# 否则，计算出 cla
			else: cla = self.contItemClass(**kwargs)

		# 创建新容器项
		new_cont_item = cla()
		new_cont_item.create(self, **kwargs)

		# 如果有参照的容器项，调用复制函数
		if cont_item is not None:
			new_cont_item.copy(cont_item)

		# 如果需要立刻 save，调用 save()
		# if save: new_cont_item.save()

		return new_cont_item

	# 确保物品类型可接受
	def ensureItemAcceptable(self, **kwargs):
		if not self.isItemAcceptable(**kwargs):
			raise ErrorException(ErrorType.IncorrectItemType)

		return True

	# 准备转移
	def prepareTransfer(self, **kwargs) -> dict:
		return kwargs

	# 接受转移
	def acceptTransfer(self, **kwargs):
		pass  # self.ensureItemAcceptable(**kwargs)

	# 转移物品（从 self 转移到 container）
	def transfer(self, container, **kwargs):
		container = container.target()
		container.acceptTransfer(**self.prepareTransfer(**kwargs))


# ===================================================
#  槽类容器
# ===================================================
class SlotContainer(BaseContainer):

	class Meta:
		verbose_name = verbose_name_plural = "槽类容器"

	# 装备容器1
	equip_container1 = models.ForeignKey('PackContainer', null=True, blank=True, related_name='equip_container1',
									   on_delete=models.CASCADE, verbose_name="装备容器1")

	# 装备容器2
	equip_container2 = models.ForeignKey('PackContainer', null=True, blank=True, related_name='equip_container2',
									   on_delete=models.CASCADE, verbose_name="装备容器2")

	target_equip_container1 = target_equip_container2 = None

	# 默认容器容量（0为无限）
	@classmethod
	def defaultCapacity(cls): return 1

	# 创建实例
	@classmethod
	def create(cls, **kwargs):
		container = cls._create(**kwargs)
		container.save()
		container.createSlots(**kwargs)
		return container

	# 所接受的槽项类（用于 contItemClass ）
	@classmethod
	def acceptedSlotItemClass(cls): return SlotContItem

	# 所接受的容器项类
	@classmethod
	def acceptedContItemClass(cls): return PackContItem

	# 目标装备项1
	def targetEquipContainer1(self):
		if self.target_equip_container1 is None:
			if self.equip_container1 is None:
				self.target_equip_container1 = None
			else:
				self.target_equip_container1 = self.equip_container1.target()

		return self.target_equip_container1

	# 目标装备项2
	def targetEquipContainer2(self):
		if self.target_equip_container2 is None:
			if self.equip_container2 is None:
				self.target_equip_container2 = None
			else:
				self.target_equip_container2 = self.equip_container2.target()

		return self.target_equip_container2

	# 获取对应的容器项（创建容器项时候调用）
	def contItemClass(self, **kwargs):
		return self.acceptedSlotItemClass()

	# 物品类型是否接受
	def isItemAcceptable(self, slot_item=None, cont_item=None, **kwargs):
		if slot_item is not None:
			# slot_item = slot_item.target()
			return isinstance(slot_item, self.acceptedSlotItemClass())

		return super().isItemAcceptable(cont_item=cont_item)

	# 创建一组槽
	def createSlots(self, **kwargs):
		# slot_items = []
		cla = self.contItemClass(**kwargs)
		cnt = self.getCapacity()

		for i in range(cnt):
			slot = self._createSlot(cla, i, **kwargs)
			slot.save()
			slot.afterCreated(**kwargs)

		# cla.objects.bulk_create(slot_items)

	# 创建一个槽
	def _createSlot(self, cla, index, **kwargs):
		slot_item: SlotContItem = self._createContItem(cla, **kwargs)
		slot_item.setupIndex(index+1, **kwargs)
		return slot_item

	# 保证满足装备条件
	def ensureEquipCondition(self, slot_item, equip_item):
		self.ensureItemAcceptable(cont_item=equip_item)

		return True

	# 保证满足装备卸下条件
	def ensureDequipCondition(self, slot_item, type=None, index=None):
		return True

	# 设置装备（装备/卸下）
	def setEquip(self, slot_item=None, equip_item=None, type=None, index=None, **kwargs):

		# 计算槽项
		if slot_item is None:
			slot_item: SlotContItem = self.getContItem(**kwargs)

		# 计算索引
		if index is None:
			index = slot_item.getEquipItemIndex(equip_item=equip_item, type=type)

		# 找出容器
		container = None

		if index == 1: container = self.targetEquipContainer1()
		if index == 2: container = self.targetEquipContainer2()

		if container is None: return

		self.transfer(container, slot_item=slot_item, index=index)

		if equip_item is not None:
			container.transfer(self, slot_item=slot_item, cont_item=equip_item)

	# 槽装备（强制装备）
	def equipSlot(self, slot_item=None, equip_index=None, equip_item=None, **kwargs):

		if slot_item is None:
			slot_item = self.getContItem(**kwargs)

		if slot_item is None:
			return None
		else:
			slot_item: SlotContItem = slot_item
			self.ensureEquipCondition(slot_item, equip_item)

			self.clearCache()
			slot_item.equip(index=equip_index, equip_item=equip_item)
			slot_item.save()

			if equip_item is not None:
				equip_item.save()

	# 槽卸下（强制卸下）
	def dequipSlot(self, slot_item=None, type=None, index=None, **kwargs):

		if slot_item is None:
			slot_item = self.getContItem(**kwargs)

		if slot_item is None:
			return None
		else:
			slot_item: SlotContItem = slot_item
			self.ensureDequipCondition(slot_item, type, index)

			self.clearCache()
			equip_item = slot_item.dequip(type, index)
			slot_item.save()

			if equip_item is not None:
				equip_item.save()

			return equip_item

	# 接受转移
	def acceptTransfer(self, slot_item=None, cont_item=None, **kwargs):
		self.equipSlot(slot_item, cont_item, **kwargs)

	# 准备转移
	def prepareTransfer(self, slot_item=None, type=None, index=None, **kwargs) -> dict:
		res = kwargs

		res['cont_item'] = self.dequipSlot(slot_item, type, index, **kwargs)

		return res


# ===================================================
#  背包类容器
# ===================================================
class PackContainer(BaseContainer):

	class Meta:
		verbose_name = verbose_name_plural = "背包类容器"

	# 容量（为0则不限）
	capacity = models.PositiveSmallIntegerField(default=0, verbose_name="容量")

	# 获取容器容量（0为无限）
	@classmethod
	def defaultCapacity(cls): return 0

	# 创建实例
	@classmethod
	def _create(cls, **kwargs):
		container: cls = super()._create(**kwargs)
		container.capacity = cls.defaultCapacity()
		return container

	# 所接受的容器项类（单个，基类）
	@classmethod
	def acceptedContItemClass(cls): return PackContItem

	# 所接受的物品类
	@classmethod
	def acceptedItemClass(cls):
		return cls.acceptedContItemClass().acceptedItemClass()

	# 物品类型是否接受
	def isItemAcceptable(self, cont_item=None, item=None, **kwargs):
		if item is not None:
			return isinstance(item, self.acceptedItemClass())

		if cont_item is not None:
			return isinstance(cont_item, self.acceptedContItemClass())

		return False

	# 获取物品对应的容器项（创建容器项时候调用）
	def contItemClass(self, item: BaseItem = None, **kwargs):
		# 如果未提供参考的物品，返回一个默认容器项类型
		if item is None: return super().contItemClass(**kwargs)
		# 否则，返回该物品的对应容器项类型
		return item.target().contItemClass()

	# 获取容器容量（0为无限）
	def getCapacity(self): return self.capacity

	# 获取指定物品数量或所有物品总数（总数）
	def itemCnt(self, **kwargs):
		cnt = self.contItems(**kwargs).aggregate(sum=Sum('count'))['sum']
		return cnt or 0

	# 判断容器容量是否可用（格子数）
	def isCapacityAvailable(self, count=1):
		if count <= 0 or self.getCapacity() <= 0: return True
		return self.contItemCnt() + count <= self.getCapacity()

	# 背包是否已满
	def isFull(self):
		return not self.isCapacityAvailable(1)

	# region EnsureFuncs

	# 确保容器容量可用（格子数）
	def ensureCapacityAvailable(self, count=1):
		if not self.isCapacityAvailable(count):
			raise ErrorException(ErrorType.CapacityInsufficient)

		return True

	# 确保持有特定数量的物品（格子数）
	def ensureHasItem(self, count, **kwargs):
		if not self.hasItem(count, **kwargs):
			raise ErrorException(ErrorType.QuantityInsufficient)

		return True

	# 是否可以获取指定数量的指定物品
	def ensureGainItemsEnable(self, item: BaseItem, count=1):
		if count == 0: return True
		if count < 0: return self.ensureLostItemsEnable(item, -count)

		self.ensureItemAcceptable(item=item)

		# 最大叠加数量
		max_cnt = item.maxCount()  # self.contItemClass(item).maxCount()

		# 仅可以叠加一个，直接判断是否还有 count 个空位
		if max_cnt == 1:
			self.ensureCapacityAvailable(count)

			return True

		# 可以无限叠加，判断是否存在对应的 ContItem，如果没有，判断是否还有1个空位
		if max_cnt <= 0:
			if self.contItemCnt(item_id=item.id) > 1: return True

			self.ensureCapacityAvailable(1)

			return True

		# 如果可以叠加一定数目，需要继续计算
		delta_cnt = self.__calcDeltaCnt(item, max_cnt)

		# 如果 delta_cnt 足够存放 count 个物品
		if delta_cnt >= count: return True

		# 如果不够，继续计算需要的 BaseContItem 数目
		sum_cont_item_cnt = math.ceil((count-delta_cnt)/max_cnt)

		# 确保还剩下计算的数目的空位
		self.ensureCapacityAvailable(sum_cont_item_cnt)

		return True

	# 中间计算步骤
	def __calcDeltaCnt(self, item, max_cnt):
		from utils.test_utils import Common as TestUtils

		# 测试计算性能
		TestUtils.start('calc delta_cnt')

		# 计算在不增加 BaseContItem 的情况下还可以获取物品的数量 delta_cnt
		# cont_item_cnt = self.contItemCnt(item_id=item.id)
		# sum_item_cnt = self.itemCnt(item_id=item.id)
		# delta_cnt = cont_item_cnt*max_cnt-sum_item_cnt

		ori_items = self.contItems(item_id=item.id)
		ori_items = ori_items.annotate(delta=max_cnt-F('count'))
		delta_cnt = ori_items.aggregate(sum=Sum('delta'))['sum']

		if delta_cnt is None: delta_cnt = 0

		TestUtils.end('delta_cnt: %d' % delta_cnt)

		return delta_cnt

	# 确保包含容器项
	def ensureContItemContained(self, cont_item):
		if cont_item.container_id != self.id:
			raise ErrorException(ErrorType.QuantityInsufficient)

	# 是否可以失去指定数量的指定物品
	def ensureLostItemsEnable(self, item: BaseItem, count=1):
		if count == 0: return True
		if count < 0: return self.ensureGainItemsEnable(item, -count)

		self.ensureHasItem(count, item_id=item.id)

		return True

	# 确保可以获得指定容器项
	def ensureGainContItemEnable(self, cont_item):
		self.ensureItemAcceptable(cont_item=cont_item)
		self.ensureCapacityAvailable(1)

		return True

	# 确保可以失去指定容器项
	def ensureLostContItemEnable(self, cont_item):
		self.ensureItemAcceptable(cont_item=cont_item)
		self.ensureContItemContained(cont_item)

	# 确保可以获得指定一组容器项
	def ensureGainContItemsEnable(self, cont_items):
		self.ensureCapacityAvailable(len(cont_items))
		for cont_item in cont_items:
			self.ensureItemAcceptable(cont_item=cont_item)

	# 确保可以失去指定一组容器项
	def ensureLostContItemsEnable(self, cont_items):
		for cont_item in cont_items:
			self.ensureLostContItemEnable(cont_item)

	# endregion

	# region Gain/LoseImpls

	# 创建一个容器项
	def _createContItem(self, cla=None, cont_item=None,
						item=None, count=0, **kwargs):
		cont_item: PackContItem = super()._createContItem(
			cla, cont_item, item=item, **kwargs)

		# 置入 count 个物品
		count = cont_item.enter(count)

		# if save: cont_item.save()

		return cont_item, count

	# 增加物品（返回修改过的容器项数组）
	def _increaseItems(self, item: BaseItem, count=1, ensure=True, **kwargs):
		# 初始化返回的数据
		old_items = []  # 旧的物品项数据
		new_items = []  # 新的物品项数据

		# 获取容器项的类型和最大叠加数量
		cla = self.contItemClass(item)
		max_cnt = item.maxCount()

		# 如果物品是可以叠加的
		if max_cnt > 1 or max_cnt == 0:
			# 获取当前未满叠加数量的容器项
			ori_items = self.contItems(item=item)

			if max_cnt > 1:
				ori_items = ori_items.filter(count__lt=max_cnt)

			# 从已有的容器项中增加
			for cont_item in ori_items:
				# cont_item = cont_item.target()
				count = cont_item.enter(count)
				old_items.append(cont_item)

			# 批量更新
			# cla.objects.bulk_update(old_items, ['count'])

		# 保证还能继续增加物品
		if ensure:
			if max_cnt > 0:
				self.ensureCapacityAvailable(math.ceil(count / max_cnt))
			else:
				self.ensureCapacityAvailable()

		# 创建新的容器项
		while count > 0:

			cont_item, count = self._createContItem(
				cla, None, item, count, **kwargs)
			# cont_item.save()
			new_items.append(cont_item)

		# 批量创建
		# cla.objects.bulk_create(new_items)

		# 返回有修改过的项
		return old_items, new_items

	# 减少物品（返回修改过的容器项数组）
	def _decreaseItems(self, item: BaseItem, count=1, ensure=True):
		# 初始化返回的数据
		old_items = []  # 旧的物品项数据
		new_items = []  # 新的物品项数据

		# 获取该物品当前所有容器项
		# cla = self.contItemClass(item)
		ori_items = self.contItems(item_id=item.id)

		# 从已有的容器项中移除
		for cont_item in ori_items:
			# cont_item = cont_item.target()
			# 分解物品，返回分解出来的物品和还需要分解的数目
			split_item, count = self._splitItem(cont_item, count, False)
			split_item.remove()

			# 记录修改过的容器项
			new_items.append(split_item)
			if split_item != cont_item:
				old_items.append(cont_item)

			if count <= 0: break

		# 如果还有剩余，抛出数量不足异常
		if ensure and count > 0:
			raise ErrorException(ErrorType.QuantityInsufficient)

		# 更新删除的物品
		# cla.objects.bulk_update(old_items, ['count', 'container'])

		# 返回有修改过的项
		return old_items, new_items

	# endregion

	# region Gain/LostAPIs

	# 拆分一个物品（BaseContItem）
	# 返回：拆分后的 BaseContItem, 剩余数量
	def _splitItem(self, cont_item, count, save=True):
		ori_cnt = cont_item.count

		# 要拆分的数量大于等于持有数量，直接返回
		if count >= ori_cnt: return cont_item, count-ori_cnt

		# 移出 count 个物品
		cont_item.leave(count)

		if save: cont_item.save()

		# 新建一个容器项，并返回
		return self._createContItem(None, cont_item,
									cont_item.targetItem(), count)

	# 拆分一个物品（BaseContItem）
	# 返回：拆分后的 BaseContItem, 剩余数量
	def splitItem(self, cont_item, count, save=True):

		self.ensureContItemContained(cont_item)

		return self._splitItem(cont_item, count, save)

	# 组合一个物品（BaseContItem）
	# 返回：组合后的 BaseContItem, 剩余数量
	def mergeItem(self, cont_items, save=True):

		for cont_item in cont_items:
			self.ensureContItemContained(cont_item)

		return self._mergeItem(cont_items, save)

	# 组合一个物品（BaseContItem）
	# 返回：组合后的 BaseContItem, 剩余数量
	def _mergeItem(self, cont_items, save=True):

		l = 0
		cnt = len(cont_items)

		l_cont_item: PackContItem = cont_items[l]
		l += 1

		for i in range(cnt):
			cont_item: PackContItem = cont_items[i]

			if cont_item == l_cont_item: continue

			if l_cont_item.isFull():
				l_cont_item = cont_items[l]
				l += 1

			l_cont_item.merge(cont_item)

		if save:
			for i in range(l): cont_items[i].save()
			for i in range(cnt-l): cont_items[l+i].delete()

		return cont_items[:l]

	# 获得物品
	def gainItems(self, item: BaseItem, count=1, save=True, combine_return=True):
		if count == 0:
			if combine_return: return []
			return [], []

		if count < 0:
			return self.lostItems(item, -count, save, combine_return)

		item = item.target()

		# self.ensureGainItemsEnable(item, count)

		old_items, new_items = self._increaseItems(item, count)

		if save:
			cla = self.contItemClass(item)
			cla.objects.bulk_update(old_items, ['count'])
			for new_item in new_items:
				new_item.save()
				new_item.afterCreated()

		self.clearCache()

		if combine_return: return old_items+new_items

		return old_items, new_items

	# 失去物品
	def lostItems(self, item: BaseItem, count=1, save=True, combine_return=True):
		if count == 0:
			if combine_return: return []
			return [], []

		if count < 0:
			return self.gainItems(item, -count, save, combine_return)

		# self.ensureLostItemsEnable(item, count)

		old_items, new_items = self._decreaseItems(item, count)

		if save:
			cla = self.contItemClass(item)
			cla.objects.bulk_update(old_items, ['count', 'container'])
			for new_item in new_items: new_item.save()

		self.clearCache()

		if combine_return: return old_items+new_items

		return old_items, new_items

	# 获得一个容器项
	def gainContItem(self, cont_item, save=True, **kwargs):
		# if not ensured:
		self.ensureGainContItemEnable(cont_item)

		cont_item.transfer(self, **kwargs)

		if save: cont_item.save()

		self.clearCache()

		return cont_item

	# 失去一个容器项（count 为 None 则失去整个容器项）
	def lostContItem(self, cont_item, count=None, save=True):
		# if not ensured:
		self.ensureLostContItemEnable(cont_item)

		if count is None:
			cont_item.remove()
		else:
			cont_item, _ = self._splitItem(cont_item, count)

		if save: cont_item.save()

		self.clearCache()

		return cont_item

	# 获得多个容器项
	def gainContItems(self, cont_items, save=True, **kwargs):

		new_cont_items = []

		for cont_item in cont_items:
			new_cont_items.append(self.gainContItem(
				cont_item, False, **kwargs))

		if save:
			for cont_item in new_cont_items: cont_item.save()

		return cont_items

	# 失去多个容器项
	def lostContItems(self, cont_items, counts=None, save=True):

		new_cont_items = []

		for i in range(len(cont_items)):
			count = None
			cont_item = cont_items[i]
			if counts is not None and i<len(counts):
				count = counts[i]

			new_cont_items.append(self.lostContItem(
				cont_item, count, False))

		if save:
			for cont_item in new_cont_items: cont_item.save()

		return cont_items

	# endregion

	# region TransferAPIs

	# 转移物品（从 self 转移到 container）
	def transferItems(self, container, item: BaseItem, count=1):
		self.transfer(container, item=item, count=count)

	# 转移容器项（从 self 转移到 container）
	def transferContItem(self, container, cont_item, count=None):
		self.transfer(container, cont_item=cont_item, count=count)

	# 转移多个容器项（从 self 转移到 container）
	def transferContItems(self, container, cont_items, counts=None):
		self.transfer(container, cont_items=cont_items, counts=counts)

	# 准备转移
	def prepareTransfer(self, cont_item=None, cont_items=None,
						item=None, count=None, counts=None, **kwargs) -> dict:
		res = kwargs

		if item is not None:
			_, res['cont_items'] = self.lostItems(item, count, True, False)
		elif cont_item is not None:
			res['cont_item'] = self.lostContItem(cont_item, count, False)
		elif cont_items is not None:
			res['cont_items'] = self.lostContItems(cont_items, counts, False)

		return res

	# 接受转移
	def acceptTransfer(self, cont_items=None, cont_item=None, **kwargs):

		if cont_item is not None:
			self.gainContItem(cont_item)
		if cont_items is not None:
			self.gainContItems(cont_items)

	# endregion

	"""占位"""

# endregion

# region 物品-容器


# ===================================================
#  基本容器项表
# ===================================================
class BaseContItem(models.Model):
	class Meta:
		verbose_name = verbose_name_plural = "基本容器项"

	TYPES = [
		(ContItemType.Unset.value, '未知容器物品'),

		# Pack
		(ContItemType.HumanPackItem.value, '人类背包物品项'),
		(ContItemType.ExerPackItem.value, '艾瑟萌背包物品项'),
		(ContItemType.HumanPackEquip.value, '人类背包装备项'),
		(ContItemType.ExerPackEquip.value, '艾瑟萌背包装备项'),

		(ContItemType.ExerFragPackItem.value, '艾瑟萌碎片背包项'),

		(ContItemType.QuesSugarPackItem.value, '题目糖背包项'),

		# Slot
		(ContItemType.HumanEquipSlotItem.value, '人类装备槽项'),
		(ContItemType.ExerEquipSlotItem.value, '艾瑟萌装备槽项'),

		(ContItemType.ExerSlotItem.value, '艾瑟萌槽项'),

		(ContItemType.ExerSkillSlotItem.value, '艾瑟萌技能槽项'),

		# Others
		(ContItemType.PlayerExerGift.value, '艾瑟萌天赋池项'),
		(ContItemType.PlayerExermon.value, '艾瑟萌仓库项'),
	]

	# 容器项类型
	TYPE = ContItemType.Unset

	# 容器
	container = models.ForeignKey('BaseContainer', on_delete=models.CASCADE,
								  null=True, blank=True, verbose_name="容器")

	# 容器标识类型
	type = models.PositiveSmallIntegerField(default=ContItemType.Unset.value,
											choices=TYPES, verbose_name="容器类型")

	# region 类型配置

	def __init__(self, *args, **kwargs):
		super().__init__(*args, **kwargs)
		if self.type == ContItemType.Unset.value:
			self.type = self.TYPE.value

	def __str__(self):
		type_name = self.getTypeName()

		return '%d %s' % (self.id, type_name)

	def __getattr__(self, item):
		raise AttributeError(item)

	# 获取类型名称
	def getTypeName(self):
		for type in self.TYPES:
			if self.type == type[0]:
				return type[1]

		return self.TYPES[0][1]

	# 获取目标的物品
	# noinspection PyUnresolvedReferences
	def target(self):

		from exermon_module.models import ExerSlotItem, \
			ExerEquipSlotItem, ExerPackItem, ExerPackEquip, \
			PlayerExerGift, PlayerExermon, ExerFragPackItem, \
			ExerSkillSlotItem
		from player_module.models import HumanPackItem, \
			HumanPackEquip, HumanEquipSlotItem

		target = BaseContItem

		type_ = ContItemType(self.type)
		if type_ != ContItemType.Unset:
			target = eval(type_.name)

		if type(self) == target: return self

		return ViewUtils.getObject(target, ErrorType.ContItemNotExist,
								   return_type='object', id=self.id)

	# 获取目标容器
	def targetContainer(self):
		if self.container is None: return None
		return self.container.target()

	# 转化为 dict
	def _convertToDict(self, **kwargs):
		return {
			'id': self.id,
			'type': self.type
		}

	def convertToDict(self, **kwargs):
		return self.target()._convertToDict(**kwargs)

	# endregion

	# 物品类型是否接受
	def isItemAcceptable(self, **kwargs): return True

	# 比较容器项
	def isEqual(self, cont_item):
		return type(self) == type(cont_item)

	# 确保物品类型可接受
	def ensureItemAcceptable(self, **kwargs):
		if not self.isItemAcceptable(**kwargs):
			raise ErrorException(ErrorType.IncorrectItemType)

		return True

	# 确保容器项可复制
	def ensureContItemCopyable(self, cont_item):
		if type(self) != type(cont_item):
			raise ErrorException(ErrorType.IncorrectItemType)

	# 确保容器项相等
	def ensureContItemEqual(self, cont_item):
		if not self.isEqual(cont_item):
			raise ErrorException(ErrorType.IncorrectContItemType)

	# 最大叠加数量
	def maxCount(self): return 1

	# 创建容器项（包含移动）
	def create(self, container: BaseContainer, **kwargs):
		self.ensureItemAcceptable(**kwargs)
		self.transfer(container, **kwargs)

	# 复制容器项
	def copy(self, cont_item):
		self.ensureContItemCopyable(cont_item)

	# 移动容器项（移动到指定的 container）
	def transfer(self, container: BaseContainer, **kwargs):
		self.container = container

	# 移除容器项（从当前容器中移除）
	def remove(self):
		self.container = None

	# 创建之后调用
	def afterCreated(self, **kwargs): pass

	# 刷新
	def refresh(self): pass


# ===================================================
#  槽类容器项表
# ===================================================
class SlotContItem(BaseContItem):
	class Meta:
		verbose_name = verbose_name_plural = "槽类容器项"

	# 槽编号
	index = models.PositiveSmallIntegerField(default=0, verbose_name="槽编号")

	# 装备项1
	equip_item1 = models.OneToOneField('PackContItem', null=True, blank=True, related_name='equip_item1',
									   on_delete=models.CASCADE, verbose_name="装备项1")

	# 装备项2
	equip_item2 = models.OneToOneField('PackContItem', null=True, blank=True, related_name='equip_item2',
									   on_delete=models.CASCADE, verbose_name="装备项2")

	target_equip_item1 = target_equip_item2 = None

	# region 配置项

	# 所接受的装备项类（2个）
	@classmethod
	def acceptedEquipItemClass(cls): return PackContItem, ()

	# 所接受的装备项属性名（2个）
	@classmethod
	def acceptedEquipItemAttr(cls): return 'equip1', 'equip2'

	# endregion

	def __str__(self):
		return '%s (%d: %s, %s)' % (
			super().__str__(), self.index,
			self.targetEquipItem1(), self.targetEquipItem2())

	# 用于获取属性值
	def __getattr__(self, item):
		attr1, attr2 = self.acceptedEquipItemAttr()

		if item == attr1:
			return self.targetEquipItem1()
		if item == attr2:
			return self.targetEquipItem2()

		return super().__getattr__(item)

	# 转化为 dict
	def _convertToDict(self, **kwargs):
		res = super()._convertToDict(**kwargs)

		c1, c2 = self.acceptedEquipItemClass()
		attr1, attr2 = self.acceptedEquipItemAttr()

		equip_item1 = ModelUtils.objectToDict(self.targetEquipItem1())
		equip_item2 = ModelUtils.objectToDict(self.targetEquipItem2())

		res['index'] = self.index

		if c1: res[attr1] = equip_item1
		if c2: res[attr2] = equip_item2

		return res

	# 目标装备项1
	def targetEquipItem1(self):
		if self.target_equip_item1 is None:
			if self.equip_item1 is None:
				self.target_equip_item1 = None
			else:
				self.target_equip_item1 = self.equip_item1.target()

		return self.target_equip_item1

	# 目标装备项2
	def targetEquipItem2(self):
		if self.target_equip_item2 is None:
			if self.equip_item2 is None:
				self.target_equip_item2 = None
			else:
				self.target_equip_item2 = self.equip_item2.target()

		return self.target_equip_item2

	# 比较容器项
	def isEqual(self, cont_item): return False

	# 获取装备项索引
	def getEquipItemIndex(self, equip_item=None, type=None):
		c1, c2 = self.acceptedEquipItemClass()

		if equip_item is not None:
			if isinstance(equip_item, c1): return 1
			if isinstance(equip_item, c2): return 2

			# raise ErrorException(ErrorType.IncorrectItemType)
		if type is not None:
			if c1 == type: return 1
			if c2 == type: return 2

		return None

	# 最大叠加数量
	def maxCount(self): return 1

	# 复制容器项（不可复制）
	def copy(self, cont_item): pass

	# 配置索引
	def setupIndex(self, index, **kwargs):
		self.index = index

	# 装备
	def equip(self, index=None, equip_item=None, **kwargs):
		if index is None:
			index = self.getEquipItemIndex(equip_item=equip_item)

		if index is None: return

		if equip_item is not None:
			equip_item.transfer(self.container)
			self.refresh()

		if index == 1: self.equip_item1 = equip_item
		if index == 2: self.equip_item2 = equip_item

		return equip_item

	# 卸下
	def dequip(self, type=None, index=None, **kwargs):

		if type is not None:
			return self.dequip(index=self.getEquipItemIndex(type=type))

		equip_item = None

		if index is not None:
			if index == 1:
				equip_item = self.targetEquipItem1()
				self.equip_item1 = None
			if index == 2:
				equip_item = self.targetEquipItem2()
				self.equip_item2 = None

		if equip_item is not None:
			equip_item.remove()
			self.refresh()

		return equip_item


# ===================================================
#  背包类容器项表
# ===================================================
class PackContItem(BaseContItem):
	class Meta:
		verbose_name = verbose_name_plural = "背包类容器项"

	# 物品
	item = models.ForeignKey('BaseItem', on_delete=models.CASCADE, verbose_name="物品")

	# 叠加数量
	count = models.PositiveSmallIntegerField(default=0, verbose_name="叠加数量")

	# region 配置项

	# 所接受的物品类
	@classmethod
	def acceptedItemClass(cls): return BaseItem

	# endregion

	def __str__(self):
		return '%s (%s :%d)' % (
			super().__str__(), self.targetItem(), self.count)

	# 转化为 dict
	def _convertToDict(self, **kwargs):
		res = super()._convertToDict(**kwargs)

		res['item_id'] = self.item_id
		res['count'] = self.count

		return res

	# 最大叠加数量
	def maxCount(self):
		if self.item is not None:
			return self.item.maxCount()

		return 1

	# 目标物品
	def targetItem(self):
		if self.item is None: return None

		return self.item.target()

	# 物品类型是否接受
	def isItemAcceptable(self, item=None, **kwargs):
		if item is not None:
			return isinstance(item, self.acceptedItemClass())

		return False

	# 比较容器项
	def isEqual(self, cont_item):
		return super().isEqual(cont_item) and self.item_id == cont_item.item_id

	# 是否已满
	def isFull(self):
		return self.count >= self.maxCount()

	# 创建容器项
	def create(self, container: BaseContainer, item=None, **kwargs):
		super().create(container, item=item, **kwargs)
		self.item = item

	# 复制容器项
	def copy(self, cont_item):
		super().copy(cont_item)
		self.item = cont_item.item

	# 组合
	def merge(self, cont_item):
		self.ensureContItemEqual(cont_item)

		count = self.enter(cont_item.count)
		cont_item.count = count

		# if cont_item.count <= 0:
		#	cont_item.remove()

	# 加入容器（返回剩余无法添加的数量）
	def enter(self, count=0):
		if count <= 0: return 0
		if self.item is None or self.container is None:
			return count  # 未设置物品和容器

		self.count += count

		max_count = self.maxCount()
		# 如果有最大叠加限制
		if max_count > 0:
			delta = self.count - max_count
			self.count = min(max_count, self.count)
			return max(0, delta)  # 返回剩余无法添加的数量

		return 0

	# 移出容器（返回剩余无法删除的数量）
	def leave(self, count=0):
		if count <= 0: return 0
		if self.item is None or self.container is None:
			return count  # 未设置物品和容器

		self.count -= count

		# 如果数量为 0
		if self.count <= 0:
			self.remove()
			delta = -self.count
			self.count = max(0, self.count)
			return max(0, delta)  # 返回剩余无法删除的数量

		return 0


# endregion

# endregion

# region 特性&效果


# ===================================================
#  特性编号枚举
# ===================================================
class TraitCode(Enum):
	Unset = 0  # 未设置


# ===================================================
#  特性表
# ===================================================
class Trait(models.Model):
	class Meta:
		verbose_name = verbose_name_plural = "特性"

	CODES = [
		(TraitCode.Unset.value, '无特性'),
	]

	# 所属物品
	item = models.ForeignKey('BaseItem', on_delete=models.CASCADE, verbose_name="物品")

	# 特性编号
	code = models.PositiveSmallIntegerField(default=0, choices=CODES, verbose_name="特性编号")

	# 特性参数
	params = jsonfield.JSONField(default=[], verbose_name="特性参数")

	# 转化为字典
	def convertToDict(self):

		return {
			'code': self.code,
			'params': self.params
		}


# ===================================================
#  使用效果编号枚举
# ===================================================
class ItemEffectCode(Enum):
	Unset = 0  # 空

	RecoverHP = 10  # 回复体力值
	RecoverMP = 11  # 回复精力值
	AddParam = 20  # 增加能力值
	TempAddParam = 21  # 临时增加能力值
	BattleAddParam = 22  # 战斗中增加能力值

	GainItem = 30  # 获得物品
	GainGold = 31  # 获得金币
	GainBoundTicket = 32  # 获得绑定点券
	GainExermonExp = 40  # 指定艾瑟萌获得经验
	GainExerSlotItemExp = 41  # 指定艾瑟萌槽项获得经验
	GainPlayerExp = 42  # 玩家获得经验

	Eval = 99  # 执行程序


# ===================================================
#  使用效果表
# ===================================================
class Effect(models.Model):

	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "使用效果"

	CODES = [
		(ItemEffectCode.Unset.value, '空'),

		(ItemEffectCode.RecoverHP.value, '回复体力值'),
		(ItemEffectCode.RecoverMP.value, '回复精力值'),
		(ItemEffectCode.AddParam.value, '增加能力值'),
		(ItemEffectCode.TempAddParam.value, '临时增加能力值'),
		(ItemEffectCode.BattleAddParam.value, '战斗中增加能力值'),

		(ItemEffectCode.GainItem.value, '获得物品'),
		(ItemEffectCode.GainGold.value, '获得金币'),
		(ItemEffectCode.GainBoundTicket.value, '获得绑定点券'),
		(ItemEffectCode.GainExermonExp.value, '指定艾瑟萌获得经验'),
		(ItemEffectCode.GainExerSlotItemExp.value, '指定艾瑟萌槽项获得经验'),
		(ItemEffectCode.GainPlayerExp.value, '玩家获得经验'),

		(ItemEffectCode.Eval.value, '执行程序'),
	]

	# 效果编号
	code = models.PositiveSmallIntegerField(default=0, choices=CODES, verbose_name="效果编号")

	# 效果参数
	params = jsonfield.JSONField(default=[], verbose_name="效果参数")

	def __str__(self):
		return self.describe()

	def describe(self):
		from game_module.models import BaseParam, GameConfigure, Subject

		if not isinstance(self.params, list):
			raise ErrorException(ErrorType.DatabaseError)

		conf: GameConfigure = GameConfigure.get()
		code = ItemEffectCode(self.code)
		params = tuple(self.params)
		p_len = len(params)

		if code == ItemEffectCode.Unset: return '无效果'

		if code == ItemEffectCode.RecoverHP or \
			code == ItemEffectCode.RecoverMP:
			name = "体力值" if code == ItemEffectCode.RecoverHP else "精力值"

			a = params[0]
			if p_len < 2:  # 如果只有 a 参数
				return "%s 回复 %s 点" % (name, a)

			b = params[1]
			if a == 0: return "%s 回复 %s %%" % (name, b)
			return "%s 回复 %s 点，%s%%" % (name, a, b)

		if code == ItemEffectCode.AddParam:
			p, a = params[0], params[1]
			name = BaseParam.get(id=p).name

			if p_len < 3:  # 如果只有 a 参数
				return "属性 %s 增加 %s 点" % (name, a)

			b = params[2]
			if a == 0: return "属性 %s 增加 %s %%" % (name, b)
			return "属性 %s 增加 %s 点，%s%%" % (name, a, b)

		if code == ItemEffectCode.TempAddParam or \
			code == ItemEffectCode.BattleAddParam:
			p, t, a = params[0], params[1], params[2]
			name = BaseParam.get(id=p).name
			time = "秒" if code == ItemEffectCode.TempAddParam else "回合"

			if p_len < 4:  # 如果只有 a 参数
				return "属性 %s 增加 %s 点，持续 %s %s" % (name, a, t, time)

			b = params[3]
			if a == 0: return "属性 %s 增加%s%%，持续 %s %s" % (name, b, t, time)
			return "属性 %s 增加 %s 点，%s%%，持续 %s %s" % (name, a, b, t, time)

		if code == ItemEffectCode.GainItem:
			from .views import Common

			i, m = params[0], params[1]
			name = Common.getItem(id=i).name

			if p_len < 3:  # 如果只有 m 参数
				return "物品 %s 增加 %s 个" % (name, m)

			n = params[2]
			return "物品 %s 增加 %s~%s 个" % (name, m, n)

		if code == ItemEffectCode.GainGold or \
			code == ItemEffectCode.GainBoundTicket:

			m = params[0]
			name = conf.gold if code == ItemEffectCode.GainGold else conf.bound_ticket

			if p_len < 2:  # 如果只有 m 参数
				return "%s 增加 %s" % (name, m)

			n = params[1]
			return "%s 增加 %s~%s" % (name, m, n)

		if code == ItemEffectCode.GainExermonExp or \
			code == ItemEffectCode.GainExerSlotItemExp:

			s, m = params[0], params[1]
			name = Subject.get(id=s).name
			exer = "艾瑟萌" if code == ItemEffectCode.GainExermonExp else "艾瑟萌槽"

			if p_len < 3:  # 如果只有 m 参数
				return "%s %s经验值增加 %s" % (name, exer, m)

			n = params[2]
			return "%s %s经验值增加 %s~%s" % (name, exer, m, n)

		if code == ItemEffectCode.GainPlayerExp:

			m = params[0]

			if p_len < 2:  # 如果只有 m 参数
				return "玩家经验值增加 %s" % (m)

			n = params[1]
			return "玩家经验值增加 %s~%s" % (m, n)

		if code == ItemEffectCode.Eval:
			return "执行程序：%s" % params[0]

	# 转化为字典
	def convertToDict(self):

		return {
			'code': self.code,
			'params': self.params,
			'description': self.describe()
		}


# ===================================================
#  物品使用效果表
# ===================================================
class ItemEffect(Effect):

	class Meta:
		verbose_name = verbose_name_plural = "物品使用效果"

	# 物品
	item = models.ForeignKey('UsableItem', on_delete=models.CASCADE, verbose_name="物品")


# endregion
