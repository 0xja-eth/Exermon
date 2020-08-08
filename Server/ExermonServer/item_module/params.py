from django.db.models.query import QuerySet

from game_module.models import BaseParam, ParamValue

from utils.cache_utils import CacheHelper
from utils.model_utils import Common as ModelUtils


# region 属性对象


class BaseParamsObject:
	"""
	基本属性对象
	"""
	# 管理界面显示属性
	def _adminParams(self, params):
		from django.utils.html import format_html

		res = ''
		for p in params:
			res += str(p) + "<br>"

		return format_html(res)

	def _getVals(self, db_vals, val_func,
				 param_cla, clamp=True) -> QuerySet or list:

		if param_cla is None: return []

		# 尝试获取数据库值（如果有的话）
		if db_vals is not None: return db_vals

		vals = []
		params = BaseParam.objs()

		for param in params:
			val = param_cla(param=param)
			val.setValue(val_func(param_id=param.id), clamp)
			vals.append(val)

		return vals

	def _getVal(self, params, **kwargs):

		if isinstance(params, QuerySet):
			params = params.filter(**kwargs)

			if not params.exists(): return 0
			param: ParamValue = params.first()

		else:
			param: ParamValue = ModelUtils.get(params, **kwargs)

		return 0 if param is None else param.getValue()


class ParamsObject(BaseParamsObject):
	"""
	拥有属性计算的对象
	术语：Val：实际值，Base：基础值，Rate：成长率
	"""

	# 管理界面用：显示属性基础值
	def adminParamVals(self):
		return self._adminParams(self.paramVals())

	adminParamVals.short_description = "属性实际值"

	# 管理界面用：显示属性基础值
	def adminParamBases(self):
		return self._adminParams(self.paramBases())

	adminParamBases.short_description = "属性基础值"

	# 管理界面用：显示属性成长率
	def adminParamRates(self):
		return self._adminParams(self.paramRates())

	adminParamRates.short_description = "属性成长率"

	# 管理界面用：显示属性成长率
	def adminBattlePoint(self):
		return self.battlePoint()

	adminBattlePoint.short_description = "战斗力"

	# 类型配置
	@classmethod
	def paramValueClass(cls):
		return cls.paramBaseClass()

	@classmethod
	def paramBaseClass(cls): return None

	@classmethod
	def paramRateClass(cls): return None

	# 数据库值获取（默认为 None 表示无）
	def _paramVals(self) -> QuerySet:
		return self._paramBases()

	def _paramBases(self) -> QuerySet:
		return None

	def _paramRates(self) -> QuerySet:
		return None

	# 值计算（用于无法从数据库直接获取的属性，一般用于计算）
	def _paramVal(self, **kwargs) -> float:
		return self._paramBase(**kwargs)

	def _paramBase(self, **kwargs) -> float:
		return 0

	def _paramRate(self, **kwargs) -> float:
		return 0

	# 最终值数组（缓存）
	@CacheHelper.normalCache
	def paramVals(self) -> QuerySet or list:
		"""
		获取所有实际属性数组
		Returns:
			返回所有实际属性的数组（元素类型为 ExerParamBase）
		"""
		return self._getVals(self._paramVals(), self._paramVal,
							 self.paramValueClass())

		# # 尝试获取数据库值（如果有的话）
		# vals = self._paramVals()
		# if vals is not None: return vals
		#
		# vals = []
		# params = BaseParam.objs()
		#
		# for param in params:
		# 	val = self.paramValueClass()(param=param)
		# 	val.setValue(self._paramVal(param_id=param.id), False)
		# 	vals.append(val)
		#
		# return vals

	@CacheHelper.normalCache
	def paramBases(self) -> QuerySet or list:
		return self._getVals(self._paramBases(), self._paramBase,
							 self.paramBaseClass())

	@CacheHelper.normalCache
	def paramRates(self) -> QuerySet or list:
		return self._getVals(self._paramRates(), self._paramRate,
							 self.paramRateClass())

	# 单个值获取（通过缓存）
	def paramVal(self, **kwargs):
		return self._getVal(self.paramVals(), **kwargs)

	def paramBase(self, **kwargs):
		return self._getVal(self.paramBases(), **kwargs)

	def paramRate(self, **kwargs):
		return self._getVal(self.paramRates(), **kwargs)

	# 清除属性缓存
	def _clearParamsCache(self):
		CacheHelper.clearCache(self,
			self.paramVals, self.paramBases, self.paramRates)

	# 战斗力
	def battlePoint(self):
		from utils.calc_utils import BattlePointCalc
		return BattlePointCalc.calc(self.paramVal)


class EquipParamsObject(BaseParamsObject):
	"""
	装备属性计算的对象
	术语：Level：等级加成值，Base：基础值
	"""

	# 管理界面用：显示属性基础值
	def adminLevelParams(self):
		return self._adminParams(self.levelParams())

	adminLevelParams.short_description = "等级属性值"

	# 管理界面用：显示属性基础值
	def adminBaseParams(self):
		return self._adminParams(self.baseParams())

	adminBaseParams.short_description = "基础属性值"

	# 类型配置
	@classmethod
	def levelParamClass(cls): return None

	@classmethod
	def baseParamClass(cls): return None

	# 数据库值获取或者间接获取（默认为 None 表示无）
	def _levelParams(self) -> QuerySet or list:
		return None

	def _baseParams(self) -> QuerySet or list:
		return None

	# 值计算（用于无法从数据库直接获取的属性，一般用于计算）
	def _levelParam(self, **kwargs) -> float:
		return 0

	def _baseParam(self, **kwargs) -> float:
		return 0

	# 最终值数组（缓存）
	@CacheHelper.normalCache
	def levelParams(self) -> QuerySet or list:
		return self._getVals(self._levelParams(), self._levelParam,
							 self.levelParamClass(), False)

	@CacheHelper.normalCache
	def baseParams(self) -> QuerySet or list:
		return self._getVals(self._baseParams(), self._baseParam,
							 self.baseParamClass(), False)

	# 单个值获取（通过缓存）
	def levelParam(self, **kwargs):
		return self._getVal(self.levelParams(), **kwargs)

	def baseParam(self, **kwargs):
		return self._getVal(self.baseParams(), **kwargs)

	# 清除属性缓存
	def _clearParamsCache(self):
		CacheHelper.clearCache(self,
			self.levelParams, self.baseParams)

# endregion
