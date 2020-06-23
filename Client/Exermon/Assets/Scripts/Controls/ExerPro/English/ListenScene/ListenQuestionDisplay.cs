using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;

using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.ListenScene.Controls {
	/// <summary>
	/// 题目显示
	/// 利大佬说的Class E
	/// </summary>
	public class ListenQuestionDisplay : ItemDisplay<ListeningQuestion> {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Text tipName;
		public ListenSubQuestionContainer subQuestions;

		// 左部分组件
		public RawImage image;

		public Button playButton;
		public Slider slider;

		// 播放源
		public AudioSource audioSource;

		public GameObject confirmButton, submitButton;

		public Texture2D play, pause;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		int selectNumber = 0;

		Sprite playSprite, pauseSprite;

		/// <summary>
		/// 外部系统
		/// </summary>
		EnglishService engSer;

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			playSprite = AssetLoader.generateSprite(play);
			pauseSprite = AssetLoader.generateSprite(pause);
		}

		/// <summary>
		/// /初始化外部系统
		/// </summary>
		protected override void initializeSystems() {
			base.initializeSystems();
			engSer = EnglishService.get();
		}

		#endregion

		#region 更新控制

		/// <summary>
		/// 更新进度
		/// </summary>
		protected override void update() {
			base.update();
			updateAudio();
		}

		/// <summary>
		/// 更新音频
		/// </summary>
		void updateAudio() {
			playButton.interactable = !audioSource.isPlaying || isPauseable();
			if (audioSource.isPlaying) slider.value = audioSource.time / audioSource.clip.length;
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 能否暂停
		/// </summary>
		/// <returns></returns>
		public bool isPauseable() {
			return false;
		}

		#endregion

		#region 回调控制

		/// <summary>
		/// 提交答案
		/// </summary>
		public void onConfirmClicked() {
			subQuestions.showAnswer = true;

			if (confirmButton) confirmButton.SetActive(false);
			if (submitButton) submitButton.SetActive(true);

			// TODO: 加入判断
		}

		/// <summary>
		/// 提交进行结算
		/// </summary>
		public void onSubmitClicked() {
			if (submitButton) submitButton.SetActive(false);
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

			setupAudio(question);

			drawBaseInfo(question);
			drawSubQuestions(question);
			/*
            subDisplays = new List<ListeningSubQuestionDisplay>();
            foreach (ListeningSubQuestion q in questions) {
                ListeningSubQuestionDisplay ss = ListeningSubQuestionDisplay.Instantiate(subDisplay, textContent.transform);
                ss.content = content;
                ss.startView(q, this);
                subDisplays.Add(ss);
            }
			*/
		}

		/// <summary>
		/// 绘制基本信息
		/// </summary>
		void drawBaseInfo(ListeningQuestion question) {
			if (tipName) tipName.text = question.eventName;
			if (image) image.texture = question.picture;
		}

		/// <summary>
		/// 绘制子题目
		/// </summary>
		void drawSubQuestions(ListeningQuestion question) {
			subQuestions.setItems(question.subQuestions);
		}

		/// <summary>
		/// 配置音频
		/// </summary>
		/// <param name="question"></param>
		void setupAudio(ListeningQuestion question) {
			if (audioSource) audioSource.clip = question.audio;
		}

		/// <summary>
		/// 清除物品
		/// </summary>
		protected override void drawEmptyItem() {
			base.drawEmptyItem();

			if (tipName) tipName.text = "";
			subQuestions.clearItems();

			audioSource.Stop();
			audioSource.clip = null;

			playButton.interactable = false;
		}

		#endregion

		#region 播放音频

		/// <summary>
		/// 播放听力音频
		/// </summary>
		public void playAudio() {
			if (audioSource.clip == null) return;
			if (audioSource.isPlaying && isPauseable()) {
				audioSource.Pause();
				playButton.image.overrideSprite = AssetLoader.generateSprite(play);
			} else {
				audioSource.Play();
				playButton.image.overrideSprite = AssetLoader.generateSprite(pause);
			}
		}

		#endregion
	}
}
