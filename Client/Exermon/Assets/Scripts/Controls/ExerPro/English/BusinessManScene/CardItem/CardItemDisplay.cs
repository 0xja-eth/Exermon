using ExerPro.EnglishModule.Data;
using UnityEngine;
using UnityEngine.UI;
namespace UI.ExerPro.EnglishPro.BusinessManScene.Controls.CardItem
{
    /// <summary>
    /// 装备背包显示
    /// </summary>
    public class CardItemDisplay : ShopItemDisplay<ExerProCard> {
        /// <summary>
        /// 外部组件定义
        /// </summary>
        public Text cost;
        public Text description;

        /// <summary>
        /// 绘制基本信息
        /// </summary>
        /// <param name="item">物品</param>
        protected override void drawBaseInfo(ExerProCard item)
        {
            base.drawBaseInfo(item);
            cost.text = item.cost.ToString();
            description.text = item.description;
        }
    }
}