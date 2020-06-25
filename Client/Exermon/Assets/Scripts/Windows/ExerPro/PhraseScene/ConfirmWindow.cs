using Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UI.ExerPro.EnglishPro.CorrectionScene.Controls;
using Core.UI.Utils;
using UnityEngine.UI;
using ExerPro.EnglishModule.Data;
using Core.Systems;
using System.Text.RegularExpressions;

namespace UI.PhraseScene.Windows {
    public class ConfirmWindow : BaseWindow {


        /// <summary>
        /// 场景组件引用
        /// </summary>

        /// <summary>
        /// 场景对象引用
        /// </summary>
        public GameObject wrongImg;
        public GameObject correctAnswerText;
        public Text answer;

        public GameObject correctImg;
        public GameObject correctTip;
        public Assets.Scripts.Scenes.ExerPro.EnglishPro.PhraseScene scene;

        string correctAnswer;
        bool correctFlag;

        protected override void initializeOnce() {
            base.initializeOnce();
        }

        /// <summary>
        /// 开启窗口
        /// </summary>
        public override void startWindow() {
            base.startWindow();
        }

        /// <summary>
        /// 判断答题结果
        /// </summary>
        bool isCorrect(string answer) {
            if (answer == "") return true;
            return false;
        }


        /// <summary>
        /// 初始化显示
        /// </summary>
        public void initView(string word, string correctAnswer) {
            this.correctAnswer = word + " " + correctAnswer;
            correctFlag = isCorrect(correctAnswer);
            scene.record(correctFlag);
            answer.text = this.correctAnswer;
            wrongImg.SetActive(!correctFlag);
            correctAnswerText.SetActive(!correctFlag);
            answer.gameObject.SetActive(!correctFlag);
            correctImg.SetActive(correctFlag);
            correctTip.SetActive(correctFlag);
            startWindow();
        }

        /// <summary>
        /// 确认
        /// </summary>
        public void confirm() {
            this.terminateWindow();
            scene.nextQuestion();
        }
    }
}
