# chat/routing.py
from django.conf.urls import re_path
from game_module.views import Service as Game
from player_module.views import Service as Player
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
		['cached', 'bool']],
		Game.getStaticData,  # 处理函数
		ChannelLayerTag.Self  # 是否需要响应
	],
	# 获取动态数据
	'game/data/dynamic': [[
	],
		Game.getDynamicData,  # 处理函数
		ChannelLayerTag.Self  # 是否需要响应
	],
	# 玩家注册
	'player/player/register': [[
		['un', 'str'],
		['pw', 'str'],
		['email', 'str'],
		['code', 'str']],
		Player.register,  # 处理函数
		ChannelLayerTag.Self  # 是否需要响应
	],
	# 玩家登陆
	'player/player/login': [[
		['un', 'str'],
		['pw', 'str']],
		Player.login,  # 处理函数
		ChannelLayerTag.Self  # 是否需要响应
	],
	# 玩家忘记密码
	'player/player/forget': [[
		['un', 'str'],
		['pw', 'str'],
		['email', 'str'],
		['code', 'str']],
		Player.forget,  # 处理函数
		ChannelLayerTag.Self  # 是否需要响应
	],
	# 发送验证码
	'player/player/code': [[
		['un', 'str'],
		['email', 'str'],
		['type', 'str']],
		Player.sendCode,  # 处理函数
		ChannelLayerTag.Self  # 是否需要响应
	],
	# 登出（客户端调用）
	'player/player/logout': [[
		['pid', 'int']],
		Player.logout,  # 处理函数
		ChannelLayerTag.NoLayer  # 是否需要响应
	],
	# 断开连接（内部调用）
	'player/player/disconnect': [[
		['pid', 'int'],
		['auth', 'str']],
		Player.disconnect,  # 处理函数
		ChannelLayerTag.NoLayer  # 是否需要响应
	],

}
