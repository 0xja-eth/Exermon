using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Core.UI;

using PlayerModule.Data;
using PlayerModule.Services;

namespace UI.MainScene.Controls {

    /// <summary>
    /// 状态显示组件
    /// </summary>
    class StatusDisplay : BaseView {

        /// <summary>
        /// 外部组件设置
        /// </summary>

        /// <summary>
        /// 内部变量设置
        /// </summary>
        Player player;

        #region 初始化

        protected override void initializeOnce() {
            base.initializeOnce();
            player = PlayerService.get().player;
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制玩家状态
        /// </summary>
        /// <param name="player">玩家</param>
        void drawPlayerStatus(Player player) {
            // 具体绘制代码
        }

        /// <summary>
        /// 刷新玩家状态
        /// </summary>
        void refreshPlayerStatus() {
            drawPlayerStatus(player);
        }

        /// <summary>
        /// 刷新视窗
        /// </summary>
        protected override void refresh() {
            base.refresh();
            refreshPlayerStatus();
        }

        #endregion

    }
}