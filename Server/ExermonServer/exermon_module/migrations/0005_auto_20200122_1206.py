# Generated by Django 2.2.6 on 2020-01-22 12:06

from django.db import migrations


class Migration(migrations.Migration):

    dependencies = [
        ('exermon_module', '0004_auto_20200122_0004'),
    ]

    operations = [
        migrations.RenameField(
            model_name='exerskillslot',
            old_name='exermon',
            new_name='player_exer',
        ),
        migrations.RenameField(
            model_name='exerskillslotitem',
            old_name='exermon',
            new_name='player_exer',
        ),
    ]
