
using UnityEngine;

using Core.UI;

using Core.Systems;

using BattleModule.Services;

/// <summary>
/// 对战场景
/// </summary>
namespace UI.BattleScene {

    using Windows;
    using Controls;

    /// <summary>
    /// 对战场景
    /// </summary>
    public class BattleScene : BaseScene {

        /// <summary>
        /// 文本常量定义
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public BattleStartAni startAni;

        public PrepareWindow prepareWindow;
        public QuestionWindow questionWindow;
        public ActionWindow actionWindow;
        public RoundResultWindow roundResultWindow;

        /// <summary>
        /// 内部组件设置
        /// </summary>

        /// <summary>
        /// 内部系统声明
        /// </summary>
        BattleService battleSer;

        #region 初始化

        /// <summary>
        /// 场景名
        /// </summary>
        /// <returns>场景名</returns>
        public override string sceneName() {
            return SceneSystem.Scene.BattleScene;
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            battleSer = BattleService.get();
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected override void start() {
            base.start();
            startAni.setItem(battleSer.battle);
        }

        #endregion

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            updateStartAnimation();
            updateStateChanged();
        }

        /// <summary>
        /// 更新开始动画
        /// </summary>
        void updateStartAnimation() {
            if (startAni.isAnimationEnd()) {
                startAni.terminateView();
                onPerparing();
            }
        }

        /// <summary>
        /// 更新状态变化
        /// </summary>
        void updateStateChanged() {
            if (battleSer.isStateChanged())
                onStateChanged();
            battleSer.update();
        }

        #endregion

        #region 场景控制

        /// <summary>
        /// 关闭所有窗口
        /// </summary>
        void clearWindows() {
            prepareWindow.terminateWindow();
            questionWindow.terminateWindow();
            roundResultWindow.terminateWindow();
            actionWindow.terminateWindow();
        }

        /// <summary>
        /// 关闭所有控件
        /// </summary>
        void clearControls() {
        }

        /// <summary>
        /// 清空场景
        /// </summary>
        void clear() {
            clearWindows();
            clearControls();
        }

        #endregion

        #region 流程控制
        
        #region 状态回调处理

        /// <summary>
        /// 状态改变回调
        /// </summary>
        void onStateChanged() {
            switch ((BattleService.State)battleSer.state) {
                case BattleService.State.Preparing: onPerparing(); break;
                case BattleService.State.Questing: onQuesting(); break;
                case BattleService.State.OneQuested: onQuested(); break;
                case BattleService.State.BothQuested: onQuested(true); break;
                case BattleService.State.Acting: onActing(); break;
                case BattleService.State.Resulting: onResulting(); break;
                case BattleService.State.Terminating: onTerminating(); break;
            }
        }

        /// <summary>
        /// 准备状态
        /// </summary>
        void onPerparing() {
            Debug.Log("onPerparing");
            clear();
            questionWindow.clearStoryboards(true);
            prepareWindow.startWindow();
        }

        /// <summary>
        /// 答题状态
        /// </summary>
        void onQuesting() {
            clear();
            questionWindow.startWindow();
        }

        /// <summary>
        /// 答题完毕
        /// </summary>
        void onQuested(bool both = false) {
            questionWindow.onQuested();
        }

        /// <summary>
        /// 开始行动完毕
        /// </summary>
        void onActing() {
            questionWindow.showStoryboards(true);
            actionWindow.startWindow();
        }

        /// <summary>
        /// 回合结算
        /// </summary>
        void onResulting() {
            roundResultWindow.startWindow();
        }

        /// <summary>
        /// 对战结束
        /// </summary>
        void onTerminating() {
            sceneSys.changeScene(SceneSystem.Scene.BattleResultScene);
        }

        #endregion

        #endregion

    }
}