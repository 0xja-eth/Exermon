from django.db import models
from django.conf import settings

from ..manager import QuesManager
from ..models import *
# import question_module.models as Models

from utils.model_utils import QuestionAudioUpload
from utils.exception import ErrorType, GameException

import os, base64


# ===================================================
#  听力题
# ===================================================
@QuesManager.registerQuestion("听力题")
class ListeningQuestion(GroupQuestion):

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

	def _convertBaseInfo(self, res, type):
		super()._convertBaseInfo(res, type)

		res['times'] = self.times
		res['audio'] = self.convertToBase64()


# ===================================================
#  听力小题
# ===================================================
@QuesManager.registerSubQuestion(ListeningQuestion)
class ListeningSubQuestion(SelectingQuestion): pass


# ===================================================
#  听力题目选项表
# ===================================================
@QuesManager.registerQuesChoice(ListeningSubQuestion)
class ListeningQuesChoice(BaseQuesChoice): pass


# ===================================================
#  听力题目记录表
# ===================================================
@QuesManager.registerQuesRecord(ListeningQuestion)
class ListeningQuesRecord(BaseQuesRecord): pass


# ===================================================
#  听力题目反馈表
# ===================================================
@QuesManager.registerQuesReport(ListeningQuestion)
class ListeningQuesReport(BaseQuesReport): pass


# ===================================================
#  阅读题
# ===================================================
@QuesManager.registerQuestion("阅读题")
class ReadingQuestion(GroupQuestion): pass


# ===================================================
#  阅读小题
# ===================================================
@QuesManager.registerSubQuestion(ReadingQuestion)
class ReadingSubQuestion(SelectingQuestion): pass


# ===================================================
#  阅读题目选项表
# ===================================================
@QuesManager.registerQuesChoice(ReadingSubQuestion)
class ReadingQuesChoice(BaseQuesChoice): pass


# ===================================================
#  阅读题目记录表
# ===================================================
@QuesManager.registerQuesRecord(ReadingQuestion)
class ReadingQuesRecord(BaseQuesRecord): pass


# ===================================================
#  阅读题目反馈表
# ===================================================
@QuesManager.registerQuesReport(ReadingQuestion)
class ReadingQuesReport(BaseQuesReport): pass
