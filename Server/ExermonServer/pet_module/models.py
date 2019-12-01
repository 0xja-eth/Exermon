from django.db import models
from django.conf import settings
from game_module.models import BaseParam
from item_module.models import LimitedItem, InfiniteItem, ItemType, ItemEffectCode, UsableItemTiming
from utils.model_utils import SkillIconUpload
from utils.exception import ErrorType, ErrorException

from enum import Enum
import jsonfield, os


# ===================================================
#  宠物表
# ===================================================
class Pet(InfiniteItem):

	class Meta:

		verbose_name = verbose_name_plural = "宠物"

	# 宠物品质
	quality = models.ForeignKey('PetQuality', on_delete=models.CASCADE, verbose_name="宠物品质")

	# 体力值成长（*100）
	mhp_rate = models.PositiveSmallIntegerField(default=100, verbose_name="体力成长")

	# 精力值成长（*100）
	mmp_rate = models.PositiveSmallIntegerField(default=100, verbose_name="精力成长")

	# 攻击成长（*100）
	atk_rate = models.PositiveSmallIntegerField(default=100, verbose_name="攻击成长")

	# 防御成长（*100）
	def_rate = models.PositiveSmallIntegerField(default=100, verbose_name="防御成长")

	# 回避率成长（*100）
	eva_rate = models.PositiveSmallIntegerField(default=100, verbose_name="回避率成长")

	# 反击率成长（*100）
	cri_rate = models.PositiveSmallIntegerField(default=100, verbose_name="反击率成长")

	# 基础体力值
	mhp_base = models.PositiveSmallIntegerField(default=100, verbose_name="基础体力")

	# 基础精力值
	mmp_base = models.PositiveSmallIntegerField(default=100, verbose_name="基础精力")

	# 基础攻击
	atk_base = models.PositiveSmallIntegerField(default=10, verbose_name="基础攻击")

	# 基础防御
	def_base = models.PositiveSmallIntegerField(default=10, verbose_name="基础防御")

	# 基础回避率（*10000）
	eva_base = models.PositiveSmallIntegerField(default=100, verbose_name="基础回避")

	# 基础反击率（*10000）
	cri_base = models.PositiveSmallIntegerField(default=100, verbose_name="基础反击")

	def __init__(self, *args, **kwargs):
		InfiniteItem.__init__(self, *args, **kwargs)
		self.type = ItemType.Pet.value

	# 获取属性基本值
	def paramBase(self, param_id=None, attr=None):
		if attr is not None:
			return getattr(self, attr+'_base')

		return self.paramBase(attr=BaseParam.getAttr(param_id))

	# 获取属性成长值
	def paramRate(self, param_id=None, attr=None):
		if attr is not None:
			return getattr(self, attr + '_rate')

		return self.paramRate(attr=BaseParam.getAttr(param_id))

	# 转换属性为 dict
	def _convertParamsToDict(self):

		data = dict()
		params = BaseParam.Params

		# 遍历每一个属性
		for param in params:
			attr = param.attr

			data[attr+'_base'] = self.paramBase(attr=attr)
			data[attr+'_rate'] = self.paramRate(attr=attr)

		return data

	# 转化为 dict
	def _convertToDict(self):
		res = InfiniteItem._convertToDict(self)

		res['quality'] = self.quality
		res['params'] = self._convertParamsToDict()

		return res


# ===================================================
#  宠物天赋表
# ===================================================
class PetGift(InfiniteItem):

	class Meta:

		verbose_name = verbose_name_plural = "宠物天赋"

	# 标志颜色（#ABCDEF）
	color = models.CharField(max_length=7, null=False, default='#FFFFFF', verbose_name="标志颜色")

	# 体力值成长（*100）
	mhp_rate = models.PositiveSmallIntegerField(default=100, verbose_name="体力成长")

	# 精力值成长（*100）
	mmp_rate = models.PositiveSmallIntegerField(default=100, verbose_name="精力成长")

	# 攻击成长（*100）
	atk_rate = models.PositiveSmallIntegerField(default=100, verbose_name="攻击成长")

	# 防御成长（*100）
	def_rate = models.PositiveSmallIntegerField(default=100, verbose_name="防御成长")

	# 回避率成长（*100）
	eva_rate = models.PositiveSmallIntegerField(default=100, verbose_name="回避率成长")

	# 反击率成长（*100）
	cri_rate = models.PositiveSmallIntegerField(default=100, verbose_name="反击率成长")

	def __init__(self, *args, **kwargs):
		InfiniteItem.__init__(self, *args, **kwargs)
		self.type = ItemType.PetGift.value

	# 获取属性成长值
	def paramRate(self, param_id=None, attr=None):
		if attr is not None:
			return getattr(self, attr + '_rate')

		return self.paramRate(attr=BaseParam.getAttr(param_id))

	# 转换属性为 dict
	def _convertParamsToDict(self):

		data = dict()
		params = BaseParam.Params

		# 遍历每一个属性
		for param in params:
			attr = param.attr
			data[attr+'_rate'] = self.paramRate(attr=attr)

		return data

	# 转化为 dict
	def _convertToDict(self):
		res = InfiniteItem._convertToDict(self)

		res['color'] = self.color
		res['params'] = self._convertParamsToDict()

		return res


