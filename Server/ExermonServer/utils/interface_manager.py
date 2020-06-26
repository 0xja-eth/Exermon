from django.conf import settings
from django.http import JsonResponse
from django.urls import path
from django.shortcuts import render
from utils.exception import ErrorType, GameException
import json, datetime, traceback

# ===============================
# 接口管理器：处理接口的检查、调用
# ===============================


# ===============================
# 处理 HTTP 接口（主要用于内部调用）
# ===============================
class HTTP:

	@classmethod
	def generateUrlPatterns(cls, interfaces):
		urlpatterns = []

		for key in interfaces:
			urlpatterns.append(path(key, cls._receiveRequest, interfaces[key]))

		return urlpatterns

	# 接收请求
	@classmethod
	def _receiveRequest(cls, request, func, method='POST', params=[], files=[], render_=False):

		res = dict()

		try:
			# 获取数据
			data = cls._getRequestDict(request, method=method, params=params, files=files)

			if render_:
				# 返回渲染的页面
				return render(request, func(**data))
			else:
				# 进行业务操作并返回结果到 res['data'] 中
				res['data'] = func(**data)

		except GameException as exception:
			# 打印错误路径
			traceback.print_exc()
			# 返回错误响应
			res = Common.getErrorResponseDict(exception)

		else:
			# 返回成功响应
			res = Common.getSuccessResponseDict(res)

		return cls._getJsonResponse(res)

	# 获取请求参数字典
	@classmethod
	def _getRequestDict(cls, request, method='POST', params=[], files=[]):

		data = dict()

		# 解析并判断请求方法
		if method.upper() != request.META['REQUEST_METHOD']:
			raise GameException(ErrorType.InvalidRequest)

		# 解析传入的 JSON 数据
		request_data = dict()
		if method == "GET": request_data = dict(request.GET)
		if method == "POST": request_data = dict(request.POST)

		for key in request_data:
			request_data[key] = request_data[key][0]

		# 具体处理函数
		data = Common.getRequestDict(request_data, params, data)
		data = cls._getRequestFileDict(request, files, data)

		print(data)

		return data

	# 获取请求文件字典
	@classmethod
	def _getRequestFileDict(cls, request, files=[], ori_data=None):

		data = ori_data or dict()

		for key in files:
			# 获取文件
			value = request.FILES.get(key)

			if value:
				data[key] = value
			else:
				raise GameException(ErrorType.ParameterError)

		return data

	# 获取成功响应对象
	@classmethod
	def _getJsonResponse(cls, dict=None):

		response = JsonResponse(dict)

		if settings.HTML_TEST: # 测试代码
			response["X-Frame-Options"] = ''

		return response


