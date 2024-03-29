# Generated by Django 2.2.6 on 2020-04-21 13:53

from django.db import migrations, models
import jsonfield.fields


class Migration(migrations.Migration):

    dependencies = [
        ('battle_module', '0001_initial'),
    ]

    operations = [
        migrations.AlterModelOptions(
            name='battleitemslotitem',
            options={'verbose_name': '对战物资槽项', 'verbose_name_plural': '对战物资槽项'},
        ),
        migrations.RenameField(
            model_name='battleplayer',
            old_name='recover_score',
            new_name='recovery_score',
        ),
        migrations.RenameField(
            model_name='battleroundresult',
            old_name='recover',
            new_name='recovery',
        ),
        migrations.RemoveField(
            model_name='battleplayer',
            name='exp_incr',
        ),
        migrations.RemoveField(
            model_name='battleplayer',
            name='slot_exp_incr',
        ),
        migrations.AddField(
            model_name='battleplayer',
            name='exer_exp_incrs',
            field=jsonfield.fields.JSONField(default={}, null=True, verbose_name='经验增加'),
        ),
        migrations.AddField(
            model_name='battleplayer',
            name='slot_exp_incrs',
            field=jsonfield.fields.JSONField(default={}, null=True, verbose_name='槽经验增加'),
        ),
        migrations.AlterField(
            model_name='battleroundresult',
            name='result_type',
            field=models.PositiveSmallIntegerField(choices=[(0, '未知'), (1, '命中'), (2, '暴击'), (3, '回避')], default=0, verbose_name='回合结果'),
        ),
    ]
