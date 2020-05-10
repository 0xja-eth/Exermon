
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using Core.UI;
using Core.UI.Utils;

using ItemModule.Data;

using UI.Common.Controls.ParamDisplays;

using UI.ShopScene.Windows;

namespace UI.ShopScene.Controls {

    /// <summary>
    /// 购买方式选择器
    /// </summary>
    public class BuySelector : ParamDisplay<ItemPrice> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public GameObject gold, ticket, boundTicket;

        /// <summary>
        /// 内部组件设置
        /// </summary>
        Text goldPrice, ticketPrice, boundTicketPrice;
        Toggle goldBuy, ticketBuy, boundTicketBuy;

        /// <summary>
        /// 内部变量定义
        /// </summary>
        int count = 1, lastBuyType = -1;
        bool itemChanged = false;

        /// <summary>
        /// 购买方式变化回调
        /// </summary>
        public UnityAction onChanged { get; set; }

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            initializeControls();
        }

        /// <summary>
        /// 更新子控件
        /// </summary>
        void initializeControls() {
            goldPrice = SceneUtils.find<Text>(gold, "Value");
            ticketPrice = SceneUtils.find<Text>(ticket, "Value");
            boundTicketPrice = SceneUtils.find<Text>(boundTicket, "Value");

            goldBuy = SceneUtils.find<Toggle>(gold, "Toggle");
            ticketBuy = SceneUtils.find<Toggle>(ticket, "Toggle");
            boundTicketBuy = SceneUtils.find<Toggle>(boundTicket, "Toggle");

            goldBuy.isOn = ticketBuy.isOn = boundTicketBuy.isOn = false;
        }

        /// <summary>
        /// 配置价格
        /// </summary>
        /// <param name="price"></param>
        public void configure(ItemPrice price) {
            base.configure(); setValue(price);
            requestRefresh(true);
        }

        #endregion

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            updateBuyType();
        }

        /// <summary>
        /// 更新购买类型
        /// </summary>
        void updateBuyType() {
            var buyType = getBuyType();
            if (lastBuyType != buyType)
                onBuyTypeChanged(buyType);
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
            Debug.Log("setCount: " + count);
            requestRefresh(true);
        }

        /// <summary>
        /// 物品改变回调
        /// </summary>
        protected override void onValueChanged(bool force = false) {
            base.onValueChanged(force);
            count = 1; itemChanged = true;
        }

        /// <summary>
        /// 购买类型改变
        /// </summary>
        void onBuyTypeChanged(int type) {
            lastBuyType = type;
            onChanged?.Invoke();
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制实际物品
        /// </summary>
        protected override void drawExactlyValue(ItemPrice price){
            Debug.Log("drawExactlyValue: " + count);

            goldPrice.text = (price.gold * count).ToString();
            ticketPrice.text = (price.ticket * count).ToString();
            boundTicketPrice.text = (price.boundTicket * count).ToString();

            gold.SetActive(price.gold > 0);
            ticket.SetActive(price.ticket > 0);
            boundTicket.SetActive(price.boundTicket > 0);

            if (itemChanged) {
                boundTicketBuy.isOn = price.boundTicket > 0;
                ticketBuy.isOn = price.ticket > 0;
                goldBuy.isOn = price.gold > 0;
            }
        }

        /// <summary>
        /// 绘制空物品
        /// </summary>
        protected override void drawEmptyValue() {
            base.drawEmptyValue();
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
