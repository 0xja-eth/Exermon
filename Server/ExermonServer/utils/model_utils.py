from django.db import models
from django.conf import settings
from django.db.models.query import QuerySet
from django.utils.deconstruct import deconstructible

from .data_manager import *

from enum import Enum
import os, random


# ===================================================
#  枚举映射类
# ===================================================
class EnumMapper:

	_Content = {}

	@classmethod
	def register(cls, enum: Enum, cla: type):
		"""
		注册枚举
		Args:
			enum (Enum): 枚举
			cla (type): 类型
		Returns:
			返回类本身（用于装饰器）
		"""
		cls._Content[enum] = cla
		return cla

	@classmethod
	def get(cls, enum: Enum):
		"""
		获取映射
		Args:
			enum (Enum): 枚举
		Returns:
			返回枚举对应的映射对象
		"""
		if enum in cls._Content:
			return cls._Content[enum]

		return None

	@classmethod
	def registerClass(cls, cla):
		"""
		注册物品/容器/容器项
		Args:
			cla (any): 物品/容器/容器项类型
		Returns:
			返回类本身（用于装饰器）
		"""
		return cls.register(cla.TYPE, cla)


# ===================================================
#  模型帮助类
# ===================================================
class ModelHelper:

	@staticmethod
	def getFuncClass(func):

		class_name = func.__qualname__.split('.')[0]
		return func.__globals__[class_name]

	# region Model 字段处理

	@classmethod
	def allKeys(cls, model: models.Model, include_id=False,
				include_m2m=True, return_name=True) -> list:
		"""
		获取模型的所有字段（包括父类）
		Args:
			model (type): 模型类
			include_id (bool): 是否包含 id 字段
			include_m2m (bool): 是否包含 M2M 关系键
			return_name (bool): 是否以字段名方式返回
		Returns:
			返回指定模型的所有字段
		"""

		map_key = 'name' if return_name else None
		excludes = ['id'] if not include_id else []

		res = cls.allMetaAttr(model, 'fields',
							  map_key, 'name', excludes)

		if include_m2m:
			res += cls.allMetaAttr(model, 'many_to_many',
								  map_key, 'name', excludes)

		return res

	@classmethod
	def localKeys(cls, model: models.Model, include_id=False,
				  include_m2m=True, return_name=True) -> list:
		"""
		获取模型本身字段（不包括父类）
		Args:
			model (type): 模型类
			include_id (bool): 是否包含 id 字段
			include_m2m (bool): 是否包含 M2M 关系键
			return_name (bool): 是否以字段名方式返回
		Returns:
			返回指定模型的本身字段
		"""

		map_key = 'name' if return_name else None
		excludes = ['id'] if not include_id else []

		res = cls.deltaMetaAttr(model, 'fields',
								map_key, 'name', excludes)

		if include_m2m:
			res += cls.deltaMetaAttr(model, 'many_to_many',
									 map_key, 'name', excludes)

		return res

	@classmethod
	def getKey(cls, model: models.Model, name):
		"""
		获取模型的指定字段对象
		Args:
			model (type): 模型类
			name (str): 字段名
		Returns:
			返回指定字段的对象
		"""
		from django.db.models import FieldDoesNotExist

		try: return model._meta.get_field(name)
		except FieldDoesNotExist: return None

	# endregion

	# region 关系处理

	@classmethod
	def allRelatedModels(cls, model: models.Model, excludes=[], return_model=True):
		"""
		获取所有的关联类
		Args:
			model (type): 模型类
			excludes (list): 例外
			return_model (bool): 是否以模型类形式返回
		Returns:
			返回所有关联的模型类
		"""

		map_key = 'related_model' if return_model else None

		return cls.allMetaAttr(model, 'related_objects',
							   map_key, 'related_model', excludes)

	@classmethod
	def localRelatedModels(cls, model: models.Model, excludes=[], return_model=True):
		"""
		获取所有的关联类（不包括父类）
		Args:
			model (type): 模型类
			excludes (list): 例外
			return_model (bool): 是否以模型类形式返回
		Returns:
			返回所有关联的模型类
		"""

		map_key = 'related_model' if return_model else None

		return cls.deltaMetaAttr(model, 'related_objects',
								 map_key, 'related_model', excludes)

	@classmethod
	def getRelatedObject(cls, model: models.Model,
						 related_model: models.Model):
		"""
		获取指定模型的关联对象（存在多个同模型关系全部无法获取的问题）
		Args:
			model (type): 模型类
			related_model (type): 关联模型类
		Returns:
			返回指定关联类的关联对象
		"""
		all_objs = cls.allRelatedModels(model, return_model=False)

		for obj in all_objs:
			if obj.related_model == related_model: return obj

		return None

	# endregion

	# region 属性获取

	@classmethod
	def allMetaAttr(cls, model: models.Model, key,
					map_key=None, judge_key=None, excludes=[]):
		"""
		所有Meta属性
		Args:
			model (type): 要获取属性的模型类
			key (str): 要获取的属性名
			map_key (str): 获取后映射的键名
			judge_key (str): 获取后判断的键名
			excludes (list): 例外的字段
		"""
		res = []

		if judge_key is None: judge_key = map_key

		# 枚举对应属性集合
		for val in getattr(model._meta, key):

			item = val if map_key is None \
				else getattr(val, map_key)

			judge = val if judge_key is None \
				else getattr(val, judge_key)

			if judge not in excludes:
				res.append(item)

		return res

	@classmethod
	def deltaMetaAttr(cls, model: models.Model,
					  key, map_key=None, judge_key=None, excludes=[]):
		"""
		与父类Meta相差的属性
		Args:
			model (type): 要获取属性的模型类
			key (str): 要获取的属性名
			map_key (str): 获取后映射的键名
			judge_key (str): 获取后判断的键名
			excludes (list): 例外的字段
		"""
		res = []
		base_cla = model.__bases__[0]

		if judge_key is None: judge_key = map_key

		# 是否包含 _meta 属性
		if hasattr(base_cla, '_meta') and \
			hasattr(base_cla._meta, key):
			base_vals = getattr(base_cla._meta, key)
		else:
			base_vals = []

		# 枚举对应属性集合
		for val in getattr(model._meta, key):
			# 去除重复项
			if val in base_vals: continue

			item = val if map_key is None \
				else getattr(val, map_key)

			judge = val if judge_key is None \
				else getattr(val, judge_key)

			if judge not in excludes:
				res.append(item)

		return res

	@classmethod
	def inhertAttr(cls, adminx_cla, key, val):
		"""
		继承属性
		"""
		base_cla = adminx_cla.__bases__[0]

		if hasattr(base_cla, key):
			base_val = getattr(base_cla, key)
			val = base_val + val

		val = list(set(val))

		if 'id' in val:
			val.remove('id')
			val.insert(0, 'id')

		setattr(adminx_cla, key, val)

	# endregion

	"""分隔符"""

