
using UnityEngine.UI;
using UnityEngine.Events;

using Core.Systems;
using Core.UI;
using Core.UI.Utils;

using QuestionModule.Data;

using PlayerModule.Services;

using RecordModule.Data;
using RecordModule.Services;

using UI.Common.Controls.ParamDisplays;
using UI.Common.Controls.QuestionDisplay;

namespace UI.ExerciseScene.Windows {

    /// <summary>
    /// 详情窗口
    /// </summary>
    public class DetailWindow : BaseWindow {

        /// <summary>
        /// 常量定义
        /// </summary>
        /// <summary>
        /// 外部组件设置
        /// </summary>
        public QuestionDisplay questionDisplay;

        public StarsDisplay quesStar;
        public MultParamsDisplay detail;
        public TimeParamDisplay firstTime, 
            lastTime, allAvgTime;

        /// <summary>
        /// 场景组件引用
        /// </summary>
        protected ExerciseScene scene;

        /// <summary>
        /// 内部变量声明
        /// </summary>
        Question question;

        /// <summary>
        /// 外部系统
        /// </summary>
        PlayerService playerSer;
        RecordService recordSer;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeEvery() {
            base.initializeEvery();
            configureControls();
        }

        /// <summary>
        /// 初始化场景
        /// </summary>
        protected override void initializeScene() {
            scene = (ExerciseScene)SceneUtils.getSceneObject("Scene");
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            recordSer = RecordService.get();
            playerSer = PlayerService.get();
        }

        /// <summary>
        /// 配置控件
        /// </summary>
        void configureControls() {
            question = questionDisplay.getItem();
            var stdTime = question.star().stdTime * 1000;
            firstTime.configure(stdTime);
            lastTime.configure(stdTime);
            allAvgTime.configure(stdTime);
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 刷新窗口
        /// </summary>
        protected override void refresh() {
            base.refresh();
            refreshQuestion();
        }

        /// <summary>
        /// 刷新结果
        /// </summary>
        void refreshQuestion() {
            quesStar.setValue(question.starId);
            detail.setValue(question, "detail");
        }
        
        #endregion
    }
}