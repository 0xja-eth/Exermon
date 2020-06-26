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

using UI.ShopScene.Controls.HumanItem;
using UI.ShopScene.Controls.ExerEquip;

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
            HumanItem, ExerEquip,
        }

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public PackTabController tabController;

        public HumanItemShopDisplay humanItemShop;
        public ExerEquipShopDisplay exerEquipShop;

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
            scene = SceneUtils.getCurrentScene<ShopScene>();
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
            startWindow(View.HumanItem);
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
        public ShopDisplay<T> currentPackContainer<T>() where T: BaseItem, new() {
            if (typeof(T) == typeof(HumanItem))
                return humanItemShop as ShopDisplay<T>;
            if (typeof(T) == typeof(ExerEquip))
                return exerEquipShop as ShopDisplay<T>;
            return null;
        }

        /// <summary>
        /// 当前物品详情显示控件
        /// </summary>
        /// <returns></returns>
        public ShopItemDetail<T> currentItemDetail<T>() where T : BaseItem, new() {
            return currentPackContainer<T>().itemDetail;
        }

        /// <summary>
        /// 操作容器
        /// </summary>
        /// <returns></returns>
        public PackContainer<PackContItem> operContainer() {
            switch (view) {
                case View.HumanItem: return player.packContainers.humanPack;
                case View.ExerEquip: return player.packContainers.exerPack;
            }
            return null;
        }

        /// <summary>
        /// 当前操作商品
        /// </summary>
        /// <returns></returns>
        public ItemService.ShopItem<T> operShopItem<T>() where T : BaseItem, new() {
            return ((IItemDisplay<ItemService.ShopItem<T>>)
                currentItemDetail<T>())?.getItem();
        }
        public ItemService.ShopItem operShopItem() {
            switch (view) {
                case View.HumanItem: return operShopItem<HumanItem>();
                case View.ExerEquip: return operShopItem<ExerEquip>();
            }
            return null;
        }

        /// <summary>
        /// 当前操作物品
        /// </summary>
        /// <returns></returns>
        public BaseItem operItem() {
            switch (view) {
                case View.HumanItem: return operShopItem<HumanItem>().item();
                case View.ExerEquip: return operShopItem<ExerEquip>().item();
            }
            return null;
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 刷新视窗
        /// </summary>
        public void refreshView() {
            clearView();
            switch (view) {
                case View.HumanItem: onHumanItemShop(); break;
                case View.ExerEquip: onExerEquipShop(); break;
            }
        }

        /// <summary>
        /// 人类物品商店
        /// </summary>
        void onHumanItemShop() {
            humanItemShop.startView();
        }

        /// <summary>
        /// 艾瑟萌装备商店
        /// </summary>
        void onExerEquipShop() {
            exerEquipShop.startView();
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
            humanItemShop.terminateView();
            exerEquipShop.terminateView();
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
            var item = operItem();
            if (item == null) return;

            startNumberWindow();
        }

        /// <summary>
        /// 购买道具
        /// </summary>
        /// <param name="count"></param>
        public void buyItem(int buyType, int count = 1) {
            var item = operItem();
            var container = operContainer();
            itemSer.buyItem(container, item, 
                count, buyType, onBuySuccess);
        }

        /// <summary>
        /// 操作成功回调
        /// </summary>
        protected virtual void onBuySuccess() {
            gameSys.requestAlert(BuySuccessText);
            numberWindow.terminateWindow();
            refreshMoney();
        }

        #endregion

        #endregion
    }
}