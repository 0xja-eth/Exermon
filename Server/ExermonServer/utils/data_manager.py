from .cache_utils import *
from .runtime_manager import *
import json, datetime


# class DataLoader:
# 	"""
# 	数据加载类
# 	"""
# 	DATE_TIME_FORMAT = '%Y-%m-%d %H:%M:%S'
# 	DATE_FORMAT = '%Y-%m-%d'
#
# 	@classmethod
# 	def preventNone(cls, judge, value=None, obj=None, func=None, empty=None):
# 		if judge is None: return empty
# 		if value is not None: return value
# 		if obj is None: obj = judge
# 		return func(obj)
#
# 	@classmethod
# 	def load(cls, type_: str or type, data: dict or list or str,
# 			 error: ErrorType = ErrorType.UnknownError):
# 		"""
# 		从 json 读取实际数据
# 		Args:
# 			type_ (str or type): 类型
# 			data (dict or list or str): 数据
# 			error (ErrorType): 抛出异常
# 		Returns:
# 			返回转化后的数据
# 		"""
#
# 		try:
# 			if data is None: return None
#
# 			# 如果是一个类型，直接转化
# 			if isinstance(type_, type):
# 				if RuntimeModel in type_.mro():
# 					return type_().load(data)
#
# 				return type(data)
#
# 			if type_ == 'int':
# 				value = int(data)
#
# 			elif type_ == 'int[]':
#
# 				if not isinstance(data, list):
# 					data = json.loads(data)
#
# 				value = []
# 				for i in range(len(data)):
# 					value.append(int(data[i]))
#
# 			elif type_ == 'int[][]':
#
# 				if not isinstance(data, list):
# 					data = json.loads(data)
#
# 				value = []
# 				for i in range(len(data)):
# 					value.append([])
# 					for j in range(len(data[i])):
# 						value[i].append(int(data[i][j]))
#
# 			elif type_ == 'bool':
# 				value = bool(data)
#
# 			elif type_ == 'date':
# 				value = cls.loadDate(data)
#
# 			elif type_ == 'datetime':
# 				value = cls.loadDateTime(data)
#
# 			else:
# 				value = data
#
# 			# 其他类型判断
# 			return value
#
# 		except:
# 			raise GameException(error)
#
# 	# if hasattr(data, '__iter__'):
# 	# 	res = list()
# 	# 	for d in data:
# 	# 		res.append(cls.load(type, d))
# 	# 	return res
#
# 	@classmethod
# 	def loadDate(cls, data, empty=None):
# 		return cls.preventNone(data, func=lambda d:
# 			datetime.datetime.strptime(d, cls.DATE_FORMAT), empty=empty)
#
# 	# return datetime.datetime.strptime(data, cls.DATE_FORMAT)
#
# 	@classmethod
# 	def loadDateTime(cls, data, empty=None):
# 		return cls.preventNone(data, func=lambda d:
# 			datetime.datetime.strptime(d, cls.DATE_TIME_FORMAT), empty=empty)
#
# 	# return datetime.datetime.strptime(data, cls.DATE_TIME_FORMAT)
#
# 	@classmethod
# 	def convert(cls, type_: str or type, data,
# 				error: ErrorType = ErrorType.UnknownError) -> object:
# 		"""
# 		数据转化为 json 数据
# 		Args:
# 			type_ (str or type): 类型
# 			data (object): 数据
# 			error (ErrorType): 异常
# 		Returns:
# 			返回转化后的 json 字典 或 值
# 		"""
#
# 		try:
#
# 			if isinstance(type_, type):
# 				if isinstance(data, RuntimeModel):
# 					return data.convert()
#
# 				return data
#
# 			if type_ == 'date':
# 				value = cls.convertDate(data)
#
# 			elif type_ == 'datetime':
# 				value = cls.convertDateTime(data)
#
# 			else:
# 				value = data
#
# 			# 其他类型判断
# 			return value
#
# 		except:
# 			raise GameException(error)
#
# 	# if hasattr(data, '__iter__'):
# 	# 	res = list()
# 	# 	for d in data:
# 	# 		res.append(cls.convert(type, d))
# 	# 	return res
#
# 	@classmethod
# 	def convertDate(cls, time: datetime, empty=None):
# 		# if time is None: return empty
# 		# return time.strftime('%Y-%m-%d')
# 		return cls.preventNone(time, func=lambda t:
# 			t.strftime(cls.DATE_FORMAT), empty=empty)
#
# 	@classmethod
# 	def convertDateTime(cls, time: datetime, empty=None):
# 		# if time is None: return empty
# 		# return time.strftime('%Y-%m-%d %H:%M:%S')
# 		return cls.preventNone(time, func=lambda t:
# 			t.strftime(cls.DATE_TIME_FORMAT), empty=empty)
#
# 	@classmethod
# 	def equals(cls, data, json_data):
# 		"""
# 		判断数据和JSON是否相等
# 		Args:
# 			data (object): 数据
# 			json_data (str): JSON字符串
# 		Returns:
# 			返回两者是否相等
# 		"""
# 		return data == cls.load(type(data), json_data)


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
	def delete(cls, cla, save=True):
		"""
		清除缓存
		Args:
			cla (type): 类型
			save (bool): 是否保存
		"""
		cls.CommonCachePool.delete(cla, save)

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
			if char == spliter:
				flag = True
			elif flag:
				res += char.upper()
				flag = False
			else:
				res += char

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
			else:
				res += char.lower()
			i += 1

		return res
