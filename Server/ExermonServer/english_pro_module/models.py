from django.db import models
from django.conf import settings

from .item_system.items import *

from game_module.models import StaticData
from question_module.models import WordRecord

from utils.model_utils import BaseModel, Common as ModelUtils

import jsonfield, random

# region 配置


# ===================================================
#  反义词表
# ===================================================
class Antonym(GameData):
	class Meta:
		verbose_name = verbose_name_plural = "反义词"

	# 卡牌词
	card_word = models.CharField(max_length=32, verbose_name="卡牌词")

	# 敌人词
	enemy_word = models.CharField(max_length=32, verbose_name="敌人词")

	# 伤害比率（*100）
	hurt_rate = models.SmallIntegerField(default=100, verbose_name="伤害比率")


# ===================================================
#  初始卡组表
# ===================================================
class FirstCardGroup(StaticData):

	class Meta:
		verbose_name = verbose_name_plural = "初始卡组"

	LIST_DISPLAY_APPEND = ['adminCards']

	# 卡组ID集
	cards = jsonfield.JSONField(default=[], verbose_name="卡组ID集")

	def adminCards(self):
		res = ""
		for id in self.cards:
			res += ExerProCard.get(id=id).name + " "

		return res

	adminCards.short_description = "包含卡牌"


# endregion


# region 题目

#
# # ===================================================
# #  英语题目类型枚举
# # ===================================================
# class QuestionType(Enum):
# 	Listening = 1  # 听力题
# 	Phrase = 2  # 不定式题
# 	Correction = 3  # 改错题
# 	Plot = 4  # 剧情题
#
#
# # ===================================================
# #  听力题目选项表
# # ===================================================
# class ListeningQuesChoice(BaseQuesChoice):
# 	class Meta:
# 		verbose_name = verbose_name_plural = "听力题目选项"
#
# 	# 所属问题
# 	question = models.ForeignKey('ListeningSubQuestion', null=False, on_delete=models.CASCADE,
# 								 verbose_name="所属问题")
#
#
# # ===================================================
# #  听力小题
# # ===================================================
# class ListeningSubQuestion(SelectingQuestion):
# 	class Meta:
# 		verbose_name = verbose_name_plural = "听力小题"
#
# 	# 听力题目
# 	question = models.ForeignKey('ListeningQuestion', on_delete=models.CASCADE,
# 								 verbose_name="听力题目")
#
# 	def choices(self):
# 		return self.listeningqueschoice_set.all()
#
#
# # ===================================================
# #  听力题
# # ===================================================
# class ListeningQuestion(GroupQuestion):
# 	class Meta:
# 		verbose_name = verbose_name_plural = "听力题"
#
# 	TYPE = QuestionType.Listening
#
# 	# 重复次数
# 	times = models.PositiveSmallIntegerField(default=2, verbose_name="重复次数")
#
# 	# 音频文件
# 	audio = models.FileField(upload_to=QuestionAudioUpload(), verbose_name="音频文件")
#
# 	# 获取完整路径
# 	def getExactlyPath(self):
# 		base = settings.STATIC_URL
# 		path = os.path.join(base, str(self.audio))
# 		if os.path.exists(path):
# 			return path
# 		else:
# 			raise GameException(ErrorType.PictureFileNotFound)
#
# 	# 获取视频base64编码
# 	def convertToBase64(self):
#
# 		with open(self.getExactlyPath(), 'rb') as f:
# 			data = base64.b64encode(f.read())
#
# 		return data.decode()
#
# 	def convert(self):
# 		res = super().convert()
#
# 		res['times'] = self.times
# 		res['audio'] = self.convertToBase64()
#
# 		return res
#
# 	def subQuestions(self) -> QuerySet:
# 		"""
# 		子题目
# 		Returns:
# 			返回该听力题目的子题目
# 		"""
# 		return self.listeningsubquestion_set.all()
#
#
# # ===================================================
# #  阅读题目选项表
# # ===================================================
# class ReadingQuesChoice(BaseQuesChoice):
# 	class Meta:
# 		verbose_name = verbose_name_plural = "阅读题目选项"
#
# 	# 所属问题
# 	question = models.ForeignKey('ReadingSubQuestion', null=False, on_delete=models.CASCADE,
# 								 verbose_name="所属问题")
#
#
# # ===================================================
# #  阅读小题
# # ===================================================
# class ReadingSubQuestion(SelectingQuestion):
# 	class Meta:
# 		verbose_name = verbose_name_plural = "阅读小题"
#
# 	# 阅读题目
# 	question = models.ForeignKey('ReadingQuestion', on_delete=models.CASCADE,
# 								 verbose_name="阅读题目")
#
# 	def choices(self):
# 		return self.readingqueschoice_set.all()
#
#
# # ===================================================
# #  阅读题
# # ===================================================
# class ReadingQuestion(GroupQuestion):
# 	class Meta:
# 		verbose_name = verbose_name_plural = "阅读题"
#
# 	def subQuestions(self) -> QuerySet:
# 		"""
# 		子题目
# 		Returns:
# 			返回该听力题目的子题目
# 		"""
# 		return self.readingsubquestion_set.all()


