from enum import Enum


# ===================================================
#  题目集类型枚举
# ===================================================
class QuesSetType(Enum):
	GeneralExercise = 100  # 常规刷题
	ListeningExercise = 101  # 听力刷题
	ReadingExercise = 102  # 阅读刷题
	WordExercise = 103  # 单词训练
	PhraseExercise = 104  # 短语训练

	Exam = 200  # 考核

	Battle = 300  # 对战

	Unset = 0  # 未设置


QUES_SET_TYPES = [
	(QuesSetType.Unset.value, '未设置'),
	(QuesSetType.Exercise.value, '刷题'),
	(QuesSetType.Exam.value, '考试'),
	(QuesSetType.Battle.value, '对战'),
]
