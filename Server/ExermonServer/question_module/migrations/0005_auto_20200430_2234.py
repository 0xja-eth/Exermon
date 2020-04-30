# Generated by Django 2.2.6 on 2020-04-30 22:34

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('question_module', '0004_auto_20200421_1353'),
    ]

    operations = [
        migrations.AlterField(
            model_name='quesreport',
            name='type',
            field=models.PositiveSmallIntegerField(choices=[(1, '题目错误'), (2, '图片错误'), (3, '答案错误'), (4, '解析错误'), (5, '科目错误'), (6, '难度分配错误'), (7, '多个错误'), (0, '其他错误')], verbose_name='类型'),
        ),
    ]
