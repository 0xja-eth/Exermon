using Core.UI;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UI.ExerPro.EnglishPro.CorrectionScene.Controls;
using UnityEngine.UI;
using ExerPro.EnglishModule.Data;
using Core.Systems;
using System.Text.RegularExpressions;
using UI.Common.Windows;

namespace UI.CorrectionScene.Windows {

	/// <summary>
	/// 改错窗口
	/// </summary>
	public class CorrectionWindow : BaseWindow {

		/// <summary>
		/// 文本常量定义
		/// </summary>
		const string ExceedWordsAlertText = "最多填写两个单词！";

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public InputField inputField;
        public CorrectionQuestion question;

		public Text changedBeforeValue;

		/// <summary>
		/// 外部系统设置
		/// </summary>
		GameSystem gameSys; 

        /// <summary>
        /// 场景组件引用
        /// </summary>
        public string selectedChangedAfterWord;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		WordDisplay currentWord;
		WordsContainer currentSenContainer;

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
            base.initializeOnce();
            gameSys = GameSystem.get();
        }

		#endregion

		#region 启动
		
		/// <summary>
		/// 开启窗口
		/// </summary>
		public void startWindow(WordsContainer container, WordDisplay word) {
			startWindow();

			changedBeforeValue.text = currentWord.originalWord;
			inputField.text = currentWord.originalWord;
			inputField.ActivateInputField();

			currentSenContainer = container;
			currentWord = word;
		}

		#endregion

		#region 流程控制

		/// <summary>
		/// 确认
		/// </summary>
		public void confirm() {
			//List<string> items = currentSenContainer.getWords();
			int currentWordIndex = currentSenContainer.getSelectedIndex();

			string inputText = inputField.text;

			if (inputText == "") {
				// 删除
				currentWord.setItem(currentWord.originalWord);
				currentWord.state = WordDisplay.State.Deleted;
			} else if (inputText == currentWord.originalWord) {
				// 复原
				currentWord.state = WordDisplay.State.Original;
				currentWord.setItem(inputText);
			} else if (inputText.StartsWith(currentWord.originalWord)) {
				// 增加（后）
				currentWord.state = WordDisplay.State.AddedNext;
				currentWord.setItem(inputText);
			} else if (inputText.EndsWith(currentWord.originalWord)) {
				// 增加（前）
				currentWord.state = WordDisplay.State.AddedPrev;
				currentWord.setItem(inputText);
			} else { 
				// 修改
				if (inputText.Split(' ').Length > 2) {
					gameSys.requestAlert(ExceedWordsAlertText);
					return;
				}
				currentWord.state = WordDisplay.State.Modefied;
				currentWord.setItem(inputText);
			}
			currentWord.requestRefresh();

			cancel();
		}

		/// <summary>
		/// 重置
		/// </summary>
		public void revert() {
			inputField.text = currentWord.originalWord;
		}

		/// <summary>
		/// 取消
		/// </summary>
		public void cancel() {
			base.terminateWindow();
			currentSenContainer?.deselect();
		}

		/// <summary>
		/// 限定输入
		/// </summary>
		public void inputLimit() {
			inputField.text = Regex.Replace(inputField.text, @"[^a-zA-Z| ]", "");
		}

		#endregion

	}
}
