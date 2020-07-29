from game_module.models import ParamRate
from item_module.models import *


# ===================================================
#  题目糖属性值表
# ===================================================
class QuesSugarParam(ParamRate):

	class Meta:
		verbose_name = verbose_name_plural = "题目糖属性值"

	# 题目糖
	sugar = models.ForeignKey("QuesSugar", on_delete=models.CASCADE,
							  verbose_name="题目糖")

	# 最大值
	def maxVal(self):
		return None

	# 最小值
	def minVal(self):
		return None


# ===================================================
#  题目糖价格
# ===================================================
class QuesSugarPrice(Currency):

	class Meta:
		verbose_name = verbose_name_plural = "题目糖价格"

	# 物品
	item = models.OneToOneField('QuesSugar', on_delete=models.CASCADE,
							 verbose_name="物品")


# ===================================================
#  题目糖表
# ===================================================
@ItemManager.registerItem("题目糖")  #, ContItems.QuesSugarPackItem)
class QuesSugar(BaseItem, ParamsObject):

	LIST_DISPLAY_APPEND = ['adminBuyPrice', 'adminParamBases']

	# 题目
	question = models.ForeignKey("Question", on_delete=models.CASCADE, verbose_name="对应题目")

	# 出售价格（出售固定为金币，为0则不可出售）
	sell_price = models.PositiveIntegerField(default=0, verbose_name="出售价格")

	# 获得概率（*100）
	get_rate = models.PositiveSmallIntegerField(default=50, verbose_name="获得概率")

	# 获得个数
	get_count = models.PositiveSmallIntegerField(default=1, verbose_name="获得个数")

	# @classmethod
	# def contItemClass(cls):
	# 	return ContItems.QuesSugarPackItem

	# 管理界面用：显示购入价格
	def adminBuyPrice(self):
		return self.buyPrice()

	adminBuyPrice.short_description = "购入价格"

	@classmethod
	def paramBaseClass(cls): return QuesSugarParam

	# 转化为 dict
	def convert(self):
		buy_price = ModelUtils.objectToDict(self.buyPrice())

		if type == "shop":
			return {
				'id': self.id,
				'type': self.TYPE.value,
				'price': buy_price
			}

		res = super().convert()

		res['question_id'] = self.question
		res['buy_price'] = buy_price
		res['sell_price'] = self.sell_price
		res['get_rate'] = self.get_rate
		res['get_count'] = self.get_count
		res['params'] = ModelUtils.objectsToDict(self.params())

		return res

	# 获取所有的属性成长率
	@CacheHelper.staticCache
	def _paramBases(self):
		return self.quessugarparam_set.all()

	# 可否被购买
	def isBoughtable(self):
		buy_price: Currency = self.buyPrice()
		if buy_price is None: return False
		return not buy_price.isEmpty()

	# 购买价格
	@CacheHelper.staticCache
	def buyPrice(self):
		try: return self.quessugarprice
		except QuesSugarPrice.DoesNotExist: return None

	# # 获取属性值
	# def param(self, param_id=None, attr=None):
	# 	param = None
	# 	if param_id is not None:
	# 		param = self.params().filter(param_id=param_id)
	# 	if attr is not None:
	# 		param = self.params().filter(param__attr=attr)
	#
	# 	if param is None or not param.exists(): return 0
	#
	# 	return param.first().getValue()