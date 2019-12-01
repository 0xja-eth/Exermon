from django.db import models
from django.conf import settings
from utils.model_utils import ItemIconUpload
from utils.exception import ErrorType, ErrorException
from enum import Enum
import jsonfield, os


# region 物品
# ===================================================
#  物品类型枚举
# ===================================================
class ItemType(Enum):
	Unset = 0  # 未设置

	HumanItem = 1 # 人类物品
	HumanUsableItem = 2 # 人类可用物品

	PetItem = 3 # 宠物物品
	PetUsableItem = 4 # 宠物可用物品
	PetEquip = 5 # 宠物装备

	QuestionCard = 6 # 题目卡片

	Pet = 7 # 宠物
	PetSkill = 8 # 宠物技能
	PetGift = 9 # 宠物天赋


# ===================================================
#  基础物品表
# ===================================================
class BaseItem(models.Model):

	class Meta:

		verbose_name = verbose_name_plural = "基础物品"

	Types = [
		(ItemType.Unset.value, '未知物品'),

		(ItemType.HumanItem.value, '人类物品'),
		(ItemType.HumanUsableItem.value, '人类可用物品'),

		(ItemType.PetItem.value, '宠物物品'),
		(ItemType.PetUsableItem.value, '宠物可用物品'),
		(ItemType.PetEquip.value, '宠物装备'),

		(ItemType.QuestionCard.value, '题目卡片'),

		(ItemType.Pet.value, '宠物'),
		(ItemType.PetSkill.value, '宠物技能'),
		(ItemType.PetGift.value, '宠物天赋'),
	]

	# 名称
	name = models.CharField(max_length=24, verbose_name="名称")

	# 描述
	description = models.CharField(max_length=128, verbose_name="描述")

	# 物品标识类型
	type = models.PositiveSmallIntegerField(default=0, choices=Types, verbose_name="物品类型")

	def __str__(self):
		type_name = self.getTypeName()

		return '%d %s（%s）' % (self.id, self.name, type_name)

	# 获取类型名称
	def getTypeName(self):
		for type in self.Types:
			if self.type == type[0]:
				return type[1]

		return self.Types[0][1]

	# 获取目标的物品
	def targetItem(self):
		from pet_module.models import Pet, PetGift, PetSkill

		target = BaseItem

		if self.type == ItemType.HumanItem.value: target = HumanItem
		if self.type == ItemType.PetItem.value: target = PetItem
		if self.type == ItemType.PetEquip.value: target = PetEquip
		if self.type == ItemType.PetSkill.value: target = PetSkill
		# if self.type == ItemType.QuestionCard.value: target = QuestionCard
		if self.type == ItemType.PetGift.value: target = PetGift
		if self.type == ItemType.Pet.value: target = Pet

		return target.objects.get(id=self.id)

	# 转化为 dict
	def _convertToDict(self):
		return {
			'id': self.id,
			'name': self.name,
			'description': self.description,
			'type': self.type
		}

	def convertToDict(self):
		return self.targetItem()._convertToDict()


# ===================================================
#  有限物品表
# ===================================================
class LimitedItem(BaseItem):

	class Meta:

		abstract = True
		verbose_name = verbose_name_plural = "有限物品"

	# 购入价格
	buy_price = models.PositiveIntegerField(default=0, verbose_name="购入价格")

	# 出售价格（为0则不可出售）
	sell_price = models.PositiveIntegerField(default=0, verbose_name="出售价格")

	# 叠加数量
	max_count = models.PositiveSmallIntegerField(default=99, verbose_name="叠加数量")

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
	def _convertToDict(self):
		res = BaseItem._convertToDict(self)

		res['buy_price'] = self.buy_price
		res['sell_price'] = self.sell_price
		res['max_count'] = self.max_count
		res['icon'] = self.convertIconToBase64()

		return res

