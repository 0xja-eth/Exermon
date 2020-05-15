using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 核心系统
/// </summary>
namespace Core.Systems {

    /// <summary>
    /// 业务控制类（父类）（单例模式）
    /// </summary>
    public class BaseSystem<T> where T : BaseSystem<T>, new() {

        /// <summary>
        /// 初始化标志
        /// </summary>
        public static bool initialized { get; protected set; } = false;
        public bool isInitialized() { return initialized; }

        /// <summary>
        /// 多例错误
        /// </summary>
        class MultCaseException : Exception {
            const string ErrorText = "单例模式下不允许多例存在！";
            public MultCaseException() : base(ErrorText) { }
        }

        /// <summary>
        /// 状态字典 (state, action)
        /// </summary>
        Dictionary<int, UnityAction> stateDict;

        /// <summary>
        /// 当前状态
        /// </summary>
        public int state { get; protected set; } = -1;
        public int lastState { get; protected set; } = -1;

        /// <summary>
        /// 单例函数
        /// </summary>
        protected static T _self;
        public static T get() {
            if (_self == null) {
                _self = new T();
                _self.initialize();
            }
            return _self;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected BaseSystem() {
            if (_self != null) throw new MultCaseException();
        }

        /// <summary>
        /// 初始化（只执行一次）
        /// </summary>
        void initialize() {
            initialized = true;
            initializeStateDict();
            initializeSystems();
            initializeOthers();
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected virtual void initializeSystems() { }

        /// <summary>
        /// 其他初始化工作
        /// </summary>
        protected virtual void initializeOthers() { }

        #region 更新控制

        /// <summary>
        /// 更新（每帧）
        /// </summary>
        public virtual void update() {
            updateState();
            updateOthers();
            updateSystems();
        }

        /// <summary>
        /// 更新状态机
        /// </summary>
        void updateState() {
            if (hasState(state) && stateDict[state] != null)
                stateDict[state].Invoke();
            lastState = state;
        }

        /// <summary>
        /// 更新外部系统
        /// </summary>
        protected virtual void updateSystems() { }

        /// <summary>
        /// 更新其他
        /// </summary>
        protected virtual void updateOthers() { }

        #endregion

        #region 状态字典

        /// <summary>
        /// 初始化状态字典
        /// </summary>
        protected virtual void initializeStateDict() {
            stateDict = new Dictionary<int, UnityAction>();
        }

        /// <summary>
        /// 添加状态字典
        /// </summary>
        /// <param name="state">状态</param>
        /// <param name="act">动作</param>
        protected void addStateDict(int state, UnityAction act = null) {
            stateDict.Add(state, act);
        }
        protected void addStateDict(Enum state, UnityAction act = null) {
            addStateDict(state.GetHashCode(), act);
        }

        /// <summary>
        /// 是否存在状态
        /// </summary>
        /// <param name="state">状态名</param>
        /// <returns>是否存在</returns>
        protected bool hasState(int state) {
            return stateDict.ContainsKey(state);
        }
        protected bool hasState(Enum state) {
            return hasState(state.GetHashCode());
        }

        /// <summary>
        /// 状态是否改变
        /// </summary>
        /// <returns>状态改变</returns>
        public bool isStateChanged() {
            return lastState != state;
        }

        /// <summary>
        /// 改变状态
        /// </summary>
        /// <param name="state">新状态</param>
        protected void changeState(int state, bool force = false) {
            Debug.Log("changeState: " + GetType() + ": " + this.state + " -> " + state);
            if ((force || hasState(state)) && this.state != state) 
                this.state = state;
        }
        protected void changeState(Enum state, bool force = false) {
            changeState(state.GetHashCode(), force);
        }

        #endregion

    }
}