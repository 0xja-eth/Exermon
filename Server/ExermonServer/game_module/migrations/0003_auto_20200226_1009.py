# Generated by Django 2.2.6 on 2020-02-26 10:09

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('game_module', '0002_auto_20200224_2304'),
    ]

    operations = [
        migrations.AlterField(
            model_name='questionstar',
            name='std_time',
            field=models.PositiveSmallIntegerField(default=0, verbose_name='标准时间（秒）'),
        ),
    ]
