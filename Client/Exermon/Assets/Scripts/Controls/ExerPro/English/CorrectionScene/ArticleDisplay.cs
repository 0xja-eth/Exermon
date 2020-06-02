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
    public class ArticleDisplay : ContainerDisplay<string>,
        IItemDisplay<CorrectionQuestion>
    {

        public List<List<string>> article;
        CorrectionQuestion question;

        /// <summary>
        /// 句子储存池
        /// </summary>
        List<GameObject> sentences;


        public CorrectionQuestion getItem()
        {
            return question;
        }

        public void setItem(CorrectionQuestion item, bool force = false)
        {
            //string[] items1 = { "aaa", "bbb" };
            question = item; setItems(item.sentences());
            //base.setItems(sentences);
            //base.setItems(items2);
            
        }

        protected override void onSubViewCreated(ItemDisplay<string> sub, int index)
        {
            base.onSubViewCreated(sub, index);
            SceneUtils.get<SentenceContainer>(subViews[index].gameObject).setItem(items[index]);
            Debug.Log("createsubview");
            Debug.Log(items[index]);

        }
        //public void test()
        //{
        //    base.createSubViews();
        //}


        /// <summary>
        /// 开启视图
        /// </summary>
        /// <param name="item"></param>
        public void startView(CorrectionQuestion item)
        {
            setItem(item, true);
        }
    }
}
