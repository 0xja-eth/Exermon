from .common import Common

from ..exception import GameException, ErrorType
from ..model_utils import Common as ModelUtils
from ..view_utils import Common as ViewUtils

from enum import Enum
import math, random

# region QuestionGen


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
class BaseQuestionGenerateConfigure:

	def __init__(self, question_set, player, subject,
				 count=None, questions=None):

		from record_module.models import QuesSetRecord
		from player_module.models import Player
		from game_module.models import Subject

		self.player: Player = player
		self.subject: Subject = subject
		self.question_set: QuesSetRecord = question_set

		self.questions = questions  # 指定的题目集（QuerySet）

		self.count = count

	def questionClasses(self):

		player_ques_clas = self.question_set.ACCEPT_PLAYER_QUES_CLASSES
		question_clas = []

		for cla in player_ques_clas:
			question_clas.append(cla.QUESTION_CLASS)

		return question_clas

	# 调试用
	def convertToDictForDebug(self):
		res = {}

		for key in dir(self):
			if "__" in key: continue
			res[key] = str(getattr(self, key))

		return res


# ================================
# 题目生成器基类
# ================================
class BaseQuestionGenerator:

	# 生成次数上限
	GENERATE_TIMES_LIMIT = 1024

	def questionClasses(self) -> list:
		raise NotImplementedError

	@classmethod
	def generate(cls, configure: BaseQuestionGenerateConfigure,
				 return_id=False):
		return cls(configure, return_id)

	def __init__(self, configure: BaseQuestionGenerateConfigure,
				 return_id=False):
		self.configure = configure
		self.return_id = return_id
		self.result = self._doGenerate()

	def _doGenerate(self):
		print("QuestionGenerator:\n%s" % self.configure.convertToDictForDebug())

		questions = self._processFilter()

		satisfaction = self._generateSatisfaction(questions)

		return self._generateQuestions(satisfaction, questions)

	# 处理过滤（过滤基本属性的非法题目）
	def _processFilter(self):
		conf = self.configure

		print("==== processFilter ====")

		print("filtering subject: %s" % conf.subject)

		questions = self._rawQuestions(conf.questions, conf.subject)

		print("questions count: %d" % questions.count())

		questions = self._customFilter(questions)

		print("questions count: %d" % questions.count())

		return questions

	def _rawQuestions(self, questions, subject):
		from question_module.views import Common
		
		if questions is not None: return questions

		clas = self.questionClasses()

		if len(clas) == 1:
			return Common.getQuestions(cla=clas[0], subject=subject)

		# TODO: 后期拓展多种类题型的生成

		questions = {}

		for cla in clas:
			questions[cla] = Common.getQuestions(cla=cla, subject=subject)

		return questions

	def _customFilter(self, questions): return questions

	def _generateSatisfaction(self, questions): return questions

	def _generateQuestions(self, satisfaction, questions):

		configure = self.configure
		count = configure.count

		result = []

		ques_len = questions.count()

		sat = self._shuffleQuestions(satisfaction)
		sat_len = len(sat)

		print("sub_len: %d, ques_len: %d" % (sat_len, ques_len))

		for i in range(count):
			if i < sat_len:
				question = sat[i]

				print("in-condition: %d" % question.id)

			else:
				ltd = self.GENERATE_TIMES_LIMIT
				index = random.randint(0, ques_len - 1)
				question = questions[index]

				# 随机出题，保证不重复
				while question in result and ltd > 0:
					index = random.randint(0, ques_len - 1)
					question = questions[index]
					ltd -= 1

				print("out-condition: %d" % question.id)

			result.append(question)

		if self.return_id:
			for i in range(len(result)):
				q = result[i]
				result[i] = (type(q), q.id)

		print("result: %s" % result)

		return result

	# 乱序题目
	@classmethod
	def _shuffleQuestions(cls, questions):

		result = []
		for question in questions:
			result.append(question)
		random.shuffle(result)

		return result


