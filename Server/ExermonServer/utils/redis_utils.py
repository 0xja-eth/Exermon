from django.conf import settings
import redis


class RedisUtils:
	"""
	Redis缓存工具类
	"""

	HOST = settings.CHANNEL_LAYERS['default']['CONFIG'][0]

	Pool = redis.ConnectionPool(host=HOST[0], port=HOST[1])

	_redis: redis.Redis = None

	@classmethod
	def redis(cls):
		"""
		获取一个Redis实例
		Returns:
			返回一个Redis实例
		"""
		if cls._redis is None:
			cls._redis = redis.Redis(connection_pool=cls.Pool)
		return cls._redis

	@classmethod
	def release(cls):
		"""
		释放当前的Redis实例
		"""
		cls._redis = None
