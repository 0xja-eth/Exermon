using System;

using UnityEngine;
using UnityEngine.UI;

using ExerPro.EnglishModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.ListenScene.Controls {

	/// <summary>
	/// 听力小题容器
	/// 利大佬说的Class D
	/// </summary>
	public class ListenSubQuestionContainer : ContainerDisplay<ListeningSubQuestion> {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public ListenQuestionDisplay questionDisplay;

		/// <summary>
		/// 显示答案解析
		/// </summary>
		public bool showAnswer {
			set {
				foreach(var sub in subViews) {
					var display = sub as ListeningSubQuestionDisplay;
					if (display) display.showAnswer = value;
				}
			}
		}

		#region 更新控制

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			updateFinish();
		}

		/// <summary>
		/// 更新完成状态
		/// </summary>
		void updateFinish() {
			questionDisplay.confirmButton?.
				gameObject.SetActive(isFinished());
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 物品变更回调
		/// </summary>
		protected override void onItemsChanged() {
			base.onItemsChanged();
			container.anchoredPosition = new Vector2(0, 0);
			// result = null; showAnswer = false;
		}

		/// <summary>
		/// 是否已完成所有题目
		/// </summary>
		/// <returns></returns>
		public bool isFinished() {
			foreach (var subView in subViews) {
				var subDisplay = subView as ListeningSubQuestionDisplay;
				if (!subDisplay.isSelected()) return false;
			}
			return true;
		}

		#endregion
		
	}
}
