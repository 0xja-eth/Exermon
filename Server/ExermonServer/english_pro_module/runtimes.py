
from utils.runtime_manager import *
from .models import *


# ===================================================
#  运行时商品
# ===================================================
class RuntimeShopItem:

	item: BaseExerProItem = None
	order = 0
	gold = 0

	is_bought = False

	def __init__(self, order, item: BaseExerProItem):
		self.order = order
		self.item = item
		self.is_bought = False

		self.gold = self.generatePrice()

	def convertToDict(self):
		"""
		转化为字典
		Returns:
			返回转化后的字典
		"""
		return {
			'order': self.order,
			'id': self.item.id,
			'is_bought': self.is_bought,
			'type': self.item.TYPE.value,
			'price': {'gold': self.gold}  # 为了保证接口一致
		}

	def generatePrice(self):
		"""
		生成价格
		Returns:
			返回价格
		"""
		from utils.calc_utils import ShopItemGenerator

		return ShopItemGenerator.generatePrice(self.item)


# ===================================================
#  运行时商店
# ===================================================
class RuntimeShop(RuntimeData):

	pro_record: ExerProRecord = None
	type_: int = 0

	items: list = None

	def __init__(self, pro_record: ExerProRecord, type_: int):
		self.pro_record = pro_record
		self.type_ = type_
		self.items = []

	def convertToDict(self):
		"""
		转化为字典
		Returns:
			返回转化后的字典
		"""
		return {'items': ModelUtils.objectsToDict(self.items)}

	def generate(self):
		"""
		生成商品
		"""
		from utils.calc_utils import ShopItemGenerator
		ShopItemGenerator(self)

	def addShopItem(self, item: BaseExerProItem):
		"""
		添加商品
		Args:
			order (int): 序号
			item (BaseExerProItem): 商品
		"""
		order = len(self.items)
		self.items.append(RuntimeShopItem(order, item))

	def contains(self, item: BaseExerProItem):
		"""
		是否包含某物品的商品
		Args:
			item (BaseExerProItem): 物品
		Returns:
			是否包含某物品的商品
		"""
		for item_ in self.items:
			if item_.item == item: return True

		return False

	def buy(self, order):
		"""
		购买商品
		Args:
			order (int): 序号
		"""
		if order >= len(self.items):
			raise GameException(ErrorType.ShopItemNotExist)

		item: RuntimeShopItem = self.items[order]
		if item.is_bought:
			raise GameException(ErrorType.ShopItemNotExist)

		price = item.gold

		if price > self.pro_record.gold:
			raise GameException(ErrorType.InvalidBuyNum)

		else:
			item.is_bought = True
			self.pro_record.gainGold(-price)

	def getKey(self) -> object:
		return "%d-%d" % (self.pro_record.id, self.type_)


RuntimeManager.register(RuntimeShop)