# region 公用模型

# ===================================================
#  Model封装拓展
# ===================================================
class BaseModel(models.Model):

	class Meta:
		abstract = True

	# region Adminx 配置

	LIST_DISPLAY = None
	LIST_DISPLAY_APPEND = []
	LIST_DISPLAY_EXCLUDE = []

	LIST_EDITABLE = None
	LIST_EDITABLE_APPEND = []
	LIST_EDITABLE_EXCLUDE = []

	INLINE_MODELS = None
	EXCLUDE_INLINES = []

	# endregion

	# region 转换/读取配置

	# 键名（关联对象）
	KEY_NAME = None

	# 转化类型与字段设定
	TYPE_FIELD_FILTER_MAP = {}
	TYPE_FIELD_EXCLUDE_MAP = {}

	# 转化类型与关系设定
	TYPE_RELATED_FILTER_MAP = {}
	TYPE_RELATED_EXCLUDE_MAP = {}

	# endregion

	@classmethod
	@CacheHelper.staticCache
	def _getFields(cls):
		return ModelHelper.allKeys(cls, return_name=False)

	@classmethod
	def _getField(cls, name):
		return ModelHelper.getKey(cls, name)

	@classmethod
	@CacheHelper.staticCache
	def _getRelatedModels(cls):
		return ModelHelper.allRelatedModels(cls)

	@classmethod
	@CacheHelper.staticCache
	def _getRelatedObjects(cls):
		return ModelHelper.allRelatedModels(cls, return_model=False)

	@classmethod
	def _getRelatedObject(cls, model):
		return ModelHelper.getRelatedObject(cls, model)

	@classmethod
	@CacheHelper.staticCache
	def typeFieldFilterMap(cls):
		return cls._getFilterMap(
			'TYPE_FIELD_FILTER_MAP', 'typeFieldFilterMap')

	@classmethod
	@CacheHelper.staticCache
	def typeFieldExcludeMap(cls):
		return cls._getFilterMap(
			'TYPE_FIELD_EXCLUDE_MAP', 'typeFieldExcludeMap')

	@classmethod
	@CacheHelper.staticCache
	def typeRelatedFilterMap(cls):
		return cls._getFilterMap(
			'TYPE_RELATED_FILTER_MAP', 'typeRelatedFilterMap')

	@classmethod
	@CacheHelper.staticCache
	def typeRelatedExcludeMap(cls):
		return cls._getFilterMap(
			'TYPE_RELATED_EXCLUDE_MAP', 'typeRelatedExcludeMap')

	@classmethod
	def _getFilterMap(cls, key_name, func_name):
		base_classes = cls.__bases__

		res = getattr(cls, key_name, {})

		for cla in base_classes:
			if hasattr(cla, func_name):
				base_res = getattr(cla, func_name)()

				for key in base_res:
					if key not in res: res[key] = base_res[key]
					else: res[key] += base_res[key]

		return res

	# @classmethod
	# @CacheHelper.staticCache
	# def doNotAutoConvertFields(cls):
	# 	base_classes = cls.__bases__
	#
	# 	res = cls.DO_NOT_AUTO_CONVERT_FIELDS
	#
	# 	for cla in base_classes:
	# 		if hasattr(cla, 'doNotAutoConvertFields'):
	# 			res += cla.doNotAutoConvertFields()
	#
	# 	return res
	#
	# @classmethod
	# @CacheHelper.staticCache
	# def doNotAutoLoadFields(cls):
	# 	base_classes = cls.__bases__
	#
	# 	res = cls.DO_NOT_AUTO_LOAD_FIELDS
	#
	# 	for cla in base_classes:
	# 		if hasattr(cla, 'doNotAutoLoadFields'):
	# 			res += cla.doNotAutoLoadFields()
	#
	# 	return res
	#
	# @classmethod
	# @CacheHelper.staticCache
	# def autoFieldKeyNames(cls):
	# 	base_classes = cls.__bases__
	#
	# 	res = cls.AUTO_FIELDS_KEY_NAMES
	#
	# 	for cla in base_classes:
	# 		if hasattr(cla, 'autoFieldKeyNames'):
	# 			tmp = cla.autoFieldKeyNames()
	# 			res = {**res, **tmp}
	#
	# 	return res

	def __init__(self, *args, _data=None, **kwargs):
		super().__init__(*args, **kwargs)

		if _data is not None: self.load(_data, **kwargs)

	# 从数据库获取
	def _getOneToOneModelInDb(self, cla):
		name = cla.__name__.lower()
		try:
			return getattr(self, name)
		# 捕捉到异常就跳过
		except cla.DoesNotExist: return None
		except AttributeError: return None

	# 从数据库获取
	def _getForeignKeyModelInDb(self, cla) -> QuerySet:
		name = cla.__name__.lower() + '_set'
		try:
			return getattr(self, name).all()
		except AttributeError:
			return None

	def __getFieldAndKeyName(self, field=None, field_name=None, mode='convert'):

		from django.db.models.fields.related import ForeignKey

		# 获取字段名
		if field_name is None:
			field_name = field.name
		if isinstance(field, ForeignKey):
			field_name = field_name + '_id'

		# 获取键名
		key_name = getattr(field, 'key_name', None)
		if key_name is None: key_name = field_name

		process_func = '__%sGeneralField' % mode
		process_func = getattr(self, process_func, None)

		return process_func, field_name, key_name

	def __getRelatedAttrAndKeyName(self, object, model, mode='convert'):
		from django.db.models.fields.reverse_related import \
			ManyToOneRel, OneToOneRel

		name = model.__name__
		process_func = attr_name = key_name = None

		if isinstance(object, ManyToOneRel):
			process_func = "__%sManyToOneRelatedObject" % mode

			attr_name = "%s_set" % name.lower()

			key_name = getattr(model, 'KEY_NAME', None)
			if key_name is None:
				key_name = "%ss" % DataManager.hump2Underline(name)

		elif isinstance(object, OneToOneRel):
			process_func = "__%sOneToOneRelatedObject" % mode

			attr_name = name.lower()

			key_name = getattr(model, 'KEY_NAME', None)
			if key_name is None:
				key_name = DataManager.hump2Underline(name)

		# key_names = self.autoFieldKeyNames()
		# if key_name in key_names:
		# 	key_name = key_names[key_name]

		process_func = getattr(self, process_func, None)

		return process_func, attr_name, key_name

	def __getFieldNameList(self, ori_list):
		from django.db.models import Field

		res = []

		for item in ori_list:
			if isinstance(item, Field):
				res.append(item.name)
			elif isinstance(item, type):
				res.append(item.__name__)
			elif isinstance(item, str):
				res.append(item)

		return res

	# region 转化

	@classmethod
	def convertFields(cls, keys, *key_list):

		key_list = list(key_list)
		key_list += keys.split(',')
		key_list = [key.strip() for key in key_list]

		def wrapper(func):

			# if not_convert:
			# 	cla: BaseModel = ModelHelper.getFuncClass(func)
			# 	cla.DO_NOT_AUTO_CONVERT_FIELDS += key_list

			def funcWrapper(self: BaseModel, res, **kwargs):

				self._convertCustomFields(res, *key_list)

				func(self, res, **kwargs)

			return funcWrapper

		return wrapper

	# 转化自动属性
	def _convertAutoAttrs(self, res, type=None, force=False):

		self._convertFields(res, type, force)
		self._convertRelatedObjects(res, type, force)

	# region 字段

	# 转换所有字段
	def _convertFields(self, res, type=None, force=False):

		for field in self._getFields():

			if force or self.__filterField(type, field):

				self._convertField(res, field, force=force)

	def __filterField(self, type, field):

		# 处理字段中的 exclude，若有则返回 False
		type_exclude = getattr(field, 'type_exclude', [])
		if 'any' in type_exclude or type in type_exclude:
			return False

		# 处理字段中的 filter，若找不到返回 False
		type_filter = getattr(field, 'type_filter', ['any'])
		if 'any' not in type_filter and type not in type_filter:
			return False

		# 处理映射 filter
		filter, filter_flag = [], False
		filter_map = self.typeFieldFilterMap()

		# 生成 filter 列表
		if type in filter_map:
			filter += filter_map[type]
			filter_flag = True

		if 'any' in filter_map:
			filter += filter_map['any']
			filter_flag = True

		if filter_flag:
			filter = self.__getFieldNameList(filter)
			if field.name not in filter: return False

		# 处理映射 exclude
		exclude, exclude_flag = [], False
		exclude_map = self.typeFieldExcludeMap()

		# 生成 exclude 列表
		if 'any' in exclude_map:
			exclude += exclude_map['any']
			exclude_flag = True

		if type in exclude_map:
			exclude += exclude_map[type]
			exclude_flag = True

		if exclude_flag:
			exclude = self.__getFieldNameList(exclude)
			if field.name not in exclude: return False

		return True

	# 转换单个字段
	def _convertField(self, res, field=None, field_name=None, force=False):

		if field is None: field = self._getField(field_name)

		if not force and not field.serialize: return

		process_func, field_name, key_name = \
			self.__getFieldAndKeyName(field, field_name, 'convert')
		if process_func is None: return

		# 判断
		# if not force and field_name in self.doNotAutoConvertFields(): return
		if not hasattr(self, field_name): return

		process_func(res, field, field_name, key_name, force)

	# 转化常规字段
	def __convertGeneralField(self, res, field, field_name, key_name, force):

		value = getattr(self, field_name)

		if hasattr(field, 'getConverter'):
			value = field.getConverter(force)(self, value)

		res[key_name] = value

	# # 转化多对多字段（id数组）
	# def __convertManyToManyField(self, res, field, field_name, key_name):
	#
	# 	res[key_name] = []
	#
	# 	m2m_manager = getattr(self, field_name)
	#
	# 	for obj in m2m_manager.all():
	# 		res[key_name].append(obj.id)

	# endregion

	# region 关系

	# 转换所有关联对象
	def _convertRelatedObjects(self, res, type=None, force=False):

		for object in self._getRelatedModels():

			if force or self.__filterRelatedObject(type, object):

				self._convertRelatedObject(res, object, type=type, force=force)

	def __filterRelatedObject(self, type, object):

		base_clas = object.related_model.mro()

		filter, exclude = [], []
		filter_flag = exclude_flag = False

		filter_map = self.typeRelatedFilterMap()

		if type in filter_map:
			filter += filter_map[type]
			filter_flag = True

		if 'any' in filter_map:
			filter += filter_map['any']
			filter_flag = True

		if filter_flag:
			filter = self.__getFieldNameList(filter)
			if all(cla.__name__ not in filter
				   for cla in base_clas): return False

		exclude_map = self.typeRelatedExcludeMap()

		if 'any' in exclude_map:
			exclude += exclude_map['any']
			exclude_flag = True

		if type in exclude_map:
			exclude += exclude_map[type]
			exclude_flag = True

		if exclude_flag:
			exclude = self.__getFieldNameList(exclude)
			if all(cla.__name__ not in exclude
				   for cla in base_clas): return False

		return True

	# 转化单个关联对象
	def _convertRelatedObject(self, res, object=None, model=None,
							  type=None, force=False):

		if object is None: object = self._getRelatedObject(model)

		if model is None: model = object.related_model

		if CoreData in model.mro(): return

		process_func, attr_name, key_name = \
			self.__getRelatedAttrAndKeyName(object, model, 'convert')
		if process_func is None: return

		# 判断
		# if not force and attr_name in self.doNotAutoConvertFields(): return
		if not hasattr(self, attr_name): return

		process_func(res, attr_name, key_name, model, type, force)

	def __convertManyToOneRelatedObject(
			self, res, attr_name, key_name, model, type=None, force=False):

		objects = getattr(self, attr_name).all()
		res[key_name] = Common.objectsToDict(objects, type=type, force=force)

	def __convertOneToOneRelatedObject(
			self, res, attr_name, key_name, model, type=None, force=False):

		object = self._getOneToOneModelInDb(model)
		res[key_name] = object.convert(type=type, force=force)

	# endregion

	# 转化自定义属性
	def _convertCustomAttrs(self, res, type=None, **kwargs): pass

	# 自定义转化字段
	def _convertCustomFields(self, res, *keys):
		for key in keys:
			self._convertField(res, field_name=key, force=True)

	# 自定义转化关系
	def _convertCustomRelates(self, res, *models):
		for model in models:
			self._convertRelatedObject(res, model=model, force=True)

	# 转换为字典（包括程序里转化以及导出转化功能）
	def convert(self, type=None, force=False, **kwargs):
		res = {}

		if force:
			self._convertAutoAttrs(res, force=True)
		else:
			self._convert(res, type, **kwargs)

		return res

	def _convert(self, res, type=None, **kwargs):

		# 如果传入了 type 参数，且定义了 _convert...Data 函数
		# 则不会自动转化属性（默认方式）
		# _convertCustomAttrs 函数中可以继续进行自定义处理
		self._convertAutoAttrs(res, type)

		if type is not None:
			type_name = DataManager.underline2UpperHump(type)
			method_name = "_convert%sData" % type_name

			func = getattr(self, method_name, None)
			if func is not None: func(res, **kwargs)

		self._convertCustomAttrs(res, type, **kwargs)

	# endregion

	# region 读取（导入）

	# 读取自动属性
	def _loadAutoAttrs(self, data):

		self._loadFields(data)
		self._loadRelatedObjects(data)

	# region 字段

	# 读取字段
	def _loadFields(self, data):

		for field in self._getFields(): self._loadField(data, field)

	def _loadField(self, data, field=None, field_name=None):

		if field is None: field = self._getField(field_name)

		process_func, field_name, key_name = \
			self.__getFieldAndKeyName(field, field_name, 'convert')
		if process_func is None: return

		# 判断
		# if field_name in self.doNotAutoLoadFields(): return
		if key_name not in data: return

		process_func(data, field, field_name, key_name)

	def __loadGeneralField(self, data, field, field_name, key_name):

		value = data[key_name]

		if hasattr(field, 'getLoader'):
			ori_obj = getattr(self, field_name, None)
			value = field.getLoader()(self, value, ori_obj)

		setattr(self, field_name, value)

	# def __loadManyToManyField(self, data, field, field_name, key_name):
	#
	# 	m2m_manager = getattr(self, field_name, None)
	# 	if m2m_manager is None: return
	#
	# 	m2m_manager.clear()
	#
	# 	for id in data[key_name]: m2m_manager.add(id)

	# endregion

	# region 关系

	def _loadRelatedObjects(self, data):

		related_objects = self._getRelatedModels()

		for object in related_objects:
			self._loadRelatedObject(data, object)

	def _loadRelatedObject(self, data, object=None, model=None):

		if object is None: object = self._getRelatedObject(model)

		if model is None: model = object.related_model

		if CoreData in model.mro(): return

		process_func, attr_name, key_name = \
			self.__getRelatedAttrAndKeyName(object, model, 'load')
		if process_func is None: return

		# 判断
		# if attr_name in self.doNotAutoLoadFields(): return
		if key_name not in data: return

		process_func(data, attr_name, key_name, model)

	def __loadManyToOneRelatedObject(
			self, data, attr_name, key_name, model):

		# 获取新旧数据
		set_manager = getattr(self, attr_name, None)
		if set_manager is None: return

		ori_objs = set_manager.all()
		new_data = data[key_name]

		ori_cnt, new_cnt = len(ori_objs), len(new_data)

		# 遍历原始数据
		for i in range(ori_cnt):
			ori_obj: BaseModel = ori_objs[i]
			if i >= new_cnt:
				ori_obj.delete()
			else:
				ori_obj.load(new_data[i])

		# 对剩下的新数据进行遍历
		if new_cnt > ori_cnt:
			for i in range(ori_cnt, new_cnt):
				new_obj = model(_data=data[key_name])
				set_manager.add(new_obj)

	def __loadOneToOneRelatedObject(
			self, data, attr_name, key_name, model):

		obj = model(_data=data[key_name])
		setattr(self, attr_name, obj)

	# endregion

	# 读取自定义属性
	def _loadCustomAttrs(self, data, **kwargs): pass

	# 读取函数
	def load(self, data, **kwargs):
		self._loadAutoAttrs(data)
		self._loadCustomAttrs(data, **kwargs)

		self.save()

	# endregion

	"""占位符"""

