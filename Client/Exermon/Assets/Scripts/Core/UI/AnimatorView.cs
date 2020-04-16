
using System.Collections.Generic;

//using UnityEditor.Animations;

using UnityEngine;
using UnityEngine.Events;

namespace Core.UI {

    /// <summary>
    /// 带有动画功能的视窗类
    /// </summary>
    /// <remarks>
    /// 一般挂载在带有 BaseView 的物体上
    /// </remarks>
    public class AnimatorView : BaseView {
        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Animator animator; // 动画组件

        /// <summary>
        /// 外部变量设置
        /// </summary>

        /// <summary>
        /// 内部变量设置
        /// </summary>
        /*
        AnimatorController controller = null;
        AnimatorControllerLayer layer = null;
        AnimatorStateMachine machine = null;
        */
        int layerIndex;

        string eventState; // 当前事件状态

        /// <summary>
        /// 状态切换回调函数（key 为 "" 时表示任意状态）
        /// </summary>
        Dictionary<string, UnityAction> changeEvents = new Dictionary<string, UnityAction>();

        /// <summary>
        /// 状态更新回调函数
        /// </summary>
        Dictionary<string, UnityAction> updateEvents = new Dictionary<string, UnityAction>();

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            initAnimator();
        }

        /// <summary>
        /// 初始化动画控制器
        /// </summary>
        void initAnimator() {
            if (animator == null) return;
            //controller = (AnimatorController)animator.runtimeAnimatorController;
            switchLayer();
            initializeStates();
        }

        /// <summary>
        /// 初始化状态
        /// </summary>
        protected virtual void initializeStates() {}

        #endregion

        #region 状态机配置

        /// <summary>
        /// 添加状态行为脚本
        /// </summary>
        /// <typeparam name="T">脚本类</typeparam>
        /// <param name="layerIndex">层ID</param>
        /// <param name="stateName">状态名</param>
        public T addStateBehaviour<T>(int layerIndex, string stateName) where T : StateMachineBehaviour {
            /*
            if (animator == null) return null;
            if (this.layerIndex != layerIndex) switchLayer(layerIndex);
            foreach (var state in machine.states)
                if (state.state.name == stateName)
                    return state.state.AddStateMachineBehaviour<T>();
            */
            return null;
        }
        public T addStateBehaviour<T>(string stateName) where T : StateMachineBehaviour {
            return addStateBehaviour<T>(layerIndex, stateName);
        }
        /// <summary>
        /// 添加状态切换事件
        /// </summary>
        /// <param name="stateName">状态名</param>
        /// <param name="action">事件</param>
        public void addChangeEvent(string stateName, UnityAction action) {
            changeEvents.Add(stateName, action);
        }

        /// <summary>
        /// 移除状态切换事件
        /// </summary>
        /// <param name="stateName">状态名</param>
        public void removeChangeEvent(string stateName) {
            changeEvents.Remove(stateName);
        }

        /// <summary>
        /// 添加状态更新事件
        /// </summary>
        /// <param name="stateName">状态名</param>
        /// <param name="action">事件</param>
        public void addUpdateEvent(string stateName, UnityAction action) {
            updateEvents.Add(stateName, action);
        }

        /// <summary>
        /// 移除状态更新事件
        /// </summary>
        /// <param name="stateName">状态名</param>
        public void removeUpdateEvent(string stateName) {
            updateEvents.Remove(stateName);
        }

        #endregion

        #region 状态机控制

        /// <summary>
        /// 切换当前缓存的状态机
        /// </summary>
        /// <param name="layerIndex">层ID</param>
        public void switchLayer(int layerIndex = 0) {
            if (animator == null) return;
            this.layerIndex = layerIndex;
            //layer = controller.layers[layerIndex];
            //machine = layer.stateMachine;
        }

        #endregion

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            updateAnimatorState();
        }

        /// <summary>
        /// 更新窗口背景
        /// </summary>
        void updateAnimatorState() {
            if (animator == null) return;
            foreach (var key in changeEvents.Keys) {
                var state = animator.GetCurrentAnimatorStateInfo(layerIndex);
                if (key != eventState && (key == "" || state.IsName(key)))
                    changeEvents[eventState = key]?.Invoke();
            }
            foreach (var key in updateEvents.Keys) {
                var state = animator.GetCurrentAnimatorStateInfo(layerIndex);
                if (state.IsName(key)) updateEvents[key]?.Invoke();
            }
        }

        #endregion

    }
}