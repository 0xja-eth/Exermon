
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using UI.Common.Controls.ItemDisplays;

using ExerPro.EnglishModule.Data;

using FrontendWrongItem = ExerPro.EnglishModule.Data.
	CorrectionQuestion.FrontendWrongItem;

namespace UI.ExerPro.EnglishPro.CorrectionScene.Controls {

    /// <summary>
    /// 改错题目显示
    /// </summary
    public class ArticleDisplay : ContainerDisplay<string>,
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

		/// <summary>
		/// 内部变量定义
		/// </summary>
		public Dictionary<WordDisplay, FrontendWrongItem> answers { get; set; }
			= new Dictionary<WordDisplay, FrontendWrongItem>();
		public Dictionary<WordDisplay, FrontendWrongItem> tmpAnswers { get; set; }
			= new Dictionary<WordDisplay, FrontendWrongItem>();

		/// <summary>
		/// 显示答案
		/// </summary>
		bool _showAnswer = false;
		public bool showAnswer {
			get { return _showAnswer; }
			set {
				_showAnswer = value;
				if (_showAnswer) setupCorrectAnswer();
				refreshWords();
			}
		} 

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

		/// <summary>
		/// 添加答案
		/// </summary>
		/// <param name="answer"></param>
		public void addAnswer(FrontendWrongItem answer) {
			var word = getWordDisplay(answer.sid, answer.wid);
			var oriAnswer = findAnswer(word);

			if (oriAnswer == null) answers.Add(word, answer);
			else oriAnswer.word = answer.word;
            Debug.Log("修改前：" + word.getItem());
			word.setItem(answer.word);
            Debug.Log("修改后：" + word.getItem());

            refreshRestCount();
			//refreshWords();
		}

		/// <summary>
		/// 撤销答案
		/// </summary>
		/// <param name="answer"></param>
		public void revertAnswer(FrontendWrongItem answer) {
			var word = getWordDisplay(answer.sid, answer.wid);
			var oriAnswer = findAnswer(word);

			if (oriAnswer != null) answers.Remove(word);

			word.revert();

			refreshRestCount();
			//refreshWords();
		}

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
		/// 配置标准答案
		/// </summary>
		void setupCorrectAnswer() {
			foreach(var item in question.wrongItems) {
				var front = item.convertToFrontendWrongItem();
				var word = getWordDisplay(front.sid, front.wid);

				word?.setCorrectWord(front.word);
			}
		}

		/// <summary>
		/// 重置所有答案
		/// </summary>
		public void revertAllAnswers() {
			foreach (var pair in answers)
				pair.Key.revert();

			answers.Clear();
			refreshRestCount();
		}

		/// <summary>
		/// 寻找答案项
		/// </summary>
		public FrontendWrongItem findAnswer(FrontendWrongItem answer) {
			return findAnswer(answer.sid, answer.wid);
		}
		public FrontendWrongItem findAnswer(int sid, int wid) {
			return findAnswer(getWordDisplay(sid, wid));
		}
		public FrontendWrongItem findAnswer(WordDisplay word) {
			if (answers.ContainsKey(word)) return answers[word];
			return null;
		}

		/// <summary>
		/// 某位置是否修改
		/// </summary>
		/// <returns></returns>
		public bool containsAnswer(FrontendWrongItem answer) {
			return containsAnswer(answer.sid, answer.wid);
		}
		public bool containsAnswer(int sid, int wid) {
			var word = getWordDisplay(sid, wid);
			return answers.ContainsKey(word);
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

		/// <summary>
		/// 获取指定坐标的单词显示控件
		/// </summary>
		/// <param name="sid">句子ID（从1开始）</param>
		/// <param name="wid">单词ID（从1开始）</param>
		/// <returns></returns>
		public WordDisplay getWordDisplay(int sid, int wid) {
			if (sid <= 0 || wid <= 0) return null;
			if (sid > subViews.Count) {
				Debug.LogWarning("Article.getWordDisplay sid warnning: " + 
					sid + " for: " + subViews.Count);
				return null;
			}

			var sentence = subViews[sid - 1] as SentenceDisplay;
			var res = sentence?.container?.getWordDisplay(wid);
			if (res == null) 
				Debug.LogWarning("Article.getWordDisplay " +
					"sid, wid warnning: " + sid + ", " + wid);
			return res;
		}

		/// <summary>
		/// 全部取消选择
		/// </summary>
		/// <param name="expect">排除项</param>
		public void deselectAll(WordsContainer expect = null) {
			foreach (var sub in subViews) {
				var display = sub as SentenceDisplay;
				if (display?.container == container) continue;

				display?.container?.deselect();
			}
		}

		/// <summary>
		/// 返回所有错误项
		/// </summary>
		/// <returns></returns>
		public List<FrontendWrongItem> getWrongItems() {
			Debug.Log("Article: getWrongItems");

			var res = new List<FrontendWrongItem>();
			/*
			foreach (var sub in subViews) {
				var display = sub as SentenceDisplay;
				if (display == null) continue;

				var answer = display.getWrongItems();
				res.AddRange(answer);
			}
			*/
			foreach (var pair in answers)
				res.Add(pair.Value);

			return res;
		}

		#endregion

		#region 界面绘制

		/// <summary>
		/// 子视图创建回调
		/// </summary>
		/// <param name="sub"></param>
		/// <param name="index"></param>
		protected override void onSubViewCreated(ItemDisplay<string> sub, int index) {
			base.onSubViewCreated(sub, index);

			var display = sub as SentenceDisplay;
			if (display == null) return;
			display.sid = index + 1;
			display.articleDisplay = this;
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
		/// 刷新所有单词
		/// </summary>
		public void refreshWords() {

			// 对玩家作答项进行刷新
			foreach (var pair in answers)
				pair.Key.requestRefresh();

			// 对正确题目项刷新
			foreach (var item in question.wrongItems) {
				var front = item.convertToFrontendWrongItem();
				var word = getWordDisplay(front.sid, front.wid);

				word?.requestRefresh();
			}

			// 对句子横线进行刷新
			foreach (var sub in subViews) {
				var display = sub as SentenceDisplay;
				display?.refreshLineColor();
			}
		}

		/// <summary>
		/// 刷新剩余次数
		/// </summary>
		void refreshRestCount() {
			count.text = getRestCount().ToString();
		}

		/// <summary>
		/// 刷新
		/// </summary>
		protected override void refresh() {
			base.refresh();
			refreshRestCount();
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
