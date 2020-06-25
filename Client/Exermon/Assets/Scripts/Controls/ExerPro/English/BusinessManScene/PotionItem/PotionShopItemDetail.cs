
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using UI.ShopScene.Controls;

using UI.Common.Controls.ParamDisplays;
using UI.Common.Controls.ItemDisplays;

using ExerPro.EnglishModule.Data;

using ItemModule.Services;

namespace UI.ExerPro.EnglishPro.BusinessManScene.Controls {

    /// <summary>
    /// 物品详情
    /// </summary>
    public class PotionShopItemDetail : ShopItemDetail<ExerProPotion> {//BaseItemDetail, IItemDetailDisplay<ExerProPotion>  {

        /// <summary>
        /// 常量定义
        /// </summary>
        const string GoldPriceFormat = "金币：{0}";

        /// <summary>
        /// 外部组件定义
        /// </summary>

        #region 画面绘制

        /// <summary>
        /// 绘制基本信息
        /// </summary>
        /// <param name="item">物品1</param>
        protected override void drawBaseInfo(ExerProPotion item) {
            name.text = item.name;
            description.text = item.description;

            // 处理物品星级和图标情况
            starsDisplay?.setValue(item.starId);

            icon.gameObject.SetActive(true);
            icon.overrideSprite = item.icon;
        }
		/*
        /// <summary>
        /// 绘制物品详情
        /// </summary>
        /// <param name="obj"></param>
        protected override void drawItemDetail(
			ParamDisplay.IDisplayDataConvertable obj) {
            detail?.setValue(obj, "detail");
        }
		*/
		/// <summary>
		/// 生成价格文本
		/// </summary>
		/// <param name="obj"></param>
		protected override string generatePriceText() {
            var price = shopItem.gold;
            if (price > 0)
                return (string.Format(GoldPriceFormat, price));
            else
                return "";
        }

        #endregion
		/*
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

        ExerProPotion IItemDisplay<ItemService.ShopItem<ExerProPotion>>.getItem() {
            return shopItem;
        }

        #endregion
		*/
    }
}