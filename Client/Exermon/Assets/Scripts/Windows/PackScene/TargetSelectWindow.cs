
using ItemModule.Data;
using ExermonModule.Data;

using UI.PackScene.Controls.TargetSelect;

/// <summary>
/// 状态场景窗口
/// </summary>
namespace UI.PackScene.Windows {

    /// <summary>
    /// 目标选择窗口
    /// </summary>
    public class TargetSelectWindow : NumberInputWindow {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public ExerHubDisplay exerHub;
        public PlayerExerParamDetail paramDetail;

        /// <summary>
        /// 内部变量声明
        /// </summary>

        /// <summary>
        /// 场景组件引用
        /// </summary>

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            configureExerHub();
        }

        /// <summary>
        /// 配置艾瑟萌仓库
        /// </summary>
        void configureExerHub() {
            exerHub.configure(playerSer.player.packContainers.exerHub);
            exerHub.select(0);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 当前选择艾瑟萌
        /// </summary>
        /// <returns></returns>
        public PlayerExermon currentTarget() {
            return paramDetail.getItem();
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 值变化回调
        /// </summary>
        protected override void onValueChanged(int value) {
            base.onValueChanged(value);
            refreshParams();
        }

        /// <summary>
        /// 刷新属性
        /// </summary>
        void refreshParams() {
            paramDetail.requestRefresh();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        protected override void refresh() {
            base.refresh();
            refreshParams();
        }

        /// <summary>
        /// 清除窗口
        /// </summary>
        protected override void clear() {
            base.clear();
            exerHub.clearItems();
            paramDetail.requestClear(true);
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 使用回调
        /// </summary>
        protected override void onUse() {
            var target = currentTarget();
            var count = currentCount();
            packWindow.useItem(count, target);
        }

        #endregion
    }
}