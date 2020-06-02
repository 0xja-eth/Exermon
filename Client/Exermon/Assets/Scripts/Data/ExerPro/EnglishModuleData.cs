
using System;
using System.Collections.Generic;
using UnityEngine;

using LitJson;
using System.Linq;

using Core.Data;
using Core.Data.Loaders;

using GameModule.Data;
using GameModule.Services;

using ItemModule.Data;
using PlayerModule.Data;
using ExermonModule.Data;
using QuestionModule.Data;

using PlayerModule.Services;

using UI.Common.Controls.ParamDisplays;

/// <summary>
/// 艾瑟萌特训模块
/// </summary>
namespace ExerPro { }

/// <summary>
/// 英语特训模块
/// </summary>
namespace ExerPro.EnglishModule { }

/// <summary>
/// 英语特训模块数据
/// </summary>
namespace ExerPro.EnglishModule.Data {

    using EnglishModule.Services;

    #region 题目

    /// <summary>
    /// 听力小题
    /// </summary>
    public class ListeningSubQuestion : BaseQuestion { }

    /// <summary>
    /// 听力题
    /// </summary>
    public class ListeningQuestion : GroupQuestion<ListeningSubQuestion> {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public AudioClip audio { get; protected set; }

    }
    /*
    /// <summary>
    /// 阅读小题
    /// </summary>
    public class ReadingSubQuestion : BaseQuestion { }

    /// <summary>
    /// 阅读题
    /// </summary>
    public class ReadingQuestion : GroupQuestion<ReadingSubQuestion> { }
    */

