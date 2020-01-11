from django.conf import settings
from .exception import ErrorType, ErrorException
from .model_utils import Common as ModelUtils

from enum import Enum
import re

# ===============================
# 视图管理器：处理视图函数中的共有业务逻辑
# ===============================
class Common:

	# 确保授权码正确
	@classmethod
	def ensureAuth(cls, auth):
		if auth != settings.AUTH_KEY:
			raise ErrorException(ErrorType.PermissionDenied)

	# 确保字符串格式按照指定正则表达式
	@classmethod
	def ensureRegexp(cls, val: str, reg, error: ErrorType, empty=False):
		# 空校验
		if not val and not empty: raise ErrorException(error)

		if not re.compile(reg).search(val): raise ErrorException(error)

	# 确保某ID存在于枚举数据中
	@classmethod
	def ensureEnumData(cls, id: int, enum_type, error: ErrorType, empty=False):
		# 空校验
		if id is None and not empty: raise ErrorException(error)

		if id is not None:
			try: enum_type(id)
			except: raise ErrorException(error)

	# 确保某模型数据对象存在
	@classmethod
	def ensureObjectExist(cls, obj_type, error, objects=None,
						  include_deleted=False, **args):
		if not cls.hasObjects(obj_type, objects, include_deleted, **args):
			raise ErrorException(error)

	# 确保某模型数据对象不存在
	@classmethod
	def ensureObjectNotExist(cls, obj_type, error, objects=None,
							 include_deleted=False, **args):
		if cls.hasObjects(obj_type, objects, include_deleted, **args):
			raise ErrorException(error)

	# 是否存在某模型数据对象
	@classmethod
	def hasObjects(cls, obj_type, objects=None,
				   include_deleted=False, **args):

		if objects is None: objects = obj_type.objects.all()

		# 实际上执行查询的部分：
		try:
			if not include_deleted and hasattr(obj_type, 'is_deleted'):
				return objects.filter(is_deleted=False, **args).exists()
			else:
				return objects.filter(**args).exists()

		except:
			raise ErrorException(ErrorType.ParameterError)

	# 获取模型数据对象
	@classmethod
	def getObject(cls, obj_type, error, objects=None,
				  return_type='QuerySet', include_deleted=False, **args):

		if objects is None: objects = obj_type.objects.all()

		# 如果是获取 object：
		if return_type == 'object':

			query_set = cls.getObject(obj_type, error, objects,
									  'QuerySet', include_deleted, **args)

			if query_set.exists():
				return query_set[0]

			else:
				raise ErrorException(error)

		# 如果是获取 字典 数据（通过 convertToDict）：
		if return_type == 'dict':
			object = cls.getObject(obj_type, error, objects,
								   'object', include_deleted, **args)

			return object.convertToDict()

		# 实际上执行查询的部分：
		try:
			if not include_deleted and hasattr(obj_type, 'is_deleted'):
				return objects.filter(is_deleted=False, **args)
			else:
				return objects.filter(**args)

		except:
			raise ErrorException(ErrorType.ParameterError)

	# 获取模型数据对象集
	@classmethod
	def getObjects(cls, obj_type, objects=None, return_type='QuerySet',
				   include_deleted=False, **args):

		# 如果没有提供 objects，获取全部的objects：
		if objects is None: objects = obj_type.objects.all()

		# 过滤 deleted：
		if not include_deleted and hasattr(obj_type, 'is_deleted'):
			result = objects.filter(is_deleted=False)
		else:
			result = objects

		# 执行查询：
		result = result.filter(**args)

		if return_type == 'dict':

			result = ModelUtils.objectsToDict(result)

		return result

	"""	
	def sendSMS(text, mobile):
		# text 必须要跟云片后台的模板内容 保持一致，不然发送不出去！
		parmas = {'apikey': settings.SMS_API_KEY, 'mobile': mobile, 'text': text}
	
		res = requests.post(settings.SMS_SEND_URL, data=parmas)
	
		print('sendSMS: ' + str(res))
	"""


