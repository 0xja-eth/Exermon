using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// 提示窗口层
/// </summary>
public class AlertWindow : BaseWindow {

    /// <summary>
    /// 最大按钮数
    /// </summary>
    public const int MaxButtons = 3;

    /// <summary>
    /// 文本常量定义
    /// </summary>
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

    public GameObject okButton, leftButton, rightButton; // 三个按钮

    /// <summary>
    /// 内部变量声明
    /// </summary>
    GameObject[] buttonObjs; // 按钮对象

    string text; // 提示文本
    string[] btns; // 选项文本
    UnityAction[] actions; // 选项回调

    #region 初始化

    /// <summary>
    /// 初次打开时初始化（子类中重载）
    /// </summary>
    protected override void initializeOnce() {
        base.initializeOnce();
        buttonObjs = buttonObjs ?? new GameObject[MaxButtons] 
            { leftButton, okButton, rightButton };
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(background);
    }

    #endregion

    #region 更新控制

    /// <summary>
    /// 更新
    /// </summary>
    protected override void update() {
        base.update(); updateLayout();
    }

    /// <summary>
    /// 更新布局
    /// </summary>
    void updateLayout() {
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
    }

    #endregion

    #region 启动/结束控制

    /// <summary>
    /// 启动窗口
    /// </summary>
    /// <param name="text">提示文本</param>
    /// <param name="btns">选项文本</param>
    /// <param name="actions">选项回调</param>
    public void startWindow(string text, string[] btns = null, UnityAction[] actions = null) {
        setup(text, btns, actions);
        startWindow(); refresh();
    }

    /// <summary>
    /// 配置Alert
    /// </summary>
    /// <param name="text">文本</param>
    /// <param name="btns">选项</param>
    /// <param name="actions">选项回调</param>
    void setup(string text, string[] btns = null, UnityAction[] actions = null) {
        this.text = text; this.btns = btns ?? OK;
        this.actions = actions ?? new UnityAction[2] { null, terminateWindow };
    }

    /// <summary>
    /// 结束窗口（重载）
    /// </summary>
    public override void terminateWindow() {
        base.terminateWindow(); clearButtons();
    }

    #endregion

    #region 界面控制

    /// <summary>
    /// 刷新窗口
    /// </summary>
    protected override void refresh() {
        base.refresh();
        clear(); setText(text);
        drawButtons();
    }

    /// <summary>
    /// 设置显示文本
    /// </summary>
    /// <param name="text">文本</param>
    void setText(string text) {
        alertText.text = text;
    }

    /// <summary>
    /// 绘制全部选项
    /// </summary>
    void drawButtons() {
        for (int i = 0; i < btns.Length; i++)
            drawButton(i);
    }

    /// <summary>
    /// 绘制单个选项
    /// </summary>
    /// <param name="btn">选项对象</param>
    void drawButton(int index) {
        string txt = btns[index];
        if (txt == null) return;
        GameObject btn = buttonObjs[index];
        Button button = SceneUtils.button(btn);
        Text label = SceneUtils.find<Text>(btn, "Text");
        UnityAction act = actions[index];
        setButtonCallback(button, act);
        label.text = txt;
        btn.SetActive(true);
    }

    /// <summary>
    /// 设置选项回调
    /// </summary>
    void setButtonCallback(Button button, UnityAction act) {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {
            if (act != null) act.Invoke();
            terminateWindow();
        });
        button.interactable = true;
    }

    /// <summary>
    /// 清除窗口
    /// </summary>
    protected override void clear() {
        base.clear(); setText(""); clearButtons();
    }

    /// <summary>
    /// 重置按钮
    /// </summary>
    void clearButtons() {
        foreach (GameObject obj in buttonObjs)
            obj.SetActive(false);
    }

    #endregion


}
