using UnityEngine;
using UnityEngine.UI;

using UI.Common.Controls.ItemDisplays;
using ExerPro.EnglishModule.Data;
using UI.Common.Controls.SystemExtend.QuestionText;
using Assets.Scripts.Controls.ExerPro.English.ListenScene;
using System.Collections.Generic;
using ExerPro.EnglishModule.Services;

namespace UI.ExerPro.EnglishPro.ListenScene.Controls {
    /// <summary>
    /// 题目显示
    /// 利大佬说的Class E
    /// </summary>
    public class ListenQuestionDisplay : ItemDisplay<ListeningQuestion> {
        /// <summary>
        /// 外部变量
        /// </summary>
        public Text tipName;
        public RawImage image;
        public RectTransform content;
        public Button playButton;
        public Slider slider;
        public ListeningSubQuestionDisplay subDisplay;
        public Button confirmButton;
        public Button submitButton;

        int selectNumber = 0;
        public RectTransform textContent; //子题目的父亲

        /// <summary>
        /// 外部系统
        /// </summary>
        EnglishService engSer;

        /// <summary>
        /// 内部变量
        /// </summary>
        List<ListeningSubQuestionDisplay> subDisplays;

        #region 初始化
        protected override void initializeSystems() {
            base.initializeSystems();
            engSer = EnglishService.get();
        }
        #endregion


        #region 回调函数
        /// <summary>
        /// 物品变更回调
        /// </summary>
        protected override void onItemChanged() {
            base.onItemChanged();
            content.anchoredPosition = new Vector2(0, 0);
            // result = null; showAnswer = false;
        }

        /// <summary>
        /// 观察子题目的选择情况
        /// </summary>
        public void onSubDisplaySelected() {
            bool isFinished = true;
            foreach (var subDisplay in subDisplays)
                if (!subDisplay.isSelected)
                    isFinished = false;
            if (isFinished)
                confirmButton?.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// 提交答案
        /// </summary>
        public void onConfirmClicked() {
            foreach(var subDisplay in subDisplays) {
                subDisplay.showAnswer = true;
            }

            confirmButton?.gameObject.SetActive(false);
            submitButton?.gameObject.SetActive(true);
        }

        /// <summary>
        /// 提交进行结算
        /// </summary>
        public void onSubmitClicked() {
            submitButton?.gameObject.SetActive(false);
            engSer.processReward(questionNumber: 10);
        }
        #endregion

        #region 界面控制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="question">题目</param>
        protected override void drawExactlyItem(ListeningQuestion question) {
            base.drawExactlyItem(question);
            ListeningSubQuestion[] questions = question.subQuestions;
            drawBaseInfo(question);

            if (audioSource) audioSource.clip = question.audio;

            subDisplays = new List<ListeningSubQuestionDisplay>();
            foreach (ListeningSubQuestion q in questions) {
                ListeningSubQuestionDisplay ss = ListeningSubQuestionDisplay.Instantiate(subDisplay, textContent.transform);
                ss.content = content;
                ss.startView(q, this);
                subDisplays.Add(ss);
            }

        }

        /// <summary>
        /// 绘制基本信息
        /// </summary>
        void drawBaseInfo(ListeningQuestion question) {
            if (tipName)
                tipName.text = question.eventName;
            if (image)
                image.texture = question.picture;
        }



        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem() {
            base.drawEmptyItem();

            if (tipName) tipName.text = "";
        }

        #endregion

        #region 播放音频
        /// <summary>
        /// 播放源
        /// </summary>
        public AudioSource audioSource;

        /// <summary>
        /// 播放听力音频
        /// </summary>
        public void playAudio() {
            if (audioSource.isPlaying) {
                audioSource.Pause();
                playButton.image.sprite = Resources.Load<Sprite>("ExerPro/ListenScene/play");
            }
            else {
                audioSource.Play();
                playButton.image.sprite = Resources.Load<Sprite>("ExerPro/ListenScene/pause");
            }
        }
        protected override void update() {
            base.update();
            if (audioSource.isPlaying)
                slider.value = audioSource.time / audioSource.clip.length;
        }
        #endregion
    }
}