# ================================
# 随机题目生成器
# ================================
class RandomQuestionGenerator(BaseQuestionGenerator):

	def questionClasses(self) -> list:
		return self.configure.questionClasses()

	def _generateSatisfaction(self, questions):
		questions = list(questions)

		if len(questions) >= self.configure.count:
			return questions

		return random.sample(questions, self.configure.count)


# ================================
# 常规题目生成配置
# ================================
class GeneralQuestionGenerateConfigure(BaseQuestionGenerateConfigure):

	def __init__(self, *args, gen_type=QuestionGenerateType.Normal.value,
				 sel_type=None, star_dtb=None, ques_star=None, **kwargs):
		super().__init__(*args, **kwargs)

		from question_module.models import SelectingQuestionType
		from record_module.models import QuesSetType
		from game_module.models import QuestionStar

		self.gen_type: QuestionGenerateType = gen_type  # 生成类型
		self.sel_type: SelectingQuestionType = sel_type  # 限制题目类型
		self.ques_star: QuestionStar = ques_star  # 限制题目星级
		self.star_dtb = star_dtb

		self.ignore_star = self.ques_star is not None or \
						   self.question_set.TYPE == QuesSetType.Exam


# ================================
# 题目生成器类
# ================================
class GeneralQuestionGenerator(BaseQuestionGenerator):

	def questionClasses(self) -> list:
		from question_module.models import GeneralQuestion
		return [GeneralQuestion]

	def _customFilter(self, questions):
		questions = super()._customFilter(questions)

		configure: GeneralQuestionGenerateConfigure = self.configure

		if configure.sel_type is not None:
			print("limiting question type: %s" % configure.sel_type)
			questions = questions.filter(type_id=configure.sel_type.id)

		if configure.ques_star is not None:
			print("limiting question star: %s" % configure.ques_star)
			questions = questions.filter(star_id=configure.ques_star.id)

		return questions

	# 处理条件（分配条件）
	def _generateSatisfaction(self, questions):
		questions = super()._generateSatisfaction(questions)

		from exermon_module.models import ExerSlot, ExerSlotItem

		configure: GeneralQuestionGenerateConfigure = self.configure

		player = configure.player
		subject = configure.subject
		gen_type = configure.gen_type

		sub = questions
		question_cla = self.questionClasses()[0]

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
			# id 限制
			print("limiting id")

			occur_questions = player.quesRecords(question_cla=question_cla)
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


# endregion

# region QuestionSetCalc


# ================================
# 刷题（单题）收益计算类
# ================================
class QuesSetSingleRewardCalc:

	@classmethod
	def calc(cls, player_ques, ques_rec, **kwargs):
		calc_obj = cls(player_ques, ques_rec, **kwargs)
		return calc_obj

	def __init__(self, player_ques, ques_rec, **kwargs):
		from player_module.models import Player
		from question_module.models import GeneralQuestion
		from record_module.models import GeneralQuesRecord, SelectingPlayerQuestion
		from exermon_module.models import ExerSlot, ExerSlotItem

		self.exer_exp_incr = 0
		self.slot_exp_incr = 0
		self.gold_incr = 0

		self.player_ques: SelectingPlayerQuestion = player_ques
		self.ques_rec: GeneralQuesRecord = ques_rec

		self.player: Player = self.ques_rec.player

		self.question: GeneralQuestion = self.player_ques.question

		self.corr = self.player_ques.correct()

		subject_id = self.question.subject_id
		exerslot: ExerSlot = self.player.exerSlot()

		if exerslot is None: return

		self.exerslot_item: ExerSlotItem = \
			exerslot.contItem(subject_id=subject_id)

		if self.exerslot_item is None: return

		self.exer_exp_incr, self.slot_exp_incr = self._calcExerExpIncr()
		self.gold_incr = self._calcGoldIncr()

		self.exer_exp_incr = self._adjust(self.exer_exp_incr)
		self.slot_exp_incr = self._adjust(self.slot_exp_incr)
		self.gold_incr = self._adjust(self.gold_incr)

	def _adjust(self, val):
		return max(0, round(val))

	# 计算艾瑟萌经验值收益
	def _calcExerExpIncr(self):
		raise NotImplementedError

	# 计算金币收益
	def _calcGoldIncr(self):
		raise NotImplementedError


