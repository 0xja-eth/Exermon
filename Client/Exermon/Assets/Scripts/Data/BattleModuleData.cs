
using LitJson;

using Core.Data;

using GameModule.Data;
using GameModule.Services;

using ItemModule.Data;
using PlayerModule.Data;
using QuestionModule.Data;
using ExermonModule.Data;
using SeasonModule.Data;
using QuestionModule.Services;
using RecordModule.Data;

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
    public class ResultJudge : TypeData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int score { get; protected set; }
        [AutoConvert]
        public int win { get; protected set; }
        [AutoConvert]
        public int lose { get; protected set; }
    }

    /// <summary>
    /// 对战记录
    /// </summary>
    public class BattleRecord : BaseData {

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
        }

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int scoreIncr { get; protected set; }
        [AutoConvert]
        public int timeScore { get; protected set; }
        [AutoConvert]
        public int hurtScore { get; protected set; }
        [AutoConvert]
        public int recoverScore { get; protected set; }
        [AutoConvert]
        public int correctScore { get; protected set; }
        [AutoConvert]
        public int plusScore { get; protected set; }
        [AutoConvert]
        public int result { get; protected set; }
        [AutoConvert]
        public int status { get; protected set; }
    }

    /// <summary>
    /// 对战物资槽
    /// </summary>
    public class BattleItemSlot : SlotContainer<BattleItemSlotItem> {

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
        /// 查找一个空的容器项
        /// </summary>
        /// <returns></returns>
        public BattleItemSlotItem emptySlotItem() {
            foreach (var item in items)
                if (item.isNullItem()) return item;
            return items[0];
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
        public HumanPackItem packItem { get; protected set; }

        /// <summary>
        /// 装备
        /// </summary>
        public override HumanPackItem equip1 {
            get { return packItem; }
            protected set { packItem = value; }
        }

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
        public ParamData[] plusParams { get; protected set; }
        [AutoConvert]
        public ParamRateData[] rateParams { get; protected set; }
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
        public ParamData[] params_ { get; protected set; }
        [AutoConvert("params")]
        public RuntimeBattleBuff[] buffs { get; protected set; }
        [AutoConvert("params")]
        public RuntimeBattleExerSkill[] skills { get; protected set; }

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
    /// 对战玩家数据（回合结果时进行数据同步）
    /// </summary>
    public class RuntimeBattlePlayer : BaseData {

        /// <summary>
        /// 基本数据
        /// </summary>
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
        public RuntimeBattleExermon[] exermons { get; protected set; }
        [AutoConvert]
        public RuntimeBattleItem[] battleItems { get; protected set; }

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

    }

    #endregion

}