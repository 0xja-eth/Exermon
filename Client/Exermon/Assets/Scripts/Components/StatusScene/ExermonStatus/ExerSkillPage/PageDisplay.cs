using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// 状态场景
/// </summary>
namespace StatusScene {

    /// <summary>
    /// 艾瑟萌状态
    /// </summary>
    namespace ExermonStatus {

        /// <summary>
        /// 艾瑟萌技能页
        /// </summary>
        namespace ExerSkillPage {

            /// <summary>
            /// 状态窗口艾瑟萌页信息显示
            /// </summary>
            public class PageDisplay : ExermonStatusPageDisplay {

                /// <summary>
                /// 外部组件设置
                /// </summary>
                public ExerSkillSlotDisplay skillSlotDisplay;

                #region 界面绘制

                /// <summary>
                /// 刷新装备槽
                /// </summary>
                void refreshExerEquipSlot() {
                    var items = item.playerExer.exerSkillSlot.items;
                    skillSlotDisplay.configure(items);
                }

                /// <summary>
                /// 刷新
                /// </summary>
                protected override void refresh() {
                    base.refresh();
                    if (item != null) refreshExerEquipSlot();
                }

                #endregion

                #region 流程控制

                #endregion
            }
        }
    }
}