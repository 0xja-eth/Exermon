
using UnityEngine;
using UnityEngine.Events;

using GameModule.Services;

using UI.Common.Windows;

namespace Core.Systems {

    /// <summary>
    /// 游戏整体流程控制类
    /// </summary>
    /// <remarks>
    /// 控制整个游戏流程，包括游戏开始、结束、错误处理等（通用流程）
    /// </remarks>
    public class GameSystem : BaseSystem<GameSystem> {

        /// <summary>
        /// 文本设定
        /// </summary>
        const string ConnectingWaitText = "连接服务器中";
        const string ConnectionFailText = "连接服务器发生错误，错误信息：\n{0}"; // \n选择“重试”重新连接，选择“取消”退出游戏。";

        const string DisconnectedAlertText = "您已断开连接：\n{0}"; // \n选择“重试”重新连接，选择“取消”退出游戏。\n若仍然无法连接，请联系管理员。";

        /// <summary>
        /// 状态
        /// </summary>
        public enum State {
            Connecting, Loading, Loaded,
            Disconnected, Reconnecting,
            // Error,
            Ending
        }
        public bool isConnectable() { // 可以执行 connect
            return state == -1 || state == (int)State.Disconnected;
        }
        public bool isReconnectable() { // 可以执行 reconnect
            return state == (int)State.Disconnected;
        }
        public bool isLoaded() {
            return state == (int)State.Loaded;
        }
        public bool isEnding() {
            return state == (int)State.Ending;
        }

        /// <summary>
        /// 请求弹窗类
        /// </summary>
        public class AlertRequest {

            /// <summary>
            /// 属性
            /// </summary>
            public string text { get; }
            public float duration { get; }

            public AlertWindow.Type type { get; }

            public UnityAction onOK { get; }
            public UnityAction onCancel { get; }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="btns">弹窗按钮文本</param>
            /// <param name="actions">弹窗按钮动作</param>
            public AlertRequest(string text,
                AlertWindow.Type type = AlertWindow.Type.Notice,
                UnityAction onOK = null, UnityAction onCancel = null,
                float duration = AlertWindow.DefaultDuration) {
                this.text = text; this.type = type;
                this.onOK = onOK; this.onCancel = onCancel;
                this.duration = duration;
            }

        }

        /// <summary>
        /// 请求加载类
        /// </summary>
        public class LoadingRequest {

            /// <summary>
            /// 属性
            /// </summary>
            public string text { get; } = "";
            public double progress { get; } = -1;
            public bool start { get; } = true;

            public bool setProgress { get; } = false;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="text">等待文本</param>
            /// <param name="start">开关标志</param>
            public LoadingRequest(string text, bool start = true) {
                this.text = text;
                this.start = start;
            }
            public LoadingRequest(double progress) {
                this.progress = progress;
                setProgress = true;
            }
            public LoadingRequest(bool start = true) {
                this.start = start;
            }
        }

        /// <summary>
        /// 提示请求
        /// </summary>
        public AlertRequest alertRequest { get; protected set; } = null;
        /// <summary>
        /// 请求提示
        /// </summary>
        /// <param name="text">提示文本</param>
        /// <param name="type">提示窗口类型</param>
        /// <param name="onOK">按确认/重试键时回调</param>
        /// <param name="onCancel">按取消键时回调（默认关闭弹窗）</param>
        /// <param name="duration">自动消失时长</param>
        /// <remarks>用于呼出提示窗口（弹窗）</remarks>
        /// <example>例 1：显示一个提示：
        /// <code>
        /// var gameSys = GameSystem.get();
        /// gameSys.requestAlert("提示！");
        /// </code>
        /// </example>
        /// <example>例 2：显示一个带确认按钮的提示，确认时执行 onOK 函数，否则关闭弹窗：
        /// <code>
        /// var gameSys = GameSystem.get();
        /// gameSys.requestAlert("确认？", AlertWindow.Type.YesOrNo, onOK);
        /// </code>
        /// </example>
        public void requestAlert(string text,
            AlertWindow.Type type = AlertWindow.Type.Notice,
            UnityAction onOK = null, UnityAction onCancel = null,
            float duration = AlertWindow.DefaultDuration) {
            alertRequest = new AlertRequest(text, type, onOK, onCancel, duration);
        }
        /// <summary>
        /// 清空提示请求
        /// </summary>
        /// <remarks>清空当前的提示请求（并不等于关闭提示窗口，只是用于清除前一个操作的请求）</remarks>
        public void clearRequestAlert() {
            alertRequest = null;
        }

