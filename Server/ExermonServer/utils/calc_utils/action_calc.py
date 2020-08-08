import random


# ===================================================
# 通用物品效果处理类
# ===================================================
class GeneralItemEffectProcessor:

    @classmethod
    def process(cls, cont_item, player, count, target=None, target_slot_item=None):
        processor = GeneralItemEffectProcessor(
            cont_item, player, count, target, target_slot_item)
        processor._process()

    def __init__(self, cont_item, player, count, target=None, target_slot_item=None):
        from player_module.models import Player
        from item_module.models import PackContItem
        from exermon_module.models import PlayerExermon, ExerSlotItem

        self.player: Player = player
        self.cont_item: PackContItem = cont_item
        self.target_slot_item: ExerSlotItem = target_slot_item
        self.target: PlayerExermon = target
        self.count = count

    def _process(self):
        """
        开始处理
        """
        from item_module.models import UsableItem

        item = self.cont_item.item

        if isinstance(item, UsableItem):
            self._processUsableItem(item)

    def _processUsableItem(self, item):
        """
        处理 可使用物品 的使用效果
        Args:
            item (UsableItem): 可使用物品
        """
        for effect in item.effects():
            self.__processEffect(effect)

    def __processEffect(self, effect):
        """
        处理具体效果
        Args:
            effect (BaseEffect): 效果
        """
        from item_module.models import BaseItem, ItemEffectCode
        from item_module.views import Common as ItemCommon

        player = self.player
        target = self.target
        target_slot_item = self.target_slot_item
        count = self.count

        code = ItemEffectCode(effect.code)
        params = effect.params

        p_len = len(params)

        if code == ItemEffectCode.AddParam and target:
            if p_len == 3: params[2] = params[2] / 100 + 1
            target.addPlusParam(*params, count=count)

        elif code == ItemEffectCode.TempAddParam and target:
            if p_len == 4: params[3] = params[3] / 100 + 1
            target.addTempPlusParam(*params, count=count)

        elif code == ItemEffectCode.GainExermonExp and target:
            min_cnt = max_cnt = params[0] * count
            if p_len == 2: max_cnt = params[1] * count

            value = random.randint(min_cnt, max_cnt)
            target.gainExp(value)

        elif code == ItemEffectCode.GainPlayerExp:
            min_cnt = max_cnt = params[0] * count
            if p_len == 2: max_cnt = params[1] * count

            value = random.randint(min_cnt, max_cnt)
            player.gainExp(value)

        elif code == ItemEffectCode.GainItem:
            type_, item_id = params[0], params[1]
            min_cnt = max_cnt = params[2] * count

            if p_len == 4: max_cnt = params[3] * count

            item: BaseItem = ItemCommon.getItem(type_, id=item_id)

            container_class = item.containerClass()
            container = player.getContainer(container_class)

            value = random.randint(min_cnt, max_cnt)
            container.gainItems(item, value)

        elif code == ItemEffectCode.GainGold:
            min_cnt = max_cnt = params[0] * count
            if p_len == 2: max_cnt = params[1] * count

            value = random.randint(min_cnt, max_cnt)
            player.gainMoney(gold=value)

        elif code == ItemEffectCode.GainBoundTicket:
            min_cnt = max_cnt = params[0] * count
            if p_len == 2: max_cnt = params[1] * count

            value = random.randint(min_cnt, max_cnt)
            player.gainMoney(bound_ticket=value)

        elif code == ItemEffectCode.Eval:
            for i in range(count):
                eval(params[0])

        target.refresh()
        if target_slot_item is not None:
            target_slot_item.refresh()


# ===================================================
# 物品效果处理类
# ===================================================
class BattleItemEffectProcessor:

    @classmethod
    def process(cls, item, player):
        processor = BattleItemEffectProcessor(item, player)
        processor._process()

    def __init__(self, item, player):
        from item_module.models import PackContItem
        from battle_module.runtimes import \
            RuntimeBattlePlayer, RuntimeBattleExermon

        self.cont_item: PackContItem = item
        # self.battle: RuntimeBattle = battle
        self.player: RuntimeBattlePlayer = player
        self.exermon: RuntimeBattleExermon = self.player.getCurrentExermon()

    def _process(self):
        """
        开始处理
        """
        from item_module.models import UsableItem

        item = self.cont_item.item

        if isinstance(item, UsableItem):
            self._processUsableItem(item)
            self.player.addPrepareAction(item)

    def _processUsableItem(self, item):
        """
        处理 可使用物品 的使用效果
        Args:
            item (UsableItem): 可使用物品
        """
        for effect in item.effects():
            self.__processEffect(effect)

    def __processEffect(self, effect):
        """
        处理具体效果
        Args:
            effect (BaseEffect): 效果
        """
        from item_module.models import ItemEffectCode

        player = self.player
        exermon = self.exermon

        code = ItemEffectCode(effect.code)
        params = effect.params.copy()

        p_len = len(params)

        if code == ItemEffectCode.RecoverHP:
            if p_len == 1: player.recoverHP(params[0])
            if p_len == 2: player.recoverHP(params[0], params[1] / 100)

        if code == ItemEffectCode.RecoverMP:
            if p_len == 1: exermon.recoverMP(params[0])
            if p_len == 2: exermon.recoverMP(params[0], params[1] / 100)

        if code == ItemEffectCode.BattleAddParam:
            if p_len == 4: params[3] = params[3] / 100 + 1
            exermon.addParam(*params)


