from django.db import models
from django.db.models import Sum, F
from django.conf import settings
from game_module.models import ParamValue, GameTerm, ExerEquipType, HumanEquipType
from utils.model_utils import ItemIconUpload, Common as ModelUtils
from utils.view_utils import Common as ViewUtils
from utils.exception import ErrorType, ErrorException
from enum import Enum
import jsonfield, os, math

# region 物品


# ===================================================
#  货币表
# ===================================================
class Currency(models.Model):
	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "货币"

	# 金币
	gold = models.PositiveIntegerField(default=0, verbose_name="金币")

	# 点券
	ticket = models.PositiveIntegerField(default=0, verbose_name="点券")

	# 绑定点券
	bound_ticket = models.PositiveIntegerField(default=0, verbose_name="金币")

	def __str__(self):
		term: GameTerm = GameTerm.get()
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

	# 名称
	name = models.CharField(max_length=24, verbose_name="名称")

	# 描述
	description = models.CharField(max_length=128, verbose_name="描述")

	# 物品标识类型
	type = models.PositiveSmallIntegerField(default=ItemType.Unset.value,
											choices=TYPES, verbose_name="物品类型")

	# 物品类型
	@classmethod
	def itemType(cls): return ItemType.Unset

	# 对应的容器物品类型（枚举）
	@classmethod
	def contItemType(cls): return ContItemType.Unset

	# 对应的容器物品类型
	@classmethod
	def contItemClass(cls):
		type = cls.contItemType()
		if type == ContItemType.Unset: return BaseContItem
		return eval(type.name)

	def __init__(self, *args, **kwargs):
		super().__init__(*args, **kwargs)
		if self.type == ItemType.Unset.value:
			self.type = self.itemType().value

	def __str__(self):
		type_name = self.getTypeName()

		return '%d %s（%s）' % (self.id, self.name, type_name)

	# 获取类型名称
	def getTypeName(self):
		for type in self.TYPES:
			if self.type == type[0]:
				return type[1]

		return self.TYPES[0][1]

	# 获取目标的物品
	def targetItem(self):

		target = BaseItem

		type_ = ItemType(self.type)
		if type_ != ItemType.Unset:
			target = eval(type_.name)

		if type(self) == target: return self

		return ViewUtils.getObject(target, ErrorType.ItemNotExist,
								   return_type='object', id=self.id)

	# 转化为 dict
	def _convertToDict(self, **kwargs):
		return {
			'id': self.id,
			'name': self.name,
			'description': self.description,
			'type': self.type
		}

	# 最大叠加数量（为0则不限）
	def _maxCount(self): return 1

	# 转化为 dict
	def convertToDict(self, **kwargs):
		return self.targetItem()._convertToDict(**kwargs)

	# 最大叠加数量（为0则不限）
	def maxCount(self):
		return self.targetItem()._maxCount()


# ===================================================
#  物品价格表
# ===================================================
class ItemPrice(Currency):
	class Meta:
		verbose_name = verbose_name_plural = "物品价格"

	def __str__(self):
		return '%s：%s' % (self.limiteditem, super().__str__())


# ===================================================
#  有限物品表（一般为背包的物品）
# ===================================================
class LimitedItem(BaseItem):

	class Meta:
		# abstract = True
		verbose_name = verbose_name_plural = "有限物品"

	# 购入价格（为0则不可购买）
	buy_price = models.OneToOneField("ItemPrice", on_delete=models.CASCADE, default=0, verbose_name="购入价格")

	# 出售价格（出售固定为金币，为0则不可出售）
	sell_price = models.PositiveIntegerField(default=0, verbose_name="出售价格")

	# 是否可丢弃
	discardable = models.BooleanField(default=True, verbose_name="可丢弃")

	# 物品图标
	icon = models.ImageField(upload_to=ItemIconUpload(), verbose_name="图标")

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

		res['buy_price'] = self.buy_price.convertToDict()
		res['sell_price'] = self.sell_price
		res['discardable'] = self.discardable
		res['icon'] = self.convertIconToBase64()

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

	# 能否交易
	tradable = models.BooleanField(default=True, verbose_name="能否交易")

	# 物品类型
	i_type = models.ForeignKey("game_module.UsableItemType",
							   on_delete=models.CASCADE, verbose_name="物品类型")

	# 转化为 dict
	def _convertToDict(self, **kwargs):
		res = super()._convertToDict(**kwargs)

		res['max_count'] = self.max_count
		res['consumable'] = self.consumable
		res['battle_use'] = self.battle_use
		res['menu_use'] = self.menu_use
		res['adventure_use'] = self.adventure_use
		res['freeze'] = self.freeze
		res['i_type_id'] = self.i_type_id

		return res

	# 最大叠加数量（为0则不限）
	def _maxCount(self): return self.max_count

	# 获取所有的效果
	def effects(self):
		return self.itemeffect_set.all()


