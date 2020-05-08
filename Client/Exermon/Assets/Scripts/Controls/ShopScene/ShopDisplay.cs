using System;

using ExermonModule.Data;

using ItemModule.Data;
using ItemModule.Services;

using GameModule.Services;

using UI.Common.Controls.InputFields;
using UI.Common.Controls.ItemDisplays;

namespace UI.ShopScene.Controls {

    /// <summary>
    /// 艾瑟萌装备商店显示
    /// </summary>
    public class ShopDisplay<T> : 
        SelectableContainerDisplay<ItemService.ShopItem<T>> 
        where T: BaseItem, new() {

        /// <summary>
        /// 常量设置
        /// </summary>
        const string UnlimitedText = "不限";

        const int UnlimitedIndex = -1;

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public DropdownField typeSelector, starSelector;

        //public ShopItemDetail<T> itemDetail;

        /// <summary>
        /// 外部系统设置
        /// </summary>
        DataService dataSer;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeEvery() {
            base.initializeEvery();
            configureSelectors();
        }

        /// <summary>
        /// 配置筛选器
        /// </summary>
        void configureSelectors() {
            typeSelector.configure(generateTypesData());
            starSelector.configure(generateStarsData());
        }

        /// <summary>
        /// 生成类型下拉框数据
        /// </summary>
        /// <returns></returns>
        protected virtual Tuple<int, string>[] generateTypesData() {
            return new Tuple<int, string>[1] {
                new Tuple<int, string>(UnlimitedIndex, UnlimitedText),
            };
        }

        /// <summary>
        /// 生成类型下拉框数据
        /// </summary>
        /// <returns></returns>
        Tuple<int, string>[] generateStarsData() {
            var data = dataSer.staticData.configure.itemStars;
            var res = new Tuple<int, string>[data.Length + 1];

            res[0] = new Tuple<int, string>(UnlimitedIndex, UnlimitedText);
            for (int i = 1; i < res.Length; ++i) {
                var _data = data[i];
                res[i] = new Tuple<int, string>(
                    _data.getID(), _data.name);
            }

            return res;
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            dataSer = DataService.get();
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 获取物品帮助组件
        /// </summary>
        /// <returns>帮助组件</returns>
        //public override IItemDetailDisplay<ItemService.ShopItem<T>>
        //    getItemDetail() { return itemDetail; }

        /// <summary>
        /// 是否包含物品
        /// </summary>
        /// <param name="packItem">物品</param>
        /// <returns>返回指定物品能否包含在容器中</returns>
        protected override bool isInclude(ShopItem<T> packItem) {
            if (!base.isIncluded(packItem)) return false;
            bool flag = true;
            // 判断类型
            var typeIndex = typeSelector.getIndex();
            if (typeIndex != UnlimitedIndex)
                flag = typeIndex == ItemIndex;
            // 判断星级
            var starIndex = starSelector.getIndex();
            if (starIndex != UnlimitedIndex) 
                flag = starIndex == packItem.item().starId;

            return flag;
        }

        /// <summary>
        /// 是否包含装备
        /// </summary>
        /// <param name="packEquip">装备</param>
        /// <returns>返回指定装备能否包含在容器中</returns>
        protected override bool isEquipIncluded(ExerPackEquip packEquip) {
            if (!base.isIncluded(packEquip)) return false;
            bool flag = true;
            // 判断类型
            var typeIndex = typeSelector.getIndex();
            if (typeIndex != UnlimitedIndex)
                flag = typeIndex == EquipIndex;
            // 判断星级
            var starIndex = starSelector.getIndex();
            if (starIndex != UnlimitedIndex)
                flag = starIndex == packEquip.item().starId;

            return flag;
        }

        #endregion
    }
}