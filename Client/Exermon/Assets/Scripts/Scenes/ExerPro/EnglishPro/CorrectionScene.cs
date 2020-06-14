﻿
using Core.UI;
using Core.Systems;
using UI.ExerPro.EnglishPro.CorrectionScene.Controls;
using System.Collections.Generic;
using UnityEngine;
using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;
using static ExerPro.EnglishModule.Data.CorrectionQuestion;
using UnityEngine.UI;
using UI.CorrectionScene.Windows;
using Core.UI.Utils;
using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.CorrectionScene {

    /// <summary>
    /// 场景
    /// </summary>
    public class CorrectionScene : BaseScene {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public ArticleDisplay articleDisplay;
        public Text changedBeforeValue;
        public GameObject windowObj;
        public CorrectionWindow correctionWindow;
        int index = 0;
        /// <summary>
        /// 外部系统设置
        /// </summary>
        EnglishService engSer;

        #region 初始化

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            engSer = EnglishService.get();
            correctionWindow = SceneUtils.get<CorrectionWindow>(windowObj);

        }

        /// <summary>
        /// 场景名
        /// </summary>
        /// <returns>场景名</returns>
        public override SceneSystem.Scene sceneIndex() {
            return SceneSystem.Scene.EnglishProCorrectionScene;
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected override void start() {
            engSer.generateQuestions<CorrectionQuestion>(1, (res) =>
            {
                articleDisplay.startView(res[0]);
            });
            base.start();
        }

        #endregion

        public void onWordSelected(SentenceContainer container, string word) {
            //清除其他句子选择
            foreach (ItemDisplay<string> item in articleDisplay.getSubViews()) {
                SentenceContainer each = SceneUtils.get<SentenceContainer>(item.gameObject);
                if (each == container || each.getSelectedIndex() == -1)
                    continue;
                each.deselect();
            }
            correctionWindow.startView();
            changedBeforeValue.text = word;
            correctionWindow.currentSenContainer = container;
        }

        public void onWordDeselected() {
            correctionWindow.terminateView();
        }

        public void onSubmit() {
            engSer.exitNode(true);
        }
    }


}

