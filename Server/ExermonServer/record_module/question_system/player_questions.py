from django.db import models

from .question_sets import *

from ..manager import RecordManager

from question_module.models import *

import record_module.models as Models

from utils.calc_utils.question_calc import *


# ===================================================
#  刷题题目关系表
# ===================================================
@RecordManager.registerPlayerQuestion(
	GeneralQuestion, ExerciseRecord,
	ExerciseSingleRewardCalc, RecordSource.Exercise)
class ExerciseQuestion(Models.SelectingPlayerQuestion): pass

