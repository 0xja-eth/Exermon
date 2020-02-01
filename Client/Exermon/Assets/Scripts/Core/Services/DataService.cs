using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using LitJson;
using UnityEditor;

/// <summary>
/// 系统数据控制类
/// </summary>
/// <remarks>
/// 控制游戏系统数据的读取、刷新等
/// </remarks>
public class DataService : BaseService<DataService> {

    /// <summary>
    /// 操作文本设定
    /// </summary>
    public new const string FailTextFormat = "{0}发生错误，错误信息：\n{{0}}\n选择“重试”进行重试，选择“取消”退出游戏。";

    public const string Initializing = "初始化数据";
    public const string Refresh = "刷新数据";

    /// <summary>
    /// 业务操作
    /// </summary>
    public enum Oper {
        Static, Dynamic, Refresh
    }

    /// <summary>
    /// 状态
    /// </summary>
    public enum State {
        Unload,
        LoadingStatic,
        LoadingDynamic,
        Loaded,
    }
    public bool isLoaded() {
        return state == (int)State.Loaded;
    }

    /// <summary>
    /// 游戏数据
    /// </summary>
    public GameStaticData staticData { get; private set; } = new GameStaticData();
    public GameDynamicData dynamicData { get; private set; } = new GameDynamicData();

    /// <summary>
    /// 接受失败函数（初始加载用）
    /// </summary>
    UnityAction unacceptFunc = null;

    /// <summary>
    /// 外部系统
    /// </summary>
    StorageSystem storageSys;

    #region 初始化

    /// <summary>
    /// 初始化外部系统
    /// </summary>
    protected override void initializeSystems() {
        base.initializeSystems();
        storageSys = StorageSystem.get();
    }

    /// <summary>
    /// 初始化状态字典
    /// </summary>
    protected override void initializeStateDict() {
        base.initializeStateDict();
        addStateDict(State.Unload);
        addStateDict(State.LoadingStatic);
        addStateDict(State.LoadingDynamic);
        addStateDict(State.Loaded);
    }

    /// <summary>
    /// 初始化操作字典
    /// </summary>
    protected override void initializeOperDict() {
        base.initializeOperDict();
        addOperDict(Oper.Static, Initializing, NetworkSystem.Interfaces.LoadStaticData);
        addOperDict(Oper.Dynamic, Initializing, NetworkSystem.Interfaces.LoadDynamicData);
        addOperDict(Oper.Refresh, Refresh, NetworkSystem.Interfaces.LoadDynamicData);
    }

    /// <summary>
    /// 其他初始化工作
    /// </summary>
    protected override void initializeOthers() {
        base.initializeOthers();
        changeState(State.Unload);
    }

    #endregion

    #region 操作控制

    /// <summary>
    /// 读取数据
    /// </summary>
    /// <param name="unaccept">接受失败函数</param>
    public void load(UnityAction unaccept = null) {
        unacceptFunc = unaccept;
        switch ((State)state) {
            case State.Unload:
            case State.LoadingStatic:
                loadStaticData(); break;
            case State.LoadingDynamic:
                loadDynamicData(); break;
        }
    }

    /// <summary>
    /// 读取静态数据
    /// </summary>
    void loadStaticData() {
        var cached = isStaticDataCached();
        changeState(State.LoadingStatic);
        JsonData data = new JsonData();
        data["main_version"] = GameStaticData.LocalMainVersion;
        data["sub_version"] = GameStaticData.LocalSubVersion;
        data["cached"] = cached; // 是否有缓存

        sendRequest(Oper.Static, data, onStaticDataLoaded, 
            unacceptFunc, failFormat: FailTextFormat);
    }

    /// <summary>
    /// 本地静态数据是否已有缓存
    /// </summary>
    bool isStaticDataCached() {
        return staticData.isLoaded();
    }

    /// <summary>
    /// 读取动态数据（读取静态数据之后）
    /// </summary>
    /// <param name="data">静态数据</param>
    void loadDynamicData() {
        changeState(State.LoadingDynamic);
        sendRequest(Oper.Dynamic, null, onDynamicDataLoaded, 
            unacceptFunc, failFormat: FailTextFormat);
    }

    /// <summary>
    /// 刷新动态数据
    /// </summary>
    /// <param name="key">数据类型</param>
    /// <param name="accept">接受函数</param>
    /// <param name="unaccept">接受失败函数</param>
    /// <param name="waitText">等待文本</param>
    /// <param name="failText">失败文本</param>
    public void refreshDynamicData(string key, UnityAction accept, UnityAction unaccept = null) {
        Oper oper;
        // 成功回调，数据加载器
        NetworkSystem.RequestObject.SuccessAction onSuccess, loader;
        switch (key) {
            case "all":
                loader = dynamicData.load;
                oper = Oper.Refresh; break;
            default: return;
        }
        onSuccess = (data) => { loader.Invoke(data); accept.Invoke(); };
        sendRequest(oper, null, onSuccess, unaccept);
    }

