from django.db import models
from django.db.models.query import QuerySet
from .redis_utils import *
from .exception import *
from .model_utils import Common as ModelUtils
from .view_utils import Common as ViewUtils
import inspect, datetime, json


# region 可转化数据 BaseData


def autoConvert(type_: type or str = None, default = None,
				enable_load=True, enable_convert=True,
				error: ErrorType = ErrorType.UnknownError):
	"""
	自动转化装饰器
	Args:
		type_ (type or str): 默认类型
		default (object): 默认值
		enable_load (bool): 能否自动读取
		enable_convert (bool): 能否自动转化
		error (ErrorType): 抛出异常
	"""
	def wrapper(func):

		# 配置附加属性
		func.auto = True

		func.type_ = type_
		func.error = error
		func.enable_load = enable_load
		func.enable_convert = enable_convert

		def getOrSetAttr(self: BaseData, value=None):
			"""
			读取/设置属性
			"""

			name = self.getName(func)

			# 初始化配置检查
			# cla = type(self)
			# if not cla.Initialized and \
			# 	name not in cla.AUTO_KEYS:
			# 	initialize(cla, name)

			# 值处理
			if not hasattr(self, name):
				setattr(self, name, value or default)
			elif value is not None:
				setattr(self, name, value)

			return getattr(self, name)

		# def initialize(cla, name):
		# 	"""
		# 	初始化配置类
		# 	"""
		#
		# 	if cla.AUTO_KEYS is None:
		# 		cla.setupDataFuncs()
		#
		# 	data_funcs = func()  # 数据操作函数，必须为函数元组
		#
		# 	if enable_load:
		# 		if type_ is None:
		# 			loader = data_funcs[0]
		# 		else:
		# 			def loader(data):
		# 				DataLoader.load(type_, data, error)
		#
		# 		if cla.AUTO_LOADERS is None:
		# 			cla.AUTO_LOADERS = []
		# 		cla.AUTO_LOADERS.append([name, loader])
		#
		# 	if enable_convert:
		# 		if type_ is None:
		# 			converter = data_funcs[1]
		# 		else:
		# 			def converter(data):
		# 				DataLoader.convert(type_, data, error)
		#
		# 		if cla.AUTO_CONVERTERS is None:
		# 			cla.AUTO_CONVERTERS = []
		# 		cla.AUTO_CONVERTERS.append([name, converter])
		#
		# cla.AUTO_KEYS.append(name)

		getOrSetAttr.func = func
		return getOrSetAttr
	return wrapper


