from django.db import models
from utils.model_utils import CharacterImageUpload, Common as ModelUtils
from enum import Enum

import datetime

# Create your models here.


# ===================================================
#  登陆信息表
# ===================================================
class LoginInfo(models.Model):
	class Meta:
		verbose_name = verbose_name_plural = "登录记录"

	# 登陆玩家
	player = models.ForeignKey('Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 登陆时间
	time = models.DateTimeField(auto_now_add=True, verbose_name="登陆时间")

	# 登出时间
	logout = models.DateTimeField(null=True, verbose_name="登出时间")

	# 登陆IP
	ip_address = models.GenericIPAddressField(verbose_name="IP地址")


# ===================================================
#  密码更改记录表
# ===================================================
class PasswordRecord(models.Model):
	class Meta:
		verbose_name = verbose_name_plural = "改密记录"

	# 登陆玩家
	player = models.ForeignKey('Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 发生时间
	time = models.DateTimeField(auto_now_add=True, verbose_name="时间")

	# 密码（ACCESS TOKEN）
	password = models.CharField(max_length=64, verbose_name="新密码")

	# 发生IP
	ip_address = models.GenericIPAddressField(verbose_name="IP地址")


# ===================================================
#  玩家性别枚举
# ===================================================
class PlayerGenders(Enum):
	Unset = 0  # 未设置
	Male = 1  # 男
	Female = 2  # 女
	Unknown = 3  # 不明


# ===================================================
#  人物表
# ===================================================
class Character(models.Model):

	class Meta:

		verbose_name = verbose_name_plural = "人物"

	PLAYER_GENDERS = [
		(PlayerGenders.Unset.value, '未设置'),
		(PlayerGenders.Male.value, '男'),
		(PlayerGenders.Female.value, '女'),
		(PlayerGenders.Unknown.value, '不明'),
	]

	# 名称
	name = models.CharField(max_length=12, verbose_name="名称")

	# 描述
	description = models.CharField(max_length=128, verbose_name="描述")

	# 性别
	gender = models.PositiveSmallIntegerField(default=0, choices=PLAYER_GENDERS, verbose_name="性别")

	# 半身像
	bust = models.ImageField(upload_to=CharacterImageUpload('bust'),
							 verbose_name="半身像")

	# 头像
	face = models.ImageField(upload_to=CharacterImageUpload('face'),
							 verbose_name="头像")

	# 战斗图
	battle = models.ImageField(upload_to=CharacterImageUpload('battle'),
							   verbose_name="战斗图")


# ===================================================
#  玩家年级枚举
# ===================================================
class PlayerGrades(Enum):
	Unset = 0  # 未设置
	Before = 1  # 初中生
	One = 2  # 高一
	Two = 3  # 高二
	Three = 4  # 高三
	Four = 5  # 高三复读
	After = 6  # 大学生及以后


# ===================================================
#  玩家状态枚举
# ===================================================
class PlayerStatus(Enum):
	OffLine = 0  # 离线
	OnLine = 1  # 在线
	Banned = 2  # 封禁
	Frozen = 3  # 冻结
	Other = -1  # 其他


# ===================================================
#  玩家类型枚举
# ===================================================
class PlayerType(Enum):
	Normal = 0  # 正常
	QQ = 1  # QQ
	Wechat = 2  # 微信
	Other = -1  # 其他


# ===================================================
#  玩家表
# ===================================================
class Player(models.Model):

	class Meta:

		verbose_name = verbose_name_plural = "玩家"

	# 常量声明
	PASSWORD_SALT = 'd28cb767c4272d8ab91000283c67747cb2ef7cd1'

	USERNAME_REG = r'^[a-z0-9A-Z_]{6,16}$'
	PASSWORD_REG = r'^[^\u4e00-\u9fa5]{8,24}$'
	PHONE_REG = r'^1[0-9]{10}$'
	EMAIL_REG = r'^([\w]+\.*)([\w]+)\@[\w]+\.\w{3}(\.\w{2}|)$'

	NAME_REG = r'^.{1,8}$'

	DEFAULT_MAX_PRESSURE = 150  # 默认压力最大值
	DEFAULT_PRESSURE_RATE = 6  # 默认压力衰减率（每小时衰减数）

	WEAPON_CORRECT_RATE = 75  # 武器库正确率
	MIN_COMP_WEAPON_COUNT = 18  # 最少对战武器题目数

	DUPLICATED_LOGIN_KICKOUT_MSG = '有用户在 %s 登陆（当前 ip: %s），' \
								   '您被强制下线！若非本人操作，您的密码可能已被泄露！请尽快修改密码！'
	CHANGE_PASSWORD_KICKOUT_MSG = '有用户在 %s 修改了该用户的密码（当前 ip: %s），' \
								  '您被强制下线！若非本人操作，您的邮箱或手机可能已被泄露！请尽快联系客服冻结账户并更改可信的邮箱和手机号！'

	SUCCESSFUL_LOGOUT_MSG = '您已成功退出登录！'

	PLAYER_GRADES = [
		(PlayerGrades.Unset.value, '未设置'),
		(PlayerGrades.Before.value, '初中及以下'),
		(PlayerGrades.One.value, '高一'),
		(PlayerGrades.Two.value, '高二'),
		(PlayerGrades.Three.value, '高三'),
		(PlayerGrades.Four.value, '高三复读'),
		(PlayerGrades.After.value, '大学及以上')
	]

	PLAYER_STATUSES = [
		(PlayerStatus.OffLine.value, '离线'),
		(PlayerStatus.OnLine.value, '在线'),
		(PlayerStatus.Banned.value, '封禁'),
		(PlayerStatus.Frozen.value, '冻结'),
		(PlayerStatus.Other.value, '其他')
	]

	PLAYER_TYPES = [
		(PlayerType.Normal.value, '标准用户'),
		(PlayerType.QQ.value, 'QQ用户'),
		(PlayerType.Wechat.value, '微信用户'),
		(PlayerType.Other.value, '其他')
	]

	# 用户名（OPENID）
	username = models.CharField(null=False, max_length=64, verbose_name="用户名")

	# 密码（ACCESS TOKEN）
	password = models.CharField(null=True, max_length=64, verbose_name="密码")

	# 手机号
	phone = models.CharField(blank=True, null=True, max_length=11, verbose_name="手机号")

	# 邮箱
	email = models.CharField(blank=True, null=True, max_length=128, verbose_name="邮箱")

	# 名字
	name = models.CharField(blank=True, null=True, max_length=12, verbose_name="昵称")

	# 人物
	character = models.ForeignKey('Character', on_delete=models.CASCADE,
								  blank=True, null=True, verbose_name="人物")

	# 年级
	grade = models.PositiveSmallIntegerField(default=0, choices=PLAYER_GRADES,
											 verbose_name="年级")

	# 注册时间
	create_time = models.DateTimeField(auto_now_add=True, verbose_name="注册时间")

	# 刷新时间
	last_refresh_time = models.DateTimeField(blank=True, null=True,
											 verbose_name="刷新时间")

	# 状态
	status = models.PositiveSmallIntegerField(default=0, choices=PLAYER_STATUSES, verbose_name="账号状态")

	# 账号类型
	type = models.PositiveSmallIntegerField(default=0, choices=PLAYER_TYPES, verbose_name="账号类型")

	# 删除标志
	is_deleted = models.BooleanField(default=False, verbose_name="删除标志")

	# 是否战斗掉线
	disconnect_battle = False

	# 当前 LoginInfo
	cur_login_info = None

	def __str__(self):
		return self.username

	def convertToDict(self, type=None):

		if type == "login":

			return {

			}

		return {

		}

	# 注册（类方法）
	@classmethod
	def register(cls, consumer, un, pw, email):

		player = Player()
		player.username = un
		player.email = email
		player.save()

		player.resetPassword(consumer, pw)

		return player

	# 是否是异常状态
	def isAbnormalState(self):
		return self.isBanned() or self.isFrozen()

	# 是否在线
	def isOnline(self):
		return self.status == PlayerStatus.OnLine.value

	# 是否在线
	def isOffline(self):
		return self.status != PlayerStatus.OnLine.value

	# 是否封禁
	def isBanned(self):
		return self.status == PlayerStatus.Banned.value

	# 是否冻结
	def isFrozen(self):
		return self.status == PlayerStatus.Frozen.value

	# 登陆
	def login(self, consumer):

		login = LoginInfo()
		login.player = self
		login.ip_address = consumer.ip_address
		login.save()

		self.status = PlayerStatus.OnLine.value
		self.save()

		self.cur_login_info = login

	# 登出
	def logout(self):

		if self.cur_login_info is None: return

		self.cur_login_info.logout = datetime.datetime.now()
		self.cur_login_info.save()

		self.status = PlayerStatus.OffLine.value
		self.save()

		self.cur_login_info = None

	# 重置密码
	def resetPassword(self, consumer, pw):

		record = PasswordRecord()
		record.player = self
		record.ip_address = consumer.ip_address
		record.password = pw
		record.save()

		self.password = pw
		self.save()

	# 创建角色
	def create(self, name, character, grade):
		self.name = name
		self.character = character
		self.grade = grade

		self._createContainers()

		self.save()

	# 创建角色相关的容器
	def _createContainers(self):
		from item_module.models import HumanPack
		from exermon_module.models import ExerHub, ExerFragPack, ExerGiftPool

		HumanPack._create(self)
		ExerHub._create(self)
		ExerFragPack._create(self)
		ExerGiftPool._create(self)
