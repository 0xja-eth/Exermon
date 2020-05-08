
using UnityEngine;
using UnityEngine.UI;

using Core.UI;

using ItemModule.Data;

using UI.Common.Controls.ItemDisplays;

using UI.ShopScene.Windows;

namespace UI.ShopScene.Controls {

    /// <summary>
    /// 购买方式选择器
    /// </summary>
    public class BuySelector : ItemDisplay<ItemPrice> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public GameObject gold, ticket, boundTicket;
        public Text goldPrice, ticketPrice, boundTicketPrice;
        public Toggle goldBuy, ticketBuy, boundTicketBuy;

        /// <summary>
        /// 内部变量定义
        /// </summary>
        int count = 1;
        bool itemChanged = false;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeEvery() {
            base.initializeEvery();
            goldBuy.isOn = ticketBuy.isOn = boundTicketBuy.isOn = false;
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 获取购买方式
        /// </summary>
        /// <returns></returns>
        public int getBuyType() {
            if (goldBuy.isOn) return 0;
            if (ticketBuy.isOn) return 1;
            if (boundTicketBuy.isOn) return 2;
            return -1;
        }

        /// <summary>
        /// 设置个数
        /// </summary>
        /// <param name="count">个数</param>
        public void setCount(int count) {
            this.count = count;
            requestRefresh();
        }

        /// <summary>
        /// 物品改变回调
        /// </summary>
        protected override void onItemChanged() {
            base.onItemChanged();
            count = 1; itemChanged = true;
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制实际物品
        /// </summary>
        protected override void drawExactlyItem(ItemPrice price){
            goldPrice.text = (price.gold * count).ToString();
            ticketPrice.text = (price.ticket * count).ToString();
            boundTicketPrice.text = (price.boundTicket * count).ToString();

            boundTicket.SetActive(price.boundTicket > 0);
            ticket.SetActive(price.ticket > 0);
            gold.SetActive(price.gold > 0);

            if (itemChanged) {
                boundTicketBuy.isOn = price.boundTicket > 0;
                ticketBuy.isOn = price.ticket > 0;
                goldBuy.isOn = price.gold > 0;
            }
        }

        /// <summary>
        /// 绘制空物品
        /// </summary>
        protected override void drawEmptyItem() {
            base.drawEmptyItem();
            gold.SetActive(false);
            ticket.SetActive(false);
            boundTicket.SetActive(false);
        }

        /// <summary>
        /// 刷新
        /// </summary>
        protected override void refresh() {
            base.refresh();
            itemChanged = false;
        }

        #endregion

    }

}
