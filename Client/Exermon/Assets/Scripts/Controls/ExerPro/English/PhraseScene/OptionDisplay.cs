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
		public float wdithOffset = 32;

		public Color[] randomColors;

		public Vector2 scaleRange = new Vector2(0.75f, 1);

		/// <summary>
		/// 内部变量定义
		/// </summary>
		bool _isCorrect = false;
		public bool isCorrect {
			get { return _isCorrect; }
			set { _isCorrect = value; requestRefresh(); }
		}

		bool randomSkin = false;

		#region 开启

		/// <summary>
		/// 开启
		/// </summary>
		public override void startView() {
			base.startView();
			randomSkin = false;
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 内容改变回调
		/// </summary>
		protected override void onItemChanged() {
			base.onItemChanged();
			isCorrect = false;
		}

		/// <summary>
		/// 获取控件尺寸
		/// </summary>
		/// <returns></returns>
		public Vector2 getSize() {
			var rt = transform as RectTransform;
			var width = item.Length * option.fontSize / 2;

			return new Vector2(width + wdithOffset * 2, rt.sizeDelta.y);
		}

		/// <summary>
		/// 获取控件尺寸
		/// </summary>
		/// <returns></returns>
		public void getMinMax(out Vector2 min, out Vector2 max) {
			var rt = transform as RectTransform;
			var width = item.Length * option.fontSize / 2;

			var size = new Vector2(width + wdithOffset * 2, rt.sizeDelta.y);
			var pos = rt.anchoredPosition;

			min = pos - size / 2; max = pos + size / 2;
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
			correctFlag.SetActive(isCorrect);

			if (!randomSkin)
				drawRandomInterface();
		}

		/// <summary>
		/// 绘制随机外表
		/// </summary>
		void drawRandomInterface() {
			randomSkin = true;
			var rand = Random.Range(0, 2) * 2 - 1;
			var scale = image.localScale;
			scale.x = rand;
			image.localScale = scale;

			var cnt = randomColors.Length;
			if (cnt > 0) {
				var cid = Random.Range(0, cnt);
				skin.color = randomColors[cid];
			}

			var rate = Random.Range(scaleRange[0], scaleRange[1]);
			transform.localScale = new Vector3(rate, rate);
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
