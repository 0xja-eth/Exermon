from django.conf import settings
from .code_manager import *
from .models import *
from .runtimes import OnlinePlayer
from utils.view_utils import Common as ViewUtils
from utils.runtime_manager import RuntimeManager
from utils.exception import WebscoketCloseCode

import hashlib, smtplib, datetime


# =======================
# 用户服务类，封装管理用户模块的业务处理函数
# =======================
class Service:

	# 业务函数
	# 玩家注册
	@classmethod
	async def register(cls, consumer, un: str, pw: str, email: str, code: str):
		# 返回数据：
		# id: int => 玩家的幸运数字

		Check.ensureUsernameFormat(un)
		Check.ensurePasswordFormat(pw)
		Check.ensureEmailFormat(email)

		CodeManager.ensureCode(un, email, code, 'register')

		pw = cls.cryptoPassword(pw)
		player = Player.register(consumer, un, pw, email)

		return {'id': player.id}

	# 玩家登陆
	@classmethod
	async def login(cls, consumer, un: str, pw: str):
		# 返回数据：
		# player: 玩家登陆数据 => 登陆后返回的玩家基本数据

		Check.ensureUsernameFormat(un)
		Check.ensurePasswordFormat(pw)

		pw = cls.cryptoPassword(pw)

		# 获取对应的 Player（程序会自动校验并报错）
		player = Common.getPlayer(error=ErrorType.IncorrectLogin, username=un, password=pw)

		Common.ensurePlayerNormalState(player)

		return await cls._doLogin(consumer, player)

	# 玩家忘记密码
	@classmethod
	async def forget(cls, consumer, un: str, pw: str, email: str, code: str):
		# 返回数据：无

		Check.ensureUsernameFormat(un)
		Check.ensurePasswordFormat(pw)
		Check.ensureEmailFormat(email)

		CodeManager.ensureCode(un, email, code, 'forget')

		# 获取对应的 Player（程序会自动校验并报错）
		player = Common.getPlayer(error=ErrorType.IncorrectForget, username=un, email=email)

		await cls._doForget(consumer, player, pw)

	# 发送验证码
	@classmethod
	async def sendCode(cls, consumer, un: str, email: str, type: str):
		# 返回数据：无

		conf = settings.CODE_TEXT[type]
		code = CodeManager.generateCode(un, email, type)

		if not (type == 'register' or type == 'forget'):
			raise ErrorException(ErrorType.ParameterError)

		print("sendCode to %s [%s]: %s" % (email, type, code))
		cls._doSendCode(un, email, code, conf)

	# 登出（客户端调用）（登出 ≠ 断开连接）
	@classmethod
	async def logout(cls, consumer, player: Player):
		# 返回数据：无

		# 获取对应的 Player（程序会自动校验并报错）
		# player = Common.getPlayer(id=pid)

		cls._doLogout(consumer, player)

	# 断开连接（内部调用，因 Consumer 断开而断开连接时调用，即调用时 Consumer 已断开）
	@classmethod
	async def disconnect(cls, consumer, player: Player, auth: str):
		# 返回数据：无
		ViewUtils.ensureAuth(auth)

		print('disconnect')

		# 获取另一个在线的玩家
		online_player: OnlinePlayer = Common.getOnlinePlayer(player.id)
		if online_player is None: return

		# 如果另一个玩家的 Consumer 是参数中的 Consumer，执行登出操作
		if online_player.consumer == consumer:
			cls._doLogout(consumer, online_player.player)

	# 辅助函数
	# 实际执行登陆操作
	@classmethod
	async def _doLogin(cls, consumer, player: Player):

		if player.isOnline():
			await cls.processAnyOnline(consumer, player, 'login')

		player.login(consumer)

		# 添加在线玩家
		Common.addOnlinePlayer(player, consumer)

		return {'player': player.convertToDict()}

	# 实际执行发送验证码操作
	@classmethod
	def _doSendCode(cls, un, email, code, conf):
		try:
			from django.core.mail import send_mail

			send_mail(conf[0], conf[1] % (un, code, CodeDatum.CODE_SECOND), conf[2], [email])

		except smtplib.SMTPException as exception:
			print("ERROR in sendEmailCode: " + str(exception))
			raise ErrorException(ErrorType.EmailSendError)

		except:
			raise ErrorException(ErrorType.UnknownError)

	# 实际执行重置密码的操作
	@classmethod
	async def _doForget(cls, consumer, player, pw):

		if player.isOnline():
			await cls.processAnyOnline(consumer, player, 'forget')

		player.resetPassword(consumer, pw)

	# 登出操作，实际上是对内存储存的玩家进行登出（登出 ≠ 断开连接）
	@classmethod
	def _doLogout(cls, consumer, player: Player):

		player.logout()

		# 删除在线玩家
		Common.deleteOnlinePlayer(player.id)

	# 检查并处理其他人在线的情况（比如登陆时已有其他玩家在线，更改密码时有其他玩家在线）
	@classmethod
	async def processAnyOnline(cls, consumer, player, type=''):

		# 获取另一个在线的玩家
		online_player: OnlinePlayer = Common.getOnlinePlayer(player.id)
		if online_player is None: return

		message = ''

		# 登陆时同时在线
		if type == 'login':
			message = Player.DUPLICATED_LOGIN_KICKOUT_MSG

		# 重置密码（忘记密码）时玩家在线
		if type == 'forget':
			message = Player.CHANGE_PASSWORD_KICKOUT_MSG

		await cls._doKickout(online_player, message)

	# 踢出玩家（被踢玩家ID，踢出信息）（踢出 ≠ 断开连接）
	@classmethod
	async def kickoutPlayer(cls, id, message):

		online_player: OnlinePlayer = Common.getOnlinePlayer(id)
		if online_player is None: return

		await cls._doKickout(online_player, message)

	# 实际执行踢出玩家的操作（发射信息告知另一方玩家）（踢出 ≠ 断开连接）
	@classmethod
	async def _doKickout(cls, online_player: OnlinePlayer, message):

		# from game_module.consumer import EmitType, ChannelLayerTag

		player = online_player.player
		consumer = online_player.consumer

		# data = {
		# 	'channel_name': consumer.channel_name,
		# 	'code': WebscoketCloseCode.Kickout.value,
		# 	'message': message
		# }

		cls._doLogout(consumer, player)

		await consumer.serverDisconnect(WebscoketCloseCode.Kickout, message, False)
		# await consumer.emit(EmitType.Disconnect, ChannelLayerTag.AllLayer, data)

	# 密码加密
	@classmethod
	def cryptoPassword(cls, value):
		value = Player.PASSWORD_SALT + value + Player.PASSWORD_SALT
		value = hashlib.sha1(value.encode()).hexdigest()
		return value

	# 选择玩家形象
	@classmethod
	async def createCharacter(cls, consumer, player: Player, grade: int, name: str, cid: int):
		# 返回数据：无

		Check.ensureNameFormat(name)
		Check.ensureGradeFormat(grade)
		Common.ensureCharacterExist(id=cid)

		player.create(name, grade, cid)

	# 选择艾瑟萌
	@classmethod
	async def createExermons(cls, consumer, player: Player, eids: list, enames: list):
		# 返回数据：
		# id: int => 玩家艾瑟萌槽ID
		from exermon_module.views import Common as ExermonCommon, Check as ExermonCheck

		ExermonCheck.ensureExermonCount(eids)
		ExermonCheck.ensureExermonCount(enames)

		for name in enames:
			ExermonCheck.ensureExermonNameFormat(name)

		exers = ExermonCommon.getExermons(eids)

		ExermonCommon.ensureExermonSubject(exers)
		ExermonCommon.ensureExermonType(exers)

		exer_slot = player.createExermons(exers, enames)

		return exer_slot.convertToDict()

	# 选择艾瑟萌天赋
	@classmethod
	async def createGifts(cls, consumer, player: Player, gids: list):
		# 返回数据：无
		from exermon_module.views import Common as ExermonCommon, Check as ExermonCheck

		ExermonCheck.ensureExermonCount(gids)

		gifts = ExermonCommon.getExerGifts(gids)

		ExermonCommon.ensureExerGiftType(gifts)

		player.createGifts(gifts)

	# 完善个人信息
	@classmethod
	async def createInfo(cls, consumer, player: Player, birth=None, school=None,
						 city=None, contact=None, description=None):
		# 返回数据：无
		from utils.interface_manager import Common as InterfaceCommon

		if birth:
			birth = InterfaceCommon.convertDataType(birth, 'date')
			Check.ensureBirthFormat(birth)

		if school: Check.ensureSchoolFormat(school)

		if city: Check.ensureCityFormat(city)

		if contact: Check.ensureContactFormat(contact)

		if description: Check.ensureDescriptionFormat(description)

		player.createInfo(birth, school, city, contact, description)

	# 获取玩家基本信息
	@classmethod
	async def getBasic(cls, consumer, player: Player, get_uid: int):
		# 返回数据：
		# player: 玩家基本数据 => 根据用户返回当前玩家基本数据/其他玩家基本数据
		if player.id == get_uid:
			type = "current"
			target_player = player
		else:
			type = "others"
			target_player = Common.getPlayer(id=get_uid)

		return {'player': target_player.convertToDict(type=type)}

	# 获取玩家状态界面信息
	@classmethod
	async def getStatus(cls, consumer, player: Player, get_uid: int, ):
		# 返回数据：
		# player: 玩家状态数据 => 玩家状态数据

		target_player = Common.getPlayer(id=get_uid)

		return {'player': target_player.convertToDict(type="status")}


	# 玩家修改昵称
	@classmethod
	async def editNickname(cls, consumer, player: Player, name: str, ):
		# 返回数据：
		pass

	# 玩家修改个人信息
	@classmethod
	async def editInfo(cls, consumer, player: Player, grade: int, birth=None, school=None,
						 city=None, contact=None, description=None):
		# 返回数据：无
		from utils.interface_manager import Common as InterfaceCommon

		Check.ensureGradeFormat(grade)

		if birth:
			birth = InterfaceCommon.convertDataType(birth, 'date')
			Check.ensureBirthFormat(birth)

		if school: Check.ensureSchoolFormat(school)

		if city: Check.ensureCityFormat(city)

		if contact: Check.ensureContactFormat(contact)

		if description: Check.ensureDescriptionFormat(description)

		player.editInfo(grade, birth, school, city, contact, description)

	# 人类装备槽装备
	@classmethod
	async def equipSlotEquip(cls, consumer, player: Player, eid: int, heid: int):
		# 返回数据：无
		from item_module.views import Service as ItemService

		HumanEquipType.ensure(id=eid)

		equip_slot = player.humanEquipSlot()

		pack_equip = Common.getPackEquip(id=heid)

		ItemService.slotContainerEquip(player, equip_slot, pack_equip, e_type_id=eid)


