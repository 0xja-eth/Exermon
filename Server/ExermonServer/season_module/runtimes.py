from season_module.models import CompSeason
from utils.runtime_manager import RuntimeManager
from utils.model_utils import Common as ModelUtils
from game_module.consumer import EmitType, GameConsumer
import datetime


# ===================================================
# 排行榜数据
# ===================================================
class RankListData:

	# 排行最大值
	MAX_RANK = 9999

	# 刷新最短间隔
	MIN_DELTA = datetime.timedelta(0, 60*30)

	# 更新时间（分钟数）
	UPDATE_MINUTES = [0, 30]

	# 排行榜列表(
	rank_list: list = None

	update_time: datetime.datetime = None

	season: 'CompSeason' = None

	def __init__(self, season: 'CompSeason'):
		self.season = season
		self.update()

	def update(self):
		"""
		生成排行
		"""
		self.rank_list = []

		count = self.MAX_RANK
		records = self.season.sortedSeasonRecords(count)
		count = min(count, len(records))

		for i in range(count):
			data = records[i].convertToDict('rank', i + 1)
			self.rank_list.append(data)

		self.update_time = datetime.datetime.now()

	def getRank(self, player: 'Player') -> dict:
		"""
		获取指定玩家的排行
		Args:
			player (Player): 玩家
		Returns:
			返回该玩家的排行数据字典
		"""
		for rank in self.rank_list:
			if rank['id'] == player.id: return rank

		record = player.seasonRecord(season=self.season)

		if record is None: return {}
		return record.convertToDict('rank', 0)

	def isUpdateRequired(self):
		"""
		是否需要更新
		Returns:
			返回排行榜是否需要更新
		"""
		now = datetime.datetime.now()
		delta = now - self.update_time

		return delta >= self.MIN_DELTA

		# return now.minute in self.UPDATE_MINUTES and \
		# 	now - self.update_time >= self.MIN_DELTA

	def convertToDict(self, player: 'Player', count=MAX_RANK):
		"""
		转化为字典
		Args:
			count (int): 排行条数
			player (Player): 当前玩家
		Returns:
			返回指定条数的排行数据
		"""
		count = min(count, len(self.rank_list))
		ranks = self.rank_list[:count]

		update_time = ModelUtils.timeToStr(self.update_time)

		return {
			'ranks': ranks,
			'rank': self.getRank(player),
			'update_time': update_time
		}


# ===================================================
# 赛季运行时管理类
# ===================================================
class SeasonManager:

	# 当前赛季
	CurrentSeason: CompSeason = None

	# 赛季切换回调事件列表
	OnSeasonChanged: list = []

	# 赛季排行榜映射（键为赛季ID，值为 RankListData）
	SeasonRankMap: dict = {}

	# region 赛季操作

	@classmethod
	def computeSeason(cls) -> CompSeason:
		"""
		计算当前赛季
		Returns:
			返回当前赛季，没有则返回 None
		"""
		# 获取所有赛季
		seasons = CompSeason.objs()

		# 当前日期和时间
		now = datetime.datetime.now()

		# 按照时间顺序从将来往以前读取赛季信息
		for season in seasons:
			# 当前日期和时间小于某个赛季的开始时间
			if season.start_time <= now < season.end_time:
				current_season = season
				return current_season

		return None

	@classmethod
	def refreshSeason(cls):
		"""
		刷新当前赛季
		"""
		cls.CurrentSeason = cls.computeSeason()
		cls.RankList = None

	@classmethod
	def getCurrentSeason(cls) -> CompSeason:
		"""
		获取缓存的当前赛季（若无缓存，自动计算）
		Returns:
			返回当前赛季
		"""
		if cls.CurrentSeason is None:
			cls.refreshSeason()

		return cls.CurrentSeason

	@classmethod
	async def maintainSeason(cls):
		"""
		维护当前赛季切换回调
		"""
		now = datetime.datetime.now()

		season = cls.getCurrentSeason()

		if season is None: return

		# 超过了目前赛季的终止日期
		if now >= season.end_time:
			await cls.onSeasonChanged()

	@classmethod
	async def onSeasonChanged(cls):
		"""
		赛季切换回调
		"""
		cls.refreshSeason()
		# 执行回调事件
		for event in cls.OnSeasonChanged: event()
		await cls.broadcastSeasonChange()

	@classmethod
	async def broadcastSeasonChange(cls):
		"""
		广播赛季切换信息
		"""
		await GameConsumer.broadcast(EmitType.SeasonSwitch,
									 {'season_id': cls.CurrentSeason.id})

	# endregion

	# region 排行榜操作

	# @classmethod
	# def updateRank(cls):
	# 	"""
	# 	管理赛季高分排行榜
	# 	"""
	# 	rank_list = cls.getCurrentRankList()
	# 	if rank_list is None: return
	#
	# 	if rank_list.isUpdateRequired():
	# 		rank_list.update()

	@classmethod
	def getCurrentRankList(cls) -> RankListData:
		"""
		获取当前赛季排行榜（如果没有会自动创建）
		Returns:
			返回当前赛季排行榜
		"""
		cur_season = cls.getCurrentSeason()
		if cur_season is None: return None

		return cls.getRankList(cur_season.id)

	@classmethod
	def getRankList(cls, season_id: int = None, season: CompSeason = None) -> RankListData:
		"""
		获取指定赛季的排行榜（如果没有会自动创建）
		Args:
			season_id (int): 赛季ID
			season (CompSeason): 赛季实例
		Returns:
			返回制定赛季ID的排行榜
		"""
		if season is not None: season_id = season.id
		else: season = CompSeason.get(id=season_id)

		# 如果该ID未有缓存结果，生成一个排行榜
		if season_id in cls.SeasonRankMap:
			rank_list = cls.SeasonRankMap[season_id]
			if rank_list.isUpdateRequired(): rank_list.update()

		else:
			# 生成排行榜并缓存
			rank_list = cls.SeasonRankMap[season_id] = RankListData(season)

		return rank_list

	# endregion


RuntimeManager.registerEvent(SeasonManager.maintainSeason)
# RuntimeManager.registerEvent(SeasonManager.updateRank)
