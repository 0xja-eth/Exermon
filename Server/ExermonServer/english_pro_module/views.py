from typing import Any

from .models import *
from player_module.models import Player
from utils.view_utils import Common as ViewUtils
from utils.exception import ErrorType, GameException

import time
import random


# Create your views here.


# =======================
# 英语模块服务类，封装管理英语模块的业务处理函数
# =======================
class Service:

	# 生成英语特训题目
	@classmethod
	async def generateQuestions(cls, consumer, player: Player, type: int, count: int, ):
		# 返回数据：
		# qids: int[] => 生成的题目ID集
		# 检验类型是否在枚举类型里面
		Check.ensureQuestionType(type)

		# 检验数量是否合法
		Check.ensureQuestionCount(count)

		# 返回对应类型的题目ID集
		qids = Common.generateQuestionList(count, type)

		return {'qids': qids}

	# 查询英语特训题目
	@classmethod
	async def getQuestions(cls, consumer, player: Player, type: int, qids: list, ):
		# 返回数据：
		# questions: 英语特训题目数据（数组） => 获取的题目数据（听力/阅读/改错）
		# 检验类型是否在枚举类型里面
		Check.ensureQuestionType(type)

		if type == 1:
			questions = Common.getQuestions(ids=qids, QuestionType=ListeningQuestion)
		elif type == 2:
			questions = Common.getQuestions(ids=qids, QuestionType=InfinitiveQuestion)
		elif type == 3:
			questions = Common.getQuestions(ids=qids, QuestionType=ReadingQuestion)

		questions = ModelUtils.objectsToDict(questions)
		return {'questions': questions}

	# 生成当前轮单词
	@classmethod
	async def generateWords(cls, consumer, player: Player, ):
		# 返回数据：
		# words: 单词数据（数组） => 单词数据集
		pass

	# 回答当前轮单词
	@classmethod
	async def answerWord(cls, consumer, player: Player, ):
		# 返回数据：
		# correct: bool => 回答是否正确
		pass

	# 查询单词
	@classmethod
	async def getWords(cls, consumer, player: Player, wids: list,):
		# 返回数据：
		# words: 单词数据（数组） => 单词数据集
		words = Common.getWords(wids)
		words = ModelUtils.objectsToDict(words)

		return {'words': words}

	# 查询单词记录
	@classmethod
	async def getRecords(cls, consumer, player: Player, ):
		# 返回数据：
		# records: 单词记录数据（数组） => 单词记录数据集
		records = Common.getWordsRecords()

		return {'records': records}


# ======================
# 英语模块校验类，封装英语模块业务数据格式校验的函数
# =======================
class Check:

	# 校验要生成题目的数量>=1
	@classmethod
	def ensureQuestionCount(cls, count: int):
		if count < 1:
			raise GameException(ErrorType.InvalidQuestionCount)

	# 校验要生成题目的类型是否合法
	@classmethod
	def ensureQuestionType(cls, type: int):
		ViewUtils.ensureEnumData(type, QuestionType, ErrorType.InvalidQuestionType, True)


# =======================
# 英语模块公用类，封装关于英语模块的公用函数
# =======================
class Common:

	# 从数据库中随机选出题目
	@classmethod
	def generateQuestionList(cls, count: int, question_type: int):
		"""
		获取题目ID集
		Args:
			question_type (int): 题目类型
			count (int) 题目数量
		Returns:
			返回对应类型题目的ID集，若超过题库数量抛出设置好的异常
		"""
		if question_type == QuestionType.Listening.value:
			question_all = [question.id for question in ListeningQuestion.objects.all()]
		elif question_type == QuestionType.Correction.value:
			question_all = [question.id for question in CorrectionQuestion.objects.all()]
		elif question_type == 3:
			question_all = [question.id for question in ReadingQuestion.objects.all()]

		if len(question_all) < count:
			print(len(question_all))
			raise GameException(ErrorType.InvalidQuestionDatabaseCount)
		else:
			random.seed(int(time.time()))
			question_list = random.sample(question_all, count)
		return question_list

	# 获取多个题目
	@classmethod
	def getQuestions(cls, ids=None, error: ErrorType = ErrorType.QuestionNotExist,
					 QuestionType = ListeningQuestion, **kwargs,) -> list:
		"""
		获取多个题目
		Args:
			ids (list): 题目ID集
			QuestionType (class): 题目类型
			error (ErrorType): 抛出异常
			**kwargs (**dict): 查询参数
		Returns:
			当 ids 不为 None 时，返回指定 ID 的题目
			否则只返回满足条件的题目
		"""
		if ids is None:
			questions = ViewUtils.getObjects(QuestionType, **kwargs)
			return questions

		unique_ids = list(set(ids))

		res = ViewUtils.getObjects(QuestionType, id__in=ids)

		# 数量不一致，说明获取出现问题
		if res.count() != len(unique_ids):
			raise GameException(error)

		return res

	# 获取指定列表的单词集
	@classmethod
	def getWords(cls, wids=None, error: ErrorType = ErrorType.WordNotExit, **kwargs) -> list:
		"""
		获取多个题目
		Args:
			wids (list): 单词ID集
			error (ErrorType): 抛出异常
		Returns:
			当 wids 不为 None 时，返回指定 ID 的单词
			否则只返回满足条件的题目
		"""
		if wids is None:
			words = ViewUtils.getObjects(Word, **kwargs)
			return words

		unique_ids = list(set(wids))

		res = ViewUtils.getObjects(Word, id__in=wids)

		# 数量不一致，说明获取出现问题
		if res.count() != len(unique_ids):
			raise GameException(error)

		return res

	# 获取单词记录
	@classmethod
	def getWordsRecords(cls, **kwargs) -> list:
		"""
		获取单词记录
		Returns:
			返回当前玩家的所有单词记录
		"""
		records = ViewUtils.getObjects(WordRecord, return_type='dict', **kwargs)
		return records
