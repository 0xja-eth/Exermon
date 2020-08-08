from enum import Enum


# ===================================================
#  题目类型枚举
# ===================================================
class QuestionType(Enum):
	Selecting = 100  # 选择题
	General = 101  # 一般选择题
	Plot = 102  # 剧情题

	Filling = 200  # 填空题

	Correcting = 300  # 判断题

	Writing = 400  # 简答题/写作题

	Element = 500  # 元素（单词、短语等，单个元素）
	Word = 501  # 单词
	Phrase = 502  # 短语

	Group = 600  # 组合题
	Listening = 601  # 听力题
	Reading = 602  # 阅读题

	Others = 0  # 其他题目


QUES_TYPES = [
	(QuestionType.Selecting.value, '选择题'),
	(QuestionType.General.value, '一般选择题'),
	(QuestionType.Plot.value, '剧情题'),

	(QuestionType.Filling.value, '填空题'),

	(QuestionType.Correcting.value, '改错题'),

	(QuestionType.Writing.value, '简答题'),

	(QuestionType.Element.value, '元素'),
	(QuestionType.Word.value, '单词'),
	(QuestionType.Phrase.value, '短语'),

	(QuestionType.Group.value, '组合题'),
	(QuestionType.Listening.value, '听力题'),
	(QuestionType.Reading.value, '阅读题'),

	(QuestionType.Others.value, '其他题'),
]
