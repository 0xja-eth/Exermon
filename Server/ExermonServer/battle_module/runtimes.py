from .models import *
from player_module.models import Player
from player_module.runtimes import OnlinePlayer
from player_module.views import Common as PlayerCommon
from exermon_module.models import ExerSlotItem, ExerSkillSlotItem
from game_module.consumer import ChannelLayerTag, EmitType
from game_module.models import ParamValue, ParamRate
from game_module.runtimes import *

from utils.model_utils import Common as ModelUtils

from enum import Enum
import datetime


class MatchingPlayer(RuntimeData):
	"""
	匹配玩家
	"""

	# 默认星星差
	DEFAULT_STAR_DELTA = 3

	# 最大星星差
	MAX_STAR_DELTA = 30

	def __init__(self, player: Player, mode: int):
		"""
		初始化
		Args:
			player (Player): 玩家
			mode (int): 模式
		"""
		self.player = player
		self.subjects = player.subjects()
		self.mode = mode

		self.matched = False
		self.star_delta = self.DEFAULT_STAR_DELTA
		self.start_time = datetime.datetime.now()

		self.season_record = player.currentSeasonRecord()
		self.star_num = self.season_record.star_num

	def getKey(self) -> object:
		"""
		生成该项对应的键
		"""
		return self.player.id

	def match(self, oppo: 'MatchingPlayer'):
		"""
		匹配对手
		Args:
			oppo (MatchingPlayer): 对手
		"""
		if self._matchable(oppo):
			self._onMatch(oppo)
		else:
			self.star_delta += 1

	def _matchable(self, oppo: 'MatchingPlayer') -> bool:
		"""
		可否匹配
		Args:
			oppo (MatchingPlayer): 对方
		Returns:
			若可以匹配，返回 True，否则返回 False
		"""
		if oppo.matched: return False
		if oppo.mode != self.mode: return False
		if oppo.subjects != self.subjects: return False
		if abs(oppo.star_num - self.star_num) > self.star_delta: return False

		return True

	def _onMatch(self, oppo: 'MatchingPlayer'):
		"""
		匹配回调
		Args:
			oppo (MatchingPlayer): 对手
		"""
		self.matched = oppo.matched = True

		RuntimeBattleManager.createBattle(self, oppo)


class MatchingManager:
	"""
	匹配管理类
	"""
	Queue: dict = None

	@classmethod
	def loadQueue(cls):
		"""
		读取匹配队列
		"""
		cls.Queue = RuntimeManager.get(MatchingPlayer)

	@classmethod
	async def process(cls):
		"""
		每帧处理
		"""
		if cls.Queue is None: cls.loadQueue()

		temp = cls.Queue.copy()
		values = list(temp.values())

		for i in range(len(temp)):
			mp: MatchingPlayer = values[i]

			for j in range(i+1, len(temp)):
				mp2: MatchingPlayer = values[j]
				mp.match(mp2)


class BattleBuff:
	"""
	对战 Buff
	"""
	plus_params = None  # 附加属性
	rate_params = None  # 附加属性（比率）
	round = 1

	def __init__(self, plus_params: list = None, rate_params: list = None, round: int = 1):
		"""
		设置题目糖
		Args:
			plus_params (list): 附加能力值
			rate_params (list): 能力值加成
			round (int): 持续回合数（行动回合数）
		"""
		if rate_params is None: rate_params = []
		if plus_params is None: plus_params = []

		self.plus_params = plus_params
		self.rate_params = rate_params
		self.round = round

	def convert(self) -> dict:
		"""
		转化为字典
		Returns:
			返回转化后的字典
		"""
		plus_params = ModelUtils.objectsToDict(self.plus_params)
		rate_params = ModelUtils.objectsToDict(self.rate_params)

		return {
			'plus_params': plus_params,
			'rate_params': rate_params,
			'round': self.round
		}

	def plusParam(self, **kwargs) -> ParamValue:
		"""
		获取附加属性值
		Args:
			**kwargs (**dict): 查询参数
		Returns:
			返回特定参数的附加属性值对象（ParamValue）
		"""
		return ModelUtils.sum(self.plus_params, **kwargs)

	def rateParam(self, **kwargs) -> ParamValue:
		"""
		获取附加属性率
		Args:
			**kwargs (**dict): 查询参数
		Returns:
			返回特定参数的附加属性率对象（ParamValue）
		"""
		return ModelUtils.mult(self.rate_params, **kwargs)


class RuntimeBattleItem:
	"""
	运行时对战物品
	"""
	def __init__(self, slot_item: BattleItemSlotItem):
		self.slot_item = slot_item
		self.freeze_round = 0

	def convert(self):
		"""
		转化为字典
		Returns:
			返回转化后的字典
		"""
		item_id = 0
		if self.slot_item.pack_item is not None:
			item_id = self.slot_item.pack_item.item_id

		return {
			'id': self.slot_item.index,
			'item_id': item_id,
			'freeze_round': self.freeze_round
		}

	# region 事件操作

	def onRoundTerminated(self):
		"""
		回合结束回调
		"""
		self.freeze_round = max(0, self.freeze_round - 1)

	# endregion

	# region 物品操作

	def item(self) -> UsableItem:
		"""
		获取对应物品
		Returns:
			返回对应的物品
		"""
		pack_item = self.slot_item.pack_item
		return None if pack_item is None else pack_item.item

	def freezeRound(self) -> int:
		"""
		获取物品的冻结时间
		Returns:
			返回对应物品的冻结时间
		"""
		item = self.item()
		return 0 if item is None else item.freeze

	def isUsable(self):
		"""
		能否使用
		Returns:
			返回物品能否使用
		"""
		if not self.slot_item.isContItemUsable(occasion=ItemUseOccasion.Battle): return False

		if self.freeze_round > 0: return False

		return True

	def ensureUsable(self):
		"""
		确保可使用
		Raises:
			ErrorType.ItemFrozen: 物品已冻结
		"""
		self.slot_item.ensureContItemUsable(occasion=ItemUseOccasion.Battle)

		if self.freeze_round > 0:
			raise GameException(ErrorType.ItemFrozen)

	def useItem(self):
		"""
		使用物品
		"""
		self.ensureUsable()

		self.freeze_round = self.freezeRound() + 1

		# occasion 固定为 ItemUseOccasion.Battle
		self.slot_item.useItem()

	# endregion

	""" 占位符 """


