import datetime


# 测试类
class Common:

	Testing = True

	TestData = None

	# 开始测试
	@classmethod
	def start(cls, title):
		if not cls.Testing: return

		if cls.TestData is not None:
			return cls.catch(title)

		cls.TestData = {
			'title': title,
			'time': datetime.datetime.now(),
			'points': []
		}

	# 测试点捕捉
	@classmethod
	def catch(cls, name):
		if cls.TestData is None: return
		dt = datetime.datetime.now() - cls.TestData['time']
		print("%s: %.3f ms" % (name, dt.microseconds/1000))

		cls.TestData['points'].append((name, dt))
		cls.TestData['time'] = datetime.datetime.now()

	# 结束测试
	@classmethod
	def end(cls, name):
		if cls.TestData is None: return
		cls.catch(name)
		cls.display()
		cls.clear()

	# 显示测试结果
	@classmethod
	def display(cls):
		if cls.TestData is None: return
		sum_time = 0
		print("[Test] "+cls.TestData['title'])
		for point in cls.TestData['points']:
			sum_time += point[1].microseconds
			print("  %s: %.3f ms" % (point[0], point[1].microseconds/1000))
		print("[End] Sum: %.3f ms" % (sum_time/1000))

	# 清除测试
	@classmethod
	def clear(cls):
		cls.TestData = None
