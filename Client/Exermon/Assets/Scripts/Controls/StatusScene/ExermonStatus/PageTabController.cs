
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using Core.UI;
using Core.UI.Utils;

using GameModule.Data;
using ItemModule.Data;
using PlayerModule.Data;
using ExermonModule.Data;

namespace UI.StatusScene.Controls.ExermonStatus {

    /// <summary>
    /// 艾瑟萌页控制器
    /// </summary>
    public class PageTabController : TabView<ExermonStatusPageDisplay> {


        /// <summary>
        /// 内部变量定义
        /// </summary>
        public ExerSlotItem slotItem { get; private set; }
        public Player player { get; private set; }
        public Subject subject { get; private set; }

        #region 数据控制

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="player">玩家</param>
        /// <param name="subject">科目</param>
        public void setup(Player player, Subject subject) {
            this.player = player; this.subject = subject;
            if (player == null || subject == null) slotItem = null;
            else slotItem = player.getExerSlotItem(subject);
            requestRefresh();
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 显示内容页
        /// </summary>
        /// <param name="content"></param>
        protected override void showContent(ExermonStatusPageDisplay content, int index) {
            UnityAction action = () => content.startView(slotItem);
            if (content.initialized) action.Invoke();
            else {
                // 读取数据后加载
                var loadFunc = content.getLoadFunction();
                if (loadFunc == null) action.Invoke();
                else loadFunc.Invoke(action);
            }
        }

        /// <summary>
        /// 显示内容页
        /// </summary>
        /// <param name="content"></param>
        protected override void hideContent(ExermonStatusPageDisplay content, int index) {
            content.terminateView();
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 装备回调
        /// </summary>
        public void onEquip() {
            currentContent()?.equipCurrentItem();
        }

        /// <summary>
        /// 卸下回调
        /// </summary>
        public void onDequip() {
            currentContent()?.dequipCurrentItem();
        }

        #endregion

    }
    
}