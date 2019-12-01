from django.db import models
from django.db.utils import ProgrammingError
from utils.model_utils import Common as ModelUtils
from utils.view_utils import Common as ViewUtils
from utils.exception import ErrorType

# Create your models here.


# ===================================================
# 基本属性表
# ===================================================
class BaseParam(models.Model):

	class Meta:

		verbose_name = verbose_name_plural = "基本属性"

	# 全局变量，BaseParam所有实例
	Params = None

	# 全局变量，BaseParam数
	Count = 0

	# 显示名称
	name = models.CharField(max_length=8, verbose_name="显示名称")

	# 程序属性名
	attr = models.CharField(max_length=8, verbose_name="程序属性名")

	# 描述
	description = models.CharField(max_length=64, verbose_name="描述")

	def convertToDict(self):

		return {
			'name': self.name,
			'attr': self.attr,
			'description': self.description
		}

	@classmethod
	def init(cls):

		try:
			cls.Params = ViewUtils.getObjects(cls)
			cls.Count = cls.Params.count()
		except ProgrammingError:
			print("Still no database")

	@classmethod
	def get(cls, index):

		from utils.view_utils import Common

		if cls.Params is None: cls.init()

		return ViewUtils.getObject(cls, ErrorType.UnknownError,
								objects=cls.Params, id=index)

	@classmethod
	def getAttr(cls, index):
		return cls.get(index).attr


# 初始化
BaseParam.init()


# ===================================================
# 游戏版本记录表
# ===================================================
class GameVersion(models.Model):

	class Meta:

		verbose_name = verbose_name_plural = "游戏版本"

	# 主版本号（检查更新）
	main_version = models.CharField(unique=True, null=False, blank=False, max_length=16, verbose_name="版本号")

	# 副版本号（建议更新）
	sub_version = models.CharField(unique=True, null=False, blank=False, max_length=16, verbose_name="版本号")

	# 更新时间
	update_time = models.DateTimeField(auto_now_add=True, verbose_name="更新时间")

	# 更新日志
	update_note = models.TextField(default='', blank=True, verbose_name="更新日志")

	# 附加描述
	description = models.CharField(default='', max_length=64, blank=True, verbose_name="附加描述")

	# 是否启用
	is_used = models.BooleanField(default=True, verbose_name="启用")

	def __str__(self):
		return self.main_version+'.'+self.sub_version

	# 新增版本
	@classmethod
	def add(cls, main, sub, note, desc):

		version = cls()
		version.main_version = main
		version.sub_version = sub
		version.update_note = note
		version.description = desc

		version.activate()

	# 激活本版本
	def activate(self):

		GameVersion.objects.all().update(is_used=False)

		self.is_used = True
		self.save()

	def convertToDict(self):

		update_time = ModelUtils.timeToStr(self.update_time)

		return {
			'main_version': self.main_version,
			'sub_version': self.sub_version,
			'update_time': update_time,
			'update_note': self.update_note,
			'description': self.description
		}


# ===================================================
# 游戏用语表
# ===================================================
class GameTerm(models.Model):

	class Meta:

		verbose_name = verbose_name_plural = "游戏用语"

	# 游戏名
	name = models.CharField(default='艾瑟萌学院', max_length=12, verbose_name="游戏名")

	# 游戏名（英文）
	eng_name = models.CharField(default='Exermon', max_length=24, verbose_name="游戏名（英文）")


