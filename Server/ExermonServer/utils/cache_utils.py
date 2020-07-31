from django.db import models
from django.db.models.query import QuerySet
from .exception import *


class CacheHelper:
	"""
	缓存帮助类
	"""
	@classmethod
	def cacheValName(cls, func):
		return "_cached_%s" % func.__name__

	@classmethod
	def staticCache(cls, func):
		"""
		静态缓存，无法进行缓存清除处理
		"""
		cache_val_name = cls.cacheValName(func)

		def wrapper(self):

			# 如果缓存属性不存在，或者缓存值为None
			if not hasattr(self, cache_val_name) or \
					getattr(self, cache_val_name) is None:

				setattr(self, cache_val_name, func(self))

			return getattr(self, cache_val_name)

		return wrapper

	@classmethod
	def normalCache(cls, func):
		"""
		普通缓存，可以进行缓存清除处理
		"""
		cache_val_name = cls.cacheValName(func)

		def wrapper(self):

			# 如果缓存属性不存在，或者缓存值为None
			if not hasattr(self, cache_val_name) or \
					getattr(self, cache_val_name) is None:

				setattr(self, cache_val_name, func(self))

			return getattr(self, cache_val_name)

		wrapper.cache_val_name = cache_val_name

		return wrapper

	@classmethod
	def clearCache(cls, obj, *funcs):
		"""
		清除缓存
		Args:
			obj (object): 对象
			*funcs (*tuple): 缓存的函数
		"""

		for func in funcs:
			if hasattr(func, 'cache_val_name') and \
				func.cache_val_name is not None:

				setattr(obj, func.cache_val_name, None)

	@classmethod
	def clearAllCache(cls, obj):
		"""
		清除所有缓存
		Args:
			obj (object): 对象
		"""
		for key in dir(obj):
			func = getattr(obj, key)
			if hasattr(func, 'cache_val_name') and \
				func.cache_val_name is not None:

				setattr(obj, func.cache_val_name, None)


