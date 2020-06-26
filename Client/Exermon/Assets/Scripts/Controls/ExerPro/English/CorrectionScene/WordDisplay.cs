using UnityEngine;
using UnityEngine.UI;

using Core.UI.Utils;

using UI.Common.Controls.ItemDisplays;

using FrontendWrongItem = ExerPro.EnglishModule.Data.
	CorrectionQuestion.FrontendWrongItem;

namespace UI.ExerPro.EnglishPro.CorrectionScene.Controls {

    /// <summary>
    /// 单词
    /// </summary
    public class WordDisplay : SelectableItemDisplay<string> {

		/// <summary>
		/// 外部组件设置
		/// </summary>
        public GameObject deleteFlag, changeFlag, addPrevFlag, addNextFlag;
        public Text text;

		/// <summary>
		/// 外部变量控制
		/// </summary>
		public Color correctColor = new Color(72, 127, 74, 255) / 255f;
		public Color wrongColor = new Color(150, 28, 70, 255) / 255f;

		/// <summary>
		/// 内部组件设置
		/// </summary>
		Text changeText, addPrevText, addNextText;

        /// <summary>
        /// 状态枚举
        /// </summary>
        public enum State {
            Original, Modefied,
			AddedNext, AddedPrev, // 后一个增加，前一个增加
			Deleted
        }

		/// <summary>
		/// 内部变量定义
		/// </summary>
		public string originalWord { get; set; } // 原始文章
		public string correctWord { get; set; } = null; // 正确答案
		
		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			changeText = SceneUtils.find<Text>(changeFlag, "Text");
			addPrevText = SceneUtils.find<Text>(addPrevFlag, "Text");
			addNextText = SceneUtils.find<Text>(addNextFlag, "Text");
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 获取wid
		/// </summary>
		/// <returns></returns>
		public int getWid() {
			return index + 1;
		}

		/// <summary>
		/// 获取sid
		/// </summary>
		/// <returns></returns>
		public int getSid() {
			return getContainer().sentenceDisplay.sid;
		}

		/// <summary>
		/// 获取容器
		/// </summary>
		/// <returns></returns>
		public new WordsContainer getContainer() {
			return container as WordsContainer;
		}

		/// <summary>
		/// 是否有修改
		/// </summary>
		/// <returns></returns>
		public bool isChanged() {
			return item != originalWord;
		}
		
		/// <summary>
		/// 计算当前修改状态
		/// </summary>
		/// <returns></returns>
		State calcState(string word) {
			if (word == "") return State.Deleted; 
			if (word == originalWord) return State.Original;

			var words = word.Split(' ');

			if (words.Length == 2) {
				if (words[0] == originalWord) return State.AddedNext;
				if (words[1] == originalWord) return State.AddedPrev;
			}

			return State.Modefied;
		}

		/// <summary>
		/// 获取增加的单词
		/// </summary>
		string getChangedWord(string word = null) {
			if (word == null) word = item;

			var words = word.Split(' ');
			if (words.Length <= 0) return "";

			if (words.Length == 2 && 
				words[0] == originalWord) return words[1];

			return words[0];
		}

		/// <summary>
		/// 复原
		/// </summary>
		public void revert() {
			setItem(originalWord);
		}

		/// <summary>
		/// 配置正确单词
		/// </summary>
		public void setCorrectWord(string word) {
			correctWord = word;
			requestRefresh();
		}

		/// <summary>
		/// 清除正确单词
		/// </summary>
		public void clearCorrectWord() {
			setCorrectWord(null);
		}

		/// <summary>
		/// 是否正确
		/// </summary>
		/// <returns></returns>
		public bool isCorrect(string word = null) {
			if (word == null) word = item;
			if (correctWord == null) // 若不需要改
				return word == originalWord; // 是否保持原样
			else // 如果需要改
				return word == correctWord; // 是否改对
		}

		/// <summary>
		/// 是否显示答案
		/// </summary>
		/// <returns></returns>
		public bool isShowAnswer() {
			return getContainer().sentenceDisplay.isShowAnswer();
		}

		/// <summary>
		/// 生成错误项
		/// </summary>
		/// <param name="display"></param>
		/// <returns></returns>
		public FrontendWrongItem generateWrongItem() {
			if (isChanged()) {
				Debug.Log("generateWrongItem: " +
					getSid() + ", " + getWid() + ": " +
					originalWord + " -> " + item);
				return new FrontendWrongItem(getSid(), getWid(), item);
			}
			return null;
		}

		#endregion

		#region 界面绘制

		/// <summary>
		/// 绘制物品
		/// </summary>
		/// <param name="item"></param>
		protected override void drawExactlyItem(string item) {
            base.drawExactlyItem(item);
			text.text = originalWord;
			drawFlag(item); drawStateColor(item);
		}

		/// <summary>
		/// 绘制标志
		/// </summary>
		/// <param name="word"></param>
		void drawFlag(string word) {
			// 显示答案
			if (isShowAnswer()) word = correctWord ?? originalWord;

			var state = calcState(word);

			clearFlags();
			switch (state) {
				case State.Modefied:
					changeText.text = getChangedWord(word);
					changeFlag.SetActive(true);
					//text.color = new Color(1.0f, 0.0f, 0.0f);
					break;
				case State.AddedNext:
					addNextText.text = getChangedWord(word);
					addNextFlag.SetActive(true);
					break;
				case State.AddedPrev:
					addPrevText.text = getChangedWord(word);
					addPrevFlag.SetActive(true);
					//text.color = new Color(1.0f, 0.0f, 0.0f);
					break;
				case State.Deleted:
					//text.color = new Color(1.0f, 0.0f, 0.0f);
					deleteFlag.SetActive(true);
					break;
			}
		}

		/// <summary>
		/// 绘制状态颜色
		/// </summary>
		/// <param name="word"></param>
		void drawStateColor(string word) {
			var color = normalColor;

			if (isShowAnswer()) // 需要显示答案
				if (correctWord == null) {// 若不需要改
					if (word != originalWord) // 但是却改了
						color = wrongColor;
				} else {// 如果需要改
					if (word == correctWord) // 改对了
						color = correctColor;
					else color = wrongColor;
				}

			text.color = color;
		}

		/// <summary>
		/// 清除所有标记
		/// </summary>
		void clearFlags() {
			deleteFlag.SetActive(false);
			changeFlag.SetActive(false);
			addPrevFlag.SetActive(false);
			addNextFlag.SetActive(false);
		}

		/// <summary>
		/// 绘制空物品
		/// </summary>
		/// <returns></returns>
		protected override void drawEmptyItem() {
			base.drawEmptyItem();

			text.text = "";
			clearFlags();
		}

		#endregion
	}


}
