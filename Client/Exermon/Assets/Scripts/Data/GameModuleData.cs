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
    public GameVersionData curVersion { get; private set; }

    /// <summary>
    /// 历史版本
    /// </summary>
    public List<GameVersionData> lastVersions { get; private set; }

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
    /// <param name="json"></param>
    public override void load(JsonData json) {
        Debug.Log("Loading static data:" + json.ToJson());
        Debug.Log("Loaded: " + loaded);
        base.load(json);
        curVersion = DataLoader.loadData<GameVersionData>(json, "cur_version");
        lastVersions = DataLoader.loadDataList<GameVersionData>(json, "last_versions");

        Debug.Log("curVersion: " + curVersion + " (" + (curVersion == default) + ")");
        if (curVersion == default) return;

        // 如果没有版本变更且数据已读取（本地缓存），则直接返回
        if (curVersion.mainVersion == LocalMainVersion &&
            curVersion.subVersion == LocalSubVersion && loaded) return;

        DataLoader.loadData(ref _configure, json, "configure");
        DataLoader.loadData(ref _data, json, "data");

        loaded = true;
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
    /// 搜索数据
    /// </summary>
    /// <param name="data">数据集合</param>
    /// <param name="id">ID</param>
    /// <returns>目标数据</returns>
    public T findData<T>(List<T> data, int id) where T : BaseData {
        return data.Find((d) => d.getID() == id);
    }

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();
        return json;
    }

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
    public Color color { get; private set; }
    public int maxScore { get; private set; }
    public bool force { get; private set; }

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
    }
}

/// <summary>
/// 基本能力数据
/// </summary>
public class BaseParam : TypeData, ParamDisplay.DisplayDataConvertable {

    /// <summary>
    /// 属性
    /// </summary>
    public Color color { get; private set; }
    public int maxValue { get; private set; }
    public int minValue { get; private set; }
    public int default_ { get; private set; }
    public int scale { get; private set; }
    public string attr { get; private set; }

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
    }
}

/// <summary>
/// 艾瑟萌星级数据
/// </summary>
public class ExerStar : TypeData {

    /// <summary>
    /// 属性
    /// </summary>
    public Color color { get; private set; }
    public int maxLevel { get; private set; }
    public ParamRangeData[] baseRanges { get; private set; }
    public ParamRangeData[] rateRanges { get; private set; }

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
    }
}

/// <summary>
/// 艾瑟萌天赋星级数据
/// </summary>
public class ExerGiftStar : TypeData {

    /// <summary>
    /// 属性
    /// </summary>
    public Color color { get; private set; }
    public List<ParamRangeData> paramRanges { get; private set; }

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
    }
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
    public string mainVersion { get; private set; }
    public string subVersion { get; private set; }
    public string updateNote { get; private set; }
    public DateTime updateTime { get; private set; }
    public string description { get; private set; }

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
    }
}

/// <summary>
/// 游戏系统配置数据
/// </summary>
public class GameConfigure : BaseData {

    /// <summary>
    /// 基本术语
    /// </summary>
    public string name { get; private set; }
    public string engName { get; private set; }
    public string gold { get; private set; }
    public string ticket { get; private set; }
    public string boundTicket { get; private set; }

    /// <summary>
    /// 配置量
    /// </summary>
    public int maxSubject { get; private set; }

    /// <summary>
    /// 组合术语
    /// </summary>
    public Tuple<int, string>[] characterGenders { get; private set; }
    public Tuple<int, string>[] playerGrades { get; private set; }
    public Tuple<int, string>[] playerStatuses { get; private set; }
    public Tuple<int, string>[] playerTypes { get; private set; }

    public Tuple<int, string>[] exermonTypes { get; private set; }
    public Tuple<int, string>[] exerSkillTargetTypes { get; private set; }
    public Tuple<int, string>[] exerSkillHitTypes { get; private set; }

    /// <summary>
    /// 组合配置
    /// </summary>
    public Subject[] subjects { get; private set; }
    public BaseParam[] baseParams { get; private set; }
    public TypeData[] usableItemTypes { get; private set; }
    public TypeData[] humanEquipTypes { get; private set; }
    public TypeData[] exerEquipTypes { get; private set; }
    public ExerStar[] exerStars { get; private set; }
    public ExerGiftStar[] exerGiftStars { get; private set; }

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

        characterGenders = DataLoader.loadTupleArray(json, "character_genders");
        playerGrades = DataLoader.loadTupleArray(json, "player_grades");
        playerStatuses = DataLoader.loadTupleArray(json, "player_statuses");
        playerTypes = DataLoader.loadTupleArray(json, "player_types");

