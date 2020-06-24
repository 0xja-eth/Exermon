using Core.UI.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UI.Common.Controls.ItemDisplays;
using UnityEngine.UI;

namespace UI.ExerPro.EnglishPro.CorrectionScene.Controls {
    public class SentenceContainer : SelectableContainerDisplay<string> {
        string ends = "!?.,\"";
        public CorrectionScene correctionScene;

        Regex regex = new Regex("[a-zA-Z|’]{2,}");

        #region 初始化
        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            correctionScene = ((CorrectionScene)SceneUtils.getSceneObject("Scene"));
        }
        #endregion

        /// <summary>
        /// 子节点创建回调
        /// </summary>
        /// <param name="sub"></param>
        /// <param name="index"></param>
        protected override void onSubViewCreated(SelectableItemDisplay<string> sub, int index) {
            base.onSubViewCreated(sub, index);
            SceneUtils.get<Text>(sub.gameObject).text = items[index];
            ((WordDisplay)sub).originalWord = items[index];
        }


        /// <summary>
        /// 设置物品
        /// </summary>
        public void setItem(string item, bool force = false) {
            string temp = item.Trim();
            List<string> items = temp.Split(' ').ToList<string>();
            int size = items.ToArray().Length;
            //for (int i = 0; i < size; i++) {
            //    items.Insert(2 * i, "  ");
            //}
            string lastWord = items.Last<string>();
            //string end = lastWord.Substring(lastWord.Length - 1);
            //items.RemoveAt(items.ToArray().Length - 1);
            //items.Add(lastWord.Substring(0, lastWord.Length - 1));
            //items.Add("  ");
            //items.Add(end);
            base.setItems(items);
        }


        /// <summary>
        /// 选择回调
        /// </summary>
        protected override void onSelectChanged() {

            base.onSelectChanged();
            int index = getSelectedIndex();
            if (index == -1) {
                return;
            }
            string word = ((WordDisplay)getSubViews()[index]).getItem();
            if (regex.IsMatch(word)) {
                correctionScene.onWordSelected(this, word);
            }
        }


    }
}
