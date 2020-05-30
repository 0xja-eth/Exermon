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
    /// 文章
    /// </summary
    public class ArticleDisplay :
        ContainerDisplay<string>,
        IItemDisplay<string>
    {
        /// <summary>
        /// 外部组件定义
        /// </summary>
        public GameObject sentencePerfab;

        /// <summary>
        /// 句子储存池
        /// </summary>
        List<GameObject> sentences;

        public string getItem()
        {
            return "";
        }

        public void setItem(string item, bool force = false)
        {

        }

        public void startView(string item)
        {

        }
    }
}
