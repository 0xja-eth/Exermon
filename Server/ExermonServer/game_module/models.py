from django.db import models
from django.db.models.query import QuerySet

from utils.model_utils import BaseModel, GameData, \
	StaticData, DynamicData, CoreDataManager, Common as ModelUtils
from utils.view_utils import Common as ViewUtils
from utils.exception import GameException, ErrorType

from enum import Enum
import jsonfield

# Create your models here.


# ===================================================
# 游戏版本记录表
# ===================================================
class GameVersion(BaseModel):

	class Meta:

		verbose_name = verbose_name_plural = "游戏版本"

	LIST_EDITABLE_EXCLUDE = ['update_time', 'description']

	# 当前版本的 GameVersion 实例
	Version = None

	# 主版本号（检查更新）
	main_version = models.CharField(null=False, blank=False, max_length=16, verbose_name="主版本号")

	# 副版本号（建议更新）
	sub_version = models.CharField(unique=True, null=False, blank=False, max_length=16, verbose_name="副版本号")

	# 更新时间
	update_time = models.DateTimeField(auto_now_add=True, verbose_name="更新时间")

	# 更新日志
	update_note = models.TextField(default='', blank=True, verbose_name="更新日志")

	# 附加描述
	description = models.CharField(default='', max_length=64, blank=True, verbose_name="附加描述")

	# 游戏配置
	configure = models.ForeignKey('GameConfigure', on_delete=models.CASCADE, verbose_name="游戏配置")

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

		cls.Version = version

	@classmethod
	def setup(cls):

		cls.Version: GameVersion = ViewUtils.getObject(
			GameVersion, ErrorType.NoCurVersion, is_used=True)

	@classmethod
	def get(cls):
		if cls.Version is None: cls.setup()

		return cls.Version

	# 激活本版本
	def activate(self):

		GameVersion.objects.all().update(is_used=False)

		self.is_used = True
		self.save()


# ===================================================
#  属性值表（用于艾瑟萌属性以及属性增减等）
# ===================================================
class ParamValue(BaseModel):

	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "属性值"

	# DO_NOT_AUTO_CONVERT_FIELDS = ['value']

	# 属性类型
	param = models.ForeignKey("game_module.BaseParam",
							   on_delete=models.CASCADE, verbose_name="属性类型")

	# 属性值/属性率
	value = models.IntegerField(default=0, verbose_name="属性值")
	value.convert = lambda obj, value: obj.getValue()

	def __init__(self, *args, **kwargs):
		super().__init__(*args, **kwargs)
		if self.param_id is not None:
			self.attr = self.param.attr

	def __str__(self):
		val = self.getValue()
		return "%s: %s" % (self.param.name, val)

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

	def __add__(self, value) -> 'ParamValue':
		if value is None: return self

		if not isinstance(value, ParamValue):
			old_val = value
			value = type(self)(param=self.param)
			value.value = old_val

		res = type(self)(param=self.param)
		# res.param_id = self.param_id
		res.value = self.value
		res.addValue(value)

		return res

	def __sub__(self, value) -> 'ParamValue':
		if value is None: return self

		if not isinstance(value, ParamValue):
			old_val = value
			value = type(self)(param=self.param)
			value.value = old_val

		res = type(self)(param=self.param)
		res.value = self.value
		res.addValue(-value)

		return res

	def __neg__(self) -> 'ParamValue':

		res = type(self)(param=self.param)
		res.value = -self.value

		return res

	# 值相乘
	def multValue(self, rate, clamp=True):
		# rate: ParamRate
		if self.param_id != rate.param_id: return
		new_val = self.getValue()*rate.getValue()
		self.setValue(new_val, clamp)

	def __mul__(self, rate) -> 'ParamValue':
		if rate is None: return self

		if not isinstance(rate, ParamValue):
			old_rate = rate
			rate = type(self)(param=self.param)
			rate.value = old_rate

		res = type(self)(param=self.param)
		# res.param_id = self.param_id
		res.value = self.value
		res.multValue(rate)

		return res

	def __truediv__(self, rate) -> 'ParamValue':
		if rate is None: return self

		if not isinstance(rate, ParamValue):
			old_rate = rate
			rate = type(self)(param=self.param)
			rate.value = old_rate

		res = type(self)(param=self.param)
		# res.param_id = self.param_id
		res.value = self.value
		res.multValue(1/rate)

		return res

	# Clamp 值
	def _clampValue(self, value):
		max_ = self.maxVal()
		min_ = self.minVal()
		if min_ is not None:
			value = max(min_, value)
		if max_ is not None:
			value = min(max_, value)
		return value

	# Scale 值（
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
		if not self.isPercent(): value = round(value)
		self.value = self.adjustValue(value, clamp)

	# 获取值
	def getValue(self):
		value = self._scaleValue(self.value, True)
		if not self.isPercent(): value = round(value)
		return value

	# 是否百分数
	def isPercent(self):
		return self.param.isPercent()

	# def _convertCustomAttrs(self, res, type=None, **kwargs):
	# 	super()._convertCustomAttrs(res, type, **kwargs)
	#
	# 	res['value'] = self.getValue()


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

	# 是否百分数
	def isPercent(self):
		return True


