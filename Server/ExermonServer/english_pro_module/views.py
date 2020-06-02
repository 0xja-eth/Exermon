from typing import Any

from .models import *
from player_module.models import Player
from utils.view_utils import Common as ViewUtils
from utils.calc_utils import NewWordsGenerator
from utils.exception import ErrorType, GameException

import time
import random
from random import choice


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

		# 检验数量是否合法
		Check.ensureQuestionCount(count)

		# 返回对应类型的题目ID集（里面已经进行了类型判断）
		qids = Common.generateQuestions(count, type)

		return {'qids': qids}

	# 查询英语特训题目
	@classmethod
	async def getQuestions(cls, consumer, player: Player, type: int, qids: list, ):
		# 返回数据：
		# questions: 英语特训题目数据（数组） => 获取的题目数据（听力/阅读/改错）
		# 检验类型是否在枚举类型里面

		# get 的时候同时也已经对类型进行了判断
		questions = Common.getQuestions(ids=qids, type_=type)
		questions = ModelUtils.objectsToDict(questions)

		# if type == QuestionType.Listening.value:
		# 	questions = Common.getQuestions(ids=qids, type_=ListeningQuestion)
		# elif type == QuestionType.Phrase.value:
		# 	questions = Common.getQuestions(ids=qids, type_=InfinitiveQuestion)
		# elif type == QuestionType.Correction.value:
		# 	questions = Common.getQuestions(ids=qids, type_=CorrectionQuestion)

		return {'questions': questions}

	# 生成当前轮单词
	@classmethod
	async def generateWords(cls, consumer, player: Player, ):
		# 返回数据：
		# words: 单词数据（数组） => 单词数据集

		pro_record: ExerProRecord = player.exerProRecrod()

		# 没有玩过的记录
		if not pro_record:
			pro_record = ExerProRecord.create(player)

		# 有玩过的记录
		else:
			Common.ensureFinishLastWords(pro_record)
			pro_record.upgrade()

		return pro_record.convertToDict("words")

	# 回答当前轮单词
	@classmethod
	async def answerWord(cls, consumer, player: Player, wid: int, chinese: str):
		# 返回数据：
		# correct: bool => 回答是否正确, new: bool => 是否进入下一轮, next: int => 下一个单词ID(可选）

		record = Common.getCurrentWordRecord(player, wid)
		correct = record.answer(chinese)

		pro_record = Common.getExerProRecord(player)
		next = pro_record.nextWord()

		if next is None:
			return {
				'new': True,
				'correct': correct
			}

		else:
			return {
				'new': False,
				'next': next.word_id,
				'correct': correct
			}

	# 查询当前轮单词
	@classmethod
	async def queryWords(cls, consumer, player: Player, ):
		# 返回数据：
		# level: int => 当前轮单词等级
		# sum: int => 当前轮总单词数
		# correct: int => 当前轮正确单词数
		# wrong: int => 当前轮错误单词数

		pro_record = Common.getExerProRecord(player)

		return pro_record.convertToDict("status")

		# exer_pro_record = ViewUtils.getObject(ExerProRecord, ErrorType.NoFirstCurrentWords, player=player)
		# wids = exer_pro_record.words
		# level = exer_pro_record.WordLevel
		# sum = len(exer_pro_record.words)
		# word_records = Common.getWordRecords(player, word_id__in=wids)
		# correct = 0
		# for word_record in word_records:
		# 	if word_record.current_correct:
		# 		correct += 1
		# wrong = sum - correct
		# return {
		# 	'level': level,
		# 	'sum': sum,
		# 	'correct': correct,
		# 	'wrong': wrong
		# }

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
		records = Common.getWordRecords(player)
		records = ModelUtils.objectsToDict(records)

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

	@classmethod
	def getQuestionClass(cls, type_: int):
		"""
		获取题目类型
		Args:
			type_ (int): 题目类型（枚举值）
		Raises:
			ErrorType.InvalidQuestionType: 题目类型不正确
		Returns:
			返回题目类型变量
		"""
		if type_ == QuestionType.Listening.value:
			return ListeningQuestion
		elif type_ == QuestionType.Phrase.value:
			return InfinitiveQuestion
		elif type_ == QuestionType.Correction.value:
			return CorrectionQuestion

		raise GameException(ErrorType.InvalidQuestionType)

	@classmethod
	def generateQuestions(cls, count: int, type_: int = None, cla: type = None, ):
		"""
		从数据库中随机选出题目
		Args:
			type_ (int): 题目类型（枚举值）
			cla (type): 题目类型
			count (int) 题目数量
		Returns:
			返回对应类型题目的ID集，若超过题库数量抛出设置好的异常
		"""
		if cla is None: cla = cls.getQuestionClass(type_)

		question_all = ViewUtils.getObjects(cla)
		question_all = [question.id for question in question_all]

		if len(question_all) < count:
			raise GameException(ErrorType.InvalidQuestionDatabaseCount)

		return random.sample(question_all, count)

	# 获取多个题目
	@classmethod
	def getQuestions(cls, ids=None, error: ErrorType = ErrorType.QuestionNotExist,
					 type_: int = None, cla: type = None, **kwargs) -> list:
		"""
		获取多个题目
		Args:
			ids (list): 题目ID集
			type_ (int): 题目类型（枚举值）
			cla (type): 题目类型
			error (ErrorType): 抛出异常
			**kwargs (**dict): 查询参数
		Returns:
			当 ids 不为 None 时，返回指定 ID 的题目
			否则只返回满足条件的题目
		"""
		if cla is None: cla = cls.getQuestionClass(type_)

		if ids is None:
			questions = ViewUtils.getObjects(cla, **kwargs)
			return questions

		unique_ids = list(set(ids))

		res = ViewUtils.getObjects(cla, id__in=ids)

		# 数量不一致，说明获取出现问题
		if res.count() != len(unique_ids):
			raise GameException(error)

		return res

	# 获取指定列表的单词集
	@classmethod
	def getWords(cls, wids=None, error: ErrorType = ErrorType.WordNotExit, **kwargs) -> list:
		"""
		获取多个单词
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

	@classmethod
	def getExerProRecord(cls, player: Player,
						 error: ErrorType = ErrorType.ExerProRecordNotExist) -> ExerProRecord:
		"""
		获取特训记录
		Args:
			player (Player): 玩家
			error (ErrorType): 抛出的错误类型
		Returns:
			返回玩家对应的特训记录
		"""
		pro_record: ExerProRecord = player.exerProRecrod()
		if pro_record is None: raise GameException(error)

		return pro_record

	@classmethod
	def getWordRecords(cls, player: Player) -> list:
		"""
		获取所有单词记录
		Returns:
			返回当前玩家的所有单词记录
		"""
		return cls.getExerProRecord(player).wordRecords()

	@classmethod
	def getWordRecord(cls, player: Player, wid,
					  error: ErrorType = ErrorType.WordRecordNotExit,
					  **kwargs) -> WordRecord:
		"""
		获取单个单词记录
		Args:
			player (Player): 玩家
			wid (int): 单词ID
			error (ErrorType): 异常类型
			**kwargs (**dict): 查询参数
		Returns:
			返回当前玩家的指定单词的单词记录
		"""
		pro_record = cls.getExerProRecord(player)

		record = pro_record.wordRecord(wid, **kwargs)
		if record is None: raise GameException(error)

		return record

	@classmethod
	def getCurrentWordRecord(cls, player: Player, wid,
							 error: ErrorType = ErrorType.NoInCurrentWords) -> WordRecord:
		"""
		获取单个当前单词记录
		Args:
			player (Player): 玩家
			wid (int): 单词ID
			error (ErrorType): 异常类型
		Returns:
			返回当前玩家的指定单词的当前单词记录
		"""
		pro_record = cls.getExerProRecord(player)

		record = pro_record.wordRecord(wid, current=True)
		if record is None: raise GameException(error)

		return record

	# # 获取新单词集
	# @classmethod
	# def getNewWords(cls, old_words: list, player: Player, error: ErrorType = ErrorType.NoEnoughNewWord):
	# 	"""
	# 	Args:
	# 		player (Player): 用户
	# 		old_words (list): 旧单词 ID 列表
	# 		error (ErrorType): 异常
	# 	Returns:
	# 		如果题库有足够的新单词就返回 单词ID集 和 单词集，否则报错
	# 	"""
	# 	words = ViewUtils.getObjects(Word, player=player).exclude(id__in=old_words)
	# 	words_list = [word.id for word in words if cls.getNewWord(word, player) is not None]
	# 	if len(words_list) < NewWordsGenerator.WordNum * NewWordsGenerator.WordNum:
	# 		raise error
	#
	# 	random.seed(int(time.time()))
	# 	new_wids = random.sample(words_list, NewWordsGenerator.WordNum * NewWordsGenerator.NewWordPercent)
	# 	new_words = ViewUtils.getObjects(Word, player=player, id__in=new_wids)
	#
	# 	return new_wids, new_words

	# # 获取新单词
	# @classmethod
	# def getNewWord(cls, word: Word, player: Player):
	# 	"""
	# 	Args:
	# 		player (Player): 用户
	# 		word (Word): 单词
	# 	Returns:
	# 		如果该单词是新单词，则返回，否则返回None
	# 	"""
	# 	word_record = WordRecord.objects.filter(word=word, player=player)
	#
	# 	# 先判断有无单词记录，无就是新单词
	# 	if not word_record:
	# 		return word
	# 	else:
	# 		# 再判断是否有答对过
	# 		record = word_record.filter(correct=0)
	# 		if not record:
	# 			return word
	# 		return None

	# 检验答词ID是否在当前轮中
	@classmethod
	def ensureWordInCurrentWords(cls, wid: int, player: Player, words: list):
		"""
		保证指定单词为当前轮单词
		Args:
			player (Player): 用户
			wid (int): 单词
			words (list)：单词ID集
		Returns:
			如果单词不在当前轮中则报错
		"""
		pro_record = cls.getExerProRecord(player)

		record = pro_record.wordRecord(wid, current=True)
		if record is None: raise GameException(ErrorType.NoInCurrentWords)

	@classmethod
	def ensureWordNotCorrect(cls, word_rec: WordRecord):
		if word_rec.current_correct: raise GameException()

	# 判断单词是否回答正确
	# @classmethod
	# def isAnswerCorrect(cls, wid: int, player: Player, chinese: str, isUpdate: bool) -> bool:
	# 	"""
	# 	Args:
	# 		player (Player): 用户
	# 		wid (int): 单词
	# 		chinese (str): 中文
	# 		isUpdate  (bool): 是否更新单词记录
	# 	Returns:
	# 		isUpdate = True时，更新单词记录并返回该单词是否正确
	# 		isUpdate = False时，返回值为False表明该单词回答错误或者未回答，返回值为True表明该单词已经回答正确了
	# 	"""
	# 	# 将回答结果记录到 WordRecord 表中
	# 	record = player.wordRecord(wid)
	# 	if record is not None:
	# 		word = ViewUtils.getObject(Word, ErrorType.WordNotExit, player=player, id=wid)
	# 		if isUpdate:
	# 			if word.chinese == chinese:
	# 				record.updateRecord(True)
	# 				return True
	# 			else:
	# 				record.updateRecord(False)
	# 				return False
	# 		else:
	# 			if not record.current:
	# 				raise ErrorType.NoInCurrentWords
	# 			elif record.current and record.correct == 0:
	# 				return False
	# 			else:
	# 				return True
	# 	else:
	# 		raise ErrorType.NoInCurrentWords

	@classmethod
	def ensureFinishLastWords(cls, pro_record: ExerProRecord,
							  error: ErrorType = ErrorType.AnswerNotFinish):
		"""
		确保当前轮单词全部通过
		Args:
			pro_record (ExerProRecord): 特训记录
			error (ErrorType): 异常
		"""
		if not pro_record.isFinished(): raise GameException(error)

