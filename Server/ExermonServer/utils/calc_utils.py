import math, random
from .exception import GameException, ErrorType
from enum import Enum


# ================================
# 通用计算
# ================================
class Common:

	# Sigmoid 函数
	@classmethod
	def sigmoid(cls, x):
		return 1/(1+math.exp(-x))


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
	def _generateTable(cls, star):
		"""
		生成某个星级的等级表
		Args:
			star (ExerStar): 艾瑟萌星级
		Returns:
			返回单个星级等级经验表格
		"""
		res = []

		_max = star.max_level
		a = star.level_exp_factors['a']
		b = star.level_exp_factors['b']
		c = star.level_exp_factors['c']

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

		if cls.StarLevelTable is None: cls.init()

		data = cls.StarLevelTable[q]
		return data[level]-data[level-1]

	# 获取下级所需经验值
	@classmethod
	def getDetlaSumExp(cls, q, level):

		if level >= q.max_level: return -1

		if cls.StarLevelTable is None: cls.init()

		data = cls.StarLevelTable[q]
		return data[level]

	# 获取累计经验
	@classmethod
	def getSumExp(cls, q, level, exp):

		if level > q.max_level: level = q.max_level

		if cls.StarLevelTable is None: cls.init()

		return cls.StarLevelTable[q][level-1]+exp


# ================================
# 艾瑟萌属性计算类
# ================================
class ExermonParamCalc:

	# 基本属性值(BPV)	EPB*((实际属性成长率/R+1)*S)^(L-1)

	S = 1.005
	R = 233

	# 计算属性
	@classmethod
	def calc(cls, base, rate, level, plus=0, plus_rate=1):
		value = base*pow((rate/cls.R+1)*cls.S, level-1)
		return value * plus_rate + plus


# ================================
# 艾瑟萌槽和玩家等级计算类
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

	# 计算对应等级所需累计经验
	@classmethod
	def calcExp(cls, level):
		return math.ceil(pow(level-1, 1.0/cls.A)*cls.T)

	# 计算玩家等级
	@classmethod
	def calcPlayerLevel(cls, exp):
		return math.floor(pow(exp/cls.TP/cls.D, cls.AP))+1

	# 计算玩家对应等级所需累计经验
	@classmethod
	def calcPlayerExp(cls, level):
		return math.ceil(pow(level-1, 1/cls.AP)*cls.TP*cls.D)


# ================================
# 艾瑟萌槽项属性成长率计算类
# ================================
class ExerSlotItemParamRateCalc:

	# 计算属性
	@classmethod
	def calc(cls, exerslot_item, **kwargs):
		if exerslot_item is None: return 0

		player_exer = exerslot_item.player_exer
		if player_exer is None: return 0
		epr = player_exer.paramRate(**kwargs)

		player_gift = exerslot_item.player_gift
		if player_gift is None: return epr
		gprr = player_gift.paramRate(**kwargs)

		return epr*gprr


# ================================
# 艾瑟萌槽项属性计算类
# ================================
class ExerSlotItemParamCalc:

	# 艾瑟萌属性值计算公式
	# 实际属性值(RPV)	(基本属性值+附加属性值)*实际加成率+追加属性值
	# 基本属性值(BPV)	EPB*((实际属性成长率/R+1)*S)^(L-1)
	# 实际属性成长率(RPR)	EPR*GPRR
	# 附加属性值(PPV)	EPPV+SPPV
	# 实际加成率(RR)		基础加成率*附加加成率
	# 基础加成率(BR)		1
	# 附加加成率(PR)		GPR*SPR（对战时还包括题目糖属性加成率）
	# 追加属性值(APV)	SAPV

	# 计算属性
	@classmethod
	def calc(cls, exerslot_item, **kwargs):
		calc_obj = cls(exerslot_item, **kwargs)
		return calc_obj.value

	def __init__(self, exerslot_item, **kwargs):
		from exermon_module.models import Exermon, ExerSlotItem, \
			PlayerExermon, PlayerExerGift, ExerEquipSlot
		self.value = 0

		self.exerslot_item: ExerSlotItem = exerslot_item
		if exerslot_item is None: return

		self.player_exer: PlayerExermon = self.exerslot_item.playerExer()
		if self.player_exer is None: return

		self.exermon: Exermon = self.player_exer.exermon()
		if self.exermon is None: return

		self.player_gift: PlayerExerGift = self.exerslot_item.playerGift()
		self.exerequip_slot: ExerEquipSlot = self.exerslot_item.exerEquipSlot()

		self.kwargs = kwargs

		self.value = self._calc()

	# 计算 RPV = (基本属性值+附加属性值)*实际加成率+追加属性值
	def _calc(self):

		bpv = self._calcBaseParamValue()
		ppv = self._calcPlusParamValue()
		rr = self._calcRealRate()
		apv = self._calcAppendParamValue()

		val = (bpv+ppv)*rr+apv
		return self._adjustParamValue(val)

	# 计算 BPV = EPB*((实际属性成长率/R+1)*S)^(L-1)
	def _calcBaseParamValue(self):

		epb = self.player_exer.paramBase(**self.kwargs)
		rpr = self._calcRealParamRate()

		plus = self.player_exer.plusParamVal(**self.kwargs)
		plus_rate = self.player_exer.plusParamRate(**self.kwargs)

		return ExermonParamCalc.calc(epb, rpr,
			 self.player_exer.level, plus, plus_rate)

	# 计算 RPR = EPR*GPRR
	def _calcRealParamRate(self):

		return ExerSlotItemParamRateCalc.calc(self.exerslot_item, **self.kwargs)

	# 计算 PPV = EPPV+SPPV
	def _calcPlusParamValue(self):
		return self._calcEquipPlusParamValue() + \
			   self._calcStatusPlusParamValue()

	# 计算 EPPV 装备附加值
	def _calcEquipPlusParamValue(self):
		if self.exerequip_slot is None: return 0
		return self.exerequip_slot.param(**self.kwargs)

	# 计算 SPPV 状态附加值
	def _calcStatusPlusParamValue(self): return 0

	# 计算 RR = 基础加成率*附加加成率
	def _calcRealRate(self):
		return self._calcBaseRate()*self._calcPlusRate()

	# 计算 BR
	def _calcBaseRate(self): return 1

	# 计算 PR = GPR*SPR（对战时还包括题目糖属性加成率）
	def _calcPlusRate(self): return 1

	# 计算 APV
	def _calcAppendParamValue(self): return 0

	# 调整值
	def _adjustParamValue(self, val):
		param = self.getParam(**self.kwargs)

		if param is None: return val

		return param.clamp(val)

	# 获取属性实例
	@classmethod
	def getParam(cls, param_id=None, attr=None):
		from game_module.models import BaseParam

		if param_id is not None:
			return BaseParam.get(id=param_id)
		if attr is not None:
			return BaseParam.get(attr=attr)
		return None


