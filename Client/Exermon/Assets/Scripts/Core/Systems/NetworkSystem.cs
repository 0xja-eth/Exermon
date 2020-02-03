
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using BestHTTP;
using BestHTTP.WebSocket;
using LitJson;

/// <summary>
/// 网络系统控制类
/// </summary>
public class NetworkSystem : BaseSystem<NetworkSystem> {

    /// <summary>
    /// 接口管理类
    /// </summary>
    public static class Interfaces {

        /// <summary>
        /// 服务器地址配置
        /// </summary>
        //public const string ServerURL = "ws://120.78.15.85:8001/game/";
        public const string ServerURL = "ws://127.0.0.1:8000/game/";

        /// <summary>
        /// 路由配置
        /// </summary>
        public const string LoadStaticData = "game/data/static";
        public const string LoadDynamicData = "game/data/dynamic";

        public const string PlayerRegister = "player/player/register";
        public const string PlayerLogin = "player/player/login";
        public const string PlayerForget = "player/player/forget";
        public const string PlayerCode = "player/player/code";

        public const string PlayerLogout = "player/player/logout";
        public const string PlayerGetBasic = "player/get/basic";

        public const string PlayerCreateCharacter = "player/create/character";
        public const string PlayerCreateExermons = "player/create/exermons";
        public const string PlayerCreateGifts = "player/create/gifts";
        public const string PlayerCreateInfo = "player/create/info";

        public const string PlayerEquipSlotEquip = "player/equipslot/equip";

        public const string ItemGetPack = "item/packcontainer/get";
        public const string ItemGetSlot = "item/slotcontainer/get";
        public const string ItemGainItem = "item/packcontainer/gain";
        public const string ItemTransferItem = "item/packcontainer/transfer";
        public const string ItemSplitItem = "item/packcontainer/split";
        public const string ItemMergeItem = "item/packcontainer/merge";

        public const string ExermonExerSlotEquip = "exermon/exerslot/equip";
        public const string ExermonEquipSlotEquip = "exermon/equipslot/equip";

    }

    /// <summary>
    /// 请求对象
    /// </summary>
    public class RequestObject {
        /// <summary>
        /// 回调函数类型
        /// </summary>
        public delegate void SuccessAction(JsonData data);
        public delegate void ErrorAction(int status, string errmsg);

        /// <summary>
        /// 请求参数
        /// </summary>
        public JsonData sendData; // 发送信息
        public SuccessAction onSuccess; // 成功回调
        public ErrorAction onError; // 失败回调
        public bool showLoading; // 是否显示Loading界面
        public string tipsText; // Loading界面提示文本

        /// <summary>
        /// 初始化WS请求对象
        /// </summary>
        /// <param name="success"></param>
        /// <param name="success">成功回调</param>
        /// <param name="error">失败回调</param>
        /// <param name="show">是否显示Loading界面</param>
        /// <param name="tips">Loading界面提示文本</param>
        public RequestObject(JsonData data, SuccessAction success = null, ErrorAction error = null,
            bool show = true, string tips = "") {
            sendData = data; onSuccess = success; onError = error;
            showLoading = show; tipsText = tips;
        }
    }

    /// <summary>
    /// 文本设定
    /// </summary>
    const string ServerNoResponse = "服务器无响应";
    const string ServerDisconnected = "服务器断开连接";
    const string DefaultErrorFormat = "未知错误：{0}\n详细信息：{1}";

    const string ServerError = "服务器错误！";
    const string DefaultDisconnectFormat = "{0}：{1}";

    /// <summary>
    /// 默认回调参数配置
    /// </summary>
    static readonly RequestObject.SuccessAction DefaultSuccessHandler =
        (data) => { Debug.Log(data.ToJson()); };
    static readonly RequestObject.ErrorAction DefaultErrorHandler =
        (status, errmsg) => { Debug.LogError(status + ": " + errmsg); };
    static readonly RequestObject.ErrorAction EmitErrorHandler =
        (status, errmsg) => { Debug.LogError(status + ": " + errmsg); };

    /// <summary>
    /// WebSocket对象
    /// </summary>
    WebSocket webSocket;

    /// <summary>
    /// Socket名字
    /// </summary>
    public string socketName;

    /// <summary>
    /// 请求计数
    /// </summary>
    static int requestCnt = 0;

    /// <summary>
    /// 请求队列声明
    /// </summary>
    static Dictionary<string, RequestObject> requests = new Dictionary<string, RequestObject>();