        exermonTypes = DataLoader.loadTupleArray(json, "exermon_types");
        exerSkillTargetTypes = DataLoader.loadTupleArray(json, "exerskill_target_types");
        exerSkillHitTypes = DataLoader.loadTupleArray(json, "exerskill_hit_types");

        subjects = DataLoader.loadDataArray<Subject>(json, "subjects");
        baseParams = DataLoader.loadDataArray<BaseParam>(json, "base_params");
        usableItemTypes = DataLoader.loadDataArray<TypeData>(json, "usable_item_types");
        humanEquipTypes = DataLoader.loadDataArray<TypeData>(json, "human_equip_types");
        exerEquipTypes = DataLoader.loadDataArray<TypeData>(json, "exer_equip_types");
        exerStars = DataLoader.loadDataArray<ExerStar>(json, "exer_stars");
        exerGiftStars = DataLoader.loadDataArray<ExerGiftStar>(json, "exer_gift_stars");
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

        json["character_genders"] = DataLoader.convertTupleArray(characterGenders);
        json["player_grades"] = DataLoader.convertTupleArray(playerGrades);
        json["player_statuses"] = DataLoader.convertTupleArray(playerStatuses);
        json["player_types"] = DataLoader.convertTupleArray(playerTypes);

        json["exermon_types"] = DataLoader.convertTupleArray(exermonTypes);
        json["exerskill_target_types"] = DataLoader.convertTupleArray(exerSkillTargetTypes);
        json["exerskill_hit_types"] = DataLoader.convertTupleArray(exerSkillHitTypes);

        json["subjects"] = DataLoader.convertDataArray(subjects);
        json["base_params"] = DataLoader.convertDataArray(baseParams);
        json["usable_item_types"] = DataLoader.convertDataArray(usableItemTypes);
        json["human_equip_types"] = DataLoader.convertDataArray(humanEquipTypes);
        json["exer_equip_types"] = DataLoader.convertDataArray(exerEquipTypes);
        json["exer_stars"] = DataLoader.convertDataArray(exerStars);
        json["exer_gift_stars"] = DataLoader.convertDataArray(exerGiftStars);

        return json;
    }

}

/// <summary>
/// 游戏资料数据
/// </summary>
public class GameDatabase : BaseData {
    
    /// <summary>
    /// 数据库
    /// </summary>
    public Character[] characters { get; private set; }
    public Exermon[] exermons { get; private set; }
    public ExerFrag[] exerFrags { get; private set; }
    public ExerSkill[] exerSkills { get; private set; }
    public ExerGift[] exerGifts { get; private set; }
    public HumanItem[] humanItems { get; private set; }
    public HumanEquip[] humanEquips { get; private set; }
    public ExerItem[] exerItems { get; private set; }
    public ExerEquip[] exerEquips { get; private set; }

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json"></param>
    public override void load(JsonData json) {
        base.load(json);

        characters = DataLoader.loadDataArray<Character>(json, "characters");
        exermons = DataLoader.loadDataArray<Exermon>(json, "exermons");
        exerFrags = DataLoader.loadDataArray<ExerFrag>(json, "exer_frags");
        exerSkills = DataLoader.loadDataArray<ExerSkill>(json, "exer_skills");
        exerGifts = DataLoader.loadDataArray<ExerGift>(json, "exer_gifts");
        exerItems = DataLoader.loadDataArray<ExerItem>(json, "exer_items");
        exerEquips = DataLoader.loadDataArray<ExerEquip>(json, "exer_equips");
        humanItems = DataLoader.loadDataArray<HumanItem>(json, "human_items");
        humanEquips = DataLoader.loadDataArray<HumanEquip>(json, "human_equips");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["characters"] = DataLoader.convertDataArray(characters);
        json["exermons"] = DataLoader.convertDataArray(exermons);
        json["exer_frags"] = DataLoader.convertDataArray(exerFrags);
        json["exer_skills"] = DataLoader.convertDataArray(exerSkills);
        json["exer_gifts"] = DataLoader.convertDataArray(exerGifts);
        json["exer_items"] = DataLoader.convertDataArray(exerItems);
        json["exer_equips"] = DataLoader.convertDataArray(exerEquips);
        json["human_items"] = DataLoader.convertDataArray(humanItems);
        json["human_equips"] = DataLoader.convertDataArray(humanEquips);

        return json;
    }

}