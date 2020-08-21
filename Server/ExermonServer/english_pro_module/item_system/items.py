from item_module.models import *
from item_module.manager import *

from game_module.models import StaticData


# ===================================================
#  使用效果编号枚举
# ===================================================
class ExerProEffectCode(Enum):
	Unset = 0  # 空

	Attack = 1  # 造成伤害
	AttackSlash = 2  # 造成伤害（完美斩击）
	AttackBlack = 3  # 造成伤害（黑旋风）
	AttackWave = 4  # 造成伤害（波动拳）
	AttackRite = 5  # 造成伤害（仪式匕首）

	Recover = 100  # 回复体力值

	AddParam = 200  # 增加能力值
	AddMHP = 201  # 获得MHP
	AddPower = 202  # 获得力量
	AddDefense = 203  # 获得格挡
	AddAgile = 204  # 获得敏捷
	AddParamUrgent = 205  # 增加能力值（紧急按钮）

	TempAddParam = 210  # 临时增加能力值
	TempAddMHP = 211  # 临时获得MHP
	TempAddPower = 212  # 临时获得力量
	TempAddDefense = 213  # 临时获得格挡
	TempAddAgile = 214  # 临时获得敏捷

	AddState = 220  # 增加状态
	RemoveState = 221  # 移除状态
	RemoveNegaState = 222  # 移除消极状态

	AddEnergy = 230  # 回复能量

	DrawCards = 300  # 抽取卡牌
	ConsumeCards = 310  # 消耗卡牌

	ChangeCost = 400  # 更改耗能
	ChangeCostDisc = 401  # 更改耗能（发现）
	ChangeCostCrazy = 402  # 更改耗能（疯狂）

	GainGold = 500  # 获得金币
	GainCard = 510  # 获得卡牌
	DropCard = 511  # 失去卡牌
	CopyCard = 512  # 复制卡牌
	GainItem = 520  # 获得道具
	LoseItem = 521  # 失去道具
	GainPotion = 530  # 获得药水
	LosePotion = 531  # 失去药水


# ===================================================
#  特训使用效果表
# ===================================================
class ExerProEffect(BaseModel):
	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "特训使用效果"

	CODES = [
		(ExerProEffectCode.Unset.value, '空'),

		(ExerProEffectCode.Attack.value, '造成伤害'),
		(ExerProEffectCode.AttackSlash.value, '造成伤害（完美斩击）'),
		(ExerProEffectCode.AttackBlack.value, '造成伤害（黑旋风）'),
		(ExerProEffectCode.AttackWave.value, '造成伤害（波动拳）'),
		(ExerProEffectCode.AttackRite.value, '造成伤害（仪式匕首）'),

		(ExerProEffectCode.Recover.value, '回复体力值'),

		(ExerProEffectCode.AddParam.value, '增加能力值'),
		(ExerProEffectCode.AddMHP.value, '获得MHP'),
		(ExerProEffectCode.AddPower.value, '获得力量'),
		(ExerProEffectCode.AddDefense.value, '获得格挡'),
		(ExerProEffectCode.AddAgile.value, '获得敏捷'),
		(ExerProEffectCode.AddParamUrgent.value, '增加能力值（紧急按钮）'),

		(ExerProEffectCode.TempAddParam.value, '临时增加能力值'),
		(ExerProEffectCode.TempAddMHP.value, '临时获得MHP'),
		(ExerProEffectCode.TempAddPower.value, '临时获得力量'),
		(ExerProEffectCode.TempAddDefense.value, '临时获得格挡'),
		(ExerProEffectCode.TempAddAgile.value, '临时获得敏捷'),

		(ExerProEffectCode.AddState.value, '增加状态'),
		(ExerProEffectCode.RemoveState.value, '移除状态'),
		(ExerProEffectCode.RemoveNegaState.value, '移除消极状态'),

		(ExerProEffectCode.AddEnergy.value, '回复能量'),

		(ExerProEffectCode.DrawCards.value, '抽取卡牌'),
		(ExerProEffectCode.ConsumeCards.value, '消耗卡牌'),

		(ExerProEffectCode.ChangeCost.value, '更改耗能'),
		(ExerProEffectCode.ChangeCostDisc.value, '更改耗能（发现）'),
		(ExerProEffectCode.ChangeCostCrazy.value, '更改耗能（疯狂）'),

		(ExerProEffectCode.GainGold.value, '获得金币'),
		(ExerProEffectCode.GainCard.value, '获得卡牌'),
		(ExerProEffectCode.DropCard.value, '失去卡牌'),
		(ExerProEffectCode.CopyCard.value, '复制卡牌'),
		(ExerProEffectCode.GainItem.value, '获得道具'),
		(ExerProEffectCode.LoseItem.value, '失去道具'),
		(ExerProEffectCode.GainPotion.value, '获得药水'),
		(ExerProEffectCode.LosePotion.value, '失去药水'),
	]

	KEY_NAME = 'effects'

	# 效果编号
	code = models.PositiveSmallIntegerField(default=0, choices=CODES, verbose_name="效果编号")

	# 效果参数
	params = jsonfield.JSONField(default=[], verbose_name="效果参数")