# endregion

# # ===================================================
# #  英语单词来源枚举
# # ===================================================
# class EnglishWordSourceType(Enum):
#
# 	MiddleSchool = 1  # 初中
# 	HighSchool = 2  # 高中
# 	CET4 = 3  # 四级
# 	CET6 = 4  # 六级
# 	Postgraduate = 5  # 考研
#
# 	Unknown = 0  # 未知
#
#
# # ===================================================
# #  英语单词来源表
# # ===================================================
# class EnglishWordSource(BaseModel):
#
# 	class Meta:
# 		verbose_name = verbose_name_plural = "英语单词来源"
#
# 	TYPES = [
# 		(EnglishWordSourceType.MiddleSchool.value, '初中'),
# 		(EnglishWordSourceType.HighSchool.value, '高中'),
# 		(EnglishWordSourceType.CET4.value, '四级'),
# 		(EnglishWordSourceType.CET6.value, '六级'),
# 		(EnglishWordSourceType.Postgraduate.value, '考研'),
#
# 		(EnglishWordSourceType.Unknown.value, '未知'),
# 	]
#
# 	# 来源
# 	source = models.PositiveSmallIntegerField(default=EnglishWordSourceType.Unknown.value,
# 											choices=TYPES, verbose_name="来源")
#
# 	# 单词
# 	word = models.ForeignKey('EnglishWord', on_delete=models.CASCADE, verbose_name="单词")
#
# 	def convert(self):
# 		return self.source
#

# endregion


# region 地图


# ===================================================
#  据点类型表
# ===================================================
class NodeType(StaticData):
	class Meta:
		verbose_name = verbose_name_plural = "据点类型"

	# 题型
	ques_types = models.CharField(max_length=32, verbose_name="题型")


# ===================================================
#  地图表
# ===================================================
class ExerProMap(BaseModel):
	class Meta:
		verbose_name = verbose_name_plural = "特训地图"

	# 地图名称
	name = models.CharField(max_length=24, verbose_name="地图名称")

	# 故事描述
	description = models.CharField(max_length=512, verbose_name="故事描述")

	# 地图难度
	level = models.PositiveSmallIntegerField(default=1, verbose_name="地图难度")

	# 等级要求
	min_level = models.PositiveSmallIntegerField(default=1, verbose_name="等级要求")

	def __str__(self):
		return "%d. %s" % (self.id, self.name)

	def _convertCustomAttrs(self, res, type=None, **kwargs):
		super()._convertCustomAttrs(res, type, **kwargs)

		stages = ModelUtils.objectsToDict(self.stages())

		res['stages'] = stages

	@CacheHelper.staticCache
	def stages(self):
		"""
		获取所有关卡
		Returns:
			返回关卡 QuerySet
		"""
		return self.exerpromapstage_set.all()

	def stage(self, order) -> 'ExerProMapStage':
		"""
		获取指定序号的关卡
		Args:
			order (int): 关卡序号
		Returns:
			返回指定序号的关卡对象
		"""
		stage = self.stages().filter(order=order)

		if stage.exists(): return stage.first()

		return None


# # ===================================================
# #  据点类型表
# # ===================================================
# class NodeType(Enum):
# 	Rest = 0  # 休息据点
# 	Treasure = 1  # 藏宝据点
# 	Shop = 2  # 商人据点
# 	Enemy = 3  # 敌人据点
# 	Elite = 4  # 精英据点
# 	Unknown = 5  # 未知据点
# 	Boss = 6  # 精英据点


