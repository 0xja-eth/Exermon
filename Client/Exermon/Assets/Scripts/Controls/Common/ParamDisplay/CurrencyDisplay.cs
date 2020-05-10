
using UnityEngine;
using UnityEngine.UI;

using ItemModule.Data;

using Core.UI;

namespace UI.Common.Controls.ParamDisplays {

    /// <summary>
    /// 信誉积分显示
    /// </summary>
    public class CurrencyDisplay : ParamDisplay<ItemPrice> {

        /// <summary>
        /// 信誉积分最大值
        /// </summary>
        const int MaxCredit = 100;

        /// <summary>
        /// 外部变量设置
        /// </summary>
        public Text gold, ticket, boundTicket; // 货币

        /// <summary>
        /// 外部变量定义
        /// </summary>
        public string goldFormat = "{0}";
        public string ticketFormat = "{0}";
        public string boundTicketFormat = "{0}";

        #region 界面绘制

        /// <summary>
        /// 绘制值
        /// </summary>
        protected override void drawExactlyValue(ItemPrice data) {
            base.drawExactlyValue(base.data);
            gold.text = string.Format(goldFormat, data.gold);
            ticket.text = string.Format(ticketFormat, data.ticket);
            boundTicket.text = string.Format(boundTicketFormat, data.boundTicket);
        }

        /// <summary>
        /// 绘制空值
        /// </summary>
        protected override void drawEmptyValue() {
            base.drawEmptyValue();
            gold.text = string.Format(goldFormat, 0);
            ticket.text = string.Format(ticketFormat, 0);
            boundTicket.text = string.Format(boundTicketFormat, 0);
        }

        #endregion

    }
}