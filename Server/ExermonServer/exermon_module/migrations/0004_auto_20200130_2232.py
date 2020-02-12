# Generated by Django 2.2.6 on 2020-01-30 22:32

from django.db import migrations, models
import utils.model_utils


class Migration(migrations.Migration):

    dependencies = [
        ('exermon_module', '0003_auto_20200127_1416'),
    ]

    operations = [
        migrations.AlterField(
            model_name='exerskill',
            name='target_ani',
            field=models.ImageField(blank=True, null=True, upload_to=utils.model_utils.SkillImageUpload('target'), verbose_name='击中动画'),
        ),
        migrations.AlterField(
            model_name='skilleffect',
            name='code',
            field=models.PositiveSmallIntegerField(choices=[(0, '空'), (10, '回复体力值'), (11, '回复精力值'), (20, '增加能力值'), (21, '临时增加能力值'), (22, '战斗中增加能力值'), (30, '获得物品'), (31, '获得金币'), (32, '获得绑定点券'), (40, '指定艾瑟萌获得经验'), (41, '指定艾瑟萌槽项获得经验'), (42, '玩家获得经验'), (99, '执行程序')], default=0, verbose_name='效果编号'),
        ),
    ]