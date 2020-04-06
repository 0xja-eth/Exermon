using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using PlayerModule.Data;

using Core.UI;

using PlayerModule.Data;
using PlayerModule.Services;
using UI.Common.Controls.ParamDisplays;

namespace UI.MainScene.Controls {

    /// <summary>
    /// 状态显示组件
    /// </summary>
    class StatusDisplay : BaseView {

        /// <summary>
        /// 等级经验文本格式
        /// </summary>
        const string LevelExpFormat = "Lv{0}: {1}/{2}";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Image face;
        public Text name;
        public Text battlePoint;
        public Text level;
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
            drawPlayerFace(player);
            drawPlayerInfo(player);
        }

        /// <summary>
        /// 绘制玩家头像
        /// </summary>
        /// <param name="player">玩家</param>
        void drawPlayerFace(Player player) {
            face.gameObject.SetActive(true);
            face.overrideSprite = player.character().face;
        }

        /// <summary>
        /// 绘制玩家信息
        /// </summary>
        /// <param name="player">玩家</param>
        void drawPlayerInfo(Player player) {
            name.text = player.name;
            level.text = string.Format(LevelExpFormat,
                player.level, player.exp, player.next);
            battlePoint.text = player.sumBattlePoint().ToString();
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

        protected override void clear() {
            face.gameObject.SetActive(false);
            face.overrideSprite = null;
            name.text = battlePoint.text = level.text = "";
        }

        #endregion

    }
}