# region 核心（静态/动态/游戏资料）数据类型


# ===================================================
#  核心数据管理类
# ===================================================
class CoreDataManager:

	# 获取所有游戏资料数据类
	@classmethod
	def getCoreData(cls):
		return cls._getSubModels(CoreData)

	# 获取所有游戏资料数据类
	@classmethod
	def getGameData(cls):
		return cls._getSubModels(GameData)

	# 获取所有游戏资料数据类
	@classmethod
	def getStaticData(cls):
		return cls._getSubModels(StaticData)

	# 获取所有游戏资料数据类
	@classmethod
	def getDynamicData(cls):
		return cls._getSubModels(DynamicData)

	# 递归地获取一个模型类的所有子类
	@classmethod
	def _getSubModels(cls, model: type):
		res = []
		cls.__doGetSubModels(res, model)

		return res

	@classmethod
	def __doGetSubModels(cls, res, model: type):

		if hasattr(model, '_meta'):
			if not model._meta.abstract:
				res.append(model)

		for sub in model.__subclasses__():
			if sub in res: continue

			cls.__doGetSubModels(res, sub)


# ===================================================
#  核心数据
# ===================================================
class CoreData(BaseModel):

	class Meta:
		abstract = True

	# 没能找到时候抛出的异常
	NOT_EXIST_ERROR = ErrorType.UnknownError

	# region 数据获取

	@classmethod
	def setup(cls):
		"""
		读取数据
		"""
		DataManager.setObjects(cls, **cls._setupKwargs())

	# 读取参数
	@classmethod
	def _setupKwargs(cls): return {}

	@classmethod
	def get(cls, cond: callable = None, **kwargs):
		"""
		根据条件获取单个数据
		Args:
			cond (callable): 查询函数
			**kwargs (**dict): 查询条件
		Returns:
			按照条件返回指定数据（若有多个返回第一个）
		Raises:
			若不存在，抛出事先设置的异常（NOT_EXIST_ERROR）
		Examples:
			获取 id 为 3 的科目：
			subject = Subject.get(id=3)
			获取 属性缩写 为 mhp 的属性：
			param = BaseParam.get(attr='mhp')
		"""
		if not DataManager.contains(cls): cls.setup()

		return DataManager.getObject(cls, cond, **kwargs)

	@classmethod
	def ensure(cls, cond: callable = None, **kwargs):
		"""
		确保指定条件的数据存在
		Args:
			cond (callable): 查询函数
			**kwargs (**dict): 查询条件
		Raises:
			若不存在，抛出事先设置的异常（NOT_EXIST_ERROR）
		Examples:
			确保 id 为 3 且名字为 英语 的科目存在：
			Subject.ensure(id=3, name="英语")
		"""
		if not DataManager.contains(cls): cls.setup()

		DataManager.ensureObjects(cls, cls.NOT_EXIST_ERROR, cond, **kwargs)

	@classmethod
	def objs(cls, cond: callable = None, **kwargs) -> QuerySet or list:
		"""
		按照一定条件获取多个数据
		Args:
			cond (callable): 查询函数
			**kwargs (**dict): 查询条件
		Returns:
			返回符合指定条件的数据列表
		Examples:
			获取全部属性数据：
			params = BaseParam.objs()
			获取分值为 150 的所有科目数据：
			subjects = Subject.objs(max_score=150)
		"""
		if not DataManager.contains(cls): cls.setup()

		return DataManager.queryObjects(cls, cond, **kwargs)

	@classmethod
	def count(cls, cond: callable = None, **kwargs) -> int:
		"""
		获取数据的数量
		Args:
			cond (callable): 查询函数
			**kwargs (**dict): 查询条件
		Returns:
			返回该数据在数据库中的数量
		"""
		if not DataManager.contains(cls): cls.setup()

		return DataManager.getCount(cls, cond, **kwargs)

	# endregion

	# region 数据迁移

	# 导出数据
	def export(self):
		return self.convert(force=True)

	# endregion

	"""占位符"""


