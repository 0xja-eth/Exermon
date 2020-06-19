using System;
using System.Collections.Generic;
using UnityEngine;

using LitJson;

using Core.Data;
using Core.Data.Loaders;

using PlayerModule.Data;
using ExermonModule.Data;
using QuestionModule.Data;
using SeasonModule.Data;
using BattleModule.Data;

using ExerPro.EnglishModule.Data;

using UI.Common.Controls.ParamDisplays;

/// <summary>
/// 游戏模块
/// </summary>
namespace GameModule { }

/// <summary>
/// 游戏模块数据
/// </summary>
namespace GameModule.Data {

    /// <summary>
    /// 游戏静态配置数据
    /// </summary>
    public class GameStaticData : BaseData {

		/// <summary>
		/// 本地版本
		/// </summary>
		//public string localVersion = PlayerSettings.bundleVersion;
		public const string LocalMainVersion = Config.LocalMainVersion; // "0.3.2";
        public const string LocalSubVersion = Config.LocalSubVersion; // "20200527";

        /// <summary>
        /// 后台版本
        /// </summary>
        [AutoConvert]
        public GameVersionData curVersion { get; protected set; }

        /// <summary>
        /// 历史版本
        /// </summary>
        [AutoConvert]
        public List<GameVersionData> lastVersions { get; protected set; }

        /// <summary>
        /// 游戏配置
        /// </summary>
        public GameConfigure configure { get; protected set; }

        /// <summary>
        /// 游戏资料（数据库）
        /// </summary>
        public GameDatabase data { get; protected set; }

        /// <summary>
        /// 读取标志
        /// </summary>
        bool loaded = false;
        public bool isLoaded() { return loaded; }

        /// <summary>
        /// 是否需要ID
        /// </summary>
        protected override bool idEnable() { return false; }

        /// <summary>
        /// 生成更新日志
        /// </summary>
        /// <returns>更新日志文本</returns>
        public string generateUpdateNote() {
            string updateNote = "当前版本：\n" + curVersion.generateUpdateNote();
            updateNote += "\n历史版本：\n";
            foreach (var ver in lastVersions)
                updateNote += ver.generateUpdateNote();
            return updateNote;
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        /// <param name="json">数据</param>
        protected override void loadCustomAttributes(JsonData json) {
            base.loadCustomAttributes(json);

            if (curVersion == null) return;
            Debug.Log("curVersion: " + curVersion.generateUpdateNote());

            // 如果没有版本变更且数据已读取（本地缓存），则直接返回
            if (curVersion.mainVersion == LocalMainVersion &&
                curVersion.subVersion == LocalSubVersion && loaded) return;

            configure = DataLoader.load(configure, json, "configure");
            data = DataLoader.load(data, json, "data");

            loaded = true;
        }

        /// <summary>
        /// 转化自定义属性
        /// </summary>
        /// <param name="json"></param>
        protected override void convertCustomAttributes(ref JsonData json) {
            base.convertCustomAttributes(ref json);
            json["configure"] = DataLoader.convert(configure);
            json["data"] = DataLoader.convert(data);
        }
    }

    /// <summary>
    /// 游戏动态数据
    /// </summary>
    public class GameDynamicData : BaseData {

        /// <summary>
        /// 是否需要ID
        /// </summary>
        protected override bool idEnable() { return false; }

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public List<CompSeason> seasons { get; protected set; }
        [AutoConvert]
        public int curSeasonId { get; set; }
    }

    /// <summary>
    /// 科目数据
    /// </summary>
    public class Subject : TypeData, ParamDisplay.IDisplayDataConvertable {

        /// <summary>
        /// 强制选择数量
        /// </summary>
        public static int ForceCount = 0;

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public Color color { get; protected set; }
        [AutoConvert]
        public int maxScore { get; protected set; }
        [AutoConvert]
        public bool force { get; protected set; }

        /// <summary>
        /// 转化为属性信息
        /// </summary>
        /// <returns>属性信息</returns>
        public JsonData convertToDisplayData(string type = "") {
            return toJson();
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        /// <param name="json">数据</param>
        protected override void loadCustomAttributes(JsonData json) {
            base.loadCustomAttributes(json);
            if (force) ForceCount++;
        }
    }

    /// <summary>
    /// 基本能力数据
    /// </summary>
    public class BaseParam : TypeData, ParamDisplay.IDisplayDataConvertable {

        /// <summary>
        /// 描述文本格式
        /// </summary>
        const string DescFormat = "{0}：{1}";

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public Color color { get; protected set; }
        [AutoConvert]
        public int maxValue { get; protected set; }
        [AutoConvert]
        public int minValue { get; protected set; }
        [AutoConvert("default")]
        public int default_ { get; protected set; }
        [AutoConvert]
        public int scale { get; protected set; }
        [AutoConvert]
        public string attr { get; protected set; }

