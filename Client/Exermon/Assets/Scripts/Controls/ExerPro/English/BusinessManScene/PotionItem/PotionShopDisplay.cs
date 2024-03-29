﻿using Core.Data;
using UnityEngine;

using GameModule.Services;

using ExerPro.EnglishModule.Data;

using UI.Common.Controls.ItemDisplays;
using UI.ShopScene.Controls;

namespace UI.ExerPro.EnglishPro.BusinessManScene.Controls.PotionItem {

	/// <summary>
	/// 药水商店显示
	/// </summary>
	public class PotionShopDisplay : ExerProShopDisplay<ExerProPotion> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public PotionShopItemDetail potionItemDetail;

        /// <summary>
        /// 内部变量
        /// </summary>
        //const int PotionNumberOnce = 3;

		/// <summary>
		/// 物品详情控件
		/// </summary>
		public override ShopItemDetail<ExerProPotion> itemDetail {
            get {
                return potionItemDetail;
            }
            set {
                potionItemDetail = value as PotionShopItemDetail;
            }
        }

		#region 启动视图
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
            itemDetail.startView();
            if (shopItems == null || shopItems.Length == 0) {
                var stageOrder = engSer.record.stageOrder;
                var items = CalcService.ExerProItemGenerator.generateBusinessItem(
                    PotionNumberOnce, stageOrder, CalcService.ExerProItemGenerator.Type.Potion);
                ExerProPotion[] exerProPotions = new ExerProPotion[items.Count];
                for (int i = 0; i < items.Count; i++) {
                    exerProPotions[i] = items[i] as ExerProPotion;
                }
                setItems(exerProPotions);
            }
            else
                setItems(shopItems);
        }
		*/
		/*
		/// <summary>
		/// 结束视窗
		/// </summary>
		public override void terminateView() {
            base.terminateView();
            itemDetail.terminateView();
        }
		*/
        #endregion
		
	}

}
