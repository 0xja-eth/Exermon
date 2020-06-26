using UnityEngine;

using Core.Systems;
using Core.UI;

using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;

using UI.ExerPro.EnglishPro.PhraseScene.Controls;
using UI.ExerPro.EnglishPro.Common.Windows;

namespace UI.PhraseScene {

	/// <summary>
	/// 短语场景
	/// </summary>
    public class PhraseScene : BaseScene {

		/// <summary>
		/// 题目数
		/// </summary>
		const int QuestionCount = 10;

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public RewardWindow rewardWindow;
		public PhraseQuestionDisplay questionDisplay;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		PhraseQuestion[] questions;
		string[] options;

		int correctNum, wrongNum = 0;
        int currentQuestionIndex = 0;

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
        /// 初始化其他
        /// </summary>
        protected override void initializeOthers() {
            base.initializeOthers();
            correctNum = wrongNum = 0;
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
			var rewardInfo = engSer.rewardInfo;
			if (rewardInfo != null)
				rewardWindow.startWindow(rewardInfo);
		}

		#endregion

		#region 回调控制

		/// <summary>
		/// 题目生成完毕回调
		/// </summary>
		void onQuestionGenerated(PhraseQuestion[] questions) {
			this.questions = questions;
			options = new string[questions.Length];
			currentQuestionIndex = -1;
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
		/// 下一道
		/// </summary>
		public void nextQuestion() {
			if (currentQuestionIndex >= questions.Length - 1) {
				onSubmit(); return;
			}
			var question = questions[++currentQuestionIndex];
			// 如果没有选项，下一题
			if (question.options().Length <= 0)
				nextQuestion();
			else
				questionDisplay.startView(question);
		}

		/// <summary>
		/// 记录正确和错误数量
		/// </summary>
		public void record(string option) {
			options[currentQuestionIndex] = option;
		}

		#endregion

    }
}
