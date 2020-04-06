from django.db import models
from django.conf import settings
from item_module.models import *
from utils.model_utils import CacheableModel, \
	CharacterImageUpload, Common as ModelUtils
from utils.exception import ErrorType, GameException
from game_module.models import ParamValue, HumanEquipType
from season_module.models import SeasonRecord
from season_module.runtimes import SeasonManager
from enum import Enum
import os, base64, datetime

# Create your models here.


# ===================================================
#  玩家性别枚举
# ===================================================
class CharacterGenders(Enum):
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

	CHARACTER_GENDERS = [
		(CharacterGenders.Unset.value, '未设置'),
		(CharacterGenders.Male.value, '男'),
		(CharacterGenders.Female.value, '女'),
		(CharacterGenders.Unknown.value, '不明'),
	]

	# 名称
	name = models.CharField(max_length=12, verbose_name="名称")

	# 描述
	description = models.CharField(max_length=128, blank=True, verbose_name="描述")

	# 性别
	gender = models.PositiveSmallIntegerField(default=0, choices=CHARACTER_GENDERS, verbose_name="性别")

	# 半身像
	bust = models.ImageField(upload_to=CharacterImageUpload('bust'), null=True, blank=True,
							 verbose_name="半身像")

	# 头像
	face = models.ImageField(upload_to=CharacterImageUpload('face'), null=True, blank=True,
							 verbose_name="头像")

	# 战斗图
	battle = models.ImageField(upload_to=CharacterImageUpload('battle'), null=True, blank=True,
							   verbose_name="战斗图")

	# 转化为字符串
	def __str__(self):
		return "%d. %s(%s)" % (self.id, self.name, self.genderText())

	# 性别文本
	def genderText(self):
		return self.CHARACTER_GENDERS[self.gender][1]

	# 获取完整路径
	def getExactlyPath(self):
		base = settings.STATIC_URL
		bust = os.path.join(base, str(self.bust))
		face = os.path.join(base, str(self.face))
		battle = os.path.join(base, str(self.battle))
		if os.path.exists(bust) and \
				os.path.exists(face) and \
				os.path.exists(battle):
			return bust, face, battle
		else:
			raise GameException(ErrorType.PictureFileNotFound)

	# 获取视频base64编码
	def convertToBase64(self):

		bust, face, battle = self.getExactlyPath()

		with open(bust, 'rb') as f:
			bust_data = base64.b64encode(f.read()).decode()

		with open(face, 'rb') as f:
			face_data = base64.b64encode(f.read()).decode()

		with open(battle, 'rb') as f:
			battle_data = base64.b64encode(f.read()).decode()

		return bust_data, face_data, battle_data

	def convertToDict(self, type=None):

		# bust_data, face_data, battle_data = self.convertToBase64()

		return {
			'id': self.id,
			'name': self.name,
			'description': self.description,
			'gender': self.gender,
			# 'bust': bust_data,
			# 'face': face_data,
			# 'battle': battle_data,
		}


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

	# 创建实例
	@classmethod
	def create(cls, player_id, addr):
		info = cls()
		info.player_id = player_id
		info.ip_address = addr

		return info


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

	# 已注册，未创建角色
	Uncreated = 1  # 未创建
	CharacterCreated = 2  # 已创建人物
	ExermonsCreated = 3  # 已选择艾瑟萌
	GiftsCreated = 4  # 已选择天赋

	# 已完全创建角色
	Normal = 10  # 正常
	Banned = 20  # 封禁
	Frozen = 30  # 冻结
	Other = 0  # 其他


# ===================================================
#  玩家类型枚举
# ===================================================
class PlayerType(Enum):
	Normal = 0  # 正常
	QQ = 1  # QQ
	Wechat = 2  # 微信
	Other = -1  # 其他


# ===================================================
#  持有金钱表
# ===================================================
class PlayerMoney(Currency):

	class Meta:
		verbose_name = verbose_name_plural = "玩家金钱"

	# 默认金币
	DEFAULT_GOLD = 500

	# 对应玩家
	player = models.OneToOneField("Player", on_delete=models.CASCADE,
									 null=True, verbose_name="玩家")

	# 创建
	@classmethod
	def create(cls, player):
		money = cls()
		money.player = player
		money.save()
		return money

	# 获得金钱
	def gain(self, gold=0, ticket=0, bound_ticket=0):
		self.gold += gold
		self.ticket += ticket
		self.bound_ticket += bound_ticket
		self.save()


