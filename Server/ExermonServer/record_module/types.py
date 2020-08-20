from enum import Enum


# ===================================================
#  题目集类型枚举
# ===================================================
class QuesSetType(Enum):
	GeneralExercise = 100  # 常规刷题
	ListeningExercise = 101  # 听力刷题
	ReadingExercise = 102  # 阅读刷题
	CollectingExercise = 103  # 改错刷题
	WordExercise = 104  # 单词训练
	PhraseExercise = 105  # 短语训练

	Exam = 200  # 考核

	Battle = 300  # 对战

	Unset = 0  # 未设置


QUES_SET_TYPES = [
	(QuesSetType.Unset.value, '未设置'),

	(QuesSetType.GeneralExercise.value, '常规刷题'),
	(QuesSetType.ListeningExercise.value, '听力刷题'),
	(QuesSetType.ReadingExercise.value, '阅读刷题'),
	(QuesSetType.CollectingExercise.value, '改错刷题'),
	(QuesSetType.WordExercise.value, '单词联系'),
	(QuesSetType.PhraseExercise.value, '短语练习'),

	(QuesSetType.Exam.value, '考试'),
	(QuesSetType.Battle.value, '对战'),
]
