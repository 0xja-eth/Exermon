using Core.UI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Common.Controls.ItemDisplays;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ExerPro.EnglishPro.CorrectionScene.Controls
{
    public class SentenceContainer : SelectableContainerDisplay<string>
    {

        /// <summary>
        /// 子节点创建回调
        /// </summary>
        /// <param name="sub"></param>
        /// <param name="index"></param>
        protected override void onSubViewCreated(SelectableItemDisplay<string> sub, int index)
        {
            base.onSubViewCreated(sub, index);
            SceneUtils.get<Text>(subViews[index].gameObject).text = items[index];
            //SceneUtils.get<RectTransform>(subViews[index].gameObject).sizeDelta = SceneUtils.get<RectTransform>(SceneUtils.get<WordDisplay>(subViews[index].gameObject).textObj).sizeDelta;
        }

        public void setItem(string item, bool force = false)
        {
            //item = item.Insert(item.Length - 1, " ");
            Debug.Log(item);
            Debug.Log(item.Insert(item.Length - 1, " "));
            List<string> items = item.Split(' ').ToList<string>();
            int size = items.ToArray().Length;
            for(int i = 1; i < size; i++)
            {
                items.Insert(2 * i - 1, "  ");
            }
            string lastWord = items.Last<string>();
            string end = lastWord.Substring(lastWord.Length - 1);
            items.RemoveAt(items.ToArray().Length - 1);
            items.Add(lastWord.Substring(0, lastWord.Length - 1));
            items.Add("  ");
            items.Add(end);
            setItems(items);
        }
    }
}
