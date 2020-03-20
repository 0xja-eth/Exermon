using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using Core.UI;
using Core.UI.Utils;

/// <summary>
/// 通用窗口
/// </summary>
namespace UI.Common.Windows {

    /// <summary>
    /// 提示窗口层
    /// </summary>
    public class AlertWindow : BaseWindow {

        /// <summary>
        /// 提示框类型
        /// </summary>
        public enum Type {
            Notice, // 通知，小窗口且无遮罩
            YesOrNo, // 确认/取消提示框
            RetryOrNo // 重试/取消提示框
        }

        /// <summary>
        /// 常量定义
        /// </summary>
        public const float DefaultDuration = 5; // 默认显示时长（秒）

        /// <summary>
        /// 文本常量定义
        /// </summary>
        /*
        public const string YesText = "是";
        public const string NoText = "否";
        public const string OKText = "确认";
        public const string RetryText = "重试";
        public const string CancelText = "取消";
        public const string CloseText = "关闭";
        public const string BackText = "返回";
        public const string KnowText = "知道了";

        /// <summary>
        /// 按键模式定义
        /// </summary>
        public static readonly string[] YesOrNo = { null, YesText, NoText };
        public static readonly string[] Close = { null, CloseText };
        public static readonly string[] Retry = { null, RetryText };
        public static readonly string[] RetryOrCancel = { null, RetryText, CancelText };
        public static readonly string[] RetryOrClose = { null, RetryText, CloseText };
        public static readonly string[] OKOrBack = { null, OKText, BackText };
        public static readonly string[] OK = { null, OKText };
        public static readonly string[] Know = { null, KnowText };
        */

        /// <summary>
        /// 界面常量定义
        /// </summary>
        const int AlertTextPaddingTop = 36;
        const int TextButtonSpacing = 18;
        const int OkButtonPaddingButtom = 36;

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Text alertText; // 提示文本

        public GameObject yesButton, retryButton, noButton; // 三个按钮

        /// <summary>
        /// 外部变量设置
        /// </summary>
        public float bigWidth, smallWidth; // 大尺寸与小尺寸

        /// <summary>
        /// 内部变量声明
        /// </summary>
        Type type;
        float duration;
        string text; // 提示文本
        UnityAction onOK, onCancel; // 选项回调

        bool enableBackground = false; // 是否显示背景

        #region 初始化

        /// <summary>
        /// 初次打开时初始化（子类中重载）
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(background);
        }

        #endregion

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            updateDuration();
        }

        /// <summary>
        /// 更新持续时间
        /// </summary>
        void updateDuration() {
            if (type == Type.Notice && duration > 0) {
                duration -= Time.deltaTime;
                if (duration <= 0) terminateWindow();
            }
        }

        #endregion

        #region 启动/结束控制

        /// <summary>
        /// 启动窗口
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="type">类型</param>
        /// <param name="onOK">确认回调</param>
        /// <param name="onCancel">取消回调</param>
        /// <param name="duration">持续时间（为 0 则永久）</param>
        public void startWindow(string text, Type type = Type.Notice,
            UnityAction onOK = null, UnityAction onCancel = null,
            float duration = DefaultDuration) {
            setup(text, type, onOK, onCancel, duration);
            startWindow();
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 判断是否处于可视状态
        /// </summary>
        /// <returns>是否可视状态</returns>
        protected override bool isBackgroundVisible() {
            return enableBackground && base.isBackgroundVisible();
        }

        /// <summary>
        /// 配置Alert
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="btns">选项</param>
        /// <param name="actions">选项回调</param>
        void setup(string text, Type type = Type.Notice,
            UnityAction onOK = null, UnityAction onCancel = null,
            float duration = DefaultDuration) {
            this.text = text; this.type = type;
            this.onOK = onOK; this.onCancel = onCancel;
            this.duration = duration;
            setupType(type);
        }

        /// <summary>
        /// 配置类型
        /// </summary>
        void setupType(Type type) {
            clearButtons();
            switch (type) {
                case Type.Notice:
                    adjustToSmallWindow();
                    break;
                case Type.YesOrNo:
                    adjustToBigWindow();
                    if (yesButton) yesButton.SetActive(true);
                    if (noButton) noButton.SetActive(true);
                    break;
                case Type.RetryOrNo:
                    adjustToBigWindow();
                    if (retryButton) retryButton.SetActive(true);
                    if (noButton) noButton.SetActive(true);
                    break;
            }
        }

        /// <summary>
        /// 设置选项回调
        /// </summary>
        void setButtonCallback(GameObject btnObj, UnityAction action) {
            Button button = SceneUtils.button(btnObj);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action);
            button.interactable = true;
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 设置显示文本
        /// </summary>
        /// <param name="text">文本</param>
        void setText(string text) {
            alertText.text = text;
        }

        /// <summary>
        /// 转换为大窗口
        /// </summary>
        void adjustToBigWindow() {
            enableBackground = true;
            SceneUtils.setRectWidth(transform as RectTransform, bigWidth);
        }

        /// <summary>
        /// 转换为小窗口
        /// </summary>
        void adjustToSmallWindow() {
            enableBackground = false;
            SceneUtils.setRectWidth(transform as RectTransform, smallWidth);
        }

        /// <summary>
        /// 刷新窗口
        /// </summary>
        protected override void refresh() {
            base.refresh();
            setText(text);
        }

        /// <summary>
        /// 清除窗口
        /// </summary>
        protected override void clear() {
            base.clear();
            setText("");
            clearButtons();
        }

        /// <summary>
        /// 重置按钮
        /// </summary>
        void clearButtons() {
            if (yesButton) yesButton.SetActive(false);
            if (retryButton) retryButton.SetActive(false);
            if (noButton) noButton.SetActive(false);
        }

        #endregion

        #region 事件控制

        /// <summary>
        /// 当确认按钮按下（包括重试按钮）
        /// </summary>
        public void onYesButtonClick() {
            onOK?.Invoke();
            terminateWindow();
        }

        /// <summary>
        /// 当取消按钮按下
        /// </summary>
        public void onNoButtonClick() {
            onCancel?.Invoke();
            terminateWindow();
        }

        /// <summary>
        /// 当背景按下
        /// </summary>
        public void onBackgroundClick() {
            if (type == Type.Notice) onNoButtonClick();
        }

        #endregion

    }

}
