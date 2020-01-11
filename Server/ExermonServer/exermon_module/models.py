from django.db import models
from django.conf import settings
from game_module.models import BaseParam, ParamValue, ParamRate, \
	ParamValueRange, ParamRateRange, Subject
from item_module.models import BaseItem, SlotContainer, PackContainer, \
	SlotContItem, PackContItem, ItemType, ContainerType, ContItemType, ItemEffect
from utils.model_utils import SkillImageUpload, ExermonImageUpload, \
	Common as ModelUtils
from utils.view_utils import Common as ViewUtils
from utils.exception import ErrorType, ErrorException

from enum import Enum
import jsonfield, os


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
#  艾瑟萌表
# ===================================================
class Exermon(BaseItem):

	class Meta:

		verbose_name = verbose_name_plural = "艾瑟萌"

	# 艾瑟萌星级
	star = models.ForeignKey('ExerStar', on_delete=models.CASCADE, verbose_name="艾瑟萌星级")

	# 科目
	subject = models.ForeignKey('game_module.Subject', on_delete=models.CASCADE, verbose_name="科目")

	# 全身像
	full = models.ImageField(upload_to=ExermonImageUpload('full'),
							 verbose_name="全身像")

	# 缩略图
	icon = models.ImageField(upload_to=ExermonImageUpload('icon'),
							 verbose_name="缩略图")

	# 战斗图
	battle = models.ImageField(upload_to=ExermonImageUpload('battle'),
							   verbose_name="战斗图")

	# # 体力值成长（*100）
	# mhp_rate = models.PositiveSmallIntegerField(default=100, verbose_name="体力成长")
	#
	# # 精力值成长（*100）
	# mmp_rate = models.PositiveSmallIntegerField(default=100, verbose_name="精力成长")
	#
	# # 攻击成长（*100）
	# atk_rate = models.PositiveSmallIntegerField(default=100, verbose_name="攻击成长")
	#
	# # 防御成长（*100）
	# def_rate = models.PositiveSmallIntegerField(default=100, verbose_name="防御成长")
	#
	# # 回避率成长（*100）
	# eva_rate = models.PositiveSmallIntegerField(default=100, verbose_name="回避率成长")
	#
	# # 反击率成长（*100）
	# cri_rate = models.PositiveSmallIntegerField(default=100, verbose_name="反击率成长")
	#
	# # 基础体力值
	# mhp_base = models.PositiveSmallIntegerField(default=100, verbose_name="基础体力")
	#
	# # 基础精力值
	# mmp_base = models.PositiveSmallIntegerField(default=100, verbose_name="基础精力")
	#
	# # 基础攻击
	# atk_base = models.PositiveSmallIntegerField(default=10, verbose_name="基础攻击")
	#
	# # 基础防御
	# def_base = models.PositiveSmallIntegerField(default=10, verbose_name="基础防御")
	#
	# # 基础回避率（*10000）
	# eva_base = models.PositiveSmallIntegerField(default=100, verbose_name="基础回避")
	#
	# # 基础反击率（*10000）
	# cri_base = models.PositiveSmallIntegerField(default=100, verbose_name="基础反击")

	# 道具类型
	@classmethod
	def itemType(cls): return ItemType.Exermon

	# 对应的容器物品类型（枚举）
	@classmethod
	def contItemType(cls): return ContItemType.PlayerExermon

	# 用于获取属性值
	def __getattr__(self, item):
		type = item[4:]

		if type == 'base':
			return self.paramBase(attr=item[:3])

		if type == 'rate':
			return self.paramRate(attr=item[:3])

	# 转换属性为 dict
	def _convertParamsToDict(self):

		data = dict()

		data['bases'] = ModelUtils.objectsToDict(self.paramBases())
		data['rates'] = ModelUtils.objectsToDict(self.paramRates())

		return data

	# 转化为 dict
	def _convertToDict(self):
		res = super()._convertToDict()

		res['star'] = self.star_id
		res['params'] = self._convertParamsToDict()

		return res

	# 获取所有的属性基本值
	def paramBases(self):
		return self.exerparambase_set.all()

	# 获取所有的属性成长率
	def paramRates(self):
		return self.exerparamrate_set.all()

	# 获取属性基本值
	def paramBase(self, param_id=None, attr=None) -> ParamValue:
		param = None
		if param_id is not None:
			param = self.paramBases().filter(param_id=param_id)
		if attr is not None:
			param = self.paramRates().filter(param__attr=attr)

		if param is None or not param.exist(): return None

		return param.first()

	# 获取属性成长值
	def paramRate(self, param_id=None, attr=None) -> ParamRate:
		param = None
		if param_id is not None:
			param = self.param_rates.filter(param_id=param_id)
		if attr is not None:
			param = self.param_rates.filter(param__attr=attr)

		if param is None or not param.exist(): return None

		return param.first()


