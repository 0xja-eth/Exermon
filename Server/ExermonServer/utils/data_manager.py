
from .cache_utils import *

import datetime


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
	def loadDate(cls, data, empty=None):
		return cls.preventNone(data, func=lambda d:
			datetime.datetime.strptime(d, cls.DATE_FORMAT), empty=empty)

	@classmethod
	def loadDateTime(cls, data, empty=None):
		return cls.preventNone(data, func=lambda d:
			datetime.datetime.strptime(d, cls.DATE_TIME_FORMAT), empty=empty)

	@classmethod
	def convertDate(cls, time: datetime, empty=None):
		return cls.preventNone(time, func=lambda t:
			t.strftime(cls.DATE_FORMAT), empty=empty)

	@classmethod
	def convertDateTime(cls, time: datetime, empty=None):
		return cls.preventNone(time, func=lambda t:
			t.strftime(cls.DATE_TIME_FORMAT), empty=empty)


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
	def underline2UpperHump(val, spliter="_"):
		"""
		下划线命名法转化为大驼峰命名法
		Args:
			val (str): 原字符串
			spliter (str): 分隔符
		Returns:
			返回转化结果
		"""
		res, flag = "", True
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
