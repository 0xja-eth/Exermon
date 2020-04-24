
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
        /// 返回提示文本
        /// </summary>
        const string BackAlertText = "确定结束刷题吗？若是则会自动提交本题目并进行结算，结算后可返回主界面。";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public QuestionWindow questionWindow;
        public ResultWindow resultWindow;
        public DetailWindow detailWindow;
        public ReportWindow reportWindow;

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

        #region 流程控制

        /// <summary>
        /// 打开详情窗口
        /// </summary>
        public void openDetail() {
            // detailWindow.startWindow();
        }

        /// <summary>
        /// 打开反馈窗口
        /// </summary>
        public void openReport() {
            reportWindow.startWindow();
        }

        /// <summary>
        /// 返回
        /// </summary>
        public void back() {
            gameSys.requestAlert(BackAlertText,
                Common.Windows.AlertWindow.Type.YesOrNo,
                onTerminateExercise);
        }

        #endregion

        #region 回调控制

        /// <summary>
        /// 选择结束刷题
        /// </summary>
        void onTerminateExercise() {
            questionWindow.terminate();
        }

        /// <summary>
        /// 刷题结束回调
        /// </summary>
        public void onExerciseTerminated() {
            resultWindow.startWindow();
        }

        /// <summary>
        /// 关闭上层窗口
        /// </summary>
        public void onUpperWindowBack() {
            resultWindow.terminateWindow();
            detailWindow.terminateWindow();
            reportWindow.terminateWindow();
        }

        #endregion

    }
}