        /// <summary>
        /// 转化为属性信息
        /// </summary>
        /// <returns>属性信息</returns>
        public JsonData convertToDisplayData(string type = "") {
            var res = toJson();
            res["description"] = string.Format(DescFormat, name, description);
            return res;
        }

        /// <summary>
        /// 是否百分比属性
        /// </summary>
        /// <returns></returns>
        public bool isPercent() {
            return scale == 10000;
        }

        /// <summary>
        /// 限制最大最小值
        /// </summary>
        /// <param name="val">原始值</param>
        /// <returns>限制值</returns>
        public double clamp(double val) {
            val = Math.Max(minValue, val);
            if (maxValue > 0) val = Math.Min(maxValue, val);
            return val;
        }
    }

    /// <summary>
    /// 可用物品类型数据
    /// </summary>
    public class UsableItemType : TypeData { }

    /// <summary>
    /// 人类装备类型数据
    /// </summary>
    public class HumanEquipType : TypeData { }

    /// <summary>
    /// 艾瑟萌装备类型数据
    /// </summary>
    public class ExerEquipType : TypeData { }

    /// <summary>
    /// 艾瑟萌星级数据
    /// </summary>
    public class ExerStar : TypeData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public Color color { get; protected set; }
        [AutoConvert]
        public int maxLevel { get; protected set; }
        [AutoConvert]
        public ParamRangeData[] baseRanges { get; protected set; }
        [AutoConvert]
        public ParamRangeData[] rateRanges { get; protected set; }
        [AutoConvert]
        public double[] levelExpFactors { get; protected set; }
    }

    /// <summary>
    /// 艾瑟萌天赋星级数据
    /// </summary>
    public class ExerGiftStar : TypeData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public Color color { get; protected set; }
        [AutoConvert]
        public ParamRangeData[] paramRanges { get; protected set; }

    }

