# Generated by Django 2.2.6 on 2020-05-05 15:07

from django.db import migrations


class Migration(migrations.Migration):

    dependencies = [
        ('game_module', '0003_auto_20200226_1009'),
        ('player_module', '0008_auto_20200505_1457'),
    ]

    operations = [
        migrations.RenameModel(
            old_name='HumanEquipParam',
            new_name='HumanEquipBaseParam',
        ),
    ]
