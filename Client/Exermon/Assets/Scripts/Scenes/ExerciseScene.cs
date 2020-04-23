
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
using RecordModule.Services;

using UI.ExerciseScene.Windows;
//using UI.BattleAnswerScene.Controls;

/// <summary>
/// 刷题场景
/// </summary>
namespace UI.ExerciseScene {

    /// <summary>
    /// 刷题场景
    /// </summary>
    public class ExerciseScene : BaseScene {
        
        /// <summary>
        /// 外部组件设置
        /// </summary>
        public QuestionWindow questionWindow;

        #region 初始化

        /// <summary>
        /// 场景名
        /// </summary>
        /// <returns>场景名</returns>
        public override string sceneName() {
            return SceneSystem.Scene.ExerciseScene;
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