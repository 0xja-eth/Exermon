using Assets.Scripts.Controls.ExerPro.English.PhraseScene;
using Core.Systems;
using Core.UI;
using ExerPro.EnglishModule.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ExerPro.EnglishModule.Data.PhraseQuestion;

namespace Assets.Scripts.Scenes.ExerPro.EnglishPro
{
    class PhraseScene : BaseScene
    {
        /// <summary>
        /// 外部组件设置
        /// </summary>
        /// 
        public OptionAreaDisplay optionAreaDisplay;

        /// <summary>
        /// 外部系统设置
        /// </summary>
        EnglishService engSer;

        #region 初始化

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems()
        {
            base.initializeSystems();
            engSer = EnglishService.get();
        }
        #endregion

        /// <summary>
        /// 场景名
        /// </summary>
        /// <returns>场景名</returns>
        public override SceneSystem.Scene sceneIndex()
        {
            return SceneSystem.Scene.EnglishProPhraseScene;
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected override void start()
        {
            base.start();
            Debug.Log(sample().word);
            optionAreaDisplay.startView(sample());
        }

        public void onSubmit()
        {
            engSer.exitNode();
        }
    }
}
