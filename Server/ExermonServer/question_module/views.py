from django.shortcuts import render
from .models import *
from utils.view_utils import Common as ViewUtils
from utils.exception import ErrorType, ErrorException

# Create your views here.


# =======================
# 题目服务类，封装管理题目模块的业务处理函数
# =======================
class Service:
	pass


# =======================
# 题目公用类，封装关于物品模块的公用函数
# =======================
class Common:

	# 获取题目
	@classmethod
	def getQuestion(cls, return_type='object', error: ErrorType = ErrorType.QuestionNotExist, **args) -> BaseItem:

		return ViewUtils.getObject(Question, error, return_type=return_type, **args).target()

	# 获取题目糖
	@classmethod
	def getQuesSugar(cls, return_type='object', error: ErrorType = ErrorType.QuesSugarNotExist, **args) -> BaseItem:

		return ViewUtils.getObject(QuesSugar, error, return_type=return_type, **args).target()

	# 确保题目存在
	@classmethod
	def ensureQuestionExist(cls, error: ErrorType = ErrorType.QuestionNotExist, **args):
		return ViewUtils.ensureObjectExist(Question, error, **args)

	# 确保题目糖存在
	@classmethod
	def ensureQuesSugarExist(cls, error: ErrorType = ErrorType.QuesSugarNotExist, **args):
		return ViewUtils.ensureObjectExist(QuesSugar, error, **args)