# ================================
# 装备属性计算类
# ================================
class EquipParamCalc:

	# 计算属性
	@classmethod
	def calc(cls, equipslot_item, param_id=None, attr=None):
		calc_obj = cls(equipslot_item, param_id, attr)
		return calc_obj.value

	def __init__(self, equipslot_item, param_id=None, attr=None):
		from exermon_module.models import PlayerExermon, \
			ExerSlotItem, ExerEquipSlotItem, ExerPackEquip

		self.value = 0

		self.equipslot_item: ExerEquipSlotItem = equipslot_item

		self.pack_equip: ExerPackEquip = self.equipslot_item.pack_equip
		if self.pack_equip is None: return

		self.equip = self.pack_equip.item
		if self.equip is None: return

		self.exerslot_item: ExerSlotItem = \
			self.equipslot_item.container.exer_slot

		self.player_exer: PlayerExermon = self.exerslot_item.player_exer
		if self.player_exer is None: return

		self.param = ExerSlotItemParamCalc.getParam(param_id, attr)
		if self.param is None: return

		self.value = self._calc()

	def _calc(self):

		value = self.pack_equip.baseParam(param_id=self.param.id)
		# rate = self.pack_equip.levelParam(param_id=self.param.id)
		# level = self.player_exer.level

		return value # + rate * level


# ================================
# 战斗力计算类
# ================================
class BattlePointCalc:

	# 艾瑟萌战斗力计算公式
	# 战斗力(BV)			round((MHP+MMP*2+ATK*6*C*(1+CRI/100)+DEF*4)*(1+EVA/50))

	@classmethod
	def calc(cls, func: callable):
		"""
		计算战斗力
		Args:
			func (callable): 一个可以传入属性ID获得相应属性值的函数 
		Returns:
			返回计算后的战斗力
		"""
		from game_module.models import BaseParam

		kwargs = {}
		params = BaseParam.objs()
		for param in params:
			attr = param.attr
			if attr == 'def': attr = 'def_'
			kwargs[attr] = func(param.id)

		return cls.doCalc(**kwargs)

	# 计算战斗力
	@classmethod
	def doCalc(cls, mhp, mmp, atk, def_, eva, cri):
		return round((mhp + mmp*2 + atk*6*BattleAttackProcessor.C
					  * (1+cri/100) + def_*4) * (1+eva/50))


# ================================
# 刷题（单题）收益计算类
# ================================
class QuestionSetSingleRewardCalc:

	@classmethod
	def calc(cls, player_ques, ques_rec, **kwargs):
		calc_obj = cls(player_ques, ques_rec, **kwargs)
		return calc_obj

	def __init__(self, player_ques, ques_rec, **kwargs):
		from player_module.models import Player
		from question_module.models import Question
		from record_module.models import QuestionRecord, PlayerQuestion
		from exermon_module.models import ExerSlot, ExerSlotItem

		self.exer_exp_incr = 0
		self.slot_exp_incr = 0
		self.gold_incr = 0

		self.player_ques: PlayerQuestion = player_ques
		self.ques_rec: QuestionRecord = ques_rec

		self.player: Player = self.ques_rec.player

		self.question: Question = self.player_ques.question

		self.corr = self.player_ques.correct()

		subject_id = self.question.subject_id
		exerslot: ExerSlot = self.player.exerSlot()

		if exerslot is None: return

		self.exerslot_item: ExerSlotItem = \
			exerslot.contItem(subject_id=subject_id)

		if self.exerslot_item is None: return

		self.exer_exp_incr, self.slot_exp_incr = self._calcExerExpIncr()
		self.gold_incr = self._calcGoldIncr()

		self.exer_exp_incr = max(0, round(self.exer_exp_incr))
		self.slot_exp_incr = max(0, round(self.slot_exp_incr))
		self.gold_incr = max(0, round(self.gold_incr))

	# 计算艾瑟萌经验值收益
	def _calcExerExpIncr(self):
		raise NotImplementedError

	# 计算金币收益
	def _calcGoldIncr(self):
		raise NotImplementedError


# ================================
# 刷题（单题）收益计算类
# ================================
class ExerciseSingleRewardCalc(QuestionSetSingleRewardCalc):

	# 单道题目奖励计算公式
	# 艾瑟萌经验奖励(EER)
	# round(基础经验奖励 * 次数修正 * 艾瑟萌等级修正 * 经验结果修正 * 经验波动修正)

	# 艾瑟萌槽经验奖励(ESER)
	# round(基础经验奖励 * 次数修正 * 艾瑟萌槽等级修正 * 经验结果修正 * 经验波动修正)

	# 金币奖励(GR)
	# round(基础金币奖励 * 金币结果修正 * 金币波动修正)

	# 基础经验奖励(BER)
	# BE

	# 次数修正(CR)
	# A ^ (C / K1)

	# 艾瑟萌等级修正(ELR)
	# 艾瑟萌等级差 > 0 ? 艾瑟萌等级惩罚 : 艾瑟萌等级奖励

	# 艾瑟萌等级差(DEL)
	# EL - L - QL

	# 艾瑟萌等级奖励
	# 1 - 艾瑟萌等级差 / (L + QL)

	# 艾瑟萌等级惩罚
	# (1-sigmoid(sqrt(艾瑟萌等级差)/K2))*2

	# 艾瑟萌槽等级修正(ESLR)
	# 艾瑟萌槽等级差 > 0 ? 艾瑟萌槽等级惩罚 : 艾瑟萌槽等级奖励

	# 艾瑟萌槽等级差(DESL)
	# ESL - L - QL

	# 艾瑟萌槽等级奖励
	# 1 - 艾瑟萌槽等级差 / (L + QL)

	# 艾瑟萌槽等级惩罚
	# (1-sigmoid(sqrt(艾瑟萌槽等级差)/K2))*2

	# 经验结果修正(ERR)
	# CORRECT ? 1 : W

	# 经验波动修正(EFR)
	# 1 + rand(EF * 2) - EF

	# 基础金币奖励(BGR)
	# BG

	# 金币结果修正(GRR)
	# CORRECT

	# 金币波动修正(GFR)
	# 1 + rand(GF * 2) - GF

	K1 = 5
	K2 = 20

	A = 0.7

	EF = 5  # 5%
	GF = 10  # 10%
	W = 0.1

	# 计算艾瑟萌经验值收益
	def _calcExerExpIncr(self):
		base = self.question.expIncr()
		count = self.ques_rec.count
		ques_level = self.question.sumLevel()
		exer_level = self.exerslot_item.exermonLevel()
		exerslot_level = self.exerslot_item.slotLevel()

		cr = self._applyCount(count)
		elr = self._applyLevel(exer_level, ques_level)
		eslr = self._applyLevel(exerslot_level, ques_level)
		err = self._applyExpResult(self.corr)
		efr = self._applyExpFloat()

		eer = base*cr*elr*err*efr
		eser = base*cr*eslr*err*efr

		return eer, eser

	# 次数修正
	def _applyCount(self, count):
		return pow(self.A, count/self.K1)

	# 艾瑟萌等级修正
	def _applyLevel(self, el, ql):
		delta = el-ql
		return self._levelReward(delta, ql) \
			if delta > 0 else self._levelPunish(delta)

	# 艾瑟萌等级惩罚
	def _levelPunish(self, delta):
		delta = max(delta, 0)
		return (1-Common.sigmoid(math.sqrt(delta)/self.K2))*2

	# 艾瑟萌等级奖励
	def _levelReward(self, delta, ql):
		if ql <= 0: return 1
		return 1-delta/ql

	# 结果修正
	def _applyExpResult(self, corr):
		return 1 if corr else self.W

	# 经验浮动修正
	def _applyExpFloat(self):
		return 1+random.randint(-self.EF, self.EF)/100.0

	# 计算金币收益
	def _calcGoldIncr(self):
		base = self.question.goldIncr()
		grr = self._applyGoldResult(self.corr)
		gfr = self._applyGoldFloat()

		return base*grr*gfr

	# 结果修正
	def _applyGoldResult(self, corr):
		return 1 if corr else 0

	# 经验浮动修正
	def _applyGoldFloat(self):
		return 1+random.randint(-self.GF, self.GF)/100.0