# ===================================================
#  使用效果编号枚举
# ===================================================
class ExerProTraitCode(Enum):
	Unset = 0  # 空

	DamagePlus = 1  # 攻击伤害加成
	HurtPlus = 2  # 受到伤害加成
	RecoverPlus = 3  # 回复加成

	RestRecoverPlus = 4  # 回复加成（休息据点）

	RoundEndRecover = 5  # 回合结束HP回复
	RoundStartRecover = 6  # 回合开始HP回复
	BattleEndRecover = 7  # 战斗结束HP回复
	BattleStartRecover = 8  # 战斗开始HP回复

	ParamAdd = 9  # 属性加成值
	ParamRoundAdd = 10  # 回合属性加成值
	ParamBattleAdd = 11  # 战斗属性加成值

	RoundDrawCards = 12  # 回合开始抽牌数加成
	BattleDrawCards = 13  # 战斗开始抽牌数加成


# ===================================================
#  特训特性表
# ===================================================
class ExerProTrait(BaseModel):
	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "特训使用效果"

	CODES = [
		(ExerProTraitCode.Unset.value, '空'),

		(ExerProTraitCode.DamagePlus.value, '攻击伤害加成'),
		(ExerProTraitCode.HurtPlus.value, '受到伤害加成'),
		(ExerProTraitCode.RecoverPlus.value, '回复加成'),

		(ExerProTraitCode.RestRecoverPlus.value, '回复加成（休息据点）'),

		(ExerProTraitCode.RoundEndRecover.value, '回合结束HP回复'),
		(ExerProTraitCode.RoundStartRecover.value, '回合开始HP回复'),
		(ExerProTraitCode.BattleEndRecover.value, '战斗结束HP回复'),
		(ExerProTraitCode.BattleStartRecover.value, '战斗开始HP回复'),

		(ExerProTraitCode.ParamAdd.value, '属性加成值'),
		(ExerProTraitCode.ParamRoundAdd.value, '回合属性加成值'),
		(ExerProTraitCode.ParamBattleAdd.value, '战斗属性加成值'),

		(ExerProTraitCode.RoundDrawCards.value, '回合开始抽牌数加成'),
		(ExerProTraitCode.BattleDrawCards.value, '战斗开始抽牌数加成'),
	]

	KEY_NAME = 'traits'

	# 效果编号
	code = models.PositiveSmallIntegerField(default=0, choices=CODES, verbose_name="效果编号")

	# 效果参数
	params = jsonfield.JSONField(default=[], verbose_name="效果参数")


# ===================================================
#  特训物品星级表
# ===================================================
class ExerProItemStar(StaticData):
	class Meta:
		verbose_name = verbose_name_plural = "特训物品星级"

	LIST_DISPLAY_APPEND = ['adminColor']

	# 星级颜色（#ABCDEF）
	color = models.CharField(max_length=7, null=False, default='#000000', verbose_name="星级颜色")

	def __str__(self):
		return self.name

	# 管理界面用：显示星级颜色
	def adminColor(self):
		from django.utils.html import format_html

		res = '<div style="background: %s; width: 48px; height: 24px;"></div>' % self.color

		return format_html(res)

	adminColor.short_description = "星级颜色"


# ===================================================
#  基本特训物品表
# ===================================================
class BaseExerProItem(BaseItem):
	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "特训物品"

	# 图标索引
	icon_index = models.PositiveSmallIntegerField(default=0, verbose_name="图标索引")

	# 起手动画索引
	start_ani_index = models.PositiveSmallIntegerField(default=0, verbose_name="起手动画索引")

	# 目标动画索引
	target_ani_index = models.PositiveSmallIntegerField(default=0, verbose_name="目标动画索引")

	# 物品星级（稀罕度）
	star = models.ForeignKey("ExerProItemStar", on_delete=models.CASCADE, verbose_name="星级")

	# 金币（0表示不可购买）
	# gold = models.PositiveSmallIntegerField(default=0, verbose_name="金币")

	def effects(self):
		raise NotImplementedError


