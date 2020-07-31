from django.db import models
from django.conf import settings
from django.db.models.query import QuerySet
from django.utils.deconstruct import deconstructible
from .data_manager import *
from enum import Enum
import os, random


# ===================================================
#  AdminX帮助类
# ===================================================
class AdminXHelper:

	InlinesMapper = {}

	# region 装饰器

	@classmethod
	def relatedModel(cls, model: models.Model, *inline_clas):
		"""
		关联一个模型
		Args:
			model (type): 模型类
			*inline_clas (*tuple): 关联类
		"""
		inline_clas = list(inline_clas)

		def wrapper(adminx_cla: type):
			import xadmin

			if not model._meta.abstract:
				xadmin.site.register(model, adminx_cla)

			processKey(adminx_cla, "list_display", True)
			processKey(adminx_cla, "list_editable")

			processInlines(adminx_cla)

			processFormLayout(adminx_cla)

			return adminx_cla

		def processKey(adminx_cla: type, key: str, include_id=False):
			const_key = key.upper()
			adminx_key = key.lower()

			if hasattr(model, const_key) and \
					getattr(model, const_key) is not None:
				tmp_val = list(getattr(model, const_key))
			else:
				tmp_val = cls.localKeys(model, include_id)

			append_key = const_key+"_APPEND"
			if hasattr(model, append_key):
				tmp_val.extend(getattr(model, append_key))

			exclude_key = const_key+"_EXCLUDE"
			if hasattr(model, exclude_key):
				for key in getattr(model, exclude_key):
					tmp_val.remove(key)

			cls._inhertAttr(adminx_cla, adminx_key, tmp_val)

		def processInlines(adminx_cla: type):

			excludes = cls._inlineClasses2Models(inline_clas)

			releated_models = cls.getRelatedModels(model, excludes)

			for inline_model in releated_models:
				inline_cla = cls.generateInlineClass(inline_model)
				if inline_cla is not None:
					inline_clas.append(inline_cla)

			print("processInlines: %s.inline = %s" % (adminx_cla.__name__, inline_clas))

			cls._inhertAttr(adminx_cla, 'inlines', inline_clas)

		def processFormLayout(adminx_cla: type):
			from xadmin.layout import Fieldset

			name = model._meta.verbose_name

			field_set = [Fieldset(name+"属性", *cls.localKeys(model))]

			cls._inhertAttr(adminx_cla, 'form_layout', field_set)

		return wrapper

	@classmethod
	def registerBaseInline(cls, model: models.Model):
		"""
		注册基本行内关系
		Args:
			model (type): 模型
		"""
		def wrapper(inline_cla: type):

			cls.InlinesMapper[model] = inline_cla

			if not model._meta.abstract:
				inline_cla.model = model

			return inline_cla

		return wrapper

	# endregion

	# region Inline 处理

	@classmethod
	def findBaseInlineClass(cls, model: models.Model):
		"""
		获取行内关系类
		Args:
			model (type): 模型
		Returns:
			返回对应行内类的基类
		"""
		for cla in model.mro():
			if cla in cls.InlinesMapper:
				return cls.InlinesMapper[cla]

		return None

	@classmethod
	def generateInlineClass(cls, inline_model: models.Model):

		base_cla = cls.findBaseInlineClass(inline_model)

		if base_cla is None: return None

		if hasattr(base_cla, 'model') and \
				base_cla.model == inline_model:
			return base_cla

		inline_cla = type("%sInline" % inline_model.__name__,
						  (base_cla, ), {"model": inline_model})

		return inline_cla

	@classmethod
	def _inlineClasses2Models(cls, inline_classes: list):
		res = []
		for inline in inline_classes:
			res.append(inline.model)

		return res

	# endregion

	# region Model 字段处理

	@classmethod
	def allKeys(cls, model: models.Model, include_id=False) -> list:

		res = cls._allMetaAttr(model, 'fields', 'name')

		if 'id' in res and not include_id: res.remove('id')

		return res

	@classmethod
	def localKeys(cls, model: models.Model, include_id=False) -> list:

		res = cls._deltaMetaAttr(model, 'fields', 'name')

		if 'id' in res and not include_id: res.remove('id')

		return res

		# keys = []
		#
		# base_cla = model.__bases__[0]
		# base_fields = base_cla._meta.fields \
		# 	if hasattr(base_cla, '_meta') else []
		#
		# for field in model._meta.fields:
		# 	if field in base_fields: continue
		# 	if field.name != 'id' or include_id:
		# 		keys.append(field.name)
		#
		# return keys

	@classmethod
	def getRelatedModels(cls, model: models.Model, excludes=[]):

		if hasattr(model, "INLINE_MODELS") and \
				model.INLINE_MODELS is not None:
			return model.INLINE_MODELS

		excludes += model.EXCLUDE_INLINES \
			if hasattr(model, "EXCLUDE_INLINES") else []

		return cls.localRelatedModels(model, excludes)

	@classmethod
	def allRelatedModels(cls, model: models.Model, excludes=[]):

		return cls._allMetaAttr(model, 'related_objects',
								'related_model', excludes)

	@classmethod
	def localRelatedModels(cls, model: models.Model, excludes=[]):

		return cls._deltaMetaAttr(model, 'related_objects',
								  'related_model', excludes)

	# endregion

	# region 属性获取

	@classmethod
	def _allMetaAttr(cls, model: models.Model, key, map_key=None, excludes=[]):
		"""
		所有Meta属性
		"""
		res = []

		for val in getattr(model._meta, key):
			item = val if map_key is None \
				else getattr(val, map_key)

			if item not in excludes:
				res.append(item)

		return res

	@classmethod
	def _deltaMetaAttr(cls, model: models.Model, key, map_key=None, excludes=[]):
		"""
		与父类Meta相差的属性
		"""
		res = []
		base_cla = model.__bases__[0]

		if hasattr(base_cla, '_meta') and \
			hasattr(base_cla._meta, key):
			base_vals = getattr(base_cla._meta, key)
		else:
			base_vals = []

		for val in getattr(model._meta, key):
			if val in base_vals: continue

			item = val if map_key is None \
				else getattr(val, map_key)

			if item not in excludes:
				res.append(item)

		return res

	@classmethod
	def _inhertAttr(cls, adminx_cla, key, val):
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

# region 核心（静态/动态/游戏资料）数据类型 CoreModel


# ===================================================
#  核心模型
# ===================================================
class CoreModel(models.Model):

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
