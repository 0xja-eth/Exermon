using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 文本输入域
/// </summary>
public class TextInputField : BaseInputField<string> {
    
    /// <summary>
    /// 外部组件设置
    /// </summary>
    public InputField inputField;
    public Text content, placeholder;

    #region 初始化

    /// <summary>
    /// 初次打开时初始化（子类中重载）
    /// </summary>
    protected override void initializeOnce() {
        inputField?.onEndEdit.AddListener((text) => {
            value = text;
            onValueChanged();
        });
    }

    #endregion

    #region 界面绘制

    /// <summary>
    /// 绘制值
    /// </summary>
    /// <param name="text">值</param>
    protected override void drawValue(string text) {
        inputField.text = text;
    }
    
    #endregion
}
