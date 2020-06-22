
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using UI.Common.Controls.ParamDisplays;
using UI.Common.Controls.ItemDisplays;

using ExerPro.EnglishModule.Data;
using GameModule.Services;

namespace UI.ExerPro.EnglishPro.BusinessManScene.Controls {

    /// <summary>
    /// 物品详情
    /// </summary>
    public class PotionItemDetail : BaseItemDetail,IItemDetailDisplay<ExerProPotion>  {

        /// <summary>
        /// 常量定义
        /// </summary>
        const string GoldPriceFormat = "金币：{0}";

        /// <summary>
        /// 外部组件定义
        /// </summary>
        public Text description, priceText;

        public StarsDisplay starsDisplay;

        public MultParamsDisplay detail;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
        }

        /// <summary>
        /// 初始化绘制函数
        /// </summary>
        protected override void initializeDrawFuncs() {
            registerItemType<ExerProPotion>(drawItem);
        }

        #endregion

        #region 画面绘制

        /// <summary>
        /// 绘制基本信息
        /// </summary>
        /// <param name="item">物品1</param>
        protected virtual void drawBaseInfo(ExerProPotion item) {
            name.text = item.name;
            description.text = item.description;

            // 处理物品星级和图标情况
            if (item != null) {
                starsDisplay?.setValue(item.starId);

                icon.gameObject.SetActive(true);
                //icon.overrideSprite = item.icon;
            }
        }

        /// <summary>
        /// 绘制物品详情
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void drawItemDetail(ParamDisplay.IDisplayDataConvertable obj) {
            ////detail?.setValue(obj, "detail");
        }

        /// <summary>
        /// 绘制物品价格
        /// </summary>
        /// <param name="obj"></param>
        void drawPrice() {
            if (shopItem == null) return;
            priceText.text = generatePriceText();
        }

        /// <summary>
        /// 生成价格文本
        /// </summary>
        /// <param name="obj"></param>
        string generatePriceText() {
            var price = shopItem.gold;
            if (price > 0)
                return (string.Format(GoldPriceFormat, price));
            else
                return "";
        }

        /// <summary>
        /// 绘制物品
        /// </summary>
        /// <param name="item">物品</param>
        void drawItem(ExerProPotion item) {
            drawPrice();
            drawBaseInfo(item);
            drawItemDetail(item);
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem() {
            base.drawEmptyItem();
            priceText.text = description.text = "";
            detail.clearValue();
        }

        #endregion

        #region 接口实现

        /// <summary>
        /// 容器
        /// </summary>
        new IContainerDisplay<ExerProPotion> container;

        /// <summary>
        /// 商品
        /// </summary>
        ExerProPotion shopItem;

        /// <summary>
        /// 配置容器
        /// </summary>
        /// <param name="container"></param>
        public void configure(IContainerDisplay<ExerProPotion> container) {
            this.container = container;
        }

        /// <summary>
        /// 开启视窗
        /// </summary>
        /// <param name="container"></param>
        public void startView(ExerProPotion item, int index = -1) {
            shopItem = item;
            base.startView(item, index);
        }

        public void setItem(ExerProPotion item, int index = -1, bool force = false) {
            shopItem = item;
            base.setItem(item, index, force);
        }

        public void startView(ExerProPotion item) {
            shopItem = item;
            base.startView(item);
        }

        public void setItem(ExerProPotion item, bool force = false) {
            shopItem = item;
            base.setItem(item, force);
        }

        ExerProPotion IItemDisplay<ExerProPotion>.getItem() {
            return shopItem;
        }

        #endregion

    }
}