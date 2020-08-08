from .models import *
from player_module.models import Player
from question_module.views import Common as QuestionCommon
from utils.view_utils import Common as ViewUtils
from utils.exception import ErrorType, GameException

# Create your views here.


# =======================
# 记录服务类，封装管理记录模块的业务处理函数
# =======================
class Service:
	# 查询记录
	@classmethod
	async def get(cls, consumer, player: Player, ):
		# 返回数据：
		# question_records: 题目记录数据（数组） => 所有题目记录
		# exercise_records: 刷题结果数据（数组） => 所有刷题记录
		# exam_records: 考核记录数据（数组） => 所有考核记录
		# battle_records: 对战记录数据（数组） => 所有对战记录
		return player.convert("records")

	# 收藏/解除收藏题目
	@classmethod
	async def collect(cls, consumer, player: Player, qid: int):
		# 返回数据：
		# collected: bool => 是否收藏（处理后）

		QuestionCommon.ensureQuestionExist(id=qid)

		rec = GeneralQuesRecord.create(player, qid)

		rec.collected = not rec.collected
		rec.save()

		return {'collected': rec.collected}

	# 解除错题
	@classmethod
	async def unwrong(cls, consumer, player: Player, qid: int):
		# 返回数据：无

		QuestionCommon.ensureQuestionExist(id=qid)

		rec = GeneralQuesRecord.create(player, qid)

		rec.wrong = False
		rec.save()

	# 添加备注
	@classmethod
	async def note(cls, consumer, player: Player, qid: int, note: str):
		# 返回数据：无

		Check.ensureNoteFormat(note)

		QuestionCommon.ensureQuestionExist(id=qid)

		rec = GeneralQuesRecord.create(player, qid)

		rec.note = note
		rec.save()

	# 开始刷题
	@classmethod
	async def exerciseGenerate(cls, consumer, player: Player, sid: int, gen_type: int, count: int):
		# 返回数据：
		# record: 刷题记录数据 => 刷题记录
		from game_module.models import Subject
		from player_module.views import Common as PlayerCommon

		subject = Subject.get(id=sid)

		PlayerCommon.ensureSubjectSelected(player, subject)

		rec = ExerciseRecord.create(player, subject=subject,
									gen_type=gen_type, count=count)

		return {'record': rec.convert()}

	# 开始刷题
	@classmethod
	async def exerciseStart(cls, consumer, player: Player, q_type: int, qid: int):
		# 返回数据：无

		exercise = player.currentQuestionSet()
		exercise.startQuestion(qid)

	# 作答刷题题目
	@classmethod
	async def exerciseAnswer(cls, consumer, player: Player, qid: int,
							 selection: list, timespan: int, terminate: bool):
		# 返回数据：
		# result: 刷题结果数据 => 刷题结果（可选）

		exercise = player.currentQuestionSet()
		exercise.answerQuestion(selection, timespan, question_id=qid)

		if terminate:
			exercise.terminate()

			return {'result': exercise.convert('result')}


# =======================
# 记录校验类，封装记录业务数据格式校验的函数
# =======================
class Check:

	# 校验备注格式
	@classmethod
	def ensureNoteFormat(cls, val: str):
		if len(val) != GeneralQuesRecord.MAX_NOTE_LEN:
			raise GameException(ErrorType.InvalidNote)


# =======================
# 记录公用类，封装关于物品模块的公用函数
# =======================
class Common:

	# 获取题目记录
	@classmethod
	def getQuestionRecord(cls, error: ErrorType = ErrorType.QuestionNotExist,
						  **kwargs) -> GeneralQuesRecord:

		return ViewUtils.getObject(GeneralQuesRecord, error, **kwargs)

	# 获取刷题记录
	@classmethod
	def getExerciseRecord(cls, error: ErrorType = ErrorType.ExerciseRecordNotExist,
						  **kwargs) -> ExerciseRecord:

		return ViewUtils.getObject(ExerciseRecord, error, **kwargs)

	# 获取刷题题目
	@classmethod
	def getExerciseQuestion(cls, error: ErrorType = ErrorType.ExerciseQuestionNotExist,
							**kwargs) -> ExerciseQuestion:

		return ViewUtils.getObject(ExerciseQuestion, error, **kwargs)

	# 确保题目集记录所属玩家
	@classmethod
	def ensureQuestionSetPlayer(cls, ques_set_rec: QuestionSetRecord, player):
		if ques_set_rec.player != player:
			raise GameException(ErrorType.ExerciseRecordNotExist)