# ===================================================
#  玩家表
# ===================================================
class Player(CacheableModel):

	class Meta:

		verbose_name = verbose_name_plural = "玩家"

	# 常量声明
	PASSWORD_SALT = 'd28cb767c4272d8ab91000283c67747cb2ef7cd1'

	USERNAME_REG = r'^[a-z0-9A-Z_]{6,16}$'
	PASSWORD_REG = r'^[^\u4e00-\u9fa5]{8,24}$'
	PHONE_REG = r'^1[0-9]{10}$'
	EMAIL_REG = r'^([\w]+\.*)([\w]+)\@[\w]+\.\w{3}(\.\w{2}|)$'

	NAME_REG = r'^.{1,8}$'

	SCHOOL_LEN = 24
	CITY_LEN = 24
	CONTACT_LEN = 24
	DESC_LEN = 128

	WEAPON_CORRECT_RATE = 75  # 武器库正确率
	MIN_COMP_WEAPON_COUNT = 18  # 最少对战武器题目数

	DUPLICATED_LOGIN_KICKOUT_MSG = '有用户在 %s 登陆（当前 ip: %s），' \
								   '您被强制下线！若非本人操作，您的密码可能已被泄露！请尽快修改密码！'
	CHANGE_PASSWORD_KICKOUT_MSG = '有用户在 %s 修改了该用户的密码（当前 ip: %s），' \
								  '您被强制下线！若非本人操作，您的邮箱或手机可能已被泄露！请尽快联系客服冻结账户并更改可信的邮箱和手机号！'

	SUCCESSFUL_LOGOUT_MSG = '您已成功退出登录！'

	GRADES = [
		(PlayerGrades.Unset.value, '不详'),
		(PlayerGrades.Before.value, '初中及以下'),
		(PlayerGrades.One.value, '高一'),
		(PlayerGrades.Two.value, '高二'),
		(PlayerGrades.Three.value, '高三'),
		(PlayerGrades.Four.value, '高三复读'),
		(PlayerGrades.After.value, '大学及以上')
	]

	STATUSES = [
		(PlayerStatus.Uncreated.value, '未创建'),
		(PlayerStatus.CharacterCreated.value, '已创建人物'),
		(PlayerStatus.ExermonsCreated.value, '已选择艾瑟萌'),
		(PlayerStatus.GiftsCreated.value, '已选择天赋'),

		(PlayerStatus.Normal.value, '正常'),
		(PlayerStatus.Banned.value, '封禁'),
		(PlayerStatus.Frozen.value, '冻结'),
		(PlayerStatus.Other.value, '其他')
	]

	TYPES = [
		(PlayerType.Normal.value, '标准用户'),
		(PlayerType.QQ.value, 'QQ用户'),
		(PlayerType.Wechat.value, '微信用户'),
		(PlayerType.Other.value, '其他')
	]

	# 登录信息缓存键
	LOGININFO_CACHE_KEY = 'login_info'

	# 当前题目集缓存键
	CUR_QUES_SET_CACHE_KEY = 'question_set'

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
	grade = models.PositiveSmallIntegerField(default=PlayerGrades.Unset.value,
											 choices=GRADES, verbose_name="年级")

	# 注册时间
	create_time = models.DateTimeField(auto_now_add=True, verbose_name="注册时间")

	# 刷新时间
	last_refresh_time = models.DateTimeField(blank=True, null=True,
											 verbose_name="刷新时间")

	# 状态
	status = models.PositiveSmallIntegerField(default=PlayerStatus.Uncreated.value,
											  choices=STATUSES, verbose_name="账号状态")

	# 账号类型
	type = models.PositiveSmallIntegerField(default=PlayerStatus.Normal.value,
											choices=TYPES, verbose_name="账号类型")

	# 在线
	online = models.BooleanField(default=False, verbose_name="在线")

	# 经验
	exp = models.PositiveIntegerField(default=0, verbose_name="经验值")

	# 生日
	birth = models.DateField(blank=True, null=True, verbose_name="生日")

	# 学校
	school = models.CharField(blank=True, null=True, max_length=24, verbose_name="学校")

	# 地区
	city = models.CharField(blank=True, null=True, max_length=24, verbose_name="地区")

	# 联系方式
	contact = models.CharField(blank=True, null=True, max_length=24, verbose_name="联系方式")

	# 个人介绍
	description = models.CharField(blank=True, null=True, max_length=128, verbose_name="个人介绍")

	# 信誉积分
	credit = models.PositiveSmallIntegerField(default=100, verbose_name="信誉积分")

	# 删除标志
	is_deleted = models.BooleanField(default=False, verbose_name="删除标志")

	# 当前 LoginInfo
	cur_login_info = None

	# 当前刷题
	cur_exercise = None

	def __str__(self):
		return "%d. %s(%s)" % (self.id, self.name, self.username)

	# 管理界面用
	def adminLevel(self):
		return self.level()

	adminLevel.short_description = "等级"

	# 管理界面用
	def adminMoney(self):
		money = self.playerMoney()
		if money: return str(money)
		return "无"

	adminMoney.short_description = "持有金钱"

	# region 字典生成

	def _packContainerIndices(self):

		human_pack = ModelUtils.objectToDict(self.humanPack())

		exer_pack = ModelUtils.objectToDict(self.exerPack())

		exer_frag_pack = ModelUtils.objectToDict(self.exerFragPack())

		exer_gift_pool = ModelUtils.objectToDict(self.exerGiftPool())

		exer_hub = ModelUtils.objectToDict(self.exerHub())

		ques_sugar_pack = ModelUtils.objectToDict(self.quesSugarPack())

		return {
			'human_pack': human_pack,
			'exer_pack': exer_pack,
			'exer_frag_pack': exer_frag_pack,
			'exer_gift_pool': exer_gift_pool,
			'exer_hub': exer_hub,
			'ques_sugar_pack': ques_sugar_pack,
		}

	def _slotContainerIndices(self):

		exer_slot = ModelUtils.objectToDict(self.exerSlot())

		human_equip_slot = ModelUtils.objectToDict(self.humanEquipSlot())

		battle_item_slot = ModelUtils.objectToDict(self.battleItemSlot())

		return {
			'exer_slot': exer_slot,
			'human_equip_slot': human_equip_slot,
			'battle_item_slot': battle_item_slot,
		}

	def _packContainerItems(self):

		human_pack = ModelUtils.objectToDict(self.humanPack(), type='items')

		exer_pack = ModelUtils.objectToDict(self.exerPack(), type='items')

		exer_frag_pack = ModelUtils.objectToDict(self.exerFragPack(), type='items')

		exer_gift_pool = ModelUtils.objectToDict(self.exerGiftPool(), type='items')

		exer_hub = ModelUtils.objectToDict(self.exerHub(), type='items')

		ques_sugar_pack = ModelUtils.objectToDict(self.quesSugarPack(), type='items')

		return {
			'human_pack': human_pack,
			'exer_pack': exer_pack,
			'exer_frag_pack': exer_frag_pack,
			'exer_gift_pool': exer_gift_pool,
			'exer_hub': exer_hub,
			'ques_sugar_pack': ques_sugar_pack,
		}

	def _slotContainerItems(self):

		exerslot = ModelUtils.objectToDict(self.exerSlot(), type='items')

		humanequipslot = ModelUtils.objectToDict(self.humanEquipSlot(), type='items')

		battle_item_slot = ModelUtils.objectToDict(self.battleItemSlot(), type='items')

		return {
			'exer_slot': exerslot,
			'human_equip_slot': humanequipslot,
			'battle_item_slot': battle_item_slot,
		}

	# 对战信息
	def _battleInfo(self):

		from battle_module.models import BattlePlayerResult

		season_record = self.currentSeasonRecord()
		battle_records = self.battlePlayerRecords()

		rank, sub_rank, _ = season_record.rank()

		count = win_cnt = corr_cnt = 0
		sum_hurt = sum_damage = sum_score = 0
		max_hurt = max_damage = max_score = 0

		for rec in battle_records:

			if rec.result == BattlePlayerResult.Win.value:
				win_cnt += 1

			hurt = rec.sumHurt()
			damage = rec.sumDamage()
			score = rec.sumScore()

			corr_cnt += rec.corrCount()

			sum_hurt += hurt
			sum_damage += damage
			sum_score += score

			max_hurt = max(max_hurt, hurt)
			max_damage = max(max_damage, damage)
			max_score = max(max_score, score)

			count += 1

		win_rate = win_cnt / count if count > 0 else 0
		corr_rate = corr_cnt / count if count > 0 else 0

		avg_hurt = sum_hurt / count if count > 0 else 0
		avg_damage = sum_damage / count if count > 0 else 0
		avg_score = sum_score / count if count > 0 else 0

		return {
			'rank_id': rank.id,
			'sub_rank': sub_rank,
			'star_num': season_record.star_num,
			'score': season_record.point,
			'credit': self.credit,
			'count': count,
			'win_rate': win_rate,
			'corr_rate': corr_rate,
			'avg_hurt': avg_hurt,
			'avg_damage': avg_damage,
			'avg_score': avg_score,
			'max_hurt': max_hurt,
			'max_damage': max_damage,
			'max_score': max_score,
		}

	# 做题信息
	def _questionInfo(self):

		question_records = self.questionRecords()

		cnt = count = corr_cnt = corr_rate = 0
		sum_timespan = avg_timespan = corr_timespan = 0
		sum_exp = sum_gold = 0

		for rec in question_records:
			cnt += 1
			count += rec.count
			corr_cnt += rec.correct
			sum_timespan += rec.count*rec.avg_time
			avg_timespan += rec.avg_time
			corr_timespan += rec.corr_time
			sum_exp += rec.sum_exp
			sum_gold += rec.sum_gold

		if count > 0:
			corr_rate = corr_cnt / count

		if cnt > 0:
			avg_timespan /= cnt
			corr_timespan /= cnt

		return {
			'count': count,
			'corr_cnt': corr_cnt,
			'corr_rate': corr_rate,
			'sum_timespan': sum_timespan,
			'avg_timespan': avg_timespan,
			'corr_timespan': corr_timespan,
			'sum_exp': sum_exp,
			'sum_gold': sum_gold,
		}

	def convertToDict(self, type: str = None, online_player = None) -> dict:
		"""
		转化为字典
		Args:
			type (str): 转化类型
			online_player (OnlinePlayer); 在线玩家信息
		Returns:
			转化后的字典数据
		"""
		create_time = ModelUtils.timeToStr(self.create_time)
		birth = ModelUtils.dateToStr(self.birth)
		money = ModelUtils.objectToDict(self.playerMoney())

		level, next = self.level(True)

		if type == "records":
			question_records = ModelUtils.objectsToDict(self.questionRecords())
			exercise_records = ModelUtils.objectsToDict(self.exerciseRecords())

			return {
				'question_records': question_records,
				'exercise_records': exercise_records
			}

		base = {
			'id': self.id,
			'name': self.name,
			'character_id': self.character_id,
			'level': level,
			'status': self.status,
			'type': self.type,
			'create_time': create_time,
		}

		if online_player is not None:
			base["channel_name"] = online_player.consumer.channel_name

		if type == "matched":
			season_record = self.currentSeasonRecord()

			rank, sub_rank, _ = season_record.rank()

			exer_slot = ModelUtils.objectToDict(self.exerSlot(), type="items")
			battle_item_slot = ModelUtils.objectToDict(self.battleItemSlot(), type='items')

			base["rank_id"] = rank.id
			base['sub_rank'] = sub_rank
			base['star_num'] = season_record.star_num
			base['exer_slot'] = exer_slot
			base['battle_item_slot'] = battle_item_slot

			return base

		base["pack_containers"] = self._packContainerIndices()
		base["slot_containers"] = self._slotContainerIndices()

		# 其他玩家
		if type == "others":
			base['online'] = self.online

		# 当前玩家
		if type == "current":
			base['username'] = self.username
			base['phone'] = self.phone
			base['email'] = self.email
			base['exp'] = self.exp
			base['next'] = next
			base['money'] = money

		# 当前玩家
		if type == "status":
			base['exp'] = self.exp
			base['next'] = next

			base['grade'] = self.grade
			base['birth'] = birth
			base['school'] = self.school
			base['city'] = self.city
			base['contact'] = self.contact
			base['description'] = self.description

			base['battle_info'] = self._battleInfo()
			base['question_info'] = self._questionInfo()

			base['pack_containers'] = self._packContainerItems()
			base['slot_containers'] = self._slotContainerItems()

		# 当前玩家
		if type == "battle":
			season_record = ModelUtils.objectToDict(self.currentSeasonRecord())

			base['exp'] = self.exp
			base['next'] = next

			base['season_record'] = season_record

			base['battle_info'] = self._battleInfo()

			base['pack_containers'] = self._packContainerItems()
			base['slot_containers'] = self._slotContainerItems()

		return base

	# endregion

	# region 容器管理

	# 获取相关容器
	def getContainer(self, cla):
		return self._getOneToOneCache(cla)

	# 获取人类背包
	def humanPack(self) -> 'HumanPack':
		return self._getOneToOneCache(HumanPack)

	# 获取艾瑟萌背包
	def exerPack(self) -> 'ExerPack':
		from exermon_module.models import ExerPack
		return self._getOneToOneCache(ExerPack)

	# 获取艾瑟萌碎片背包
	def exerFragPack(self) -> 'ExerFragPack':
		from exermon_module.models import ExerFragPack
		return self._getOneToOneCache(ExerFragPack)

	# 获取艾瑟萌天赋池
	def exerGiftPool(self) -> 'ExerGiftPool':
		from exermon_module.models import ExerGiftPool
		return self._getOneToOneCache(ExerGiftPool)

	# 获取艾瑟萌仓库
	def exerHub(self) -> 'ExerHub':
		from exermon_module.models import ExerHub
		return self._getOneToOneCache(ExerHub)

	# 获取题目糖背包
	def quesSugarPack(self) -> 'QuesSugarPack':
		from question_module.models import QuesSugarPack
		return self._getOneToOneCache(QuesSugarPack)

	# 获取艾瑟萌槽
	def exerSlot(self) -> 'ExerSlot':
		from exermon_module.models import ExerSlot
		return self._getOneToOneCache(ExerSlot)

	# 获取装备槽
	def humanEquipSlot(self) -> 'HumanEquipSlot':
		return self._getOneToOneCache(HumanEquipSlot)

	# 获取对战物资槽
	def battleItemSlot(self) -> 'BattleItemSlot':
		from battle_module.models import BattleItemSlot
		return self._getOneToOneCache(BattleItemSlot)

	# 获取金钱
	def playerMoney(self) -> PlayerMoney:
		return self._getOneToOneCache(PlayerMoney)

	# endregion

	# region 账号操作

	# 注册（类方法）
	@classmethod
	def register(cls, consumer, un, pw, email):

		player = Player()
		player.username = un
		player.email = email
		player.save()

		player.resetPassword(consumer, pw)

		return player

	# 登陆
	def login(self, consumer):
		"""
		登陆操作
		"""
		self._setLoginInfo(
			LoginInfo.create(self.id, consumer.ip_address)
		)
		self.online = True

		self.checkRelations()

		self.save()

	# 登出
	def logout(self):

		login_info = self._getLoginInfo()

		if login_info is None: return

		login_info.logout = datetime.datetime.now()

		self.online = False
		self.save()

		self._clearCache()

	def _setLoginInfo(self, login_info):
		self._cache(self.LOGININFO_CACHE_KEY, login_info)

	def _getLoginInfo(self):
		return self._getCache(self.LOGININFO_CACHE_KEY)

	# 重置密码
	def resetPassword(self, consumer, pw):

		record = PasswordRecord()
		record.player = self
		record.ip_address = consumer.ip_address
		record.password = pw
		record.save()

		self.password = pw
		self.save()

	# endregion

	# region 创建/修改操作

	# 创建角色
	def create(self, name, grade, cid):

		self.name = name
		self.grade = grade
		self.character_id = cid
		self.status = PlayerStatus.CharacterCreated.value

		self.save()

		self.checkRelations()

	# 创建艾瑟萌槽
	def createExermons(self, exers, enames):
		from exermon_module.models import ExerSlot, PlayerExermon

		player_exers = []

		for i in range(len(exers)):

			player_exers.append(self._createPlayerExer(exers[i], enames[i]))

		# PlayerExermon.objects.bulk_create(player_exers)

		if self.exerSlot(): self._deleteCache(ExerSlot)

		exer_slot = ExerSlot.create(player=self, player_exers=player_exers)

		# for player_exer in player_exers:
		# 	player_exer.save()

		self._cache(ExerSlot, exer_slot)

		self.status = PlayerStatus.ExermonsCreated.value
		self.save()

		return exer_slot

	# 创建艾瑟萌天赋
	def createGifts(self, gifts):
		from exermon_module.models import PlayerExerGift

		exerslot = self.exerSlot()

		if exerslot is None:
			self.status = PlayerStatus.CharacterCreated
			raise GameException(ErrorType.ExerSlotNotExist)

		for i in range(len(gifts)):

			player_gift = self._createPlayerGift(gifts[i])
			exerslot.setPlayerGift(player_gift=player_gift, slot_index=i+1)

		self.status = PlayerStatus.GiftsCreated.value
		self.save()

	# 补全人物信息
	def createInfo(self, birth, school, city, contact, description):

		self.birth = birth
		self.school = school
		self.city = city
		self.contact = contact
		self.description = description

		self.status = PlayerStatus.Normal.value
		self.save()

	def _createPlayerExer(self, exer, name):
		from exermon_module.models import PlayerExermon, ExerHub

		exer_hub: ExerHub = self.exerHub()

		player_exer: PlayerExermon = exer_hub.gainItems(exer)[0]
		player_exer.nickname = name

		return player_exer

	def _createPlayerGift(self, gift):
		from exermon_module.models import PlayerExerGift, ExerGiftPool

		gift_pool: ExerGiftPool = self.exerGiftPool()

		player_gift: PlayerExerGift = gift_pool.gainItems(gift)[0]
		player_gift.save()

		return player_gift

	# 修改人物名称
	def editName(self, name):

		self.name = name

	# 修改人物信息
	def editInfo(self, grade, birth, school, city, contact, description):

		self.grade = grade
		self.birth = birth
		self.school = school
		self.city = city
		self.contact = contact
		self.description = description

	# endregion

	# region 状态判断

	# 是否是异常状态
	def isAbnormalState(self):
		return self.isBanned() or self.isFrozen()

	# 是否在线
	def isCreated(self):
		return self.status >= PlayerStatus.OffLine.value

	# 是否在线
	def isOnline(self):
		return self.online

	# 是否离线
	def isOffline(self):
		return not self.online

	# 是否封禁
	def isBanned(self):
		return self.status == PlayerStatus.Banned.value

	# 是否冻结
	def isFrozen(self):
		return self.status == PlayerStatus.Frozen.value

	# endregion

	# region 检查关联数据

	def checkRelations(self):
		"""
		检查关联数据（缺少数据时自动添加）
		"""
		# 如果是已经创建了角色的玩家，对容器进行检查并创建
		if self.status >= PlayerStatus.CharacterCreated.value:
			self._checkContainers()
			self._checkSeasonRecord()

	# 创建角色相关的容器
	def _checkContainers(self):
		"""
		检查相关容器
		"""
		from exermon_module.models import ExerHub, ExerFragPack, ExerGiftPool, ExerPack
		from question_module.models import QuesSugarPack
		from battle_module.models import BattleItemSlot

		if self.humanPack() is None:
			HumanPack.create(player=self)

		if self.exerPack() is None:
			ExerPack.create(player=self)

		if self.exerHub() is None:
			ExerHub.create(player=self)

		if self.exerFragPack() is None:
			ExerFragPack.create(player=self)

		if self.exerGiftPool() is None:
			ExerGiftPool.create(player=self)

		if self.humanEquipSlot() is None:
			HumanEquipSlot.create(player=self)

		if self.quesSugarPack() is None:
			QuesSugarPack.create(player=self)

		if self.battleItemSlot() is None:
			BattleItemSlot.create(player=self)

		if self.playerMoney() is None:
			PlayerMoney.create(self)

	def _checkSeasonRecord(self):
		"""
		检查赛季记录
		"""
		cur_rec = self.currentSeasonRecord()
		if cur_rec is None:
			season = SeasonManager.getCurrentSeason()
			SeasonRecord.create(self, season)

	# endregion

	# region 基本属性操作

	def level(self, calc_next: bool = False) -> int or (int, int):
		"""
		获取玩家等级和下一级所需经验
		Args:
			calc_next (bool): 是否需要计算下一级经验
		Returns:
			如果 calc_next 为 True，返回等级以及下一级经验，否则仅返回当前等级
		"""
		from utils.calc_utils import ExermonSlotLevelCalc

		level = ExermonSlotLevelCalc.calcPlayerLevel(self.exp)

		if not calc_next: return level

		next = ExermonSlotLevelCalc.calcPlayerExp(level+1)
		return level, next

	def battlePoint(self) -> int:
		"""
		获取战斗力
		Returns:
			返回战斗力点数
		"""
		return self.exerSlot().battlePoint()

	def gainMoney(self, gold=0, ticket=0, bound_ticket=0):
		"""
		获得金钱
		Args:
			gold (int): 金币
			ticket (int): 点券
			bound_ticket (int): 绑定点券
		"""
		money = self.playerMoney()
		if money is None: return

		money.gain(gold, ticket, bound_ticket)

	def gainExp(self, sum_exp: int, slot_exps: dict, exer_exps: dict):
		"""
		获得经验
		Args:
			sum_exp (int): 人类经验
			slot_exps (dict): 槽经验
			exer_exps (dict): 艾瑟萌经验
		"""
		exerslot = self.exerSlot()
		if exerslot is None: return

		self.exp += sum_exp
		exerslot.gainExp(slot_exps, exer_exps)

	def subjects(self) -> set:
		"""
		获取玩家所选科目
		Returns:
			玩家所选科目数组
		"""
		exerslot = self.exerSlot()
		if exerslot is None: return set()
		slot_items = exerslot.contItems()
		return set(ModelUtils.query(slot_items, lambda item: item.subject))

	# endregion

	# region 纪录操作

	def questionRecords(self) -> QuerySet:
		"""
		获取所有题目记录
		Returns:
			所有与该玩家相关的题目记录 QuerySet 对象
		"""
		return self.questionrecord_set.all()

	def questionRecord(self, question_id: int) -> 'QuestionRecord':
		"""
		通过题目ID查找题目记录
		Args:
			question_id (int): 题目ID
		Returns:
			若存在题目记录，返回之，否则返回 None
		"""
		res = self.questionRecords().filter(question_id=question_id)
		if res.exists(): return res.first()
		return None

	def exerciseRecords(self) -> QuerySet:
		"""
		获取所有刷题记录
		Returns:
			所有与该玩家相关的刷题记录 QuerySet 对象
		"""
		return self.exerciserecord_set.all()

	def setCurrentQuestionSet(self, ques_set: 'QuestionSetRecord'):
		"""
		设置当前题目集
		Args:
			ques_set (QuestionSetRecord): 题目集
		"""
		self._cache(self.CUR_QUES_SET_CACHE_KEY, ques_set)

	def currentQuestionSet(self) -> 'QuestionRecord':
		"""
		获取当前题目集记录
		Returns:
			返回当前题目集记录
		"""
		return self._getCache(self.CUR_QUES_SET_CACHE_KEY)

	def clearCurrentQuestionSet(self):
		"""
		清除当前题目集记录
		"""
		self._clearCache(self.CUR_QUES_SET_CACHE_KEY)

	# endregion

	# region 赛季操作

	def seasonRecords(self) -> QuerySet:
		"""
		获取所有赛季记录
		Returns:
			返回所有赛季记录（SeasonRecord） QuerySet 对象
		"""
		return self.seasonrecord_set.all()

	def seasonRecord(self, **kwargs) -> SeasonRecord:
		"""
		获取指定条件的赛季记录
		Args:
			**kwargs (**dict): 查询条件
		Returns:
			返回指定条件的赛季记录
		"""
		rec = self.seasonRecords().filter(**kwargs)
		if rec.exists(): return rec.first()
		return None

	def currentSeasonRecord(self) -> SeasonRecord:
		"""
		获取当前赛季记录
		Returns:
			返回玩家当前赛季记录
		"""
		season = SeasonManager.getCurrentSeason()
		return self.seasonRecord(season_id=season.id)

	# endregion

	# region 对战操作

	def battleRecords(self) -> list:
		"""
		获取对战记录
		Returns:
			返回对战记录列表
		"""
		return ModelUtils.getObjectRelatedForAll(
			self.battlePlayerRecords(), 'record')

	def battlePlayerRecords(self) -> QuerySet:
		"""
		获取对战玩家记录（对战结果）
		Returns:
			返回对战玩家记录（BattlePlayer） QuerySet 对象
		"""
		return self.battleplayer_set.all()

	# endregion

	""" 占位符 """


