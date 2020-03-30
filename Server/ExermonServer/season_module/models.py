from django.db import models
from django.db.models.query import QuerySet

from utils.exception import ErrorType
from utils.model_utils import Common as ModelUtils
from game_module.models import GroupConfigure
import datetime

# Create your models here.


# =======================
# 赛季记录表
# =======================
class SeasonRecord(models.Model):

	class Meta:
		verbose_name = verbose_name_plural = "赛季记录"

	SUSPEN_SCORE = 50

	# 玩家
	player = models.ForeignKey('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 赛季
	season = models.ForeignKey('CompSeason', on_delete=models.CASCADE, verbose_name="赛季")

	# 赛季积分
	point = models.PositiveSmallIntegerField(default=0, verbose_name="赛季积分")

	# 段位星星（初始有一个，不可小于0）
	star_num = models.PositiveSmallIntegerField(default=1, verbose_name="段位星星")

	@classmethod
	def create(cls, player: 'Player', season: 'CompSeason') -> 'SeasonRecord':
		"""
		创建记录
		Args:
			player (Player): 玩家
			season (CompSeason): 赛季
		Returns:
			返回创建的记录
		"""
		rec = cls()
		rec.player = player
		rec.season = season
		rec.save()

		return rec

	def onNewSeason(self, season: 'CompSeason'):
		"""
		新赛季回调
		Args:
			season (CompSeason): 新赛季数据
		Returns:
			返回新赛季的赛季记录
		"""
		new_rec = SeasonRecord()
		new_rec.player_id = self.player_id
		new_rec.season = season
		# TODO: 在这里计算并设置 new_rec 的段位星星
		#  （根据当前段位星星来计算新赛季的初始段位星星）
		#  最好在 utils.calc_utils 创建一个专门计算这个的类
		#  具体参考 utils.calc_utils 里面的其他算法

		return new_rec

	def convertToDict(self, type=None, order=None):

		if type == "rank":
			return {
				'id': self.player_id,
				'order': order,
				'name': self.player.name,
				'battle_point': self.player.battlePoint(),
				'star_num': self.star_num
			}

		suspensions = ModelUtils.objectsToDict(self.suspensions())

		return {
			'id': self.id,
			'season_id': self.season_id,
			'star_num': self.star_num,
			'point': self.point,

			'suspensions': suspensions,
		}

	# 增减赛季积分
	def adjustPoint(self, value):
		self.point = max(self.point+value, 0)

		self.save()

	# 增减星星数量
	async def adjustStarNum(self, value):
		rank, sub_rank, _ = self.rank()

		# 星星减少
		while value < 0:
			# 当前段位赛季积分和抵消因子
			self.point -= rank.offset_factor
			value += 1

			# 情况1，赛季积分不够完全抵消，point用完，此时value<0，段位可能改变
			if self.point < 0:
				self.point += rank.offset_factor
				value -= 1
				break

		# 情况2，赛季积分可以完全抵消星星减少数，args提前变为0，段位不变
		# 情况3，星星数增加，段位可能改变
		if value != 0:
			self.star_num += value
			# 限制不小于 0
			if self.star_num < 0: self.star_num = 0

			new_rank, new_sub_rank, _ = self.rank()
			if new_rank != rank or new_sub_rank != sub_rank:
				await self._emitRankChanged(new_rank, new_sub_rank)

		self.save()

	# 发送段位变更信息
	async def _emitRankChanged(self, rank, sub_rank):
		from game_module.consumer import EmitType
		from player_module.views import Common

		# 生成返回信息，规范见接口文档
		data = {'rank_id': rank, 'sub_rank': sub_rank, 'star_num': self.star_num}

		player = Common.getOnlinePlayer(self.player_id)
		# 使用 emit 函数发送信息，type 为发送信息的类型，
		# tag 为发送信息的标签（按照默认值即可）
		# data 为发送的信息，需要传一个 dict
		await player.consumer.emit(EmitType.RankChanged, data=data)

	def adjustCredit(self, player: 'Player', credit):
		player.credit += credit

		count = self.suspensions().count()
		now = datetime.datetime.now()

		# 针对第1，2，>=3次禁赛，分别设置3，7，30天的禁赛期
		if player.credit < self.SUSPEN_SCORE and count >= 2:
			SuspensionRecord.create(self.id, now, 30)

		elif player.credit < self.SUSPEN_SCORE and count == 1:
			SuspensionRecord.create(self.id, now, 7)

		elif player.credit < self.SUSPEN_SCORE and count == 0:
			SuspensionRecord.create(self.id, now, 3)

		player.save()

	# 所有的禁赛纪录
	def suspensions(self):
		return self.suspensionrecord_set.all()

	def isBanned(self) -> bool:
		"""
		当前是否禁赛
		Returns:
			返回是否禁赛
		"""
		now = datetime.datetime.now()
		suspensions = self.suspensions()

		for susp in suspensions:
			if susp.start_time <= now < susp.end_time:
				return True

		return False

	def rank(self) -> ('CompRank', int):
		"""
		计算当前实际段位
		Returns:
			返回实际段位对象（CompRank），子段位数目（从0开始）以及剩余星星数
		Examples:
			0 = > 学渣I(1, 0, 0)
			1 = > 学渣I(1, 0, 1)
			2 = > 学渣I(1, 0, 2)
			3 = > 学渣I(1, 0, 3)
			4 = > 学渣II(1, 1, 1)
			5 = > 学渣II(1, 1, 2)
			6 = > 学渣II(1, 1, 3)
			7 = > 学渣III(1, 2, 1)
			10 = > 学酥I(2, 1, 1)
		"""
		# ranks 储存了段位列表中的每一个段位的详细信息
		ranks = CompRank.objs()

		# 每个段位需要的星星数量相加
		star_num = self.star_num

		# 需要保证数据库的数据有序
		for rank in ranks:
			rank_stars = rank.rankStars()

			# 判断最后一个段位
			if rank_stars == 0:
				return rank, 0, star_num

			# 如果星星数目还可以扣
			if star_num > rank_stars:
				star_num -= rank_stars
			else:
				tmp_star = star_num - 1
				if tmp_star < 0:
					sub_rank = star_num = 0
				else:
					sub_rank = int(tmp_star / CompRank.STARS_PER_SUBRANK)
					star_num = (tmp_star % CompRank.STARS_PER_SUBRANK) + 1

				return rank, sub_rank, star_num

		return None, 0, star_num


# =======================
# 禁赛记录表
# =======================
class SuspensionRecord(models.Model):

	class Meta:
		verbose_name = verbose_name_plural = "禁赛记录"

	# 赛季记录,一对多的关系，多个禁赛记录对应一个赛季记录
	season_rec = models.ForeignKey('SeasonRecord', on_delete=models.CASCADE,
								   verbose_name="赛季记录")

	# 开始时间
	start_time = models.DateTimeField(auto_now_add=True, verbose_name="开始时间")

	# 结束时间
	end_time = models.DateTimeField(verbose_name="结束时间")

	# 创建一个实例，cls 可以直接当做 SuspensionRecord 来用
	@classmethod
	def create(cls, season_rec, start_time, time_length):
		record = cls()
		record.season_rec = season_rec
		record.start_time = start_time
		record.end_time = start_time + datetime.timedelta(days=time_length)

		record.save()

		return record

	def convertToDict(self):
		start_time = ModelUtils.timeToStr(self.start_time)
		end_time = ModelUtils.timeToStr(self.end_time)

		return {
			'start_time': start_time,
			'end_time': end_time
		}


# =======================
# 赛季配置表
# =======================
class CompSeason(GroupConfigure):

	class Meta:
		verbose_name = verbose_name_plural = "赛季信息"

	NOT_EXIST_ERROR = ErrorType.SeasonNotExist

	# 排行最大值
	MAX_RANK = 9999

	# 开始时间
	start_time = models.DateTimeField(verbose_name="开始时间")

	# 结束时间
	end_time = models.DateTimeField(verbose_name="结束时间")

	def _convertRanksData(self, count=MAX_RANK, player=None):
		"""
		转化排行数据
		Args:
			count (int): 排行条数
			player (Player): 对应玩家
		Returns:
			返回指定条数的排行数据
		"""
		res = []
		records = self.sortedSeasonRecords(count)
		count = min(count, len(records))

		for i in range(count):
			res.append(records[i].convertToDict('rank', i + 1))

		record, order = self.getPlayerSeasonRecord(player)
		record = record.convertToDict('rank', order)

		now = datetime.datetime.now()
		now = ModelUtils.timeToStr(now)

		return {
			'ranks': res,
			'rank': record,
			'update_time': now
		}

	def convertToDict(self, type=None, count=MAX_RANK, player=None):

		if type == "ranks":
			return self._convertRanksData(count, player)

		res = super().convertToDict()

		start_time = ModelUtils.timeToStr(self.start_time)
		end_time = ModelUtils.timeToStr(self.end_time)

		res['start_time'] = start_time
		res['end_time'] = end_time

		return res

	def seasonRecords(self) -> QuerySet:
		"""
		获取赛季的所有玩家记录
		Returns:
			返回赛季玩家记录
		"""
		return self.seasonrecord_set.all()

	def sortedSeasonRecords(self, count: int = MAX_RANK) -> QuerySet:
		"""
		获取排序后的赛季记录（根据段位星星排序）
		Args:
			count (int): 个数
		Returns:
			返回排序后的 QuerySet对 象
		"""
		return list(self.seasonRecords().order_by('-star_num'))[:count]

	def getPlayerSeasonRecord(self, player: 'Player'):
		"""
		获取玩家的排行
		Args:
			player (Player): 玩家
		Returns:
			返回玩家在当前赛季的排行（如果没有排行返回 0）
		"""
		order = 1
		record = None
		records = self.sortedSeasonRecords()
		for record in records:
			if record.player_id == player.id: break
			order += 1

		# 如果不在排行榜内，置为0
		if order >= self.MAX_RANK: order = 0

		return record, order


# =======================
# 段位配置表
# =======================
class CompRank(GroupConfigure):

	class Meta:
		verbose_name = verbose_name_plural = "段位信息"

	NOT_EXIST_ERROR = ErrorType.CompRankNotExist

	# 每个小段位所需星星数
	STARS_PER_SUBRANK = 3

	# 颜色
	color = models.CharField(max_length=7, null=False,
							 default='#000000', verbose_name="颜色")

	# 子段位数（0 为无限）
	sub_rank_num = models.PositiveSmallIntegerField(default=3, verbose_name="小段位数")

	# 积分因子
	score_factor = models.PositiveSmallIntegerField(default=80, verbose_name="积分因子")

	# 抵消积分
	offset_factor = models.PositiveSmallIntegerField(default=60, verbose_name="抵消使用积分")

	# 管理界面用：显示星级颜色
	def adminColor(self):
		from django.utils.html import format_html

		res = '<div style="background: %s; width: 48px; height: 24px;"></div>' % self.color

		return format_html(res)

	adminColor.short_description = "星级颜色"

	def convertToDict(self):
		res = super().convertToDict()

		res['color'] = self.color
		res['sub_rank_num'] = self.sub_rank_num
		res['score_factor'] = self.score_factor
		res['offset_factor'] = self.offset_factor

		return res

	# 计算每个段位需要的星星数量
	def rankStars(self):
		return self.sub_rank_num * self.STARS_PER_SUBRANK
