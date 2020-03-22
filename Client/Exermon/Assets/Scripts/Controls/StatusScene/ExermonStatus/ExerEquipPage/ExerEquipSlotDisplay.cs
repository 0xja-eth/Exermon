using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ExermonModule.Data;

using UI.Common.Controls.ItemDisplays;

/// <summary>
/// 状态场景命名空间
/// </summary>
namespace UI.StatusScene { }

/// <summary>
/// 状态场景控件
/// </summary>
namespace UI.StatusScene.Controls { }

/// <summary>
/// 状态场景中艾瑟萌状态页控件
/// </summary>
namespace UI.StatusScene.Controls.ExermonStatus { }

/// <summary>
/// 状态场景中艾瑟萌状态-装备页控件
/// </summary>
namespace UI.StatusScene.Controls.ExermonStatus.ExerEquipPage {

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