# ===================================================
#  人类物品使用效果表
# ===================================================
class HumanItemEffect(BaseEffect):

	class Meta:
		verbose_name = verbose_name_plural = "人类物品使用效果"

	# 物品
	item = models.ForeignKey('HumanItem', on_delete=models.CASCADE,
							 verbose_name="物品")


# ===================================================
#  人类物品价格
# ===================================================
class HumanItemPrice(Currency):

	class Meta:
		verbose_name = verbose_name_plural = "人类物品价格"

	# 物品
	item = models.OneToOneField('HumanItem', on_delete=models.CASCADE,
							 verbose_name="物品")


# ===================================================
#  人类物品表
# ===================================================
class HumanItem(UsableItem):

	class Meta:
		verbose_name = verbose_name_plural = "人类物品"

	# 道具类型
	TYPE = ItemType.HumanItem

	# 对应的容器项类
	@classmethod
	def contItemClass(cls): return HumanPackItem

	# 转化为 dict
	def convertToDict(self, **kwargs):
		res = super().convertToDict(**kwargs)

		return res

	# 获取所有的效果
	def effects(self):
		return self.humanitemeffect_set.all()

	# 购买价格
	def buyPrice(self):
		try: return self.humanitemprice
		except HumanItemPrice.DoesNotExist: return None


