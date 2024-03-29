# Generated by Django 2.2.6 on 2020-06-02 17:40

from django.db import migrations, models
import django.db.models.deletion


class Migration(migrations.Migration):

    dependencies = [
        ('english_pro_module', '0006_merge_20200602_1739'),
    ]

    operations = [
        migrations.AlterModelOptions(
            name='exerprorecord',
            options={'verbose_name': '特训记录', 'verbose_name_plural': '特训记录'},
        ),
        migrations.AlterModelOptions(
            name='infinitivequestion',
            options={'verbose_name': '短语题', 'verbose_name_plural': '短语题'},
        ),
        migrations.AlterModelOptions(
            name='nodetype',
            options={'verbose_name': '据点类型', 'verbose_name_plural': '据点类型'},
        ),
        migrations.RenameField(
            model_name='exerprorecord',
            old_name='WordLevel',
            new_name='word_level',
        ),
        migrations.RemoveField(
            model_name='exerprorecord',
            name='words',
        ),
        migrations.RemoveField(
            model_name='wordrecord',
            name='player',
        ),
        migrations.AddField(
            model_name='wordrecord',
            name='record',
            field=models.ForeignKey(default=None, on_delete=django.db.models.deletion.CASCADE, to='english_pro_module.ExerProRecord', verbose_name='特训记录'),
            preserve_default=False,
        ),
        migrations.AlterField(
            model_name='wordrecord',
            name='current_correct',
            field=models.BooleanField(default=None, null=True, verbose_name='当前轮是否答对'),
        ),
    ]
