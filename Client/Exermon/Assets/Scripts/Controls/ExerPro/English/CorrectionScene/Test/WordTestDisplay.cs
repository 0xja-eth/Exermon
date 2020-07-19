using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Core.UI.Utils;

using UI.Common.Controls.ItemDisplays;

using FrontendWrongItem = ExerPro.EnglishModule.Data.
    CorrectionQuestion.FrontendWrongItem;
using System.Text.RegularExpressions;
using UI.ExerPro.EnglishPro.CorrectionScene.Controls.Test;

//改错布局
namespace UI.ExerPro.EnglishPro.CorrectionScene.Controls {

    /// <summary>
    /// 单词
    /// </summary
    public class WordTestDisplay : SelectableItemDisplay<string> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public GameObject deleteFlag, changeFlag, addPrevFlag, addNextFlag;
        public Text text;

        /// <summary>
        /// 外部变量控制
        /// </summary>
        public Color correctColor = new Color(72, 127, 74, 255) / 255f;
        public Color wrongColor = new Color(150, 28, 70, 255) / 255f;

        public ArticleTestDisplay articleDisplay { get; set; }
        /// <summary>
        /// 内部组件设置
        /// </summary>
        Text changeText, addPrevText, addNextText;

        /// <summary>
        /// 状态枚举
        /// </summary>
        public enum State {
            Original, Modefied,
            AddedNext, AddedPrev, // 后一个增加，前一个增加
            Deleted
        }

        /// <summary>
        /// 内部变量定义
        /// </summary>
        string _originalWord; // 原始文章
        public string originalWord {
            get { return _originalWord; }
            set {
                _originalWord = value;
                noPunWord = filterWord(value);
            }
        }

        public string noPunWord { get; set; } // 去除标点后的word，用于判断，orginalWord用于展示
        public string correctWord { get; set; } = null; // 正确答案

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            changeText = SceneUtils.find<Text>(changeFlag, "Text");
            addPrevText = SceneUtils.find<Text>(addPrevFlag, "Text");
            addNextText = SceneUtils.find<Text>(addNextFlag, "Text");
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 获取wid
        /// </summary>
        /// <returns></returns>
        public int getWid() {
            return index + 1;
        }

        /// <summary>
        /// 获取sid
        /// </summary>
        /// <returns></returns>
        public int getSid() {
            return getContainer().sentenceDisplay.sid;
        }

        /// <summary>
        /// 获取容器
        /// </summary>
        /// <returns></returns>
        public new WordsContainer getContainer() {
            return container as WordsContainer;
        }

        /// <summary>
        /// 是否有修改
        /// </summary>
        /// <returns></returns>
        public bool isChanged(string word = null) {
            if (word == null) word = item;
            return word != noPunWord;
        }

        /// <summary>
        /// 过滤单词非法符号
        /// </summary>
        /// <param name="str">原单词</param>
        /// <returns></returns>
        string filterWord(string str) {
            return Regex.Replace(str, @"[^a-zA-Z '‘\-]", "");
        }

        /// <summary>
        /// 计算当前修改状态
        /// </summary>
        /// <returns></returns>
        State calcState(string word) {
            if (word == "") return State.Deleted;
            word = filterWord(word); // Regex.Replace(word, @"[^a-zA-Z '‘\-]", "");

            //var words = word.Split("", StringSplitOptions.RemoveEmptyEntries);
            var words = word.Split(' ');

            if (words.Length == 2) {
                if (words[0] == noPunWord) return State.AddedNext;
                if (words[1] == noPunWord) return State.AddedPrev;
            }

            if (word == noPunWord) return State.Original;


            return State.Modefied;
        }

        /// <summary>
        /// 获取增加的单词
        /// </summary>
        string getChangedWord(string word = null) {
            if (word == null) word = item;

            var words = word.Split(' ');
            if (words.Length <= 0) return "";

            if (words.Length == 2 &&
                words[0] == noPunWord) return words[1];

            return words[0];
        }

        /// <summary>
        /// 复原
        /// </summary>
        public void revert() {
            setItem(originalWord);
        }

        /// <summary>
        /// 配置正确单词
        /// </summary>
        public void setCorrectWord(string word) {
            correctWord = word;
            requestRefresh();
        }

        /// <summary>
        /// 清除正确单词
        /// </summary>
        public void clearCorrectWord() {
            setCorrectWord(null);
        }

        /// <summary>
        /// 是否正确
        /// </summary>
        /// <returns></returns>
        public bool isCorrect(string word = null) {
            if (word == null) word = item;

            if (correctWord == null) // 若不需要改
                return word == originalWord; // 是否保持原样

            // 如果需要改，筛选出备选项
            var changed = getChangedWord(word);
            var corrChanged = getChangedWord(correctWord);
            var corrWords = corrChanged.Split('/').ToList();

            return corrWords.Contains(changed);
        }
        

        /// <summary>
        /// 生成错误项
        /// </summary>
        /// <param name="display"></param>
        /// <returns></returns>
        public FrontendWrongItem generateWrongItem() {
            if (isChanged()) {
                Debug.Log("generateWrongItem: " +
                    getSid() + ", " + getWid() + ": " +
                    originalWord + " -> " + item);
                return new FrontendWrongItem(getSid(), getWid(), item);
            }
            return null;
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制物品
        /// </summary>
        /// <param name="item"></param>
        protected override void drawExactlyItem(string item) {
            base.drawExactlyItem(item);
            text.text = originalWord;
        }
        

        /// <summary>
        /// 清除所有标记
        /// </summary>
        void clearFlags() {
            deleteFlag.SetActive(false);
            changeFlag.SetActive(false);
            addPrevFlag.SetActive(false);
            addNextFlag.SetActive(false);
        }

        /// <summary>
        /// 绘制空物品
        /// </summary>
        /// <returns></returns>
        protected override void drawEmptyItem() {
            base.drawEmptyItem();

            text.text = "";
            clearFlags();
        }

        #endregion
    }


}
