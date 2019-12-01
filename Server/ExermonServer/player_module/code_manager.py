from  utils.exception import ErrorException, ErrorType
import datetime, random

# =======================
# 验证码数据类
# =======================
class CodeDatum:

    CODE_LENGTH = 6 # 验证码位数
    CODE_SECOND = 60 # 验证码有效时间（秒）

    def __init__(self, un, email, type):
        self.un = un
        self.email = email
        self.type = type
        self.code = self.generateCode()

        # token 过期时间
        now = datetime.datetime.now()
        delta = datetime.timedelta(0, self.CODE_SECOND)
        self.out_time = now+delta

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
        code = cls.getCode(un, email, type) or CodeDatum(un, email, type)
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
        else:
            raise ErrorException(ErrorType.InvalidCode)

    # 确保验证码正确，否则抛出异常：ErrorType.InvalidCode
    @classmethod
    def ensureCode(cls, un, email, code, type):
        if cls.getKey(un, email, type) in cls.code_data:
            return cls.code_data[cls.getKey(un, email, type)]==code
        else:
            raise ErrorException(ErrorType.IncorrectCode)

    # 扫描 Code，如果过期就删去
    @classmethod
    def scanCodes(cls):
        for code in cls.code_data:
            if datetime.datetime.now() > cls.code_data[code].token.out_time:
                cls.code_data.pop(code)