# ================================
# 题目集（结算）收益计算类
# ================================
class QuestionSetResultRewardCalc:

	@classmethod
	def calc(cls, question_set, player_queses, **kwargs):
		calc_obj = cls(question_set, player_queses, **kwargs)
		return calc_obj

	def __init__(self, question_set, player_queses, **kwargs):

		self.question_set = question_set

		self.exer_exp_incr = 0
		self.slot_exp_incr = 0
		self.gold_incr = 0

		self.exer_exp_incrs = {}
		self.slot_exp_incrs = {}

		self.item_rewards = []

		self.player_queses = player_queses

		self.exer_exp_incrs, self.slot_exp_incrs, \
			self.exer_exp_incr, self.slot_exp_incr, \
			self.gold_incr = self._calcSumReward()

		for sid in self.exer_exp_incrs:
			self.exer_exp_incrs[sid] = max(0, round(self.exer_exp_incrs[sid]))
		for sid in self.slot_exp_incrs:
			self.slot_exp_incrs[sid] = max(0, round(self.slot_exp_incrs[sid]))

		self.exer_exp_incr = max(0, round(self.exer_exp_incr))
		self.slot_exp_incr = max(0, round(self.slot_exp_incr))
		self.gold_incr = max(0, round(self.gold_incr))

		self.item_rewards = self._calcItemReward()

	# 计算总收益
	def _calcSumReward(self) -> (dict, dict, int, int, int):
		raise NotImplementedError

	# 计算物品奖励（返回一个列表，每个元素格式如下：
	# {item: BaseItem, count: int} 表示物品对象以及数量
	def _calcItemReward(self) -> list:
		raise NotImplementedError


# ================================
# 刷题（结算）收益计算类
# ================================
class ExerciseResultRewardCalc(QuestionSetResultRewardCalc):

	# 刷题结算奖励计算公式
	# 艾瑟萌经验奖励(EER)
	# round(∑单道题目艾瑟萌经验奖励 * 艾瑟萌经验奖励加成)

	# 艾瑟萌槽经验奖励(ESER)
	# round(∑单道题目艾瑟萌槽经验奖励 * 艾瑟萌槽经验奖励加成)

	# 金币奖励(GR)
	# round(∑单道题目金币奖励 * 金币奖励加成)

	# 艾瑟萌经验奖励加成(EERP)
	# 正确率加成 * 艾瑟萌经验附加加成

	# 艾瑟萌槽经验奖励加成(ESERP)
	# 正确率加成 * 艾瑟萌槽经验附加加成

	# 金币奖励加成(GRP)
	# 正确率加成 * 金币附加加成

	# 正确率加成(CRP)
	# 1 + 正确率 / 2 - B

	# 艾瑟萌经验附加加成
	# EEIP * EIP

	# 艾瑟萌槽经验附加加成
	# ESEIP * EIP

	# 金币附加加成
	# GIP

	B = 0.15

	# 计算总收益
	def _calcSumReward(self):

		eers = {}
		esers = {}
		eer = eser = gr = 0
		corr_cnt = 0

		for player_ques in self.player_queses:
			subject_id = player_ques.question.subject_id

			if subject_id not in eers: eers[subject_id] = 0
			if subject_id not in esers: esers[subject_id] = 0

			corr_cnt += int(player_ques.correct())

			eers[subject_id] += player_ques.exp_incr
			esers[subject_id] += player_ques.slot_exp_incr

			eer += player_ques.exp_incr
			eser += player_ques.slot_exp_incr
			gr += player_ques.gold_incr

		corr_rate = corr_cnt / len(self.player_queses)

		exer_rate = self._exerExpRewardRate(corr_rate)
		slot_rate = self._slotExpRewardRate(corr_rate)

		for sid in eers: eers[sid] *= exer_rate
		for sid in esers: esers[sid] *= slot_rate

		eer *= exer_rate
		eser *= slot_rate

		gr *= self._goldRewardRate(corr_rate)

		return eers, esers, eer, eser, gr

	# 计算物品奖励
	def _calcItemReward(self) -> list:
		# TODO: 完成刷题物品奖励逻辑
		return []

	# 艾瑟萌经验奖励加成
	def _exerExpRewardRate(self, corr_rate):
		return self._corrRatePlus(corr_rate)*self._exerExpPlusRate()

	# 艾瑟萌槽经验奖励加成
	def _slotExpRewardRate(self, corr_rate):
		return self._corrRatePlus(corr_rate)*self._exerSlotExpPlusRate()

	# 金币奖励加成
	def _goldRewardRate(self, corr_rate):
		return self._corrRatePlus(corr_rate)*self._goldPlusRate()

	# 正确率加成
	def _corrRatePlus(self, corr_rate):
		return 1+corr_rate/2-self.B

	# 艾瑟萌经验附加加成
	def _exerExpPlusRate(self):
		return 1

	# 艾瑟萌槽经验附加加成
	def _exerSlotExpPlusRate(self):
		return 1

	# 金币附加加成
	def _goldPlusRate(self):
		return 1


# ===================================================
#  题目生成类型枚举
# ===================================================
class QuestionGenerateType(Enum):
	Normal = 0  # 普通模式
	OccurFirst = 1  # 已做优先
	NotOccurFirst = 2  # 未做优先
	WrongFirst = 3  # 错题优先
	CollectedFirst = 4  # 收藏优先
	SimpleFirst = 5  # 简单题优先
	MiddleFirst = 6  # 中等题优先
	DifficultFirst = 7  # 难题优先


# ================================
# 题目生成配置
# ================================
class QuestionGenerateConfigure:

	def __init__(self, question_set, player, subject,
				 gen_type=QuestionGenerateType.Normal.value,
				 count=None, star_dtb=None, questions=None,
				 ques_type=None, ques_star=None):
		from record_module.models import QuestionSetRecord, QuestionSetType
		from question_module.models import QuestionType
		from player_module.models import Player
		from game_module.models import Subject, QuestionStar

		self.player: Player = player
		self.subject: Subject = subject
		self.question_set: QuestionSetRecord = question_set
		self.gen_type: QuestionGenerateType = gen_type  # 生成类型
		self.ques_type: QuestionType = ques_type  # 限制题目类型
		self.ques_star: QuestionStar = ques_star  # 限制题目星级

		self.questions = questions  # 指定的题目集（QuerySet）

		self.count = count
		self.star_dtb = star_dtb

		self.ignore_star = self.ques_star is not None or \
						   self.question_set.TYPE == QuestionSetType.Exam

	# 调试用
	def convertToDictForDebug(self):
		return {
			# 'question_set': self.question_set,
			'player': str(self.player),
			'subject': str(self.subject),
			'gen_type': str(self.gen_type),
			# 'questions': self.questions,
			'count': self.count,
			'star_dtb': self.star_dtb,
			'ques_type': str(self.ques_type),
			'ques_star': str(self.ques_star),
			'ignore_star': self.ignore_star,
		}


