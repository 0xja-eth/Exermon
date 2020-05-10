
using ExermonModule.Data;

using UI.Common.Controls.ItemDisplays;

using UI.PackScene.Windows;

namespace UI.PackScene.Controls.TargetSelect {

    /// <summary>
    /// 艾瑟萌仓库显示
    /// </summary>
    public class ExerHubDisplay : PackContainerDisplay<PlayerExermon> {

        /// <summary>
        /// 常量设置
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public PlayerExerParamDetail detail; // 帮助界面

        public TargetSelectWindow selectWindow;

        /// <summary>
        /// 内部变量声明
        /// </summary>
        int subjectId = 0;

        #region 数据控制

        /// <summary>
        /// 获取物品帮助组件
        /// </summary>
        /// <returns>帮助组件</returns>
        public override IItemDetailDisplay<PlayerExermon> getItemDetail() {
            return detail;
        }

        /*
        /// <summary>
        /// 是否包含容器项
        /// </summary>
        /// <param name="packItem"></param>
        /// <returns></returns>
        protected override bool isIncluded(PlayerExermon packItem) {
            var res = base.isIncluded(packItem);
            var item = selectWindow.operPackItem();

            TODO: 添加可用性判断
        }
        */

        #endregion
    }
}