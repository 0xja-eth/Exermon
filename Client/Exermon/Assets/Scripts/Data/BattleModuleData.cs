using System;
using System.Collections.Generic;

using UnityEngine;

using LitJson;

using Core.Data;
using Core.Data.Loaders;

using Core.UI.Utils;

using GameModule.Data;
using GameModule.Services;

using ItemModule.Data;
using PlayerModule.Data;
using QuestionModule.Data;
using ExermonModule.Data;
using SeasonModule.Data;
using RecordModule.Data;

using QuestionModule.Services;
using PlayerModule.Services;

using UI.Common.Controls.ParamDisplays;
using UI.Common.Controls.RadarDisplay;

/// <summary>
/// 对战模块
/// </summary>
namespace BattleModule { }

/// <summary>
/// 对战模块数据
/// </summary>
namespace BattleModule.Data {

    #region 内存数据

    /// <summary>
    /// 评分数据
    /// </summary>
    public class ResultJudge : TypeData, IComparable<ResultJudge> {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int score { get; protected set; }
        [AutoConvert]
        public int win { get; protected set; }
        [AutoConvert]
        public int lose { get; protected set; }

        /// <summary>
        /// 比较函数
        /// </summary>
        /// <param name="b">另一实例</param>
        /// <returns>大小关系</returns>
        public int CompareTo(ResultJudge b) {
            return score.CompareTo(b.score);
        }
    }

    /// <summary>
    /// 对战记录
    /// </summary>
    public class BattleRecord : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int mode { get; protected set; }
        [AutoConvert]
        public int seasonId { get; protected set; }
        [AutoConvert]
        public DateTime createTime { get; protected set; }
        [AutoConvert]
        public DateTime resultTime { get; protected set; }
        [AutoConvert]
        public BattlePlayer[] players { get; protected set; }
        [AutoConvert]
        public BattleRound[] rounds { get; protected set; }

        /// <summary>
        /// 获取赛季对象
        /// </summary>
        /// <returns>返回赛季对象</returns>
        public CompSeason season() {
            return DataService.get().season(seasonId);
        }

