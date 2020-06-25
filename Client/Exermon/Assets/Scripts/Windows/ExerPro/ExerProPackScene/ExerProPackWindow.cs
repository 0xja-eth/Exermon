using Core.UI;
using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;
using UI.ExerPro.EnglishPro.ExerProPackScene.Controls;
using UnityEngine;
using UI.ExerPro.EnglishPro.ExerProPackScene.Pack;

namespace UI.ExerPro.EnglishPro.ExerProPackScene.Windows {
    public class ExerProPackWindow : BaseWindow {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public PackTabController tabController;


        /// <summary>
        /// 内部变量声明
        /// </summary>
        View view;

        /// <summary>
        /// 外部系统设置
        /// </summary>
        EnglishService engSer;

        /// <summary>
        /// 视图枚举
        /// </summary>
        public enum View {
            Item, Potion, Card
        }

        public ItemPackDisplay itemPackDisplay;
        public CardPackDisplay cardPackDisplay;
        public PotionPackDisplay potionPackDisplay;

        #region 初始化
        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            engSer = EnglishService.get();
        }
        #endregion
        #region 开启控制

        /// <summary>
        /// 开始窗口
        /// </summary>
        public override void startWindow() {
            startWindow(View.Item);
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
        //public SelectableContainerDisplay<ExerProPackItem<BaseExerProItem>>
        //    currentPackContainer() {
        //    switch (view) {
        //        case View.Card: return cardPackDisplay;
        //        case View.Item: return itemPackDisplay;
        //            //case View.Potion: return cardPackDisplay;
        //    }
        //    return null;
        //}
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
        #endregion

        #region 界面绘制

        /// <summary>
        /// 刷新视窗
        /// </summary>
        public void refreshView() {
            clearView();
            //refreshMoney();
            switch (view) {
                case View.Item: onItemPack(); break;
                case View.Potion: onPotionPack(); break;
                case View.Card: onCardPack(); break;
            }
        }

        /// <summary>
        /// 刷新窗口
        /// </summary>
        protected override void refresh() {
            base.refresh();
            refreshView();
        }

        /// <summary>
        /// 物品背包
        /// </summary>
        void onItemPack() {
            //var container = engSer.record.actor.itemPack;
            ExerProPackItem[] items = new ExerProPackItem[8];
            items[0] = ExerProPackItem.sample();
            items[1] = ExerProPackItem.sample();
            items[2] = ExerProPackItem.sample();
            items[3] = ExerProPackItem.sample();
            items[4] = ExerProPackItem.sample();
            items[5] = ExerProPackItem.sample();
            items[6] = ExerProPackItem.sample();
            items[7] = ExerProPackItem.sample();
            itemPackDisplay.setItems(items);
            itemPackDisplay.startView();

            //startWindow(View.Item);
            //humanPack.startView(); humanPack.setPackData(container);
        }

        /// <summary>
        /// 药水背包
        /// </summary>
        void onPotionPack() {
            var container = engSer.record.actor.potionPack;
            ExerProPackPotion[] potions = new ExerProPackPotion[8];
            potions[0] = ExerProPackPotion.sample();
            potions[1] = ExerProPackPotion.sample();
            potions[2] = ExerProPackPotion.sample();
            potions[3] = ExerProPackPotion.sample();
            potions[4] = ExerProPackPotion.sample();
            potions[5] = ExerProPackPotion.sample();
            potions[6] = ExerProPackPotion.sample();
            potions[7] = ExerProPackPotion.sample();
            potionPackDisplay.setItems(potions);
            potionPackDisplay.startView();
            //startWindow(View.Potion);
            //exerPack.startView(); exerPack.setPackData(container);
        }

        /// <summary>
        /// 卡片背包
        /// </summary>
        void onCardPack() {
            var container = engSer.record.actor.cardGroup;
            ExerProPackCard[] cards = new ExerProPackCard[8];
            cards[0] = ExerProPackCard.sample();
            cards[1] = ExerProPackCard.sample();
            cards[2] = ExerProPackCard.sample();
            cards[3] = ExerProPackCard.sample();
            cards[4] = ExerProPackCard.sample();
            cards[5] = ExerProPackCard.sample();
            cards[6] = ExerProPackCard.sample();
            cards[7] = ExerProPackCard.sample();
            cardPackDisplay.setItems(cards);
            cardPackDisplay.startView();
            //cardPackDisplay.setPackData(container);
        }
        /// <summary>
        /// 清除视图
        /// </summary>
        public void clearView() {
            itemPackDisplay.terminateView();
            cardPackDisplay.terminateView();
            potionPackDisplay.terminateView();
        }

        #endregion
    }
}
