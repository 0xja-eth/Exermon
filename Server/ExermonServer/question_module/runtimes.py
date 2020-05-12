from question_module.models import Question
from record_module.models import QuestionRecord
import datetime
from utils.model_utils import Common as ModelUtils


# =======================================
# 生成某个题目记录的各项统计资料-lgy5.1
# =======================================
class QuestionDetail:

	# 刷新最短间隔
	MIN_DELTA = datetime.timedelta(0, 60*30)

	# 更新时间（分钟数）
	UPDATE_MINUTES = [0, 30]

	# 记录字典
	RecordDict: dict = {}

	update_time: datetime.datetime = None

	sum_player = 0
	sum_count = 0
	sum_collect = 0
	all_avg_time = 0
	all_corr_rate = 0

	@classmethod
	def getData(cls, question_id, type=None, player=None):
		"""
		获得该题目的做题信息
		Args:
			question_id (int): 题目ID
			type (str): 获取数据类型
			player (Player): 玩家
		Returns:
			返回指定玩家的题目详情
		"""
		if question_id in cls.RecordDict:
			detail = cls.RecordDict[question_id]
			if detail.isUpdateRequired(): detail.update()

		else:
			detail = cls.RecordDict[question_id] = QuestionDetail(question_id)

		return detail.convertToDict(type, player)

	def __init__(self, question_id):
		self.question_id = question_id
		self.update()

	def update(self):
		"""
		更新当前题目详情
		"""
		from utils.view_utils import Common as ViewUtils

		# TODO: 使用 ViewUtils.getObjects 来代替 QuestionRecord.object.filter(...)
		# TODO: 至于为什么，可以看看 ViewUtils.getObjects 的实现，其他函数也同理
		records = ViewUtils.getObjects(QuestionRecord,
									   question_id=self.question_id)

		self.sum_player = len(records)
		self.sum_count = sum(r.count for r in records)
		self.sum_collect = len(list(r for r in records if r.collected))

		if self.sum_player == 0:
			self.all_corr_rate = self.all_avg_time = 0
		else:
			self.all_corr_rate = sum(r.corrRate() for r in records) / self.sum_player
			self.all_avg_time = int(sum(r.avg_time for r in records) / self.sum_player)

		self.update_time = datetime.datetime.now()

		# dict[self.ques_id] = self.convertToDict(self.ques_id, type="overall")

	def isUpdateRequired(self):
		"""
		是否需要更新
		Returns:
			返回详情是否需要更新
		"""
		now = datetime.datetime.now()
		delta = now - self.update_time

		# 当更新时间间隔大于指定值，表示需要更新
		return delta >= self.MIN_DELTA

	def convertToDict(self, type=None, player=None):
		"""
		转化为字典
		Args:
			type (str): 类型
			player (Player): 玩家
		Returns:
			返回转化后的字典
		"""
		update_time = ModelUtils.timeToStr(self.update_time)

		# 全服
		if type == "overall" or player is None:
			return {
				'id': self.question_id,
				'sum_player': self.sum_player,
				'sum_count': self.sum_count,
				'sum_collect': self.sum_collect,
				'all_corr_rate': self.all_corr_rate,
				'all_avg_time': self.all_avg_time,
				'update_time': update_time
			}

		# 网上看的，使用Q方法同时匹配多个关键字
		# TODO: 不需要用这个方法，QuestionRecord 是和 Player 有外键关系的
		# TODO: 可以直接用 player.questionrecord_set.filter(...)
		# TODO: 实际上，在 Player 中已经实现了这个功能，
		# TODO: 详情查看 Player.questionRecords() 和 Player.questionRecord() 函数
		# TODO: 另外，以后要有类似的功能，也需要封装成一个函数来调用
		# TODO: 除非是复杂的查询操作，否则一般不要直接使用 Django 的 API
		# record = QuestionRecord.objects.filter(Q(question=self.ques_id) | Q(player=player))
		record: QuestionRecord = player.questionRecord(self.question_id)

		if record is not None:
			first_date = ModelUtils.timeToStr(record.first_date)
			count = record.count
			corr_rate = record.corrRate()
			first_time = record.first_time
			avg_time = record.avg_time
		else:
			first_date = None
			count = corr_rate = first_time = avg_time = 0

		# 个人
		if type == "personal":
			return {
				'id': self.question_id,
				'count': count,
				'corr_rate': corr_rate,
				'first_time': first_time,
				'avg_time': avg_time,
				'first_date': first_date,
			}

		# 详情（默认）
		return {
			'id': self.question_id,
			'sum_player': self.sum_player,
			'sum_collect': self.sum_collect,
			'sum_count': self.sum_count,
			'all_corr_rate': self.all_corr_rate,
			'all_avg_time': self.all_avg_time,

			'count': count,
			'corr_rate': corr_rate,
			'first_time': first_time,
			'avg_time': avg_time,
			'first_date': first_date,

			'update_time': update_time
		}
