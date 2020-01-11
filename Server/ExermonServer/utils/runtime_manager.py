import threading, asyncio, traceback


# =====================
# 运行时数据的父类
# 定义了增删改查的公共操作函数
# =====================
class RuntimeData:

	def add(self):
		pass

	def delete(self):
		pass


# =====================
# 运行时数据管理器
# 所有运行时管理器的父类
# 对运行时数据进行增删改查
# 也对运行时的更新函数进行处理
# =====================
class RuntimeManager:

	# 更新时间
	PROCESS_DELTA = 0.1

	# 数据储存池
	objects = {}

	# 事件操作池
	event_processors = []

	# 运行时数据操作
	# 注册一个数据类型（类型）
	@classmethod
	def register(cls, type):
		if not cls.contains(type):
			cls.objects[type] = {}

	# 添加元素
	@classmethod
	def add(cls, type, key, data=None, **args):
		object = cls.get(type)

		if object is not None:
			data = data or type(**args)

			object[key] = data
			data.add()

	# 获取元素
	@classmethod
	def get(cls, type, key=None, data=None):
		# 首先判断 type 是否存在
		if not cls.contains(type): return None

		# 如果只是获取 type，直接返回
		if data is None and key is None:
			return cls.objects[type]

		# 如果是 data 传入，返回所在键
		if data is not None:
			return cls._getKey(type, data)

		# 否则返回 data
		if cls.contains(type, key):
			return cls.objects[type][key]

		return None

	# 获取元素对应的键（第一个键）
	@classmethod
	def _getKey(cls, type, data):
		# 获取类型对象
		object = cls.get(type)
		if object is not None:
			keys = object.keys()
			values = object.values()
			index = list(values).index(data)
			return keys[index]

		return None

	# 删除元素
	@classmethod
	def delete(cls, type, key=None, data=None):

		# 获取正确的 key 和 data
		if data is not None: # 直接删除数据，获取 key
			key = cls._getKey(type, data)
		elif key is not None: # 删除键，获取 data
			data = cls.get(type, key)

		# 若数据存在，则删除
		if data is not None and key is not None:
			data.delete()
			del cls.objects[type][key]

	# 包含元素
	@classmethod
	def contains(cls, type, key=None, data=None):
		# 首先判断 type 是否存在
		if type not in cls.objects: return False

		# 如果只是判断 type，直接返回True
		if data is None and key is None: return True

		# 获取类型对象
		object = cls.get(type)
		if data is not None:  # 如果是直接判断数据
			return data in object.values()

		return key in object

	# 更新事件操作
	# 注册更新事件
	@classmethod
	def registerEvent(cls, event):
		cls.event_processors.append(event)

	# 更新
	@classmethod
	async def update(cls):
		while True:
			# 对于每一个 event_processors 内的函数都进行操作

			# 测试
			p1 = Player.objects.get(id=1)
			p2 = Player.objects.get(id=2)
			hp1 = p1.humanpack
			hp2 = p2.humanpack

			item = HumanItem.objects.all()[0]
			item.max_count = 16
			item.save()

			hp1.gainItems(item)
			hp2.gainItems(item, 15)
			hp1.gainItems(item, 20)
			items = hp1.lostItems(item, 8)
			hp1.transferItems(hp2, item, 8)

			# es = p.exerslot
			# egp = p.exergiftpool
			# peg = egp.contItems()[0].targetContItem()
			# es.setGift(subject_id=1, player_gift=peg)

			for method in cls.event_processors:
				try: await method()
				except: traceback.print_exc()

			await asyncio.sleep(cls.PROCESS_DELTA)

	# 事件循环
	@classmethod
	def eventLoop(cls, loop):

		future = cls.update()
		loop.run_until_complete(future)

from exermon_module.models import *
from item_module.models import *
from player_module.models import *

# 创建一个事件循环thread_loop
thread_loop = asyncio.new_event_loop()

# 将thread_loop作为参数传递给子线程
t = threading.Thread(target=RuntimeManager.eventLoop, args=(thread_loop,))
t.daemon = True
t.start()
