using UnityEngine;

using QuestionModule.Data;
using RecordModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.Common.Controls.QuestionDisplay {
    
    /// <summary>
    /// 题目导航栏
    /// </summary>
    public class QuestionNavigation :
        SelectableContainerDisplay<Question>, 
        IItemDetailDisplay<QuestionSetRecord> {

        /// <summary>
        /// 常量设置
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public GameObject prevBtn;

        public QuestionDisplay detail; // 题目显示

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

        /// <summary>
        /// 显示答案解析
        /// </summary>
        bool _showAnswer = false;
        public bool showAnswer {
            get { return _showAnswer; }
            set {
                detail.showAnswer = value;
                selectable = _showAnswer = value;
                requestRefresh();
            }
        }

        #region 数据控制

        /// <summary>
        /// 获取物品帮助组件
        /// </summary>
        /// <returns>帮助组件</returns>
        public override IItemDetailDisplay<Question> getItemDetail() {
            return detail;
        }

        /// <summary>
        /// 下一个
        /// </summary>
        public void next(bool force = false) {
            select(selectedIndex + 1, force);
        }

        /// <summary>
        /// 上一个
        /// </summary>
        public void prev(bool force = false) {
            select(selectedIndex - 1, force);
        }

        /// <summary>
        /// 当前是否为最后一个题目
        /// </summary>
        /// <returns></returns>
        public bool isLastQuestion() {
            return selectedIndex == itemsCount() - 1;
        }

        /// <summary>
        /// 选择改变回调
        /// </summary>
        protected override void onSelectChanged() {
            base.onSelectChanged();
            Debug.Log("selectedIndex: " + name + ": " + selectedIndex);
            if (results != null && selectedIndex < results.Length)
                detail.result = results[selectedIndex];
            if (!isItemVisible(selectedIndex))
                scrollTo(selectedIndex);
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 刷新
        /// </summary>
        protected override void refresh() {
            base.refresh();
            if (prevBtn) prevBtn.SetActive(showAnswer);
        }

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
        public void setItem(QuestionSetRecord item, int _ = -1, bool __ = false) {
            record = item; setItems(item.getQuestions());
        }

        public void setItem(QuestionSetRecord item, bool _ = false) {
            setItem(item, -1, _);
        }

        public void clearItem() {
            setItem(null, -1);
        }

        public void startView(QuestionSetRecord item, int _ = -1) {
            startView(); setItem(item, _, true);
        }

        public void startView(QuestionSetRecord item) {
            startView(); setItem(item, true);
        }

        #endregion

    }
}