class CacheItem:
	"""
	QuerySet 缓存项，用于缓存 QuerySet 或 Model 的列表
	"""

	# QuerySet 缓存
	query_set = None

	# 实际数据缓存
	data = None

	# 已删除的对象
	removed = None

	def __init__(self, obj_type: models.Model = None,
				 objects=None, reload_func=None, **kwargs):
		"""
		构造函数
		Args:
			obj_type (type): 对象类型
			objects (list or QuerySet): 初始内容
			reload_func (callable): 重载函数
			**kwargs (**dict): 查询参数
		"""

		self.obj_type: models.Model = obj_type
		self.objects = objects
		self.reload_func = reload_func
		self.kwargs = kwargs

		self.removed = []

		self._load()

	def _load(self):
		if isinstance(self.objects, QuerySet):
			self.query_set = self.objects
			self.data = None

		elif isinstance(self.objects, list):
			self.query_set = None
			self.data = self.objects

		elif self.reload_func is not None:
			tmp_data = self.reload_func()

			if isinstance(tmp_data, QuerySet):
				self.query_set = tmp_data
				self.data = None

			elif isinstance(tmp_data, list):
				self.query_set = None
				self.data = tmp_data

			else:
				self.query_set = None
				self.data = [tmp_data]

		elif self.obj_type is not None:
			self.query_set: QuerySet = \
				self.obj_type.objects.filter(**self.kwargs)
			self.data = None  # 缓存列表

		else:
			self.query_set = self.data = None

		if self.data is not None:
			while None in self.data:
				self.data.remove(None)

	def reload(self, save=True):
		"""
		重新从数据库中载入
		"""
		if save: self.save()
		self._load()

	def isEmpty(self):
		"""
		缓存是否为空
		Returns:
			返回当前缓存是否为空
		"""
		return self.query_set is None and (
				self.data is None or len(self.data) <= 0)

	def _isReloadEmpty(self):
		"""
		若为空则重载并判断是否为空
		Returns:
			返回当前缓存是否为空
		"""
		if not self.isEmpty(): return False

		self.reload()

		return self.isEmpty()

	def _genData(self):
		"""
		从 QuerySet 转化为 list 数据
		"""
		if not self.isCached():
			self.data = list(self.query_set) \
				if self.query_set is not None else []

		return self.data

	def append(self, obj: models.Model):
		"""
		加入数据
		Args:
			obj (models.Model): 数据
		"""
		self._genData()

		index = self._findExactly(obj)

		if index == -1:  # 未找到
			self.data.append(obj)

	def remove(self, obj: models.Model):
		"""
		移除数据
		Args:
			obj (models.Model): 数据
		"""
		if self._isReloadEmpty(): return

		self._genData()

		index = self._findExactly(obj)

		if index >= 0:  # 已找到
			self.removed.append(self.data[index])
			self.data.pop(index)

	def _findExactly(self, obj: models.Model) -> int:
		index = 0
		objs = self.objs()

		for obj_ in objs:
			index += 1
			if obj_.id == obj.id and \
					type(obj_) == type(obj):
				return index

		return -1

	def query(self, cond: callable = None,
			  listed=False, **kwargs) -> QuerySet or list:
		"""
		查询数据
		Args:
			cond (callable): 查询函数
			listed (bool): 以列表形式返回
			**kwargs (**dict): 查询参数
		Returns:
			返回按条件查询到的数据
		"""
		if self._isReloadEmpty(): return []

		if listed: self._genData()

		if cond is not None:
			data = self._queryByCond(cond=cond)
		else:
			data = self.objs()

		if kwargs == {}: return data

		if self.isCached():
			return self._queryCache(data, **kwargs)

		return self._queryDjango(**kwargs)

	def _queryByCond(self, data=None, cond: callable = None) -> list:
		"""
		查询数据（传入一个函数）
		注意，调用了之后 QuerySet 数据会加入到缓存中
		Args:
			data (list or QuerySet): 源数据
			cond (callable): 查询函数
		Returns:
			返回按条件查询到的数据
		"""
		from .model_utils import Common

		if data is None: data = self.list()
		return Common.filter(data, cond)

	# 查询缓存
	def _queryCache(self, data=None, **kwargs) -> list:
		"""
		查询缓存数据
		Args:
			data (list or QuerySet): 源数据
			**kwargs (**dict): 查询参数
		Returns:
			返回按条件查询到的数据
		"""
		from .model_utils import Common

		if data is None: data = self.data
		return Common.query(data, **kwargs)

	# Django模式查询
	def _queryDjango(self, **kwargs) -> QuerySet:
		"""
		查询数据库数据
		Args:
			**kwargs (**dict): 查询参数
		Returns:
			返回按条件查询到的数据
		"""
		return self.query_set.filter(**kwargs)

	def get(self, cond: callable = None,
			listed=False, **kwargs) -> models.Model:
		"""
		获取数据
		Args:
			cond (callable): 查询函数
			listed (bool): 以列表形式返回
			**kwargs (**dict): 查询参数
		Returns:
			返回按条件查询到的数据（第一条）
		"""
		objs = self.query(cond, listed, **kwargs)

		if isinstance(objs, QuerySet):
			if objs.exists(): return objs.first()
		else:
			if len(objs) > 0: return objs[0]

		return None

		# if self.isEmpty(): return None
		#
		# if kwargs == {}: return self.first()
		#
		# if cond is not None:
		# 	data = self._queryByCond(cond)
		# 	if len(data) == 0: return None
		# else:
		# 	data = self.objs()
		#
		# if self.isCached():
		# 	data = self._queryCache(**kwargs)
		# 	if len(data) > 0: return data[0]
		#
		# else:
		# 	data = self.query_set.filter(**kwargs)
		# 	if data.exists(): return data.first()
		#
		# return None

	# def _getByCond(self, cond: callable = None) -> list:
	# 	"""
	# 	获取数据（传入一个函数）
	# 	Args:
	# 		cond (callable): 查询函数
	# 	Returns:
	# 		返回按条件查询到的数据（第一条）
	# 	"""
	# 	items = self._queryByCond(cond)
	#
	# 	if len(items) > 0: return items[0]
	# 	return None

	def ensure(self, error: ErrorType,
			   cond: callable = None, **kwargs):
		"""
		确保数据存在
		Args:
			error (ErrorType): 错误类型
			cond (callable): 查询函数
			**kwargs (**dict): 查询参数
		"""
		if self.count(cond, **kwargs) <= 0:
			raise GameException(error)

	def count(self, cond: callable = None, **kwargs) -> int:
		"""
		计数
		Args:
			cond (callable): 查询函数
			**kwargs (**dict): 查询参数
		Returns:
			返回数据数量
		"""
		if self._isReloadEmpty(): return 0

		if cond is not None:
			return len(self._queryByCond(cond))

		if self.isCached():
			return len(self._queryCache(**kwargs))
		return self._queryDjango(**kwargs).count()

	def first(self, listed=False) -> models.Model:
		"""
		获取第一个数据
		Args:
			listed (bool): 以列表形式返回
		Returns:
			返回第一个数据
		"""
		if self._isReloadEmpty(): return None

		if listed: self._genData()

		if self.isCached():
			if len(self.data) > 0:
				return self.data[0]

		else:
			if self.query_set.count() > 0:
				return self.query_set.first()

		return None

	def objs(self) -> QuerySet or list:
		"""
		获取所有对象
		Returns:
			返回所有缓存对象
		"""
		if self.data is None: return self.query_set

		return self.data

	def isCached(self) -> bool:
		"""
		是否属于缓存状态
		Returns:
			返回是否属于缓存状态（经过了 append 或者 remove 操作）
		"""
		return self.data is not None

	def save(self):
		"""
		保存缓存数据到数据库
		"""
		if self.objs() is not None:
			for obj in self.objs(): obj.save()

		for obj in self.removed: obj.save()
		self.removed = []

	def clear(self, save=True):
		"""
		清空缓存数据
		"""
		if save: self.save()
		self.query_set = self.data = None

	def list(self):
		"""
		以列表形式返回
		Returns:
			返回列表数据
		"""
		if not self.isCached():
			self.data = list(self.query_set)

		return self.data

	def __iter__(self):
		"""
		返回迭代器
		Returns:
			返回迭代器
		"""
		return self.data.__iter__() if self.isCached() \
			else self.query_set.__iter__()


