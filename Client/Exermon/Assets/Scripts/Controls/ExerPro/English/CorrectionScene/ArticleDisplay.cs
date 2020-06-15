﻿using Core.UI.Utils;
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

        public void revert() {
            int index = 0;
            foreach (ItemDisplay<string> item in getSubViews()) {
                SceneUtils.get<SentenceContainer>(item.gameObject).clearItems();
                SceneUtils.get<SentenceContainer>(item.gameObject).setItem(items[index++]);
            }
            startView(question);
        }

    }
}