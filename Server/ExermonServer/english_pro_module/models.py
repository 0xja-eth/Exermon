from django.db import models
from django.conf import settings

from .item_system.items import *

from game_module.models import GroupConfigure
from question_module.models import BaseQuestion, BaseQuesChoice, GroupQuestion

from utils.model_utils import QuestionAudioUpload, PlotQuestionImageUpload, Common as ModelUtils
from utils.exception import ErrorType, GameException

import os, base64, datetime, jsonfield, random
from enum import Enum

# region 配置


# ===================================================
#  反义词表
# ===================================================
class Antonym(GroupConfigure):
	class Meta:
		verbose_name = verbose_name_plural = "反义词"

	# 卡牌词
	card_word = models.CharField(max_length=32, verbose_name="卡牌词")

	# 敌人词
	enemy_word = models.CharField(max_length=32, verbose_name="敌人词")

	# 伤害比率（*100）
	hurt_rate = models.SmallIntegerField(default=100, verbose_name="伤害比率")

	def convert(self):
		"""
		转化为字典
		Returns:
			返回转化后的字典
		"""
		return {
			'card_word': self.card_word,
			'enemy_word': self.enemy_word,
			'hurt_rate': self.hurt_rate / 100,
		}


# ===================================================
#  初始卡组表
# ===================================================
class FirstCardGroup(GroupConfigure):

	class Meta:
		verbose_name = verbose_name_plural = "初始卡组"

	LIST_DISPLAY_APPEND = ['adminCards']

	# 卡组ID集
	cards = jsonfield.JSONField(default=[], verbose_name="卡组ID集")

	def convert(self):
		"""
		转化为字典
		Returns:
			返回转化后的字典
		"""
		res = super().convert()

		res["cards"] = self.cards

		return res

	def adminCards(self):
		res = ""
		for id in self.cards:
			res += ExerProCard.get(id=id).name + " "

		return res

	adminCards.short_description = "包含卡牌"


# endregion


# region 题目


# ===================================================
#  英语题目类型枚举
# ===================================================
class QuestionType(Enum):
	Listening = 1  # 听力题
	Phrase = 2  # 不定式题
	Correction = 3  # 改错题
	Plot = 4  # 剧情题


# ===================================================
#  听力题目选项表
# ===================================================
class ListeningQuesChoice(BaseQuesChoice):
	class Meta:
		verbose_name = verbose_name_plural = "听力题目选项"

	# 所属问题
	question = models.ForeignKey('ListeningSubQuestion', null=False, on_delete=models.CASCADE,
								 verbose_name="所属问题")


# ===================================================
#  听力小题
# ===================================================
class ListeningSubQuestion(BaseQuestion):
	class Meta:
		verbose_name = verbose_name_plural = "听力小题"

	# 听力题目
	question = models.ForeignKey('ListeningQuestion', on_delete=models.CASCADE,
								 verbose_name="听力题目")

	def choices(self):
		return self.listeningqueschoice_set.all()


# ===================================================
#  听力题
# ===================================================
class ListeningQuestion(GroupQuestion):
	class Meta:
		verbose_name = verbose_name_plural = "听力题"

	TYPE = QuestionType.Listening

	# 重复次数
	times = models.PositiveSmallIntegerField(default=2, verbose_name="重复次数")

	# 音频文件
	audio = models.FileField(upload_to=QuestionAudioUpload(), verbose_name="音频文件")

	# 获取完整路径
	def getExactlyPath(self):
		base = settings.STATIC_URL
		path = os.path.join(base, str(self.audio))
		if os.path.exists(path):
			return path
		else:
			raise GameException(ErrorType.PictureFileNotFound)

	# 获取视频base64编码
	def convertToBase64(self):

		with open(self.getExactlyPath(), 'rb') as f:
			data = base64.b64encode(f.read())

		return data.decode()

	def convert(self):
		res = super().convert()

		res['times'] = self.times
		res['audio'] = self.convertToBase64()

		return res

	def subQuestions(self) -> QuerySet:
		"""
		子题目
		Returns:
			返回该听力题目的子题目
		"""
		return self.listeningsubquestion_set.all()


# ===================================================
#  阅读题目选项表
# ===================================================
class ReadingQuesChoice(BaseQuesChoice):
	class Meta:
		verbose_name = verbose_name_plural = "阅读题目选项"

	# 所属问题
	question = models.ForeignKey('ReadingSubQuestion', null=False, on_delete=models.CASCADE,
								 verbose_name="所属问题")


