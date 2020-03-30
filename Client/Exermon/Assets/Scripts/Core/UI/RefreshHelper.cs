
namespace Core.UI {

    /// <summary>
    /// 批量刷新组件
    /// </summary>
    public class RefreshHelper : BaseView {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public BaseView[] views;
        
        #region 界面控制

        /// <summary>
        /// 刷新视窗
        /// </summary>
        protected override void refresh() {
            base.refresh();
            foreach (var view in views)
                refreshView(view);
        }

        /// <summary>
        /// 刷新单个视窗
        /// </summary>
        /// <param name="view">视窗</param>
        protected virtual void refreshView(BaseView view) {
            view.requestRefresh(true);
        }

        #endregion
    }
}