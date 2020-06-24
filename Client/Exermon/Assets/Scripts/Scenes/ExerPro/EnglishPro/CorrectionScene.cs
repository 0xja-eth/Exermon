
using Core.UI;
using Core.Systems;
using UI.ExerPro.EnglishPro.CorrectionScene.Controls;
using System.Collections.Generic;
using UnityEngine;
using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;
using static ExerPro.EnglishModule.Data.CorrectionQuestion;
using UnityEngine.UI;
using UI.CorrectionScene.Windows;
using Core.UI.Utils;
using UI.Common.Controls.ItemDisplays;

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
            engSer.generateQuestions<CorrectionQuestion>(1, (res) =>
            {
                articleDisplay.startView(res[0]);
            });
            base.start();
        }

        #endregion


        #region 控制

        /// <summary>
        /// 单词选择回调
        /// </summary>
        /// <param name="container">句子容器</param>
        /// <param name="word">单词</param>
        public void onWordSelected(SentenceContainer container, string word) {
            //清除其他句子选择
            foreach (ItemDisplay<string> item in articleDisplay.getSubViews()) {
                SentenceContainer each = SceneUtils.get<SentenceContainer>(item.gameObject);
                if (each == container || each.getSelectedIndex() == -1)
                    continue;
                each.deselect();
            }
            changedBeforeValue.text = word;
            correctionWindow.currentSenContainer = container;
            correctionWindow.startWindow();
        }


        /// <summary>
        /// 提交回调
        /// </summary>
        public void onSubmit() {
            int sentenceIndex = 1;
            int wordIndex = 1;
            List<Answer> answers = new List<Answer>();
            foreach (var sentenceDisplay in articleDisplay.getSubViews()) {
                SentenceContainer sentenceContainer = SceneUtils.get<SentenceContainer>(sentenceDisplay.gameObject);
                wordIndex = 1;
                foreach (WordDisplay wordDisplay in sentenceContainer.getSubViews()) {
                    Debug.Log(wordDisplay.state);
                    if (wordDisplay.state != WordDisplay.State.Original) {
                        Answer answer = new Answer();
                        answer.sid = sentenceIndex; answer.wid = wordIndex;
                        answer.word = wordDisplay.getItem();
                        Debug.Log(sentenceIndex + wordIndex + wordDisplay.getItem());
                        answers.Add(answer);
                    }
                    wordIndex++;
                }
                sentenceIndex++;
            }
            Debug.Log("AAA" + answers.ToArray().Length);

            engSer.exitNode(true);
        }
        #endregion
    }


}

