"""
此脚本用于拓展 Django 内部的功能
"""

# region Field 相关

from django.db.models.fields import *
from django.db.models.fields.related import *

from .data_manager import DataLoader

# 新增属性声明
Field.convert = Field.load = None
# 转换类型过滤，为空则所有类型，形如：类型1|类型2...
Field.type_filter = ''
# 转换类型过滤（取反），为空则忽略
Field.type_exclude = ''


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
def convert(obj, value): return value


@registerConvertFunc(DateField)
def convert(obj, value):
	return DataLoader.convertDate(value)


@registerConvertFunc(TimeField)
def convert(obj, value):
	return DataLoader.convertDateTime(value)


@registerConvertFunc(DateTimeField)
def convert(obj, value):
	return DataLoader.convertDateTime(value)


@registerConvertFunc(ManyToManyField)
def convert(obj, value):
	return [item.id for item in value]


@registerLoadFunc(Field)
def load(obj, value, self): return value


@registerLoadFunc(DateField)
def load(obj, value, self):
	return DataLoader.loadDate(value)


@registerLoadFunc(TimeField)
def load(obj, value, self):
	return DataLoader.loadDateTime(value)


@registerLoadFunc(DateTimeField)
def load(obj, value, self):
	return DataLoader.loadDateTime(value)


@registerLoadFunc(ManyToManyField)
def load(obj, value, self):
	if self is None: return None
	self.clear()
	self.add(*value)
	return self

# endregion
