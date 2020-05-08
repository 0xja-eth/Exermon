using System;

using UnityEngine;

using Core.Systems;
using Core.UI;
using Core.UI.Utils;

using ItemModule.Data;
using ItemModule.Services;

using PlayerModule.Data;
using PlayerModule.Services;

using ExermonModule.Data;

using RecordModule.Services;

using UI.ShopScene.Controls;

using UI.Common.Controls.ParamDisplays;
using UI.Common.Controls.ItemDisplays;

namespace UI.ShopScene.Windows {

    /// <summary>
    /// 状态窗口
    /// </summary>
    public class ShopWindow : BaseWindow {

        /// <summary>
        /// 文本常量定义
        /// </summary>
        const string BuySuccessText = "购买成功！";

        /// <summary>
        /// 视图枚举
        /// </summary>
        public enum View {
            Human, Exermon,
        }

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public PackTabController tabController;

        public HumanItemShopDisplay humanItemShop;
        public ExerEquipShopDisplay exerEquipShop;

        public PackItemDetail itemDetail;

        public NumberInputWindow numberWindow;

        public CurrencyDisplay moneyDisplay;

        /// <summary>
        /// 内部变量声明
        /// </summary>
        View view;
        Player player;

        /// <summary>
        /// 场景组件引用
        /// </summary>
        ShopScene scene;

        /// <summary>
        /// 外部系统引用
        /// </summary>
        GameSystem gameSys = null;
        PlayerService playerSer = null;
        RecordService recordSer = null;
        ItemService itemSer = null;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            player = playerSer.player;
        }

        /// <summary>
        /// 初始化场景
        /// </summary>
        protected override void initializeScene() {
            scene = (ShopScene)SceneUtils.getSceneObject("Scene");
        }

        /// <summary>
        /// 初始化系统/服务
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            gameSys = GameSystem.get();
            playerSer = PlayerService.get();
            recordSer = RecordService.get();
            itemSer = ItemService.get();
        }
        
        #endregion

        #region 开启控制

        /// <summary>
        /// 开始窗口
        /// </summary>
        public override void startWindow() {
            startWindow(View.Human);
        }

        /// <summary>
        /// 开始窗口
        /// </summary>
        public void startWindow(View view) {
            base.startWindow();
            tabController.startView((int)view);
        }
        
        #endregion

        #region 数据控制

        /// <summary>
        /// 当前背包容器
        /// </summary>
        /// <returns></returns>
        public SelectableContainerDisplay<ItemService.ShopItem<T>> 
            currentPackContainer<T>() where T: BaseItem, new() {
            if (typeof(T) == typeof(HumanItem))
                return (SelectableContainerDisplay<ItemService.ShopItem<T>>)
                    (object)humanItemShop;
            if (typeof(T) == typeof(ExerEquip))
                return (SelectableContainerDisplay<ItemService.ShopItem<T>>)
                    (object)exerEquipShop;
            return null;
        }

        /// <summary>
        /// 当前物品详情显示控件
        /// </summary>
        /// <returns></returns>
        public ShopItemDetail<T> currentItemDetail<T>() where T : BaseItem, new() {
            if (typeof(T) == typeof(HumanItem))
                return (SelectableContainerDisplay<ItemService.ShopItem<T>>)
                    (object)humanItemShop;
            if (typeof(T) == typeof(ExerEquip))
                return (SelectableContainerDisplay<ItemService.ShopItem<T>>)
                    (object)exerEquipShop;
            return null;
        }

        /// <summary>
        /// 操作容器
        /// </summary>
        /// <returns></returns>
        public PackContainer<PackContItem> operContainer() {
            return currentPackContainer()?.getPackData();
        }

        /// <summary>
        /// 当前操作物品
        /// </summary>
        /// <returns></returns>
        public T operPackItem<T>() where T : PackContItem {
            return currentItemDetail()?.getItem() as T;
        }
        /// <summary>
        /// 操作物品
        /// </summary>
        /// <returns>返回操作物品</returns>
        public PackContItem operPackItem() {
            return currentItemDetail()?.getItem();
        }

        /// <summary>
        /// 当前操作物品
        /// </summary>
        /// <returns></returns>
        public T operItem<T>() where T : BaseItem {
            return currentItemDetail()?.getContainedItem<T>();
        }
        
        #endregion

        #region 界面绘制

        /// <summary>
        /// 刷新视窗
        /// </summary>
        public void refreshView() {
            clearView();
            switch (view) {
                case View.Human: onHumanPack(); break;
                case View.Exermon: onExerPack(); break;
            }
        }

        /// <summary>
        /// 人类背包
        /// </summary>
        void onHumanPack() {
            var container = player.packContainers.humanPack;
            humanPack.startView(); humanPack.setPackData(container);
        }

        /// <summary>
        /// 艾瑟萌背包
        /// </summary>
        void onExerPack() {
            var container = player.packContainers.exerPack;
            exerPack.startView(); exerPack.setPackData(container);
        }

        /// <summary>
        /// 题目糖背包
        /// </summary>
        void onQuesSugar() {

        }

        /// <summary>
        /// 刷新金钱
        /// </summary>
        void refreshMoney() {
            moneyDisplay.setValue(player.money);
        }

        /// <summary>
        /// 清除视图
        /// </summary>
        public void clearView() {
            humanPack.terminateView();
            exerPack.terminateView();
        }

        /// <summary>
        /// 刷新背包
        /// </summary>
        void refreshPack() {
            currentPackContainer()?.requestRefresh();
            refreshMoney();
        }

        /// <summary>
        /// 刷新窗口
        /// </summary>
        protected override void refresh() {
            base.refresh();
            refreshMoney();
            refreshView();
        }

        /// <summary>
        /// 清除窗口
        /// </summary>
        protected override void clear() {
            base.clear();
            clearView();
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 切换视图
        /// </summary>
        public void switchView(int view) {
            switchView((View)view);
        }
        public void switchView(View view) {
            this.view = view;
            requestRefresh(true);
        }

        /// <summary>
        /// 开启数量选择窗口
        /// </summary>
        /// <param name="window">窗口</param>
        /// <param name="mode">模式</param>
        void startNumberWindow() {
            numberWindow.startWindow();
        }

        #region 道具操作

        /// <summary>
        /// 使用物品
        /// </summary>
        public void onBuy() {
            var item = operItem<UsableItem>();
            if (item == null) return;

            startNumberWindow();
        }

        /// <summary>
        /// 购买道具
        /// </summary>
        /// <param name="count"></param>
        public void buyItem(int count = 1) {
            var container = operContainer();
            var item = operItem<BaseItem>();
            itemSer.buyItem(container, item, count, onBuySuccess);
        }

        /// <summary>
        /// 操作成功回调
        /// </summary>
        protected virtual void onBuySuccess() {
            gameSys.requestAlert(BuySuccessText);
            numberWindow.terminateWindow();
            refreshPack();
        }

        #endregion

        #endregion
    }
}