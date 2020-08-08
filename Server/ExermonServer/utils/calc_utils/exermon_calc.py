import math


# ================================
# 艾瑟萌等级计算类
# ================================
class ExermonLevelCalc:
    # 星级等级表
    StarLevelTable = None

    # 初始化，计算所有星级的等级表
    @classmethod
    def init(cls):
        from game_module.models import ExerStar

        cls.StarLevelTable = {}
        stars = ExerStar.objs()

        for s in stars:
            cls.StarLevelTable[s] = cls._generateTable(s)

    # 生成某个星级的等级表
    @classmethod
    def _generateTable(cls, star):
        """
        生成某个星级的等级表
        Args:
            star (ExerStar): 艾瑟萌星级
        Returns:
            返回单个星级等级经验表格
        """
        res = []

        _max = star.max_level
        a = star.level_exp_factors['a']
        b = star.level_exp_factors['b']
        c = star.level_exp_factors['c']

        # 生成每一等级的最低累计经验值
        for x in range(_max):
            res.append(cls._calcTable(x - 1, a, b, c))

        return res

    # 计算表格函数
    @classmethod
    def _calcTable(cls, x, a, b, c):
        return a / 3 * x * x * x + (a + b) / 2 * x * x + (a + b * 3 + c * 6) / 6 * x

    # 获取所需经验值
    @classmethod
    def getDetlaExp(cls, q, level):

        if level >= q.max_level: return -1

        if cls.StarLevelTable is None: cls.init()

        data = cls.StarLevelTable[q]
        return data[level] - data[level - 1]

    # 获取下级所需经验值
    @classmethod
    def getDetlaSumExp(cls, q, level):

        if level >= q.max_level: return -1

        if cls.StarLevelTable is None: cls.init()

        data = cls.StarLevelTable[q]
        return data[level]

    # 获取累计经验
    @classmethod
    def getSumExp(cls, q, level, exp):

        if level > q.max_level: level = q.max_level

        if cls.StarLevelTable is None: cls.init()

        return cls.StarLevelTable[q][level - 1] + exp


# ================================
# 艾瑟萌属性计算类
# ================================
class ExermonParamCalc:
    # 基本属性值(BPV)	EPB*((实际属性成长率/R+1)*S)^(L-1)

    S = 1.005
    R = 233

    # 计算属性
    @classmethod
    def calc(cls, base, rate, level, plus=0, plus_rate=1):
        value = base * pow((rate / cls.R + 1) * cls.S, level - 1)
        return value * plus_rate + plus


# ================================
# 艾瑟萌槽和玩家等级计算类
# ================================
class ExermonSlotLevelCalc:
    T = 500
    A = 0.66

    TP = 500
    AP = 0.7
    D = 3

    # 计算等级
    @classmethod
    def calcLevel(cls, exp):
        return math.floor(pow(exp / cls.T, cls.A)) + 1

    # 计算对应等级所需累计经验
    @classmethod
    def calcExp(cls, level):
        return math.ceil(pow(level - 1, 1.0 / cls.A) * cls.T)

    # 计算玩家等级
    @classmethod
    def calcPlayerLevel(cls, exp):
        return math.floor(pow(exp / cls.TP / cls.D, cls.AP)) + 1

    # 计算玩家对应等级所需累计经验
    @classmethod
    def calcPlayerExp(cls, level):
        return math.ceil(pow(level - 1, 1 / cls.AP) * cls.TP * cls.D)


# ================================
# 艾瑟萌槽项属性成长率计算类
# ================================
class ExerSlotItemParamRateCalc:

    # 计算属性
    @classmethod
    def calc(cls, exerslot_item, **kwargs):

        if exerslot_item is None: return 0

        player_exer = exerslot_item.playerExer()
        if player_exer is None: return 0
        epr = player_exer.paramRate(**kwargs)

        player_gift = exerslot_item.playerGift()
        if player_gift is None: return epr
        gprr = player_gift.paramRate(**kwargs)

        return epr * gprr


