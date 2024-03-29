from django.db import models
from django.db.models.query import QuerySet

from game_module.models import GameConfigure, \
	BaseParam, ParamValue

from utils.cache_utils import CachePool, CacheHelper
from utils.view_utils import Common as ViewUtils
from utils.model_utils import EnumMapper, \
	CacheableModel, CoreModel, Common as ModelUtils
from utils.exception import ErrorType, GameException

from enum import Enum

import jsonfield, math


# region 枚举设定

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

	# ExerProItem 301+
	ExerProItem = 301  # 特训物品
	ExerProPotion = 302  # 特训药水
	ExerProCard = 303  # 特训卡片

	ExerProEnemy = 310  # 特训敌人
	ExerProState = 311  # 特训状态


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

	# 对战物资槽
	BattleItemSlot = 301  # 对战物资槽


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

	# 对战物资槽
	BattleItemSlotItem = 301  # 对战物资槽项


# endregion

# region 物品系统管理类


class ItemManager:
	"""
	物品管理类
	"""

	@classmethod
	def registerItem(cls, verbose_name):
		# cont_item_cla: 'PackContItem' = None):
		"""
		注册物品
		Args:
			verbose_name (str): 别名
			# cont_item_cla (type): 容器项类
		"""
		def wrapper(cla: BaseItem):
			print("registerItem: %s" % cla)

			cla._meta.verbose_name = \
				cla._meta.verbose_name_plural = verbose_name

			cla.TYPE = eval("ItemType.%s" % cla.__name__)

			# cla.CONTITEM_CLASS = cont_item_cla

			EnumMapper.registerClass(cla)

			return cla

		return wrapper

	@classmethod
	def registerPackContainer(cls, verbose_name):
		"""
		注册容器
		Args:
			verbose_name (str): 别名
		"""
		def wrapper(cla: PackContainer):
			cla._meta.verbose_name = \
				cla._meta.verbose_name_plural = verbose_name

			cla.TYPE = eval("ContainerType.%s" % cla.__name__)

			EnumMapper.registerClass(cla)

			return cla

		return wrapper

	@classmethod
	def registerSlotContainer(cls, verbose_name):
		"""
		注册容器
		Args:
			verbose_name (str): 别名
		"""
		def wrapper(cla: SlotContainer):
			cla._meta.verbose_name = \
				cla._meta.verbose_name_plural = verbose_name

			cla.TYPE = eval("ContainerType.%s" % cla.__name__)

			EnumMapper.registerClass(cla)

			return cla

		return wrapper

	@classmethod
	def registerPackContItem(cls, verbose_name,
							 container_cla: 'PackContainer',
							 accepted_item_cla: 'BaseItem'):
		"""
		注册背包容器
		Args:
			verbose_name (str): 别名
			container_cla (type): 背包容器类
			accepted_item_cla (type): 接受物品类
		"""
		def wrapper(cla: PackContItem):
			cla._meta.verbose_name = \
				cla._meta.verbose_name_plural = verbose_name

			cla.TYPE = eval("ContItemType.%s" % cla.__name__)

			cla.CONTAINER_CLA = container_cla
			cla.ACCEPTED_ITEM_CLASS = accepted_item_cla

			container = models.ForeignKey(
				container_cla, on_delete=models.CASCADE,
				null=True, blank=True, verbose_name="容器")

			item = models.ForeignKey(
				accepted_item_cla, on_delete=models.CASCADE,
				null=True, blank=True, verbose_name="物品")

			cla.add_to_class('container', container)
			cla.add_to_class('item', item)

			setupItem()
			setupContainer(cla)

			EnumMapper.registerClass(cla)

			return cla

		def setupContainer(cla: PackContItem):

			if container_cla.ACCEPTED_CONTITEM_CLASSES is None:
				container_cla.ACCEPTED_CONTITEM_CLASSES = []

			container_cla.ACCEPTED_CONTITEM_CLASSES.append(cla)

			base_cont_item_cla = cls._getCommonSuperClass(
				*container_cla.ACCEPTED_CONTITEM_CLASSES)

			container_cla.BASE_CONTITEM_CLASS = base_cont_item_cla

		def setupItem():

			accepted_item_cla.CONTAINER_CLASS = container_cla

		return wrapper

	@classmethod
	def registerSlotContItem(cls, verbose_name,
							 container_cla: 'SlotContainer',
							 **equip_item_args):
		"""
		注册槽容器
		Args:
			verbose_name (str): 别名
			container_cla (type): 容器类
			**equip_item_args (**dict): 装备项参数（键：装备项键名，值：装备项类型）
		"""
		def wrapper(cla: SlotContItem):

			cla._meta.verbose_name = \
				cla._meta.verbose_name_plural = verbose_name

			cla.TYPE = eval("ContItemType.%s" % cla.__name__)
			cla.CONTAINER_CLA = container_cla

			cla.ACCEPTED_EQUIP_ITEM_ATTRS = []
			cla.ACCEPTED_EQUIP_ITEM_CLASSES = []

			container = models.ForeignKey(
				container_cla, on_delete=models.CASCADE, verbose_name="容器")

			cla.add_to_class('container', container)

			for key in equip_item_args:
				equip_item_cla: PackContItem = equip_item_args[key]
				cla.ACCEPTED_EQUIP_ITEM_ATTRS.append(key)
				cla.ACCEPTED_EQUIP_ITEM_CLASSES.append(equip_item_cla)

				field = models.ForeignKey(equip_item_cla,
					null=True, blank=True, on_delete=models.SET_NULL,
					verbose_name=equip_item_cla._meta.verbose_name)

				cla.add_to_class(key, field)

			setupContainer(cla)

			EnumMapper.registerClass(cla)

			return cla

		def setupContainer(cla: SlotContItem):

			pack_item_clas = []

			for key in equip_item_args:
				pack_item_clas.append(equip_item_args[key])

			base_cont_item_cla = cls.\
				_getCommonSuperClass(*pack_item_clas)

			container_cla.BASE_CONTITEM_CLASS = base_cont_item_cla
			container_cla.ACCEPTED_CONTITEM_CLASSES = pack_item_clas
			container_cla.ACCEPTED_SLOTITEM_CLASS = cla

		return wrapper

	@classmethod
	def _getCommonSuperClass(cls, *clas) -> type:
		# 极端情况
		if len(clas) == 0: return None
		if len(clas) == 1: return clas[0]
		# 两个的情况
		if len(clas) == 2:
			mro1, mro2 = clas[0].mro(), clas[1].mro()

			for cla_i in mro1:
				for cla_j in mro2:
					if cla_i == cla_j: return cla_i

			return object

		# 其他情况
		cla: type = clas[0]
		for i in range(len(clas)-1):
			cla = cls._getCommonSuperClass(cla, clas[i+1])

		return cla


# endregion

# region 属性对象

class BaseParamsObject:
	"""
	基本属性对象
	"""
	# 管理界面显示属性
	def _adminParams(self, params):
		from django.utils.html import format_html

		res = ''
		for p in params:
			res += str(p) + "<br>"

		return format_html(res)

	def _getVals(self, db_vals, val_func,
				 param_cla, clamp=True) -> QuerySet or list:

		if param_cla is None: return []

		# 尝试获取数据库值（如果有的话）
		if db_vals is not None: return db_vals

		vals = []
		params = BaseParam.objs()

		for param in params:
			val = param_cla(param=param)
			val.setValue(val_func(param_id=param.id), clamp)
			vals.append(val)

		return vals

	def _getVal(self, params, **kwargs):

		if isinstance(params, QuerySet):
			params = params.filter(**kwargs)

			if not params.exists(): return 0
			param: ParamValue = params.first()

		else:
			param: ParamValue = ModelUtils.get(params, **kwargs)

		return 0 if param is None else param.getValue()


class ParamsObject(BaseParamsObject):
	"""
	拥有属性计算的对象
	术语：Val：实际值，Base：基础值，Rate：成长率
	"""

	# 管理界面用：显示属性基础值
	def adminParamVals(self):
		return self._adminParams(self.paramVals())

	adminParamVals.short_description = "属性实际值"

	# 管理界面用：显示属性基础值
	def adminParamBases(self):
		return self._adminParams(self.paramBases())

	adminParamBases.short_description = "属性基础值"

	# 管理界面用：显示属性成长率
	def adminParamRates(self):
		return self._adminParams(self.paramRates())

	adminParamRates.short_description = "属性成长率"

	# 管理界面用：显示属性成长率
	def adminBattlePoint(self):
		return self.battlePoint()

	adminBattlePoint.short_description = "战斗力"

	# 类型配置
	@classmethod
	def paramValueClass(cls):
		return cls.paramBaseClass()

	@classmethod
	def paramBaseClass(cls): return None

	@classmethod
	def paramRateClass(cls): return None

	# 数据库值获取（默认为 None 表示无）
	def _paramVals(self) -> QuerySet:
		return self._paramBases()

	def _paramBases(self) -> QuerySet:
		return None

	def _paramRates(self) -> QuerySet:
		return None

	# 值计算（用于无法从数据库直接获取的属性，一般用于计算）
	def _paramVal(self, **kwargs) -> float:
		return self._paramBase(**kwargs)

	def _paramBase(self, **kwargs) -> float:
		return 0

	def _paramRate(self, **kwargs) -> float:
		return 0

	# 最终值数组（缓存）
	@CacheHelper.normalCache
	def paramVals(self) -> QuerySet or list:
		"""
		获取所有实际属性数组
		Returns:
			返回所有实际属性的数组（元素类型为 ExerParamBase）
		"""
		return self._getVals(self._paramVals(), self._paramVal,
							 self.paramValueClass())

		# # 尝试获取数据库值（如果有的话）
		# vals = self._paramVals()
		# if vals is not None: return vals
		#
		# vals = []
		# params = BaseParam.objs()
		#
		# for param in params:
		# 	val = self.paramValueClass()(param=param)
		# 	val.setValue(self._paramVal(param_id=param.id), False)
		# 	vals.append(val)
		#
		# return vals

	@CacheHelper.normalCache
	def paramBases(self) -> QuerySet or list:
		return self._getVals(self._paramBases(), self._paramBase,
							 self.paramBaseClass())

	@CacheHelper.normalCache
	def paramRates(self) -> QuerySet or list:
		return self._getVals(self._paramRates(), self._paramRate,
							 self.paramRateClass())

	# 单个值获取（通过缓存）
	def paramVal(self, **kwargs):
		return self._getVal(self.paramVals(), **kwargs)

	def paramBase(self, **kwargs):
		return self._getVal(self.paramBases(), **kwargs)

	def paramRate(self, **kwargs):
		return self._getVal(self.paramRates(), **kwargs)

	# 清除属性缓存
	def _clearParamsCache(self):
		CacheHelper.clearCache(self,
			self.paramVals, self.paramBases, self.paramRates)

	# 战斗力
	def battlePoint(self):
		from utils.calc_utils import BattlePointCalc
		return BattlePointCalc.calc(self.paramVal)


class EquipParamsObject(BaseParamsObject):
	"""
	装备属性计算的对象
	术语：Level：等级加成值，Base：基础值
	"""

	# 管理界面用：显示属性基础值
	def adminLevelParams(self):
		return self._adminParams(self.levelParams())

	adminLevelParams.short_description = "等级属性值"

	# 管理界面用：显示属性基础值
	def adminBaseParams(self):
		return self._adminParams(self.baseParams())

	adminBaseParams.short_description = "基础属性值"

	# 类型配置
	@classmethod
	def levelParamClass(cls): return None

	@classmethod
	def baseParamClass(cls): return None

	# 数据库值获取或者间接获取（默认为 None 表示无）
	def _levelParams(self) -> QuerySet or list:
		return None

	def _baseParams(self) -> QuerySet or list:
		return None

	# 值计算（用于无法从数据库直接获取的属性，一般用于计算）
	def _levelParam(self, **kwargs) -> float:
		return 0

	def _baseParam(self, **kwargs) -> float:
		return 0

	# 最终值数组（缓存）
	@CacheHelper.normalCache
	def levelParams(self) -> QuerySet or list:
		return self._getVals(self._levelParams(), self._levelParam,
							 self.levelParamClass(), False)

	@CacheHelper.normalCache
	def baseParams(self) -> QuerySet or list:
		return self._getVals(self._baseParams(), self._baseParam,
							 self.baseParamClass(), False)

	# 单个值获取（通过缓存）
	def levelParam(self, **kwargs):
		return self._getVal(self.levelParams(), **kwargs)

	def baseParam(self, **kwargs):
		return self._getVal(self.baseParams(), **kwargs)

	# 清除属性缓存
	def _clearParamsCache(self):
		CacheHelper.clearCache(self,
			self.levelParams, self.baseParams)

# endregion

# region 物品系统


