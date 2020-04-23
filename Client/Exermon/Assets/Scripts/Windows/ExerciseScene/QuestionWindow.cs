
using UnityEngine.UI;

using Core.UI;
using Core.UI.Utils;

using QuestionModule.Data;

using RecordModule.Data;
using RecordModule.Services;
using UI.Common.Controls.QuestionDisplay;

/// <summary>
/// 对战解析场景窗口
/// </summary>
namespace UI.ExerciseScene.Windows {

    /// <summary>
    /// 题目窗口
    /// </summary>
    public class QuestionWindow : BaseWindow {

        /// <summary>
        /// 常量定义
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public QuestionDisplay questionDisplay;
        
        /// <summary>
        /// 场景组件引用
        /// </summary>
        protected ExerciseScene scene;

        /// <summary>
        /// 内部变量声明
        /// </summary>
        int index = 0; // 回合索引

        ExerciseRecord record;
        Question question;

        /// <summary>
        /// 外部系统
        /// </summary>
        RecordService recordSer;

        #region 初始化

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
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            record = recordSer.exerciseRecord;
        }

        #endregion

        #region 启动/结束窗口
        
        #endregion

        #region 数据控制
        

        #endregion

        #region 界面控制
        
        #endregion

        #region 流程控制

        /// <summary>
        /// 下一题
        /// </summary>
        public void next() {
        }

        /// <summary>
        /// 上一题
        /// </summary>
        public void prev() {
        }

        #endregion

    }
}