# ================================
# 题目生成器类
# ================================
class QuestionGenerator:
	GENERATE_TIMES_LIMIT = 1024

	@classmethod
	def generate(cls, configure: QuestionGenerateConfigure, return_id=False):
		generator = cls(configure, return_id)
		return generator

	def __init__(self, configure: QuestionGenerateConfigure, return_id=False):
		self.configure = configure
		self.return_id = return_id
		self.result = self.doGenerate()

	# 生成题目
	def doGenerate(self):

		print("QuestionGenerator:\n%s" % self.configure.convertToDictForDebug())

		questions = self.processFilter()

		sub = self.processConditions(questions)

		return self.processGenerate(sub, questions)

	# 处理过滤（过滤非法题目）
	def processFilter(self):
		from question_module.views import Common as QuestionCommon

		configure = self.configure

		print("==== processFilter ====")

		print("filtering subject: %s" % configure.subject)

		questions = configure.questions

		if questions is None:
			questions = QuestionCommon.getQuestions(subject=configure.subject)

		print("questions count: %d" % questions.count())

		if configure.ques_type is not None:
			print("limiting question type: %s" % configure.ques_type)
			questions = questions.filter(type_id=configure.ques_type.id)

		if configure.ques_star is not None:
			print("limiting question star: %s" % configure.ques_star)
			questions = questions.filter(star_id=configure.ques_star.id)

		print("questions count: %d" % questions.count())

		return questions

	# 处理条件（分配条件）
	def processConditions(self, questions):

		from exermon_module.models import ExerSlot, ExerSlotItem

		configure = self.configure

		player = configure.player
		subject = configure.subject
		gen_type = configure.gen_type

		sub = questions

		print("==== processConditions ====")

		print("questions count: " + str(questions.count()))

		# 如果不忽略玩家的最大题目星级限制
		# 处理星级限制
		if not configure.ignore_star:

			exer_slot: ExerSlot = player.exerSlot()
			slot_item: ExerSlotItem = exer_slot.contItem(subject_id=subject.id)
			level = slot_item.slotLevel()

			min_star = 1
			max_star = self.getMaxStar(level).id

			print("slot level: %d, max_star: %d" % (level, max_star))

			if gen_type >= QuestionGenerateType.SimpleFirst.value:
				# 根据星级生成模式
				print("limiting star")

				if gen_type == QuestionGenerateType.SimpleFirst.value:
					print("SimpleFirst")

					max_star = int((max_star - 1) / 2) + 1

				elif gen_type == QuestionGenerateType.MiddleFirst.value:
					print("MiddleFirst")

					min_star = int((max_star - 1) / 4) + 1
					max_star = int((max_star - 1) / 4 * 3) + 1

				elif gen_type == QuestionGenerateType.DifficultFirst.value:
					print("DifficultFirst")

					min_star = int((max_star - 1) / 2) + 1

			print("min_star, max_star: %d, %d" % (min_star, max_star))

			sub = sub.filter(star_id__in=range(min_star, max_star))

		elif gen_type >= QuestionGenerateType.SimpleFirst.value:
			raise GameException(ErrorType.InvalidGenerateConfigure)

		# 处理ID限制
		if QuestionGenerateType.OccurFirst.value <= gen_type <= \
			QuestionGenerateType.CollectedFirst.value:
			from .model_utils import Common as ModelUtils

			# id 限制
			print("limiting id")

			occur_questions = player.questionRecords()
			occur_qids = ModelUtils.getObjectRelatedForAll(
				occur_questions, 'question', 'question_id')

			id_limit = []
			exclude = False

			if gen_type == QuestionGenerateType.OccurFirst.value:
				print("OccurFirst")

				id_limit = occur_qids

			elif gen_type == QuestionGenerateType.NotOccurFirst.value:
				print("NotOccurFirst")

				id_limit = occur_qids
				exclude = True

			elif gen_type == QuestionGenerateType.WrongFirst.value:
				print("WrongFirst")

				wrong_questions = occur_questions.filter(wrong=True)
				wrong_qids = ModelUtils.getObjectRelatedForFilter(wrong_questions, 'question')
				wrong_qids = list(q.id for q in wrong_qids)

				id_limit = wrong_qids

			elif gen_type == QuestionGenerateType.CollectedFirst.value:
				print("CollectedFirst")

				collected_questions = occur_questions.filter(collected=True)
				collected_qids = ModelUtils.getObjectRelatedForFilter(collected_questions, 'question')
				collected_qids = list(q.id for q in collected_qids)

				id_limit = collected_qids

			print("id_limit: %s" % id_limit)
			print("id_limit count: %d" % len(id_limit))

			if exclude:
				sub = questions.exclude(id__in=id_limit)
			else:
				sub = questions.filter(id__in=id_limit)

		print("sub questions count: %d" % sub.count())
		for q in sub:
			print("star_id: %d" % q.star_id)

		return sub

	# 处理生成
	def processGenerate(self, sub, questions):

		# from record_module.models import QuestionSetType
		#
		# configure = self.configure
		# question_set = configure.question_set

		print("==== processGenerate ====")

		return self.processGeneralGenerate(sub, questions)

	# 默认生成方式
	def processGeneralGenerate(self, sub, questions):
		configure = self.configure
		count = configure.count

		result = []
		ques_len = questions.count()

		sub = self.shuffleQuestions(sub)
		sub_len = len(sub)

		print("sub_len: %d, ques_len: %d" % (sub_len, ques_len))

		for i in range(count):
			if i < sub_len:
				question = sub[i]

				print("in-condition: %d" % question.id)

			else:
				ltd = self.GENERATE_TIMES_LIMIT
				index = random.randint(0, ques_len-1)
				question = questions[index]

				# 随机出题，保证不重复
				while ((self.return_id and question.id in result) or
						(not self.return_id and question in result)) \
						and ltd > 0:

					index = random.randint(0, ques_len-1)
					question = questions[index]
					ltd -= 1

				print("out-condition: %d" % question.id)

			if self.return_id:
				result.append(question.id)
			else:
				result.append(question)

		print("result: %s" % result)

		return result

	# 乱序题目
	@classmethod
	def shuffleQuestions(cls, questions):

		result = []
		for question in questions:
			result.append(question)
		random.shuffle(result)

		return result

	# 获取最大星级编号
	def getMaxStar(self, level):

		return MaxStarCalc.calc(level)


# ===================================================
# 计算最大星级类
# player: Player
# ===================================================
class MaxStarCalc:

	@classmethod
	def calc(cls, level):

		from game_module.models import QuestionStar

		stars = QuestionStar.objs()

		last_star = stars[0]
		for star in stars:
			if level < star.level:
				return last_star
			last_star = star

		return last_star

# # ================================
# # 对战（单题）收益计算类
# # ================================
# class BattleSingleRewardCalc(ExerciseSingleRewardCalc):
#
# 	def __init__(self, player_ques, ques_rec, **kwargs):
# 		super().__init__(player_ques, ques_rec, **kwargs)