# ===================================================
#  货币表
# ===================================================
class Currency(models.Model):
	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "货币"

	LIST_DISPLAY_APPEND = ['item']

	# 金币
	gold = models.PositiveIntegerField(default=0, verbose_name="金币")

	# 点券
	ticket = models.PositiveIntegerField(default=0, verbose_name="点券")

	# 绑定点券
	bound_ticket = models.PositiveIntegerField(default=0, verbose_name="绑定点券")

	def __str__(self):
		term: GameConfigure = GameConfigure.get()
		return '%s：%d %s：%d %s：%d' % (
			term.gold, self.gold,
			term.ticket, self.ticket,
			term.bound_ticket, self.bound_ticket)

	def convert(self):
		return {
			'gold': self.gold,
			'ticket': self.ticket,
			'bound_ticket': self.bound_ticket
		}

	def __add__(self, val) -> 'Currency':
		res = self

		if isinstance(val, int):
			res = type(self)()
			res.gold = self.gold + val
			res.ticket = self.ticket
			res.bound_ticket = self.bound_ticket

		if isinstance(val, Currency):
			res = type(self)()
			res.gold = self.gold + val.gold
			res.ticket = self.ticket + val.ticket
			res.bound_ticket = \
				self.bound_ticket + val.bound_ticket

		return res

	def __sub__(self, val) -> 'Currency':
		res = self

		if isinstance(val, int):
			res = type(self)()
			res.gold = self.gold - val
			res.ticket = self.ticket
			res.bound_ticket = self.bound_ticket

		if isinstance(val, Currency):
			res = type(self)()
			res.gold = self.gold - val.gold
			res.ticket = self.ticket - val.ticket
			res.bound_ticket = \
				self.bound_ticket - val.bound_ticket

		return res

	def __neg__(self) -> 'Currency':
		res = type(self)()
		res.gold = -self.gold
		res.ticket = -self.ticket
		res.bound_ticket = -self.bound_ticket

		return res

	def __mul__(self, cnt) -> 'Currency':
		res = self

		if isinstance(cnt, int):
			res = type(self)()
			res.gold = self.gold * cnt
			res.ticket = self.ticket * cnt
			res.bound_ticket = self.bound_ticket * cnt

		if isinstance(cnt, Currency):
			res = type(self)()
			res.gold = self.gold * cnt.gold
			res.ticket = self.ticket * cnt.ticket
			res.bound_ticket = \
				self.bound_ticket * cnt.bound_ticket

		return res

	def isEmpty(self):
		return self.gold == self.ticket == self.bound_ticket == 0


# region 物品


# ===================================================
#  基础物品表
# ===================================================
class BaseItem(CoreModel):
	class Meta:
		abstract = True
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

		# ExerProItem
		(ItemType.ExerProItem.value, '特训物品'),
		(ItemType.ExerProPotion.value, '特训药水'),
		(ItemType.ExerProCard.value, '特训卡片'),

		(ItemType.ExerProEnemy.value, '特训敌人'),
		(ItemType.ExerProState.value, '特训状态'),
	]

	# 道具类型
	TYPE = ItemType.Unset

	# CONTITEM_CLASS: 'PackContItem' = None
	CONTAINER_CLASS: 'PackContainer' = None

	# 名称
	name = models.CharField(max_length=24, verbose_name="名称")

	# 描述
	description = models.CharField(max_length=128, blank=True, verbose_name="描述")

	# 物品 标识类型
	# type = models.PositiveSmallIntegerField(default=ItemType.Unset.value,
	# 										choices=TYPES, verbose_name="物品类型")

	# region 配置项

	# 对应的容器项类
	@classmethod
	def contItemClass(cls):
		return cls.containerClass().contItemClass(cls)

	# 对应的容器类
	@classmethod
	def containerClass(cls): return cls.CONTAINER_CLASS

	# endregion

	# region 类型配置

	def __str__(self):
		return '%d %s' % (self.id, self.name)

	# def __getattr__(self, item):
	# 	raise AttributeError(item)

	# 可否被购买
	def isBoughtable(self):
		return False

	# 转化为 dict
	def convert(self, **kwargs):
		return {
			'id': self.id,
			'name': self.name,
			'description': self.description,
			'type': self.TYPE.value
		}

	# endregion

	# 最大叠加数量（为0则不限）
	def maxCount(self): return 1


# ===================================================
#  物品价格表
# ===================================================
# class ItemPrice(Currency):
# 	class Meta:
# 		verbose_name = verbose_name_plural = "物品价格"
#
# 	# 对应物品
# 	item = models.OneToOneField("LimitedItem", on_delete=models.CASCADE,
# 									 null=True, verbose_name="物品")
#
# 	# def __str__(self):
# 	# 	return '%s：%s' % (self.limiteditem, super().__str__())


# ===================================================
#  有限物品表（一般为背包的物品）
# ===================================================
class LimitedItem(BaseItem):
	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "有限物品"

	LIST_DISPLAY_APPEND = ['adminBuyPrice']

	# 物品星级
	star = models.ForeignKey("game_module.ItemStar", on_delete=models.CASCADE, verbose_name="星级")

	# 购买价格（None为不可购买）
	# buy_price = models.OneToOneField('item_module.Currency', null=True, blank=True,
	# 								 on_delete=models.CASCADE, verbose_name="购买价格")

	# 图标索引
	icon_index = models.PositiveSmallIntegerField(default=0, verbose_name="图标索引")

	# 出售价格（出售固定为金币，为0则不可出售）
	sell_price = models.PositiveIntegerField(default=0, verbose_name="出售价格")

	# 是否可丢弃
	discardable = models.BooleanField(default=True, verbose_name="可丢弃")

	# 能否交易
	tradable = models.BooleanField(default=True, verbose_name="可交易")

	# 物品图标
	# icon = models.ImageField(upload_to=ItemIconUpload(),
	# 						 null=True, blank=True, verbose_name="图标")

	# 管理界面用：显示购入价格
	def adminBuyPrice(self):
		return self.buyPrice()

	adminBuyPrice.short_description = "购入价格"

	# 可否被购买
	def isBoughtable(self):
		buy_price: Currency = self.buyPrice()
		if buy_price is None: return False
		return not buy_price.isEmpty()

	# 获取购买价格
	def buyPrice(self):
		raise NotImplementedError

	# try: return self.itemprice
	# except ItemPrice.DoesNotExist: return None

	# # 获取完整路径
	# def getExactlyIconPath(self):
	# 	base = settings.STATIC_URL
	# 	path = os.path.join(base, str(self.icon))
	# 	if os.path.exists(path):
	# 		return path
	# 	else:
	# 		raise ErrorException(ErrorType.PictureFileNotFound)
	#
	# # 获取图标base64编码
	# def convertIconToBase64(self):
	# 	import base64
	#
	# 	with open(self.getExactlyIconPath(), 'rb') as f:
	# 		data = base64.b64encode(f.read())
	#
	# 	return data.decode()

	# 转化为 dict
	def convert(self, type=None, **kwargs):
		buy_price = ModelUtils.objectToDict(self.buyPrice())

		if type == "shop":
			return {
				'id': self.id,
				'type': self.TYPE.value,
				'price': buy_price
			}

		res = super().convert(**kwargs)

		res['star_id'] = self.star_id
		res['icon_index'] = self.icon_index
		res['buy_price'] = buy_price
		res['sell_price'] = self.sell_price
		res['discardable'] = self.discardable
		res['tradable'] = self.tradable
		# res['icon'] = self.convertIconToBase64()

		return res


# ===================================================
#  物品使用场合
# ===================================================
class ItemUseOccasion(Enum):
	Unknown = 0  # 未知
	Battle = 1  # 对战中
	Menu = 2  # 菜单（背包中）
	Adventure = 3  # 冒险中


# ===================================================
#  物品使用目标
# ===================================================
class ItemUseTargetType(Enum):
	Default = 0  # 默认（对战物品设置为默认即可）
	Human = 1  # 人类
	Exermon = 2  # 艾瑟萌


# ===================================================
#  可用物品表
# ===================================================
class UsableItem(LimitedItem):
	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "可用物品"

	TARGET_TYPES = [
		(ItemUseTargetType.Default.value, '无'),
		(ItemUseTargetType.Human.value, '人物'),
		(ItemUseTargetType.Exermon.value, '艾瑟萌'),
	]

	LIST_DISPLAY_APPEND = ['adminEffects']

	# 叠加数量
	max_count = models.PositiveSmallIntegerField(default=99, verbose_name="叠加数量")

	# 对战道具
	battle_use = models.BooleanField(default=True, verbose_name="对战道具")

	# 背包道具
	menu_use = models.BooleanField(default=True, verbose_name="背包道具")

	# 冒险道具
	adventure_use = models.BooleanField(default=True, verbose_name="冒险道具")

	# 使用目标
	target = models.PositiveSmallIntegerField(default=ItemUseTargetType.Default.value,
											  choices=TARGET_TYPES, verbose_name="使用目标")

	# 批量使用个数（为0则不限）
	batch_count = models.PositiveSmallIntegerField(default=0, verbose_name="使用个数")

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
	def convert(self, **kwargs):
		res = super().convert(**kwargs)

		effects = ModelUtils.objectsToDict(self.effects())

		res['max_count'] = self.max_count
		res['consumable'] = self.consumable
		res['battle_use'] = self.battle_use
		res['menu_use'] = self.menu_use
		res['adventure_use'] = self.adventure_use
		res['target'] = self.target
		res['freeze'] = self.freeze
		res['batch_count'] = self.batch_count
		res['i_type'] = self.i_type_id
		res['effects'] = effects

		return res

	def _isTargetUsable(self, target=None):
		from exermon_module.models import PlayerExermon

		if self.target == ItemUseTargetType.Exermon.value and \
				not isinstance(target, PlayerExermon): return False

		return True

	def _isBatchCountUsable(self, count):
		if self.batch_count <= 0: return True
		return count <= self.batch_count

	def _isOccasionUsable(self, occasion: ItemUseOccasion):
		return (occasion == ItemUseOccasion.Battle and self.battle_use) or \
			   (occasion == ItemUseOccasion.Menu and self.menu_use) or \
			   (occasion == ItemUseOccasion.Adventure and self.adventure_use)

	def isUsable(self, occasion: ItemUseOccasion, target=None, count=1):
		"""
		物品在特定场合下是否可用
		Args:
			occasion (ItemUseOccasion): 场合枚举
			target (PlayerExermon): 目标
			count (int): 使用次数
		Returns:
			返回物品在指定场合下的可用性
		"""
		return self._isTargetUsable(target) and \
			   self._isBatchCountUsable(count) and \
			   self._isOccasionUsable(occasion)

	def ensureUsable(self, occasion: ItemUseOccasion, count=1, target=None):
		"""
		确保物品在特定场合下是否可用
		Args:
			occasion (ItemUseOccasion): 场合枚举
			target (PlayerExermon): 目标
			count (int): 使用次数
		Raises:
			ErrorType.InvalidUseTarget: 使用目标错误
			ErrorType.UnusableItem: 物品不可用
		"""
		if not self._isTargetUsable(target):
			raise GameException(ErrorType.InvalidUseTarget)

		if not self._isBatchCountUsable(count):
			raise GameException(ErrorType.InvalidBatchCount)

		if not self._isOccasionUsable(occasion):
			raise GameException(ErrorType.UnusableItem)

	# 最大叠加数量（为0则不限）
	def maxCount(self):
		return self.max_count

	# 获取所有的效果
	def effects(self):
		raise NotImplementedError

	# 获取购买价格
	def buyPrice(self):
		raise NotImplementedError


# # ===================================================
# #  装备属性值表
# # ===================================================
# class EquipParam(ParamValue):
#
# 	class Meta:
# 		verbose_name = verbose_name_plural = "装备属性值"
#
# 	# 装备
# 	equip = models.ForeignKey("EquipableItem", on_delete=models.CASCADE, verbose_name="装备")
#
# 	# 最大值
# 	def maxVal(self):
# 		return None
#
# 	# 最小值
# 	def minVal(self):
# 		return None


# ===================================================
#  装备属性类型枚举
# ===================================================
# class EquipParamType(Enum):
# 	NoType = 0  # 无类型
# 	Attack = 1  # 攻击型
# 	Defense = 2  # 防御型


# ===================================================
#  可装备物品
# ===================================================
class EquipableItem(LimitedItem, EquipParamsObject):
	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "可装备物品"

	LIST_DISPLAY_APPEND = ['adminLevelParams', 'adminBaseParams']

	# PARAM_TYPES = [
	# 	(EquipParamType.NoType.value, '无类型'),
	#
	# 	(EquipParamType.Attack.value, '攻击型'),
	# 	(EquipParamType.Defense.value, '防御型'),
	# ]

	# 最低等级
	min_level = models.PositiveSmallIntegerField(default=0, verbose_name="最低等级")

	# # 属性类型
	# param_type = models.PositiveSmallIntegerField(default=EquipParamType.NoType.value,
	# 											  choices=PARAM_TYPES, verbose_name="属性类型")
	#
	# # 属性比率（*100）
	# param_rate = models.PositiveSmallIntegerField(default=10, verbose_name="属性比率")

	# 管理界面用：显示属性成长值
	def adminLevelParams(self):
		from django.utils.html import format_html

		params = self.levelParams()

		res = ''
		for p in params:
			res += str(p) + "<br>"

		return format_html(res)

	adminLevelParams.short_description = "等级属性值"

	# 管理界面用：显示属性基础值
	def adminBaseParams(self):
		from django.utils.html import format_html

		params = self.baseParams()

		res = ''
		for p in params:
			res += str(p) + "<br>"

		return format_html(res)

	adminBaseParams.short_description = "基础属性值"

	# 转化为 dict
	def convert(self, **kwargs):
		res = super().convert(**kwargs)

		res['min_level'] = self.min_level
		# res['param_type'] = self.param_type
		# res['param_rate'] = self.param_rate
		res['level_params'] = ModelUtils.objectsToDict(self.levelParams())
		res['base_params'] = ModelUtils.objectsToDict(self.baseParams())

		return res

	# 获取所有的等级属性值
	def _levelParams(self):
		raise NotImplementedError

	# 获取所有的属性基本值
	def _baseParams(self):
		raise NotImplementedError

	# 获取购买价格
	def buyPrice(self):
		raise NotImplementedError

