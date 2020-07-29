from game_module.models import ParamValue, ParamRate, \
	EquipParamValue, EquipParamRate
from item_module.models import *

# from utils.cache_utils import CacheHelper


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
@ItemManager.registerItem("艾瑟萌")  #, ContItems.PlayerExermon)
class Exermon(BaseItem, ParamsObject):

	NAME_LEN = 4

	TYPES = [
		(ExermonType.Initial.value, '初始艾瑟萌'),
		(ExermonType.Wild.value, '野生艾瑟萌'),
		(ExermonType.Task.value, '剧情艾瑟萌'),
		(ExermonType.Rare.value, '稀有艾瑟萌'),
	]

	LIST_DISPLAY_APPEND = ['adminParamBases', 'adminParamRates']

	# 品种
	animal = models.CharField(max_length=24, verbose_name="品种")

	# 艾瑟萌星级
	star = models.ForeignKey('game_module.ExerStar', on_delete=models.CASCADE, verbose_name="艾瑟萌星级")

	# 科目
	subject = models.ForeignKey('game_module.Subject', on_delete=models.CASCADE, verbose_name="科目")

	# 全身像
	# full = models.ImageField(upload_to=ExermonImageUpload('full'), null=True, blank=True,
	# 						 verbose_name="全身像")
	#
	# # 缩略图
	# icon = models.ImageField(upload_to=ExermonImageUpload('icon'), null=True, blank=True,
	# 						 verbose_name="缩略图")
	#
	# # 战斗图
	# battle = models.ImageField(upload_to=ExermonImageUpload('battle'), null=True, blank=True,
	# 						   verbose_name="战斗图")

	# 艾瑟萌类型
	e_type = models.PositiveSmallIntegerField(default=ExermonType.Initial.value,
											choices=TYPES, verbose_name="艾瑟萌类型")

	@classmethod
	def paramBaseClass(cls):
		return ExerParamBase

	@classmethod
	def paramRateClass(cls):
		return ExerParamRate

	# 用于获取属性值
	# def __getattr__(self, item):
	# 	type = item[4:]
	#
	# 	if type == 'base':
	# 		return self.paramBase(attr=item[:3])
	#
	# 	if type == 'rate':
	# 		return self.paramRate(attr=item[:3])
	#
	# 	return super().__getattr__(item)

	# 转换属性为 dict
	def _convertParamsToDict(self, res):
		res['base_params'] = ModelUtils.objectsToDict(self.paramBases())
		res['rate_params'] = ModelUtils.objectsToDict(self.paramRates())

	# 转化为 dict
	def convert(self):
		res = super().convert()

		res['animal'] = self.animal
		res['star_id'] = self.star_id
		res['subject_id'] = self.subject_id
		res['e_type'] = self.e_type

		self._convertParamsToDict(res)

		return res

	# 获取艾瑟萌的技能
	@CacheHelper.staticCache
	def skills(self):
		return self.exerskill_set.all()

	# 获取所有的属性基本值
	@CacheHelper.staticCache
	def _paramBases(self):
		return self.exerparambase_set.all()

	# 获取所有的属性成长率
	@CacheHelper.staticCache
	def _paramRates(self):
		return self.exerparamrate_set.all()

	# # 获取属性基本值
	# def paramBase(self, param_id=None, attr=None):
	# 	param = None
	# 	if param_id is not None:
	# 		param = self.paramBases().filter(param_id=param_id)
	# 	if attr is not None:
	# 		param = self.paramBases().filter(param__attr=attr)
	#
	# 	if param is None or not param.exists(): return 0
	#
	# 	return param.first().getValue()
	#
	# # 获取属性成长值
	# def paramRate(self, param_id=None, attr=None):
	# 	param = None
	# 	if param_id is not None:
	# 		param = self.paramRates().filter(param_id=param_id)
	# 	if attr is not None:
	# 		param = self.paramRates().filter(param__attr=attr)
	#
	# 	if param is None or not param.exists(): return 0
	#
	# 	return param.first().getValue()

	# 战斗力
	# def battlePoint(self):
	# 	from utils.calc_utils import BattlePointCalc
	# 	return BattlePointCalc.calc(self.paramBase)


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
@ItemManager.registerItem("艾瑟萌天赋")  #, ContItems.PlayerExerGift)
class ExerGift(BaseItem, ParamsObject):

	LIST_DISPLAY_APPEND = ['adminParamRates']

	TYPES = [
		(ExerGiftType.Initial.value, '初始天赋'),
		(ExerGiftType.Other.value, '其他天赋'),
	]

	# 艾瑟萌星级
	star = models.ForeignKey('game_module.ExerGiftStar', on_delete=models.CASCADE, verbose_name="艾瑟萌星级")

	# 标志颜色（#ABCDEF）
	color = models.CharField(max_length=7, null=False, default='#FFFFFF', verbose_name="标志颜色")

	# 艾瑟萌天赋类型
	g_type = models.PositiveSmallIntegerField(default=ExerGiftType.Initial.value,
											choices=TYPES, verbose_name="艾瑟萌天赋类型")

	@classmethod
	def paramRateClass(cls):
		return ExerParamRate

	# 管理界面用：显示天赋颜色
	def adminColor(self):
		from django.utils.html import format_html

		res = '<div style="background: %s; width: 48px; height: 24px;"></div>' % self.color

		return format_html(res)

	adminColor.short_description = "天赋颜色"

	# 用于获取属性值
	# def __getattr__(self, item):
	# 	type = item[4:]
	#
	# 	if type == 'rate':
	# 		return self.paramRate(attr=item[:3])
	#
	# 	return super().__getattr__(item)

	# 转化为 dict
	def convert(self, **kwargs):
		res = super().convert(**kwargs)

		res['star_id'] = self.star_id
		res['color'] = self.color
		res['g_type'] = self.g_type
		res['params'] = ModelUtils.objectsToDict(self.paramRates())

		return res

	# 获取所有的属性成长加成率
	@CacheHelper.staticCache
	def _paramRates(self):
		return self.giftparamrate_set.all()

	# # 获取属性成长加成率
	# def paramRate(self, param_id=None, attr=None):
	# 	param = None
	# 	if param_id is not None:
	# 		param = self.paramRates().filter(param_id=param_id)
	# 	if attr is not None:
	# 		param = self.paramRates().filter(param__attr=attr)
	#
	# 	if param is None or not param.exists(): return 0
	#
	# 	return param.first().getValue()


