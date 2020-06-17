using UnityEngine;
using ExerPro.EnglishModule.Data;

namespace UI.ExerPro.EnglishPro.BusinessManScene.Controls.CardItem {
    public class CardItemShopDisplay : ShopDisplay<ExerProCard> {

        #region 启动视图
        /// <summary>
        /// 启动视窗
        /// </summary>
        /// <param name="items"></param>
        public override void startView() {
            if (shopItems == null)
                Debug.Log("startView: " + name + ": null");
            else
                Debug.Log("startView: " + name + ": " + string.Join(", ", (object[])shopItems));
            base.startView();
            if (shopItems == null || shopItems.Length == 0) {
                base.startView();
                var d = dataSer.staticData.data.exerProCards;
                setItems(d);
            }
            else
                setItems(shopItems);
        }

        #endregion

    }

}
