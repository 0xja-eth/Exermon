using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// UI核心代码
/// </summary>
/// <remarks>
/// 封装了 Unity 内置的 MonoBehaviour，规定了组件、窗口和场景之间的总体流程
/// 同时 AnimationUtils 类和 SceneUtils 类还分别封装了动画和场景/节点查找（Find 函数）/获取组件（GetComponent 函数）等操作
/// </remarks>
namespace Core.UI { }

/// <summary>
/// UI实用类
/// </summary>
/// <remarks>
/// AnimationUtils 以及 SceneUtils 的包
/// </remarks>
namespace Core.UI.Utils {

    /// <summary>
    /// 动画工具类
    /// </summary>
    public static class AnimationUtils {

        /// <summary>
        /// 常量定义
        /// </summary>
        const string AniClipName = "Animation";
        public const float AniDuration = 0.5f;

        /// <summary>
        /// 临时动画对象
        /// </summary>
        public class TempAnimation {

            /// <summary>
            /// 动画片段
            /// </summary>
            public AnimationClip clip;

            /// <summary>
            /// 动画组件
            /// </summary>
            Animation animation = null;

			/// <summary>
			/// 动画开始前事件
			/// </summary>
			UnityAction beforeEvent = null;

			/// <summary>
			/// 构造函数
			/// </summary>
			/// <param name="name">动画片段名称</param>
			/// <param name="legacy">是否用在 Animation 组件中</param>
			public TempAnimation(string name = AniClipName, bool legacy = true) {
				clip = new AnimationClip();
				clip.legacy = legacy;
				clip.name = name;
				clip.wrapMode = WrapMode.Once;
			}
			public TempAnimation(AnimationClip clip) {
				this.clip = clip;
				clip.wrapMode = WrapMode.Once;
			}

			/// <summary>
			/// 获取动画名称
			/// </summary>
			/// <returns>返回动画名称</returns>
			public string getName() {
                return clip.name;
            }

			/// <summary>
			/// 设置前置事件
			/// </summary>
			/// <param name="action">事件</param>
			public void setBeforeEvent(UnityAction action) {
				beforeEvent = action;
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
                animation = ani;
                animation.AddClip(clip, clip.name);
				beforeEvent?.Invoke();
				if (play) this.play();
                return clip.name;
            }
			
			/// <summary>
			/// 是否正在播放
			/// </summary>
			/// <returns></returns>
			public bool isPlaying() {
				return animation && animation.IsPlaying(clip.name);
			}

			/// <summary>
			/// 是否播放完毕
			/// </summary>
			/// <returns></returns>
			public bool isPlayed() {
                return animation && !animation.IsPlaying(clip.name);
            }

			/// <summary>
			/// 两个动画片段是否相等
			/// </summary>
			/// <param name="ani"></param>
			/// <returns></returns>
			public bool isClipEquals(TempAnimation ani) {
				return ani.clip == clip;
			}

            /// <summary>
            /// 播放
            /// </summary>
            public void play() {
                animation?.Play(clip.name);
            }
        }

		/// <summary>
		/// 生成一个动画
		/// </summary>
		/// <returns>临时动画数据</returns>
		public static TempAnimation createAnimation(string name = AniClipName, bool legacy = true) {
			return new TempAnimation(name, legacy);
		}
		public static TempAnimation createAnimation(AnimationClip clip) {
			return new TempAnimation(clip);
		}
	}
}