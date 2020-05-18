
using Core.UI;

using RecordModule.Services;

using QuestionModule.Services;

using UI.Common.Controls.ParamDisplays;

namespace UI.Common.Controls.QuestionDisplay {

    using Question = QuestionModule.Data.Question;

    /// <summary>
    /// 详情窗口
    /// </summary>
    public class QuestionDetailView : BaseView {
        
        /// <summary>
        /// 外部组件设置
        /// </summary>
        public QuestionDisplay questionDisplay;

        public StarsDisplay quesStar;
        public MultParamsDisplay detail;
        public TimeParamDisplay firstTime, avgTime, allAvgTime;
        
        /// <summary>
        /// 内部变量声明
        /// </summary>
        Question question;

        /// <summary>
        /// 外部系统
        /// </summary>
        RecordService recordSer;
        QuestionService quesSer;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeEvery() {
            base.initializeEvery();
            configureControls();
        }
        
        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            recordSer = RecordService.get();
            quesSer = QuestionService.get();
        }

        /// <summary>
        /// 配置控件
        /// </summary>
        void configureControls() {
            question = questionDisplay.getItem();
            var stdTime = question.star().stdTime * 1000;
            firstTime?.configure(stdTime);
            avgTime?.configure(stdTime);
            allAvgTime?.configure(stdTime);
        }

        #endregion

        #region 启动控制

        /// <summary>
        /// 显示窗口
        /// </summary>
        protected override void showView() {
            quesSer.loadQuestionDetail(
                questionDisplay.getItem(), base.showView);
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
            quesStar?.setValue(question.starId);
            detail.setValue(question, "detail");
        }
        
        #endregion
    }
}