# ===============================
# 处理 WebSocket 接口
# ===============================
class WebSocket:

	from game_module.consumer import EmitType

	# 处理 Websocket 路由
	# data: {'route': 路由, 'data': 实际数据 }
	@classmethod
	async def receiveRequest(cls, consumer, data):

		from game_module.consumer import ChannelLayerTag

		res = dict()
		index = data['index']
		route = data['route']
		tag = ChannelLayerTag.Self

		try:
			# 保证请求格式正确
			cls._ensureRequestFormat(data)

			# 从配置的路由中获取调用数据
			params, method, tag = cls._getRouterData(route)

			# 处理请求数据
			data = Common.getRequestDict(data['data'], params)
			print('getRequestDict: ' + str(data))

			if 'uid' in data:
				player = consumer.getPlayer()
				if player is None or player.id != data['uid']:
					raise GameException(ErrorType.InvalidUserOper)

				data.pop('uid')
				data['player'] = player

			# 执行
			res['data'] = await method(consumer, **data)

		except GameException as exception:
			# 打印错误路径
			traceback.print_exc()

			# 返回错误响应
			res = Common.getErrorResponseDict(exception)

		else:
			# 返回成功响应
			res = Common.getSuccessResponseDict(res)

		# 封装其他数据
		res['method'] = 'response'
		res['route'] = route
		res['index'] = index
		res['tag'] = tag.value

		# 如果不需要返回数据
		if tag == ChannelLayerTag.NoLayer:
			print('No layer response:' + str(res))
			return None, tag

		print('response: ')
		try:
			print(res)
		except:
			traceback.print_stack()

		return res, tag

	# 保证请求格式正确
	@classmethod
	def _ensureRequestFormat(cls, data):
		if 'route' not in data or 'data' not in data:
			raise GameException(ErrorType.ParameterError)
		# if data['data'] == "None": data['data'] = {}

	# 获取路由配置数据
	@classmethod
	def _getRouterData(cls, route):

		from game_module.routing import WEBSOCKET_METHOD_ROUTER

		if route not in WEBSOCKET_METHOD_ROUTER:
			raise GameException(ErrorType.InvalidRoute)

		return WEBSOCKET_METHOD_ROUTER[route]

	# 处理数据发射
	@classmethod
	def processEmit(cls, type: EmitType, data=None, tag=None, tags=None):

		from game_module.consumer import ChannelLayerTag

		# 封装发射信息
		res = cls._getEmitDict(type, data, tag, tags)

		if tag == ChannelLayerTag.NoLayer:
			print("No layer response:" + str(res))
			return None

		print('Emit: '+str(res))

		return res

	# 封装发射数据字典
	@classmethod
	def _getEmitDict(cls, type: EmitType, data=None, tag=None, tags=None):

		# 处理 ChannelLayerTag (ChannelLayerTag 不可序列化，转换成 JSON 会出错）
		new_tags = []

		if tag is not None:
			new_tags.append(tag.value)

		if tags is not None:
			for t in tags:
				new_tags.append(t.value)

		return {
			'data': data,
			'type': type.value,
			'method': 'emit',
			'tags': new_tags
		}

	# 处理连接断开
	@classmethod
	async def processDisconnect(cls, consumer, close_code):

		player = consumer.getPlayer()

		if player is not None:
			data = cls._generateDisconnectRequestData(player)

			print("processDisconnect: "+str(player))

			await cls.receiveRequest(consumer, data)

		print("XConsumer: %s (%s) disconnected: %s" %
			  (consumer.channel_name, consumer.ip_address[0], close_code))

	@classmethod
	def _generateDisconnectRequestData(cls, player):

		from game_module.routing import DISCONNECT_ROUTE

		return {
			'route': DISCONNECT_ROUTE,
			'data': {'uid': player.id, 'auth': settings.AUTH_KEY},
			'index': -1
		}


class Common:

	# 处理请求的数据
	@classmethod
	def getRequestDict(cls, request_data, params=[], ori_data=None):

		data = ori_data or dict()

		# 校验每一个参数
		for item in params:

			key, type = item[0], item[1]

			if key in request_data:
				# 如果该接口所需参数在 request_data 中，进行数据类型转换
				data[key] = Common.convertDataType(request_data[key], type)
			elif type != 'var':
				# 否则且如果该参数不是可选的，抛出异常
				raise GameException(ErrorType.ParameterError)

		return data

	# 转换数据类型
	@classmethod
	def convertDataType(cls, value, type='str'):
		try:
			if type == 'int':
				value = int(value)

			elif type == 'int[]':

				if not isinstance(value, list):
					value = json.loads(value)

				for i in range(len(value)):
					value[i] = int(value[i])

			elif type == 'int[][]':

				if not isinstance(value, list):
					value = json.loads(value)

				for i in range(len(value)):
					for j in range(len(value[i])):
						value[i][j] = int(value[i][j])

			elif type == 'bool':
				value = bool(value)

			elif type == 'date':
				value = datetime.datetime.strptime(value, '%Y-%m-%d')

			elif type == 'datetime':
				value = datetime.datetime.strptime(value, '%Y-%m-%d %H:%M:%S')

			# 其他类型判断
			return value

		except:
			raise GameException(ErrorType.ParameterError)

	# 封装成功响应数据字典
	@classmethod
	def getSuccessResponseDict(cls, dict=None):

		dict = dict or {'data': None}

		dict['status'] = ErrorType.Success.value

		return dict

	# 封装错误响应数据字典
	@classmethod
	def getErrorResponseDict(cls, exception: GameException):
		return {
			'status': exception.error_type.value,
			'errmsg': str(exception)
		}