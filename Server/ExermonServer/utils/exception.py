from enum import Enum


class WebscoketCloseCode(Enum):

	ServerClose			= 1000 # 服务器切断连接
	ClientClose			= 1001 # 客户端切断连接
	AbnormalClose		= 1006 # 非正常断开连接

	Logout				= 3000 # 用户登出
	Kickout				= 3001 # 用户踢出


class ErrorType(Enum):
	# Common
	UnknownError = -1  # 未知错误
	Success = 0  # 成功，无错误
	InvalidRequest = 1  # 非法的请求方法
	ParameterError = 2  # 参数错误
	InvalidRoute = 3  # 非法路由
	PermissionDenied = 4  # 无权操作
	NoCurVersion = 5  # 未设置当前版本
	NoCurConfigure = 6  # 当前版本无配置
	RequestUpdate = 7  # 需要更新
	ErrorVersion = 8  # 错误的游戏版本
	InvalidUserOper = 10  # 无效的用户操作
	SubjectNotExist = 20  # 科目不存在
	BaseParamNotExist = 21  # 属性不存在
	DatabaseError = 30  # 数据库错误

	# PlayerCommon
	PlayerNotExist = 100  # 玩家不存在
	PlayerExist = 101  # 玩家已存在
	IncorrectPassword = 102  # 密码错误
	IncorrectCode = 103  # 验证码错误
	EmailSendError = 104  # 邮件发送错误

	# PlayerRegister
	UsernameExist = 110  # 用户名已存在
	InvalidUsername = 111  # 非法的用户名
	InvalidPassword = 112  # 非法的密码
	PhoneExist = 113  # 电话号码已存在
	InvalidPhone = 114  # 非法的电话号码
	EmailExist = 115  # 邮箱地址已存在
	InvalidEmail = 116  # 非法的邮箱地址

	# PlayerLogin
	IncorrectLogin = 120  # 不正确的登陆
	UserAbnormal = 121  # 用户状态异常
	UserFrozen = 122  # 用户被冻结

	# PlayerForget
	IncorrectForget = 130  # 不正确的找回密码

	# PlayerInfo
	NameExist = 140  # 该名字已存在
	InvalidName = 141  # 非法的昵称
	InvalidGender = 142  # 非法的性别
	InvalidGrade = 143  # 非法的年级
	InvalidBirth = 144  # 非法的出生日期
	InvalidSchool = 145  # 非法的学校名称
	InvalidCity = 146  # 非法的居住地
	InvalidContact = 147  # 非法的联系方式
	InvalidDescription = 148  # 非法的个人介绍

	# CharacterCommon
	CharacterNotExist = 150  # 形象不存在

	# ItemCommon
	ItemNotExist = 200  # 物品不存在
	ContainerNotExist = 201  # 容器不存在
	ContItemNotExist = 202  # 容器项不存在
	ContainerNotOwner = 203  # 该容器不属于当前玩家
	ContItemNotHold = 204  # 容器中不存在该容器项
	IncorrectItemType = 210  # 物品类型不正确
	IncorrectContainerType = 211  # 容器类型不正确
	IncorrectContItemType = 212  # 容器项类型不正确

	# Gain/LostItem
	CapacityInsufficient = 220  # 容器剩余容量不足
	QuantityInsufficient = 221  # 物品数量不足
	InvalidContItem = 222  # 无效容器项

	# Equip/DequipEquip
	IncorrectEquipType = 230  # 装备类型不正确
	InvalidContainer = 231  # 无效容器

	# ExermonCommon
	ExermonNotExist = 300  # 艾瑟萌不存在
	ExerGiftNotExist = 301  # 艾瑟萌天赋不存在
	ExerSlotNotExist = 302  # 艾瑟萌槽不存在
	PlayerExermonNotExist = 303  # 玩家艾瑟萌不存在
	PlayerExerGiftNotExist = 304  # 玩家艾瑟萌天赋不存在

	InvalidExermonCount = 310  # 非法的艾瑟萌数量
	InvalidExermonName = 311  # 非法的艾瑟萌昵称
	InvalidExermonSubject = 312  # 非法的艾瑟萌科目
	InvalidExermonType = 313  # 非法的艾瑟萌类型
	InvalidExerGiftType = 314  # 非法的艾瑟萌天赋类型

	# ExerSlot
	IncorrectSubject = 320  # 科目不正确

	# QuestionCommon
	QuestionNotExist = 400  # 题目不存在
	QuesSugarNotExist = 401  # 题目糖不存在

	# QuestionSet
	QuestionNotStarted = 410  # 本题还没开始作答
	InvalidTimeSpan = 411  # 作答时间有误


