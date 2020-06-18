
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Core.Data.Loaders;

using ItemModule.Data;
using ItemModule.Services;

using UI.Common.Controls.ParamDisplays;
using UI.Common.Controls.ItemDisplays;

using ExerPro.EnglishModule.Data;

namespace UI.ExerPro.EnglishPro.Common.Controls {

    /// <summary>
    /// 装备背包显示
    /// </summary>
    public class CardItemDisplay : SelectableItemDisplay<ExerProCard> {

        /// <summary>
        /// 常量定义
        /// </summary>

        /// <summary>
        /// 外部组件定义
        /// </summary>
        public BaseItemDisplay itemDisplay;

        public StarsDisplay starsDisplay;

        public Image icon;
        public Text name;
        public Text cost;
        public Text description;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            initializeDrawFuncs();
        }

        /// <summary>
        /// 初始化绘制函数
        /// </summary>
        protected void initializeDrawFuncs() {
            itemDisplay.registerItemType<ExerProCard>(drawItem);
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 绘制基本信息
        /// </summary>
        /// <param name="item">物品</param>
        protected virtual void drawBaseInfo(ExerProCard item) {
            name.text = item.name;
            // 处理物品星级和图标情况
            if (item != null) {
                starsDisplay?.setValue(item.starId);
                cost.text = item.cost.ToString();
                description.text = item.description;
                icon.gameObject.SetActive(true);
                //icon.overrideSprite = item.icon;
            }
        }

        /// <summary>
        /// 绘制物品
        /// </summary>
        /// <param name="item">物品</param>
        void drawItem(ExerProCard item) {
            drawBaseInfo(item);
        }

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="shopItem"></param>
        protected override void drawExactlyItem(ExerProCard shopItem) {
            base.drawExactlyItem(shopItem);
            itemDisplay.setItem(shopItem);
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem() {
            base.drawEmptyItem();
            name.text = "";
            icon.gameObject.SetActive(false);
        }

        #endregion

    }
}