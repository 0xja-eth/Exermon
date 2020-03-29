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
            // 绘制头像
            var character = player.character();
            var face = character.face;
            var rect = new Rect(0, 0, face.width, face.height);
            this.face.gameObject.SetActive(true);
            this.face.overrideSprite = Sprite.Create(
                face, rect, new Vector2(0.5f, 0.5f));
            this.face.overrideSprite.name = face.name;
            // 设置状态栏上的等级，战斗力等信息
            name.text = player.name;
            level.text = player.level.ToString();
            var exerSlot = player.slotContainers.exerSlot;
            battlePoint.text = exerSlot.sumBattlePoint().ToString();
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