
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using UI.Common.Controls.ItemDisplays;

using ExerPro.EnglishModule.Data;

using FrontendWrongItem = ExerPro.EnglishModule.Data.
    CorrectionQuestion.FrontendWrongItem;
using Assets.Scripts.Core.UI;

//改错布局
namespace UI.ExerPro.EnglishPro.CorrectionScene.Controls.Test {

    /// <summary>
    /// 改错题目显示
    /// </summary
    public class ArticleTestDisplay : SelectableContainerDisplay<string>,
        IItemDisplay<CorrectionQuestion> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public GameObject correctionWindow;
        public Text count;

        /// <summary>
        /// 外部变量定义
        /// </summary>
        public float scrollDelta = 64;
        public CorrectionLayout correctionLayout;
        /// <summary>
        /// 内部变量定义
        /// </summary>
        public Dictionary<WordDisplay, FrontendWrongItem> answers { get; set; }
            = new Dictionary<WordDisplay, FrontendWrongItem>();
        public Dictionary<WordDisplay, FrontendWrongItem> tmpAnswers { get; set; }
            = new Dictionary<WordDisplay, FrontendWrongItem>();
        
        

        #region 接口实现

        /// <summary>
        /// 内部变量定义
        /// </summary>
        CorrectionQuestion question;

        /// <summary>
        /// 获取物品
        /// </summary>
        /// <returns></returns>
        public CorrectionQuestion getItem() {
            return question;
        }

        /// <summary>
        /// 设置物品
        /// </summary>
        /// <param name="item">改错题项</param>
        /// <param name="force">强制</param>
        /// <returns>null</returns>
        public void setItem(CorrectionQuestion item, bool force = false) {
            question = item; setItems(item.sentences());
        }

        /// <summary>
        /// 开启视图
        /// </summary>
        /// <param name="item"></param>
        public void startView(CorrectionQuestion item) {
            startView(); setItem(item, true);
        }

        #endregion

        #region 数据控制

        #region 错误项控制
        

        /*
		/// <summary>
		/// 配置玩家答案
		/// </summary>
		void setupAnswer() {
			foreach (var item in question.wrongItems) {
				var front = item.convertToFrontendWrongItem();
				var word = getWordDisplay(front.sid, front.wid);

				word.clearCorrectWord();
			}
		}
		*/
        

        /// <summary>
        /// 重置所有答案
        /// </summary>
        public void revertAllAnswers() {
            correctionLayout.initialize();
            return;
            foreach (var pair in answers)
                pair.Key.revert();

            answers.Clear();

        }
        

        #endregion

        /// <summary>
        /// 获取剩余修改次数
        /// </summary>
        /// <returns></returns>
        public int getRestCount() {
            if (question == null) return 0;
            var max = question.wrongItems.Length;
            return max - answers.Count;
        }

        /// <summary>
        /// 能否进行修改
        /// </summary>
        /// <returns></returns>
        public bool isCorrectEnable() {
            return getRestCount() > 0;
        }
        

        #endregion

        #region 界面绘制

        /// <summary>
        /// 子视图创建回调
        /// </summary>
        /// <param name="sub"></param>
        /// <param name="index"></param>
        protected override void onSubViewCreated(SelectableItemDisplay<string> sub, int index) {
            base.onSubViewCreated(sub, index);

            var display = sub as WordTestDisplay;
            if (display == null) return;
            display.articleDisplay = this;
            display.originalWord = items[index];
        }

        /// <summary>
        /// 向上移动
        /// </summary>
        public void scrollUp() {
            var oriPos = container.anchoredPosition;
            oriPos.y -= scrollDelta;
            container.anchoredPosition = oriPos;
        }

        /// <summary>
        /// 向下移动
        /// </summary>
        public void scrollDown() {
            var oriPos = container.anchoredPosition;
            oriPos.y += scrollDelta;
            container.anchoredPosition = oriPos;
        }
        

        /// <summary>
        /// 刷新
        /// </summary>
        protected override void refresh() {
            base.refresh();
        }

        /// <summary>
        /// 清除
        /// </summary>
        protected override void clear() {
            base.clear();
            count.text = "";
        }

        #endregion

    }
}
