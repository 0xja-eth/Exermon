# -*-coding:GBK -*-

from django.db import models
from django.conf import settings
from game_module.models import GroupConfigure
from item_module.models import *
from question_module.models import BaseQuestion, BaseQuesChoice, GroupQuestion
from utils.model_utils import QuestionAudioUpload, Common as ModelUtils
from utils.exception import ErrorType, GameException
import os, base64, datetime, jsonfield, random
from enum import Enum

# Create your models here.

# region 题目


# ===================================================
#  英语题目类型枚举
# ===================================================
class QuestionType(Enum):
	Listening = 1  # 听力题
	Phrase = 2  # 不定式题
	Correction = 3  # 改错题


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

	def convertToDict(self):
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
	word = models.TextField(verbose_name="正确单词", null=True, blank=True)

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

	AddState = 220  # 增加状态
	RemoveState = 221  # 移除状态
	RemoveNegaState = 222  # 移除消极状态

	DrawCards = 300  # 抽取卡牌
	ConsumeCards = 310  # 消耗卡牌

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

		(ExerProEffectCode.AddState.value, '增加状态'),
		(ExerProEffectCode.RemoveState.value, '移除状态'),
		(ExerProEffectCode.RemoveNegaState.value, '移除消极状态'),

		(ExerProEffectCode.DrawCards.value, '抽取卡牌'),
		(ExerProEffectCode.ConsumeCards.value, '消耗卡牌'),

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
#  特训物品星级表
# ===================================================
class ExerProItemStar(GroupConfigure):
	class Meta:
		verbose_name = verbose_name_plural = "特训物品星级"

	# 星级颜色（#ABCDEF）
	color = models.CharField(max_length=7, null=False, default='#000000', verbose_name="星级颜色")

	def __str__(self):
		return self.name

	# 管理界面用：显示星级颜色
	def adminColor(self):
		from django.utils.html import format_html

		res = '<div style="background: %s; width: 48px; height: 24px;"></div>' % self.color

		return format_html(res)

	adminColor.short_description = "星级颜色"

	def convertToDict(self):
		return {
			'id': self.id,
			'name': self.name,
			'color': self.color,
		}


# ===================================================
#  基本特训物品表
# ===================================================
class BaseExerProItem(BaseItem):
	class Meta:
		abstract = True
		verbose_name = verbose_name_plural = "特训物品"

	# 物品星级（稀罕度）
	star = models.ForeignKey("ExerProItemStar", on_delete=models.CASCADE, verbose_name="星级")

	# 金币（0表示不可购买）
	gold = models.PositiveSmallIntegerField(default=0, verbose_name="金币")

	def convertToDict(self, **kwargs):
		"""
		转化为字典
		Returns:
			返回转化后的字典
		"""
		res = super().convertToDict(**kwargs)

		effects = ModelUtils.objectsToDict(self.effects())

		res['star_id'] = self.star_id
		res['gold'] = self.gold
		res['effects'] = effects

		return res

	def effects(self):
		raise NotImplementedError


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
class ExerProItem(BaseExerProItem):
	class Meta:
		verbose_name = verbose_name_plural = "特训物品"

	# 道具类型
	TYPE = ItemType.ExerProItem

	def effects(self):
		return self.exerproitemeffect_set.all()


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
class ExerProPotion(BaseExerProItem):
	class Meta:
		verbose_name = verbose_name_plural = "特训药水"

	# 道具类型
	TYPE = ItemType.ExerProPotion

	def convertToDict(self):
		"""
		转化为字典
		Returns:
			返回转化后的字典
		"""
		res = super().convertToDict()

		return res

	def effects(self):
		return self.exerpropotioneffect_set.all()


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

	def convertToDict(self):
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
#  卡片类型枚举
# ===================================================
class ExerProCardType(Enum):
	Attack = 1  # 攻击
	Skill = 2  # 技能
	Ability = 3  # 能力
	Evil = 4  # 诅咒


# ===================================================
#  卡片类目标举
# ===================================================
class ExerProCardTarget(Enum):
	Default = 0  # 默认
	One = 1  # 单体
	All = 2  # 群体


# ===================================================
#  特训卡片表
# ===================================================
class ExerProCard(BaseExerProItem):
	class Meta:
		verbose_name = verbose_name_plural = "特训卡片"

	# 道具类型
	TYPE = ItemType.ExerProCard

	CARD_TYPES = [
		(ExerProCardType.Attack.value, '攻击'),
		(ExerProCardType.Skill.value, '技能'),
		(ExerProCardType.Ability.value, '能力'),
		(ExerProCardType.Evil.value, '诅咒'),
	]

	TARGETS = [
		(ExerProCardTarget.Default.value, '默认'),
		(ExerProCardTarget.One.value, '单体'),
		(ExerProCardTarget.All.value, '群体'),
	]

	# 消耗能量
	cost = models.PositiveSmallIntegerField(default=1, verbose_name="消耗能量")

	# 卡片类型
	card_type = models.PositiveSmallIntegerField(default=ExerProCardType.Attack.value,
												 choices=CARD_TYPES, verbose_name="卡片类型")

	# 固有
	inherent = models.BooleanField(default=False, verbose_name="固有")

	# 消耗（一次性的）
	disposable = models.BooleanField(default=False, verbose_name="消耗")

	# 性质
	character = models.CharField(default="", blank=True, max_length=32, verbose_name="性质")

	# 目标
	target = models.PositiveSmallIntegerField(default=ExerProCardTarget.Default.value,
											  choices=TARGETS, verbose_name="目标")

	def convertToDict(self):
		"""
		转化为字典
		Returns:
			返回转化后的字典
		"""
		res = super().convertToDict()

		res['cost'] = self.cost
		res['card_type'] = self.card_type
		res['inherent'] = self.inherent
		res['disposable'] = self.disposable
		res['character'] = self.character
		res['target'] = self.target

		return res

	def effects(self):
		return self.exerprocardeffect_set.all()


