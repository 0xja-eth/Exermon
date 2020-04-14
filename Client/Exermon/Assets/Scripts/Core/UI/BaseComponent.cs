
using System.Collections;

using UnityEngine;

namespace Core.UI {

    /// <summary>
    /// 组件基类
    /// </summary>
    /// <remarks>
    /// 所有自定义组件的基类，组件即 Unity 中的 Component
    /// 初步封装了 MonoBehaviour，用 awake/start/update 代替 Awake/Start/Update 函数，并且定义了一些状态
    /// </remarks>
    public abstract class BaseComponent : MonoBehaviour {

        /// <summary>
        /// 开始标志
        /// </summary>
        public bool awaked { get; protected set; } = false;
        public bool started { get; protected set; } = false;
        public bool updating { get; protected set; } = false;

        #region 初始化

        /// <summary>
        /// 初始化（唤醒）
        /// </summary>
        private void Awake() {
            awaked = true; awake();
        }

        /// <summary>
        /// 初始化（同Awake）
        /// </summary>
        protected virtual void awake() { }

        /// <summary>
        /// 初始化（开始）
        /// </summary>
        private void Start() {
            started = true; start();
        }

        /// <summary>
        /// 初始化（同Start）
        /// </summary>
        protected virtual void start() { }

        #endregion

        #region 更新

        /// <summary>
        /// 更新
        /// </summary>
        private void Update() {
            updating = true;
            update();
            updating = false;
        }

        /// <summary>
        /// 更新（同Update）
        /// </summary>
        protected virtual void update() { }

        #endregion

        #region 协程控制

        /// <summary>
        /// 协程
        /// </summary>
        protected Coroutine doRoutine(string methodName) {
            return StartCoroutine(methodName);
        }
        protected Coroutine doRoutine(IEnumerator routine) {
            return StartCoroutine(routine);
        }
        
        #endregion
    }
}