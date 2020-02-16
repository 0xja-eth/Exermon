using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using LitJson;

/// <summary>
/// 业务控制类（父类）（单例模式）
/// </summary>
public class BaseService<T> : BaseSystem<T> where T : BaseService<T>, new()  {

    /// <summary>
    /// 操作文本设定
    /// </summary>
    public const string WaitTextFormat = "{0}中...";
    public const string FailTextFormat = "{0}失败：\n{{0}}";

    /// <summary>
    /// 操作字典 (key, (oper, route))
    /// </summary>
    Dictionary<int, Tuple<string, string>> operDict;

    /// <summary>
    /// 外部系统
    /// </summary>
    NetworkSystem networkSys;
    protected GameSystem gameSys;

    #region 操作字典

    /// <summary>
    /// 其他初始化工作
    /// </summary>
    protected override void initializeOthers() {
        base.initializeOthers();
        initializeOperDict();
    }

    /// <summary>
    /// 初始化操作字典
    /// </summary>
    protected virtual void initializeOperDict() {
        operDict = new Dictionary<int, Tuple<string, string>>();
    }

    /// <summary>
    /// 初始化外部系统
    /// </summary>
    protected override void initializeSystems() {
        base.initializeSystems();
        networkSys = NetworkSystem.get();
        gameSys = GameSystem.get();
    }

    /// <summary>
    /// 添加操作字典
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="oper">操作</param>
    /// <param name="route">路由</param>
    protected void addOperDict(int key, string oper, string route) {
        operDict.Add(key, new Tuple<string, string>(oper, route));
    }
    protected void addOperDict(string oper, string route) {
        addOperDict(operDict.Count, oper, route);
    }
    protected void addOperDict(Enum key, string oper, string route) {
        addOperDict(key.GetHashCode(), oper, route);
    }

    /// <summary>
    /// 获取指定名称的接口路由
    /// </summary>
    /// <param name="name">接口名</param>
    /// <returns>路由</returns>
    protected string getRoute(string name) {
        var type = typeof(NetworkSystem.Interfaces);
        return (string)type.GetField(name).GetValue(null);
    }

    #endregion

    #region 请求控制

    /// <summary>
    /// 发送请求（带有重试功能）
    /// </summary>
    /// <param name="key">操作字典键（枚举类型）</param>
    /// <param name="data">数据</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    /// <param name="uid">是否需要携带玩家信息</param>
    protected void sendRequest(Enum key, JsonData data,
        NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null,
        string waitFormat = WaitTextFormat, string failFormat = FailTextFormat, bool uid = false) {
        sendRequest(key.GetHashCode(), data, onSuccess, onError, waitFormat, failFormat, uid);
    }

    /// <summary>
    /// 发送请求（带有重试功能）
    /// 将会自动根据操作字典键确定操作文本
    /// </summary>
    /// <param name="key">操作字典键</param>
    /// <param name="data">数据</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    protected void sendRequest(int key, JsonData data,
        NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null,
        string waitFormat = WaitTextFormat, string failFormat = FailTextFormat, bool uid = false) {
        if (operDict.ContainsKey(key)) {
            var tuple = operDict[key];
            sendRequest(tuple.Item2, data, tuple.Item1, 
                onSuccess, onError, waitFormat, failFormat, uid);
        } else Debug.LogError("未找到操作键 " + key + "，请检查操作字典");
    }

    /// <summary>
    /// 发送请求（带有重试功能）
    /// 将会自动根据操作文本确定错误文本和等待文本
    /// </summary>
    /// <param name="route">路由</param>
    /// <param name="data">数据</param>
    /// <param name="oper">操作文本</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    protected void sendRequest(string route, JsonData data, string oper,
        NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null,
        string waitFormat = WaitTextFormat, string failFormat = FailTextFormat, bool uid = false) {
        sendRequest(route, data, string.Format(waitFormat, oper), 
            string.Format(failFormat, oper), onSuccess, onError, uid);
    }

    /// <summary>
    /// 发送请求（带有重试功能）
    /// </summary>
    /// <param name="route">路由</param>
    /// <param name="data">数据</param>
    /// <param name="waitText">等待文本</param>
    /// <param name="failText">错误文本</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    protected void sendRequest(string route, JsonData data, string waitText, string failText,
        NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null, bool uid = false) {

        NetworkSystem.RequestObject.ErrorAction _onError = generateOnErrorFunc(failText,
            () => sendRequest(route, data, waitText, failText,
                onSuccess, onError), onError);
        if (uid) data["uid"] = getPlayerID(); // 添加玩家信息
        networkSys.setupRequest(route, data, onSuccess, _onError, true, waitText);
    }

    /// <summary>
    /// 生成实际的 onError 函数
    /// </summary>
    /// <param name="format">文本格式</param>
    /// <param name="retry">重试函数</param>
    /// <param name="onError">失败回调</param>
    /// <returns>onError 函数</returns>
    NetworkSystem.RequestObject.ErrorAction generateOnErrorFunc(string format, 
        UnityAction retry, UnityAction onError = null) {
        return (status, errmsg) => {
            var text = string.Format(format, errmsg);
            var btns = AlertWindow.RetryOrCancel;
            var actions = new UnityAction[] {
                null, retry, onError
            };
            gameSys.requestAlert(text, btns, actions);
        };
    }

    #endregion

    #region 其他静态函数

    /// <summary>
    /// 获取当前玩家实例
    /// </summary>
    /// <returns>玩家实例</returns>
    public static Player getPlayer() {
        return PlayerService.get().player;
    }

    /// <summary>
    /// 获取当前玩家ID
    /// </summary>
    /// <returns>玩家ID</returns>
    public static int getPlayerID() {
        var player = getPlayer();
        return player == null ? -1 : player.getID();
    }

    #endregion

}