# ===================================================
#  装备属性值
# ===================================================
class EquipParamValue(ParamValue):

	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "装备属性值"

	KEY_NAME = 'base_params'


# ===================================================
#  装备属性率
# ===================================================
class EquipParamRate(EquipParamValue):

	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "装备属性率"

	KEY_NAME = 'level_params'

	# 比例
	def scale(self):
		return 100

	# 最大值
	def maxVal(self):
		return None

	# 最小值
	def minVal(self):
		return None

	# 是否百分数
	def isPercent(self):
		return True


# ===================================================
#  属性值区间表
# ===================================================
class ParamValueRange(BaseModel):

	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "属性值区间"

	# DO_NOT_AUTO_CONVERT_FIELDS = ['min_value', 'max_value']

	KEY_NAME = 'base_ranges'

	# 属性类型
	param = models.ForeignKey("game_module.BaseParam",
							   on_delete=models.CASCADE, verbose_name="属性类型")

	# 最小值
	min_value = models.IntegerField(default=0, verbose_name="最小值")
	min_value.convert = lambda obj, value: obj.getMinValue()

	# 最大值
	max_value = models.IntegerField(default=0, verbose_name="最大值")
	max_value.convert = lambda obj, value: obj.getMaxValue()

	def __str__(self):
		min_val, max_val = self.getValue()
		return "%s: %s ~ %s" % (self.param.name, min_val, max_val)

	# 比例
	def scale(self):
		return self.param.scale

	# 设置值
	def setValue(self, min_, max_):
		self.min_value = min_ * self.scale()
		self.max_value = max_ * self.scale()

	# 获取值
	def getValue(self):
		return self.getMinValue(), self.getMaxValue()

	# 获取值
	def getMinValue(self):
		return self.min_value/self.scale()

	# 获取值
	def getMaxValue(self):
		return self.max_value/self.scale()

	# def _convertCustomAttrs(self, res, type=None, **kwargs):
	# 	super()._convertCustomAttrs(res, type, **kwargs)
	# 	values = self.getValue()
	#
	# 	res['min_value'] = values[0]
	# 	res['max_value'] = values[1]


# ===================================================
#  属性率区间表（用于艾瑟萌天赋的成长加成率等）
# ===================================================
class ParamRateRange(ParamValueRange):

	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "属性率区间"

	KEY_NAME = 'rate_ranges'

	# 比例
	def scale(self):
		return 100


# ===================================================
# 小贴士类型枚举
# ===================================================
class TipType(Enum):

	Study = 1  # 学习知识
	Game = 2  # 游戏知识
	Others = 0  # 其他知识


# ===================================================
# 游戏小贴士
# ===================================================
class GameTip(DynamicData):

	class Meta:

		verbose_name = verbose_name_plural = "游戏小贴士"

	TYPES = [
		(TipType.Study.value, '学习小贴士'),
		(TipType.Game.value, '游戏小贴士'),
		(TipType.Others.value, '其他小贴士'),
	]

	NOT_EXIST_ERROR = ErrorType.TypeNotExist

	# 小贴士类型
	type = models.PositiveSmallIntegerField(default=TipType.Study.value,
											choices=TYPES, verbose_name="小贴士类型")


# ===================================================
#  科目表
# ===================================================
class Subject(StaticData):

	class Meta:
		verbose_name = verbose_name_plural = "科目"

	NOT_EXIST_ERROR = ErrorType.SubjectNotExist

	LIST_DISPLAY_APPEND = ['adminColor']

	# 选科最大数目
	MAX_SELECTED = 6

	# 科目颜色（#ABCDEF）
	color = models.CharField(max_length=7, null=False, default='#000000', verbose_name="科目颜色")

	# 科目分值
	max_score = models.PositiveSmallIntegerField(default=100, verbose_name="分值")

	# 必选科目
	force = models.BooleanField(default=False, verbose_name="必选科目")

	# 管理界面用：显示科目颜色
	def adminColor(self):
		from django.utils.html import format_html

		res = '<div style="background: %s; width: 48px; height: 24px;"></div>' % self.color

		return format_html(res)

	adminColor.short_description = "科目颜色"


