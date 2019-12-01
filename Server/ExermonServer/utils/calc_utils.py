import math

# ================================
# 宠物等级计算类
# ================================
class PetLevelCalc:

	# 品质等级表
	QualityLevelTable = None

	# 初始化，计算所有品质的等级表
	@classmethod
	def init(cls):
		from pet_module.models import PetQuality
		from .view_utils import Common

		cls.QualityLevelTable = {}
		qualities = Common.getObjects(PetQuality)

		for q in qualities:
			cls.QualityLevelTable[q] = cls._generateTable(q)

	# 生成某个品质的等级表
	@classmethod
	def _generateTable(cls, q):
		# from pet_module.models import PetQuality
		# q: PetQuality
		res = []

		_max = q.max_level
		a = q.level_exp_factors['a']
		b = q.level_exp_factors['b']
		c = q.level_exp_factors['c']

		# 生成每一等级的最低累计经验值
		for x in range(_max):
			res.append(cls._calcTable(x-1, a, b, c))

		return res

	# 极端表格函数
	@classmethod
	def _calcTable(cls, x, a, b, c):
		return a/3*x*x*x+(a+b)/2*x*x+(a+b*3+c*6)/6*x

	# 获取所需经验值
	@classmethod
	def getDetlaExp(cls, q, level):

		if level >= q.max_level: return -1

		if cls.QualityLevelTable is None:
			cls.init()

		data = cls.QualityLevelTable[q]
		return data[level]-data[level-1]

	# 获取累计经验
	@classmethod
	def getSumExp(cls, q, level, exp):

		if level > q.max_level: level = q.max_level

		if cls.QualityLevelTable is None:
			cls.init()

		return cls.QualityLevelTable[q][level-1]+exp


# ================================
# 宠物属性计算类
# ================================
class PetParamCalc:

	STD_RATE = 1.01

	# 计算属性
	@classmethod
	def calc(cls, base, rate, level):
		return base*pow((rate/100+1)*cls.STD_RATE, level-1)


# ================================
# 宠物槽等级计算类
# ================================
class PetSlotLevelCalc:

	D = 300
	X = 0.75
	M = 3

	# 计算等级
	@classmethod
	def calcLevel(cls, exp):
		return math.floor(pow(exp/cls.D, cls.X))+1

	# 计算下一级经验
	@classmethod
	def calcNext(cls, level):
		return pow(level,1/cls.X)*cls.D

	# 计算玩家等级
	@classmethod
	def calcPlayerLevel(cls, exp):
		return math.floor(pow(exp/cls.D/cls.M, cls.X))+1

	# 计算玩家下一级经验
	@classmethod
	def calcPlayerNext(cls, level):
		return pow(level,1/cls.X)*cls.D*cls.M