    /// <summary>
    /// 响应数据处理类
    /// </summary>
    public static class ResponseDataHandler {

        /// <summary>
        /// 处理接收的响应
        /// </summary>
        /// <param name="data">接收响应数据</param>
        public static void handleResponse(JsonData data) {
            Debug.Log("handleResponse: " + data.ToJson());

            string route = DataLoader.loadString(data, "route");
            int index = DataLoader.loadInt(data, "index");
            int status = DataLoader.loadInt(data, "status");
            string errmsg = DataLoader.loadString(data, "errmsg");
            JsonData _data = DataLoader.loadJsonData(data, "data");

            processResponseData(route, index, status, errmsg, _data);
        }

        /// <summary>
        /// 处理接收发送数据
        /// </summary>
        /// <param name="type">发送类型</param>
        /// <param name="data">发送数据</param>
        public static void processResponseData(string route, int index, 
            int status, string errmsg, JsonData data) {
            var sys = get();

            RequestObject req = sys.popRequestObject(route, index);
            RequestObject.SuccessAction onSuccess = req.onSuccess;
            RequestObject.ErrorAction onError = req.onError;

            // 如果没有剩下的 ReqObj 了，清除加载窗口
            if (!sys.hasRequestObject()) sys.gameSys.requestLoadEnd();

            if (req == null) throw new GameException(
                GameException.Type.RequestObjectNotFound, onError);
            if (status > 0) throw new GameException(status, errmsg, onError);
            onSuccess.Invoke(data);
        }

    }

    /// <summary>
    /// 发射数据处理类
    /// </summary>
    public static class EmitDataHandler {

        /// <summary>
        /// 处理接收的发射信息
        /// </summary>
        /// <param name="data">接收发送数据</param>
        public static void handleEmit(JsonData data) {
            string type = DataLoader.loadString(data, "type");
            int status = DataLoader.loadInt(data, "status");
            if (status > 0) throw new GameException(status, DataLoader.loadString(data, "errmsg"), EmitErrorHandler);
            processEmitData(type, data["data"]);
        }

        /// <summary>
        /// 处理接收发送数据
        /// </summary>
        /// <param name="type">发送类型</param>
        /// <param name="data">发送数据</param>
        public static void processEmitData(string type, JsonData data) {
            Debug.Log("processEmitData: " + type + ": " + data.ToJson());
            Debug.Log("Self.socketName = " + get().socketName);

            switch (type) {
                case "link": processLink(data); break;
                case "disconnect": processDisconnect(data); break;
            }
        }

        /// <summary>
        /// 处理连接数据
        /// </summary>
        /// <param name="data">发送数据</param>
        static void processLink(JsonData data) {
            get().socketName = DataLoader.loadString(data, "channel_name");
        }

        /// <summary>
        /// 处理断开连接数据
        /// </summary>
        /// <param name="data">发送数据</param>
        static void processDisconnect(JsonData data) {
            var channelName = DataLoader.loadString(data, "channel_name");
            var code = DataLoader.loadInt(data, "code");
            var message = DataLoader.loadString(data, "message");

            if (channelName == get().socketName)
                onSelfDisconnect(code, message);
            else
                onOtherDisconnect(channelName, code, message);
        }

        /// <summary>
        /// 断开自身连接
        /// </summary>
        /// <param name="code">断开连接码</param>
        /// <param name="message">断开连接信息</param>
        static void onSelfDisconnect(int code, string message) {
            get().handleDisconnect(-1, message);
        }
        /// <summary>
        /// 断开他人连接
        /// </summary>
        /// <param name="name">对方的 channel name</param>
        /// <param name="code">断开连接码</param>
        /// <param name="message">断开连接信息</param>
        static void onOtherDisconnect(string name, int code, string message) {

        }

    }

    /// <summary>
    /// 状态
    /// </summary>
    public enum State {
        Connecting, // 连接中
        Connected, // 已连接
        Disconnecting, // 断开连接中
        Disconnected, // 已断开连接
        Error, // 连接错误
    }
    public bool isConnected() {
        return state == (int)State.Connected;
    }
    public bool isDisconnected() {
        return state == (int)State.Disconnected;
    }
    public bool isError() {
        return state == (int)State.Error;
    }

    /// <summary>
    /// 改变状态
    /// </summary>
    /// <param name="state">状态枚举</param>
    public void changeState(State state, Tuple<int, string> info = null) {
        if (info != null) stateInfo = info;
        base.changeState(state);
    }

