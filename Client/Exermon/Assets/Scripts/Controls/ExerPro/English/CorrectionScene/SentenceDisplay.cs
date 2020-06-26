
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Core.UI.Utils;

using UI.Common.Controls.ItemDisplays;

using FrontendWrongItem = ExerPro.EnglishModule.Data.
	CorrectionQuestion.FrontendWrongItem;

namespace UI.ExerPro.EnglishPro.CorrectionScene.Controls {

    /// <summary>
    /// 句子
    /// </summary
    public class SentenceDisplay : ItemDisplay<string> {

		/// <summary>
		/// 外部组件设置
		/// </summary>
        public WordsContainer container;
		public Image line;

		/// <summary>
		/// 外部变量控制
		/// </summary>
		public Color normalColor = new Color(142, 123, 95, 255) / 255f;
		public Color correctColor = new Color(72, 127, 74, 255) / 255f;
		public Color wrongColor = new Color(150, 28, 70, 255) / 255f;

		/// <summary>
		/// 句子ID
		/// </summary>
		public int sid { get; set; }

		public ArticleDisplay articleDisplay { get; set; }

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
            base.initializeOnce();
            container = SceneUtils.get<WordsContainer>(gameObject);
        }

		#endregion

		#region 界面绘制

		/// <summary>
		/// 绘制物品
		/// </summary>
		/// <param name="item"></param>
		protected override void drawExactlyItem(string item) {
            base.drawExactlyItem(item);
            container.setItems(generateWords());
		}

		/// <summary>
		/// 绘制线段颜色
		/// </summary>
		public void refreshLineColor() {
			var color = normalColor;
			if (isShowAnswer() && isChanged())
				color = isCorrect() ? correctColor : wrongColor;

			line.color = color;
		}

		/// <summary>
		/// 刷新
		/// </summary>
		protected override void refresh() {
			base.refresh();
			refreshLineColor();
		}

		#endregion

		#region 数据控制
		
		/// <summary>
		/// 是否显示答案
		/// </summary>
		/// <returns></returns>
		public bool isShowAnswer() {
			return articleDisplay.showAnswer;
		}

		/// <summary>
		/// 本行是否正确
		/// </summary>
		/// <returns></returns>
		public bool isCorrect() {
			foreach (var sub in container.getSubViews()) {
				var display = sub as WordDisplay;
				if (display == null) continue;

				if (!display.isCorrect()) return false;
			}
			return true;
		}

		/// <summary>
		/// 本行是否需要修改
		/// </summary>
		/// <returns></returns>
		public bool isChanged() {
			foreach (var sub in container.getSubViews()) {
				var display = sub as WordDisplay;
				if (display == null) continue;

				// 如果有一个单词需要修改
				if (display.correctWord != null) return true;
			}
			return false;
		}

		/// <summary>
		/// 生成单词
		/// </summary>
		string[] generateWords() {
			return item.Trim().Split(' ');
			/*
			string temp = item.Trim();
			List<string> words = temp.Split(' ').ToList();

			//for (int i = 0; i < size; i++) {
			//    items.Insert(2 * i, "  ");
			//}

			string lastWord = words.Last<string>();

			//string end = lastWord.Substring(lastWord.Length - 1);
			//items.RemoveAt(items.ToArray().Length - 1);
			//items.Add(lastWord.Substring(0, lastWord.Length - 1));
			//items.Add("  ");
			//items.Add(end);

			return words;
			*/
		}

		/// <summary>
		/// 返回所有错误项
		/// </summary>
		/// <returns></returns>
		public List<FrontendWrongItem> getWrongItems() {
			Debug.Log(name + ": getWrongItems");

			return container.getWrongItems();
		}

		#endregion
	}
}
