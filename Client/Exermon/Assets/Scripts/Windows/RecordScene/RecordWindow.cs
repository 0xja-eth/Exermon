using System;

using UnityEngine;

using Core.UI;
using Core.UI.Utils;

using PlayerModule.Services;
using RecordModule.Services;

using UI.RecordScene.Controls;
using UI.RecordScene.Controls.Question;

using UI.Common.Controls.InputFields;

namespace UI.RecordScene.Windows {

    /// <summary>
    /// 状态窗口
    /// </summary>
    public class RecordWindow : BaseWindow {

        /// <summary>
        /// 视图枚举
        /// </summary>
        public enum View {
            Question, Collect, Wrong,
            Exercise, Battle, Exam, Statistic
        }

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public RecordTabController tabController;
        public SubjectTabController subjectController;

        public QuestionRecordPage questionPage;

        public DateTimeField fromDate, toDate;

        /// <summary>
        /// 内部变量声明
        /// </summary>
        View view;
        int subjectId = 1; // 科目ID

        /// <summary>
        /// 场景组件引用
        /// </summary>
        RecordScene scene;

        /// <summary>
        /// 外部系统引用
        /// </summary>
        PlayerService playerSer = null;
        RecordService recordSer = null;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            configSubViews();
        }

        /// <summary>
        /// 配置子视图
        /// </summary>
        void configSubViews() {
            var now = DateTime.Now;
            var player = playerSer.player;
            subjectController.configure(player);

            fromDate.configure(player.createTime, now);
            toDate.configure(player.createTime, now, now);
        }

        /// <summary>
        /// 初始化场景
        /// </summary>
        protected override void initializeScene() {
            scene = (RecordScene)SceneUtils.getSceneObject("Scene");
        }

        /// <summary>
        /// 初始化系统/服务
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            playerSer = PlayerService.get();
            recordSer = RecordService.get();
        }
        
        #endregion

        #region 开启控制

        /// <summary>
        /// 开始窗口
        /// </summary>
        public override void startWindow() {
            startWindow(View.Question);
        }

        /// <summary>
        /// 开始窗口
        /// </summary>
        public void startWindow(View view) {
            base.startWindow();
            tabController.startView((int)view);
        }

        /// <summary>
        /// 结束视窗
        /// </summary>
        public override void terminateView() {
            base.terminateView();
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 设置科目ID
        /// </summary>
        /// <param name="sid"></param>
        public void setSubjectId(int sid) {
            subjectId = sid;
            requestRefresh();
        }

        /// <summary>
        /// 获取科目ID
        /// </summary>
        /// <returns></returns>
        public int getSubjectId() {
            return subjectId;
        }

        /// <summary>
        /// 获取筛选开始时间
        /// </summary>
        /// <returns></returns>
        public DateTime getFromDate() {
            return fromDate.getValue().Date;
        }

        /// <summary>
        /// 获取筛选结束时间
        /// </summary>
        /// <returns></returns>
        public DateTime getToDate() {
            return toDate.getValue().AddDays(1).Date;
        }

        /// <summary>
        /// 设置筛选器起始时间
        /// </summary>
        /// <param name="date"></param>
        public void setFromDate(DateTime date) {
            fromDate.setValue(date);
        }

        /// <summary>
        /// 设置筛选器结束时间
        /// </summary>
        /// <param name="date"></param>
        public void setToDate(DateTime date) {
            toDate.setValue(date);
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 刷新视窗
        /// </summary>
        public void refreshView() {
            clearView();
            switch (view) {
                case View.Question:
                    questionPage.startView(QuestionRecordPage.Mode.All); break;
                case View.Collect:
                    questionPage.startView(QuestionRecordPage.Mode.Collect); break;
                case View.Wrong:
                    questionPage.startView(QuestionRecordPage.Mode.Wrong); break;
            }
        }

        /// <summary>
        /// 清除视图
        /// </summary>
        public void clearView() {
            questionPage.terminateView();
        }

        /// <summary>
        /// 刷新窗口
        /// </summary>
        protected override void refresh() {
            base.refresh();
            refreshView();
        }

        /// <summary>
        /// 清除窗口
        /// </summary>
        protected override void clear() {
            base.clear();
            clearView();
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 三天内
        /// </summary>
        public void set3Days() {
            var now = DateTime.Now;
            var ts = new TimeSpan(3, 0, 0, 0);
            var from = (now - ts).Date;
            setFromDate(from); setToDate(now);
        }

        /// <summary>
        /// 本周
        /// </summary>
        public void setWeek() {
            var now = DateTime.Now;
            var dow = (int)now.DayOfWeek;
            var ts = new TimeSpan(dow, 0, 0, 0);
            var from = (now - ts).Date;
            setFromDate(from); setToDate(now);
        }

        /// <summary>
        /// 本月
        /// </summary>
        public void setMonth() {
            var now = DateTime.Now;
            var from = new DateTime(now.Year, now.Month, 1);
            setFromDate(from); setToDate(now);
        }

        /// <summary>
        /// 查询
        /// </summary>
        public void query() {
            requestRefresh(true);
        }

        /// <summary>
        /// 切换视图
        /// </summary>
        public void switchView(int view) {
            switchView((View)view);
        }
        public void switchView(View view) {
            this.view = view;
            requestRefresh(true);
        }

        #endregion
    }
}