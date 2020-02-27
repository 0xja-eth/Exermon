using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using LitJson;
using UnityEditor;

/// <summary>
/// 游戏静态配置数据
/// </summary>
public class GameStaticData : BaseData {

    /// <summary>
    /// 本地版本
    /// </summary>
    //public string localVersion = PlayerSettings.bundleVersion;
    public const string LocalMainVersion = "0.3.1";
    public const string LocalSubVersion = "20200126";

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
    GameConfigure _configure;
    public GameConfigure configure {
        get { return _configure; }
        private set { _configure = value; }
    }

    /// <summary>
    /// 游戏资料（数据库）
    /// </summary>
    GameDatabase _data;
    public GameDatabase data {
        get { return _data; }
        private set { _data = value; }
    }

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
        string updateNote = "当前版本：\n" + generateUpdateNote(curVersion);
        updateNote += "\n历史版本：\n";
        foreach (var ver in lastVersions)
            updateNote += generateUpdateNote(ver);
        return updateNote;
    }

    /// <summary>
    /// 生成单个版本的更新日志
    /// </summary>
    /// <param name="ver">版本对象</param>
    /// <returns>更新日志文本</returns>
    string generateUpdateNote(GameVersionData ver) {
        string time = ver.updateTime.ToString(DataLoader.SystemDateFormat);
        return string.Format(GameVersionData.UpdateNoteFormat, ver.mainVersion,
            ver.subVersion, time, ver.updateNote, ver.description);
    }

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    protected override void loadCustomAttributes(JsonData json) {
        base.loadCustomAttributes(json);

        Debug.Log("curVersion: " + curVersion + " (" + (curVersion == default) + ")");
        if (curVersion == default) return;

        // 如果没有版本变更且数据已读取（本地缓存），则直接返回
        if (curVersion.mainVersion == LocalMainVersion &&
            curVersion.subVersion == LocalSubVersion && loaded) return;

        configure = DataLoader.load(configure, json, "configure");
        data = DataLoader.load(data, json, "data");

        loaded = true;
    }
    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json"></param>
    public override void load(JsonData json) {
        Debug.Log("Loading static data:" + json.ToJson());
        Debug.Log("Loaded: " + loaded);
        base.load(json);
        curVersion = DataLoader.loadData<GameVersionData>(json, "cur_version");
        lastVersions = DataLoader.loadDataList<GameVersionData>(json, "last_versions");

        Debug.Log("Load end");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();
        json["cur_version"] = DataLoader.convertData(curVersion);
        json["last_versions"] = DataLoader.convertDataArray(lastVersions);
        json["configure"] = DataLoader.convertData(configure);
        json["data"] = DataLoader.convertData(data);

        return json;
    }*/
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
    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        seasons = DataLoader.loadDataList<CompSeason>(json, "seasons");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["seasons"] = DataLoader.convertDataArray(seasons);

        return json;
    }
    */
}

/// <summary>
/// 科目数据
/// </summary>
public class Subject : TypeData, ParamDisplay.DisplayDataConvertable {

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
        Debug.Log("BaseParam.convertToDisplayData: " + toJson().ToJson());
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
    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);
        color = DataLoader.loadColor(json, "color");
        maxScore = DataLoader.loadInt(json, "max_score");
        force = DataLoader.loadBool(json, "force");

        if (force) ForceCount++;
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();
        json["color"] = DataLoader.convertColor(color);
        json["max_score"] = maxScore;
        json["force"] = force;
        return json;
    }*/
}

