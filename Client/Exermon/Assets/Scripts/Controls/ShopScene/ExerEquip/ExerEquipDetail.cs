
using GameModule.Services;

using UI.Common.Controls.ParamDisplays;

namespace UI.ShopScene.Controls.ExerEquip {

    using ExerEquip = ExermonModule.Data.ExerEquip;

    /// <summary>
    /// 物品详情
    /// </summary>
    public class ExerEquipDetail : ShopItemDetail<ExerEquip> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public ParamDisplaysGroup paramsGroup;

        /// <summary>
        /// 外部系统设置
        /// </summary>
        DataService dataSer;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            configureParamsGroup();
        }

        /// <summary>
        /// 配置属性组
        /// </summary>
        void configureParamsGroup() {
            var params_ = dataSer.staticData.configure.baseParams;
            paramsGroup.configure(params_);
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            dataSer = DataService.get();
        }

        #endregion

    }

}