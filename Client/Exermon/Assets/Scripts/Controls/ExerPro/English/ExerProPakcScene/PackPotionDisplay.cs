﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ExerPro.EnglishModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.ExerProPackScene.Controls.Menu {


    /// <summary>
    /// 特训背包药水
    /// </summary>
    public class PackPotionDisplay : PackContItemDisplay
        <ExerProPackPotion, ExerProPotion> {


        /// <summary>
        /// 外部组件定义
        /// </summary>
        public Text description;


        #region 数据控制

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制卡牌
        /// </summary>
        /// <param name="card"></param>
        protected override void drawExactlyItem(ExerProPackPotion item) {
            base.drawExactlyItem(item);
        }

        protected override void drawItem(ExerProPotion item) {
            base.drawItem(item);
            name.text = item.name;
            icon.gameObject.SetActive(true);
            Debug.Log("AAA" + item.icon);
            icon.overrideSprite = item.icon;
        }

        /// <summary>
        /// 绘制空物品
        /// </summary>
        protected override void drawEmptyItem() {
            base.drawEmptyItem();
        }

        #endregion

    }
}
