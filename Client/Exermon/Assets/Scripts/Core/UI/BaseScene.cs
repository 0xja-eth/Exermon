
using System.Collections;

using Core.Systems;
using Core.UI.Utils;

using UI.Common.Windows;

namespace Core.UI {

    /// <summary>
    /// 场景基类
    /// </summary>
    /// <remarks>
    /// 所有场景类的基类，场景脚本均需要从该类派生
    /// 每个场景原则上都需要有一个场景脚本，用于对场景进行控制管理
    /// 该类定义了 alertWindow 和 loadingWindow，用于配置该场景的提示窗口和加载窗口
    /// 一般来说该脚本挂载在 MainCamera 或者 Canvas，请不要忘记给 alertWindow 和 loadingWindow 赋值
    /// </remarks>
    public abstract class BaseScene : BaseComponent {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public AlertWindow alertWindow; // 提示窗口
        public LoadingWindow loadingWindow; // 加载窗口

        /// <summary>
        /// 初始化标志
        /// </summary>
        public bool initialized { get; protected set; } = false;

        /// <summary>
        /// 内部系统声明
        /// </summary>
        protected GameSystem gameSys;
        protected SceneSystem sceneSys;

        #region 初始化

        /// <summary>
        /// 场景名
        /// </summary>
        /// <returns>场景名</returns>
        public abstract string sceneName();

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void awake() {
            base.awake();
            initialized = true;
            initializeSceneUtils();
            initializeSystems();
            initializeOthers();
            checkFirstScene();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        void initializeSceneUtils() {
            SceneUtils.initialize(sceneName(), alertWindow, loadingWindow);
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected virtual void initializeSystems() {
            gameSys = GameSystem.get();
            sceneSys = SceneSystem.get();
        }

        /// <summary>
        /// 初始化其他项
        /// </summary>
        protected virtual void initializeOthers() {

        }

        /// <summary>
        /// 检查初始场景
        /// </summary>
        /// <returns></returns>
        public void checkFirstScene() {
            var first = SceneSystem.Scene.FirstScene;
            if (gameSys.isConnectable() && sceneName() != first)
                sceneSys.gotoScene(first);
        }

        #endregion

        #region 更新控制

        protected override void update() {
            base.update(); SceneUtils.update();
        }

        /// <summary>
        /// 创建协程
        /// </summary>
        /// <param name="func">协程函数</param>
        public void createCoroutine(IEnumerator func) {
            StartCoroutine(func);
        }

        #endregion

        #region 场景控制

        /// <summary>
        /// 返回场景
        /// </summary>
        public void popScene() {
            sceneSys.popScene();
        }

        #endregion
    }
}