# ===================================================
#  特训敌人攻击效果表
# ===================================================
class EnemyEffect(ExerProEffect):
	class Meta:
		verbose_name = verbose_name_plural = "特训敌人攻击效果"

	# 敌人
	enemy = models.ForeignKey('ExerProEnemy', on_delete=models.CASCADE,
							  verbose_name="敌人")


# ===================================================
#  敌人行动类型枚举
# ===================================================
class EnemyActionType(Enum):
	Attack = 1  # 攻击
	PowerUp = 2  # 提升
	PowerDown = 3  # 削弱
	AddStates = 4  # 状态
	Escape = 5  # 逃跑
	Unset = 6  # 什么都不做


# ===================================================
#  敌人行动表
# ===================================================
class EnemyAction(models.Model):
	class Meta:
		verbose_name = verbose_name_plural = "敌人行动"

	TYPES = [
		# (1, '攻击'),
		# (2, '提升'),
		# (3, '削弱'),
		# (4, '状态'),
		# (5, '逃跑'),
		# (6, '无'),
		(EnemyActionType.Attack.value, '攻击'),
		(EnemyActionType.PowerUp.value, '提升'),
		(EnemyActionType.PowerDown.value, '削弱'),
		(EnemyActionType.AddStates.value, '状态'),
		(EnemyActionType.Escape.value, '逃跑'),
		(EnemyActionType.Unset.value, '无'),
	]

	# 回合
	rounds = jsonfield.JSONField(default=[], verbose_name="回合")

	# 类型
	type = models.PositiveSmallIntegerField(default=EnemyActionType.Unset.value,
											choices=TYPES, verbose_name="类型")

	# 参数
	params = jsonfield.JSONField(default=[], verbose_name="参数")

	# 权重
	rate = models.PositiveSmallIntegerField(default=10, verbose_name="权重")

	# 敌人
	enemy = models.ForeignKey("ExerProEnemy", on_delete=models.CASCADE, verbose_name="敌人")

	def convertToDict(self):
		"""
		转化为字典
		Returns:
			返回转化后的字典
		"""
		return {
			'rounds': self.rounds,
			'type': self.type,
			'params': self.params,
			'rate': self.rate,
		}


# ===================================================
#  敌人等级枚举
# ===================================================
class ExerProEnemyType(Enum):
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

	ENEMY_TYPES = [
		(ExerProEnemyType.Normal.value, '普通'),
		(ExerProEnemyType.Elite.value, '精英'),
		(ExerProEnemyType.Boss.value, 'BOSS'),
	]

	# 等级
	type = models.PositiveSmallIntegerField(default=ExerProEnemyType.Normal.value,
											choices=ENEMY_TYPES, verbose_name="等级")

	# 最大体力值
	mhp = models.PositiveSmallIntegerField(default=100, verbose_name="最大体力值")

	# 力量
	power = models.PositiveSmallIntegerField(default=10, verbose_name="力量")

	# 格挡
	defense = models.PositiveSmallIntegerField(default=10, verbose_name="格挡")

	# 格挡
	character = models.CharField(default="", blank=True, max_length=32, verbose_name="性格")

	def convertToDict(self):
		"""
		转化为字典
		Returns:
			返回转化后的字典
		"""
		res = super().convertToDict()

		actions = ModelUtils.objectsToDict(self.actions())
		effects = ModelUtils.objectsToDict(self.effects())

		res['mhp'] = self.mhp
		res['power'] = self.power
		res['mhp'] = self.mhp
		res['power'] = self.power
		res['defense'] = self.defense
		res['character'] = self.character
		res['type_'] = self.type

		res['actions'] = actions
		res['effects'] = effects

		return res

	def actions(self):
		"""
		获取敌人的行动计划
		Returns:
			返回敌人行动计划
		"""
		return self.enemyaction_set.all()

	def effects(self):
		"""
		获取敌人的攻击效果
		Returns:
			返回敌人攻击效果
		"""
		return self.enemyeffect_set.all()


# ===================================================
#  特训状态表
# ===================================================
class ExerProState(BaseItem):
	class Meta:
		verbose_name = verbose_name_plural = "特训状态"

	# 道具类型
	TYPE = ItemType.ExerProState

	# 最大状态回合数
	max_turns = models.PositiveSmallIntegerField(default=0, verbose_name="最大状态回合数")

	# 是否负面状态
	is_nega = models.BooleanField(default=False, verbose_name="是否负面状态")

	def convertToDict(self):
		"""
		转化为字典
		Returns:
			返回转化后的字典
		"""
		res = super().convertToDict()

		res['max_turns'] = self.max_turns
		res['is_nega'] = self.is_nega

		return res


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

	def convertToDict(self):
		"""
		转化为字典
		Returns:
			返回转化后的字典
		"""
		res = super().convertToDict()

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


# ===================================================
#  特训记录表
# ===================================================
class ExerProRecord(CacheableModel):
	class Meta:

		verbose_name = verbose_name_plural = "特训记录"

	# 当前单词缓存键
	CUR_WORDS_CACHE_KEY = 'cur_words'

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

	# 创建新记录
	@classmethod
	def create(cls, player):
		"""
		创建特训记录
		Args:
			player (Player): 顽疾
			wids (list): 单词ID数组
		"""
		record = cls()
		record.player = player
		record.save()

		record._generateWordRecords()

		return record

	def convertToDict(self, type: str = None, **kwargs):
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
					 if word_rec.current_correct is None]

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

# endregion
