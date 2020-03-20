using System.Collections.Generic;
using UnityEngine.Events;

using ExermonModule.Data;
using ExermonModule.Services;

using UI.Common.Controls.ItemDisplays;

namespace UI.StatusScene.Controls.ExermonStatus.ExerGiftPage {

    /// <summary>
    /// 状态窗口艾瑟萌页信息显示
    /// </summary>
    public class PageDisplay : ExermonStatusPageDisplay<PlayerExerGift> {

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
            return action => ExermonService.get().loadExerGiftPool(action);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 获取容器组件
        /// </summary>
        protected override ItemContainer<PlayerExerGift>
            getPackDisplay() { return packDisplay; }

        /// <summary>
        /// 获取艾瑟萌槽项显示组件
        /// </summary>
        protected override ExermonStatusSlotItemDisplay<PlayerExerGift>
            getSlotItemDisplay() { return slotItemDisplay; }

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
            return action => exerSer.equipPlayerGift(item, equip, action);
        }

        #endregion

        #endregion
    }
}