# ================================
# 艾瑟萌槽项属性计算类
# ================================
class ExerSlotItemParamCalc:

    # 艾瑟萌属性值计算公式
    # 实际属性值(RPV)	(基本属性值+附加属性值)*实际加成率+追加属性值
    # 基本属性值(BPV)	EPB*((实际属性成长率/R+1)*S)^(L-1)
    # 实际属性成长率(RPR)	EPR*GPRR
    # 附加属性值(PPV)	EPPV+SPPV
    # 实际加成率(RR)		基础加成率*附加加成率
    # 基础加成率(BR)		1
    # 附加加成率(PR)		GPR*SPR（对战时还包括题目糖属性加成率）
    # 追加属性值(APV)	SAPV

    # 计算属性
    @classmethod
    def calc(cls, exerslot_item, **kwargs):
        calc_obj = cls(exerslot_item, **kwargs)
        return calc_obj.value

    def __init__(self, exerslot_item, **kwargs):
        from exermon_module.models import Exermon, ExerSlotItem, \
            PlayerExermon, PlayerExerGift, ExerEquipSlot
        self.value = 0

        self.exerslot_item: ExerSlotItem = exerslot_item
        if exerslot_item is None: return

        self.player_exer: PlayerExermon = self.exerslot_item.playerExer()
        if self.player_exer is None: return

        self.exermon: Exermon = self.player_exer.exermon()
        if self.exermon is None: return

        self.player_gift: PlayerExerGift = self.exerslot_item.playerGift()
        self.exer_equip_slot: ExerEquipSlot = self.exerslot_item.exerEquipSlot()

        self.kwargs = kwargs

        self.value = self._calc()

    # 计算 RPV = (基本属性值+附加属性值)*实际加成率+追加属性值
    def _calc(self):

        bpv = self._calcBaseParamValue()
        ppv = self._calcPlusParamValue()
        rr = self._calcRealRate()
        apv = self._calcAppendParamValue()

        val = (bpv + ppv) * rr + apv
        return self._adjustParamValue(val)

    # 计算 BPV = EPB*((实际属性成长率/R+1)*S)^(L-1)
    def _calcBaseParamValue(self):

        epb = self.player_exer.paramBase(**self.kwargs)
        rpr = self._calcRealParamRate()

        plus = self.player_exer.plusParamVal(**self.kwargs)
        plus_rate = self.player_exer.plusParamRate(**self.kwargs)

        return ExermonParamCalc.calc(epb, rpr,
                                     self.player_exer.level, plus, plus_rate)

    # 计算 RPR = EPR*GPRR
    def _calcRealParamRate(self):

        return ExerSlotItemParamRateCalc.calc(self.exerslot_item, **self.kwargs)

    # 计算 PPV = EPPV+SPPV
    def _calcPlusParamValue(self):
        return self._calcEquipPlusParamValue() + \
               self._calcStatusPlusParamValue()

    # 计算 EPPV 装备附加值
    def _calcEquipPlusParamValue(self):
        if self.exer_equip_slot is None: return 0
        return self.exer_equip_slot.paramVal(**self.kwargs)

    # 计算 SPPV 状态附加值
    def _calcStatusPlusParamValue(self):
        return 0

    # 计算 RR = 基础加成率*附加加成率
    def _calcRealRate(self):
        return self._calcBaseRate() * self._calcPlusRate()

    # 计算 BR
    def _calcBaseRate(self):
        return 1

    # 计算 PR = GPR*SPR（对战时还包括题目糖属性加成率）
    def _calcPlusRate(self):
        return 1

    # 计算 APV
    def _calcAppendParamValue(self):
        return 0

    # 调整值
    def _adjustParamValue(self, val):
        param = self.getParam(**self.kwargs)

        if param is None: return val

        return param.clamp(val)

    # 获取属性实例
    @classmethod
    def getParam(cls, **kwargs):
        from game_module.models import BaseParam

        if 'param_id' in kwargs:
            kwargs['id'] = kwargs['param_id']
            kwargs.pop('param_id')

        return BaseParam.get(**kwargs)


# ================================
# 装备属性计算类
# ================================
class ExerEquipParamCalc:

    # 计算属性
    @classmethod
    def calc(cls, equip_slot_item, **kwargs):
        calc_obj = cls(equip_slot_item, **kwargs)
        return calc_obj.value

    def __init__(self, equip_slot_item, **kwargs):
        from exermon_module.models import PlayerExermon, \
            ExerSlotItem, ExerEquipSlotItem, EquipPackItem
        from player_module.models import Player

        self.value = 0

        self.equip_slot_item: ExerEquipSlotItem = equip_slot_item

        self.player: Player = self.equip_slot_item.exactlyPlayer()

        self.pack_equip: EquipPackItem = self.equip_slot_item.packEquip()
        if self.pack_equip is None: return

        self.equip = self.pack_equip.item
        if self.equip is None: return

        self.exerslot_item: ExerSlotItem = \
            self.equip_slot_item.container.exerSlotItem()

        self.player_exer: PlayerExermon = self.exerslot_item.playerExer()
        if self.player_exer is None: return

        self.param = ExerSlotItemParamCalc.getParam(**kwargs)
        if self.param is None: return

        self.value = self._calc()

    def _calc(self):

        value = self.pack_equip.baseParam(param_id=self.param.id)
        # rate = self.pack_equip.levelParam(param_id=self.param.id)
        # level = self.player_exer.level

        return value  # + rate * level


# ================================
# 战斗力计算类
# ================================
class BattlePointCalc:

    # 艾瑟萌战斗力计算公式
    # 战斗力(BV)			round((MHP+MMP*2+ATK*6*C*(1+CRI/100)+DEF*4)*(1+EVA/50))

    @classmethod
    def calc(cls, func: callable):
        """
        计算战斗力
        Args:
            func (callable): 一个可以传入属性ID获得相应属性值的函数
        Returns:
            返回计算后的战斗力
        """
        from game_module.models import BaseParam

        kwargs = {}
        params = BaseParam.objs()
        for param in params:
            attr = param.attr
            if attr == 'def': attr = 'def_'
            kwargs[attr] = func(param_id=param.id)

        return cls.doCalc(**kwargs)

    # 计算战斗力
    @classmethod
    def doCalc(cls, mhp, mmp, atk, def_, eva, cri):
        from .action_calc import BattleAttackProcessor

        return round((mhp + mmp * 2 + atk * 6 * BattleAttackProcessor.C
                      * (1 + cri / 100) + def_ * 4) * (1 + eva / 50))