    /// <summary>
    /// 状态信息（错误状态/断开连接状态）
    /// </summary>
    public Tuple<int, string> stateInfo { get; private set; } = null;
    public void clearStateInfo() { stateInfo = null; }

    /// <summary>
    /// 回调参数声明
    /// </summary>
    public RequestObject.SuccessAction successHandler { get; set; } = DefaultSuccessHandler;
    public RequestObject.ErrorAction errorHandler { get; set; } = DefaultErrorHandler;
    public bool showLoading { get; set; } = true;
    public string tipsText { get; set; } = "";

    /// <summary>
    /// 外部系统
    /// </summary>
    GameSystem gameSys;

    #region 初始化

    /// <summary>
    /// 初始化状态字典
    /// </summary>
    protected override void initializeStateDict() {
        base.initializeStateDict();
        addStateDict(State.Connecting);
        addStateDict(State.Connected);
        addStateDict(State.Disconnecting);
        addStateDict(State.Disconnected);
        addStateDict(State.Error);
    }

    /// <summary>
    /// 初始化外部系统
    /// </summary>
    protected override void initializeSystems() {
        gameSys = GameSystem.get();
    }

    #endregion

    // 配置请求对象属性
    #region 配置请求配置

    /// <summary>
    /// 统一配置回调参数
    /// </summary>
    /// <param name="success">成功回调</param>
    /// <param name="error">错误回调</param>
    /// <param name="show">是否显示加载</param>
    /// <param name="tips">加载提示文本</param>
    public void setup(
        RequestObject.SuccessAction success = null,
        RequestObject.ErrorAction error = null,
        bool show = true, string tips = "") {
        successHandler = success;
        errorHandler = error;
        showLoading = show;
        tipsText = tips;
    }

    /// <summary>
    /// 清除配置
    /// </summary>
    public void clear() {
        successHandler = null;
        errorHandler = null;
        showLoading = true;
        tipsText = "";
    }

    #endregion

    // 初始化回调
    #region 回调配置

    /// <summary>
    /// 配置WS
    /// </summary>
    void setupWebSocket() {
        webSocket = new WebSocket(new Uri(Interfaces.ServerURL));
        webSocket.OnOpen += onConnected;
        webSocket.OnMessage += onReceived;
        webSocket.OnClosed += onDisconnected;
        webSocket.OnError += onError;
    }

    /// <summary>
    /// 连接成功回调
    /// </summary>
    /// <param name="ws">WebSocket对象</param>
    void onConnected(WebSocket ws) {
        Debug.Log("onConnected");
        changeState(State.Connected);
        resendAllRequest();
    }

    /// <summary>
    /// 接收回调
    /// </summary>
    /// <param name="ws">WebSocket对象</param>
    /// <param name="message">接收数据字符串</param>
    void onReceived(WebSocket ws, string message) {
        Debug.Log("onReceived: " + message);
        try {
            JsonData data = JsonMapper.ToObject(message);
            switch (DataLoader.loadString(data, "method")) {
                // 接收处理请求响应（模仿HTTP）
                case "response": ResponseDataHandler.handleResponse(data); break;
                // 接收服务器主动发射的消息
                case "emit": EmitDataHandler.handleEmit(data); break;
            }
        } catch (GameException e) {
            handleProcessError(e.code, e.message, e.action);
        } /*catch (Exception e) {
            handleProcessError((int)GameException.Type.SystemError, "系统错误："+e);
        }*/
    }

    /// <summary>
    /// 错误回调
    /// </summary>
    /// <param name="ws">WebSocket对象</param>
    /// <param name="ex">异常对象</param>
    void onError(WebSocket ws, Exception ex) {
        Debug.Log("onError: " + ws + " " + ex);
        int code = -1; string errmsg = "";
        
        //#if !UNITY_WEBGL || UNITY_EDITOR
        if (ws.InternalRequest.Response != null) {
            code = ws.InternalRequest.Response.StatusCode;
            errmsg = ws.InternalRequest.Response.Message;
        }
        //#endif
        
        handleConnectError(code, errmsg);
    }
    
    /// <summary>
    /// 断开连接回调
    /// </summary>
    /// <param name="ws">WebSocket对象</param>
    /// <param name="code">状态码</param>
    /// <param name="message">断开连接信息</param>
    void onDisconnected(WebSocket ws, ushort code, string message) {
        //if (GameSystem.isGameEnding()) return;
        Debug.Log("onDisconnected: " + code + ": " + message);
        handleDisconnect(code, message);
    }

