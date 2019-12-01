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

	def __init__(self, _dir):
		_dir = os.path.join(self.SYSTEM_DIR, _dir)
		ImageUpload.__init__(self, _dir)

# ===================================================
#  物品图标
# ===================================================
@deconstructible
class ItemIconUpload(SystemImageUpload):

	ITEM_ICON_DIR = 'item/icon'

	def __init__(self, _dir=ITEM_ICON_DIR):
		SystemImageUpload.__init__(self, _dir)

	def generateFileName(self, instance, filename):

		# 文件拓展名
		ext = os.path.splitext(filename)[1]

		# 定义文件名,用户id_年月日时分秒_随机数
		filename = "item_%d" % instance.id

		return filename+ext

# ===================================================
#  技能图标
# ===================================================
@deconstructible
class SkillIconUpload(SystemImageUpload):

	SKILL_ICON_DIR = 'skill/icon'

	def __init__(self, _dir=SKILL_ICON_DIR):
		SystemImageUpload.__init__(self, _dir)

	def generateFileName(self, instance, filename):

		# 文件拓展名
		ext = os.path.splitext(filename)[1]

		# 定义文件名,用户id_年月日时分秒_随机数
		filename = "skill_%d" % instance.id

		return filename+ext


# ============================================
# 公用类：处理模型函数的共有业务逻辑
# ============================================
class Common:

	@classmethod
	def timeToStr(cls, time, empty=''):
		if not time: return empty
		return time.strftime('%Y-%m-%d %H:%M:%S')

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