class RuntimeBattleExerSkill:
	"""
	运行时缓存的艾瑟萌技能
	"""
	def __init__(self, exermon: 'RuntimeBattleExermon',
				 skill_slot_item: ExerSkillSlotItem):
		"""
		初始化
		Args:
			exermon (RuntimeBattleExermon): 艾瑟萌
			skill_slot_item (ExerSkillSlotItem): 艾瑟萌技能槽项
		"""
		self.exermon = exermon
		self.skill_item = skill_slot_item
		self.skill: ExerSkill = self.skill_item.skill

		self.use_count = 0
		self.freeze_round = 0

	def convert(self):
		"""
		转化为字典
		Returns:
			返回转化后的字典
		"""
		skill_id = 0 if self.skill is None else self.skill.id
		
		return {
			'id': self.skill_item.index,
			'skill_id': skill_id,
			'use_count': self.use_count,
			'freeze_round': self.freeze_round
		}

	# region 事件操作

	def onRoundTerminated(self):
		"""
		回合结束回调
		"""
		self.freeze_round = max(0, self.freeze_round - 1)

	# endregion

	# region 技能操作

	def freezeRound(self) -> int:
		"""
		获取技能的冻结时间
		Returns:
			返回技能的冻结时间
		"""
		return 0 if self.skill is None else self.skill.freeze

	def maxUseCount(self) -> int:
		"""
		获取技能最大使用次数
		Returns:
			返回最大使用次数
		"""
		return 0 if self.skill is None else self.skill.max_use_count

	def mpCost(self) -> int:
		"""
		获取精力值消耗点数
		Returns:
			返回精力值消耗点数
		"""
		return 0 if self.skill is None else self.skill.mp_cost

	def isUsable(self):
		"""
		能否使用
		Returns:
			返回技能能否使用
		"""
		occ = ItemUseOccasion.Battle
		if not self.skill_item.isContItemUsable(occ): return False

		if self.exermon.mp < self.mpCost(): return False

		use_count = self.maxUseCount()
		if self.use_count >= use_count > 0: return False

		if self.freeze_round > 0: return False

		return True

	def ensureUsable(self):
		"""
		确保可使用
		Raises:
			ErrorType.MPInsufficient: MP不足
			ErrorType.NoUseCount: 使用次数已满
			ErrorType.ItemFrozen: 物品已冻结
		"""
		self.skill_item.ensureContItemUsable(occasion=ItemUseOccasion.Battle)

		if self.exermon.mp < self.mpCost():
			raise GameException(ErrorType.MPInsufficient)

		use_count = self.maxUseCount()
		if self.use_count >= use_count > 0:
			raise GameException(ErrorType.NoUseCount)

		if self.freeze_round > 0:
			raise GameException(ErrorType.ItemFrozen)

	def useSkill(self):
		"""
		使用技能
		"""
		self.ensureUsable()

		self.exermon.costMP(self.mpCost())
		self.freeze_round = self.freezeRound()
		self.use_count += 1

		self.skill_item.useItem()

	# endregion

	""" 占位符 """