# ===================================================
#  游戏数据（游戏资料）
# ===================================================
class GameData(CoreData):

	class Meta:
		abstract = True


# ===================================================
#  静态数据（游戏配置数据）
# ===================================================
class StaticData(CoreData):

	class Meta:
		abstract = True

	# 所属配置
	configure = models.ForeignKey('game_module.GameConfigure', on_delete=models.CASCADE, verbose_name="所属配置")

	# 名称
	name = models.CharField(max_length=8, verbose_name="名称")

	# 描述
	description = models.CharField(max_length=64, blank=True, verbose_name="描述")

	def __str__(self):
		return self.name

	# 读取参数
	@classmethod
	def _setupKwargs(cls):
		from game_module.models import GameConfigure

		configure: GameConfigure = GameConfigure.get()

		return {"configure": configure}


# ===================================================
#  动态数据（动态更新，定时清除缓存）
# ===================================================
class DynamicData(CoreData):

	class Meta:
		abstract = True

	# 名称
	name = models.CharField(max_length=8, verbose_name="名称")

	# 描述
	description = models.CharField(max_length=64, blank=True, verbose_name="描述")


# endregion

# ===================================================
#  缓存机制模型
# ===================================================
class CacheableModel(BaseModel, CacheableObject):

	class Meta:
		abstract = True

	def __init__(self, *args, **kwargs):
		self.saved = True
		self.delete_save = False

		self.cache_values = {}
		self.cache_pool = CachePool()

		# self._setupCachePool()

		super().__init__(*args, **kwargs)

	# 配置缓存池
	def _setupCachePool(self):
		super()._setupCachePool()

		for cla in self._cacheOneToOneModels():
			if cla is None: continue

			def genReloadFunc():
				_cla = cla

				def reloadFunc():
					return [self._getOneToOneModelInDb(_cla)]

				return reloadFunc

			self._setModelCache(key=cla, reload_func=genReloadFunc())

		for cla in self._cacheForeignKeyModels():
			if cla is None: continue
			objs = self._getForeignKeyModelInDb(cla)
			self._setModelCache(key=cla, objects=objs)

	# region 关系操作

	# 需要缓存的模型类列表（必须为 OneToOne）
	@classmethod
	def _cacheOneToOneModels(cls): return []

	# 需要缓存的模型类列表（必须为 外键）
	@classmethod
	def _cacheForeignKeyModels(cls): return []

	def _getOneToOneCache(self, key) -> models.Model:
		if key in self._cacheOneToOneModels():
			return self._firstModelCache(key)
		return None

	def _getForeignKeyCache(self, key, cond: callable = None, **kwargs) -> models.Model:
		if key in self._cacheForeignKeyModels():
			return self._getModelCache(key, cond, **kwargs)
		return None

	def _getForeignKeyCaches(self, key, cond: callable = None, **kwargs) -> models.Model:
		if key in self._cacheForeignKeyModels():
			return self._queryModelCache(key, cond, **kwargs)
		return None

	# endregion

	def __del__(self):
		if self.delete_save: self.save()

	def __setattr__(self, key, value):
		if key != 'saved': self.saved = False
		super().__setattr__(key, value)

	def save(self, *args, **kwargs):

		if not self.saved:
			super().save(*args, **kwargs)
			# print(str(self) + " saved!")
			self.saved = True

		self.cache_pool.saveAll()

	"""占位符"""

