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

using UI.PackScene.Controls;

using UI.PackScene.Controls.GeneralPack;

using HumanPackDisplay = UI.PackScene.Controls.
    GeneralPack.HumanPackDisplay;
using ExerPackDisplay = UI.PackScene.Controls.
    GeneralPack.ExerPackDisplay;

using UI.Common.Controls.ParamDisplays;
using UI.Common.Controls.ItemDisplays;

namespace UI.PackScene.Windows {

    /// <summary>
    /// 状态窗口
    /// </summary>
    public class PackWindow : BaseWindow {

        /// <summary>
        /// 文本常量定义
        /// </summary>
        const string UseSuccessText = "使用成功！";
        const string SellSuccessText = "出售成功！";
        const string DiscardSuccessText = "丢弃成功！";

        /// <summary>
        /// 视图枚举
        /// </summary>
        public enum View {
            Human, Exermon, QuesSugar,
        }

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public PackTabController tabController;

        public HumanPackDisplay humanPack;
        public ExerPackDisplay exerPack;

        public PackItemDetail itemDetail;

        public NumberInputWindow numberWindow;
        public TargetSelectWindow selectWindow;

        public CurrencyDisplay moneyDisplay;

        /// <summary>
        /// 内部变量声明
        /// </summary>
        View view;
        Player player;

        NumberInputWindow currentNumberWindow;

        /// <summary>
        /// 场景组件引用
        /// </summary>
        PackScene scene;

        /// <summary>
        /// 外部系统引用
        /// </summary>
        GameSystem gameSys = null;
        SceneSystem sceneSys = null;
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
            scene = SceneUtils.getCurrentScene<PackScene>();
        }

        /// <summary>
        /// 初始化系统/服务
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            gameSys = GameSystem.get();
            sceneSys = SceneSystem.get();
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

        /// <summary>
        /// 结束视窗
        /// </summary>
        public override void terminateView() {
            base.terminateView();
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 当前背包容器
        /// </summary>
        /// <returns></returns>
        public PackContainerDisplay<PackContItem> 
            currentPackContainer() {
            switch (view) {
                case View.Human: return humanPack;
                case View.Exermon: return exerPack;
                case View.QuesSugar: return null;
            }
            return null;
        }

        /// <summary>
        /// 当前物品详情显示控件
        /// </summary>
        /// <returns></returns>
        public PackContItemDetail currentItemDetail() {
            switch (view) {
                case View.Human: return itemDetail;
                case View.Exermon: return itemDetail;
                case View.QuesSugar: return null;
            }
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
            refreshMoney();
            switch (view) {
                case View.Human: onHumanPack(); break;
                case View.Exermon: onExerPack(); break;
                case View.QuesSugar: onQuesSugar(); break;
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
        /*
        /// <summary>
        /// 刷新背包
        /// </summary>
        void refreshPack() {
            currentPackContainer()?.requestRefresh();
            refreshMoney();
        }
        */
        /// <summary>
        /// 刷新窗口
        /// </summary>
        protected override void refresh() {
            base.refresh();
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
        void startNumberWindow(NumberInputWindow window, 
            NumberInputWindow.Mode mode) {
            currentNumberWindow = window;
            currentNumberWindow.startWindow(mode);
        }

        #region 道具操作

        /// <summary>
        /// 装备
        /// </summary>
        public void onEquip() {
            sceneSys.pushScene(
                SceneSystem.Scene.StatusScene,
                new int[] { 1, 2 }
            );
        }

        /// <summary>
        /// 使用物品
        /// </summary>
        public void onUse() {
            var item = operItem<UsableItem>();
            if (item == null) return;

            var packItem = operPackItem();

            if (item.target == (int)UsableItem.TargetType.Exermon)
                startNumberWindow(selectWindow, NumberInputWindow.Mode.Use);
            else if (packItem.count > 1 && item.batchCount != 1)
                startNumberWindow(numberWindow, NumberInputWindow.Mode.Use);
            else useItem();
        }

        /// <summary>
        /// 出售物品
        /// </summary>
        public void onSell() {
            var item = operItem<LimitedItem>();
            if (item == null) return;

            var packItem = operPackItem();
            if (packItem.count > 1)
                startNumberWindow(numberWindow,
                    NumberInputWindow.Mode.Sell);
            else sellItem();
        }

        /// <summary>
        /// 丢弃物品
        /// </summary>
        public void onDiscard() {
            var item = operItem<LimitedItem>();
            if (item == null) return;

            var packItem = operPackItem();
            if (packItem.count > 1)
                startNumberWindow(numberWindow,
                    NumberInputWindow.Mode.Discard);
            else discardItem();
        }

        /// <summary>
        /// 使用道具
        /// </summary>
        /// <param name="count"></param>
        public void useItem(int count = 1, PlayerExermon target = null) {
            var container = operContainer();
            var contItem = operPackItem();
            if (target == null) 
                itemSer.useItem(container, contItem, count, ItemUseOccasion.Menu,
                    () => onOperSuccess(NumberInputWindow.Mode.Use));
            else
                itemSer.useItem(container, contItem, count, ItemUseOccasion.Menu, 
                    target, () => onOperSuccess(NumberInputWindow.Mode.Use));
        }

        /// <summary>
        /// 出售道具
        /// </summary>
        /// <param name="count"></param>
        public void sellItem(int count = 1) {
            var container = operContainer();
            var contItem = operPackItem();
            itemSer.sellItem(container, contItem, count,
                () => onOperSuccess(NumberInputWindow.Mode.Sell));
        }

        /// <summary>
        /// 丢弃道具
        /// </summary>
        /// <param name="count"></param>
        public void discardItem(int count = 1) {
            var container = operContainer();
            var contItem = operPackItem();
            itemSer.discardItem(container, contItem, count,
                () => onOperSuccess(NumberInputWindow.Mode.Discard));
        }

        /// <summary>
        /// 操作成功回调
        /// </summary>
        protected virtual void onOperSuccess(NumberInputWindow.Mode mode) {
            var successText = "";
            switch (mode) {
                case NumberInputWindow.Mode.Use:
                    successText = UseSuccessText; break;
                case NumberInputWindow.Mode.Sell:
                    successText = SellSuccessText; break;
                case NumberInputWindow.Mode.Discard:
                    successText = DiscardSuccessText; break;
            }
            if (successText != "") gameSys.requestAlert(successText);
            numberWindow.terminateWindow();
            selectWindow.terminateWindow();
            refreshView();
        }

        #endregion

        #endregion

    }
}