# ===================================================
# 基本属性表
# ===================================================
class BaseParam(StaticData):

	# MHP = 1  # 体力值
	# MMP = 2  # 精力值
	# ATK = 3  # 攻击力
	# DEF = 4  # 防御力
	# EVA = 5  # 回避率（*10000）
	# CRI = 6  # 暴击率（*10000）

	class Meta:

		verbose_name = verbose_name_plural = "基本属性"

	NOT_EXIST_ERROR = ErrorType.BaseParamNotExist

	LIST_DISPLAY_APPEND = ['adminColor']

	# 属性颜色（#ABCDEF）
	color = models.CharField(max_length=7, null=False, default='#000000', verbose_name="属性颜色")

	# 程序属性名
	attr = models.CharField(max_length=8, verbose_name="程序属性名")

	# 最大值（为 None 时无限制）
	max_value = models.PositiveIntegerField(null=True, blank=True, verbose_name="最大值")

	# 最小值
	min_value = models.PositiveSmallIntegerField(default=0, verbose_name="最小值")

	# 默认值
	default = models.PositiveSmallIntegerField(default=0, verbose_name="默认值")

	# 属性比例值
	scale = models.PositiveSmallIntegerField(default=1, verbose_name="属性比例值")

	# 管理界面用：显示星级颜色
	def adminColor(self):
		from django.utils.html import format_html

		res = '<div style="background: %s; width: 48px; height: 24px;"></div>' % self.color

		return format_html(res)

	adminColor.short_description = "属性颜色"

	# clamp 属性值
	def clamp(self, val):

		min_ = self.min_value
		max_ = self.max_value
		if min_ is not None: val = max(min_, val)
		if max_ is not None: val = min(max_, val)

		return val

	# 是否百分比属性
	def isPercent(self):
		return self.scale == 10000

	@classmethod
	def getAttr(cls, id):
		return cls.get(id=id).attr


# ===================================================
# 可用物品类型
# ===================================================
class GameItemType(StaticData):

	# Supply = 1  # 补给道具
	# Reinforce = 2  # 强化道具
	# Function = 3  # 功能道具
	# Chest = 4  # 宝箱
	# Material = 5  # 材料道具
	# Task = 6  # 任务道具
	# Others = 7  # 其他

	class Meta:

		verbose_name = verbose_name_plural = "物品类型"

	NOT_EXIST_ERROR = ErrorType.TypeNotExist


# ===================================================
# 艾瑟萌装备类型
# ===================================================
class GameEquipType(StaticData):

	# Weapon = 1  # 武器
	# Head = 2  # 头部
	# Body = 3  # 身体
	# Foot = 4  # 腿部
	# Other = 5  # 其他

	class Meta:

		verbose_name = verbose_name_plural = "装备类型"

	NOT_EXIST_ERROR = ErrorType.TypeNotExist


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
class ExerStar(StaticData):

	class Meta:

		verbose_name = verbose_name_plural = "艾瑟萌星级"

	NOT_EXIST_ERROR = ErrorType.StarNotExist

	# 星级颜色（#ABCDEF）
	color = models.CharField(max_length=7, null=False, default='#000000', verbose_name="星级颜色")

	# 最大等级
	max_level = models.PositiveSmallIntegerField(default=0, verbose_name="最大等级")

	# 等级经验计算因子
	# {'a', 'b', 'c'}
	level_exp_factors = jsonfield.JSONField(default={}, verbose_name="等级经验计算因子")
	level_exp_factors.convert = lambda model, value: list(value.values())

	CAN_AUTO_RELATED_MODELS = [ExerParamBaseRange, ExerParamRateRange]

	def __str__(self):
		return self.name

	# region AdminX配置

	# 管理界面用：显示星级颜色
	def adminColor(self):
		from django.utils.html import format_html

		res = '<div style="background: %s; width: 48px; height: 24px;"></div>' % self.color

		return format_html(res)

	adminColor.short_description = "星级颜色"

	# 管理界面用：显示经验计算因子
	def adminLevelExpFactors(self):
		from django.utils.html import format_html

		a = self.level_exp_factors['a']
		b = self.level_exp_factors['b']
		c = self.level_exp_factors['c']
		res = "A: %s<br>B: %s<br>C: %s" % (a, b, c)

		return format_html(res)

	adminLevelExpFactors.short_description = "等级经验计算因子"

	# 管理界面用：显示属性限制范围
	def adminParamBaseRanges(self):
		from django.utils.html import format_html

		base_ranges = self.paramBaseRanges()

		res = ''
		for r in base_ranges:
			res += str(r) + "<br>"

		return format_html(res)

	adminParamBaseRanges.short_description = "属性基础值范围"

	# 管理界面用：显示属性限制范围
	def adminParamRateRanges(self):
		from django.utils.html import format_html

		rate_ranges = self.paramRateRanges()

		res = ''
		for r in rate_ranges:
			res += str(r) + "<br>"
		res += "</span>"

		return format_html(res)

	adminParamRateRanges.short_description = "属性成长率范围"

	LIST_DISPLAY_APPEND = ['adminColor', 'adminLevelExpFactors',
						   'adminParamBaseRanges', 'adminParamRateRanges']

	# endregion

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
class ExerGiftStar(StaticData):

	class Meta:

		verbose_name = verbose_name_plural = "艾瑟萌天赋星级"

	NOT_EXIST_ERROR = ErrorType.StarNotExist

	CAN_AUTO_RELATED_MODELS = [ExerGiftParamRateRange]

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

	def _convertCustomAttrs(self, res, type=None, **kwargs):
		super()._convertCustomAttrs(res, type, **kwargs)

		res['param_ranges'] = ModelUtils.objectsToDict(self.paramRateRanges())

	# 获取所有的属性成长率
	def paramRateRanges(self):
		return self.exergiftparamraterange_set.all()


