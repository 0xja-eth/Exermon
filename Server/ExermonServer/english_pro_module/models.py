#-*-coding:GBK -*-

from django.db import models
from django.conf import settings
from item_module.models import *
from question_module.models import BaseQuestion, BaseQuesChoice, GroupQuestion
from utils.model_utils import QuestionAudioUpload, Common as ModelUtils
from utils.exception import ErrorType, GameException
import os, base64, datetime, jsonfield
from enum import Enum

# Create your models here.

# region 题目


# ===================================================
#  题目选项表
# ===================================================
class ListeningQuesChoice(BaseQuesChoice):
	class Meta:
		verbose_name = verbose_name_plural = "听力题目选项"

	# 所属问题
	question = models.ForeignKey('ListeningSubQuestion', null=False, on_delete=models.CASCADE,
								 verbose_name="所属问题")


# ===================================================
#  听力题
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

	def convertToDict(self):
		res = super().convertToDict()

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
#  题目选项表
# ===================================================
class ReadingQuesChoice(BaseQuesChoice):
	class Meta:
		verbose_name = verbose_name_plural = "阅读题目选项"

	# 所属问题
	question = models.ForeignKey('ReadingSubQuestion', null=False, on_delete=models.CASCADE,
								 verbose_name="所属问题")


# ===================================================
#  听力题
# ===================================================
class ReadingSubQuestion(BaseQuestion):

	class Meta:
		verbose_name = verbose_name_plural = "阅读小题"

	# 听力题目
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
#  改错题
# ===================================================
class CorrectionQuestion(models.Model):

	class Meta:
		verbose_name = verbose_name_plural = "改错题"

	# 文章
	article = models.TextField(verbose_name="文章")

	# 解析
	description = models.TextField(null=True, blank=True, verbose_name="解析")

	def convertToDict(self):
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
	word = models.TextField(verbose_name="正确单词")

	# 对应题目
	question = models.ForeignKey('CorrectionQuestion', on_delete=models.CASCADE,
								 verbose_name="改错题目")

	def convertToDict(self):
		return {
			'id': self.id,
			'sentence_index': self.sentence_index,
			'word_index': self.word_index,
			'type': self.type,
			'word': self.word,
			'question': self.question,
		}


# ===================================================
#  单词
# ===================================================
class Word(models.Model):

	class Meta:
		verbose_name = verbose_name_plural = "单词"

	# 英文
	english = models.CharField(max_length=64, verbose_name="英文")

	# 中文
	chinese = models.CharField(max_length=64, verbose_name="中文")

	# 词性
	type = models.CharField(max_length=32, verbose_name="词性")

	# 等级
	level = models.PositiveSmallIntegerField(default=1, verbose_name="等级")

	# 是否初中题目
	is_middle = models.BooleanField(default=True, verbose_name="是否初中题目")

	# 是否高中题目
	is_high = models.BooleanField(default=True, verbose_name="是否高中题目")

	def __str__(self):
		return "%d. %s" % (self.id, self.english)

	def convertToDict(self):
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

	# 单词
	word = models.ForeignKey('Word', null=False,
							 on_delete=models.CASCADE, verbose_name="单词")

	# 玩家
	player = models.ForeignKey('player_module.Player', null=False,
							   on_delete=models.CASCADE, verbose_name="玩家")

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

	# 转化为字符串
	def __str__(self):
		return '%s (%s)' % (self.word, self.player)

	# 转化为字典
	def convertToDict(self, type=None):

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
		}

	# 创建新记录
	@classmethod
	def create(cls, player, word_id):
		record = player.wordRecord(word_id)

		if record is None:
			record = cls()
			record.player = player
			record.word_id = word_id
			record.save()

		return record

	def updateRecord(self, correct):
		"""
		更新已有记录
		Args:
			correct (bool): 是否正确
		"""

		if correct: self.correct += 1
		else: self.wrong = True

		if self.count <= 0:
			self.first_date = datetime.datetime.now()

		self.last_date = datetime.datetime.now()
		self.count += 1

		self.save()

	# 正确率
	def corrRate(self):
		if self.count is None or self.count == 0:
			return 0
		return self.correct / self.count


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
# 	def convertToDict(self):
# 		return self.source
#

# endregion

# region 物品