class RuntimeBattleExermon:
	"""
	运行时缓存的对战艾瑟萌
	"""
	# 原始属性值
	params = None
	mp = 0

	# Buff
	buffs = None  # Buff
	skills = None  # 技能

	# region 初始化

	def __init__(self, exer_slot_item: ExerSlotItem):
		"""
		初始化
		Args:
			exer_slot_item (ExerSlotItem): 艾瑟萌槽项
		"""
		self.exermon = exer_slot_item
		self.subject = self.exermon.subject
		self.initParams()
		self.initExerSkills()

	def initParams(self):
		"""
		初始化属性
		"""
		self.params = self.exermon.paramVals()
		self.mp = self.paramVal(attr='mmp').getValue()
		self.buffs = []

	def initExerSkills(self):
		"""
		初始化艾瑟萌技能
		"""
		self.skills = []

		skill_slot = self.exermon.player_exer.exerSkillSlot()

		for slot_item in skill_slot.contItems():
			skill = RuntimeBattleExerSkill(self, slot_item)
			self.skills.append(skill)

	# endregion

	def convert(self) -> dict:
		"""
		转化为字典
		Returns:
			返回转化后的字典
		"""
		params = ModelUtils.objectsToDict(self.params)
		buffs = ModelUtils.objectsToDict(self.buffs)
		skills = ModelUtils.objectsToDict(self.skills)

		exermon_id = 0
		if self.exermon.player_exer is not None:
			exermon_id = self.exermon.player_exer.item_id

		return {
			'subject_id': self.subject.id,
			'exermon_id': exermon_id,
			'mp': self.mp,
			'params': params,
			'buffs': buffs,
			'skills': skills
		}

	# region 回合操作

	def onRoundTerminated(self):
		"""
		回合结束回调
		"""
		buffs = self.buffs.copy()

		for buff in buffs:
			buff.round -= 1
			if buff.round <= 0:
				self.removeBuff(buff)

	# endregion

	# region Buff 处理

	def addBuff(self, plus_params: list = None, rate_params: list = None, round: int = 1, buff: BattleBuff = None) -> BattleBuff:
		"""
		添加 Buff
		Args:
			plus_params (list): 附加能力值
			rate_params (list): 能力值加成
			round (int): 持续回合数（行动回合数）
			buff (BattleBuff): 已经定义好的 Buff
		Returns:
			返回添加的 Buff
		"""
		if buff is None:
			buff = BattleBuff(plus_params, rate_params, round)

		self.buffs.append(buff)

	def removeBuff(self, buff: BattleBuff):
		"""
		移除 Buff
		Args:
			buff (BattleBuff): Buff
		"""
		self.buffs.remove(buff)

	# endregion

	# region 技能处理

	def getUsableSkills(self) -> list:
		"""
		获取可用的技能列表
		Returns:
			返回可用技能列表
		"""
		return ModelUtils.filter(self.skills, lambda s: s.isUsable())

	# endregion

	# region 属性处理

	def costMP(self, val: int):
		"""
		消耗精力值
		Args:
			val (int): 消耗量
		"""
		self.mp -= val

	def recoverMP(self, val: int = 0, rate: float = 0):
		"""
		回复精力值
		Args:
			val (int): 精力回复值
			rate (int): 精力回复率
		"""
		param = self.paramVal(attr='mmp')
		value = param.getValue()

		new_mp = self.mp + val + value * rate
		self.mp = max(min(new_mp, value), param.param.min_value)

	def addParam(self, param_id: int, round: int = 1, val: int = 0, rate: float = 1):
		"""
		添加能力值
		Args:
			param_id (int): 属性ID
			round (int): 回合数
			val (int): 属性提升值
			rate (int): 属性提升率
		"""
		from exermon_module.models import PlayerExerParamBase, PlayerExerParamRate

		plus_params = []
		rate_params = []

		if val != 0:
			plus_param = PlayerExerParamBase(param_id=param_id)
			plus_param.setValue(val, False)
			plus_params.append(plus_param)

		if rate != 1:
			rate_param = PlayerExerParamRate(param_id=param_id)
			rate_param.setValue(rate, False)
			rate_params.append(rate_param)

		self.addBuff(plus_params, rate_params, round)

	def originParam(self, **kwargs) -> ParamValue:
		"""
		获取原始属性值
		Args:
			**kwargs (**dict): 查询参数
		Returns:
			返回特定参数的原始属性值对象（ParamValue）
		"""
		return ModelUtils.sum(self.params, **kwargs)

	def plusParam(self, **kwargs) -> ParamValue:
		"""
		获取附加属性值
		Args:
			**kwargs (**dict): 查询参数
		Returns:
			返回特定参数的附加属性值对象（ParamValue）
		"""
		return ModelUtils.sum(self.buffs, lambda buff: buff.plusParam(**kwargs))

	def rateParam(self, **kwargs) -> ParamValue:
		"""
		获取附加属性率
		Args:
			**kwargs (**dict): 查询参数
		Returns:
			返回特定参数的附加属性率对象（ParamValue）
		"""
		return ModelUtils.mult(self.buffs, lambda buff: buff.rateParam(**kwargs))

	def paramVal(self, **kwargs) -> ParamValue:
		"""
		获取实际属性值
		Args:
			**kwargs (**dict): 查询参数
		Returns:
			返回特定参数的实际属性值对象（ParamValue）
		"""
		base = self.originParam(**kwargs)
		plus = self.plusParam(**kwargs)
		rate = self.rateParam(**kwargs)

		return (base + plus) * rate

	# endregion

	""" 占位符 """


class ActionType(Enum):
	"""
	行动类型枚举
	"""
	Prepare = 0  # 准备
	Attack = 1  # 攻击


class RuntimeAction:
	"""
	运行时行动
	"""
	# 行动类型
	type: ActionType = None

	# 准备操作
	item_type: ItemType = ItemType.Unset
	item_id = 0

	# 攻击操作
	skill_id = 0
	target_type: TargetType = TargetType.Enemy
	result_type: HitResultType = HitResultType.Unknown
	hurt: int = 0

	def __init__(self, type: ActionType):
		self.type = type

	def convert(self):
		"""
		转化为字典
		Returns:
			返回转化后的字典
		"""
		res = {'type': self.type.value}

		if self.type == ActionType.Prepare:
			res['item_type'] = self.item_type.value
			res['item_id'] = self.item_id

		if self.type == ActionType.Attack:
			res['skill_id'] = self.skill_id
			res['target_type'] = self.target_type.value
			res['result_type'] = self.result_type.value
			res['hurt'] = self.hurt

		return res


