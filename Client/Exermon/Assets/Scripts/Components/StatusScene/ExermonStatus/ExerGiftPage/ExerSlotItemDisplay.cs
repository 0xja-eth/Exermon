
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
        /// 艾瑟萌装备页
        /// </summary>
        namespace ExerGiftPage {

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
    }
}