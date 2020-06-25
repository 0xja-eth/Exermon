using Core.UI;
using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;
using UI.ExerPro.EnglishPro.ExerProPackScene.Controls;
using UnityEngine;
using UI.ExerPro.EnglishPro.ExerProPackScene.Pack;
using Core.Systems;
using PlayerModule.Services;
using RecordModule.Services;
using ItemModule.Services;
using UI.Common.Controls.ItemDisplays;
using ItemModule.Data;
using UI.ExerPro.EnglishPro.ExerProPackScene.CardPackItemDetail;

namespace UI.ExerPro.EnglishPro.ExerProPackScene.Windows {
    public class ExerProPackWindow : BaseWindow {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public PackTabController tabController;
        public ExerProPackItemDetail packItemDetail;
        public GameObject detail;

        /// <summary>
        /// 内部变量声明
        /// </summary>
        View view;
        RuntimeActor player;

        /// <summary>
        /// 外部系统引用
        /// </summary>
        GameSystem gameSys = null;
        PlayerService playerSer = null;
        RecordService recordSer = null;
        ItemService itemSer = null;
        EnglishService engSer = null;

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
        public View
            CurrentPackContainer() {
            //switch (view) {
            //    case View.Item: return itemPackDisplay;
            //    case View.Potion: return potionPackDisplay;
            //    case View.Card: return cardPackDisplay;
            //}
            //return null;
            return view;
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
            packItemDetail.updateButtons(view);
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
            itemPackDisplay.startView();
            itemPackDisplay.setItems(items);
            detail.SetActive(true);
            packItemDetail.clearItem();
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
            potionPackDisplay.startView();
            potionPackDisplay.setItems(potions);
            detail.SetActive(true);
            packItemDetail.clearItem();
        }

        /// <summary>
        /// 卡片背包
        /// </summary>
        void onCardPack() {
            var cards = engSer.record.actor.cardGroup.items;
            cardPackDisplay.startView();
            cardPackDisplay.setItems(cards.ToArray());
            detail.SetActive(false);
            packItemDetail.clearItem();
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


        /// <summary>
        /// 装备
        /// </summary>
        public void equip() {
            var slot = player.potionSlot;
            //if ()
        }

        /// <summary>
        /// 装备
        /// </summary>
        public void equip(ExerProPackPotion potion) {
            var slot = player.potionSlot;
            slot.setEquip(player.potionPack, potion);
        }

        #endregion
    }
}
