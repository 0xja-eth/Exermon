
using Core.Systems;
using Core.UI;

using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;

/// <summary>
/// 艾瑟萌特训UI
/// </summary>
namespace UI.ExerPro { }

/// <summary>
/// 艾瑟萌英语特训UI
/// </summary>
namespace UI.ExerPro.EnglishPro { }

/// <summary>
/// 地图场景
/// </summary>
namespace UI.ExerPro.EnglishPro.MapScene {

    using Controls;
    using UI.ExerPro.EnglishPro.Common.Windows;

    /// <summary>
    /// 地图场景
    /// </summary>
    public class MapScene : BaseScene {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public StageRecordDisplay stageRecordDisplay;
		public RestNodeDisplay restNodeDisplay;
        public SwitchWindow switchWindow;

		public BaseWindow nodeDetail;

        /// <summary>
        /// 内部变量定义
        /// </summary>
        bool firstMove = true;

        /// <summary>
        /// 外部系统设置
        /// </summary>
        EnglishService engSer;

        #region 初始化

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            engSer = EnglishService.get();
        }

        /// <summary>
        /// 场景名称
        /// </summary>
        /// <returns></returns>
        public override SceneSystem.Scene sceneIndex() {
            return SceneSystem.Scene.EnglishProMapScene;
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected override void start() {
            base.start();
            stageRecordDisplay.setItem(engSer.record);
        }

        #endregion

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            engSer?.update();
			updateRestNode();
            updateSwitchWindow();
        }

		/// <summary>
		/// 更新休息据点
		/// </summary>
		void updateRestNode() {
			if (!engSer.isRestNode()) return;
			restNodeDisplay.startView(engSer.randomTips());
			stageRecordDisplay.requestRefresh();
		}

        /// <summary>
        /// 更新死亡/关卡切换窗口
        /// </summary>
        void updateSwitchWindow() {
            if (engSer.record.actor.isDead()) {
                switchWindow.startWindow(type: SwitchWindow.Type.Die, onDieExit);
            }
        }


        #endregion

        #region 流程控制

        /// <summary>
        /// 进入下一步
        /// </summary>
        public void moveNext() {
            var mapDisplay = stageRecordDisplay.mapDisplay;
            var playerDisplay = stageRecordDisplay.playerDisplay;

            var nodeDisplay = mapDisplay.selectedItemDisplay();
            var node = mapDisplay.selectedItem();

            var record = engSer.record;
            var first = !record.isFirstSelected();

            engSer.startMove(node.id, first);
            playerDisplay.gotoNode(nodeDisplay, first);

            stageRecordDisplay.requestRefresh();
            nodeDetail.terminateWindow();
        }

        /// <summary>
        /// 退出场景
        /// </summary>
        public override void popScene() {
            engSer.terminate();
        }


        /// <summary>
        /// 打开背包
        /// </summary>
        public void openPack() {
            sceneSys.pushScene(SceneSystem.Scene.EnglishProPackScene);
        }
        #endregion

        #region 回调函数
        /// <summary>
        /// 死亡退出回调
        /// </summary>
        void onDieExit() {
            popScene();
        }
        #endregion
    }
}
