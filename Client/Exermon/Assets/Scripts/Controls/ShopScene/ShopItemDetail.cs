
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

using ItemModule.Data;
using ItemModule.Services;

using PlayerModule.Data;
using ExermonModule.Data;

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

        /// <summary>
        /// 外部变量定义
        /// </summary>
        public ShopWindow shopWindow;

        public Text description;

        public MultParamsDisplay detail;

        /// <summary>
        /// 内部变量定义
        /// </summary>

        #region 初始化

        /// <summary>
        /// 初始化绘制函数
        /// </summary>
        protected override void initializeDrawFuncs() {
            registerItemType<T>(drawHumanPackItem);
            registerItemType<HumanPackEquip>(drawHumanPackEquip);
            registerItemType<ExerPackItem>(drawExerPackItem);
            registerItemType<ExerPackEquip>(drawExerPackEquip);
        }

        #endregion

        #region 数据控制

        #region 物品控制

        /// <summary>
        /// 获取物品容器
        /// </summary>
        /// <returns>容器</returns>
        public IContainerDisplay<ItemService.ShopItem<T>> getContainer() {
            return shopWindow.currentPackContainer();
        }

        #endregion

        #endregion

        #region 画面绘制

        /// <summary>
        /// 绘制基本信息
        /// </summary>
        /// <param name="item">物品1</param>
        protected virtual void drawBaseInfo(T item) {
            name.text = item.name;
            description.text = item.description;

            /*
            starsDisplay.setValue(item.starId);

            icon.gameObject.SetActive(true);
            icon.overrideSprite = item.icon;
            */
        }

        /// <summary>
        /// 绘制人类背包物品
        /// </summary>
        /// <param name="packItem">人类背包物品</param>
        void drawItem(T item) {
            drawBaseInfo(item); drawItemDetail(item);
        }

        /// <summary>
        /// 绘制人类背包装备
        /// </summary>
        /// <param name="packEquip">人类背包装备</param>
        void drawHumanPackEquip(HumanPackEquip packEquip) {
            drawBaseInfo(packEquip.item());
            drawEquipDetail(packEquip);
        }

        /// <summary>
        /// 绘制艾瑟萌背包物品
        /// </summary>
        /// <param name="packItem">艾瑟萌背包物品</param>
        void drawExerPackItem(ExerPackItem packItem) {
            drawBaseInfo(packItem.item());
            drawItemDetail(packItem);
        }

        /// <summary>
        /// 绘制艾瑟萌背包装备
        /// </summary>
        /// <param name="packEquip">艾瑟萌背包装备</param>
        void drawExerPackEquip(ExerPackEquip packEquip) {
            drawBaseInfo(packEquip.item());
            drawEquipDetail(packEquip);
        }

        /// <summary>
        /// 绘制物品详情
        /// </summary>
        /// <param name="obj"></param>
        void drawItemDetail(ParamDisplay.IDisplayDataConvertable obj) {
            equipDetail.gameObject.SetActive(false);
            itemDetail.gameObject.SetActive(true);
            itemDetail.setValue(obj, "detail");
        }

        /// <summary>
        /// 绘制物品详情
        /// </summary>
        /// <param name="obj"></param>
        void drawEquipDetail(ParamDisplay.IDisplayDataConvertable obj) {
            itemDetail.gameObject.SetActive(false);
            equipDetail.gameObject.SetActive(true);
            equipDetail.setValue(obj, "detail");
        }

        /// <summary>
        /// 配置控制按钮
        /// </summary>
        /// <param name="item">物品</param>
        void setupButtons(LimitedItem item) {
            equip.SetActive(isEquip());
            use.SetActive(isUsable());
            discard.SetActive(item.discardable);
            sell.SetActive(item.sellable());
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem() {
            base.drawEmptyItem();
            starsDisplay.clearValue();
            icon.gameObject.SetActive(false);
            name.text = description.text = "";

            itemDetail.gameObject.SetActive(false);
            equipDetail.gameObject.SetActive(false);

            equip.SetActive(false);
            use.SetActive(false);
            sell.SetActive(false);
            discard.SetActive(false);
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
        ItemService.ShopItem<T> shopItem;

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
            shopItem = item; startView(item.item(), index);
        }

        public void setItem(ItemService.ShopItem<T> item, int index = -1, bool force = false) {
            shopItem = item; setItem(item.item(), index, force);
        }

        public void startView(ItemService.ShopItem<T> item) {
            shopItem = item; startView(item.item());
        }

        public void setItem(ItemService.ShopItem<T> item, bool force = false) {
            shopItem = item; setItem(item.item(), force);
        }

        ItemService.ShopItem<T> IItemDisplay<ItemService.ShopItem<T>>.getItem() {
            return shopItem;
        }

        #endregion

    }
}