using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Core.UI.Utils;
using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.CorrectionScene.Controls {

    /// <summary>
    /// 句子
    /// </summary
    public class SentenceDisplay : ItemDisplay<string> {

		/// <summary>
		/// 外部组件设置
		/// </summary>
        public WordsContainer container;

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

		#endregion

		#region 数据控制

		/// <summary>
		/// 生成单词
		/// </summary>
		List<string> generateWords() {
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
		}

		#endregion
	}
}