# ===================================================
# 通用物品效果处理类
# ===================================================
class GeneralItemEffectProcessor:

	@classmethod
	def process(cls, cont_item, player, count, target=None, target_slot_item=None):
		processor = GeneralItemEffectProcessor(
			cont_item, player, count, target, target_slot_item)
		processor._process()

	def __init__(self, cont_item, player, count, target=None, target_slot_item=None):
		from player_module.models import Player
		from item_module.models import PackContItem
		from exermon_module.models import PlayerExermon, ExerSlotItem

		self.player: Player = player
		self.cont_item: PackContItem = cont_item
		self.target_slot_item: ExerSlotItem = target_slot_item
		self.target: PlayerExermon = target
		self.count = count

	def _process(self):
		"""
		开始处理
		"""
		from item_module.models import UsableItem

		item = self.cont_item.item

		if isinstance(item, UsableItem):
			self._processUsableItem(item)

	def _processUsableItem(self, item):
		"""
		处理 可使用物品 的使用效果
		Args:
			item (UsableItem): 可使用物品
		"""
		for effect in item.effects():
			self.__processEffect(effect)

	def __processEffect(self, effect):
		"""
		处理具体效果
		Args:
			effect (BaseEffect): 效果
		"""
		from item_module.models import BaseItem, ItemEffectCode
		from item_module.views import Common as ItemCommon

		player = self.player
		target = self.target
		target_slot_item = self.target_slot_item
		count = self.count

		code = ItemEffectCode(effect.code)
		params = effect.params

		p_len = len(params)

		if code == ItemEffectCode.AddParam and target:
			if p_len == 3: params[2] = params[2]/100+1
			target.addPlusParam(*params, count=count)

		elif code == ItemEffectCode.TempAddParam and target:
			if p_len == 4: params[3] = params[3]/100+1
			target.addTempPlusParam(*params, count=count)

		elif code == ItemEffectCode.GainExermonExp and target:
			min_cnt = max_cnt = params[0]*count
			if p_len == 2: max_cnt = params[1]*count

			value = random.randint(min_cnt, max_cnt)
			target.gainExp(value)

		elif code == ItemEffectCode.GainPlayerExp:
			min_cnt = max_cnt = params[0]*count
			if p_len == 2: max_cnt = params[1]*count

			value = random.randint(min_cnt, max_cnt)
			player.gainExp(value)

		elif code == ItemEffectCode.GainItem:
			type_, item_id = params[0], params[1]
			min_cnt = max_cnt = params[2]*count

			if p_len == 4: max_cnt = params[3]*count

			item: BaseItem = ItemCommon.getItem(type_, id=item_id)

			container_class = item.contItemClass().containerClass()
			container = player.getContainer(container_class)

			value = random.randint(min_cnt, max_cnt)
			container.gainItems(item, value)

		elif code == ItemEffectCode.GainGold:
			min_cnt = max_cnt = params[0]*count
			if p_len == 2: max_cnt = params[1]*count

			value = random.randint(min_cnt, max_cnt)
			player.gainMoney(gold=value)

		elif code == ItemEffectCode.GainBoundTicket:
			min_cnt = max_cnt = params[0]*count
			if p_len == 2: max_cnt = params[1]*count

			value = random.randint(min_cnt, max_cnt)
			player.gainMoney(bound_ticket=value)

		elif code == ItemEffectCode.Eval:
			for i in range(count):
				eval(params[0])

		target.refresh()
		if target_slot_item is not None:
			target_slot_item.refresh()


# ===================================================
# 物品效果处理类
# ===================================================
class BattleItemEffectProcessor:

	@classmethod
	def process(cls, item, player):
		processor = BattleItemEffectProcessor(item, player)
		processor._process()

	def __init__(self, item, player):
		from item_module.models import PackContItem
		from battle_module.runtimes import \
			RuntimeBattlePlayer, RuntimeBattleExermon

		self.cont_item: PackContItem = item
		# self.battle: RuntimeBattle = battle
		self.player: RuntimeBattlePlayer = player
		self.exermon: RuntimeBattleExermon = self.player.getCurrentExermon()

	def _process(self):
		"""
		开始处理
		"""
		from item_module.models import UsableItem
		from question_module.models import QuesSugar

		item = self.cont_item.item

		if isinstance(item, UsableItem):
			self._processUsableItem(item)
			self.player.addPrepareAction(item)

		if isinstance(item, QuesSugar):
			self._processQuesSugar(item)
			self.player.addPrepareAction(item)

	def _processUsableItem(self, item):
		"""
		处理 可使用物品 的使用效果
		Args:
			item (UsableItem): 可使用物品
		"""
		for effect in item.effects():
			self.__processEffect(effect)

	def __processEffect(self, effect):
		"""
		处理具体效果
		Args:
			effect (BaseEffect): 效果
		"""
		from item_module.models import ItemEffectCode

		player = self.player
		exermon = self.exermon

		code = ItemEffectCode(effect.code)
		params = effect.params.copy()

		p_len = len(params)

		if code == ItemEffectCode.RecoverHP:
			if p_len == 1: player.recoverHP(params[0])
			if p_len == 2: player.recoverHP(params[0], params[1]/100)

		if code == ItemEffectCode.RecoverMP:
			if p_len == 1: exermon.recoverMP(params[0])
			if p_len == 2: exermon.recoverMP(params[0], params[1]/100)

		if code == ItemEffectCode.BattleAddParam:
			if p_len == 4: params[3] = params[3]/100+1
			exermon.addParam(*params)

	def _processQuesSugar(self, item):
		"""
		处理 题目糖 的使用效果
		Args:
			item (QuesSugar): 题目糖
		"""
		params = item.params()
		exermon = self.player.getCurrentExermon()
		exermon.addBuff(rate_params=list(params))


