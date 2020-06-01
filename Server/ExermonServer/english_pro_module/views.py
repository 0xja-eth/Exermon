from typing import Any

from .models import *
from player_module.models import Player
from utils.view_utils import Common as ViewUtils
from utils.calc_utils import CurrentWordsCalc
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

		if type == QuestionType.Listening.value:
			questions = Common.getQuestions(ids=qids, QuestionType=ListeningQuestion)
		elif type == QuestionType.Infinitive.value:
			questions = Common.getQuestions(ids=qids, QuestionType=InfinitiveQuestion)
		elif type == QuestionType.Correction.value:
			questions = Common.getQuestions(ids=qids, QuestionType=CorrectionQuestion)

		questions = ModelUtils.objectsToDict(questions)
		return {'questions': questions}

	# 生成当前轮单词
	@classmethod
	async def generateWords(cls, consumer, player: Player, ):
		# 返回数据：
		# words: 单词数据（数组） => 单词数据集
		Common.ensureFinishLastWords(player)
		words = Common.generateWords(player)
		words = ModelUtils.objectsToDict(words)

		return {'words': words}

	# 回答当前轮单词
	@classmethod
	async def answerWord(cls, consumer, player: Player, wid: int, chinese: str):
		# 返回数据：
		# correct: bool => 回答是否正确, new: bool => 是否进入下一轮, next: int => 下一个单词ID(可选）
		exer_pro_record = ViewUtils.getObject(ExerProRecord, ErrorType.NoFirstCurrentWords, player=player)
		words = exer_pro_record.words
		Common.ensureWordInCurrentWords(wid, player, words)

		# 判断该单词是否回答正确
		correct = Common.isAnswerCorrect(wid, player, chinese, True)

		# 判断是否该结束该轮单词回答
		words_query_set = Common.getWords(words, player=player)
		left_words = [word for word in words_query_set if not Common.isAnswerCorrect(word.id, player, word.chinese, False)]
		if len(left_words) == 0:
			exer_pro_record.update()
			return {'new': True, 'correct': correct}
		else:
			return {
				'next': choice(left_words).id,
				'new': False,
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
		exer_pro_record = ViewUtils.getObject(ExerProRecord, ErrorType.NoFirstCurrentWords, player=player)
		wids = exer_pro_record.words
		level = exer_pro_record.WordLevel
		sum = len(exer_pro_record.words)
		word_records = Common.getWordsRecords(player, word_id__in=wids)
		correct = 0
		for word_record in word_records:
			if word_record.current_correct:
				correct += 1
		wrong = sum - correct
		return {
			'level': level,
			'sum': sum,
			'correct': correct,
			'wrong': wrong
		}

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
		records = Common.getWordsRecords(player)
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
			question_all = ViewUtils.getObjects(ListeningQuestion)
		elif question_type == QuestionType.Correction.value:
			question_all = ViewUtils.getObjects(CorrectionQuestion)
		elif question_type == QuestionType.Infinitive.value:
			question_all = ViewUtils.getObjects(InfinitiveQuestion)

		question_all = [question.id for question in question_all]

		if len(question_all) < count:
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
	def getWordsRecords(cls, player: Player, **kwargs) -> list:
		"""
		获取单词记录
		Returns:
			返回当前玩家的所有单词记录
		"""
		records = ViewUtils.getObjects(WordRecord, player=player, **kwargs)
		return records

	# 生成当前轮单词
	@classmethod
	def generateWords(cls, player: Player,):
		"""
		Args:
			player (Player): 用户
			error (ErrorType): 抛出异常
		获取单词记录
		Returns:
			如果玩家有玩过的记录，则根据上一轮单词生成当前轮单词，否则随机生成当前轮单词并返回单词集
		"""
		ProRecord = ExerProRecord.objects.filter(player=player).first()
		# 没有玩过的记录
		if not ProRecord:
			words = ViewUtils.getObjects(Word)
			full_word_list = [word.id for word in words]
			wids = CurrentWordsCalc.generateNewWords([], full_word_list)

			words = ViewUtils.getObjects(Word, id__in=wids, player=player)

			# 创建轮数记录
			ExerProRecord.create(player, wids)

		# 有玩过的记录
		else:
			old_words = ProRecord.words
			# 将旧单词记录中的 current 字段置为 False
			cls.makeCurrentFalse(old_words, player)
			# 获取旧单词
			random.seed(int(time.time()))
			old_wids = random.sample(old_words, CurrentWordsCalc.WordNum * (1 - CurrentWordsCalc.NewWordPercent))
			old_words_set = ViewUtils.getObjects(Word, player=player, id__in=old_wids)
			# 获取新单词
			new_wids, new_words_set = cls.getNewWords(old_words, player)
			words = old_words_set | new_words_set

			wids = old_wids.extend(new_wids)
			ProRecord.update(wids)

		# 为生成的每个word建立wordRecord
		for wid in wids:
			WordRecord.create(player, wid)

		return words

	# 获取新单词集
	@classmethod
	def getNewWords(cls, old_words: list, player: Player, error: ErrorType = ErrorType.NoEnoughNewWord):
		"""
		Args:
			player (Player): 用户
			old_words (list): 旧单词 ID 列表
			error (ErrorType): 异常
		Returns:
			如果题库有足够的新单词就返回 单词ID集 和 单词集，否则报错
		"""
		words = ViewUtils.getObjects(Word, player=player).exclude(id__in=old_words)
		words_list = [word.id for word in words if cls.getNewWord(word, player) is not None]
		if len(words_list) < CurrentWordsCalc.WordNum * CurrentWordsCalc.WordNum:
			raise error

		random.seed(int(time.time()))
		new_wids = random.sample(words_list, CurrentWordsCalc.WordNum * CurrentWordsCalc.NewWordPercent)
		new_words = ViewUtils.getObjects(Word, player=player, id__in=new_wids)

		return new_wids, new_words

	# 获取新单词
	@classmethod
	def getNewWord(cls, word: Word, player: Player):
		"""
		Args:
			player (Player): 用户
			word (Word): 单词
		Returns:
			如果该单词是新单词，则返回，否则返回None
		"""
		word_record = WordRecord.objects.filter(word=word, player=player)

		# 先判断有无单词记录，无就是新单词
		if not word_record:
			return word
		else:
			# 再判断是否有答对过
			record = word_record.filter(correct=0)
			if not record:
				return word
			return None

	# 检验答词ID是否在当前轮中
	@classmethod
	def ensureWordInCurrentWords(cls, wid: int, player: Player, words: list):
		"""
		Args:
			player (Player): 用户
			wid (int): 单词
			words (list)：单词ID集
		Returns:
			如果单词不在当前轮中则报错
		"""
		if wid not in words:
			raise ErrorType.NoInCurrentWords

		record = ViewUtils.getObject(WordRecord, ErrorType.NoInCurrentWords, player=player, word_id=wid)

		current = record.current
		if not current:
			raise GameException(ErrorType.NoInCurrentWords)

	# 判断单词是否回答正确
	@classmethod
	def isAnswerCorrect(cls, wid: int, player: Player, chinese: str, isUpdate: bool) -> bool:
		"""
		Args:
			player (Player): 用户
			wid (int): 单词
			chinese (str): 中文
			isUpdate  (bool): 是否更新单词记录
		Returns:
			isUpdate = True时，更新单词记录并返回该单词是否正确
			isUpdate = False时，返回值为False表明该单词回答错误或者未回答，返回值为True表明该单词已经回答正确了
		"""
		# 将回答结果记录到 WordRecord 表中
		record = player.wordRecord(wid)
		if record is not None:
			word = ViewUtils.getObject(Word, ErrorType.WordNotExit, player=player, id=wid)
			if isUpdate:
				if word.chinese == chinese:
					record.updateRecord(True)
					return True
				else:
					record.updateRecord(False)
					return False
			else:
				if not record.current:
					raise ErrorType.NoInCurrentWords
				elif record.current and record.correct == 0:
					return False
				else:
					return True
		else:
			raise ErrorType.NoInCurrentWords

	# 将上一轮单词的 current 字段置为False
	@classmethod
	def makeCurrentFalse(cls, wids, player):
		"""
		Args:
			player (Player): 用户
			wid (int): 单词
		Returns:
			将上一轮单词的 current 字段置为False，无返回
		"""
		word_record_sets = cls.getWordsRecords(player, word_id__in=wids)
		for word_record in word_record_sets:
			word_record.current = False
			word_record.save()

	# 检测上一轮是否
	@classmethod
	def ensureFinishLastWords(cls, player: Player, error: ErrorType = ErrorType.NoFirstCurrentWords):
		"""
		Args:
			player (Player): 用户
			error (ErrorType): 异常
		Returns:
			无返回
		"""
		exer_pro_record = ViewUtils.getObject(ExerProRecord, ErrorType.NoFirstCurrentWords, player=player)
		if not exer_pro_record.finished:
			raise error

