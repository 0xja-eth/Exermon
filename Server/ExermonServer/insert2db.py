# -*- coding: utf-8 -*-

if __name__ == '__main__':

	import os
	import uuid
	import json
	import django
	from django.core.files import File

	os.environ.setdefault("DJANGO_SETTINGS_MODULE", "ExermonServer.settings")

	if django.VERSION >= (1, 7):  # 自动判断版本
		django.setup()

	# 这个模型不能放在文件头部导入
	from english_pro_module.models import Word, PhraseQuestion, PhraseType, ListeningQuestion, ListeningSubQuestion, \
		ListeningQuesChoice

	base_path = 'english_pro_module/raw_data/'

	# 英语单词的路径
	words_path = base_path + 'words.txt'
	# 短语的路径
	phrase_path = base_path + 'phrase.txt'
	# 听力题的路径
	learning_question_path = base_path + 'listening.json'
	# 判断短语的分类
	todo = ['to do sth.', 'doing sth.']
	sb = ['sb. to do sth.', 'sb. doing sth.', 'sb. do sth.', 'sth. for sb.', 'sb. of sth.', 'sb. for doing sth.', 'sb. into doing sth.']
	of = ['about', 'at', 'for', 'from', 'in', 'of', 'to', 'with', 'of', 'to']
	# 听力题辅助词判断
	learning_list = ['groupQuestion', 'audioPath', 'article', 'question', 'Answer1', 'Answer2', 'Answer3', 'option']

	def center_print(*args, **kwargs):
		print(10 * '*', end=' ')
		print(*args, **kwargs, end=' ')
		print(10 * '*')

	# 插入单词
	def insert_words():
		f = open(words_path, encoding='utf-8')

		for line in f:
			str_list = line.strip().strip('\n').split(sep='?')
			if len(str_list) == 2: save_word(str_list)

		f.close()

		center_print('Done inserting words')

	# 保存单个单词
	def save_word(str_list):
		english = str_list[0]
		chinese = str_list[1]

		word = Word.objects.filter(english=english)
		flag = word.exists()

		if not flag:
			word = Word.objects.create(english=english, chinese=chinese)
		else:
			word = word.first()
			word.chinese = chinese
			word.save()

		print("%d. %s saved" % (word.id, word.english))

	# 插入不定式题目
	def insert_phrase_questions():
		f = open(phrase_path, encoding='utf-8')

		for line in f:
			str_list = line.strip().strip('\n').split(sep='?')
			if len(str_list) == 3: save_phrase_question(str_list)

		f.close()
		center_print('Done inserting PhraseQuestion')

	# 保存短语题目
	def save_phrase_question(str_list):
		word = str_list[0]
		chinese = str_list[2]
		phrase = str_list[1]

		ques = PhraseQuestion.objects.filter(word=word, chinese=chinese)
		flag = ques.exists()

		type = None

		if phrase in todo:
			type = PhraseType.Do.value
		elif phrase in sb:
			type = PhraseType.SB.value
		elif phrase in of:
			type = PhraseType.Prep.value

		if not flag:
			ques = PhraseQuestion.objects.create(word=word, chinese=chinese,
										  phrase=phrase, type=type)
		else:
			ques = ques.first()
			ques.phrase = phrase
			ques.type = type
			ques.save()

		print("%d. %s %s saved" % (ques.id, ques.word, ques.phrase))

	# 插入听力题目
	def insert_listening_questions():
		with open(learning_question_path, encoding='utf-8') as f:
			pop_data = json.load(f)
			index = 0
			for pop_dict in pop_data:
				group_questions = pop_dict['groupQuestions']
				for group_question in group_questions:
					index += 1
					save_listening_question(index, group_question)

		f.close()
		center_print('Done inserting ListeningQuestion')

	# 保存短语题目
	def save_listening_question(id, group_question):
		article = group_question['article']
		audio = base_path + group_question['audio']

		ques = ListeningQuestion.objects.filter(id=id)
		flag = ques.exists()

		if not flag:
			ques = ListeningQuestion()
		else:
			ques = ques.first()

		with open(audio, 'rb') as file:
			_f = File(file)

			ques.article = article
			ques.audio = _f

			questions = group_question['questions']
			ques.times = 1 if len(questions) <= 1 else 2

			ques.save()

			sub_queses = ques.listeningsubquestion_set.all()
			sub_cnt = sub_queses.count()

			index = 0
			for question in questions:
				title = question['title']
				answers = question['answers']
				option = question['option']

				if index < sub_cnt: sub_ques = sub_queses[index]
				else: sub_ques = ListeningSubQuestion()

				sub_ques.question_id = id
				sub_ques.title = title
				sub_ques.save()

				select_answer(option, answers, sub_ques)
				index += 1

		print("%d. %s saved" % (ques.id, ques.audio))

	def select_answer(option, answers, question: ListeningSubQuestion):

		choices = question.listeningqueschoice_set.all()
		cho_cnt = choices.count()

		for i in range(len(answers)):
			if i < cho_cnt: choice = choices[i]
			else: choice = ListeningQuesChoice()

			choice.order = i + 1
			choice.text = answers[i]
			choice.answer = (option == i+1)
			choice.question = question
			choice.save()

	# insert_words()
	# insert_phrase_questions()
	# insert_listening_questions()

	from english_pro_module.raw_data.upload import upload

	upload("ExerProItem")
	# upload("ExerProPotion")
	# upload("ExerProCard")
	# upload("ExerProEnemy")
	upload("ExerProState")
