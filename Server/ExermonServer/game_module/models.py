from django.db import models
from django.db.utils import ProgrammingError
from utils.model_utils import Common as ModelUtils
from utils.view_utils import Common as ViewUtils
from utils.exception import ErrorType

# Create your models here.


# ===================================================
# 游戏版本记录表
# ===================================================
class GameVersion(models.Model):

	class Meta:

		verbose_name = verbose_name_plural = "游戏版本"

	# 主版本号（检查更新）
	main_version = models.CharField(unique=True, null=False, blank=False, max_length=16, verbose_name="版本号")

	# 副版本号（建议更新）
	sub_version = models.CharField(unique=True, null=False, blank=False, max_length=16, verbose_name="版本号")

	# 更新时间
	update_time = models.DateTimeField(auto_now_add=True, verbose_name="更新时间")

	# 更新日志
	update_note = models.TextField(default='', blank=True, verbose_name="更新日志")

	# 附加描述
	description = models.CharField(default='', max_length=64, blank=True, verbose_name="附加描述")

	# 是否启用
	is_used = models.BooleanField(default=True, verbose_name="启用")

	def __str__(self):
		return self.main_version+'.'+self.sub_version

	# 新增版本
	@classmethod
	def add(cls, main, sub, note, desc):

		version = cls()
		version.main_version = main
		version.sub_version = sub
		version.update_note = note
		version.description = desc

		version.activate()

	# 激活本版本
	def activate(self):

		GameVersion.objects.all().update(is_used=False)

		self.is_used = True
		self.save()

	def convertToDict(self):

		update_time = ModelUtils.timeToStr(self.update_time)

		return {
			'main_version': self.main_version,
			'sub_version': self.sub_version,
			'update_time': update_time,
			'update_note': self.update_note,
			'description': self.description
		}


# ===================================================
# 基本属性表
# ===================================================
class BaseParam(models.Model):

	# MHP = 1  # 体力值
	# MMP = 2  # 精力值
	# ATK = 3  # 攻击力
	# DEF = 4  # 防御力
	# EVA = 5  # 回避率（*10000）
	# CRI = 6  # 暴击率（*10000）

	class Meta:

		verbose_name = verbose_name_plural = "基本属性"

	# 全局变量，BaseParam 所有实例
	Params = None

	# 全局变量，BaseParam 数
	Count = None

	# 显示名称
	name = models.CharField(max_length=8, verbose_name="显示名称")

	# 程序属性名
	attr = models.CharField(max_length=8, verbose_name="程序属性名")

	# 最大值（为 None 时无限制）
	max_value = models.PositiveIntegerField(null=True, blank=True, verbose_name="最大值")

	# 最小值
	min_value = models.PositiveSmallIntegerField(default=0, verbose_name="最小值")

	# 默认值
	default = models.PositiveSmallIntegerField(default=0, verbose_name="最小值")

	# 属性比例值
	scale = models.PositiveSmallIntegerField(default=1, verbose_name="属性比例值")

	# 描述
	description = models.CharField(max_length=64, verbose_name="描述")

	def convertToDict(self):

		return {
			'id': self.id,
			'name': self.name,
			'attr': self.attr,
			'max_value': self.max_value,
			'min_value': self.min_value,
			'default': self.default,
			'scale': self.scale,
			'description': self.description
		}

	@classmethod
	def load(cls):

		cls.Params = ViewUtils.getObjects(cls)
		cls.Count = len(list(cls.Params))  # 强制查询，加入缓存

	@classmethod
	def get(cls, index=None, attr=None):

		if cls.Params is None: cls.load()

		if index is not None:
			return ViewUtils.getObject(cls, ErrorType.UnknownError,
									   objects=cls.Params, id=index)
		if attr is not None:
			return ViewUtils.getObject(cls, ErrorType.UnknownError,
									   objects=cls.Params, attr=attr)
		return cls.Params

	@classmethod
	def getAttr(cls, index):
		return cls.get(index).attr

	@classmethod
	def count(cls):

		if cls.Count is None: cls.load()
		return cls.Count


