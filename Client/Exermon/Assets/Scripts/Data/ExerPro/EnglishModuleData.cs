
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
    /// 短语题目
    /// </summary>
    public class PhraseQuestion : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public string word { get; protected set; }
        [AutoConvert]
        public string chinese { get; protected set; }
        [AutoConvert]
        public string phrase { get; protected set; }
        [AutoConvert]
        public int type { get; protected set; }

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
        [AutoConvert]
        public bool current { get; protected set; }
        [AutoConvert]
        public bool currentCorrect { get; protected set; }
        [AutoConvert]
        public bool currentDone { get; protected set; }

        /// <summary>
        /// 获取单词实例
        /// </summary>
        /// <returns></returns>
        public Word word() {
            return EnglishService.get().getQuestion<Word>(wordId);
        }

        /// <summary>
        /// 是否当前轮正确单词
        /// </summary>
        /// <returns></returns>
        public bool isCurrentCorrect() {
            return currentDone && currentCorrect;
        }

        /// <summary>
        /// 是否当前轮错误单词
        /// </summary>
        /// <returns></returns>
        public bool isCurrentWrong() {
            return currentDone && !currentCorrect;
        }
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

            AddState = 220, // 增加状态
            RemoveState = 221, // 移除状态
            RemoveNegaState = 222, // 移除消极状态

            DrawCards = 300, // 抽取卡牌
            ConsumeCards = 310, // 消耗卡牌

            ChangeCost = 400, // 更改耗能
            ChangeCostDisc = 401, // 更改耗能（发现）
            ChangeCostCrazy = 402, // 更改耗能（疯狂）

            Sadistic = 1000, // 残虐天性
            ForceAddStatus = 1100, // 增加己方状态
        }

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int code { get; protected set; }
        [AutoConvert("params")]
        public JsonData params_ { get; protected set; } // 参数（数组）

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerProEffectData() { }
        public ExerProEffectData(Code code, JsonData params_) {
            this.code = (int)code; this.params_ = params_;
        }

    }

    /// <summary>
    /// 特训物品数据
    /// </summary>
    public abstract class BaseExerProItem : BaseItem {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public int iconIndex { get; protected set; }
		[AutoConvert]
        public int starId { get; protected set; }
        [AutoConvert]
        public int gold { get; protected set; }
        [AutoConvert]
        public ExerProEffectData[] effects { get; protected set; }

		/// <summary>
		/// 物品图标
		/// </summary>
		public Sprite icon { get; protected set; }

		/// <summary>
		/// 读取函数
		/// </summary>
		/// <param name="index">索引</param>
		/// <returns>返回裁剪后的精灵</returns>
		public delegate Sprite LoadFun(int index);

		/// <summary>
		/// 获取读取函数
		/// </summary>
		/// <returns>读取函数</returns>
		protected abstract LoadFun loadFun();

		/// <summary>
		/// 物品星级
		/// </summary>
		/// <returns>返回物品星级对象</returns>
		public ExerProItemStar star() {
			return DataService.get().exerProItemStar(starId);
		}

		/// <summary>
		/// 加载自定义属性
		/// </summary>
		/// <param name="json"></param>
		protected override void loadCustomAttributes(JsonData json) {
			base.loadCustomAttributes(json);
			icon = loadFun()?.Invoke(iconIndex);
		}
	}

    /// <summary>
    /// 特训物品数据
    /// </summary>
    public class ExerProItem : BaseExerProItem {

		/// <summary>
		/// 属性
		/// </summary>

		/// <summary>
		/// 获取读取函数
		/// </summary>
		/// <returns>读取函数</returns>
		protected override LoadFun loadFun() {
			return AssetLoader.getExerProItemIconSprite;
		}
	}

    /// <summary>
    /// 特训药水数据
    /// </summary>
    public class ExerProPotion : BaseExerProItem {

		/// <summary>
		/// 属性
		/// </summary>
		
		/// <summary>
		/// 获取读取函数
		/// </summary>
		/// <returns>读取函数</returns>
		protected override LoadFun loadFun() {
			return AssetLoader.getExerProItemIconSprite;
		}
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
		/// 获取读取函数
		/// </summary>
		/// <returns>读取函数</returns>
		protected override LoadFun loadFun() {
			return AssetLoader.getExerProCardIconSprite;
		}

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
                AddStates = 4, Escape = 5, Unset = 6
            }

            /// <summary>
            /// 属性
            /// </summary>
            [AutoConvert]
            public int[] rounds { get; protected set; }
            [AutoConvert]
            public int type { get; protected set; }
            [AutoConvert("params")]
            public JsonData params_ { get; protected set; }
            [AutoConvert]
            public int rate { get; protected set; }

			/// <summary>
			/// 获取类型枚举
			/// </summary>
			/// <returns></returns>
			public Type typeEnum() {
				return (Type)type;
			}
			
            /// <summary>
            /// 是否为逃跑
            /// </summary>
            /// <returns></returns>
            public bool isEscape() {
                return type == (int)Type.Escape;
            }

            /// <summary>
            /// 是否无操作
            /// </summary>
            /// <returns></returns>
            public bool isUnset() {
                return type == (int)Type.Unset;
            }

            /// <summary>
            /// 测试回合
            /// </summary>
            /// <param name="round">回合</param>
            /// <returns>返回指定回合能否发动本行动</returns>
            public bool testRound(int round) {
                if (rounds.Length <= 0) return true;
                foreach (var r in rounds) if (r == round) return true;
                return false;
            }
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
        [AutoConvert("type_")]
        public int type_ { get; protected set; }
        [AutoConvert]
        public string character { get; protected set; } // 性格

        [AutoConvert]
        public Action[] actions { get; protected set; }
        [AutoConvert]
        public ExerProEffectData[] effects { get; protected set; }

		public Texture2D battle { get; protected set; }

		/// <summary>
		/// 类型文本
		/// </summary>
		/// <returns></returns>
		public string typeText() {
            return DataService.get().enemyType(type_).Item2;
		}

		/// <summary>
		/// 数据加载
		/// </summary>
		/// <param name="json">数据</param>
		protected override void loadCustomAttributes(JsonData json) {
			base.loadCustomAttributes(json);
			battle = AssetLoader.loadExerProEnemyBattle(id);
		}
	}

    /// <summary>
    /// 特训状态数据
    /// </summary>
    public class ExerProState : BaseItem {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public int iconIndex { get; protected set; } 
		[AutoConvert]
		public int maxTurns { get; protected set; } // 最大状态叠加回合数
		[AutoConvert]
        public bool isNega { get; protected set; } // 是否负面
		
		/// <summary>
		/// 物品图标
		/// </summary>
		public Sprite icon { get; protected set; }
		
		/// <summary>
		/// 加载自定义属性
		/// </summary>
		/// <param name="json"></param>
		protected override void loadCustomAttributes(JsonData json) {
			base.loadCustomAttributes(json);
			icon = AssetLoader.getExerProStateIconSprite(iconIndex);
		}
	}
    
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

        /// <summary>
        /// 抽出的牌
        /// </summary>
        List<ExerProPackCard> drawnCards = new List<ExerProPackCard>();

        /// <summary>
        /// 获取并清除抽取的卡牌
        /// </summary>
        public ExerProPackCard[] getDrawnCards() {
            var res = drawnCards.ToArray();
            drawnCards.Clear(); return res;
        }

        /// <summary>
        /// 接收物品
        /// </summary>
        /// <param name="item"></param>
        protected override void acceptItem(ExerProPackCard item) {
            base.acceptItem(item); drawnCards.Add(item);
        }
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
        public void discardCard() {
            var card = handGroup.getRandomItem(); // 随机
            if (card == null) return;
            handGroup.transferItem(discardGroup, card);
        }

        /// <summary>
        /// 消耗牌（本次战斗不再出现）
        /// </summary>
        public void consumeCard(ExerProPackCard card) {
            handGroup.transferItem(this, card);
        }
        public void consumeCard() {
            var card = handGroup.getRandomItem(); // 随机
            if (card == null) return;
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
        [AutoConvert("enemies")]
        public int[] _enemies { get; protected set; }
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
        /// 缓存敌人集合
        /// </summary>
        List<ExerProEnemy> tmpEnemies;

        /// <summary>
        /// 敌人数组
        /// </summary>
        /// <returns>返回敌人数组</returns>
        public List<ExerProEnemy> enemies() {
            if (tmpEnemies == null) {
                if (_enemies == null) return null;
                tmpEnemies = new List<ExerProEnemy>(_enemies.Length);
                foreach (var enemy in _enemies)
                    tmpEnemies.Add(DataService.get().exerProEnemy(enemy));
            }
            return tmpEnemies;
        }

        /// <summary>
        /// BOSS数组
        /// </summary>
        /// <returns>返回敌人数组</returns>
        public List<ExerProEnemy> bosses() {
            return enemies().FindAll(e => e.type == (int)ExerProEnemy.EnemyType.Boss);
        }

        /// <summary>
        /// BOSS数组
        /// </summary>
        /// <returns>返回敌人数组</returns>
        public List<ExerProEnemy> normalEnemies() {
            return enemies().FindAll(e => e.type == (int)ExerProEnemy.EnemyType.Normal);
        }

        /// <summary>
        /// BOSS数组
        /// </summary>
        /// <returns>返回敌人数组</returns>
        public List<ExerProEnemy> eliteEnemies() {
            return enemies().FindAll(e => e.type == (int)ExerProEnemy.EnemyType.Elite);
        }
        /*
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerProMapStage() { }
        public ExerProMapStage(int order, int[] _enemies, int maxBattleEnemies,
            int[] steps, int maxForkNode, int maxFork, int[] nodeRate) {
            this.order = order; this._enemies = _enemies;
            this.maxBattleEnemies = maxBattleEnemies;
            this.steps = steps; this.maxFork = maxFork;
            this.maxForkNode = maxForkNode;
            this.nodeRate = nodeRate;
        }
        */
    }

    #endregion

    #region 储存信息

    /// <summary>
    /// 特训记录
    /// </summary>
    public class ExerProRecord : BaseData, ParamDisplay.IDisplayDataConvertable {

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
        public int curIndex { get; protected set; } = -1; // 当前节点索引
        [AutoConvert]
        public bool nodeFlag { get; set; } = false; // 是否完成据点事件
        [AutoConvert]
        public RuntimeActor actor { get; protected set; } = null;

        /// <summary>
        /// 单词轮属性
        /// </summary>
        [AutoConvert]
        public int wordLevel { get; protected set; } = 1;

        [AutoConvert(autoConvert = false)]
        public List<WordRecord> wordRecords { get; protected set; }

        /// <summary>
        /// 下一单词ID
        /// </summary>
        public int next { get; set; } = 0;

        /*
        [AutoConvert]
        public int sum { get; protected set; }
        [AutoConvert]
        public int correct { get; protected set; }
        [AutoConvert]
        public int wrong { get; protected set; }
        */

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
            var sum = wordRecords.Count;
            var corrCnt = getCorrCnt();
            var wrongCnt = getWrongCnt();

            var corrRate = corrCnt * 1.0 / sum;
            var wrongRate = corrRate + wrongCnt * 1.0 / sum;
            var rest = sum - corrCnt - wrongCnt;

            res["level"] = wordLevel;
            res["sum"] = sum;
            res["corr_cnt"] = corrCnt;
            res["wrong_cnt"] = wrongCnt;
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
        /*
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
        */
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
            return getNode(curIndex);
        }

        /// <summary>
        /// 是否已选择起点
        /// </summary>
        /// <returns></returns>
        public bool isFirstSelected() {
            return curIndex >= 0;
        }

        /// <summary>
        /// 下一个单词
        /// </summary>
        /// <returns></returns>
        public Word nextWord() {
            if (next <= 0) return wordRecords[0].word();
            return EnglishService.get().getQuestion<Word>(next);
        }

        /// <summary>
        /// 下一个单词
        /// </summary>
        /// <returns></returns>
        public int[] recordWordIds() {
            var cnt = wordRecords.Count;
            var res = new int[cnt];
            for (int i = 0; i < cnt; ++i)
                res[i] = wordRecords[i].wordId;
            return res;
        }

        #region 记录控制

        /// <summary>
        /// 获取本轮单词正确数量
        /// </summary>
        /// <returns></returns>
        public int getCorrCnt() {
            int cnt = 0;
            foreach (var record in wordRecords)
                if (record.isCurrentCorrect()) cnt++;
            return cnt;
        }

        /// <summary>
        /// 获取本轮单词错误数量
        /// </summary>
        /// <returns></returns>
        public int getWrongCnt() {
            int cnt = 0;
            foreach (var record in wordRecords)
                if (record.isCurrentWrong()) cnt++;
            return cnt;
        }

        #endregion

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
        /// 开始特训
        /// </summary>
        public void start() {
            createActor(); generate();
        }
        /*
        /// <summary>
        /// 设置地图
        /// </summary>
        public void setup(bool restart = false) {
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
            _enemies = null; generated = false; nodes.Clear();
        }
        */

        /// <summary>
        /// 重置单词
        /// </summary>
        /// <param name="words">单词集合</param>
        public void setupWords(Word[] words) {
            if (words.Length > 0) next = words[0].id;
            else next = 0;
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
            curIndex = id; onMoved();
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

            nodes = nodes ?? new List<ExerProMapNode>();

            for (int i = 0; i < nodes.Count; ++i) {
                nodes[i].setId(i); nodes[i].stage = this;
            }

            if (DataLoader.contains(json, "words")) {
                var words = DataLoader.load<Word[]>(json, "words");
                EnglishService.get().questionCache.addQuestions(words);
                setupWords(words);
            }
            // generate();
        }

        /// <summary>
        /// 创建一个Actor
        /// </summary>
        public void createActor() {
            var player = PlayerService.get().player;
            if (player == null) return;

            actor = actor ?? new RuntimeActor();
            actor.setupPlayer(player);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerProRecord() { }
        public ExerProRecord(ExerProMapStage stage) : this(stage.map.id, stage.order) { }
        public ExerProRecord(ExerProMap map, int order) : this(map.id, order) { }

        public ExerProRecord(int mapId, int stageOrder) {
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
        public ExerProRecord stage { get; set; }

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

        /// <summary>
        /// 是否当前据点
        /// </summary>
        public bool isCurrent() {
            return status == (int)Status.Current;
        }

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerProMapNode() { }
        public ExerProMapNode(int id, ExerProRecord stage, 
            int xOrder, int yOrder, Type type) {
            this.id = id; this.stage = stage;
            this.xOrder = xOrder; this.yOrder = yOrder;
            typeId = (int)type; 

            generatePosition();
        }
    }

    #endregion

    #region 运行时数据

    /// <summary>
    /// 运行时BUFF
    /// </summary>
    public class RuntimeBuff : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int paramId { get; protected set; } // 属性ID
        [AutoConvert]
        public int value { get; protected set; } // 改变数值
        [AutoConvert]
        public double rate { get; protected set; } // 改变比率
        [AutoConvert]
        public int turns { get; protected set; } // BUFF回合

        /// <summary>
        /// 是否为Debuff
        /// </summary>
        /// <returns></returns>
        public bool isDebuff() {
            return value < 0 || rate < 1;
        }

        /// <summary>
        /// BUFF是否过期
        /// </summary>
        /// <returns></returns>
        public bool isOutOfDate() {
            return turns == 0;
        }

        /// <summary>
        /// 回合结束回调
        /// </summary>
        public void onRoundEnd() {
            if (turns > 0) turns--;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public RuntimeBuff() { }
        public RuntimeBuff(int paramId, 
            int value = 0, double rate = 1, int turns=0) {
            this.paramId = paramId; this.value = value;
            this.rate = rate; this.turns = turns;
        }

    }

    /// <summary>
    /// 运行时状态
    /// </summary>
    public class RuntimeState : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int stateId { get; protected set; } // 状态ID
        [AutoConvert]
        public int turns { get; protected set; } // 状态回合

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <returns></returns>
        public ExerProState state() {
            return DataService.get().exerProState(stateId);
        }

        /// <summary>
        /// 是否为负面状态
        /// </summary>
        /// <returns></returns>
        public bool isNega() {
            return state().isNega;
        }

        /// <summary>
        /// 状态是否过期
        /// </summary>
        /// <returns></returns>
        public bool isOutOfDate() {
            return turns <= 0;
        }

        /// <summary>
        /// 回合结束回调
        /// </summary>
        public void onRoundEnd() {
            if (turns > 0) turns--;
        }

        /// <summary>
        /// 移除状态回合
        /// </summary>
        /// <param name="turns">回合数</param>
        public void remove(int turns) {
            this.turns -= turns;
        }

        /// <summary>
        /// 增加状态回合
        /// </summary>
        /// <param name="turns">回合数</param>
        public void add(int turns) {
            this.turns += turns;
            var max = state().maxTurns;
            if (max > 0 && this.turns > max)
                this.turns = max; 
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public RuntimeState() { }
        public RuntimeState(int stateId, int turns = 0) {
            this.stateId = stateId; this.turns = turns;
        }

    }

    /// <summary>
    /// 运行时战斗者
    /// </summary>
    public abstract class RuntimeBattler : BaseData, ParamDisplay.IDisplayDataConvertable {

        /// <summary>
        /// 属性ID约定
        /// </summary>
        public const int MHPParamId = 1;
        public const int PowerParamId = 2;
        public const int DefenseParamId = 3;
        public const int AgileParamId = 4;

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int hp { get; protected set; }

        /// <summary>
        /// 状态和BUFF
        /// </summary>
        [AutoConvert]
        public Dictionary<int, RuntimeState> states { get; protected set; } = new Dictionary<int, RuntimeState>();
        [AutoConvert]
        public List<RuntimeBuff> buffs { get; protected set; } = new List<RuntimeBuff>();

		/// <summary>
		/// 是否逃跑
		/// </summary>
		public bool isEscaped = false;

		#region 数据转化

		/// <summary>
		/// 转化为显示数据
		/// </summary>
		/// <param name="type">类型</param>
		/// <returns></returns>
		public virtual JsonData convertToDisplayData(string type = "") {
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

            res["hp"] = hp; res["mhp"] = mhp();
            res["rate"] = hp * 1.0 / mhp();

            return res;
        }

		#endregion

		#region 数据控制

		/// <summary>
		/// 是否玩家
		/// </summary>
		/// <returns></returns>
		public virtual bool isActor() { return false; }

		/// <summary>
		/// 是否敌人
		/// </summary>
		/// <returns></returns>
		public virtual bool isEnemy() { return false; }

		/// <summary>
		/// 获取战斗图
		/// </summary>
		/// <returns></returns>
		public abstract Texture2D getBattlePicture();

		#region 属性控制

		#region 属性快捷定义

		/// <summary>
		/// 最大生命值
		/// </summary>
		/// <returns></returns>
		public int mhp() {
            return param(MHPParamId);
        }

        /// <summary>
        /// 力量
        /// </summary>
        /// <returns></returns>
        public int power() {
            return param(PowerParamId);
        }

        /// <summary>
        /// 格挡
        /// </summary>
        /// <returns></returns>
        public int defense() {
            return param(DefenseParamId);
        }

        /// <summary>
        /// 敏捷
        /// </summary>
        /// <returns></returns>
        public int agile() {
            return param(AgileParamId);
        }

        /// <summary>
        /// 基础最大生命值
        /// </summary>
        /// <returns></returns>
        protected virtual int baseMHP() { return 0; }

        /// <summary>
        /// 基础力量
        /// </summary>
        /// <returns></returns>
        protected virtual int basePower() { return 0; }

        /// <summary>
        /// 基础格挡
        /// </summary>
        /// <returns></returns>
        protected virtual int baseDefense() { return 0; }

        /// <summary>
        /// 基础敏捷
        /// </summary>
        /// <returns></returns>
        protected virtual int baseAgile() { return 0; }

		#endregion

		#region HP控制

		#region HP变化显示

		/// <summary>
		/// HP该变量
		/// </summary>
		public class DeltaHP {

			public int value = 0; // 值

			public bool critical = false; // 是否暴击
			public bool miss = false; // 是否闪避

			/// <summary>
			/// 构造函数
			/// </summary>
			/// <param name="value"></param>
			public DeltaHP(int value = 0, bool critical = false, bool miss = false) {
				this.value = value; this.critical = critical; this.miss = miss;
			}
		}

		/// <summary>
		/// HP变化量
		/// </summary>
		DeltaHP _deltaHP = null;
		public DeltaHP deltaHP {
			get {
				var res = _deltaHP;
				_deltaHP = null; return res;
			}
		}

		/// <summary>
		/// 设置闪避标志
		/// </summary>
		public void setMissFlag() {
			_deltaHP = _deltaHP ?? new DeltaHP();
			_deltaHP.miss = true;
		}

		/// <summary>
		/// 设置暴击标志
		/// </summary>
		public void setCriticalFlag() {
			_deltaHP = _deltaHP ?? new DeltaHP();
			_deltaHP.critical = true;
		}

		/// <summary>
		/// 设置值变化
		/// </summary>
		/// <param name="value"></param>
		public void setHPChange(int value) {
			_deltaHP = _deltaHP ?? new DeltaHP();
			_deltaHP.value = value;
		}
		
		#endregion

		/// <summary>
		/// 改变HP
		/// </summary>
		/// <param name="value">目标值</param>
		/// <param name="show">是否显示</param>
		public void changeHP(int value, bool show = true) {
			var oriHp = hp;
			hp = Mathf.Clamp(value, 0, mhp());
			if (show) setHPChange(hp - oriHp);
            if (hp <= 0) onDie();
        }

		/// <summary>
		/// 增加HP
		/// </summary>
		/// <param name="value">增加值</param>
		/// <param name="show">是否显示</param>
		public void addHP(int value, bool show = true) {
            changeHP(hp + value, show);
        }

		/// <summary>
		/// 增加HP
		/// </summary>
		/// <param name="rate">增加率</param>
		/// <param name="show">是否显示</param>
		public void addHP(double rate, bool show = true) {
            changeHP((int)Math.Round(hp * rate), show);
        }

        /// <summary>
        /// 死亡回调
        /// </summary>
        protected virtual void onDie() { }

        /// <summary>
        /// 是否死亡
        /// </summary>
        /// <returns></returns>
        public bool isDeath() {
            return hp <= 0;
        }

        #endregion

        #region 属性统一接口

        /// <summary>
        /// 基本属性值
        /// </summary>
        /// <param name="paramId">属性ID</param>
        /// <returns>属性值</returns>
        public virtual int baseParam(int paramId) {
            switch (paramId) {
                case MHPParamId: return baseMHP();
                case PowerParamId: return basePower();
                case DefenseParamId: return baseDefense();
                case AgileParamId: return baseAgile();
                default: return 0;
            }
        }

        /// <summary>
        /// Buff附加值
        /// </summary>
        /// <param name="paramId">属性ID</param>
        /// <returns></returns>
        public int buffValue(int paramId) {
            int value = 0;
            foreach (var buff in buffs)
                if (!buff.isOutOfDate()) value += buff.value;
            return value;
        }

        /// <summary>
        /// Buff附加率
        /// </summary>
        /// <param name="paramId">属性ID</param>
        /// <returns></returns>
        public double buffRate(int paramId) {
            double rate = 1;
            foreach (var buff in buffs)
                if (!buff.isOutOfDate()) rate *= buff.rate;
            return rate;
        }

        /// <summary>
        /// 额外属性
        /// </summary>
        /// <param name="paramId">属性ID</param>
        /// <returns></returns>
        public virtual int extraParam(int paramId) {
            return 0;
        }

        /// <summary>
        /// 属性值
        /// </summary>
        /// <param name="paramId">属性ID</param>
        /// <returns>属性值</returns>
        public int param(int paramId) {
            var base_ = baseParam(paramId);
            var rate = buffRate(paramId);
            var value = buffValue(paramId);
            var extra = extraParam(paramId);
            return (int)Math.Round(base_ * rate + value + extra);
        }
        
        /// <summary>
        /// 属性相加
        /// </summary>
        /// <param name="paramId">属性ID</param>
        /// <param name="value">增加值</param>
        public void addParam(int paramId, int value) {
            addBuff(paramId, value);
        }

        /// <summary>
        /// 属性相乘
        /// </summary>
        /// <param name="paramId">属性ID</param>
        /// <param name="rate">比率</param>
        public void multParam(int paramId, double rate) {
            addBuff(paramId, 0, rate);
        }

        #endregion

        #endregion

        #region Buff控制

        /// <summary>
        /// BUFF添加回调
        /// </summary>
        public virtual void onBuffAdded(RuntimeBuff buff) { }

        /// <summary>
        /// BUFF移除回调
        /// </summary>
        public virtual void onBuffRemoved(RuntimeBuff buff, bool force = false) { }

        #region Buff变更

        /// <summary>
        /// 添加Buff
        /// </summary>
        /// <param name="paramId">属性ID</param>
        /// <param name="value">变化值</param>
        /// <param name="rate">变化率</param>
        /// <param name="turns">持续回合</param>
        /// <returns>返回添加的Buff</returns>
        public RuntimeBuff addBuff(int paramId,
            int value = 0, double rate = 1, int turns = 0) {
            return addBuff(new RuntimeBuff(paramId, value, rate, turns));
        }
        public RuntimeBuff addBuff(RuntimeBuff buff) {
            buffs.Add(buff); onBuffAdded(buff);
            return buff;
        }

        /// <summary>
        /// 移除Buff
        /// </summary>
        /// <param name="index">Buff索引</param>
        public void removeBuff(int index, bool force = false) {
            var buff = buffs[index];
            buffs.RemoveAt(index);
            onBuffRemoved(buff, force);
        }
        /// <param name="buff">Buff对象</param>
        public void removeBuff(RuntimeBuff buff, bool force = false) {
            buffs.Remove(buff);
            onBuffRemoved(buff, force);
        }

        /// <summary>
        /// 移除多个满足条件的Buff
        /// </summary>
        /// <param name="p">条件</param>
        public void removeBuffs(Predicate<RuntimeBuff> p, bool force = true) {
            for (int i = buffs.Count() - 1; i >= 0; --i)
                if (p(buffs[i])) removeBuff(i, force);
        }

        /// <summary>
        /// 清除所有Debuff
        /// </summary>
        public void removeDebuffs(bool force = true) {
            removeBuffs(buff => buff.isDebuff(), force);
        }

        /// <summary>
        /// 清除所有Buff
        /// </summary>
        public void clearBuffs(bool force = true) {
            for (int i = buffs.Count() - 1; i >= 0; --i)
                removeBuff(i, force);
        }

        /// <summary>
        /// 是否处于指定条件的Buff
        /// </summary>
        public bool containsBuff(int paramId) {
            return buffs.Exists(buff => buff.paramId == paramId);
        }
        public bool containsBuff(Predicate<RuntimeBuff> p) {
            return buffs.Exists(p);
        }

        #endregion

        #region Buff判断

        /// <summary>
        /// 是否处于指定条件的Debuff
        /// </summary>
        public bool containsDebuff(int paramId) {
            return buffs.Exists(buff => buff.isDebuff() && buff.paramId == paramId);
        }

        /// <summary>
        /// 是否存在Debuff
        /// </summary>
        public bool anyDebuff() {
            return buffs.Exists(buff => buff.isDebuff());
        }

        /// <summary>
        /// 获取指定条件的Buff
        /// </summary>
        public RuntimeBuff getBuff(int paramId) {
            return buffs.Find(buff => buff.paramId == paramId);
        }
        public RuntimeBuff getBuff(Predicate<RuntimeBuff> p) {
            return buffs.Find(p);
        }

        /// <summary>
        /// 获取指定条件的Buff（多个）
        /// </summary>
        public List<RuntimeBuff> getBuffs(Predicate<RuntimeBuff> p) {
            return buffs.FindAll(p);
        }

		#endregion

		#endregion

		#region 状态控制

		/// <summary>
		/// 状态是否改变
		/// </summary>
		bool _isStateChanged = false;
		public bool isStateChanged {
			get {
				var res = _isStateChanged;
				_isStateChanged = false; return res;
			}
		}
		
        /// <summary>
        /// 状态添加回调
        /// </summary>
        public virtual void onStateAdded(RuntimeState state) { }

        /// <summary>
        /// 状态解除回调
        /// </summary>
        public virtual void onStateRemoved(RuntimeState state, bool force = false) { }

		/// <summary>
		/// 获取所有状态
		/// </summary>
		/// <returns></returns>
		public List<ExerProState> allStates() {
			var res = new List<ExerProState>(states.Count);
			foreach (var pair in states)
				res.Add(pair.Value.state());
			return res;
		}

		/// <summary>
		/// 获取所有运行时状态
		/// </summary>
		/// <returns></returns>
		public List<RuntimeState> allRuntimeStates() {
			var res = new List<RuntimeState>(states.Count);
			foreach (var pair in states) res.Add(pair.Value);
			return res;
		}

		#region 状态变更

		/// <summary>
		/// 添加状态
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <param name="value">变化值</param>
		/// <param name="rate">变化率</param>
		/// <param name="turns">持续回合</param>
		/// <returns>返回添加的Buff</returns>
		public RuntimeState addState(int stateId, int turns = 0) {
            RuntimeState state;
			_isStateChanged = true;

			if (states.ContainsKey(stateId)) {
                state = states[stateId];
                state.add(turns);
            } else {
                state = new RuntimeState(stateId, turns);
                states.Add(stateId, state);
                onStateAdded(state);
            }

            return state;
        }

        /// <summary>
        /// 移除状态
        /// </summary>
        /// <param name="stateId">状态ID</param>
        /// <param name="turns">移除回合数</param>
        public RuntimeState removeState(int stateId, int turns = 0, bool force = false) {
            if (!containsState(stateId)) return null;
            var state = states[stateId];
			_isStateChanged = true;

			if (turns <= 0) {
                states.Remove(stateId);
                onStateRemoved(state, force);
            } else {
                state.remove(turns);
                if (state.isOutOfDate())
                    removeState(stateId, force: force);
            }

            return state;
        }
        /// <param name="buff">Buff对象</param>
        public RuntimeState removeState(RuntimeState state, int turns = 0, bool force = false) {
            return removeState(state.stateId, turns, force);
        }

        /// <summary>
        /// 移除多个满足条件的状态
        /// </summary>
        /// <param name="p">条件</param>
        public void removeStates(Predicate<RuntimeState> p, bool force = true) {
            for (int i = states.Count() - 1; i >= 0; --i)
                if (p(states[i])) removeState(i, force: force);
        }

        /// <summary>
        /// 清除所有Debuff
        /// </summary>
        public void removeNegeStates() {
            removeStates(state => state.isNega());
        }

        /// <summary>
        /// 清除所有状态
        /// </summary>
        public void clearStates(bool force = true) {
            foreach (var pair in states)
                removeState(pair.Value, force: force);
        }

        #endregion

        #region 状态判断

        /// <summary>
        /// 是否处于指定条件的状态
        /// </summary>
        public bool containsState(int stateId) {
            return states.ContainsKey(stateId);
        }
        public bool containsState(Predicate<RuntimeState> p) {
            foreach(var pair in states)
                if (p(pair.Value)) return true;
            return false;
        }

        /// <summary>
        /// 是否存在负面状态
        /// </summary>
        public bool anyNegaState() {
            return containsState(state => state.isNega());
        }

        /// <summary>
        /// 获取指定条件的状态
        /// </summary>
        public RuntimeState getState(int stateId) {
            return states[stateId];
        }
        public RuntimeState getState(Predicate<RuntimeState> p) {
            foreach (var pair in states)
                if (p(pair.Value)) return pair.Value;
            return null;
        }

		/// <summary>
		/// 获取指定条件的状态（多个）
		/// </summary>
		public List<RuntimeState> getStates(Predicate<RuntimeState> p) {
			var res = new List<RuntimeState>();
			foreach (var pair in states)
				if (p(pair.Value)) res.Add(pair.Value);
			return res;
		}

		/// <summary>
		/// 是否处于可移动状态
		/// </summary>
		/// <returns></returns>
		public bool isMovableState() {
			return !isEscaped && !isDeath();
		}

		#endregion

		#endregion

		#region 结果控制

		/// <summary>
		/// 当前结果
		/// </summary>
		RuntimeActionResult currentResult = null;

        /// <summary>
        /// 应用结果
        /// </summary>
        /// <param name="result"></param>
        public void applyResult(RuntimeActionResult result) {
            currentResult = result;
            // TODO: 结果应用
            CalcService.ResultApplyCalc.apply(this, result);
        }

        /// <summary>
        /// 获取当前结果（用于显示）
        /// </summary>
        /// <returns></returns>
        public RuntimeActionResult getResult() {
            var res = currentResult;
            currentResult = null;
            return res;
        }

		#endregion

		#region 行动控制

		/// <summary>
		/// 行动序列
		/// </summary>
		Queue<RuntimeAction> actions;

		/// <summary>
		/// 当前行动
		/// </summary>
		/// <returns></returns>
		public virtual RuntimeAction currentAction() {
			if (actions.Count <= 0) return null;
			return actions.Peek();
		}

		/// <summary>
		/// 增加行动
		/// </summary>
		/// <param name="action">行动</param>
		public void addAction(RuntimeAction action) {
			actions.Enqueue(action);
		}

		/// <summary>
		/// 处理当前行动（处理后移出队列）
		/// </summary>
		public void processAction() {
			if (actions.Count <= 0) return;

			var action = actions.Peek();
			if (!isMovableState()) return;

			action.generateResult();
			action.object_.applyResult(action.result);
		}

		/// <summary>
		/// 结束当前行动
		/// </summary>
		public void endAction() {
			if (actions.Count <= 0) return;
			actions.Dequeue();
		}

		#endregion

		#region 回调控制

        /// <summary>
        /// 回合结束回调
        /// </summary>
        public virtual void onRoundEnd() {
            processBuffsRoundEnd();
            processStatesRoundEnd();
        }

        /// <summary>
        /// 处理状态回合结束
        /// </summary>
        void processBuffsRoundEnd() {
            var buffs = this.buffs.ToArray();

            for (int i = 0; i < buffs.Length; ++i) {
                var buff = buffs[i]; buff.onRoundEnd();
                if (buff.isOutOfDate()) removeBuff(i);
            }
        }

        /// <summary>
        /// 处理状态回合结束
        /// </summary>
        void processStatesRoundEnd() {
            var states = this.states.ToArray();

            foreach (var pair in states) {
                var state = pair.Value;
                state.onRoundEnd();
				_isStateChanged = true;
				if (state.isOutOfDate())
                    removeState(state.stateId);
            }
        }

        /// <summary>
        /// 战斗结束回调
        /// </summary>
        public virtual void onBattleEnd() {

        }

        #endregion

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void reset() {
            hp = mhp();
            clearStates();
            clearBuffs();
        }

        #endregion

    }

    /// <summary>
    /// 特训玩家
    /// </summary>
    public class RuntimeActor : RuntimeBattler {

        /// <summary>
        /// 默认属性
        /// </summary>
        public const int DefaultMHP = 50; // 初始体力值
        public const int DefaultPower = 5; // 初始力量
        public const int DefaultDefense = 5; // 初始格挡
        public const int DefaultAgile = 5; // 初始敏捷

        public const int DefaultGold = 100; // 初始金币

        const int EnglishSubjectId = 3; // 英语科目ID

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert("mhp")]
        public int _mhp { get; protected set; }
        [AutoConvert("power")]
        public int _power { get; protected set; }
        [AutoConvert("defense")]
        public int _defense { get; protected set; }
        [AutoConvert("agile")]
        public int _agile { get; protected set; }

        [AutoConvert]
        public int gold { get; protected set; }

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

		///// <summary>
		///// 转化为显示数据
		///// </summary>
		///// <param name="type">类型</param>
		///// <returns></returns>
		//public JsonData convertToDisplayData(string type = "") {
		//    switch (type) {
		//        case "hp": return convertHp();
		//        default: return toJson();
		//    }
		//}

		#endregion

		#region 数据控制

		/// <summary>
		/// 获取战斗图
		/// </summary>
		public override Texture2D getBattlePicture() {
			if (slotItem == null || slotItem.isNullItem()) return null;
			return slotItem.playerExer.exermon().battle;
		}

		/// <summary>
		/// 是否玩家
		/// </summary>
		/// <returns></returns>
		public override bool isActor() { return true; }

		#region 属性定义

		/// <summary>
		/// 基础最大生命值
		/// </summary>
		/// <returns></returns>
		protected override int baseMHP() { return _mhp; }

		/// <summary>
		/// 基础力量
		/// </summary>
		/// <returns></returns>
		protected override int basePower() { return _power; }

		/// <summary>
		/// 基础格挡
		/// </summary>
		/// <returns></returns>
		protected override int baseDefense() { return _defense; }

		/// <summary>
		/// 基础敏捷
		/// </summary>
		/// <returns></returns>
		protected override int baseAgile() { return _agile; }

		#endregion

		/// <summary>
		/// 重置
		/// </summary>
		public override void reset() {
			_mhp = DefaultMHP;
			_power = DefaultPower;
			_defense = DefaultDefense;
			_agile = DefaultAgile;
			gold = DefaultGold;
			base.reset();
		}

		/// <summary>
		/// 配置玩家
		/// </summary>
		/// <param name="player"></param>
		public void setupPlayer(Player player) {
			slotItem = player.getExerSlotItem(EnglishSubjectId);
		}

		#endregion

		///// <summary>
		///// 构造函数
		///// </summary>
		//public ExerProActor() { }
		//public ExerProActor(Player player) {
		//    setupPlayer(player);
		//}
	}

	/// <summary>
	/// 运行时敌人数据
	/// </summary>
	public class RuntimeEnemy : RuntimeBattler {

        /// <summary>
        /// 最大偏移量
        /// </summary>
        const int MaxXOffset = 24;
        const int MaxYOffset = 16;

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int pos { get; protected set; }
        [AutoConvert]
        public int xOffset { get; protected set; }
        [AutoConvert]
        public int yOffset { get; protected set; }

        [AutoConvert]
        public int enemyId { get; protected set; }

		/// <summary>
		/// 当前行动状态
		/// </summary>
		public bool isActionEnd = false; // 当前行动是否结束
        public bool isActionStarted = false; // 当前行动是否开始

        /// <summary>
        /// 缓存敌人对象
        /// </summary>
        ExerProEnemy tempEnemy = null;

		#region 数据控制

		/// <summary>
		/// 获取战斗图
		/// </summary>
		public override Texture2D getBattlePicture() {
			var enemy = this.enemy();
			if (enemy == null) return null;
			return enemy.battle;
		}

		/// <summary>
		/// 是否玩家
		/// </summary>
		/// <returns></returns>
		public override bool isEnemy() { return true; }
		
		#region 属性控制

		/// <summary>
		/// 最大体力值
		/// </summary>
		/// <returns></returns>
		protected override int baseMHP() {
            return enemy().mhp;
        }

        /// <summary>
        /// 力量
        /// </summary>
        /// <returns></returns>
        protected override int basePower() {
            return enemy().power;
        }

        /// <summary>
        /// 格挡
        /// </summary>
        /// <returns></returns>
        protected override int baseDefense() {
            return enemy().defense;
        }
        
        #endregion

        /// <summary>
        /// 敌人
        /// </summary>
        /// <returns></returns>
        public ExerProEnemy enemy() {
            if (tempEnemy != null) return tempEnemy;
            return tempEnemy = DataService.get().exerProEnemy(enemyId);
        }

        /// <summary>
        /// 生成位置
        /// </summary>
        public void generatePosition() {
            xOffset = UnityEngine.Random.Range(-MaxXOffset, MaxXOffset);
            yOffset = UnityEngine.Random.Range(-MaxYOffset, MaxYOffset);
        }

		#endregion

		#region 行动控制

		/// <summary>
		/// 当前行动
		/// </summary>
		ExerProEnemy.Action _currentEnemyAction = null;
		public ExerProEnemy.Action currentEnemyAction {
			get {
				if (!isActionStarted) return null;
				var res = _currentEnemyAction;
				_currentEnemyAction = null;
				return res;
			}
		}

		/// <summary>
		/// 当前行动类型
		/// </summary>
		/// <returns></returns>
		public int currentActionType() {
			return _currentEnemyAction.type;
		}

		/// <summary>
		/// 当前行动类型枚举
		/// </summary>
		/// <returns></returns>
		public ExerProEnemy.Action.Type currentActionTypeEnum() {
			return _currentEnemyAction.typeEnum();
		}

		/// <summary>
		/// 当前行动参数
		/// </summary>
		/// <returns></returns>
		public JsonData currentActionParams() {
			return _currentEnemyAction.params_;
		}

		/// <summary>
		/// 当前行动
		/// </summary>
		/// <returns></returns>
		public override RuntimeAction currentAction() {
			if (!isActionStarted) return null;
			return base.currentAction();
		}

		/// <summary>
		/// 更新行动
		/// </summary>
		public bool updateAction() {
            if (!isMovableState()) return isActionEnd = true;
			processEnemyAction(); return isActionEnd;
        }

        /// <summary>
        /// 计算下一步
        /// </summary>
        public void calcNext(int round, RuntimeActor actor) {
			RuntimeAction action;
            CalcService.EnemyNextCalc.calc(round, this, actor,
                out _currentEnemyAction, out action);
			addAction(action);
        }

        /// <summary>
        /// 处理敌人行动
        /// </summary>
        public void processEnemyAction() {
			isActionStarted = true;
			if (_currentEnemyAction.isUnset())
				isActionEnd = true;
        }

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public RuntimeEnemy() { }
        public RuntimeEnemy(int pos, ExerProEnemy enemy) {
            tempEnemy = enemy; enemyId = enemy.id;
            this.pos = pos;

            generatePosition();
            reset();
        }

    }

    /// <summary>
    /// 运行时行动
    /// </summary>
    public class RuntimeAction : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public RuntimeBattler subject { get; protected set; } // 主体
        [AutoConvert]
        public RuntimeBattler object_ { get; protected set; } // 对象
        [AutoConvert]
        public ExerProEffectData[] effects { get; protected set; } // 效果

        /// <summary>
        /// 结果
        /// </summary>
        [AutoConvert]
        public RuntimeActionResult result { get; protected set; } = null;

        /// <summary>
        /// 生成结果
        /// </summary>
        public void generateResult() {
            // TODO: 结果生成
            result = CalcService.ExerProActionResultGenerator.generate(this);
        }

		/// <summary>
		/// 构造函数
		/// </summary>
		public RuntimeAction() { }
        public RuntimeAction(RuntimeBattler subject,
            RuntimeBattler object_, ExerProEffectData[] effects = null) {
            this.subject = subject; this.object_ = object_;
            this.effects = effects ?? new ExerProEffectData[0];
        }
        public RuntimeAction(RuntimeBattler subject,
            RuntimeBattler object_, BaseExerProItem item) :
            this(subject, object_, item.effects) { }
    }

    /// <summary>
    /// 运行时行动结果
    /// </summary>
    public class RuntimeActionResult : BaseData {

        /// <summary>
        /// 状态改变
        /// </summary>
        public class StateChange : RuntimeState {

            /// <summary>
            /// 属性
            /// </summary>
            [AutoConvert]
            public new bool remove { get; protected set; } // 是否移除状态（turns为0时移除全部回合）

            /// <summary>
            /// 构造函数
            /// </summary>
            public StateChange() { }
            public StateChange(int stateId, int turns = 0, bool remove = false) : base(stateId, turns) {
                this.remove = remove;
            }
        }

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int hpDamage { get; set; }
        [AutoConvert]
        public int hpRecover { get; set; }
        [AutoConvert]
        public int hpDrain { get; set; }

        /// <summary>
        /// 状态/Buff变更
        /// </summary>
        [AutoConvert]
        public StateChange[] stateChanges { get; set; }
        [AutoConvert]
        public RuntimeBuff[] addBuffs { get; set; }

        /// <summary>
        /// 抽牌数据
        /// </summary>
        [AutoConvert]
        public int drawCardCnt { get; set; } // 抽牌数量
        [AutoConvert]
        public int consumeCardCnt { get; set; } // 消耗牌数量
        [AutoConvert]
        public bool consumeSelect { get; set; } // 消耗是否可选
        
        /// <summary>
        /// 行动
        /// </summary>
        public RuntimeAction action { get; protected set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public RuntimeActionResult() { }
        public RuntimeActionResult(RuntimeAction action) {
            this.action = action;
        }
    }

    #endregion

}