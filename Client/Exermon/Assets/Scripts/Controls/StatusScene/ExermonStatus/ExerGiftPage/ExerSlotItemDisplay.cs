
using ExermonModule.Data;

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

        /// <summary>
        /// 详情显示组件
        /// </summary>
        protected override ExermonStatusExerSlotDetail<PlayerExerGift> detail {
            get { return playerGiftDetail; }
            set { playerGiftDetail = (PlayerExerGiftDetail)value; }
        }
    }
}