# ===================================================
#  属性值表（用于艾瑟萌属性以及属性增减等）
# ===================================================
class ParamValue(models.Model):

	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "属性值"

	# 属性类型
	param = models.ForeignKey("game_module.BaseParam",
							   on_delete=models.CASCADE, verbose_name="属性类型")

	# 属性值/属性率
	value = models.IntegerField(default=0, verbose_name="属性值")

	# 比例
	def scale(self):
		return self.param.scale

	# 最大值
	def maxVal(self):
		return self.param.max_value

	# 最小值
	def minVal(self):
		return self.param.min_value

	# 值增加
	def addValue(self, value, clamp=True):
		# value: ParamValue
		if self.param_id != value.param_id: return
		new_val = self.getValue()+value.getValue()
		self.setValue(new_val, clamp)

	# 值相乘
	def multValue(self, rate, clamp=True):
		# rate: ParamRate
		if self.param_id != rate.param_id: return
		new_val = self.getValue()*rate.getValue()
		self.setValue(new_val, clamp)

	# Clamp 值
	def _clampValue(self, value):
		max_ = self.maxVal()
		min_ = self.minVal()
		if min_ is not None:
			value = max(min_, value)
		if max_ is not None:
			value = min(max_, value)
		return value

	# Scale 值
	def _scaleValue(self, value, reverse=False):
		if reverse: return value / self.scale()
		return value * self.scale()

	# 调整值
	def adjustValue(self, value, clamp=True):
		if clamp: value = self._clampValue(value)
		value = self._scaleValue(value)
		return value

	# 设置值
	def setValue(self, value, clamp=True):
		self.value = self.adjustValue(value, clamp)

	# 获取值
	def getValue(self):
		value = self._scaleValue(self.value, True)
		return value

	def convertToDict(self):
		return {
			'param_id': self.param_id,
			'value': self.getValue(),
		}


# ===================================================
#  属性率表（用于成长率、加成率等）
# ===================================================
class ParamRate(ParamValue):

	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "属性率"

	# 比例
	def scale(self):
		return 100

	# 最大值
	def maxVal(self):
		return None

	# 最小值
	def minVal(self):
		return None


# ===================================================
#  属性值区间表
# ===================================================
class ParamValueRange(models.Model):

	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "属性值区间"

	# 属性类型
	param = models.ForeignKey("game_module.BaseParam",
							   on_delete=models.CASCADE, verbose_name="属性类型")

	# 最小值
	min_value = models.IntegerField(default=0, verbose_name="最小值")

	# 最大值
	max_value = models.IntegerField(default=0, verbose_name="最大值")

	# 比例
	def scale(self):
		return self.param.scale

	# 设置值
	def setValue(self, min_, max_):
		self.min_value = min_ * self.scale()
		self.max_value = max_ * self.scale()

	# 获取值
	def getValue(self):
		return self.min_value/self.scale(), self.max_value/self.scale()

	def convertToDict(self):
		values = self.getValue()
		return {
			'param_id': self.param_id,
			'min_value': values[0],
			'max_value': values[1],
		}


# ===================================================
#  属性率区间表（用于艾瑟萌天赋的成长加成率等）
# ===================================================
class ParamRateRange(ParamValueRange):

	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "属性率区间"

	# 比例
	def scale(self):
		return 100


# ===================================================
# 可用物品类型
# ===================================================
class UsableItemType(models.Model):

	# Supply = 1  # 补给道具
	# Reinforce = 2  # 强化道具
	# Function = 3  # 功能道具
	# Chest = 4  # 宝箱
	# Material = 5  # 材料道具
	# Task = 6  # 任务道具
	# Others = 7  # 其他

	class Meta:

		verbose_name = verbose_name_plural = "可用物品类型"

	# 全局变量，UsableItemType 所有实例
	Types = None

	# 全局变量，UsableItemType 数
	Count = None

	# 名称
	name = models.CharField(max_length=8, verbose_name="名称")

	# 描述
	description = models.CharField(max_length=64, verbose_name="描述")

	def convertToDict(self):

		return {
			'id': self.id,
			'name': self.name,
			'description': self.description
		}

	@classmethod
	def load(cls):

		cls.Types = ViewUtils.getObjects(cls)
		cls.Count = len(list(cls.Types))  # 强制查询，加入缓存

	@classmethod
	def get(cls, index=None):

		if cls.Types is None: cls.load()

		if index is None: return cls.Types

		return ViewUtils.getObject(cls, ErrorType.UnknownError,
								objects=cls.Types, id=index)

	@classmethod
	def count(cls):

		if cls.Count is None: cls.load()
		return cls.Count