# ===================================================
# 对战攻击处理类
# ===================================================
class BattleAttackProcessor:

	# 实际伤害点数(RD)
	# round(((基础伤害 + 附加伤害) * 伤害加成 + 追加伤害) * 攻击结果修正 * 波动修正)
	#
	# 基础伤害(BD)
	# AATK * 实际攻击比率 - BDEF * 实际防御比率 + 附加伤害
	#
	# 实际攻击比率
	# AR * 技能攻击比率
	#
	# 实际防御比率
	# DR * 技能防御比率
	#
	# 附加伤害
	# 技能附加伤害公式
	#
	# 附加伤害(PD)
	# 0（用于拓展）
	#
	# 伤害加成(DR)
	# 1（用于拓展）
	#
	# 追加伤害(AD)
	# 0（用于拓展）
	#
	# 攻击结果修正(RR)
	# 回避修正 * 暴击修正
	#
	# 回避修正(MR)
	# rand() < BEVA / 100 ? 0 : 1
	#
	# 暴击修正(CR)
	# rand() < ACRI / 100 ? C : 1
	#
	# 波动修正(FR)
	# 1 + rand(F * 2) - F

	AR = 3  # 攻击比率
	DR = 1  # 防御比率

	C = 2  # 暴击伤害加成
	F = 8  # 伤害波动（*100）

	class TempParam:
		"""
		临时属性类
		"""
		MHP = MMP = ATK = DEF = EVA = CRI = 0

		def __init__(self, exermon):
			from battle_module.runtimes import RuntimeBattleExermon
			self.exermon: RuntimeBattleExermon = exermon
			self.load()

		def load(self):
			from game_module.models import BaseParam

			params = BaseParam.objs()

			for param in params:
				attr = str(param.attr).upper()
				if hasattr(self, attr):
					val = self.exermon.paramVal(param_id=param.id)
					setattr(self, attr, val.getValue())

	@classmethod
	def process(cls, attacker, oppo):
		processor = BattleAttackProcessor(attacker, oppo)
		processor._processSkill()
		processor._processAction()

	# region 处理内容

	def __init__(self, attacker, oppo):
		from exermon_module.models import ExerSkill, TargetType
		from battle_module.models import HitResultType
		from battle_module.runtimes import RuntimeBattlePlayer, RuntimeBattleExermon

		self.a_player: RuntimeBattlePlayer = attacker
		self.a_exer: RuntimeBattleExermon = self.a_player.getCurrentExermon()

		self.o_player: RuntimeBattlePlayer = oppo
		self.o_exer: RuntimeBattleExermon = self.o_player.getCurrentExermon()

		self.skills = self.a_exer.getUsableSkills()
		self.skill: ExerSkill = None

		self.target_type: TargetType = TargetType.Enemy
		self.hit_result: HitResultType = HitResultType.Unknown
		self.hurt = 0

	def _generateTargets(self, target_type=None):
		"""
		生成目标玩家
		Args:
			oppo (RuntimeBattlePlayer): 对方
			target_type (TargetType): 目标类型
		"""
		from exermon_module.models import TargetType

		if target_type is None:
			if self.skill is None:
				target_type = TargetType.Enemy
			else:
				target_type = TargetType(self.skill.target_type)

		if target_type == TargetType.BothRandom:
			target_type = random.choice([TargetType.Self, TargetType.Enemy])

		self.target_type = target_type

		if target_type == TargetType.Enemy:
			return [self.o_player]

		if target_type == TargetType.Self:
			return [self.a_player]

		if target_type == TargetType.Both:
			return [self.a_player, self.o_player]

		return []

	def _processSkill(self):
		"""
		处理技能使用
		"""
		from battle_module.runtimes import RuntimeBattleExerSkill

		skill: RuntimeBattleExerSkill = self._generateRandomSkill()

		if skill is None: self.skill = None
		else:
			self.skill = skill.skill
			skill.useSkill()

	def _generateRandomSkill(self):
		"""
		生成随机使用技能
		"""
		for skill in self.skills:
			rand = random.randint(1, 100)
			if rand <= skill.skill.rate:
				return skill

		return None

	def _processAction(self):
		"""
		处理行动
		"""
		for target in self._generateTargets():
			self._processAttack(target)

	def _processAttack(self, target):
		"""
		处理攻击
		Args:
			target (RuntimeBattlePlayer): 目标
		"""
		from battle_module.models import HitResultType, TargetType
		from battle_module.runtimes import RuntimeBattlePlayer, RuntimeBattleExermon

		target: RuntimeBattlePlayer = target
		b_exer: RuntimeBattleExermon = target.getCurrentExermon()

		a = self.TempParam(self.a_exer)
		b = self.TempParam(b_exer)

		self.hit_result = self._calcResultType(a, b)

		if self.hit_result != HitResultType.Miss:

			bd = self._baseDamage(a, b)
			pd = self._plusDamage()
			dr = self._damageRate()
			ad = self._appendDamage()
			fr = self._floatRate()

			rd = ((bd+pd)*dr+ad)*fr

			if self.hit_result == HitResultType.Critical:
				rd *= self._criticalRate()

			self.hurt = round(rd)

		target_type = self.target_type
		if target_type == TargetType.Both:
			if target == self.a_player: target_type = TargetType.Self
			if target == self.o_player: target_type = TargetType.Enemy

		self._addAttackAction(target_type)
		self._applyAttack(target)

		target.round_result.processAttack(self.skill,
			self.target_type, self.hit_result, self.hurt, False)

		# 当目标类型为双方时，如果目标与自身一致，则不再处理攻击者（避免处理两次）
		if not (self.target_type == TargetType.Both and target == self.a_player):
			self.a_player.round_result.processAttack(self.skill,
				self.target_type, self.hit_result, self.hurt, True)

	def _addAttackAction(self, target_type=None):
		"""
		添加攻击行动
		"""
		if target_type is None: target_type = self.target_type

		self.a_player.addAttackAction(self.skill, target_type, self.hit_result, self.hurt)

	def _applyAttack(self, target):
		"""
		应用攻击
		Args:
			target (RuntimeBattlePlayer): 目标
		"""
		from exermon_module.models import HitType
		from battle_module.runtimes import RuntimeBattleExermon

		if self.skill is None:
			hit_type = HitType.HPDamage
		else:
			hit_type = HitType(self.skill.hit_type)

		b_exer: RuntimeBattleExermon = target.getCurrentExermon()

		if hit_type == HitType.HPDamage:
			target.recoverHP(-self.hurt)

		elif hit_type == HitType.HPRecover:
			target.recoverHP(self.hurt)

		elif hit_type == HitType.HPDrain:
			target.recoverHP(-self.hurt)
			drain_val = self.hurt * self.skill.drain_rate
			self.a_player.recoverHP(drain_val)

		elif hit_type == HitType.MPDamage:
			b_exer.recoverMP(-self.hurt)

		elif hit_type == HitType.MPRecover:
			b_exer.recoverMP(self.hurt)

		elif hit_type == HitType.MPDrain:
			b_exer.recoverMP(-self.hurt)
			drain_val = self.hurt * self.skill.drain_rate
			self.a_exer.recoverMP(drain_val)

	def _calcResultType(self, a, b):
		"""
		计算攻击结果
		"""
		from battle_module.models import HitResultType

		rand = random.random()

		if rand <= self._realEVA(a, b): return HitResultType.Miss
		if rand <= self._realCRI(a, b): return HitResultType.Critical

		return HitResultType.Hit

	def _realEVA(self, a, b):
		"""
		实际回避率
		"""
		if self.skill is None: return b.EVA
		return b.EVA - self.skill.hit_rate/100

	def _realCRI(self, a, b):
		"""
		实际暴击率
		"""
		if self.skill is None: return a.CRI
		return a.CRI + self.skill.cri_rate/100

	def _criticalRate(self):
		"""
		暴击修正
		"""
		return self.C

	def _baseDamage(self, a, b):
		"""
		基础伤害
		"""
		return a.ATK * self._realAttackRate() - b.DEF * self._realDefenseRate()

	def _realAttackRate(self):
		"""
		实际攻击比率
		"""
		return self.AR * self._skillAttackRate()

	def _skillAttackRate(self):
		"""
		技能攻击比率
		"""
		return 1 if self.skill is None else self.skill.atk_rate

	def _realDefenseRate(self):
		"""
		实际防御比率
		"""
		return self.DR * self._skillDefenseRate()

	def _skillDefenseRate(self):
		"""
		技能防御比率
		"""
		return 1 if self.skill is None else self.skill.def_rate

	def _skillPlusDamage(self, a, b):
		"""
		技能附加伤害
		"""
		try: return eval(self.skill.plus_formula)
		except: return 0

	def _plusDamage(self):
		"""
		附加伤害
		"""
		return 0

	def _damageRate(self):
		"""
		伤害加成
		"""
		return 1

	def _appendDamage(self):
		"""
		追加伤害
		"""
		return 0

	def _floatRate(self):
		"""
		波动修正
		"""
		return 1+random.randint(-self.F, self.F)/100.0

	# endregion

	"""占位符"""