# ===================================================
#  使用效果编号枚举
# ===================================================
class ExerProEffectCode(Enum):
	Unset = 0  # 空

	Attack = 1  # 造成伤害
	AttackSlash = 2  # 造成伤害（完美斩击）
	AttackBlack = 3  # 造成伤害（黑旋风）
	AttackWave = 4  # 造成伤害（波动拳）
	AttackRite = 5  # 造成伤害（仪式匕首）

	Recover = 100  # 回复体力值

	AddParam = 200  # 增加能力值
	AddParamUrgent = 201  # 增加能力值（紧急按钮）
	TempAddParam = 210  # 临时增加能力值
	AddStatus = 220  # 增加状态

	GetCards = 300  # 抽取卡牌
	RemoveCards = 310  # 移除卡牌

	ChangeCost = 400  # 更改耗能
	ChangeCostDisc = 401  # 更改耗能（发现）
	ChangeCostCrazy = 402  # 更改耗能（疯狂）

	Sadistic = 500  # 残虐天性
	ForceAddStatus = 600  # 增加己方状态


# ===================================================
#  特训使用效果表
# ===================================================
class ExerProEffect(models.Model):

	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "特训使用效果"

	CODES = [
		(ExerProEffectCode.Unset.value, '空'),

		(ExerProEffectCode.Attack.value, '造成伤害'),
		(ExerProEffectCode.AttackSlash.value, '造成伤害（完美斩击）'),
		(ExerProEffectCode.AttackBlack.value, '造成伤害（黑旋风）'),
		(ExerProEffectCode.AttackWave.value, '造成伤害（波动拳）'),
		(ExerProEffectCode.AttackRite.value, '造成伤害（仪式匕首）'),

		(ExerProEffectCode.Recover.value, '回复体力值'),

		(ExerProEffectCode.AddParam.value, '增加能力值'),
		(ExerProEffectCode.AddParamUrgent.value, '增加能力值（紧急按钮）'),
		(ExerProEffectCode.TempAddParam.value, '临时增加能力值'),
		(ExerProEffectCode.AddStatus.value, '增加状态'),

		(ExerProEffectCode.GetCards.value, '抽取卡牌'),
		(ExerProEffectCode.RemoveCards.value, '移除卡牌'),

		(ExerProEffectCode.ChangeCost.value, '更改耗能'),
		(ExerProEffectCode.ChangeCostDisc.value, '更改耗能（发现）'),
		(ExerProEffectCode.ChangeCostCrazy.value, '更改耗能（疯狂）'),

		(ExerProEffectCode.Sadistic.value, '残虐天性'),
		(ExerProEffectCode.ForceAddStatus.value, '增加己方状态'),
	]

	# 效果编号
	code = models.PositiveSmallIntegerField(default=0, choices=CODES, verbose_name="效果编号")

	# 效果参数
	params = jsonfield.JSONField(default=[], verbose_name="效果参数")

	# 转化为字典
	def convertToDict(self):

		return {
			'code': self.code,
			'params': self.params,
		}


# ===================================================
#  特训物品使用效果表
# ===================================================
class ExerProItemEffect(ExerProEffect):

	class Meta:
		verbose_name = verbose_name_plural = "特训物品使用效果"

	# 物品
	item = models.ForeignKey('ExerProItem', on_delete=models.CASCADE,
							 verbose_name="物品")


# ===================================================
#  特训物品表
# ===================================================
class ExerProItem(BaseItem):

	class Meta:

		verbose_name = verbose_name_plural = "特训物品"

	# 道具类型
	TYPE = ItemType.ExerProItem


# ===================================================
#  特训药水使用效果表
# ===================================================
class ExerProPotionEffect(ExerProEffect):

	class Meta:
		verbose_name = verbose_name_plural = "特训药水使用效果"

	# 物品
	item = models.ForeignKey('ExerProPotion', on_delete=models.CASCADE,
							 verbose_name="物品")


