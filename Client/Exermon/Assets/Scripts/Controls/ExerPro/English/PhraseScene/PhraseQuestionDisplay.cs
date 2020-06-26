using Core.UI.Utils;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Core.UI;

using ExerPro.EnglishModule.Data;

using UI.Common.Controls.ItemDisplays;
using UI.PhraseScene.Windows;

namespace UI.ExerPro.EnglishPro.PhraseScene.Controls {

	/// <summary>
	/// 答案显示
	/// </summary>
    public class PhraseQuestionDisplay : ItemDisplay<PhraseQuestion>, IDropHandler {

		/// <summary>
		/// 字符串常量定义
		/// </summary>
		const string Blank = " ...";
		const string RotateAnimation = "Rotate";
		const string MoveUpAnimation = "MoveUp";
		const string MoveDownAnimation = "MoveDown";

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Text chineseDisplay;
		public Text phraseDisplay;

		public Image quesImg;
		public Color normalColor = new Color(1, 1, 0);
		public Color correctColor = new Color(0, 1, 0);
		public Color wrongColor = new Color(1, 0, 0);

		public Animation animation;

		public OptionAreaDisplay areaDisplay;
		public ConfirmWindow window;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		string option = null;

		#region 数据控制

		/// <summary>
		/// 物品改变回调
		/// </summary>
		protected override void onItemChanged() {
			base.onItemChanged();
			clearOption();
			areaDisplay.setItems(item.options());
		}

		/// <summary>
		/// 是否正确
		/// </summary>
		/// <returns></returns>
		public bool isCorrect() {
			return option == item?.phrase;
		}

		/// <summary>
		/// 清除选项
		/// </summary>
		public void clearOption() {
			option = null; requestRefresh();
		}

		#endregion

		#region 界面绘制

		/// <summary>
		/// 绘制物品
		/// </summary>
		/// <param name="item"></param>
		protected override void drawExactlyItem(PhraseQuestion item) {
			base.drawExactlyItem(item);
			if (option == null)
				drawNormalQuestion(item);
			else
				drawAnswerQuestion(item);
		}

		/// <summary>
		/// 绘制普通题目
		/// </summary>
		/// <param name="question"></param>
		void drawNormalQuestion(PhraseQuestion question) {
			chineseDisplay.text = item.chinese;
			phraseDisplay.text = item.word + Blank;
			quesImg.color = normalColor;

			animation.Stop(MoveUpAnimation);
			animation.Play(MoveDownAnimation);
			animation.Play(RotateAnimation);
		}

		/// <summary>
		/// 绘制回答题目
		/// </summary>
		/// <param name="question"></param>
		void drawAnswerQuestion(PhraseQuestion question) {
			chineseDisplay.text = item.chinese;
			phraseDisplay.text = item.word + " " + option;
			quesImg.color = isCorrect() ? correctColor : wrongColor;

			animation.Stop(MoveDownAnimation);
			animation.Stop(RotateAnimation);
			animation.Play(MoveUpAnimation);
		}

		/// <summary>
		/// 绘制空物品
		/// </summary>
		protected override void drawEmptyItem() {
			base.drawEmptyItem();
			chineseDisplay.text = phraseDisplay.text = "";
		}

		#endregion

		#region 事件控制

		/// <summary>
		/// 拖拽物品放下回
		/// </summary>
		/// <param name="data">事件数据</param>
		public void OnDrop(PointerEventData data) {
            processItemDrop(getDraggingItemDisplay(data));
        }

		/// <summary>
		/// 获取拖拽中的物品显示项
		/// </summary>
		/// <param name="data">事件数据</param>
		/// <returns>物品显示项</returns>
		OptionDisplay getDraggingItemDisplay(PointerEventData data) {
            var obj = data.pointerDrag;
            if (obj == null) return null;
            return SceneUtils.get<OptionDisplay>(obj);
        }

        /// <summary>
        /// 处理物品放下
        /// </summary>
        protected void processItemDrop(OptionDisplay display) {
            if (display == null) return;

			option = display.getItem();

			window.startWindow(item.word, option, item.phrase);
			requestRefresh();
        }

        #endregion


    }
}
