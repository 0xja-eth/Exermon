
using UnityEngine;
using UnityEngine.UI;

using Core.UI;
using Core.UI.Utils;

using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;

using UI.Common.Controls.QuestionDisplay;

namespace UI.ExerPro.EnglishPro.BattleScene.Windows {

	using Controls.Word;

	/// <summary>
	/// 单词窗口
	/// </summary>
	public class WordWindow : BaseWindow {

		/// <summary>
		/// 文本常量定义
		/// </summary>
		const string CountFormat = "{0}/{1}";
		const string DrawCountFormat = "抽牌次数：{0}/{1}";

		const int WordSecond = 10;

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Text count, drawCnt;

		public WordQuestionDisplay wordQuestionDisplay;
		public WordChoiceContainer choiceContainer;

		public QuestionTimer timer;

		public GameObject bonus;

		public GameObject next;

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
		bool pushingAnswer = false; // 提交作答中

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
			base.terminateWindow();
			wordQuestionDisplay.terminateView();
			//if (terminated) {

			//}
		}
		//public void terminateWindow(bool force) {
		//	if (force) terminated = true;
		//	terminateWindow();
		//}

		#endregion

		#region 更新

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			updateTimer();
		}

		/// <summary>
		/// 更新计时
		/// </summary>
		void updateTimer() {
			if (!pushingAnswer && timer.isTimeUp()) answer();
		}

		#endregion

		#region 画面绘制

		/// <summary>
		/// 刷新
		/// </summary>
		protected override void refresh() {
			base.refresh();
			resetQuestion();
			drawWordQuestion();
			drawCount();

			scene.refreshStatus();
		}

		/// <summary>
		/// 重置题目
		/// </summary>
		void resetQuestion() {
			next.SetActive(false);
			choiceContainer.selectable = true;
			wordQuestionDisplay.showAnswer = false;
			timer.startTimer(WordSecond);
		}

		/// <summary>
		/// 绘制单词题目
		/// </summary>
		void drawWordQuestion() {
			var word = battleSer.currentWord();
			wordQuestionDisplay.setItem(word);
			wordQuestionDisplay.startView();
		}

		/// <summary>
		/// 绘制数目
		/// </summary>
		void drawCount() {
			var bonus = battleSer.isBonus();
			var index = battleSer.wordIndex();
			var max = battleSer.wordCount();
			var drawCnt = battleSer.drawCount();
			var maxDraw = battleSer.maxDrawCount();

			this.bonus.SetActive(bonus);
			count.text = string.Format(CountFormat, index, max);
			this.drawCnt.text = string.Format(DrawCountFormat, drawCnt, maxDraw);
		}

		#endregion

		#region 流程控制

		/// <summary>
		/// 回答单词
		/// </summary>
		public void answer(string chinese = "") {
			pushingAnswer = true;
			choiceContainer.selectable = false;
			battleSer.answer(chinese, onAnswerSuccess,
				() => pushingAnswer = false);
		}

		/// <summary>
		/// 回答是否正确
		/// </summary>
		/// <param name="correct"></param>
		void onAnswerSuccess(bool correct) {
			timer.stopTimer();
			wordQuestionDisplay.showAnswer = true;

			if (correct) onAnswerCorrect();
			else onAnswerWrong();

			next.SetActive(true);

			terminated = battleSer.isStateChanged();
			pushingAnswer = false;
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

		/// <summary>
		/// 下一题
		/// </summary>
		public void nextWord() {
			if (terminated) scene.draw();
			else requestRefresh();
		}

		#endregion

	}
}
