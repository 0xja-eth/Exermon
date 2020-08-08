from django.db import models
from django.conf import settings

from item_module.models import BaseEffect

from ..manager import QuesManager
import question_module.models as Models

from utils.cache_utils import CacheHelper
from utils.model_utils import QuestionImageUpload, \
	PlotQuestionImageUpload, Common as ModelUtils
from utils.exception import ErrorType, GameException

import os, base64

# region General


# ===================================================
#  一般题目表
# ===================================================
@QuesManager.registerQuestion("一般题目")
class GeneralQuestion(Models.SelectingQuestion):

	# 常量声明
	DEFAULT_SCORE = 6
	UNDER_SELECT_SCORE_RATE = 0.5

	# 起始编号
	NUMBER_BASE = 10000

	# 星数
	star = models.ForeignKey('game_module.QuestionStar', default=1,
							 on_delete=models.CASCADE, verbose_name="星级")

	# 题目附加等级（计算用）
	level = models.SmallIntegerField(default=0, verbose_name="附加等级")

	# 转化索引信息
	def _convertIndexInfo(self, res, type):
		super()._convertIndexInfo(res, type)

		res['star_id'] = self.star_id

	def _convertBaseInfo(self, res, type):
		super()._convertBaseInfo(res, type)

		res['level'] = self.level

	def stdTime(self):
		return self.star.std_time

	# 基础经验值增量
	def expIncr(self):
		return self.star.exp_incr

	# 基础金币增量
	def goldIncr(self):
		return self.star.gold_incr

	# 总等级
	def sumLevel(self):
		return self.star.level + self.level

	# 计算分数
	def calcScore(self, selection):

		count = 0
		answers = self.correctAnswer()

		for select in selection:
			if select not in answers: return 0
			else: count += 1

		score = self.quesScore()

		if count == len(answers): return score
		elif count > 0: return round(score * self.UNDER_SELECT_SCORE_RATE)

		return 0


# ===================================================
#  题目选项表
# ===================================================
@QuesManager.registerQuesChoice(GeneralQuestion)
class GeneralQuesChoice(Models.BaseQuesChoice): pass


# ===================================================
#  题目图片表
# ===================================================
@QuesManager.registerQuesPicture(GeneralQuestion)
class GeneralQuesPicture(Models.BaseQuesPicture): pass


# ===================================================
#  一般题目记录表
# ===================================================
@QuesManager.registerQuesRecord(GeneralQuestion)
class GeneralQuesRecord(Models.BaseQuesRecord):

	# 初次用时（毫秒数）
	first_time = models.PositiveIntegerField(default=0, verbose_name="初次用时")

	# 平均用时（毫秒数）
	avg_time = models.PositiveIntegerField(default=0, verbose_name="平均用时")

	# 首次正确用时（毫秒数）
	corr_time = models.PositiveIntegerField(null=True, verbose_name="首次正确用时")

	# 累计获得经验
	sum_exp = models.PositiveSmallIntegerField(default=0, verbose_name="上次得分")

	# 累计获得金币
	sum_gold = models.PositiveSmallIntegerField(default=0, verbose_name="平均得分")

	# 转化为字符串
	def __str__(self):
		return '%s (%s)' % (self.question.number(), self.player)

	# 转化为字典
	def convert(self):
		res = super().convert()

		res['first_time'] = self.first_time
		res['avg_time'] = self.avg_time
		res['corr_time'] = self.corr_time
		res['sum_exp'] = self.sum_exp
		res['sum_gold'] = self.sum_gold

		return res

	# 创建新记录
	@classmethod
	def create(cls, player, question_id):
		record = player.questionRecord(question_id)

		if record is None:
			record = cls()
			record.player = player
			record.question_id = question_id
			record.save()

		return record

	# 更新已有记录
	def _updateRecord(self, player_ques):

		super()._updateRecord(player_ques)

		timespan = player_ques.timespan

		if player_ques.correct():
			if self.corr_time is None:
				self.corr_time = timespan

		if self.count <= 0:
			self.first_time = timespan

		sum_time = self.avg_time*self.count+timespan
		self.avg_time = round(sum_time/(self.count+1))

		self.sum_exp += player_ques.exp_incr
		self.sum_gold += player_ques.gold_incr

	# 正确率
	def corrRate(self):
		if self.count is None or self.count == 0:
			return 0
		return self.correct / self.count


# ===================================================
#  一般题目反馈表
# ===================================================
@QuesManager.registerQuesReport(GeneralQuestion)
class GeneralQuesReport(Models.BaseQuesReport): pass

# endregion


# ===================================================
#  剧情题目
# ===================================================
@QuesManager.registerQuestion("剧情题目")
class PlotQuestion(Models.SelectingQuestion):

	# 剧情内容
	plot = models.TextField(verbose_name="剧情内容")

	# 剧情图标
	picture = models.ImageField(null=True, blank=True,
								upload_to=PlotQuestionImageUpload(), verbose_name="剧情图标")

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

	def _convertBaseInfo(self, res, type):
		super()._convertBaseInfo(res, type)

		res['plot'] = self.plot
		res['picture'] = self.convertToBase64()


# ===================================================
#  剧情题目选项表
# ===================================================
@QuesManager.registerQuesChoice(PlotQuestion)
class PlotQuesChoice(Models.BaseQuesChoice):

	# 所需金币
	gold = models.PositiveSmallIntegerField(default=0, verbose_name="所需金币")

	# 选项对应的结果文本
	result_text = models.TextField(verbose_name="选项对应的结果文本")

	# 所属问题
	question = models.ForeignKey('PlotQuestion', null=False, on_delete=models.CASCADE,
								 verbose_name="所属问题")

	@CacheHelper.staticCache
	def effects(self):
		return self.plotchoiceeffect_set.all()

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
class PlotChoiceEffect(BaseEffect):
	class Meta:
		verbose_name = verbose_name_plural = "剧情题目效果"

	# 选项
	choice = models.ForeignKey('PlotQuesChoice', on_delete=models.CASCADE,
							 verbose_name="选项")