        /// <summary>
        /// 加载请求
        /// </summary>
        public LoadingRequest loadingRequest { get; protected set; } = null;

        /// <summary>
        /// 请求加载
        /// </summary>
        /// <remarks>在下一帧开启加载窗口</remarks>
        /// <example>例：
        /// <code>
        /// var gameSys = GameSystem.get();
        /// gameSys.requestLoadStart("等待中..."); // 将开启一个加载窗口
        /// </code>
        /// </example>
        /// <param name="text">等待文本</param>
        public void requestLoadStart(string text) {
            Debug.Log("requestLoadStart: " + text);
            loadingRequest = new LoadingRequest(text);
        }
        /// <summary>
        /// 请求加载进度
        /// </summary>
        /// <param name="progress">加载进度</param>
        public void requestLoadProgress(double progress = -1) {
            Debug.Log("requestLoadProgress: " + progress);
            loadingRequest = new LoadingRequest(progress);
        }
        /// <summary>
        /// 请求结束加载
        /// </summary>
        /// <remarks>在下一帧关闭加载窗口</remarks>
        public void requestLoadEnd() {
            loadingRequest = new LoadingRequest(false);
        }
        /// <summary>
        /// 清空加载请求
        /// </summary>
        /// <remarks>清空当前的加载请求（并不等于关闭加载窗口，只是用于清除前一个操作的请求）</remarks>
        public void clearRequestLoad() {
            loadingRequest = null;
        }
        
        /// <summary>
        /// 状态回调函数
        /// </summary>
        public UnityAction connectedCallback { get; set; } = null;
        public UnityAction reconnectedCallback { get; set; } = null;
        public UnityAction disconnectedCallback { get; set; } = null;
        public UnityAction connectErrorCallback { get; set; } = null;

        /// <summary>
        /// 外部系统
        /// </summary>
        NetworkSystem networkSys;
        StorageSystem storageSys;
        DataService dataSer;

        #region 初始化

        /// <summary>
        /// 初始化状态字典
        /// </summary>
        protected override void initializeStateDict() {
            base.initializeStateDict();
            addStateDict(State.Connecting, updateConnecting);
            addStateDict(State.Loading, updateLoading);
            addStateDict(State.Loaded);
            addStateDict(State.Disconnected);
            addStateDict(State.Reconnecting, updateReconnecting);
            //addStateDict(State.Error);
            addStateDict(State.Ending);
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            storageSys = StorageSystem.get();
            networkSys = NetworkSystem.get();
            dataSer = DataService.get();
        }

        #endregion

        #region 更新控制

        /// <summary>
        /// 更新其他
        /// </summary>
        protected override void updateOthers() {
            updateConnectedError();
            updateDisconnection();
        }

        /// <summary>
        /// 更新外部系统
        /// </summary>
        protected override void updateSystems() {
            networkSys.update();
        }

        /// <summary>
        /// 更新连接状态
        /// </summary>
        void updateConnecting() {
            if (!networkSys.isStateChanged()) return;
            if (networkSys.isConnected()) onConnected();
        }

        /// <summary>
        /// 更新数据读取
        /// </summary>
        void updateLoading() {
            if (dataSer.isLoaded())
                changeState(State.Loaded);
        }

