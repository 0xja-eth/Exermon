
from utils.runtime_manager import *
from .models import *


# ===================================================
#  ����ʱ��Ʒ
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
		ת��Ϊ�ֵ�
		Returns:
			����ת������ֵ�
		"""
		return {
			'order': self.order,
			'id': self.item.id,
			'type': self.item.TYPE,
			'price': {'gold': self.gold}  # Ϊ�˱�֤�ӿ�һ��
		}

	def generatePrice(self):
		"""
		���ɼ۸�
		Returns:
			���ؼ۸�
		"""
		from utils.calc_utils import ShopItemGenerator

		return ShopItemGenerator.generatePrice(self.item)


# ===================================================
#  ����ʱ�̵�
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
		ת��Ϊ�ֵ�
		Args:
			type (str): ����
			**kwargs (**dict): ��չ����
		Returns:
			����ת������ֵ�
		"""
		return {'items': ModelUtils.objectsToDict(self.items)}

	def generate(self):
		"""
		������Ʒ
		"""
		from utils.calc_utils import ShopItemGenerator
		ShopItemGenerator(self)

	def addShopItem(self, item: BaseExerProItem):
		"""
		�����Ʒ
		Args:
			order (int): ���
			item (BaseExerProItem): ��Ʒ
		"""
		order = len(self.items)
		self.items.append(RuntimeShopItem(order, item))

	def contains(self, item: BaseExerProItem):
		"""
		�Ƿ����ĳ��Ʒ����Ʒ
		Args:
			item (BaseExerProItem): ��Ʒ
		Returns:
			�Ƿ����ĳ��Ʒ����Ʒ
		"""
		for item_ in self.items:
			if item_.item == item: return True

		return False

	def buy(self, order, num):
		"""
		������Ʒ
		Args:
			id (int): ��ƷID
			num (int): ����
		"""
		if order >= len(self.items):
			raise GameException(ErrorType.ShopItemNotExist)

		item: RuntimeShopItem = self.items[order]
		if item.is_bought:
			raise GameException(ErrorType.ShopItemNotExist)

		price = num * item.gold

		if price > self.pro_record.gold:
			raise GameException(ErrorType.InvalidBuyNum)

		else:
			item.is_bought = True
			self.pro_record.gainGold(-price)

	def getKey(self) -> object:
		return "%d-%d" % (self.pro_record.id, self.type_)


RuntimeManager.register(RuntimeShop)