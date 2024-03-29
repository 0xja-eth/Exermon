# Generated by Django 2.2.7 on 2020-06-02 14:23

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('english_pro_module', '0005_auto_20200601_0301'),
    ]

    operations = [
        migrations.AddField(
            model_name='exerprorecord',
            name='finished',
            field=models.BooleanField(default=False, verbose_name='当前轮是否回答完毕'),
        ),
        migrations.AddField(
            model_name='infinitivequestion',
            name='infinitive_type',
            field=models.PositiveSmallIntegerField(choices=[(1, '[sb. sth. 开头的短语选项]'), (2, '[to do, doing 开头的短语选项]'), (3, '[sb. sth. 开头的短语选项]')], default=2, verbose_name='修改类型'),
        ),
        migrations.AlterField(
            model_name='word',
            name='type',
            field=models.CharField(blank=True, max_length=32, null=True, verbose_name='词性'),
        ),
    ]
