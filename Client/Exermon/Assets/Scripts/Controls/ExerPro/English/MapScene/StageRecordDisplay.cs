
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Core.UI.Utils;

using UI.Common.Controls.ItemDisplays;
using ExerPro.EnglishModule.Data;

/// <summary>
/// 地图场景控件
/// </summary>
namespace UI.ExerPro.EnglishPro.MapScene.Controls {

    /// <summary>
    /// 关卡记录显示组件
    /// </summary>
    public class StageRecordDisplay : ItemDisplay<MapStageRecord> {

        /// <summary>
        /// 外部组件定义
        /// </summary>
        public MapDisplay mapDisplay;
        public PlayerDisplay playerDisplay;

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
        protected override void drawExactlyItem(MapStageRecord item) {
            base.drawExactlyItem(item);
            drawMapInfo();
            drawPlayerInfo();
        }

        /// <summary>
        /// 绘制地图信息
        /// </summary>
        void drawMapInfo() {
            mapDisplay.setItem(item);
        }

        /// <summary>
        /// 绘制玩家信息
        /// </summary>
        void drawPlayerInfo() {
            playerDisplay.setItem(item.actor);
        }

        #endregion

    }
}