# ===================================================
#  人类物品表
# ===================================================
class HumanItem(UsableItem):

	class Meta:
		verbose_name = verbose_name_plural = "人类物品"

	# 道具类型
	@classmethod
	def itemType(cls): return ItemType.HumanItem

	# 对应的容器物品类型（枚举）
	@classmethod
	def contItemType(cls): return ContItemType.HumanPackItem

	# 转化为 dict
	def _convertToDict(self, **kwargs):
		res = super()._convertToDict(**kwargs)

		return res


# ===================================================
#  艾瑟萌物品表
# ===================================================
class ExerItem(UsableItem):

	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌物品"

	# 使用几率（*100）
	rate = models.PositiveSmallIntegerField(default=0, verbose_name="使用几率")

	# 道具类型
	@classmethod
	def itemType(cls): return ItemType.ExerItem

	# 对应的容器物品类型（枚举）
	@classmethod
	def contItemType(cls): return ContItemType.ExerPackItem

	# 转化为 dict
	def _convertToDict(self, **kwargs):
		res = super()._convertToDict(**kwargs)

		res['rate'] = self.rate

		return res


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


# ===================================================
#  艾瑟萌装备
# ===================================================
class ExerEquip(EquipableItem):
	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌装备"

	# 装备类型
	e_type = models.ForeignKey("game_module.ExerEquipType",
							   on_delete=models.CASCADE, verbose_name="装备类型")

	# 道具类型
	@classmethod
	def itemType(cls): return ItemType.ExerEquip

	# 对应的容器物品类型（枚举）
	@classmethod
	def contItemType(cls): return ContItemType.ExerPackEquip

	# 转化为 dict
	def _convertToDict(self, **kwargs):
		res = super()._convertToDict(**kwargs)

		res['e_type'] = self.e_type

		return res

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

	# 玩家
	# player = models.ForeignKey('player_module.Player', on_delete=models.CASCADE,
	#						   verbose_name="玩家")

	# 容器标识类型
	type = models.PositiveSmallIntegerField(default=ContainerType.Unset.value,
											choices=TYPES, verbose_name="容器类型")

	# 缓存上一次调用获得的cont_items
	# {'args': **kwargs, 'res': 容器项[]}
	cache_cont_items = None

	accepted_classes = None

	# 容器类型
	@classmethod
	def containerType(cls): return ContainerType.Unset

	# 所接受容器项的基类
	@classmethod
	def baseContItemClass(cls): return BaseContItem

	# 所接受物品的基类
	@classmethod
	def baseItemClass(cls): return BaseItem

	# 容器接受物品类型（数组）
	@classmethod
	def acceptTypes(cls): return []

	# 容器接受物品类型（数组）
	@classmethod
	def acceptClasses(cls):
		if cls.accepted_classes is None:
			a_types = cls.acceptTypes()
			a_classes = []
			for type_ in a_types:
				a_classes.append(type_.name)
			cls.accepted_classes = a_classes

		return cls.accepted_classes

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

	def __init__(self, *args, **kwargs):
		super().__init__(*args, **kwargs)
		if self.type == ContainerType.Unset.value:
			self.type = self.containerType().value

	def __str__(self):
		type_name = self.getTypeName()

		return '%d %s' % (self.id, type_name)

	# 获取类型名称
	def getTypeName(self):
		for type in self.TYPES:
			if self.type == type[0]:
				return type[1]

		return self.TYPES[0][1]

	# 获取目标的物品
	def targetContainer(self):

		target = BaseItem

		type_ = ContainerType(self.type)
		if type_ != ContainerType.Unset:
			target = eval(type_.name)

		if type(self) == target: return self

		return ViewUtils.getObject(target, ErrorType.ContainerNotExist,
								   return_type='object', id=self.id)

	# 转化为 dict
	def _convertToDict(self, **kwargs):
		return {
			'id': self.id,
			'type': self.type
		}

	def convertToDict(self, **kwargs):
		return self.targetContainer()._convertToDict(**kwargs)

	# 获取物品对应的容器项
	def contItemClass(self, **kwargs):
		return self.baseContItemClass()

	# 获取容器容量（0为无限）
	def getCapacity(self): return 0

	# 物品类型是否接受
	def isItemAcceptable(self, item=None, cont_item=None, **kwargs):
		if isinstance(item, self.baseItemClass()):
			item = item.targetItem()
			return type(item).__name__ in self.acceptClasses()

		if isinstance(cont_item, self.baseContItemClass()):
			cont_item = cont_item.targetContItem()
			return type(cont_item).__name__ in self.acceptClasses()

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
		return ViewUtils.getObjects(cla, container_id=self.id, **kwargs)

	# 清空缓存
	def clearCache(self):
		self.cache_cont_items = None

	# 获取指定物品数量或所有物品总数（占用格子数）
	def contItemCnt(self, **kwargs):
		return self.contItems(**kwargs).count()

	# 获取指定物品数量或所有物品总数（总数）
	def itemCnt(self, **kwargs):
		return self.contItemCnt(**kwargs)

	# 背包是否存在指定物品或任意物品指定数量
	def hasItem(self, count, **kwargs):
		return self.itemCnt(**kwargs) >= count

	# 创建一个容器项
	def _createContItem(self, cla=None, cont_item=None, save=False, **kwargs):
		if cla is None:
			if cont_item is not None: cla = type(cont_item)
			else: cla = self.contItemClass(**kwargs)

		new_cont_item = cla()
		new_cont_item.create(self, **kwargs)

		if cont_item is not None:
			new_cont_item.copy(cont_item)

		if save: new_cont_item.save()

		return new_cont_item

	# 确保物品类型可接受
	def ensureItemAcceptable(self, cont_item=None, item=None, **kwargs):
		if not self.isItemAcceptable(cont_item=cont_item, item=item, **kwargs):
			raise ErrorException(ErrorType.ItemIncorrect)

		return True

	# 准备转移
	def prepareTransfer(self, **kwargs)->dict:
		return kwargs

	# 接受转移
	def acceptTransfer(self, **kwargs):
		self.ensureItemAcceptable(**kwargs)

	# 转移物品（从 self 转移到 container）
	def transfer(self, container, **kwargs):
		container = container.targetContainer()
		container.acceptTransfer(**self.prepareTransfer(**kwargs))


