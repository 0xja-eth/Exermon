# Generated by Django 2.2.6 on 2020-01-31 15:35

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('exermon_module', '0005_auto_20200130_2301'),
    ]

    operations = [
        migrations.AddField(
            model_name='exermon',
            name='animal',
            field=models.CharField(default='', max_length=24, verbose_name='品种'),
            preserve_default=False,
        ),
    ]