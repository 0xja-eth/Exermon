# Generated by Django 2.2.6 on 2020-03-27 12:37

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('season_module', '0002_comprank_compseason_seasonrecord_suspensionrecord'),
    ]

    operations = [
        migrations.AlterField(
            model_name='seasonrecord',
            name='star_num',
            field=models.PositiveSmallIntegerField(default=1, verbose_name='段位星星'),
        ),
    ]