# ===================================================
#  艾瑟萌碎片表
# ===================================================
@ItemManager.registerItem("艾瑟萌碎片")  #, ContItems.ExerFragPackItem)
class ExerFrag(BaseItem):

	# 所属艾瑟萌
	o_exermon = models.ForeignKey('Exermon', on_delete=models.CASCADE,
								 verbose_name="所属艾瑟萌")

	# 所需碎片数目
	count = models.PositiveSmallIntegerField(default=16, verbose_name="所需碎片数")

	# 出售价格（出售固定为金币，为0则不可出售）
	sell_price = models.PositiveIntegerField(default=0, verbose_name="出售价格")

	# 转化为 dict
	def convert(self, **kwargs):

		res = super().convert(**kwargs)

		res['eid'] = self.o_exermon_id
		res['sell_price'] = self.sell_price
		res['count'] = self.count

		return res

	# 最大叠加数量（为0则不限）
	def maxCount(self): return 0


# ===================================================
#  技能使用效果表
# ===================================================
class ExerSkillEffect(BaseEffect):
	class Meta:
		verbose_name = verbose_name_plural = "技能使用效果"

	# 物品
	item = models.ForeignKey('ExerSkill', on_delete=models.CASCADE, verbose_name="物品")


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
@ItemManager.registerItem("艾瑟萌技能")  #, ContItems.ExerSkillSlotItem)
class ExerSkill(BaseItem):

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

	LIST_DISPLAY_APPEND = ['adminEffects']

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
	target_type = models.PositiveSmallIntegerField(default=TargetType.Enemy.value, choices=TARGET_TYPES,
												   null=True, blank=True, verbose_name="目标")

	# 命中类型
	hit_type = models.PositiveSmallIntegerField(default=HitType.HPDamage.value, choices=HIT_TYPES,
												null=True, blank=True, verbose_name="命中类型")

	# 吸收率（*100）（若命中类型为吸收）
	drain_rate = models.PositiveSmallIntegerField(default=0, null=True, blank=True, verbose_name="吸收率")

	# 攻击比率（*100）
	atk_rate = models.PositiveSmallIntegerField(default=100, null=True, blank=True, verbose_name="攻击比率")

	# 防御比率（*100）
	def_rate = models.PositiveSmallIntegerField(default=100, null=True, blank=True, verbose_name="防御比率")

	# 命中率加成（判定时用于减少对方的回避率）（*100）
	hit_rate = models.SmallIntegerField(default=0, null=True, blank=True, verbose_name="命中率加成")

	# 暴击率加成（判定时用于增加己方的暴击率）（*100）
	cri_rate = models.SmallIntegerField(default=0, null=True, blank=True, verbose_name="暴击率加成")

	# 附加公式
	# 说明：a, b 分别为 攻击方, 目标的 TempParam 对象
	plus_formula = models.CharField(default="0", max_length=256, null=True, blank=True, verbose_name="附加公式")

	# # 技能图标
	# icon = models.ImageField(upload_to=SkillImageUpload('icon'),
	# 						 null=True, blank=True, verbose_name="图标")
	#
	# # 技能动画
	# ani = models.ImageField(upload_to=SkillImageUpload('ani'),
	# 						 null=True, blank=True, verbose_name="技能动画")
	#
	# # 击中动画
	# target_ani = models.ImageField(upload_to=SkillImageUpload('target'),
	# 						 null=True, blank=True, verbose_name="击中动画")

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

		res['eid'] = self.o_exermon_id
		res['passive'] = self.passive
		res['next_skill_id'] = self.next_skill_id
		res['need_count'] = self.need_count
		res['mp_cost'] = self.mp_cost
		res['rate'] = self.rate
		# res['timing'] = self.timing
		res['freeze'] = self.freeze
		res['max_use_count'] = self.max_use_count
		res['target'] = self.target_type
		res['hit_type'] = self.hit_type
		res['drain_rate'] = self.drain_rate
		res['atk_rate'] = self.atk_rate
		res['def_rate'] = self.def_rate
		res['plus_formula'] = self.plus_formula
		res['effects'] = effects

		return res

	# 获取所有的效果
	@CacheHelper.staticCache
	def effects(self):
		return self.skilleffect_set.all()