# ===================================================
#  人类装备属性值表
# ===================================================
class HumanEquipParam(ParamValue):

	class Meta:
		verbose_name = verbose_name_plural = "人类装备属性值"

	# 装备
	equip = models.ForeignKey("HumanEquip", on_delete=models.CASCADE, verbose_name="装备")

	# 最大值
	def maxVal(self):
		return None

	# 最小值
	def minVal(self):
		return None


# ===================================================
#  人类装备价格
# ===================================================
class HumanEquipPrice(Currency):

	class Meta:
		verbose_name = verbose_name_plural = "人类装备价格"

	# 物品
	item = models.OneToOneField('HumanEquip', on_delete=models.CASCADE,
							 verbose_name="物品")


# ===================================================
#  人类装备
# ===================================================
class HumanEquip(EquipableItem):
	class Meta:
		verbose_name = verbose_name_plural = "人类装备"

	# 道具类型
	TYPE = ItemType.HumanEquip

	# 装备类型
	e_type = models.ForeignKey("game_module.HumanEquipType",
							   on_delete=models.CASCADE, verbose_name="装备类型")

	# 对应的容器项类
	@classmethod
	def contItemClass(cls): return HumanPackEquip

	# 转化为 dict
	def convertToDict(self, **kwargs):
		res = super().convertToDict(**kwargs)

		res['e_type'] = self.e_type_id

		return res

	# 获取所有的属性基本值
	def params(self):
		return self.humanequipparam_set.all()

	# 购买价格
	def buyPrice(self):
		try: return self.humanequipprice
		except HumanEquipPrice.DoesNotExist: return None


