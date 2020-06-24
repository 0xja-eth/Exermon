using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Common.Controls.ItemDisplays;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ExerPro.EnglishPro.CorrectionScene.Controls {

    /// <summary>
    /// 单词
    /// </summary
    public class WordDisplay :
        SelectableItemDisplay<string> {

        public GameObject deletedImage;
        public Text text;

        /// <summary>
        /// 状态枚举
        /// </summary>
        public enum State {
            Original, Modefied, Added, Deleted
        }

        public string originalWord;
        public State state = State.Original;


        #region 界面绘制
        /// <summary>
        /// 绘制物品
        /// </summary>
        /// <param name="item"></param>
        protected override void drawExactlyItem(string item) {
            base.drawExactlyItem(item);
            text.text = item;
            switch (state) {
                case State.Modefied:
                    text.color = new Color(1.0f, 0.0f, 0.0f);
                    break;
                case State.Added:
                    text.color = new Color(1.0f, 0.0f, 0.0f);
                    break;
                case State.Deleted:
                    text.color = new Color(1.0f, 0.0f, 0.0f);
                    deletedImage.SetActive(true);
                    break;
                default:
                    text.color = new Color(1.0f, 1.0f, 1.0f);
                    deletedImage.SetActive(false);
                    break;
            }
        }
        #endregion
    }


}
