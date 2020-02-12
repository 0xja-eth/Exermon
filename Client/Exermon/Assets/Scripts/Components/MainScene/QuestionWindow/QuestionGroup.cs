using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 题目组
/// </summary>
public class QuestionGroup : BaseView {

    public QuestionText[] texts;
    public RectTransform[] layouts;

    /// <summary>
    /// 重绘
    /// </summary>
    public void rebuild() {
        foreach(var t in texts) 
            t.SetAllDirty();
        foreach(var l in layouts)
            LayoutRebuilder.ForceRebuildLayoutImmediate(l);
    }
}
