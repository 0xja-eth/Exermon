using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ExerPro.EnglishModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.ExerProPackScene.Controls.Menu {


    /// <summary>
    /// 特训背包药水
    /// </summary>
    public class PackCardDisplay : PackContItemDisplay
        <ExerProPackCard, ExerProCard> {


        /// <summary>
        /// 外部组件定义
        /// </summary>
        public Text cost;
        public Text description;

        public Text character;
        public Text type;


        #region 数据控制

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制卡牌
        /// </summary>
        /// <param name="card"></param>
        protected override void drawExactlyItem(ExerProPackCard item) {
            base.drawExactlyItem(item);
        }

        protected override void drawItem(ExerProCard item) {
            base.drawItem(item);
            description.text = item.description;
            name.text = item.name;
            cost.text = item.cost.ToString();
            //character.text = item.character;
            type.text = item.typeText();

            icon.gameObject.SetActive(true);
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
