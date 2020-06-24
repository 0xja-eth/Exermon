using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

using Core.UI;
using Core.UI.Utils;

using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;

using UI.Common.Controls.InputFields;
using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.ListenScene.Controls {
	/// <summary>
	/// 题目显示
	/// 利大佬说的Class E
	/// </summary>
	public class ListenQuestionDisplay : ItemDisplay<ListeningQuestion> {

		/// <summary>
		/// 常量定义
		/// </summary>
		const string CountFormat = "播放次数：{0}/{1}";
		const string TimeFormat = "{0}/{1}";

		const string ArticleDisableTipText = "作答中不可查看听力材料";

		const int ArticleViewIndex = 2;

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Text tipName, article, count, time;
		public ListenSubQuestionContainer subQuestions;

		// 左部组件
		public RawImage image;

		public Button playButton;
		public Slider slider;

		// 播放源
		public AudioSource audioSource;

		public GameObject confirmButton, submitButton;

		public Texture2D play, pause;

		public DropdownField showTypeSelect;
		public GameObject[] leftViews; // 左侧视图

		/// <summary>
		/// 内部变量定义
		/// </summary>
		int selectNumber = 0;
		int playCnt = 0; // 播放次数

		int[] selections = null;

		bool isLastPlaying = false;

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

			if (showTypeSelect) showTypeSelect.onChanged = onShowTypeChanged;
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

			if (audioSource.isPlaying)
				drawTimer(audioSource.time, audioSource.clip.length);
			else if (isLastPlaying) onAudioStop();

			isLastPlaying = audioSource.isPlaying;
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 能否暂停
		/// </summary>
		/// <returns></returns>
		public bool isPauseable() {
			return subQuestions.showAnswer;
		}
		
		/// <summary>
		/// 能否暂停
		/// </summary>
		/// <returns></returns>
		public bool isPlayable() {
			if (audioSource.isPlaying) return false;
			if (isNullItem(item)) return false;
			if (item.times <= 0) return true;

			return playCnt < item.times || subQuestions.showAnswer;
		}

		#endregion

		#region 回调控制

		/// <summary>
		/// 音频停止回调
		/// </summary>
		public void onAudioStop() {
			refreshAudioInfo();
		}

		/// <summary>
		/// 提交答案
		/// </summary>
		public void onConfirmClicked() {

			subQuestions.showAnswer = true;
			selections = subQuestions.saveSelections();

			showTypeSelect.setIndex(ArticleViewIndex);

			if (confirmButton) confirmButton.SetActive(false);
			if (submitButton) submitButton.SetActive(true);

			stopAudio();
		}

		/// <summary>
		/// 提交进行结算
		/// </summary>
		public void onSubmitClicked() {
			if (submitButton) submitButton.SetActive(false);

			stopAudio();
			engSer.answerListening(item, selections, processReward);
		}

		/// <summary>
		/// 处理奖励
		/// </summary>
		/// <param name="corrCnt">正确题目数</param>
		void processReward(int corrCnt) {
			Debug.Log("Correct Count = " + corrCnt);
			engSer.processReward(questionNumber: corrCnt);
		}

		/// <summary>
		/// 显示类型改变回调
		/// </summary>
		public void onShowTypeChanged(Tuple<int, string> obj) {
			if (!showTypeSelect) return;
			var index = showTypeSelect.getIndex();
			for (int i = 0; i < leftViews.Length; ++i) {
				var go = leftViews[i];
				var view = SceneUtils.get<BaseView>(go);
				if (view != null)
					if (i == index) view.startView();
					else view.terminateView();
				else go.SetActive(i == index);
			}
		}
		
		#endregion

		#region 界面控制

		/// <summary>
		/// 刷新音频信息
		/// </summary>
		void refreshAudioInfo() {
			if (isNullItem(item)) drawEmptyItem();
			else {
				drawAudioInfo(item);
				drawBaseInfo(item);
			}
		}

		/// <summary>
		/// 绘制确切物品
		/// </summary>
		/// <param name="question">题目</param>
		protected override void drawExactlyItem(ListeningQuestion question) {
			base.drawExactlyItem(question);
			ListeningSubQuestion[] questions = question.subQuestions;

			setupAudio(question);

			drawAudioInfo(question);
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
		/// 配置音频
		/// </summary>
		/// <param name="question"></param>
		void setupAudio(ListeningQuestion question) {
			audioSource.clip = question.audio;
		}

		/// <summary>
		/// 配置音频
		/// </summary>
		/// <param name="question"></param>
		void drawAudioInfo(ListeningQuestion question) {
			if (question.times <= 0) return;
			if (count) {
				if (!subQuestions.showAnswer)
					count.text = string.Format(CountFormat, playCnt, question.times);
				else count.text = "";
			}
			var sprite = audioSource.isPlaying ? pauseSprite : playSprite;
			playButton.image.overrideSprite = sprite;
		}

		/// <summary>
		/// 绘制基本信息
		/// </summary>
		void drawBaseInfo(ListeningQuestion question) {
			if (tipName) tipName.text = question.eventName;
			if (image) image.texture = question.picture;

			if (subQuestions.showAnswer) article.text = question.article;
			else article.text = ArticleDisableTipText;
		}

		/// <summary>
		/// 绘制子题目
		/// </summary>
		void drawSubQuestions(ListeningQuestion question) {
			subQuestions.setItems(question.subQuestions);
		}

		/// <summary>
		/// 绘制时间
		/// </summary>
		void drawTimer(double cur, double max) {
			slider.value = (float)(cur / max);

			if (time) {
				var curTxt = SceneUtils.time2Str(cur);
				var maxTxt = SceneUtils.time2Str(max);

				time.text = string.Format(TimeFormat, curTxt, maxTxt);
			}
		}

		/// <summary>
		/// 清除物品
		/// </summary>
		protected override void drawEmptyItem() {
			base.drawEmptyItem();

			if (tipName) tipName.text = "";
			if (count) count.text = "";
			if (time) time.text = "";

			article.text = "";

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
		public void toggleAudio() {
			if (audioSource.clip == null) return;
			if (isPauseable()) pauseAudio();
			else if (isPlayable()) playAudio();
		}

		/// <summary>
		/// 暂停
		/// </summary>
		void pauseAudio() {
			audioSource.Pause();
			refreshAudioInfo();
		}

		/// <summary>
		/// 播放
		/// </summary>
		void playAudio() {
			audioSource.Play(); playCnt++;
			refreshAudioInfo();
		}

		/// <summary>
		/// 停止
		/// </summary>
		void stopAudio() {
			audioSource.Stop();
			refreshAudioInfo();
		}

		#endregion
	}
}