# ===================================================
#  阅读小题
# ===================================================
class ReadingSubQuestion(BaseQuestion):
	class Meta:
		verbose_name = verbose_name_plural = "阅读小题"

	# 阅读题目
	question = models.ForeignKey('ReadingQuestion', on_delete=models.CASCADE,
								 verbose_name="阅读题目")

	def choices(self):
		return self.readingqueschoice_set.all()


# ===================================================
#  阅读题
# ===================================================
class ReadingQuestion(GroupQuestion):
	class Meta:
		verbose_name = verbose_name_plural = "阅读题"

	def subQuestions(self) -> QuerySet:
		"""
		子题目
		Returns:
			返回该听力题目的子题目
		"""
		return self.readingsubquestion_set.all()


# ===================================================
#  剧情题目
# ===================================================
class PlotQuestion(BaseQuestion):
	class Meta:
		verbose_name = verbose_name_plural = "剧情题目"

	TYPE = QuestionType.Plot

	# 剧情事件名称
	event_name = models.CharField(max_length=64, verbose_name="剧情事件名称")

	# 剧情图标
	picture = models.ImageField(null=True, blank=True,
								upload_to=PlotQuestionImageUpload(), verbose_name="剧情图标")

	def __str__(self):
		return self.picture.url

	# 获取剧情图片完整路径
	def getExactlyPath(self):
		base = settings.STATIC_URL
		path = os.path.join(base, str(self.picture))
		if os.path.exists(path):
			return path
		else:
			raise GameException(ErrorType.PictureFileNotFound)

	# 获取base64编码
	def convertToBase64(self):

		with open(self.getExactlyPath(), 'rb') as f:
			data = base64.b64encode(f.read())

		return data.decode()

	def choices(self):
		return self.plotqueschoice_set.all()

	def convert(self, type=None):
		res = super().convert(type)

		res['event_name'] = self.event_name
		res['picture'] = self.convertToBase64()

		return res


# ===================================================
#  剧情题目选项表
# ===================================================
class PlotQuesChoice(BaseQuesChoice):
	class Meta:
		verbose_name = verbose_name_plural = "剧情题目选项"

	# 所需金币
	gold = models.PositiveSmallIntegerField(default=0, verbose_name="所需金币")

	# 选项对应的结果文本
	result_text = models.TextField(verbose_name="选项对应的结果文本")

	# 所属问题
	question = models.ForeignKey('PlotQuestion', null=False, on_delete=models.CASCADE,
								 verbose_name="所属问题")

	def effects(self):
		return self.exerproploteffect_set.all()

	def convert(self):
		res = super().convert()

		plot_effects = ModelUtils.objectsToDict(self.effects())

		res['gold'] = self.gold
		res['result_text'] = self.result_text
		res['effects'] = plot_effects

		return res


# ===================================================
#  剧情题目效果表
# ===================================================
class ExerProPlotEffect(ExerProEffect):
	class Meta:
		verbose_name = verbose_name_plural = "剧情题目效果"

	# 选项
	choice = models.ForeignKey('PlotQuesChoice', on_delete=models.CASCADE,
							 verbose_name="选项")


# ===================================================
#  短语题目类型枚举
# ===================================================
class PhraseType(Enum):
	SB = 1  # [sb. sth. 开头的短语选项]
	Do = 2  # [to do, doing 开头的短语选项]
	Prep = 3  # [介词短语选项]


# ===================================================
#  短语题
# ===================================================
class PhraseQuestion(models.Model):
	class Meta:
		verbose_name = verbose_name_plural = "短语题"

	TYPES = [
		(PhraseType.SB.value, '[包含 sb. 的短语选项]'),
		(PhraseType.Do.value, '[do 形式的短语选项]'),
		(PhraseType.Prep.value, '[介词短语选项]'),
	]

	TYPE = QuestionType.Phrase

	# 单词
	word = models.CharField(max_length=64, verbose_name="单词")

	# 中文翻译
	chinese = models.CharField(max_length=64, verbose_name="中文")

	# 不定式项
	phrase = models.CharField(max_length=64, verbose_name="不定式项")

	# 不定式项的类型
	type = models.PositiveSmallIntegerField(default=PhraseType.Do.value,
											choices=TYPES, verbose_name="修改类型")

	def convert(self):
		"""
		转化为字典
		Returns:
			返回转化后的字典
		"""
		return {
			'id': self.id,
			'word': self.word,
			'chinese': self.chinese,
			'phrase': self.phrase,
			'type': self.type
		}