# =======================
# 用户校验类，封装用户业务数据格式校验的函数
# =======================
class Check:
	# 校验用户名格式
	@classmethod
	def ensureUsernameFormat(cls, val: str):
		ViewUtils.ensureRegexp(val, Player.USERNAME_REG, ErrorType.InvalidUsername)

	# 校验密码格式
	@classmethod
	def ensurePasswordFormat(cls, val: str):
		ViewUtils.ensureRegexp(val, Player.PASSWORD_REG, ErrorType.InvalidPassword)

	# 校验邮箱格式
	@classmethod
	def ensureEmailFormat(cls, val: str):
		ViewUtils.ensureRegexp(val, Player.EMAIL_REG, ErrorType.InvalidEmail)

	# 校验名字格式
	@classmethod
	def ensureNameFormat(cls, val: str):
		ViewUtils.ensureRegexp(val, Player.NAME_REG, ErrorType.InvalidName)

	# 校验年级格式
	@classmethod
	def ensureGradeFormat(cls, val: int):
		ViewUtils.ensureEnumData(val, PlayerGrades, ErrorType.InvalidGrade, True)

	# 校验年级格式
	@classmethod
	def ensureBirthFormat(cls, val: datetime.date):
		now = datetime.datetime.now()
		min_date = datetime.datetime(1900, 1, 1)
		if val < min_date or val > now:
			raise ErrorException(ErrorType.InvalidBirth)

	# 校验学校格式
	@classmethod
	def ensureSchoolFormat(cls, val: int):
		if len(val) > Player.SCHOOL_LEN:
			raise ErrorException(ErrorType.InvalidSchool)

	# 校验居住地格式
	@classmethod
	def ensureCityFormat(cls, val: int):
		if len(val) > Player.CITY_LEN:
			raise ErrorException(ErrorType.InvalidCity)

	# 校验联系方式格式
	@classmethod
	def ensureContactFormat(cls, val: int):
		if len(val) > Player.CONTACT_LEN:
			raise ErrorException(ErrorType.InvalidContact)

	# 校验个人介绍格式
	@classmethod
	def ensureDescriptionFormat(cls, val: int):
		if len(val) > Player.DESC_LEN:
			raise ErrorException(ErrorType.InvalidDescription)


