﻿using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using QuestionModule.Data;
using RecordModule.Data;
using RecordModule.Services;

using UI.Common.Controls.InputFields;
using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.QuestionDisplay;

/// <summary>
/// 记录场景题目页
/// </summary>
namespace UI.RecordScene.Controls.Question {

    /// <summary>
    /// 题目记录页
    /// </summary>
    public class QuestionRecordDetail : ItemDetailDisplay<QuestionRecord> {

        /// <summary>
        /// 常量定义
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public QuestionDisplay questionDisplay;

        public GameObject wrongButton;

        public Toggle showAnswer;

        /// <summary>
        /// 内部变量定义
        /// </summary>
        PlayerQuestion playerQues = null; // 对应的玩家题目记录

        /// <summary>
        /// 外部系统设置
        /// </summary>
        RecordService recordSer;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            recordSer = RecordService.get();
        }

        #endregion

        #region 画面绘制

        /// <summary>
        /// 绘制物品
        /// </summary>
        /// <param name="item">物品</param>
        protected override void drawExactlyItem(QuestionRecord item) {
            base.drawExactlyItem(item);
            drawQuestion(item);
            drawButtonsStatus(item);
        }

        /// <summary>
        /// 绘制题目内容
        /// </summary>
        /// <param name="record"></param>
        void drawQuestion(QuestionRecord record) {
            questionDisplay.setItem(record.question());
            questionDisplay.result = playerQues;
        }

        /// <summary>
        /// 绘制按钮状态
        /// </summary>
        void drawButtonsStatus(QuestionRecord record) {
            if (wrongButton) wrongButton.SetActive(record.wrong);
        }

        /// <summary>
        /// 清除物品绘制
        /// </summary>
        protected override void drawEmptyItem() {
            base.drawEmptyItem();
            questionDisplay.clearItem();

            if (wrongButton) wrongButton.SetActive(false);
        }

        #endregion

        #region 操作控制

        /// <summary>
        /// 开启/关闭答案
        /// </summary>
        public void toggleAnswer() {
            questionDisplay.showAnswer = showAnswer.isOn;
        }
        /*
        /// <summary>
        /// 收藏/解除收藏
        /// </summary>
        public void toggleCollect() {
            if (item == null) return;
            recordSer.collect(item, () => drawButtonsStatus(item));
        }
        */
        /// <summary>
        /// 解除错题
        /// </summary>
        public void unwrong() {
            if (item == null) return;
            recordSer.unwrong(item, () => drawButtonsStatus(item));
        }

        #endregion
    }
}
