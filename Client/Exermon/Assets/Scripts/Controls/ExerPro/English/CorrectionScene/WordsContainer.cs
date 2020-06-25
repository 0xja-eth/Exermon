using Core.UI.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UI.Common.Controls.ItemDisplays;
using UnityEngine.UI;

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
		public CorrectionScene scene;

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
			if (display) display.originalWord = items[index];
		}

		#endregion

    }
}