    #endregion

    #region 获取数据

    /// <summary>
    /// 获取数据
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="collection">数据集合</param>
    /// <param name="id">ID</param>
    /// <returns>目标数据</returns>
    public static T get<T>(T[] collection, int id) where T : BaseData {
        foreach (var element in collection)
            if (element.getID() == id) return element;
        return default;
    }

    /// <summary>
    /// 获取数据
    /// </summary>
    /// <param name="collection">数据集合</param>
    /// <param name="id">ID</param>
    /// <returns>目标数据</returns>
    public static Tuple<int, string> get(Tuple<int, string>[] collection, int id) {
        foreach (var element in collection)
            if (element.Item1 == id) return element;
        return default;
    }

    /// <summary>
    /// 获取数据索引
    /// </summary>
    /// <param name="collection">数据集合</param>
    /// <param name="id">ID</param>
    /// <returns>目标数据索引</returns>
    public static int getIndex(Tuple<int, string>[] collection, int id) {
        for (int i = 0; i < collection.Length; ++i)
            if (collection[i].Item1 == id) return i;
        return -1;
    }

    /// <summary>
    /// 获取数据索引
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="collection">数据集合</param>
    /// <param name="id">ID</param>
    /// <returns>目标数据索引</returns>
    public static int getIndex<T>(T[] collection, int id) where T : BaseData {
        for (int i = 0; i < collection.Length; ++i)
            if (collection[i].getID() == id) return i;
        return -1;
    }

    /// <summary>
    /// 组合术语
    /// </summary>
    public Tuple<int, string> characterGender(int id) {
        return get(staticData.configure.characterGenders, id);
    }
    public Tuple<int, string> playerGrade(int id) {
        return get(staticData.configure.playerGrades, id);
    }
    public Tuple<int, string> playerStatus(int id) {
        return get(staticData.configure.playerStatuses, id);
    }
    public Tuple<int, string> playerType(int id) {
        return get(staticData.configure.playerTypes, id);
    }
    public Tuple<int, string> exermonType(int id) {
        return get(staticData.configure.exermonTypes, id);
    }
    public Tuple<int, string> exerSkillTargetType(int id) {
        return get(staticData.configure.exerSkillTargetTypes, id);
    }
    public Tuple<int, string> exerSkillHitType(int id) {
        return get(staticData.configure.exerSkillHitTypes, id);
    }

    /// <summary>
    /// 组合配置
    /// </summary>
    public Subject subject(int id) {
        return get(staticData.configure.subjects, id);
    }
    public BaseParam baseParam(int id) {
        return get(staticData.configure.baseParams, id);
    }
    public TypeData usableItemType(int id) {
        return get(staticData.configure.usableItemTypes, id);
    }
    public TypeData humanEquipType(int id) {
        return get(staticData.configure.humanEquipTypes, id);
    }
    public TypeData exerEquipType(int id) {
        return get(staticData.configure.exerEquipTypes, id);
    }
    public ExerStar exerStar(int id) {
        return get(staticData.configure.exerStars, id);
    }
    public ExerGiftStar exerGiftStar(int id) {
        return get(staticData.configure.exerGiftStars, id);
    }

    /// <summary>
    /// 数据库
    /// </summary>
    public Character character(int id) {
        return get(staticData.data.characters, id);
    }
    public Exermon exermon(int id) {
        return get(staticData.data.exermons, id);
    }
    public ExerFrag exerFrag(int id) {
        return get(staticData.data.exerFrags, id);
    }
    public ExerSkill exerSkill(int id) {
        return get(staticData.data.exerSkills, id);
    }
    public ExerGift exerGift(int id) {
        return get(staticData.data.exerGifts, id);
    }
    public HumanItem humanItem(int id) {
        return get(staticData.data.humanItems, id);
    }
    public HumanEquip humanEquip(int id) {
        return get(staticData.data.humanEquips, id);
    }
    public ExerItem exerItem(int id) {
        return get(staticData.data.exerItems, id);
    }
    public ExerEquip exerEquip(int id) {
        return get(staticData.data.exerEquips, id);
    }
    public ExerEquip quesSugar(int id) {
        return null;
        // return get(staticData.data.quesSugars, id);
    }

    #endregion

    #region 回调控制

    /// <summary>
    /// 静态数据读取回调
    /// </summary>
    /// <param name="data">静态数据</param>
    void onStaticDataLoaded(JsonData data) {
        staticData.load(DataLoader.loadJsonData(data, "data"));
        storageSys.save();
        loadDynamicData();
    }

    /// <summary>
    /// 初始化成功回调
    /// </summary>
    /// <param name="data">数据</param>
    void onDynamicDataLoaded(JsonData data) {
        dynamicData.load(DataLoader.loadJsonData(data, "data"));
        changeState(State.Loaded);
    }

    #endregion

}