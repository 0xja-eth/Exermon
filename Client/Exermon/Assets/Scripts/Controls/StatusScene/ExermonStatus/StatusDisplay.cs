
using UnityEngine;

using PlayerModule.Data;

using GameModule.Data;
using GameModule.Services;

using UI.Common.Controls.ItemDisplays;

namespace UI.StatusScene.Controls.ExermonStatus {

    /// <summary>
    /// 状态窗口艾瑟萌视图
    /// </summary>
    public class StatusDisplay : ItemDisplay<Player> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public PageTabController tabController;

        public SubjectTabController subjectController;

        public GameObject confirm, equip, dequip;

        /// <summary>
        /// 内部组件设置
        /// </summary>

        /// <summary>
        /// 内部变量设置
        /// </summary>
        Subject subject;

        /// <summary>
        /// 外部系统设置
        /// </summary>
        DataService dataSer;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            dataSer = DataService.get();
        }

        #endregion

        #region 视窗启动

        /// <summary>
        /// 开始视窗
        /// </summary>
        public override void startView() {
            startView(1);
        }

        /// <summary>
        /// 开始视窗
        /// </summary>
        /// <param name="subject">科目</param>
        public void startView(Subject subject) {
            base.startView();
            setSubject(subject);
            switchButtons();
        }
        /// <param name="sid">科目ID</param>
        public void startView(int sid) {
            base.startView();
            setSubject(dataSer.subject(sid));
            switchButtons();
        }

        /// <summary>
        /// 切换按钮
        /// </summary>
        void switchButtons() {
            confirm.SetActive(false);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 获取科目
        /// </summary>
        /// <returns></returns>
        public Subject getSubject() {
            return subject;
        }

        /// <summary>
        /// 设置科目
        /// </summary>
        /// <param name="subject">科目</param>
        public void setSubject(Subject subject) {
            this.subject = subject;
            onItemChanged();
        }

        /// <summary>
        /// 物品改变回调
        /// </summary>
        protected override void onItemChanged() {
            base.onItemChanged();

            subjectController.setPlayer(item);
            tabController.setup(item, subject);
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 清空视图
        /// </summary>
        protected override void clear() {
            base.clear();
        }

        #endregion

        #region 流程控制

        #endregion
    }
}