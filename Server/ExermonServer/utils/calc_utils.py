import math


# ================================
# 艾瑟萌等级计算类
# ================================
class ExermonLevelCalc:

	# 星级等级表
	StarLevelTable = None

	# 初始化，计算所有星级的等级表
	@classmethod
	def init(cls):
		from game_module.models import ExerStar
		from .view_utils import Common

		cls.StarLevelTable = {}
		stars = Common.getObjects(ExerStar)

		for s in stars:
			cls.StarLevelTable[s] = cls._generateTable(s)

	# 生成某个星级的等级表
	@classmethod
	def _generateTable(cls, q):
		# from exermon_module.models import PetStar
		# q: PetStar
		res = []

		_max = q.max_level
		a = q.level_exp_factors['a']
		b = q.level_exp_factors['b']
		c = q.level_exp_factors['c']

		# 生成每一等级的最低累计经验值
		for x in range(_max):
			res.append(cls._calcTable(x-1, a, b, c))

		return res

	# 计算表格函数
	@classmethod
	def _calcTable(cls, x, a, b, c):
		return a/3*x*x*x+(a+b)/2*x*x+(a+b*3+c*6)/6*x

	# 获取所需经验值
	@classmethod
	def getDetlaExp(cls, q, level):

		if level >= q.max_level: return -1

		if cls.StarLevelTable is None:
			cls.init()

		data = cls.StarLevelTable[q]
		return data[level]-data[level-1]

	# 获取累计经验
	@classmethod
	def getSumExp(cls, q, level, exp):

		if level > q.max_level: level = q.max_level

		if cls.StarLevelTable is None:
			cls.init()

		return cls.StarLevelTable[q][level-1]+exp


# ================================
# 艾瑟萌属性计算类
# ================================
class ExermonParamCalc:

	S = 1.005
	R = 233

	# 计算属性
	@classmethod
	def calc(cls, base, rate, level):
		return base*pow((rate/cls.R+1)*cls.S, level-1)


# ================================
# 艾瑟萌槽等级计算类
# ================================
class ExermonSlotLevelCalc:

	T = 500
	A = 0.66

	TP = 500
	AP = 0.7
	D = 3

	# 计算等级
	@classmethod
	def calcLevel(cls, exp):
		return math.floor(pow(exp/cls.T, cls.A))+1

	# 计算下一级经验
	@classmethod
	def calcNext(cls, level):
		return math.ceil(pow(level, 1/cls.A)*cls.T)

	# 计算玩家等级
	@classmethod
	def calcPlayerLevel(cls, exp):
		return math.floor(pow(exp/cls.TP/cls.D, cls.AP))+1

	# 计算玩家下一级经验
	@classmethod
	def calcPlayerNext(cls, level):
		return math.ceil(pow(level, 1/cls.AP)*cls.TP*cls.D)

