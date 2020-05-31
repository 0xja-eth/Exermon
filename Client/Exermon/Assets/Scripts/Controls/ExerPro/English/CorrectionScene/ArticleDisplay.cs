using Core.UI.Utils;
using ExerPro.EnglishModule.Data;
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
        IItemDisplay<CorrectionQuestion>
    {

        public List<List<string>> article;

        /// <summary>
        /// 句子储存池
        /// </summary>
        List<GameObject> sentences;



        public CorrectionQuestion getItem()
        {
            return new CorrectionQuestion();
        }

        public void setItem(CorrectionQuestion item, bool force = false)
        {
            Debug.Log("setitem");
            string[] items1 = { "aaa", "bbb" };
            base.setItems(items1);
            //base.setItems(items2);
            //var a = Instantiate(subViewPrefab, container);
            //SceneUtils.get<RectTransform>(a).SetParent(this)

        }

        public void test()
        {
            //base.createSubView(0);
            //base.createSubView(1);
        }

        public void startView(CorrectionQuestion item)
        {
            Debug.Log("article");
        }
    }
}
