"""
此脚本用于拓展 Django 内部的功能
"""

# region Field 相关

from django.db.models.fields import *
from django.db.models.fields.related import *
from django.db.models.fields.files import FileField

# 新增属性声明
Field.convert = Field.load = None
Field.std_convert = Field.std_load = None


def getConverter(self, force=False):
	if force: return self.std_convert
	return self.convert or self.std_convert


def getLoader(self, force=False):
	if force: return self.std_load
	return self.load or self.std_load


Field.getConverter = getConverter
Field.getLoader = getLoader

# 转换类型过滤，为 any 则所有类型，为 None 则为 None 类型
Field.type_filter = ['any']
# 转换类型过滤（取反）
Field.type_exclude = []

Field.key_name = None

FileField.type_exclude = ['any']


def registerConvertFunc(field):
	def wrapper(func):
		field.convert = func
		return func

	return wrapper


def registerLoadFunc(field):
	def wrapper(func):
		field.load = func
		return func

	return wrapper


@registerConvertFunc(Field)
def convert(self, model, value): return value


@registerConvertFunc(DateField)
def convert(self, model, value):
	from .data_manager import DataLoader
	return DataLoader.convertDate(value)


@registerConvertFunc(TimeField)
def convert(self, model, value):
	from .data_manager import DataLoader
	return DataLoader.convertDateTime(value)


@registerConvertFunc(DateTimeField)
def convert(self, model, value):
	from .data_manager import DataLoader
	return DataLoader.convertDateTime(value)


@registerConvertFunc(ManyToManyField)
def convert(self, model, value):
	return [item.id for item in value]


@registerLoadFunc(Field)
def load(self, model, value, ori_val): return value


@registerLoadFunc(DateField)
def load(self, model, value, ori_val):
	from .data_manager import DataLoader
	return DataLoader.loadDate(value)


@registerLoadFunc(TimeField)
def load(self, model, value, ori_val):
	from .data_manager import DataLoader
	return DataLoader.loadDateTime(value)


@registerLoadFunc(DateTimeField)
def load(self, model, value, ori_val):
	from .data_manager import DataLoader
	return DataLoader.loadDateTime(value)


@registerLoadFunc(ManyToManyField)
def load(self, model, value, ori_val):
	if ori_val is None: return None
	ori_val.clear()
	ori_val.add(*value)
	return ori_val

# endregion