# ===================================================
# 对战对战结果收益计算类
# ===================================================
class BattleResultRewardCalc(ExerciseResultRewardCalc):

	EXER_EXP_RATE = 0.75
	SLOT_EXP_RATE = 0.75
	GOLD_RATE = 0.5

	CREDIT_REWARD = 5
	CREDIT_PUNISH = [10, 20]

	# @classmethod
	# def process(cls, battle):
	# 	processor = cls(battle)
	# 	processor._judgeWinner()
	# 	processor._processBattler(processor.battle_player1)
	# 	processor._processBattler(processor.battle_player2)

	def __init__(self, battle_player, player_queses, battle):
		# battle_player 即 question_set
		super().__init__(battle_player, player_queses)

		from battle_module.runtimes import RuntimeBattle, RuntimeBattlePlayer
		from battle_module.models import BattleRecord, BattlePlayer

		self.battle: RuntimeBattle = battle

		self.battle_record: BattleRecord = battle.record

		self.self_battle_player: BattlePlayer = \
			self.battle_record.getBattlePlayer(battle_player=battle_player)
		self.oppo_battle_player: BattlePlayer = \
			self.battle_record.getOppoBattlePlayer(battle_player=battle_player)

		self.self_runtime_battler: RuntimeBattlePlayer = \
			self.battle.getBattler(battle_player=battle_player)
		self.oppo_runtime_battler: RuntimeBattlePlayer = \
			self.battle.getOppoBattler(battle_player=battle_player)

		self.result = self._calcResult()
		self.status = self._calcStatus()

		self.credit_incr = self._calcCreditIncr()

		self.battle_scores, self.score_incr = self._calcScores()

		self.judge, self.star_incr = self._calcJudgeAndStarIncr()

		print("self.judge:"+str(self.judge))

	# 艾瑟萌经验奖励加成
	def _exerExpRewardRate(self, corr_rate):
		return super()._exerExpRewardRate(corr_rate)*self.EXER_EXP_RATE

	# 艾瑟萌槽经验奖励加成
	def _slotExpRewardRate(self, corr_rate):
		return super()._slotExpRewardRate(corr_rate) * self.SLOT_EXP_RATE

	# 金币奖励加成
	def _goldRewardRate(self, corr_rate):
		return super()._goldRewardRate(corr_rate)*self.GOLD_RATE

	# 计算对战结果
	def _calcResult(self):
		from battle_module.models import BattlePlayerResult

		if self.self_runtime_battler.isDead():
			return BattlePlayerResult.Lose

		if self.oppo_runtime_battler.isDead():
			return BattlePlayerResult.Win

		return BattlePlayerResult.Tie

	# 计算对战状态
	def _calcStatus(self):
		from battle_module.models import BattlePlayerStatus

		return BattlePlayerStatus.Normal

	# 计算信誉积分奖励
	def _calcCreditIncr(self):
		from battle_module.models import BattlePlayerStatus

		if self.status == BattlePlayerStatus.Normal:
			return self.CREDIT_REWARD

		if self.status == BattlePlayerStatus.Cancelled:
			return random.randint(*self.CREDIT_PUNISH)

		return 0

	# 计算赛季积分奖励
	def _calcScores(self):

		battle_scores = BattleScoreCalc.calc(self.self_battle_player,
							 self.self_runtime_battler,
							 self.oppo_runtime_battler)

		score_incr = BattleRankScoreCalc.calc(self.self_battle_player,
											  self.oppo_battle_player)

		return battle_scores, score_incr

	def __battleScore(self):
		return (self.battle_scores.time_score + self.battle_scores.hurt_score +
				self.battle_scores.damage_score + self.battle_scores.recovery_score +
				self.battle_scores.correct_score) / 5 + self.battle_scores.plus_score

	# 计算评价结果及星星奖励
	def _calcJudgeAndStarIncr(self):
		from battle_module.models import BattleResultJudge, BattlePlayerResult

		score = self.__battleScore()
		judges = BattleResultJudge.objs()

		battle_judge: BattleResultJudge = None

		for judge in judges:
			if score >= judge.score:
				battle_judge = judge
			else: break

		if self.result == BattlePlayerResult.Win:
			return battle_judge, battle_judge.win

		return battle_judge, battle_judge.lose


# ===================================================
# 对战积分计算类
# ===================================================
class BattleScoreCalc:

	# 对战评分(BS)	(用时分 数 +伤害分 数 +承伤分 数 +回复分 数 +行动分数 ) / 5 +奖励分数
	# 用时分数(TS)	 -100 * (AT / ST) * (AT / ST) + 100
	# 伤害分数(HS)
	# 伤害率 > PX ? 高伤害分数: 常规伤害分数
	# 承伤分数(DS)
	# 100 - (承伤率 > 100 ? 高承伤分数: 常规承伤分数)
	# 回复分数(RS)	(回复 率= =0 ? 1 : 1- sigmoid((回复率 - MX) / K4))*100
	# 行动分数(AS)	(- 1 /100 ) *行动 率 *(行动 率 -200)
	# 奖励分数(PS)		0（用于拓展）
	# 高伤害分数		P Y +K 1 *(伤害 率 -PX)
	# 常规伤害分数	 A* 伤害率 * 伤害率
	# 高承伤分数
	# K2 * 100 + K3 * (承伤率 - 100)
	# 常规承伤分数
	# 承伤率 * K2
	# 伤害率(HR)
	# AH / BMHP * 100
	# 承伤率(DR)
	# AD / AMHP * 100
	# 回复率(RR)
	# AR / AMHP * 100
	# 行动率(AR)
	# AA / CNT * 100

	PY = 85
	PX = 80

	A = PY/PX/PX

	K1 = 0.25
	K2 = 0.333
	K3 = 0.233

	MX = 100
	K4 = 20

	@classmethod
	def calc(cls, battler, self_battler, oppo_battler):
		calc_obj = cls(battler, self_battler, oppo_battler)
		return calc_obj

	def __init__(self, battler, self_battler, oppo_battler):

		from battle_module.models import BattlePlayer
		from battle_module.runtimes import RuntimeBattlePlayer

		self.battler: BattlePlayer = battler

		self.self_battler: RuntimeBattlePlayer = self_battler
		self.oppo_battler: RuntimeBattlePlayer = oppo_battler

		self._calc()

	def _calc(self):
		self.time_score = self._clamp(self._calcTimeScore())
		self.hurt_score = self._clamp(self._calcHurtScore())
		self.damage_score = self._clamp(self._calcDamageScore())
		self.recovery_score = self._clamp(self._calcRecoveryScore())
		self.correct_score = self._clamp(self._calcCorrectScore())
		self.plus_score = self._clamp(self._calcPlusScore())

	def _clamp(self, val):
		return max(min(val, 100), 0)

	# 计算时间分数
	def _calcTimeScore(self):
		at = self.battler.sumTimespan()
		st = self.battler.sumStdTime() * 1000

		return -100*(at/st)*(at/st)+100

	# 计算伤害分数
	def _calcHurtScore(self):
		hr = self.__calcHurtRate()
		return self.__highHurtScore(hr) \
			if hr > self.PX else self.__stdHurtScore(hr)

	def __calcHurtRate(self):
		ah = self.battler.sumHurt()
		bmhp = self.oppo_battler.mhp
		return ah / bmhp * 100  # 伤害率

	def __highHurtScore(self, hr):
		return self.PY+self.K1*(hr-self.PX)

	def __stdHurtScore(self, hr):
		return self.A*hr*hr

	# 计算承伤分数
	def _calcDamageScore(self):
		dr = self.__calcDamageRate()
		return 100 - (self.__highDamageScore(dr)
			if dr > 100 else self.__stdDamageScore(dr))

	def __calcDamageRate(self):
		ad = self.battler.sumDamage()
		amhp = self.self_battler.mhp
		return ad / amhp * 100  # 承伤率

	def __highDamageScore(self, dr):
		return self.K2*100+self.K3*(dr-100)

	def __stdDamageScore(self, dr):
		return dr*self.K2

	# 计算回复分数
	def _calcRecoveryScore(self):
		ar = self.__calcRecoveryRate()
		if ar <= 0: return 100
		return 1-Common.sigmoid((ar-self.MX)/self.K4)*100

	def __calcRecoveryRate(self):
		ar = self.battler.sumRecovery()
		amhp = self.self_battler.mhp
		return ar / amhp * 100  # 承伤率

	# 计算正确分数
	def _calcCorrectScore(self):
		cr = self.battler.corrRate()*100
		return -0.01*cr*(cr-200)

	# 计算奖励分数
	def _calcPlusScore(self):
		return 0