# ===================================================
# 人类装备类型
# ===================================================
class HumanEquipType(models.Model):

	# Weapon = 1  # 武器
	# Head = 2  # 头部
	# Body = 3  # 身体
	# Foot = 4  # 腿部
	# Other = 5  # 其他

	class Meta:

		verbose_name = verbose_name_plural = "人类装备类型"

	# 全局变量，ExermonEquipType 所有实例
	Types = None

	# 全局变量，ExermonEquipType 数
	Count = None

	# 名称
	name = models.CharField(max_length=8, verbose_name="名称")

	# 描述
	description = models.CharField(max_length=64, verbose_name="描述")

	def convertToDict(self):

		return {
			'id': self.id,
			'name': self.name,
			'description': self.description
		}

	@classmethod
	def load(cls):

		cls.Types = ViewUtils.getObjects(cls)
		cls.Count = len(list(cls.Types))  # 强制查询，加入缓存

	@classmethod
	def get(cls, index=None):

		if cls.Types is None: cls.load()

		if index is None: return cls.Types

		return ViewUtils.getObject(cls, ErrorType.UnknownError,
								objects=cls.Types, id=index)

	@classmethod
	def count(cls):

		if cls.Count is None: cls.load()
		return cls.Count


# ===================================================
# 艾瑟萌装备类型
# ===================================================
class ExerEquipType(models.Model):

	# Weapon = 1  # 武器
	# Head = 2  # 头部
	# Body = 3  # 身体
	# Foot = 4  # 腿部
	# Other = 5  # 其他

	class Meta:

		verbose_name = verbose_name_plural = "艾瑟萌装备类型"

	# 全局变量，ExermonEquipType 所有实例
	Types = None

	# 全局变量，ExermonEquipType 数
	Count = None

	# 名称
	name = models.CharField(max_length=8, verbose_name="名称")

	# 描述
	description = models.CharField(max_length=64, verbose_name="描述")

	def convertToDict(self):

		return {
			'id': self.id,
			'name': self.name,
			'description': self.description
		}

	@classmethod
	def load(cls):

		cls.Types = ViewUtils.getObjects(cls)
		cls.Count = len(list(cls.Types))  # 强制查询，加入缓存

	@classmethod
	def get(cls, index=None):

		if cls.Types is None: cls.load()

		if index is None: return cls.Types

		return ViewUtils.getObject(cls, ErrorType.UnknownError,
								objects=cls.Types, id=index)

	@classmethod
	def count(cls):

		if cls.Count is None: cls.load()
		return cls.Count


# ===================================================
#  科目表
# ===================================================
class Subject(models.Model):
	class Meta:
		verbose_name = verbose_name_plural = "科目"

	# 全局变量，ExermonEquipType 所有实例
	Subjects = None

	# 全局变量，ExermonEquipType 数
	Count = None

	# 科目名
	name = models.CharField(max_length=4, verbose_name="名称")

	# 科目分值
	max_score = models.PositiveSmallIntegerField(default=100, verbose_name="分值")

	def __str__(self):
		return self.name

	def convertToDict(self):
		return {
			'id': self.id,
			'name': self.name,
			'max_score': self.max_score
		}

	@classmethod
	def load(cls):

		cls.Subjects = ViewUtils.getObjects(cls)
		cls.Count = len(list(cls.Subjects))  # 强制查询，加入缓存

	@classmethod
	def get(cls, index=None):

		if cls.Subjects is None: cls.load()

		if index is None: return cls.Subjects

		return ViewUtils.getObject(cls, ErrorType.UnknownError,
								objects=cls.Subjects, id=index)

	@classmethod
	def count(cls):

		if cls.Count is None: cls.load()
		return cls.Count


# ===================================================
# 游戏用语表
# ===================================================
class GameTerm(models.Model):

	class Meta:

		verbose_name = verbose_name_plural = "游戏用语"

	# 全局变量，GameTerm实例
	Term = None

	# 游戏名
	name = models.CharField(default='艾瑟萌学院', max_length=12, verbose_name="游戏名")

	# 游戏名（英文）
	eng_name = models.CharField(default='Exermon', max_length=24, verbose_name="游戏名（英文）")

	# 金币
	gold = models.CharField(default='金币', max_length=6, verbose_name="金币")

	# 点券
	ticket = models.CharField(default='点券', max_length=6, verbose_name="点券")

	# 绑定点券
	bound_ticket = models.CharField(default='绑定点券', max_length=6, verbose_name="绑定点券")

	# 激活
	activated = models.BooleanField(default=True, verbose_name="激活")

	def convertToDict(self):

		return {
			'name': self.name,
			'eng_name': self.eng_name,
			'gold': self.gold,
			'ticket': self.ticket,
			'bound_ticket': self.bound_ticket,
		}


	@classmethod
	def load(cls):

		BaseParam.load()
		Subject.load()
		BaseParam.load()
		cls.Term = ViewUtils.getObject(cls, ErrorType.UnknownError, activated=True)

	@classmethod
	def get(cls):

		if cls.Term is None: cls.load()

		return cls.Term


# 初始化
try:
	GameTerm.load()
except ProgrammingError:
	print("仍未建立数据库")