# ===================================================
#  可转化数据结构
# ===================================================
class BaseData:

	# 自动读取/转化设定，形如：[(key_name, key_type)]
	AUTO_LOAD_SETTING = []
	AUTO_CONVERT_SETTING = []

	# 自动读取/转化函数（自动生成，不需要配置）
	AUTO_LOADERS = None
	AUTO_CONVERTERS = None

	# 自动转化键记录
	AUTO_KEYS = None

	# 类是否初始化完毕
	Initialized = False

	def __init__(self, *args, **kwargs):
		cla = type(self)
		if not cla.Initialized:
			cla.initialize()

	@classmethod
	def initialize(cls):
		cls.setupDataFuncs()
		cls.setupAttrs()
		cls.Initialized = True

	@classmethod
	def setupDataFuncs(cls):
		cls.AUTO_LOADERS = []
		cls.AUTO_CONVERTERS = []
		# cls.AUTO_KEYS = []

	@classmethod
	def setupAttrs(cls):
		"""
		配置所有属性
		"""
		for key in dir(cls):
			attr = getattr(cls, key)
			if hasattr(attr, "func"):
				func = getattr(attr, "func")
				if hasattr(func, "auto"):
					cls.setupAttr(func)

	@classmethod
	def setupAttr(cls, func):
		"""
		初始化配置类
		"""
		name = cls.getName(func)
		data_funcs = func()  # 数据操作函数，必须为函数元组

		if func.enable_load:
			if func.type_ is None:
				loader = data_funcs[0]
			else:
				def loader(data):
					DataLoader.load(func.type_, data, func.error)

			cls.AUTO_LOADERS.append([name, loader])

		if func.enable_convert:
			if func.type_ is None:
				converter = data_funcs[1]
			else:
				def converter(data):
					DataLoader.convert(func.type_, data, func.error)

			cls.AUTO_CONVERTERS.append([name, converter])

	@staticmethod
	def getName(func):
		return "_%s" % DataManager. \
			hump2Underline(func.__name__)

	# region 数据读取/转化

	def load(self, data: dict):
		"""
		从JSON数据中读取
		Args:
			data (dict): 原始数据
		"""
		self.__loadAutoAttrs(data)
		self._loadCustomAttrs(data)

	def __loadAutoAttrs(self, data):
		"""
		读取自动属性
		"""
		for item in self.AUTO_LOAD_SETTING:
			key, type = item
			if hasattr(self, key) and key in data:
				setattr(self, key, DataLoader.load(type, data[key]))

		for item in self.AUTO_LOADERS:
			key, loader = item
			if hasattr(self, key) and key in data:
				setattr(self, key, loader(data[key]))

	def _loadCustomAttrs(self, data):
		"""
		读取自定义属性
		"""
		pass

	def convert(self) -> dict:
		"""
		转化为JSON数据
		Returns:
			返回转化后的JSON数据
		"""
		data = {}
		self.__convertAutoAttrs(data)
		self._convertCustomAttrs(data)

		return data

	def __convertAutoAttrs(self, data):
		"""
		转化自动属性
		"""
		for item in self.AUTO_CONVERT_SETTING:
			key, type = item
			if hasattr(self, key):
				data[key] = DataLoader.convert(
					type, getattr(self, key))

		for item in self.AUTO_CONVERTERS:
			key, converter = item
			if hasattr(self, key) and key in data:
				setattr(self, key, converter(data[key]))

	def _convertCustomAttrs(self, data):
		"""
		转化自动属性
		"""
		pass


# endregion


# region 核心（静态/动态/游戏资料）数据类型 CoreModel

# ===================================================
#  核心模型
# ===================================================
class CoreModel:

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
	def get(cls, cond: callable = None, **kwargs) -> 'BaseData':
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

# endregion


# region 可缓存模型 CacheableModel

# ===================================================
#  缓存机制模型
# ===================================================
class CacheableModel(models.Model):

	class Meta:
		abstract = True

	# 需要缓存的模型类列表（必须为外键）
	CACHE_ONE_TO_ONE_MODELS = []
	CACHE_FOREIGN_KEY_MODELS = []

	cache_pool: 'CachePool' = None

	def __init__(self, *args, **kwargs):
		self.saved = True
		self.cache_lock = False
		self.delete_save = False
		self.cache_pool = CachePool()

		super().__init__(*args, **kwargs)

	def __del__(self):
		if self.delete_save: self.save()

	def __setattr__(self, key, value):
		if key != 'saved': self.saved = False
		super().__setattr__(key, value)

	# region 关联缓存

	def __getattribute__(self, name):

		if not self.cache_lock:
			for cla in self.CACHE_ONE_TO_ONE_MODELS:
				if name == cla.__name__.lower():
					return self.getOneToOneModel(cla)

			for cla in self.CACHE_FOREIGN_KEY_MODELS:
				if name == cla.__name__.lower() + '_set':
					return self.getForeignKeyModel(cla)

		return super().__getattribute__(name)

	def getOneToOneModel(self, cla) -> models.Model:

		res = self.cache_pool.firstObject(cla)

		if res is None:
			res = self._getOneToOneModelInDb(cla)
			self.cache_pool.setObjects(cla, objects=[res])

		return res

	# 从数据库获取
	def _getOneToOneModelInDb(self, cla):
		self.cache_lock = True

		name = cla.__name__.lower()

		res = None
		try:
			res = getattr(self, name)
		# 捕捉到异常就跳过
		except cla.DoesNotExist: pass
		except AttributeError: pass

		self.cache_lock = False
		return res

	def getForeignKeyModel(self, cla) -> models.Model:

		if not self.cache_pool.contains(cla):
			res = self._getForeignKeyModelInDb(cla)
			self.cache_pool.setObjects(cla, objects=res)

		else:
			res = self.cache_pool.queryObjects(cla)

		return res

	# 从数据库获取
	def _getForeignKeyModelInDb(self, cla) -> QuerySet:
		self.cache_lock = True

		name = cla.__name__.lower() + '_set'
		res = getattr(self, name).all()

		self.cache_lock = False
		return res

	def save(self, *args, **kwargs):

		super().save(*args, **kwargs)
		self.cache_pool.saveAll()


	# endregion

	"""占位符"""

