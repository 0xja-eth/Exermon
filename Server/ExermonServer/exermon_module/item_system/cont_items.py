from item_module.models import *
from item_module.manager import *

from game_module.models import ParamValue, ParamRate

import exermon_module.item_system.items as Items
import exermon_module.item_system.containers as Containers

import datetime


# ===================================================
#  玩家艾瑟萌附加属性值表
# ===================================================
class PlayerExerParamBase(ParamValue):

	class Meta:
		verbose_name = verbose_name_plural = "玩家艾瑟萌附加属性值"

	# 玩家艾瑟萌
	player_exer = models.ForeignKey("PlayerExermon", on_delete=models.CASCADE, verbose_name="玩家艾瑟萌")

	# 过期时间
	expires_time = models.DateTimeField(null=True, verbose_name="过期时间")

	# 是否过期
	def isOutOfDate(self):
		if self.expires_time is None: return False
		return datetime.datetime.now() > self.expires_time

	def remove(self):
		"""
		移除容器项（从当前容器中移除）
		"""
		self.player_exer = None

	def save(self, judge=True, **kwargs):
		if judge and self.player_exer is None:
			self.delete_save = False
			if self.id is not None: self.delete()
		else: super().save(**kwargs)


# ===================================================
#  玩家艾瑟萌属性加成表
# ===================================================
class PlayerExerParamRate(ParamRate):

	class Meta:
		verbose_name = verbose_name_plural = "玩家艾瑟萌属性加成"

	# 玩家艾瑟萌
	player_exer = models.ForeignKey("PlayerExermon", on_delete=models.CASCADE, verbose_name="玩家艾瑟萌")

	# 过期时间
	expires_time = models.DateTimeField(null=True, verbose_name="过期时间")

	# 是否过期
	def isOutOfDate(self):
		if self.expires_time is None: return False
		return datetime.datetime.now() > self.expires_time

	def remove(self):
		"""
		移除容器项（从当前容器中移除）
		"""
		self.player_exer = None

	def save(self, judge=True, **kwargs):
		if judge and self.player_exer is None:
			self.delete_save = False
			if self.id is not None: self.delete()
		else: super().save(**kwargs)


# ===================================================
#  玩家艾瑟萌表
# ===================================================
@ItemManager.registerPackContItem("玩家艾瑟萌",
	Containers.ExerHub, Items.Exermon)
