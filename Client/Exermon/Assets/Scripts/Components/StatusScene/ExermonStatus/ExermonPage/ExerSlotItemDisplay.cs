
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 状态场景
/// </summary>
namespace StatusScene {

    /// <summary>
    /// 艾瑟萌状态
    /// </summary>
    namespace ExermonStatus {

        /// <summary>
        /// 艾瑟萌页
        /// </summary>
        namespace ExermonPage {

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
            }
        }
    }
}