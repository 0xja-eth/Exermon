
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Core.UI.Utils;

namespace Core.UI {

    /// <summary>
    /// 视图基类接口
    /// </summary>
    public interface IBaseView {

        /// <summary>
        /// 配置组件
        /// </summary>
        void configure();

        /// <summary>
        /// 开启视窗
        /// </summary>
        void startView();

        /// <summary>
        /// 结束视窗
        /// </summary>
        void terminateView();

        /// <summary>
        /// 请求刷新
        /// </summary>
        void requestRefresh(bool force = false);

        /// <summary>
        /// 请求清除
        /// </summary>
        void requestClear(bool force = false);

    }

    /// <summary>
    /// 游戏所有视图（包括窗口/控件）的基类
    /// </summary>
    /// <remarks>
    /// 视图一般为场景中可见的物体，具体包括窗口（Window）和控件（Control）
    /// （原来的”控件“命名为”组件“，但是为了避免与 Unity 中的组件混淆，改称为控件）
    /// 该类派生有一个 BaseWindow 类，作为所有窗口的基类
    /// 其他从该类派生的类统称为控件，控件即一个控制其挂载物体及其子物体内部行为和显示的组件
    /// 一般来说，控件需要放在窗口内部，不过根据实际情况需要，一些特殊的控件也可以直接放在场景里，比如 MainScene
    /// </remarks>
    public class BaseView : BaseComponent, IBaseView {

        /// <summary>
        /// 布局稳定帧
        /// </summary>
        const int LayoutStableFrame = 10;

        /// <summary>
        /// 外部组件设置
        /// </summary>

        /// <summary>
        /// 内部变量声明
        /// </summary>
        public bool initialized { get; protected set; } = false; // 初始化标志

        /// <summary>
        /// 显示状态
        /// </summary>
        public virtual bool shown {
            get {
                return gameObject.activeSelf;
            }
            protected set {
                gameObject.SetActive(value);
            }
        }

        bool refreshRequested = true;
        bool clearRequested = false;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        void initialize() {
            if (!initialized) {
                initialized = true;
                initializeSystems();
                initializeOnce();
            }
            initializeEvery();
        }

        /// <summary>
        /// 初次打开时初始化（子类中重载）
        /// </summary>
        protected virtual void initializeOnce() { }

        /// <summary>
        /// 初始化系统/服务
        /// </summary>
        protected virtual void initializeSystems() { }

        /// <summary>
        /// 每次打开时初始化（子类中重载）
        /// </summary>
        protected virtual void initializeEvery() { }

        /// <summary>
        /// 配置组件
        /// </summary>
        public virtual void configure() {
            Debug.Log("configure: " + name + ": " + shown);
            initialize();
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        protected override void awake() {
            base.awake();
            // 如果还没有初始化，则自动进行配置
            if (!initialized) configure();
        }

        #endregion

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            updateRefresh();
        }

        /// <summary>
        /// 更新刷新
        /// </summary>
        void updateRefresh() {
            if (isRefreshRequested()) refresh();
            else if (isClearRequested()) clear();
            resetRequests();
        }

        /// <summary>
        /// 注册布局更新（仅用于挂载 Layout 的物体）
        /// </summary>
        /// <param name="rect">物体 RectTransform</param>
        public void registerUpdateLayout() {
            registerUpdateLayout(transform);
        }
        public void registerUpdateLayout(Transform rect) {
            registerUpdateLayout((RectTransform)rect);
        }
        public void registerUpdateLayout(RectTransform rect) {
            SceneUtils.registerUpdateLayout(rect);
        }
        
        #endregion

        #region 启动/结束控制

        /// <summary>
        /// 启动视窗
        /// </summary>
        public virtual void startView() {
            initialize(); showView();
        }

        /// <summary>
        /// 显示视窗
        /// </summary>
        protected virtual void showView() {
            shown = true; requestRefresh(true);
            Debug.Log("showView: " + name);
        }

        /// <summary>
        /// 结束视窗
        /// </summary>
        public virtual void terminateView() {
            hideView();
        }

        /// <summary>
        /// 隐藏视窗
        /// </summary>
        protected virtual void hideView() {
            requestClear(true); shown = false;
            Debug.Log("hideView: " + name);
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 请求刷新
        /// </summary>
        public virtual void requestRefresh(bool force = false) {
            if (force) refresh(); else refreshRequested = true;
        }

        /// <summary>
        /// 请求清除
        /// </summary>
        public virtual void requestClear(bool force = false) {
            if (force) clear(); else clearRequested = true;
        }

        /// <summary>
        /// 重置所有请求
        /// </summary>
        void resetRequests() {
            clearRequested = refreshRequested = false;
        }

        /// <summary>
        /// 是否需要刷新
        /// </summary>
        /// <returns>需要刷新</returns>
        protected virtual bool isRefreshRequested() {
            return shown && refreshRequested;
        }

        /// <summary>
        /// 是否需要清除
        /// </summary>
        /// <returns>需要清除</returns>
        protected virtual bool isClearRequested() {
            return clearRequested;
        }

        /// <summary>
        /// 刷新视窗
        /// </summary>
        protected virtual void refresh() {
            //Debug.Log(name + " refresh");
        }

        /// <summary>
        /// 清除视窗
        /// </summary>
        protected virtual void clear() {
            //Debug.Log(name + " clear");
        }

        #endregion
    }
}