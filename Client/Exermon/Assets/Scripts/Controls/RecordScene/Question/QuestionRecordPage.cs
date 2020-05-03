﻿using System;
using System.Collections.Generic;

using QuestionModule.Data;
using RecordModule.Data;
using RecordModule.Services;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.QuestionDisplay;

/// <summary>
/// 记录场景题目页
/// </summary>
namespace UI.RecordScene.Controls.Question {

    /// <summary>
    /// 题目记录页
    /// </summary>
    public class QuestionRecordPage : ItemDetailDisplay<QuestionRecord> {

        /// <summary>
        /// 模式
        /// </summary>
        public enum Mode {
            All, Collect, Wrong, Exercise, Battle, Exam
        }

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public QuestionRecordDetail questionDisplay;
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
        public void configure(int mode) {
            base.configure();
            this.mode = (Mode)mode;
        }

        #endregion

        #region 数据控制

        #endregion

        #region 画面绘制

        /// <summary>
        /// 绘制物品
        /// </summary>
        /// <param name="item">物品</param>
        protected override void drawExactlyItem(QuestionRecord item) {
            base.drawExactlyItem(item);
            if (record != null && (int)mode > (int)Mode.Wrong) {
                questionNav.setItems(record.getQuestionRecords());
                questionNav.results = record.questions;
            } else {
                questionNav.setItems(records);
                questionNav.results = null;
            }
        }

        #endregion
    }
}
