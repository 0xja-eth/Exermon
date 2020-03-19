from utils.runtime_manager import RuntimeManager, RuntimeData
import datetime

"""
import threading, asyncio, datetime, traceback

# event loop
PROCESS_DELTA = 0.1
event_processors = []

def eventLoop(loop):

	async def process():

		while True:
			for method in event_processors:
				try: await method()
				except: traceback.print_exc()
			await asyncio.sleep(PROCESS_DELTA)

	future = process()
	loop.run_until_complete(future)

# 创建一个事件循环thread_loop
thread_loop = asyncio.new_event_loop()

# 将thread_loop作为参数传递给子线程
t = threading.Thread(target=eventLoop, args=(thread_loop,))
t.daemon = True
t.start()
"""


class AsyncOper(RuntimeData):
	"""
	异步操作项数据
	主要用于 Consumer 的一些异步/定时操作
	"""

	Index = 0

	# func: 异步的函数
	def __init__(self, func: callable, timespan: int = 0, *args, **kwargs):
		"""
		初始化
		Args:
			func (callable): 异步函数
			params (dict): 函数参数
			timespan (int): 延迟秒数
		"""
		self.func = func
		self.args = args
		self.kwargs = kwargs
		self.datetime = datetime.datetime.now() + \
			datetime.timedelta(0, timespan)
		self.index = AsyncOper.Index
		AsyncOper.Index += 1

	def getKey(self) -> str:
		"""
		生成该项对应的键
		Returns:
			返回操作名称
		"""
		return self.index

	async def action(self):

		await self.func(*self.args, **self.kwargs)

	@classmethod
	async def process(cls):
		"""
		处理所有的异步操作
		"""

		now = datetime.datetime.now()

		objs: dict = RuntimeManager.get(AsyncOper)

		tmp_objs = objs.copy()

		for key in tmp_objs:

			item: AsyncOper = tmp_objs[key]
			if now >= item.datetime:
				await item.action()
				RuntimeManager.delete(AsyncOper, key)


RuntimeManager.register(AsyncOper)
RuntimeManager.registerEvent(AsyncOper.process)