# endregion

# region 容器


# ===================================================
#  基本容器
# ===================================================
class BaseContainer(CacheableModel):
	class Meta:
		abstract = True
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

	BASE_CONTITEM_CLASS: 'BaseContItem' = None

	ACCEPTED_CONTITEM_CLASSES: tuple = None

	LIST_DISPLAY_APPEND = ['adminOwnerPlayer']

	# 玩家
	# player = models.ForeignKey('player_module.Player', on_delete=models.CASCADE,
	#						   verbose_name="玩家")

	# 容器标识类型
	# type = models.PositiveSmallIntegerField(default=ContainerType.Unset.value,
	# 										choices=TYPES, verbose_name="容器类型")

	# region 配置项

	@classmethod
	def baseContItemClass(cls) -> 'BaseContItem':
		"""
		获取设定好的所接受的容器项基类
		Returns:
			所接受容器基类（基于 BaseContItem ）
		"""
		return cls.BASE_CONTITEM_CLASS

	@classmethod
	def acceptedContItemClasses(cls) -> tuple:
		"""
		返回设定好的所接受的容器项类（返回多个，查询/判断时用）
		Returns:
			所接受的容器项类数元组
		"""
		return cls.ACCEPTED_CONTITEM_CLASSES or (cls.baseContItemClass(),)

	@classmethod
	def _cacheForeignKeyModels(cls):
		return cls.acceptedContItemClasses()

	# 获取对应的容器项类（返回单个，创建容器项时候调用）
	# 可以根据参数获取实际的容器项类
	# 默认为基类，即 baseContItemClass
	@classmethod
	def contItemClass(cls, **kwargs) -> 'BaseContItem':
		"""
		获取对应的容器项类（返回单个，创建容器项时候调用）
		可以根据参数获取实际的容器项类
		Args:
			**kwargs (**dict): 子类重载参数
		Returns:

		"""
		return cls.baseContItemClass()

	@classmethod
	def defaultCapacity(cls) -> int:
		"""
		默认容器容量
		Returns:
			容器容量
		"""
		return 0

	# endregion

	# region 创建函数

	# 创建实例
	@classmethod
	def create(cls, **kwargs):
		container = cls()
		container._create(**kwargs)
		container.save()
		return container

	# 创建实例
	def _create(self, **kwargs):
		pass

	def __init__(self, *args, **kwargs):
		super().__init__(*args, **kwargs)

		self.exactlyPlayer().registerContainer(self)

	# endregion

	# region 类型配置

	# def __init__(self, *args, **kwargs):
	# 	super().__init__(*args, **kwargs)
	# 	self._cache(self.REMOVED_CACHE_KEY, [])

	def __str__(self):
		name = self._meta.verbose_name
		return '%s %s(%s)' % (str(self.id), name, str(self.owner()))

	def __getattr__(self, item):
		raise AttributeError(item)

	# 获取类型名称
	# def getTypeName(self):
	# 	for type in self.TYPES:
	# 		if self.type == type[0]:
	# 			return type[1]
	#
	# 	return self.TYPES[0][1]

	# 获取目标的容器
	# noinspection PyUnresolvedReferences
	# def target(self):
	#
	# 	from exermon_module.models import ExerHub, \
	# 		ExerGiftPool, ExerFragPack, ExerPack, \
	# 		ExerSlot, ExerSkillSlot, ExerEquipSlot
	# 	from player_module.models import HumanPack, HumanEquipSlot
	#
	# 	target = BaseItem
	#
	# 	type_ = ContainerType(self.type)
	# 	if type_ != ContainerType.Unset:
	# 		target = eval(type_.name)
	#
	# 	if type(self) == target: return self
	#
	# 	return ViewUtils.getObject(target, ErrorType.ContainerNotExist,
	# 							   return_type='object', id=self.id)

	# 转化容器项为 dict
	def _convertItemsToDict(self):
		return ModelUtils.objectsToDict(self.contItems())

	# 转化为 dict
	def convert(self, type: str = None, **kwargs) -> dict:
		"""
		转化为字典
		Args:
			type (str): 转化类型
			**kwargs (**dict): 子类重载参数
		Returns:
			转化后的字典数据
		"""
		res = {
			'id': self.id,
			'type': self.TYPE.value,
			'capacity': self.getCapacity(),
		}

		if type == 'items':
			res['items'] = self._convertItemsToDict()

		return res

	# endregion

	# admin 显示
	def adminOwnerPlayer(self):
		return str(self.ownerPlayer())

	adminOwnerPlayer.short_description = "所属玩家"

	# 持有者
	def owner(self) -> any:
		"""
		获取持有者（不一定是 Player )
		Returns:
			返回容器持有者
		"""
		raise NotImplementedError

	# 持有玩家
	def ownerPlayer(self):  # -> Player.Player:
		"""
		获取持有玩家
		Returns:
			返回容器持有玩家
		"""
		return self.owner()

	# 实际玩家（获取在线信息）
	def exactlyPlayer(self):  # -> Player.Player:
		"""
		获取实际的玩家（优先查找在线玩家）
		Returns:
			返回实际玩家对象
		"""
		from player_module.views import Common

		player = self.ownerPlayer()
		# 如果获取不到实际的玩家，该对象释放时需要自动保存
		player.delete_save = True

		online_player = Common.getOnlinePlayer(player.id)

		if online_player is None: return player
		return online_player.player

	def cateData(self):
		return type(self), self.id

	# 获取容器容量（0为无限）
	def getCapacity(self):
		return self.defaultCapacity()

	# 物品类型是否接受
	def isItemAcceptable(self, cont_item: 'BaseContItem' = None) -> bool:
		"""
		判断容器项/物品类型是否接受
		Args:
			cont_item (BaseContItem): 容器项
		Returns:
			返回该容器项/物品能否放入该容器
		"""
		if cont_item is not None:
			return isinstance(cont_item, self.acceptedContItemClasses())

		return False

	def contItems(self, cla: type = None,
				  cond: callable = None, **kwargs) -> list or QuerySet:
		"""
		获取满足一定条件的容器项（并载入缓存）
		支持条件获取和映射获取
		Args:
			cla (type): 类型
			cond (callable): 条件函数
			**kwargs (dict): 查询参数
		Returns:
			返回满足条件的所有容器项
		"""
		if cla is not None:
			# 根据 cache_pool 中储存类型返回 list or QuerySet
			return self._queryModelCache(cla, cond, **kwargs)

		# 只返回 list，且只读
		cont_items = []
		for _cla in self.acceptedContItemClasses():
			cont_items += list(self._queryModelCache(
				_cla, cond, **kwargs))

		return cont_items

	# # 查询缓存，如果有相同的参数，直接返回
	# cache = self.cache_cont_items
	# if cache is not None and cache['params'] == kwargs:
	# 	return cache['res']
	#
	# res = self._contItems(**kwargs)
	#
	# # 配置缓存
	# self.cache_cont_items = {'params': kwargs, 'res': res}
	#
	# return res

	# def _contItemsFromDb(self, listed: bool = False, cla: type = None,
	# 					 **kwargs) -> list or QuerySet:
	# 	"""
	# 	从数据库获取所有的容器物项
	# 	Args:
	# 		listed (bool): 是否以列表形式返回
	# 		cla (type): 类型
	# 		**kwargs (**dict): 查询参数
	# 	Returns:
	# 		从数据库中读取的物品容器项数据表>
	# 		（若 listed 为 True，返回 list 类型列表，否则返回 QuerySet 对象
	# 	"""
	# 	# 如果指定了一个物品，直接查询这个物品所在的 class
	# 	if 'item' in kwargs and kwargs['item'] is not None:
	# 		item = kwargs['item']
	# 		cla = type(item)
	#
	# 		kwargs['item_id'] = item.id
	# 		kwargs.pop('item')
	#
	# 		return self._contItemsFromDb(listed, cla, **kwargs)
	#
	# 	if cla is not None:
	# 		res = ViewUtils.getObjects(
	# 			cla, container_id=self.id, **kwargs)
	#
	# 		for obj in res: obj.container = self
	# 		return list(res) if listed else res
	#
	# 	clas = self._databaseQueryClass()
	#
	# 	if listed:
	# 		res = []
	# 		for c in clas:
	# 			res += list(ViewUtils.getObjects(
	# 				c, container_id=self.id, **kwargs))
	#
	# 		for obj in res: obj.container = self
	# 		return res
	# 	else:
	# 		res = {}
	# 		for c in clas:
	# 			objs = ViewUtils.getObjects(
	# 				c, container_id=self.id, **kwargs)
	# 			for obj in objs: obj.container = self
	# 			res[c] = objs
	# 		return res

	# 以人类可理解的形式展示该容器

	def show(self, name=None, cla=None):
		cont_items = self.contItems(cla)

		if name is None: name = str(self)

		print("%s(%s) ======================" % (name, self))
		print("Capacity: %d" % self.getCapacity())
		print("Used: %d" % self.contItemCnt(cla=cla))
		print("ContItems:")

		for cont_item in cont_items:
			print(cont_item)

	# def _cachedContItems(self) -> list:
	# 	"""
	# 	获取/生成缓存容器项
	# 	Returns:
	# 		返回缓存了的容器项列表
	# 	"""
	# 	return self._getOrSetCache(self.CONTITEM_CACHE_KEY,
	# 							   lambda: self._contItemsFromDb(listed=True))

	def _addCachedContItem(self, cont_item: 'BaseContItem'):
		"""
		添加缓存容器项
		Args:
			cont_item (BaseContItem): 容器项
		"""
		self._appendModelCache(type(cont_item), cont_item)

		# cache = self._cachedContItems()
		# index = self._findCachedContItem(cont_item)
		# if index == -1:
		# 	cache.append(cont_item)
		# else:
		# 	cache[index] = cont_item

	def _containCachedContItem(self, cont_item: 'BaseContItem') -> bool:
		"""
		是否包含某容器项（根据 ID 和 type 进行比较）
		Args:
			cont_item (BaseContItem): 容器项
		Returns:
			返回是否包含容器项
		"""
		return self._containsModelCache(type(cont_item), cont_item)
		# return self._findCachedContItem(cont_item) >= 0

	def _removeCachedContItem(self, cont_item: 'BaseContItem'):
		"""
		移除缓存容器项
		Args:
			cont_item (BaseContItem): 容器项
		"""
		self._removeModelCache(type(cont_item), cont_item)

		# cache = self._cachedContItems()
		# index = self._findCachedContItem(cont_item)
		# if index >= 0:
		# 	del cache[index]
		# 	self._getCache(self.REMOVED_CACHE_KEY).append(cont_item)

	# def _findCachedContItem(self, cont_item: 'BaseContItem') -> int:
	# 	"""
	# 	查找某容器项所在下标（根据 ID 和 type 进行比较）
	# 	Args:
	# 		cont_item (BaseContItem): 容器项
	# 	Returns:
	# 		返回容器项下标，找不到则返回 -1
	# 	"""
	# 	index = 0
	# 	cache = self._cachedContItems()
	# 	for item in cache:
	# 		if item == cont_item or (item.id == cont_item.id and
	# 								 type(item) == type(cont_item)): return index
	# 		index += 1
	# 
	# 	return -1

	# 获取指定物品数量或所有物品总数（占用格子数）
	def contItemCnt(self, *args, **kwargs):
		return len(self.contItems(*args, **kwargs))

	# 获取指定物品数量或所有物品总数（总数）
	def itemCnt(self, *args, **kwargs):
		return self.contItemCnt(*args, **kwargs)

	def hasItem(self, count, *args, **kwargs):
		return self.itemCnt(*args, **kwargs) >= count

	# 容器是否存在指定物品或任意物品指定数量

	# 获取一个容器项
	def contItem(self, *args, **kwargs):
		cont_items = self.contItems(*args, **kwargs)

		if isinstance(cont_items, QuerySet):
			if cont_items.exists(): return cont_items.first()
		else:
			if len(cont_items) > 0: return cont_items[0]

		return None

	# 创建一个容器项
	# cla: 直接提供一个容器项类型
	# cont_item: 从提供的已有容器项中复制
	def _createContItem(self, cla=None, cont_item=None, **kwargs):
		# 如果没有提供 cla
		if cla is None:
			# 如果有提供已有容器项，直接求出 cla
			if cont_item is not None:
				cla = type(cont_item)
			# 否则，计算出 cla
			else:
				cla = self.contItemClass(**kwargs)

		# 创建新容器项
		new_cont_item = cla.create(self, **kwargs)
		# new_cont_item.afterCreated()

		# 如果有参照的容器项，调用复制函数
		if cont_item is not None:
			new_cont_item.copy(cont_item)

		self._addCachedContItem(new_cont_item)

		return new_cont_item

	# 移除一个容器项
	def _removeContItem(self, cont_item, count=None):
		if count is not None:
			cont_item.leave(count)
		else:
			cont_item.remove()

		if cont_item.container is None:
			self._removeCachedContItem(cont_item)

	# 确保物品类型可接受
	def ensureItemAcceptable(self, **kwargs):
		if not self.isItemAcceptable(**kwargs):
			raise GameException(ErrorType.IncorrectItemType)

	# 确保包含容器项
	def ensureContItemContained(self, cont_item,
								error: ErrorType = ErrorType.QuantityInsufficient):
		if cont_item.container_id != self.id:
			raise GameException(error)

	# 准备转移
	def prepareTransfer(self, container, **kwargs) -> dict:
		return kwargs

	# 接受转移
	def acceptTransfer(self, container, **kwargs):
		pass  # self.ensureItemAcceptable(**kwargs)

	# 转移物品（从 self 转移到 container）
	def transfer(self, container, **kwargs):
		container.acceptTransfer(self, **self.prepareTransfer(container, **kwargs))

	def refresh(self):
		pass


