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

    # 英语单词的路径
    words_path = 'english_materials/words.txt'
    # 短语的路径
    phrase_path = 'english_materials/phrase.txt'
    # 听力题的路径
    learning_question_path = 'english_materials/LearningQuestion.json'
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
            if len(str_list) == 2:
                english = str_list[0]
                chinese = str_list[1]
                Word.objects.create(english=english, chinese=chinese)

        f.close()

        center_print('Done inserting words')

    # 插入不定式题目
    def insert_infinitive_question():
        f = open(phrase_path, encoding='utf-8')

        for line in f:
            str_list = line.strip().strip('\n').split(sep='?')
            if len(str_list) == 3:
                word = str_list[0]
                chinese = str_list[2]
                phrase = str_list[1]

                type = None

                if phrase in todo:
                    type = PhraseType.Do.value
                elif phrase in sb:
                    type = PhraseType.SB.value
                elif phrase in of:
                    type = PhraseType.Prep.value

                PhraseQuestion.objects.create(word=word, chinese=chinese,
                                              phrase=phrase, type=type)

        f.close()
        center_print('Done inserting InfinitiveQuestion')

    def select_answer(option, answers, question):
        for i in range(3):
            if option == i + 1:
                ListeningQuesChoice.objects.create(order=i+1, text=answers[i], answer=True, question=question)
            else:
                ListeningQuesChoice.objects.create(order=i+1, text=answers[i], answer=False, question=question)

    # 插入听力题目
    def insert_learning_questions():
        with open(learning_question_path, encoding='utf-8') as f:
            pop_data = json.load(f)
            for pop_dict in pop_data:
                audio = pop_dict['audio']
                with open(audio, 'rb') as file:
                    _f = File(file)
                    group_questions = pop_dict['groupQuestions']
                    for group_question in group_questions:
                        article = group_question['article']
                        if article != "":
                            learning_question = ListeningQuestion.objects.create(audio=_f, article=article)
                        else:
                            learning_question = ListeningQuestion.objects.create(audio=_f)
                        questions = group_question['questions']
                        for question in questions:
                            title = question['title']
                            answers = question['answers']
                            option = question['option']
                            learning_sub_question = ListeningSubQuestion.objects.create(title=title,
                                                                                        question=learning_question)
                            select_answer(option, answers, learning_sub_question)

    # insert_words()
    # insert_infinitive_question()
    insert_learning_questions()