# ===================================================
#  特训物品特性表
# ===================================================
class ExerProItemTrait(ExerProTrait):
	class Meta:
		verbose_name = verbose_name_plural = "特训物品特性"

	# 物品
	item = models.ForeignKey('ExerProItem', on_delete=models.CASCADE,
							 verbose_name="物品")


# ===================================================
#  特训物品表
# ===================================================
@ItemManager.registerItem("特训物品")
class ExerProItem(BaseExerProItem):

	def effects(self):
		return None

	@CacheHelper.staticCache
	def traits(self):
		return self.exerproitemtrait_set.all()


# ===================================================
#  特训药水使用效果表
# ===================================================
class ExerProPotionEffect(ExerProEffect):
	class Meta:
		verbose_name = verbose_name_plural = "特训药水使用效果"

	# 物品
	item = models.ForeignKey('ExerProPotion', on_delete=models.CASCADE,
							 verbose_name="物品")


# ===================================================
#  特训药水表
# ===================================================
@ItemManager.registerItem("特训药水")
class ExerProPotion(BaseExerProItem):

	# def convert(self):
	# 	"""
	# 	转化为字典
	# 	Returns:
	# 		返回转化后的字典
	# 	"""
	# 	res = super().convert()
	#
	# 	return res

	@CacheHelper.staticCache
	def effects(self):
		return self.exerpropotioneffect_set.all()


# ===================================================
#  特训卡片使用效果表
# ===================================================
class ExerProCardEffect(ExerProEffect):
	class Meta:
		verbose_name = verbose_name_plural = "特训卡片使用效果"

	# 物品
	item = models.ForeignKey('ExerProCard', on_delete=models.CASCADE,
							 verbose_name="物品")


# ===================================================
#  卡片类型枚举
# ===================================================
class ExerProCardType(Enum):
	Attack = 1  # 攻击
	Skill = 2  # 技能
	Ability = 3  # 能力
	Evil = 4  # 诅咒


# ===================================================
#  卡片类目标举
# ===================================================
class ExerProCardTarget(Enum):
	Default = 0  # 默认
	One = 1  # 单体
	All = 2  # 群体


# ===================================================
#  特训卡片表
# ===================================================
@ItemManager.registerItem("特训卡片")
class ExerProCard(BaseExerProItem):

	CARD_TYPES = [
		(ExerProCardType.Attack.value, '攻击'),
		(ExerProCardType.Skill.value, '技能'),
		(ExerProCardType.Ability.value, '能力'),
		(ExerProCardType.Evil.value, '诅咒'),
	]

	TARGETS = [
		(ExerProCardTarget.Default.value, '默认'),
		(ExerProCardTarget.One.value, '单体'),
		(ExerProCardTarget.All.value, '群体'),
	]

	NOT_EXIST_ERROR = ErrorType.ExerProCardNotExist

	# 消耗能量
	cost = models.PositiveSmallIntegerField(default=1, verbose_name="消耗能量")

	# 卡片类型
	card_type = models.PositiveSmallIntegerField(default=ExerProCardType.Attack.value,
												 choices=CARD_TYPES, verbose_name="卡片类型")

	# 卡牌皮肤索引
	skin_index = models.PositiveSmallIntegerField(default=0, verbose_name="卡牌皮肤索引")

	# 固有
	inherent = models.BooleanField(default=False, verbose_name="固有")

	# 消耗（一次性的）
	disposable = models.BooleanField(default=False, verbose_name="消耗")

	# 消耗（一次性的）
	boughtable = models.BooleanField(default=True, verbose_name="可否购买")

	# 性质
	character = models.CharField(default="", blank=True, max_length=32, verbose_name="性质")

	# 目标
	target = models.PositiveSmallIntegerField(default=ExerProCardTarget.Default.value,
											  choices=TARGETS, verbose_name="目标")

	@CacheHelper.staticCache
	def effects(self):
		return self.exerprocardeffect_set.all()

	def isBoughtable(self):
		"""
		能否购买
		"""
		if self.card_type == ExerProCardType.Evil.value: return False

		return self.boughtable


