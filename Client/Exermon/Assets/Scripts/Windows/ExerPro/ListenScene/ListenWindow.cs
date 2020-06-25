
using System.IO;

using UnityEngine;

using Core.UI;
using Core.UI.Utils;

using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;

namespace UI.ExerPro.EnglishPro.ListenScene.Windows {

	using Controls;

	/// <summary>
	/// 剧情窗口
	/// </summary>
	public class ListenWindow : BaseWindow {

        /// <summary>
        /// 外部变量
        /// </summary>
        public ListenQuestionDisplay questionDisplay;

        /// <summary>
        /// 外部系统
        /// </summary>
        EnglishService engServ;

        /// <summary>
        /// 场景组件引用
        /// </summary>
        protected ListenScene scene;

        /// <summary>
        /// 内部变量
        /// </summary>
        ListeningQuestion question;
        ListeningSubQuestion[] questions;

        #region 初始化

        /// 初始化场景
        /// </summary>
        protected override void initializeScene() {
            scene = SceneUtils.getCurrentScene<ListenScene>();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            engServ = EnglishService.get();
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 当前题目
        /// </summary>
        /// <returns></returns>
        public ListeningQuestion currentQuestion() {
            return questionDisplay.getItem();
        }

		#endregion

		#region 界面绘制

		/// <summary>
		/// 刷新窗口
		/// </summary>
		protected override void refresh() {
			base.refresh();
			configureQuestion();
		}

		/// <summary>
		/// 配置题目
		/// </summary>
		void configureQuestion() {
			Debug.Log("configureQuestion");
			engServ.generateQuestion<ListeningQuestion>(onGetQuestionSuccess);
			//onGetQuestionFailed();
		}

		#endregion

		#region 回调控制

		/// <summary>
		/// 获取题目回调
		/// </summary>
		/// <param name="questions"></param>
		void onGetQuestionSuccess(ListeningQuestion remoteQuestion) {
            Debug.Log("onGetQuestionSuccess: " + remoteQuestion.toJson().ToJson());
            question = remoteQuestion;
            questions = question.subQuestions;
            questionDisplay.startView(question);
        }

        #endregion
    }
}
