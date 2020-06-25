using System;

using UnityEngine;

using Core.Data;

using ItemModule.Data;
using ItemModule.Services;

using GameModule.Services;

using UI.Common.Controls.InputFields;
using UI.Common.Controls.ItemDisplays;

using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;

using UI.ShopScene.Controls;

namespace UI.ExerPro.EnglishPro.BusinessManScene.Controls {

    /// <summary>
    /// 商人据点显示
    /// </summary>
    public abstract class ExerProShopDisplay<T> : 
		ShopDisplay<T> where T : BaseExerProItem, new() {

        /// <summary>
        /// 外部系统设置
        /// </summary>
        protected EnglishService engSer;

        #region 初始化

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            engSer = EnglishService.get();
        }

		#endregion

		#region 数据控制

		/// <summary>
		/// 类型数据
		/// </summary>
		/// <returns></returns>
		protected override TypeData[] typeData() {
			return new TypeData[0];
		}

		/// <summary>
		/// 星级数据
		/// </summary>
		/// <returns></returns>
		protected override TypeData[] starData() {
			return dataSer.staticData.configure.exerProItemStars;
		}

		/// <summary>
		/// 生成商品
		/// </summary>
		protected override void generateShop() {
			engSer.shopGenerate<T>(setItems);
		}

		#endregion

	}
}