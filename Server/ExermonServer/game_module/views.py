from django.conf import settings

from .models import *
from utils.view_utils import Common as ViewUtils
from utils.exception import ErrorType, ErrorException


# =======================
# 服务类
# =======================
class Service:

	# 获取静态数据
	@classmethod
	async def getStaticData(cls, consumer, main_version: str, sub_version: str, cached: bool):
		# 返回数据：
		# data: 静态数据 => 系统的静态配置数据

		updated = not Check.ensureVersion(main_version, sub_version)

		return {'data': cls._generateStaticData(updated or not cached)}

	# 获取动态数据
	@classmethod
	async def getDynamicData(cls, consumer):
		# 返回数据：
		# data: 动态数据 => 系统动态数据

		pass

	# 生成静态数据
	@classmethod
	def _generateStaticData(cls, updated):

		cur_version = Common.getCurVersion()
		data = cls._generateVersionData(cur_version)

		if updated:
			data = cls._generateMainData(data)

		return data

	# 生成版本数据
	@classmethod
	def _generateVersionData(cls, cur_version):

		last_versions = Common.getLastVersions(cur_version, 'dict')

		return {
			'cur_version': cur_version.convertToDict(),
			'last_versions': last_versions,
		}

	# 生成资源数据
	@classmethod
	def _generateResourceData(cls):
		from exermon_module.models import Exermon, ExerFrag, ExerGift, ExerSkill, ExerItem, ExerEquip
		from player_module.models import Character, HumanItem, HumanEquip

		exermons = ModelUtils.objectsToDict(Exermon.objects.all())
		exer_frags = ModelUtils.objectsToDict(ExerFrag.objects.all())
		exer_skills = ModelUtils.objectsToDict(ExerSkill.objects.all())
		exer_gifts = ModelUtils.objectsToDict(ExerGift.objects.all())
		exer_items = ModelUtils.objectsToDict(ExerItem.objects.all())
		exer_equips = ModelUtils.objectsToDict(ExerEquip.objects.all())
		human_items = ModelUtils.objectsToDict(HumanItem.objects.all())
		human_equips = ModelUtils.objectsToDict(HumanEquip.objects.all())
		characters = ModelUtils.objectsToDict(Character.objects.all())

		return {
			'exermons': exermons,
			'exer_frags': exer_frags,
			'exer_skills': exer_skills,
			'exer_gifts': exer_gifts,
			'exer_items': exer_items,
			'exer_equips': exer_equips,
			'human_items': human_items,
			'human_equips': human_equips,
			'characters': characters
		}

	# 生成主体数据
	@classmethod
	def _generateMainData(cls, data, cur_version: GameVersion):
		configure = cur_version.configure

		data['configure'] = configure.convertToDict()
		data['data'] = cls._generateResourceData()

		return data


# =======================
# 校验类
# =======================
class Check:

	# 确保版本正确
	@classmethod
	def ensureVersion(cls, main, sub):

		# 确保版本存在
		Common.ensureVersion(main, sub)

		# 获取当前版本
		cur_version = Common.getCurVersion()

		if main != cur_version.main_version:

			raise ErrorException(ErrorType.RequestUpdate)

		return sub == cur_version.sub_version


# =======================
# 公用类，封装公用函数
# =======================
class Common:

	# 获取当前版本
	@classmethod
	def getCurVersion(cls) -> GameVersion:
		return GameVersion.get()

	# 获取当前版本
	@classmethod
	def getLastVersions(cls, version: GameVersion, return_type='QuerySet'):
		return ViewUtils.getObjects(GameVersion, return_type=return_type, update_time__lt=version.update_time)

	# 获取指定版本
	@classmethod
	def getVersion(cls, main, sub, return_type='object', error: ErrorType = ErrorType.ErrorVersion) -> GameVersion:
		return ViewUtils.getObject(GameVersion, error, return_type=return_type, main_version=main, sub_version=sub)

	# 获取主版本的所有副版本
	@classmethod
	def getSubVersions(cls, main, return_type='QuerySet'):
		return ViewUtils.getObjects(GameVersion, return_type=return_type, main_version=main)

	# 确保版本存在
	@classmethod
	def ensureVersion(cls, main, sub):
		ViewUtils.ensureObjectExist(GameVersion, ErrorType.ErrorVersion, main_version=main, sub_version=sub)
