using Core.Systems;
using Core.UI;
using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;
using UI.ExerPro.EnglishPro.PhraseScene.Controls;
using UnityEngine;
using UI.Common.Windows;
namespace Assets.Scripts.Scenes.ExerPro.EnglishPro {
    public class PhraseScene : BaseScene {
        /// <summary>
        /// 外部组件设置
        /// </summary>
        public OptionAreaDisplay optionAreaDisplay;
        int correctNum = 0;
        int wrongNum = 0;
        PhraseQuestion[] questions;
        int currentQuesIndex = 0;
        int questionNum = 10;
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

        #endregion

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
            engSer.generateQuestions<PhraseQuestion>(questionNum, (res) =>
            {
                questions = res;
                PhraseQuestion question = questions[currentQuesIndex++];
                while (question.options().Length == 0)
                    question = res[currentQuesIndex++];
                optionAreaDisplay.startView(question);

            });
            base.start();
        }

        /// <summary>
        /// 下一道
        /// </summary>
        public void nextQuestion() {
            if (currentQuesIndex >= questionNum)
                onSubmit();
            PhraseQuestion question = questions[currentQuesIndex++];
            while (question.options().Length == 0)
                question = questions[currentQuesIndex++];
            optionAreaDisplay.startView(question);
        }

        /// <summary>
        /// 提交
        /// </summary>
        public void onSubmit() {
            Debug.Log("答对：" + correctNum + "题,答错：" + wrongNum + "题。");
            gameSys.requestAlert("答对：" + correctNum + "题,答错：" + wrongNum + "题。",
                AlertWindow.Type.Notice);

            engSer.exitNode(true);
        }

        /// <summary>
        /// 记录正确和错误数量
        /// </summary>
        public void record(bool isCorrect) {
            if (isCorrect) correctNum++;
            else wrongNum++;
        }
    }
}
