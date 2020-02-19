from .models import Player
from game_module.consumer import GameConsumer
from utils.runtime_manager import RuntimeData, RuntimeManager


# 在线玩家缓存信息
class OnlinePlayer(RuntimeData):

	def __init__(self, player, consumer):
		self.player: Player = player
		self.consumer: GameConsumer = consumer

	def add(self):
		if self.consumer is None: return
		self.consumer.setOnlineInfo(self)

	def delete(self):
		RuntimeData.delete(self)
		if self.consumer is None: return
		self.consumer.setOnlineInfo(None)


RuntimeManager.register(OnlinePlayer)