# ===================================================
#  槽类容器
# ===================================================
class SlotContainer(BaseContainer):

	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "槽类容器"

	# 创建实例
	@classmethod
	def create(cls, **kwargs):
		container = cls._create(**kwargs)
		container.save()
		container.createSlots(**kwargs)
		return container

	# 所接受容器项的基类
	@classmethod
	def baseContItemClass(cls): return SlotContItem

	# 获取容器容量（0为无限）
	def getCapacity(self): return 1

	# 物品类型是否接受
	def isItemAcceptable(self, item=None, cont_item=None, **kwargs):
		if isinstance(cont_item, self.baseContItemClass()):
			cont_item = cont_item.targetContItem()
			return type(cont_item).__name__ in self.acceptClasses()

		return False

	# 获取物品对应的容器项
	def contItemClass(self, **kwargs):
		return SlotContItem

	# 创建一组槽
	def createSlots(self, **kwargs):
		# slot_items = []
		cla = self.contItemClass(**kwargs)
		cnt = self.getCapacity()

		for i in range(cnt):
			slot = self._createSlot(cla, i+1, **kwargs)
			slot.save()

		# cla.objects.bulk_create(slot_items)

	# 创建一个槽
	def _createSlot(self, cla, index, **kwargs):
		slot_item: SlotContItem = self._createContItem(
			cla, None, False, **kwargs)
		slot_item.setupIndex(index)
		return slot_item

	# 获取一个槽项
	def getSlotItem(self, **kwargs):
		slot_items = self.contItems(**kwargs)
		if slot_items.exists():
			return slot_items.first().targetContItem()

		return None


