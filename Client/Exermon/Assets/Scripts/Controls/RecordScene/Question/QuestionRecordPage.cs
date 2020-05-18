using System;
using System.Collections.Generic;

using Core.UI;

using RecordModule.Data;
using RecordModule.Services;

/// <summary>
/// 记录场景题目页
/// </summary>
namespace UI.RecordScene.Controls.Question {

    using Question = QuestionModule.Data.Question;

    /// <summary>
    /// 题目记录页
    /// </summary>
    public class QuestionRecordPage : BaseView {

        /// <summary>
        /// 模式
        /// </summary>
        public enum Mode {
            All, Collect, Wrong, Exercise, Battle, Exam
        }

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public QuestionRecordDetail recordDisplay;
        public QuestionNavigation questionNav;
        
        /// <summary>
        /// 内部变量定义
        /// </summary>
        List<QuestionRecord> records;

        /// <summary>
        /// 模式
        /// </summary>
        Mode _mode = Mode.All;
        public Mode mode {
            get { return _mode; }
            set {
                _mode = value;
                requestRefresh();
            }
        }

        /// <summary>
        /// 对应的题目集记录
        /// </summary>
        QuestionSetRecord _record = null;
        public QuestionSetRecord record {
            get { return _record; }
            set {
                _record = value;
                requestRefresh();
            }
        }

        /// <summary>
        /// 呃外部系统设置
        /// </summary>
        RecordService recordSer;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            records = recordSer.recordData.questionRecords;
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            recordSer = RecordService.get();
        }

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="mode">模式</param>
        public void configure(int mode) {
            configure((Mode)mode);
        }
        public void configure(Mode mode) {
            base.configure();
            this.mode = mode;
        }

        #endregion

        #region 启动控制

        /// <summary>
        /// 启动视窗
        /// </summary>
        public override void startView() {
            base.startView();
            questionNav.startView();
        }

        /// <summary>
        /// 启动视窗
        /// </summary>
        /// <param name="mode">模式</param>
        public void startView(int mode) {
            startView((Mode)mode);
        }
        public void startView(Mode mode) {
            startView(); this.mode = mode;
        }

        /// <summary>
        /// 结束视窗
        /// </summary>
        public override void terminateView() {
            base.terminateView();
            questionNav.terminateView();
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 获取当前题目
        /// </summary>
        /// <returns></returns>
        public Question currentQuestion() {
            return recordDisplay.questionDisplay.getItem();
        }

        #endregion

        #region 画面绘制

        /// <summary>
        /// 绘制物品
        /// </summary>
        protected override void refresh() {
            base.refresh();
            if (record != null && (int)mode > (int)Mode.Wrong) {
                questionNav.setItems(record.getQuestionRecords());
                questionNav.results = record.questions;
            } else {
                questionNav.setItems(records);
                questionNav.results = null;
            }
            questionNav.select(0);
        }

        #endregion

    }
}