# ===================================================
#  特训敌人攻击效果表
# ===================================================
class EnemyEffect(ExerProEffect):
	class Meta:
		verbose_name = verbose_name_plural = "特训敌人攻击效果"

	# 敌人
	enemy = models.ForeignKey('ExerProEnemy', on_delete=models.CASCADE,
							  verbose_name="敌人")


# ===================================================
#  敌人行动类型枚举
# ===================================================
class EnemyActionType(Enum):
	Attack = 1  # 攻击
	PowerUp = 2  # 属性提升
	PosStates = 3  # 状态提升
	PowerDown = 4  # 属性削弱
	NegStates = 5  # 状态削弱
	Escape = 6  # 逃跑
	Unset = 7  # 什么都不做


# ===================================================
#  敌人行动表
# ===================================================
class EnemyAction(BaseModel):
	class Meta:
		verbose_name = verbose_name_plural = "敌人行动"

	TYPES = [
		(EnemyActionType.Attack.value, '攻击'),
		(EnemyActionType.PowerUp.value, '属性提升'),
		(EnemyActionType.PosStates.value, '状态提升'),
		(EnemyActionType.PowerDown.value, '属性削弱'),
		(EnemyActionType.NegStates.value, '状态削弱'),
		(EnemyActionType.Escape.value, '逃跑'),
		(EnemyActionType.Unset.value, '无'),
	]

	KEY_NAME = 'actions'

	# 回合
	rounds = jsonfield.JSONField(default=[], verbose_name="回合")

	# 类型
	type = models.PositiveSmallIntegerField(default=EnemyActionType.Unset.value,
											choices=TYPES, verbose_name="类型")

	# 参数
	params = jsonfield.JSONField(default=[], verbose_name="参数")

	# 权重
	rate = models.PositiveSmallIntegerField(default=10, verbose_name="权重")

	# 敌人
	enemy = models.ForeignKey("ExerProEnemy", on_delete=models.CASCADE, verbose_name="敌人")


# ===================================================
#  敌人等级枚举
# ===================================================
class ExerProEnemyType(Enum):
	Normal = 1  # 普通
	Elite = 2  # 精英
	Boss = 3  # BOSS


# ===================================================
#  特训敌人表
# ===================================================
@ItemManager.registerItem("特训敌人")
class ExerProEnemy(BaseItem):

	ENEMY_TYPES = [
		(ExerProEnemyType.Normal.value, '普通'),
		(ExerProEnemyType.Elite.value, '精英'),
		(ExerProEnemyType.Boss.value, 'BOSS'),
	]

	# AUTO_FIELDS_KEY_NAMES = {'type': 'type_'}

	# 等级
	type = models.PositiveSmallIntegerField(default=ExerProEnemyType.Normal.value,
											choices=ENEMY_TYPES, verbose_name="等级")
	type.key_name = '_type'

	# 最大体力值
	mhp = models.PositiveSmallIntegerField(default=100, verbose_name="最大体力值")

	# 力量
	power = models.PositiveSmallIntegerField(default=10, verbose_name="力量")

	# 格挡
	defense = models.PositiveSmallIntegerField(default=10, verbose_name="格挡")

	# 格挡
	character = models.CharField(default="", blank=True, max_length=32, verbose_name="性格")

	@CacheHelper.staticCache
	def actions(self):
		"""
		获取敌人的行动计划
		Returns:
			返回敌人行动计划
		"""
		return self.enemyaction_set.all()

	@CacheHelper.staticCache
	def effects(self):
		"""
		获取敌人的攻击效果
		Returns:
			返回敌人攻击效果
		"""
		return self.enemyeffect_set.all()


# ===================================================
#  特训状态特性表
# ===================================================
class ExerProStateTrait(ExerProTrait):
	class Meta:
		verbose_name = verbose_name_plural = "特训状态特性"

	# 状态
	item = models.ForeignKey('ExerProState', on_delete=models.CASCADE,
							 verbose_name="状态")


# ===================================================
#  特训状态表
# ===================================================
@ItemManager.registerItem("特训状态")
class ExerProState(BaseItem):

	# 图标索引
	icon_index = models.PositiveSmallIntegerField(default=0, verbose_name="图标索引")

	# 最大状态回合数
	max_turns = models.PositiveSmallIntegerField(default=0, verbose_name="最大状态回合数")

	# 是否负面状态
	is_nega = models.BooleanField(default=False, verbose_name="是否负面状态")

	@CacheHelper.staticCache
	def traits(self):
		return self.exerprostatetrait_set.all()
