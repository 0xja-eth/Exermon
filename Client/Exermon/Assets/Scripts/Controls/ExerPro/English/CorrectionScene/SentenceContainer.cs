using Core.UI.Utils;
using ExerPro.EnglishModule.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Common.Controls.ItemDisplays;
using UI.CorrectionScene.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ExerPro.EnglishPro.CorrectionScene.Controls
{
    public class SentenceContainer : SelectableContainerDisplay<string>
    {
        string ends = "!?.,";

        /// <summary>
        /// 子节点创建回调
        /// </summary>
        /// <param name="sub"></param>
        /// <param name="index"></param>
        protected override void onSubViewCreated(SelectableItemDisplay<string> sub, int index)
        {
            base.onSubViewCreated(sub, index);
            SceneUtils.get<Text>(subViews[index].gameObject).text = items[index];
        }


        public void setItem(string item, bool force = false)
        {
            List<string> items = item.Split(' ').ToList<string>();
            int size = items.ToArray().Length;
            for (int i = 0; i < size; i++)
            {
                items.Insert(2 * i, "  ");
            }
            string lastWord = items.Last<string>();
            string end = lastWord.Substring(lastWord.Length - 1);
            items.RemoveAt(items.ToArray().Length - 1);
            items.Add(lastWord.Substring(0, lastWord.Length - 1));
            items.Add("  ");
            items.Add(end);
            setItems(items);
        }


        protected override void onSelectChanged()
        {
            base.onSelectChanged();
            int index = getSelectedIndex();

            if (index == -1)
            {
                ((CorrectionScene)SceneUtils.getSceneObject("Scene")).onWordDeselected();
                return;
            }
            if (ends.IndexOf(items[index]) == -1)
            {
                ((CorrectionScene)SceneUtils.getSceneObject("Scene")).onWordSelected(this, items[index]);
            }
        }
        

    }
}
