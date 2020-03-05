using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        namespace ExerEquipPage {

            /// <summary>
            /// 艾瑟萌装备槽显示
            /// </summary>
            public class ExerEquipSlotDisplay : ItemContainer<ExerEquipSlotItem> {
                
                /// <summary>
                /// 外部组件设置
                /// </summary>
                public ExerSlotItemDisplay exerSlotItemDisplay; // 帮助界面

                /// <summary>
                /// 内部变量声明
                /// </summary>

                #region 数据控制

                /// <summary>
                /// 获取物品帮助组件
                /// </summary>
                /// <returns>帮助组件</returns>
                protected override IItemDetail<ExerEquipSlotItem> getItemDetail() {
                    return exerSlotItemDisplay;
                }

                #endregion
            }
        }
    }
}