using System;

using Core.Data;

using ItemModule.Services;

namespace UI.ShopScene.Controls.ExerEquip {

    using ExerEquip = ExermonModule.Data.ExerEquip;

    /// <summary>
    /// 艾瑟萌装备商店显示
    /// </summary>
    public class ExerEquipShopDisplay : ShopDisplay<ExerEquip> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public ExerEquipDetail exerEquipDetail;

        /// <summary>
        /// 物品详情控件
        /// </summary>
        public override ShopItemDetail<ExerEquip> itemDetail {
            get{
                return exerEquipDetail;
            }
            set {
                exerEquipDetail = value as ExerEquipDetail;   
            }
        }

        #region 初始化
        
        /// <summary>
        /// 类型数据
        /// </summary>
        /// <returns></returns>
        protected override TypeData[] typeData() {
            return dataSer.staticData.configure.exerEquipTypes;

        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 是否包含物品
        /// </summary>
        /// <param name="shopItem">物品</param>
        /// <returns>返回指定物品能否包含在容器中</returns>
        protected override bool isIncluded(ItemService.ShopItem<ExerEquip> shopItem) {
            if (!base.isIncluded(shopItem)) return false;
            
            var typeIndex = typeSelector.getValueId();
            return typeIndex == UnlimitedIndex || 
                typeIndex == shopItem.item().eType;
        }

        #endregion
    }
}