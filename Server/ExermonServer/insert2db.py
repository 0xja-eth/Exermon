# -*- coding: utf-8 -*-
import os
import uuid
import django
from django.core.files import File

os.environ.setdefault("DJANGO_SETTINGS_MODULE", "ExermonServer.settings")

if django.VERSION >= (1, 7):  # 自动判断版本
    django.setup()

# 这个模型不能放在文件头部导入
from english_pro_module.models import Word, InfinitiveQuestion, InfinitiveType

# 英语单词的路径
words_path = 'english_materials/words.txt'
# 短语的路径
phrase_path = 'english_materials/phrase.txt'
# 判断短语的分类
todo = ['to do sth.', 'doing sth.']
sb = ['sb. to do sth.', 'sb. doing sth.', 'sb. do sth.', 'sth. for sb.', 'sb. of sth.', 'sb. for doing sth.', 'sb. into doing sth.']
of = ['about', 'at', 'for', 'from', 'in', 'of', 'to', 'with', 'of', 'to']


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
            infinitive = str_list[1]

            infinitive_type = None

            if infinitive in todo:
                infinitive_type = InfinitiveType.Do.value
            elif infinitive in sb:
                infinitive_type = InfinitiveType.SB.value
            elif infinitive in of:
                infinitive_type = InfinitiveType.Prep.value

            InfinitiveQuestion.objects.create(word=word,
                                              chinese=chinese,
                                              infinitive=infinitive,
                                              type=infinitive_type)
    f.close()
    center_print('Done inserting InfinitiveQuestion')


if __name__ == '__main__':
    insert_words()
    insert_infinitive_question()