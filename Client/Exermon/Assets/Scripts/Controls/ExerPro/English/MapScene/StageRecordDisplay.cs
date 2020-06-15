
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Core.UI.Utils;

using ExerPro.EnglishModule.Data;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.ParamDisplays;

/// <summary>
/// 地图场景控件
/// </summary>
namespace UI.ExerPro.EnglishPro.MapScene.Controls {

    /// <summary>
    /// 关卡记录显示组件
    /// </summary>
    public class StageRecordDisplay : ItemDisplay<ExerProRecord> {

        /// <summary>
        /// 外部组件定义
        /// </summary>
        public MapDisplay mapDisplay;
        public PlayerDisplay playerDisplay;

        public PlayerStatus playerStatus;

        public MultParamsDisplay mapProgress;
        public MultParamsDisplay wordProgress;

        #region 数据控制

        /// <summary>
        /// 当前据点
        /// </summary>
        public ExerProMapNode currentNode() {
            return item.currentNode();
        }

        /// <summary>
        /// 是否移动中
        /// </summary>
        /// <returns></returns>
        public bool isMoving() {
            return playerDisplay.isMoving;
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制物品
        /// </summary>
        /// <param name="item"></param>
        protected override void drawExactlyItem(ExerProRecord item) {
            base.drawExactlyItem(item);
            drawMapInfo(item);
            drawPlayerStatus(item);
            drawPlayerDisplay(item);
            drawProgresses(item);
        }

        /// <summary>
        /// 绘制地图信息
        /// </summary>
        void drawMapInfo(ExerProRecord item) {
            mapDisplay.setItem(item, true);
        }

        /// <summary>
        /// 绘制玩家信息
        /// </summary>
        void drawPlayerStatus(ExerProRecord item) {
            playerStatus.setItem(item.actor);
            // 需要显示
            if (item.isFirstSelected()) {
                var nodeDisplay = mapDisplay.currentNode();
                playerDisplay.gotoNode(nodeDisplay, true);
            }
        }

        /// <summary>
        /// 绘制玩家信息
        /// </summary>
        void drawPlayerDisplay(ExerProRecord item) {
            playerDisplay.setItem(item.actor, true);
        }

        /// <summary>
        /// 绘制进度
        /// </summary>
        void drawProgresses(ExerProRecord item) {
            mapProgress.setValue(item, "map_progress");
            wordProgress.setValue(item, "word_progress");
        }

        #endregion

    }
}
