# Generated by Django 2.2.6 on 2020-05-13 22:56

from django.db import migrations, models
import django.db.models.deletion


class Migration(migrations.Migration):

    dependencies = [
        ('record_module', '0003_auto_20200421_1353'),
    ]

    operations = [
        migrations.AlterField(
            model_name='exercisequestion',
            name='exercise',
            field=models.ForeignKey(null=True, on_delete=django.db.models.deletion.CASCADE, to='record_module.GeneralExerciseRecord', verbose_name='刷题记录'),
        ),
    ]
