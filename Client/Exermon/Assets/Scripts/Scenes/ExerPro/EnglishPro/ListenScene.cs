
using Core.Systems;
using Core.UI;

using ExerPro.EnglishModule.Services;

using UI.ExerPro.EnglishPro.ListenScene.Windows;

using UI.ExerPro.EnglishPro.Common.Windows;

/// <summary>
/// 背包场景
/// </summary>
namespace UI.ExerPro.EnglishPro.ListenScene {

    /// <summary>
    /// 商店场景
    /// </summary>
    public class ListenScene : BaseScene {
        /// <summary>
        /// 外部变量
        /// </summary>
        public ListenWindow listenWindow;
        public RewardWindow rewardWindow;

        /// <summary>
        /// 外部系统
        /// </summary>
        EnglishService engSer;

        #region 初始化

        /// <summary>
        /// 场景名
        /// </summary>
        /// <returns>场景名</returns>
        public override SceneSystem.Scene sceneIndex() {
            return SceneSystem.Scene.EnglishProListenScene;
        }

        protected override void initializeSystems() {
            base.initializeSystems();
            engSer = EnglishService.get();
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected override void start() {
            base.start();
			listenWindow.startWindow();
		}
		
		#endregion

		#region 场景控制

		/// <summary>
		/// 场景更新
		/// </summary>
		protected override void update() {
            base.update();
            var rewardInfo = engSer.rewardInfo;
            if (rewardInfo != null) 
                rewardWindow.startWindow(rewardInfo);
        }

		/// <summary>
		/// 退出场景
		/// </summary>
        public override void popScene() {
            engSer.exitNode();
        }

        #endregion

    }
}