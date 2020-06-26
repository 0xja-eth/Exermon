
using Core.UI;
using Core.Systems;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Core.UI.Utils;

using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;

using FrontendWrongItem = ExerPro.EnglishModule.Data.
	CorrectionQuestion.FrontendWrongItem;

using UI.ExerPro.EnglishPro.CorrectionScene.Controls;
using UI.CorrectionScene.Windows;
using UI.Common.Controls.ItemDisplays;
using UI.ExerPro.EnglishPro.Common.Windows;

namespace UI.ExerPro.EnglishPro.CorrectionScene {

    /// <summary>
    /// 场景
    /// </summary>
    public class CorrectionScene : BaseScene {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public ArticleDisplay articleDisplay;

		public Text changedBeforeValue;

		public RewardWindow rewardWindow;
		public CorrectionWindow correctionWindow;

        List<int> doneIds = new List<int>();

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
			engSer.generateQuestions<CorrectionQuestion>(1, (res) => {
				articleDisplay.startView(res[0]);
			});
			base.start();
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
		/// 单词选择回调
		/// </summary>
		/// <param name="container">句子容器</param>
		/// <param name="word">单词</param>
		public void onWordSelected(WordsContainer container, WordDisplay word) {

            // 清除其他句子选择
            foreach (var sub in articleDisplay.getSubViews()) {
				if (sub == container) continue;
				var display = sub as SentenceDisplay;
				display?.container?.deselect();
            }

			correctionWindow.startWindow(container, word);
        }

        /// <summary>
        /// 提交回调
        /// </summary>
        public void onSubmit() {
			var answers = generateWrongItems();
			var question = articleDisplay.getItem();

			engSer.answerCorrection(question, answers, processReward);
		}

		/// <summary>
		/// 生成错误项列表
		/// </summary>
		List<FrontendWrongItem> generateWrongItems() {
			Debug.Log("generateWrongItems");

			var answers = new List<FrontendWrongItem>();
			int sid = 1, wid = 1;

			// 遍历每句话
			foreach (var sub in articleDisplay.getSubViews()) {
				var sentenceDisplay = sub as SentenceDisplay;
				var sentenceContainer = sentenceDisplay.container;

				wid = 1;
				foreach (var subWord in sentenceContainer.getSubViews()) {
					var wordDisplay = subWord as WordDisplay;
					if (wordDisplay == null) continue;

					var answer = wordDisplay.generateWrongItem(sid, wid++);
					if (answer != null) answers.Add(answer);
				}
				sid++;
			}
			return answers;
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

