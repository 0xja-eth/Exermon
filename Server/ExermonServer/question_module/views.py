from django.shortcuts import render
from .models import *

from player_module.models import Player

from utils.view_utils import Common as ViewUtils
from utils.model_utils import EnumMapper
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

	# 查询题目详情
	@classmethod
	async def getDetail(cls, consumer, player: Player, qid: int, ):
		# 返回数据：
		# detail: 题目详情数据 => 题目详情数据
		from .runtimes import QuestionDetail

		return {'detail': QuestionDetail.getData(qid, player=player)}

	# 查询玩家题目反馈-lgy
	@classmethod
	async def getReports(cls, consumer, player: Player):

		reports = player.questionrecord_set.all()

		return {"reports": ModelUtils.objectsToDict(reports)}

	# 提交题目反馈-lgy
	@classmethod
	async def pushReport(cls, consumer, player: Player, qid: int, type: int, description: str ):

		# 检验题目是否存在
		Common.ensureQuestionExist(qid)

		# 检验类型是否在枚举类型里面
		Check.ensureQuestionTypeExist(type)

		# 检验描述是否超过字数限制
		Check.ensureFeedbackFormat(description)

		BaseQuesReport.create(player, qid, type, description)

	# 上传题目
	@classmethod
	def upload(cls, auth: str):
		# 返回数据：无
		ViewUtils.ensureAuth(auth)

		from .raw_questions.upload import doPreprocess

		questions = doPreprocess()

		cnt = len(questions)
		ques_index = 1

		for q in questions:
			print('saving: %d/%d' % (ques_index, cnt))
			ques_index += 1
			cls._upload(q)

	@classmethod
	def _upload(cls, q):
		print(q)

		question = GeneralQuestion()
		question.title = q['title']
		question.score = q['score']
		question.star_id = q['star'] + 1
		question.level = q['level']
		question.subject_id = q['subjectId'] + 1
		question.type = q['type'].value
		question.save()

		index = 1
		for c in q['choices']:
			cls.__uploadChoice(c, index, question)
			index += 1

		index = 1
		for p in q['pictures']:
			cls.__uploadPicture(p, index, question)
			index += 1

	@classmethod
	def __uploadChoice(cls, c, index, question):
		choice = GeneralQuesChoice()
		choice.order = index
		choice.text = c['text']
		choice.answer = c['ans']
		choice.question = question
		choice.save()

	@classmethod
	def __uploadPicture(cls, p, index, question):
		picture = GeneralQuesPicture()
		picture.number = index
		picture.file = p
		picture.question = question
		picture.save()


# ====================
# 题目校验类，封装管理题目模块的参数格式是否正确-lgy
# ====================
class Check:

	# 校验题目类型
	@classmethod
	def ensureQuestionType(cls, val: int):
		if val == 0:
			raise GameException(ErrorType.IncorrectQuestionType)
		ViewUtils.ensureEnumData(val, QuestionType, ErrorType.IncorrectQuestionType, True)

	# 校验题目反馈的长度-lgy
	@classmethod
	def ensureFeedbackFormat(cls, val: str):
		if len(val) > BaseQuesReport.MAX_DESC_LEN:
			raise GameException(ErrorType.QuesReportTooLong)

	# 校验题目反馈的长度-lgy
	@classmethod
	def ensureQuestionTypeExist(cls, val: int):
		ViewUtils.ensureEnumData(val, QuesReportType, ErrorType.InvalidQuesReportType)


# =======================
# 题目公用类，封装关于物品模块的公用函数
# =======================
class Common:

	# 获取题目
	@classmethod
	def getQuestion(cls, type_: int = None, cla: BaseQuestion = None,
					error: ErrorType = ErrorType.QuestionNotExist,
					**kwargs) -> GeneralQuestion:
		"""
		获取题目
		Args:
			type_ (int): 类型（枚举值）
			cla (type): 类型（类）
			error (ErrorType): 不存在时抛出异常类型
			**kwargs (**dict): 查询参数
		Returns:
			返回符合条件的题目（若有多个返回第一个）
		"""
		if cla is None:
			Check.ensureQuestionType(type_)
			cla = EnumMapper.get(QuestionType(type_))

		return ViewUtils.getObject(cla, error, **kwargs)

	# 获取多个题目
	@classmethod
	def getQuestions(cls, type_: int = None, cla: BaseQuestion = None,
					 ids=None, error: ErrorType = ErrorType.QuestionNotExist,
					 **kwargs) -> list:
		"""
		获取多个题目
		Args:
			type_ (int): 类型（枚举值）
			cla (type): 类型（类）
			ids (list): 题目ID集
			error (ErrorType): 抛出异常
			**kwargs (**dict): 查询参数
		Returns:
			当 ids 不为 None 时，返回指定 ID 的题目
			否则只返回满足条件的题目
		"""
		if cla is None:
			Check.ensureQuestionType(type_)
			cla = EnumMapper.get(QuestionType(type_))

		if ids is None:
			return ViewUtils.getObjects(cla, **kwargs)

		unique_ids = list(set(ids))

		res = ViewUtils.getObjects(cla, id__in=ids)

		# 数量不一致，说明获取出现问题
		if res.count() != len(unique_ids): raise GameException(error)

		return res

	# 确保题目存在
	@classmethod
	def ensureQuestionExist(cls, type_: int = None, cla: BaseQuestion = None,
							error: ErrorType = ErrorType.QuestionNotExist, **kwargs):
		if cla is None:
			Check.ensureQuestionType(type_)
			cla = EnumMapper.get(QuestionType(type_))

		ViewUtils.ensureObjectExist(cla, error, **kwargs)

	# 确保反馈类型是枚举类型-lgy
	# @classmethod
	# def ensureQuesReportExist(cls, val: int):
	# 	return ViewUtils.ensureEnumData(val, QuesReportType, ErrorType.InvalidQuesReportType )