    /// <summary>
    /// 不定式题目
    /// </summary>
    public class InfinitiveQuestion : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public string word { get; protected set; }
        [AutoConvert]
        public string chinese { get; protected set; }
        [AutoConvert]
        public string infinitive { get; protected set; }

    }

    /// <summary>
    /// 改错题
    /// </summary>
    public class CorrectionQuestion : BaseData {

        /// <summary>
        /// 错误项
        /// </summary>
        public class WrongItem : BaseData {

            /// <summary>
            /// 属性
            /// </summary>
            [AutoConvert]
            public int sentenceIndex { get; protected set; }
            [AutoConvert]
            public int wordIndex { get; protected set; }
            [AutoConvert]
            public int type { get; protected set; }
            [AutoConvert]
            public string word { get; protected set; }

            /// <summary>
            /// 类型文本
            /// </summary>
            /// <returns></returns>
            public string typeText() {
                return DataService.get().correctType(type).Item2;
            }

        }

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public string article { get; protected set; }
        [AutoConvert]
        public string description { get; protected set; }

        [AutoConvert]
        public WrongItem[] wrongItems { get; protected set; }
    }

    /// <summary>
    /// 单词
    /// </summary>
    public class Word : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public string english { get; protected set; }
        [AutoConvert]
        public string chinese { get; protected set; }
        [AutoConvert]
        public string type { get; protected set; }
        [AutoConvert]
        public int level { get; protected set; }
        [AutoConvert]
        public bool isMiddle { get; protected set; }
        [AutoConvert]
        public bool isHigh { get; protected set; }

    }

    /// <summary>
    /// 单词记录
    /// </summary>
    public class WordRecord : BaseData {
        
        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int wordId { get; protected set; }
        [AutoConvert]
        public int count { get; protected set; }
        [AutoConvert]
        public int correct { get; protected set; }
        [AutoConvert]
        public DateTime firstDate { get; protected set; }
        [AutoConvert]
        public DateTime lastDate { get; protected set; }
        [AutoConvert]
        public bool collected { get; protected set; }
        [AutoConvert]
        public bool wrong { get; protected set; }

    }

    /// <summary>
    /// 反义词表
    /// </summary>
    public class Antonym : TypeData {
        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public string cardWord { get; protected set; }
        [AutoConvert]
        public string enemyWord { get; protected set; }
        [AutoConvert]
        public double hurtRate { get; protected set; }

    }

    #endregion

    #region 物品

    /// <summary>
    /// 特训效果数据
    /// </summary>
    public class ExerProEffectData : BaseData {

        /// <summary>
        /// 效果代码枚举
        /// </summary>
        public enum Code {
            Unset = 0, // 空

            Attack = 1, // 造成伤害
            AttackSlash = 2, // 造成伤害（完美斩击）
            AttackBlack = 3, // 造成伤害（黑旋风）
            AttackWave = 4, // 造成伤害（波动拳）
            AttackRite = 5, // 造成伤害（仪式匕首）
            Recover = 100, // 回复体力值
            AddParam = 200, // 增加能力值
            AddParamUrgent = 201, // 增加能力值（紧急按钮）
            TempAddParam = 210, // 临时增加能力值
            AddStatus = 220, // 增加状态
            GetCards = 300, // 抽取卡牌
            RemoveCards = 310, // 移除卡牌
            ChangeCost = 400, // 更改耗能
            ChangeCostDisc = 401, // 更改耗能（发现）
            ChangeCostCrazy = 402, // 更改耗能（疯狂）

            Sadistic = 500, // 残虐天性
            ForceAddStatus = 600, // 增加己方状态
        }

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int code { get; protected set; }
        [AutoConvert("params")]
        public JsonData params_ { get; protected set; } // 参数（数组）

        /*
        /// <summary>
        /// 构造函数
        /// </summary>
        public EffectData() { }
        public EffectData(Code code, JsonData params_,
            string description, string shortDescription) {
            this.code = (int)code;
            this.params_ = params_;
        }
        */
    }

    /// <summary>
    /// 特训物品数据
    /// </summary>
    public class BaseExerProItem : BaseItem {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int starId { get; protected set; }
        [AutoConvert]
        public int gold { get; protected set; }
        [AutoConvert]
        public ExerProEffectData[] effects { get; protected set; }

    }

    /// <summary>
    /// 特训物品数据
    /// </summary>
    public class ExerProItem : BaseExerProItem {

        /// <summary>
        /// 属性
        /// </summary>

    }

    /// <summary>
    /// 特训药水数据
    /// </summary>
    public class ExerProPotion : BaseExerProItem {

        /// <summary>
        /// 属性
        /// </summary>

    }

    /// <summary>
    /// 特训卡片数据
    /// </summary>
    public class ExerProCard : BaseExerProItem {

        /// <summary>
        /// 目标
        /// </summary>
        public enum Target {
            Default = 0,  // 默认
            One = 1,  // 敌方单体
            All = 2,  // 敌方全体
        }

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int cost { get; protected set; }
        [AutoConvert]
        public int cardType { get; protected set; }
        [AutoConvert]
        public bool inherent { get; protected set; }
        [AutoConvert]
        public bool disposable { get; protected set; }
        [AutoConvert]
        public string character { get; protected set; }
        [AutoConvert]
        public int target { get; protected set; }

        /// <summary>
        /// 类型文本
        /// </summary>
        /// <returns></returns>
        public string typeText() {
            return DataService.get().cardType(cardType).Item2;
        }
    }

    /// <summary>
    /// 特训敌人数据
    /// </summary>
    public class ExerProEnemy : BaseItem {

        /// <summary>
        /// 类型
        /// </summary>
        public enum EnemyType {
            Normal = 1, Elite = 2, Boss = 3
        }

        /// <summary>
        /// 行动数据
        /// </summary>
        public class Action : BaseData {

            /// <summary>
            /// 类型
            /// </summary>
            public enum Type {
                Attack = 1, PowerUp = 2, PowerDown = 3,
                Escape = 4, Unset = 5
            }

            /// <summary>
            /// 属性
            /// </summary>
            [AutoConvert]
            public int round { get; protected set; }
            [AutoConvert]
            public int type { get; protected set; }
            [AutoConvert("params")]
            public JsonData params_ { get; protected set; }
            [AutoConvert]
            public int rate { get; protected set; }

        }

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int mhp { get; protected set; }
        [AutoConvert]
        public int power { get; protected set; }
        [AutoConvert]
        public int defense { get; protected set; }
        [AutoConvert]
        public int type { get; protected set; }
        [AutoConvert]
        public string character { get; protected set; }

        [AutoConvert]
        public Action[] actions { get; protected set; }
        [AutoConvert]
        public ExerProEffectData[] effects { get; protected set; }

        /// <summary>
        /// 类型文本
        /// </summary>
        /// <returns></returns>
        public string typeText() {
            return DataService.get().enemyType(type).Item2;
        }
    }

    /// <summary>
    /// 特训状态数据
    /// </summary>
    public class ExerProStatus : BaseItem { }
    
    #endregion
    
    #region 容器项

    /// <summary>
    /// 特训背包物品
    /// </summary>
    public class ExerProPackItem : PackContItem<ExerProItem> { }

    /// <summary>
    /// 特训背包药水
    /// </summary>
    public class ExerProPackPotion : PackContItem<ExerProPotion> { }

    /// <summary>
    /// 特训背包卡片
    /// </summary>
    public class ExerProPackCard : PackContItem<ExerProCard> {

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerProPackCard() { }
        public ExerProPackCard(ExerProCard card) : base(card) {}
    }

    #endregion

    #region 容器

    /// <summary>
    /// 特训物品背包
    /// </summary>
    public class ExerProItemPack : PackContainer<ExerProPackItem> { }

    /// <summary>
    /// 特训药水背包
    /// </summary>
    public class ExerProPotionPack : PackContainer<ExerProPackPotion> { }

    /// <summary>
    /// 特训抽牌堆
    /// </summary>
    public class ExerProCardDrawGroup : PackContainer<ExerProPackCard> {

        /// <summary>
        /// 卡组
        /// </summary>
        public ExerProCardGroup cardGroup { get; set; }

        /// <summary>
        /// 接受物品
        /// </summary>
        /// <param name="item"></param>
        protected override void acceptItem(ExerProPackCard item) {
            // 卡牌进入的时候洗牌
            if (containItem(item)) return;
            var index = UnityEngine.Random.Range(0, items.Count);
            items.Insert(index, item);
        }

        /// <summary>
        /// 洗牌
        /// </summary>
        public void shuffle() {
            var tmpItems = items.ToArray();
            foreach (var item in tmpItems)
                transferItem(this, item);
        }

        /// <summary>
        /// 获取第一个卡牌（最后一张）
        /// </summary>
        /// <returns></returns>
        public ExerProPackCard firstCard() {
            return items[items.Count - 1];
        }
    }

    /// <summary>
    /// 特训弃牌堆
    /// </summary>
    public class ExerProCardDiscardGroup : PackContainer<ExerProPackCard> {

        /// <summary>
        /// 卡组
        /// </summary>
        public ExerProCardGroup cardGroup { get; set; }
    }

    /// <summary>
    /// 特训手牌
    /// </summary>
    public class ExerProCardHandGroup : PackContainer<ExerProPackCard> {

        /// <summary>
        /// 卡组
        /// </summary>
        public ExerProCardGroup cardGroup { get; set; }
    }

    /// <summary>
    /// 特训卡组
    /// </summary>
    public class ExerProCardGroup : PackContainer<ExerProPackCard> {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public ExerProCardDrawGroup drawGroup { get; protected set; } = new ExerProCardDrawGroup();
        [AutoConvert]
        public ExerProCardDiscardGroup discardGroup { get; protected set; } = new ExerProCardDiscardGroup();
        [AutoConvert]
        public ExerProCardHandGroup handGroup { get; protected set; } = new ExerProCardHandGroup();

        #region 卡牌获取

        /// <summary>
        /// 获取卡牌
        /// </summary>
        public void addCard(ExerProCard card) {
            pushItem(new ExerProPackCard(card));
        }

        #endregion

        #region 卡牌转移

        /// <summary>
        /// 战斗开始，生成牌堆
        /// </summary>
        public void onBattleStart() {
            var tmpItems = items.ToArray();
            foreach (var item in tmpItems)
                if (item.item().inherent) // 固有牌
                    transferItem(handGroup, item);
                else
                    transferItem(drawGroup, item);
        }

        /// <summary>
        /// 战斗结束，回收牌堆
        /// </summary>
        public void onBattleEnd() {
            recycle(drawGroup); recycle(discardGroup); recycle(handGroup);
        }

        /// <summary>
        /// 回合结束，自动弃牌
        /// </summary>
        public void onRoundEnd() {
            var tmpItems = handGroup.items.ToArray();
            foreach (var item in tmpItems)
                handGroup.transferItem(discardGroup, item);
        }

        /// <summary>
        /// 回合结束，自动弃牌
        /// </summary>
        public void recycle(PackContainer<ExerProPackCard> container) {
            var tmpItems = container.items.ToArray();
            foreach (var item in tmpItems)
                container.transferItem(this, item);
        }

        /// <summary>
        /// 抽牌
        /// </summary>
        public void drawCard() {
            drawGroup.transferItem(handGroup, drawGroup.firstCard());
        }

        /// <summary>
        /// 使用牌
        /// </summary>
        public void useCard(ExerProPackCard card) {
            if (card.item().disposable) consumeCard(card);
            else discardCard(card);
        }

        /// <summary>
        /// 弃牌
        /// </summary>
        public void discardCard(ExerProPackCard card) {
            handGroup.transferItem(discardGroup, card);
        }

        /// <summary>
        /// 消耗牌（本次战斗不再出现）
        /// </summary>
        public void consumeCard(ExerProPackCard card) {
            handGroup.transferItem(this, card);
        }

        #endregion

        /// <summary>
        /// 读取自定义数据
        /// </summary>
        /// <param name="json"></param>
        protected override void loadCustomAttributes(JsonData json) {
            base.loadCustomAttributes(json);
            drawGroup.cardGroup = discardGroup.cardGroup = handGroup.cardGroup = this;
        }

    }

    #endregion

    #region 地图

    /// <summary>
    /// 地图阶段数据
    /// </summary>
    public class ExerProMap : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public string name { get; protected set; }
        [AutoConvert]
        public string description { get; protected set; }
        [AutoConvert]
        public int level { get; protected set; }
        [AutoConvert]
        public int minLevel { get; protected set; }

        [AutoConvert]
        public ExerProMapStage[] stages { get; protected set; }

        /// <summary>
        /// 获取关卡
        /// </summary>
        /// <param name="order">序号</param>
        /// <returns>返回对应序号的关卡</returns>
        public ExerProMapStage stage(int order) {
            foreach (var stage in stages)
                if (stage.order == order) return stage;
            return null;
        }

        /// <summary>
        /// 读取自定义数据
        /// </summary>
        /// <param name="json"></param>
        protected override void loadCustomAttributes(JsonData json) {
            base.loadCustomAttributes(json);
            foreach (var stage in stages) stage.map = this;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerProMap() { }
        public ExerProMap(string name, int level, int minLevel, ExerProMapStage[] stages) {
            this.name = name; this.level = level; this.minLevel = minLevel;
            this.stages = stages;
        }
    }

    /// <summary>
    /// 地图关卡
    /// </summary>
    public class ExerProMapStage : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int order { get; protected set; }
        [AutoConvert]
        public int[] enemies { get; protected set; }
        [AutoConvert]
        public int maxBattleEnemies { get; protected set; }
        [AutoConvert]
        public int[] steps { get; protected set; }
        [AutoConvert]
        public int maxForkNode { get; protected set; }
        [AutoConvert]
        public int maxFork { get; protected set; }
        [AutoConvert]
        public int[] nodeRate { get; protected set; }

        /// <summary>
        /// 地图
        /// </summary>
        public ExerProMap map { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerProMapStage() { }
        public ExerProMapStage(int order, int[] enemies, int maxBattleEnemies,
            int[] steps, int maxForkNode, int maxFork, int[] nodeRate) {
            this.order = order; this.enemies = enemies;
            this.maxBattleEnemies = maxBattleEnemies;
            this.steps = steps; this.maxFork = maxFork;
            this.maxForkNode = maxForkNode;
            this.nodeRate = nodeRate;
        }
    }

    #endregion

    #region 储存信息

    /// <summary>
    /// 特训玩家
    /// </summary>
    public class ExerProActor : BaseData, MultParamsDisplay.IDisplayDataConvertable {

        /// <summary>
        /// 默认属性
        /// </summary>
        public const int DefaultMHP = 50; // 初始体力值
        public const int DefaultPower = 5; // 初始力量
        public const int DefaultDefense = 5; // 初始格挡
        public const int DefaultAgile = 5; // 初始敏捷

        public const int DefaultGold = 100; // 初始金币

        const int EnglishSubjectId = 3;

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int mhp { get; protected set; } = DefaultMHP;
        [AutoConvert]
        public int hp { get; protected set; } = DefaultMHP;
        [AutoConvert]
        public int power { get; protected set; } = DefaultPower;
        [AutoConvert]
        public int defense { get; protected set; } = DefaultDefense;
        [AutoConvert]
        public int agile { get; protected set; } = DefaultAgile;

        [AutoConvert]
        public int gold { get; protected set; } = DefaultGold;

        [AutoConvert]
        public ExerProItemPack itemPack { get; protected set; } = new ExerProItemPack();
        [AutoConvert]
        public ExerProPotionPack potionPack { get; protected set; } = new ExerProPotionPack();
        [AutoConvert]
        public ExerProCardGroup cardGroup { get; protected set; } = new ExerProCardGroup();

        /// <summary>
        /// 对应的艾瑟萌槽项
        /// </summary>
        public ExerSlotItem slotItem { get; protected set; } = null;

        #region 数据转化

        /// <summary>
        /// 转化为显示数据
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public JsonData convertToDisplayData(string type = "") {
            switch (type) {
                case "hp": return convertHp();
                default: return toJson();
            }
        }

        /// <summary>
        /// 转化状态
        /// </summary>
        /// <returns></returns>
        JsonData convertHp() {
            var res = new JsonData();

            res["hp"] = hp; res["mhp"] = mhp;
            res["rate"] = hp * 1.0 / mhp;

            return res;
        }

        #endregion

        /// <summary>
        /// 配置玩家
        /// </summary>
        /// <param name="player"></param>
        public void setupPlayer(Player player) {
            slotItem = player.getExerSlotItem(EnglishSubjectId);
        }

        ///// <summary>
        ///// 构造函数
        ///// </summary>
        //public ExerProActor() { }
        //public ExerProActor(Player player) {
        //    setupPlayer(player);
        //}
    }

    /// <summary>
    /// 地图关卡记录
    /// </summary>
    public class MapStageRecord : BaseData, MultParamsDisplay.IDisplayDataConvertable {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int mapId { get; protected set; }
        [AutoConvert]
        public int stageOrder { get; protected set; }

        /// <summary>
        /// 是否开始（游戏是否一开始，尚未结束）
        /// </summary>
        [AutoConvert]
        public bool started { get; protected set; } = false;

        /// <summary>
        /// 地图是否生成（本关卡的地图是否已经生成）
        /// </summary>
        [AutoConvert]
        public bool generated { get; protected set; } = false;

        /// <summary>
        /// 生成的据点
        /// </summary>
        [AutoConvert]
        public List<ExerProMapNode> nodes { get; protected set; } = new List<ExerProMapNode>();

        /// <summary>
        /// 角色属性
        /// </summary>
        [AutoConvert]
        public int currentId { get; protected set; } = -1; // 当前节点索引
        [AutoConvert]
        public ExerProActor actor { get; protected set; } = null;

        /// <summary>
        /// 单词轮属性
        /// </summary>
        [AutoConvert]
        public int level { get; protected set; } = 1;
        [AutoConvert]
        public int next { get; set; } = 0; // 下一个单词ID

        [AutoConvert]
        public int sum { get; protected set; }
        [AutoConvert]
        public int correct { get; protected set; }
        [AutoConvert]
        public int wrong { get; protected set; }

        /// <summary>
        /// 敌人（缓存用）
        /// </summary>
        List<ExerProEnemy> _enemies = null;

        #region 数据转化

        /// <summary>
        /// 转化为显示数据
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public JsonData convertToDisplayData(string type = "") {
            switch (type) {
                case "map_progress": return convertMapProgressData();
                case "word_progress": return convertWordProgressData();
                default: return toJson();
            }
        }

        /// <summary>
        /// 转化地图进度数据
        /// </summary>
        /// <returns></returns>
        JsonData convertMapProgressData() {
            var node = currentNode();
            var last = lastNode();
            var res = new JsonData();

            if (node != null && last != null)
                res["rate"] = (node.xOrder * 1.0 / last.xOrder);

            return res;
        }

        /// <summary>
        /// 转化单词进度数据
        /// </summary>
        /// <returns></returns>
        JsonData convertWordProgressData() {
            var res = new JsonData();
            var corrRate = correct * 1.0 / sum;
            var wrongRate = corrRate + wrong * 1.0 / sum;
            var rest = sum - correct - wrong;

            res["level"] = level;
            res["sum"] = sum;
            res["correct"] = correct;
            res["wrong"] = wrong;
            res["rest"] = rest;

            res["corr_rate"] = corrRate;
            res["wrong_rate"] = wrongRate;

            return res;
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 获取关卡
        /// </summary>
        /// <param name="order">序号</param>
        /// <returns>返回对应序号的关卡</returns>
        public ExerProMap map() {
            return DataService.get().exerProMap(mapId);
        }

        /// <summary>
        /// 获取关卡
        /// </summary>
        /// <param name="order">序号</param>
        /// <returns>返回对应序号的关卡</returns>
        public ExerProMapStage stage() {
            return map()?.stage(stageOrder);
        }

        /// <summary>
        /// 敌人数组
        /// </summary>
        /// <returns>返回敌人数组</returns>
        public List<ExerProEnemy> enemies() {
            if (_enemies == null) {
                var enemies = stage()?.enemies;
                if (enemies == null) return null;
                _enemies = new List<ExerProEnemy>(enemies.Length);
                foreach (var enemy in enemies)
                    _enemies.Add(DataService.get().exerProEnemy(enemy));
            }
            return _enemies;
        }

        /// <summary>
        /// BOSS数组
        /// </summary>
        /// <returns>返回敌人数组</returns>
        public List<ExerProEnemy> bosses() {
            return enemies().FindAll(e => e.type == (int)ExerProEnemy.EnemyType.Boss);
        }

        /// <summary>
        /// 获取据点对象
        /// </summary>
        /// <param name="id">据点ID</param>
        /// <returns>返回据点对象</returns>
        public ExerProMapNode getNode(int id) {
            if (id < 0 || id >= nodes.Count) return null;
            return nodes[id]; // nodes.Find(node => node.id == id);
        }
        /// <param name="xOrder">X顺序</param>
        /// <param name="yOrder">Y顺序</param>
        public ExerProMapNode getNode(int xOrder, int yOrder) {
            return nodes.Find(node => node.xOrder == xOrder &&
                node.yOrder == yOrder);
        }

        /// <summary>
        /// 初始据点
        /// </summary>
        /// <returns></returns>
        public List<ExerProMapNode> firstNodes() {
            return nodes.FindAll(node => node.xOrder == 0);
        }

        /// <summary>
        /// 最后据点
        /// </summary>
        /// <returns></returns>
        public ExerProMapNode lastNode() {
            if (nodes.Count <= 0) return null;
            return nodes[nodes.Count - 1];
        }

        /// <summary>
        /// 获取当前据点
        /// </summary>
        /// <returns>返回当前据点对象</returns>
        public ExerProMapNode currentNode() {
            return getNode(currentId);
        }

        /// <summary>
        /// 是否已选择起点
        /// </summary>
        /// <returns></returns>
        public bool isFirstSelected() {
            return currentId >= 0;
        }

        /// <summary>
        /// 下一个单词
        /// </summary>
        /// <returns></returns>
        public Word nextWord() {
            return EnglishService.get().getQuestion<Word>(next);
        }

        #endregion

        #region 据点控制

        /// <summary>
        /// 生成地图
        /// </summary>
        void generate() {
            if (generated) return; 
            generated = CalcService.NodeGenerator.generate(this);
            refreshNodeStatuses();
        }

        /// <summary>
        /// 创建据点
        /// </summary>
        /// <param name="xOrder">X序号</param>
        /// <param name="yOrder">Y序号</param>
        /// <param name="type">据点类型</param>
        public ExerProMapNode createNode(int xOrder, int yOrder, ExerProMapNode.Type type) {
            var node = new ExerProMapNode(nodes.Count, this, xOrder, yOrder, type);
            nodes.Add(node); return node;
        }

        #endregion

        #region 游戏控制

        /// <summary>
        /// 设置地图
        /// </summary>
        /// <param name="mapId">地图ID</param>
        /// <param name="stageOrder">关卡序号</param>
        /// <param name="restart">重新开始</param>
        public void setup(int mapId, int stageOrder, bool restart = false) {
            this.mapId = mapId; this.stageOrder = stageOrder;
            reset(restart); createActor(); generate();
        }

        /// <summary>
        /// 重置地图
        /// </summary>
        /// <param name="restart">重新开始</param>
        void reset(bool restart = false) {
            if (restart) {
                started = false; actor = null;
            }
            resetMap();
        }

        /// <summary>
        /// 重置地图
        /// </summary>
        /// <param name="restart">重新开始</param>
        void resetMap() {
            currentId = -1; _enemies = null;
            generated = false; nodes.Clear();
        }

        /// <summary>
        /// 重置单词
        /// </summary>
        /// <param name="words">单词集合</param>
        public void resetWords(Word[] words) {
            sum = words.Length;
            correct = wrong = 0;
            if (sum > 0) next = words[0].id;
        }

        /// <summary>
        /// 走到下一步
        /// </summary>
        /// <param name="id">下一步的据点ID</param>
        /// <param name="force">强制转移</param>
        /// <param name="emit">是否发射事件</param>
        public void moveNext(int id, bool force = false) {
            if (force) changePosition(id);
            else {
                var node = currentNode();
                if (node == null) return;
                foreach (var next in node.nexts)
                    if (id == next) changePosition(id);
            }
        }

        /// <summary>
        /// 更改位置
        /// </summary>
        /// <param name="id">新结点ID</param>
        /// <param name="emit">是否发射事件</param>
        void changePosition(int id) {
            currentId = id; onMoved();
        }
        
        /// <summary>
        /// 移动结束回调
        /// </summary>
        void onMoved() {
            refreshNodeStatuses();
        }

        /// <summary>
        /// 刷新据点状态
        /// </summary>
        void refreshNodeStatuses() {
            var current = currentNode();
            foreach(var node in nodes) {
                if (node.status == (int)ExerProMapNode.Status.Passed) continue;

                var status = ExerProMapNode.Status.Deactive;

                if (current != null) {
                    if (node == current)
                        status = ExerProMapNode.Status.Current;
                    else if (node.xOrder < current.xOrder)
                        status = ExerProMapNode.Status.Over;
                    else if (node.status == (int)ExerProMapNode.Status.Current)
                        status = ExerProMapNode.Status.Passed;
                    else if (current.nexts.Contains(node.id))
                        status = ExerProMapNode.Status.Active;
                } else if (node.xOrder <= 0)
                    status = ExerProMapNode.Status.Active;

                node.setStatus(status);
            }
        }

        #endregion

        #region 数据读取

        /// <summary>
        /// 读取自定义数据
        /// </summary>
        /// <param name="json"></param>
        protected override void loadCustomAttributes(JsonData json) {
            base.loadCustomAttributes(json);
            for (int i = 0; i < nodes.Count; ++i) {
                nodes[i].setId(i); nodes[i].stage = this;
            }

            // generate();
        }

        /// <summary>
        /// 创建一个Actor
        /// </summary>
        public void createActor() {
            var player = PlayerService.get().player;
            if (player == null) return;

            actor = actor ?? new ExerProActor();
            actor.setupPlayer(player);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public MapStageRecord() { }
        public MapStageRecord(ExerProMapStage stage) : this(stage.map.id, stage.order) { }
        public MapStageRecord(ExerProMap map, int order) : this(map.id, order) { }

        public MapStageRecord(int mapId, int stageOrder) {
            this.mapId = mapId; this.stageOrder = stageOrder;
            generate();
        }

        #endregion
    }

    /// <summary>
    /// 据点类型
    /// </summary>
    public class NodeType : TypeData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public string quesTypes { get; protected set; }

        /// <summary>
        /// 图标
        /// </summary>
        public Texture2D icon { get; protected set; }

        /// <summary>
        /// 读取自定义属性
        /// </summary>
        /// <param name="json"></param>
        protected override void loadCustomAttributes(JsonData json) {
            base.loadCustomAttributes(json);
            icon = AssetLoader.loadNodeIcon(id);
        }

    }

    /// <summary>
    /// 地图据点
    /// </summary>
    public class ExerProMapNode : BaseData {

        /// <summary>
        /// 最大偏移量
        /// </summary>
        const int MaxXOffset = 24;
        const int MaxYOffset = 16;

        /// <summary>
        /// 据点类型
        /// </summary>
        public enum Type {
            Rest = 1, //休息据点
            Treasure = 2, //藏宝据点
            Shop = 3, //商人据点
            Enemy = 4, //敌人据点
            Elite = 5, //精英据点
            Unknown = 6, //未知据点
            Boss = 7, // 最终BOSS
            Story = 8, // 剧情据点
        }

        /// <summary>
        /// 状态类型
        /// </summary>
        public enum Status {
            Deactive = 0, // 未激活（未到达）
            Active = 1, // 已激活（下一步）
            Current = 2, // 当前
            Passed = 3, // 已经过
            Over = 4, // 已结束
        }

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int xOrder { get; protected set; }
        [AutoConvert]
        public int yOrder { get; protected set; }
        [AutoConvert]
        public double xOffset { get; protected set; }
        [AutoConvert]
        public double yOffset { get; protected set; }
        [AutoConvert]
        public int typeId { get; protected set; }

        /// <summary>
        /// 下一个Y序号（数组）
        /// </summary>
        [AutoConvert]
        public HashSet<int> nexts { get; protected set; } = new HashSet<int>();
        [AutoConvert]
        public int status { get; protected set; }

        /// <summary>
        /// 地图关卡
        /// </summary>
        public MapStageRecord stage { get; set; }

        /// <summary>
        /// 分叉标记
        /// </summary>
        public bool fork = false;

        #region 生成

        /// <summary>
        /// 生成位置
        /// </summary>
        public void generatePosition() {
            xOffset = UnityEngine.Random.Range(-MaxXOffset, MaxXOffset);
            yOffset = UnityEngine.Random.Range(-MaxYOffset, MaxYOffset);
        }

        /// <summary>
        /// 设置ID
        /// </summary>
        /// <param name="id"></param>
        public void setId(int id) {
            this.id = id; 
        }

        /// <summary>
        /// 添加下一步
        /// </summary>
        /// <param name="next"></param>
        public void addNext(int next) {
            nexts.Add(next);
        }
        public void addNext(ExerProMapNode next) {
            addNext(next.id);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 获取类型
        /// </summary>
        /// <returns></returns>
        public NodeType type() {
            return DataService.get().nodeType(typeId);
        }
        /// <summary>
        /// 获取类型枚举
        /// </summary>
        /// <returns></returns>
        public Type typeEnum() {
            return (Type)typeId;
        }

        /// <summary>
        /// 获取下一节点
        /// </summary>
        /// <returns></returns>
        public List<ExerProMapNode> getNexts() {
            if (stage == null) return null;
            var nodes = new List<ExerProMapNode>();
            foreach (var next in nexts)
                nodes.Add(stage.nodes[next]);
            return nodes;
        }

        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="status">状态</param>
        public void setStatus(Status status) {
            this.status = (int)status;
        }
        public void setStatus(int status) {
            this.status = status;
        }

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerProMapNode() { }
        public ExerProMapNode(int id, MapStageRecord stage, 
            int xOrder, int yOrder, Type type) {
            this.id = id; this.stage = stage;
            this.xOrder = xOrder; this.yOrder = yOrder;
            typeId = (int)type; 

            generatePosition();
        }
    }

    #endregion

}