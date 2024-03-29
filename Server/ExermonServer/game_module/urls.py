
from .views import Service
from utils.interface_manager import HTTP

Interfaces = {
	'backend': {
		'method': "GET",
		# 接收GET数据（字段名，数据类型）
		'params': [['auth', 'str']],
		# 逻辑处理函数
		'func': Service.backend,
		'render_': True
	},
}

urlpatterns = HTTP.generateUrlPatterns(Interfaces)