# ===================================================
#  艾瑟萌物品使用效果表
# ===================================================
class ExerItemEffect(BaseEffect):

	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌物品使用效果"

	# 物品
	item = models.ForeignKey('ExerItem', on_delete=models.CASCADE,
							 verbose_name="物品")


# ===================================================
#  艾瑟萌物品价格
# ===================================================
class ExerItemPrice(Currency):

	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌物品价格"

	# 物品
	item = models.OneToOneField('ExerItem', on_delete=models.CASCADE,
							 verbose_name="物品")


# ===================================================
#  艾瑟萌物品表
# ===================================================
@ItemManager.registerItem("艾瑟萌物品")  #, ContItems.ExerPackItem)
class ExerItem(UsableItem):

	# 使用几率（*100）
	rate = models.PositiveSmallIntegerField(default=0, verbose_name="使用几率")

	# 转化为 dict
	def convert(self, **kwargs):
		res = super().convert(**kwargs)

		res['rate'] = self.rate

		return res

	# 获取所有的效果
	@CacheHelper.staticCache
	def effects(self):
		return self.exeritemeffect_set.all()

	# 购买价格
	@CacheHelper.staticCache
	def buyPrice(self):
		try: return self.exeritemprice
		except ExerItemPrice.DoesNotExist: return None


# ===================================================
#  艾瑟萌装备等级属性值表
# ===================================================
class ExerEquipLevelParam(EquipParamRate):

	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌装备等级属性值"

	# 装备
	equip = models.ForeignKey("ExerEquip", on_delete=models.CASCADE, verbose_name="装备")


# ===================================================
#  艾瑟萌装备属性值表
# ===================================================
class ExerEquipBaseParam(EquipParamValue):

	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌装备基础属性值"

	# 装备
	equip = models.ForeignKey("ExerEquip", on_delete=models.CASCADE, verbose_name="装备")

	# 最大值
	def maxVal(self):
		return None

	# 最小值
	def minVal(self):
		return None


# ===================================================
#  艾瑟萌装备价格
# ===================================================
class ExerEquipPrice(Currency):

	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌装备价格"

	# 物品
	item = models.OneToOneField('ExerEquip', on_delete=models.CASCADE,
							 verbose_name="物品")


# ===================================================
#  艾瑟萌装备
# ===================================================
@ItemManager.registerItem("艾瑟萌装备")  #, ContItems.ExerPackEquip)
class ExerEquip(EquipableItem):

	# 装备类型
	e_type = models.ForeignKey("game_module.ExerEquipType",
							   on_delete=models.CASCADE, verbose_name="装备类型")

	# 转化为 dict
	def convert(self, **kwargs):
		res = super().convert(**kwargs)

		res['e_type'] = self.e_type_id

		return res

	@classmethod
	def levelParamClass(cls):
		return ExerEquipLevelParam

	@classmethod
	def baseParamClass(cls):
		return ExerEquipBaseParam

	# 获取所有的属性基本值
	@CacheHelper.staticCache
	def _levelParams(self):
		return self.exerequiplevelparam_set.all()

	# 获取所有的属性基本值
	@CacheHelper.staticCache
	def _baseParams(self):
		return self.exerequipbaseparam_set.all()

	# 购买价格
	@CacheHelper.staticCache
	def buyPrice(self):
		try: return self.exerequipprice
		except ExerEquipPrice.DoesNotExist: return None