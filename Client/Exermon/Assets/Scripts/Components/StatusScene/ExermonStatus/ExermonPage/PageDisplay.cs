
using System;
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
        /// 艾瑟萌页
        /// </summary>
        namespace ExermonPage {

            /// <summary>
            /// 状态窗口艾瑟萌页信息显示
            /// </summary>
            public class PageDisplay : ExermonStatusPageDisplay<PlayerExermon> {

                /// <summary>
                /// 外部组件设置
                /// </summary>
                public ExerSlotItemDisplay slotItemDisplay;

                public PackContainerDisplay packDisplay;
                
                #region 启动控制

                /// <summary>
                /// 加载函数
                /// </summary>
                /// <returns></returns>
                public override UnityAction<UnityAction> getLoadFunction() {
                    return action => ExermonService.get().loadExerHub(action);
                }
                
                #endregion

                #region 数据控制

                /// <summary>
                /// 获取容器组件
                /// </summary>
                protected override ItemContainer<PlayerExermon>
                    getPackDisplay() { return packDisplay; }

                /// <summary>
                /// 获取艾瑟萌槽项显示组件
                /// </summary>
                protected override ExermonStatusSlotItemDisplay<PlayerExermon>
                    getSlotItemDisplay() { return slotItemDisplay; }
                
                /// <summary>
                /// 背包条件
                /// </summary>
                /// <returns></returns>
                protected override Predicate<PlayerExermon> packCondition() {
                    return (item) => (item.exermon().subjectId == this.item.subjectId);
                }

                #endregion

                #region 界面绘制

                #endregion

                #region 流程控制
                
                #region 请求控制

                /// <summary>
                /// 装备请求函数
                /// </summary>
                /// <returns></returns>
                protected override UnityAction<UnityAction> equipRequestFunc() {
                    var equip = slotItemDisplay.getEquip();
                    return action => exerSer.equipPlayerExer(item, equip, action); ;
                }

                #endregion

                #endregion
            }
        }
    }
}