class PlayerExermon(PackContItem, ParamsObject):

	# 属性缓存键
	PARAMS_CACHE_KEY = 'params'

	# 附加属性缓存键
	PLUS_PARAM_VALS_CACHE_KEY = 'plus_param_vals'

	# 附加属性缓存键
	PLUS_PARAM_RATES_CACHE_KEY = 'plus_param_rates'

	# 移除的附加属性缓存键
	REMOVED_PLUS_PARAMS_CACHE_KEY = 'removed_plus_params'

	# 艾瑟萌昵称
	nickname = models.CharField(null=True, blank=True, max_length=4, verbose_name="艾瑟萌昵称")

	# 经验值（相对）
	exp = models.PositiveIntegerField(default=0, verbose_name="经验值")

	# 等级
	level = models.PositiveSmallIntegerField(default=1, verbose_name="等级")

	# region 重写配置

	@classmethod
	def _cacheForeignKeyModels(cls):
		return [PlayerExerParamBase, PlayerExerParamRate]

	@classmethod
	def paramBaseClass(cls):
		return Items.ExerParamBase
	
	@classmethod
	def paramRateClass(cls):
		return Items.ExerParamRate

	# 创建之后调用
	def _afterCreated(self, **kwargs):
		self._createExerSkillSlot()

	def _createExerSkillSlot(self):
		Containers.ExerSkillSlot.create(player_exer=self)

	# 获取艾瑟萌技能槽
	def exerSkillSlot(self):
		return self.getContainer(Containers.ExerSkillSlot)

	# 艾瑟萌
	def exermon(self): return self.item

	# endregion

	# 转换属性为 dict
	def _convertParamsToDict(self, res):
		res['param_values'] = ModelUtils.objectsToDict(self.paramVals())
		res['rate_params'] = ModelUtils.objectsToDict(self.paramRates())

		res['plus_param_values'] = ModelUtils.objectsToDict(self.plusParamVals())
		res['plus_param_rates'] = ModelUtils.objectsToDict(self.plusParamRates())

	def _convertCustomAttrs(self, res, type=None, **kwargs):
		super()._convertCustomAttrs(res, type, **kwargs)

		from utils.calc_utils import ExermonLevelCalc

		star = self.item.star
		next = ExermonLevelCalc.getDetlaExp(star, self.level)

		exer_skill_slot = ModelUtils.objectToDict(
			self.exerSkillSlot(), type="items")

		res['next'] = int(next)
		res['exer_skill_slot'] = exer_skill_slot

		self._convertParamsToDict(res)

	# 获取属性当前值（计算）
	def _paramVal(self, **kwargs):
		from utils.calc_utils import ExermonParamCalc

		base = self.paramBase(**kwargs)
		rate = self.paramRate(**kwargs)

		plus = self.plusParamVal(**kwargs)
		plus_rate = self.plusParamRate(**kwargs)

		return ExermonParamCalc.calc(
			base, rate, self.level, plus, plus_rate)

	# 艾瑟萌基础属性
	def _paramBases(self):
		exermon = self.exermon()

		if exermon is None: return []
		return exermon.paramBases()

	# 获取属性成长值
	def _paramRates(self):
		exermon = self.exermon()

		if exermon is None: return []
		return exermon.paramRates()

	# region 附加能力值

	def addPlusParam(self, param_id, val=0, rate=0, count=1):
		"""
		添加附加属性
		Args:
			param_id (int): 属性ID
			val (int): 附加值
			rate (float): 加成率
			count (int): 叠加次数
		"""
		self.addTempPlusParam(param_id, None, val, rate, count)

	def addTempPlusParam(self, param_id, seconds, val=0, rate=0, count=1):
		"""
		临时添加附加属性
		Args:
			param_id (int): 属性ID
			val (int): 附加值
			rate (float): 加成率
			seconds (int): 持续秒数（为 None 则永久）
			count (int): 叠加次数
		"""
		if seconds is None: duration = None
		else:
			duration = datetime.timedelta(0, seconds, 0)

		val *= count
		rate = pow(rate, count)

		if val != 0:
			self._addPlusParam(PlayerExerParamBase,
							   param_id, val, duration)

		if rate != 0:
			self._addPlusParam(PlayerExerParamRate,
							   param_id, rate, duration)

		self._clearParamsCache()

	def _addPlusParam(self, cla, param_id, value, duration):
		now = datetime.datetime.now()

		param = cla(param_id=param_id)
		param.player_exer = self
		param.setValue(value)

		if duration is not None:
			param.expires_time = now + duration

		self._appendModelCache(cla, param)

	# 附加艾瑟萌属性
	def plusParamVal(self, param_id=None, attr=None):
		sum_param: PlayerExerParamBase = None
		values = self.plusParamVals()

		if param_id is not None:
			sum_param = ModelUtils.sum(values, param_id=param_id)

		if attr is not None:
			sum_param = ModelUtils.sum(values, attr=attr)

		if sum_param is None: return 0

		return sum_param.getValue()

	# 附加艾瑟萌属性率
	def plusParamRate(self, param_id=None, attr=None):
		sum_param: PlayerExerParamRate = None
		values = self.plusParamRates()

		if param_id is not None:
			sum_param = ModelUtils.mult(values, param_id=param_id)

		if attr is not None:
			sum_param = ModelUtils.mult(values, attr=attr)

		if sum_param is None: return 1

		return sum_param.getValue()

	# 所有附加的能力值
	def plusParamVals(self, need_filter=True) -> list:
		return self._plusParam(PlayerExerParamBase, need_filter)

	# 所有附加的能力加成率
	def plusParamRates(self, need_filter=True) -> list:
		return self._plusParam(PlayerExerParamRate, need_filter)

	# 所有附加的能力加成率
	def _plusParam(self, cla, need_filter=True) -> list:
		if not need_filter:
			return self._queryModelCache(cla)

		return self._queryModelCache(
			cla, lambda v: not v.isOutOfDate())

	def updatePlusParams(self):
		"""
		更新附加能力值（判断过期自动删除）
		"""
		self._updatePlusParam(PlayerExerParamBase)
		self._updatePlusParam(PlayerExerParamRate)
		
		self._clearParamsCache()

	def _updatePlusParam(self, cla):
		params = self._queryModelCache(cla)

		for param in params:
			if param.isOutOfDate():
				self._removeModelCache(cla, param)

	# endregion

	# endregion

	# region 等级相关

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

		self.exp = 0
		self.level = int(level)
		self.refresh()

	# 更改经验
	def gainExp(self, val):
		self.exp += int(val)
		self.refresh()

	# 刷新等级
	def _refreshLevel(self):
		if self.item is None: return

		from utils.calc_utils import ExermonLevelCalc

		exp = self.exp
		level = self.level
		star = self.item.star
		delta = ExermonLevelCalc.getDetlaExp(star, level)
		if delta < 0: return  # 如果已经满级，跳过

		# 当可以升级的时候
		while exp > delta:
			level += 1  # 升级
			exp -= delta  # 扣除所需的经验
			if level >= star.max_level: break

			# 更新所需经验值
			delta = ExermonLevelCalc.getDetlaExp(star, level)

		if level > self.level:
			self._onUpgrade()

		self.exp = int(exp)
		self.level = int(level)

	# 升级触发事件
	def _onUpgrade(self):
		pass

	# endregion

	# 修改昵称
	def editNickname(self, name):
		self.nickname = name

	# 刷新艾瑟萌
	def refresh(self):
		super().refresh()
		self._clearParamsCache()
		self._refreshLevel()


