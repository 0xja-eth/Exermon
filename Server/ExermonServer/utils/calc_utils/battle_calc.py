from .common import Common
from .question_calc import ExerciseResultRewardCalc

import random


# ===================================================
# 对战对战结果收益计算类
# ===================================================
class BattleResultRewardCalc(ExerciseResultRewardCalc):
    EXER_EXP_RATE = 0.75
    SLOT_EXP_RATE = 0.75
    GOLD_RATE = 0.5

    CREDIT_REWARD = 5
    CREDIT_PUNISH = [10, 20]

    # @classmethod
    # def process(cls, battle):
    # 	processor = cls(battle)
    # 	processor._judgeWinner()
    # 	processor._processBattler(processor.battle_player1)
    # 	processor._processBattler(processor.battle_player2)

    def __init__(self, battle_player, player_queses, battle):
        # battle_player 即 question_set
        super().__init__(battle_player, player_queses)

        from battle_module.runtimes import RuntimeBattle, RuntimeBattlePlayer
        from battle_module.models import BattleRecord, BattlePlayer

        self.battle: RuntimeBattle = battle

        self.battle_record: BattleRecord = battle.record

        self.self_battle_player: BattlePlayer = \
            self.battle_record.getBattlePlayer(battle_player=battle_player)
        self.oppo_battle_player: BattlePlayer = \
            self.battle_record.getOppoBattlePlayer(battle_player=battle_player)

        self.self_runtime_battler: RuntimeBattlePlayer = \
            self.battle.getBattler(battle_player=battle_player)
        self.oppo_runtime_battler: RuntimeBattlePlayer = \
            self.battle.getOppoBattler(battle_player=battle_player)

        self.result = self._calcResult()
        self.status = self._calcStatus()

        self.credit_incr = self._calcCreditIncr()

        self.battle_scores, self.score_incr = self._calcScores()

        self.judge, self.star_incr = self._calcJudgeAndStarIncr()

        print("self.judge:" + str(self.judge))

    # 艾瑟萌经验奖励加成
    def _exerExpRewardRate(self, corr_rate):
        return super()._exerExpRewardRate(corr_rate) * self.EXER_EXP_RATE

    # 艾瑟萌槽经验奖励加成
    def _slotExpRewardRate(self, corr_rate):
        return super()._slotExpRewardRate(corr_rate) * self.SLOT_EXP_RATE

    # 金币奖励加成
    def _goldRewardRate(self, corr_rate):
        return super()._goldRewardRate(corr_rate) * self.GOLD_RATE

    # 计算对战结果
    def _calcResult(self):
        from battle_module.models import BattlePlayerResult

        if self.self_runtime_battler.isDead():
            return BattlePlayerResult.Lose

        if self.oppo_runtime_battler.isDead():
            return BattlePlayerResult.Win

        return BattlePlayerResult.Tie

    # 计算对战状态
    def _calcStatus(self):
        from battle_module.models import BattlePlayerStatus

        return BattlePlayerStatus.Normal

    # 计算信誉积分奖励
    def _calcCreditIncr(self):
        from battle_module.models import BattlePlayerStatus

        if self.status == BattlePlayerStatus.Normal:
            return self.CREDIT_REWARD

        if self.status == BattlePlayerStatus.Cancelled:
            return random.randint(*self.CREDIT_PUNISH)

        return 0

    # 计算赛季积分奖励
    def _calcScores(self):

        battle_scores = BattleScoreCalc.calc(self.self_battle_player,
                                             self.self_runtime_battler,
                                             self.oppo_runtime_battler)

        score_incr = BattleRankScoreCalc.calc(battle_scores.sumScore(),
                                              self.self_battle_player,
                                              self.oppo_battle_player)

        return battle_scores, score_incr

    def __battleScore(self):
        return self.battle_scores.sumScore()

    # 计算评价结果及星星奖励
    def _calcJudgeAndStarIncr(self):
        from battle_module.models import BattleResultJudge, BattlePlayerResult

        score = self.__battleScore()
        judges = BattleResultJudge.objs()

        battle_judge: BattleResultJudge = None

        for judge in judges:
            if score >= judge.score:
                battle_judge = judge
            else:
                break

        if self.result == BattlePlayerResult.Win:
            return battle_judge, battle_judge.win

        return battle_judge, battle_judge.lose


