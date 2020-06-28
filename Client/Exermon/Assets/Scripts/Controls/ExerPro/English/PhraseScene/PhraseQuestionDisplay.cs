using Core.UI.Utils;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Core.Data.Loaders;

using Core.UI;

using ExerPro.EnglishModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.PhraseScene.Controls {

	using Windows;

	/// <summary>
	/// 答案显示
	/// </summary>
	public class PhraseQuestionDisplay : ItemDisplay<PhraseQuestion>, IDropHandler {

		/// <summary>
		/// 字符串常量定义
		/// </summary>
		const string Blank = "";
		const string RotateAnimation = "Rotate";
		const string MoveUpAnimation = "MoveUp";
		const string MoveDownAnimation = "MoveDown";

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Text chineseDisplay;
		public Text phraseDisplay;
		public Text optionDisplay;

		public GameObject optionObject;

		public Image resultEffect;

		public Animation animation;

		public OptionAreaDisplay areaDisplay;

		/// <summary>
		/// 场景引用
		/// </summary>
		PhraseScene scene;

		/// <summary>
		/// 外部变量定义
		/// </summary>
		public Texture2D correct, wrong;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		string option = null;

		Sprite correctSprite, wrongSprite;

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			scene = SceneUtils.getCurrentScene<PhraseScene>();

			correctSprite = AssetLoader.generateSprite(correct);
			wrongSprite = AssetLoader.generateSprite(wrong);
		}

		#endregion

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
		/// 设置选项
		/// </summary>
		/// <param name="option"></param>
		public void setOption(string option) {
			scene.answer(this.option = option);
			requestRefresh();
		}

		/// <summary>
		/// 清除选项
		/// </summary>
		public void clearOption() {
			option = null;
			requestRefresh();
		}

		#endregion

		#region 界面绘制

		/// <summary>
		/// 绘制物品
		/// </summary>
		/// <param name="item"></param>
		protected override void drawExactlyItem(PhraseQuestion item) {
			base.drawExactlyItem(item);

			drawOption();

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
			phraseDisplay.text = item.word;

			var corr = isCorrect();

			resultEffect.overrideSprite =
				corr ? correctSprite : wrongSprite;

			hideCurrentOption();
			if (!corr) drawCorrectOption();

			animation.Stop(MoveDownAnimation);
			animation.Stop(RotateAnimation);
			animation.Play(MoveUpAnimation);
		}

		/// <summary>
		/// 隐藏当前选项
		/// </summary>
		void hideCurrentOption() {
			var option = areaDisplay.getOption(this.option);
			option?.terminateView();
		}

		/// <summary>
		/// 绘制正确选项
		/// </summary>
		void drawCorrectOption() {
			var option = areaDisplay.getOption(item.phrase);
			if (option) option.isCorrect = true;
		}

		/// <summary>
		/// 绘制所选项
		/// </summary>
		void drawOption() {
			optionObject.SetActive(option != null && option != "");
			optionDisplay.text = option ?? "";
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
			setOption(display.getItem());
        }

        #endregion


    }
}