class ErrorException(Exception):

	ERROR_DICT = {
		# Common
		ErrorType.UnknownError: "服务器发生错误，请联系管理员！",
		ErrorType.Success: "",
		ErrorType.InvalidRequest: "非法的请求方法！",
		ErrorType.ParameterError: "参数错误！",
		ErrorType.InvalidRoute: "非法的请求路由！",
		ErrorType.PermissionDenied: "无权操作！",
		ErrorType.NoCurVersion: "未设置当前版本，请联系管理员！",
		ErrorType.NoCurConfigure: "当前版本无游戏配置，请联系管理员！",
		ErrorType.RequestUpdate: "当前客户端版本过旧，请更新游戏！",
		ErrorType.ErrorVersion: "错误的客户端版本，请更新游戏！",
		ErrorType.InvalidUserOper: "无效的用户操作！",
		ErrorType.SubjectNotExist: "科目不存在！",
		ErrorType.BaseParamNotExist: "属性不存在！",
		ErrorType.DatabaseError: "数据库错误！",

		# PlayerCommon
		ErrorType.PlayerNotExist: "玩家不存在！",
		ErrorType.PlayerExist: "玩家已存在！",
		ErrorType.IncorrectPassword: "密码错误！",
		ErrorType.IncorrectCode: "验证码过时或不正确！",
		ErrorType.EmailSendError: "邮件发送错误！",

		# PlayerRegister
		ErrorType.UsernameExist: "用户名已存在！",
		ErrorType.InvalidUsername: "非法的用户名格式！",
		ErrorType.InvalidPassword: "非法的密码格式！",
		ErrorType.PhoneExist: "电话号码已存在！",
		ErrorType.InvalidPhone: "非法的电话号码格式！",
		ErrorType.EmailExist: "邮箱地址已存在！",
		ErrorType.InvalidEmail: "非法的邮箱地址格式！",

		# PlayerLogin
		ErrorType.IncorrectLogin: "用户名不存在或密码错误！",
		ErrorType.UserAbnormal: "用户状态异常，请联系管理员！",
		ErrorType.UserFrozen: "用户已被冻结，请联系管理员！",

		# PlayerForget
		ErrorType.IncorrectForget: "用户名不存在或邮箱错误！",

		# PlayerInfo
		ErrorType.NameExist: "该名字已存在！",
		ErrorType.InvalidName: "非法的昵称格式！",
		ErrorType.InvalidGender: "非法的性别！",
		ErrorType.InvalidGrade: "非法的年级！",
		ErrorType.InvalidBirth: "非法的出生日期！",
		ErrorType.InvalidSchool: "非法的学校名称！",
		ErrorType.InvalidCity: "非法的居住地！",
		ErrorType.InvalidContact: "非法的联系方式！",
		ErrorType.InvalidDescription: "非法的个人介绍！",

		# CharacterCommon
		ErrorType.CharacterNotExist: "形象不存在！",

		# ItemCommon
		ErrorType.ItemNotExist: "物品不存在！",
		ErrorType.ContainerNotExist: "容器不存在！",
		ErrorType.ContItemNotExist: "物品不存在！",
		ErrorType.ContainerNotOwner: "该容器不属于当前玩家！",
		ErrorType.ContItemNotHold: "玩家未获得此物品！",
		ErrorType.IncorrectItemType: "物品类型不正确！",
		ErrorType.IncorrectContainerType: "容器类型不正确！",
		ErrorType.IncorrectContItemType: "物品类型不正确！",

		# Gain/LostItem
		ErrorType.CapacityInsufficient: "容器剩余容量不足！",
		ErrorType.QuantityInsufficient: "物品数量不足或未拥有此物品！",
		ErrorType.InvalidContItem: "无法对此物品进行操作！",

		# Equip/DequipEquip
		ErrorType.IncorrectEquipType: "装备类型不正确！",
		ErrorType.InvalidContainer: "找不到所属容器，无法进行装备操作！",

		# ExermonCommon
		ErrorType.ExermonNotExist: "艾瑟萌不存在！",
		ErrorType.ExerGiftNotExist: "艾瑟萌天赋不存在！",
		ErrorType.ExerSlotNotExist: "艾瑟萌槽未创建！",
		ErrorType.PlayerExermonNotExist: "尚未拥有该艾瑟萌！",
		ErrorType.PlayerExerGiftNotExist: "尚未拥有该天赋！",

		ErrorType.InvalidExermonCount: "非法的艾瑟萌数量！",
		ErrorType.InvalidExermonName: "非法的艾瑟萌昵称格式！",
		ErrorType.InvalidExermonSubject: "非法的艾瑟萌科目！",
		ErrorType.InvalidExermonType: "非法的艾瑟萌类型！",
		ErrorType.InvalidExerGiftType: "非法的艾瑟萌天赋类型！",

		# ExerSlot
		ErrorType.IncorrectSubject: "艾瑟萌科目与艾瑟萌槽科目不一致！",

		# QuestionCommon
		ErrorType.QuestionNotExist: "题目不存在！",
		ErrorType.QuesSugarNotExist: "题目糖不存在！",

		# QuestionSet
		ErrorType.QuestionNotStarted: "本题还没开始作答！",
		ErrorType.InvalidTimeSpan: "作答时间有误！",

	}

	def __init__(self, error_type: ErrorType):
		self.error_type = error_type
		self.msg = ErrorException.ERROR_DICT[error_type]

	def __str__(self):
		return self.msg