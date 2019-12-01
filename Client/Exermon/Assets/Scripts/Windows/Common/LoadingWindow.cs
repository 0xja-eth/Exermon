using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Events;

/// <summary>
/// 加载窗口
/// </summary>
public class LoadingWindow : BaseWindow {

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public Transform rotateBar; // 旋转圈
    public Transform progressBar; // 下方进度条
    public Text progressText, loadingText; // 进度文本，提示文本
    public GameObject textArea; // 提示框

    public float rotateSpeed = 300; // 转圈速度

    /// <summary>
    /// 内部变量声明
    /// </summary>
    string text; // 提示文本

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
        base.update(); updateRotateBar();
    }

    /// <summary>
    /// 更新旋转圈
    /// </summary>
    void updateRotateBar() {
        rotateBar.Rotate(0, 0, rotateSpeed * Time.deltaTime); // rotation on the Z axis.
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

    #region 界面控制

    /// <summary>
    /// 刷新窗口（clear后重绘）
    /// </summary>
    public override void refresh() {
        base.refresh();
        gameObject.SetActive(true);
        textArea.SetActive(text.Length > 0);
        loadingText.text = text;
    }

    /// <summary>
    /// 设置加载进度
    /// </summary>
    /// <param name="rate"></param>
    public void setProgress(float rate) {
        var _rate = Mathf.Clamp01(rate);
        progressBar.localScale = new Vector3(_rate, 0.9f, 1);
        if (rate >= 0) progressText.text = SceneUtils.double2Perc(rate);
        else progressText.text = "";
    }

    /// <summary>
    /// 清除窗口
    /// </summary>
    public override void clear() {
        base.clear();
        setProgress(-1);
        loadingText.text = "";
    }

    #endregion

}