# ===================================================
#  地图关卡表
# ===================================================
class ExerProMapStage(BaseModel):
	class Meta:
		verbose_name = verbose_name_plural = "地图关卡"

	LIST_EDITABLE_EXCLUDE = ['map']

	DO_NOT_AUTO_CONVERT_FIELDS = ['enemies']

	# 序号
	order = models.PositiveSmallIntegerField(default=1, verbose_name="序号")

	# 地图
	map = models.ForeignKey("english_pro_module.ExerProMap", on_delete=models.CASCADE, verbose_name="地图")

	# 敌人集合（本阶段会刷的敌人）
	enemies = models.ManyToManyField("ExerProEnemy", verbose_name="敌人集合")

	# 战斗最大敌人数量
	max_battle_enemies = models.PositiveSmallIntegerField(default=1, verbose_name="战斗最大敌人数量")

	# 每步据点个数（最后一个需为1）
	steps = jsonfield.JSONField(default=[3, 4, 5, 2, 1], verbose_name="每步据点个数")

	# 最大分叉据点数量
	max_fork_node = models.PositiveSmallIntegerField(default=5, verbose_name="最大分叉据点数量")

	# 最大分叉选择数
	max_fork = models.PositiveSmallIntegerField(default=3, verbose_name="最大分叉选择数")

	# 据点比例（一共6种据点，按照该比例的几率进行生成，实际不一定为该比例）
	node_rate = jsonfield.JSONField(default=[1, 1, 1, 1, 1, 1], verbose_name="据点比例")

	def __str__(self):
		return "%s 第 %s 关" % (self.map, self.order)

	def _convertCustomAttrs(self, res, type=None, **kwargs):
		super()._convertCustomAttrs(res, type, **kwargs)

		enemies = list(e.id for e in self.enemies.all())

		res['enemies'] = enemies


