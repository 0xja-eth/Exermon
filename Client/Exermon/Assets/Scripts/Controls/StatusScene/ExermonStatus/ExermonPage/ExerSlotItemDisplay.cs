
using UnityEngine.Events;

using ExermonModule.Data;
using ExermonModule.Services;

/// <summary>
/// 状态场景中艾瑟萌状态-艾瑟萌页控件
/// </summary>
namespace UI.StatusScene.Controls.ExermonStatus.ExermonPage {

    /// <summary>
    /// 状态窗口艾瑟萌页信息显示
    /// </summary>
    public class ExerSlotItemDisplay : ExermonStatusSlotItemDisplay<PlayerExermon> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public PlayerExermonDetail playerExerDetail;
                
        /// <summary>
        /// 详情显示组件
        /// </summary>
        protected override ExermonStatusExerSlotDetail<PlayerExermon> detail {
            get { return playerExerDetail; }
            set { playerExerDetail = (PlayerExermonDetail)value; }
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
        /// 装备请求函数
        /// </summary>
        /// <returns></returns>
        protected override UnityAction<UnityAction> equipRequestFunc(PlayerExermon item) {
            return action => exermonSer.equipPlayerExer(this.item, item, action);
        }

        #endregion
    }
}