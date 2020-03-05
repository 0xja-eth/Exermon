
# chat/consumers.py
from channels.generic.websocket import AsyncWebsocketConsumer
from channels.layers import get_channel_layer
from utils.exception import WebscoketCloseCode
from enum import Enum
import json, random, datetime


# =====================
# 发送数据类型枚举
# =====================
class EmitType(Enum):
	Link = 'link'
	Disconnect = 'disconnect'
	SeasonSwitch = 'season_switch'
	RankChanged = 'rank_changed'


# =====================
# Channel层枚举
# =====================
class ChannelLayerTag(Enum):
	AllLayer = -2
	NoLayer = -1
	Self = 0
	Battle = 1
	Common = 2


# =====================
# 游戏Consumer
# =====================
class GameConsumer(AsyncWebsocketConsumer):

	# 加入的组（{ ChannelLayerTag: string }
	joined_groups = None

	# 所关联的在线玩家
	online_info = None

	# 连接断开标记
	server_close = None

	# IP地址
	ip_address = None

	# 公用组名
	COMMON_GROUP_NAME = "COMMON"

	# 默认 Channel 层
	DEFAULT_CHANNEL_LAYER = get_channel_layer()

	# 广播全体（对全体在线玩家广播）
	@classmethod
	async def broadcast(cls, type: EmitType, data, method='response'):

		from utils.interface_manager import WebSocket

		result = WebSocket.processEmit(type, data, ChannelLayerTag.Common)

		group_name = cls.COMMON_GROUP_NAME
		group_name = cls.processGroupName(group_name)

		await cls.DEFAULT_CHANNEL_LAYER.group_send(
			group_name, {'type': method, 'data': result})

	# 处理组名（保证都是英文字符）
	@classmethod
	def processGroupName(cls, name):

		res = ''
		for c in name:
			res += chr(ord('A') + ord(c) % 26)

		return res

	# 设置关联在线玩家
	def setOnlineInfo(self, online_info):

		self.online_info = online_info

	# 获取对应的玩家
	def getPlayer(self):

		if self.online_info is None: return None

		return self.online_info.player

	# 连接操作
	# 连接服务器（内部调用）
	async def connect(self):

		# 初始化变量
		self.joined_groups = {}
		self.online_info = None
		self.server_close = False
		self.ip_address = self.scope['client']

		# 加入公共组
		await self.joinGroup(ChannelLayerTag.Common, self.COMMON_GROUP_NAME)

		# 加入组
		await self.joinGroup(ChannelLayerTag.Self, self.channel_name)

		# 初次连接，发射连接信息
		await self.emit(EmitType.Link, ChannelLayerTag.Self, {'channel_name': self.channel_name})

		# 内部函数，固定调用
		await self.accept()

	# 断开连接（内部调用）
	async def disconnect(self, close_code):

		from utils.interface_manager import WebSocket

		print("disconnect: "+str(close_code))

		# 处理断开连接
		await WebSocket.processDisconnect(self, close_code)

		# 如果是在客户端退出（即直接内部调用 disconnect 函数，没有经过 closeConnection 函数）
		if not self.server_close:
			await self.serverDisconnect(WebscoketCloseCode.ClientClose, close=False)
			"""
			await self.emitAll(EmitType.Disconnect, data={
				'channel_name': self.channel_name,
				'code': close_code,
				'message': ''
			})			
			"""

		await self.clearGroups()

	# 服务器主动关闭连接
	async def serverDisconnect(self, close_code_enum: WebscoketCloseCode, message='', close=True):

		self.server_close = True  # 标记是主动关闭

		data = self._generateDisconnectData(close_code_enum, message)

		# 全体广播，该 Consumer 关闭连接
		await self.emitAll(EmitType.Disconnect, data=data)

		# 如果需要关闭连接，调用关闭连接的内部函数，关闭连接（错误码）
		if close: await self.close(close_code_enum.value)

	# 生成断开连接发射数据
	def _generateDisconnectData(self, close_code_enum: WebscoketCloseCode, message=''):

		return {
			'channel_name': self.channel_name,
			'code': close_code_enum.value,
			'message': message,
		}

	# 组操作
	# 寻找组的标签
	def findGroupTag(self, group_name):

		for tag in self.joined_groups:
			if self.joined_groups[tag] == group_name:
				return tag

		return None

	# 获取所有加入的组的标签的数组
	def allGroupTags(self):

		tags = []
		for tag in self.joined_groups:
			tags.append(tag)

		return tags

	# 加入一个组（标签，组名）
	async def joinGroup(self, tag, group_name):

		# 对组名进行处理
		group_name = self.processGroupName(group_name)

		print("joinGroup:"+str(group_name))

		# 建立 标签-组名 映射关系
		self.joined_groups[tag] = group_name

		# 加入组，调用内部函数
		await self.channel_layer.group_add(
			group_name, self.channel_name
		)

	# 离开组（标签）
	async def leaveGroup(self, tag):

		if tag not in self.joined_groups: return

		# 离开组，调用内部函数
		await self.channel_layer.group_discard(
			self.joined_groups[tag], self.channel_name
		)

		# 将组从标签映射表中删除
		self.joined_groups.pop(tag)

	# 离开所有组
	async def clearGroups(self):

		tmp = self.joined_groups.copy()

		for tag in tmp: await self.leaveGroup(tag)

	# 广播操作
	# 组内广播（标签，数据，响应函数）
	async def groupSend(self, tag: ChannelLayerTag, data, method='response'):

		if tag not in self.joined_groups: return

		# 如果发送的数据有 tag 字段（枚举类型），转化为整数
		# if 'tag' in data: data['tag'] = data['tag'].value

		# 发送，调用内部函数
		await self.channel_layer.group_send(
			# type 为接收方采取响应的函数名，默认为 response 函数，即 Consumer 内的 response 函数
			self.joined_groups[tag], {'type': method, 'data': data}
		)

	# 多组广播
	async def groupsSend(self, tags, data, method='response'):

		for tag in tags:
			await self.groupSend(tag, data, method)

	"""
	def generateCode(self, type):

		max_ = pow(10, settings.CODE_LENGTH)

		code = str(random.randint(max_/10 ,max_-1))
		time = datetime.datetime.now()+datetime.timedelta(0,settings.CODE_SECOND,0)

		self.code = {'code': code, 'time': time, 'type': type}

		return code

	def checkCode(self, code, type):

		if self.code == None: return False

		now = datetime.datetime.now()

		if now > self.code['time']: return False

		return type == self.code['type'] and code == self.code['code']

	def clearCode(self):

		self.code = None
	"""

	# 接收操作
	# 接收客户端请求（内部调用）
	async def receive(self, text_data=None, bytes_data=None):

		from utils.interface_manager import WebSocket

		print("receive:" + str(text_data))

		# 解析 JSON 文本
		data = json.loads(text_data)

		print(data)

		# 处理接收的请求
		result, tag = await WebSocket.receiveRequest(self, data)

		if result: await self.groupSend(tag, result)

	# 发射操作
	# 发射数据（类型，组标签，数据）
	async def emit(self, type: EmitType, tag=ChannelLayerTag.Self, data=None):

		from utils.interface_manager import WebSocket

		if tag == ChannelLayerTag.AllLayer:
			await self.emitAll(type, data)

		else:
			result = WebSocket.processEmit(type, data, tag=tag)

			if result: await self.groupSend(tag, result)

	# 发射数据到多个组（类型，多组标签，数据）
	async def emits(self, type: EmitType, tags, data=None):

		from utils.interface_manager import WebSocket

		result = WebSocket.processEmit(type, data, tags=tags)

		if result:
			for tag in tags:
				# result['tag'] = tag
				await self.groupSend(tag, result)

	# 发射数据到所有组
	async def emitAll(self, type: EmitType, data=None):

		await self.emits(type, self.allGroupTags(), data)

	# 响应 response:
	# data: {'method': 'response', 'route': 路由信息, 'tag': 广播标签, 'status': 请求状态, 'data': 返回信息（JSON）（可选）, 'errmsg': 错误描述（可选）}
	# 主动发送 emit:
	# data: {'method': 'emit', 'type': 发射类型, 'tags': 广播标签（多个）, 'data': 返回信息（JSON）}
	async def response(self, data):

		await self.send(text_data=json.dumps(data["data"]))
