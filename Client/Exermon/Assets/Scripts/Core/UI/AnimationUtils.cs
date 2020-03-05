using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 动画工具类
/// </summary>
public static class AnimationUtils {

    /// <summary>
    /// 常量定义
    /// </summary>
    const string AniClipName = "Animation";
    const float AniDuration = 0.5f;

    /// <summary>
    /// 临时动画对象
    /// </summary>
    public struct TempAnimation {
        AnimationClip clip; // 动画片段

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">动画片段名称</param>
        /// <param name="legacy">是否用在 Animation 组件中</param>
        public TempAnimation(string name = AniClipName, bool legacy = true) {
            clip = new AnimationClip();
            clip.legacy = legacy;
            clip.name = name;
        }

        /// <summary>
        /// 添加曲线
        /// </summary>
        /// <param name="type">动画作用的组件类型</param>
        /// <param name="attr">动画改变的属性名称</param>
        /// <param name="ori">原始值</param>
        /// <param name="target">目标值</param>
        /// <param name="duration">时间</param>
        public void addCurve(Type type, string attr,
            float ori, float target, float duration = AniDuration) {
            var curve = generateAnimationCurve(ori, target, duration);
            clip.SetCurve("", type, attr, curve);
        }

        /// <summary>
        /// 生成动画轨迹
        /// </summary>
        /// <param name="ori">原始值</param>
        /// <param name="target">目标值</param>
        /// <param name="duration">时间</param>
        /// <returns>动画轨迹</returns>
        AnimationCurve generateAnimationCurve(float ori, float target, float duration) {
            var keys = new Keyframe[2];
            keys[0] = new Keyframe(0, ori);
            keys[1] = new Keyframe(duration, target);

            keys[1].inTangent = 0;

            return new AnimationCurve(keys);
        }

        /// <summary>
        /// 配置动画
        /// </summary>
        /// <param name="ani">动画对象</param>
        public string setupAnimation(Animation ani, bool play = true) {
            ani.AddClip(clip, clip.name);
            if (play) ani.Play(clip.name);
            return clip.name;
        }
    }

    /// <summary>
    /// 生成一个动画
    /// </summary>
    /// <returns>临时动画数据</returns>
    public static TempAnimation createAnimation(string name = AniClipName, bool legacy = true) {
        return new TempAnimation(name, legacy);
    }
}