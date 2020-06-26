using UnityEngine;
using UnityEngine.UI;

using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.PhraseScene.Controls {

	/// <summary>
	/// 选项显示控件
	/// </summary>
    public class OptionDisplay : DraggableItemDisplay<string> {

		/// <summary>
		/// 外部组件设置
		/// </summary>
        public Text option;
        public Image skin;

		public GameObject correctFlag;

		public RectTransform image;

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public Color[] randomColors;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		bool _isCorrect = false;
		public bool isCorrect {
			get{ return _isCorrect; }
			set{ _isCorrect = value; requestRefresh(); }
		}

		#region 关闭
		/*
		/// <summary>
		/// 关闭窗口
		/// </summary>
		public override void terminateView() {
			base.terminateView();
			requestDestroy = true;
		}
		*/
		#endregion

		#region 数据控制

		/// <summary>
		/// 内容改变回调
		/// </summary>
		protected override void onItemChanged() {
			base.onItemChanged();
			isCorrect = false;
		}

		#endregion

		#region 界面绘制

		/// <summary>
		/// 绘制物品
		/// </summary>
		/// <param name="item"></param>
		protected override void drawExactlyItem(string item) {
            base.drawExactlyItem(item);
            option.text = item;
			drawRandomInterface();
		}

		/// <summary>
		/// 绘制随机外表
		/// </summary>
		void drawRandomInterface() {
			var rand = Random.Range(0, 2) * 2 - 1;
			var scale = image.localScale;
			scale.x = rand;
			image.localScale = scale;

			var cnt = randomColors.Length;
			if (cnt > 0) {
				var cid = Random.Range(0, cnt);
				skin.color = randomColors[cid];
			}

			correctFlag.SetActive(isCorrect);
		}

		/// <summary>
		/// 绘制空物品
		/// </summary>
		protected override void drawEmptyItem() {
			base.drawEmptyItem();
			option.text = "";
		}

		#endregion
	}
}