# ===================================================
#  背包类容器
# ===================================================
class PackContainer(BaseContainer):

	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "背包类容器"

	# 容量（为0则不限）
	capacity = models.PositiveSmallIntegerField(default=0, verbose_name="容量")

	# 所接受的容器项类（单个，基类）
	@classmethod
	def baseContItemClass(cls) -> 'PackContItem':
		return PackContItem

	# 获取容器容量（0为无限）
	@classmethod
	def defaultCapacity(cls):
		return 0

	# 获取物品对应的容器项（创建容器项时候调用）
	@classmethod
	def contItemClass(cls, item: BaseItem, **kwargs):
		"""
		获取对应的容器项类（返回单个，创建容器项时候调用）
		可以根据参数获取实际的容器项类
		Args:
			item (BaseItem): 物品
			**kwargs (**dict): 子类重载参数
		Returns:
			返回容器项类
		"""
		if item is not None:
			# 返回该物品的对应容器项类型
			for cla in cls.acceptedContItemClasses():
				item_cla = cla.acceptedItemClass()
				if item == item_cla or isinstance(item, item_cla): return cla

		return super().contItemClass(**kwargs)

	# 转化容器项为 dict
	def _convertItemsToDict(self):
		return ModelUtils.objectsToDict(self.contItems(include_equipped=True))

	def _create(self, **kwargs):
		"""
		创建背包容器
		Args:
			**kwargs (**dict): 子类重载参数
		"""
		super()._create(**kwargs)
		self.capacity = self.defaultCapacity()

	def contItems(self, include_equipped: bool = False,
				  *args, **kwargs) -> list or QuerySet:
		"""
		获取满足一定条件的容器项（并载入缓存）
		支持条件获取和映射获取
		Args:
			include_equipped (bool): 包含装备
			*args (*tuple): 继承参数
			**kwargs (**dict): 查询参数
		Returns:
			返回满足条件的所有容器项
		"""
		if include_equipped:
			return super().contItems(*args, **kwargs)

		return super().contItems(equiped=False, *args, **kwargs)

	# def _contItemsFromDb(self, listed: bool = False, cla: type = None,
	# 					 **kwargs) -> list or QuerySet:
	# 	"""
	# 	从数据库获取所有的容器物项
	# 	Args:
	# 		listed (bool): 是否以列表形式返回
	# 		cla (type): 类型
	# 		**kwargs (**dict): 查询参数
	# 	Returns:
	# 		从数据库中读取的物品容器项数据表>
	# 		（若 listed 为 True，返回 list 类型列表，否则返回 QuerySet 对象
	# 	"""
	# 	return super()._contItemsFromDb(listed, cla, **kwargs)

	def owner(self):  # -> Player.Player:
		"""
		获取容器的持有者
		Returns:
			持有玩家
		"""
		raise NotImplementedError

	def isItemAcceptable(self, cont_item: 'PackContItem' = None,
						 item: BaseItem = None, **kwargs) -> bool:
		"""
		判断容器项/物品类型是否接受
		Args:
			cont_item (BaseContItem): 容器项
			item (BaseItem): 物品
			**kwargs (**dict): 子类重载参数
		Returns:
			返回该容器项/物品能否放入该容器
		"""
		if item is not None:
			clas = self.acceptedContItemClasses()
			for cla in clas:
				item_cla = cla.acceptedItemClass()
				if item == item_cla or isinstance(item, item_cla):
					return True

		if cont_item is not None:
			clas = self.acceptedContItemClasses()
			return cont_item in clas or isinstance(cont_item, clas)

		return False

	def getCapacity(self) -> int:
		"""
		获取容器容量
		Returns:
			容量
		"""
		return self.capacity

	# 获取指定物品数量或所有物品总数（占用格子数）
	# def contItemCnt(self, db: bool = False, cla: type = None,
	# 				include_equipped: bool = False, **kwargs):
	# 	"""
	# 	获取容器项数目
	# 	Args:
	# 		db (bool): 是否要从数据库中读取
	# 		cla (type): 容器项类型
	# 		include_equipped (bool): 是否包含装备
	# 		**kwargs (**dict): 查询参数
	# 	Returns:
	# 		返回符合条件的容器项数目
	# 	"""
	# 	cont_items = self.contItems(db=db, listed=True, cla=cla,
	# 								include_equipped=include_equipped, **kwargs)
	#
	# 	return len(cont_items)

	def itemCnt(self, *args, **kwargs) -> int:
		"""
		获取指定物品数量或所有物品总数（总数）
		Args:
			db (bool): 是否要从数据库中读取
			cla (type): 容器项类型
			include_equipped (bool): 是否包含装备
			**kwargs (**dict): 查询参数
		Returns:
			返回符合条件的物品数目
		"""
		cnt = 0

		cont_items = self.contItems(*args, **kwargs)
		for cont_item in cont_items: cnt += cont_item.count

		return cnt

	# def hasItem(self, count: int, db: bool = False, cla: type = None,
	# 			include_equipped: bool = False, **kwargs) -> bool:
	# 	"""
	# 	判断是否存在一定数量的物品
	# 	Args:
	# 		count (int): 数量
	# 		db (bool): 是否要从数据库中读取
	# 		cla (type): 容器项类型
	# 		include_equipped (bool): 是否包含装备
	# 		**kwargs (**dict): 查询参数
	# 	Returns:
	# 		返回是否存在一定数量的物品
	# 	"""
	# 	return self.itemCnt(db=db, cla=cla,
	# 						include_equipped=include_equipped, **kwargs) >= count

	# cnt = self.contItems(**kwargs).aggregate(sum=Sum('count'))['sum']

	# 获取一个容器项
	# def contItem(self, db: bool = False, listed: bool = False, cla: type = None,
	# 			 include_equipped: bool = False, **kwargs) -> 'PackContItem':
	# 	"""
	# 	获取满足一定条件的容器项（单个）
	# 	Args:
	# 		db (bool): 是否查找数据库
	# 		listed (bool): 是否以列表形式返回
	# 		cla (type): 类型
	# 		include_equipped (bool): 包含装备
	# 		**kwargs (dict): 查询参数
	# 	Returns:
	# 		返回满足条件的第一个容器项
	# 	"""
	#
	# 	if db and cla is None: return None
	#
	# 	cont_items = self.contItems(db=db, listed=listed, cla=cla,
	# 								include_equipped=include_equipped, **kwargs)
	#
	# 	if db and not listed:
	# 		if cont_items.exists(): return cont_items.first()
	# 	else:
	# 		if len(cont_items) > 0: return cont_items[0]
	# 	return None

	# 判断容器容量是否可用（格子数）
	def isCapacityAvailable(self, count=1):
		if count <= 0 or self.getCapacity() <= 0: return True
		return self.contItemCnt() + count <= self.getCapacity()

	# 背包是否已满（格子数满）
	def isContItemFull(self):
		return not self.isCapacityAvailable(1)

	# 是否可以获取指定数量的指定物品
	def isGainItemAvailable(self, item: BaseItem, count=1):
		# 最大叠加数量
		max_cnt = item.maxCount()  # self.contItemClass(item).maxCount()

		# 仅可以叠加一个，直接判断是否还有 count 个空位
		if max_cnt == 1:
			return self.isCapacityAvailable(count)

		# 可以无限叠加，判断是否存在对应的 ContItem，如果没有，判断是否还有1个空位
		if max_cnt <= 0:
			if self.contItemCnt(item_id=item.id) > 1: return True
			return self.isCapacityAvailable()

		# 如果可以叠加一定数目，需要继续计算
		delta_cnt = self.__calcDeltaCnt(item, max_cnt)

		# 如果 delta_cnt 足够存放 count 个物品
		if delta_cnt >= count: return True

		# 如果不够，继续计算需要的 BaseContItem 数目
		sum_cont_item_cnt = math.ceil((count - delta_cnt) / max_cnt)

		# 是否还剩下计算的数目的空位
		return self.isCapacityAvailable(sum_cont_item_cnt)

	# 中间计算步骤
	def __calcDeltaCnt(self, item, max_cnt):
		# from utils.test_utils import Common as TestUtils

		# 测试计算性能
		# TestUtils.start('calc delta_cnt')

		# 计算在不增加 BaseContItem 的情况下还可以获取物品的数量 delta_cnt
		# cont_item_cnt = self.contItemCnt(item_id=item.id)
		# sum_item_cnt = self.itemCnt(item_id=item.id)
		# delta_cnt = cont_item_cnt*max_cnt-sum_item_cnt

		# ori_items = self.contItems(item_id=item.id)
		# ori_items = ori_items.annotate(delta=max_cnt-F('count'))
		# delta_cnt = ori_items.aggregate(sum=Sum('delta'))['sum']

		delta_cnt = 0
		cont_items = self.contItems(item_id=item.id)
		for cont_item in cont_items:
			delta_cnt += max_cnt - cont_item.count

		# TestUtils.end('delta_cnt: %d' % delta_cnt)

		return delta_cnt

	# region EnsureFuncs

	# 确保持有特定数量的物品（格子数）
	def ensureHasItem(self, count, include_equipped: bool = False, **kwargs):
		if not self.hasItem(count, include_equipped=include_equipped, **kwargs):
			raise GameException(ErrorType.QuantityInsufficient)

	# 确保容器容量可用（格子数）
	def ensureCapacityAvailable(self, count=1):
		if not self.isCapacityAvailable(count):
			raise GameException(ErrorType.CapacityInsufficient)

	# 确保可以获取指定数量的指定物品
	def ensureGainItemAvailable(self, item: BaseItem, count=1):
		if not self.isGainItemAvailable(item, count):
			raise GameException(ErrorType.CapacityInsufficient)

	# 确保可以获取指定数量的指定物品
	def ensureGainItemsEnable(self, item: BaseItem, count=1):
		if count == 0: return
		if count < 0: return self.ensureLostItemsEnable(item, -count)

		self.ensureItemAcceptable(item=item)
		self.ensureGainItemAvailable(item, count)

	# 是否可以失去指定数量的指定物品
	def ensureLostItemsEnable(self, item: BaseItem, count=1):
		if count == 0: return
		if count < 0: return self.ensureGainItemsEnable(item, -count)

		self.ensureHasItem(count, item_id=item.id)

	# 确保可以获得指定容器项
	def ensureGainContItemEnable(self, cont_item):
		self.ensureItemAcceptable(cont_item=cont_item)
		self.ensureCapacityAvailable(1)

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
	# 对于背包类容器，需要传入 item, count 参数
	def _createContItem(self, cla: type = None, cont_item: 'PackContItem' = None,
						item: BaseItem = None, count: int = 0, **kwargs) -> ('PackContItem', int):
		"""
		创建一个容器项
		对于背包类容器，需要传入 item, count 参数
		Args:
			cla (type): 容器项类型
			cont_item (PackContItem): 背包类容器项
			item (BaseItem): 对应物品
			count (int): 移入数目
			**kwargs (**dict): 用于创建的参数
		Returns:
			新创建的一个容器项以及剩余数量
		"""
		cont_item: PackContItem = super()._createContItem(
			cla, cont_item, item=item, **kwargs)

		# 置入 count 个物品
		count = cont_item.enter(count)

		return cont_item, count

	def _increaseItems(self, item: BaseItem, count: int = 1, ensure: bool = True,
					   **kwargs) -> ('PackContItem', 'PackContItem'):
		"""
		增加物品
		Args:
			item (BaseItem): 要增加的物品
			count (int): 增加数量
			ensure (bool): 是否类型检查
			**kwargs (**dict): 用于创建容器项的参数
		Returns:
			返回修改过的容器项（包括现有容器项和新添加的容器项）数组
		"""
		# 初始化返回的数据
		old_items = []  # 旧的物品项数据
		new_items = []  # 新的物品项数据

		# 获取容器项的类型和最大叠加数量
		cla = self.contItemClass(item)
		max_cnt = item.maxCount()

		# 如果物品是可以叠加的，先处理
		if max_cnt > 1 or max_cnt == 0:
			# 获取当前未满叠加数量的容器项
			ori_items = self.contItems(item=item)

			# 如果有容量限制，获取未满的容器项
			if max_cnt > 1:
				ori_items = ModelUtils.filter(
					ori_items, lambda item: item.count < max_cnt)
			# ori_items = ori_items.filter(count__lt=max_cnt)

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
	def _decreaseItems(self, item: BaseItem, count: int = 1,
					   ensure: bool = True) -> ('PackContItem', 'PackContItem'):
		"""
		减少物品
		Args:
			item (BaseItem): 物品对象
			count (int): 减少个数
			ensure (bool): 是否类型检查
		Returns:
			返回修改过的容器项（包括现有容器项和新添加的容器项）数组
		"""
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
			split_item, count = self._splitItem(cont_item, count)

			self._removeContItem(split_item)
			# split_item.remove()

			# 记录修改过的容器项
			new_items.append(split_item)
			# 有了分解（容器项剩余数量不为0）
			if split_item != cont_item:
				old_items.append(cont_item)

			if count <= 0: break

		# 如果还有剩余，抛出数量不足异常
		if ensure and count > 0:
			raise GameException(ErrorType.QuantityInsufficient)

		# 更新删除的物品
		# cla.objects.bulk_update(old_items, ['count', 'container'])

		# 返回有修改过的项
		return old_items, new_items

	def _splitItem(self, cont_item: 'PackContItem', count: int) -> ('PackContItem', int):
		"""
		拆分物品
		Args:
			cont_item (PackContItem): 容器项
			count (int): 拆分数目
		Returns:
			返回拆分后的容器项和剩余数量
		"""
		ori_cnt = cont_item.count

		# 要拆分的数量大于等于持有数量，直接返回
		if count >= ori_cnt: return cont_item, count - ori_cnt

		# 移出 count 个物品
		self._removeContItem(cont_item, count)

		# 新建一个容器项，并返回
		return self._createContItem(None, cont_item,
									cont_item.item, count)

	# 组合一个物品（BaseContItem）
	# 返回：组合后的 BaseContItem, 剩余数量
	def _mergeItem(self, cont_items: list) -> list:
		"""
		合并容器项
		Args:
			cont_items (list): 待合并的容器项列表
		Returns:
			合并后的容器项数组
		"""
		cnt = len(cont_items)

		l = 0
		l_cont_item: PackContItem = cont_items[l]

		for i in range(cnt):
			cont_item: PackContItem = cont_items[i]

			if cont_item == l_cont_item: continue

			# 如果最左边的容器项满了
			if l_cont_item.isFull():
				# 更换到下一个
				l += 1
				l_cont_item = cont_items[l]

			# 进行合成
			l_cont_item.merge(cont_item)
		l += 1

		# if save:
		for i in range(cnt - l):
			self._removeContItem(cont_items[l + i])

		return cont_items[:l]

	# endregion

	# region Gain/LostAPIs

	def splitItem(self, cont_item: 'PackContItem', count: int) -> ('PackContItem', int):
		"""
		拆分一个容器项
		Args:
			cont_item (PackContItem): 容器项
			count (int): 拆分数目
		Returns:
			返回拆分后新的容器项和剩余数量
		"""

		self.ensureContItemContained(cont_item)
		self.ensureCapacityAvailable()

		return self._splitItem(cont_item, count)

	def mergeItem(self, cont_items: list) -> list:
		"""
		合并容器项
		Args:
			cont_items (list): 待合并的容器项数组
		Returns:
			返回合并后的容器项数组
		"""
		for cont_item in cont_items:
			self.ensureContItemContained(cont_item)

		return self._mergeItem(cont_items)

	def arrangeItems(self, cont_item: 'PackContItem' = None,
					 cont_items: list = None) -> list:
		"""
		整理背包
		Args:
			cont_item (PackContItem): 需要整理的容器项（类型）
			cont_items (list): 需要整理的容器项列表
		Returns:
			返回整理后的容器项列表
		"""
		if cont_items is None:
			cont_items = self.contItems()

		if cont_item is not None:
			item_data = cont_item.itemData()
			cont_items = ModelUtils.filter(cont_items,
										   lambda ct: ct.itemData() == item_data)

			self._mergeItem(cont_items)

		else:
			cont_item_dict = {}

			for cont_item_ in cont_items:
				key = cont_item_.itemData()
				if key not in cont_item_dict:
					cont_item_dict[key] = [cont_item_]
				else:
					cont_item_dict[key].append(cont_item_)

			for key in cont_item_dict:
				if len(cont_item_dict[key]) > 1:
					self._mergeItem(cont_item_dict[key])

	# return self.contItems()

	# 获得物品
	def gainItems(self, item: BaseItem, count: int = 1,
				  combine_return: bool = True) -> list or (list, list):
		"""
		获得物品
		Args:
			item (BaseItem): 物品类型
			count (int): 要获取的数量
			combine_return (bool): 是否组合返回
		Returns:
			返回修改过的容器项，若 combine_return 为 True，分别旧容器项和新容器项，否则新旧容器项组合为 list 一并返回
		"""
		if item is None or count == 0:
			if combine_return: return []
			return [], []

		if count < 0:
			return self.lostItems(item, -count, combine_return)

		self.ensureGainItemsEnable(item, count)

		old_items, new_items = self._increaseItems(item, count, False)

		# for new_item in new_items: new_item.afterCreated()

		if combine_return: return old_items + new_items

		return old_items, new_items

	# 失去物品
	def lostItems(self, item: BaseItem, count: int = 1,
				  combine_return: bool = True) -> list or (list, list):
		"""
		失去物品
		Args:
			item (BaseItem): 物品类型
			count (int): 要获取的数量
			combine_return (bool): 是否组合返回
		Returns:
			返回修改过的容器项，若 combine_return 为 True，分别旧容器项和新容器项，否则新旧容器项组合为 list 一并返回
		"""
		if item is None or count == 0:
			if combine_return: return []
			return [], []

		if count < 0:
			return self.gainItems(item, -count, combine_return)

		self.ensureLostItemsEnable(item, count)

		old_items, new_items = self._decreaseItems(item, count, False)

		if combine_return: return old_items + new_items

		return old_items, new_items

	def gainContItem(self, cont_item: 'PackContItem', fixed: bool = False,
					 combine_return: bool = True, **kwargs):
		"""
		获得一个容器项
		Args:
			cont_item (PackContItem): 容器项
			fixed (bool): 对固定容器项进行操作
			combine_return (bool): 是否组合返回
			**kwargs (**dict): 附加参数
		Returns:
			如果 fixed 为 True 或容器项无法叠加，返回添加的容器项，否则返回与 getItems 一致
		"""
		if cont_item is None: return None

		if fixed or cont_item.maxCount() == 1:
			self.ensureGainContItemEnable(cont_item)

			cont_item.transfer(self, **kwargs)

			self._addCachedContItem(cont_item)

			return cont_item
		else:
			res = self.gainItems(cont_item.item, cont_item.count, combine_return)

			self._removeContItem(cont_item)

			return res

	def lostContItem(self, cont_item: 'PackContItem', count=None,
					 fixed: bool = True, combine_return: bool = True):
		"""
		失去一个容器项
		Args:
			cont_item (PackContItem): 容器项
			count (int): 数量
			fixed (bool): 对固定容器项进行操作
			combine_return (bool): 是否组合返回
		Returns:
			如果容 fixed 为 True 或容器项无法叠加，返回失去的容器项，否则返回与 lostItems 一致
		"""
		if cont_item is None: return None

		self.ensureLostContItemEnable(cont_item)

		if fixed or cont_item.maxCount() == 1:
			self.ensureLostContItemEnable(cont_item)

			if count is not None:
				cont_item, _ = self._splitItem(cont_item, count)

			self._removeContItem(cont_item)

			# self._addCachedContItem(cont_item)

			return cont_item
		else:
			if count is None: count = cont_item.count

			return self.lostItems(cont_item.item, count, combine_return)

	# if count is not None:
	# 	cont_item, _ = self._splitItem(cont_item, count)
	#
	# self._removeContItem(cont_item)

	def gainContItems(self, cont_items: list, fixed: bool = False,
					  combine_return: bool = True, **kwargs):
		"""
		获得多个容器项
		Args:
			cont_items (list): 容器项列表
			fixed (bool): 对固定容器项进行操作
			combine_return (bool): 是否组合返回
			**kwargs (**dict): 附加参数
		Returns:
			返回多次 gainContItem 的结果
		"""
		new_cont_items = []

		for cont_item in cont_items:
			new_cont_items.append(self.gainContItem(
				cont_item, fixed, combine_return, **kwargs))

		return new_cont_items

	def lostContItems(self, cont_items: list, counts: list = None,
					  fixed: bool = True, combine_return: bool = True):
		"""
		失去多个容器项
		Args:
			cont_items (list): 容器项列表
			counts (list): 每个容器项对应失去的数目
			fixed (bool): 对固定容器项进行操作
			combine_return (bool): 是否组合返回
		Returns:
			返回多次 lostContItem 的结果
		"""
		new_cont_items = []

		for i in range(len(cont_items)):
			count = None
			cont_item = cont_items[i]
			if counts is not None and i < len(counts):
				count = counts[i]

			new_cont_items.append(self.lostContItem(
				cont_item, count, fixed, combine_return))

		return new_cont_items

	def refreshContItems(self):
		"""
		刷新所有容器项
		"""
		for cont_item in self.contItems():
			self.refreshContItem(cont_item)

	def refreshContItem(self, cont_item: 'PackContItem'):
		"""
		刷新容器项
		Args:
			cont_item (PackContItem): 容器项
		"""
		if cont_item.container is None or cont_item.container.id != self.id:
			self._removeCachedContItem(cont_item)

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
	def prepareTransfer(self, container, cont_item=None, cont_items=None,
						item=None, count=None, counts=None, **kwargs) -> dict:
		res = kwargs

		if isinstance(container, SlotContainer):
			if cont_item is not None:
				res['cont_item'] = cont_item
			elif cont_items is not None:
				res['cont_items'] = cont_items
		elif isinstance(container, PackContainer):
			if item is not None:
				_, res['cont_items'] = self.lostItems(item, count, True)
			elif cont_item is not None:
				res['cont_item'] = self.lostContItem(cont_item, count)
			elif cont_items is not None:
				res['cont_items'] = self.lostContItems(cont_items, counts)

		return res

	# 接受转移
	def acceptTransfer(self, container, cont_items=None,
					   cont_item=None, fixed: bool = False, **kwargs):

		# 如果物品是槽容器项，不进行后续操作（直接设置equiped即可）
		if isinstance(container, SlotContainer): return

		if cont_item is not None:
			self.gainContItem(cont_item, fixed=fixed)
		if cont_items is not None:
			self.gainContItems(cont_items, fixed=fixed)

	# endregion

	"""占位"""


