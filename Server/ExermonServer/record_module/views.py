from .models import *
from player_module.models import Player
from utils.view_utils import Common as ViewUtils
from utils.exception import ErrorType, ErrorException

# Create your views here.


# =======================
# 记录服务类，封装管理记录模块的业务处理函数
# =======================
class Service:
	# 查询记录
	@classmethod
	async def get(cls, consumer, player: Player):
		# 返回数据：
		# question_records: 题目记录数据（数组） => 所有题目记录
		pass

	# 收藏/解除收藏题目
	@classmethod
	async def collect(cls, consumer, player: Player, qid: int):
		# 返回数据：
		# collected: bool => 是否收藏（处理后）
		pass

	# 解除错题
	@classmethod
	async def unwrong(cls, consumer, player: Player, qid: int):
		# 返回数据：
		pass

	# 添加备注
	@classmethod
	async def note(cls, consumer, player: Player, qid: int, note: str):
		# 返回数据：
		pass

	# 开始刷题
	@classmethod
	async def exerciseStart(cls, consumer, player: Player, sid: int, dtb_type: int, count: int):
		# 返回数据：
		# record: 刷题记录数据 => 刷题记录
		pass

	# 作答刷题题目
	@classmethod
	async def exerciseAnswer(cls, consumer, player: Player, eid: int, eqid: int, selection: list, timespan: int, terminate: bool):
		# 返回数据：
		# result: 刷题结果数据 => 刷题结果（可选）
		pass


# =======================
# 记录公用类，封装关于物品模块的公用函数
# =======================
class Common:

	# 获取题目记录
	@classmethod
	def getQuestionRecord(cls, return_type='object', error: ErrorType = ErrorType.QuestionNotExist, **kwargs) -> BaseItem:

		return ViewUtils.getObject(QuestionRecord, error, return_type=return_type, **kwargs)

	# 获取刷题记录
	@classmethod
	def getExerciseRecord(cls, return_type='object', error: ErrorType = ErrorType.QuesSugarNotExist, **kwargs) -> BaseItem:

		return ViewUtils.getObject(ExerciseRecord, error, return_type=return_type, **kwargs)