# ===================================================
# 对战攻击处理类
# ===================================================
class BattleAttackProcessor:
    # 实际伤害点数(RD)
    # round(((基础伤害 + 附加伤害) * 伤害加成 + 追加伤害) * 攻击结果修正 * 波动修正)
    #
    # 基础伤害(BD)
    # AATK * 实际攻击比率 - BDEF * 实际防御比率 + 附加伤害
    #
    # 实际攻击比率
    # AR * 技能攻击比率
    #
    # 实际防御比率
    # DR * 技能防御比率
    #
    # 附加伤害
    # 技能附加伤害公式
    #
    # 附加伤害(PD)
    # 0（用于拓展）
    #
    # 伤害加成(DR)
    # 1（用于拓展）
    #
    # 追加伤害(AD)
    # 0（用于拓展）
    #
    # 攻击结果修正(RR)
    # 回避修正 * 暴击修正
    #
    # 回避修正(MR)
    # rand() < BEVA / 100 ? 0 : 1
    #
    # 暴击修正(CR)
    # rand() < ACRI / 100 ? C : 1
    #
    # 波动修正(FR)
    # 1 + rand(F * 2) - F

    AR = 3  # 攻击比率
    DR = 1  # 防御比率

    C = 2  # 暴击伤害加成
    F = 8  # 伤害波动（*100）

    class TempParam:
        """
        临时属性类
        """
        MHP = MMP = ATK = DEF = EVA = CRI = 0

        def __init__(self, exermon):
            from battle_module.runtimes import RuntimeBattleExermon
            self.exermon: RuntimeBattleExermon = exermon
            self.load()

        def load(self):
            from game_module.models import BaseParam

            params = BaseParam.objs()

            for param in params:
                attr = str(param.attr).upper()
                if hasattr(self, attr):
                    val = self.exermon.paramVal(param_id=param.id)
                    setattr(self, attr, val.getValue())

    @classmethod
    def process(cls, attacker, oppo):
        processor = BattleAttackProcessor(attacker, oppo)
        processor._processSkill()
        processor._processAction()

    # region 处理内容

    def __init__(self, attacker, oppo):
        from exermon_module.models import ExerSkill, TargetType
        from battle_module.models import HitResultType
        from battle_module.runtimes import RuntimeBattlePlayer, RuntimeBattleExermon

        self.a_player: RuntimeBattlePlayer = attacker
        self.a_exer: RuntimeBattleExermon = self.a_player.getCurrentExermon()

        self.o_player: RuntimeBattlePlayer = oppo
        self.o_exer: RuntimeBattleExermon = self.o_player.getCurrentExermon()

        self.skills = self.a_exer.getUsableSkills()
        self.skill: ExerSkill = None

        self.target_type: TargetType = TargetType.Enemy
        self.hit_result: HitResultType = HitResultType.Unknown
        self.hurt = 0

    def _generateTargets(self, target_type=None):
        """
        生成目标玩家
        Args:
            oppo (RuntimeBattlePlayer): 对方
            target_type (TargetType): 目标类型
        """
        from exermon_module.models import TargetType

        if target_type is None:
            if self.skill is None:
                target_type = TargetType.Enemy
            else:
                target_type = TargetType(self.skill.target_type)

        if target_type == TargetType.BothRandom:
            target_type = random.choice([TargetType.Self, TargetType.Enemy])

        self.target_type = target_type

        if target_type == TargetType.Enemy:
            return [self.o_player]

        if target_type == TargetType.Self:
            return [self.a_player]

        if target_type == TargetType.Both:
            return [self.a_player, self.o_player]

        return []

    def _processSkill(self):
        """
        处理技能使用
        """
        from battle_module.runtimes import RuntimeBattleExerSkill

        skill: RuntimeBattleExerSkill = self._generateRandomSkill()

        if skill is None:
            self.skill = None
        else:
            self.skill = skill.skill
            skill.useSkill()

    def _generateRandomSkill(self):
        """
        生成随机使用技能
        """
        for skill in self.skills:
            rand = random.randint(1, 100)
            if rand <= skill.skill.rate:
                return skill

        return None

    def _processAction(self):
        """
        处理行动
        """
        for target in self._generateTargets():
            self._processAttack(target)

    def _processAttack(self, target):
        """
        处理攻击
        Args:
            target (RuntimeBattlePlayer): 目标
        """
        from battle_module.models import HitResultType, TargetType
        from battle_module.runtimes import RuntimeBattlePlayer, RuntimeBattleExermon

        target: RuntimeBattlePlayer = target
        b_exer: RuntimeBattleExermon = target.getCurrentExermon()

        a = self.TempParam(self.a_exer)
        b = self.TempParam(b_exer)

        self.hit_result = self._calcResultType(a, b)

        if self.hit_result != HitResultType.Miss:

            bd = self._baseDamage(a, b)
            pd = self._plusDamage()
            dr = self._damageRate()
            ad = self._appendDamage()
            fr = self._floatRate()

            rd = ((bd + pd) * dr + ad) * fr

            if self.hit_result == HitResultType.Critical:
                rd *= self._criticalRate()

            self.hurt = round(rd)

        target_type = self.target_type
        if target_type == TargetType.Both:
            if target == self.a_player: target_type = TargetType.Self
            if target == self.o_player: target_type = TargetType.Enemy

        self._addAttackAction(target_type)
        self._applyAttack(target)

        target.round_result.processAttack(self.skill,
                                          self.target_type, self.hit_result, self.hurt, False)

        # 当目标类型为双方时，如果目标与自身一致，则不再处理攻击者（避免处理两次）
        if not (self.target_type == TargetType.Both and target == self.a_player):
            self.a_player.round_result.processAttack(self.skill,
                                                     self.target_type, self.hit_result, self.hurt, True)

    def _addAttackAction(self, target_type=None):
        """
        添加攻击行动
        """
        if target_type is None: target_type = self.target_type

        self.a_player.addAttackAction(self.skill, target_type, self.hit_result, self.hurt)

    def _applyAttack(self, target):
        """
        应用攻击
        Args:
            target (RuntimeBattlePlayer): 目标
        """
        from exermon_module.models import HitType
        from battle_module.runtimes import RuntimeBattleExermon

        if self.skill is None:
            hit_type = HitType.HPDamage
        else:
            hit_type = HitType(self.skill.hit_type)

        b_exer: RuntimeBattleExermon = target.getCurrentExermon()

        if hit_type == HitType.HPDamage:
            target.recoverHP(-self.hurt)

        elif hit_type == HitType.HPRecover:
            target.recoverHP(self.hurt)

        elif hit_type == HitType.HPDrain:
            target.recoverHP(-self.hurt)
            drain_val = self.hurt * self.skill.drain_rate
            self.a_player.recoverHP(drain_val)

        elif hit_type == HitType.MPDamage:
            b_exer.recoverMP(-self.hurt)

        elif hit_type == HitType.MPRecover:
            b_exer.recoverMP(self.hurt)

        elif hit_type == HitType.MPDrain:
            b_exer.recoverMP(-self.hurt)
            drain_val = self.hurt * self.skill.drain_rate
            self.a_exer.recoverMP(drain_val)

    def _calcResultType(self, a, b):
        """
        计算攻击结果
        """
        from battle_module.models import HitResultType

        rand = random.random()

        if rand <= self._realEVA(a, b): return HitResultType.Miss
        if rand <= self._realCRI(a, b): return HitResultType.Critical

        return HitResultType.Hit

    def _realEVA(self, a, b):
        """
        实际回避率
        """
        if self.skill is None: return b.EVA
        return b.EVA - self.skill.hit_rate / 100

    def _realCRI(self, a, b):
        """
        实际暴击率
        """
        if self.skill is None: return a.CRI
        return a.CRI + self.skill.cri_rate / 100

    def _criticalRate(self):
        """
        暴击修正
        """
        return self.C

    def _baseDamage(self, a, b):
        """
        基础伤害
        """
        return a.ATK * self._realAttackRate() - b.DEF * self._realDefenseRate()

    def _realAttackRate(self):
        """
        实际攻击比率
        """
        return self.AR * self._skillAttackRate()

    def _skillAttackRate(self):
        """
        技能攻击比率
        """
        return 1 if self.skill is None else self.skill.atk_rate

    def _realDefenseRate(self):
        """
        实际防御比率
        """
        return self.DR * self._skillDefenseRate()

    def _skillDefenseRate(self):
        """
        技能防御比率
        """
        return 1 if self.skill is None else self.skill.def_rate

    def _skillPlusDamage(self, a, b):
        """
        技能附加伤害
        """
        try:
            return eval(self.skill.plus_formula)
        except:
            return 0

    def _plusDamage(self):
        """
        附加伤害
        """
        return 0

    def _damageRate(self):
        """
        伤害加成
        """
        return 1

    def _appendDamage(self):
        """
        追加伤害
        """
        return 0

    def _floatRate(self):
        """
        波动修正
        """
        return 1 + random.randint(-self.F, self.F) / 100.0

    # endregion

    """占位符"""

