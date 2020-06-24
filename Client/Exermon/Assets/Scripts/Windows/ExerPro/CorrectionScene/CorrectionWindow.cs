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
    public class CorrectionWindow : BaseWindow {

        public InputField inputField;
        public Text changedBeforeValue;
        public CorrectionQuestion question;
        string text = "最多填写两个单词！";

        GameSystem gameSys; 

        /// <summary>
        /// 场景组件引用
        /// </summary>
        public string selectedChangedAfterWord;
        public SentenceContainer currentSenContainer;
        public WordDisplay currentWord;

        protected override void initializeOnce() {
            base.initializeOnce();
            gameSys = GameSystem.get();
        }

        /// <summary>
        /// 开启窗口
        /// </summary>
        public override void startWindow() {
            base.startWindow();
            inputField.text = "";
            inputField.ActivateInputField();
            int currentWordIndex = currentSenContainer.getSelectedIndex();
            currentWord = ((WordDisplay)(currentSenContainer.getSubViews()[currentWordIndex]));
        }

        /// <summary>
        /// 确认
        /// </summary>
        public void confirm() {
            List<string> items = currentSenContainer.getItems().ToList<string>();
            int currentWordIndex = currentSenContainer.getSelectedIndex();
            string inputText = inputField.text;
            Debug.Log(inputText);
            //删除
            if (inputText == "") {
                currentWord.setItem(currentWord.originalWord);
                currentWord.state = WordDisplay.State.Deleted;
            }
            //复原
            else if (inputText == currentWord.originalWord) {
                currentWord.state = WordDisplay.State.Original;
                currentWord.setItem(inputText);
            }
            //增加
            else if (inputText.StartsWith(currentWord.originalWord)) {
                currentWord.state = WordDisplay.State.Added;
                currentWord.setItem(inputText);
            }
            //修改
            else {
                if (inputText.Split(' ').Length > 2) {
                    gameSys.requestAlert(text,
                        AlertWindow.Type.Notice);
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
            changedBeforeValue.text = currentWord.originalWord;
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

    }
}
