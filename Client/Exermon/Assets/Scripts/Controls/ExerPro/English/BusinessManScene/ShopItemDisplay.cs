
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Core.Data.Loaders;

using ItemModule.Data;
using ItemModule.Services;

using UI.ShopScene.Controls;

using UI.Common.Controls.ParamDisplays;
using UI.Common.Controls.ItemDisplays;

using ExerPro.EnglishModule.Data;

namespace UI.ExerPro.EnglishPro.BusinessManScene.Controls {

    /// <summary>
    /// 特训商店显示基类
    /// </summary>
    public class ExerProShopItemDisplay<T> :
		ShopItemDisplay<T> where T : BaseExerProItem, new() {

		/// <summary>
		/// 常量定义
		/// </summary>

		/// <summary>
		/// 外部组件定义
		/// </summary>
		/*
		public BaseItemDisplay itemDisplay;

        public StarsDisplay starsDisplay;

        public Image icon, priceTag;
        public Text name, priceText;

        public Texture2D goldTag;
		*/

		/*
        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            initializeDrawFuncs();
        }

        /// <summary>
        /// 初始化绘制函数
        /// </summary>
        protected void initializeDrawFuncs() {
            itemDisplay.registerItemType<T>(drawItem);
        }

        #endregion
		*/

        #region 界面控制

        /// <summary>
        /// 绘制基本信息
        /// </summary>
        /// <param name="item">物品</param>
        protected override void drawBaseInfo(T item) {
            name.text = item.name;

            // 处理物品星级和图标情况
            starsDisplay?.setValue(item.starId);

            icon.gameObject.SetActive(true);
            icon.overrideSprite = item.icon;
        }
		
        #endregion

    }
}