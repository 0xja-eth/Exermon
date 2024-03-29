﻿
using Core.UI;

using UI.PackScene.Windows;
using UnityEngine;

namespace UI.PackScene.Controls {

    /// <summary>
    /// 状态页控制组
    /// </summary>
    public class PackTabController : TabView<PackWindow> {

        #region 界面绘制

        /// <summary>
        /// 显示内容页
        /// </summary>
        /// <param name="content"></param>
        protected override void showContent(PackWindow content, int index) {
            Debug.Log("showContent" + index);
            content.switchView(index);
        }

        /// <summary>
        /// 隐藏内容页
        /// </summary>
        /// <param name="content"></param>
        protected override void hideContent(PackWindow content, int index) {
            Debug.Log("hideContent" + index);
            content.clearView();
        }

        #endregion

    }

}
