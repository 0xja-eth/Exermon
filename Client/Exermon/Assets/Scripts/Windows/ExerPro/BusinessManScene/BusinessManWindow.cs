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

using UI.Common.Controls.ItemDisplays;

using ExerPro.EnglishModule.Data;
using UI.ExerPro.BusinessManScene.Controls;
using UI.ExerPro.BusinessManScene.Controls.CardItem;
using UI.ExerPro.BusinessManScene.Controls.PotionItem;
using UI.ExerPro.BusinessManScene.ParamDisplays;

using ExerPro.EnglishModule.Services;

namespace UI.ExerPro.BusinessManScene.Windows
{

    /// <summary>
    /// 状态窗口
    /// </summary>
    public class BusinessManWindow : BaseWindow
    {

        /// <summary>
        /// 文本常量定义
        /// </summary>
        const string BuySuccessText = "购买成功！";

        /// <summary>
        /// 视图枚举
        /// </summary>
        public enum View
        {
            CardItem, PotionItem,
        }

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public PackTabController tabController;

        public CardItemShopDisplay cardItemShop;
        public PotionItemShopDisplay potionItemShop;

        //public NumberInputWindow numberWindow;

        public CurrencyDisplay moneyDisplay;

        /// <summary>
        /// 内部变量声明
        /// </summary>
        View view;
        ExerProActor player;

        /// <summary>
        /// 场景组件引用
        /// </summary>
        BusinessManScene scene;

        /// <summary>
        /// 外部系统引用
        /// </summary>
        GameSystem gameSys = null;
        PlayerService playerSer = null;
        RecordService recordSer = null;
        ItemService itemSer = null;
        EnglishService englishSer = null;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce()
        {
            base.initializeOnce();
            player = EnglishService.get().record.actor;
        }

        /// <summary>
        /// 初始化场景
        /// </summary>
        protected override void initializeScene()
        {
            scene = (BusinessManScene)SceneUtils.getSceneObject("Scene");
        }

        /// <summary>
        /// 初始化系统/服务
        /// </summary>
        protected override void initializeSystems()
        {
            base.initializeSystems();
            gameSys = GameSystem.get();
            playerSer = PlayerService.get();
            recordSer = RecordService.get();
            itemSer = ItemService.get();
            englishSer = EnglishService.get();
        }

        #endregion

        #region 开启控制

        /// <summary>
        /// 开始窗口
        /// </summary>
        public override void startWindow()
        {
            startWindow(View.CardItem);
        }

        /// <summary>
        /// 开始窗口
        /// </summary>
        public void startWindow(View view)
        {
            base.startWindow();
            tabController.startView((int)view);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 当前商店显示容器
        /// </summary>
        /// <returns></returns>
        public ShopDisplay currentPackContainer()
        {
            switch (view)
            {
                case View.CardItem: return cardItemShop;
                case View.PotionItem: return potionItemShop;
            }
            return null;
        }

        /// <summary>
        /// 当前操作背包容器
        /// </summary>
        /// <returns></returns>
        public PackContainer<T> operContainer<T>() where T : PackContItem, new()
        {
            if (typeof(T) == typeof(ExerProPackCard))
                return (PackContainer<T>)(object)player.cardGroup;
            else if (typeof(T) == typeof(ExerProPackPotion))
                return (PackContainer<T>)(object)player.potionPack;
            return null;
        }

        /// <summary>
        /// 操作商品
        /// </summary>
        /// <returns></returns>
        public BaseExerProItem operShopItem()
        {
            var currPack = currentPackContainer();
            return currPack?.selectedItem();
        }

        public
        #endregion

        #region 界面绘制

        /// <summary>
        /// 刷新视窗
        /// </summary>
        void refreshView()
        {
            clearView();
            switch (view)
            {
                case View.CardItem: onCardItemShop(); break;
                case View.PotionItem: onPotionItemShop(); break;
            }
        }

        /// <summary>
        /// 卡牌物品商店
        /// </summary>
        void onCardItemShop()
        {
            cardItemShop.startView();
        }

        /// <summary>
        /// 药水物品商店
        /// </summary>
        void onPotionItemShop()
        {
            potionItemShop.startView();
        }

        /// <summary>
        /// 刷新金钱
        /// </summary>
        void refreshMoney()
        {
            moneyDisplay.setValue(player.gold);
        }

        /// <summary>
        /// 清除视图
        /// </summary>
        public void clearView()
        {
            cardItemShop.terminateView();
            potionItemShop.terminateView();
        }

        /// <summary>
        /// 刷新窗口
        /// </summary>
        protected override void refresh()
        {
            base.refresh();
            refreshMoney();
            refreshView();
        }

        /// <summary>
        /// 清除窗口
        /// </summary>
        protected override void clear()
        {
            base.clear();
            clearView();
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 切换视图
        /// </summary>
        public void switchView(int view)
        {
            switchView((View)view);
        }
        public void switchView(View view)
        {
            this.view = view;
            requestRefresh(true);
        }

        #region 道具操作

        /// <summary>
        /// 使用物品
        /// </summary>
        public void onBuy()
        {
            var item = operShopItem();
            if (item == null) return;

        }

        /// <summary>
        /// 购买道具
        /// </summary>
        /// <param name="count"></param>
        public void buyItem()
        {
            switch (view)
            {
                case View.CardItem:
                    buyCardItem();
                    break;
                case View.PotionItem:
                    buyPotionItem();
                    break;
            }
        }

        public void buyCardItem()
        {
            var container = (ExerProCardGroup)operContainer<ExerProPackCard>();
            var item = (ExerProCard)operShopItem();
            if (container == null || item == null)
                return;
            var price = item.gold;
            player.gainGold(price);
            container.addCard(item);
            onBuySuccess();
        }

        public void buyPotionItem()
        {
            var container = (ExerProPotionPack)operContainer<ExerProPackPotion>();
            var item = (ExerProPotion)operShopItem();
            if (container == null || item == null)
                return;
            var price = item.gold;
            player.gainGold(price);
            container.pushItem(new ExerProPackPotion(item));
            onBuySuccess();
        }
        /// <summary>
        /// 操作成功回调
        /// </summary>
        protected virtual void onBuySuccess()
        {
            gameSys.requestAlert(BuySuccessText);
            refreshMoney();
        }

        #endregion

        #endregion
    }
}