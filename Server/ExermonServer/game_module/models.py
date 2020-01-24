from django.db import models
from django.db.utils import ProgrammingError
from utils.model_utils import Common as ModelUtils
from utils.view_utils import Common as ViewUtils
from utils.exception import ErrorType
import jsonfield

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
# 集合型游戏术语
# ===================================================
class GameGroupTerm(models.Model):

	class Meta:

		abstract = True
		verbose_name = verbose_name_plural = "人类装备类型"

	NOT_EXIST_ERROR = ErrorType.UnknownError

	# 全局变量，ExermonEquipType 所有实例
	Objects = None

	# 全局变量，ExermonEquipType 数
	Count = None

	# 所属术语
	term = models.ForeignKey('GameTerm', on_delete=models.CASCADE, verbose_name="所属术语")

	# 名称
	name = models.CharField(max_length=8, verbose_name="名称")

	# 描述
	description = models.CharField(max_length=64, verbose_name="描述")

	def __str__(self):
		return self.name

	def convertToDict(self):

		return {
			'id': self.id,
			'name': self.name,
			'description': self.description
		}

	@classmethod
	def load(cls):

		cls.Objects = ViewUtils.getObjects(cls, term__activated=True)
		cls.Count = len(list(cls.Objects))  # 强制查询，加入缓存

	# 获取单个
	@classmethod
	def get(cls, **kwargs):

		if cls.Objects is None: cls.load()

		return ViewUtils.getObject(cls, cls.NOT_EXIST_ERROR,
								objects=cls.Objects, **kwargs)

	# 确保存在
	@classmethod
	def ensure(cls, **kwargs):

		if cls.Objects is None: cls.load()

		return ViewUtils.ensureObjectExist(cls, cls.NOT_EXIST_ERROR,
										   objects=cls.Objects, **kwargs)

	# 获取多个
	@classmethod
	def objs(cls, **kwargs):

		if cls.Objects is None: cls.load()

		return ViewUtils.getObjects(cls, cls.Objects, **kwargs)

	@classmethod
	def count(cls):

		if cls.Count is None: cls.load()
		return cls.Count


# ===================================================
#  科目表
# ===================================================
class Subject(GameGroupTerm):

	class Meta:
		verbose_name = verbose_name_plural = "科目"

	NOT_EXIST_ERROR = ErrorType.SubjectNotExist

	# 选科最大数目
	MAX_SELECTED = 6

	# 科目分值
	max_score = models.PositiveSmallIntegerField(default=100, verbose_name="分值")

	# 必选科目
	force = models.BooleanField(default=False, verbose_name="必选科目")

	def convertToDict(self):
		res = super().convertToDict()

		res['max_score'] = self.max_score
		res['force'] = self.force

		return res


# ===================================================
# 基本属性表
# ===================================================
class BaseParam(GameGroupTerm):

	# MHP = 1  # 体力值
	# MMP = 2  # 精力值
	# ATK = 3  # 攻击力
	# DEF = 4  # 防御力
	# EVA = 5  # 回避率（*10000）
	# CRI = 6  # 暴击率（*10000）

	class Meta:

		verbose_name = verbose_name_plural = "基本属性"

	NOT_EXIST_ERROR = ErrorType.BaseParamNotExist

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

	def convertToDict(self):
		res = super().convertToDict()

		res['attr'] = self.attr
		res['max_value'] = self.max_value
		res['min_value'] = self.min_value
		res['default'] = self.default
		res['scale'] = self.scale

		return res

	# clamp 属性值
	def clamp(self, val):

		min_ = self.min_value
		max_ = self.max_value
		if min_ is not None: val = max(min_, val)
		if max_ is not None: val = min(max_, val)

		return val

	@classmethod
	def getAttr(cls, id):
		return cls.get(id=id).attr


# ===================================================
# 可用物品类型
# ===================================================
class UsableItemType(GameGroupTerm):

	# Supply = 1  # 补给道具
	# Reinforce = 2  # 强化道具
	# Function = 3  # 功能道具
	# Chest = 4  # 宝箱
	# Material = 5  # 材料道具
	# Task = 6  # 任务道具
	# Others = 7  # 其他

	class Meta:

		verbose_name = verbose_name_plural = "可用物品类型"


# ===================================================
# 人类装备类型
# ===================================================
class HumanEquipType(GameGroupTerm):

	# Weapon = 1  # 武器
	# Head = 2  # 头部
	# Body = 3  # 身体
	# Foot = 4  # 腿部
	# Other = 5  # 其他

	class Meta:

		verbose_name = verbose_name_plural = "人类装备类型"


# ===================================================
# 艾瑟萌装备类型
# ===================================================
class ExerEquipType(GameGroupTerm):

	# Weapon = 1  # 武器
	# Head = 2  # 头部
	# Body = 3  # 身体
	# Foot = 4  # 腿部
	# Other = 5  # 其他

	class Meta:

		verbose_name = verbose_name_plural = "艾瑟萌装备类型"


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
class ExerStar(GameGroupTerm):

	class Meta:

		verbose_name = verbose_name_plural = "艾瑟萌星级"

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
#  艾瑟萌天赋属性成长加成率范围表
# ===================================================
class ExerGiftParamRateRange(ParamRateRange):

	class Meta:
		verbose_name = verbose_name_plural = "艾瑟萌天赋属性成长加成率范围"

	# 艾瑟萌星级
	star = models.ForeignKey("ExerGiftStar", on_delete=models.CASCADE, verbose_name="艾瑟萌星级")


# ===================================================
#  艾瑟萌天赋星级表
# ===================================================
class ExerGiftStar(GameGroupTerm):

	class Meta:

		verbose_name = verbose_name_plural = "艾瑟萌天赋星级"

	# 星级颜色（#ABCDEF）
	color = models.CharField(max_length=7, null=False, default='#000000', verbose_name="星级颜色")

	def __str__(self):
		return self.name

	def convertToDict(self):

		return {
			'id': self.id,
			'name': self.name,
			'color': self.color,
			'param_ranges': ModelUtils.objectsToDict(self.paramRateRanges()),
		}

	# 获取所有的属性成长率
	def paramRateRanges(self):
		return self.exergiftparamraterange_set.all()


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
try: GameTerm.load()
except: print("仍未建立数据库")
