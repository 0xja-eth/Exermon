using UnityEngine;

using QuestionModule.Data;
using RecordModule.Data;

using UI.Common.Controls.ItemDisplays;
using UI.RecordScene.Windows;

namespace UI.RecordScene.Controls.Question {

    /// <summary>
    /// 题目记录导航
    /// </summary>
    public class QuestionNavigation :
        SelectableContainerDisplay<QuestionRecord>, 
        IItemDetailDisplay<QuestionSetRecord> {

        /// <summary>
        /// 常量设置
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public RecordWindow window;
        public QuestionRecordPage page;

        public QuestionRecordDetail detail;

        /// <summary>
        /// 显示结果
        /// </summary>
        IQuestionResult[] _results = null;
        public IQuestionResult[] results {
            get { return _results; }
            set {
                _results = value;
                requestRefresh();
            }
        }

        #region 数据控制

        /// <summary>
        /// 获取物品帮助组件
        /// </summary>
        /// <returns>帮助组件</returns>
        public override IItemDetailDisplay<QuestionRecord> getItemDetail() {
            return detail;
        }

        /// <summary>
        /// 是否包含某题目记录
        /// </summary>
        /// <param name="item">题目记录</param>
        /// <returns></returns>
        protected override bool isIncluded(QuestionRecord item) {
            if (!base.isIncluded(item)) return false;
            // 模式
            switch (page.mode) {
                case QuestionRecordPage.Mode.All: break;
                case QuestionRecordPage.Mode.Collect:
                    if (!item.collected) return false; break;
                case QuestionRecordPage.Mode.Wrong:
                    if (!item.wrong) return false; break;
                default: // 题目集
                    return true;
                    // 是否包含该题目
                    // return page.record.hasQuestion(item.questionId);
            }
            // 检查日期范围
            if (window.getFromDate() > item.lastDate ||
                item.lastDate > window.getToDate()) return false;
            // 检查科目
            if (window.getSubjectId() != item.question().subjectId)
                return false;
            return true;
        }

        #endregion

        #region 界面绘制

        #endregion

        #region 接口实现

        /// <summary>
        /// 题目
        /// </summary>
        QuestionSetRecord record;

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="container"></param>
        public void configure(IContainerDisplay<QuestionSetRecord> container) { }

        /// <summary>
        /// 获取物品
        /// </summary>
        /// <returns></returns>
        public QuestionSetRecord getItem() { return record; }

        /// <summary>
        /// 设置物品
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="index"></param>
        /// <param name="refresh"></param>
        public void setItem(QuestionSetRecord item, int _ = -1, bool __ = false) {
            record = item; setItems(item.getQuestionRecords());
        }
        public void setItem(QuestionSetRecord item, bool _ = false) {
            setItem(item, -1, _);
        }

        public void clearItem() {
            setItem(null, -1);
        }

        /// <summary>
        /// 开启视窗
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="index"></param>
        /// <param name="refresh"></param>
        public void startView(QuestionSetRecord item, int _ = -1) {
            startView();
            setItem(item, _, true);
        }
        public void startView(QuestionSetRecord item) {
            startView();
            setItem(item, true);
        }

        #endregion

    }
}