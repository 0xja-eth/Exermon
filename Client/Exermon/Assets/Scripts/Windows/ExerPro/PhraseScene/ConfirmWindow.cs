
using UnityEngine;
using UnityEngine.UI;

using Core.UI;
using Core.UI.Utils;

namespace UI.ExerPro.EnglishPro.PhraseScene.Windows {

	/// <summary>
	/// 结果窗口
	/// </summary>
    public class ConfirmWindow : BaseWindow {

		/// <summary>
		/// 常量定义
		/// </summary>
		const string WrongText = "答错了！";
		const string CorrectText = "答对了！";

		/// <summary>
		/// 场景组件引用
		/// </summary>
		PhraseScene scene;

		/// <summary>
		/// 场景对象引用
		/// </summary>
		public Text answerText;

		public GameObject wrongImg;
        public GameObject correctImg;

		public Text resultTip;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		string option, word, answer;

		#region 初始化

		protected override void initializeScene() {
			base.initializeScene();
			scene = SceneUtils.getCurrentScene<PhraseScene>();
		}

		#endregion

		#region 开启

		/// <summary>
		/// 初始化显示
		/// </summary>
		public void startWindow(string word, string option, string correctAnswer) {
			startWindow();
			setupAnswer(word, option, correctAnswer);
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 判断答题结果
		/// </summary>
		bool isCorrect() {
			return option == answer;
		}

		/// <summary>
		/// 配置答案
		/// </summary>
		/// <param name="word">原单词</param>
		/// <param name="option">回答答案</param>
		/// <param name="answer">正确答案</param>
		void setupAnswer(string word, string option, string answer) {
			this.word = word; this.option = option;
			this.answer = answer;

			scene.answer(option);

			requestRefresh();
		}

		#endregion

		#region 界面控制

		/// <summary>
		/// 刷新
		/// </summary>
		protected override void refresh() {
			base.refresh();
			var corr = isCorrect();

			answerText.text = word + " " + answer;
			resultTip.text = corr ? CorrectText : WrongText;

			wrongImg.SetActive(!corr);
			correctImg.SetActive(corr);
		}

		#endregion

		#region 流程控制

		/// <summary>
		/// 确认
		/// </summary>
		public void confirm() {
            terminateWindow();
            scene.nextQuestion();
        }

		#endregion
	}
}
