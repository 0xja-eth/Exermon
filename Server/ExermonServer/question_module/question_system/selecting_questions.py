from django.db import models
from django.conf import settings

from item_module.models import BaseEffect

from ..manager import QuesManager
from ..models import *
# import question_module.models as Models

from utils.cache_utils import CacheHelper
from utils.model_utils import PlotQuestionImageUpload, Common as ModelUtils
from utils.exception import ErrorType, GameException

import os, base64

# region General


# ===================================================
#  一般题目表
# ===================================================
@QuesManager.registerQuestion("一般题目")
class GeneralQuestion(SelectingQuestion):

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

	# region 转化读取配置

	TYPE_FIELD_FILTER_MAP = {
		'info': [star],
	}

	# # 转化索引信息
	# def _convertIndexInfo(self, res, type):
	# 	super()._convertIndexInfo(res, type)
	#
	# 	res['star_id'] = self.star_id
	#
	# def _convertBaseInfo(self, res, type):
	# 	super()._convertBaseInfo(res, type)
	#
	# 	res['level'] = self.level

	# endregion

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
class GeneralQuesChoice(BaseQuesChoice): pass


# ===================================================
#  题目图片表
# ===================================================
@QuesManager.registerQuesPicture(GeneralQuestion)
class GeneralQuesPicture(BaseQuesPicture): pass


# ===================================================
#  一般题目记录表
# ===================================================
@QuesManager.registerQuesRecord(GeneralQuestion)
class GeneralQuesRecord(BaseQuesRecord): pass


# ===================================================
#  一般题目反馈表
# ===================================================
@QuesManager.registerQuesReport(GeneralQuestion)
class GeneralQuesReport(BaseQuesReport): pass

# endregion


# ===================================================
#  剧情题目
# ===================================================
@QuesManager.registerQuestion("剧情题目")
class PlotQuestion(SelectingQuestion):

	# 剧情内容
	plot = models.TextField(verbose_name="剧情内容")

	# 剧情图标
	picture = models.ImageField(null=True, blank=True,
								upload_to=PlotQuestionImageUpload(), verbose_name="剧情图标")

	def _convertCustomAttrs(self, res, type=None, **kwargs):
		super()._convertCustomAttrs(res, type, **kwargs)

		res['picture'] = self.convertToBase64()

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


# ===================================================
#  剧情题目选项表
# ===================================================
@QuesManager.registerQuesChoice(PlotQuestion)
class PlotQuesChoice(BaseQuesChoice):

	# 所需金币
	gold = models.PositiveSmallIntegerField(default=0, verbose_name="所需金币")

	# 选项对应的结果文本
	result_text = models.TextField(verbose_name="选项对应的结果文本")

	@CacheHelper.staticCache
	def effects(self):
		return self.plotchoiceeffect_set.all()

	# def convert(self):
	# 	res = super().convert()
	#
	# 	plot_effects = ModelUtils.objectsToDict(self.effects())
	#
	# 	res['gold'] = self.gold
	# 	res['result_text'] = self.result_text
	# 	res['effects'] = plot_effects
	#
	# 	return res


# ===================================================
#  剧情题目效果表
# ===================================================
class PlotChoiceEffect(BaseEffect):
	class Meta:
		verbose_name = verbose_name_plural = "剧情题目效果"

	# 选项
	choice = models.ForeignKey('PlotQuesChoice', on_delete=models.CASCADE,
							 verbose_name="选项")
