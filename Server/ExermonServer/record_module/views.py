from .models import *

from player_module.models import Player

from question_module.views import Common as QuestionCommon

from utils.view_utils import Common as ViewUtils
from utils.model_utils import EnumMapper
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

	# 开始刷题
	@classmethod
	async def generate(cls, consumer, player: Player, type: int,
					   sid: int = None, count: int = None, gen_type: int = None):
		# 返回数据：
		# record: 刷题记录数据 => 刷题记录
		from utils.interface_manager import Common as InterfaceCommon

		cla = Common.getQuesSetClass(type)

		kwargs = {}

		if BaseExerciseRecord in cla.mro():

			from game_module.models import Subject
			from player_module.views import Common as PlayerCommon

			sid = InterfaceCommon.convertDataType(sid, 'int')
			count = InterfaceCommon.convertDataType(count, 'int')

			subject = Subject.get(id=sid)

			PlayerCommon.ensureSubjectSelected(player, subject)

			kwargs['subject'] = subject
			kwargs['count'] = count

			if cla == GeneralExerciseRecord:
				gen_type = InterfaceCommon.convertDataType(gen_type, 'int')

				kwargs['gen_type'] = gen_type

		ques_set: QuesSetRecord = cla.create(player, **kwargs)

		return {'record': ques_set.convert()}

	# 开始刷题
	@classmethod
	async def startQuestion(cls, consumer, player: Player,
							q_type: int, qid: int):
		# 返回数据：无
		cla = QuestionCommon.getQuestionClass(q_type)

		ques_set: QuesSetRecord = player.currentQuesSet()
		ques_set.startQuestion(cla, qid)

	# 作答刷题题目
	@classmethod
	async def answerQuestion(cls, consumer, player: Player,
							 q_type: int, qid: int, answer: dict,
							 timespan: int, terminate: bool):
		# 返回数据：
		# result: 刷题结果数据 => 刷题结果（可选）
		cla = QuestionCommon.getQuestionClass(q_type)

		ques_set: QuesSetRecord = player.currentQuesSet()
		ques_set.answerQuestion(answer, timespan, cla, qid)

		if terminate:
			ques_set.terminate()

			return {'result': ques_set.convert('result')}


# =======================
# 记录校验类，封装记录业务数据格式校验的函数
# =======================
class Check:

	# 校验题目类型
	@classmethod
	def ensureQuesSetType(cls, val: int):
		if val == 0:
			raise GameException(ErrorType.IncorrectQuesSetType)
		ViewUtils.ensureEnumData(val, QuesSetType,
								 ErrorType.IncorrectQuesSetType, True)


# =======================
# 记录公用类，封装关于物品模块的公用函数
# =======================
class Common:

	@classmethod
	def getQuesSetClass(cls, type_: int):
		"""
		获取题目集类
		Args:
			type_ (int): 类型（枚举值）
		Returns:
			返回相应类型的题目集类
		"""
		Check.ensureQuesSetType(type_)
		return EnumMapper.get(QuesSetType(type_))

	# 获取刷题记录
	@classmethod
	def getExerciseRecord(cls, error: ErrorType = ErrorType.ExerciseRecordNotExist,
						  **kwargs) -> GeneralExerciseRecord:

		return ViewUtils.getObject(GeneralExerciseRecord, error, **kwargs)

	# 获取刷题题目
	@classmethod
	def getExerciseQuestion(cls, error: ErrorType = ErrorType.ExerciseQuestionNotExist,
							**kwargs) -> GeneralExerciseQuestion:

		return ViewUtils.getObject(GeneralExerciseQuestion, error, **kwargs)

	# 确保题目集记录所属玩家
	@classmethod
	def ensureQuesSetPlayer(cls, ques_set_rec: QuesSetRecord, player):
		if ques_set_rec.player_id != player.id:
			raise GameException(ErrorType.ExerciseRecordNotExist)