# ===================================================
#  物品星级表
# ===================================================
class ItemStar(StaticData):

	class Meta:

		verbose_name = verbose_name_plural = "物品星级"

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
#  题目星级表
# ===================================================
class QuestionStar(StaticData):
	class Meta:
		verbose_name = verbose_name_plural = "题目星级"

	NOT_EXIST_ERROR = ErrorType.StarNotExist

	LIST_DISPLAY_APPEND = ['adminColor']

	# 星级颜色（#ABCDEF）
	color = models.CharField(max_length=7, null=False, default='#000000', verbose_name="星级颜色")

	# 等级要求
	level = models.PositiveSmallIntegerField(default=0, verbose_name="等级要求")

	# 等级权重（用于生成分数）
	weight = models.PositiveSmallIntegerField(default=0, verbose_name="等级权重")

	# 基础经验奖励
	exp_incr = models.PositiveSmallIntegerField(default=0, verbose_name="基础经验奖励")

	# 基础金币奖励
	gold_incr = models.PositiveSmallIntegerField(default=0, verbose_name="基础金币奖励")

	# 等级标准时间（单位：秒）
	std_time = models.PositiveSmallIntegerField(default=0, verbose_name="标准时间（秒）")

	# 等级最短时间（单位：秒）
	min_time = models.PositiveSmallIntegerField(default=0, verbose_name="最短时间（秒）")

	# 管理界面用：显示星级颜色
	def adminColor(self):
		from django.utils.html import format_html

		res = '<div style="background: %s; width: 48px; height: 24px;"></div>' % self.color

		return format_html(res)

	adminColor.short_description = "星级颜色"


