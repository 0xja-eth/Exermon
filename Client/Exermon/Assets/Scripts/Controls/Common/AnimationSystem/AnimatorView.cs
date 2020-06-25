
using System.Collections.Generic;

//using UnityEditor.Animations;

using UnityEngine;
using UnityEngine.Events;

using Core.UI;

namespace UI.Common.Controls.AnimationSystem {

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

        protected string eventState; // 当前事件状态

        /// <summary>
        /// 状态切换回调函数（key 为 "" 时表示任意状态）
        /// </summary>
        Dictionary<string, UnityAction> changeEvents = new Dictionary<string, UnityAction>();

		/// <summary>
		/// 状态结束回调函数（key 为 "" 时表示任意状态）
		/// </summary>
		Dictionary<string, UnityAction> endEvents = new Dictionary<string, UnityAction>();

		/// <summary>
		/// 状态更新回调函数
		/// </summary>
		Dictionary<string, UnityAction> updateEvents = new Dictionary<string, UnityAction>();

		/// <summary>
		/// 动画重载
		/// </summary>
		AnimatorOverrideController override_;

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
		/// 添加状态结束事件
		/// </summary>
		/// <param name="stateName">状态名</param>
		/// <param name="action">事件</param>
		public void addEndEvent(string stateName, UnityAction action) {
			endEvents.Add(stateName, action);
		}

		/// <summary>
		/// 移除状态结束事件
		/// </summary>
		/// <param name="stateName">状态名</param>
		public void removeEndEvent(string stateName) {
			endEvents.Remove(stateName);
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
			var state = animator.GetCurrentAnimatorStateInfo(layerIndex);
			/*
			if (eventState != null && !state.IsName(eventState) &&
				endEvents.ContainsKey(eventState)) {
				endEvents[eventState]?.Invoke();
				eventState = null;
			}
			*/
			foreach (var key in changeEvents.Keys) {
				if (key != eventState && (key == "" || state.IsName(key))) 
					changeEvents[eventState = key]?.Invoke();
			}
			foreach (var key in updateEvents.Keys) {
                if (state.IsName(key)) updateEvents[eventState = key]?.Invoke();
            }
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 是否处于某状态
        /// </summary>
        /// <param name="name">状态名</param>
        /// <returns>返回是否处于某状态</returns>
        public bool isState(string name) {
            if (animator == null) return false;
            var state = animator.GetCurrentAnimatorStateInfo(layerIndex);
            return state.IsName(name);
        }

        /// <summary>
        /// 设置动画变量
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="name">变量名</param>
        /// <param name="val">值</param>
        public void setVar<T>(string name, T val) {
            Debug.Log("setVar: " + name + "(" + typeof(T) + "): " + val);
            if (typeof(T) == typeof(bool))
                animator.SetBool(name, (bool)(object)val);
            else if (typeof(T) == typeof(int))
                animator.SetInteger(name, (int)(object)val);
            else if (typeof(T) == typeof(double) || typeof(T) == typeof(float))
                animator.SetFloat(name, (float)(object)val);
            else if (typeof(T) == typeof(Vector3))
                animator.SetVector(name, (Vector3)(object)val);
            else if (typeof(T) == typeof(Quaternion))
                animator.SetQuaternion(name, (Quaternion)(object)val);
            else animator.SetTrigger(name);
        }
        public void setVar(string name) {
            animator.SetTrigger(name);
        }

		/// <summary>
		/// 配置动画
		/// </summary>
		/// <param name="state">状态名</param>
		/// <param name="clip">动画片段</param>
		public void changeAni(string state, AnimationClip clip) {
			Debug.Log("changeAni: " + state + ": " + clip);
			if (override_ == null) {
				var runtime = animator.runtimeAnimatorController;
				override_ = new AnimatorOverrideController(runtime);

				//animator.runtimeAnimatorController = null;
				animator.runtimeAnimatorController = override_;

				Resources.UnloadUnusedAssets();
			}

			clip.legacy = false;
			override_[state] = clip;
		}

        #endregion

    }
}