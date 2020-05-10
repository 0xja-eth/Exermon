
using UnityEngine.UI;
using UnityEngine.Events;

using Core.Systems;
using Core.UI;
using Core.UI.Utils;

using QuestionModule.Data;

using PlayerModule.Services;

using RecordModule.Data;
using RecordModule.Services;

using UI.Common.Controls.ParamDisplays;
using UI.ExerciseScene.Controls.Result;

namespace UI.ExerciseScene.Windows {

    /// <summary>
    /// 结算窗口
    /// </summary>
    public class ResultWindow : BaseWindow {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public MultParamsDisplay mainResult;
        public ExpParamDisplay humanExp, exerExp;

        public ItemRewardContainer rewardContaienr;

        /// <summary>
        /// 场景组件引用
        /// </summary>
        protected ExerciseScene scene;

        /// <summary>
        /// 内部变量声明
        /// </summary>
        ExerciseRecord record;

        /// <summary>
        /// 外部系统
        /// </summary>
        PlayerService playerSer;
        RecordService recordSer;

        #region 初始化

        /// <summary>
        /// 初始化场景
        /// </summary>
        protected override void initializeScene() {
            scene = (ExerciseScene)SceneUtils.getSceneObject("Scene");
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            recordSer = RecordService.get();
            playerSer = PlayerService.get();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            record = recordSer.exerciseRecord;
            configureControls();
        }

        /// <summary>
        /// 配置控件
        /// </summary>
        void configureControls() {
            var player = playerSer.player;
            var exerSlot = player.slotContainers.exerSlot;
            var slotItem = exerSlot.getSlotItem(record.subjectId);
            var playerExer = slotItem.playerExer;

            humanExp.configure(player);
            exerExp.configure(playerExer);
        }

        #endregion

        #region 启动/结束窗口

        #endregion

        #region 数据控制

        #endregion

        #region 界面控制

        /// <summary>
        /// 刷新窗口
        /// </summary>
        protected override void refresh() {
            base.refresh();
            refreshResult();
            refreshItemReward();
        }

        /// <summary>
        /// 刷新结果
        /// </summary>
        void refreshResult() {
            mainResult.setValue(record, "result");
        }

        /// <summary>
        /// 刷新物品奖励
        /// </summary>
        void refreshItemReward() {
            rewardContaienr.setItems(record.rewards);
        }

        #endregion

        #region 流程控制

        #endregion

        #region 回调控制

        #endregion

    }
}