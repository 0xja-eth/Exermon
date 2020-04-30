
using UnityEngine;

namespace Core.UI {

    /// <summary>
    /// 窗口基类
    /// </summary>
    /// <remarks>
    /// 游戏所有窗口的基类，实际上几乎是一个带有打开动画和关闭动画的视图
    /// 一般用于控制管理一个窗口下的组件，也用于进行不同窗口间的交互
    /// </remarks>
    public class BaseWindow : AnimatorView {
        /*
        /// <summary>
        /// 窗口状态
        /// </summary>
        public enum State {
            None, // 未设置
            Showing, // 开启中
            Shown, // 已开启
            Hiding, // 关闭中
            Hidden, // 已关闭
        }
        */
        /// <summary>
        /// 外部组件设置
        /// </summary>
        public GameObject background; // 窗口背景

        /// <summary>
        /// 外部变量设置
        /// </summary>
        public string shownState = "Shown"; // 显示状态名称
        public string hiddenState = "Hidden"; // 隐藏状态名称

        public string shownAttr = "shown"; // 显示状态属性

        /// <summary>
        /// 动画过渡
        /// </summary>
        bool isShowing = false, isHiding = false;

        /*
        /// <summary>
        /// 内部变量声明
        /// </summary>
        protected State state = State.None; // 窗口状态（可以在子类自定义）
        */

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            initializeScene();
        }

        /// <summary>
        /// 初始化场景
        /// </summary>
        protected virtual void initializeScene() {}

        /// <summary>
        /// 初始化状态
        /// </summary>
        protected override void initializeStates() {
            addChangeEvent(shownState, onWindowShown);
            addChangeEvent(hiddenState, onWindowHidden);
        }

        #endregion

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            Debug.Log(name + ": isShowing: " + isShowing + ", isHiding: " + isHiding);
            updateBackground();
        }

        /// <summary>
        /// 更新窗口背景
        /// </summary>
        void updateBackground() {
            if (background != null) background.SetActive(isBackgroundVisible());
        }
        
        #endregion

        #region 启动/结束控制

        /// <summary>
        /// 启动窗口
        /// </summary>
        public virtual void startWindow() {
            Debug.Log("startWindow: " + name);
            isShowing = true; base.startView();
        }

        /// <summary>
        /// 显示窗口（视窗）
        /// </summary>
        protected override void showView() {
            gameObject.SetActive(shown = true);
            if (!animator) onWindowShown();
            else animator.SetBool(shownAttr, true);
        }

        /// <summary>
        /// 结束窗口
        /// </summary>
        public virtual void terminateWindow() {
            Debug.Log("terminateWindow: " + name);
            isHiding = true; base.terminateView();
        }

        /// <summary>
        /// 隐藏窗口（视窗）
        /// </summary>
        protected override void hideView() {
            if (!animator) onWindowHidden();
            else animator.SetBool(shownAttr, false);
        }

        /// <summary>
        /// 窗口完全显示回调
        /// </summary>
        protected virtual void onWindowShown() {
            Debug.Log("onWindowShown: " + name);
            isShowing = false; requestRefresh(true);
        }

        /// <summary>
        /// 窗口完全隐藏回调
        /// </summary>
        protected virtual void onWindowHidden() {
            Debug.Log("onWindowHidden: " + name);
            isHiding = false; if (isShowing) return;
            base.hideView(); updateBackground();
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 判断是否处于可视状态
        /// </summary>
        /// <returns>是否可视状态</returns>
        protected virtual bool isBackgroundVisible() {
            return shown;
        }

        #endregion

    }
}