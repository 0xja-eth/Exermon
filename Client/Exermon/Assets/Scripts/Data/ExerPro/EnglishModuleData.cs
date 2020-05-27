
using System;
using System.Collections.Generic;
using UnityEngine;

using LitJson;
using System.Linq;

using Core.Data;
using Core.Data.Loaders;

using GameModule.Data;
using GameModule.Services;

using QuestionModule.Data;

using ItemModule.Data;
using PlayerModule.Data;

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

    /// <summary>
    /// 阅读小题
    /// </summary>
    public class ReadingSubQuestion : BaseQuestion { }

    /// <summary>
    /// 阅读题
    /// </summary>
    public class ReadingQuestion : GroupQuestion<ReadingSubQuestion> { }

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

    #endregion

    #region 物品

    /// <summary>
    /// 特训物品数据
    /// </summary>
    public class ExerProItem : BaseItem {

        /// <summary>
        /// 属性
        /// </summary>

    }

    /// <summary>
    /// 特训药水数据
    /// </summary>
    public class ExerProPotion : BaseItem {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int hpRecover { get; protected set; }
        [AutoConvert]
        public double hpRate { get; protected set; }
        [AutoConvert]
        public int powerAdd { get; protected set; }
        [AutoConvert]
        public int powerRate { get; protected set; }

    }

    /// <summary>
    /// 特训卡片数据
    /// </summary>
    public class ExerProCard : BaseItem {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int cost { get; protected set; }
        [AutoConvert]
        public int cardType { get; protected set; }

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
        /// 等级
        /// </summary>
        public enum Level {
            Normal = 1, Elite = 2, Boss = 3
        }

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int mhp { get; protected set; }
        [AutoConvert]
        public int power { get; protected set; }
        [AutoConvert]
        public int level { get; protected set; }

        /// <summary>
        /// 类型文本
        /// </summary>
        /// <returns></returns>
        public string levelText() {
            return DataService.get().enemyLevel(level).Item2;
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
    public class ExerProPackCard : PackContItem<ExerProCard> { }

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
    /// 特训卡片背包
    /// </summary>
    public class ExerProCardPack : PackContainer<ExerProPackCard> { }

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
    public class ExerProActor : BaseData {

        /// <summary>
        /// 默认属性
        /// </summary>
        public const int DefaultMHP = 50; // 初始体力值
        public const int DefaultPower = 5; // 初始力量
        public const int DefaultDefense = 5; // 初始格挡
        public const int DefaultAgile = 5; // 初始敏捷

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
        public ExerProItemPack itemPack { get; protected set; } = new ExerProItemPack();
        [AutoConvert]
        public ExerProPotionPack potionPack { get; protected set; } = new ExerProPotionPack();
        [AutoConvert]
        public ExerProCardPack cardPack { get; protected set; } = new ExerProCardPack();
    }

    /// <summary>
    /// 地图关卡记录
    /// </summary>
    public class MapStageRecord : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int mapId { get; protected set; }
        [AutoConvert]
        public int stageOrder { get; protected set; }

        [AutoConvert]
        public bool initialized { get; protected set; } = false;

        [AutoConvert]
        public List<ExerProMapNode> nodes { get; protected set; } = new List<ExerProMapNode>();

        /// <summary>
        /// 角色属性
        /// </summary>
        [AutoConvert]
        public int currentId { get; protected set; } // 当前节点索引
        [AutoConvert]
        public ExerProActor actor { get; protected set; }

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
            var enemies = stage()?.enemies;
            if (enemies == null) return null;
            var res = new List<ExerProEnemy>(enemies.Length);
            foreach (var enemy in enemies)
                res.Add(DataService.get().exerProEnemy(enemy));
            return res;
        }

        /// <summary>
        /// BOSS数组
        /// </summary>
        /// <returns>返回敌人数组</returns>
        public List<ExerProEnemy> bosses() {
            return enemies().FindAll(e => e.type == (int)ExerProEnemy.Level.Boss);
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

        /// <summary>
        /// 生成地图
        /// </summary>
        void generate() {
            if (initialized) return; 
            initialized = CalcService.NodeGenerator.generate(this);
        }

        /// <summary>
        /// 读取自定义数据
        /// </summary>
        /// <param name="json"></param>
        protected override void loadCustomAttributes(JsonData json) {
            base.loadCustomAttributes(json);
            for (int i = 0; i < nodes.Count; ++i) {
                nodes[i].setId(i); nodes[i].stage = this;
            }

            generate();
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
            Rest = 0, //休息据点
	        Treasure = 1, //藏宝据点
	        Shop = 2, //商人据点
	        Enemy = 3, //敌人据点
	        Elite = 4, //精英据点
	        Unknown = 5, //未知据点
            Boss = 6, // 最终BOSS
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
        public Type type { get; protected set; }

        /// <summary>
        /// 下一个Y序号（数组）
        /// </summary>
        [AutoConvert]
        public List<int> nexts { get; protected set; } = new List<int>();

        /// <summary>
        /// 地图关卡
        /// </summary>
        public MapStageRecord stage { get; set; }

        /// <summary>
        /// 分叉标记
        /// </summary>
        public bool fork = false;

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
            if (nexts.Contains(next)) return;
            nexts.Add(next);
        }
        public void addNext(ExerProMapNode next) {
            addNext(next.id);
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
        /// 构造函数
        /// </summary>
        public ExerProMapNode() { }
        public ExerProMapNode(int id, MapStageRecord stage, 
            int xOrder, int yOrder, Type type) {
            this.id = id; this.stage = stage;
            this.xOrder = xOrder; this.yOrder = yOrder;
            this.type = type; 

            generatePosition();
        }
    }

    #endregion

}