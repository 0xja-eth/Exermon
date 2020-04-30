using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Events;

using Core.UI;
using Core.UI.Utils;

namespace UI.Common.Windows {

    /// <summary>
    /// 加载窗口
    /// </summary>
    public class LoadingWindow : BaseWindow {

        /// <summary>
        /// 常量定义
        /// </summary>
        const string ProgressTextFormat = "当前：{0}";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Text progressText, loadingText; // 进度文本，提示文本

        /// <summary>
        /// 内部变量声明
        /// </summary>
        string text; // 提示文本
        double progress = -1; // 进度

        #region 初始化

        /// <summary>
        /// 初次打开时初始化（子类中重载）
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            //DontDestroyOnLoad(gameObject);
            //DontDestroyOnLoad(background);
        }

        #endregion

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            updateProgress();
        }

        /// <summary>
        /// 更新进度
        /// </summary>
        void updateProgress() {
            drawProgress(progress);            
        }

        #endregion

        #region 启动/结束控制

        /// <summary>
        /// 启动窗口
        /// </summary>
        /// <param name="text">提示文本</param>
        public void startWindow(string text) {
            this.text = text; base.startWindow();
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 设置进度
        /// </summary>
        /// <param name="progress"></param>
        public void setProgress(double progress) {
            this.progress = progress;
        }

        /// <summary>
        /// 清空进度
        /// </summary>
        public void clearProgress() {
            setProgress(-1);  
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 绘制进度
        /// </summary>
        void drawProgress(double progress) {
            if (progress >= 0) {
                progress = Mathf.Clamp01((float)progress);
                var txt = SceneUtils.double2RoundedPerc(progress);
                progressText.text = string.Format(ProgressTextFormat, txt);
            } else progressText.text = "";
        }

        /// <summary>
        /// 刷新窗口
        /// </summary>
        protected override void refresh() {
            base.refresh();
            loadingText.text = text;
        }

        /// <summary>
        /// 清除窗口
        /// </summary>
        protected override void clear() {
            base.clear();
            clearProgress();
            loadingText.text = "";
            progressText.text = "";
        }

        #endregion

    }
}