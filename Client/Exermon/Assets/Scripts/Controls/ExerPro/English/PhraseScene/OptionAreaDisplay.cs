using System;
using Core.UI.Utils;
using ExerPro.EnglishModule.Data;
using UI.Common.Controls.ItemDisplays;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ExerPro.EnglishPro.PhraseScene.Controls {

	/// <summary>
	/// 选项区域显示
	/// </summary>
    public class OptionAreaDisplay : SelectableContainerDisplay<string> {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public RectTransform quesImage;
		public RectTransform draggingParent;

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public Vector2 optionsSpacing = new Vector2(12, 12);
		
		#region 数据控制

		/// <summary>
		/// 获取指定内容的选项控件
		/// </summary>
		/// <param name="option"></param>
		/// <returns></returns>
		public OptionDisplay getOption(string option) {
			return subViews.Find(sub => sub.getItem() == option) as OptionDisplay; 
		}

		#endregion

		#region 界面控制

		/// <summary>
		/// 生成子视图
		/// </summary>
		protected override void createSubViews() {
			base.createSubViews();
			randomOptionPositions();
		}

		/// <summary>
		/// 子节点创建回调
		/// </summary>
		/// <param name="sub"></param>
		/// <param name="index"></param>
		protected override void onSubViewCreated(SelectableItemDisplay<string> sub, int index) {
			var display = sub as OptionDisplay;
			if (display) display.draggingParent = draggingParent;

			base.onSubViewCreated(sub, index);
		}

		/// <summary>
		/// 随机位置生成
		/// </summary>
		void randomOptionPositions() {
			foreach(var sub in subViews) {
				var rt = sub.transform as RectTransform;
				rt.anchoredPosition = generatePosition(rt);

				int cnt = 0;
				while (isRectTransformOverlap(rt, sub) && cnt++ <= 100) 
					rt.anchoredPosition = generatePosition(rt);
			}
		}

		/// <summary>
		/// 生成位置
		/// </summary>
		/// <param name="sub"></param>
		/// <returns></returns>
		Vector2 generatePosition(RectTransform rt) {
			var size = container.rect.size / 2;
			var imgSize = quesImage.rect.size / 2;
			var rtSize = rt.rect.size / 2;
			var oriSize = size;

			size -= rtSize; imgSize += rtSize;

			var x = UnityEngine.Random.Range(-size.x, size.x);
            var y = UnityEngine.Random.Range(-size.y, size.y);

			while(-imgSize.x <= x && x <= imgSize.x &&
				y + imgSize.y >= oriSize.y) {
				x = UnityEngine.Random.Range(-size.x, size.x);
				y = UnityEngine.Random.Range(-size.y, size.y);
			}

			return new Vector2(x, y);
        }

		/// <summary>
		/// 是否重叠
		/// </summary>
		/// <param name="rect1"></param>
		/// <param name="sub"></param>
		/// <returns></returns>
        public bool isRectTransformOverlap(RectTransform rect1, 
			SelectableItemDisplay<string> sub) {

			var min = rect1.anchoredPosition - rect1.rect.size / 2;
			var max = rect1.anchoredPosition + rect1.rect.size / 2;

			foreach (var subView in subViews) {
                if (subView == sub) continue;

                RectTransform rect2 = subView.transform as RectTransform;

				if (rect2 == null) continue;

				Debug.Log("subView: " + subView.name + ".rect2.rect: " + rect2.rect);

				var min2 = rect2.anchoredPosition - rect2.rect.size / 2;
				var max2 = rect2.anchoredPosition + rect2.rect.size / 2;

				min2 -= optionsSpacing; max2 += optionsSpacing;

				if (min.x < max2.x && min2.x < max.x && 
					min.y < max2.y && min2.y < max.y)
                    return true;
            }

            return false;
        }
		
		#endregion
	}
}