# ===================================================
# 对战赛季积分计算类
# ===================================================
class BattleRankScoreCalc:

	# 赛季积分奖励(RS)
	# round(对战评分 / (max(50, ARF + ARF - BRF) / 100)) - 50

	@classmethod
	def calc(cls, battler1, battler2):
		calc_obj = cls(battler1, battler2)
		return calc_obj.value

	def __init__(self, battler1, battler2):
		from battle_module.models import BattlePlayer
		from season_module.models import SeasonRecord
		from player_module.models import Player
		from player_module.views import Common as PlayerCommon

		self.battler1: BattlePlayer = battler1
		self.battler2: BattlePlayer = battler2

		online_player = PlayerCommon.getOnlinePlayer(self.battler1.player_id)

		if online_player is not None:
			self.player1: Player = online_player.player
		else:
			self.player1: Player = self.battler1.player

		online_player = PlayerCommon.getOnlinePlayer(self.battler2.player_id)

		if online_player is not None:
			self.player2: Player = online_player.player
		else:
			self.player2: Player = self.battler2.player

		self.season_record1: SeasonRecord = self.player1.currentSeasonRecord()
		self.season_record2: SeasonRecord = self.player2.currentSeasonRecord()

		self.value = self._calc()

	def _calc(self):
		from season_module.models import CompRank

		rank1, _, _ = self.season_record1.rank()
		rank2, _, _ = self.season_record2.rank()

		rank1: CompRank = rank1
		rank2: CompRank = rank2

		arf = rank1.score_factor
		brf = rank2.score_factor

		return round(self.battler1.sumScore()/(max(50, arf+arf-brf)/100))-50


# ===================================================
# 根据星星数量计算出当前段位数和子段位
# ===================================================
class CompRankCalc:

	@classmethod
	def starNum2Rank(cls, star_num) -> ('CompRank', int, int):
		"""
		原来：#def rank(self) -> ('CompRank', int):
		根据星星数计算当前实际段位
		Returns:
			返回实际段位对象（CompRank），子段位数目（从0开始）以及剩余星星数
		Examples:
			0 = > 学渣I(1, 0, 0)
			1 = > 学渣I(1, 0, 1)
			2 = > 学渣I(1, 0, 2)
			3 = > 学渣I(1, 0, 3)
			4 = > 学渣II(1, 1, 1)
			5 = > 学渣II(1, 1, 2)
			6 = > 学渣II(1, 1, 3)
			7 = > 学渣III(1, 2, 1)
			10 = > 学酥I(2, 1, 1)
		"""
		from season_module.models import CompRank

		# ranks 储存了段位列表中的每一个段位的详细信息
		ranks = CompRank.objs()

		# 需要保证数据库的数据有序
		for rank in ranks:
			rank_stars = rank.rankStars()

			# 判断最后一个段位
			if rank_stars == 0:
				return rank, 0, star_num

			# 如果星星数目还可以扣
			if star_num > rank_stars:
				star_num -= rank_stars
			else:
				tmp_star = star_num - 1
				if tmp_star < 0:
					sub_rank = star_num = 0
				else:
					sub_rank = int(tmp_star / CompRank.STARS_PER_SUBRANK)
					star_num = (tmp_star % CompRank.STARS_PER_SUBRANK) + 1

				return rank, sub_rank, star_num

		return None, 0, star_num

	@classmethod
	def rank2StarNum(cls, sub_rank: int, star_num: int = 0,
					 rank: 'CompRank' = None, rank_id: int = None) -> int:
		"""
		根据段位信息计算星星数
		Args:
			sub_rank (int): 子短位数
			star_num (int): 剩余星星数
			rank (CompRank): 段位实例
			rank_id (int): 段位ID
		Returns:
			返回由段位计算出来的星星数量
		"""
		from season_module.models import CompRank

		if rank_id is None: rank_id = rank.id

		# ranks 储存了段位列表中的每一个段位的详细信息
		ranks = CompRank.objs()
		rank_id_ = 1  # 用于标识大段位
		# star_num = star_num

		for rank_ in ranks:
			if rank_id_ < rank_id:
				star_num += rank_.rankStars()
				rank_id_ += 1

			# 大段位计算好了，剩下小段位
			if rank_id_ == rank_id:
				star_num += sub_rank * CompRank.STARS_PER_SUBRANK
				break

		return star_num


# ===================================================
# 赛季切换，根据当前的段位星星计算新赛季的初始段位星星-lgy4.15
# ===================================================
class NewSeasonStarNumCalc:

	@classmethod
	def calc(cls, star_num):

		rank, sub_rank, star_num = CompRankCalc.starNum2Rank(star_num)
		data = (rank.id, sub_rank)

		# 段位元组第三位，不满三个的星星直接清零了
		if data < (3, 2): new_rank = (1, 1, 0)
		elif data < (3, 4): new_rank = (3, 2, 0)
		elif data < (4, 2): new_rank = (3, 3, 0)
		elif data < (4, 4): new_rank = (3, 4, 0)
		elif data < (5, 2): new_rank = (4, 3, 0)
		elif data < (5, 5): new_rank = (4, 4, 0)
		else: new_rank = (4, 5, 0)

		star_num = CompRankCalc.rank2StarNum(
			new_rank[1], new_rank[2], rank_id=new_rank[0])

		return star_num
