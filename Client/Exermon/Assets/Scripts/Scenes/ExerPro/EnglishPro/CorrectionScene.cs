
using System.Collections.Generic;

using Core.UI;
using Core.Systems;

using UnityEngine;
using UnityEngine.UI;

using Core.UI.Utils;

using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;

using FrontendWrongItem = ExerPro.EnglishModule.Data.
	CorrectionQuestion.FrontendWrongItem;

using UI.Common.Controls.ItemDisplays;

using UI.ExerPro.EnglishPro.CorrectionScene.Controls;
using UI.ExerPro.EnglishPro.Common.Windows;

namespace UI.ExerPro.EnglishPro.CorrectionScene {

	using Windows;

    /// <summary>
    /// 场景
    /// </summary>
    public class CorrectionScene : BaseScene {

		/// <summary>
		/// 文本常量定义
		/// </summary>
		const string UncorrectableAlertText = "已超出改错次数上限！请清除一些修改";

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public ArticleDisplay articleDisplay;

		public GameObject revertBtn, confirmBtn, submitBtn, answerBtn;

		public RewardWindow rewardWindow;
		public CorrectionWindow correctionWindow;
		
		/// <summary>
		/// 答题已结束
		/// </summary>
		bool terminated = false;

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
        }

        /// <summary>
        /// 场景名
        /// </summary>
        /// <returns>场景名</returns>
        public override SceneSystem.Scene sceneIndex() {
            return SceneSystem.Scene.EnglishProCorrectionScene;
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected override void start() {
			base.start();
			engSer.generateQuestion<CorrectionQuestion>(articleDisplay.startView);
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

		#region 数据控制

		/// <summary>
		/// 增加答案
		/// </summary>
		/// <returns>返回是否成功</returns>
		public bool addAnswer(FrontendWrongItem answer) {
			var res = articleDisplay.isCorrectEnable();
			if (res) articleDisplay.addAnswer(answer);
			else gameSys.requestAlert(UncorrectableAlertText);
			return res;
		}

		/// <summary>
		/// 撤销答案
		/// </summary>
		/// <returns>返回是否成功</returns>
		public void revertAnswer(FrontendWrongItem answer) {
			articleDisplay.revertAnswer(answer);
		}

		#endregion

		#region 回调控制

		/// <summary>
		/// 单词选择回调
		/// </summary>
		/// <param name="container">句子容器</param>
		/// <param name="word">单词</param>
		public void onWordSelected(WordsContainer container, WordDisplay word) {

			// 清除其他句子选择
			articleDisplay.deselectAll(container);

			if (!terminated) correctionWindow.startWindow(container, word);
        }

		/// <summary>
		/// 重置
		/// </summary>
		public void onRevert() {
			articleDisplay.revertAllAnswers();
		}

		/// <summary>
		/// 切换答案显示
		/// </summary>
		public void onAnswer() {
			articleDisplay.showAnswer = !articleDisplay.showAnswer;
		}

		/// <summary>
		/// 确认回调
		/// </summary>
		public void onConfirm() {
			terminated = true;
			correctionWindow.cancel();
			articleDisplay.showAnswer = true;

			revertBtn.SetActive(false);
			confirmBtn.SetActive(false);
			answerBtn.SetActive(true);
			submitBtn.SetActive(true);
		}

		/// <summary>
		/// 提交回调
		/// </summary>
		public void onSubmit() {
			var answers = articleDisplay.getWrongItems();
			var question = articleDisplay.getItem();

			engSer.answerCorrection(question, answers, processReward);
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
	}


}