# ===================================================
#  改错题
# ===================================================
class CorrectionQuestion(models.Model):
	class Meta:
		verbose_name = verbose_name_plural = "改错题"

	TYPE = QuestionType.Correction

	# 文章
	article = models.TextField(verbose_name="文章")

	# 解析
	description = models.TextField(null=True, blank=True, verbose_name="解析")

	def convert(self):
		wrong_items = ModelUtils.objectsToDict(self.wrongItems())

		return {
			'id': self.id,
			'article': self.article,
			'description': self.description,

			'wrong_items': wrong_items
		}

	def wrongItems(self) -> QuerySet:
		"""
		错误项
		Returns:
			返回该改错题目的错误项
		"""
		return self.wrongitem_set.all()


# ===================================================
#  纠正类型
# ===================================================
class CorrectType(Enum):
	Add = 1  # 增加
	Edit = 2  # 修改
	Delete = 3  # 删除


# ===================================================
#  改错题错误项
# ===================================================
class WrongItem(models.Model):
	class Meta:
		verbose_name = verbose_name_plural = "改错题错误项"

	TYPES = [
		(CorrectType.Add.value, '增加'),
		(CorrectType.Edit.value, '修改'),
		(CorrectType.Delete.value, '删除'),
	]

	# 句子编号
	sentence_index = models.PositiveSmallIntegerField(verbose_name="句子编号")

	# 单词编号
	word_index = models.PositiveSmallIntegerField(verbose_name="单词编号")

	# 修改类型
	type = models.PositiveSmallIntegerField(default=CorrectType.Edit.value,
											choices=TYPES, verbose_name="修改类型")

	# 正确单词
	word = models.TextField(verbose_name="正确单词", null=True, blank=True)

	# 对应题目
	question = models.ForeignKey('CorrectionQuestion', on_delete=models.CASCADE,
								 verbose_name="改错题目")

	def convert(self):
		return {
			'id': self.id,
			'sentence_index': self.sentence_index,
			'word_index': self.word_index,
			'type': self.type,
			'word': self.word,
		}


# ===================================================
#  单词
# ===================================================
class Word(models.Model):
	class Meta:
		verbose_name = verbose_name_plural = "单词"

	# 英文
	english = models.CharField(unique=True, max_length=64, verbose_name="英文")

	# 中文
	chinese = models.CharField(max_length=256, verbose_name="中文")

	# 词性
	type = models.CharField(max_length=64, verbose_name="词性", null=True, blank=True)

	# 等级
	level = models.PositiveSmallIntegerField(default=1, verbose_name="等级")

	# 是否初中题目
	is_middle = models.BooleanField(default=True, verbose_name="是否初中题目")

	# 是否高中题目
	is_high = models.BooleanField(default=True, verbose_name="是否高中题目")

	def __str__(self):
		return "%d. %s" % (self.id, self.english)

	def convert(self):
		return {
			'id': self.id,
			'english': self.english,
			'chinese': self.chinese,
			'type': self.type,
			'level': self.level,

			'is_middle': self.is_middle,
			'is_high': self.is_high,
		}