# endregion

# endregion


class CacheItem:
	"""
	QuerySet 缓存项
	"""
	def __init__(self, obj_type, objects=None, **kwargs):

		self.obj_type = obj_type

		if isinstance(objects, QuerySet):
			self.query_set = objects
			self.data = None

		elif isinstance(objects, list):
			self.query_set = None
			self.data = objects

		else:
			self.query_set: QuerySet = \
				obj_type.objects.filter(**kwargs)
			self.data = None  # 缓存列表

	def append(self, obj: models.Model):
		"""
		加入数据
		Args:
			obj (models.Model): 数据
		"""
		if not self.isCached():
			self.data = list(self.query_set)
		
		self.data.append(obj)

	def remove(self, obj: models.Model):
		"""
		移除数据
		Args:
			obj (models.Model): 数据
		"""
		if not self.isCached():
			self.data = list(self.query_set)

		self.data.remove(obj)

	def query(self, cond: callable = None, **kwargs) -> QuerySet or list:
		"""
		查询数据
		Args:
			cond (callable): 查询函数
			**kwargs (**dict): 查询参数
		Returns:
			返回按条件查询到的数据
		"""
		if cond is not None:
			return self._queryByCond(cond)

		if kwargs == {}: return self.objs()

		if self.isCached():
			return self._queryCache(**kwargs)

		return self._queryDjango(**kwargs)

	def _queryByCond(self, cond: callable = None) -> list:
		"""
		查询数据（传入一个函数）
		Args:
			cond (callable): 查询函数
		Returns:
			返回按条件查询到的数据
		"""
		return ModelUtils.filter(self.data, cond)

	# 查询缓存
	def _queryCache(self, **kwargs) -> list:
		return ModelUtils.query(self.data, **kwargs)

	# Django模式查询
	def _queryDjango(self, **kwargs) -> QuerySet:
		return self.query_set.filter(**kwargs)

	def get(self, cond: callable = None, **kwargs) -> models.Model:
		"""
		获取数据
		Args:
			cond (callable): 查询函数
			**kwargs (**dict): 查询参数
		Returns:
			返回按条件查询到的数据（第一条）
		"""
		if cond is not None:
			return self._getByCond(cond)

		if kwargs == {}: return self.first()

		if self.isCached():
			data = self._queryCache(**kwargs)
			if len(data) > 0: return data[0]

		else:
			data = self.query_set.filter(**kwargs)
			if data.exists(): return data.first()

		return None

	def _getByCond(self, cond: callable = None) -> list:
		"""
		获取数据（传入一个函数）
		Args:
			cond (callable): 查询函数
		Returns:
			返回按条件查询到的数据（第一条）
		"""
		items = self._queryByCond(cond)

		if len(items) > 0: return items[0]
		return None

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
		if cond is not None:
			return len(self._queryByCond(cond))

		if self.isCached():
			return len(self._queryCache(**kwargs))
		return self._queryDjango(**kwargs).count()

	def first(self) -> models.Model:
		"""
		获取第一个数据
		Returns:
			返回第一个数据
		"""
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
		return self.data or self.query_set

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
		for obj in self.objs():
			obj.save()