class RuntimeBattlePlayer(RuntimeData):
	"""
	运行时缓存的对战玩家类
	"""
	# 体力值
	mhp = hp = 0

	# 艾瑟萌
	exermons = None

	# 物品
	battle_items = None

	# region 阶段属性

	# 进度
	progress = 0

	# 准备阶段
	prepared = False
	slot_item: SlotContItem = None
	pack_item: PackContItem = None

	# 作答阶段
	round_result: BattleRoundResult = None
	question_completed = False

	# 行动阶段
	actions = None
	action_completed = False

	# 结算阶段
	result_completed = False

	# endregion

	# region 初始化

	def __init__(self, battle: 'RuntimeBattle', battle_player: BattlePlayer):
		"""
		初始化
		Args:
			battle_player (BattlePlayer): 玩家
		"""
		self.battle = battle

		self.battle_player = battle_player
		self.player = battle_player.exactlyPlayer()
		self.exer_slot = self.player.exerSlot()
		self.battle_item_slot = self.player.battleItemSlot()

		self.initExermons()
		self.initBattleItems()
		self.initParams()

		self.actions = []

	def initParams(self):
		"""
		初始化属性
		"""
		self.mhp = self.hp = self.getSumMHP().getValue()

	def initExermons(self):
		"""
		初始化艾瑟萌
		"""
		self.exermons = {}

		for slot_item in self.exer_slot.contItems():
			battle_exermon = RuntimeBattleExermon(slot_item)
			self.exermons[slot_item.subject] = battle_exermon

	def initBattleItems(self):
		"""
		初始化对战物资
		"""
		self.battle_items = []

		for slot_item in self.battle_item_slot.contItems():
			battle_item = RuntimeBattleItem(slot_item)
			self.battle_items.append(battle_item)

	# endregion

	def getKey(self) -> tuple:
		"""
		生成该项对应的键
		Returns:
			返回双方玩家ID组成的元组
		"""
		return self.player.id

	def convert(self, res: dict = None) -> dict:
		"""
		转化为字典
		Args:
			res (dict): 数据
		Returns:
			返回转化后的字典
		"""
		if res is None: res = {}

		exermons = ModelUtils.objectsToDict(list(self.exermons.values()))
		battle_items = ModelUtils.objectsToDict(self.battle_items)

		res['id'] = self.player.id
		res['mhp'] = self.mhp
		res['hp'] = self.hp
		res['exermons'] = exermons
		res['battle_items'] = battle_items

		return res

	# region 回合操作

	def onRoundTerminated(self):
		"""
		回合结束回调
		"""
		self.resetRoundStatus()

		for key in self.exermons:
			self.exermons[key].onRoundTerminated()
		for item in self.battle_items:
			item.onRoundTerminated()

		self.mhp = self.getSumMHP().getValue()

	def resetRoundStatus(self):
		"""
		重置回合状态
		"""
		self.prepared = False
		self.question_completed = False
		self.action_completed = False
		self.result_completed = False

		self.slot_item = None
		self.pack_item = None

		self.round_result = None

		self.actions = []

	def isCorrect(self) -> bool:
		"""
		当是否已作答且答题正确
		Returns:
			返回作答是否正确
		"""
		return self.round_result is not None and self.round_result.correct()

	# endregion

	# region 数据操作

	def isDead(self):
		"""
		是否已死亡
		Returns:
			返回当前玩家是否已死亡
		"""
		return self.hp <= 0

	def setupRoundResult(self, selection: list = [], timespan: int = -1):
		"""
		配置回合结果
		Args:
			selection (list): 选择情况
			timespan (int): 作答时长
		"""
		if self.round_result is not None: return

		self.battle_player.answerCurrentRound(selection, timespan)

		self.round_result = self.battle_player.currentRound()

	def getExermons(self) -> list:
		"""
		获取以列表为组织形式的艾瑟萌列表
		Returns:
			返回运行时艾瑟萌对象列表
		"""
		return list(self.exermons.values())

	def getCurrentExermon(self) -> RuntimeBattleExermon:
		"""
		获取当前回合的艾瑟萌
		Returns:
			返回当前艾瑟萌
		"""
		subject = self.battle.getSubject()

		return ModelUtils.get(self.getExermons(), subject=subject)

	# region 物品操作

	def getRuntimeItem(self, cont_item: BattleItemSlotItem) -> RuntimeBattleItem:
		"""
		获取运行时物品
		Args:
			cont_item (BattleItemSlotItem): 对战物资槽项
		Returns:
			返回运行时物品对象
		Raises:
			ErrorType.ItemNotEquiped: 未装备物品
		"""
		for item in self.battle_items:
			if item.slot_item == cont_item: return item

		raise GameException(ErrorType.ItemNotEquiped)

	def prepareItem(self, cont_item: BaseContItem):
		"""
		准备物品
		Args:
			cont_item (BaseContItem): 物品
		Raises:
			ErrorType.IncorrectContItemType: 错误物品类型
		"""
		from question_module.models import QuesSugarPackItem

		self.prepared = True

		if cont_item is None: return

		# 如果是对战物资
		if isinstance(cont_item, BattleItemSlotItem):
			# runtime_item = self.getRuntimeItem(cont_item)
			# runtime_item.useItem()

			self.slot_item = cont_item
			self.pack_item = cont_item.pack_item

		# 如果是题目糖
		elif isinstance(cont_item, QuesSugarPackItem):
			# cont_item.useItem()

			self.pack_item = cont_item

		else: raise GameException(ErrorType.IncorrectContItemType)

	def useItem(self):
		"""
		使用当前设置好的物品
		"""
		if self.slot_item is not None:
			if isinstance(self.slot_item, BattleItemSlotItem):
				runtime_item = self.getRuntimeItem(self.slot_item)
				runtime_item.useItem()

		elif self.pack_item is not None:
			from question_module.models import QuesSugarPackItem

			if isinstance(self.pack_item, QuesSugarPackItem):
				self.pack_item.useItem(ItemUseOccasion.Battle)

	def processItem(self):
		"""
		处理物品使用
		"""
		if self.pack_item is None: return
			# self.addPrepareAction()

		self.useItem()

		from utils.calc_utils import BattleItemEffectProcessor

		BattleItemEffectProcessor.process(self.pack_item, self)

	# endregion

	# region 攻击操作

	def processAttack(self, oppo: 'RuntimeBattlePlayer'):
		"""
		执行攻击
		Args:
			oppo (RuntimeBattlePlayer): 对手
		"""
		from utils.calc_utils import BattleAttackProcessor

		BattleAttackProcessor.process(self, oppo)

	# endregion

	# region 行动操作

	def addPrepareAction(self, item: BaseItem = None,
						 item_type: ItemType = ItemType.Unset, item_id: int = 0):
		"""
		添加准备行动
		Args:
			item (BaseItem): 物品
			item_type (ItemType): 物品类型
			item_id (int): 物品ID
		"""
		if item is not None:
			item_type = item.TYPE
			item_id = item.id

		# if item_id == 0 or item_type == ItemType.Unset:
		# 	return

		action = RuntimeAction(ActionType.Prepare)
		action.item_type = item_type
		action.item_id = item_id

		self.actions.append(action)

	def addAttackAction(self, skill: ExerSkill = None,
						target_type: TargetType = TargetType.Enemy,
						result_type: HitResultType = HitResultType.Unknown,
						hurt: int = 0):
		"""
		添加攻击行动
		Args:
			skill (ExerSkill): 技能（为 None 则为普通攻击）
			target_type (TargetType): 实际目标类型（有可能与技能的目标类型不一致）
			result_type (HitResultType): 命中结果类型
			hurt (int): 伤害点数
		"""
		action = RuntimeAction(ActionType.Attack)
		action.skill_id = 0 if skill is None else skill.id
		action.target_type = target_type
		action.result_type = result_type
		action.hurt = hurt

		self.actions.append(action)

	# endregion

	# region 属性操作

	def getSumMHP(self) -> ParamValue:
		"""
		获取总体力值
		Returns:
			总体力值对象
		"""
		return ModelUtils.sum(self.getExermons(),
							  lambda exer: exer.paramVal(attr="mhp"))

	def recoverHP(self, val: int = 0, rate: float = 0):
		"""
		回复体力值
		Args:
			val (int): 体力回复值
			rate (float): 体力回复率
		"""
		param = self.getSumMHP()
		value = param.getValue()

		old_hp = self.hp
		new_hp = self.hp + val + value * rate
		self.hp = max(min(new_hp, value), 0)

		self.round_result.processRecovery(self.hp - old_hp)

	# endregion

	# endregion

	""" 占位符 """