    /// <summary>
    /// 物品星级数据
    /// </summary>
    public class ItemStar : TypeData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public Color color { get; protected set; }
    }

    /// <summary>
    /// 题目星级数据
    /// </summary>
    public class QuesStar : TypeData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public Color color { get; protected set; }
        [AutoConvert]
        public int level { get; protected set; }
        [AutoConvert]
        public int weight { get; protected set; }
        [AutoConvert]
        public int expIncr { get; protected set; }
        [AutoConvert]
        public int goldIncr { get; protected set; }
        [AutoConvert]
        public int stdTime { get; protected set; }
        [AutoConvert]
        public int minTime { get; protected set; }

    }

	/// <summary>
	/// 游戏小贴士
	/// </summary>
	public class GameTip : TypeData {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public int type { get; protected set; }

	}

	/// <summary>
	/// 游戏版本数据
	/// </summary>
	public class GameVersionData : BaseData {

        /// <summary>
        /// 更新日志格式
        /// </summary>
        public const string UpdateNoteFormat = "版本号：{1}.{2}\n更新日期：{0}\n更新内容：\n{3}\n";

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public string mainVersion { get; protected set; }
        [AutoConvert]
        public string subVersion { get; protected set; }
        [AutoConvert]
        public string updateNote { get; protected set; }
        [AutoConvert]
        public DateTime updateTime { get; protected set; }
        [AutoConvert]
        public string description { get; protected set; }

        /// <summary>
        /// 生成单个版本的更新日志
        /// </summary>
        /// <returns>更新日志文本</returns>
        public string generateUpdateNote() {
            string time = updateTime.ToString(DataLoader.SystemDateFormat);
            return string.Format(GameVersionData.UpdateNoteFormat, mainVersion,
                subVersion, time, updateNote, description);
        }

    }

    /// <summary>
    /// 游戏系统配置数据
    /// </summary>
    public class GameConfigure : BaseData {

        /// <summary>
        /// 基本术语
        /// </summary>
        [AutoConvert]
        public string name { get; protected set; }
        [AutoConvert]
        public string engName { get; protected set; }
        [AutoConvert]
        public string gold { get; protected set; }
        [AutoConvert]
        public string ticket { get; protected set; }
        [AutoConvert]
        public string boundTicket { get; protected set; }

        /// <summary>
        /// 配置量
        /// </summary>
        [AutoConvert]
        public int maxSubject { get; protected set; }
        [AutoConvert]
        public int maxExerciseCount { get; protected set; }
        [AutoConvert(format: "date")]
        public DateTime minBirth { get; protected set; }

        /// <summary>
        /// 组合术语
        /// </summary>
        [AutoConvert]
        public Tuple<int, string>[] characterGenders { get; protected set; }
        [AutoConvert]
        public Tuple<int, string>[] playerGrades { get; protected set; }
        [AutoConvert]
        public Tuple<int, string>[] playerStatuses { get; protected set; }
        [AutoConvert]
        public Tuple<int, string>[] playerTypes { get; protected set; }

        [AutoConvert]
        public Tuple<int, string>[] exermonTypes { get; protected set; }
        [AutoConvert]
        public Tuple<int, string>[] exerSkillTargetTypes { get; protected set; }
        [AutoConvert]
        public Tuple<int, string>[] exerSkillHitTypes { get; protected set; }

        [AutoConvert]
        public Tuple<int, string>[] questionTypes { get; protected set; }
        [AutoConvert]
        public Tuple<int, string>[] questionStatuses { get; protected set; }
        [AutoConvert]
        public Tuple<int, string>[] quesReportTypes { get; protected set; }
        [AutoConvert]
        public Tuple<int, string>[] quesReportTypeDescs { get; protected set; }

        [AutoConvert]
        public Tuple<int, string>[] recordSources { get; protected set; }
        [AutoConvert]
        public Tuple<int, string>[] exerciseGenTypes { get; protected set; }

        [AutoConvert]
        public Tuple<int, string>[] battleModes { get; protected set; }
        [AutoConvert]
        public Tuple<int, string>[] roundResultTypes { get; protected set; }
        [AutoConvert]
        public Tuple<int, string>[] battleResultTypes { get; protected set; }
        [AutoConvert]
        public Tuple<int, string>[] battleStatuses { get; protected set; }

        [AutoConvert]
        public Tuple<int, string>[] itemUseTargetTypes { get; protected set; }

        /// <summary>
        /// 英语特训
        /// </summary>
        [AutoConvert]
        public Tuple<int, string>[] correctTypes { get; protected set; }
        [AutoConvert]
        public Tuple<int, string>[] cardTypes { get; protected set; }
        [AutoConvert]
        public Tuple<int, string>[] enemyTypes { get; protected set; }

        /// <summary>
        /// 组合配置
        /// </summary>
        [AutoConvert]
        public Subject[] subjects { get; protected set; }
        [AutoConvert]
        public BaseParam[] baseParams { get; protected set; }
        [AutoConvert]
        public UsableItemType[] usableItemTypes { get; protected set; }
        [AutoConvert]
        public HumanEquipType[] humanEquipTypes { get; protected set; }
        [AutoConvert]
        public ExerEquipType[] exerEquipTypes { get; protected set; }
        [AutoConvert]
        public ExerStar[] exerStars { get; protected set; }
        [AutoConvert]
        public ExerGiftStar[] exerGiftStars { get; protected set; }
        [AutoConvert]
        public ItemStar[] itemStars { get; protected set; }
        [AutoConvert]
        public QuesStar[] quesStars { get; protected set; }
        [AutoConvert]
        public CompRank[] compRanks { get; protected set; }
        [AutoConvert]
        public ResultJudge[] resultJudges { get; protected set; }

		[AutoConvert]
		public GameTip[] gameTips { get; protected set; }

		[AutoConvert]
        public Antonym[] antonyms { get; protected set; }
        [AutoConvert]
        public ExerProItemStar[] exerProItemStars { get; protected set; }
        [AutoConvert]
        public NodeType[] nodeTypes { get; protected set; }
		[AutoConvert]
		public FirstCardGroup[] firstCardGroups { get; protected set; }
	}

	/// <summary>
	/// 游戏资料数据
	/// </summary>
	public class GameDatabase : BaseData {

        /// <summary>
        /// 数据库
        /// </summary>
        [AutoConvert]
        public Character[] characters { get; protected set; }
        [AutoConvert]
        public HumanItem[] humanItems { get; protected set; }
        [AutoConvert]
        public HumanEquip[] humanEquips { get; protected set; }
        [AutoConvert]
        public Exermon[] exermons { get; protected set; }
        [AutoConvert]
        public ExerFrag[] exerFrags { get; protected set; }
        [AutoConvert]
        public ExerSkill[] exerSkills { get; protected set; }
        [AutoConvert]
        public ExerGift[] exerGifts { get; protected set; }
        [AutoConvert]
        public ExerItem[] exerItems { get; protected set; }
        [AutoConvert]
        public ExerEquip[] exerEquips { get; protected set; }
        [AutoConvert]
        public QuesSugar[] quesSugars { get; protected set; }

        /// <summary>
        /// 英语特训
        /// </summary>
        [AutoConvert]
        public ExerProItem[] exerProItems { get; protected set; }
        [AutoConvert]
        public ExerProPotion[] exerProPotions { get; protected set; }
        [AutoConvert]
        public ExerProCard[] exerProCards { get; protected set; }
        [AutoConvert]
        public ExerProEnemy[] exerProEnemies { get; protected set; }
        [AutoConvert]
        public ExerProState[] exerProStates { get; protected set; }
        
        [AutoConvert]
        public ExerProMap[] exerProMaps { get; protected set; }

    }
}