# ===================================================
#  槽类容器
# ===================================================
class SlotContainer(BaseContainer):
	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "槽类容器"

	CONTAINERS_CACHE_KEY = 'equip_container%d'

	ACCEPTED_SLOTITEM_CLASS: 'SlotContItem' = None

	# 装备容器1
	# equip_container1 = models.ForeignKey('PackContainer', null=True, blank=True, related_name='equip_container1',
	# 								   on_delete=models.CASCADE, verbose_name="装备容器1")

	# 装备容器2
	# equip_container2 = models.ForeignKey('PackContainer', null=True, blank=True, related_name='equip_container2',
	# 								   on_delete=models.CASCADE, verbose_name="装备容器2")

	# 默认容器容量（0为无限）
	@classmethod
	def defaultCapacity(cls):
		return 1

	# 创建实例（创建槽）
	@classmethod
	def create(cls, **kwargs):
		container: SlotContainer = super().create(**kwargs)
		container.createSlots(**kwargs)
		return container

	# 所接受的槽项类（用于 contItemClass ）
	@classmethod
	def acceptedSlotItemClass(cls) -> 'SlotContItem':
		return cls.ACCEPTED_SLOTITEM_CLASS

	# 所接受的装备项类（用于 contItemClass ）
	@classmethod
	def acceptedContItemClasses(cls):
		return cls.ACCEPTED_CONTITEM_CLASSES or \
			   cls.acceptedSlotItemClass().acceptedEquipItemClasses()

	# 所接受的装备项基类（由于重载了 contItemClass，该函数意义有改变）
	# @classmethod
	# def baseContItemClass(cls):
	# 	return PackContItem

	# def __init__(self, *args, **kwargs):
	# 	super().__init__(*args, **kwargs)
	# 	self.cache(self.CONTAINERS_CACHE_KEY, [])

	# 持有者
	def owner(self):
		raise NotImplementedError

	# 获取对应的容器项（创建容器项时候调用）
	def contItemClass(self, **kwargs):
		return self.acceptedSlotItemClass()

	# 装备容器类
	def equipContainerClass(self, index):

		pack_item_cla: PackContItem = self.acceptedContItemClasses()[index]
		return pack_item_cla.containerClass()

	# 装备容器实例
	def equipContainer(self, index):

		return self.exactlyPlayer().getContainer(
			self.equipContainerClass(index))

		# key = self.CONTAINERS_CACHE_KEY % index
		#
		# return self._getOrSetCache(key, lambda: self._equipContainer(index))

	# def _equipContainer(self, index: int) -> PackContainer:
	# 	"""
	# 	获取指定装备ID的所属容器
	# 	Args:
	# 		index (int): 装备ID
	# 	Returns:
	# 		指定装备ID的所属容器项
	# 	"""
	# 	raise NotImplementedError

	# 物品类型是否接受
	# slot_item: 槽项
	# cont_item: 装备项
	def isItemAcceptable(self, slot_item: 'SlotContItem' = None,
						 cont_item: 'PackContItem' = None, **kwargs):
		"""
		判断容器项/槽项类型是否接受
		Args:
			slot_item (SlotContItem): 容器项（槽项）
			cont_item (PackContItem): 装备项（背包容器项）
			**kwargs (**dict): 子类重载参数
		Returns:
			返回该容器项/槽项能否放入该容器
		"""
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
			self._createSlot(cla, i, **kwargs)

	# slot.afterCreated(**kwargs)

	# cla.objects.bulk_create(slot_items)

	# 创建一个槽
	def _createSlot(self, cla, index, **kwargs):
		return self._createContItem(cla, index=index + 1, **kwargs)

	# slot_item: SlotContItem = self._createContItem(cla, **kwargs)
	# slot_item.setupIndex(index+1, **kwargs)
	# return slot_item

	def ensureEquipCondition(self, slot_item: 'SlotContItem', equip_item: 'PackContItem'):
		"""
		确保满足装备条件
		Args:
			slot_item (SlotContItem): 装备槽项
			equip_item (PackContItem): 装备项
		"""
		self.ensureItemAcceptable(cont_item=equip_item)

	# 保证满足装备卸下条件
	def ensureDequipCondition(self, slot_item: 'SlotContItem', type: type = None,
							  equip_index: int = None):
		"""
		保证满足装备卸下条件
		Args:
			slot_item ():
			type ():
			equip_index ():

		Returns:

		"""
		pass

	# 设置装备（装备/卸下）
	def setEquip(self, slot_item: 'SlotContItem' = None,
				 equip_item: 'PackContItem' = None, type_: type = None,
				 equip_index: int = None, force: bool = False, **kwargs):
		"""
		设置装备
		Args:
			slot_item (SlotContItem): 槽项
			equip_item (PackContItem): 装备项
			type_ (type): 装备类型
			equip_index (int): 装备索引
			force (bool): 是否强制装备（不改变背包物品）
			**kwargs (**dict): 其他用于查询槽项的参数
		Returns:
			返回对应容器实例以及原始装备实例
		"""

		# 强制装备（替换原有装备）
		if force:
			if equip_item is None:
				return self._dequipSlot(slot_item=slot_item, type=type_,
										equip_index=equip_index, **kwargs)
			else:
				return self._equipSlot(slot_item=slot_item, equip_item=equip_item,
									   equip_index=equip_index, **kwargs)

		if slot_item is not None:
			self.ensureContItemContained(slot_item, ErrorType.InvalidContItem)

		# 获取槽项
		if slot_item is None:
			slot_item: SlotContItem = self.contItem(**kwargs)

		# 找不到槽项，抛出异常
		if slot_item is None:
			raise GameException(ErrorType.SlotItemNotFound)

		# 获取装备索引
		if equip_index is None:
			equip_index = slot_item.getEquipItemIndex(equip_item=equip_item, type_=type_)

		# 找不到装备索引，抛出异常
		if equip_index is None:
			raise GameException(ErrorType.EquipIndexNotFound)

		# 获取容器
		container: PackContainer = self.equipContainer(equip_index)

		# 找不到所属容器，抛出异常
		if container is None:
			raise GameException(ErrorType.ContainerNotFound)

		old_equip_item = slot_item.equipItem(equip_index)

		# 如果装备前和装备后的装备都是空
		if old_equip_item is None and equip_item is None:
			return container, None

		if equip_item is None:
			# 如果要装备的装备为空，则相当于卸下，需要确保容器有位置
			container.ensureCapacityAvailable()
		# 其他情况下都不需要判断

		self._dequipSlot(slot_item, equip_index=equip_index)

		# self.transfer(container, slot_item=slot_item, index=equip_index)

		# 装备不为空
		if equip_item is not None:
			self._equipSlot(slot_item, equip_index, equip_item)
			# container.transfer(self, slot_item=slot_item, cont_item=equip_item)

		return container, old_equip_item

	# 槽装备（强制装备）
	# 可以传入 index 参数指定槽的索引
	def _equipSlot(self, slot_item=None, equip_index=None, equip_item=None, **kwargs):

		if slot_item is not None:
			self.ensureContItemContained(slot_item, ErrorType.InvalidContItem)

		if slot_item is None:
			slot_item = self.contItem(**kwargs)

		if slot_item is None: return None

		slot_item: SlotContItem = slot_item
		self.ensureEquipCondition(slot_item, equip_item)

		equip_item = slot_item.equip(equip_item, equip_index)
		
		return equip_item

	# slot_item.save()

	# if equip_item is not None:
	# 	equip_item.save()

	# 槽卸下（强制卸下）
	def _dequipSlot(self, slot_item=None, type=None, equip_index=None, **kwargs):

		if slot_item is not None:
			self.ensureContItemContained(slot_item, ErrorType.InvalidContItem)

		if slot_item is None:
			slot_item = self.contItem(**kwargs)

		if slot_item is None: return None

		slot_item: SlotContItem = slot_item
		self.ensureDequipCondition(slot_item, type, equip_index)

		equip_item = slot_item.dequip(type, equip_index)

		return equip_item

	# 接受转移
	def acceptTransfer(self, container, slot_item=None, cont_item=None, **kwargs):
		self._equipSlot(slot_item, equip_item=cont_item, **kwargs)

	# 准备转移
	def prepareTransfer(self, container, slot_item=None, type=None, index=None, **kwargs) -> dict:
		res = kwargs

		res['cont_item'] = self._dequipSlot(slot_item, type, index, **kwargs)

		return res


