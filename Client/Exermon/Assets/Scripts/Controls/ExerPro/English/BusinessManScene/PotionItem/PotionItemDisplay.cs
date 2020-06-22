using ExerPro.EnglishModule.Data;
using GameModule.Services;

namespace UI.ExerPro.EnglishPro.BusinessManScene.Controls.PotionItem {
    /// <summary>
    /// 装备背包显示
    /// </summary>
    public class PotionItemDisplay : ShopItemDisplay<ExerProPotion> {

        protected override void drawPrice(ExerProPotion item) {
            if (price == 0) {
                price = CalcService.ExerProItemGenerator.generatePotionPrice(item);
                item.gold = price;
            }
            if (price > 0) {
                priceText.text = price.ToString();
                setPriceTag(goldTag);
            }
            else {
                priceText.text = "";
                setPriceTag(null);
            }
        }

    }
}