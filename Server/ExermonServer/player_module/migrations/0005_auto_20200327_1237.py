# Generated by Django 2.2.6 on 2020-03-27 12:37

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('player_module', '0004_auto_20200322_1816'),
    ]

    operations = [
        migrations.AddField(
            model_name='humanequip',
            name='icon_index',
            field=models.PositiveSmallIntegerField(default=0, verbose_name='图标索引'),
        ),
        migrations.AddField(
            model_name='humanitem',
            name='icon_index',
            field=models.PositiveSmallIntegerField(default=0, verbose_name='图标索引'),
        ),
    ]