        /// <summary>
        /// 获取赛季对象
        /// </summary>
        /// <returns>返回赛季对象</returns>
        public string mdoeText() {
            return DataService.get().battleMode(mode).Item2;
        }
    }

    /// <summary>
    /// 对战回合数据
    /// </summary>
    public class BattleRound : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int order { get; protected set; }
        [AutoConvert]
        public int subjectId { get; protected set; }
        [AutoConvert]
        public int starId { get; protected set; }
        [AutoConvert]
        public int questionId { get; protected set; }

        /// <summary>
        /// 是否读取
        /// </summary>
        public bool loaded { get; protected set; } = false;

        /// <summary>
        /// 科目对象
        /// </summary>
        /// <returns>返回本回合科目对象</returns>
        public Subject subject() {
            return DataService.get().subject(subjectId);
        }

        /// <summary>
        /// 题目星级对象
        /// </summary>
        /// <returns>返回本回合题目星级对象</returns>
        public QuesStar star() {
            return DataService.get().quesStar(starId);
        }

        /// <summary>
        /// 题目对象
        /// </summary>
        /// <returns>返回题目对象</returns>
        public Question question() {
            return QuestionService.get().getQuestion(questionId);
        }

        /// <summary>
        /// 读取自定义数据
        /// </summary>
        /// <param name="json">数据</param>
        protected override void loadCustomAttributes(JsonData json) {
            base.loadCustomAttributes(json);
            QuestionService.get().loadQuestions(
                new int[] { questionId }, () => loaded = true);
        }
    }

    /// <summary>
    /// 对战回合结果
    /// </summary>
    public class BattleRoundResult : PlayerQuestion {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int order { get; protected set; }
        [AutoConvert]
        public bool attack { get; protected set; }
        [AutoConvert]
        public int skillId { get; protected set; }
        [AutoConvert]
        public int targetType { get; protected set; }
        [AutoConvert]
        public int resultType { get; protected set; }
        [AutoConvert]
        public int hurt { get; protected set; }
        [AutoConvert]
        public int damage { get; protected set; }
        [AutoConvert]
        public int recovery { get; protected set; }

        /// <summary>
        /// 获取艾瑟萌技能
        /// </summary>
        /// <returns>返回艾瑟萌技能</returns>
        public ExerSkill skill() {
            return DataService.get().exerSkill(skillId);
        }

        /// <summary>
        /// 目标类型文本
        /// </summary>
        /// <returns>返回目标类型文本</returns>
        public string targetTypeText() {
            return DataService.get().exerSkillTargetType(targetType).Item2;
        }

        /// <summary>
        /// 命中结果类型文本
        /// </summary>
        /// <returns>返回命中结果类型文本</returns>
        public string resultTypeText() {
            return DataService.get().roundResultType(targetType).Item2;
        }
    }

    /// <summary>
    /// 对战玩家记录
    /// </summary>
    public class BattlePlayer : QuestionSetRecord
        <BattleRoundResult, QuestionSetReward>,
        RadarDiagram.IRadarDataConvertable {

        /// <summary>
        /// 对战玩家结果
        /// </summary>
        public enum Result {
            Win = 1, Lose = 2, Tie = 3
        }

        /// <summary>
        /// 结果颜色
        /// </summary>
        static readonly Color[] ResultColor = new Color[] {
            Color.white,
            new Color(1, 0.8078431f, 0.2627451f),
            RuntimeBattlePlayer.WrongColor,
            Color.white,
        };

        /// <summary>
        /// 结果文本
        /// </summary>
        static readonly string[] ResultText = new string[] {
            "", "胜利", "失败", "平局"
        };

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int scoreIncr { get; protected set; }
        [AutoConvert]
        public double timeScore { get; protected set; }
        [AutoConvert]
        public double hurtScore { get; protected set; }
        [AutoConvert]
        public double damageScore { get; protected set; }
        [AutoConvert]
        public double recoveryScore { get; protected set; }
        [AutoConvert]
        public double correctScore { get; protected set; }
        [AutoConvert]
        public double plusScore { get; protected set; }
        [AutoConvert]
        public int result { get; protected set; }
        [AutoConvert]
        public int status { get; protected set; }

        #region 数据转换

        /// <summary>
        /// 转化为显示数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override JsonData convertToDisplayData(string type = "") {
            switch(type) {
                case "score": return convertScoreData();
                default: return base.convertToDisplayData(type);
            }
        }

        /// <summary>
        /// 转换为统计数据
        /// </summary>
        /// <returns></returns>
        protected override JsonData convertResultData() {
            var res = base.convertResultData();

            res["result"] = ResultText[result];
            res["color"] = DataLoader.convert(ResultColor[result]);

            res["sum_hurt"] = sumHurt();
            res["sum_damage"] = sumDamage();
            res["sum_recovery"] = sumRecovery();

            res["score_incr"] = scoreIncr;
            res["star_incr"] = starIncr();

            return res;
        }

        /// <summary>
        /// 转换为分数数据
        /// </summary>
        /// <returns></returns>
        JsonData convertScoreData() {
            var res = new JsonData();

            res["time_score"] = timeScore;
            res["hurt_score"] = hurtScore;
            res["damage_score"] = damageScore;
            res["recovery_score"] = recoveryScore;
            res["correct_score"] = correctScore;
            res["plus_score"] = timeScore;
            res["sum_score"] = score();

            res["judge"] = judge().name;

            return res;
        }

        /// <summary>
        /// 转化为雷达数据
        /// </summary>
        /// <returns></returns>
        public List<float> convertToRadarData(string type = "") {
            var res = new List<float>();

            res.Add((float)timeScore / 100);
            res.Add((float)hurtScore / 100);
            res.Add((float)damageScore / 100);
            res.Add((float)recoveryScore / 100);
            res.Add((float)correctScore / 100);

            return res;
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 总分（缓存)
        /// </summary>
        double score_ = -1;

        /// <summary>
        /// 星星增量（缓存)
        /// </summary>
        int starIncr_ = -9999;

        /// <summary>
        /// 评价（缓存）
        /// </summary>
        ResultJudge judge_ = null;

        /// <summary>
        /// 最终评分
        /// </summary>
        /// <returns>返回最终评分</returns>
        public double score() {
            if (score_ < 0)
                score_ = (timeScore + hurtScore + damageScore + 
                    recoveryScore + correctScore) / 5 + plusScore;
            return score_;
        }

        /// <summary>
        /// 获取评价
        /// </summary>
        /// <returns>返回评价</returns>
        public ResultJudge judge() {
            if (judge_ == null) {
                var score = this.score();
                var judges = DataService.get().staticData.configure.resultJudges;
                judge_ = judges[0];
                foreach (var judge in judges) 
                    if (score >= judge_.score) judge_ = judge;
            }
            return judge_;
        }

        /// <summary>
        /// 获取星星奖励量
        /// </summary>
        /// <returns></returns>
        public int starIncr() {
            if (starIncr_ == -9999) {
                var judge = this.judge();
                var win = result == (int)Result.Win;
                starIncr_ = win ? judge.win : judge.lose;
            }
            return starIncr_;
        }
        
        #region 统计量

        /// <summary>
        /// 总伤害
        /// </summary>
        /// <returns></returns>
        public int sumHurt() {
            var res = 0;
            foreach (var round in questions)
                res += round.hurt;
            return res;
        }

        /// <summary>
        /// 总承伤
        /// </summary>
        /// <returns></returns>
        public int sumDamage() {
            var res = 0;
            foreach (var round in questions)
                res += round.damage;
            return res;
        }

        /// <summary>
        /// 总回复
        /// </summary>
        /// <returns></returns>
        public int sumRecovery() {
            var res = 0;
            foreach (var round in questions)
                res += round.recovery;
            return res;
        }

        #endregion

        /// <summary>
        /// 是否胜利
        /// </summary>
        /// <returns>返回是否胜利</returns>
        public bool isWon() {
            return result == (int)Result.Win;
        }

        /// <summary>
        /// 是否失败
        /// </summary>
        /// <returns>返回是否失败</returns>
        public bool isLost() {
            return result == (int)Result.Lose;
        }

        /// <summary>
        /// 是否平局
        /// </summary>
        /// <returns>返回是否平局</returns>
        public bool isTie() {
            return result == (int)Result.Tie;
        }

        /// <summary>
        /// 对战状态文本
        /// </summary>
        /// <returns>返回对战状态文本</returns>
        public string statusText() {
            return DataService.get().battleStatus(status).Item2;
        }

        #endregion

        #region 数据读取



        #endregion
    }

    /// <summary>
    /// 对战物资槽
    /// </summary>
    public class BattleItemSlot : SlotContainer<BattleItemSlotItem> {

        /// <summary>
        /// 玩家
        /// </summary>
        public Player player { get; set; }

        /// <summary>
        /// 通过装备物品获取槽项
        /// </summary>
        /// <typeparam name="E">装备物品类型</typeparam>
        /// <param name="equipItem">装备物品</param>
        /// <returns>槽项</returns>
        public override BattleItemSlotItem getSlotItemByEquipItem<E>(E equipItem) {
            return emptySlotItem();
        }

        /// <summary>
        /// 读取单个物品
        /// </summary>
        /// <param name="json">数据</param>
        protected override BattleItemSlotItem loadItem(JsonData json) {
            var slotItem = base.loadItem(json);
            slotItem.equipSlot = this;
            return slotItem;
        }

    }

    /// <summary>
    /// 对战物资槽项
    /// </summary>
    public class BattleItemSlotItem : SlotContItem<HumanPackItem> {
        
        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int packItemId { get; protected set; }

        /// <summary>
        /// 艾瑟萌背包装备实例
        /// </summary>
        public HumanPackItem packItem {
            get {
                var humanPack = equipSlot.player.
                    packContainers.humanPack;
                return humanPack.getItem<HumanPackItem>(
                    item => item.getID() == packItemId);
            }
            set {
                packItemId = value.getID();
            }
        }

        /// <summary>
        /// 装备
        /// </summary>
        public override HumanPackItem equip1 {
            get { return packItem; }
            protected set { packItem = value; }
        }

        /// <summary>
        /// 装备槽
        /// </summary>
        public BattleItemSlot equipSlot { get; set; }

        /// <summary>
        /// 获取装备类型
        /// </summary>
        /// <returns>装备类型</returns>
        public HumanItem item() {
            return packItem == null ? null : packItem.item();
        }

    }

    #endregion

    #region 运行时数据

    /// <summary>
    /// 对战艾瑟萌BUFF
    /// </summary>
    public class RuntimeBattleBuff : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public ParamData[] plusParams { get; protected set; } = new ParamData[] { };
        [AutoConvert]
        public ParamRateData[] rateParams { get; protected set; } = new ParamRateData[] { };
        [AutoConvert]
        public int round { get; protected set; }

    }

    /// <summary>
    /// 对战艾瑟萌技能
    /// </summary>
    public class RuntimeBattleExerSkill : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int useCount { get; protected set; }
        [AutoConvert]
        public int skillId { get; protected set; }
        [AutoConvert]
        public int freezeRound { get; protected set; }

        /// <summary>
        /// 所属艾瑟萌
        /// </summary>
        RuntimeBattleExermon exermon = null;

        /// <summary>
        /// 设置所属艾瑟萌
        /// </summary>
        /// <param name="exermon">艾瑟萌</param>
        public void setExermon(RuntimeBattleExermon exermon) {
            this.exermon = exermon;
        }

        /// <summary>
        /// 艾瑟萌技能实例
        /// </summary>
        /// <returns>返回艾瑟萌技能实例</returns>
        public ExerSkill skill() {
            return DataService.get().exerSkill(skillId);
        }
    }

    /// <summary>
    /// 对战艾瑟萌状态数据
    /// </summary>
    public class RuntimeBattleExermon : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int subjectId { get; protected set; }
        [AutoConvert]
        public int exermonId { get; protected set; }
        [AutoConvert]
        public int mp { get; protected set; }
        [AutoConvert("params")]
        public ParamData[] params_ { get; protected set; } = new ParamData[] { };
        [AutoConvert("params")]
        public RuntimeBattleBuff[] buffs { get; protected set; } = new RuntimeBattleBuff[] { };
        [AutoConvert("params")]
        public RuntimeBattleExerSkill[] skills { get; protected set; } = new RuntimeBattleExerSkill[] { };

        /// <summary>
        /// 对战玩家
        /// </summary>
        RuntimeBattlePlayer player = null;

        /// <summary>
        /// 设置对战玩家
        /// </summary>
        /// <param name="player">对战玩家</param>
        public void setPlayer(RuntimeBattlePlayer player) {
            this.player = player;
        }

        /// <summary>
        /// 读取自定义属性
        /// </summary>
        /// <param name="json">数据</param>
        protected override void loadCustomAttributes(JsonData json) {
            base.loadCustomAttributes(json);
            foreach (var skill in skills)
                skill.setExermon(this);
        }

        /// <summary>
        /// 获取艾瑟萌
        /// </summary>
        /// <returns>返回艾瑟萌实例</returns>
        public Exermon exermon() {
            return DataService.get().exermon(exermonId);
        }

        /// <summary>
        /// 修改MP
        /// </summary>
        /// <param name="value">MP</param>
        public void changeMP(int value) {
            var mmp = (int)Math.Round(param(1).value);
            mp = Mathf.Clamp(mp + value, 0, mmp);
        }

        /// <summary>
        /// 修改MP（百分比）
        /// </summary>
        /// <param name="rate">MP百分比</param>
        public void changePercentMP(double rate) {
            var mmp = param(1).value;
            changeMP((int)Math.Round(mmp * rate));
        }
        
        /// <summary>
        /// 添加属性值
        /// </summary>
        /// <param name="paramId">属性ID</param>
        /// <param name="value">属性值</param>
        public void changeParam(int paramId, double value) {
            var param = this.param(paramId);
            param += value;
        }

        /// <summary>
        /// 添加属性值（百分比）
        /// </summary>
        /// <param name="paramId">属性ID</param>
        /// <param name="rate">属性值（百分比）</param>
        public void changePercentParam(int paramId, double rate) {
            var param = this.param(paramId);
            param *= rate;
        }

        /// <summary>
        /// 获取属性基础值
        /// </summary>
        /// <param name="paramId">属性ID</param>
        /// <returns>属性数据</returns>
        public ParamData param(int paramId) {
            foreach (var param in params_)
                if (param.paramId == paramId) return param;
            return new ParamData(paramId);
        }

        /// <summary>
        /// 获取对应的艾瑟萌槽项
        /// </summary>
        /// <returns>返回艾瑟萌槽项</returns>
        public ExerSlotItem exerSlotItem() {
            var player = PlayerService.get().player;
            if (player == null ||
                this.player.getID() != player.getID()) return null;
            return player.slotContainers.exerSlot.getSlotItem(subjectId);
        }

        /// <summary>
        /// 获取艾瑟萌技能槽
        /// </summary>
        /// <returns>返回技能槽</returns>
        public ExerSkillSlot exerSkillSlot() {
            var slotItem = exerSlotItem();
            if (slotItem == null) return null;
            return slotItem.playerExer.exerSkillSlot;
        }
    }

    /// <summary>
    /// 对战艾瑟萌BUFF
    /// </summary>
    public class RuntimeBattleItem : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int itemId { get; protected set; }
        [AutoConvert]
        public int freezeRound { get; protected set; }

        /// <summary>
        /// 对战玩家
        /// </summary>
        RuntimeBattlePlayer player = null;

        /// <summary>
        /// 设置对战玩家
        /// </summary>
        /// <param name="player">对战玩家</param>
        public void setPlayer(RuntimeBattlePlayer player) {
            this.player = player;
        }

        /// <summary>
        /// 获取物品
        /// </summary>
        /// <returns>返回物品对象</returns>
        public HumanItem item() {
            return DataService.get().humanItem(itemId);
        }

        /// <summary>
        /// 获取对应的对战物资槽物品
        /// </summary>
        /// <returns>当所属玩家为当前玩家时返回其物资槽对象，否则返回 null</returns>
        public BattleItemSlotItem battleItemSlotItem() {
            var player = PlayerService.get().player;
            if (player == null || 
                this.player.getID() != player.getID()) return null;
            return player.slotContainers.battleItemSlot.getSlotItem(getID());
        }
    }

    /// <summary>
    /// 行动数据
    /// </summary>
    public class RuntimeAction : BaseData {

        /// <summary>
        /// 行动类型
        /// </summary>
        public enum Type {
            Prepare = 0, Attack = 1
        }

        /// <summary>
        /// 默认攻击类型
        /// </summary>
        const ExerSkill.HitType DefaultHitType = ExerSkill.HitType.HPDamage;

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int type { get; protected set; }

        [AutoConvert]
        public int itemType { get; protected set; }
        [AutoConvert]
        public int itemId { get; protected set; }

        [AutoConvert]
        public int skillId { get; protected set; }
        [AutoConvert]
        public int targetType { get; protected set; }
        [AutoConvert]
        public int resultType { get; protected set; }
        [AutoConvert]
        public int hurt { get; protected set; }

        /// <summary>
        /// 对战玩家
        /// </summary>
        public RuntimeBattlePlayer player { get; protected set; } = null;

        /// <summary>
        /// 设置对战玩家
        /// </summary>
        /// <param name="player">对战玩家</param>
        public void setPlayer(RuntimeBattlePlayer player) {
            this.player = player;
        }

        /// <summary>
        /// 获取使用的物品
        /// </summary>
        /// <returns>返回使用物品</returns>
        public BaseItem item() {
            if (type != (int)Type.Prepare) return null;
            if (itemId <= 0) return null;
            switch (itemType) {
                case (int)BaseItem.Type.HumanItem:
                    return DataService.get().humanItem(itemId);
                case (int)BaseItem.Type.QuesSugar:
                    return DataService.get().quesSugar(itemId);
                default: return null;
            }
        }

        /// <summary>
        /// 获取使用的艾瑟萌技能
        /// </summary>
        /// <returns>返回使用的技能</returns>
        public ExerSkill skill() {
            if (type != (int)Type.Attack) return null;
            return DataService.get().exerSkill(skillId);
        }

        /// <summary>
        /// 命中类型
        /// </summary>
        /// <returns></returns>
        public int hitType() {
            var skill = this.skill();
            if (skill == null) return (int)DefaultHitType;
            return skill.hitType;
        }
    }

    /// <summary>
    /// 对战玩家数据（回合结果时进行数据同步）
    /// </summary>
    public class RuntimeBattlePlayer : BaseData, 
        IQuestionResult, 
        ParamDisplay.IDisplayDataConvertable,
        IItemUseTarget {

        /// <summary>
        /// 常量设置
        /// </summary>
        const string CorrectText = "AC";
        const string WrongText = "WA";

        public static readonly Color CorrectColor = new Color(0.5450981f, 0.9647059f, 1);
        public static readonly Color WrongColor = new Color(1, 0.1647059f, 0.3921569f);

        /// <summary>
        /// 基本数据
        /// </summary>
        [AutoConvert]
        public string channelName { get; protected set; }
        [AutoConvert]
        public string name { get; protected set; }
        [AutoConvert]
        public int characterId { get; protected set; }
        [AutoConvert]
        public int level { get; protected set; }
        [AutoConvert]
        public int rankId { get; protected set; }
        [AutoConvert]
        public int subRank { get; protected set; }
        [AutoConvert]
        public int starNum { get; protected set; }
        /*
        [AutoConvert]
        public ExerSlot exerSlot { get; protected set; } = new ExerSlot();
        [AutoConvert]
        public BattleItemSlot battleItemSlot { get; protected set; } = new BattleItemSlot();
        */

        /// <summary>
        /// 每回合更新的状态数据
        /// </summary>
        [AutoConvert]
        public int hp { get; protected set; }
        [AutoConvert]
        public int mhp { get; protected set; }
        [AutoConvert]
        public RuntimeBattleExermon[] exermons { get; protected set; } = new RuntimeBattleExermon[] { };
        [AutoConvert]
        public RuntimeBattleItem[] battleItems { get; protected set; } = new RuntimeBattleItem[] { };

        /// <summary>
        /// 上一回合结果数据
        /// </summary>
        [AutoConvert]
        public int order { get; protected set; }
        [AutoConvert]
        public int hurt { get; protected set; }
        [AutoConvert]
        public int damage { get; protected set; }
        [AutoConvert]
        public int recover { get; protected set; }
        [AutoConvert]
        public bool correct { get; protected set; }
        [AutoConvert]
        public int[] selection { get; protected set; } = null;
        [AutoConvert]
        public int timespan { get; protected set; }

        /// <summary>
        /// 对战实例
        /// </summary>
        public RuntimeBattle battle { get; set; } = null;

        /// <summary>
        /// 进度
        /// </summary>
        public int progress { get; protected set; } = -1;

        /// <summary>
        /// 行动数据
        /// </summary>
        public RuntimeAction[] actions { get; protected set; } = null;

        /// <summary>
        /// 结果数据
        /// </summary>
        public BattlePlayer result { get; protected set; }

        #region 数据转换

        /// <summary>
        /// 转换为显示数据
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public JsonData convertToDisplayData(string type = "") {
            switch(type) {
                case "base_status": return convertBaseStatusData();
                case "round_result": return convertRoundResultData();
                default: return toJson();
            }
        }

        /// <summary>
        /// 转化基本状态数据
        /// </summary>
        /// <returns>返回数据</returns>
        JsonData convertBaseStatusData() {
            var res = new JsonData();
            var exermon = currentExermon();
            if (exermon != null) {
                var mp = exermon.mp;
                var mmp = exermon.param(1).value;
                res["mp"] = string.Format("{0}/{1}", mp, mmp);
                res["mp_rate"] = mp * 1.0 / mmp;
            }

            res["name"] = name;
            res["hp"] = string.Format("{0}/{1}", hp, mhp);
            res["hp_rate"] = hp * 1.0 / mhp;

            return res;
        }

        /// <summary>
        /// 转化回合结算数据
        /// </summary>
        /// <returns>返回数据</returns>
        JsonData convertRoundResultData() {
            var res = new JsonData();
            var color = correct ? CorrectColor : WrongColor;
            var time = DataLoader.convertDouble(timespan / 1000.0);

            res["timespan"] = time;
            res["hurt"] = hurt;
            res["damage"] = damage;
            res["recover"] = recover;
            res["color"] = DataLoader.convert(color);
            res["result"] = correct ? CorrectText : WrongText;
            res["rest_hp"] = string.Format("{0}/{1}", hp, mhp);
            res["name"] = name;

            return res;
        }

        #endregion

        #region 数据操作

        /// <summary>
        /// 形象对象
        /// </summary>
        /// <returns>返回形象对象</returns>
        public Character character() {
            return DataService.get().character(characterId);
        }

        /// <summary>
        /// 段位对象
        /// </summary>
        /// <returns>返回段位对象</returns>
        public CompRank rank() {
            return DataService.get().compRank(rankId);
        }

        /// <summary>
        /// 获取当前回合
        /// </summary>
        /// <returns>返回当前对战回合</returns>
        public BattleRound currentRound() {
            return battle.round;
        }

        /// <summary>
        /// 获取当前回合
        /// </summary>
        /// <returns>返回当前对战回合</returns>
        public RuntimeBattleExermon currentExermon() {
            var round = battle.round;
            return exermon(round.subjectId);
        }

        /// <summary>
        /// 获取指定科目的艾瑟萌
        /// </summary>
        /// <param name="subjectId">科目ID</param>
        /// <returns>返回指定科目的为什么</returns>
        public RuntimeBattleExermon exermon(int subjectId) {
            foreach (var exermon in exermons)
                if (exermon.subjectId == subjectId)
                    return exermon;
            return null;
        }

        /// <summary>
        /// 清除回合状态数据
        /// </summary>
        public void clearRoundStatus() {
            selection = null;
            correct = false;
            timespan = 0;
            actions = null;
        }

        /// <summary>
        /// 获取对方玩家
        /// </summary>
        /// <returns></returns>
        public RuntimeBattlePlayer getOppo() {
            if (battle == null) return null;
            return battle.getOppo(this);
        }

        /// <summary>
        /// 当前选择
        /// </summary>
        /// <returns>返回当前选择</returns>
        public int[] getSelection() { return selection; }

        /// <summary>
        /// 设置选择
        /// </summary>
        public void setSelection(int[] selection) {
            this.selection = selection;
        }

        /// <summary>
        /// 做题用时（毫秒）
        /// </summary>
        /// <returns>返回做题用时</returns>
        public int getTimespan() { return timespan; }

        /// <summary>
        /// 当前选择
        /// </summary>
        /// <returns>返回当前选择</returns>
        public bool isAnswered() { return selection != null; }

        /// <summary>
        /// 使用的物品
        /// </summary>
        /// <returns></returns>
        public BaseItem roundItem() {
            var action = perpareAction();
            if (action == null) return null;
            return action.item();
        }

        /// <summary>
        /// 准备行动
        /// </summary>
        /// <returns>返回当前回合的准备行动</returns>
        public RuntimeAction perpareAction() {
            if (actions == null) return null;
            foreach (var action in actions)
                if (action.type == (int)RuntimeAction.Type.Prepare)
                    return action;
            return null;
        }

        /// <summary>
        /// 攻击行动
        /// </summary>
        /// <returns>返回当前回合的准备行动</returns>
        public RuntimeAction attackAction() {
            if (actions == null) return null;
            foreach (var action in actions)
                if (action.type == (int)RuntimeAction.Type.Attack)
                    return action;
            return null;
        }

        #endregion

        #region 数据读取

        /// <summary>
        /// 读取自定义属性
        /// </summary>
        /// <param name="json">数据</param>
        protected override void loadCustomAttributes(JsonData json) {
            base.loadCustomAttributes(json);
            foreach (var exermon in exermons)
                exermon.setPlayer(this);
            foreach (var battleItem in battleItems)
                battleItem.setPlayer(this);
        }

        /// <summary>
        /// 读取进度
        /// </summary>
        /// <param name="progress">进度</param>
        public void loadProgress(int progress) {
            this.progress = progress;
        }

        /// <summary>
        /// 加载答作答
        /// </summary>
        /// <param name="correct">是否正确</param>
        /// <param name="timespan">用时</param>
        public void loadAnswer(bool correct, int timespan) {
            this.correct = correct;
            this.timespan = timespan;
            if (selection == null) selection = new int[0];
        }

        /// <summary>
        /// 加载行动
        /// </summary>
        /// <param name="json">数据</param>
        public void loadActions(JsonData json) {
            actions = DataLoader.load<RuntimeAction[]>(json);
            foreach (var action in actions) action.setPlayer(this);
        }

        /// <summary>
        /// 加载结果
        /// </summary>
        /// <param name="json">数据</param>
        public void loadResult(JsonData json) {
            result = DataLoader.load<BattlePlayer>(json);
        }

        #endregion

        #region 实现接口
        
        /// <summary>
        /// 修改HP
        /// </summary>
        /// <param name="value">HP</param>
        public void changeHP(int value) {
            hp = Mathf.Clamp(hp + value, 0, mhp);
        }

        /// <summary>
        /// 修改HP（百分比）
        /// </summary>
        /// <param name="hp">HP百分比</param>
        public void changePercentHP(double rate) {
            changeHP((int)Math.Round(mhp * rate));
        }

        /// <summary>
        /// 修改MP
        /// </summary>
        /// <param name="value">MP</param>
        public void changeMP(int value) {
            var exermon = currentExermon();
            if (exermon == null) return;
            exermon.changeMP(value);
        }

        /// <summary>
        /// 修改MP（百分比）
        /// </summary>
        /// <param name="rate">MP百分比</param>
        public void changePercentMP(double rate) {
            var exermon = currentExermon();
            if (exermon == null) return;
            exermon.changePercentMP(rate);
        }

        /// <summary>
        /// 添加属性值
        /// </summary>
        /// <param name="paramId">属性ID</param>
        /// <param name="value">属性值</param>
        public void changeParam(int paramId, double value) {
            var exermon = currentExermon();
            if (exermon == null) return;
            exermon.changeParam(paramId, value);
        }

        /// <summary>
        /// 添加属性值（百分比）
        /// </summary>
        /// <param name="paramId">属性ID</param>
        /// <param name="rate">属性值（百分比）</param>
        public void changePercentParam(int paramId, double rate) {
            var exermon = currentExermon();
            if (exermon == null) return;
            exermon.changePercentParam(paramId, rate);
        }

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="player">玩家</param>
        public RuntimeBattlePlayer() {}

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="player">玩家</param>
        public RuntimeBattlePlayer(Player player) {
            name = player.name;
            characterId = player.characterId;
            level = player.level;
            starNum = player.battleInfo.starNum;
        }
    }

    /// <summary>
    /// 运行时对战
    /// </summary>
    public class RuntimeBattle : BaseData {

        /// <summary>
        /// 玩家1
        /// </summary>
        [AutoConvert]
        public RuntimeBattlePlayer player1 { get; protected set; } = new RuntimeBattlePlayer();
        /// <summary>
        /// 玩家2
        /// </summary>
        [AutoConvert]
        public RuntimeBattlePlayer player2 { get; protected set; } = new RuntimeBattlePlayer();

        /// <summary>
        /// 当前回合
        /// </summary>
        [AutoConvert]
        public BattleRound round { get; protected set; } = new BattleRound();

        /// <summary>
        /// 上次更新的玩家
        /// </summary>
        public RuntimeBattlePlayer lastPlayer { get; protected set; } = null;

        /// <summary>
        /// 获取自身玩家运行时数据
        /// </summary>
        /// <returns>返回自身运行时数据</returns>
        public RuntimeBattlePlayer self() {
            return getPlayer(PlayerService.get().player.getID());
        }

        /// <summary>
        /// 获取对方玩家运行时数据
        /// </summary>
        /// <returns>返回对方运行时数据</returns>
        public RuntimeBattlePlayer oppo() {
            return getPlayer(PlayerService.get().player.getID(), true);
        }

        /// <summary>
        /// 获取运行时玩家
        /// </summary>
        /// <param name="pid">玩家ID</param>
        /// <param name="oppo">获取对方玩家</param>
        /// <returns>返回对应的运行时玩家</returns>
        public RuntimeBattlePlayer getPlayer(int pid, bool oppo = false) {
            if (player1.getID() == pid) return oppo ? player2 : player1;
            if (player2.getID() == pid) return oppo ? player1 : player2;
            return null;
        }

        /// <summary>
        /// 获取对方玩家
        /// </summary>
        /// <param name="player">当前玩家</param>
        /// <returns>返回当前玩家的对方玩家</returns>
        public RuntimeBattlePlayer getOppo(RuntimeBattlePlayer player) {
            if (player1 == player) return player2;
            if (player2 == player) return player1;
            return null;
        }

        /// <summary>
        /// 是否匹配完成
        /// </summary>
        /// <returns>返回当前是否匹配完成</returns>
        public bool isMatchingCompleted() {
            return player1.progress == 100 && player2.progress == 100;
        }

        /// <summary>
        /// 答题阶段是否结束
        /// </summary>
        /// <returns>返回答题阶段是否结束</returns>
        public bool isQuestCompleted() {
            return player1.correct || player2.correct ||
                (player1.isAnswered() && player2.isAnswered());
        }

        /// <summary>
        /// 新回合
        /// </summary>
        public void onNewRound() {
            lastPlayer = null;
            player1.clearRoundStatus();
            player2.clearRoundStatus();
        }

        /// <summary>
        /// 返回当前回合的攻击行动
        /// </summary>
        /// <returns></returns>
        public RuntimeAction attackAction() {
            return player1.attackAction() ?? player2.attackAction();
        }

        #region 数据读取

        /// <summary>
        /// 读取匹配进度
        /// </summary>
        /// <param name="pid">玩家ID</param>
        /// <param name="progress">进度</param>
        public void loadProgress(int pid, int progress) {
            var player = getPlayer(pid);
            player.loadProgress(progress);
        }

        /// <summary>
        /// 读取匹配进度
        /// </summary>
        /// <param name="pid">玩家ID</param>
        /// <param name="correct">是否正确</param>
        /// <param name="timespan">用时</param>
        public void loadAnswer(int pid, bool correct, int timespan) {
            lastPlayer = getPlayer(pid);
            lastPlayer.loadAnswer(correct, timespan);

            if (correct) { // 如果是某方作答正确
                var oppo = getPlayer(pid, true);
                if (oppo != null && !oppo.isAnswered()) // 对方没有作答
                    oppo.loadAnswer(false, 0); // 直接设置其作答
            }
        }

        /// <summary>
        /// 加载行动
        /// </summary>
        /// <param name="json">数据</param>
        public void loadActions(JsonData json) {
            var actions1 = DataLoader.load(json, "actions1");
            var actions2 = DataLoader.load(json, "actions2");
            player1.loadActions(actions1);
            player2.loadActions(actions2);
        }

        /// <summary>
        /// 加载对战结果
        /// </summary>
        /// <param name="json">数据</param>
        public void loadResults(JsonData json) {
            var result1 = DataLoader.load(json, "player1");
            var result2 = DataLoader.load(json, "player2");
            player1.loadResult(result1);
            player2.loadResult(result2);
        }

        /// <summary>
        /// 读取自定义属性
        /// </summary>
        /// <param name="json"></param>
        protected override void loadCustomAttributes(JsonData json) {
            base.loadCustomAttributes(json);
            player1.battle = player2.battle = this;
        }

        #endregion

    }

    #endregion

}