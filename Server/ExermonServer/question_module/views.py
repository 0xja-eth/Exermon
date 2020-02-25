from django.shortcuts import render
from .models import *
from utils.view_utils import Common as ViewUtils
from utils.exception import ErrorType, GameException

# Create your views here.


# =======================
# 题目服务类，封装管理题目模块的业务处理函数
# =======================
class Service:

	# 查询题目
	@classmethod
	async def get(cls, consumer, player, qids: list):
		# 返回数据：
		# questions: 题目数据（数组） => 题目数据集

		questions = Common.getQuestions(qids)

		questions = ModelUtils.objectsToDict(questions)

		return {'questions': questions}


# =======================
# 题目公用类，封装关于物品模块的公用函数
# =======================
class Common:

	# 获取题目
	@classmethod
	def getQuestion(cls, error: ErrorType = ErrorType.QuestionNotExist,
					**kwargs) -> Question:

		return ViewUtils.getObject(Question, error, **kwargs)

	# 获取多个题目
	@classmethod
	def getQuestions(cls, ids, error: ErrorType = ErrorType.QuestionNotExist):

		unique_ids = list(set(ids))

		res = ViewUtils.getObjects(Question, id__in=ids)

		# 数量不一致，说明获取出现问题
		if res.count() != len(unique_ids): raise GameException(error)

		return res

	# 获取题目糖
	@classmethod
	def getQuesSugar(cls, error: ErrorType = ErrorType.QuesSugarNotExist,
					 **kwargs) -> QuesSugar:

		return ViewUtils.getObject(QuesSugar, error, **kwargs)

	# 确保题目存在
	@classmethod
	def ensureQuestionExist(cls, error: ErrorType = ErrorType.QuestionNotExist, **kwargs):
		return ViewUtils.ensureObjectExist(Question, error, **kwargs)

	# 确保题目糖存在
	@classmethod
	def ensureQuesSugarExist(cls, error: ErrorType = ErrorType.QuesSugarNotExist, **kwargs):
		return ViewUtils.ensureObjectExist(QuesSugar, error, **kwargs)