        /// <summary>
        /// 更新连接状态
        /// </summary>
        void updateReconnecting() {
            Debug.Log("updateReconnecting");
            if (!networkSys.isStateChanged()) return;
            if (networkSys.isConnected()) onReconnected();
        }

        /// <summary>
        /// 更新连接错误
        /// </summary>
        void updateConnectedError() {
            if (networkSys.isError() && networkSys.stateInfo != null) {
                var info = networkSys.stateInfo;
                onConnectingError(info.Item1, info.Item2);
                networkSys.clearStateInfo();
            }
        }

        /// <summary>
        /// 更新断开连接
        /// </summary>
        void updateDisconnection() {
            if (networkSys.isDisconnected() && networkSys.stateInfo != null) {
                var info = networkSys.stateInfo;
                onDisconnected(info.Item1, info.Item2);
                networkSys.clearStateInfo();
            }
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 开始
        /// </summary>
        public void start(bool reconnect = false) {
            storageSys.load();
            if (!reconnect) connect();
            else this.reconnect();
        }

        /// <summary>
        /// 连接
        /// </summary>
        protected void connect() {
            if (!isConnectable()) return;
            changeState(State.Connecting);
            doConnect();
        }

        /// <summary>
        /// 重连
        /// </summary>
        protected void reconnect() {
            if (!isReconnectable()) return;
            changeState(State.Reconnecting);
            doConnect();
        }

        /// <summary>
        /// 连接（无状态变换）
        /// </summary>
        void doConnect() {
            requestLoadStart(ConnectingWaitText);
            networkSys.connect();
        }

        /// <summary>
        /// 结束
        /// </summary>
        public void terminate(UnityAction action) {
            changeState(State.Ending);
            storageSys.save();
            if (action != null) action.Invoke();
            Application.Quit();
        }

        /// <summary>
        /// 结束
        /// </summary>
        public void terminate() { terminate(null); }

        #endregion

        #region 回调控制

        /// <summary>
        /// 连接回调
        /// </summary>
        protected virtual void onConnected() {
            requestLoadEnd();
            changeState(State.Loading);
            dataSer.load(terminate);
            if (connectedCallback != null)
                connectedCallback.Invoke();
        }

        /// <summary>
        /// 重连回调
        /// </summary>
        protected virtual void onReconnected() {
            requestLoadEnd();
            changeState(State.Loaded);
            if (reconnectedCallback != null)
                reconnectedCallback.Invoke();
        }

        /// <summary>
        /// 连接断开回调
        /// </summary>
        /// <param name="status">状态码</param>
        /// <param name="errmsg">错误信息</param>
        protected virtual void onDisconnected(int status, string errmsg) {
            Debug.LogError("onDisconnected, " + status + ": " + errmsg);
            processException(status, errmsg, DisconnectedAlertText, 
                State.Disconnected, reconnect);
            if (disconnectedCallback != null) disconnectedCallback.Invoke();
        }

        /// <summary>
        /// 连接错误回调
        /// </summary>
        /// <param name="status">状态码</param>
        /// <param name="errmsg">错误信息</param>
        protected virtual void onConnectingError(int status, string errmsg) {
            if (dataSer.isLoaded()) onDisconnected(status, errmsg);
            else {
                Debug.LogError("onConnectingError, " + status + ": " + errmsg);
                processException(status, errmsg, ConnectionFailText,
                    State.Disconnected, connect);
                if (connectErrorCallback != null) connectErrorCallback.Invoke();
            }
        }

        /// <summary>
        /// 处理异常
        /// </summary>
        /// <param name="status">状态码</param>
        /// <param name="errmsg">错误信息</param>
        /// <param name="format">提示文本格式</param>
        /// <param name="retry">重试函数</param>
        void processException(int status, string errmsg,
            string format, State state, UnityAction retry) {
            var text = string.Format(format, errmsg);
            var type = AlertWindow.Type.RetryOrNo;
            requestAlert(text, type, retry, terminate);
            changeState(state);
        }

        #endregion

    }

}