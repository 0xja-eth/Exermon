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


# =====================
# 异步操作项（主要用于 Consumer 的一些异步操作，可能已弃用）
# =====================
class AsyncItem(RuntimeData):

	# func: 异步的函数
	def __init__(self, func, params={}, timespan=0):
		self.func = func
		self.params = params
		self.datetime = datetime.datetime.now() + \
			datetime.timedelta(0, timespan)

	async def action(self):

		await self.func(**self.params)

	@classmethod
	async def process(cls):

		now = datetime.datetime.now()

		objs: dict = RuntimeManager.get(AsyncItem)

		tmp_objs = objs.copy()

		for key in tmp_objs:

			item = tmp_objs[key]
			if now >= item.datetime:
				await item.action()
				RuntimeManager.delete(AsyncItem, key)


RuntimeManager.register(AsyncItem)
RuntimeManager.registerEvent(AsyncItem.process)
