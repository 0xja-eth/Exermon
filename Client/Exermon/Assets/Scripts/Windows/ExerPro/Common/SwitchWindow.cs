using UnityEngine;
using Core.UI;
using Core.UI.Utils;
using UI.ExerPro.EnglishPro.PlotScene.Controls;
using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;
using Core.Data.Loaders;
using System.IO;
using Core.Systems;
using GameModule.Services;
using UI.ExerPro.EnglishPro.Common.Controls;
using UnityEngine.UI;
using UnityEngine.Events;

namespace UI.ExerPro.EnglishPro.Common.Windows {
    /// <summary>
    /// 剧情窗口
    /// </summary>
    public class SwitchWindow : BaseWindow {
        /// <summary>
        /// 常量
        /// </summary>
        const string scoreFormat = "当前积分：{0}";
        const string passString = "恭喜通过该阶段";
        const string dieString = "败北，你已死亡";

        public enum Type {
            Boss, Die,
        }

        /// <summary>
        /// 外部变量
        /// </summary>
        public Text title;
        public Text score;
        public Button confirm;

        /// <summary>
        /// 外部系统
        /// </summary>
        CalcService calServ;
        EnglishService engServ;
        ExerProRecord record;

        /// <summary>
        /// 内部变量
        /// </summary>
        ExerProMapNode node { get; set; } = null;
        UnityAction terminateCallback = null;

        #region 初始化
        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            calServ = CalcService.get();
            engServ = EnglishService.get();
            record = engServ.record;
        }

        #endregion


        #region 窗口控制
        /// <summary>
        /// 关卡切换请调用该接口
        /// </summary>
        /// <param name="type">切换类型</param>
        /// <param name="terminateAction">退出回调</param>
        public void startWindow(Type type = Type.Die, UnityAction terminateAction = null) {
            base.startWindow();
            node = engServ.record.currentNode();
            terminateCallback = terminateAction;

            switch (type) {
                case Type.Boss:
                    title.text = passString;
                    break;
                case Type.Die:
                    title.text = dieString;
                    break;
            }
            configureMarks();
        }

        /// <summary>
        /// 配置分数
        /// </summary>
        void configureMarks() {
            configureScore();
        }

        /// <summary>
        /// 配置积分
        /// </summary>
        void configureScore() {
            var actor = engServ.record.actor;
            int score = CalcService.RewardGenerator.generateScore(record);
            this.score.text = string.Format(scoreFormat, score);
        }

        #endregion

        #region 回调函数
        /// <summary>
        /// 完成退出
        /// </summary>
        void onSubmitClicked() {
            terminateWindow();
            terminateCallback?.Invoke();
        }
        #endregion
    }
}
