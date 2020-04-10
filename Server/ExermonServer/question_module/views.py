from django.shortcuts import render
from .models import *
from player_module.models import Player
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

		QuesReport.create(player, qid, type, description)

	# 上传题目
	@classmethod
	def upload(cls, auth: str):
		# 返回数据：无
		ViewUtils.ensureAuth(auth[0])

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

		question = Question()
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
		choice = QuesChoice()
		choice.order = index
		choice.text = c['text']
		choice.answer = c['ans']
		choice.question = question
		choice.save()

	@classmethod
	def __uploadPicture(cls, p, index, question):
		picture = QuesPicture()
		picture.number = index
		picture.file = p
		picture.question = question
		picture.save()

# ====================
# 题目校验类，封装管理题目模块的参数格式是否正确-lgy
# ====================
class Check:

	# 校验题目反馈的长度-lgy
	@classmethod
	def ensureFeedbackFormat(cls, val: str):
		if len(val) > QuesReport.MAX_DESC_LEN:
			raise GameException(ErrorType.QuesReportTooLong)

	# 校验题目反馈的长度-lgy
	@classmethod
	def ensureQuestionTypeExist(cls, val: int):
		ViewUtils.ensureEnumData(val, QuesReportType, ErrorType.InvalidFeedbackType)


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
	def getQuestions(cls, ids=None, error: ErrorType = ErrorType.QuestionNotExist,
					 **kwargs) -> list:
		"""
		获取多个题目
		Args:
			ids (list): 题目ID集
			error (ErrorType): 抛出异常
			**kwargs (**dict): 查询参数
		Returns:
			当 ids 不为 None 时，返回指定 ID 的题目
			否则只返回满足条件的题目
		"""
		if ids is None:
			return ViewUtils.getObjects(Question, **kwargs)

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

	# 确保反馈类型是枚举类型-lgy
	@classmethod
	def ensureQuesReportExist(cls, val:int):
		return ViewUtils.ensureEnumData(val, QuesReportType, ErrorType.InvalidQuesReportType )