    #endregion

    // 管理WS请求对象
    #region WS请求处理

    /// <summary>
    /// WS请求队列是否不为空
    /// </summary>
    /// <returns>是否不为空</returns>
    bool hasRequestObject() {
        return requests.Count > 0;
    }

    /// <summary>
    /// 获取指定路由的请求
    /// </summary>
    /// <param name="route">路由</param>
    /// <returns>请求对象</returns>
    RequestObject getRequestObject(string route, int index) {
        var key = generateKey(route, index);
        if (requests.ContainsKey(key))
            return requests[key];
        return null;
    }

    /// <summary>
    /// 配置并添加请求
    /// </summary>
    /// <param name="route">路由</param>
    /// <param name="success">成功回调</param>
    /// <param name="error">错误回调</param>
    /// <param name="show">是否显示加载</param>
    /// <param name="tips">加载提示文本</param>
    RequestObject pushRequestObject(string route, JsonData data,
        RequestObject.SuccessAction success = null, RequestObject.ErrorAction error = null,
        bool show = true, string tips = "") {
        return pushRequestObject(route, new RequestObject(data, success, error, show, tips));
    }

    /// <summary>
    /// 添加请求
    /// </summary>
    /// <param name="route">路由</param>
    /// <param name="req">请求对象</param>
    RequestObject pushRequestObject(string route, RequestObject req) {
        return requests[generateKey(route)] = req;
    }

    /// <summary>
    /// 生成实际存储的键
    /// </summary>
    /// <param name="route">路由</param>
    string generateKey(string route, int index = -1) {
        if (index < 0) index = requestCnt++;
        return route + ":" + index.ToString();
    }

    /// <summary>
    /// 获取并移除特定路由的请求
    /// </summary>
    /// <param name="route">路由</param>
    /// <returns>请求对象</returns>
    RequestObject popRequestObject(string route, int index) {
        var key = generateKey(route, index);
        if (!requests.ContainsKey(key)) return null;
        var obj = requests[key];
        requests.Remove(key);
        return obj;
    }

    /// <summary>
    /// 配置并发送请求对象
    /// </summary>
    /// <param name="route">路由</param>
    /// <param name="data">数据</param>
    /// <param name="success">成功回调</param>
    /// <param name="error">错误回调</param>
    /// <param name="show">是否显示加载</param>
    /// <param name="tips">加载提示文本</param>
    public void setupRequest(string route, JsonData data = null,
        RequestObject.SuccessAction success = null,
        RequestObject.ErrorAction error = null,
        bool show = true, string tips = "") {
        setup(success, error, show, tips);
        postRequest(route, data);
    }

    /*
    /// <summary>
    /// 配置并发送请求对象（带用户检查）
    /// </summary>
    /// <param name="route">路由</param>
    /// <param name="data">数据</param>
    /// <param name="success">成功回调</param>
    /// <param name="error">错误回调</param>
    /// <param name="show">是否显示加载</param>
    /// <param name="tips">加载提示文本</param>
    public void setupWSUserRequest(string route, JsonData data = null,
        RequestObject.SuccessAction success = null,
        RequestObject.ErrorAction error = null,
        bool show = true, string tips = "") {
        if (data == null) data = new JsonData();
        if (UserSystem.player == null) {
            GameException e = new GameException(GameException.Type.UserUnlogin);
            error.Invoke(e.getCode(), e.getMessage());
        } else {
            data["pid"] = UserSystem.player.getID();
            setupWSRequest(route, data, success, error, show, tips);
        }
    }
    */

    /// <summary>
    /// 重发所有请求
    /// </summary>
    void resendAllRequest() {
        Debug.Log("ResendAll");
        foreach (var req in requests.Values)
            doSendRequest(req);
    }

    /// <summary>
    /// 发送请求
    /// </summary>
    /// <param name="route">路由</param>
    /// <param name="data">数据</param>
    public void postRequest(string route, JsonData data = null) {
        if(data == null) Debug.Log("PostRequest: " + route + "\nData: null");
        else Debug.Log("PostRequest: " + route + "\nData: " + data.ToJson());

        if (!isConnected()) {
            GameException e = new GameException(GameException.Type.GameDisconnected);
            errorHandler.Invoke(e.code, e.message);
        } else {
            var sendData = setupSendData(route, data);
            var req = pushRequestObject(route, sendData, 
                successHandler, errorHandler, showLoading, tipsText);
            doSendRequest(req);
        }
    }

