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
	def add(cls, type, key, data=None, **kwargs):
		object = cls.get(type)

		if object is not None:
			data = data or type(**kwargs)

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

	# # 测试函数
	# @classmethod
	# def testFunc(cls):
	#
	# 	from utils.test_utils import Common as TestUtils
	#
	# 	# def createTerms():
	# 	# 	if GameTerm.get().exists(): return
	# 	#
	# 	# 	term = GameTerm.objects.create()
	# 	#
	# 	# 	createSubjects(term)
	# 	# 	createBaseParams(term)
	# 	# 	createUsableItemType(term)
	# 	# 	createHumanEquipType(term)
	# 	# 	createExerEquipType(term)
	# 	# 	createExerStar(term)
	# 	# 	createExerGiftStar(term)
	# 	#
	# 	# def createSubjects(term):
	# 	# 	Subject.objects.create(term=term, name="语文", max_score=150, force=True)
	# 	# 	Subject.objects.create(term=term, name="数学", max_score=150, force=True)
	# 	# 	Subject.objects.create(term=term, name="英语", max_score=150, force=True)
	# 	# 	Subject.objects.create(term=term, name="物理")
	# 	# 	Subject.objects.create(term=term, name="化学")
	# 	# 	Subject.objects.create(term=term, name="生物")
	# 	# 	Subject.objects.create(term=term, name="政治")
	# 	# 	Subject.objects.create(term=term, name="历史")
	# 	# 	Subject.objects.create(term=term, name="地理")
	# 	#
	# 	# def createBaseParams(term):
	# 	# 	BaseParam.objects.create(term=term, name="体力值", attr='MHP', min_value=1)
	# 	# 	BaseParam.objects.create(term=term, name="精力值", attr='MMP')
	# 	# 	BaseParam.objects.create(term=term, name="攻击力", attr='ATK')
	# 	# 	BaseParam.objects.create(term=term, name="防御力", attr='DEF')
	# 	# 	BaseParam.objects.create(term=term, name="回避率", attr='EVA', scale=10000)
	# 	# 	BaseParam.objects.create(term=term, name="暴击率", attr='CRI', scale=10000)
	# 	#
	# 	# def createUsableItemType(term):
	# 	# 	UsableItemType.objects.create(term=term, name="补给道具")
	# 	# 	UsableItemType.objects.create(term=term, name="强化道具")
	# 	# 	UsableItemType.objects.create(term=term, name="功能道具")
	# 	# 	UsableItemType.objects.create(term=term, name="宝箱")
	# 	# 	UsableItemType.objects.create(term=term, name="材料道具")
	# 	# 	UsableItemType.objects.create(term=term, name="任务道具")
	# 	# 	UsableItemType.objects.create(term=term, name="其他")
	# 	#
	# 	# def createHumanEquipType(term):
	# 	# 	HumanEquipType.objects.create(term=term, name="武器")
	# 	# 	HumanEquipType.objects.create(term=term, name="头部")
	# 	# 	HumanEquipType.objects.create(term=term, name="身体")
	# 	# 	HumanEquipType.objects.create(term=term, name="腿部")
	# 	# 	HumanEquipType.objects.create(term=term, name="其他")
	# 	#
	# 	# def createExerEquipType(term):
	# 	# 	ExerEquipType.objects.create(term=term, name="武器")
	# 	# 	ExerEquipType.objects.create(term=term, name="头部")
	# 	# 	ExerEquipType.objects.create(term=term, name="身体")
	# 	# 	ExerEquipType.objects.create(term=term, name="腿部")
	# 	# 	ExerEquipType.objects.create(term=term, name="其他")
	# 	#
	# 	# def createExerStar(term):
	# 	# 	ExerStar.objects.create(term=term, name="一星", max_level=40,
	# 	# 							level_exp_factors={'a': 1.4, 'b': -3, 'c': 300})
	# 	# 	ExerStar.objects.create(term=term, name="二星", max_level=50,
	# 	# 							level_exp_factors={'a': 1.84, 'b': -2, 'c': 315})
	# 	# 	ExerStar.objects.create(term=term, name="三星", max_level=60,
	# 	# 							level_exp_factors={'a': 2.33, 'b': -1, 'c': 340})
	# 	# 	ExerStar.objects.create(term=term, name="四星", max_level=70,
	# 	# 							level_exp_factors={'a': 2.88, 'b': 0, 'c': 375})
	# 	# 	ExerStar.objects.create(term=term, name="五星", max_level=80,
	# 	# 							level_exp_factors={'a': 3.6, 'b': 1, 'c': 420})
	# 	# 	ExerStar.objects.create(term=term, name="六星", max_level=100,
	# 	# 							level_exp_factors={'a': 4.5, 'b': 2, 'c': 500})
	# 	#
	# 	# def createExerGiftStar(term):
	# 	# 	ExerGiftStar.objects.create(term=term, name="一星")
	# 	# 	ExerGiftStar.objects.create(term=term, name="二星")
	# 	# 	ExerGiftStar.objects.create(term=term, name="三星")
	# 	#
	# 	# createTerms()
	#
	# 	# def createPlayer(username, name="", grade=1, cid=None):
	# 	# 	player = Player()
	# 	# 	player.username = username
	# 	# 	player.create(name, grade, cid)
	# 	#
	# 	# 	return player
	#
	# 	# def createRandomItem(type, cnt):
	# 	# 	import random
	# 	#
	# 	# 	for i in range(cnt):
	# 	# 		item: BaseItem = type()
	# 	# 		item.name = "测试%3d" % i
	# 	#
	# 	# 		if isinstance(item, Exermon):
	# 	# 			item.star_id = random.randrange(1, 7)
	# 	# 			item.subject_id = random.randrange(1, 10)
	# 	# 			item.e_type = random.randrange(1, 5)
	# 	#
	# 	# 		if isinstance(item, ExerGift):
	# 	# 			item.star_id = random.randrange(1, 4)
	# 	#
	# 	# 		if isinstance(item, (ExerSkill, ExerFrag)):
	# 	# 			item.o_exermon_id = random.randrange(
	# 	# 				1, Exermon.objects.count()+1)
	# 	#
	# 	# 		if isinstance(item, (ExerEquip, HumanEquip)):
	# 	# 			item.e_type_id = random.randrange(1, 6)
	# 	#
	# 	# 		if isinstance(item, (ExerItem, HumanItem)):
	# 	# 			item.i_type_id = random.randrange(1, 8)
	# 	#
	# 	# 		item.save()
	#
	# 	# createRandomItem(Exermon, 81)
	# 	# createRandomItem(ExerSkill, 81)
	# 	# createRandomItem(HumanItem, 150)
	# 	# createRandomItem(ExerItem, 75)
	# 	# createRandomItem(HumanEquip, 80)
	# 	# createRandomItem(ExerEquip, 100)
	# 	# createRandomItem(ExerGift, 30)
	# 	# createRandomItem(ExerFrag, 40)
	#
	# 	p: Player = Player.objects.all().first()
	#
	# 	from player_module.views import Common
	#
	# 	Common.addOnlinePlayer(p, None)
	#
	# 	hp: HumanPack = p.humanPack()
	# 	ep: ExerPack = p.exerPack()
	# 	es: ExerSlot = p.exerSlot()
	# 	eh: ExerHub = p.exerHub()
	# 	egp: ExerGiftPool = p.exerGiftPool()
	#
	# 	exermons = Exermon.objects.all()
	# 	human_items = HumanItem.objects.all()
	# 	human_equips = HumanEquip.objects.all()
	# 	exer_items = ExerItem.objects.all()
	# 	exer_equips = ExerEquip.objects.all()
	# 	exer_gifts = ExerGift.objects.all()
	#
	# 	human_item1 = human_items[0]
	# 	human_item2 = human_items[1]
	# 	human_item3 = human_items[2]
	#
	# 	exermon1 = exermons[1]
	# 	exermon2 = exermons[3]
	# 	exermon3 = exermons.last()
	#
	# 	"========== 背包物品测试 =========="
	#
	# 	# TestUtils.start('人类背包测试')
	# 	#
	# 	# hp.show()
	# 	# TestUtils.catch('查看容器情况')
	# 	#
	# 	# cont_item = hp.gainItems(human_item1, 50)[0]
	# 	# TestUtils.catch('hp.gainItems(human_item1, 50)')
	# 	#
	# 	# hp.show()
	# 	# TestUtils.catch('查看容器情况（缓存）')
	# 	# hp.show(db=True)
	# 	# TestUtils.catch('查看容器情况（数据库）')
	# 	#
	# 	# hp.gainItems(human_item2, 199)
	# 	# TestUtils.catch('hp.gainItems(human_item2, 199)')
	# 	#
	# 	# hp.show()
	# 	# TestUtils.catch('查看容器情况（缓存）')
	# 	# hp.show(db=True)
	# 	# TestUtils.catch('查看容器情况（数据库）')
	# 	#
	# 	# hp.gainItems(human_item1, 50)
	# 	# hp.gainItems(human_item3, 19)
	# 	# TestUtils.catch('hp.gainItems(human_item1, 50) & hp.gainItems(human_item3, 19)')
	# 	#
	# 	# hp.show()
	# 	# TestUtils.catch('查看容器情况（缓存）')
	# 	# hp.show(db=True)
	# 	# TestUtils.catch('查看容器情况（数据库）')
	# 	#
	# 	# hp.lostItems(human_item2, 101)
	# 	# TestUtils.catch('hp.lostItems(human_item2, 101)')
	# 	#
	# 	# hp.show()
	# 	# TestUtils.catch('查看容器情况（缓存）')
	# 	# hp.show(db=True)
	# 	# TestUtils.catch('查看容器情况（数据库）')
	# 	#
	# 	# cont_item2, _ = hp.splitItem(cont_item, 11)
	# 	# TestUtils.catch('hp.splitItem(cont_item, 11)')
	# 	#
	# 	# hp.show()
	# 	# TestUtils.catch('查看容器情况（缓存）')
	# 	# hp.show(db=True)
	# 	# TestUtils.catch('查看容器情况（数据库）')
	# 	#
	# 	# hp.mergeItem([cont_item, cont_item2])
	# 	# TestUtils.catch('hp.mergeItem([cont_item, cont_item2])')
	# 	#
	# 	# hp.show()
	# 	# TestUtils.catch('查看容器情况（缓存）')
	# 	# hp.show(db=True)
	# 	# TestUtils.catch('查看容器情况（数据库）')
	# 	#
	# 	# try:
	# 	# 	hp.lostItems(human_item1, 101)
	# 	# except ErrorException as exception:
	# 	# 	TestUtils.catch('抛出异常测试')
	# 	# 	traceback.print_exc()
	# 	#
	# 	# hp.clearCache()
	# 	# TestUtils.catch('清除缓存并储存容器情况')
	# 	#
	# 	# hp.show()
	# 	# TestUtils.catch('查看容器情况（缓存）')
	# 	# hp.show(db=True)
	# 	# TestUtils.catch('查看容器情况（数据库）')
	# 	#
	# 	# # 抛出异常
	# 	# TestUtils.end('人类背包测试结束')
	#
	# 	# TestUtils.start('装备艾瑟萌测试')
	# 	#
	# 	# eh.show()
	# 	# TestUtils.catch('查看艾瑟萌仓库情况')
	#
	# 	# player_exers3 = eh.gainItems(exermon3, 2)
	# 	# TestUtils.catch('eh.gainItems(exermon3, 2)')
	# 	#
	# 	# eh.show()
	# 	# TestUtils.catch('查看容器情况（缓存）')
	# 	# eh.show(db=True)
	# 	# TestUtils.catch('查看容器情况（数据库）')
	# 	#
	# 	# player_exers1 = eh.gainItems(exermon1, 5)
	# 	# TestUtils.catch('eh.gainItems(exermon1, 5)')
	# 	# player_exers2 = eh.gainItems(exermon2, 2)
	# 	# TestUtils.catch('eh.gainItems(exermon2, 2)')
	# 	#
	# 	# eh.show()
	# 	# TestUtils.catch('查看容器情况（缓存）')
	# 	# eh.show(db=True)
	# 	# TestUtils.catch('查看容器情况（数据库）')
	#
	# 	# player_exer = eh.contItem(item=exermon3)
	# 	# # player_exers5 = eh.contItem(item=exermon2)
	# 	#
	# 	# es.show()
	# 	# TestUtils.catch('查看槽情况')
	# 	#
	# 	# es.setPlayerExer(player_exer)
	# 	# TestUtils.catch('es.setPlayerExer(player_exer)')
	# 	#
	# 	# eh.show()
	# 	# TestUtils.catch('查看容器情况（缓存）')
	# 	# es.show()
	# 	# TestUtils.catch('查看槽情况（缓存）')
	# 	# es.show(db=True)
	# 	# TestUtils.catch('查看槽情况（数据库）')
	# 	#
	# 	# es.setPlayerExer(None, 2)
	# 	# TestUtils.catch('es.setPlayerExer(None, 2)')
	# 	#
	# 	# eh.show()
	# 	# TestUtils.catch('查看容器情况（缓存）')
	# 	# es.show()
	# 	# TestUtils.catch('查看槽情况（缓存）')
	# 	# es.show(db=True)
	# 	# TestUtils.catch('查看槽情况（数据库）')
	# 	#
	# 	# player_gifts = egp.gainItems(exer_gifts[0], 10)
	# 	# TestUtils.catch('egp.gainItems(exer_gifts[0], 10)')
	# 	#
	# 	# egp.show()
	# 	# TestUtils.catch('查看容器情况（缓存）')
	# 	#
	# 	# print('player_gifts: ' + str(player_gifts))
	# 	#
	# 	# es.setPlayerGift(player_gifts[1], 8)
	# 	# TestUtils.catch('es.setPlayerGift(player_gifts[1], 8)')
	# 	#
	# 	# egp.show()
	# 	# TestUtils.catch('查看容器情况（缓存）')
	# 	# egp.show(db=True)
	# 	# TestUtils.catch('查看容器情况（数据库）')
	# 	# es.show()
	# 	# TestUtils.catch('查看槽情况（缓存）')
	# 	# es.show(db=True)
	# 	# TestUtils.catch('查看槽情况（数据库）')
	# 	#
	# 	# p.clearCache()
	# 	# TestUtils.catch('清除缓存并储存容器情况')
	# 	#
	# 	# es.show()
	# 	# TestUtils.catch('查看槽情况（缓存）')
	# 	# es.show(db=True)
	# 	# TestUtils.catch('查看槽情况（数据库）')
	# 	#
	# 	# eh.show()
	# 	# TestUtils.catch('查看容器情况（缓存）')
	# 	# eh.show(db=True)
	# 	# TestUtils.catch('查看容器情况（数据库）')
	# 	#
	# 	# egp.show()
	# 	# TestUtils.catch('查看容器情况（缓存）')
	# 	# egp.show(db=True)
	# 	# TestUtils.catch('查看容器情况（数据库）')
	# 	#
	# 	# TestUtils.end('装备艾瑟萌测试结束')
	#
	# 	TestUtils.start('艾瑟萌装备槽测试')
	#
	# 	exer_slot_item: ExerSlotItem = es.contItem(subject_id=2)
	# 	equip_slot: ExerEquipSlot = exer_slot_item.exerEquipSlot()
	#
	# 	exer_equip1 = exer_equips.get(e_type_id=1)
	# 	exer_equip2 = exer_equips.get(e_type_id=2)
	# 	exer_equip3 = exer_equips.get(e_type_id=3)
	#
	# 	pack_equip1 = ep.contItem(item=exer_equip1)
	# 	pack_equip2 = ep.contItem(item=exer_equip2)
	# 	pack_equip3 = ep.contItem(item=exer_equip3)
	#
	# 	equip_slot.show()
	# 	TestUtils.catch('查看背包情况')
	# 	ep.show()
	# 	TestUtils.catch('查看装备情况')
	#
	# 	equip_slot.setPackEquip(pack_equip1)
	# 	TestUtils.catch('equip_slot.setPackEquip(pack_equip1)')
	#
	# 	equip_slot.show()
	# 	TestUtils.catch('查看背包情况')
	# 	ep.show()
	# 	TestUtils.catch('查看装备情况')
	#
	# 	equip_slot.setPackEquip(pack_equip2)
	# 	equip_slot.setPackEquip(pack_equip3)
	# 	TestUtils.catch('equip_slot.setPackEquip(pack_equip2, pack_equip3)')
	#
	# 	equip_slot.show()
	# 	TestUtils.catch('查看背包情况')
	# 	ep.show()
	# 	TestUtils.catch('查看装备情况')
	#
	# 	equip_slot.setPackEquip(pack_equip2)
	# 	TestUtils.catch('equip_slot.setPackEquip(pack_equip2)')
	#
	# 	equip_slot.show()
	# 	TestUtils.catch('查看背包情况')
	# 	ep.show()
	# 	TestUtils.catch('查看装备情况')
	#
	# 	equip_slot.setPackEquip(None, 1)
	# 	TestUtils.catch('equip_slot.setPackEquip(None, 1)')
	#
	# 	equip_slot.show()
	# 	TestUtils.catch('查看背包情况')
	# 	ep.show()
	# 	TestUtils.catch('查看装备情况')
	#
	# 	p._clearCache()
	# 	TestUtils.catch('清除缓存并储存容器情况')
	#
	# 	equip_slot.show()
	# 	TestUtils.catch('查看背包情况')
	# 	ep.show()
	# 	TestUtils.catch('查看装备情况')
	#
	# 	TestUtils.end('装备艾瑟萌测试结束')
	#
	# 	temp = 2
	#
	# 	"========== 背包物品测试 =========="
	#
	# 	# human_pack1 = p1.humanpack
	# 	# human_pack2 = p2.humanpack
	#
	# 	# human_item1 = human_items[0]
	# 	# human_item2 = human_items[1]
	# 	# human_item3 = human_items[2]
	# 	#
	# 	# human_equip1 = human_equips[0]
	# 	# human_equip2 = human_equips[1]
	# 	#
	# 	# human_pack1.show("HumanPack1")
	# 	# human_pack2.show("HumanPack2")
	#
	# 	# human_pack1.lostItems(human_item2, 5000)
	#
	# 	# 测试计算性能
	# 	# TestUtils.start('人类背包测试')
	#
	# 	# human_pack1.gainItems(human_item1, 5)
	# 	# TestUtils.catch('gainItems test1')
	# 	# human_pack1.show("HumanPack1")
	# 	# TestUtils.catch('show result')
	# 	#
	# 	# human_pack1.gainItems(human_item2, 50)
	# 	# TestUtils.catch('gainItems test2')
	# 	# human_pack1.show("HumanPack1")
	# 	# TestUtils.catch('show result')
	#
	# 	# human_pack1.lostItems(human_item2, 40)
	# 	# TestUtils.catch('lostItems test1')
	# 	# human_pack1.show("HumanPack1")
	# 	# TestUtils.catch('show result')
	# 	#
	# 	# human_pack1.gainItems(human_item3, 500)
	# 	# TestUtils.catch('gainItems test3')
	# 	# human_pack1.show("HumanPack1")
	# 	# TestUtils.catch('show result')
	#
	# 	# human_pack1.gainItems(human_equip1, 2)
	# 	# TestUtils.catch('gainItems test4')
	# 	# human_pack1.show("HumanPack1")
	# 	# TestUtils.catch('show result')
	# 	#
	# 	# human_pack1.lostItems(human_equip1, 1)
	# 	# TestUtils.catch('lostItems test2')
	# 	# human_pack1.show("HumanPack1")
	# 	# TestUtils.catch('show result')
	# 	#
	# 	# human_pack1.gainItems(human_equip2, 3)
	# 	# TestUtils.catch('gainItems test5')
	# 	# human_pack1.show("HumanPack1")
	# 	# TestUtils.catch('show result')
	# 	#
	# 	# human_pack2.gainItems(human_item3, 100)
	# 	# TestUtils.catch('gainItems test6')
	# 	# human_pack2.show("HumanPack2")
	# 	# TestUtils.catch('show result')
	# 	#
	# 	# human_pack2.gainItems(human_equip2, 3)
	# 	# TestUtils.catch('gainItems test7')
	# 	# human_pack2.show("HumanPack2")
	# 	# TestUtils.catch('show result')
	# 	#
	# 	# human_pack2.transferItems(human_pack1, human_item3, 50)
	# 	# TestUtils.catch('transferItems test1')
	# 	# human_pack1.show("HumanPack1")
	# 	# human_pack2.show("HumanPack2")
	# 	# TestUtils.catch('show result')
	# 	#
	# 	# human_pack1.transferItems(human_pack2, human_equip2, 2)
	# 	# TestUtils.catch('transferItems test2')
	# 	# human_pack1.show("HumanPack1")
	# 	# human_pack2.show("HumanPack2")
	# 	# TestUtils.catch('show result')
	# 	#
	# 	# TestUtils.end('测试结束')
	#
	# 	"========== 槽物品测试 =========="

		# sbj_exers = {}
		#
		# for sbj in Subject.objs():
		# 	sbj_exers[sbj.id] = exermons.filter(subject=sbj)
		#
		# # p1.createExermons([sbj_exers[1][0], sbj_exers[2][0], sbj_exers[3][0],
		# # 				   sbj_exers[4][0], sbj_exers[5][0], sbj_exers[6][0]],
		# # 				  ['', '', '', '', '', ''])
		# #
		# # p2.createExermons([sbj_exers[1][0], sbj_exers[2][0], sbj_exers[3][0],
		# # 				   sbj_exers[5][0], sbj_exers[7][0], sbj_exers[9][1]],
		# # 				  ['', '', '', '', '', ''])
		#
		# exer_slot1 = p1.exerslot
		# exer_slot2 = p2.exerslot
		#
		# exer_slot1.show("exer_slot1")
		# exer_slot2.show("exer_slot2")
		#
		# exer_pack1 = p1.exerpack
		# exer_pack2 = p2.exerpack
		#
		# gift_pack1 = p1.exergiftpool
		# gift_pack2 = p2.exergiftpool
		#
		# exer_hub1 = p1.exerhub
		# exer_hub2 = p2.exerhub
		#
		# exer_equip1 = exer_equips[0]
		# exer_equip2 = exer_equips[1]
		# exer_equip3 = exer_equips[2]
		# exer_equip4 = exer_equips[3]
		#
		# exer_gift1 = exer_gifts[0]
		# exer_gift2 = exer_gifts[1]
		# exer_gift3 = exer_gifts[3]
		#
		# exermon1 = sbj_exers[1][1]
		# exermon2 = sbj_exers[2][1]
		#
		# print("exer_equip1.e_type = %d" % exer_equip1.e_type_id)
		# print("exer_equip2.e_type = %d" % exer_equip2.e_type_id)
		# print("exer_equip3.e_type = %d" % exer_equip3.e_type_id)
		# print("exer_equip4.e_type = %d" % exer_equip4.e_type_id)
		#
		# print("获取艾瑟萌")
		#
		# exer_hub1.gainItems(exermon1, 3)
		# exer_hub1.gainItems(exermon2, 2)
		#
		# exer_hub1.transferItems(exer_hub2, exermon1, 2)
		#
		# exer_hub1.show("exer_hub1")
		# exer_hub2.show("exer_hub2")
		#
		# print("获取艾瑟萌天赋")
		#
		# gift_pack1.gainItems(exer_gift1, 3)
		# gift_pack1.gainItems(exer_gift2, 5)
		#
		# gift_pack2.gainItems(exer_gift3, 5)
		#
		# gift_pack2.transferItems(gift_pack1, exer_gift3, 2)
		#
		# gift_pack1.show("gift_pack1")
		# gift_pack2.show("gift_pack2")
		#
		# print("获取艾瑟萌装备")
		#
		# exer_pack1.gainItems(exer_equip1, 3)
		# exer_pack1.gainItems(exer_equip2, 3)
		# exer_pack1.gainItems(exer_equip3, 3)
		# exer_pack1.gainItems(exer_equip4, 3)
		#
		# exer_pack1.transferItems(exer_pack2, exer_equip1, 2)
		# exer_pack1.transferItems(exer_pack2, exer_equip2, 2)
		# exer_pack1.transferItems(exer_pack2, exer_equip3, 1)
		#
		# exer_pack1.show("exer_pack1")
		# exer_pack2.show("exer_pack2")
		#
		# print("装备天赋")
		#
		# player_gift = gift_pack1.contItems(item=exer_gift3)[0]
		# exer_slot1.setEquip(equip_item=player_gift, index=2, subject_id=1)
		# exer_slot1.show("exer_slot1")
		# gift_pack1.show("gift_pack1")
		#
		# print("更改天赋")
		#
		# player_gift = gift_pack1.contItems(item=exer_gift2)[0]
		# exer_slot1.setEquip(equip_item=player_gift, index=2, subject_id=1)
		# exer_slot1.show("exer_slot1")
		# gift_pack1.show("gift_pack1")
		#
		# print("更改艾瑟萌")
		#
		# player_exer = exer_hub2.contItems(item=exermon1)[0]
		# exer_slot2.setEquip(equip_item=player_exer, index=1, subject_id=1)
		# exer_slot2.show("exer_slot2")
		# exer_hub2.show("exer_hub2")
		#
		# print("卸下艾瑟萌")
		#
		# exer_slot2.setEquip(equip_item=None, index=1, subject_id=1)
		# exer_slot2.show("exer_slot2")
		# exer_hub2.show("exer_hub2")
		#
		# print("卸下天赋")
		#
		# exer_slot1.setEquip(equip_item=None, index=2, subject_id=1)
		# exer_slot1.show("exer_slot1")
		# gift_pack1.show("gift_pack1")
		#
		# print("获取艾瑟萌槽装备槽")
		#
		# exer_slot_item = exer_slot1.contItem(subject_id=1)
		# equip_slot = exer_slot_item.exerequipslot
		# equip_slot.show("equip_slot")
		#
		# print("装备艾瑟萌槽装备")
		#
		# pack_equip = exer_pack1.contItems(item=exer_equip1)[0]
		# equip_slot.setEquip(equip_item=pack_equip, index=1,
		# 					e_type_id=exer_equip1.e_type_id)
		# equip_slot.show("equip_slot")
		# exer_pack1.show("exer_pack1")
		#
		# print("卸下艾瑟萌槽装备")
		#
		# equip_slot.setEquip(equip_item=None, index=1,
		# 					e_type_id=exer_equip1.e_type_id)
		# equip_slot.show("equip_slot")
		# exer_pack1.show("exer_pack1")
		#
		# print("测试完毕")

	# 更新
	@classmethod
	async def update(cls):
		while True:
			# 对于每一个 event_processors 内的函数都进行操作

			# 测试
			# cls.testFunc()

			for method in cls.event_processors:
				try: await method()
				except: traceback.print_exc()

			await asyncio.sleep(cls.PROCESS_DELTA)

	# 事件循环
	@classmethod
	def eventLoop(cls, loop):

		future = cls.update()
		loop.run_until_complete(future)

# from game_module.models import *
# from exermon_module.models import *
# from item_module.models import *
# from player_module.models import *

# 初始化
try:
	from game_module.models import GameConfigure
	GameConfigure.load()
except:
	# 打印错误路径
	traceback.print_exc()
	print("仍未建立数据库")

# 创建一个事件循环thread_loop
thread_loop = asyncio.new_event_loop()

# 将thread_loop作为参数传递给子线程
t = threading.Thread(target=RuntimeManager.eventLoop, args=(thread_loop,))
t.daemon = True
t.start()
