using Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UI.ExerPro.EnglishPro.CorrectionScene.Controls;
using Core.UI.Utils;
using UnityEngine.UI;
using ExerPro.EnglishModule.Data;

namespace UI.CorrectionScene.Windows {
	public class CorrectionWindow : BaseWindow {

		public InputField inputField;
		public CorrectionQuestion question;

		/// <summary>
		/// 场景组件引用
		/// </summary>
		public string selectedChangedAfterWord;
		public SentenceContainer currentSenContainer;

		/// <summary>
		/// 开启窗口
		/// </summary>
		public override void startWindow() {
			base.startWindow();
			inputField.text = "";
			inputField.ActivateInputField();
		}

		/// <summary>
		/// 确认
		/// </summary>
		public void confirm() {
			List<string> items = currentSenContainer.getItems().ToList<string>();
			int currentWordIndex = currentSenContainer.getSelectedIndex();
			string selectedChangedAfterWord = inputField.text;
			if (items[currentWordIndex] == "  ") {
				//空格->空
				if (selectedChangedAfterWord == "") {
					return;
				}
				//增加
				else {
					items.Insert(currentWordIndex, selectedChangedAfterWord);
					items.Insert(currentWordIndex, "  ");
				}
			} else {
				//删除
				if (selectedChangedAfterWord == "") {
					items[currentWordIndex] = selectedChangedAfterWord;
					items.RemoveRange(currentWordIndex, 2);
				}
				//更改
				else {
					items[currentWordIndex] = selectedChangedAfterWord;
				}
			}

			currentSenContainer.clearItems();
			currentSenContainer.setItem(string.Join("", items));
			cancel();
		}

		/// <summary>
		/// 取消
		/// </summary>
		public void cancel() {
			base.terminateWindow();
		}
	}
}