# =======================
# 用户公用类，封装关于用户模块的公用函数
# =======================
class Common:

	# 获取玩家
	@classmethod
	def getPlayer(cls, return_type='object', error: ErrorType = ErrorType.PlayerNotExist, **kwargs) -> Player:

		if 'id' in kwargs and return_type == 'object':
			# 首先在在线玩家中查找
			online_player: OnlinePlayer = cls.getOnlinePlayer(kwargs['id'])
			if online_player: return online_player.player

		return ViewUtils.getObject(Player, error, return_type=return_type, **kwargs)

	# 获取形象
	@classmethod
	def getCharacter(cls, return_type='object', error: ErrorType = ErrorType.CharacterNotExist,
					 **kwargs) -> Character:
		return ViewUtils.getObject(Character, error, return_type=return_type, **kwargs)

	# 获取玩家持有装备
	@classmethod
	def getPackEquip(cls, return_type='object', error: ErrorType = ErrorType.ContItemNotExist,
					 **kwargs) -> HumanPackEquip:

		return ViewUtils.getObject(HumanPackEquip, error, return_type=return_type, **kwargs)

	# 确保玩家存在
	@classmethod
	def ensurePlayerExist(cls, error: ErrorType = ErrorType.PlayerNotExist, **kwargs):
		return ViewUtils.ensureObjectExist(Player, error, **kwargs)

	# 确保形象存在
	@classmethod
	def ensureCharacterExist(cls, error: ErrorType = ErrorType.CharacterNotExist, **kwargs):
		return ViewUtils.ensureObjectExist(Character, error, **kwargs)

	# 确保用户名存在（登陆可用）
	@classmethod
	def ensureUsernameExist(cls, un, pw=None):
		if pw is not None:
			pw = Service.cryptoPassword(pw)
			return cls.ensurePlayerExist(error=ErrorType.IncorrectLogin, username=un, password=pw)

		return cls.ensurePlayerExist(error=ErrorType.UsernameNotExist, username=un)

	# 确保玩家不存在
	@classmethod
	def ensurePlayerNotExist(cls, error: ErrorType=ErrorType.PlayerExist, **kwargs):
		return ViewUtils.ensureObjectNotExist(Player, error, **kwargs)

	# 确保用户名不存在
	@classmethod
	def ensureUsernameNotExist(cls, un):
		return cls.ensurePlayerNotExist(error=ErrorType.UsernameExist, username=un)

	# 确保玩家正常状态
	@classmethod
	def ensurePlayerNormalState(cls, player):
		if player.isBanned():
			raise ErrorException(ErrorType.UserAbnormal)
		if player.isFrozen():
			raise ErrorException(ErrorType.UserFrozen)

	# 确保密码正确
	@classmethod
	def ensurePasswordCorrect(cls, player, pw):
		pw = Service.cryptoPassword(pw)
		if player.password != pw:
			raise ErrorException(ErrorType.IncorrectPassword)

	# 添加在线玩家
	@classmethod
	def addOnlinePlayer(cls, player, consumer):
		RuntimeManager.add(OnlinePlayer, player.id, player=player, consumer=consumer)

	# 获取在线玩家
	@classmethod
	def getOnlinePlayer(cls, pid):
		return RuntimeManager.get(OnlinePlayer, pid)

	# 获取在线玩家
	@classmethod
	def deleteOnlinePlayer(cls, pid):
		return RuntimeManager.delete(OnlinePlayer, pid)

	# 获取所有在线玩家（返回：{pid: OnlinePlayer}）
	@classmethod
	def getOnlinePlayers(cls):
		return RuntimeManager.get(OnlinePlayer)