# ===================================================
# 游戏配置表
# ===================================================
class GameConfigure(BaseModel):

	class Meta:

		verbose_name = verbose_name_plural = "游戏配置"

	# 全局变量，GameConfigure 实例
	Configure = None

	# 游戏名
	name = models.CharField(default='艾瑟萌学院', max_length=12, verbose_name="游戏名")
	name.filter_type = ['static']

	# 游戏名（英文）
	eng_name = models.CharField(default='Exermon', max_length=24, verbose_name="游戏名（英文）")
	eng_name.filter_type = ['static']

	# 金币
	gold = models.CharField(default='金币', max_length=6, verbose_name="金币")
	gold.filter_type = ['static']

	# 点券
	ticket = models.CharField(default='点券', max_length=6, verbose_name="点券")
	ticket.filter_type = ['static']

	# 绑定点券
	bound_ticket = models.CharField(default='绑定点券', max_length=6, verbose_name="绑定点券")
	bound_ticket.filter_type = ['static']

	# 激活
	# activated = models.BooleanField(default=True, verbose_name="激活")

	def __str__(self):
		return "%d. %s 配置" % (self.id, self.name)

	# region Static Data

	def __convertStaticValues(self, res):

		from item_module.models import UsableItem
		from player_module.models import Character, Player
		from exermon_module.models import Exermon, ExerSkill
		from question_module.models import GeneralQuestion, BaseQuesReport, WrongItem
		from record_module.models import GeneralQuesRecord, GeneralExerciseRecord
		from battle_module.models import BattleRecord, BattlePlayer, BattleRoundResult
		from english_pro_module.models import ExerProCard, ExerProEnemy

		min_birth = ModelUtils.dateToStr(Player.MIN_BIRTH)

		# 配置量
		res['max_subject'] = Subject.MAX_SELECTED
		res['max_exercise_count'] = GeneralExerciseRecord.MAX_COUNT
		res['report_desc_len'] = BaseQuesReport.MAX_DESC_LEN
		res['min_birth'] = min_birth

		# player_module
		res['character_genders'] = Character.CHARACTER_GENDERS
		res['player_grades'] = Player.GRADES
		res['player_statuses'] = Player.STATUSES
		res['player_types'] = Player.TYPES

		# item_module
		res['item_use_target_types'] = UsableItem.TARGET_TYPES

		# exermon_module
		res['exermon_types'] = Exermon.TYPES
		res['exerskill_target_types'] = ExerSkill.TARGET_TYPES
		res['exerskill_hit_types'] = ExerSkill.HIT_TYPES

		# question_module
		res['question_types'] = GeneralQuestion.TYPES
		res['question_statuses'] = GeneralQuestion.STATUSES
		res['ques_report_types'] = BaseQuesReport.TYPES
		res['ques_report_type_descs'] = BaseQuesReport.TYPES_WITH_DESC

		# record_module
		res['record_sources'] = GeneralQuesRecord.SOURCES
		res['exercise_gen_types'] = GeneralExerciseRecord.GEN_TYPES

		# battle_module
		res['battle_modes'] = BattleRecord.MODES
		res['round_result_types'] = BattleRoundResult.RESULT_TYPES
		res['battle_result_types'] = BattlePlayer.RESULT_TYPES
		res['battle_statuses'] = BattlePlayer.STATUSES

		# english_pro_module
		res['correct_types'] = WrongItem.TYPES
		res['card_types'] = ExerProCard.CARD_TYPES
		res['enemy_types'] = ExerProEnemy.ENEMY_TYPES

	def __convertStaticModel(self, data):
		from utils.data_manager import DataManager

		static_clas = CoreDataManager.getStaticData()

		for cla in static_clas:

			cla_name = cla.__name__
			key_name = DataManager.hump2Underline(cla_name) + 's'
			attr_name = cla_name.lower() + '_set'

			objs = getattr(self, attr_name).all()

			data[key_name] = ModelUtils.objectsToDict(objs)

		return data

	def _convertStaticData(self, res, **kwargs):

		self.__convertStaticValues(res)
		self.__convertStaticModel(res)

	# endregion

	# region Dynamic Data

	@classmethod
	def __convertDynamicModel(cls, data):
		from utils.data_manager import DataManager

		dynamic_clas = CoreDataManager.getDynamicData()

		for cla in dynamic_clas:

			cla_name = cla.__name__
			key_name = DataManager.hump2Underline(cla_name) + 's'

			objs = cla.objects.all()

			data[key_name] = ModelUtils.objectsToDict(objs)

		return data

	@classmethod
	def __convertDynamicValues(cls, res):

		from season_module.runtimes import SeasonManager

		cur_season_id = SeasonManager.getCurrentSeason().id

		res['cur_season_id'] = cur_season_id

	@classmethod
	def _convertDynamicData(cls, res, **kwargs):

		cls.__convertDynamicValues(res)
		cls.__convertDynamicModel(res)

	# endregion

	# region Game Data

	@classmethod
	def _convertGameData(cls, res, **kwargs):
		from utils.data_manager import DataManager

		data_clas = CoreDataManager.getGameData()

		for cla in data_clas:

			cla_name = cla.__name__
			key_name = DataManager.hump2Underline(cla_name) + 's'

			objs = cla.objects.all()

			res[key_name] = ModelUtils.objectsToDict(objs)

	# endregion

	@classmethod
	def setup(cls):

		# 获取当前版本
		version: GameVersion = GameVersion.get()

		if version is None:
			raise GameException(ErrorType.NoCurVersion)

		cls.Configure = version.configure

		static_clas = CoreDataManager.getStaticData()

		for cla in static_clas: cla.setup()

	@classmethod
	def get(cls):

		if cls.Configure is None: cls.setup()

		return cls.Configure
