
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Core.Data.Loaders;

using ItemModule.Data;
using ItemModule.Services;

using UI.Common.Controls.ParamDisplays;
using UI.Common.Controls.ItemDisplays;

using ExerPro.EnglishModule.Data;

namespace UI.ExerPro.EnglishPro.BusinessManScene.Controls {

    /// <summary>
    /// 装备背包显示
    /// </summary>
    public class ShopItemDisplay<T> : SelectableItemDisplay<T> where T : BaseExerProItem {

        /// <summary>
        /// 常量定义
        /// </summary>

        /// <summary>
        /// 外部组件定义
        /// </summary>
        public BaseItemDisplay itemDisplay;

        public StarsDisplay starsDisplay;

        public Image icon, priceTag;
        public Text name, priceText;

        public Texture2D goldTag;

        /// <summary>
        /// 内部变量
        /// </summary>
        protected int price = 0;

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
            itemDisplay.registerItemType<T>(drawItem);
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 绘制基本信息
        /// </summary>
        /// <param name="item">物品</param>
        protected virtual void drawBaseInfo(T item) {
            name.text = item.name;
            // 处理物品星级和图标情况
            if (item != null) {
                starsDisplay?.setValue(item.starId);

                icon.gameObject.SetActive(true);
                //icon.overrideSprite = item.icon;
            }
        }

        /// <summary>
        /// 绘制物品价格
        /// </summary>
        /// <param name="item">商品</param>
        void drawPrice(T item) {
            var price = item.gold;
            if (price > 0) {
                priceText.text = price.ToString();
                setPriceTag(goldTag);
            }
            else {
                priceText.text = "";
                setPriceTag(null);
            }
        }

        /// <summary>
        /// 设置物品价格标签
        /// </summary>
        /// <param name="obj"></param>
        void setPriceTag(Texture2D texture) {
            if (texture == null)
                priceTag.gameObject.SetActive(false);
            else {
                priceTag.gameObject.SetActive(true);
                priceTag.overrideSprite = AssetLoader.generateSprite(texture);
            }
        }

        /// <summary>
        /// 绘制物品
        /// </summary>
        /// <param name="item">物品</param>
        void drawItem(T item) {
            drawBaseInfo(item);
        }

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="shopItem"></param>
        protected override void drawExactlyItem(T shopItem) {
            base.drawExactlyItem(shopItem);
            itemDisplay.setItem(shopItem);
            drawPrice(shopItem);
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem() {
            base.drawEmptyItem();
            name.text = priceText.text = "";
            icon.gameObject.SetActive(false);
            priceTag.gameObject.SetActive(false);
        }

        #endregion

    }
}