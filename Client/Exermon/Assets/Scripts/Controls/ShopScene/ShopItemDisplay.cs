
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Core.Data.Loaders;

using ItemModule.Data;
using ItemModule.Services;

using UI.Common.Controls.ParamDisplays;
using UI.Common.Controls.ItemDisplays;

namespace UI.ShopScene.Controls {

    /// <summary>
    /// 装备背包显示
    /// </summary>
    public class ShopItemDisplay<T> : SelectableBaseItemDisplay,
        ISelectableItemDisplay<ItemService.ShopItem<T>> where T: BaseItem, new() {

        /// <summary>
        /// 常量定义
        /// </summary>

        /// <summary>
        /// 外部组件定义
        /// </summary>
        public StarsDisplay starsDisplay;

        public Image priceTag;
        public Text priceText;

        public Texture2D goldTag, ticketTag, boundTicketTag;
        
        /// <summary>
        /// 内部变量声明
        /// </summary>

        #region 初始化

        /// <summary>
        /// 初始化绘制函数
        /// </summary>
        protected override void initializeDrawFuncs() {
            registerItemType<T>(drawItem);
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
            var item_ = item as LimitedItem;
            if (item_ != null){
                starsDisplay?.setValue(item_.starId);

                icon.gameObject.SetActive(true);
                icon.overrideSprite = item_.icon;
            }           
        }

        /// <summary>
        /// 绘制物品价格
        /// </summary>
        /// <param name="obj"></param>
        void drawPrice() {
            if (shopItem == null) return;
            var price = shopItem.price;
            if (price.gold > 0) {
                priceText.text = price.gold.ToString();
                setPriceTag(goldTag);
            } else if (price.ticket > 0) {
                priceText.text = price.ticket.ToString();
                setPriceTag(ticketTag);
            } else if (price.boundTicket > 0) {
                priceText.text = price.boundTicket.ToString();
                setPriceTag(boundTicketTag);
            } else {
                priceText.text = "";
                setPriceTag(null);
            }
        }

        /// <summary>
        /// 设置物品价格标签
        /// </summary>
        /// <param name="obj"></param>
        void setPriceTag(Texture2D texture){
            if (texture == null)
                priceTag.gameObject.SetActive(false);
            else{
                priceTag.gameObject.SetActive(true);
                priceTag.overrideSprite = AssetLoader.generateSprite(texture);
            }
        }

        /// <summary>
        /// 绘制物品
        /// </summary>
        /// <param name="item">物品</param>
        void drawItem(T item) {
            drawPrice();
            drawBaseInfo(item); 
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem() {
            base.drawEmptyItem();
            priceTag.gameObject.SetActive(false);
            priceText.text = "";
        }

        #endregion

        #region 接口实现

        /// <summary>
        /// 容器
        /// </summary>
        SelectableContainerDisplay<ItemService.ShopItem<T>> container;

        /// <summary>
        /// 商品
        /// </summary>
        ItemService.ShopItem<T> shopItem;

        /// <summary>
        /// 配置窗口
        /// </summary>
        public void configure(SelectableContainerDisplay<ItemService.ShopItem<T>> container, int index) { 
            this.container = container; 
        }

        /// <summary>
        /// 获取容器
        /// </summary>
        /// <returns></returns>
        SelectableContainerDisplay<ItemService.ShopItem<T>> 
            ISelectableItemDisplay<ItemService.ShopItem<T>>.getContainer() {
            return container;
        }            

        /// <summary>
        /// 获取物品
        /// </summary>
        /// <returns></returns>
        ItemService.ShopItem<T> IItemDisplay<ItemService.ShopItem<T>>.getItem() {
            return shopItem;
        }

        /// <summary>
        /// 开启视窗
        /// </summary>
        /// <param name="container"></param>
        public void startView(ItemService.ShopItem<T> item, int index = -1) {
            shopItem = item; startView(item.item());
        }

        public void setItem(ItemService.ShopItem<T> item, int index = -1, bool force = false) {
            shopItem = item; setItem(item.item(), force);
        }

        public void startView(ItemService.ShopItem<T> item) {
            shopItem = item; startView(item.item());
        }

        public void setItem(ItemService.ShopItem<T> item, bool force = false) {
            shopItem = item; setItem(item.item(), force);
        }
        
        #endregion
    }
}