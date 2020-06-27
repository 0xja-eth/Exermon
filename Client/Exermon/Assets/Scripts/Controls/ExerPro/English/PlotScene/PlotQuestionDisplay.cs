using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;

using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.PlotScene.Controls {

	/// <summary>
	/// 剧情题目显示
	/// </summary>
    public class PlotQuestionDisplay : ItemDetailDisplay<PlotQuestion> {

        /// <summary>
        /// 外部变量
        /// </summary>
        //public Text tipName;
        public Image image;
        public Text title;
        public PlotChoiceContainer choiceContainer; // 选项容器
        public RectTransform content;
        public Button exitButton;

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

		EnglishService engSer;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            choiceContainer?.addClickedCallback(onChoose);
        }

		/// <summary>
		/// 初始化外部系统
		/// </summary>
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
		/// 选项选择回调
		/// </summary>
		/// <param name="index"></param>
		void onChoose(int index) {
            showAnswer = true;

            var question = choiceContainer.getItem();
            var resultChoice = question.choices[index];
            var resultText = resultChoice.resultText;
            var resultEffect = resultChoice.effects;

			title.text = resultText;

			exitButton?.gameObject.SetActive(true);

			processEffects(resultEffect);
            requestRefresh(true);
        }

		/// <summary>
		/// 处理效果
		/// </summary>
		/// <param name="effects"></param>
		void processEffects(ExerProEffectData[] effects) {
			var actor = engSer.record.actor;
			engSer.processEffects(effects);
			processResult(actor.getResult());
		}

		/// <summary>
		/// 处理结果
		/// </summary>
		/// <param name="result"></param>
		void processResult(RuntimeActionResult result) {

		}

        #endregion

        #region 界面控制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="question">题目</param>
        protected override void drawExactlyItem(PlotQuestion question) {
            base.drawExactlyItem(question);
            drawBaseInfo(question);
            if (showAnswer) {
                if (title) title.gameObject.SetActive(false);
                if (choiceContainer) choiceContainer.gameObject.SetActive(false);
                //if (description) description.gameObject.SetActive(true);
            }
            else {
                if (title) title.gameObject.SetActive(true);
                if (choiceContainer) choiceContainer.gameObject.SetActive(true);
                //if (description) description.gameObject.SetActive(false);
                drawTitle(question);
                drawChoices(question);
            }
        }

        /// <summary>
        /// 绘制基本信息
        /// </summary>
        void drawBaseInfo(PlotQuestion question) {
            //if (tipName)
            //    tipName.text = question.eventName;
            if (image)
                image.overrideSprite = AssetLoader.generateSprite(item.picture);
        }

        /// <summary>
        /// 绘制题干
        /// </summary>
        /// <param name="question">题目</param>
        void drawTitle(PlotQuestion question) {
            if (title)
                title.text = question.title;
        }

        /// <summary>
        /// 绘制图片和选项
        /// </summary>
        /// <param name="question">题目</param>
        void drawChoices(PlotQuestion question) {
            choiceContainer?.startView(question);
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem() {
            base.drawEmptyItem();
            title.text = "";
			image.gameObject.SetActive(false);

            //if (tipName) tipName.text = "";
            if (choiceContainer) choiceContainer.clearItems();
            //if (description) description.text = "";
            //if (description.gameObject) description.gameObject.SetActive(false);
        }

        #endregion

    }
}
