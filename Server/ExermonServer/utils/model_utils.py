from django.db import models
from django.conf import settings
from django.utils.deconstruct import deconstructible
import os


# ===================================================
#  缓存机制模型
# ===================================================
class CacheableModel(models.Model):

	class Meta:
		abstract = True

	cached_dict = None

	def __init__(self, *args, **kwargs):
		super().__init__(*args, **kwargs)
		self.cached_dict = {}
		self.delete_save = False
		self.saved = True

	def __del__(self):
		if self.delete_save:
			self.save()

	def __setattr__(self, key, value):
		if key != 'saved': self.saved = False
		super().__setattr__(key, value)

	# 进行缓存
	def cache(self, key, value):
		self.cached_dict[key] = value

	# 获取缓存
	def getCache(self, key):
		if key in self.cached_dict:
			return self.cached_dict[key]
		return None

	# 获取或者设置（如果不存在）缓存
	def getOrSetCache(self, key, func):
		if key not in self.cached_dict:
			self.cache(key, func())
		return self.cached_dict[key]

	# 获取一对一关系的缓存
	def getOneToOneCache(self, cla, key=None):
		try:
			if key is None: key = cla
			attr_name = cla.__name__.lower()
			return self.getOrSetCache(key,
				lambda: getattr(self, attr_name))
		except cla.DoesNotExist: return None
		except AttributeError: return None

	# 获取外键关系的缓存
	def getForeignKeyCache(self, cla, key=None):
		try:
			if key is None: key = cla
			attr_name = cla.__name__.lower()+'_set'
			return self.getOrSetCache(key,
				lambda: list(getattr(self, attr_name).all()))
		except AttributeError: return None

	# 保存缓存
	def saveCache(self, key=None):
		if key is None:
			self._saveCacheItem(self.cached_dict)

		elif key in self.cached_dict:
			self._saveCacheItem(self.cached_dict[key])

	# 保存缓存项
	def _saveCacheItem(self, item):
		if isinstance(item, (list, tuple)):
			for val in item: self._saveCacheItem(val)

		elif isinstance(item, dict):
			for key in item: self._saveCacheItem(item[key])

		elif hasattr(item, '__iter__'):
			for val in item: self._saveCacheItem(val)

		elif isinstance(item, models.Model): item.save()

	# 删除缓存
	def deleteCache(self, key=None):
		if key is None:
			self._deleteCacheItem(self.cached_dict)

		elif key in self.cached_dict:
			self._deleteCacheItem(self.cached_dict[key])

		self.clearCache(key, False)

	# 删除缓存项
	def _deleteCacheItem(self, item):
		if isinstance(item, (list, tuple)):
			for val in item: self._saveCacheItem(val)

		elif isinstance(item, dict):
			for key in item: self._saveCacheItem(item[key])

		elif hasattr(item, '__iter__'):
			for val in item: self._saveCacheItem(val)

		elif isinstance(item, models.Model): item.delete()

	# 清除缓存
	def clearCache(self, key=None, save=True):
		if save: self.saveCache(key)

		if key is None:
			self.cached_dict.clear()
		elif key in self.cached_dict:
			self.cached_dict.pop(key)

	# 重载保存函数
	def save(self, **kwargs):

		if not self.saved:
			super().save(**kwargs)
			print(str(self)+" saved!")

		self.saved = True
		self.saveCache()


# ===================================================
#  图片上传处理（父类）
# ===================================================
@deconstructible
class ImageUpload:

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
		filename = "question_%d" % instance.id

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
	def timeToStr(cls, time, empty=''):
		# if time is None: return empty
		# return time.strftime('%Y-%m-%d %H:%M:%S')
		return cls.preventNone(time, func=lambda t: t.strftime('%Y-%m-%d %H:%M:%S'), empty=empty)

	@classmethod
	def dateToStr(cls, time, empty=''):
		# if time is None: return empty
		# return time.strftime('%Y-%m-%d')
		return cls.preventNone(time, func=lambda t: t.strftime('%Y-%m-%d'), empty=empty)

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

	# 通过 all() 获取一个QuerySet的相关属性
	# objects: QuerySet
	@classmethod
	def getObjectRelatedForAll(cls, objects, key, unique=False):
		temp = objects.select_related(key).all()
		return cls.getObjectRelated(temp, key, unique)

	# 通过 filter() 获取一个QuerySet的相关属性
	# objects: QuerySet
	@classmethod
	def getObjectRelatedForFilter(cls, objects, key, unique=False, **kwargs):
		temp = objects.select_related(key).filter(**kwargs)
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

	# QuerySet式查询（通过list）
	@classmethod
	def query(cls, list, map=None, **kwargs):
		res = []

		for item in list:
			flag = True
			for key in kwargs:
				val = kwargs[key]
				flag = (hasattr(item, key) and getattr(item, key) == val)
				if not flag: break

			if flag:
				if map: res.append(map(item))
				else: res.append(item)

		return res

	# 过滤（通过list）
	@classmethod
	def filter(cls, list, cond=None, map=None):
		res = []

		for item in list:
			if cond and cond(item):
				if map: res.append(map(item))
				else: res.append(item)

		return res
