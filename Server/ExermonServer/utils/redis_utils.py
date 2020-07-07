from django.conf import settings
import redis


class RedisUtils:
	"""
	Redis���湤����
	"""

	HOST = settings.CHANNEL_LAYERS['default']['CONFIG'][0]

	Pool = redis.ConnectionPool(host=HOST[0], port=HOST[1])

	_redis: redis.Redis = None

	@classmethod
	def redis(cls):
		"""
		��ȡһ��Redisʵ��
		Returns:
			����һ��Redisʵ��
		"""
		if cls._redis is None:
			cls._redis = redis.Redis(connection_pool=cls.Pool)
		return cls._redis

	@classmethod
	def release(cls):
		"""
		�ͷŵ�ǰ��Redisʵ��
		"""
		cls._redis = None