class CachePool:
	"""
	缓存管理器，用键值对方式对 CacheItem 进行管理
	"""

	# 对象缓存（缓存 CacheItem 对象）
	objects = None

	def __init__(self): self.objects = {}

	def setObjects(self, cla=None, key=None,
				   objects=None, reload_func=None,
				   save=True, **kwargs):
		"""
		设置 QuerySet 缓存
		Args:
			cla (type): 类型
			key (object): 键
			objects (list or QuerySet): 默认值
			reload_func (callable): 加载函数
			save (bool): 是否保存
			**kwargs (**dict): 查询参数
		"""
		if key is None: key = cla
		if key is None: return

		if self.contains(key): self.delete(save)

		self.objects[key] = CacheItem(cla, objects, reload_func, **kwargs)

	def appendObject(self, key, obj: object, set_new=True):
		"""
		加入物体
		Args:
			key (object): 键
			obj (object): 对象
			set_new (bool): 如果不存在的时候是否设置一个新的缓存
		"""
		if self.contains(key):
			self.getItem(key).append(obj)

		elif set_new:
			self.setObjects(key=key, objects=[obj])

	def removeObject(self, key, obj: object):
		"""
		移出物体
		Args:
			key (object): 键
			obj (object): 对象
		"""
		if self.contains(key):
			self.getItem(key).remove(obj)

	def queryObjects(self, key, cond: callable = None,
					 listed=False, **kwargs) -> QuerySet or list:
		"""
		读取所有指定类型的 Object
		Args:
			key (object): 键
			cond (callable): 查询函数
			listed (bool): 以数组形式返回
			**kwargs (**dict): 查询参数
		Returns:
			返回该类型数据 QuerySet 或者 列表
		"""
		if self.contains(key):
			return self.getItem(key).query(cond, listed, **kwargs)
		return []

	def ensureObjects(self, key, error: ErrorType,
					  cond: callable = None, **kwargs) -> QuerySet or list:
		"""
		确保有指定条件指定类型的 Object
		Args:
			key (object): 键
			cond (callable): 查询函数
			error (ErrorType): 错误类型
			**kwargs (**dict): 查询参数
		Returns:
			返回该类型数据 QuerySet 或者 列表
		"""
		if self.contains(key):
			return self.getItem(key).ensure(error, cond, **kwargs)
		return []

	def getObject(self, key, cond: callable = None,
				  listed=False, **kwargs) -> models.Model:
		"""
		读取单个指定条件类型的 Object
		Args:
			key (object): 键
			cond (callable): 查询函数
			listed (bool): 以数组形式返回
			**kwargs (**dict): 查询参数
		Returns:
			返回指定条件类型的第一个数据
		"""
		if self.contains(key):
			return self.getItem(key).get(cond, listed, **kwargs)
		return None

	def firstObject(self, key, listed=False) -> models.Model:
		"""
		读取第一个指定类型的 Object
		Args:
			key (object): 键
			listed (bool): 以数组形式返回
		Returns:
			返回指定类型的第一个数据
		"""
		if self.contains(key):
			return self.getItem(key).first(listed)
		return None

	def contains(self, key) -> bool:
		"""
		是否包含某类型
		Args:
			key (object): 键
		Returns:
			返回是否包含某类型
		"""
		return key in self.objects

	def containsObject(self, key, cond: callable = None, **kwargs) -> bool:
		"""
		是否包含某条件类型数据
		Args:
			key (object): 键
			cond (callable): 查询函数
			**kwargs (**dict): 查询参数
		Returns:
			返回是否包含某条件类型缓存数据
		"""
		return self.getCount(cond, **kwargs) > 0

	def getCount(self, key, cond: callable = None, **kwargs):
		"""
		获取数量
		Args:
			key (object): 键
			cond (callable): 查询函数
			**kwargs (**dict): 查询参数
		Returns:
			返回数据数量
		"""
		item = self.getItem(key)
		return item.count(cond, **kwargs) \
			if item is not None else 0

	def reload(self, key, save=True):
		"""
		重载缓存
		Args:
			key (object): 键
			save (bool): 是否保存
		"""
		if self.contains(key):
			self.objects[key].reload(save)

	def reloadAll(self, save=True):
		"""
		重载全部缓存
		Args:
			save (bool): 是否保存
		"""
		for key in self.objects:
			self.objects[key].reload(save)

	def save(self, key):
		"""
		保存缓存
		Args:
			key (object): 键
		"""
		if self.contains(key):
			self.objects[key].save()

	def saveAll(self):
		"""
		保存全部缓存
		"""
		for key in self.objects:
			self.objects[key].save()

	def clear(self, key, save=True):
		"""
		清除缓存
		Args:
			key (object): 键
			save (bool): 是否保存
		"""
		if self.contains(key):
			self.objects[key].clear(save)

	def clearAll(self, save=True):
		"""
		清除全部缓存
		Args:
			save (bool): 是否保存
		"""
		for key in self.objects:
			self.objects[key].clear(save)

	def delete(self, key, save=True):
		"""
		删除缓存
		Args:
			key (object): 键
			save (bool): 是否保存
		"""
		if self.contains(key):
			if save: self.objects[key].save()
			del self.objects[key]

	def deleteAll(self, save=True):
		"""
		删除全部缓存
		Args:
			save (bool): 是否保存
		"""
		if save: self.saveAll()

		self.objects = {}

	def getItem(self, key) -> CacheItem:
		"""
		获取项
		Args:
			key (object): 键
		Returns:
			返回对应类型的项
		"""
		return self.objects[key] if self.contains(key) else None


