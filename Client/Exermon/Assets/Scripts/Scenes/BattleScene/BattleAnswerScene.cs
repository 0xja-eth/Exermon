
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using Core.UI;
using Core.UI.Utils;

using Core.Systems;

using PlayerModule.Services;
using SeasonModule.Services;
using BattleModule.Services;

using UI.BattleAnswerScene.Windows;
//using UI.BattleAnswerScene.Controls;

/// <summary>
/// 对战解析场景
/// </summary>
namespace UI.BattleAnswerScene {

    /// <summary>
    /// 对战解析场景
    /// </summary>
    public class BattleAnswerScene : BaseScene {
        
        /// <summary>
        /// 外部组件设置
        /// </summary>
        public QuestionWindow questionWindow;

        #region 初始化

        /// <summary>
        /// 场景名
        /// </summary>
        /// <returns>场景名</returns>
        public override SceneSystem.Scene sceneIndex() {
            return SceneSystem.Scene.BattleAnswerScene;
        }
        
        /// <summary>
        /// 开始
        /// </summary>
        protected override void start() {
            base.start();
            questionWindow.startWindow();
        }

        #endregion

    }
}