# endregion


# region 文件上传类

# ===================================================
#  图片上传处理（父类）
# ===================================================
@deconstructible
class ImageUpload:

	# 随机字符集
	CHARSET = 'zyxwvutsrqponmlkjihgfedcba'

	# 随机字符串长度
	RANDOM_LEN = 5

	def __init__(self, _dir):
		self.dir = _dir

	def __call__(self, instance, filename):
		base = os.path.join(settings.BASE_DIR, settings.STATIC_BASE)

		path = os.path.join(base, self.dir, self.generateFileName(instance, filename))

		return path

	def generateFileName(self, instance, filename):
		return filename

# ===================================================
#  系统图片上传处理
# ===================================================
@deconstructible
class SystemImageUpload(ImageUpload):

	SYSTEM_DIR = 'system'
	IMAGE_DIR = ''

	def __init__(self, _dir=None):
		if _dir is None: _dir = self.IMAGE_DIR
		_dir = os.path.join(self.SYSTEM_DIR, _dir)
		super().__init__(_dir)

# ===================================================
#  人物图片
# ===================================================
@deconstructible
class CharacterImageUpload(SystemImageUpload):

	IMAGE_DIR = 'character'

	BUST_DIR = 'character/bust'
	FACE_DIR = 'character/face'
	BATTLE_DIR = 'character/battle'

	def __init__(self, type):
		_dir = None
		if type == 'bust': _dir = self.BUST_DIR
		if type == 'face': _dir = self.FACE_DIR
		if type == 'battle': _dir = self.BATTLE_DIR

		super().__init__(_dir)

	def generateFileName(self, instance, filename):

		# 文件拓展名
		ext = os.path.splitext(filename)[1]

		# 定义文件名
		filename = "character_%d" % instance.id

		return filename+ext

