import xadmin

from django.db import models
from .model_utils import ModelHelper


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
				tmp_val = ModelHelper.localKeys(model, include_id)

			append_key = const_key+"_APPEND"
			if hasattr(model, append_key):
				tmp_val.extend(getattr(model, append_key))

			exclude_key = const_key+"_EXCLUDE"
			if hasattr(model, exclude_key):
				for key in getattr(model, exclude_key):
					if key in tmp_val: tmp_val.remove(key)

			ModelHelper.inhertAttr(adminx_cla, adminx_key, tmp_val)

		def processInlines(adminx_cla: type):

			excludes = cls._inlineClasses2Models(inline_clas)

			releated_models = cls.getInlineModels(model, excludes)

			for inline_model in releated_models:
				inline_cla = cls.generateInlineClass(inline_model)
				if inline_cla is not None:
					inline_clas.append(inline_cla)

			print("processInlines: %s.inline = %s" % (adminx_cla.__name__, inline_clas))

			ModelHelper.inhertAttr(adminx_cla, 'inlines', inline_clas)

		def processFormLayout(adminx_cla: type):
			from xadmin.layout import Fieldset

			name = model._meta.verbose_name

			field_set = [Fieldset(name+"属性", *ModelHelper.localKeys(model))]

			ModelHelper.inhertAttr(adminx_cla, 'form_layout', field_set)

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
	def getInlineModels(cls, model: models.Model, excludes=[]):
		"""
		获取所有行关系模型
		Args:
			model (type): 模型类
			excludes (list): 例外
		Returns:
			返回模型的所有行关系模型
		"""
		inline_models = getattr(model, "INLINE_MODELS", None)
		if inline_models is not None: return inline_models

		excludes += getattr(model, "EXCLUDE_INLINES", [])

		return ModelHelper.localRelatedModels(model, excludes)

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

	"""分隔符"""