# ===================================================
#  玩家艾瑟萌天赋
# ===================================================
@ItemManager.registerPackContItem("玩家艾瑟萌天赋",
	Containers.ExerGiftPool, Items.ExerGift)
class PlayerExerGift(PackContItem, ParamsObject):

	@classmethod
	def paramRateClass(cls):
		return Items.ExerParamRate

	# 获取属性成长值
	def _paramRate(self, **kwargs):
		return self.item.paramRate(**kwargs)


# ===================================================
#  艾瑟萌碎片背包物品
# ===================================================
@ItemManager.registerPackContItem("艾瑟萌碎片背包物品",
	Containers.ExerFragPack, Items.ExerFrag)
class ExerFragPackItem(PackContItem): pass


# ===================================================
#  艾瑟萌槽项表
# ===================================================
@ItemManager.registerSlotContItem("艾瑟萌槽项", Containers.ExerSlot,
	player_exer=PlayerExermon, player_gift=PlayerExerGift)
class ExerSlotItem(SlotContItem, ParamsObject):

	# 玩家
	player = models.ForeignKey('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 科目
	# subject = models.ForeignKey('game_module.Subject', on_delete=models.CASCADE, verbose_name="科目")

	# 经验值（累计）
	exp = models.PositiveIntegerField(default=0, verbose_name="经验值")

	# 属性值（缓存）
	init_exer = None

	@classmethod
	def paramBaseClass(cls):
		return Items.ExerParamBase

	@classmethod
	def paramRateClass(cls):
		return Items.ExerParamRate

	# 转换属性为 dict
	def _convertParamsToDict(self, res):
		res['param_values'] = ModelUtils.objectsToDict(self.paramVals())
		res['rate_params'] = ModelUtils.objectsToDict(self.paramRates())

	def _convertCustomAttrs(self, res, type=None, **kwargs):
		super()._convertCustomAttrs(res, type, **kwargs)

		level, next = self.slotLevel(True)

		exer_equip_slot = ModelUtils.objectToDict(self.exerEquipSlot(), type="items")

		res['level'] = level
		res['next'] = next
		res['exer_equip_slot'] = exer_equip_slot

		self._convertParamsToDict(res)

	def playerExer(self) -> PlayerExermon:
		return self.equipItem(0)

	def playerGift(self) -> PlayerExerGift:
		return self.equipItem(1)

	# region 容器相关

	# 配置索引
	def setupIndex(self, index, init_exer: PlayerExermon = None, **kwargs):
		super().setupIndex(index, **kwargs)
		self.subject = init_exer.exermon().subject
		self.init_exer = init_exer

	# 创建之后调用
	def _afterCreated(self, **kwargs):
		container = self.exactlyContainer()
		container.setPlayerExer(self, self.init_exer)

		self._createEquipSlot()

	def _createEquipSlot(self):
		Containers.ExerEquipSlot.create(exer_slot_item=self)

	# 获取艾瑟萌技能槽
	def exerEquipSlot(self):
		return self.getContainer(Containers.ExerEquipSlot)

	# 移动容器项
	def transfer(self, container: Containers.ExerSlot, **kwargs):
		super().transfer(container, **kwargs)
		self.player = container.player

	# endregion

	# region 属性

	def _paramVal(self, **kwargs) -> float:

		from utils.calc_utils import ExerSlotItemParamCalc

		return ExerSlotItemParamCalc.calc(self, **kwargs)

	# 所有属性基础值
	def _paramBases(self):
		player_exer: PlayerExermon = self.player_exer

		if player_exer is None: return []
		return player_exer.paramBases()

	# 所有属性成长值
	def _paramRate(self, **kwargs) -> float:

		from utils.calc_utils import ExerSlotItemParamRateCalc

		return ExerSlotItemParamRateCalc.calc(self, **kwargs)

	# endregion

	# region 等级相关

	# 槽等级（本等级, 下一级所需经验）
	def slotLevel(self, calc_next=False):
		from utils.calc_utils import ExermonSlotLevelCalc

		level = ExermonSlotLevelCalc.calcLevel(self.exp)

		if not calc_next: return level

		next = ExermonSlotLevelCalc.calcExp(level + 1)
		return level, next

	# 艾瑟萌等级
	def exermonLevel(self):
		player_exer = self.playerExer()
		if player_exer is not None:
			return player_exer.level
		return 0

	# 获得经验
	def gainExp(self, slot_exp, exer_exp):
		player_exer: PlayerExermon = self.playerExer()
		if player_exer is not None:
			player_exer.gainExp(exer_exp)

		self.exp += int(slot_exp)
		self.refresh()
		self.save()

	# endregion

	def refresh(self):
		super().refresh()
		self._clearParamsCache()


# ===================================================
#  艾瑟萌技能槽项表
# ===================================================
@ItemManager.registerSlotContItem(
	"艾瑟萌技能槽项", Containers.ExerSkillSlot)
class ExerSkillSlotItem(SlotContItem):

	# 技能
	skill = models.ForeignKey('ExerSkill', on_delete=models.CASCADE,
							  null=True, blank=True, verbose_name="技能")

	# 使用次数
	use_count = models.PositiveIntegerField(default=0, verbose_name="使用次数")

	def isContItemUsable(self, occasion: ItemUseOccasion, **kwargs) -> bool:
		"""
		配置当前物品是否可用
		Args:
			occasion (ItemUseOccasion): 使用场合枚举
			**kwargs (**dict): 拓展参数
		Returns:
			返回当前物品是否可用
		"""
		return occasion == ItemUseOccasion.Battle and \
			   self.skill and not self.skill.passive

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
		skill: Items.ExerSkill = self.skill
		if skill.need_count > 0:

			self.use_count += 1
			if self.use_count >= skill.need_count:
				self.setSkill(skill.next_skill)

	def useItem(self, occasion: ItemUseOccasion, **kwargs):
		"""
		使用物品
		Args:
			occasion (ItemUseOccasion): 使用场合枚举
			**kwargs (**dict): 拓展参数
		"""
		self.ensureContItemUsable(occasion)
		self.useSkill()


# ===================================================
#  艾瑟萌背包装备
# ===================================================
@ItemManager.registerPackContItem("装备背包项",
								  Containers.EquipPack, Items.GameEquip)
class EquipPackItem(PackContItem, EquipParamsObject):

	@classmethod
	def levelParamClass(cls):
		return cls.acceptedItemClass().levelParamClass()

	@classmethod
	def baseParamClass(cls):
		return cls.acceptedItemClass().baseParamClass()

	# 获取等级属性值
	def _levelParam(self, **kwargs):
		return self.item.levelParam(**kwargs)

	# 获取属性值
	def _baseParam(self, **kwargs):
		return self.item.baseParam(**kwargs)

	def refresh(self):
		super().refresh()
		self._clearParamsCache()


# ===================================================
#  艾瑟萌装备槽项
# ===================================================
@ItemManager.registerSlotContItem("艾瑟萌装备槽项",
								  Containers.ExerEquipSlot, pack_equip=EquipPackItem)
class ExerEquipSlotItem(SlotContItem, ParamsObject):

	# AUTO_FIELDS_KEY_NAMES = {'e_type_id': 'e_type'}

	# 装备槽类型
	e_type = models.ForeignKey('game_module.models.GameEquipType', on_delete=models.CASCADE,
							   verbose_name="装备槽类型")
	e_type.key_name = 'e_type'

	@classmethod
	def paramValueClass(cls):
		cla = cls.acceptedEquipItemClasses()[0]
		return cla.baseParamClass()

	# 装备
	def packEquip(self) -> EquipPackItem:
		return self.equipItem(0)

	# 配置索引
	def setupIndex(self, index, **kwargs):
		super().setupIndex(index, **kwargs)
		self.e_type_id = index

	# 获取属性值
	def _paramVal(self, **kwargs) -> float:
		"""
		获取装备属性值
		"""
		from utils.calc_utils import ExerEquipParamCalc
		return ExerEquipParamCalc.calc(self, **kwargs)

	def refresh(self):
		super().refresh()
		self._clearParamsCache()