# ===================================================
#  物品图标
# ===================================================
@deconstructible
class ItemIconUpload(SystemImageUpload):

	IMAGE_DIR = 'item/icon'

	def generateFileName(self, instance, filename):

		# 文件拓展名
		ext = os.path.splitext(filename)[1]

		# 定义文件名
		filename = "item_%d" % instance.id

		return filename+ext

# ===================================================
#  艾瑟萌图片
# ===================================================
@deconstructible
class ExermonImageUpload(SystemImageUpload):

	IMAGE_DIR = 'exermon'

	FULL_DIR = 'exermon/full'
	ICON_DIR = 'exermon/icon'
	BATTLE_DIR = 'exermon/battle'

	def __init__(self, type):
		_dir = None
		if type == 'full': _dir = self.FULL_DIR
		if type == 'icon': _dir = self.ICON_DIR
		if type == 'battle': _dir = self.BATTLE_DIR

		super().__init__(_dir)

	def generateFileName(self, instance, filename):

		# 文件拓展名
		ext = os.path.splitext(filename)[1]

		# 定义文件名
		filename = "exermon_%d" % instance.id

		return filename+ext

# ===================================================
#  技能图标
# ===================================================
@deconstructible
class SkillImageUpload(SystemImageUpload):

	IMAGE_DIR = 'exermon/skill'

	ICON_DIR = 'exermon/skill/icon'
	ANI_DIR = 'exermon/skill/ani'
	EFFECT_DIR = 'exermon/skill/target'

	def __init__(self, type):
		_dir = None
		if type == 'icon': _dir = self.ICON_DIR
		if type == 'ani': _dir = self.ANI_DIR
		if type == 'target': _dir = self.EFFECT_DIR

		super().__init__(_dir)

	def generateFileName(self, instance, filename):

		# 文件拓展名
		ext = os.path.splitext(filename)[1]

		# 定义文件名
		filename = "skill_%d" % instance.id

		return filename+ext


