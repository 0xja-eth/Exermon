using System;

using Core.Data;

using ItemModule.Services;

namespace UI.ShopScene.Controls.HumanItem {

    using HumanItem = PlayerModule.Data.HumanItem;

    /// <summary>
    /// 人类商店显示
    /// </summary>
    public class HumanItemShopDisplay : ShopDisplay<HumanItem> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public HumanItemDetail humanItemDetail;

        /// <summary>
        /// 物品详情控件
        /// </summary>
        public override ShopItemDetail<HumanItem> itemDetail {
            get{
                return humanItemDetail;
            }
            set {
                humanItemDetail = value as HumanItemDetail;   
            }
        }
        
        #region 初始化
        
        /// <summary>
        /// 类型数据
        /// </summary>
        /// <returns></returns>
        protected override TypeData[] typeData() {
            return dataSer.staticData.configure.usableItemTypes;

        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 是否包含物品
        /// </summary>
        /// <param name="shopItem">物品</param>
        /// <returns>返回指定物品能否包含在容器中</returns>
        protected override bool isIncluded(ItemService.ShopItem<HumanItem> shopItem) {
            if (!base.isIncluded(shopItem)) return false;

            var typeIndex = typeSelector.getValueId();
            return typeIndex == UnlimitedIndex || 
                typeIndex == shopItem.item().iType;
        }

        #endregion
    }
}