using UnityEngine;
using ExerPro.EnglishModule.Data;
using Core.Data.Loaders;

namespace UI.ExerPro.EnglishPro.BusinessManScene.Controls.PotionItem {
    public class PotionItemShopDisplay : ShopDisplay<ExerProPotion> {

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
                var d = dataSer.staticData.data.exerProPotions;

                //LitJson.JsonData data = new LitJson.JsonData();
                //data["gold"] = 1;
                //data["starId"] = 1;
                //d = DataLoader.load(d, data);
                setItems(d);
            }
            else
                setItems(shopItems);
        }

        #endregion

    }

}