# endregion

# region 容器项


# ===================================================
#  基本容器项表
# ===================================================
class BaseContItem(CacheableModel):
	class Meta:
		abstract = True
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

	CONTAINER_CLA: BaseContainer = None

	# 容器
	# container = models.ForeignKey('BaseContainer', on_delete=models.CASCADE,
	# 							  null=True, blank=True, verbose_name="容器")

	# 容器
	container = None

	# 容器标识类型
	# type = models.PositiveSmallIntegerField(default=ContItemType.Unset.value,
	# 										choices=TYPES, verbose_name="容器类型")

	# region 类型配置

	def __str__(self):
		# name = type(self).__name__
		name = self._meta.verbose_name
		owner = self.container.owner() if self.container else None
		return '%d %s(%s)' % (self.id, name, str(owner))

	def __getattr__(self, item):
		raise AttributeError(item)

	# 转化为 dict
	def convert(self, **kwargs):
		return {
			'id': self.id,
			'type': self.TYPE.value
		}

	# endregion

	# region 配置项

	# 所属容器的类
	@classmethod
	def containerClass(cls):
		return cls.CONTAINER_CLA

	# endregion

	# region 创建容器项

	# 创建容器项
	@classmethod
	def create(cls, container: BaseContainer, **kwargs) -> 'BaseContItem':
		"""
		创建容器项
		Args:
			container (BaseContainer): 容器
			**kwargs (**dict): 拓展创建参数
		Returns:
			返回创建的容器项
		"""
		cont_item = cls()
		cont_item._create(container, **kwargs)
		cont_item.save(judge=False)
		cont_item.afterCreated()
		return cont_item

	# 创建容器项（包含移动）
	def _create(self, container: BaseContainer, **kwargs):
		"""
		创建容器项（内部处理）
		Args:
			container (BaseContainer): 所属容器
			**kwargs (**dict): 拓展参数
		"""
		# self.ensureItemAcceptable(**kwargs)
		self.transfer(container, **kwargs)

	# 创建之后调用
	def afterCreated(self, **kwargs):
		"""
		创建后回调
		Args:
			**kwargs (**dict): 子类重载参数
		"""
		pass

	# endregion

	# 最大叠加数量
	def maxCount(self) -> int:
		"""
		最大叠加数量
		Returns:
			返回容器项的最大叠加数量
		"""
		return 1

	# region 容器项操作

	# region 复制操作

	def isEqual(self, cont_item: 'BaseContItem') -> bool:
		"""
		比较两个容器项是否相等
		Args:
			cont_item (BaseContItem): 另一个容器项
		Returns:
			返回两者是否相等
		"""
		return type(self) == type(cont_item)

	def ensureContItemCopyable(self, cont_item: 'BaseContItem'):
		"""
		保证容器项可以复制
		Args:
			cont_item (BaseContItem): 源容器项
		Raises:
			ErrorType.IncorrectContItemType: 两者容器项类型不匹配（不可复制）
		"""
		if type(self) != type(cont_item):
			raise GameException(ErrorType.IncorrectContItemType)

	def ensureContItemEqual(self, cont_item: 'BaseContItem'):
		"""
		保证两者容器项相等
		Args:
			cont_item (BaseContItem): 另一个容器项
		Raises:
			ErrorType.IncorrectContItemType: 两者容器项类型不匹配（不可复制）
		"""
		if not self.isEqual(cont_item):
			raise GameException(ErrorType.IncorrectContItemType)

	def copy(self, cont_item: 'BaseContItem'):
		"""
		复制容器项（复制到当前对象中）
		Args:
			cont_item (BaseContItem): 要复制的容器项
		"""
		self.ensureContItemCopyable(cont_item)

	# endregion

	# region 转移操作

	def transfer(self, container: BaseContainer, **kwargs):
		"""
		转移容器项（移动到指定的 Container）
		Args:
			container (BaseContainer): 目标容器
			**kwargs (**dict): 附加参数
		"""
		self.container = container
		self.refresh()

	def remove(self):
		"""
		移除容器项（从当前容器中移除）
		"""
		self.container = None
		self.refresh()

	# endregion

	# region 功能操作

	def isContItemUsable(self, occasion: ItemUseOccasion, **kwargs) -> bool:
		"""
		配置当前物品是否可用
		Args:
			occasion (ItemUseOccasion): 使用场合枚举
			**kwargs (**dict): 拓展参数
		Returns:
			返回当前物品是否可用
		"""
		return False

	def ensureContItemUsable(self, occasion: ItemUseOccasion, **kwargs):
		"""
		确保物品可用
		Args:
			occasion (ItemUseOccasion): 使用场合枚举
			**kwargs (**dict): 拓展参数
		Raises:
			ErrorType.UnusableItem: 该类型物品不可用
		"""
		if not self.isContItemUsable(occasion, **kwargs):
			raise GameException(ErrorType.UnusableItem)

	def useItem(self, occasion: ItemUseOccasion, **kwargs):
		"""
		使用物品
		Args:
			occasion (ItemUseOccasion): 使用场合枚举
			**kwargs (**dict): 拓展参数
		"""
		self.ensureContItemUsable(occasion, **kwargs)

	# endregion

	# region 数据获取

	# 实际的容器
	def exactlyContainer(self):
		return self.exactlyPlayer().getContainer(container=self.container)

	# 实际的容器
	def exactlyPlayer(self):
		return self.container.exactlyPlayer()

	def getContainer(self, cla: BaseContainer):
		attr_name = cla.__name__.lower()

		try: container = getattr(self, attr_name)
		except cla.DoesNotExist: return None

		return self.exactlyPlayer().getContainer(container=container)

	# endregion

	def refresh(self):
		"""
		刷新容器项
		"""
		container = self.exactlyContainer()

		if container is not None: container.refresh()

	# endregion

	def save(self, judge=True, **kwargs):
		if judge and self.container is None:
			self.delete_save = False  # 防止重复保存
			if self.id is not None: self.delete()
		else:
			super().save(**kwargs)


