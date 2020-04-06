using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using LitJson;

using Core.Systems;

using PlayerModule.Data;
using PlayerModule.Services;

using UI.Common.Windows;

/// <summary>
/// 核心服务
/// </summary>
namespace Core.Services {

    /// <summary>
    /// 业务控制类（父类）（单例模式）
    /// </summary>
    public class BaseService<T> : BaseSystem<T> where T : BaseService<T>, new() {

        /// <summary>
        /// 操作
        /// </summary>
        public struct Operation {
            public string name;
            public string route;
            public bool emit;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="name">操作名</param>
            /// <param name="route">路由</param>
            /// <param name="emit">是否为发射操作</param>
            public Operation(string name, string route, bool emit = false) {
                this.name = name; this.route = route; this.emit = emit;
            }
        }

        /// <summary>
        /// 操作文本设定
        /// </summary>
        public const string WaitTextFormat = "{0}中...";
        public const string FailTextFormat = "{0}失败：\n{{0}}";

        /// <summary>
        /// 操作字典 (key, (oper, route))
        /// </summary>
        Dictionary<int, Operation> operDict;

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
            operDict = new Dictionary<int, Operation>();
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
        /// <param name="oper">操作名</param>
        /// <param name="route">路由</param>
        /// <param name="emit">是否为发射操作</param>
        protected void addOperDict(int key, string oper, string route, bool emit = false) {
            operDict.Add(key, new Operation(oper, route, emit));
        }
        protected void addOperDict(string oper, string route, bool emit = false) {
            addOperDict(operDict.Count, oper, route, emit);
        }
        protected void addOperDict(Enum key, string oper, string route, bool emit = false) {
            addOperDict(key.GetHashCode(), oper, route, emit);
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
        protected void sendRequest(Enum key, JsonData data = null,
            NetworkSystem.RequestObject.SuccessAction onSuccess = null,
            UnityAction onError = null, string waitFormat = WaitTextFormat, 
            string failFormat = FailTextFormat, bool uid = false) {
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
        /// <param name="uid">是否需要携带玩家信息</param>
        protected void sendRequest(int key, JsonData data = null,
            NetworkSystem.RequestObject.SuccessAction onSuccess = null, 
            UnityAction onError = null, string waitFormat = WaitTextFormat, 
            string failFormat = FailTextFormat, bool uid = false) {
            if (operDict.ContainsKey(key)) {
                var oper = operDict[key];
                sendRequest(oper.name, oper.route, data,
                    onSuccess, onError, waitFormat, failFormat, uid, oper.emit);
            } else Debug.LogError("未找到操作键 " + key + "，请检查操作字典");
        }

        /// <summary>
        /// 发送请求（带有重试功能）
        /// 将会自动根据操作文本确定错误文本和等待文本
        /// </summary>
        /// <param name="oper">操作文本</param>
        /// <param name="route">路由</param>
        /// <param name="data">数据</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        /// <param name="uid">是否需要携带玩家信息</param>
        /// <param name="emit">是否为发射操作</param>
        protected void sendRequest(string oper, string route, JsonData data = null,
            NetworkSystem.RequestObject.SuccessAction onSuccess = null,
            UnityAction onError = null, string waitFormat = WaitTextFormat, 
            string failFormat = FailTextFormat, bool uid = false, bool emit = false) {
            sendRequest(route, data, string.Format(waitFormat, oper),
                string.Format(failFormat, oper), onSuccess, onError, uid, emit);
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
        /// <param name="uid">是否需要携带玩家信息</param>
        /// <param name="emit">是否为发射操作</param>
        protected void sendRequest(string route, JsonData data = null, 
            string waitText = "", string failText = "",
            NetworkSystem.RequestObject.SuccessAction onSuccess = null,
            UnityAction onError = null, bool uid = false, bool emit = false) {
            NetworkSystem.RequestObject.ErrorAction _onError = generateOnErrorFunc(failText,
                () => sendRequest(route, data, waitText, failText,
                    onSuccess, onError, uid, emit), onError);
            if (uid) {
                if (data == null) data = new JsonData();
                data["uid"] = getPlayerID(); // 添加玩家信息
            }
            networkSys.setupRequest(route, data, onSuccess, _onError, true, waitText, emit);
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
                var type = AlertWindow.Type.RetryOrNo;
                gameSys.requestAlert(text, type, retry, onError);
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
}