# ===================================================
#  艾瑟萌碎片表
# ===================================================
class ExerFrag(BaseItem):

	class Meta:

		verbose_name = verbose_name_plural = "艾瑟萌碎片"

	# 所属艾瑟萌
	o_exermon = models.ForeignKey('Exermon', on_delete=models.CASCADE,
								 verbose_name="所属艾瑟萌")

	# 所需碎片数目
	count = models.PositiveSmallIntegerField(default=16, verbose_name="所需碎片数")

	# 出售价格（出售固定为金币，为0则不可出售）
	sell_price = models.PositiveIntegerField(default=0, verbose_name="出售价格")

	# 道具类型
	@classmethod
	def itemType(cls): return ItemType.ExerFrag

	# 对应的容器物品类型（枚举）
	@classmethod
	def contItemType(cls): return ContItemType.ExerFragPackItem

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

	# 玩家
	player = models.OneToOneField('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 容器类型
	@classmethod
	def containerType(cls): return ContainerType.ExerFragPack

	# 容器接受物品类型（数组）
	@classmethod
	def acceptTypes(cls):
		return [ItemType.ExerFrag]

	# 创建一个背包（创建角色时候执行）
	@classmethod
	def _create(cls, player):
		pack: cls = super()._create()
		pack.player = player
		return pack


# ===================================================
#  艾瑟萌碎片背包物品
# ===================================================
class ExerFragPackItem(PackContItem):
	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌碎片背包物品"

	# 容器类型
	@classmethod
	def contItemType(cls): return ContItemType.ExerFragPackItem


# ===================================================
#  艾瑟萌天赋成长加成率表
# ===================================================
class GiftParamRate(ParamRate):

	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌天赋成长加成率"

	# 艾瑟萌天赋
	gift = models.ForeignKey("ExerGift", on_delete=models.CASCADE, verbose_name="艾瑟萌天赋")


# ===================================================
#  艾瑟萌天赋表
# ===================================================
class ExerGift(BaseItem):

	class Meta:

		verbose_name = verbose_name_plural = "艾瑟萌天赋"

	# 标志颜色（#ABCDEF）
	color = models.CharField(max_length=7, null=False, default='#FFFFFF', verbose_name="标志颜色")

	# # 体力值成长（*100）
	# mhp_rate = models.PositiveSmallIntegerField(default=100, verbose_name="体力成长")
	#
	# # 精力值成长（*100）
	# mmp_rate = models.PositiveSmallIntegerField(default=100, verbose_name="精力成长")
	#
	# # 攻击成长（*100）
	# atk_rate = models.PositiveSmallIntegerField(default=100, verbose_name="攻击成长")
	#
	# # 防御成长（*100）
	# def_rate = models.PositiveSmallIntegerField(default=100, verbose_name="防御成长")
	#
	# # 回避率成长（*100）
	# eva_rate = models.PositiveSmallIntegerField(default=100, verbose_name="回避率成长")
	#
	# # 反击率成长（*100）
	# cri_rate = models.PositiveSmallIntegerField(default=100, verbose_name="反击率成长")

	# 道具类型
	@classmethod
	def itemType(cls): return ItemType.ExerGift

	# 对应的容器物品类型（枚举）
	@classmethod
	def contItemType(cls): return ContItemType.PlayerExerGift

	# 用于获取属性值
	def __getattr__(self, item):
		type = item[4:]

		if type == 'rate':
			return self.paramRate(attr=item[:3])

	# 转换属性为 dict
	def _convertParamsToDict(self):

		data = dict()

		data['rates'] = ModelUtils.objectsToDict(self.paramRates())

		return data

	# 转化为 dict
	def _convertToDict(self, **kwargs):
		res = super()._convertToDict(**kwargs)

		res['color'] = self.color
		res['params'] = self._convertParamsToDict()

		return res

	# 获取所有的属性成长加成率
	def paramRates(self):
		return self.giftparamrate_set.all()

	# 获取属性成长加成率
	def paramRate(self, param_id=None, attr=None) -> ParamRate:
		param = None
		if param_id is not None:
			param = self.paramRates().filter(param_id=param_id)
		if attr is not None:
			param = self.paramRates().filter(param__attr=attr)

		if param is None or not param.exist(): return None

		return param.first()

	# 最大叠加数量（为0则不限）
	def _maxCount(self): return 0


# ===================================================
#  艾瑟萌天赋池
# ===================================================
class ExerGiftPool(PackContainer):

	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌天赋池"

	# 玩家
	player = models.OneToOneField('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 容器类型
	@classmethod
	def containerType(cls): return ContainerType.ExerGiftPool

	# 容器接受物品类型（数组）
	@classmethod
	def acceptTypes(cls): return [ItemType.ExerGift]

	# 创建一个背包（创建角色时候执行）
	@classmethod
	def _create(cls, player):
		pool: cls = super()._create()
		pool.player = player
		return pool


# ===================================================
#  玩家艾瑟萌天赋
# ===================================================
class PlayerExerGift(PackContItem):
	class Meta:
		verbose_name = verbose_name_plural = "玩家艾瑟萌天赋"

	# 登陆玩家
	# player = models.ForeignKey('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 艾瑟萌天赋
	# exer_gift = models.ForeignKey('ExerGift', on_delete=models.CASCADE, verbose_name="艾瑟萌天赋")

	# 容器类型
	@classmethod
	def contItemType(cls): return ContItemType.PlayerExerGift

	# # 创建容器项
	# def transfer(self, container, **kwargs):
	# 	container: ExerGiftPool = container.targetContainer()
	# 	super().transfer(container, **kwargs)
	# 	self.player = container.player


# ===================================================
#  玩家艾瑟萌表
# ===================================================
class PlayerExermon(PackContItem):

	class Meta:

		verbose_name = verbose_name_plural = "玩家艾瑟萌"

	# 登陆玩家
	player = models.ForeignKey('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 装备的艾瑟萌
	# exermon = models.ForeignKey('Exermon', on_delete=models.CASCADE, verbose_name="艾瑟萌")

	# 艾瑟萌昵称
	nickname = models.CharField(max_length=8, verbose_name="艾瑟萌昵称")

	# 经验值（相对）
	exp = models.PositiveIntegerField(default=0, verbose_name="经验值")

	# 等级
	level = models.PositiveSmallIntegerField(default=1, verbose_name="等级")

	# 容器类型
	@classmethod
	def contItemType(cls): return ContItemType.PlayerExermon

	def __str__(self):
		return str(self.player)+'-'+str(self.item.targetItem())

	# 用于获取属性值
	def __getattr__(self, item):
		type = item[4:]

		if type == 'base':
			return self.paramBase(attr=item[:3])

		if type == 'rate':
			return self.paramRate(attr=item[:3])

	# 转换属性为 dict
	def _convertParamsToDict(self):

		data = dict()
		params = BaseParam.Params

		# 遍历每一个属性
		for param in params:
			attr = param.attr

			data[attr + '_val'] = self.paramVal(attr=attr)
			data[attr + '_rate'] = self.paramRate(attr=attr)

		return data

	# 转化为 dict
	def convertToDict(self, type=None):

		return {
			'pet_id': self.pet_id,
			'nickname': self.nickname,
			'level': self.level,
			'exp': self.exp,
			'sum_exp': self.sumExp(),
			'delta_exp': self.deltaExp(),
			'params': self._convertParamsToDict()
		}

	def exermon(self) -> Exermon:
		return self.item.targetItem()

	# 创建容器项
	def transfer(self, container, **kwargs):
		container: ExerHub = container.targetContainer()
		super().transfer(container, **kwargs)
		self.player = container.player

	# 获取属性基本值
	def paramBase(self, param_id=None, attr=None) -> ParamValue:
		param = None
		if param_id is not None:
			param = self.base_params.filter(param_id=param_id)
		if attr is not None:
			param = self.base_params.filter(param__attr=attr)

		if param is None or not param.exist(): return None

		return param.first()

	# 获取属性成长值
	def paramRate(self, param_id=None, attr=None) -> ParamRate:
		param = None
		if param_id is not None:
			param = self.param_rates.filter(param_id=param_id)
		if attr is not None:
			param = self.param_rates.filter(param__attr=attr)

		if param is None or not param.exist(): return None

		return param.first()

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

	# 获取属性当前值
	def paramVal(self, param_id=None, attr=None):
		from utils.calc_utils import ExermonParamCalc
		base = self.paramBase(param_id, attr)
		rate = self.paramRate(param_id, attr)
		value = ExermonParamCalc.calc(base, rate, self.level)

		return value

	# # 艾瑟萌基础属性
	# def paramBase(self, param_id=None, attr=None):
	# 	return self.exermon.paramBase(param_id, attr)
	#
	# # 获取属性成长值
	# def paramRate(self, param_id=None, attr=None):
	# 	return self.exermon.paramRate(param_id, attr)

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
		self.save()

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

	# 玩家
	player = models.OneToOneField('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 容器类型
	@classmethod
	def containerType(cls): return ContainerType.ExerHub

	# 容器接受物品类型（数组）
	@classmethod
	def acceptTypes(cls): return [ItemType.Exermon, ContItemType.PlayerExermon]

	# 创建一个背包（创建角色时候执行）
	@classmethod
	def _create(cls, player):
		pack: cls = super()._create()
		pack.player = player
		return pack


# ===================================================
#  艾瑟萌槽表
# ===================================================
class ExerSlot(SlotContainer):

	class Meta:

		verbose_name = verbose_name_plural = "艾瑟萌槽"

	# 玩家
	player = models.OneToOneField('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 艾瑟萌仓库
	exer_hub = models.ForeignKey('ExerHub', on_delete=models.CASCADE, verbose_name="艾瑟萌仓库")

	# 天赋池
	gift_pool = models.ForeignKey('ExerGiftPool', on_delete=models.CASCADE, verbose_name="天赋池")

	# 容器类型
	@classmethod
	def containerType(cls): return ContainerType.ExerSlot

	# 容器接受物品类型（数组）
	@classmethod
	def acceptTypes(cls):
		return [ContItemType.PlayerExerGift, ContItemType.PlayerExermon]

	# 容器接受物品类型（数组）
	@classmethod
	def baseContItemClass(cls): return PackContItem

	# 获取物品对应的容器项
	def contItemClass(self, **kwargs): return ExerSlotItem

	# 获取容器容量（0为无限）
	def getCapacity(self): return Subject.count()

	# 创建一个艾瑟萌槽（创建角色时候执行）
	@classmethod
	def _create(cls, player):
		slot: cls = super()._create()
		slot.player = player
		slot.exer_hub = player.exerhub
		slot.gift_pool = player.exergiftpool
		return slot

	# 保证满足装备条件
	def ensureEquipCondition(self, player_exer=None, player_gift=None):
		if player_exer is not None:
			self.ensureItemAcceptable(cont_item=player_exer)
		if player_gift is not None:
			self.ensureItemAcceptable(cont_item=player_gift)

		return True

	# 保证满足装备卸下条件
	def ensureDequipCondition(self, slot_item, exermon=False, gift=False):
		return True

	# 获取一个槽项
	def getSlotItem(self, subject=None, subject_id=None, **kwargs):
		if subject_id is not None:
			return super().getSlotItem(subject_id=subject_id, **kwargs)
		elif subject is not None:
			return super().getSlotItem(subject=subject, **kwargs)

		return None

	# 更换艾瑟萌
	def setExermon(self, subject=None, subject_id=None,
				   slot_item=None, player_exer=None):
		self.transfer(self.exer_hub, subject=subject,
			subject_id=subject_id, slot_item=slot_item, exermon=True)

		if player_exer is not None:
			self.exer_hub.transfer(self, subject=subject,
				subject_id=subject_id, cont_item=player_exer)

	# 更换天赋
	def setGift(self, subject=None, subject_id=None,
				slot_item=None, player_gift=None):
		self.transfer(self.exer_hub, subject=subject,
			subject_id=subject_id, slot_item=slot_item, exermon=False)

		if player_gift is not None:
			self.gift_pool.transfer(self, subject=subject,
				subject_id=subject_id, cont_item=player_gift)

	# 槽装备（强制装备）
	def equipSlot(self, subject=None, subject_id=None,
				  slot_item=None, player_exer=None, player_gift=None):

		if slot_item is None:
			slot_item = self.getSlotItem(subject=subject, subject_id=subject_id)

		if slot_item is None:
			return None
		else:
			slot_item: ExerSlotItem = slot_item
			self.ensureEquipCondition(player_exer, player_gift)
			slot_item.equip(player_exer, player_gift)

	# 槽卸下（强制卸下）
	def dequipSlot(self, subject=None, subject_id=None, slot_item=None,
				   exermon=False, gift=False):

		if slot_item is None:
			slot_item = self.getSlotItem(subject=subject, subject_id=subject_id)

		if slot_item is None:
			return None
		else:
			slot_item: ExerSlotItem = slot_item
			self.ensureDequipCondition(slot_item, exermon, gift)
			return slot_item.dequip(exermon, gift)

	# 接受转移
	def acceptTransfer(self, subject=None, subject_id=None, slot_item=None, cont_item=None, **kwargs):

		if isinstance(cont_item, PlayerExermon):
			self.equipSlot(subject, subject_id,
						   slot_item, player_exer=cont_item)

		elif isinstance(cont_item, PlayerExerGift):
			self.equipSlot(subject, subject_id,
						   slot_item, player_gift=cont_item)

	# 准备转移
	def prepareTransfer(self, e_type_id=None, e_type=None, slot_item=None,
						exermon=False, **kwargs) -> dict:
		res = kwargs

		res['cont_item'] = self.dequipSlot(
			e_type_id, e_type, slot_item, exermon, not exermon)

		return res


# ===================================================
#  艾瑟萌槽项表
# ===================================================
class ExerSlotItem(SlotContItem):

	class Meta:

		verbose_name = verbose_name_plural = "艾瑟萌槽项"

	# 玩家
	player = models.ForeignKey('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 玩家艾瑟萌关系
	exermon = models.OneToOneField('PlayerExermon', null=True, blank=True, on_delete=models.CASCADE, verbose_name="玩家艾瑟萌")

	# 装备的天赋
	gift = models.ForeignKey('PlayerExerGift', null=True, blank=True, on_delete=models.CASCADE, verbose_name="天赋")

	# 科目
	subject = models.ForeignKey('game_module.Subject', on_delete=models.CASCADE, verbose_name="科目")

	# 艾瑟萌昵称
	nickname = models.CharField(max_length=8, verbose_name="艾瑟萌昵称")

	# 经验值（累计）
	exp = models.PositiveIntegerField(default=0, verbose_name="经验值")

	# 属性值（缓存）
	param_val = None

	# 容器类型
	@classmethod
	def contItemType(cls): return ContItemType.ExerSlotItem

	def __init__(self, *args, **kwargs):
		super().__init__(*args, **kwargs)
		self.param_val = {}

	# 移动容器项
	def transfer(self, container: ExerSlot, **kwargs):
		super().transfer(container, **kwargs)
		self.player = container.player

	# 配置索引
	def setupIndex(self, index):
		self.subject_id = index

	# 装备
	def equip(self, player_exer=None, player_gift=None, **kwargs):
		if player_exer is not None:
			self.exermon = player_exer
			self.exermon.transfer(self.container)

		if player_gift is not None:
			self.gift = player_gift
			self.gift.transfer(self.container)

	# 卸下
	def dequip(self, exermon=False, gift=False,  **kwargs):
		player_exer = player_gift = None

		if exermon:
			self.exermon.remove()
			player_exer = self.exermon
			self.exermon = None

		if gift:
			self.gift.remove()
			player_gift = self.gift
			self.gift = None

		return player_exer, player_gift

	# # 槽等级（本等级, 下一级所需经验）
	# def slotLevel(self):
	# 	from utils.calc_utils import ExermonSlotLevelCalc
	#
	# 	level = ExermonSlotLevelCalc.calcLevel(self.exp)
	# 	next = ExermonSlotLevelCalc.calcNext(level)
	#
	# 	return level, next
	#
	# # 艾瑟萌等级
	# def petLevel(self):
	# 	return self.player_pet.level
	#
	# # 获取属性当前值
	# def paramVal(self, param_id=None, attr=None):
	# 	if attr not in self.param_val: # 如果没有缓存该属性
	#
	# 		from utils.calc_utils import ExermonParamCalc
	# 		base = self.paramBase(param_id, attr)
	# 		rate = self.paramRate(param_id, attr)
	# 		value = ExermonParamCalc.calc(base, rate, self.petLevel)
	#
	# 		self.param_val[attr] = value
	#
	# 	return self.param_val[attr]
	#
	# # 艾瑟萌基础属性
	# def paramBase(self, param_id=None, attr=None):
	# 	return self.player_pet.paramBase(param_id, attr)
	#
	# # 获取属性成长值
	# def paramRate(self, param_id=None, attr=None):
	# 	return self.player_pet.paramRate(param_id, attr)*self.giftParamRate(param_id, attr)
	#
	# # 获取天赋属性加成
	# def giftParamRate(self, param_id=None, attr=None):
	# 	if self.gift is None: return 1
	# 	return self.gift.paramRate(param_id, attr)
	#
	# # 刷新
	# def refresh(self, refresh_pet=True):
	# 	if self.player_pet is not None and refresh_pet:
	# 		self.player_pet.refresh()
	#
	# 	self.param_val = {}
	# 	self.save()
	#
	# # 放置艾瑟萌
	# def setPet(self, player_pet: PlayerExermon):
	# 	self.player_pet = player_pet
	# 	self.refresh()
	#
	# # 放置天赋
	# def setGift(self, gift: ExerGift):
	# 	self.gift = gift
	# 	self.refresh()
	#
	# # 增加经验
	# def changeExp(self, exp):
	# 	self.exp += exp
	# 	if self.player_pet is not None:
	# 		self.player_pet.changeExp(exp)
	#
	# 	self.refresh(False)


# ===================================================
#  艾瑟萌基础属性范围表
# ===================================================
class ExerParamBaseRange(ParamValueRange):

	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌基础属性范围"

	# 艾瑟萌星级
	star = models.ForeignKey("ExerStar", on_delete=models.CASCADE, verbose_name="艾瑟萌星级")


# ===================================================
#  艾瑟萌属性成长率范围表
# ===================================================
class ExerParamRateRange(ParamRateRange):

	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌属性成长率范围"

	# 艾瑟萌星级
	star = models.ForeignKey("ExerStar", on_delete=models.CASCADE, verbose_name="艾瑟萌星级")


# ===================================================
#  艾瑟萌星级表
# ===================================================
class ExerStar(models.Model):

	class Meta:

		verbose_name = verbose_name_plural = "艾瑟萌星级"

	# 星级名称
	name = models.CharField(max_length=2, verbose_name="星级名称")

	# 星级颜色（#ABCDEF）
	color = models.CharField(max_length=7, null=False, default='#000000', verbose_name="星级颜色")

	# 最大等级
	max_level = models.PositiveSmallIntegerField(default=0, verbose_name="最大等级")

	# 等级经验计算因子
	# {'a', 'b', 'c'}
	level_exp_factors = jsonfield.JSONField(default={}, verbose_name="等级经验计算因子")

	def __str__(self):
		return self.name

	# 转换属性为 dict
	def _convertParamsToDict(self):

		data = dict()

		data['bases'] = ModelUtils.objectsToDict(self.paramBaseRanges())
		data['rates'] = ModelUtils.objectsToDict(self.paramRateRanges())

		return data

	def convertToDict(self):

		return {
			'id': self.id,
			'name': self.name,
			'color': self.color,
			'max_level': self.max_level,
			'level_exp_factors': self.level_exp_factors,
			'param_ranges': self._convertParamsToDict(),
		}

	# 获取所有的属性基本值
	def paramBaseRanges(self):
		return self.exerparambaserange_set.all()

	# 获取所有的属性成长率
	def paramRateRanges(self):
		return self.exerparamraterange_set.all()


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

		verbose_name = verbose_name_plural = "有限物品"

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
	icon = models.ImageField(upload_to=SkillImageUpload('icon'), verbose_name="图标")

	# 技能动画
	ani = models.ImageField(upload_to=SkillImageUpload('ani'), verbose_name="技能动画")

	# 击中动画
	effect = models.ImageField(upload_to=SkillImageUpload('effect'), verbose_name="击中动画")

	# 道具类型
	@classmethod
	def itemType(cls): return ItemType.ExerSkill

	# 对应的容器物品类型（枚举）
	@classmethod
	def contItemType(cls): return ContItemType.ExerSkillSlotItem

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

		res['exermon_id'] = self.o_exermon_id
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
		res['icon'] = self.convertIconToBase64()

		return res


# ===================================================
#  艾瑟萌技能槽
# ===================================================
class ExerSkillSlot(SlotContainer):

	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌技能槽"

	# 默认容量
	DEFAULT_CAPACITY = 3

	# 艾瑟萌
	exermon = models.OneToOneField('exermon_module.PlayerExermon',
								   on_delete=models.CASCADE, verbose_name="艾瑟萌")

	# 容器类型
	@classmethod
	def containerType(cls): return ContainerType.ExerSkillSlot

	# 容器接受物品类型（数组）
	@classmethod
	def acceptTypes(cls): return []

	# 容器接受物品基类
	@classmethod
	def baseContItemClass(cls): return None

	# 获取物品对应的容器项
	def contItemClass(self, **kwargs): return ExerSkillSlotItem

	# 获取容器容量（0为无限）
	def getCapacity(self): return self.DEFAULT_CAPACITY

	# 创建一个背包（创建角色时候执行）
	@classmethod
	def _create(cls, exermon):
		slot: cls = super()._create()
		slot.exermon = exermon
		return slot


# ===================================================
#  艾瑟萌技能槽项表
# ===================================================
class ExerSkillSlotItem(SlotContItem):

	class Meta:

		verbose_name = verbose_name_plural = "艾瑟萌技能槽项"

	# 艾瑟萌
	exermon = models.ForeignKey('exermon_module.PlayerExermon',
								   on_delete=models.CASCADE, verbose_name="艾瑟萌")

	# 技能
	skill = models.ForeignKey('ExerSkill', on_delete=models.CASCADE, verbose_name="技能")

	# 使用次数
	use_count = models.PositiveIntegerField(default=0, verbose_name="使用次数")

	skills = None

	# 容器类型
	@classmethod
	def containerType(cls): return ContainerType.ExerSlot

	# 容器接受物品类型（数组）
	@classmethod
	def acceptTypes(cls): return [ItemType.ExerSkill]

	def __init__(self, *args, **kwargs):
		super().__init__(*args, **kwargs)
		self.param_val = {}

	# 移动容器项
	def transfer(self, container, **kwargs):
		container: ExerSkillSlot = container.targetContainer()
		super().transfer(container, **kwargs)
		self.exermon = container.exermon
		self.skills = self.exermon.exermon.exerskill_set.all()

	# 配置索引
	def setupIndex(self, index):
		if self.skills is None:
			self.skills = self.exermon.exermon.exerskill_set.all()

		self.skill = self.skills[index]

	# 装备
	def equip(self, player_exer=None, player_gift=None, **kwargs):
		if player_exer is not None:
			self.exermon = player_exer
			self.exermon.transfer(self.container)

		if player_gift is not None:
			self.gift = player_gift
			self.gift.transfer(self.container)

	# 卸下
	def dequip(self, exermon=False, gift=False,  **kwargs):
		player_exer = player_gift = None

		if exermon:
			self.exermon.remove()
			player_exer = self.exermon
			self.exermon = None

		if gift:
			self.gift.remove()
			player_gift = self.gift
			self.gift = None

		return player_exer, player_gift