# ===================================================
#  可用物品使用时机
# ===================================================
class UsableItemTiming(Enum):
	Unset = 0       # 不可用
	Menu = 1        # 仅菜单可用
	Battle = 2      # 仅对战可用
	Any = 3         # 随时可用

	Attack = 4      # 战斗进攻可用
	BeAttacked = 5  # 战斗防御可用

# ===================================================
#  可用物品类型
# ===================================================
class UsableItemType(Enum):
	Auxiliary = 0  # 辅助类
	Attack = 1     # 攻击类
	Defense = 2    # 防守类
	Chest = 3      # 宝箱类
	Material = 4   # 材料类
	Task = 5       # 任务类
	Special = 6    # 活动类

# ===================================================
#  可用物品表
# ===================================================
class UsableItem(LimitedItem):

	class Meta:

		verbose_name = verbose_name_plural = "可用物品"

	Timings = [
		(UsableItemTiming.Unset.value, '不可用'),
		(UsableItemTiming.Menu.value, '仅菜单可用'),
		(UsableItemTiming.Battle.value, '仅对战可用'),
		(UsableItemTiming.Any.value, '随时可用'),
		(UsableItemTiming.Attack.value, '进攻可用'),
		(UsableItemTiming.BeAttacked.value, '防御可用'),
	]

	Types = [
		(UsableItemType.Auxiliary.value, '辅助类'),
		(UsableItemType.Attack.value, '攻击类'),
		(UsableItemType.Defense.value, '防御类'),
		(UsableItemType.Chest.value, '宝箱类'),
		(UsableItemType.Material.value, '材料类'),
		(UsableItemType.Task.value, '任务类'),
		(UsableItemType.Special.value, '活动类')
	]

	# 消耗品
	consumable = models.BooleanField(default=False, verbose_name="消耗品")

	# 使用时机
	timing = models.PositiveSmallIntegerField(default=0, choices=Timings, verbose_name="使用时机")

	# 冻结时间（秒数）
	freeze = models.PositiveSmallIntegerField(default=0, verbose_name="冻结时间")

	# 物品类型
	i_type = models.PositiveSmallIntegerField(default=0, choices=Types, verbose_name="物品类型")

	def __init__(self, *args, **kwargs):
		LimitedItem.__init__(self, *args, **kwargs)
		self.type = ItemType.HumanUsableItem.value

	# 转化为 dict
	def _convertToDict(self):
		res = LimitedItem._convertToDict(self)

		res['consumable'] = self.consumable
		res['timing'] = self.timing
		res['freeze'] = self.freeze
		res['i_type'] = self.i_type

		return res


# ===================================================
#  人类物品表
# ===================================================
class HumanItem(UsableItem):

	class Meta:

		verbose_name = verbose_name_plural = "人类物品"

	def __init__(self, *args, **kwargs):
		UsableItem.__init__(self, *args, **kwargs)
		self.type = ItemType.HumanItem.value

	# 转化为 dict
	def _convertToDict(self):
		res = UsableItem._convertToDict(self)

		return res


# ===================================================
#  宠物物品表
# ===================================================
class PetItem(UsableItem):

	class Meta:

		verbose_name = verbose_name_plural = "宠物物品"

	# 使用几率（*100）
	rate = models.PositiveSmallIntegerField(default=0, verbose_name="使用几率")

	def __init__(self, *args, **kwargs):
		UsableItem.__init__(self, *args, **kwargs)
		self.type = ItemType.PetItem.value

	# 转化为 dict
	def _convertToDict(self):
		res = UsableItem._convertToDict(self)

		res['rate'] = self.rate

		return res


