using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 雷达图控件
/// </summary>
public class RadarDiagram : BaseView {

    /// <summary>
    /// 移动常量设定
    /// </summary>
    protected const float MoveSpeed = 0.1f;
    protected const float StopMoveDist = 0.015f;

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public PolygonImage polygonBackground;
    public PolygonImage polygonImage;
    public Text[] edgeTexts;

    /// <summary>
    /// 内部变量声明
    /// </summary>
    List<float> targetValues = new List<float>();
    int weightCount;

    #region 初始化

    /// <summary>
    /// 每次打开时初始化（子类中重载）
    /// </summary>
    protected override void initializeOnce() {
        base.initializeOnce();
        setWeightCount(edgeTexts.Length);
    }

    #endregion

    #region 数据设置

    /// <summary>
    /// 启动视窗
    /// </summary>
    public void startView(List<float> values) {
        base.startView(); setValues(values);
    }

    /// <summary>
    /// 设置权重边数
    /// </summary>
    /// <param name="count">数量</param>
    public void setWeightCount(int cnt) {
        polygonBackground.setWeightCount(weightCount = cnt);
        polygonImage.setWeightCount(weightCount = cnt);
        targetValues = polygonImage.weights;
    }

    /// <summary>
    /// 设置权重值（有动画）
    /// </summary>
    /// <param name="values">权重值</param>
    /// <param name="force">强制（无动画）</param>
    public void setValues(List<float> values, bool force = false) {
        targetValues = values;
        if (force) polygonImage.setWeights(values);
    }

    /// <summary>
    /// 设置单个权重值
    /// </summary>
    /// <param name="index">索引</param>
    /// <param name="value">值</param>
    /// <param name="force">强制（无动画）</param>
    public void setValue(int index, float value, bool force = false) {
        if (index < targetValues.Count) targetValues[index] = value;
        if (force) polygonImage.setWeight(index, value);
    }

    /// <summary>
    /// 设置名称
    /// </summary>
    /// <param name="names">名称</param>
    public void setNames(List<string> names) {
        for (int i = 0; i < edgeTexts.Length; i++)
            edgeTexts[i].text = names[i];
    }

    #endregion

    #region 动画控制

    /// <summary>
    /// 更新
    /// </summary>
    protected override void update() {
        base.update();
        if (isAnimationPlaying())
            updateMoveAnimation();
        else resetAnimation();
    }

    /// <summary>
    /// 更新移动动画
    /// </summary>
    void updateMoveAnimation() {
        for (int i = 0; i < weightCount; i++)
            polygonImage.addWeight(i, (targetValues[i] -
                polygonImage.getWeight(i)) * MoveSpeed);
    }

    /// <summary>
    /// 动画是否正在播放
    /// </summary>
    /// <returns></returns>
    public bool isAnimationPlaying() {
        return !isAnimationStopping();
    }

    /// <summary>
    /// 动画是否将要停止
    /// </summary>
    /// <returns></returns>
    bool isAnimationStopping() {
        float delta = 0;
        for (int i = 0; i < weightCount; i++)
            delta += Mathf.Abs(targetValues[i] - polygonImage.getWeight(i));
        return delta < StopMoveDist;
    }

    /// <summary>
    /// 重置动画
    /// </summary>
    /// <returns></returns>
    public void resetAnimation() {
        polygonImage.setWeights(targetValues);
    }

    #endregion

    #region 界面控制

    /// <summary>
    /// 清除视窗
    /// </summary>
    public override void clear() {
        clearWeights();
        clearNames();
    }

    /// <summary>
    /// 清空权重
    /// </summary>
    public void clearWeights() {
        int max = polygonImage.getEdgeCount();
        for (int i = 0; i < max; i++) setValue(i, 0, true);
    }

    /// <summary>
    /// 清空名称
    /// </summary>
    public void clearNames() {
        for (int i = 0; i < edgeTexts.Length; i++)
            edgeTexts[i].text = "";
    }

    #endregion
}
