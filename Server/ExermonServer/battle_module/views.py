from django.shortcuts import render
from .models import *
from .runtimes import *
from player_module.models import Player
from player_module.views import Common as PlayerCommon
from item_module.views import Common as ItemCommon
from item_module.views import Service as ItemService, Common as ItemCommon
from utils.exception import ErrorType

# Create your views here.


# =======================
# 对战服务类，封装管理题目模块的业务处理函数
# =======================
class Service:
	# 装备对战物资槽
	@classmethod
	async def equipItem(cls, consumer, player: Player, index: int, contitem_id: int, ):
		# 返回数据：无

		item_slot = Common.getBattleItemSlot(player)

		human_pack = PlayerCommon.getHumanPack(player)

		pack_item = PlayerCommon.getPackItem(player, id=contitem_id)

		pack_item, _ = human_pack.splitItem(pack_item, 1)

		return ItemService.slotContainerEquip(item_slot, pack_item, index=index)

	# 卸下对战物资槽
	@classmethod
	async def dequipItem(cls, consumer, player: Player, index: int, ):
		# 返回数据：无

		item_slot = Common.getBattleItemSlot(player)

		return ItemService.slotContainerEquip(item_slot, None, type_=HumanPackItem, index=index)

	# 对战开始匹配
	@classmethod
	async def matchStart(cls, consumer, player: Player, mode: int, ):
		# 返回数据：无
		Common.ensureNotBanned(player)

		Common.addMatchingPlayer(player, mode)

	# 对战取消匹配
	@classmethod
	async def matchCancel(cls, consumer, player: Player, ):
		# 返回数据：无
		Common.deleteMatchingPlayer(player)

	# 对战匹配进度
	@classmethod
	async def matchProgress(cls, consumer, player: Player, progress: int, ):
		# 返回数据：无
		battle = Common.getRuntimeBattle(player)

		battle.onMatchingProgress(player, progress)

	# 对战准备阶段完成
	@classmethod
	async def prepareComplete(cls, consumer, player: Player, type=None, contitem_id=None):
		# 返回数据：无
		from utils.interface_manager import Common as InterfaceCommon
		from item_module.models import ContItemType

		if type:
			type = InterfaceCommon.convertDataType(type, 'int')

		if contitem_id:
			contitem_id = InterfaceCommon.convertDataType(contitem_id, 'int')

		battle = Common.getRuntimeBattle(player)

		if type is None: cont_item = None

		elif type == ContItemType.QuesSugarPackItem.value or type == ContItemType.BattleItemSlotItem.value:

			cont_item = ItemCommon.getContItem(player=player, type_=type, id=contitem_id)

		else: raise GameException(ErrorType.IncorrectContItemType)

		battle.completePrepare(player, cont_item)

	@classmethod
	async def questionAnswer(cls, consumer, player: Player, selection: list, timespan: int, ):
		# 返回数据：无

		battle = Common.getRuntimeBattle(player)

		battle.answerQuestion(player, selection, timespan)

	@classmethod
	async def actionComplete(cls, consumer, player: Player, ):
		# 返回数据：无

		battle = Common.getRuntimeBattle(player)

		battle.completeAction(player)

	# 对战结果播放完成
	@classmethod
	async def resultComplete(cls, consumer, player: Player, ):
		# 返回数据：无

		battle = Common.getRuntimeBattle(player)

		battle.completeResult(player)


# =======================
# 对战公用类，封装关于物品模块的公用函数
# =======================
class Common:

	@classmethod
	def getBattleItemSlot(cls, player: Player, error: ErrorType = ErrorType.ContainerNotExist) -> BattleItemSlot:
		"""
		获取对战物资槽
		Args:
			player (Player): 所属玩家
			error (ErrorType): 异常时抛出错误
		Returns:
			玩家对应的对战物资槽
		"""
		return ItemCommon.getContainer(cla=BattleItemSlot, player=player, error=error)

	@classmethod
	def getBattleItemSlotItem(cls, player: Player, type: int, id: int) -> BattleItemSlotItem:
		"""
		从指定玩家的对战物资槽中获取指定物品容器项
		Args:
			player (Player): 玩家
			type (int): 容器项类型
			id (int): 容器项ID
		Returns:
			返回对应的物品容器项
		"""
		container = cls.getBattleItemSlot(player)

		return ItemCommon.getContItem(container=container, type_=type, id=id,
									  error=ErrorType.ItemNotEquiped)

	@classmethod
	def getMatchingPlayer(cls, player: Player) -> MatchingPlayer:
		"""
		获取匹配重的玩家
		Args:
			player (Player): 玩家
		Returns:
			返回匹配中的玩家对象（MatchingPlayer）
		"""
		return RuntimeManager.get(MatchingPlayer, player.id)

	@classmethod
	def addMatchingPlayer(cls, player: Player, mode: int) -> MatchingPlayer:
		"""
		添加匹配玩家（到匹配队列），若已存在，则不进行任何操作
		Args:
			player (Player): 玩家
			mode (int): 模式
		Returns:
			返回添加的匹配中的玩家对象（MatchingPlayer）
		"""
		mp = cls.getMatchingPlayer(player)
		if mp is not None: return mp

		data = MatchingPlayer(player, mode)

		return RuntimeManager.add(MatchingPlayer, data)

	@classmethod
	def deleteMatchingPlayer(cls, player: Player):
		"""
		获取匹配重的玩家
		Args:
			player (Player): 玩家
		Returns:
			返回匹配中的玩家对象（MatchingPlayer）
		"""
		mp = cls.getMatchingPlayer(player)
		if mp is None: return

		if mp.matched:
			raise GameException(ErrorType.AlreadyMatched)

		RuntimeManager.delete(MatchingPlayer, player.id)

	@classmethod
	def getRuntimeBattle(cls, player: Player, error: ErrorType = ErrorType.NotInBattle) -> RuntimeBattle:
		"""
		获取运行时对战
		Args:
			player (Player): 玩家
			error (ErrorType): 不存在时抛出异常
		Returns:
			返回运行时对战对象（RuntimeBattle）
		"""
		battler = cls.getRuntimeBattlePlayer(player, error)

		if battler is None: return None

		battle = battler.battle

		if battle is None and error is not None:
			raise GameException(error)

		return battle

	@classmethod
	def getRuntimeBattlePlayer(cls, player: Player, error: ErrorType = ErrorType.NotInBattle) -> RuntimeBattlePlayer:
		"""
		获取运行时对战玩家
		Args:
			player (Player): 玩家
			error (ErrorType): 不存在时抛出异常
		Returns:
			返回运行时对战玩家对象（RuntimeBattlePlayer）
		"""
		battler = RuntimeManager.get(RuntimeBattlePlayer, player.id)

		if battler is None and error is not None:
			raise GameException(error)

		return battler

	@classmethod
	def ensureNotBanned(cls, player: Player):
		"""
		保证没有被禁赛
		Args:
			player (Player): 玩家
		Raises:
			ErrorType.IsBanned: 已被禁赛
		"""
		cur_rec = player.currentSeasonRecord()

		if cur_rec.isBanned():
			raise GameException(ErrorType.IsBanned)
