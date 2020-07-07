from django.db import models
from django.conf import settings
from django.db.models.query import QuerySet
from django.utils.deconstruct import deconstructible
import os, random

#
# # ===================================================
# #  缓存机制模型
# # ===================================================
# class CacheableModel(models.Model):
#
# 	class Meta:
# 		abstract = True
#
# 	CACHE_KEYS = []
#
# 	cached_dict = None
#
# 	def __init__(self, *args, **kwargs):
# 		super().__init__(*args, **kwargs)
# 		self.cached_dict = {}
# 		self.delete_save = False
# 		self.saved = True
#
# 	def __del__(self):
# 		if self.delete_save:
# 			self.save()
#
# 	def __setattr__(self, key, value):
# 		if key != 'saved': self.saved = False
# 		super().__setattr__(key, value)
#
# 	# 进行缓存
# 	def _cache(self, key, value):
# 		self.cached_dict[key] = value
#
# 	# 获取缓存
# 	def _getCache(self, key):
# 		if key in self.cached_dict:
# 			return self.cached_dict[key]
# 		return None
#
# 	# 获取或者设置（如果不存在）缓存
# 	def _getOrSetCache(self, key, func):
# 		if key not in self.cached_dict:
# 			self._cache(key, func())
# 		return self.cached_dict[key]
#
# 	# 获取一对一关系的缓存
# 	def _getOneToOneCache(self, cla, key=None):
# 		try:
# 			if key is None: key = cla
# 			attr_name = cla.__name__.lower()
# 			return self._getOrSetCache(key,
# 				lambda: getattr(self, attr_name))
# 		except cla.DoesNotExist: return None
# 		except AttributeError: return None
#
# 	# 获取外键关系的缓存
# 	def _getForeignKeyCache(self, cla, key=None):
# 		try:
# 			if key is None: key = cla
# 			attr_name = cla.__name__.lower()+'_set'
# 			return self._getOrSetCache(key,
# 									   lambda: list(getattr(self, attr_name).all()))
# 		except AttributeError: return None
#
# 	# 保存缓存
# 	def _saveCache(self, key=None):
# 		if key is None:
# 			self.__saveCacheItem(self.cached_dict)
#
# 		elif key in self.cached_dict:
# 			self.__saveCacheItem(self.cached_dict[key])
#
# 	# 保存缓存项
# 	def __saveCacheItem(self, item):
# 		if isinstance(item, (list, tuple)):
# 			tmp_item = item.copy()
# 			for val in tmp_item: self.__saveCacheItem(val)
#
# 		elif isinstance(item, dict):
# 			tmp_item = item.copy()
# 			for key in tmp_item: self.__saveCacheItem(item[key])
#
# 		elif hasattr(item, '__iter__'):
# 			for val in item: self.__saveCacheItem(val)
#
# 		elif isinstance(item, models.Model): item.save()
#
# 	# 删除缓存
# 	def _deleteCache(self, key=None):
# 		if key is None:
# 			self.__deleteCacheItem(self.cached_dict)
#
# 		elif key in self.cached_dict:
# 			self.__deleteCacheItem(self.cached_dict[key])
#
# 		self._clearCache(key, False)
#
# 	# 删除缓存项
# 	def __deleteCacheItem(self, item):
# 		if isinstance(item, (list, tuple)):
# 			for val in item: self.__saveCacheItem(val)
#
# 		elif isinstance(item, dict):
# 			for key in item: self.__saveCacheItem(item[key])
#
# 		elif hasattr(item, '__iter__'):
# 			for val in item: self.__saveCacheItem(val)
#
# 		elif isinstance(item, models.Model): item.delete()
#
# 	# 清除缓存
# 	def _clearCache(self, key=None, save=True):
# 		if save: self._saveCache(key)
#
# 		if key is None:
# 			self.cached_dict.clear()
# 		elif key in self.cached_dict:
# 			self.cached_dict.pop(key)
#
# 	# 重载保存函数
# 	def save(self, **kwargs):
#
# 		if not self.saved:
# 			super().save(**kwargs)
# 			# print(str(self)+" saved!")
#
# 		self.saved = True
# 		self._saveCache()


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


# ============================================
# 公用类：处理模型函数的共有业务逻辑
# ============================================
class Common:

	@classmethod
	def preventNone(cls, judge, value=None, obj=None, func=None, empty=None):
		if judge is None: return empty
		if value is not None: return value
		if obj is None: obj = judge
		return func(obj)

	@classmethod
	def timeToStr(cls, time, empty=None):
		# if time is None: return empty
		# return time.strftime('%Y-%m-%d %H:%M:%S')
		return cls.preventNone(time, func=lambda t:
			t.strftime('%Y-%m-%d %H:%M:%S'), empty=empty)

	@classmethod
	def dateToStr(cls, time, empty=None):
		# if time is None: return empty
		# return time.strftime('%Y-%m-%d')
		return cls.preventNone(time, func=lambda t:
			t.strftime('%Y-%m-%d'), empty=empty)

	@classmethod
	def objectToId(cls, object, empty=None):
		if not object: return empty
		return object.id

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
			result.append(obj.convertToDict(**kwargs))

		return result

	# 物体转化为字典
	@classmethod
	def objectToDict(cls, object, **kwargs):

		if object is None: return {}
		return object.convertToDict(**kwargs)

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
