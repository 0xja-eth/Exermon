using UnityEngine;

using RecordModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.Common.Controls.QuestionDisplay {
    using QuestionModule.Data;
    using Question = QuestionModule.Data.Question;

    /// <summary>
    /// 题目选项容器
    /// </summary>
    public class QuesChoiceContainer :
        SelectableContainerDisplay<Question.Choice>, IItemDetailDisplay<Question> {

        /// <summary>
        /// 常量设置
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public QuesPictureContainer pictureContaienr; // 图片容器

        /// <summary>
        /// 显示结果
        /// </summary>
        IQuestionResult _result = null; // 是否显示答案
        public IQuestionResult result {
            get { return _result; }
            set {
                _result = value;
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
                _showAnswer = value;
                requestRefresh();
            }
        }

        #region 数据控制

        /// <summary>
        /// 同步选择
        /// </summary>
        void setupSelection() {
            clearChecks();
            if (result == null) return;
            var selection = result.getSelection();
            if (selection == null) return;
            foreach (var sel in selection)
                check(items.Find(c => c.order == sel), true);
        }

        /// <summary>
        /// 物品变更回调
        /// </summary>
        protected override void onItemsChanged() {
            base.onItemsChanged();
            if (items != null) setupSelection();
        }

        #endregion

        #region 画面绘制

        /// <summary>
        /// 子窗口创建回调
        /// </summary>
        /// <param name="sub">子视图</param>
        /// <param name="index">索引</param>
        protected override void onSubViewCreated(SelectableItemDisplay<Question.Choice> sub, int index) {
            base.onSubViewCreated(sub, index);
            var choice = sub as QuesChoiceDisplay;
            if (choice == null || choice.text == null) return;
            choice.text.imageContainer = pictureContaienr;
        }

        #endregion

        #region 接口实现

        /// <summary>
        /// 题目
        /// </summary>
        Question question;

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="container"></param>
        public void configure(IContainerDisplay<Question> container) { }

        /// <summary>
        /// 获取物品
        /// </summary>
        /// <returns></returns>
        public Question getItem() { return question; }

        /// <summary>
        /// 设置物品
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="index"></param>
        /// <param name="refresh"></param>
        public void setItem(Question item, int _ = -1, bool __ = false) {
            question = item;
            setItems(item.shuffleChoices());
            if (item.isMultiple()) maxCheck = 0;
            else maxCheck = 1;
        }

        public void setItem(Question item, bool _ = false) {
            setItem(item, -1, _);
        }

        public void clearItem() {
            setItem(null, -1);
        }

        public void startView(Question item, int _ = -1) {
            startView(); setItem(item, _, true);
        }

        public void startView(Question item) {
            startView(); setItem(item, true);
        }

        #endregion

    }
}