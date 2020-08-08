from .types import *
from .models import *

from utils.model_utils import EnumMapper


# region 物品系统管理类


class QuesManager:
	"""
	物品管理类
	"""

	@classmethod
	def registerQuestion(cls, verbose_name):
		"""
		注册题目
		Args:
			verbose_name (str): 别名
			# cont_item_cla (type): 容器项类
		"""
		def wrapper(cla: BaseQuestion):
			print("registerQuestion: %s" % cla)

			cla._meta.verbose_name = \
				cla._meta.verbose_name_plural = verbose_name

			name = cla.__name__.replace('Question', '')

			cla.TYPE = eval("QuestionType.%s" % name)

			# cla.CONTITEM_CLASS = cont_item_cla

			EnumMapper.registerClass(cla)

			return cla

		return wrapper

	@classmethod
	def registerQuesPicture(cls, question_cla: BaseQuestion):
		"""
		注册题目图片
		Args:
			question_cla (BaseQuestion): 题目类型
		"""
		def wrapper(cla: BaseQuesPicture):
			verbose_name = question_cla._meta.verbose_name + "图片"

			cla._meta.verbose_name = \
				cla._meta.verbose_name_plural = verbose_name

			cla.QUESTION_CLASS = question_cla
			question_cla.PICTURE_CLASS = cla

			question = models.ForeignKey(question_cla,
				on_delete=models.CASCADE, verbose_name="题目")

			cla.add_to_class('question', question)

			return cla

		return wrapper

	@classmethod
	def registerQuesChoice(cls, question_cla: SelectingQuestion):
		"""
		注册题目选项
		Args:
			question_cla (SelectingQuestion): 题目类型
		"""
		def wrapper(cla: BaseQuesChoice):
			verbose_name = question_cla._meta.verbose_name + "选项"

			cla._meta.verbose_name = \
				cla._meta.verbose_name_plural = verbose_name

			cla.QUESTION_CLASS = question_cla
			question_cla.CHOICE_CLASS = cla

			question = models.ForeignKey(question_cla,
				on_delete=models.CASCADE, verbose_name="题目")

			cla.add_to_class('question', question)

			return cla

		return wrapper

	@classmethod
	def registerSubQuestion(cls, question_cla: GroupQuestion):
		"""
		注册组合题目
		Args:
			question_cla (GroupQuestion): 题目类型
		"""
		def wrapper(cla: BaseQuestion):
			verbose_name = question_cla._meta.verbose_name + "小题"

			cla._meta.verbose_name = \
				cla._meta.verbose_name_plural = verbose_name

			question_cla.SUB_QUES_CLASS = cla

			question = models.ForeignKey(question_cla,
				on_delete=models.CASCADE, verbose_name="题目")

			cla.add_to_class('question', question)

			return cla

		return wrapper

	@classmethod
	def registerQuesRecord(cls, question_cla: BaseQuestion):
		"""
		注册题目记录
		Args:
			question_cla (BaseQuestion): 题目类型
		"""
		def wrapper(cla: BaseQuesRecord):
			verbose_name = question_cla._meta.verbose_name + "反馈"

			cla._meta.verbose_name = \
				cla._meta.verbose_name_plural = verbose_name

			cla.QUESTION_CLASS = question_cla
			question_cla.RECORD_CLASS = cla

			question = models.ForeignKey(question_cla,
										 on_delete=models.CASCADE, verbose_name="题目")

			cla.add_to_class('question', question)

			return cla

		return wrapper

	@classmethod
	def registerQuesReport(cls, question_cla: BaseQuestion):
		"""
		注册题目反馈
		Args:
			question_cla (BaseQuestion): 题目类型
		"""
		def wrapper(cla: BaseQuesReport):
			verbose_name = question_cla._meta.verbose_name + "反馈"

			cla._meta.verbose_name = \
				cla._meta.verbose_name_plural = verbose_name

			cla.QUESTION_CLASS = question_cla
			question_cla.REPORT_CLASS = cla

			question = models.ForeignKey(question_cla,
										 on_delete=models.CASCADE, verbose_name="题目")

			cla.add_to_class('question', question)

			return cla

		return wrapper


# endregion