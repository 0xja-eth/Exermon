using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.UI;

using Core.UI;
using Core.UI.Utils;
using Core.Systems;

using ExerPro.EnglishModule.Data;

using UI.Common.Controls.InputFields;

using FrontendWrongItem = ExerPro.EnglishModule.Data.
	CorrectionQuestion.FrontendWrongItem;

namespace UI.ExerPro.EnglishPro.CorrectionScene.Windows {

	using Controls;

	/// <summary>
	/// 改错窗口
	/// </summary>
	public class CorrectionWindow : BaseWindow {

		/// <summary>
		/// 文本常量定义
		/// </summary>
		const string AddPrevWordsAlertText = "只有句首单词才可以往前面添加单词！";
		const string AddExceedWordsAlertText = "增添最多填写两个单词！";
		const string EditExceedWordsAlertText = "修改只能填写一个单词！";

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public TextInputField inputField;
        public CorrectionQuestion question;

		public Text originWord;

		/// <summary>
		/// 外部系统设置
		/// </summary>
		GameSystem gameSys;

		/// <summary>
		/// 场景组件引用
		/// </summary>
		CorrectionScene scene;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		WordDisplay currentWord;

		#region 初始化

		/// <summary>
		/// 初始化场景组件
		/// </summary>
		protected override void initializeScene() {
			base.initializeScene();
			scene = SceneUtils.getCurrentScene<CorrectionScene>();
		}

		/// <summary>
		/// 初始化外部系统
		/// </summary>
		protected override void initializeSystems() {
            base.initializeSystems();
            gameSys = GameSystem.get();
        }

		#endregion

		#region 启动
		
		/// <summary>
		/// 开启窗口
		/// </summary>
		public void startWindow(WordsContainer container, WordDisplay word) {
			startWindow();
			setupCurrent(container, word);
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 配置当前关联内容
		/// </summary>
		void setupCurrent(WordsContainer container, WordDisplay word) {
			currentWord = word;
		}

		#endregion

		#region 界面绘制

		/// <summary>
		/// 刷新
		/// </summary>
		protected override void refresh() {
			base.refresh();
			Debug.Log("CorrectionWindow.refresh");

			originWord.text = currentWord.originalWord;
			inputField.setValue(currentWord.originalWord);
			//inputField.activate();
		}

		#endregion

		#region 流程控制

		/// <summary>
		/// 确认
		/// </summary>
		public void confirm() {
			var word = inputField.getValue();
			var ori = currentWord.originalWord;
			if (!check(word, ori, currentWord.getWid()))
				return;

			var answer = generateWrongItem(word);
			if (word == ori) // 撤销修改
				scene.revertAnswer(answer);
			else if (!scene.addAnswer(answer)) return;

			cancel();
		}
		
		/// <summary>
		/// 生成错误项
		/// </summary>
		/// <param name="display"></param>
		/// <returns></returns>
		public FrontendWrongItem generateWrongItem(string word) {
			int sid = currentWord.getSid();
			int wid = currentWord.getWid();

			Debug.Log("generateWrongItem: " +
				sid + ", " + wid + ": " +
				currentWord.originalWord + " -> " + word);

			return new FrontendWrongItem(sid, wid, word);
		}

		/// <summary>
		/// 检查格式
		/// </summary>
		/// <returns></returns>
		bool check(string word, string ori, int wid) {
			var words = word.Split(' ');

			if (words.Length <= 1) return true;

			if (words.Length > 2)
				return requestAlert(AddExceedWordsAlertText);

			// 如果长度为2
			if (words.Length == 2) 
				// 第二个单词为原单词，但是wid不在句首
				if (words[1] == ori && wid > 1)
					return requestAlert(AddPrevWordsAlertText);
				// 两个单词都不与原单词相同
				else if (words[0] != ori && words[1] != ori)
					return requestAlert(EditExceedWordsAlertText);

			return true;
		}

		/// <summary>
		/// 错误提示请求
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		bool requestAlert(string text) {
			gameSys.requestAlert(text); return false;
		}

		/// <summary>
		/// 重置
		/// </summary>
		public void revert() {
			inputField.setValue(currentWord.originalWord);
		}

		/// <summary>
		/// 取消
		/// </summary>
		public void cancel() {
			base.terminateWindow();
			//currentSenContainer?.deselect();
		}
		/*
		/// <summary>
		/// 限定输入
		/// </summary>
		public void inputLimit() {
			var text = inputField.getValue();
			inputField.setValue(Regex.Replace(text, @"[^a-zA-Z| ]", ""));
		}
		*/
		#endregion

	}
}
