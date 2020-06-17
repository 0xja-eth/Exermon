using UnityEngine;
using UnityEngine.UI;

using UI.Common.Controls.ItemDisplays;
using ExerPro.EnglishModule.Data;
using UI.Common.Controls.SystemExtend.QuestionText;
using Assets.Scripts.Controls.ExerPro.English.ListenScene;

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
        public RectTransform textContent; //子题目的父亲

        /// <summary>
        /// 显示答案解析
        /// </summary>
        bool _showAnswer = false;
        public bool showAnswer {
            get { return _showAnswer; }
            set {
                _showAnswer = value;
                requestRefresh();
            }
        }

        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            //choiceContainer?.addClickedCallback(onChoiceSelected);
            
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

        void onChoiceSelected(int index) {
            showAnswer = true;

            //var resultChoice = question.choices[index];
            var resultText = "";
            //var resultEffect = resultChoice.effects;

            requestRefresh(true);
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
            if (showAnswer) {
            }
            else {
                if (audioSource) audioSource.clip = question.audio;
                foreach (ListeningSubQuestion q in questions)
                {
                    ListeningSubQuestionDisplay ss = ListeningSubQuestionDisplay.Instantiate(subDisplay, textContent.transform);
                    ss.content = content;
                    ss.startView(q);
                }
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
		public void playAudio()
		{
            if (audioSource.isPlaying) {
                audioSource.Pause();
                playButton.image.sprite = Resources.Load<Sprite>("ExerPro/ListenScene/play");
            }
            else {
                audioSource.Play();
                playButton.image.sprite = Resources.Load<Sprite>("ExerPro/ListenScene/pause");
            }
		}
		protected override void update()
		{
			base.update();
            if(audioSource.isPlaying)
                slider.value = audioSource.time / audioSource.clip.length;
        }
		#endregion
	}
}
