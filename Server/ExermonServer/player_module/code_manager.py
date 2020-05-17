from utils.exception import GameException, ErrorType
from utils.runtime_manager import RuntimeManager
import datetime, random


# =======================
# 验证码数据类
# =======================
class CodeDatum:

    CODE_LENGTH = 6  # 验证码位数
    CODE_RESEND_MINUTE = 1  # 验证码发送间隔时间（分钟）
    CODE_MINUTE = 5  # 验证码有效时间（分钟）

    def __init__(self, un, email, type):
        self.un = un
        self.email = email
        self.type = type
        self.code = self.generateCode()

        # token 过期时间
        now = datetime.datetime.now()
        delta = datetime.timedelta(0, self.CODE_MINUTE * 60)
        self.out_time = now+delta

        delta = datetime.timedelta(0, self.CODE_RESEND_MINUTE * 60)
        self.resend_time = now+delta

    # 生成一个 code（100000~999999）
    def generateCode(self):
        max_ = pow(10, self.CODE_LENGTH)
        return str(random.randint(max_ / 10, max_ - 1))


# =======================
# 验证码管理类，管理验证码数据，处理验证码发送
# =======================
class CodeManager:

    code_data = {}

    # 生成一个 CodeDatum 并存入 code_data 中
    @classmethod
    def generateCode(cls, un, email, type):

        now = datetime.datetime.now()

        code = cls.getCode(un, email, type)
        if code is not None and now < code.resend_time:
            raise GameException(ErrorType.CodeTooFrequent)

        code = CodeDatum(un, email, type)
        cls.code_data[cls.getKey(un, email, type)] = code

        return code.code

    # 获取键名
    @classmethod
    def getKey(cls, un, email, type):
        return '%s,%s,%s' % (un, email, type)

    # 寻找指定的 CodeDatum
    @classmethod
    def getCode(cls, un, email, type):
        key = cls.getKey(un, email, type)
        if key in cls.code_data:
            return cls.code_data[key]
        return None

    # 删除指定的 CodeDatum
    @classmethod
    def deleteCode(cls, un, email, type):
        key = cls.getKey(un, email, type)
        if key in cls.code_data:
            cls.code_data.pop(key)

    # 确保验证码正确，否则抛出异常：ErrorType.IncorrectCode
    @classmethod
    def ensureCode(cls, un, email, code, type):
        key = cls.getKey(un, email, type)
        if key not in cls.code_data:
            raise GameException(ErrorType.IncorrectCode)

        if cls.code_data[key].code != code:
            raise GameException(ErrorType.IncorrectCode)

    # 扫描 Code，如果过期就删去
    @classmethod
    async def scanCodes(cls):
        tmp_data = cls.code_data.copy()
        for code in tmp_data:
            if datetime.datetime.now() > tmp_data[code].out_time:
                cls.code_data.pop(code)


RuntimeManager.registerEvent(CodeManager.scanCodes)
