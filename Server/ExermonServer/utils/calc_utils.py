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

	# 基本属性值(BPV)	EPB*((实际属性成长率/R+1)*S)^(L-1)

	# 艾瑟萌战斗力计算公式
	# 战斗力(BV)			round((MHP+MMP*2+ATK*6*C*(1+CRI/100)+DEF*4)*(1+EVA/50))

	S = 1.005
	R = 233

	# 计算属性
	@classmethod
	def calc(cls, base, rate, level):
		return base*pow((rate/cls.R+1)*cls.S, level-1)


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

	# 艾瑟萌战斗力计算公式
	# 战斗力(BV)			round((MHP+MMP*2+ATK*6*C*(1+CRI/100)+DEF*4)*(1+EVA/50))

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

		self.player_exer: PlayerExermon = self.exerslot_item.player_exer
		if self.player_exer is None: return

		self.exermon: Exermon = self.player_exer.exermon()
		if self.exermon is None: return

		self.player_gift: PlayerExerGift = self.exerslot_item.player_gift
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

		return ExermonParamCalc.calc(epb, rpr, self.player_exer.level)

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
		param = self._getParam()

		if param is None: return val

		return param.clamp(val)

	# 获取属性实例
	def _getParam(self):
		from game_module.models import BaseParam

		if 'param_id' in self.kwargs:
			return BaseParam.get(id=self.kwargs['param_id'])

		if 'attr' in self.kwargs:
			return BaseParam.get(attr=self.kwargs['attr'])

		return None


# ================================
# 刷题（单题）收益计算类
# ================================
class BattlePointCalc:
	# 暴击加成
	C = 2

	# 计算（object 是实现了 BaseParam 中 attr 属性的对象）
	@classmethod
	def calc(cls, func):
		from game_module.models import BaseParam

		kwargs = {}
		params = BaseParam.objs()
		for param in params:
			kwargs[param.attr] = func(param.id)

		return cls.doCalc(**kwargs)

	# 计算战斗力
	@classmethod
	def doCalc(cls, mhp, mmp, atk, def_, eva, cri):
		return round((mhp + mmp*2 + atk*6*cls.C*(1+cri/100)
					  + def_*4) * (1+eva/50))


# ================================
# 刷题（单题）收益计算类
# ================================
class QuestionSetSingleRewardCalc:

	@classmethod
	def calc(cls, player_ques, ques_rec):
		calc_obj = cls(player_ques, ques_rec)
		return calc_obj

	def __init__(self, player_ques, ques_rec):
		from player_module.models import Player
		from question_module.models import Question
		from record_module.models import QuestionRecord, PlayerQuestion
		from exermon_module.models import ExerSlot, ExerSlotItem

		self.exer_exp_incr = 0
		self.exerslot_exp_incr = 0
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

		self.exer_exp_incr, self.exerslot_exp_incr = self._calcExerExpIncr()
		self.gold_incr = self._calcGoldIncr()

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

		eer = round(base*cr*elr*err*efr)
		eser = round(base*cr*eslr*err*efr)

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
		return (1-Common.sigmoid(math.sqrt(delta)/self.K2))*2

	# 艾瑟萌等级奖励
	def _levelReward(self, delta, ql):
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

		return round(base*grr*gfr)

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
	def calc(cls, player_queses):
		calc_obj = cls(player_queses)
		return calc_obj

	def __init__(self, player_queses):

		self.exer_exp_incr = 0
		self.exerslot_exp_incr = 0
		self.gold_incr = 0

		self.exer_exp_incrs = {}
		self.exerslot_exp_incrs = {}

		self.player_queses = player_queses

		self.exer_exp_incrs, self.exerslot_exp_incrs, \
			self.exer_exp_incr, self.exerslot_exp_incr, \
			self.gold_incr = self._calcSumReward()

	# 计算总收益
	def _calcSumReward(self):
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
		slot_rate = self._exerSlotExpRewardRate(corr_rate)

		for sid in eers: eers[sid] *= exer_rate
		for sid in esers: esers[sid] *= slot_rate

		eer *= exer_rate
		eser *= slot_rate

		gr *= self._goldRewardRate(corr_rate)

		return eers, esers, eer, eser, gr

	# 艾瑟萌经验奖励加成
	def _exerExpRewardRate(self, corr_rate):
		return self._corrRatePlus(corr_rate)*self._exerExpPlusRate()

	# 艾瑟萌槽经验奖励加成
	def _exerSlotExpRewardRate(self, corr_rate):
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
			'question_set': self.question_set,
			'player': str(self.player),
			'subject': str(self.subject),
			'gen_type': str(self.gen_type),
			'questions': self.questions,
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
			max_star = self.getMaxStar(level)

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
			occur_qids = ModelUtils.getObjectRelatedForFilter(occur_questions, 'question_id')

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
				wrong_qids = ModelUtils.getObjectRelatedForFilter(wrong_questions, 'question_id')

				id_limit = wrong_qids

			elif gen_type == QuestionGenerateType.CollectedFirst.value:
				print("CollectedFirst")

				collected_questions = occur_questions.filter(collected=True)
				collected_qids = ModelUtils.getObjectRelatedForFilter(collected_questions, 'question_id')

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
				while ((self.return_id and question.id in result) or \
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
class MaxStarCalc():

	@classmethod
	def calc(cls, level):

		from game_module.models import QuestionStar

		stars = QuestionStar.objs()

		star_id = 0
		for star in stars:
			if level < star.level:
				return star_id
			star_id += 1

		return star_id