/// <summary>
/// 基本能力数据
/// </summary>
public class BaseParam : TypeData, ParamDisplay.DisplayDataConvertable {

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
        Debug.Log("BaseParam.convertToDisplayData: " + toJson().ToJson());
        return toJson();
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
    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);
        color = DataLoader.loadColor(json, "color");
        maxValue = DataLoader.loadInt(json, "max_value");
        minValue = DataLoader.loadInt(json, "min_value");
        default_ = DataLoader.loadInt(json, "default");
        scale = DataLoader.loadInt(json, "scale");
        attr = DataLoader.loadString(json, "attr");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();
        json["color"] = DataLoader.convertColor(color);
        json["max_value"] = maxValue;
        json["min_value"] = minValue;
        json["default"] = default_;
        json["scale"] = scale;
        json["attr"] = attr;
        return json;
    }*/
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
    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);
        color = DataLoader.loadColor(json, "color");
        maxLevel = DataLoader.loadInt(json, "max_level");

        var paramRanges = DataLoader.loadJsonData(json, "param_ranges");

        baseRanges = DataLoader.loadDataArray<ParamRangeData>(paramRanges, "bases");
        rateRanges = DataLoader.loadDataArray<ParamRangeData>(paramRanges, "rates");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["color"] = DataLoader.convertColor(color);
        json["max_level"] = maxLevel;
        json["param_ranges"] = new JsonData();
        json["param_ranges"]["bases"] = DataLoader.convertDataArray(baseRanges);
        json["param_ranges"]["rates"] = DataLoader.convertDataArray(rateRanges);

        return json;
    }*/
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

    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);
        color = DataLoader.loadColor(json, "color");
        paramRanges = DataLoader.loadDataList<ParamRangeData>(json, "param_ranges");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();
        json["color"] = DataLoader.convertColor(color);
        json["param_ranges"] = DataLoader.convertDataArray(paramRanges);

        return json;
    }*/
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

    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);
        color = DataLoader.loadColor(json, "color");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();
        json["color"] = DataLoader.convertColor(color);

        return json;
    }*/
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

    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);
        color = DataLoader.loadColor(json, "color");
        level = DataLoader.loadInt(json, "level");
        weight = DataLoader.loadInt(json, "weight");
        expIncr = DataLoader.loadInt(json, "exp_incr");
        goldIncr = DataLoader.loadInt(json, "gold_incr");
        stdTime = DataLoader.loadInt(json, "std_time");
        minTime = DataLoader.loadInt(json, "min_time");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();
        json["color"] = DataLoader.convertColor(color);
        json["level"] = level;
        json["weight"] = weight;
        json["exp_incr"] = expIncr;
        json["gold_incr"] = goldIncr;
        json["std_time"] = stdTime;
        json["min_time"] = minTime;

        return json;
    }*/
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
    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json"></param>
    public override void load(JsonData json) {
        base.load(json);
        mainVersion = DataLoader.loadString(json, "main_version");
        subVersion = DataLoader.loadString(json, "sub_version");
        updateNote = DataLoader.loadString(json, "update_note");
        updateTime = DataLoader.loadDateTime(json, "update_time");
        description = DataLoader.loadString(json, "description");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();
        json["main_version"] = mainVersion;
        json["sub_version"] = subVersion;
        json["update_note"] = updateNote;
        json["update_time"] = DataLoader.convertDateTime(updateTime);
        json["description"] = description;
        return json;
    }*/
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
    public Tuple<int, string>[] recordSources { get; protected set; }
    [AutoConvert]
    public Tuple<int, string>[] exerciseGenTypes { get; protected set; }

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
    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json"></param>
    public override void load(JsonData json) {
        base.load(json);
        name = DataLoader.loadString(json, "name");
        engName = DataLoader.loadString(json, "eng_name");
        gold = DataLoader.loadString(json, "gold");
        ticket = DataLoader.loadString(json, "ticket");
        boundTicket = DataLoader.loadString(json, "bound_ticket");

        maxSubject = DataLoader.loadInt(json, "max_subject");
        maxExerciseCount = DataLoader.loadInt(json, "max_exercise_count");

        characterGenders = DataLoader.loadTupleArray(json, "character_genders");
        playerGrades = DataLoader.loadTupleArray(json, "player_grades");
        playerStatuses = DataLoader.loadTupleArray(json, "player_statuses");
        playerTypes = DataLoader.loadTupleArray(json, "player_types");

        exermonTypes = DataLoader.loadTupleArray(json, "exermon_types");
        exerSkillTargetTypes = DataLoader.loadTupleArray(json, "exerskill_target_types");
        exerSkillHitTypes = DataLoader.loadTupleArray(json, "exerskill_hit_types");

        questionTypes = DataLoader.loadTupleArray(json, "question_types");
        questionStatuses = DataLoader.loadTupleArray(json, "question_statuses");
        quesReportTypes = DataLoader.loadTupleArray(json, "ques_report_types");

        recordSources = DataLoader.loadTupleArray(json, "record_sources");
        exerciseGenTypes = DataLoader.loadTupleArray(json, "exercise_gen_types");

        subjects = DataLoader.loadDataArray<Subject>(json, "subjects");
        baseParams = DataLoader.loadDataArray<BaseParam>(json, "base_params");
        usableItemTypes = DataLoader.loadDataArray<UsableItemType>(json, "usable_item_types");
        humanEquipTypes = DataLoader.loadDataArray<HumanEquipType>(json, "human_equip_types");
        exerEquipTypes = DataLoader.loadDataArray<ExerEquipType>(json, "exer_equip_types");
        exerStars = DataLoader.loadDataArray<ExerStar>(json, "exer_stars");
        exerGiftStars = DataLoader.loadDataArray<ExerGiftStar>(json, "exer_gift_stars");
        itemStars = DataLoader.loadDataArray<ItemStar>(json, "item_stars");
        quesStars = DataLoader.loadDataArray<QuesStar>(json, "ques_stars");
        compRanks = DataLoader.loadDataArray<CompRank>(json, "comp_ranks");
    }
    
    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();
        json["name"] = name;
        json["eng_name"] = engName;
        json["gold"] = gold;
        json["ticket"] = ticket;
        json["bound_ticket"] = boundTicket;

        json["max_subject"] = maxSubject;
        json["max_exercise_count"] = maxExerciseCount;

        json["character_genders"] = DataLoader.convertTupleArray(characterGenders);
        json["player_grades"] = DataLoader.convertTupleArray(playerGrades);
        json["player_statuses"] = DataLoader.convertTupleArray(playerStatuses);
        json["player_types"] = DataLoader.convertTupleArray(playerTypes);

        json["exermon_types"] = DataLoader.convertTupleArray(exermonTypes);
        json["exerskill_target_types"] = DataLoader.convertTupleArray(exerSkillTargetTypes);
        json["exerskill_hit_types"] = DataLoader.convertTupleArray(exerSkillHitTypes);

        json["question_types"] = DataLoader.convertTupleArray(questionTypes);
        json["question_statuses"] = DataLoader.convertTupleArray(questionStatuses);
        json["ques_report_types"] = DataLoader.convertTupleArray(quesReportTypes);

        json["record_sources"] = DataLoader.convertTupleArray(recordSources);
        json["exercise_gen_types"] = DataLoader.convertTupleArray(exerciseGenTypes);

        json["subjects"] = DataLoader.convertDataArray(subjects);
        json["base_params"] = DataLoader.convertDataArray(baseParams);
        json["usable_item_types"] = DataLoader.convertDataArray(usableItemTypes);
        json["human_equip_types"] = DataLoader.convertDataArray(humanEquipTypes);
        json["exer_equip_types"] = DataLoader.convertDataArray(exerEquipTypes);
        json["exer_stars"] = DataLoader.convertDataArray(exerStars);
        json["exer_gift_stars"] = DataLoader.convertDataArray(exerGiftStars);
        json["item_stars"] = DataLoader.convertDataArray(itemStars);
        json["ques_stars"] = DataLoader.convertDataArray(quesStars);
        json["comp_ranks"] = DataLoader.convertDataArray(compRanks);

        return json;
    }
    */
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
    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json"></param>
    public override void load(JsonData json) {
        base.load(json);

        characters = DataLoader.loadDataArray<Character>(json, "characters");
        humanItems = DataLoader.loadDataArray<HumanItem>(json, "human_items");
        humanEquips = DataLoader.loadDataArray<HumanEquip>(json, "human_equips");
        exermons = DataLoader.loadDataArray<Exermon>(json, "exermons");
        exerFrags = DataLoader.loadDataArray<ExerFrag>(json, "exer_frags");
        exerSkills = DataLoader.loadDataArray<ExerSkill>(json, "exer_skills");
        exerGifts = DataLoader.loadDataArray<ExerGift>(json, "exer_gifts");
        exerItems = DataLoader.loadDataArray<ExerItem>(json, "exer_items");
        exerEquips = DataLoader.loadDataArray<ExerEquip>(json, "exer_equips");
        quesSugars = DataLoader.loadDataArray<QuesSugar>(json, "ques_sugars");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["characters"] = DataLoader.convertDataArray(characters);
        json["human_items"] = DataLoader.convertDataArray(humanItems);
        json["human_equips"] = DataLoader.convertDataArray(humanEquips);
        json["exermons"] = DataLoader.convertDataArray(exermons);
        json["exer_frags"] = DataLoader.convertDataArray(exerFrags);
        json["exer_skills"] = DataLoader.convertDataArray(exerSkills);
        json["exer_gifts"] = DataLoader.convertDataArray(exerGifts);
        json["exer_items"] = DataLoader.convertDataArray(exerItems);
        json["exer_equips"] = DataLoader.convertDataArray(exerEquips);
        json["ques_sugars"] = DataLoader.convertDataArray(quesSugars);

        return json;
    }
    */
}