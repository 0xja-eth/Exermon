from enum import Enum


class WebscoketCloseCode(Enum):

	ServerClose = 1000  # 鏈嶅姟鍣ㄥ垏鏂繛鎺?
	ClientClose = 1001  # 瀹㈡埛绔垏鏂繛鎺?
	AbnormalClose = 1006  # 闈炴甯告柇寮�杩炴帴

	Logout = 3000  # 鐢ㄦ埛鐧诲嚭
	Kickout = 3001  # 鐢ㄦ埛韪㈠嚭


class ErrorType(Enum):
	# Common
	Success = 0  # 成功，无错误
	InvalidRequest = 1  # 非法的请求方法
	ParameterError = 2  # 参数错误
	InvalidRoute = 3  # 非法路由
	PermissionDenied = 4  # 无权操作
	NoCurVersion = 5  # 未设置当前版本
	RequestUpdate = 6  # 需要更新
	ErrorVersion = 7  # 错误的游戏版本
	InvalidUserOper = 10  # 无效的用户操作
	SubjectNotExist = 20  # 科目不存在
	BaseParamNotExist = 21  # 属性不存在
	StarNotExist = 22  # 星级不存在
	TypeNotExist = 23  # 类型不存在
	DatabaseError = 30  # 数据库错误
	UnknownError = 999  # 未知错误

	# PlayerCommon
	PlayerNotExist = 100  # 玩家不存在
	PlayerExist = 101  # 玩家已存在
	IncorrectPassword = 102  # 密码错误
	UnselectedSubject = 103  # 人物未选择该科目
	CodeTooFrequent = 104  # 验证码发送太频繁

	# PlayerRegister/Retrieve
	UsernameExist = 110  # 用户名已存在
	InvalidUsername = 111  # 非法的用户名
	InvalidPassword = 112  # 非法的密码
	PhoneExist = 113  # 电话号码已存在
	InvalidPhone = 114  # 非法的电话号码
	EmailExist = 115  # 邮箱地址已存在
	InvalidEmail = 116  # 非法的邮箱地址
	IncorrectRetrieve = 117  # 不正确的找回密码参数
	IncorrectCode = 118  # 验证码错误
	EmailSendError = 119  # 邮件发送错误

	# PlayerLogin
	IncorrectLogin = 130  # 不正确的登陆参数
	UserAbnormal = 131  # 用户状态异常
	UserFrozen = 132  # 用户被冻结

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
	SlotItemNotFound = 230  # 找不到装备槽
	EquipIndexNotFound = 231  # 找不到装备索引
	ContainerNotFound = 232  # 找不到所属容器
	IncorrectEquipType = 233  # 装备类型不正确

	# UseItem
	UnusableItem = 240  # 物品不可用
	InvalidItemUsing = 241  # 无效的物品使用
	ItemFrozen = 242  # 物品冻结中
	InvalidOccasion = 243  # 非法的使用场合
	InvalidUseTarget = 244  # 非法的使用目标
	InvalidBatchCount = 245  # 无效的使用次数

	# Buy/Sell/DiscardItem
	UnsellableItem = 250  # 物品不可出售
	UndiscardableItem = 251  # 物品不可丢弃
	UnboughtableItem = 252  # 物品不可购买
	NotEnoughMoney = 253  # 没有足够金钱
	InvalidBuyType = 254  # 非法的购买方式

	# ExermonCommon
	ExermonNotExist = 300  # 艾瑟萌不存在
	ExerGiftNotExist = 301  # 艾瑟萌天赋不存在
	ExerSlotNotExist = 302  # 艾瑟萌槽不存在
	ExerSlotItemNotExist = 303  # 艾瑟萌槽项不存在
	PlayerExermonNotExist = 304  # 玩家艾瑟萌不存在
	PlayerExerGiftNotExist = 305  # 玩家艾瑟萌天赋不存在

	# Create/Edit
	InvalidExermonCount = 310  # 非法的艾瑟萌数量
	InvalidExermonName = 311  # 非法的艾瑟萌昵称
	InvalidExermonSubject = 312  # 非法的艾瑟萌科目
	InvalidExermonType = 313  # 非法的艾瑟萌类型
	InvalidExerGiftType = 314  # 非法的艾瑟萌天赋类型

	# ExerSlot
	IncorrectSubject = 320  # 科目不正确

	# ExerEquipSlot
	InsufficientLevel = 330  # 等级不足

	# UseExerSkill
	PassiveSkill = 340  # 被动技能
	MPInsufficient = 341  # 精力值不足
	NoUseCount = 342  # 无剩余使用次数

	# QuestionCommon
	QuestionNotExist = 400  # 题目不存在
	QuestionLinkNotExist = 401  # 题目关系不存在
	PictureNotFound = 402  # 图片不存在
	IncorrectQuestionType = 403  # 题目类型不正确

	# QuestionGenerate
	InvalidGenerateConfigure = 410  # 非法的题目生成配置

	# RecordCommon
	QuestionRecordNotExist = 500  # 题目记录不存在
	ExerciseRecordNotExist = 501  # 刷题记录不存在
	ExerciseQuestionNotExist = 502  # 刷题题目不存在

	# QuestionRecord
	InvalidNote = 510  # 无效的备注格式

	# QuestionSetRecord
	QuestionNotStarted = 520  # 本题还没开始作答
	InvalidTimeSpan = 521  # 作答时间有误

	# QuestionGenerate
	GenerateError = 530  # 题目生成有误

	# QuesReport
	QuesReportTooLong = 540  # 题目反馈太长
	InvalidQuesReportType = 541  # 题目反馈类型不对
	QuesReportNotExist = 542  # 查找不到反馈记录

	# BattleCommon
	BattleNotExist = 600  # 对战不存在
	BattleRecordNotExist = 601  # 对战记录不存在
	NotInBattle = 602  # 未加入对战
	AlreadyInBattle = 603  # 已加入对战
	BattleStarted = 604  # 对战已开始
	BattleTerminated = 605  # 对战已结束

	# BattleMatching
	IsBanned = 610  # 账号被禁赛
	AlreadyMatched = 611  # 已经匹配到对手

	# BattlePreparing
	ItemNotEquiped = 620  # 物品未装备
	IncorrectTarget = 621  # 不正确的使用目标

	# BattleQuesting
	OpponentAnswered = 630  # 对方已抢答

	# BattleActing

	# BattleResulting

	# SeasonCommon
	SeasonNotExist = 700  # 赛季不存在
	SeasonRecordNotExist = 701  # 赛季记录不存在
	CompRankNotExist = 702  # 赛季排位不存在
	ResultJudgeNotExist = 703  # 结果判断类型不存在

	# EnglishProCommon
	MapNotFound = 800  # 地图不存在
	StageNotFound = 801  # 关卡不存在
	ExerProStarted = 802  # 特训正在进行
	ExerProRecordNotExist = 803  # 特训记录不存在

	# EnglishProItem
	ExerProItemNotExist = 810  # 特训道具不存在
	ExerProPotionNotExist = 811  # 特训药水不存在
	ExerProCardNotExist = 812  # 特训卡牌不存在

	# EnglishProQuestion
	InvalidQuestionCount = 820  # 非法的题目数量
	InvalidQuestionType = 821  # 非法的题目类型
	InvalidQuestionDatabaseCount = 822  # 超过题库数量

	# EnglishProWord
	WordNotExit = 830  # 单词不存在
	WordRecordNotExit = 831  # 单词记录不存在
	AnswerNotFinish = 832  # 当前轮单词没有问答完毕
	NoEnoughNewWord = 833  # 单词库中没有足够的新单词
	NoInCurrentWords = 834  # 该单词不在当前轮中
	WordAlreadyCorrect = 835  # 本轮该单词已答对

	# EnglishProShop
	ShopNotGenerated = 840  # 商品尚未生成
	ShopItemNotExist = 841  # 商品不存在
	InvalidBuyNum = 842  # 金钱不足

	# EnglishProAnswer
	InvalidAnswer = 843  # 题目答案不合法


