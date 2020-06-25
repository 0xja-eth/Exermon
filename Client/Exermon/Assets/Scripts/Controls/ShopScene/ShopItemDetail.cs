
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

using ItemModule.Data;
using ItemModule.Services;

using UI.Common.Controls.ParamDisplays;
using UI.Common.Controls.ItemDisplays;

using UI.ShopScene.Windows;

namespace UI.ShopScene.Controls {

    /// <summary>
    /// 物品详情
    /// </summary>
    public class ShopItemDetail<T> : BaseItemDetail, 
        IItemDetailDisplay<ItemService.ShopItem<T>> where T: BaseItem, new() {

        /// <summary>
        /// 常量定义
        /// </summary>
        const string GoldPriceFormat = "金币：{0}";
        const string TicketPriceFormat = "点券：{0}";
        const string BoundTicketPriceFormat = "绑定点券：{0}";

        /// <summary>
        /// 外部组件定义
        /// </summary>
        //public RectTransform layoutContent;

        //public ShopWindow shopWindow;

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
            registerItemType<T>(drawItem);
        }

        #endregion

        #region 画面绘制

        /// <summary>
        /// 绘制基本信息
        /// </summary>
        /// <param name="item">物品1</param>
        protected virtual void drawBaseInfo(T item) {
            name.text = item.name;
            description.text = item.description;

            // 处理物品星级和图标情况
            var item_ = item as LimitedItem;
            if (item_ != null){
                starsDisplay?.setValue(item_.starId);

                icon.gameObject.SetActive(true);
                icon.overrideSprite = item_.icon;
            }           
        }

        /// <summary>
        /// 绘制物品详情
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void drawItemDetail(ParamDisplay.IDisplayDataConvertable obj) {
            detail?.setValue(obj, "detail");
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
        protected virtual string generatePriceText() {
            var price = shopItem.price;
            var res = new List<string>();
            if (price.gold > 0)
                res.Add(string.Format(GoldPriceFormat, price.gold));
            if (price.ticket > 0)
                res.Add(string.Format(TicketPriceFormat, price.ticket));
            if (price.boundTicket > 0)
                res.Add(string.Format(BoundTicketPriceFormat, price.boundTicket));

            return string.Join("\n", res);
        }

        /// <summary>
        /// 绘制物品
        /// </summary>
        /// <param name="item">物品</param>
        void drawItem(T item) {
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
            detail?.clearValue();
        }

        #endregion

        #region 接口实现

        /// <summary>
        /// 容器
        /// </summary>
        IContainerDisplay<ItemService.ShopItem<T>> container;

        /// <summary>
        /// 商品
        /// </summary>
        protected ItemService.ShopItem<T> shopItem;

        /// <summary>
        /// 配置容器
        /// </summary>
        /// <param name="container"></param>
        public void configure(IContainerDisplay<ItemService.ShopItem<T>> container) {
            this.container = container;
        }

        /// <summary>
        /// 开启视窗
        /// </summary>
        /// <param name="container"></param>
        public void startView(ItemService.ShopItem<T> item, int index = -1) {
            shopItem = item; startView(item?.item(), index);
        }

        public void setItem(ItemService.ShopItem<T> item, int index = -1, bool force = false) {
            shopItem = item; setItem(item?.item(), index, force);
        }

        public void startView(ItemService.ShopItem<T> item) {
            shopItem = item; startView(item?.item());
        }

        public void setItem(ItemService.ShopItem<T> item, bool force = false) {
            shopItem = item; setItem(item?.item(), force);
        }

        ItemService.ShopItem<T> IItemDisplay<ItemService.ShopItem<T>>.getItem() {
            return shopItem;
        }

        #endregion

    }
}