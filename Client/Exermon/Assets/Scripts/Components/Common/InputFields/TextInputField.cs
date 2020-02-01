using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 文本输入域
/// </summary>
public class TextInputField : BaseView {

    /// <summary>
    /// 检查函数类型
    /// </summary>
    public delegate string checkFunc(string value);

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public InputField inputField;
    public Text content, explainer, placeholder;
    public GameObject correct, wrong;

    /// <summary>
    /// 内部变量声明
    /// </summary>
    public checkFunc check { get; set; }

    #region 初始化

    /// <summary>
    /// 初次打开时初始化（子类中重载）
    /// </summary>
    protected override void initializeOnce() {
        /*
        inputField = SceneUtils.find<InputField>(transform, "Enter");
        content = SceneUtils.find<Text>(transform, "Enter/Text");
        explainer = SceneUtils.find<Text>(transform, "Explainer");
        correct = SceneUtils.find(transform, "Status/Correct");
        wrong = SceneUtils.find(transform, "Status/Wrong");
        */
        Debug.Log("inputField = "+ inputField);
        if (inputField) inputField.onEndEdit.AddListener(onContentChange);
    }
    
    #endregion
    
    #region 监控输入

    /// <summary>
    /// 内容变化回调
    /// </summary>
    public void onContentChange(string _) {
        Debug.Log("onContentChange");
        doCheck();
    }

    /// <summary>
    /// 执行校验
    /// </summary>
    /// <param name="display">判断后是否显示校验信息</param>
    public string doCheck(bool display = true) {
        var text = getText();
        var res = (check != null ? check.Invoke(text) : "");
        if (display) displayCheckResult(res);
        return res;
    }

    #endregion

    #region 状态控制

    /// <summary>
    /// 获取输入文本内容
    /// </summary>
    /// <returns>内容</returns>
    public string getText() {
        return inputField ? inputField.text : "";
    }

    /// <summary>
    /// 设置输入文本内容
    /// </summary>
    /// <param name="text">文本内容</param>
    /// <param name="update">是否更新控件</param>
    public void setText(string text, bool update=true) {
        if (!inputField) return;
        inputField.text = text;
        if(update) onContentChange(text);
    }

    /// <summary>
    /// 判断是否正确
    /// </summary>
    /// <param name="display">判断后是否显示校验信息</param>
    /// <returns>是否正确</returns>
    public bool isCorrect(bool display = true) {
        return doCheck(display) == "";
    }

    /// <summary>
    /// 显示校验结果
    /// </summary>
    /// <param name="res">校验结果</param>
    public void displayCheckResult(string res) {
        setStatus(res == ""); setExplainerText(res);
    }

    /// <summary>
    /// 设置提示文本
    /// </summary>
    /// <param name="text">提示文本</param>
    public void setExplainerText(string text) {
        if (explainer == null) return;
        explainer.text = text;
    }
    
    #region 状态控制

    /// <summary>
    /// 设置状态
    /// </summary>
    /// <param name="corr">正误</param>
    public void setStatus(bool corr) {
        if (corr) showCorrect();
        else showWrong();
    }

    /// <summary>
    /// 清空状态
    /// </summary>
    public void clearStatus() {
        if (correct != null) correct.SetActive(false);
        if (wrong != null) wrong.SetActive(false);
    }

    /// <summary>
    /// 显示正确
    /// </summary>
    public void showCorrect() {
        if (correct != null) correct.SetActive(true);
        if (wrong != null) wrong.SetActive(false);
    }

    /// <summary>
    /// 显示正错误
    /// </summary>
    public void showWrong() {
        if (correct != null) correct.SetActive(false);
        if (wrong != null) wrong.SetActive(true);
    }

    #endregion
    
    /// <summary>
    /// 清空状态
    /// </summary>
    public override void clear() {
        setText("", false);
        setExplainerText("");
        clearStatus();
    }

    #endregion
}