# ===================================================
#  背包类容器项表
# ===================================================
class PackContItem(BaseContItem):
	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "背包类容器项"

	ACCEPTED_ITEM_CLASS: BaseItem = None

	# 物品
	item = None  # models.ForeignKey('BaseItem', on_delete=models.CASCADE, verbose_name="物品")

	# 叠加数量
	count = models.PositiveSmallIntegerField(default=0, verbose_name="叠加数量")

	# 装备标志
	equiped = models.BooleanField(default=False, verbose_name="是否装备中")

	# region 配置项

	# 所接受的物品类
	@classmethod
	def acceptedItemClass(cls):
		return cls.ACCEPTED_ITEM_CLASS

	# endregion

	def __str__(self):
		return '%s (%s :%d)' % (
			super().__str__(), self.item, self.count)

	# 转化为 dict
	def convert(self, **kwargs):
		res = super().convert(**kwargs)

		item_id = ModelUtils.objectToId(self.item)

		res['item_id'] = item_id
		res['count'] = self.count
		res['equiped'] = self.equiped

		return res

	# region 创建容器项

	def _create(self, container: BaseContainer, item: BaseItem = None, **kwargs):
		"""
		创建容器项（内部处理）
		Args:
			container (BaseContainer): 所属容器
			item (BaseItem): 承载道具
			**kwargs (**dict): 拓展参数
		"""
		super()._create(container, **kwargs)
		self.setItem(item)

	def setItem(self, item: BaseItem):
		"""
		设置物品
		Args:
			item (BaseItem): 物品对象
		"""
		self.ensureItemAcceptable(item)

		self.item = item

		self.refresh()

	def itemData(self):
		"""
		类型数据（用于唯一标识一类容器项）
		Returns:
			返回类型数据
		"""
		return type(self), self.item_id

	def isItemAcceptable(self, item: BaseItem):
		"""
		物品是否接受
		Args:
			item (BaseItem): 物品
		Returns:
			返回物品是否接受
		"""
		if item is not None:
			return isinstance(item, self.acceptedItemClass())

		return False

	def ensureItemAcceptable(self, item: BaseItem):
		"""
		确保物品可以接受
		Args:
			item (BaseItem): 物品对象
		Raises:
			ErrorType.IncorrectItemType: 不正确的物品类型
		"""
		if not self.isItemAcceptable(item):
			raise GameException(ErrorType.IncorrectItemType)

	# endregion

	# region 容量判断

	def maxCount(self) -> int:
		"""
		最大叠加数量
		Returns:
			返回容器项的最大叠加数量
		"""
		if self.item is not None:
			return self.item.maxCount()

		return 1

	def isEqual(self, cont_item: 'PackContItem') -> bool:
		"""
		比较两个容器项是否相等
		Args:
			cont_item (PackContItem): 另一个容器项
		Returns:
			返回两者是否相等
		"""
		return super().isEqual(cont_item) and self.item_id == cont_item.item_id

	def isFull(self) -> bool:
		"""
		判断容器项是否已满
		Returns:
			返回该容器项是否已满
		"""
		return self.count >= self.maxCount()

	# endregion

	# region 容器项操作

	# region 转移操作

	def transfer(self, container: BaseContainer, **kwargs):
		"""
		转移容器项（移动到指定的 Container）
		Args:
			container (BaseContainer): 目标容器
			**kwargs (**dict): 附加参数
		"""
		super().transfer(container, **kwargs)
		self.equiped = False

	def copy(self, cont_item: 'PackContItem'):
		"""
		复制容器项（复制到当前对象中）
		Args:
			cont_item (PackContItem): 要复制的容器项
		"""
		super().copy(cont_item)
		self.item = cont_item.item

	def merge(self, cont_item: 'PackContItem'):
		"""
		组合两个容器项（将另一个容器项组合到当前容器项）
		若当前容器项不足以容纳，则剩余数量将保留在原来的容器项中
		Args:
			cont_item (PackContItem): 另一个容器项
		"""
		self.ensureContItemEqual(cont_item)

		count = self.enter(cont_item.count)
		cont_item.count = count

	def enter(self, count: int = 0) -> int:
		"""
		加入容器
		Args:
			count (int): 移出数量
		Returns:
			返回剩余无法添加的数量
		"""
		if count <= 0: return 0
		if self.item is None or self.container is None:
			return count  # 未设置物品和容器

		self.count += count

		self.refresh()

		max_count = self.maxCount()
		# 如果有最大叠加限制
		if max_count > 0:
			delta = self.count - max_count
			self.count = min(max_count, self.count)
			return max(0, delta)  # 返回剩余无法添加的数量

		return 0

	def leave(self, count: int = 0) -> int:
		"""
		移出容器
		Args:
			count (int): 移出数量
		Returns:
			返回剩余无法移出的数量
		"""
		if count <= 0: return 0
		if self.item is None or self.container is None:
			return count  # 未设置物品和容器

		self.count -= count

		self.refresh()

		# 如果数量为 0
		if self.count <= 0:
			self.remove()
			delta = -self.count
			self.count = max(0, self.count)
			return max(0, delta)  # 返回剩余无法删除的数量

		return 0

	# endregion

	# region 功能操作

	def ensureContItemUsable(self, occasion: ItemUseOccasion,
							 count: int = 1, **kwargs):
		"""
		确保物品可用
		Args:
			occasion (ItemUseOccasion): 使用场合枚举
			count (int): 使用数量
			**kwargs (**dict): 拓展参数
		Raises:
			ErrorType.QuantityInsufficient: 物品数量不足
		"""
		if isinstance(self.item, UsableItem):
			self.item.ensureUsable(occasion, count, **kwargs)

		super().ensureContItemUsable(occasion, **kwargs)

		if self.count < count:
			raise GameException(ErrorType.QuantityInsufficient)

	def useItem(self, occasion: ItemUseOccasion, count: int = 1, **kwargs):
		"""
		使用物品
		Args:
			occasion (ItemUseOccasion): 使用场合枚举
			count (int): 使用数量
			**kwargs (**dict): 拓展参数
		"""
		super().useItem(occasion, count=count, **kwargs)

		self.leave(count)

	def equip(self):
		"""
		装备容器项（设置 equip）
		"""
		self.equiped = True

	def dequip(self):
		"""
		卸下容器项（设置 equip）
		"""
		self.equiped = False

	# endregion

	# endregion

	def save(self, judge=True, **kwargs):
		# 容器为空且未装备
		if judge and self.container is None and not self.equiped:
			self.delete_save = False  # 防止重复保存
			if self.id is not None: self.delete()
		else:
			super().save(False, **kwargs)


