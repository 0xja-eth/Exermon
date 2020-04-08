
from .views import Service
from utils.interface_manager import HTTP

Interfaces = {
	'upload': {
		# 接收POST数据（字段名，数据类型）
		'params': [['auth', 'str']],
		# 逻辑处理函数
		'func': Service.upload
	},
}

urlpatterns = HTTP.generateUrlPatterns(Interfaces)
