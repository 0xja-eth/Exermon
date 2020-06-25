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

namespace UI.ExerPro.EnglishPro.CorrectionScene.Controls {


    /// <summary>
    /// 文章
    /// </summary
    public class ArticleDisplay : ContainerDisplay<string>,
        IItemDisplay<CorrectionQuestion> {

        CorrectionQuestion question;
        public GameObject correctionWindow;
        public Text changedBeforeValue;

        public CorrectionQuestion getItem() {
            return question;
        }

        /// <summary>
        /// 设置物品
        /// </summary>
        /// <param name="item">改错题项</param>
        /// <param name="force">强制</param>
        /// <returns>null</returns>
        public void setItem(CorrectionQuestion item, bool force = false) {
            question = item; base.setItems(item.sentences());
        }


        /// <summary>
        /// 开启视图
        /// </summary>
        /// <param name="item"></param>
        public void startView(CorrectionQuestion item) {
            base.startView();
            setItem(item, true);
        }

    }
}