# ===================================================
#  可装备物品
# ===================================================
class EquipableItem(LimitedItem):

	class Meta:

		verbose_name = verbose_name_plural = "可装备物品"

	# 基础攻击
	atk_base = models.PositiveSmallIntegerField(default=0, verbose_name="基础攻击")

	# 基础防御
	def_base = models.PositiveSmallIntegerField(default=0, verbose_name="基础防御")

	# 基础敏捷
	agi_base = models.PositiveSmallIntegerField(default=0, verbose_name="基础敏捷")

	# 基础体力
	mhp_base = models.PositiveSmallIntegerField(default=0, verbose_name="基础体力")

	# 基础暴击率（*10000）
	cri_base = models.PositiveSmallIntegerField(default=0, verbose_name="基础暴击")

	# 基础破防率（*10000）
	bdf_base = models.PositiveSmallIntegerField(default=0, verbose_name="基础破防")

	# 基础回避率（*10000）
	eva_base = models.PositiveSmallIntegerField(default=0, verbose_name="基础回避")

	# 基础反击率（*10000）
	cnt_base = models.PositiveSmallIntegerField(default=0, verbose_name="基础反击")

	def __init__(self, *args, **kwargs):
		LimitedItem.__init__(self, *args, **kwargs)

	# 转化为 dict
	def _convertToDict(self):
		res = LimitedItem._convertToDict(self)

		res['atk_base'] = self.atk_base
		res['def_base'] = self.def_base
		res['agi_base'] = self.agi_base
		res['mhp_base'] = self.mhp_base
		res['cri_base'] = self.cri_base
		res['bdf_base'] = self.bdf_base
		res['eva_base'] = self.eva_base
		res['cnt_base'] = self.cnt_base

		return res


# ===================================================
#  宠物装备类型
# ===================================================
class PetEquipType(Enum):
	Weapon = 0  # 武器
	Head = 1    # 头部
	Body = 2    # 身体
	Foot = 3    # 腿部
	Other = 4   # 其他


# ===================================================
#  宠物装备
# ===================================================
class PetEquip(EquipableItem):

	class Meta:

		verbose_name = verbose_name_plural = "宠物装备"

	Types = [
		(PetEquipType.Weapon.value, '武器'),
		(PetEquipType.Head.value, '头部'),
		(PetEquipType.Body.value, '身体'),
		(PetEquipType.Foot.value, '腿部'),
		(PetEquipType.Other.value, '其他'),
	]

	# 装备类型
	e_type = models.PositiveSmallIntegerField(default=0, choices=Types, verbose_name="装备类型")

	def __init__(self, *args, **kwargs):
		EquipableItem.__init__(self, *args, **kwargs)
		self.type = ItemType.PetEquip.value

	# 转化为 dict
	def _convertToDict(self):
		res = EquipableItem._convertToDict(self)

		res['e_type'] = self.e_type

		return res


# ===================================================
#  无限物品表
# ===================================================
class InfiniteItem(BaseItem):

	class Meta:

		abstract = True
		verbose_name = verbose_name_plural = "无限物品"

	# 转化为 dict
	def convertToDict(self):
		res = BaseItem.convertToDict(self)

		return res

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

	Codes = [
		(TraitCode.Unset.value, '无特性'),
	]

	# 所属物品
	item = models.ForeignKey('BaseItem', on_delete=models.CASCADE, verbose_name="物品")

	# 特性编号
	code = models.PositiveSmallIntegerField(default=0, choices=Codes, verbose_name="特性编号")

	# 特性参数
	params = jsonfield.JSONField(default=[], verbose_name="特性参数")


# ===================================================
#  使用效果编号枚举
# ===================================================
class ItemEffectCode(Enum):
	Unset = 0  # 未设置


# ===================================================
#  使用效果表（包括宠物技能）
# ===================================================
class ItemEffect(models.Model):

	class Meta:

		verbose_name = verbose_name_plural = "物品使用效果"

	Codes = [
		(ItemEffectCode.Unset.value, '无效果'),
	]

	# 物品
	item = models.ForeignKey('BaseItem', on_delete=models.CASCADE, verbose_name="物品")

	# 效果编号
	code = models.PositiveSmallIntegerField(default=0, choices=Codes, verbose_name="效果编号")

	# 效果参数
	params = jsonfield.JSONField(default=[], verbose_name="效果参数")

# endregion