class BattleStatus(Enum):
	"""
	对战状态枚举
	"""
	Matching = 1  # 匹配中
	Preparing = 2  # 准备中
	Questing = 3  # 作答中
	Acting = 4  # 行动中
	Resulting = 5  # 结算中
	Terminating = 6  # 结束中
	Terminated = 7  # 已结束


class RuntimeBattle(RuntimeData):
	"""
	运行时缓存的对战类
	"""
	# 后台等待附加时长（秒）
	BACKEND_DELTA_TIME = 30

	# 准备阶段时长（秒）
	PREPARE_TIME = 5

	# 准备阶段结束数据发射时差（秒）
	PREPARE_END_WAIT_TIME = 2

	# 准备阶段结束数据发射时差（秒）
	QUESTION_TIME_RATE = 1.5

	# 最大行动时长（秒）（仅后台计时）
	MAX_ACTION_TIME = 30

	# 结算时长（秒）
	RESULT_TIME = 5

	# 对战开始前等待时长（秒）
	BATTLE_START_WAIT_TIME = 2

	# 行动开始前等待时长（秒）
	ACTION_START_WAIT_TIME = 2

	# 答题结果数据发射时差（秒）
	ROUND_RESULT_WAIT_TIME = 2

	# 当前回合数据
	cur_round: BattleRound = None

	# 攻击方
	attacker: RuntimeBattlePlayer = None

	# 计时器
	timer = None

	def __init__(self, player1: Player, player2: Player, mode: int):
		"""
		初始化
		Args:
			player1 (Player): 玩家1
			player2 (Player): 玩家2
			mode (int): 对战模式
		"""
		self.player1 = player1
		self.player2 = player2

		self.record = BattleRecord.create(player1, player2, mode)

		battler1 = self.record.firstPlayer()
		battler2 = self.record.secondPlayer()

		self.runtime_battler1 = RuntimeBattlePlayer(self, battler1)
		self.runtime_battler2 = RuntimeBattlePlayer(self, battler2)

		self._startMatching()

		self.round_num = 0

	# region 基本操作封装

	def _emit(self, type: EmitType, data: dict = None, timespan: int = 0, player: Player = None):
		"""
		发射数据，player 参数默认为 None，为 None 时将向对战中所有玩家发送信息
		若 player 不为 None，则只对指定的玩家发送信息
		Args:
			type (EmitType): 发射类型
			data (dict): 发射数据
			timespan (int): 延迟时间（秒）
			player (Player): 玩家
		"""

		if data is None: data = {}

		tag = ChannelLayerTag.Self

		if player is None:
			player = self.player1
			tag = ChannelLayerTag.Battle

		consumer = self.getOnlinePlayer(player).consumer

		oper = AsyncOper(consumer.emit, timespan, type, tag, data)
		RuntimeManager.add(AsyncOper, oper)

	def _joinGroup(self, player: Player):
		"""
		加入对战组
		Args:
			player (Player): 玩家
		"""
		name = self.__generateGroupName()

		consumer = self.getOnlinePlayer(player).consumer

		oper = AsyncOper(consumer.joinGroup, 0, ChannelLayerTag.Battle, name)
		RuntimeManager.add(AsyncOper, oper)

	def _leaveGroup(self, player: Player):
		"""
		离开对战组
		Args:
			player (Player): 玩家
		"""
		consumer = self.getOnlinePlayer(player).consumer

		oper = AsyncOper(consumer.leaveGroup, 0, ChannelLayerTag.Battle)
		RuntimeManager.add(AsyncOper, oper)

	# endregion

	# region 加入/退出

	def add(self):
		"""
		添加时回调函数
		"""
		self._joinBattleGroup()
		self._addRuntimeBattlers()

	def delete(self):
		"""
		删除时回调
		"""
		self._leaveBattleGroup()
		self._deleteRuntimeBattlers()

	def _addRuntimeBattlers(self):
		"""
		添加运行时对战玩家到 RuntimeManager
		"""
		RuntimeManager.add(RuntimeBattlePlayer, self.runtime_battler1)
		RuntimeManager.add(RuntimeBattlePlayer, self.runtime_battler2)

	def _deleteRuntimeBattlers(self):
		"""
		移除运行时对战玩家
		"""
		RuntimeManager.delete(RuntimeBattlePlayer, self.runtime_battler1)
		RuntimeManager.delete(RuntimeBattlePlayer, self.runtime_battler2)

	def _joinBattleGroup(self):
		"""
		加入对战组
		"""
		self._joinGroup(self.player1)
		self._joinGroup(self.player2)

		self._emit(EmitType.Matched, self._generateMatchedData(),
				   self.BATTLE_START_WAIT_TIME)

	def __generateGroupName(self) -> str:
		"""
		生成对战组名
		Returns:
			返回对应的组名
		"""
		return str(self.record)

	def _generateMatchedData(self) -> dict:
		"""
		生成匹配信息
		Returns:
			返回匹配信息字典
		"""
		online_player1 = self.getOnlinePlayer(self.player1)
		online_player2 = self.getOnlinePlayer(self.player2)

		return {
			'player1': self.player1.convert(
				type="matched", online_player=online_player1,
				battle_player=self.runtime_battler1),
			'player2': self.player2.convert(
				type="matched", online_player=online_player2,
				battle_player=self.runtime_battler2),
		}

	def _leaveBattleGroup(self):
		"""
		移除对战组
		"""
		self._leaveGroup(self.player1)
		self._leaveGroup(self.player2)

	# endregion

	# region 数据处理

	# region 计时器

	def setTimer(self, second: int):
		"""
		设置计时器
		Args:
			second (int): 秒数
		"""
		now = datetime.datetime.now()
		second += self.BACKEND_DELTA_TIME
		self.timer = now + datetime.timedelta(0, second)

	def clearTimer(self):
		"""
		清空计时器
		"""
		self.timer = None

	def checkTimer(self) -> bool:
		"""
		检查时钟是否过期
		Returns:
			返回是否过期
		"""
		if self.timer is None: return False
		return datetime.datetime.now() >= self.timer

	# endregion

	# region 获取数据

	def getKey(self) -> tuple:
		"""
		生成该项对应的键
		Returns:
			返回双方玩家ID组成的元组
		"""
		return self.player1.id, self.player2.id

	def getBattler(self, player: Player = None, battle_player: BattlePlayer = None,
					   runtime_battler: RuntimeBattlePlayer = None) -> RuntimeBattlePlayer:
		"""
		获取对战玩家
		Args:
			player (Player): 玩家实例
			battle_player (BattlePlayer): 对战玩家
			runtime_battler (RuntimeBattlePlayer): 对战玩家实例
		Returns:
			玩家对应的运行时对战玩家
		"""
		if player and player.id == self.player1.id: return self.runtime_battler1
		if player and player.id == self.player2.id: return self.runtime_battler2

		if battle_player and battle_player.player_id == self.player1.id:
			return self.runtime_battler1
		if battle_player and battle_player.player_id == self.player2.id:
			return self.runtime_battler2

		if runtime_battler and runtime_battler == self.runtime_battler1:
			return self.runtime_battler1
		if runtime_battler and runtime_battler == self.runtime_battler2:
			return self.runtime_battler2

		return None

	def getOppoBattler(self, player: Player = None, battle_player: BattlePlayer = None,
					   runtime_battler: RuntimeBattlePlayer = None) -> RuntimeBattlePlayer:
		"""
		获取对方对战玩家
		Args:
			player (Player): 玩家实例
			battle_player (BattlePlayer): 对战玩家
			runtime_battler (RuntimeBattlePlayer): 对战玩家实例
		Returns:
			玩家对应的运行时对战玩家
		"""
		if player and player == self.player1: return self.runtime_battler2
		if player and player == self.player2: return self.runtime_battler1

		if battle_player and battle_player.player_id == self.player1.id:
			return self.runtime_battler2
		if battle_player and battle_player.player_id == self.player2.id:
			return self.runtime_battler1

		if runtime_battler and runtime_battler == self.runtime_battler1:
			return self.runtime_battler2
		if runtime_battler and runtime_battler == self.runtime_battler2:
			return self.runtime_battler1

		return None

	def getOnlinePlayer(self, player: Player) -> OnlinePlayer:
		"""
		获取在线玩家
		Args:
			player (Player): 玩家
		Returns:
			返回玩家对应的在线玩家，找不到返回 None
		"""
		return PlayerCommon.getOnlinePlayer(player.id)

	def getOppoOnlinePlayer(self, player: Player) -> OnlinePlayer:
		"""
		获取对方在线玩家
		Args:
			player (Player): 玩家
		Returns:
			返回玩家对应的在线玩家，找不到返回 None
		"""
		if player == self.player1: player = self.player2
		if player == self.player2: player = self.player1

		return PlayerCommon.getOnlinePlayer(player.id)

	def getSubject(self) -> Subject:
		"""
		获取当前回合的科目
		Returns:
			返回当前回合科目
		"""
		return self.cur_round.question.subject

	# endregion

	# region 回合/对战操作

	def newRound(self):
		"""
		添加新回合
		"""
		self.round_num += 1
		self.cur_round = self.record.addRound()

		# 发送新回合数据
		self._emit(EmitType.NewRound, self._generateRoundData())

	def resetRoundStatus(self):
		"""
		重置对战玩家回合状态
		"""
		self.attacker = None
		self.clearTimer()

	def _generateRoundData(self) -> dict:
		"""
		生成当前回合数据
		Returns:
			返回当前回合的数据
		"""
		return {'round': self.record.currentRound().convert()}

	# endregion

	# endregion

	# region 请求处理

	def onMatchingProgress(self, player: Player, progress: int):
		"""
		匹配进度处理
		Args:
			player (Player): 玩家
			progress (int): 进度
		"""
		if self.status != BattleStatus.Matching: return

		data = self._generateProgressData(player, progress)

		battler = self.getBattler(player)
		battler.progress = progress

		self._emit(EmitType.MatchProgress, data)

		# self.updateTransition()

	def _generateProgressData(self, player: Player, progress: int) -> dict:
		"""
		生成进度数据
		Args:
			player (Player): 玩家
			progress (int): 进度
		Returns:
			返回玩家进度数据
		"""
		return {
			'pid': player.id, 'progress': progress
		}

	def completePrepare(self, player: Player, cont_item: BaseContItem = None):
		"""
		准备阶段完成回调
		Args:
			player (Player): 玩家
			cont_item (BaseContItem): 物品
		"""
		if self.status != BattleStatus.Preparing: return

		battler = self.getBattler(player)
		battler.prepareItem(cont_item)

	def answerQuestion(self, player: Player, selection: list, timespan: int):
		"""
		准备阶段完成回调
		Args:
			player (Player): 玩家
			selection (list): 选择情况
			timespan (int): 用时
		"""
		if self.status != BattleStatus.Questing: return

		oppo_battler = self.getOppoBattler(player)

		if oppo_battler.isCorrect(): return

		battler = self.getBattler(player)
		battler.setupRoundResult(selection, timespan)

		correct = battler.round_result.correct()

		self._emit(EmitType.QuesResult,
				   self._generateQuesResultData(player, correct, timespan))

	def _generateQuesResultData(self, player: Player, correct: bool, timespan: int) -> dict:
		"""
		生成答题结果数据
		Args:
			player (Player): 玩家
			correct (bool): 是否正确
			timespan (int): 用时（毫秒）
		Returns:
			返回转化后的数据
		"""
		return {
			'pid': player.id,
			'correct': correct,
			'timespan': timespan
		}

	def completeQuestion(self, player: Player):
		"""
		行动完成
		Args:
			player (Player): 玩家
		"""
		battler = self.getBattler(player)
		battler.question_completed = True

	def completeAction(self, player: Player):
		"""
		行动完成
		Args:
			player (Player): 玩家
		"""
		battler = self.getBattler(player)
		battler.action_completed = True

	def completeResult(self, player: Player):
		"""
		结算完成
		Args:
			player (Player): 玩家
		"""
		battler = self.getBattler(player)
		battler.result_completed = True

	# endregion

	# region 状态机

	def updateTransition(self):
		"""
		更新状态转换
		"""
		if self.status == BattleStatus.Matching:
			# 进度完成
			if self.isMatched(): self._startPreparing()

		elif self.status == BattleStatus.Preparing:
			# 准备完成
			if self.isPrepared(): self._startQuesting()

		elif self.status == BattleStatus.Questing:
			# 准备完成
			if self.isQuestionCompleted():
				corr_battler = self.getCorrectBattler()

				if corr_battler or self.isQuested():
					self._startActing(corr_battler)

		elif self.status == BattleStatus.Acting:
			if self.isActionCompleted(): self._startResulting()

		elif self.status == BattleStatus.Resulting:
			if self.isResultCompleted(): self._onRoundTerminated()

		elif self.status == BattleStatus.Terminating:
			self.status = BattleStatus.Terminated

	# region 状态完成判断

	def isMatched(self) -> bool:
		"""
		是否已经匹配并加载完成
		Returns:
			返回是否匹配加载完成
		"""
		return self.runtime_battler1.progress == \
			self.runtime_battler2.progress == 100

	def isPrepared(self) -> bool:
		"""
		是否已经准备完成
		Returns:
			返回是否准备完成
		"""
		if self.checkTimer(): return True
		return self.runtime_battler1.prepared and \
			self.runtime_battler2.prepared

	def isQuested(self) -> bool:
		"""
		是否双方均作答完毕
		Returns:
			返回是否作答完毕
		"""
		return self.runtime_battler1.round_result and \
			self.runtime_battler2.round_result

	def isQuestionCompleted(self) -> bool:
		"""
		题目答案是否查看完成
		Returns:
			返回题目结果是否查看完成
		"""
		if self.checkTimer(): return True
		return self.runtime_battler1.question_completed and \
			self.runtime_battler2.question_completed

	def getCorrectBattler(self) -> RuntimeBattlePlayer:
		"""
		获取正确方玩家
		Returns:
			返回正确作答方对战玩家实例，若没有返回 None
		"""
		if self.runtime_battler1.isCorrect():
			return self.runtime_battler1

		if self.runtime_battler2.isCorrect():
			return self.runtime_battler2

		return None

	def isActionCompleted(self) -> bool:
		"""
		是否行动完毕
		Returns:
			返回是否行动完毕
		"""
		if self.checkTimer(): return True
		return self.runtime_battler1.action_completed and \
			self.runtime_battler2.action_completed

	def isResultCompleted(self) -> bool:
		"""
		是否结算完毕
		Returns:
			返回是否结算完毕
		"""
		if self.checkTimer(): return True
		return self.runtime_battler1.result_completed and \
			self.runtime_battler2.result_completed

	def isBattleEnd(self) -> bool:
		"""
		是否对战结束
		Returns:
			返回是否对战结束
		"""
		return self.runtime_battler1.hp <= 0 or \
			self.runtime_battler2.hp <= 0

	def isTerminated(self) -> bool:
		"""
		对战是否结束
		Returns:
			返回对战是否结束
		"""
		return self.status == BattleStatus.Terminated

	# endregion

	# region 状态切换

	def _startMatching(self):
		"""
		进入匹配状态
		"""
		self.status = BattleStatus.Matching
		self.clearTimer()

	def _startPreparing(self):
		"""
		进入准备状态（同时开始新回合）
		"""
		self.status = BattleStatus.Preparing
		self.setTimer(self.PREPARE_TIME)
		self.newRound()

	def _startQuesting(self):
		"""
		准备状态改变回调（开始答题）
		"""
		from question_module.models import GeneralQuestion

		self._emit(EmitType.PrepareCompleted,
				   None, self.PREPARE_END_WAIT_TIME)

		self.status = BattleStatus.Questing
		# 开始答题
		self.record.startCurrentRound()

		question: GeneralQuestion = self.cur_round.question
		time = question.star.std_time
		self.setTimer(time*self.QUESTION_TIME_RATE)

	def _startActing(self, corr_battler: RuntimeBattlePlayer):
		"""
		答题状态改变回调
		Args:
			corr_battler (RuntimeBattlePlayer): 行动方
		"""
		self.__setupCurrentRound()

		if corr_battler is None:
			self._startResulting()
		else:
			self.attacker = corr_battler
			self.status = BattleStatus.Acting

			self._processAction()

			self.setTimer(self.MAX_ACTION_TIME)

	def _generateActionData(self) -> dict:
		"""
		生成行动数据
		Returns:
			返回转化后的数据
		"""
		actions1 = self.runtime_battler1.actions
		actions2 = self.runtime_battler2.actions

		return {
			'actions1': ModelUtils.objectsToDict(actions1), 
			'actions2': ModelUtils.objectsToDict(actions2)
		}

	def _startResulting(self):
		"""
		行动状态改变回调
		"""
		self._emit(EmitType.RoundResult, self._generateRoundResultData(),
				   self.ROUND_RESULT_WAIT_TIME)

		self.status = BattleStatus.Resulting
		self.setTimer(self.RESULT_TIME)

	def _generateRoundResultData(self) -> dict:
		"""
		生成回合结果数据
		Returns:
			返回回合结果数据
		"""
		result1 = self.runtime_battler1.round_result  # battler.currentRound()
		result2 = self.runtime_battler2.round_result  # battler.currentRound()

		if result1 is None: result1 = self.runtime_battler1.battle_player.currentRound()
		if result2 is None: result2 = self.runtime_battler2.battle_player.currentRound()

		return {
			'player1': result1.convert(
				type="result", runtime_battler=self.runtime_battler1),
			'player2': result2.convert(
				type="result", runtime_battler=self.runtime_battler2)
		}

	def _onRoundTerminated(self):
		"""
		回合结束回调
		"""
		self.resetRoundStatus()

		self.runtime_battler1.onRoundTerminated()
		self.runtime_battler2.onRoundTerminated()

		if self.isBattleEnd(): self._onBattleTerminated()
		else: self._startPreparing()

	def _onBattleTerminated(self):
		"""
		对战结束回调
		"""
		self.record.terminate(self)
		self._startTerminating()

	def _startTerminating(self):
		"""
		开始结束对战
		"""
		self._emit(EmitType.BattleResult, self._generateBattleResultData())

		self.status = BattleStatus.Terminating

	def _generateBattleResultData(self) -> dict:
		"""
		生成对战结果数据
		Returns:
			返回对战结果数据
		"""
		# result1 = self.runtime_battler1.battle_player
		# result2 = self.runtime_battler2.battle_player

		return {
			'record': self.record.convert(type="result")
			# 'player1': result1.convert(type="result"),
			# 'player2': result2.convert(type="result")
		}

	# endregion

	# endregion

	# region 行动控制

	def _processAction(self):
		"""
		执行对战行动
		"""
		self.__processPrepareAction()
		self.__processAttackAction()

		self._emit(EmitType.ActionStart, self._generateActionData(),
				   self.ACTION_START_WAIT_TIME)

	def __setupCurrentRound(self):
		"""
		配置当前回合（如果没配置）
		"""
		self.runtime_battler1.setupRoundResult()
		self.runtime_battler2.setupRoundResult()

	def __processPrepareAction(self):
		"""
		执行准备行动
		"""
		self.runtime_battler1.processItem()
		self.runtime_battler2.processItem()

	def __processAttackAction(self):
		"""
		执行攻击行动
		"""
		oppo = self.getOppoBattler(runtime_battler=self.attacker)
		# if oppo.round_result is None:
		# 	oppo.setupRoundResult()

		self.attacker.processAttack(oppo)

	# endregion

	""" 占位符 """


