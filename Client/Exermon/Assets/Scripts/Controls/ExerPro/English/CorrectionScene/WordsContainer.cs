using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.UI;

using Core.UI.Utils;

using UI.Common.Controls.ItemDisplays;

using FrontendWrongItem = ExerPro.EnglishModule.Data.
	CorrectionQuestion.FrontendWrongItem;

namespace UI.ExerPro.EnglishPro.CorrectionScene.Controls {

	/// <summary>
	/// 句子容器
	/// </summary>
    public class WordsContainer : SelectableContainerDisplay<string> {

		/// <summary>
		/// 常量定义
		/// </summary>
		const string ends = "!?.,\"";
		static readonly Regex regex = new Regex("[a-zA-Z|’]{2,}");

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public SentenceDisplay sentenceDisplay;

		/// <summary>
		/// 场景引用
		/// </summary>
		CorrectionScene scene;

        #region 初始化
        
		/// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
			scene = SceneUtils.getCurrentScene<CorrectionScene>();
        }

		#endregion

		#region 数据控制

		/// <summary>
		/// 获取单词列表
		/// </summary>
		/// <returns></returns>
		public List<string> getWords() {
			return items.ToList();
		}

		/// <summary>
		/// 获取指定坐标的单词显示控件
		/// </summary>
		/// <param name="wid">单词ID（从1开始）</param>
		/// <returns></returns>
		public WordDisplay getWordDisplay(int wid) {
			if (wid <= 0) return null;
			if (wid > subViews.Count) {
				Debug.LogWarning("Sentence.getWordDisplay wid warnning: " +
					wid + " for: " + subViews.Count);
				return null;
			}
			return subViews[wid - 1] as WordDisplay;
		}

		/// <summary>
		/// 返回所有错误项
		/// </summary>
		/// <returns></returns>
		public List<FrontendWrongItem> getWrongItems() {
			var res = new List<FrontendWrongItem>();

			foreach(var sub in subViews) {
				var display = sub as WordDisplay;
				if (display == null) continue;

				var answer = display.generateWrongItem();
				if (answer != null) res.Add(answer);
			}

			return res;
		}

		#endregion

		#region 回调控制

		/// <summary>
		/// 点击回调
		/// </summary>
		/// <param name="index"></param>
		public override void onClick(int index) {
			base.onClick(index);
			scene.onWordSelected(this, subViews[index] as WordDisplay);
		}

		#endregion

		#region 画面控制

		/// <summary>
		/// 子节点创建回调
		/// </summary>
		/// <param name="sub"></param>
		/// <param name="index"></param>
		protected override void onSubViewCreated(SelectableItemDisplay<string> sub, int index) {
			base.onSubViewCreated(sub, index);

			//SceneUtils.text(sub.gameObject).text = items[index];

			var display = sub as WordDisplay;
            if (display) 
                display.originalWord = items[index];
		}

		#endregion

    }
}
