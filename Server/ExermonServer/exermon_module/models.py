from django.db import models
from django.conf import settings
from game_module.models import BaseParam, ParamValue, ParamRate, \
	ParamValueRange, ParamRateRange, Subject
from item_module.models import *
from utils.model_utils import SkillImageUpload, ExermonImageUpload, \
	Common as ModelUtils
from utils.view_utils import Common as ViewUtils
from utils.exception import ErrorType, ErrorException

from enum import Enum
import jsonfield, os, base64


# ===================================================
#  艾瑟萌基础属性值表
# ===================================================
class ExerParamBase(ParamValue):

	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌基础属性值"

	# 艾瑟萌
	exermon = models.ForeignKey("Exermon", on_delete=models.CASCADE, verbose_name="艾瑟萌")


# ===================================================
#  艾瑟萌属性成长率表
# ===================================================
class ExerParamRate(ParamRate):

	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌属性成长率"

	# 艾瑟萌
	exermon = models.ForeignKey("Exermon", on_delete=models.CASCADE, verbose_name="艾瑟萌")


# ===================================================
#  艾瑟萌类型枚举
# ===================================================
class ExermonType(Enum):
	Initial = 1  # 初始艾瑟萌
	Wild = 2  # 野生艾瑟萌
	Task = 3  # 剧情艾瑟萌
	Rare = 4  # 稀有艾瑟萌


# ===================================================
#  艾瑟萌表
# ===================================================
class Exermon(BaseItem):

	class Meta:

		verbose_name = verbose_name_plural = "艾瑟萌"

	NAME_REG = r'^.{1,6}$'

	TYPES = [
		(ExermonType.Initial.value, '初始艾瑟萌'),
		(ExermonType.Wild.value, '野生艾瑟萌'),
		(ExermonType.Task.value, '剧情艾瑟萌'),
		(ExermonType.Rare.value, '稀有艾瑟萌'),
	]

	# 道具类型
	TYPE = ItemType.Exermon

	# 艾瑟萌星级
	star = models.ForeignKey('game_module.ExerStar', on_delete=models.CASCADE, verbose_name="艾瑟萌星级")

	# 科目
	subject = models.ForeignKey('game_module.Subject', on_delete=models.CASCADE, verbose_name="科目")

	# 全身像
	full = models.ImageField(upload_to=ExermonImageUpload('full'), null=True, blank=True,
							 verbose_name="全身像")

	# 缩略图
	icon = models.ImageField(upload_to=ExermonImageUpload('icon'), null=True, blank=True,
							 verbose_name="缩略图")

	# 战斗图
	battle = models.ImageField(upload_to=ExermonImageUpload('battle'), null=True, blank=True,
							   verbose_name="战斗图")

	# 艾瑟萌类型
	e_type = models.PositiveSmallIntegerField(default=ExermonType.Initial.value,
											choices=TYPES, verbose_name="艾瑟萌类型")

	# 管理界面用：显示属性基础值
	def adminParamBases(self):
		from django.utils.html import format_html

		params = self.paramBases()

		res = ''
		for p in params:
			res += str(p) + "<br>"

		return format_html(res)

	adminParamBases.short_description = "属性基础值"

	# 管理界面用：显示属性成长率
	def adminParamRates(self):
		from django.utils.html import format_html

		params = self.paramRates()

		res = ''
		for p in params:
			res += str(p) + "<br>"

		return format_html(res)

	adminParamRates.short_description = "属性成长率"

	# 对应的容器项类
	@classmethod
	def contItemClass(cls): return PlayerExermon

	# 用于获取属性值
	def __getattr__(self, item):
		type = item[4:]

		if type == 'base':
			return self.paramBase(attr=item[:3])

		if type == 'rate':
			return self.paramRate(attr=item[:3])

		return super().__getattr__(item)

	# 转换属性为 dict
	def _convertParamsToDict(self):

		data = dict()

		data['bases'] = ModelUtils.objectsToDict(self.paramBases())
		data['rates'] = ModelUtils.objectsToDict(self.paramRates())

		return data

	# 转化为 dict
	def _convertToDict(self):
		res = super()._convertToDict()

		res['star_id'] = self.star_id
		res['subject_id'] = self.subject_id
		res['e_type'] = self.e_type
		res['params'] = self._convertParamsToDict()

		return res

	# 获取艾瑟萌的技能
	def skills(self, **kwargs):
		return ViewUtils.getObjects(ExerSkill, o_exermon_id=self.id, **kwargs)

	# 获取所有的属性基本值
	def paramBases(self):
		return self.exerparambase_set.all()

	# 获取所有的属性成长率
	def paramRates(self):
		return self.exerparamrate_set.all()

	# 获取属性基本值
	def paramBase(self, param_id=None, attr=None):
		param = None
		if param_id is not None:
			param = self.paramBases().filter(param_id=param_id)
		if attr is not None:
			param = self.paramRates().filter(param__attr=attr)

		if param is None or not param.exist(): return None

		return param.first().getValue()

	# 获取属性成长值
	def paramRate(self, param_id=None, attr=None):
		param = None
		if param_id is not None:
			param = self.param_rates.filter(param_id=param_id)
		if attr is not None:
			param = self.param_rates.filter(param__attr=attr)

		if param is None or not param.exist(): return None

		return param.first().getValue()


