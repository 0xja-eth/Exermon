# Generated by Django 2.2.6 on 2020-05-27 15:27

from django.db import migrations, models
import django.db.models.deletion
import jsonfield.fields
import utils.model_utils


class Migration(migrations.Migration):

    initial = True

    dependencies = [
        ('player_module', '0014_auto_20200510_1947'),
    ]

    operations = [
        migrations.CreateModel(
            name='CorrectionQuestion',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('article', models.TextField(verbose_name='文章')),
                ('description', models.TextField(blank=True, null=True, verbose_name='解析')),
            ],
            options={
                'verbose_name': '改错题',
                'verbose_name_plural': '改错题',
            },
        ),
        migrations.CreateModel(
            name='ExerProCard',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('name', models.CharField(max_length=24, verbose_name='名称')),
                ('description', models.CharField(blank=True, max_length=128, verbose_name='描述')),
                ('cost', models.PositiveSmallIntegerField(default=1, verbose_name='消耗能量')),
                ('card_type', models.PositiveSmallIntegerField(choices=[(1, '普通'), (2, '诅咒')], default=1, verbose_name='卡片类型')),
            ],
            options={
                'verbose_name': '特训卡片',
                'verbose_name_plural': '特训卡片',
            },
        ),
        migrations.CreateModel(
            name='ExerProEnemy',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('name', models.CharField(max_length=24, verbose_name='名称')),
                ('description', models.CharField(blank=True, max_length=128, verbose_name='描述')),
                ('mhp', models.PositiveSmallIntegerField(default=100, verbose_name='最大体力值')),
                ('power', models.PositiveSmallIntegerField(default=10, verbose_name='力量')),
                ('level', models.PositiveSmallIntegerField(choices=[(1, '普通'), (2, '精英'), (3, 'BOSS')], default=1, verbose_name='等级')),
            ],
            options={
                'verbose_name': '特训敌人',
                'verbose_name_plural': '特训敌人',
            },
        ),
        migrations.CreateModel(
            name='ExerProItem',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('name', models.CharField(max_length=24, verbose_name='名称')),
                ('description', models.CharField(blank=True, max_length=128, verbose_name='描述')),
            ],
            options={
                'verbose_name': '特训物品',
                'verbose_name_plural': '特训物品',
            },
        ),
        migrations.CreateModel(
            name='ExerProMap',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('name', models.CharField(max_length=24, verbose_name='地图名称')),
                ('description', models.CharField(max_length=512, verbose_name='故事描述')),
                ('level', models.PositiveSmallIntegerField(default=1, verbose_name='地图难度')),
                ('min_level', models.PositiveSmallIntegerField(default=1, verbose_name='等级要求')),
            ],
            options={
                'verbose_name': '特训地图',
                'verbose_name_plural': '特训地图',
            },
        ),
        migrations.CreateModel(
            name='ExerProPotion',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('name', models.CharField(max_length=24, verbose_name='名称')),
                ('description', models.CharField(blank=True, max_length=128, verbose_name='描述')),
                ('hp_recover', models.SmallIntegerField(default=0, verbose_name='HP回复点数')),
                ('hp_rate', models.IntegerField(default=0, verbose_name='HP回复率')),
                ('power_add', models.SmallIntegerField(default=0, verbose_name='力量提升点数')),
                ('power_rate', models.IntegerField(default=0, verbose_name='力量提升率')),
            ],
            options={
                'verbose_name': '特训物品',
                'verbose_name_plural': '特训物品',
            },
        ),
        migrations.CreateModel(
            name='ListeningQuestion',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('article', models.TextField(blank=True, null=True, verbose_name='文章')),
                ('source', models.TextField(blank=True, null=True, verbose_name='来源')),
                ('audio', models.FileField(upload_to=utils.model_utils.QuestionAudioUpload(), verbose_name='音频文件')),
            ],
            options={
                'verbose_name': '听力题',
                'verbose_name_plural': '听力题',
            },
        ),
        migrations.CreateModel(
            name='ReadingQuestion',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('article', models.TextField(blank=True, null=True, verbose_name='文章')),
                ('source', models.TextField(blank=True, null=True, verbose_name='来源')),
            ],
            options={
                'verbose_name': '阅读题',
                'verbose_name_plural': '阅读题',
            },
        ),
        migrations.CreateModel(
            name='Word',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('english', models.CharField(max_length=64, verbose_name='英文')),
                ('chinese', models.CharField(max_length=64, verbose_name='中文')),
                ('type', models.CharField(max_length=32, verbose_name='词性')),
                ('level', models.PositiveSmallIntegerField(default=1, verbose_name='等级')),
                ('is_middle', models.BooleanField(default=True, verbose_name='是否初中题目')),
                ('is_high', models.BooleanField(default=True, verbose_name='是否高中题目')),
            ],
            options={
                'verbose_name': '单词',
                'verbose_name_plural': '单词',
            },
        ),
        migrations.CreateModel(
            name='WrongItem',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('sentence_index', models.PositiveSmallIntegerField(verbose_name='句子编号')),
                ('word_index', models.PositiveSmallIntegerField(verbose_name='单词编号')),
                ('type', models.PositiveSmallIntegerField(choices=[(1, '增加'), (2, '修改'), (3, '删除')], default=2, verbose_name='修改类型')),
                ('word', models.TextField(verbose_name='正确单词')),
                ('question', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, to='english_pro_module.CorrectingQuestion', verbose_name='改错题目')),
            ],
            options={
                'verbose_name': '改错题错误项',
                'verbose_name_plural': '改错题错误项',
            },
        ),
        migrations.CreateModel(
            name='WordRecord',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('count', models.PositiveSmallIntegerField(default=0, verbose_name='回答次数')),
                ('correct', models.PositiveSmallIntegerField(default=0, verbose_name='正确数')),
                ('last_date', models.DateTimeField(null=True, verbose_name='上次回答日期')),
                ('first_date', models.DateTimeField(null=True, verbose_name='初次回答日期')),
                ('collected', models.BooleanField(default=False, verbose_name='收藏标志')),
                ('wrong', models.BooleanField(default=False, verbose_name='错题标志')),
                ('player', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, to='player_module.Player', verbose_name='玩家')),
                ('word', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, to='english_pro_module.Word', verbose_name='单词')),
            ],
            options={
                'verbose_name': '单词记录',
                'verbose_name_plural': '单词记录',
            },
        ),
        migrations.CreateModel(
            name='ReadingSubQuestion',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('title', models.TextField(verbose_name='题干')),
                ('description', models.TextField(blank=True, null=True, verbose_name='题解')),
                ('type', models.PositiveSmallIntegerField(choices=[(0, '单选题'), (1, '多选题'), (2, '判断题'), (-1, '其他题')], default=0, verbose_name='类型')),
                ('for_test', models.BooleanField(default=False, verbose_name='测试')),
                ('question', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, to='english_pro_module.ReadingQuestion', verbose_name='阅读题目')),
            ],
            options={
                'verbose_name': '阅读小题',
                'verbose_name_plural': '阅读小题',
            },
        ),
        migrations.CreateModel(
            name='ReadingQuesChoice',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('order', models.PositiveSmallIntegerField(verbose_name='编号')),
                ('text', models.TextField(verbose_name='文本')),
                ('answer', models.BooleanField(default=False, verbose_name='正误')),
                ('question', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, to='english_pro_module.ReadingSubQuestion', verbose_name='所属问题')),
            ],
            options={
                'verbose_name': '阅读题目选项',
                'verbose_name_plural': '阅读题目选项',
            },
        ),
        migrations.CreateModel(
            name='ListeningSubQuestion',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('title', models.TextField(verbose_name='题干')),
                ('description', models.TextField(blank=True, null=True, verbose_name='题解')),
                ('type', models.PositiveSmallIntegerField(choices=[(0, '单选题'), (1, '多选题'), (2, '判断题'), (-1, '其他题')], default=0, verbose_name='类型')),
                ('for_test', models.BooleanField(default=False, verbose_name='测试')),
                ('question', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, to='english_pro_module.ListeningQuestion', verbose_name='听力题目')),
            ],
            options={
                'verbose_name': '听力小题',
                'verbose_name_plural': '听力小题',
            },
        ),
        migrations.CreateModel(
            name='ListeningQuesChoice',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('order', models.PositiveSmallIntegerField(verbose_name='编号')),
                ('text', models.TextField(verbose_name='文本')),
                ('answer', models.BooleanField(default=False, verbose_name='正误')),
                ('question', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, to='english_pro_module.ListeningSubQuestion', verbose_name='所属问题')),
            ],
            options={
                'verbose_name': '听力题目选项',
                'verbose_name_plural': '听力题目选项',
            },
        ),
        migrations.CreateModel(
            name='ExerProMapStage',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('order', models.PositiveSmallIntegerField(default=1, verbose_name='序号')),
                ('max_battle_enemies', models.PositiveSmallIntegerField(default=1, verbose_name='战斗最大敌人数量')),
                ('steps', jsonfield.fields.JSONField(default=[3, 4, 5, 2, 1], verbose_name='每步据点个数')),
                ('max_fork_node', models.PositiveSmallIntegerField(default=5, verbose_name='最大分叉据点数量')),
                ('max_fork', models.PositiveSmallIntegerField(default=3, verbose_name='最大分叉选择数')),
                ('node_rate', jsonfield.fields.JSONField(default=[1, 1, 1, 1, 1, 1], verbose_name='据点比例')),
                ('enemies', models.ManyToManyField(to='english_pro_module.ExerProEnemy', verbose_name='敌人集合')),
                ('map', models.ForeignKey(on_delete=django.db.models.deletion.CASCADE, to='english_pro_module.ExerProMap', verbose_name='地图')),
            ],
            options={
                'verbose_name': '地图关卡',
                'verbose_name_plural': '地图关卡',
            },
        ),
    ]
