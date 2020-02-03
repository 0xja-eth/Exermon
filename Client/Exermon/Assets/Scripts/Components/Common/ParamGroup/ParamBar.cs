using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 属性值条
/// </summary>
public class ParamBar : BaseView {

    #region 辅助类型定义

    /// <summary>
    /// 能转化为 ParamConfigInfo[] 的接口
    /// </summary>
    public interface ParamConfigInfoConvertable {

        /// <summary>
        /// 转化为属性信息集
        /// </summary>
        /// <returns>属性信息集</returns>
        ParamConfigInfo convertToParamInfo();
    }

    /// <summary>
    /// 能转化为 ParamValueInfo[] 的接口
    /// </summary>
    public interface ParamValueInfosConvertable {

        /// <summary>
        /// 转化为属性信息集
        /// </summary>
        /// <returns>属性信息集</returns>
        ParamValueInfo[] convertToParamInfos();
    }

    /// <summary>
    /// 属性配置信息
    /// </summary>
    public struct ParamConfigInfo {
        public string name;
        public Color? color;
        public float max;
    }

    /// <summary>
    /// 属性值信息
    /// </summary>
    public struct ParamValueInfo {
        public float value, max;
        public float? rate;
    }

    /// <summary>
    /// 属性信息
    /// </summary>
    protected struct ParamInfo {
        public string name;
        public float value, max;
        public float? rate;
        public Color? color;
    }

    #endregion

    /// <summary>
    /// 常量定义
    /// </summary>
    const float AniDuration = 0.5f;
    const string AniClipName = "Bar";

    /// <summary>
    /// 外部组设置
    /// </summary>
    public Text name, value, rate;
    public GameObject bar;

    /// <summary>
    /// 外部变量设置
    /// </summary>
    public string nameFormat = "{0}"; // 名称格式
    public string valueFormat = "{0}/{1}"; // 值格式（0为当前值，1为最大值）
    public bool animated = true; // 是否有动画效果

    /// <summary>
    /// 内部变量声明
    /// </summary>
    Image barImg;
    Animation barAni;
    RectTransform barRt;
    ParamInfo info = new ParamInfo();
    
    #region 初始化

    /// <summary>
    /// 初次打开时初始化（子类中重载）
    /// </summary>
    protected override void initializeOnce() {
        if(bar) {
            barRt = bar.transform as RectTransform;
            barImg = SceneUtils.image(bar);
            barAni = SceneUtils.ani(bar);
        }
    }

    /// <summary>
    /// 配置组件
    /// </summary>
    public void configure(string name, float max, Color? color = null) {
        base.configure();
        generateParamInfo(name, max, color);
    }

    /// <summary>
    /// 配置组件
    /// </summary>
    public void configure(ParamConfigInfo info) {
        configure(info.name, info.max, info.color);
    }

    #endregion

    #region 数据控制

    /// <summary>
    /// 生成属性信息
    /// </summary>
    /// <param name="name">属性名称</param>
    /// <param name="max">最大值</param>
    /// <param name="color">属性颜色</param>
    /// <returns>属性信息</returns>
    void generateParamInfo(string name, float max, Color? color = null) {
        info.name = name;
        info.max = max;
        info.color = color;
        clearValue();
        //changeValue(value, max, rate, true);
    }

    /// <summary>
    /// 改变值
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="max">最大值</param>
    /// <param name="rate">成长率</param>
    /// <param name="force">强制</param>
    public void setValue(float value, float? max = null,
        float? rate = null, bool force = false) {
        info.value = value; info.rate = rate;
        if (max != null) info.max = (float)max;

        onValueChanged(force);
    }
    /// <param name="info">属性信息</param>
    public void setValue(ParamValueInfo info, bool force = false) {
        setValue(info.value, info.max, info.rate, force);
    }

    /// <summary>
    /// 清除值
    /// </summary>
    public void clearValue() {
        setValue(0, rate: 0, force: true);
    }

    /// <summary>
    /// 比率
    /// </summary>
    /// <returns>比率</returns>
    public float paramRate() {
        if (info.max == 0) return 0;
        return info.value / info.max;
    }

    /// <summary>
    /// 值改变回调
    /// </summary>
    void onValueChanged(bool force) {
        requestRefresh();

        float oriRate = barRt.localScale.x;
        float targetRate = paramRate();

        if (animated && !force)
            setupAnimation(oriRate, targetRate);
        else drawBarValue(targetRate);
    }

    #endregion

    #region 界面绘制

    /// <summary>
    /// 绘制条值（强制）
    /// </summary>
    void drawBarValue(float targetRate) {
        barRt.localScale = new Vector3(targetRate, 1, 1);
    }

    /// <summary>
    /// 绘制文本
    /// </summary>
    void drawTexts() {
        name.text = string.Format(nameFormat, info.name);
        value.text = string.Format(valueFormat, info.value, 
            info.max, paramRate());
        if(rate && info.rate != null)
            rate.text = SceneUtils.double2Str((float)info.rate);
    }

    /// <summary>
    /// 绘制条颜色
    /// </summary>
    void drawBarColor() {
        if (info.color != null)
            barImg.color = (Color)info.color;
    }

    /// <summary>
    /// 刷新视窗
    /// </summary>
    protected override void refresh() {
        base.refresh();
        drawTexts();
        drawBarColor();
    }

    /// <summary>
    /// 清除视窗
    /// </summary>
    protected override void clear() {
        base.clear();
        clearValue();
    }

    #region 动画控制

    /// <summary>
    /// 生成动画
    /// </summary>
    void setupAnimation(float oriRate, float targetRate) {
        var clip = generateAnimationClip(oriRate, targetRate);
        barAni.RemoveClip(AniClipName);
        barAni.AddClip(clip, clip.name);
        barAni.Play(clip.name);
    }

    /// <summary>
    /// 生成动画片段
    /// </summary>
    /// <param name="oriRate">原缩放比率</param>
    /// <param name="targetRate">目标缩放比率</param>
    /// <returns>动画片段</returns>
    AnimationClip generateAnimationClip(float oriRate, float targetRate) {
        var clip = new AnimationClip(); clip.legacy = true;
        var curve = generateAnimationCurve(oriRate, targetRate);
        clip.SetCurve("", typeof(Transform), "m_LocalScale.x", curve);
        clip.name = AniClipName;

        return clip;
    }

    /// <summary>
    /// 生成动画轨迹曲线
    /// </summary>
    /// <param name="oriRate">原缩放比率</param>
    /// <param name="targetRate">目标缩放比率</param>
    /// <returns></returns>
    AnimationCurve generateAnimationCurve(float oriRate, float targetRate) {

        var keys = new Keyframe[2];
        keys[0] = new Keyframe(0, oriRate);
        keys[1] = new Keyframe(AniDuration, targetRate);

        //keys[0].outTangent = 60;
        keys[1].inTangent = 0;

        return new AnimationCurve(keys);
    }

    #endregion

    #endregion
}