# ===================================================
#  艾瑟萌碎片表
# ===================================================
class ExerFrag(BaseItem):

	class Meta:

		verbose_name = verbose_name_plural = "艾瑟萌碎片"

	# 道具类型
	TYPE = ItemType.ExerFrag

	# 所属艾瑟萌
	o_exermon = models.ForeignKey('Exermon', on_delete=models.CASCADE,
								 verbose_name="所属艾瑟萌")

	# 所需碎片数目
	count = models.PositiveSmallIntegerField(default=16, verbose_name="所需碎片数")

	# 出售价格（出售固定为金币，为0则不可出售）
	sell_price = models.PositiveIntegerField(default=0, verbose_name="出售价格")

	# 对应的容器项类
	@classmethod
	def contItemClass(cls): return ExerFragPackItem

	# 转化为 dict
	def _convertToDict(self, **kwargs):

		res = super()._convertToDict(**kwargs)

		res['exermon_id'] = self.o_exermon_id
		res['count'] = self.count

		return res

	# 最大叠加数量（为0则不限）
	def _maxCount(self): return 0


# ===================================================
#  艾瑟萌碎片背包
# ===================================================
class ExerFragPack(PackContainer):

	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌碎片背包"

	# 容器类型
	TYPE = ContainerType.ExerFragPack

	# 玩家
	player = models.OneToOneField('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 所接受的容器项类
	@classmethod
	def acceptedContItemClass(cls): return ExerFragPackItem

	# 创建一个背包（创建角色时候执行）
	@classmethod
	def _create(cls, player):
		pack: cls = super()._create()
		pack.player = player
		return pack

	# 持有玩家
	def ownerPlayer(self): return self.player


# ===================================================
#  艾瑟萌碎片背包物品
# ===================================================
class ExerFragPackItem(PackContItem):
	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌碎片背包物品"

	# 容器项类型
	TYPE = ContItemType.ExerFragPackItem

	# 所接受的物品类
	@classmethod
	def acceptedItemClass(cls): return ExerFrag


# ===================================================
#  艾瑟萌天赋成长加成率表
# ===================================================
class GiftParamRate(ParamRate):

	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌天赋成长加成率"

	# 艾瑟萌天赋
	gift = models.ForeignKey("ExerGift", on_delete=models.CASCADE, verbose_name="艾瑟萌天赋")


# ===================================================
#  艾瑟萌天赋类型枚举
# ===================================================
class ExerGiftType(Enum):
	Initial = 1  # 初始艾瑟萌天赋
	Other = 2  # 其他艾瑟萌天赋


# ===================================================
#  艾瑟萌天赋表
# ===================================================
class ExerGift(BaseItem):

	class Meta:

		verbose_name = verbose_name_plural = "艾瑟萌天赋"

	# 道具类型
	TYPE = ItemType.ExerGift

	TYPES = [
		(ExerGiftType.Initial.value, '初始天赋'),
		(ExerGiftType.Other.value, '其他天赋'),
	]

	# 艾瑟萌星级
	star = models.ForeignKey('game_module.ExerGiftStar', on_delete=models.CASCADE, verbose_name="艾瑟萌星级")

	# # 标志颜色（#ABCDEF）
	# color = models.CharField(max_length=7, null=False, default='#FFFFFF', verbose_name="标志颜色")

	# 艾瑟萌天赋类型
	g_type = models.PositiveSmallIntegerField(default=ExerGiftType.Initial.value,
											choices=TYPES, verbose_name="艾瑟萌天赋类型")

	# 管理界面用：显示属性成长率
	def adminParamRates(self):
		from django.utils.html import format_html

		params = self.paramRates()

		res = ''
		for p in params:
			res += str(p) + "<br>"

		return format_html(res)

	adminParamRates.short_description = "属性成长率"

	# 对应的容器项类
	@classmethod
	def contItemClass(cls): return PlayerExerGift

	# 用于获取属性值
	def __getattr__(self, item):
		type = item[4:]

		if type == 'rate':
			return self.paramRate(attr=item[:3])

		return super().__getattr__(item)

	# 转化为 dict
	def _convertToDict(self, **kwargs):
		res = super()._convertToDict(**kwargs)

		res['star_id'] = self.star_id
		res['color'] = self.color
		res['g_type'] = self.g_type
		res['params'] = ModelUtils.objectsToDict(self.paramRates())

		return res

	# 获取所有的属性成长加成率
	def paramRates(self):
		return self.giftparamrate_set.all()

	# 获取属性成长加成率
	def paramRate(self, param_id=None, attr=None):
		param = None
		if param_id is not None:
			param = self.paramRates().filter(param_id=param_id)
		if attr is not None:
			param = self.paramRates().filter(param__attr=attr)

		if param is None or not param.exist(): return None

		return param.first().getValue()


# ===================================================
#  艾瑟萌天赋池
# ===================================================
class ExerGiftPool(PackContainer):

	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌天赋池"

	# 容器类型
	TYPE = ContainerType.ExerGiftPool

	# 玩家
	player = models.OneToOneField('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 所接受的容器项类
	@classmethod
	def acceptedContItemClass(cls): return PlayerExerGift

	# 创建一个背包（创建角色时候执行）
	@classmethod
	def _create(cls, player):
		pool: cls = super()._create()
		pool.player = player
		return pool

	# 持有玩家
	def ownerPlayer(self): return self.player


# ===================================================
#  玩家艾瑟萌天赋
# ===================================================
class PlayerExerGift(PackContItem):

	class Meta:
		verbose_name = verbose_name_plural = "玩家艾瑟萌天赋"

	# 容器项类型
	TYPE = ContItemType.PlayerExerGift

	# 所接受的物品类
	@classmethod
	def acceptedItemClass(cls): return ExerGift

	# 登陆玩家
	# player = models.ForeignKey('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 艾瑟萌天赋
	# exer_gift = models.ForeignKey('ExerGift', on_delete=models.CASCADE, verbose_name="艾瑟萌天赋")

	# # 创建容器项
	# def transfer(self, container, **kwargs):
	# 	container: ExerGiftPool = container.targetContainer()
	# 	super().transfer(container, **kwargs)
	# 	self.player = container.player

	# 获取属性成长值
	def paramRate(self, param_id=None, attr=None):
		return self.targetItem().paramRate(param_id, attr)


# ===================================================
#  玩家艾瑟萌表
# ===================================================
class PlayerExermon(PackContItem):

	class Meta:
		verbose_name = verbose_name_plural = "玩家艾瑟萌"

	# 容器项类型
	TYPE = ContItemType.PlayerExermon

	# 玩家
	player = models.ForeignKey('player_module.Player', on_delete=models.CASCADE,
							   null=True, verbose_name="玩家")

	# 艾瑟萌昵称
	nickname = models.CharField(max_length=8, verbose_name="艾瑟萌昵称")

	# 经验值（相对）
	exp = models.PositiveIntegerField(default=0, verbose_name="经验值")

	# 等级
	level = models.PositiveSmallIntegerField(default=1, verbose_name="等级")

	# region 重写配置

	# 所接受的物品类
	@classmethod
	def acceptedItemClass(cls): return Exermon

	# def __str__(self):
	# 	return str(self.player)+'-'+str(self.item.target())

	# 创建之后调用
	def afterCreated(self, **kwargs):
		ExerSkillSlot.create(player_exer=self)

	# 创建容器项
	def transfer(self, container, **kwargs):
		container: ExerHub = container.target()
		super().transfer(container, **kwargs)
		self.player = container.player

	# endregion

	# 转换属性为 dict
	def _convertParamsToDict(self):

		data = dict()

		data['vals'] = ModelUtils.objectsToDict(self.paramVals())

		return data

	# 转化为 dict
	def _convertToDict(self, **kwargs):
		res = super()._convertToDict(**kwargs)

		res['nickname'] = self.nickname
		res['exp'] = self.exp
		res['level'] = self.level
		res['params'] = self._convertParamsToDict()
		res['exerskillslot_id'] = self.exerskillslot.id

		return res

	# 用于获取属性值
	def __getattr__(self, item):
		type = item[4:]

		if type == 'base':
			return self.paramBase(attr=item[:3])

		if type == 'rate':
			return self.paramRate(attr=item[:3])

		return super().__getattr__(item)

	# 获取所有属性
	def paramVals(self):
		vals = []
		params = BaseParam.objs()

		for param in params:
			val = ParamValue()
			val.param = param
			val.value = self.paramVal(param_id=param.id)
			vals.append(val)

		return vals

	# 艾瑟萌基础属性
	def paramBase(self, param_id=None, attr=None):
		exermon = self.exermon()

		if exermon is None: return 0
		return exermon.paramBase(param_id, attr)

	# 获取属性成长值
	def paramRate(self, param_id=None, attr=None):
		exermon = self.exermon()

		if exermon is None: return 0
		return exermon.paramRate(param_id, attr)

	# 获取属性当前值
	def paramVal(self, param_id=None, attr=None):
		from utils.calc_utils import ExermonParamCalc

		base = self.paramBase(param_id, attr)
		rate = self.paramRate(param_id, attr)
		value = ExermonParamCalc.calc(base, rate, self.level)

		return value

	# 总经验
	def sumExp(self):
		from utils.calc_utils import ExermonLevelCalc
		return ExermonLevelCalc.getSumExp(self.exermon().star, self.level, self.exp)

	# 所剩经验
	def deltaExp(self):
		from utils.calc_utils import ExermonLevelCalc

		delta = ExermonLevelCalc.getDetlaExp(self.exermon().star, self.level)
		if delta == -1: return -1

		return max(delta-self.exp, 0)

	# 更改等级
	def changeLevel(self, level, event=True):
		if level > self.level and event:
			self._onUpgrade()

		self.level = level
		self.exp = 0
		self.refresh()

	# 更改经验
	def changeExp(self, val):
		self.exp += val
		self.refresh()

	# 刷新艾瑟萌
	def refresh(self):

		self.refreshLevel()

	# 刷新等级
	def refreshLevel(self):
		from utils.calc_utils import ExermonLevelCalc

		exp = self.exp
		level = self.level
		delta = ExermonLevelCalc.getDetlaExp(self.exermon().star, level)

		# 当可以升级的时候
		while exp > delta:
			level += 1  # 升级
			exp -= delta  # 扣除所需的经验
			# 更新所需经验值
			delta = ExermonLevelCalc.getDetlaExp(self.exermon().star, level)

		if level > self.level:

			self.exp = exp
			self.level = level

			self._onUpgrade()

	# 升级触发事件
	def _onUpgrade(self):
		pass


# ===================================================
#  艾瑟萌仓库
# ===================================================
class ExerHub(PackContainer):

	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌仓库"

	# 容器类型
	TYPE = ContainerType.ExerHub

	# 玩家
	player = models.OneToOneField('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 所接受的容器项类
	@classmethod
	def acceptedContItemClass(cls): return PlayerExermon

	# 创建一个背包（创建角色时候执行）
	@classmethod
	def _create(cls, player):
		pack: cls = super()._create()
		pack.player = player
		return pack

	# 持有玩家
	def ownerPlayer(self): return self.player


# ===================================================
#  艾瑟萌槽表
# ===================================================
class ExerSlot(SlotContainer):

	class Meta:

		verbose_name = verbose_name_plural = "艾瑟萌槽"

	# 容器类型
	TYPE = ContainerType.ExerSlot

	# 玩家
	player = models.OneToOneField('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# # 艾瑟萌仓库
	# exer_hub = models.ForeignKey('ExerHub', on_delete=models.CASCADE, verbose_name="艾瑟萌仓库")
	#
	# # 天赋池
	# gift_pool = models.ForeignKey('ExerGiftPool', on_delete=models.CASCADE, verbose_name="天赋池")

	init_exers = None

	# 所接受的槽项类
	@classmethod
	def acceptedSlotItemClass(cls): return ExerSlotItem

	# 所接受的容器项类
	@classmethod
	def acceptedContItemClass(cls): return PlayerExerGift, PlayerExermon

	# 默认容器容量（0为无限）
	@classmethod
	def defaultCapacity(cls): return Subject.MAX_SELECTED

	# 创建一个艾瑟萌槽（选择艾瑟萌时候执行）
	@classmethod
	def _create(cls, player, player_exers):
		slot: cls = super()._create()
		slot.player = player
		slot.equip_container1 = player.exerhub
		slot.equip_container2 = player.exergiftpool
		slot.init_exers = player_exers

		return slot

	# 创建一个槽
	def _createSlot(self, cla, index, **kwargs):
		return super()._createSlot(cla, index,
			init_exer=self.init_exers[index], **kwargs)

	# 保证科目与槽一致
	def ensureSubject(self, slot_item, exermon):
		if slot_item.subject_id != exermon.exermon().subject_id:
			raise ErrorException(ErrorType.IncorrectSubject)

		return True

	# 保证满足装备条件
	def ensureEquipCondition(self, slot_item, equip_item):
		super().ensureEquipCondition(slot_item, equip_item)

		if isinstance(equip_item, PlayerExermon):
			self.ensureSubject(slot_item, equip_item)

		return True

	# 持有玩家
	def ownerPlayer(self): return self.player


# ===================================================
#  艾瑟萌槽项表
# ===================================================
class ExerSlotItem(SlotContItem):

	class Meta:

		verbose_name = verbose_name_plural = "艾瑟萌槽项"

	# 容器类型
	TYPE = ContItemType.ExerSlotItem

	# 玩家
	player = models.ForeignKey('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 玩家艾瑟萌关系
	# exermon = models.OneToOneField('PlayerExermon', null=True, blank=True, on_delete=models.CASCADE, verbose_name="玩家艾瑟萌")

	# 装备的天赋
	# gift = models.OneToOneField('PlayerExerGift', null=True, blank=True, on_delete=models.CASCADE, verbose_name="天赋")

	# 科目
	subject = models.ForeignKey('game_module.Subject', on_delete=models.CASCADE, verbose_name="科目")

	# 经验值（累计）
	exp = models.PositiveIntegerField(default=0, verbose_name="经验值")

	# 属性值（缓存）
	cached_params = None

	# 所接受的装备项类
	@classmethod
	def acceptedEquipItemClass(cls): return PlayerExermon, PlayerExerGift

	# 所接受的装备项属性名（2个）
	@classmethod
	def acceptedEquipItemAttr(cls): return 'player_exer', 'player_gift'

	# def __init__(self, *args, **kwargs):
	# 	super().__init__(*args, **kwargs)
	# 	self.cached_params = {}

	# 转换属性为 dict
	def _convertParamsToDict(self):

		data = dict()

		data['vals'] = ModelUtils.objectsToDict(self.paramVals())

		return data

	# 转化为 dict
	def _convertToDict(self, **kwargs):
		res = super()._convertToDict(**kwargs)

		level, next = self.slotLevel()

		res['subject_id'] = self.subject_id
		res['exp'] = self.exp
		res['level'] = level
		res['next'] = next
		res['params'] = self._convertParamsToDict()
		res['exerequipslot_id'] = self.exerequipslot.id

		return res

	# 移动容器项
	def transfer(self, container: ExerSlot, **kwargs):
		super().transfer(container, **kwargs)
		self.player = container.player

	# 配置索引
	def setupIndex(self, index, init_exer: PlayerExermon = None, **kwargs):
		super().setupIndex(index, **kwargs)
		self.subject = init_exer.exermon().subject
		self.equip(1, init_exer)

	# 创建之后调用
	def afterCreated(self, **kwargs):
		ExerEquipSlot.create(exer_slot=self)

	# # 获取装备的艾瑟萌
	# def exermon(self) -> PlayerExermon: return self.targetEquipItem1()
	#
	# # 获取装备的天赋
	# def gift(self) -> PlayerExerGift: return self.targetEquipItem2()

	# region 属性

	# 获取所有属性
	def paramVals(self):
		vals = []
		params = BaseParam.objs()

		for param in params:
			val = ParamValue()
			val.param = param
			val.value = self.paramVal(param_id=param.id)
			vals.append(val)

		return vals

	# 艾瑟萌实际属性值（RPV）
	def paramVal(self, param_id=None, attr=None):
		id = None
		if param_id is not None: id = param_id
		if attr is not None: id = attr

		if id is None: return 0

		if self.cached_params is None or id not in self.cached_params:

			bpv = self.baseParamVal(param_id, attr)
			ppv = self.plusParamVal(param_id, attr)
			rr = self.realRate(param_id, attr)
			apv = self.appendParamVal(param_id, attr)

			self.cached_params[id] = self.adjustParamVal(
				(bpv+ppv)*rr+apv, param_id, attr)

		return self.cached_params[id]

	# 艾瑟萌基础属性
	def paramBase(self, param_id=None, attr=None):
		player_exer: PlayerExermon = self.player_exer

		if player_exer is None: return 0
		return player_exer.paramBase(param_id, attr)

	# 获取属性成长值
	def paramRate(self, param_id=None, attr=None):
		player_exer: PlayerExermon = self.player_exer
		player_gift: PlayerExerGift = self.player_gift

		if player_exer is None: return 0
		base = player_exer.paramRate(param_id, attr)

		if player_gift is None: rate = 1
		else: rate = player_gift.paramRate(param_id, attr)

		return base*rate

	# 艾瑟萌基本属性值（BPV）
	def baseParamVal(self, param_id=None, attr=None):
		from utils.calc_utils import ExermonParamCalc

		base = self.paramBase(param_id, attr)
		rate = self.paramRate(param_id, attr)
		return ExermonParamCalc.calc(base, rate, self.exermon().level)

	# 艾瑟萌附加属性值（PPV）
	def plusParamVal(self, param_id=None, attr=None):
		return self.exerequipslot.param(param_id, attr)

	# 实际加成率（RR）
	def realRate(self, param_id=None, attr=None):
		return self.baseRate(param_id, attr)*self.plusRate(param_id, attr)

	# 基础加成率（BR）
	def baseRate(self, param_id=None, attr=None): return 1

	# 附加加成率（PR）
	def plusRate(self, param_id=None, attr=None): return 1

	# 追加属性值（APV）
	def appendParamVal(self, param_id=None, attr=None): return 0

	# 调整属性值
	def adjustParamVal(self, val, param_id=None, attr=None):
		param = None
		if param_id is not None:
			param: BaseParam = BaseParam.get(id=param_id)
		if attr is not None:
			param: BaseParam = BaseParam.get(attr=attr)

		if param is None: return val

		return param.clamp(val)

	# endregion

	# 清除属性缓存
	def clearCache(self): self.cached_params = None

	# 刷新艾瑟萌
	def refresh(self):
		self.clearCache()

	# 槽等级（本等级, 下一级所需经验）
	def slotLevel(self):
		from utils.calc_utils import ExermonSlotLevelCalc

		level = ExermonSlotLevelCalc.calcLevel(self.exp)
		next = ExermonSlotLevelCalc.calcNext(level)

		return level, next

	# 艾瑟萌等级
	def exermonLevel(self):
		return self.exermon().level

	# 增加槽经验
	def changeExp(self, exp):
		self.exp += exp
		self.refresh()

	# 增加艾瑟萌经验
	def changeExerExp(self, exp):
		self.player_exer.changeExp(exp)


# ===================================================
#  技能效果表
# ===================================================
class SkillEffect(models.Model):
	class Meta:
		verbose_name = verbose_name_plural = "技能使用效果"

	# 物品
	item = models.ForeignKey('ExerSkill', on_delete=models.CASCADE, verbose_name="物品")

	# 效果编号
	code = models.PositiveSmallIntegerField(default=0, choices=ItemEffect.CODES, verbose_name="效果编号")

	# 效果参数
	params = jsonfield.JSONField(default=[], verbose_name="效果参数")


# ===================================================
#  目标类型枚举
# ===================================================
class TargetType(Enum):
	Empty = 0  # 无
	Self = 1  # 己方
	Enemy = 2  # 敌方
	BothRandom = 3  # 双方随机
	Both = 4  # 双方全部


# ===================================================
#  命中类型枚举
# ===================================================
class HitType(Enum):
	Empty = 0  # 无
	HPDamage = 1  # 体力值伤害
	HPRecover = 2  # 体力值回复
	HPDrain = 3  # 体力值吸收
	MPDamage = 4  # 精力值伤害
	MPRecover = 5  # 精力值回复
	MPDrain = 6  # 精力值吸收


# ===================================================
#  艾瑟萌技能表
# ===================================================
class ExerSkill(BaseItem):

	class Meta:

		verbose_name = verbose_name_plural = "艾瑟萌技能"

	TARGET_TYPES = [
		(TargetType.Empty.value, '无'),
		(TargetType.Self.value, '己方'),
		(TargetType.Enemy.value, '敌方'),
		(TargetType.BothRandom.value, '双方随机'),
		(TargetType.Both.value, '双方全部'),
	]

	HIT_TYPES = [
		(HitType.Empty.value, '无'),
		(HitType.HPDamage.value, '体力值伤害'),
		(HitType.HPRecover.value, '体力值回复'),
		(HitType.HPDrain.value, '体力值吸收'),
		(HitType.MPDamage.value, '精力值伤害'),
		(HitType.MPRecover.value, '精力值回复'),
		(HitType.MPDrain.value, '精力值吸收'),
	]

	# 道具类型
	TYPE = ItemType.ExerSkill

	# 艾瑟萌
	o_exermon = models.ForeignKey('Exermon', on_delete=models.CASCADE, verbose_name="艾瑟萌")

	# 是否被动技能
	passive = models.BooleanField(default=False, verbose_name="被动技能")

	# 下级技能
	next_skill = models.ForeignKey('ExerSkill', on_delete=models.CASCADE,
								   null=True, blank=True, verbose_name="下级技能")

	# 升级所需的使用次数（0为无法升级）
	need_count = models.PositiveSmallIntegerField(default=0, null=True, blank=True, verbose_name="次数需求")

	# MP消耗
	mp_cost = models.PositiveSmallIntegerField(default=0, null=True, blank=True, verbose_name="MP消耗")

	# 使用几率（*100）
	rate = models.PositiveSmallIntegerField(default=0, null=True, blank=True, verbose_name="使用几率")

	# 使用时机
	# timing = models.PositiveSmallIntegerField(default=0, choices=Timings, verbose_name="使用时机")

	# 冻结时间（回合数）
	freeze = models.PositiveSmallIntegerField(default=0, null=True, blank=True, verbose_name="冻结时间")

	# 最大使用次数（0为不限）
	max_use_count = models.PositiveSmallIntegerField(default=0, null=True, blank=True, verbose_name="最大使用次数")

	# 目标
	target = models.PositiveSmallIntegerField(default=TargetType.Enemy.value, choices=TARGET_TYPES,
											  null=True, blank=True, verbose_name="目标")

	# 命中类型
	hit_type = models.PositiveSmallIntegerField(default=HitType.HPDamage.value, choices=HIT_TYPES,
												null=True, blank=True, verbose_name="命中类型")

	# 攻击比率（*100）
	atk_rate = models.PositiveSmallIntegerField(default=100, null=True, blank=True, verbose_name="攻击比率")

	# 防御比率（*100）
	def_rate = models.PositiveSmallIntegerField(default=100, null=True, blank=True, verbose_name="防御比率")

	# 附加公式
	# 说明：
	plus_formula = models.CharField(default="", max_length=256, null=True, blank=True, verbose_name="附加公式")

	# 技能图标
	icon = models.ImageField(upload_to=SkillImageUpload('icon'),
							 null=True, blank=True, verbose_name="图标")

	# 技能动画
	ani = models.ImageField(upload_to=SkillImageUpload('ani'),
							 null=True, blank=True, verbose_name="技能动画")

	# 击中动画
	target_ani = models.ImageField(upload_to=SkillImageUpload('effect'),
							 null=True, blank=True, verbose_name="击中动画")

	# 对应的容器项类
	@classmethod
	def contItemClass(cls): return ExerSkillSlotItem

	# 获取完整路径
	def getExactlyPath(self):
		base = settings.STATIC_URL
		icon = os.path.join(base, str(self.icon))
		ani = os.path.join(base, str(self.ani))
		target_ani = os.path.join(base, str(self.target_ani))
		if os.path.exists(icon) and \
				os.path.exists(ani) and \
				os.path.exists(target_ani):
			return icon, ani, target_ani
		else:
			raise ErrorException(ErrorType.PictureFileNotFound)

	# 获取图标base64编码
	def convertToBase64(self):
		icon, ani, target_ani = self.getExactlyPath()

		with open(icon, 'rb') as f:
			icon_data = base64.b64encode(f.read()).decode()

		with open(ani, 'rb') as f:
			ani_data = base64.b64encode(f.read()).decode()

		with open(target_ani, 'rb') as f:
			target_ani_data = base64.b64encode(f.read()).decode()

		return icon_data, ani_data, target_ani_data

	# 转化为 dict
	def _convertToDict(self, **kwargs):
		res = super()._convertToDict(**kwargs)

		icon_data, ani_data, target_ani_data = self.convertToBase64()

		res['eid'] = self.o_exermon_id
		res['passive'] = self.passive
		res['next_skill_id'] = self.next_skill_id
		res['need_count'] = self.need_count
		res['mp_cost'] = self.mp_cost
		res['rate'] = self.rate
		# res['timing'] = self.timing
		res['freeze'] = self.freeze
		res['max_use_count'] = self.max_use_count
		res['target'] = self.target
		res['hit_type'] = self.hit_type
		res['atk_rate'] = self.atk_rate
		res['def_rate'] = self.def_rate
		res['plus_formula'] = self.plus_formula
		res['icon'] = icon_data
		res['ani'] = ani_data
		res['target_ani'] = target_ani_data

		return res


# ===================================================
#  艾瑟萌技能槽
# ===================================================
class ExerSkillSlot(SlotContainer):

	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌技能槽"

	# 容器类型
	TYPE = ContainerType.ExerSkillSlot

	# 最大技能数量
	MAX_SKILL_COUNT = 3

	# 艾瑟萌
	player_exer = models.OneToOneField('exermon_module.PlayerExermon',
								   on_delete=models.CASCADE, verbose_name="艾瑟萌")

	skills = None

	# 所接受的槽项类
	@classmethod
	def acceptedSlotItemClass(cls): return ExerSkillSlotItem

	# 所接受的容器项类
	@classmethod
	def acceptedContItemClass(cls): return ()

	# 默认容器容量（0为无限）
	@classmethod
	def defaultCapacity(cls): return cls.MAX_SKILL_COUNT

	# 创建一个背包（创建角色时候执行）
	@classmethod
	def _create(cls, player_exer):
		slot: cls = super()._create()
		slot.player_exer = player_exer
		slot.skills = player_exer.exermon().skills()
		return slot

	# 创建一个槽
	def _createSlot(self, cla, index, **kwargs):
		if index >= self.skills.count(): skill = None
		else: skill = self.skills[index]

		return super()._createSlot(cla, index, skill=skill, **kwargs)

	# 持有玩家
	def ownerPlayer(self): return self.player_exer.player


# ===================================================
#  艾瑟萌技能槽项表
# ===================================================
class ExerSkillSlotItem(SlotContItem):

	class Meta:

		verbose_name = verbose_name_plural = "艾瑟萌技能槽项"

	# 容器项类型
	TYPE = ContItemType.ExerSkillSlotItem

	# 艾瑟萌
	player_exer = models.ForeignKey('PlayerExermon', on_delete=models.CASCADE, verbose_name="艾瑟萌")

	# 技能
	skill = models.ForeignKey('ExerSkill', on_delete=models.CASCADE,
							  null=True, blank=True, verbose_name="技能")

	# 使用次数
	use_count = models.PositiveIntegerField(default=0, verbose_name="使用次数")

	# region 配置项

	# 所接受的装备项类（2个）
	@classmethod
	def acceptedEquipItemClass(cls): return (), ()

	# endregion

	# 转化为 dict
	def _convertToDict(self, **kwargs):
		res = super()._convertToDict(**kwargs)

		res['skill_id'] = self.skill_id
		res['use_count'] = self.use_count

		return res

	# 移动容器项
	def transfer(self, container, **kwargs):
		container: ExerSkillSlot = container.target()
		super().transfer(container, **kwargs)
		self.player_exer = container.player_exer

	# 配置索引
	def setupIndex(self, index, skill=None, **kwargs):
		super().setupIndex(index, **kwargs)
		self.setSkill(skill)

	# 设置技能
	def setSkill(self, skill):
		self.skill = skill
		self.use_count = 0

	# 使用技能
	def useSkill(self):
		skill: ExerSkill = self.skill
		if skill.need_count > 0:

			self.use_count += 1
			if self.use_count >= skill.need_count:
				self.setSkill(skill.next_skill)


# ===================================================
#  艾瑟萌物品表
# ===================================================
class ExerItem(UsableItem):

	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌物品"

	# 道具类型
	TYPE = ItemType.ExerItem

	# 使用几率（*100）
	rate = models.PositiveSmallIntegerField(default=0, verbose_name="使用几率")

	# 对应的容器项类
	@classmethod
	def contItemClass(cls): return ExerPackItem

	# 转化为 dict
	def _convertToDict(self, **kwargs):
		res = super()._convertToDict(**kwargs)

		res['rate'] = self.rate

		return res


# ===================================================
#  艾瑟萌装备
# ===================================================
class ExerEquip(EquipableItem):
	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌装备"

	# 道具类型
	TYPE = ItemType.ExerEquip

	# 装备类型
	e_type = models.ForeignKey("game_module.ExerEquipType",
							   on_delete=models.CASCADE, verbose_name="装备类型")

	# 对应的容器项类
	@classmethod
	def contItemClass(cls): return ExerPackEquip

	# 转化为 dict
	def _convertToDict(self, **kwargs):
		res = super()._convertToDict(**kwargs)

		res['e_type'] = self.e_type

		return res


# ===================================================
#  艾瑟萌背包
# ===================================================
class ExerPack(PackContainer):

	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌背包"

	# 容器类型
	TYPE = ContainerType.ExerPack

	# 默认容量
	DEFAULT_CAPACITY = 32

	# 玩家
	player = models.OneToOneField('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 所接受的容器项类
	@classmethod
	def acceptedContItemClass(cls): return ExerPackItem

	# 获取容器容量（0为无限）
	@classmethod
	def defaultCapacity(cls): return cls.DEFAULT_CAPACITY

	# 创建一个背包（创建角色时候执行）
	@classmethod
	def _create(cls, player):
		pack: cls = super()._create()
		pack.player = player
		return pack

	# 持有玩家
	def ownerPlayer(self): return self.player


# ===================================================
#  艾瑟萌背包物品
# ===================================================
class ExerPackItem(PackContItem):
	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌背包物品"

	# 容器项类型
	TYPE = ContItemType.ExerPackItem

	# 所接受的物品类
	@classmethod
	def acceptedItemClass(cls): return ExerItem, ExerEquip


# ===================================================
#  艾瑟萌背包装备
# ===================================================
class ExerPackEquip(ExerPackItem):
	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌背包装备"

	# 容器项类型
	TYPE = ContItemType.ExerPackEquip

	# 所接受的物品类
	@classmethod
	def acceptedItemClass(cls): return ExerEquip

	# 获取属性值
	def param(self, param_id=None, attr=None):
		return self.targetItem().param(param_id, attr)


# ===================================================
#  艾瑟萌装备槽
#  有 ExermonEquipType.Count 个固定的槽
# ===================================================
class ExerEquipSlot(SlotContainer):

	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌装备槽"

	# 容器类型
	TYPE = ContainerType.ExerEquipSlot

	# 艾瑟萌
	exer_slot = models.OneToOneField('exermon_module.ExerSlotItem',
								   on_delete=models.CASCADE, verbose_name="艾瑟萌")

	# # 艾瑟萌背包
	# exer_pack = models.ForeignKey('ExerPack', on_delete=models.CASCADE, verbose_name="艾瑟萌背包")

	# 所接受的槽项类
	@classmethod
	def acceptedSlotItemClass(cls): return ExerEquipSlotItem

	# 所接受的容器项类
	@classmethod
	def acceptedContItemClass(cls): return ExerPackEquip

	# 默认容器容量（0为无限）
	@classmethod
	def defaultCapacity(cls): return ExerEquipType.count()

	# 创建一个槽（创建角色时候执行）
	@classmethod
	def _create(cls, exer_slot: ExerSlotItem):
		slot: cls = super()._create()
		slot.exer_slot = exer_slot
		slot.equip_container1 = exer_slot.player.exerpack
		return slot

	# 保证装备类型与槽一致
	def ensureEquipType(self, slot_item, equip):
		if slot_item.e_type_id != equip.targetItem().e_type_id:
			raise ErrorException(ErrorType.IncorrectEquipType)

		return True

	# 保证满足装备条件
	def ensureEquipCondition(self, slot_item, equip_item):
		super().ensureEquipCondition(slot_item, equip_item)

		self.ensureEquipType(slot_item, equip_item)

		return True

	# 获取属性值
	def param(self, param_id=None, attr=None):
		sum = 0
		slot_items = self.contItems()

		for slot_item in slot_items:
			sum += slot_item.param(param_id, attr)

		return sum

	# 持有玩家
	def ownerPlayer(self): return self.exer_slot.player


# ===================================================
#  艾瑟萌装备槽项
# ===================================================
class ExerEquipSlotItem(SlotContItem):
	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌装备槽项"

	# 容器项类型
	TYPE = ContItemType.ExerEquipSlotItem

	# 艾瑟萌背包装备
	# pack_equip = models.OneToOneField('ExerPackEquip', on_delete=models.CASCADE,
	# 							 verbose_name="艾瑟萌背包装备")

	# 装备槽类型
	e_type = models.ForeignKey('game_module.ExerEquipType', on_delete=models.CASCADE,
							   verbose_name="装备槽类型")

	# 所接受的装备项类
	@classmethod
	def acceptedEquipItemClass(cls): return ExerPackEquip, ()

	# 所接受的装备项属性名（2个）
	@classmethod
	def acceptedEquipItemAttr(cls): return 'pack_equip', '_'

	# 转化为 dict
	def _convertToDict(self, **kwargs):
		res = super()._convertToDict(**kwargs)

		res['e_type_id'] = self.e_type_id

		return res

	# 配置索引
	def setupIndex(self, index, **kwargs):
		super().setupIndex(index, **kwargs)
		self.e_type_id = index

	# 获取属性值
	def param(self, param_id=None, attr=None):
		return self.pack_equip.param(param_id, attr)

