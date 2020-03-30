
using UnityEngine.Events;

using Core.Data.Loaders;

using ExermonModule.Data;
using ExermonModule.Services;

using UI.Common.Controls.ItemDisplays;

/// <summary>
/// 状态场景中艾瑟萌状态-天赋页控件
/// </summary>
namespace UI.StatusScene.Controls.ExermonStatus.ExerGiftPage {
    
    /// <summary>
    /// 状态窗口艾瑟萌页信息显示
    /// </summary>
    public class ExerSlotItemDisplay : ExermonStatusSlotItemDisplay<PlayerExerGift> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public PlayerExerGiftDetail playerGiftDetail;

        public PackContainerDisplay packDisplay;

        /// <summary>
        /// 详情显示组件
        /// </summary>
        protected override ExermonStatusExerSlotDetail<PlayerExerGift> detail {
            get { return playerGiftDetail; }
            set { playerGiftDetail = (PlayerExerGiftDetail)value; }
        }

        /// <summary>
        /// 外部系统设置
        /// </summary>
        ExermonService exermonSer;

        #region 初始化

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            exermonSer = ExermonService.get();
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 装备改变回调
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="equipItem">装备项</param>
        protected override void onEquipChanged(PackContainerDisplay<PlayerExerGift> container, PlayerExerGift equipItem) {
            base.onEquipChanged(container, equipItem);
            var packDisplay = DataLoader.cast<PackContainerDisplay>(getPackDisplay());
            packDisplay?.setEquipItem(item.playerGift);
        }

        #endregion

        #region 请求控制

        /// <summary>
        /// 装备请求函数
        /// </summary>
        /// <returns></returns>
        protected override UnityAction<UnityAction> equipRequestFunc(PlayerExerGift item) {
            return action => exermonSer.equipPlayerGift(this.item, item, action);
        }

        #endregion
    }
}