class GameException(Exception):

	ERROR_DICT = {
		# Common
		ErrorType.Success: "",
		ErrorType.InvalidRequest: "非法的请求方法！",
		ErrorType.ParameterError: "参数错误！",
		ErrorType.InvalidRoute: "非法的请求路由！",
		ErrorType.PermissionDenied: "无权操作！",
		ErrorType.NoCurVersion: "未设置当前版本，请联系管理员！",
		ErrorType.RequestUpdate: "当前客户端版本过旧，请更新游戏！",
		ErrorType.ErrorVersion: "错误的客户端版本，请更新游戏！",
		ErrorType.InvalidUserOper: "无效的用户操作！",
		ErrorType.SubjectNotExist: "科目不存在！",
		ErrorType.BaseParamNotExist: "属性不存在！",
		ErrorType.StarNotExist: "星级不存在！",
		ErrorType.TypeNotExist: "类型不存在！",
		ErrorType.DatabaseError: "数据库错误！",
		ErrorType.UnknownError: "服务器发生错误，请联系管理员！",

		# PlayerCommon
		ErrorType.PlayerNotExist: "玩家不存在！",
		ErrorType.PlayerExist: "玩家已存在！",
		ErrorType.IncorrectPassword: "密码错误！",
		ErrorType.UnselectedSubject: "人物未选择该科目！",
		ErrorType.CodeTooFrequent: "验证码发送太频繁，请1分钟后重试！",

		# PlayerRegister/Retrieve
		ErrorType.UsernameExist: "用户名已存在！",
		ErrorType.InvalidUsername: "非法的用户名格式！",
		ErrorType.InvalidPassword: "非法的密码格式！",
		ErrorType.PhoneExist: "电话号码已存在！",
		ErrorType.InvalidPhone: "非法的电话号码格式！",
		ErrorType.EmailExist: "邮箱地址已存在！",
		ErrorType.InvalidEmail: "非法的邮箱地址格式！",
		ErrorType.IncorrectRetrieve: "用户名不存在或邮箱错误！",
		ErrorType.IncorrectCode: "验证码过时或不正确！",
		ErrorType.EmailSendError: "邮件发送错误！",

		# PlayerLogin
		ErrorType.IncorrectLogin: "用户名不存在或密码错误！",
		ErrorType.UserAbnormal: "用户状态异常，请联系管理员！",
		ErrorType.UserFrozen: "用户已被冻结，请联系管理员！",

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
		ErrorType.SlotItemNotFound: "找不到装备槽！",
		ErrorType.EquipIndexNotFound: "找不到装备类型！",
		ErrorType.ContainerNotFound: "找不到所属容器，无法进行装备操作！",
		ErrorType.IncorrectEquipType: "装备类型不正确！",

		# UseItem
		ErrorType.UnusableItem: "物品不可用！",
		ErrorType.InvalidItemUsing: "无效的物品使用！",
		ErrorType.ItemFrozen: "物品冻结中！",
		ErrorType.InvalidOccasion: "非法的使用场合！",
		ErrorType.InvalidUseTarget: "非法的使用目标！",
		ErrorType.InvalidBatchCount: "无效的使用次数！",

		# Buy/Sell/DiscardItem
		ErrorType.UnsellableItem: "物品不可出售！",
		ErrorType.UndiscardableItem: "物品不可丢弃！",
		ErrorType.UnboughtableItem: "物品暂时无法购买！",
		ErrorType.NotEnoughMoney: "没有足够的金钱！",
		ErrorType.InvalidBuyType: "非法的购买方式！",

		# ExermonCommon
		ErrorType.ExermonNotExist: "艾瑟萌不存在！",
		ErrorType.ExerGiftNotExist: "艾瑟萌天赋不存在！",
		ErrorType.ExerSlotNotExist: "艾瑟萌槽未创建！",
		ErrorType.ExerSlotItemNotExist: "尚未拥有该艾瑟萌槽！",
		ErrorType.PlayerExermonNotExist: "尚未拥有该艾瑟萌！",
		ErrorType.PlayerExerGiftNotExist: "尚未拥有该天赋！",

		# Create/Edit
		ErrorType.InvalidExermonCount: "非法的艾瑟萌数量！",
		ErrorType.InvalidExermonName: "非法的艾瑟萌昵称格式！",
		ErrorType.InvalidExermonSubject: "非法的艾瑟萌科目！",
		ErrorType.InvalidExermonType: "非法的艾瑟萌类型！",
		ErrorType.InvalidExerGiftType: "非法的艾瑟萌天赋类型！",

		# ExerSlot
		ErrorType.IncorrectSubject: "艾瑟萌科目与槽科目不一致！",

		# ExerEquipSlot
		ErrorType.InsufficientLevel: "艾瑟萌槽等级不足！",

		# UseExerSkill
		ErrorType.PassiveSkill: "被动技能无法使用！",
		ErrorType.MPInsufficient: "精力值不足！",
		ErrorType.NoUseCount: "无剩余使用次数！",

		# QuestionCommon
		ErrorType.QuestionNotExist: "题目不存在！",
		ErrorType.QuestionLinkNotExist: "题目不存在！",
		ErrorType.PictureNotFound: "找不到图片文件！",
		ErrorType.IncorrectQuestionType: "题目类型不正确！",

		# QuestionGenerate
		ErrorType.InvalidGenerateConfigure: "非法的题目生成配置！",

		# RecordCommon
		ErrorType.QuestionRecordNotExist: "题目记录不存在！",
		ErrorType.ExerciseRecordNotExist: "刷题记录不存在！",
		ErrorType.ExerciseQuestionNotExist: "刷题题目不存在！",

		# QuestionRecord
		ErrorType.InvalidNote: "无效的备注格式",

		# QuestionSetRecord
		ErrorType.QuestionNotStarted: "本题还没开始作答！",
		ErrorType.InvalidTimeSpan: "作答时间有误！",

		# QuestionGenerate
		ErrorType.GenerateError: "题目生成有误！",

		# QuesReport
		ErrorType.QuesReportTooLong: "题目反馈太长！",
		ErrorType.InvalidQuesReportType: "题目反馈类型不对！",
		ErrorType.QuesReportNotExist: "查找不到反馈记录！",

		# BattleCommon
		ErrorType.BattleNotExist: "对战不存在！",
		ErrorType.BattleRecordNotExist: "对战记录不存在！",
		ErrorType.NotInBattle: "尚未加入对战！",
		ErrorType.AlreadyInBattle: "已加入一场对战！",
		ErrorType.BattleStarted: "对战已开始！",
		ErrorType.BattleTerminated: "对战已结束！",

		# BattleMatching
		ErrorType.IsBanned: "您因信誉积分不足，暂时已被禁赛！如有疑问请联系管理员。",
		ErrorType.AlreadyMatched: "已经匹配到对手！",

		# BattlePreparing
		ErrorType.ItemNotEquiped: "所使用的物品未被装备！",
		ErrorType.IncorrectTarget: "不正确的使用目标！",

		# BattleQuesting
		ErrorType.OpponentAnswered: "对方已抢答！",

		# BattleActing

		# BattleResulting

		# SeasonCommon
		ErrorType.SeasonNotExist: "赛季不存在！",
		ErrorType.SeasonRecordNotExist: "赛季记录不存在！",
		ErrorType.CompRankNotExist: "赛季排位不存在！",
		ErrorType.ResultJudgeNotExist: "结果判断类型不存在！",

		# EnglishProCommon
		ErrorType.MapNotFound: "地图不存在！",
		ErrorType.StageNotFound: "关卡不存在！",
		ErrorType.ExerProStarted: "特训进行中！请结束当前特训再开启新的特训！",
		ErrorType.ExerProRecordNotExist: "特训记录不存在！",

		# EnglishProItem
		ErrorType.ExerProItemNotExist: "特训道具不存在！",
		ErrorType.ExerProPotionNotExist: "特训药水不存在！",
		ErrorType.ExerProCardNotExist: "特训卡牌不存在！",

		# EnglishProQuestion
		ErrorType.InvalidQuestionCount: "非法的题目数量！",
		ErrorType.InvalidQuestionType: "非法的题目类型！",
		ErrorType.InvalidQuestionDatabaseCount: "超过题库数量！",

		# EnglishProWord
		ErrorType.WordNotExit: "单词不存在！",
		ErrorType.WordRecordNotExit: "单词记录不存在！",
		ErrorType.AnswerNotFinish: "当前轮单词没有问答完毕！",
		ErrorType.NoEnoughNewWord: "单词库中没有足够的新单词！",
		ErrorType.NoInCurrentWords: "该单词不在当前轮中！",
		ErrorType.WordAlreadyCorrect: "本轮该单词已答对！",

		# EnglishProShop
		ErrorType.ShopNotGenerated: "商品尚未生成！",
		ErrorType.ShopItemNotExist: "商品不存在或已售出！",
		ErrorType.InvalidBuyNum: "金钱不足！",

		# EnglishProAnswer
		ErrorType.InvalidAnswer: "题目答案不合法！",

	}

	def __init__(self, error_type: ErrorType):
		"""

		Args:
			error_type (ErrorType): 閿欒绫诲瀷
		"""
		self.error_type = error_type
		self.msg = GameException.ERROR_DICT[error_type]

	def __str__(self):
		return self.msg
