from enum import Enum


# ===================================================
#  题目集类型枚举
# ===================================================
class QuestionSetType(Enum):
	Exercise = 1  # 刷题
	Exam = 2  # 考核
	Battle = 3  # 对战

	Unset = 0  # 未设置


QUES_SET_TYPES = [
	(QuestionSetType.Unset.value, '未设置'),
	(QuestionSetType.Exercise.value, '刷题'),
	(QuestionSetType.Exam.value, '考试'),
	(QuestionSetType.Battle.value, '对战'),
]
