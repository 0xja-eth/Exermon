from enum import Enum


# ===================================================
#  题目集类型枚举
# ===================================================
class QuestionSetType(Enum):
	GeneralExercise = 100  # 常规刷题
	ListeningExercise = 101  # 听力刷题
	ReadingExercise = 102  # 阅读刷题
	WordExercise = 103  # 单词训练
	PhraseExercise = 104  # 短语训练

	Exam = 200  # 考核

	Battle = 300  # 对战

	Unset = 0  # 未设置


QUES_SET_TYPES = [
	(QuestionSetType.Unset.value, '未设置'),
	(QuestionSetType.Exercise.value, '刷题'),
	(QuestionSetType.Exam.value, '考试'),
	(QuestionSetType.Battle.value, '对战'),
]