# ===================================================
# 对战积分计算类
# ===================================================
class BattleScoreCalc:
    # 对战评分(BS)	(用时分 数 +伤害分 数 +承伤分 数 +回复分 数 +行动分数 ) / 5 +奖励分数
    # 用时分数(TS)	 -100 * (AT / ST) * (AT / ST) + 100
    # 伤害分数(HS)
    # 伤害率 > PX ? 高伤害分数: 常规伤害分数
    # 承伤分数(DS)
    # 100 - (承伤率 > 100 ? 高承伤分数: 常规承伤分数)
    # 回复分数(RS)	(回复 率= =0 ? 1 : 1- sigmoid((回复率 - MX) / K4))*100
    # 行动分数(AS)	(- 1 /100 ) *行动 率 *(行动 率 -200)
    # 奖励分数(PS)		0（用于拓展）
    # 高伤害分数		P Y +K 1 *(伤害 率 -PX)
    # 常规伤害分数	 A* 伤害率 * 伤害率
    # 高承伤分数
    # K2 * 100 + K3 * (承伤率 - 100)
    # 常规承伤分数
    # 承伤率 * K2
    # 伤害率(HR)
    # AH / BMHP * 100
    # 承伤率(DR)
    # AD / AMHP * 100
    # 回复率(RR)
    # AR / AMHP * 100
    # 行动率(AR)
    # AA / CNT * 100

    PY = 85
    PX = 80

    A = PY / PX / PX

    K1 = 0.25
    K2 = 0.333
    K3 = 0.233

    MX = 100
    K4 = 20

    @classmethod
    def calc(cls, battler, self_battler, oppo_battler):
        calc_obj = cls(battler, self_battler, oppo_battler)
        return calc_obj

    def __init__(self, battler, self_battler, oppo_battler):
        from battle_module.models import BattlePlayer
        from battle_module.runtimes import RuntimeBattlePlayer

        self.battler: BattlePlayer = battler

        self.self_battler: RuntimeBattlePlayer = self_battler
        self.oppo_battler: RuntimeBattlePlayer = oppo_battler

        self._calc()

    def _calc(self):
        self.time_score = self._clamp(self._calcTimeScore())
        self.hurt_score = self._clamp(self._calcHurtScore())
        self.damage_score = self._clamp(self._calcDamageScore())
        self.recovery_score = self._clamp(self._calcRecoveryScore())
        self.correct_score = self._clamp(self._calcCorrectScore())
        self.plus_score = self._clamp(self._calcPlusScore())

    def _clamp(self, val):
        return max(min(val, 100), 0)

    # 计算时间分数
    def _calcTimeScore(self):
        at = self.battler.sumTimespan()
        st = self.battler.sumStdTime() * 1000

        return -100 * (at / st) * (at / st) + 100

    # 计算伤害分数
    def _calcHurtScore(self):
        hr = self.__calcHurtRate()
        return self.__highHurtScore(hr) \
            if hr > self.PX else self.__stdHurtScore(hr)

    def __calcHurtRate(self):
        ah = self.battler.sumHurt()
        bmhp = self.oppo_battler.mhp
        return ah / bmhp * 100  # 伤害率

    def __highHurtScore(self, hr):
        return self.PY + self.K1 * (hr - self.PX)

    def __stdHurtScore(self, hr):
        return self.A * hr * hr

    # 计算承伤分数
    def _calcDamageScore(self):
        dr = self.__calcDamageRate()
        return 100 - (self.__highDamageScore(dr)
                      if dr > 100 else self.__stdDamageScore(dr))

    def __calcDamageRate(self):
        ad = self.battler.sumDamage()
        amhp = self.self_battler.mhp
        return ad / amhp * 100  # 承伤率

    def __highDamageScore(self, dr):
        return self.K2 * 100 + self.K3 * (dr - 100)

    def __stdDamageScore(self, dr):
        return dr * self.K2

    # 计算回复分数
    def _calcRecoveryScore(self):
        ar = self.__calcRecoveryRate()
        if ar <= 0: return 100
        return (1 - Common.sigmoid((ar - self.MX) / self.K4)) * 100

    def __calcRecoveryRate(self):
        ar = self.battler.sumRecovery()
        amhp = self.self_battler.mhp
        return ar / amhp * 100  # 承伤率

    # 计算正确分数
    def _calcCorrectScore(self):
        cr = self.battler.corrRate() * 100
        return -0.01 * cr * (cr - 200)

    # 计算奖励分数
    def _calcPlusScore(self):
        return 0

    # 总分
    def sumScore(self):
        return (self.time_score + self.hurt_score +
                self.damage_score + self.recovery_score +
                self.correct_score) / 5 + self.plus_score


# ===================================================
# 对战赛季积分计算类
# ===================================================
class BattleRankScoreCalc:

    # 赛季积分奖励(RS)
    # round(对战评分 / (max(50, ARF + ARF - BRF) / 100)) - 50

    @classmethod
    def calc(cls, sum_score, battler1, battler2):
        calc_obj = cls(sum_score, battler1, battler2)
        return calc_obj.value

    def __init__(self, sum_score, battler1, battler2):
        from battle_module.models import BattlePlayer
        from season_module.models import SeasonRecord
        from player_module.models import Player
        from player_module.views import Common as PlayerCommon

        self.battler1: BattlePlayer = battler1
        self.battler2: BattlePlayer = battler2

        self.sum_score: BattleScoreCalc = sum_score

        online_player = PlayerCommon.getOnlinePlayer(self.battler1.player_id)

        if online_player is not None:
            self.player1: Player = online_player.player
        else:
            self.player1: Player = self.battler1.player

        online_player = PlayerCommon.getOnlinePlayer(self.battler2.player_id)

        if online_player is not None:
            self.player2: Player = online_player.player
        else:
            self.player2: Player = self.battler2.player

        self.season_record1: SeasonRecord = self.player1.currentSeasonRecord()
        self.season_record2: SeasonRecord = self.player2.currentSeasonRecord()

        self.value = self._calc()

    def _calc(self):
        from season_module.models import CompRank

        rank1, _, _ = self.season_record1.rank()
        rank2, _, _ = self.season_record2.rank()

        rank1: CompRank = rank1
        rank2: CompRank = rank2

        arf = rank1.score_factor
        brf = rank2.score_factor

        return round(self.sum_score / (max(50, arf + arf - brf) / 100)) - 50

