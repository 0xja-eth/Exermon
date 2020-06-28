using UnityEngine;
using UnityEngine.UI;

using Core.Systems;
using Core.UI;

using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;

using UI.ExerPro.EnglishPro.Common.Windows;

using UI.Common.Controls.QuestionDisplay;

namespace UI.ExerPro.EnglishPro.PhraseScene {

	using Controls;

	/// <summary>
	/// 短语场景
	/// </summary>
	public class PhraseScene : BaseScene {

		/// <summary>
		/// 题目数
		/// </summary>
		const int QuestionCount = 10;

		const int PhraseSecond = 30;

		const string RestCountFormat = "当前题目：{0}/{1}";
		const string CorrCountFormat = "正确题目：{0}";

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Text count, corrCnt;
		public QuestionTimer timer;

		public GameObject nextBtn;

		public RewardWindow rewardWindow;

		public PhraseQuestionDisplay questionDisplay;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		PhraseQuestion[] questions;
		string[] options;
		
        int currentIndex = 0, corrCount = 0;
		
		bool answering = false; // 回答中

		/// <summary>
		/// 外部系统设置
		/// </summary>
		EnglishService engSer;

		#region 初始化

		/// <summary>
		/// 初始化外部系统
		/// </summary>
		protected override void initializeSystems() {
            base.initializeSystems();
            engSer = EnglishService.get();
            gameSys = GameSystem.get();
        }

		/// <summary>
		/// 场景名
		/// </summary>
		/// <returns>场景名</returns>
		public override SceneSystem.Scene sceneIndex() {
			return SceneSystem.Scene.EnglishProPhraseScene;
		}
		
		/// <summary>
		/// 开始
		/// </summary>
		protected override void start() {
			base.start();
			engSer.generateQuestions<PhraseQuestion>(
				QuestionCount, onQuestionGenerated);
		}

		#endregion
		
		#region 更新控制

		/// <summary>
		/// 场景更新
		/// </summary>
		protected override void update() {
			base.update();
			updateReward();
			updateTimer();
		}

		/// <summary>
		/// 更新计时
		/// </summary>
		void updateReward() {
			var rewardInfo = engSer.rewardInfo;
			if (rewardInfo != null)
				rewardWindow.startWindow(rewardInfo);
		}

		/// <summary>
		/// 更新计时
		/// </summary>
		void updateTimer() {
			if (answering && timer.isTimeUp())
				questionDisplay.setOption("");
		}

		#endregion

		#region 界面绘制

		/// <summary>
		/// 刷新
		/// </summary>
		void refresh() {
			count.text = string.Format(RestCountFormat,
				currentIndex + 1, questions.Length);
			corrCnt.text = string.Format(CorrCountFormat, corrCount);
		}

		#endregion

		#region 回调控制

		/// <summary>
		/// 题目生成完毕回调
		/// </summary>
		void onQuestionGenerated(PhraseQuestion[] questions) {
			this.questions = questions;
			options = new string[questions.Length];
			currentIndex = -1;

			nextQuestion();
			/*
			PhraseQuestion question = questions[currentQuesIndex++];
			while (question.options().Length <= 0)
				question = questions[currentQuesIndex++];
			optionAreaDisplay.startView(question);
			*/
		}

		/// <summary>
		/// 提交回调
		/// </summary>
		public void onSubmit() {
			engSer.answerPhrase(questions, options, processReward);

			//Debug.Log("答对：" + correctNum + "题,答错：" + wrongNum + "题。");
			//gameSys.requestAlert("答对：" + correctNum + "题,答错：" + wrongNum + "题。",
			//	AlertWindow.Type.Notice);

			//engSer.exitNode(true);
		}

		/// <summary>
		/// 提交成功回调
		/// </summary>
		/// <param name="corrCnt">正确数量</param>
		void processReward(int corrCnt) {
			Debug.Log("Correct Count = " + corrCnt);
			engSer.processReward(questionNumber: corrCnt);
		}

		#endregion

		#region 流程控制

		/// <summary>
		/// 回答
		/// </summary>
		/// <param name="option"></param>
		public void answer(string option) {
			answering = false;
			timer.stopTimer();

			questionDisplay.areaDisplay.actived = false;
			options[currentIndex] = option;

			if (questions[currentIndex].phrase == option)
				corrCount++;

			nextBtn.SetActive(true);
			refresh();
		}

		/// <summary>
		/// 下一道
		/// </summary>
		public void nextQuestion() {
			if (currentIndex >= questions.Length - 1) {
				onSubmit(); return;
			}
			var question = questions[++currentIndex];
			// 如果没有选项，下一题
			if (question.options().Length <= 0)
				nextQuestion();
			else
				onNextQuestion(question);
		}

		/// <summary>
		/// 下一题目回调
		/// </summary>
		/// <param name="question"></param>
		void onNextQuestion(PhraseQuestion question) {
			answering = true;
			timer.startTimer(PhraseSecond);

			questionDisplay.areaDisplay.actived = true;
			questionDisplay.startView(question);

			nextBtn.SetActive(false);
			refresh();
		}

		#endregion

	}
}
