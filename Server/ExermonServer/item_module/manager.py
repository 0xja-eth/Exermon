from .types import *
from .models import *

from utils.model_utils import EnumMapper


# region 物品系统管理类


class ItemManager:
	"""
	物品管理类
	"""

	@classmethod
	def registerItem(cls, verbose_name):
		# cont_item_cla: 'PackContItem' = None):
		"""
		注册物品
		Args:
			verbose_name (str): 别名
			# cont_item_cla (type): 容器项类
		"""
		def wrapper(cla: BaseItem):
			print("registerItem: %s (%s)" % (cla, cla._meta.app_label))

			cla._meta.verbose_name = \
				cla._meta.verbose_name_plural = verbose_name

			cla.TYPE = eval("ItemType.%s" % cla.__name__)

			# cla.CONTITEM_CLASS = cont_item_cla

			EnumMapper.registerClass(cla)

			return cla

		return wrapper

	@classmethod
	def registerContainer(cls, verbose_name):
		"""
		注册容器
		Args:
			verbose_name (str): 别名
		"""
		def wrapper(cla: PackContainer):
			cla._meta.verbose_name = \
				cla._meta.verbose_name_plural = verbose_name

			cla.TYPE = eval("ContainerType.%s" % cla.__name__)

			EnumMapper.registerClass(cla)

			return cla

		return wrapper

	@classmethod
	def registerPackContainer(cls, verbose_name):
		return cls.registerContainer(verbose_name)

	@classmethod
	def registerSlotContainer(cls, verbose_name):
		return cls.registerContainer(verbose_name)

	@classmethod
	def registerPackContItem(cls, verbose_name,
							 container_cla: 'PackContainer',
							 accepted_item_cla: 'BaseItem'):
		"""
		注册背包容器
		Args:
			verbose_name (str): 别名
			container_cla (type): 背包容器类
			accepted_item_cla (type): 接受物品类
		"""
		def wrapper(cla: PackContItem):
			cla._meta.verbose_name = \
				cla._meta.verbose_name_plural = verbose_name

			cla.TYPE = eval("ContItemType.%s" % cla.__name__)

			cla.CONTAINER_CLA = container_cla
			cla.ACCEPTED_ITEM_CLASS = accepted_item_cla

			container = models.ForeignKey(
				container_cla, on_delete=models.CASCADE,
				null=True, blank=True, verbose_name="容器")

			item = models.ForeignKey(
				accepted_item_cla, on_delete=models.CASCADE,
				null=True, blank=True, verbose_name="物品")

			cla.add_to_class('container', container)
			cla.add_to_class('item', item)

			setupItem()
			setupContainer(cla)

			EnumMapper.registerClass(cla)

			return cla

		def setupContainer(cla: PackContItem):

			if container_cla.ACCEPTED_CONTITEM_CLASSES is None:
				container_cla.ACCEPTED_CONTITEM_CLASSES = []

			container_cla.ACCEPTED_CONTITEM_CLASSES.append(cla)

			base_cont_item_cla = cls._getCommonSuperClass(
				*container_cla.ACCEPTED_CONTITEM_CLASSES)

			container_cla.BASE_CONTITEM_CLASS = base_cont_item_cla

		def setupItem():

			accepted_item_cla.CONTAINER_CLASS = container_cla

		return wrapper

	@classmethod
	def registerSlotContItem(cls, verbose_name,
							 container_cla: 'SlotContainer',
							 **equip_item_args):
		"""
		注册槽容器
		Args:
			verbose_name (str): 别名
			container_cla (type): 容器类
			**equip_item_args (**dict): 装备项参数（键：装备项键名，值：装备项类型）
		"""
		def wrapper(cla: SlotContItem):

			cla._meta.verbose_name = \
				cla._meta.verbose_name_plural = verbose_name

			cla.TYPE = eval("ContItemType.%s" % cla.__name__)
			cla.CONTAINER_CLA = container_cla

			cla.ACCEPTED_EQUIP_ITEM_ATTRS = []
			cla.ACCEPTED_EQUIP_ITEM_CLASSES = []

			container = models.ForeignKey(
				container_cla, on_delete=models.CASCADE, verbose_name="容器")

			cla.add_to_class('container', container)

			for key in equip_item_args:
				equip_item_cla: PackContItem = equip_item_args[key]
				cla.ACCEPTED_EQUIP_ITEM_ATTRS.append(key)
				cla.ACCEPTED_EQUIP_ITEM_CLASSES.append(equip_item_cla)

				field = models.ForeignKey(equip_item_cla,
					null=True, blank=True, on_delete=models.SET_NULL,
					verbose_name=equip_item_cla._meta.verbose_name)

				cla.add_to_class(key, field)

			setupContainer(cla)

			EnumMapper.registerClass(cla)

			return cla

		def setupContainer(cla: SlotContItem):

			pack_item_clas = []

			for key in equip_item_args:
				pack_item_clas.append(equip_item_args[key])

			base_cont_item_cla = cls.\
				_getCommonSuperClass(*pack_item_clas)

			container_cla.BASE_CONTITEM_CLASS = base_cont_item_cla
			container_cla.ACCEPTED_CONTITEM_CLASSES = pack_item_clas
			container_cla.ACCEPTED_SLOTITEM_CLASS = cla

		return wrapper

	@classmethod
	def _getCommonSuperClass(cls, *clas) -> type:
		# 极端情况
		if len(clas) == 0: return None
		if len(clas) == 1: return clas[0]
		# 两个的情况
		if len(clas) == 2:
			mro1, mro2 = clas[0].mro(), clas[1].mro()

			for cla_i in mro1:
				for cla_j in mro2:
					if cla_i == cla_j: return cla_i

			return object

		# 其他情况
		cla: type = clas[0]
		for i in range(len(clas)-1):
			cla = cls._getCommonSuperClass(cla, clas[i+1])

		return cla


# endregion