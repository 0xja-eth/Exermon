using System;

using LitJson;

using Core.Data;
using Core.Data.Loaders;

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
    /// 对战玩家记录
    /// </summary>
    public class BattlePlayer : QuestionSetRecord {

        /// <summary>
        /// 对战玩家结果
        /// </summary>
        public enum Result {
            Win = 1, Lose = 2, Tie = 3
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
            public int recover { get; protected set; }

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
        public double recoverScore { get; protected set; }
        [AutoConvert]
        public double correctScore { get; protected set; }
        [AutoConvert]
        public double plusScore { get; protected set; }
        [AutoConvert]
        public int result { get; protected set; }
        [AutoConvert]
        public int status { get; protected set; }

        /// <summary>
        /// 总分（缓存)
        /// </summary>
        double score_ = -1;

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
                    recoverScore + correctScore) / 5 + plusScore;
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
            if (exermon == null) return null;
            var skillSlotItem = exermon.exerSkillSlot().getSlotItem(getID());
            return skillSlotItem.skill();
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
        /// 获取对应的艾瑟萌槽项
        /// </summary>
        /// <returns>返回艾瑟萌槽项</returns>
        public ExerSlotItem exerSlotItem() {
            if (player == null) return null;
            return player.exerSlot.getSlotItem(subjectId);
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
        /// 获取对应的对战物资槽物品
        /// </summary>
        /// <returns></returns>
        public BattleItemSlotItem battleItemSlotItem() {
            if (player == null) return null;
            return player.battleItemSlot.getSlotItem(getID());
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
            Prepare = 0, Action = 1
        }

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
            if (type != (int)Type.Prepare) return null;
            return DataService.get().exerSkill(skillId);
        }
    }

    /// <summary>
    /// 对战玩家数据（回合结果时进行数据同步）
    /// </summary>
    public class RuntimeBattlePlayer : BaseData, 
        QuestionSetRecord.IQuestionResult {

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
        [AutoConvert]
        public ExerSlot exerSlot { get; protected set; } = new ExerSlot();
        [AutoConvert]
        public BattleItemSlot battleItemSlot { get; protected set; } = new BattleItemSlot();

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
        public int[] selection { get; protected set; }
        [AutoConvert]
        public int timespan { get; protected set; }

        /// <summary>
        /// 当前选择
        /// </summary>
        /// <returns>返回当前选择</returns>
        public int[] getSelection() { return selection; }

        /// <summary>
        /// 做题用时（毫秒）
        /// </summary>
        /// <returns>返回做题用时</returns>
        public int getTimespan() { return timespan; }

        /// <summary>
        /// 进度
        /// </summary>
        public int progress { get; protected set; } = -1;

        /// <summary>
        /// 行动数据
        /// </summary>
        public RuntimeAction[] actions { get; protected set; }

        /// <summary>
        /// 结果数据
        /// </summary>
        public BattlePlayer result { get; protected set; }

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
        }

        /// <summary>
        /// 加载行动
        /// </summary>
        /// <param name="json">数据</param>
        public void loadActions(JsonData json) {
            actions = DataLoader.load<RuntimeAction[]>(json);
        }

        /// <summary>
        /// 加载结果
        /// </summary>
        /// <param name="json">数据</param>
        public void loadResult(JsonData json) {
            result = DataLoader.load<BattlePlayer>(json);
        }

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
        /// 是否匹配完成
        /// </summary>
        /// <returns>返回当前是否匹配完成</returns>
        public bool isMatchingCompleted() {
            return player1.progress == 100 &&
                player2.progress == 100;
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
            var player = getPlayer(pid);
            player.loadAnswer(correct, timespan);
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

    }

    #endregion

}