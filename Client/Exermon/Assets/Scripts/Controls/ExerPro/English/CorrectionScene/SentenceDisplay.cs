using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Common.Controls.ItemDisplays;
using UnityEngine;

namespace UI.ExerPro.EnglishPro.CorrectionScene.Controls
{

    /// <summary>
    /// 句子
    /// </summary
    public class SentenceDisplay :
        SelectableContainerDisplay<string>,
        IItemDisplay<string>
    {

        /// <summary>
        /// 单词储存池
        /// </summary>
        List<GameObject> words;

        public string getItem()
        {
            return "";
        }

        public void setItem(string item, bool force = false)
        {

        }

        /// <summary>
        /// 子节点创建回调
        /// </summary>
        /// <param name="sub"></param>
        /// <param name="index"></param>
        protected override void onSubViewCreated(SelectableItemDisplay<string> sub, int index)
        {
            base.onSubViewCreated(sub, index);

        }
        public void startView(string item)
        {

        }
    }
}