# ===================================================
#  可缓存对象
# ===================================================
class CacheableObject:

	cache_pool: CachePool = None

	cache_values = None

	def __init__(self, *args, **kwargs):
		self.cache_values = {}
		self.cache_pool = CachePool()

		# self._setupCachePool()

	# 配置缓存池
	def _setupCachePool(self):
		self._setupCustomCacheItems()

	# 配置自定义缓存项
	def _setupCustomCacheItems(self): pass

	# region 缓存池操作

	def _setModelCache(self, cla=None, key=None, objects=None,
					   reload_func=None, save=True, **kwargs):
		self.cache_pool.setObjects(cla, key, objects,
								   reload_func, save, **kwargs)

	def _appendModelCache(self, key, obj: object, set_new=True):
		self.cache_pool.appendObject(key, obj, set_new)

	def _removeModelCache(self, key, obj: object):
		self.cache_pool.removeObject(key, obj)

	def _queryModelCache(self, key, cond: callable = None,
						 listed=False, **kwargs) -> QuerySet or list:
		return self.cache_pool.queryObjects(key, cond, listed, **kwargs)

	def _ensureModelCache(self, key, error: ErrorType,
					  cond: callable = None, **kwargs) -> QuerySet or list:
		return self.cache_pool.ensureObjects(key, error, cond, **kwargs)

	def _getModelCache(self, key, cond: callable = None,
					   listed=False, **kwargs) -> models.Model:
		return self.cache_pool.getObject(key, cond, listed, **kwargs)

	def _firstModelCache(self, key, listed=False) -> models.Model:
		return self.cache_pool.firstObject(key, listed)

	def _containsModelKey(self, key) -> bool:
		return self.cache_pool.contains(key)

	def _containsModelCache(self, key, cond: callable = None, **kwargs) -> bool:
		return self.cache_pool.containsObject(key, cond, **kwargs)

	def _clearModelCache(self, key, save=True) -> CacheItem:
		self.cache_pool.clear(key, save)

	def _saveModelCache(self, key) -> CacheItem:
		self.cache_pool.save(key)

	def _reloadModelCache(self, key, save=True) -> CacheItem:
		self.cache_pool.reload(key, save)

	def _modelCacheCount(self, key, cond: callable = None, **kwargs):
		return self.cache_pool.getCount(key, cond, **kwargs)

	def _modelCacheItem(self, key) -> CacheItem:
		return self.cache_pool.getItem(key)

	# endregion

	# region 值缓存

	# 进行缓存
	def _setCache(self, key, value):
		self.cache_values[key] = value

	# 获取缓存
	def _getCache(self, key):
		if key in self.cache_values:
			return self.cache_values[key]
		return None

	# 删除缓存
	def _deleteCache(self, key):
		if key in self.cache_values:
			del self.cache_values[key]

	# 获取或者设置（如果不存在）缓存
	def _getOrSetCache(self, key, func):
		if key not in self.cache_values:
			self._setCache(key, func())
		return self.cache_values[key]

	# endregion

	"""占位符"""


# ===================================================
#  缓存机制模型
# ===================================================
class CacheableModel(models.Model, CacheableObject):

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

			def genReloadFunc():
				_cla = cla

				def reloadFunc():
					return [self._getOneToOneModelInDb(_cla)]

				return reloadFunc

			self._setModelCache(key=cla, reload_func=genReloadFunc())

		for cla in self._cacheForeignKeyModels():
			objs = self._getForeignKeyModelInDb(cla)
			self._setModelCache(key=cla, objects=objs)

	# region 关系操作

	# 需要缓存的模型类列表（必须为 OneToOne）
	@classmethod
	def _cacheOneToOneModels(cls): return []

	# 需要缓存的模型类列表（必须为 外键）
	@classmethod
	def _cacheForeignKeyModels(cls): return []

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