# ===================================================
#  玩家宠物从属关系表
# ===================================================
class PlayerPet(models.Model):

	class Meta:

		verbose_name = verbose_name_plural = "玩家宠物从属关系"

	# 登陆玩家
	player = models.ForeignKey('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 装备的宠物
	pet = models.ForeignKey('Pet', on_delete=models.CASCADE, verbose_name="宠物")

	# 宠物昵称
	nickname = models.CharField(max_length=8, verbose_name="宠物昵称")

	# 经验值（相对）
	exp = models.PositiveIntegerField(default=0, verbose_name="经验值")

	# 等级
	level = models.PositiveSmallIntegerField(default=1, verbose_name="等级")

	def __str__(self):
		return str(self.player)+'-'+str(self.pet)

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

	# 总经验
	def sumExp(self):
		from utils.calc_utils import PetLevelCalc
		return PetLevelCalc.getSumExp(self.pet.quality, self.level, self.exp)

	# 所剩经验
	def deltaExp(self):
		from utils.calc_utils import PetLevelCalc

		delta = PetLevelCalc.getDetlaExp(self.pet.quality, self.level)
		if delta == -1: return -1

		return max(delta-self.exp, 0)

	# 获取属性当前值
	def paramVal(self, param_id=None, attr=None):
		from utils.calc_utils import PetParamCalc
		base = self.paramBase(param_id, attr)
		rate = self.paramRate(param_id, attr)
		value = PetParamCalc.calc(base, rate, self.level)

		return value

	# 宠物基础属性
	def paramBase(self, param_id=None, attr=None):
		return self.pet.paramBase(param_id, attr)

	# 获取属性成长值
	def paramRate(self, param_id=None, attr=None):
		return self.pet.paramRate(param_id, attr)

	# 获取成长值
	def mhpRate(self): return self.paramRate(0)
	def mmpRate(self): return self.paramRate(1)
	def atkRate(self): return self.paramRate(2)
	def defRate(self): return self.paramRate(3)
	def evaRate(self): return self.paramRate(4)
	def criRate(self): return self.paramRate(5)

	# 获取当前值
	def mhpVal(self): return self.paramVal(0)
	def mmpVal(self): return self.paramVal(1)
	def atkVal(self): return self.paramVal(2)
	def defVal(self): return self.paramVal(3)
	def evaVal(self): return self.paramVal(4)
	def criVal(self): return self.paramVal(5)

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

	# 刷新宠物
	def refresh(self):
		self.refreshLevel()
		self.save()

	# 刷新等级
	def refreshLevel(self):
		from utils.calc_utils import PetLevelCalc

		exp = self.exp
		level = self.level
		delta = PetLevelCalc.getDetlaExp(self.pet.quality, level)

		# 当可以升级的时候
		while exp > delta:
			level += 1  # 升级
			exp -= delta  # 扣除所需的经验
			# 更新所需经验值
			delta = PetLevelCalc.getDetlaExp(self.pet.quality, level)

		if level > self.level:

			self.exp = exp
			self.level = level

			self._onUpgrade()

	# 升级触发事件
	def _onUpgrade(self):
		pass


# ===================================================
#  玩家宠物槽表
# ===================================================
class PetSlot(models.Model):

	class Meta:

		verbose_name = verbose_name_plural = "玩家宠物槽"

	# 玩家
	player = models.ForeignKey('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 玩家宠物
	player_pet = models.OneToOneField('PlayerPet', on_delete=models.CASCADE, verbose_name="玩家宠物")

	# 装备的天赋
	gift = models.ForeignKey('PetGift', on_delete=models.CASCADE, verbose_name="天赋")

	# 科目
	subject = models.ForeignKey('question_module.Subject', on_delete=models.CASCADE, verbose_name="科目")

	# 宠物昵称
	nickname = models.CharField(max_length=8, verbose_name="宠物昵称")

	# 经验值（累计）
	exp = models.PositiveIntegerField(default=0, verbose_name="经验值")

	# 属性值（缓存）
	param_val = None

	def __init__(self, *args, **kwargs):
		models.Model.__init__(self, *args, **kwargs)
		self.param_val = {}

	# 槽等级（本等级, 下一级所需经验）
	def slotLevel(self):
		from utils.calc_utils import PetSlotLevelCalc

		level = PetSlotLevelCalc.calcLevel(self.exp)
		next = PetSlotLevelCalc.calcNext(level)

		return level, next

	# 宠物等级
	def petLevel(self):
		return self.player_pet.level

	# 获取属性当前值
	def paramVal(self, param_id=None, attr=None):
		if attr not in self.param_val: # 如果没有缓存该属性

			from utils.calc_utils import PetParamCalc
			base = self.paramBase(param_id, attr)
			rate = self.paramRate(param_id, attr)
			value = PetParamCalc.calc(base, rate, self.petLevel)

			self.param_val[attr] = value

		return self.param_val[attr]

	# 宠物基础属性
	def paramBase(self, param_id=None, attr=None):
		return self.player_pet.paramBase(param_id, attr)

	# 获取属性成长值
	def paramRate(self, param_id=None, attr=None):
		return self.player_pet.paramRate(param_id, attr)*self.giftParamRate(param_id, attr)

	# 获取天赋属性加成
	def giftParamRate(self, param_id=None, attr=None):
		if self.gift is None: return 1
		return self.gift.paramRate(param_id, attr)

	# 刷新
	def refresh(self, refresh_pet=True):
		if self.player_pet is not None and refresh_pet:
			self.player_pet.refresh()

		self.param_val = {}
		self.save()

	# 放置宠物
	def setPet(self, player_pet: PlayerPet):
		self.player_pet = player_pet
		self.refresh()

	# 放置天赋
	def setGift(self, gift: PetGift):
		self.gift = gift
		self.refresh()

	# 增加经验
	def changeExp(self, exp):
		self.exp += exp
		if self.player_pet is not None:
			self.player_pet.changeExp(exp)

		self.refresh(False)


# ===================================================
#  宠物品质表
# ===================================================
class PetQuality(models.Model):

	class Meta:

		verbose_name = verbose_name_plural = "宠物品质"

	# 品质名称
	name = models.CharField(max_length=2, verbose_name="品质名称")

	# 品质颜色（#ABCDEF）
	color = models.CharField(max_length=7, null=False, default='#000000', verbose_name="品质颜色")

	# 最大等级
	max_level = models.PositiveSmallIntegerField(default=0, verbose_name="最大等级")

	# 等级经验计算因子
	# {'a', 'b', 'c'}
	level_exp_factors = jsonfield.JSONField(default=[0], verbose_name="等级经验计算因子")

	def __str__(self):
		return self.name

# ===================================================
#  宠物技能表
# ===================================================
class PetSkill(InfiniteItem):

	class Meta:

		verbose_name = verbose_name_plural = "有限物品"

	Timings = [
		(UsableItemTiming.Attack.value, '进攻可用'),
		(UsableItemTiming.BeAttacked.value, '防御可用'),
	]

	# 宠物
	held_pet = models.ForeignKey('Pet', on_delete=models.CASCADE, verbose_name="宠物")

	# 使用几率（*100）
	rate = models.PositiveSmallIntegerField(default=0, verbose_name="使用几率")

	# 使用时机
	timing = models.PositiveSmallIntegerField(default=0, choices=Timings, verbose_name="使用时机")

	# 冻结时间（回合数）
	freeze = models.PositiveSmallIntegerField(default=0, verbose_name="冻结时间")

	# 最大使用次数（0为不限）
	max_use_count = models.PositiveSmallIntegerField(default=0, verbose_name="最大使用次数")

	# 技能图标
	icon = models.ImageField(upload_to=SkillIconUpload(), verbose_name="图标")

	def __init__(self, *args, **kwargs):
		InfiniteItem.__init__(self, *args, **kwargs)
		self.type = ItemType.PetSkill.value

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
		res = InfiniteItem._convertToDict(self)

		res['rate'] = self.rate
		res['timing'] = self.timing
		res['freeze'] = self.freeze
		res['max_use_count'] = self.max_use_count
		res['icon'] = self.convertIconToBase64()

		return res