    /// <summary>
    /// 发送信息（无回调）
    /// </summary>
    /// <param name="route">路由</param>
    /// <param name="data">数据</param>
    public void postEmit(string route, JsonData data = null) {
        if (data == null) Debug.Log("PostSend: " + route + "\nData: null");
        else Debug.Log("PostSend: " + route + "\nData: " + data.ToJson());

        clear();

        var sendData = setupSendData(route, data);
        doSendRequest(sendData);
    }

    /// <summary>
    /// 配置请求数据
    /// </summary>
    /// <param name="route">路由</param>
    /// <param name="data">数据</param>
    /// <returns>请求数据</returns>
    JsonData setupSendData(string route, JsonData data) {
        data = data ?? new JsonData();
        data.SetJsonType(JsonType.Object);

        var sendData = new JsonData();
        sendData["route"] = route;
        sendData["data"] = data;
        sendData["index"] = requestCnt;
        return sendData;
    }

    /// <summary>
    /// 实际发送请求
    /// </summary>
    /// <param name="sendData">发送数据</param>
    void doSendRequest(RequestObject req) {
        Debug.Log("Send:" + req.sendData.ToJson());
        webSocket.Send(req.sendData.ToJson());
        if (req.showLoading) SceneUtils.startLoadingWindow(req.tipsText);
    }

    /// <summary>
    /// 实际发送请求
    /// </summary>
    /// <param name="sendData">发送数据</param>
    void doSendRequest(JsonData sendData) {
        Debug.Log("Send:" + sendData.ToJson());
        webSocket.Send(sendData.ToJson());
        if (showLoading) SceneUtils.startLoadingWindow(tipsText);
    }

    #endregion

    // WS连接控制
    #region 连接控制

    /// <summary>
    /// 发起连接
    /// </summary>
    public void connect() {
        Debug.Log("connect");
        if (!isConnected()) {
            changeState(State.Connecting);
            setupWebSocket(); webSocket.Open();
        }
    }

    /// <summary>
    /// 断开连接
    /// </summary>
    public void disconnect() {
        Debug.Log("disconnect");
        if (isConnected()) {
            changeState(State.Disconnecting);
            webSocket.Close();
        }
    }

    #endregion

    // 逻辑处理
    #region 处理函数

    /// <summary>
    /// 处理处理错误（有action的错误）
    /// </summary>
    /// <param name="code">错误码</param>
    /// <param name="errmsg">错误信息</param>
    void handleProcessError(int code, string errmsg, RequestObject.ErrorAction action = null) {
        action = action ?? DefaultErrorHandler;
        action.Invoke(code, errmsg);
    }

    /// <summary>
    /// 处理连接错误
    /// </summary>
    /// <param name="code">错误码</param>
    /// <param name="errmsg">错误信息</param>
    void handleConnectError(int code, string errmsg) {
        errmsg = generateErrorMessage(code, errmsg);
        Debug.Log("Error: code: " + code + " msg: " + errmsg);
        changeState(State.Error, new Tuple<int, string>(code, errmsg));
    }

    /// <summary>
    /// 生成错误信息
    /// </summary>
    /// <param name="code"></param>
    /// <param name="errorMsg"></param>
    string generateErrorMessage(int code, string errorMsg) {
        string format = DefaultErrorFormat;
        switch (code) {
            case 101: format = ServerDisconnected; break;
            case -1: format = ServerNoResponse; break;
        }
        return string.Format(format, code, errorMsg);
    }

    /// <summary>
    /// 处理断开连接
    /// </summary>
    /// <param name="code">状态码</param>
    /// <param name="message">信息</param>
    void handleDisconnect(int code, string message) {
        //if (disconnectHandled) return; disconnectHandled = true;
        Debug.Log("handleDisconnect: " + message);
        string text = generateDisconnectText(code, message);
        changeState(State.Disconnected, new Tuple<int, string>(code, text));
    }

    /// <summary>
    /// 生成断开连接文本
    /// </summary>
    /// <param name="code">代码</param>
    /// <param name="message">消息</param>
    /// <returns></returns>
    string generateDisconnectText(int code, string message) {
        string format = DefaultDisconnectFormat;
        switch (code) {
            case 1011: format = ServerError; break;
            case -1: format = message; break;
        }
        return string.Format(format, code, message);
    }

    #endregion

}