# ===================================================
#  背包类容器
# ===================================================
class PackContainer(BaseContainer):

	class Meta:
		abstract = True
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

	# 容器项基类
	@classmethod
	def baseContItemClass(cls): return PackContItem

	# 物品类型是否接受
	def isItemAcceptable(self, item=None, cont_item=None, **kwargs):
		if isinstance(cont_item, self.baseContItemClass()):
			return self.isItemAcceptable(item=cont_item.targetItem())

		return super().isItemAcceptable(item=item)

	# 获取物品对应的容器项
	def contItemClass(self, item: BaseItem=None, **kwargs):
		if item is None: return super().contItemClass(**kwargs)
		return item.contItemClass()

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
			raise ErrorException(ErrorType.ContainerFull)

		return True

	# 确保持有特定数量的物品（格子数）
	def ensureHasItem(self, count, **kwargs):
		if not self.hasItem(count, **kwargs):
			raise ErrorException(ErrorType.ItemNotEnough)

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
		if cont_item.targetContainer() != self:
			raise ErrorException(ErrorType.ItemNotEnough)
		return True

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
	def _createContItem(self, cla=None, cont_item=None, save=False,
						item=None, count=0, **kwargs):
		cont_item: PackContItem = super()._createContItem(
			cla, cont_item, False, item=item, **kwargs)
		count = cont_item.enter(count)

		if save: cont_item.save()

		return cont_item, count

	# 增加物品
	def _increaseItems(self, item: BaseItem, count=1, **kwargs):
		# 初始化返回的数据
		old_items = []
		new_items = []

		# 获取容器项的类型和最大叠加数量
		cla = self.contItemClass(item)
		max_cnt = item.maxCount()

		# 如果物品是可以叠加的
		if max_cnt > 1 or max_cnt == 0:
			# 获取当前未满叠加数量的容器项
			ori_items = self.contItems(item_id=item.id).filter(count__lt=max_cnt)

			# 从已有的容器项中增加
			for cont_item in ori_items:
				cont_item = cont_item.targetContItem()
				count = cont_item.enter(count)
				old_items.append(cont_item)

			# 批量更新
			cla.objects.bulk_update(old_items, ['count'])

		# 创建新的容器项
		while count > 0:
			cont_item, count = self._createContItem(
				cla, None, False, item, count, **kwargs)
			cont_item.save()
			new_items.append(cont_item)

		# 批量创建
		# cla.objects.bulk_create(new_items)

		# 返回有修改过的项
		return old_items+new_items

	# 减少物品
	def _decreaseItems(self, item: BaseItem, count=1):
		old_items = []
		cla = self.contItemClass(item)

		# 获取该物品当前所有容器项
		ori_items = self.contItems(item_id=item.id)

		# 从已有的容器项中移除
		for cont_item in ori_items:
			cont_item = cont_item.targetContItem()
			# 分解物品，返回分解出来的物品和还需要分解的数目
			split_item, count = self.splitItem(cont_item, count)
			split_item.remove()
			old_items.append(split_item)
			if count <= 0: break

		# 更新删除的物品
		cla.objects.bulk_update(old_items, ['count', 'container'])

		return old_items

	# endregion

	# region Gain/LostAPIs

	# 拆分一个物品（BaseContItem）
	# 返回：拆分后的 BaseContItem, 剩余数量
	def splitItem(self, cont_item, count):
		ori_cnt = cont_item.count

		# 要拆分的数量大于等于持有数量，直接返回
		if count >= ori_cnt: return cont_item, count-ori_cnt

		# 移出 count 个物品
		cont_item.leave(count)
		cont_item.save()

		# 新建一个容器项，并返回
		return self._createContItem(None, cont_item, True, None, count)

	# 获得物品
	def gainItems(self, item: BaseItem, count=1):
		if count == 0: return []
		if count < 0: return self.lostItems(item, -count)

		self.ensureGainItemsEnable(item, count)

		return self._increaseItems(item, count)

	# 失去物品
	def lostItems(self, item: BaseItem, count=1):
		if count == 0: return []
		if count < 0: return self.gainItems(item, -count)

		self.ensureLostItemsEnable(item, count)

		return self._decreaseItems(item, count)

	# 获得一个容器项
	def gainContItem(self, cont_item, ensured=False, **kwargs):
		if not ensured:
			self.ensureGainContItemEnable(cont_item)

		cont_item.transfer(self, **kwargs)
		cont_item.save()

		return cont_item

	# 失去一个容器项
	def lostContItem(self, cont_item, ensured=False):
		if not ensured:
			self.ensureLostContItemEnable(cont_item)

		cont_item.remove()
		cont_item.save()

		return cont_item

	# 获得多个容器项
	def gainContItems(self, cont_items):
		self.ensureGainContItemsEnable(cont_items)

		for cont_item in cont_items:
			self.gainContItem(cont_item, True)

		return cont_items

	# 失去多个容器项
	def lostContItems(self, cont_items):
		self.ensureLostContItemsEnable(cont_items)

		for cont_item in cont_items:
			self.lostContItem(cont_item, True)

		return cont_items

	# endregion

	# 转移物品（从 self 转移到 container）
	def transferItems(self, container, item: BaseItem, count=1):
		self.transfer(container, item=item, count=count)

	# 转移容器项（从 self 转移到 container）
	def transferContItem(self, container, cont_item):
		self.transfer(container, cont_item=cont_item)

	# 转移多个容器项（从 self 转移到 container）
	def transferContItems(self, container, cont_items):
		self.transfer(container, cont_items=cont_items)

	# 准备转移
	def prepareTransfer(self, cont_item=None, cont_items=None,
						item=None, count=1, **kwargs) -> dict:
		res = kwargs

		if item is not None:
			res['cont_items'] = self.lostItems(item, count)
		elif cont_item is not None:
			res['cont_item'] = self.lostContItem(cont_item)
		elif cont_items is not None:
			res['cont_items'] = self.lostContItems(cont_items)

		return res

	# 接受转移
	def acceptTransfer(self, cont_items=None, cont_item=None, **kwargs):
		if cont_item is not None:
			self.gainContItem(cont_item)
		if cont_items is not None:
			self.gainContItems(cont_items)