# ================================
# 刷题（单题）收益计算类
# ================================
class ExerciseSingleRewardCalc(QuesSetSingleRewardCalc):
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

		eer = base * cr * elr * err * efr
		eser = base * cr * eslr * err * efr

		return eer, eser

	# 次数修正
	def _applyCount(self, count):
		return pow(self.A, count / self.K1)

	# 艾瑟萌等级修正
	def _applyLevel(self, el, ql):
		delta = el - ql
		return self._levelReward(delta, ql) \
			if delta > 0 else self._levelPunish(delta)

	# 艾瑟萌等级惩罚
	def _levelPunish(self, delta):
		delta = max(delta, 0)
		return (1 - Common.sigmoid(math.sqrt(delta) / self.K2)) * 2

	# 艾瑟萌等级奖励
	def _levelReward(self, delta, ql):
		if ql <= 0: return 1
		return 1 - delta / ql

	# 结果修正
	def _applyExpResult(self, corr):
		return 1 if corr else self.W

	# 经验浮动修正
	def _applyExpFloat(self):
		return 1 + random.randint(-self.EF, self.EF) / 100.0

	# 计算金币收益
	def _calcGoldIncr(self):
		base = self.question.goldIncr()
		grr = self._applyGoldResult(self.corr)
		gfr = self._applyGoldFloat()

		return base * grr * gfr

	# 结果修正
	def _applyGoldResult(self, corr):
		return 1 if corr else 0

	# 经验浮动修正
	def _applyGoldFloat(self):
		return 1 + random.randint(-self.GF, self.GF) / 100.0


# ================================
# 题目集（结算）收益计算类
# ================================
class QuesSetResultRewardCalc:

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
			self.exer_exp_incrs[sid] = \
				self._adjust(self.exer_exp_incrs[sid])
		for sid in self.slot_exp_incrs:
			self.slot_exp_incrs[sid] = \
				self._adjust(self.slot_exp_incrs[sid])

		self.exer_exp_incr = self._adjust(self.exer_exp_incr)
		self.slot_exp_incr = self._adjust(self.slot_exp_incr)
		self.gold_incr = self._adjust(self.gold_incr)

		self.item_rewards = self._calcItemReward()

	def _adjust(self, val):
		return max(0, round(val))

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
class ExerciseResultRewardCalc(QuesSetResultRewardCalc):
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
		return self._corrRatePlus(corr_rate) * self._exerExpPlusRate()

	# 艾瑟萌槽经验奖励加成
	def _slotExpRewardRate(self, corr_rate):
		return self._corrRatePlus(corr_rate) * self._exerSlotExpPlusRate()

	# 金币奖励加成
	def _goldRewardRate(self, corr_rate):
		return self._corrRatePlus(corr_rate) * self._goldPlusRate()

	# 正确率加成
	def _corrRatePlus(self, corr_rate):
		return 1 + corr_rate / 2 - self.B

	# 艾瑟萌经验附加加成
	def _exerExpPlusRate(self):
		return 1

	# 艾瑟萌槽经验附加加成
	def _exerSlotExpPlusRate(self):
		return 1

	# 金币附加加成
	def _goldPlusRate(self):
		return 1


# # ================================
# # 对战（单题）收益计算类
# # ================================
# class BattleSingleRewardCalc(ExerciseSingleRewardCalc):
#
# 	def __init__(self, player_ques, ques_rec, **kwargs):
# 		super().__init__(player_ques, ques_rec, **kwargs)

# endregion
