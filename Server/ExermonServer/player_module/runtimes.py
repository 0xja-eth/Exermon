from .models import Player
from game_module.consumer import GameConsumer
from utils.runtime_manager import RuntimeData, RuntimeManager


class OnlinePlayer(RuntimeData):
	"""
	在线玩家缓存信息，维护一个 Player 与 GameConsumer 的关系
	"""

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
		# RuntimeData.delete(self)
		if self.consumer is None: return
		self.consumer.setOnlineInfo(None)

	def getKey(self) -> object:
		"""
		生成该项对应的键
		Returns:
			返回玩家ID
		"""
		return self.player.id


RuntimeManager.register(OnlinePlayer)