class RuntimeBattleManager:
	"""
	运行时对战管理类
	"""
	Battles: dict = None

	@classmethod
	def createBattle(cls, player1: MatchingPlayer, player2: MatchingPlayer):
		"""
		创建对战
		Args:
			player1 (MatchingPlayer): 匹配玩家1
			player2 (MatchingPlayer): 匹配玩家2
		"""
		battle = RuntimeBattle(player1.player, player2.player, player1.mode)

		RuntimeManager.add(RuntimeBattle, battle)

		RuntimeManager.delete(MatchingPlayer, data=player1)
		RuntimeManager.delete(MatchingPlayer, data=player2)

	@classmethod
	def loadBattles(cls):
		"""
		读取匹配队列
		"""
		cls.Battles = RuntimeManager.get(RuntimeBattle)

	@classmethod
	async def update(cls):
		"""
		更新所有对战
		"""
		if cls.Battles is None: cls.loadBattles()

		temp = cls.Battles.copy()

		for key in temp:
			battle: RuntimeBattle = temp[key]
			battle.updateTransition()

			if battle.isTerminated():
				RuntimeManager.delete(RuntimeBattle, key)


RuntimeManager.register(MatchingPlayer)
RuntimeManager.register(RuntimeBattlePlayer)
RuntimeManager.register(RuntimeBattle)

RuntimeManager.registerEvent(MatchingManager.process)
RuntimeManager.registerEvent(RuntimeBattleManager.update)
