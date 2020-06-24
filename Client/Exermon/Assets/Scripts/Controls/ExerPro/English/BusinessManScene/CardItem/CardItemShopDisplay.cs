using UnityEngine;

using GameModule.Services;

using ExerPro.EnglishModule.Data;

using UI.ShopScene.Controls;

namespace UI.ExerPro.EnglishPro.BusinessManScene.Controls.CardItem {

	/// <summary>
	/// 卡牌商店显示
	/// </summary>
	public class CardItemShopDisplay : ExerProShopDisplay<ExerProCard> {

        /// <summary>
        /// 内部变量
        /// </summary>
        //const int CardNumberOnce = 10;

		/// <summary>
		/// 物品详情控件
		/// </summary>
		public override ShopItemDetail<ExerProCard> itemDetail {
			get { return null; }
			set { }
		}

		#region 启动/结束视图
		/*
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
                var stageOrder = engSer.record.stageOrder;
                var items = CalcService.ExerProItemGenerator.generateBusinessItem(CardNumberOnce, stageOrder);
                ExerProCard[] exerProCards = new ExerProCard[items.Count];
                for(int i = 0; i < items.Count; i++) {
                    exerProCards[i] = items[i] as ExerProCard;
                }
                setItems(exerProCards);
            }
            else
                setItems(shopItems);
        }
		*/
        #endregion

    }

}