# ===================================================
#  单词记录表
# ===================================================
class WordRecord(models.Model):
	class Meta:

		verbose_name = verbose_name_plural = "单词记录"

	LIST_EDITABLE_EXCLUDE = ['word', 'record', 'count']

	# 单词
	word = models.ForeignKey('Word', on_delete=models.CASCADE, verbose_name="单词")

	# 对应的特训记录
	record = models.ForeignKey('ExerProRecord', on_delete=models.CASCADE, verbose_name="特训记录")

	# 回答次数
	count = models.PositiveSmallIntegerField(default=0, verbose_name="回答次数")

	# 正确次数
	correct = models.PositiveSmallIntegerField(default=0, verbose_name="正确数")

	# 上次回答日期
	last_date = models.DateTimeField(null=True, verbose_name="上次回答日期")

	# 初次回答日期
	first_date = models.DateTimeField(null=True, verbose_name="初次回答日期")

	# 收藏标志
	collected = models.BooleanField(default=False, verbose_name="收藏标志")

	# 错题标志
	wrong = models.BooleanField(default=False, verbose_name="错题标志")

	# 当前轮单词
	current = models.BooleanField(default=False, verbose_name="是否是当前轮")

	# 当前轮是否答对
	current_correct = models.BooleanField(default=None, null=True, verbose_name="当前轮是否答对")

	# 转化为字符串
	def __str__(self):
		return '%s (%s)' % (self.word, self.record)

	# 转化为字典
	def convert(self, type=None):

		last_date = ModelUtils.timeToStr(self.last_date)
		first_date = ModelUtils.timeToStr(self.first_date)

		return {
			'id': self.id,
			'word_id': self.word_id,
			'count': self.count,
			'correct': self.correct,
			'first_date': first_date,
			'last_date': last_date,

			'collected': self.collected,
			'wrong': self.wrong,

			'current': self.current,
			'current_correct': self.current_correct,
			# 由于前段无法判断 None，需要返回一个附加的字段
			'current_done': self.current_correct is not None,
		}

	# 创建新记录
	@classmethod
	def create(cls, pro_record, word_id):
		record = pro_record.wordRecord(word_id)

		if record is None:
			record = cls()
			record.record = pro_record
			record.word_id = word_id

		record.current = True
		record.save()

		return record

	def updateRecord(self, correct):
		"""
		更新已有记录
		Args:
			correct (bool): 是否正确
		"""
		self.current_correct = correct

		if correct:
			self.correct += 1
		else:
			self.wrong = True

		if self.count <= 0:
			self.first_date = datetime.datetime.now()

		self.last_date = datetime.datetime.now()
		self.count += 1

		self.save()

	def answer(self, chinese):
		"""
		单词作答
		Args:
			chinese (str): 中文
		Returns::
			返回答案是否正确
		"""
		correct = chinese == self.word.chinese
		self.updateRecord(correct)

		return correct

	# 正确率
	def corrRate(self):
		if self.count is None or self.count == 0:
			return 0
		return self.correct / self.count


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
# class EnglishWordSource(models.Model):
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
class NodeType(GroupConfigure):
	class Meta:
		verbose_name = verbose_name_plural = "据点类型"

	# 题型
	ques_types = models.CharField(max_length=32, verbose_name="题型")

	def convert(self):
		"""
		转化为字典
		Returns:
			返回转化后的字典
		"""
		res = super().convert()

		res['ques_types'] = self.ques_types

		return res


# ===================================================
#  地图表
# ===================================================
class ExerProMap(models.Model):
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

	def convert(self):
		"""
		转化为字典
		Returns:
			返回转化后的字典
		"""
		stages = ModelUtils.objectsToDict(self.stages())

		return {
			'id': self.id,
			'name': self.name,
			'description': self.description,
			'level': self.level,
			'min_level': self.min_level,

			'stages': stages
		}

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
class ExerProMapStage(models.Model):
	class Meta:
		verbose_name = verbose_name_plural = "地图关卡"

	LIST_EDITABLE_EXCLUDE = ['map']

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

	def convert(self):
		"""
		转化为字典
		Returns:
			返回转化后的字典
		"""
		enemies = list(e.id for e in self.enemies.all())

		return {
			'order': self.order,
			'max_battle_enemies': self.max_battle_enemies,
			'steps': self.steps,
			'max_fork_node': self.max_fork_node,
			'max_fork': self.max_fork,
			'node_rate': self.node_rate,

			'enemies': enemies,
		}


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

	def convert(self, type: str = None, **kwargs):
		"""
		转化为字典
		Args:
			type (str): 类型
			**kwargs (**dict): 拓展参数
		Returns:
			返回转化后的字典
		"""

		if type == "records":
			return ModelUtils.objectsToDict(self.wordRecords())

		word_records = self.currentWordRecords()
		words = [record.word for record in word_records]

		if type == "words":
			return {
				'word_level': self.word_level,
				'words': ModelUtils.objectsToDict(words),
				'word_records': ModelUtils.objectsToDict(word_records),
			}

		# if type == "status":
		# 	records = self.currentWordRecords()
		#
		# 	corr_recs = [record for record in records
		# 				 if record.current_correct is True]
		# 	wrong_recs = [record for record in records
		# 				  if record.current_correct is False]
		#
		# 	sum = len(records)
		# 	correct = len(corr_recs)
		# 	wrong = len(wrong_recs)
		#
		# 	return {
		# 		'level': self.word_level,
		# 		'sum': sum,
		# 		'correct': correct,
		# 		'wrong': wrong
		# 	}

		cur_index = self.cur_index
		if cur_index is None: cur_index = -1

		return {
			'id': self.id,
			'map_id': self.stage.map_id,
			'stage_order': self.stage.order,
			'started': self.started,
			'generated': self.generated,
			'cur_index': cur_index,
			'word_level': self.word_level,

			'gold': self.gold,
			'nodes': self.nodes,
			'actor': self.actor,

			'words': ModelUtils.objectsToDict(words),
			'word_records': ModelUtils.objectsToDict(word_records),
		}

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
# class ExerProScore(models.Model):
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
