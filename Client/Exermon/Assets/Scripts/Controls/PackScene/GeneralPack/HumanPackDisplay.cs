using System;

using PlayerModule.Data;
using ItemModule.Data;

using GameModule.Services;

using UI.Common.Controls.InputFields;

using UI.Common.Controls.ItemDisplays;
using CommonHumanPackDisplay = UI.Common.Controls.ItemDisplays.HumanPackDisplay;

namespace UI.PackScene.Controls.GeneralPack {

    /// <summary>
    /// 人类背包显示
    /// </summary>
    public class HumanPackDisplay : CommonHumanPackDisplay {

        /// <summary>
        /// 常量设置
        /// </summary>
        const string UnlimitedText = "不限";
        const string EquipText = "人物装备";
        const string ItemText = "人物物品";

        const int UnlimitedIndex = -1;
        const int EquipIndex = 0;
        const int ItemIndex = 1;

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public DropdownField typeSelector, starSelector;

        public PackItemDetail itemDetail;

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
        Tuple<int, string>[] generateTypesData() {
            return new Tuple<int, string>[3] {
                new Tuple<int, string>(UnlimitedIndex, UnlimitedText),
                new Tuple<int, string>(EquipIndex, EquipText),
                new Tuple<int, string>(ItemIndex, ItemText),
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
        protected override IItemDetailDisplay<PackContItem> 
            getItemDetail() { return itemDetail; }

        /// <summary>
        /// 是否包含物品
        /// </summary>
        /// <param name="packItem">物品</param>
        /// <returns>返回指定物品能否包含在容器中</returns>
        protected override bool isItemIncluded(HumanPackItem packItem) {
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
        protected override bool isEquipIncluded(HumanPackEquip packEquip) {
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