# ===================================================
#  特训记录表
# ===================================================
class ExerProRecord(CacheableModel):
	class Meta:

		verbose_name = verbose_name_plural = "特训记录"

	LIST_EDITABLE_EXCLUDE = ['player']

	# 当前单词缓存键
	CUR_WORDS_CACHE_KEY = 'cur_words'

	# 默认金币
	DEFAULT_GOLD = 100

	# 关卡
	stage = models.ForeignKey('ExerProMapStage', null=True,
							  on_delete=models.CASCADE, verbose_name="关卡")

	# 开始标志
	started = models.BooleanField(default=False, verbose_name="开始标志")

	# 生成标志
	generated = models.BooleanField(default=False, verbose_name="生成标志")

	# 当前据点索引
	cur_index = models.PositiveSmallIntegerField(default=None, null=True, verbose_name="当前据点索引")

	# 是否完成据点事件
	node_flag = models.BooleanField(default=False, verbose_name="是否完成据点事件")

	# 单词等级（同时也是玩家在英语模块的等级）
	word_level = models.PositiveSmallIntegerField(default=1, verbose_name="单词等级")

	# # 下一单词
	# next = models.ForeignKey('Word', null=True, blank=True,
	# 						 on_delete=models.CASCADE, verbose_name="下一单词")

	# 金币
	gold = models.PositiveSmallIntegerField(default=DEFAULT_GOLD, verbose_name="金币")

	# 据点数据
	nodes = jsonfield.JSONField(default=None, null=True, blank=True,
								verbose_name="据点数据")

	# 角色数据
	actor = jsonfield.JSONField(default=None, null=True, blank=True,
								verbose_name="角色数据")

	# 玩家
	player = models.OneToOneField('player_module.Player', null=False,
								  on_delete=models.CASCADE, verbose_name="玩家")

	def __str__(self):
		return "%d. %s 特训记录" % (self.id, self.player)

	def __init__(self, *args, **kwargs):
		super().__init__(*args, **kwargs)
		self._setupCachePool()

	# 创建新记录
	@classmethod
	def create(cls, player):
		"""
		创建特训记录
		Args:
			player (Player): 顽疾
		"""
		record = cls()
		record.player = player
		record.save()

		record._generateWordRecords()

		return record

	def _convertCustomAttrs(self, res, type=None, **kwargs):
		super()._convertCustomAttrs(res, type, **kwargs)

		if type is None:
			res['map_id'] = self.stage.map_id
			res['order'] = self.stage.order

		if type is None or type == 'records':
			word_records = self.currentWordRecords()
			words = [record.word for record in word_records]

			res['words'] = ModelUtils.objectsToDict(words)
			res['word_records'] = ModelUtils.objectsToDict(word_records)

	def _convertRecordsData(self, res, **kwargs):

		res['word_records'] = ModelUtils.objectsToDict(self.wordRecords())

	def _convertWordsData(self, res, **kwargs):

		res['word_level'] = self.word_level

	def loadFromDict(self, data: dict):
		"""
		从字典中读取
		Args:
			data (dict): 字典
		"""
		from .views import Common

		map_id = ModelUtils.loadKey(data, 'map_id')
		stage_order = ModelUtils.loadKey(data, 'stage_order')

		self.stage = Common.getMapStage(mid=map_id, order=stage_order)

		ModelUtils.loadKey(data, 'started', self)
		ModelUtils.loadKey(data, 'generated', self)
		ModelUtils.loadKey(data, 'cur_index', self)
		ModelUtils.loadKey(data, 'node_flag', self)
		ModelUtils.loadKey(data, 'word_level', self)
		ModelUtils.loadKey(data, 'gold', self)
		ModelUtils.loadKey(data, 'nodes', self)
		ModelUtils.loadKey(data, 'actor', self)

	# region 流程控制

	def setupMap(self, map: 'ExerProMap'):
		"""
		设置地图
		Args:
			map (ExerProMap): 地图
		"""
		self.reset()
		self.stage = map.stage(1)
		self.started = True

	def gainGold(self, val):
		"""
		获得金币
		Args:
			val (int): 金币
		"""
		self.gold = max(0, self.gold + val)

	def reset(self):
		"""
		重置，重置特训记录状态
		"""
		self.started = self.generated = False
		self.cur_index = self.nodes = self.actor = None

	def upgrade(self):
		"""
		升级单词
		"""
		# 初始状态，为 None
		if self.word_level is None:
			self.word_level = 1
		else:
			self.word_level += 1

		self._generateWordRecords()
		self.save()

	def _generateWordRecords(self):
		"""
		生成单词和记录
		"""
		word_recs = self.wordRecords()

		wids = self._generateWords()
		self.clearCurrentWords()

		for wid in wids:
			word_rec = WordRecord.create(self, wid)
			word_recs.append(word_rec)

	def _generateWords(self) -> list:
		"""
		生成单词
		Returns:
			返回生成单词ID数组
		"""
		from utils.calc_utils import NewWordsGenerator

		# 获取旧单词的ID数组
		old_words = self.currentWordRecords()
		old_words = [record.word_id for record in old_words]

		return NewWordsGenerator.generate(self.word_level, old_words)

	def terminate(self):
		"""
		结束特训
		"""
		self.started = self.generated = False
		self.save()

	# endregion

	# region 单词记录管理

	def isFinished(self):
		"""
		本轮单词是否完成
		Returns:
			返回本轮单词是否完成
		"""
		if self.word_level is None: return True

		word_recs = self.currentWordRecords()

		for word_rec in word_recs:
			if not word_rec.current_correct: return False

		return True

	def nextWord(self) -> WordRecord:
		"""
		生成下一个单词
		Returns:
			返回下一个单词记录
		"""
		word_recs = self.currentWordRecords()
		word_recs = [word_rec for word_rec in word_recs
					 if not word_rec.current_correct]

		if len(word_recs) <= 0: return None

		return random.choice(word_recs)

	def clearCurrentWords(self):
		"""
		清除当前单词
		"""
		word_recs = self.currentWordRecords()

		for word_rec in word_recs:
			word_rec.current = False
			word_rec.current_correct = None

	def wordRecords(self):
		"""
		全部单词记录（缓存）
		Returns:
			返回缓存的全部单词记录列表
		"""
		return self._getOrSetCache(self.CUR_WORDS_CACHE_KEY,
								   lambda: list(self._wordRecords()))

	def _wordRecords(self):
		"""
		全部单词记录（数据库）
		Returns:
			返回全部单词记录列表
		"""
		return self.wordrecord_set.all()

	def currentWordRecords(self):
		"""
		当前单词记录
		Returns:
			返回当前单词记录列表
		"""
		return ModelUtils.query(self.wordRecords(), current=True)

	def wordRecord(self, word_id: int, **kwargs) -> 'WordRecord':
		"""
		通过单词ID查找单词记录
		Args:
			word_id (int): 单词ID
			**kwargs (**dict): 其他查询条件
		Returns:
			若存在单词记录，返回之，否则返回 None
		"""
		return ModelUtils.get(self.wordRecords(), word_id=word_id, **kwargs)


# TODO: 排行榜
# # ===================================================
# #  特训积分表
# # ===================================================
# class ExerProScore(BaseModel):
# 	class Meta:
#
# 		verbose_name = verbose_name_plural = "特训积分"
#
# 	# 关卡
# 	stage = models.ForeignKey('ExerProMapStage', null=True,
# 							  on_delete=models.CASCADE, verbose_name="关卡")
#
# 	# 开始标志
# 	started = models.BooleanField(default=False, verbose_name="开始标志")
#
# 	# 生成标志
# 	generated = models.BooleanField(default=False, verbose_name="生成标志")



# endregion