class CachePool:
	"""
	缓存管理器
	"""

	# 对象缓存（缓存 CacheItem 对象）
	objects = None

	def __init__(self): self.objects = {}

	def setObjects(self, cla, key=None, save=True, objects=None, **kwargs):
		"""
		设置 QuerySet 缓存
		Args:
			cla (type): 类型
			key (object): 键
			save (bool): 是否保存
			objects (list or QuerySet): 默认值
			**kwargs (**dict): 查询参数
		"""
		if key is None: key = cla
		
		if self.contains(key): self.clear(save)

		self.objects[key] = CacheItem(cla, objects, **kwargs)

	def queryObjects(self, key, cond: callable = None, **kwargs) -> QuerySet or list:
		"""
		读取所有指定类型的 Object
		Args:
			key (object): 键
			cond (callable): 查询函数
			**kwargs (**dict): 查询参数
		Returns:
			返回该类型数据 QuerySet 或者 列表
		"""
		if self.contains(key):
			return self.getItem(key).query(cond, **kwargs)
		return []

	# def filterObjects(self, cla, cond: callable = None) -> QuerySet or list:
	# 	"""
	# 	读取所有指定条件类型的 Object
	# 	Args:
	# 		cla (type): 类型
	# 		cond (callable): 查询函数
	# 	Returns:
	# 		返回指定条件类型的数据列表
	# 	"""
	# 	if self.contains(cla):
	# 		return self.getItem(cla)._queryByCond(cond)
	# 	return []

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

	def getObject(self, key, cond: callable = None, **kwargs) -> models.Model:
		"""
		读取单个指定条件类型的 Object
		Args:
			key (object): 键
			cond (callable): 查询函数
			**kwargs (**dict): 查询参数
		Returns:
			返回指定条件类型的第一个数据
		"""
		if self.contains(key):
			return self.getItem(key).get(cond, **kwargs)
		return None

	def firstObject(self, key) -> models.Model:
		"""
		读取第一个指定类型的 Object
		Args:
			key (object): 键
		Returns:
			返回指定类型的第一个数据
		"""
		if self.contains(key):
			return self.getItem(key).first()
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

	def save(self, key):
		"""
		保存缓存
		Args:
			key (object): 键
		"""
		item = self.getItem(key)
		if item is not None: item.save()

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
			if save: self.objects[key].save()
			del self.objects[key]

	def clearAll(self, save=True):
		"""
		清除全部缓存
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


class DataLoader:
	"""
	数据加载类
	"""
	DATE_TIME_FORMAT = '%Y-%m-%d %H:%M:%S'
	DATE_FORMAT = '%Y-%m-%d'

	@classmethod
	def preventNone(cls, judge, value=None, obj=None, func=None, empty=None):
		if judge is None: return empty
		if value is not None: return value
		if obj is None: obj = judge
		return func(obj)

	@classmethod
	def load(cls, type_: str or type, data: dict or list or str,
			 error: ErrorType = ErrorType.UnknownError):
		"""
		从 json 读取实际数据
		Args:
			type_ (str or type): 类型
			data (dict or list or str): 数据
			error (ErrorType): 抛出异常
		Returns:
			返回转化后的数据
		"""

		try:
			if data is None: return None

			# 如果是一个类型，直接转化
			if isinstance(type_, type):
				if BaseData in inspect.getmro(type_):
					return type_().load(data)

				return type(data)

			if type_ == 'int':
				value = int(data)

			elif type_ == 'int[]':

				if not isinstance(data, list):
					data = json.loads(data)

				value = []
				for i in range(len(data)):
					value.append(int(data[i]))

			elif type_ == 'int[][]':

				if not isinstance(data, list):
					data = json.loads(data)

				value = []
				for i in range(len(data)):
					value.append([])
					for j in range(len(data[i])):
						value[i].append(int(data[i][j]))

			elif type_ == 'bool':
				value = bool(data)

			elif type_ == 'date':
				value = cls.loadDate(data)

			elif type_ == 'datetime':
				value = cls.loadDateTime(data)

			else: value = data

			# 其他类型判断
			return value

		except:
			raise GameException(error)

		# if hasattr(data, '__iter__'):
		# 	res = list()
		# 	for d in data:
		# 		res.append(cls.load(type, d))
		# 	return res

	@classmethod
	def loadDate(cls, data, empty=None):
		return cls.preventNone(data, func=lambda d:
			datetime.datetime.strptime(d, cls.DATE_FORMAT), empty=empty)
		# return datetime.datetime.strptime(data, cls.DATE_FORMAT)

	@classmethod
	def loadDateTime(cls, data, empty=None):
		return cls.preventNone(data, func=lambda d:
			datetime.datetime.strptime(d, cls.DATE_TIME_FORMAT), empty=empty)
		# return datetime.datetime.strptime(data, cls.DATE_TIME_FORMAT)

	@classmethod
	def convert(cls, type_: str or type, data,
				error: ErrorType = ErrorType.UnknownError) -> object:
		"""
		数据转化为 json 数据
		Args:
			type_ (str or type): 类型
			data (object): 数据
			error (ErrorType): 异常
		Returns:
			返回转化后的 json 字典 或 值
		"""

		try:

			if isinstance(type_, type):
				if isinstance(data, BaseData):
					return data.convert()

				return data

			if type_ == 'date':
				value = cls.convertDate(data)

			elif type_ == 'datetime':
				value = cls.convertDateTime(data)

			else: value = data

			# 其他类型判断
			return value

		except:
			raise GameException(error)

		# if hasattr(data, '__iter__'):
		# 	res = list()
		# 	for d in data:
		# 		res.append(cls.convert(type, d))
		# 	return res

	@classmethod
	def convertDate(cls, time: datetime, empty=None):
		# if time is None: return empty
		# return time.strftime('%Y-%m-%d')
		return cls.preventNone(time, func=lambda t:
			t.strftime(cls.DATE_FORMAT), empty=empty)

	@classmethod
	def convertDateTime(cls, time: datetime, empty=None):
		# if time is None: return empty
		# return time.strftime('%Y-%m-%d %H:%M:%S')
		return cls.preventNone(time, func=lambda t:
			t.strftime(cls.DATE_TIME_FORMAT), empty=empty)

	@classmethod
	def equals(cls, data, json_data):
		"""
		判断数据和JSON是否相等
		Args:
			data (object): 数据
			json_data (str): JSON字符串
		Returns:
			返回两者是否相等
		"""
		return data == cls.load(type(data), json_data)


class DataManager:
	"""
	数据管理器
	"""
	# 公共缓存池
	CommonCachePool = CachePool()

	# 对象缓存（缓存 CacheItem 对象）
	# Objects = {}

	# region 缓存操作

	@classmethod
	def setObjects(cls, cla, save=True, **kwargs):
		"""
		设置 QuerySet 缓存
		Args:
			cla (type): 类型
			save (bool): 是否保存
			**kwargs (**dict): 查询参数
		"""
		cls.CommonCachePool.setObjects(cla, cla, save, **kwargs)

	@classmethod
	def queryObjects(cls, cla, cond: callable = None, **kwargs) -> QuerySet or list:
		"""
		读取所有指定类型的 Object
		Args:
			cla (type): 类型
			cond (callable): 查询函数
			**kwargs (**dict): 查询参数
		Returns:
			返回该类型数据 QuerySet 或者 列表
		"""
		return cls.CommonCachePool.queryObjects(cla, cond, **kwargs)

	# @classmethod
	# def filterObjects(cls, cla, cond: callable = None) -> QuerySet or list:
	# 	"""
	# 	读取所有指定条件类型的 Object
	# 	Args:
	# 		cla (type): 类型
	# 		cond (callable): 查询函数
	# 	Returns:
	# 		返回指定条件类型的数据列表
	# 	"""
	# 	if cls.contains(cla):
	# 		return cls.getItem(cla)._queryByCond(cond)
	# 	return []

	@classmethod
	def ensureObjects(cls, cla, error: ErrorType,
					  cond: callable = None, **kwargs) -> QuerySet or list:
		"""
		确保有指定条件指定类型的 Object
		Args:
			cla (type): 类型
			cond (callable): 查询函数
			error (ErrorType): 错误类型
			**kwargs (**dict): 查询参数
		Returns:
			返回该类型数据 QuerySet 或者 列表
		"""
		cls.CommonCachePool.ensureObjects(cla, error, cond, **kwargs)

	@classmethod
	def getObject(cls, cla, cond: callable = None, **kwargs) -> models.Model:
		"""
		读取单个指定条件类型的 Object
		Args:
			cla (type): 类型
			cond (callable): 查询函数
			**kwargs (**dict): 查询参数
		Returns:
			返回指定条件类型的第一个数据
		"""
		return cls.CommonCachePool.getObject(cla, cond, **kwargs)

	@classmethod
	def contains(cls, cla) -> bool:
		"""
		是否包含某类型
		Args:
			cla (type): 类型
		Returns:
			返回是否包含某类型
		"""
		return cls.CommonCachePool.contains(cla)

	@classmethod
	def containsObject(cls, cla, cond: callable = None, **kwargs) -> bool:
		"""
		是否包含某条件类型数据
		Args:
			cla (type): 类型
			cond (callable): 查询函数
			**kwargs (**dict): 查询参数
		Returns:
			返回是否包含某条件类型缓存数据
		"""
		return cls.CommonCachePool.containsObject(cla, cond, **kwargs)

	@classmethod
	def getCount(cls, cla, cond: callable = None, **kwargs):
		"""
		获取数量
		Args:
			cla (type): 类型
			cond (callable): 查询函数
			**kwargs (**dict): 查询参数
		Returns:
			返回数据数量
		"""
		return cls.CommonCachePool.getCount(cla, cond, **kwargs)

	@classmethod
	def save(cls, cla):
		"""
		保存缓存
		Args:
			cla (type): 类型
		"""
		cls.CommonCachePool.save(cla)

	@classmethod
	def clear(cls, cla, save=True):
		"""
		清除缓存
		Args:
			cla (type): 类型
			save (bool): 是否保存
		"""
		cls.CommonCachePool.clear(cla, save)

	@classmethod
	def getItem(cls, cla) -> CacheItem:
		"""
		获取项
		Args:
			cla (type): 类型
		Returns:
			返回对应类型的项
		"""
		return cls.CommonCachePool.getItem(cla)

	# endregion
	
	@staticmethod
	def underline2LowerHump(val, spliter="_"):
		"""
		下划线命名法转化为小驼峰命名法
		Args:
			val (str): 原字符串
			spliter (str): 分隔符
		Returns:
			返回转化结果
		"""
		res, flag = "", False
		for char in val:
			if char == spliter: flag = True
			elif flag:
				res += char.upper()
				flag = False
			else: res += char

		return res

	@staticmethod
	def hump2Underline(val, spliter="_"):
		"""
		驼峰命名法转化为下划线命名法
		Args:
			val (str): 原字符串
			spliter (str): 分隔符
		Returns:
			返回转化结果
		"""
		res, i = "", 0
		for char in val:
			if i > 0 and char.isupper():
				res += spliter + char.lower()
			else: res += char
			i += 1

		return res

	# @classmethod
	# def get(cls, cla: type, **kwargs) -> BaseData:
	# 	"""
	# 	获取指定模型数据对象
	# 	Args:
	# 		cla (type): 模型类型
	# 		**kwargs (**dict): 查询参数
	# 	Returns:
	# 		返回模型对象
	# 	"""
	# 	if cla is None: return None
	#
	# 	is_model = models.Model in cla.__mro__
	# 	if BaseData not in cla.__mro__ and not is_model: return None
	#
	# 	res = cls._getCache(cla, **kwargs)
	#
	# 	if is_model and res is None:
	# 		res = cls._getDB(cla, **kwargs)
	#
	# 	return res
	#
	# @classmethod
	# def set(cls, data):
	# 	"""
	# 	设置指定模型数据对象
	# 	Args:
	# 		data (object): 设置数据
	# 	"""
	# 	if data is None: return
	#
	# 	if not isinstance(data, BaseData): return
	#
	# 	# 获取Redis实例
	# 	r = RedisUtils.redis()
	#
	# 	# 获取对应的缓存键
	# 	cla = type(data)
	# 	key = cls.getKeyName(cla)
	#
	# 	# 获取键上集合内（一个json字符串列表）
	# 	obj = data.convert()
	# 	r.sadd(key, json.dumps(obj))
	#
	# @classmethod
	# def setObjects(cls, cla):
	# 	"""
	# 	储存对象的所有内容
	# 	Args:
	# 		data (object): 设置数据
	# 	"""
	# 	if cla is None: return
	#
	# 	if BaseData not in cla.__mro__ and \
	# 		models.Model not in cla.__mro__: return None
	#
	# 	# 获取Redis实例
	# 	r = RedisUtils.redis()
	#
	# 	# 获取对应的缓存键
	# 	key = cls.getKeyName(cla)
	#
	# 	for data in cla.objects.all():
	#
	# 		# 获取键上集合内（一个json字符串列表）
	# 		obj = data.convert()
	# 		r.sadd(key, json.dumps(obj))
	#
	# @classmethod
	# def _getCache(cls, cla: type, **kwargs):
	# 	"""
	# 	获取缓存数据
	# 	"""
	# 	# 获取对应的缓存键
	# 	key = cls.getKeyName(cla)
	#
	# 	# 获取Redis实例
	# 	r = RedisUtils.redis()
	#
	# 	# 获取键上集合内（一个json字符串列表
	# 	set = r.smembers(key)
	# 	# 按条件获取指定 json 字符串
	# 	obj = cls._get(set, **kwargs)
	#
	# 	if obj is None: return None
	#
	# 	# 反序列化
	# 	res: BaseData = cla()
	# 	res.load(obj)
	#
	# 	return res
	#
	# @classmethod
	# def _getDB(cls, cla, **kwargs):
	# 	"""
	# 	获取数据库数据
	# 	"""
	# 	objs = cla.objects.filter(**kwargs)
	# 	if objs.exists():
	# 		return objs.first()
	# 	else:
	# 		return None
	#
	# @classmethod
	# def _get(cls, list_: list, **kwargs):
	# 	"""
	# 	获取单个符合的数据
	# 	"""
	# 	if list_ is None: return None
	# 	for item in list_: # item 为 一串 json 字符串
	# 		flag = True
	# 		item = json.loads(item)
	# 		for key in kwargs:
	# 			flag = key in item and DataLoader.\
	# 				equals(kwargs[key], item[key])
	# 			if not flag: break
	#
	# 		if flag: return item
	#
	# 	return None
	#
	# @classmethod
	# def getKeyName(cls, cla: type) -> str:
	# 	"""
	# 	获取指定类型的键名
	# 	Args:
	# 		cla (type): 类型
	# 	Returns:
	# 		返回对应的键名
	# 	"""
	# 	return "%s:%s" % (cla.__module__, cla.__name__)
