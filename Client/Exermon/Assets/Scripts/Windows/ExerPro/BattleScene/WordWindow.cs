
using UnityEngine.UI;

using Core.UI;
using Core.UI.Utils;

using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;

namespace UI.ExerPro.EnglishPro.BattleScene.Windows {

	using Controls.Word;

	/// <summary>
	/// 单词窗口
	/// </summary>
	public class WordWindow : BaseWindow {

		/// <summary>
		/// 文本常量定义
		/// </summary>
		const string NormalCountFormat = "单词题（{0}/{1}）";
		const string BonusCountFormat = "Bonuse（{0}/{1}）";

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Text count;

		public WordQuestionDisplay wordQuestionDisplay;

		/// <summary>
		/// 场景组件引用
		/// </summary>
		BattleScene scene;

		/// <summary>
		/// 外部系统设置
		/// </summary>
		BattleService battleSer;
		EnglishService engSer;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		bool terminated = false; // 作答完毕

		#region 初始化

		/// <summary>
		/// 初始化场景
		/// </summary>
		protected override void initializeScene() {
			base.initializeScene();
			scene = SceneUtils.getCurrentScene<BattleScene>();
		}

		/// <summary>
		/// 初始化外部系统
		/// </summary>
		protected override void initializeSystems() {
			base.initializeSystems();
			battleSer = BattleService.get();
			engSer = EnglishService.get();
		}

		#endregion

		#region 启动/关闭

		/// <summary>
		/// 打开窗口
		/// </summary>
		public override void startWindow() {
			terminated = false;
			base.startWindow();
		}

		/// <summary>
		/// 关闭窗口
		/// </summary>
		public override void terminateWindow() {
			if (terminated) base.terminateWindow();
		}
		public void terminateWindow(bool force) {
			if (force) terminated = true;
			terminateWindow();
		}

		#endregion

		#region 画面绘制

		/// <summary>
		/// 刷新
		/// </summary>
		protected override void refresh() {
			base.refresh();
			drawWordQuestion();
			drawCount();
		}

		/// <summary>
		/// 绘制单词题目
		/// </summary>
		void drawWordQuestion() {
			var word = battleSer.currentWord();
			wordQuestionDisplay.setItem(word);
		}

		/// <summary>
		/// 绘制数目
		/// </summary>
		void drawCount() {
			var bonus = battleSer.isBonus();
			var format = bonus ? BonusCountFormat : NormalCountFormat;
			var index = battleSer.wordIndex();
			var max = battleSer.wordCount();

			count.text = string.Format(format, index, max);
		}

		#endregion

		#region 流程控制

		/// <summary>
		/// 回答单词
		/// </summary>
		public void answer(string chinese) {
			battleSer.answer(chinese, onAnswerSuccess);
		}

		/// <summary>
		/// 回答是否正确
		/// </summary>
		/// <param name="correct"></param>
		void onAnswerSuccess(bool correct) {
			wordQuestionDisplay.showAnswer = true;
			if (correct) onAnswerCorrect();
			else onAnswerWrong();

			terminated = battleSer.isStateChanged();
			requestRefresh();
		}

		/// <summary>
		/// 回答正确回调
		/// </summary>
		void onAnswerCorrect() {

		}

		/// <summary>
		/// 回答错误回调
		/// </summary>
		void onAnswerWrong() {

		}

		#endregion

	}
}