# ===================================================
#  人类背包
# ===================================================
class HumanPack(PackContainer):

	class Meta:
		verbose_name = verbose_name_plural = "人类背包"

	# 容器类型
	TYPE = ContainerType.HumanPack

	# 默认容量
	DEFAULT_CAPACITY = 64

	# 玩家
	player = models.OneToOneField('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# 所接受的容器项类
	@classmethod
	def acceptedContItemClasses(cls): return HumanPackItem, HumanPackEquip

	# 获取容器容量（0为无限）
	@classmethod
	def defaultCapacity(cls): return cls.DEFAULT_CAPACITY

	# 创建一个背包（创建角色时候执行）
	def _create(self, player):
		super()._create()
		self.player = player

	# 持有者
	def owner(self): return self.player


# ===================================================
#  人类背包物品
# ===================================================
class HumanPackItem(PackContItem):
	class Meta:
		verbose_name = verbose_name_plural = "人类背包物品"

	# 容器项类型
	TYPE = ContItemType.HumanPackItem

	# 容器
	container = models.ForeignKey('HumanPack', on_delete=models.CASCADE,
							   null=True, verbose_name="容器")

	# 物品
	item = models.ForeignKey('HumanItem', on_delete=models.CASCADE,
							 null=True, verbose_name="物品")

	# 所属容器的类
	@classmethod
	def containerClass(cls): return HumanPack

	# 所接受的物品类
	@classmethod
	def acceptedItemClass(cls): return HumanItem

	def isContItemUsable(self) -> bool:
		"""
		配置当前物品是否可用
		Returns:
			返回当前物品是否可用
		"""
		return self.item.battle_use


# ===================================================
#  人类背包装备
# ===================================================
class HumanPackEquip(PackContItem):
	class Meta:
		verbose_name = verbose_name_plural = "人类背包装备"

	# 容器项类型
	TYPE = ContItemType.HumanPackEquip

	# 容器
	container = models.ForeignKey('HumanPack', on_delete=models.CASCADE,
							   null=True, verbose_name="容器")

	# 物品
	item = models.ForeignKey('HumanEquip', on_delete=models.CASCADE,
							 null=True, verbose_name="物品")

	# 所属容器的类
	@classmethod
	def containerClass(cls): return HumanPack

	# 所接受的物品类
	@classmethod
	def acceptedItemClass(cls): return HumanEquip


# ===================================================
#  人类装备槽
# ===================================================
class HumanEquipSlot(SlotContainer):

	class Meta:
		verbose_name = verbose_name_plural = "人类装备槽"

	# 容器类型
	TYPE = ContainerType.HumanEquipSlot

	# 玩家
	player = models.OneToOneField('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

	# # 人类背包
	# human_pack = models.ForeignKey('HumanPack', on_delete=models.CASCADE, verbose_name="人类背包")

	# 所接受的槽项类
	@classmethod
	def acceptedSlotItemClass(cls): return HumanEquipSlotItem

	# 所接受的装备项基类（由于重载了 contItemClass，该函数意义有改变）
	@classmethod
	def baseContItemClass(cls): return HumanPackEquip

	# 默认容器容量（0为无限）
	@classmethod
	def defaultCapacity(cls): return HumanEquipType.count()

	def _create(self, player: Player):
		"""
		创建对战物资容器（创建角色时候执行）
		Args:
			player (Player): 玩家
		"""
		super()._create()
		self.player = player

	def _equipContainer(self, index: int) -> HumanPack:
		"""
		获取指定装备ID的所属容器
		Args:
			index (int): 装备ID
		Returns:
			指定装备ID的所属容器项
		"""
		return self.exactlyPlayer().humanPack()

	# 保证装备类型与槽一致
	def ensureEquipType(self, slot_item, equip):
		if slot_item.e_type_id != equip.item.e_type_id:
			raise GameException(ErrorType.IncorrectEquipType)

		return True

	# 保证满足装备条件
	def ensureEquipCondition(self, slot_item, equip_item):
		super().ensureEquipCondition(slot_item, equip_item)

		self.ensureEquipType(slot_item, equip_item)

		return True

	# 设置艾瑟萌装备
	def setPackEquip(self, pack_equip: HumanPackEquip = None, e_type_id=None, force=False):

		if pack_equip is not None:
			e_type_id = pack_equip.item.e_type_id

		self.setEquip(equip_index=0, equip_item=pack_equip, e_type_id=e_type_id, force=force)

	def owner(self) -> Player:
		"""
		获取容器的持有者
		Returns:
			持有玩家
		"""
		return self.player


# ===================================================
#  人类装备槽项
# ===================================================
class HumanEquipSlotItem(SlotContItem):

	class Meta:
		verbose_name = verbose_name_plural = "人类装备槽项"

	# 容器项类型
	TYPE = ContItemType.HumanEquipSlotItem

	# 容器
	container = models.ForeignKey('HumanEquipSlot', on_delete=models.CASCADE,
							   null=True, verbose_name="容器")

	# 装备项
	pack_equip = models.OneToOneField('HumanPackEquip', null=True, blank=True,
									  on_delete=models.SET_NULL, verbose_name="装备")

	# 装备槽类型
	e_type = models.ForeignKey('game_module.HumanEquipType', on_delete=models.CASCADE,
							   verbose_name="装备槽类型")

	# 所属容器的类
	@classmethod
	def containerClass(cls): return HumanEquipSlot

	# 所接受的装备项类（可多个）
	@classmethod
	def acceptedEquipItemClass(cls): return (HumanPackEquip, )

	# 所接受的装备项属性名（可多个）
	@classmethod
	def acceptedEquipItemAttr(cls): return ('pack_equip', )

	# 转化为 dict
	def convertToDict(self, **kwargs):
		res = super().convertToDict(**kwargs)

		res['e_type'] = self.e_type_id

		return res

	# def _equipItem(self, index):
	# 	if index == 0: return self.pack_equip

	# 配置索引
	def setupIndex(self, index, **kwargs):
		super().setupIndex(index, **kwargs)
		self.e_type_id = index

	# 获取属性值
	def param(self, param_id=None, attr=None):
		return self.pack_equip.param(param_id, attr)

	# # 装备
	# def equip(self, pack_equip=None, **kwargs):
	# 	self.pack_equip: HumanPackEquip = pack_equip
	# 	self.pack_equip.transfer(self.container)
	#
	# # 卸下
	# def dequip(self, **kwargs):
	# 	self.pack_equip.remove()
	# 	pack_equip = self.pack_equip
	# 	self.pack_equip = None
	# 	return pack_equip