# ===================================================
#  题目图片
# ===================================================
@deconstructible
class QuestionImageUpload(SystemImageUpload):

	IMAGE_DIR = 'question/picture'

	def generateFileName(self, instance, filename):

		# 文件拓展名
		ext = os.path.splitext(filename)[1]

		# 定义文件名
		rand_str = random.sample(self.CHARSET, self.RANDOM_LEN)
		filename = "question_%s" % rand_str

		return filename+ext


# ===================================================
#  剧情题目图标
# ===================================================
@deconstructible
class PlotQuestionImageUpload(SystemImageUpload):

	IMAGE_DIR = 'plot_question/picture'

	def generateFileName(self, instance, filename):

		# 文件拓展名
		ext = os.path.splitext(filename)[1]

		# 定义文件名
		rand_str = random.sample(self.CHARSET, self.RANDOM_LEN)
		filename = "question_%s" % rand_str

		return filename+ext


# ===================================================
#  题目音频
# ===================================================
@deconstructible
class QuestionAudioUpload(SystemImageUpload):

	IMAGE_DIR = 'question/audio'

	def generateFileName(self, instance, filename):

		# 文件拓展名
		ext = os.path.splitext(filename)[1]

		# 定义文件名
		rand_str = random.sample(self.CHARSET, self.RANDOM_LEN)
		filename = "question_%s" % rand_str

		return filename+ext

# endregion


