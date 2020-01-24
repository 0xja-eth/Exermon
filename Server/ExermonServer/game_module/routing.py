# chat/routing.py
from django.conf.urls import re_path
from game_module.views import Service as Game
from player_module.views import Service as Player
from item_module.views import Service as Item
from exermon_module.views import Service as Exermon
from question_module.views import Service as Question
from .consumer import ChannelLayerTag, GameConsumer

websocket_urlpatterns = [
	re_path('game/', GameConsumer),
]

DISCONNECT_ROUTE = 'player/player/disconnect'

WEBSOCKET_METHOD_ROUTER = {
	# 获取静态数据
	'game/data/static': [[
		['main_version', 'str'],
		['sub_version', 'str'],
		['cached', 'bool']
	],
		Game.getStaticData,  # 处理函数
		ChannelLayerTag.Self  # 是否需要响应
	],
	# 获取动态数据
	'game/data/dynamic': [[],
		Game.getDynamicData,  # 处理函数
		ChannelLayerTag.Self  # 是否需要响应
	],
	# 玩家注册
	'player/player/register': [[
		['un', 'str'],
		['pw', 'str'],
		['email', 'str'],
		['code', 'str']
	],
		Player.register,  # 处理函数
		ChannelLayerTag.Self  # 是否需要响应
	],
	# 玩家登陆
	'player/player/login': [[
		['un', 'str'],
		['pw', 'str']
	],
		Player.login,  # 处理函数
		ChannelLayerTag.Self  # 是否需要响应
	],
	# 玩家忘记密码
	'player/player/forget': [[
		['un', 'str'],
		['pw', 'str'],
		['email', 'str'],
		['code', 'str']
	],
		Player.forget,  # 处理函数
		ChannelLayerTag.Self  # 是否需要响应
	],
	# 发送验证码
	'player/player/code': [[
		['un', 'str'],
		['email', 'str'],
		['type', 'str']
	],
		Player.sendCode,  # 处理函数
		ChannelLayerTag.Self  # 是否需要响应
	],
	# 登出（客户端调用）
	'player/player/logout': [[
		['uid', 'int']
	],
		Player.logout,  # 处理函数
		ChannelLayerTag.NoLayer  # 是否需要响应
	],
	# 断开连接（内部调用）
	'player/player/disconnect': [[
		['uid', 'int']
	],
		Player.disconnect,  # 处理函数
		ChannelLayerTag.NoLayer  # 是否需要响应
	],
	# 选择玩家形象
	'player/create/character': [[
		['uid', 'int'],
		['name', 'str'],
		['grade', 'int'],
		['cid', 'int'],
	],
		Player.createCharacter,  # 处理函数
		ChannelLayerTag.Self  # 是否需要响应
	],
	# 选择艾瑟萌
	'player/create/exermons': [[
		['uid', 'int'],
		['eids', 'int[]'],
		['enames', 'str[]'],
	],
		Player.createExermons,  # 处理函数
		ChannelLayerTag.Self  # 是否需要响应
	],
	# 选择艾瑟萌天赋
	'player/create/gifts': [[
		['uid', 'int'],
		['gids', 'int[]'],
	],
		Player.createGifts,  # 处理函数
		ChannelLayerTag.Self  # 是否需要响应
	],
	# 获取玩家基本信息
	'player/get/basic': [[
		['uid', 'int'],
		['get_uid', 'int'],
	],
		Player.getBasic,  # 处理函数
		ChannelLayerTag.Self  # 是否需要响应
	],
	# 获取背包类容器项数据
	'item/packcontainer/get': [[
		['uid', 'int'],
		['cid', 'int'],
	],
		Item.packContainerGet,  # 处理函数
		ChannelLayerTag.Self  # 是否需要响应
	],
	# 获取背包类容器项数据
	'item/slotcontainer/get': [[
		['uid', 'int'],
		['cid', 'int'],
	],
		Item.slotContainerGet,  # 处理函数
		ChannelLayerTag.Self  # 是否需要响应
	],
	# 背包类容器获得物品
	'item/packcontainer/gain': [[
		['uid', 'int'],
		['cid', 'int'],
		['item_id', 'int'],
		['count', 'int'],
	],
		Item.packContainerGain,  # 处理函数
		ChannelLayerTag.Self  # 是否需要响应
	],
	# 背包类容器转移
	'item/packcontainer/transfer': [[
		['uid', 'int'],
		['cid', 'int'],
		['target_cid', 'int'],
		['contitem_id', 'int[]'],
		['count', 'int[]'],
	],
		Item.packContainerTransfer,  # 处理函数
		ChannelLayerTag.Self  # 是否需要响应
	],
	# 背包类容器拆分
	'item/packcontainer/split': [[
		['uid', 'int'],
		['cid', 'int'],
		['contitem_id', 'int'],
		['count', 'int'],
	],
		Item.packContainerSplit,  # 处理函数
		ChannelLayerTag.Self  # 是否需要响应
	],
	# 背包类容器组合
	'item/packcontainer/merge': [[
		['uid', 'int'],
		['cid', 'int'],
		['contitem_ids', 'int[]'],
	],
		Item.packContainerGroup,  # 处理函数
		ChannelLayerTag.Self  # 是否需要响应
	],
	# 艾瑟萌槽装备
	'exermon/exerslot/equip': [[
		['uid', 'int'],
		['cid', 'int'],
		['sid', 'int'],
		['peid', 'int'],
		['pgid', 'int'],
	],
		Exermon.exerSlotEquip,  # 处理函数
		ChannelLayerTag.Self  # 是否需要响应
	],
	# 人类装备槽装备
	'player/equipslot/equip': [[
		['uid', 'int'],
		['cid', 'int'],
		['eid', 'int'],
		['heid', 'int'],
	],
		Player.equipSlotEquip,  # 处理函数
		ChannelLayerTag.Self  # 是否需要响应
	],
	# 艾瑟萌装备槽装备
	'exermon/equipslot/equip': [[
		['uid', 'int'],
		['cid', 'int'],
		['eid', 'int'],
		['eeid', 'int'],
	],
		Exermon.equipSlotEquip,  # 处理函数
		ChannelLayerTag.Self  # 是否需要响应
	],

}
