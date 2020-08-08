from .types import *
from .models import *

from utils.calc_utils import *
from utils.model_utils import EnumMapper


# region 物品系统管理类


class RecordManager:
	"""
	物品管理类
	"""

	@classmethod
	def registerQuestionSet(cls, verbose_name,
							ques_gen_cla: BaseQuestionGenerator,
							reward_calc: QuestionSetResultRewardCalc):
		"""
		注册题目集
		Args:
			verbose_name (str): 别名
			ques_gen_cla (BaseQuestionGenerator): 题目生成类
			reward_calc (QuestionSetResultRewardCalc): 奖励生成类
			# cont_item_cla (type): 容器项类
		"""
		def wrapper(cla: QuestionSetRecord):
			print("registerQuestionSet: %s" % cla)

			cla._meta.verbose_name = \
				cla._meta.verbose_name_plural = verbose_name

			name = cla.__name__.replace('Record', '')

			cla.TYPE = eval("QuestionSetType.%s" % name)

			cla.REWARD_CALC = reward_calc
			cla.QUES_GEN_CLASS = ques_gen_cla

			EnumMapper.registerClass(cla)

			return cla

		return wrapper

	@classmethod
	def registerQuestionSetReward(cls, question_set_cla: QuestionSetRecord):
		"""
		注册题目集奖励
		Args:
			question_set_cla (QuestionSetRecord): 题目集类
		"""
		def wrapper(cla: QuestionSetReward):
			question_set_name = question_set_cla._meta.verbose_name

			verbose_name = question_set_name + "奖励"

			cla._meta.verbose_name = \
				cla._meta.verbose_name_plural = verbose_name

			question_set_cla.REWARD_CLASS = cla

			record = models.ForeignKey(question_set_cla,
				on_delete=models.CASCADE, verbose_name=question_set_name)

			cla.add_to_class('record', record)

			return cla

		return wrapper

	@classmethod
	def registerPlayerQuestion(cls, question_cla: BaseQuestion,
							   question_set_cla: QuestionSetRecord,
							   reward_calc=QuestionSetSingleRewardCalc,
							   source=RecordSource.Others):
		"""
		注册题目图片
		Args:
			question_cla (BaseQuestion): 题目类型
			question_set_cla (QuestionSetRecord): 题目集类型
			reward_calc (QuestionSetSingleRewardCalc): 奖励计算类
			source (RecordSource): 来源
		"""
		def wrapper(cla: BasePlayerQuestion):
			question_set_name = question_set_cla._meta.verbose_name

			verbose_name = question_set_name + "题目记录"

			cla._meta.verbose_name = \
				cla._meta.verbose_name_plural = verbose_name

			question_set_cla.ACCEPT_PLAYER_QUES_CLASSES.append(cla)

			cla.QUESTION_CLASS = question_cla
			cla.QUESTION_SET_CLASS = question_set_cla

			cla.REWARD_CALC = reward_calc
			cla.SOURCE = source

			question = models.ForeignKey(question_cla,
				on_delete=models.CASCADE, verbose_name="题目")

			question_set = models.ForeignKey(question_set_cla,
				on_delete=models.CASCADE, verbose_name=question_set_name)

			cla.add_to_class('question', question)
			cla.add_to_class('question_set', question_set)

			return cla

		return wrapper


# endregion