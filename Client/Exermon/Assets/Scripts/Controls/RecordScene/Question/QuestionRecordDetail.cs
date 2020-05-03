using System;
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
    public class QuestionRecordDetail : ItemDetailDisplay<QuestionRecord> {
        
        /// <summary>
        /// 外部组件设置
        /// </summary>
        public QuestionDisplay questionDisplay;

        /// <summary>
        /// 内部变量定义
        /// </summary>
        PlayerQuestion playerQues = null; // 对应的玩家题目记录

        #region 画面绘制

        /// <summary>
        /// 绘制物品
        /// </summary>
        /// <param name="item">物品</param>
        protected override void drawExactlyItem(QuestionRecord item) {
            base.drawExactlyItem(item);
            questionDisplay.setItem(item.question());
            questionDisplay.result = playerQues;
        }

        #endregion
    }
}
