using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine.UI;
using UnityEngine.Events;

using Core.Systems;
using Core.UI;

using QuestionModule.Services;

using GameModule.Services;
using PlayerModule.Services;

using RecordModule.Data;
using RecordModule.Services;

using UI.Common.Controls.InputFields;

namespace UI.Common.Controls.QuestionDisplay {

    /// <summary>
    /// 反馈窗口
    /// </summary>
    public class QuestionReportView : BaseView {

        /// <summary>
        /// 常量定义
        /// </summary>
        const string PushReportSuccessText = "成功提交反馈！我们将会尽快处理！谢谢！";
        const string ReportTypeRegText = @"(.+)（(.+)）";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public QuestionDisplay questionDisplay;

        public DropdownField typeInput;
        public InputField descriptionInput;
        public Text placeHolder;

        /// <summary>
        /// 成功回调
        /// </summary>
        UnityAction onSuccess = null;

        /// <summary>
        /// 内部变量声明
        /// </summary>
        ExerciseRecord record;

        Dictionary<string, string> reportTypes = new Dictionary<string, string>();

        /// <summary>
        /// 外部系统
        /// </summary>
        GameSystem gameSys;
        DataService dataSer;
        PlayerService playerSer;
        RecordService recordSer;
        QuestionService quesSer;

        #region 初始化
        
        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            gameSys = GameSystem.get();
            dataSer = DataService.get();
            recordSer = RecordService.get();
            playerSer = PlayerService.get();
            quesSer = QuestionService.get();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            setupReportTypes();
            configureControls();
        }

        /// <summary>
        /// 配置控件
        /// </summary>
        void configureControls() {
            setupReportTypes();
            var types = dataSer.staticData.configure.quesReportTypes;
            typeInput.configure(types);
            typeInput.setIndex(0);
            typeInput.onChanged = onTypeChanged;
        }

        /// <summary>
        /// 配置反馈类型
        /// </summary>
        void setupReportTypes() {
            string name, desc;
            reportTypes.Clear();
            var types = dataSer.staticData.configure.quesReportTypeDescs;
            foreach (var type in types) {
                splitTypeNameAndDesc(type.Item2, out name, out desc);
                reportTypes.Add(name, desc);
            }
        }

        /// <summary>
        /// 分解类型为名称和描述
        /// </summary>
        /// <param name="text">原始文本</param>
        /// <param name="name">名称</param>
        /// <param name="desc">描述</param>
        void splitTypeNameAndDesc(string text, out string name, out string desc) {
            var reg = new Regex(ReportTypeRegText);
            var matches = reg.Match(text);
            name = matches.Groups[1].Value;
            desc = matches.Groups[2].Value;
        }

        #endregion

        #region 流程控制
        
        /// <summary>
        /// 提交
        /// </summary>
        public void push() {
            var question = questionDisplay.getItem();
            var type = typeInput.getValueId();
            var desc = descriptionInput.text;
            quesSer.pushReport(question.getID(),
                type, desc, onPushSuccess);
        }

        #endregion

        #region 回调控制

        /// <summary>
        /// 反馈类型改变回调
        /// </summary>
        void onTypeChanged(Tuple<int, string> item) {
            placeHolder.text = reportTypes[item.Item2];
        }

        /// <summary>
        /// 成功提交反馈
        /// </summary>
        void onPushSuccess() {
            gameSys.requestAlert(PushReportSuccessText);
            onSuccess?.Invoke();
        }

        #endregion

    }
}