# ============================================
# 公用类：处理模型函数的共有业务逻辑
# ============================================
class Common:

	# @classmethod
	# def preventNone(cls, judge, value=None, obj=None, func=None, empty=None):
	# 	if judge is None: return empty
	# 	if value is not None: return value
	# 	if obj is None: obj = judge
	# 	return func(obj)

	@classmethod
	def timeToStr(cls, time, empty=None):
		from .data_manager import DataLoader
		return DataLoader.convertDateTime(time, empty)

	@classmethod
	def dateToStr(cls, time, empty=None):
		from .data_manager import DataLoader
		return DataLoader.convertDate(time, empty)

	# 获取一个QuerySet的相关属性集合
	# objects: QuerySet
	@classmethod
	def getObjectRelated(cls, objects, key, unique=False):
		result = []

		for obj in objects:
			result.append(getattr(obj, key))

		if unique: result = list(set(result))
		return result

	@classmethod
	def getObjectRelatedForAll(cls, objects: QuerySet, select_key: str,
							   key: str = None, unique: bool = False) -> list:
		"""
		通过 all() 获取一个 QuerySet 的相关属性
		Args:
			objects (QuerySet): 源 QuerySet
			select_key (str): 用于 select_related 的关联的键名
			key (str): 要获取的关联的键名
			unique (bool): 元素是否唯一
		Returns:
			返回所有转化后的由对应键的值组成的数组
		Examples:
			获取玩家所选的科目：先获取艾瑟萌槽，对每个艾瑟萌槽项获取其科目：
				exerslot = player.exerSlot()  # 先获取装备槽
				slot_items = exerslot.contItems()  # 在获取所有槽项

				# 最后通过该函数获取科目
				subjects = ModelUtils.getObjectRelatedForAll(slot_items, 'subject')
		"""
		if key is None: key = select_key
		temp = objects.select_related(select_key).all()
		return cls.getObjectRelated(temp, key, unique)

	@classmethod
	def getObjectRelatedForFilter(cls, objects: QuerySet, select_key: str,
								  key: str = None, unique: bool = False, **kwargs) -> list:
		"""
		通过 filter() 获取一个 QuerySet 的相关属性
		Args:
			objects (QuerySet): 源 QuerySet
			select_key (str): 用于 select_related 的关联的键名
			key (str): 要获取的关联的键名
			unique (bool): 元素是否唯一
			**kwargs (**dict): 过滤参数（与 QuerySet.filter 的参数一致）
		Returns:
			返回按一定条件转化后的由对应键的值组成的数组
		Examples:
			获取玩家对应槽项等级大于10的科目：先获取艾瑟萌槽，对每个艾瑟萌槽项获取其科目：
				exerslot = player.exerSlot()  # 先获取装备槽
				slot_items = exerslot.contItems()  # 在获取所有槽项

				# 最后通过该函数获取科目
				subjects = ModelUtils.getObjectRelatedForFilter(slot_items, 'subject', level_gt=10)
		"""
		if key is None: key = select_key
		temp = objects.select_related(select_key).filter(**kwargs)
		return cls.getObjectRelated(temp, key, unique)

	# 物体集转化为字典
	@classmethod
	def objectsToDict(cls, objects, **kwargs):
		result = []

		if objects is None: return []
		for obj in objects:
			result.append(obj.convert(**kwargs))

		return result

	# 物体转化为字典
	@classmethod
	def objectToDict(cls, object, **kwargs):

		if object is None: return {}
		return object.convert(**kwargs)

	@classmethod
	def sum(cls, list_: list, map: callable = None, **kwargs) -> object:
		"""
		对所有符合数据进行求和（ list_ 中的元素或通过 map 之后的值必须实现了 __add__ 方法）
		Args:
			list_ (list): 要查询的列表
			map (callable): 映射函数
			**kwargs (**dict): 查询参数
		Returns:
			返回求和后的对象，没有则返回 None
		"""
		if list_ is None or len(list_) <= 0: return None

		sum = None

		for item in list_:
			flag = True
			for key in kwargs:
				val = kwargs[key]
				flag = (hasattr(item, key) and getattr(item, key) == val)
				if not flag: break

			if flag:
				val = (map(item) if map else item)
				if sum is None: sum = val
				else: sum += val

		return sum

	@classmethod
	def mult(cls, list_: list, map: callable = None, **kwargs) -> object:
		"""
		对所有符合数据进行求积（ list_ 中的元素或通过 map 之后的值必须实现了 __mul__ 方法）
		Args:
			list_ (list): 要查询的列表
			map (callable): 映射函数
			**kwargs (**dict): 查询参数
		Returns:
			返回求积后的对象，没有则返回 None
		"""
		if list_ is None or len(list_) <= 0: return None

		sum = None

		for item in list_:
			flag = True
			for key in kwargs:
				val = kwargs[key]
				flag = (hasattr(item, key) and getattr(item, key) == val)
				if not flag: break

			if flag:
				val = (map(item) if map else item)
				if sum is None: sum = val
				else: sum *= val

		return sum

	@classmethod
	def get(cls, list_: list, map: callable = None, **kwargs) -> object:
		"""
		获取单个符合数据（类似不带条件参数的 QuerySet.get()）
		Args:
			list_ (list): 要查询的列表
			map (callable): 映射函数
			**kwargs (**dict): 查询参数
		Returns:
			返回获得到的对象（多个则返回第一个），没有则返回 None
		"""
		if list_ is None: return None

		for item in list_:
			flag = True
			for key in kwargs:
				val = kwargs[key]
				flag = (hasattr(item, key) and getattr(item, key) == val)
				if not flag: break

			if flag: return map(item) if map else item

		return None

	@classmethod
	def query(cls, list_: list, map: callable = None, **kwargs) -> list:
		"""
		查询过滤（类似不带条件参数的 QuerySet.filter()）
		Args:
			list_ (list): 要查询的列表
			map (callable): 映射函数
			**kwargs (**dict): 查询参数
		Returns:
			返回过滤后的对象组成的列表
		"""
		res = []

		if list_ is None: return res

		for item in list_:
			flag = True
			for key in kwargs:
				val = kwargs[key]
				flag = (hasattr(item, key) and getattr(item, key) == val)
				if not flag: break

			if flag:
				if map: res.append(map(item))
				else: res.append(item)

		return res

	@classmethod
	def filter(cls, list_: list, cond: callable = None, map: callable = None) -> list:
		"""
		条件过滤（类似带条件参数的 QuerySet.filter()）
		Args:
			list_ (list): 要过滤的列表
			cond (callable): 过滤条件函数
			map (callable): 映射函数
		Returns:
			返回过滤后的对象组成的列表
		"""
		res = []

		if list_ is None: return res

		for item in list_:
			if cond and cond(item):
				if map: res.append(map(item))
				else: res.append(item)

		return res
	
	@classmethod
	def loadKey(cls, data, key, obj=None, attr=None, set_none=False):
		"""
		读取键
		Args:
			data (dict): 字典
			key (str): 字典键
			obj (object): 对象
			attr (str): 属性键
			set_none (bool): 是否设置 None 值
		Returns:
			返回字典指定键的值
		"""
		value = None
		if key in data: value = data[key]

		if obj is not None:

			if attr is None: attr = key
			if value is not None: setattr(obj, attr, value)
			elif set_none: setattr(obj, attr, None)

		return value
