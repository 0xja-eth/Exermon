using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using LitJson;
using UnityEditor;

/// <summary>
/// 游戏数据父类
/// </summary>
public class BaseData {
    int id; // ID（只读）
    public int getID() { return id; }

    /// <summary>
    /// 是否需要ID
    /// </summary>
    protected bool idEnable = true;

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    /// <param name="idEnable">是否可用ID字段</param>
    public virtual void load(JsonData json) {
        id = idEnable ? DataLoader.loadInt(json, "id") : -1;
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public virtual JsonData toJson() {
        var json = new JsonData();
        json.SetJsonType(JsonType.Object);
        if (idEnable) json["id"] = id;
        return json;
    }
}

/// <summary>
/// 游戏静态配置数据
/// </summary>
public class GameStaticData : BaseData {

    /// <summary>
    /// 本地版本
    /// </summary>
    //public string localVersion = PlayerSettings.bundleVersion;
    public static string localMainVersion = "0.3.1";
    public static string localSubVersion = "0";

    /// <summary>
    /// 游戏版本数据
    /// </summary>
    public class GameVersionData : BaseData {

        /// <summary>
        /// 更新日志格式
        /// </summary>
        public const string UpdateNoteFormat = "版本号：{1}.{2}\n更新日期：{0}\n更新内容：\n{3}\n";

        /// <summary>
        /// 更新时间格式
        /// </summary>
        public const string UpdateTimeFormat = "yyyy-MM-dd";

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
    /// 后台版本
    /// </summary>
    public GameVersionData curVersion { get; private set; }

    /// <summary>
    /// 历史版本
    /// </summary>
    public List<GameVersionData> lastVersions { get; private set; }

    /// <summary>
    /// 读取标志
    /// </summary>
    bool loaded = false;
    public bool isLoaded() { return loaded; }

    /// <summary>
    /// 具体数据
    /// </summary>
    public Tuple<int, string>[] playerGenders { get; private set; }
    public Tuple<int, string>[] playerGrades { get; private set; }
    public Tuple<int, string>[] playerStatuses { get; private set; }
    public Tuple<int, string>[] playerTypes { get; private set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    public GameStaticData() { idEnable = false; }

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
        string time = ver.updateTime.ToString(GameVersionData.UpdateTimeFormat);
        return string.Format(GameVersionData.UpdateNoteFormat, ver.mainVersion, 
            ver.subVersion, time, ver.updateNote, ver.description);
    }

    /// <summary>
    /// 搜索数据
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="collection">数据集合</param>
    /// <param name="id">ID</param>
    /// <returns>目标数据</returns>
    public T findData<T>(T[] collection, int id) where T : BaseData {
        foreach (var element in collection)
            if (element.getID() == id) return element;
        return default;
    }

    /// <summary>
    /// 搜索数据
    /// </summary>
    /// <param name="collection">数据集合</param>
    /// <param name="id">ID</param>
    /// <returns>目标数据</returns>
    public Tuple<int, string> findData(Tuple<int, string>[] collection, int id) {
        foreach (var element in collection)
            if (element.Item1 == id) return element;
        return default;
    }

    /// <summary>
    /// 搜索具体数据
    /// </summary>


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
        if (curVersion.mainVersion == localMainVersion &&
            curVersion.subVersion == localSubVersion && loaded) return;

        playerGenders = DataLoader.loadTupleArray(json, "player_genders");
        playerGrades = DataLoader.loadTupleArray(json, "player_grades");
        playerStatuses = DataLoader.loadTupleArray(json, "player_statuses");
        playerTypes = DataLoader.loadTupleArray(json, "player_types");

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
        
        json["player_genders"] = DataLoader.convertTupleArray(playerGenders);
        json["player_grades"] = DataLoader.convertTupleArray(playerGrades);
        json["player_statuses"] = DataLoader.convertTupleArray(playerStatuses);
        json["player_types"] = DataLoader.convertTupleArray(playerTypes);

        return json;
    }
}

/// <summary>
/// 游戏动态数据
/// </summary>
public class GameDynamicData : BaseData {

    /// <summary>
    /// 构造函数
    /// </summary>
    public GameDynamicData() { idEnable = false; }

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
