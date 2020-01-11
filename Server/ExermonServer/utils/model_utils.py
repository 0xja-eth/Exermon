from django.conf import settings
from django.utils.deconstruct import deconstructible
import os

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
	EFFECT_DIR = 'exermon/skill/effect'

	def __init__(self, type):
		_dir = None
		if type == 'icon': _dir = self.ICON_DIR
		if type == 'ani': _dir = self.ANI_DIR
		if type == 'effect': _dir = self.EFFECT_DIR

		super().__init__(_dir)

	def generateFileName(self, instance, filename):

		# 文件拓展名
		ext = os.path.splitext(filename)[1]

		# 定义文件名
		filename = "skill_%d" % instance.id

		return filename+ext


# ============================================
# 公用类：处理模型函数的共有业务逻辑
# ============================================
class Common:

	@classmethod
	def preventNone(cls, judge, value=None, obj=None, func=None, empty=0):
		if judge is None: return empty
		if value is not None: return value
		return func(obj)

	@classmethod
	def timeToStr(cls, time, empty=''):
		# if time is None: return empty
		# return time.strftime('%Y-%m-%d %H:%M:%S')
		return cls.preventNone(
			time, obj=time, func=lambda t: t.strftime
			('%Y-%m-%d %H:%M:%S'), empty=empty)

	@classmethod
	def objectToId(cls, object, empty=0):
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
	def getObjectRelatedForFilter(cls, objects, key, unique=False, **args):
		temp = objects.select_related(key).filter(**args)
		return cls.getObjectRelated(temp, key, unique)

	# 物体集转化为字典
	@classmethod
	def objectsToDict(cls, objects, **args):
		result = []

		for obj in objects:
			result.append(obj.convertToDict(**args))

		return result
