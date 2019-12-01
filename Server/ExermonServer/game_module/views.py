from django.conf import settings

from .models import *
from player_module.models import Player
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

		data = cls._generateVersionData()

		if updated:
			data = cls._generateMainData(data)

		return data

	# 生成版本数据
	@classmethod
	def _generateVersionData(cls):

		cur_version = Common.getCurVersion()
		last_versions = Common.getLastVersions(cur_version, 'dict')

		return {
			'cur_version': cur_version.convertToDict(),
			'last_versions': last_versions
		}

	# 生成主体数据
	@classmethod
	def _generateMainData(cls, data={}):

		data = cls._generateGameTermsData(data)
		data = cls._generatePlayerStaticData(data)

		return data

	# 生成游戏核心静态数据
	@classmethod
	def _generateGameTermsData(cls, data={}):

		data['base_params'] = ViewUtils.getObjects(BaseParam, return_type='dict')

		return data

	# 生成玩家模块静态数据
	@classmethod
	def _generatePlayerStaticData(cls, data={}):

		data['player_genders'] = Player.PLAYER_GENDERS
		data['player_grades'] = Player.PLAYER_GRADES
		data['player_statuses'] = Player.PLAYER_STATUSES
		data['player_types'] = Player.PLAYER_TYPES

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
	def getCurVersion(cls, return_type='object') -> GameVersion:
		return ViewUtils.getObject(GameVersion, ErrorType.NoCurVersion, return_type=return_type, is_used=True)

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