# ===================================================
#  人类背包
# ===================================================
class HumanPack(PackContainer):

	class Meta:
		verbose_name = verbose_name_plural = "人类背包"

	# 默认容量
	DEFAULT_CAPACITY = 64

	# 玩家
	player = models.OneToOneField('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 容器类型
	@classmethod
	def containerType(cls): return ContainerType.HumanPack

	# 容器接受物品类型（数组）
	@classmethod
	def acceptTypes(cls):
		return [ItemType.HumanItem, ItemType.HumanEquip]

	# 获取容器容量（0为无限）
	@classmethod
	def defaultCapacity(cls): return cls.DEFAULT_CAPACITY

	# 创建一个背包（创建角色时候执行）
	@classmethod
	def _create(cls, player):
		pack: cls = super()._create()
		pack.player = player
		return pack


# ===================================================
#  艾瑟萌背包
# ===================================================
class ExerPack(PackContainer):

	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌背包"

	# 默认容量
	DEFAULT_CAPACITY = 32

	# 玩家
	player = models.OneToOneField('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 容器类型
	@classmethod
	def containerType(cls): return ContainerType.ExermonPack

	# 容器接受物品类型（数组）
	@classmethod
	def acceptTypes(cls):
		return [ItemType.Exermon, ItemType.ExerEquip]

	# 获取容器容量（0为无限）
	@classmethod
	def defaultCapacity(cls): return cls.DEFAULT_CAPACITY

	# 创建一个背包（创建角色时候执行）
	@classmethod
	def _create(cls, player):
		pack: cls = super()._create()
		pack.player = player
		return pack


# ===================================================
#  人类装备槽
# ===================================================
class HumanEquipSlot(SlotContainer):

	class Meta:
		verbose_name = verbose_name_plural = "人类装备槽"

	# 玩家
	player = models.OneToOneField('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 人类背包
	human_pack = models.ForeignKey('HumanPack', on_delete=models.CASCADE, verbose_name="人类背包")

	# 容器类型
	@classmethod
	def containerType(cls): return ContainerType.HumanEquipSlot

	# 容器接受物品类型（数组）
	@classmethod
	def acceptTypes(cls):
		return [ContItemType.HumanPackEquip]

	# 容器接受物品基类
	@classmethod
	def baseContItemClass(cls): return HumanPackEquip

	# 获取物品对应的容器项
	def contItemClass(self, **kwargs): return HumanEquipSlotItem

	# 获取容器容量（0为无限）
	def getCapacity(self): return HumanEquipType.count()

	# 创建一个槽（创建角色时候执行）
	@classmethod
	def _create(cls, player):
		slot: cls = super()._create()
		slot.player = player
		slot.human_pack = player.humanpack
		return slot

	# 保证满足装备条件
	def ensureEquipCondition(self, pack_equip):
		self.ensureItemAcceptable(cont_item=pack_equip)

		return True

	# 保证满足装备卸下条件
	def ensureDequipCondition(self, slot_item):
		return True

	# 获取一个槽项
	def getSlotItem(self, e_type_id=None, e_type=None, **kwargs):
		if e_type_id is not None:
			return super().getSlotItem(e_type_id=e_type_id, **kwargs)
		elif e_type is not None:
			return super().getSlotItem(e_type=e_type, **kwargs)

		return None

	# 更换装备
	def setEquip(self, e_type=None, e_type_id=None, slot_item=None, pack_equip=None):
		self.transfer(self.human_pack, e_type=e_type,
					  e_type_id=e_type_id, slot_item=slot_item)

		if pack_equip is not None:
			self.human_pack.transfer(self, e_type=e_type, e_type_id=e_type_id,
									slot_item=slot_item, cont_item=pack_equip)

	# 槽装备（强制装备）
	def equipSlot(self, e_type_id=None, e_type=None, slot_item=None, pack_equip=None):

		if slot_item is None:
			slot_item = self.getSlotItem(e_type_id=e_type_id, e_type=e_type)

		if slot_item is None:
			return None
		else:
			slot_item: HumanEquipSlotItem = slot_item

			self.ensureEquipCondition(pack_equip)
			slot_item.equip(pack_equip)

	# 槽卸下（强制卸下）
	def dequipSlot(self, e_type_id=None, e_type=None, slot_item=None):

		if slot_item is None:
			slot_item = self.getSlotItem(e_type_id=e_type_id, e_type=e_type)

		if slot_item is None:
			return None
		else:
			self.ensureDequipCondition(slot_item)
			return slot_item.dequip()

	# 接受转移
	def acceptTransfer(self, e_type_id=None, e_type=None, slot_item=None, cont_item=None, **kwargs):

		if cont_item is not None:
			self.equipSlot(e_type_id, e_type, slot_item, cont_item)

	# 准备转移
	def prepareTransfer(self, e_type_id=None, e_type=None, slot_item=None, **kwargs) -> dict:
		res = kwargs

		res['cont_item'] = self.dequipSlot(e_type_id, e_type, slot_item)

		return res


# ===================================================
#  艾瑟萌装备槽
#  有 ExermonEquipType.Count 个固定的槽
# ===================================================
class ExerEquipSlot(SlotContainer):

	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌装备槽"

	# 艾瑟萌
	exermon = models.OneToOneField('exermon_module.ExerSlotItem',
								   on_delete=models.CASCADE, verbose_name="艾瑟萌")

	# 艾瑟萌背包
	exer_pack = models.ForeignKey('ExerPack', on_delete=models.CASCADE, verbose_name="艾瑟萌背包")

	# 容器类型
	@classmethod
	def containerType(cls): return ContainerType.ExerEquipSlot

	# 容器接受物品类型（数组）
	@classmethod
	def acceptTypes(cls): return [ContItemType.ExerPackEquip]

	# 容器接受物品类型（数组）
	@classmethod
	def baseContItemClass(cls): return ExerPackEquip

	# 获取物品对应的容器项
	def contItemClass(self, **kwargs): return ExerEquipSlotItem

	# 获取容器容量（0为无限）
	def getCapacity(self): return ExerEquipType.count()

	# 创建一个槽（创建角色时候执行）
	@classmethod
	def _create(cls, exermon):
		slot: cls = super()._create()
		slot.exermon = exermon
		slot.exer_pack = exermon.player.exerpack
		return slot

	# 保证满足装备条件
	def ensureEquipCondition(self, pack_equip):
		self.ensureItemAcceptable(cont_item=pack_equip)

		return True

	# 保证满足装备卸下条件
	def ensureDequipCondition(self, slot_item):
		return True

	# 获取一个槽项
	def getSlotItem(self, e_type_id=None, e_type=None, **kwargs):
		if e_type_id is not None:
			return super().getSlotItem(e_type_id=e_type_id, **kwargs)
		elif e_type is not None:
			return super().getSlotItem(e_type=e_type, **kwargs)

		return None

	# 更换装备
	def setEquip(self, e_type=None, e_type_id=None, slot_item=None, pack_equip=None):
		self.transfer(self.exer_pack, e_type=e_type,
			e_type_id=e_type_id, slot_item=slot_item)

		if pack_equip is not None:
			self.exer_pack.transfer(self, e_type=e_type, e_type_id=e_type_id,
				slot_item=slot_item, cont_item=pack_equip)

	# 槽装备（强制装备）
	def equipSlot(self, e_type_id=None, e_type=None, slot_item=None, pack_equip=None):

		if slot_item is None:
			slot_item = self.getSlotItem(e_type_id=e_type_id, e_type=e_type)

		if slot_item is None:
			return None
		else:
			slot_item: ExerEquipSlotItem = slot_item
			self.ensureEquipCondition(pack_equip)
			slot_item.equip(pack_equip)

	# 槽卸下（强制卸下）
	def dequipSlot(self, e_type_id=None, e_type=None, slot_item=None):

		if slot_item is None:
			slot_item = self.getSlotItem(e_type_id=e_type_id, e_type=e_type)

		if slot_item is None:
			return None
		else:
			slot_item: ExerEquipSlotItem = slot_item
			self.ensureDequipCondition(slot_item)
			return slot_item.dequip()

	# 接受转移
	def acceptTransfer(self, e_type_id=None, e_type=None, slot_item=None, cont_item=None, **kwargs):

		if cont_item is not None:
			self.equipSlot(e_type_id, e_type, slot_item, cont_item)

	# 准备转移
	def prepareTransfer(self, e_type_id=None, e_type=None, slot_item=None, **kwargs)->dict:
		res = kwargs

		res['cont_item'] = self.dequipSlot(e_type_id, e_type, slot_item)

		return res

# endregion

# region 物品-容器


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
#  基本容器物品表
# ===================================================
class BaseContItem(models.Model):
	class Meta:
		verbose_name = verbose_name_plural = "基本容器物品"

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
		(ContItemType.PlayerExermon.value, '艾瑟萌槽项'),
	]

	# 容器
	container = models.ForeignKey('BaseContainer', on_delete=models.CASCADE,
								  null=True, blank=True, verbose_name="容器")

	# 容器标识类型
	type = models.PositiveSmallIntegerField(default=ContItemType.Unset.value,
											choices=TYPES, verbose_name="容器类型")

	# 容器类型
	@classmethod
	def contItemType(cls): return ContItemType.Unset

	def __init__(self, *args, **kwargs):
		super().__init__(*args, **kwargs)
		if self.type == ContItemType.Unset.value:
			self.type = self.contItemType().value

	def __str__(self):
		type_name = self.getTypeName()

		return '%d %s' % (self.id, type_name)

	# 获取类型名称
	def getTypeName(self):
		for type in self.TYPES:
			if self.type == type[0]:
				return type[1]

		return self.TYPES[0][1]

	# 获取目标的物品
	def targetContItem(self):

		target = BaseContItem

		type_ = ContItemType(self.type)
		if type_ != ContItemType.Unset:
			target = eval(type_.name)

		if type(self) == target: return self

		# if self.type == ContainerType.HumanPack.value: target = HumanPack
		# if self.type == ContainerType.ExermonPack.value: target = HumanPack
		# if self.type == ContainerType.ExermonFragmentPack.value: target = HumanPack
		# if self.type == ContainerType.QuestionSugarPack.value: target = HumanPack
		# if self.type == ContainerType.HumanEquipSlot.value: target = HumanPack
		# if self.type == ContainerType.ExermonEquipSlot.value: target = HumanPack
		# if self.type == ContainerType.ExerSlot.value: target = ExerSlot
		# if self.type == ContainerType.ExermonGiftPool.value: target = HumanPack
		# if self.type == ContainerType.ExermonWarehouse.value: target = HumanPack

		return ViewUtils.getObject(target, ErrorType.ContItemNotExist,
								   return_type='object', id=self.id)

	# 获取目标容器
	def targetContainer(self):
		if self.container is None: return None
		return self.container.targetContainer()

	# 最大叠加数量
	def maxCount(self): return 1

	# 创建容器项
	def create(self, container: BaseContainer, **kwargs):
		self.transfer(container, **kwargs)

	# 赋值容器项
	def copy(self, cont_item):
		pass

	# 移动容器项
	def transfer(self, container: BaseContainer, **kwargs):
		self.container = container

	# 移除容器项
	def remove(self):
		self.container = None


# ===================================================
#  槽类容器项表
# ===================================================
class SlotContItem(BaseContItem):
	class Meta:
		verbose_name = verbose_name_plural = "槽类容器项"

	# 最大叠加数量
	def maxCount(self): return 1

	# 配置索引
	def setupIndex(self, index):
		pass

	# 装备
	def equip(self, **kwargs):
		pass

	# 卸下
	def dequip(self, **kwargs):
		pass


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

	# 最大叠加数量
	def maxCount(self):
		if self.item is not None:
			return self.item.maxCount()
		return 1

	# 目标物品
	def targetItem(self):
		if self.item is None: return None
		return self.item.targetItem()

	# 创建容器项
	def create(self, container: BaseContainer, item=None, **kwargs):
		self.item = item
		super().create(container, **kwargs)

	# 赋值容器项
	def copy(self, cont_item):
		super().copy(cont_item)
		self.item = cont_item.item

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

		if self.count <= 0:
			delta = -self.count
			self.count = max(0, self.count)
			self.remove()
			return max(0, delta)  # 返回剩余无法删除的数量

		return 0


# ===================================================
#  人类背包物品
# ===================================================
class HumanPackItem(PackContItem):
	class Meta:
		verbose_name = verbose_name_plural = "人类背包物品"

	# 容器类型
	@classmethod
	def contItemType(cls): return ContItemType.HumanPackItem


# ===================================================
#  人类背包装备
# ===================================================
class HumanPackEquip(PackContItem):
	class Meta:
		verbose_name = verbose_name_plural = "人类背包装备"

	# 容器类型
	@classmethod
	def contItemType(cls): return ContItemType.HumanPackEquip


# ===================================================
#  艾瑟萌背包物品
# ===================================================
class ExerPackItem(PackContItem):
	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌背包物品"

	# 容器类型
	@classmethod
	def contItemType(cls): return ContItemType.ExerPackItem


# ===================================================
#  艾瑟萌背包装备
# ===================================================
class ExerPackEquip(PackContItem):
	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌背包装备"

	# 容器类型
	@classmethod
	def contItemType(cls): return ContItemType.ExerPackEquip


# ===================================================
#  艾瑟萌装备槽项
# ===================================================
class ExerEquipSlotItem(SlotContItem):
	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌装备槽项"

	# 艾瑟萌背包装备
	pack_equip = models.OneToOneField('ExerPackEquip', on_delete=models.CASCADE,
								 verbose_name="艾瑟萌背包装备")

	# 装备槽类型
	e_type = models.ForeignKey('game_module.ExerEquipType', on_delete=models.CASCADE,
							   verbose_name="装备槽类型")

	# 容器类型
	@classmethod
	def contItemType(cls): return ContItemType.ExerEquipSlotItem

	# 配置索引
	def setupIndex(self, index):
		self.e_type_id = index

	# 装备
	def equip(self, pack_equip=None, **kwargs):
		self.pack_equip = pack_equip
		self.pack_equip.transfer(self.container)

	# 卸下
	def dequip(self, **kwargs):
		self.pack_equip.remove()
		pack_equip = self.pack_equip
		self.pack_equip = None
		return pack_equip


# ===================================================
#  人类装备槽项
# ===================================================
class HumanEquipSlotItem(SlotContItem):
	class Meta:
		verbose_name = verbose_name_plural = "人类装备槽项"

	# 人类背包装备
	pack_equip = models.OneToOneField('HumanPackEquip', on_delete=models.CASCADE,
								 verbose_name="人类背包装备")

	# 装备槽类型
	e_type = models.ForeignKey('game_module.HumanEquipType', on_delete=models.CASCADE,
							   verbose_name="装备槽类型")

	# 容器类型
	@classmethod
	def contItemType(cls): return ContItemType.HumanEquipSlotItem

	# 配置索引
	def setupIndex(self, index):
		self.e_type_id = index

	# 装备
	def equip(self, pack_equip=None):
		self.pack_equip: ExerPackEquip = pack_equip
		self.pack_equip.transfer(self.container)

	# 卸下
	def dequip(self):
		self.pack_equip.remove()
		pack_equip = self.pack_equip
		self.pack_equip = None
		return pack_equip


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


# ===================================================
#  使用效果编号枚举
# ===================================================
class ItemEffectCode(Enum):
	Unset = 0  # 未设置


# ===================================================
#  使用效果表（包括艾瑟萌技能）
# ===================================================
class ItemEffect(models.Model):
	class Meta:
		verbose_name = verbose_name_plural = "物品使用效果"

	CODES = [
		(ItemEffectCode.Unset.value, '无效果'),
	]

	# 物品
	item = models.ForeignKey('UsableItem', on_delete=models.CASCADE, verbose_name="物品")

	# 效果编号
	code = models.PositiveSmallIntegerField(default=0, choices=CODES, verbose_name="效果编号")

	# 效果参数
	params = jsonfield.JSONField(default=[], verbose_name="效果参数")

# endregion

from exermon_module.models import *