# ===================================================
#  特训药水表
# ===================================================
class ExerProPotion(BaseItem):

	class Meta:

		verbose_name = verbose_name_plural = "特训物品"

	# 道具类型
	TYPE = ItemType.ExerProPotion

	# HP回复点数
	hp_recover = models.SmallIntegerField(default=0, verbose_name="HP回复点数")

	# HP回复率（*100）
	hp_rate = models.IntegerField(default=0, verbose_name="HP回复率")

	# 力量提升点数
	power_add = models.SmallIntegerField(default=0, verbose_name="力量提升点数")

	# 力量提升率（*100）（提升的概率，即计算中要+1）
	power_rate = models.IntegerField(default=0, verbose_name="力量提升率")

	def convertToDict(self):
		"""
		转化为字典
		Returns:
			返回转化后的字典
		"""
		res = super().convertToDict()

		res['hp_recover'] = self.hp_recover
		res['hp_rate'] = self.hp_rate / 100
		res['power_add'] = self.power_add
		res['power_rate'] = self.power_rate / 100

		return res


# ===================================================
#  特训卡片使用效果表
# ===================================================
class ExerProCardEffect(ExerProEffect):

	class Meta:
		verbose_name = verbose_name_plural = "特训卡片使用效果"

	# 物品
	item = models.ForeignKey('ExerProCard', on_delete=models.CASCADE,
							 verbose_name="物品")


# ===================================================
#  卡片类型枚举
# ===================================================
class ExerProCardType(Enum):

	Normal = 1  # 普通
	Evil = 2  # 诅咒


# ===================================================
#  特训卡片表
# ===================================================
class ExerProCard(BaseItem):

	class Meta:

		verbose_name = verbose_name_plural = "特训卡片"

	# 道具类型
	TYPE = ItemType.ExerProCard

	CARD_TYPES = [
		(ExerProCardType.Normal.value, '普通'),
		(ExerProCardType.Evil.value, '诅咒'),
	]

	# 消耗能量
	cost = models.PositiveSmallIntegerField(default=1, verbose_name="消耗能量")

	# 卡片类型
	card_type = models.PositiveSmallIntegerField(
		default=ExerProCardType.Normal.value, choices=CARD_TYPES, verbose_name="卡片类型")

	def convertToDict(self):
		"""
		转化为字典
		Returns:
			返回转化后的字典
		"""
		res = super().convertToDict()

		res['cost'] = self.cost
		res['card_type'] = self.card_type

		return res


# ===================================================
#  敌人等级枚举
# ===================================================
class ExerProEnemyLevel(Enum):

	Normal = 1  # 普通
	Elite = 2  # 精英
	Boss = 3  # BOSS


# ===================================================
#  特训敌人表
# ===================================================
class ExerProEnemy(BaseItem):

	class Meta:

		verbose_name = verbose_name_plural = "特训敌人"

	# 道具类型
	TYPE = ItemType.ExerProEnemy

	LEVELS = [
		(ExerProEnemyLevel.Normal.value, '普通'),
		(ExerProEnemyLevel.Elite.value, '精英'),
		(ExerProEnemyLevel.Boss.value, 'BOSS'),
	]

	# 最大体力值
	mhp = models.PositiveSmallIntegerField(default=100, verbose_name="最大体力值")

	# 力量
	power = models.PositiveSmallIntegerField(default=10, verbose_name="力量")

	# 等级
	level = models.PositiveSmallIntegerField(
		default=ExerProEnemyLevel.Normal.value, choices=LEVELS, verbose_name="等级")

	def convertToDict(self):
		"""
		转化为字典
		Returns:
			返回转化后的字典
		"""
		res = super().convertToDict()

		res['mhp'] = self.mhp
		res['power'] = self.power
		res['level'] = self.level

		return res


# ===================================================
#  特训状态表
# ===================================================
class ExerProStatus(BaseItem):

	class Meta:

		verbose_name = verbose_name_plural = "特训状态"

	# 道具类型
	TYPE = ItemType.ExerProStatus


# endregion

# region 地图


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

	def convertToDict(self):
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


# ===================================================
#  据点类型表
# ===================================================
class NodeType(Enum):
	Rest = 0  # 休息据点
	Treasure = 1  # 藏宝据点
	Shop = 2  # 商人据点
	Enemy = 3  # 敌人据点
	Elite = 4  # 精英据点
	Unknown = 5  # 未知据点
	Boss = 6  # 精英据点


# ===================================================
#  地图关卡表
# ===================================================
class ExerProMapStage(models.Model):
	class Meta:
		verbose_name = verbose_name_plural = "地图关卡"

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

	def convertToDict(self):
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

# endregion

