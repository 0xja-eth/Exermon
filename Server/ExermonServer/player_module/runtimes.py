from .models import Player
from game_module.consumer import GameConsumer
from utils.runtime_manager import RuntimeData, RuntimeManager
import datetime


class OnlinePlayer(RuntimeData):
	"""
	在线玩家缓存信息，维护一个 Player 与 GameConsumer 的关系
	"""

	# 同步间隔
	MIN_DELTA = datetime.timedelta(0, 600)

	def __init__(self, player: Player, consumer: GameConsumer):
		"""
		初始化在线玩家缓存信息
		Args:
			player (Player): 玩家
			consumer (GameConsumer): 消费者
		"""
		self.player: Player = player
		self.consumer: GameConsumer = consumer

	def add(self):
		"""
		添加回调函数
		"""
		if self.consumer is None: return
		self.consumer.setOnlineInfo(self)

	def delete(self):
		"""
		删除回调函数
		"""
		from battle_module.runtimes import MatchingPlayer

		if self.consumer is None: return
		self.consumer.setOnlineInfo(None)

		# 清除对战匹配队列
		RuntimeManager.delete(MatchingPlayer, self.getKey())

	def getKey(self) -> object:
		"""
		生成该项对应的键
		Returns:
			返回玩家ID
		"""
		return self.player.id

	def isSaveRequired(self):
		"""
		是否需要保存数据
		"""
		if self.player.last_refresh_time is None: return True

		now = datetime.datetime.now()
		delta = now - self.player.last_refresh_time
		return delta >= self.MIN_DELTA


class OnlinePlayerManager:
	"""
	在线玩家管理器
	"""

	OnlinePlayers: dict = None

	@classmethod
	def loadOnlinePlayers(cls):
		"""
		读取在线玩家
		"""
		cls.OnlinePlayers = RuntimeManager.get(OnlinePlayer)

	@classmethod
	async def update(cls):
		"""
		更新维护
		"""
		if cls.OnlinePlayers is None: cls.loadOnlinePlayers()

		for key in cls.OnlinePlayers:
			online_player: OnlinePlayer = cls.OnlinePlayers[key]

			if online_player.isSaveRequired():
				online_player.player.save()


RuntimeManager.register(OnlinePlayer)
RuntimeManager.registerEvent(OnlinePlayerManager.update)
