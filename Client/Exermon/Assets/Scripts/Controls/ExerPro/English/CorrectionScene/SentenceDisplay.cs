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
    public class SentenceDisplay : ItemDisplay<string>
    {


        /// <summary>
        /// 单词储存池
        /// </summary>
        List<GameObject> words;

        /// <summary>
        /// 开启视图
        /// </summary>
        /// <param name="item"></param>
        public override void startView(string item)
        {
            base.startView(item);
        }

    }
}