# ===================================================
#  槽类容器项表
# ===================================================
class SlotContItem(BaseContItem):
	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "槽类容器项"

	EQUIPS_CACHE_KEY = 'equip%d'

	REMOVED_CACHE_KEY = 'removed'

	ACCEPTED_EQUIP_ITEM_CLASSES: list = []

	ACCEPTED_EQUIP_ITEM_ATTRS: list = []

	# 槽编号（从一开始）
	index = models.PositiveSmallIntegerField(default=0, verbose_name="槽编号")

	# 装备项必须是 SET_NULL ！！！！！！
	# 装备项1
	# equip_item1 = models.OneToOneField('PackContItem', null=True, blank=True, related_name='equip_item1',
	# 								   on_delete=models.CASCADE, verbose_name="装备项1")

	# 装备项2
	# equip_item2 = models.OneToOneField('PackContItem', null=True, blank=True, related_name='equip_item2',
	# 								   on_delete=models.CASCADE, verbose_name="装备项2")

	# region 配置项

	# 所接受的装备项类（可多个）
	@classmethod
	def acceptedEquipItemClasses(cls):
		return cls.ACCEPTED_EQUIP_ITEM_CLASSES

	# 所接受的装备项属性名（可多个）
	@classmethod
	def acceptedEquipItemAttrs(cls):
		return cls.ACCEPTED_EQUIP_ITEM_ATTRS

	# 装备数目
	@classmethod
	def equipCount(cls):
		return len(cls.acceptedEquipItemClasses())

	# endregion

	# def __init__(self, *args, **kwargs):
	# 	super().__init__(*args, **kwargs)
		# if self.type == ContainerType.Unset.value:
		# 	self.type = self.TYPE.value
		# self._cache(self.REMOVED_CACHE_KEY, [])

	def __str__(self):
		equip_cnt = self.equipCount()

		equip_str = ""
		for i in range(equip_cnt):
			equip_item = self.equipItem(i)
			item = equip_item.item if equip_item else None
			equip_str += " " + str(item)

		return '%s (%d: %s)' % (
			super().__str__(), self.index, equip_str)

	# 用于获取装备
	# def __getattr__(self, item):
	# 	equip_cnt = self.equipCount()
	# 	attrs = self.acceptedEquipItemAttr()
	#
	# 	for i in range(equip_cnt):
	# 		if item == attrs[i]: return self.equipItem(i)
	#
	# 	return super().__getattr__(item)

	# 转化装备项为 dict
	def _convertEquipToDict(self, res):
		equip_cnt = self.equipCount()
		attrs = self.acceptedEquipItemAttrs()

		for i in range(equip_cnt):
			res[attrs[i] + '_id'] = self._equipItemId(i)

	# 转化为 dict
	def convert(self, **kwargs):
		res = super().convert(**kwargs)

		res['index'] = self.index

		self._convertEquipToDict(res)

		return res

	def _setupCustomCacheItems(self):
		super()._setupCustomCacheItems()

		clas = self.acceptedEquipItemClasses()

		for i in range(len(clas)):
			self._setModelCache(key=clas[i],
								reload_func=lambda: self._equipItem(i))

	# region 创建容器项

	# 创建容器项
	def _create(self, container: BaseContainer, index: int = None, **kwargs):
		"""
		创建槽项
		Args:
			container (BaseContainer): 所属容器
			index (int): 槽索引
			**kwargs (**dict): 其他参数（用于 setupIndex）
		"""
		super()._create(container, **kwargs)
		self.setupIndex(index, **kwargs)

	# 配置索引
	def setupIndex(self, index: int, **kwargs):
		"""
		设置槽索引
		Args:
			index (int): 槽索引
			**kwargs (**dict): 拓展参数
		"""
		self.index = index

	# endregion

	# region 装备项操作

	def equipItem(self, index) -> PackContItem:
		"""
		获取装备项（缓存）
		Args:
			index (int): 装备项索引
		Returns:
			返回指定索引的装备项
		"""
		cla = self.acceptedEquipItemClasses()[index]
		return self._firstModelCache(cla)

	def _equipItem(self, index) -> PackContItem:
		"""
		获取装备项（缓存）
		Args:
			index (int): 装备项索引
		Returns:
			返回指定索引的装备项
		"""
		# 获取装备项的属性名称
		attr = self.acceptedEquipItemAttrs()[index]
		# 判断并获取属性（必须为 Django model 中的外键）
		if hasattr(self, attr):
			obj: PackContItem = getattr(self, attr)
			if obj is None: return None

			return self.exactlyPlayer().getContItem(
				cont_item=obj, include_equipped=True)

		return None

	def _equipItemId(self, index) -> PackContItem:
		"""
		获取数据库中的装备项ID
		Args:
			index (int): 装备项索引
		Returns:
			返回指定索引的实际装备项ID（若找不到返回 0）
		"""
		# 获取装备项的属性名称
		attr = self.acceptedEquipItemAttrs()[index]
		# 判断并获取属性（必须为 Django model 中的外键）
		if hasattr(self, attr):
			obj: PackContainer = getattr(self, attr)
			if obj is None: return 0
			return obj.id

		return 0

	def _reloadEquipItem(self, index):
		"""
		刷新装备项
		Args:
			index (int): 装备项索引
		"""
		cla = self.acceptedEquipItemClasses()[index]
		self._reloadModelCache(cla)

	# def _setEquipItemCache(self, index: int, equip_item: PackContItem):
	# 	"""
	# 	设置装备项缓存
	# 	Args:
	# 		index (int): 装备项索引
	# 		equip_item (PackContItem): 装备项
	# 	"""
	# 	key = self.EQUIPS_CACHE_KEY % index
	# 	self._cache(key, equip_item)
	#
	# 	self._setEquipItem(index, equip_item)
	#
	# def _removeEquipItemCache(self, index):
	# 	"""
	# 	移除装备项缓存
	# 	Args:
	# 		index (int): 装备项索引
	# 	"""
	# 	key = self.EQUIPS_CACHE_KEY % index
	# 	equip_item = self.equipItem(index)
	#
	# 	if equip_item is not None:
	# 		self._getCache(self.REMOVED_CACHE_KEY).append(equip_item)
	# 		self._cache(key, None)
	#
	# 	self._setEquipItem(index, None)
	#
	# 	return equip_item

	# 设置装备项
	def _setEquipItem(self, index: int, equip_item: PackContItem):
		"""
		设置装备项
		Args:
			index (int): 装备项索引
			equip_item (PackContItem): 装备项
		Returns:
			返回原来的装备项
		"""
		old_equip_item = self.equipItem(index)

		if old_equip_item is not None:
			old_equip_item.dequip()

		# 获取装备项的属性名称
		attr = self.acceptedEquipItemAttrs()[index]
		# 设置属性（必须为 Django model 中的外键）
		setattr(self, attr, equip_item)
		equip_item.equip()

		self._reloadEquipItem(index)

		return old_equip_item

	# 移除装备项
	def _removeEquipItem(self, index: int):
		"""
		移除装备项
		Args:
			index (int): 装备项索引
		Returns:
			返回原来的装备项
		"""
		# cla = self.acceptedEquipItemClasses()[index]
		# self._clearModelCache(cla)
		#
		return self._setEquipItem(index, None)

	# 获取装备项索引
	def getEquipItemIndex(self, equip_item: PackContItem = None, type_: type = None) -> int:
		"""
		根据装备项对象或者类型获取对应装备项的索引
		Args:
			equip_item (PackContItem): 装备项对象
			type_ (type): 装备项类型
		Returns:
			返回对应装备项索引
		"""
		equip_cnt = self.equipCount()
		classes = self.acceptedEquipItemClasses()

		# 如果传入 equip_item，直接获取其类型
		if equip_item is not None: type_ = type(equip_item)

		# 找到装备项索引
		for i in range(equip_cnt):
			if type_ == classes[i]: return i

		return None

	# endregion

	# region 容器项操作

	# region 复制操作

	# 比较容器项
	def isEqual(self, cont_item: 'SlotContItem') -> bool:
		"""
		比较两个容器项是否相等
		Args:
			cont_item (SlotContItem): 另一个容器项
		Returns:
			返回两者是否相等
		"""
		return self == cont_item

	# 复制容器项（不可复制）
	def copy(self, cont_item: 'SlotContItem'):
		"""
		复制容器项（无法复制）
		Args:
			cont_item (SlotContItem): 要复制的容器项
		Raises:
			ErrorType.IncorrectContItemType: 无法复制
		"""
		raise GameException(ErrorType.IncorrectContItemType)

	# endregion

	# region 功能操作

	def equip(self, equip_item: PackContItem, index: int = None):
		"""
		装备装备（强制）
		Args:
			index (int): 装备类型索引
			equip_item (PackContItem): 装备项
		"""
		if index is None:
			index = self.getEquipItemIndex(equip_item=equip_item)

		if index is None: return None

		old_equip_item = self._setEquipItem(index, equip_item)

		self.refresh()

		return old_equip_item

	def dequip(self, type_: type = None, index: int = None):
		"""
		卸下装备（强制）
		Args:
			type_ (type): 装备类型（类）
			index (int): 装备类型索引
		"""
		if type_ is not None:
			index = self.getEquipItemIndex(type_=type_)

		return self.equip(None, index)
		#
		# if index is None: return None
		#
		# old_equip_item = self._removeEquipItem(index)
		#
		# self.refresh()
		#
		# return old_equip_item

	def ensureContItemUsable(self, occasion: ItemUseOccasion,
							 index: int = 0, type_: type = None,
							 count: int = 1, **kwargs):
		"""
		确保物品可用
		Args:
			occasion (ItemUseOccasion): 使用场合枚举
			index (int): 装备类型索引
			type_ (type): 装备类型（类）
			count (int): 使用数量
			**kwargs (**dict): 拓展参数
		"""
		if type_ is not None:
			index = self.getEquipItemIndex(type_=type_)

		equip_item = self.equipItem(index)
		if equip_item is not None:
			equip_item.ensureContItemUsable(occasion, count, **kwargs)

	def useItem(self, occasion: ItemUseOccasion, index: int = 0,
				type_: type = None, count: int = 1, **kwargs):
		"""
		使用物品
		Args:
			occasion (ItemUseOccasion): 使用场合枚举
			index (int): 装备类型索引
			type_ (type): 装备类型（类）
			count (int): 使用数量
			**kwargs (**dict): 拓展参数
		"""
		if type_ is not None:
			index = self.getEquipItemIndex(type_=type_)

		equip_item = self.equipItem(index)
		if equip_item is not None:
			equip_item.useItem(occasion, count, **kwargs)

	# endregion

	# endregion

	""" 占位符 """


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
class BaseTrait(models.Model):
	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "特性"

	CODES = [
		(TraitCode.Unset.value, '无特性'),
	]

	# 所属物品
	# item = models.ForeignKey('BaseItem', on_delete=models.CASCADE, verbose_name="物品")

	# 特性编号
	code = models.PositiveSmallIntegerField(default=0, choices=CODES, verbose_name="特性编号")

	# 特性参数
	params = jsonfield.JSONField(default=[], verbose_name="特性参数")

	# 转化为字典
	def convert(self):
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
#  使用效果描述格式
# ===================================================
class EffectDescriptionFormat:
	DefaultFormat = None
	ShortFormat = None

	def __init__(self, short=False, **kwargs):
		from game_module.models import BaseParam, GameConfigure

		hp_name = BaseParam.get(attr="mhp").name
		mp_name = BaseParam.get(attr="mmp").name

		conf: GameConfigure = GameConfigure.get()

		self.short = short

		self.kwargs = kwargs

		self.empty = self.get("empty", "无效果")

		self.second = self.get("second", "秒")
		self.minute = self.get("minute", "分")
		self.hour = self.get("hour", "时")
		self.day = self.get("day", "天")

		self.round = self.get("round", "回合")

		self.gold = self.get("gold", conf.gold)
		self.bound_ticket = self.get("bound_ticket", conf.bound_ticket)

		self.exermon = self.get("exermon", "艾瑟萌")
		self.exer_slot = self.get("exer_slot", "艾瑟萌槽")

		self.hp_attr = self.get("hp_attr", hp_name)
		self.mp_attr = self.get("mp_attr", mp_name)

		self.recover1 = self.get("recover1", "%s 回复 %s 点")
		self.recover2 = self.get("recover2", "%s 回复 %s %%")
		self.recover3 = self.get("recover3", "%s 回复 %s 点，%s%%")

		self.promote1 = self.get("promote1", "%s 增加 %s 点")
		self.promote2 = self.get("promote2", "%s 增加 %s %%")
		self.promote3 = self.get("promote3", "%s 增加 %s 点，%s%%")

		self.tmp_promote1 = self.get("tmp_promote1", "%s 增加 %s 点，持续 %s %s")
		self.tmp_promote2 = self.get("tmp_promote2", "%s 增加%s%%，持续 %s %s")
		self.tmp_promote3 = self.get("tmp_promote3", "%s 增加 %s 点，%s%%，持续 %s %s")

		self.gain_item1 = self.get("gain_item1", "%s 增加 %s 个")
		self.gain_item2 = self.get("gain_item2", "%s 增加 %s~%s 个")

		self.gain_gold1 = self.get("gain_gold1", "%s 增加 %s")
		self.gain_gold2 = self.get("gain_gold2", "%s 增加 %s~%s")

		self.gain_exp1 = self.get("gain_exp1", "%s %s经验值增加 %s")
		self.gain_exp2 = self.get("gain_exp2", "%s %s经验值增加 %s~%s")

		self.player_gain_exp1 = self.get("player_gain_exp1", "玩家经验值增加 %s")
		self.player_gain_exp2 = self.get("player_gain_exp2", "玩家经验值增加 %s~%s")

		self.eval = self.get("eval", "执行程序：%s")

	def get(self, key, default=""):
		"""
		获取参数
		Args:
			key (str): 参数名
			default (str): 默认值
		Returns:
			返回参数值
		"""
		if key in self.kwargs:
			return self.kwargs[key]
		else:
			return default

	@classmethod
	def defaultFormat(cls) -> 'EffectDescriptionFormat':
		"""
		默认格式
		Returns:
			返回默认描述格式
		"""
		if cls.DefaultFormat is None:
			cls.DefaultFormat = EffectDescriptionFormat()
		return cls.DefaultFormat

	@classmethod
	def shortFormat(cls) -> 'EffectDescriptionFormat':
		"""
		简要格式
		Returns:
			返回简要描述格式
		"""
		return EffectDescriptionFormat(True,
									   empty="无",

									   second="s", minute="m", hour="h", day="d",

									   round="回",

									   hp_attr="HP", mp_attr="MP",

									   gold="金", bound_ticket="绑点",

									   exermon="萌", exer_slot="槽",

									   recover1="%s +%s", recover2="%s +%s%%", recover3="%s +%s, %s%%",

									   promote1="%s +%s", promote2="%s +%s%%", promote3="%s +%s, %s%%",

									   tmp_promote1="%s +%s %s%s", tmp_promote2="%s +%s%% %s%s",
									   tmp_promote3="%s +%s, %s%% %s%s",

									   gain_item1="%s +%s", gain_item2="%s +%s~%s",

									   gain_gold1="%s +%s", gain_gold2="%s +%s~%s",

									   gain_exp1="%s%s经验 +%s", gain_exp2="%s%s经验 +%s~%s",

									   player_gain_exp1="玩家经验 +%s", player_gain_exp2="玩家经验 +%s~%s",

									   eval=""
									   )


# ===================================================
#  使用效果表
# ===================================================
class BaseEffect(models.Model):
	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "使用效果"

	LIST_DISPLAY_APPEND = ['item', 'adminDescribe']

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

	def adminDescribe(self):
		return self.describe()

	adminDescribe.short_description = "描述"

	def describe(self, format: EffectDescriptionFormat = None):
		from game_module.models import BaseParam, Subject

		if format is None: format = EffectDescriptionFormat.defaultFormat()

		if not isinstance(self.params, list):
			raise GameException(ErrorType.DatabaseError)

		code = ItemEffectCode(self.code)
		params = tuple(self.params)
		p_len = len(params)

		if code == ItemEffectCode.RecoverHP or \
				code == ItemEffectCode.RecoverMP:

			name = format.hp_attr if code == ItemEffectCode.RecoverHP \
				else format.mp_attr

			a = params[0]
			if p_len < 2:  # 如果只有 a 参数
				return format.recover1 % (name, a)

			b = params[1]
			if a == 0: return format.recover2 % (name, b)
			return format.recover3 % (name, a, b)

		elif code == ItemEffectCode.AddParam:
			p, a = params[0], params[1]
			name = BaseParam.get(id=p).name
			if format.short: name = name[0]

			if p_len < 3:  # 如果只有 a 参数
				return format.promote1 % (name, a)

			b = params[2]
			if a == 0: return format.promote2 % (name, b)
			return format.promote3 % (name, a, b)

		elif code == ItemEffectCode.TempAddParam or \
				code == ItemEffectCode.BattleAddParam:
			p, t, a = params[0], params[1], params[2]
			name = BaseParam.get(id=p).name
			if format.short: name = name[0]

			t, unit = self._getUnit(format, code, t)

			if p_len < 4:  # 如果只有 a 参数
				return format.tmp_promote1 % (name, a, t, unit)

			b = params[3]
			if a == 0: return format.tmp_promote2 % (name, b, t, unit)
			return format.tmp_promote3 % (name, a, b, t, unit)

		elif code == ItemEffectCode.GainItem:
			from .views import Common

			t, i, m = params[0], params[0], params[2]
			name = Common.getItem(type_=t, id=i).name

			if p_len < 4:  # 如果只有 m 参数
				return format.gain_item1 % (name, m)

			n = params[3]
			return format.gain_item2 % (name, m, n)

		elif code == ItemEffectCode.GainGold or \
				code == ItemEffectCode.GainBoundTicket:

			m = params[0]
			name = format.gold if code == ItemEffectCode.GainGold else format.bound_ticket

			if p_len < 2:  # 如果只有 m 参数
				return format.gain_gold1 % (name, m)

			n = params[1]
			return format.gain_gold2 % (name, m, n)

		elif code == ItemEffectCode.GainExermonExp or \
				code == ItemEffectCode.GainExerSlotItemExp:

			s, m = params[0], params[1]
			name = Subject.get(id=s).name
			if format.short: name = name[0]

			exer = format.exermon if code == ItemEffectCode.GainExermonExp \
				else format.exer_slot

			if p_len < 3:  # 如果只有 m 参数
				return format.gain_exp1 % (name, exer, m)

			n = params[2]
			return format.gain_exp2 % (name, exer, m, n)

		elif code == ItemEffectCode.GainPlayerExp:

			m = params[0]

			if p_len < 2:  # 如果只有 m 参数
				return format.player_gain_exp1 % (m)

			n = params[1]
			return format.player_gain_exp2 % (m, n)

		elif code == ItemEffectCode.Eval:
			if p_len == 2:  # 如果只有 m 参数
				return params[1]
			else:
				return format.eval % params[0]

		return format.empty

	@classmethod
	def _getUnit(cls, format: EffectDescriptionFormat,
				 code: ItemEffectCode, t) -> (int, str):
		"""
		获取时间单位
		Args:
			format (EffectDescriptionFormat): 描述格式
			code (ItemEffectCode): 效果类型
			t (int): 数值
		Returns:
			返回转化后的数值以及单位
		"""

		if code == ItemEffectCode.BattleAddParam:
			return t, format.round

		if 60 <= t <= 3600: return t / 60, format.minute
		if 3600 <= t <= 86400: return t / 3600, format.hour

		return t / 86400, format.day

	# 转化为字典
	def convert(self):

		return {
			'code': self.code,
			'params': self.params,
			'description': self.describe(),
			'short_description': self.describe(
				EffectDescriptionFormat.shortFormat()),
		}

# endregion
