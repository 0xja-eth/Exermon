using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using Core.UI;
using Core.UI.Utils;

/// <summary>
/// 游戏界面层，按照场景进行划分命名空间
/// </summary>
namespace UI { }

/// <summary>
/// 公用的界面类
/// </summary>
namespace UI.Common { }

/// <summary>
/// 公用控件
/// </summary>
namespace UI.Common.Controls { }

/// <summary>
/// 动画控制系统
/// </summary>
namespace UI.Common.Controls.AnimationSystem {

    /// <summary>
    /// 动画项
    /// </summary>
    public class AnimationView : BaseView {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Animation animation;
        public AnimationController controller;

		public RectTransform rectTransform;
		public CanvasGroup canvasGroup;
		public Graphic graphic;

		/// <summary>
		/// 动画栈/缓冲队列
		/// </summary>
		LinkedList<AnimationUtils.TempAnimation> animations = 
            new LinkedList<AnimationUtils.TempAnimation>();
		//Queue<AnimationUtils.TempAnimation> tmpAnimations = 
		//    new Queue<AnimationUtils.TempAnimation>();

		/// <summary>
		/// 动画切换（开始）回调函数（key 为 "" 时表示任意状态）
		/// </summary>
		Dictionary<string, UnityAction> changeEvents = new Dictionary<string, UnityAction>();

		/// <summary>
		/// 动画结束回调函数（key 为 "" 时表示任意状态）
		/// </summary>
		Dictionary<string, UnityAction> endEvents = new Dictionary<string, UnityAction>();

		/// <summary>
		/// 动画更新回调函数
		/// </summary>
		Dictionary<string, UnityAction> updateEvents = new Dictionary<string, UnityAction>();

		/// <summary>
		/// 开始标志
		/// </summary>
		bool isStarted = false;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
			setupAnimationObjects();
			animation = animation ?? SceneUtils.ani(gameObject);
        }

		/// <summary>
		/// 配置动画物体
		/// </summary>
		void setupAnimationObjects(){
			rectTransform = rectTransform ?? transform as RectTransform;
			canvasGroup = canvasGroup ?? SceneUtils.get<CanvasGroup>(gameObject);
			graphic = graphic ?? SceneUtils.get<Graphic>(gameObject);
		}

		#endregion

		#region 回调控制

		/// <summary>
		/// 添加状态切换事件
		/// </summary>
		/// <param name="aniName">状态名</param>
		/// <param name="action">事件</param>
		public void addChangeEvent(string aniName, UnityAction action) {
			changeEvents.Add(aniName, action);
		}

		/// <summary>
		/// 移除状态切换事件
		/// </summary>
		/// <param name="aniName">状态名</param>
		public void removeChangeEvent(string aniName) {
			changeEvents.Remove(aniName);
		}

		/// <summary>
		/// 添加状态结束事件
		/// </summary>
		/// <param name="aniName">状态名</param>
		/// <param name="action">事件</param>
		public void addEndEvent(string aniName, UnityAction action) {
			endEvents.Add(aniName, action);
		}

		/// <summary>
		/// 移除状态结束事件
		/// </summary>
		/// <param name="aniName">状态名</param>
		public void removeEndEvent(string aniName) {
			endEvents.Remove(aniName);
		}

		/// <summary>
		/// 添加状态更新事件
		/// </summary>
		/// <param name="aniName">状态名</param>
		/// <param name="action">事件</param>
		public void addUpdateEvent(string aniName, UnityAction action) {
			updateEvents.Add(aniName, action);
		}

		/// <summary>
		/// 移除状态更新事件
		/// </summary>
		/// <param name="aniName">状态名</param>
		public void removeUpdateEvent(string aniName) {
			updateEvents.Remove(aniName);
		}
		
		/// <summary>
		/// 动画播放完毕回调
		/// </summary>
		void onAnimationPlayed(AnimationUtils.TempAnimation ani) {
			Debug.Log(this.name + " onAnimationPlayed");

			var name = ani.getName();
			if (updateEvents.ContainsKey(""))
				updateEvents[""]?.Invoke();
			else if (endEvents.ContainsKey(name))
				endEvents[name]?.Invoke();
			next(); onNextAnimationPlay(curAni());
		}

		/// <summary>
		/// 下一动画播放开始回调
		/// </summary>
		void onNextAnimationPlay(AnimationUtils.TempAnimation ani) {
			if (ani == null) return;

			var name = ani.getName();
			if (updateEvents.ContainsKey(""))
				updateEvents[""]?.Invoke();
			else if (changeEvents.ContainsKey(name))
				changeEvents[name]?.Invoke();
		}

		#endregion

		#region 更新控制

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
            base.update();
			var ani = curAni();
			updateCurrentEvent(ani);
            updateAnimation(ani);
        }

		/// <summary>
		/// 更新当前动画事件
		/// </summary>
		void updateCurrentEvent(AnimationUtils.TempAnimation ani) {
			if (ani != null && ani.isPlaying()) {
				var name = ani.getName();
				if (updateEvents.ContainsKey(name))
					updateEvents[name]?.Invoke();
			}
		}

		/// <summary>
		/// 更新动画
		/// </summary>
		void updateAnimation(AnimationUtils.TempAnimation ani) {
			if (ani != null && ani.isPlayed()) onAnimationPlayed(ani);
		}

		#endregion

		#region 动画控制

		/// <summary>
		/// 当前动画
		/// </summary>
		/// <returns></returns>
		public AnimationUtils.TempAnimation curAni() {
			if (animations.Count <= 0) return null;
            return animations.First.Value;
        }

        /// <summary>
        /// 是否播放中
        /// </summary>
        /// <returns></returns>
        public bool isPlaying() {
            return animation.isPlaying;
		}

		/// <summary>
		/// 是否播放完毕（全部）
		/// </summary>
		/// <returns></returns>
		public bool isPlayed() {
			return animations.Count <= 0;
		}

		/// <summary>
		/// 当前是否播放完毕
		/// </summary>
		/// <returns></returns>
		public bool isCurPlayed() {
			var ani = curAni(); 
			return ani != null && ani.isPlayed();
		}

		#region 播放队列操作

		/// <summary>
		/// 添加到播放队列
		/// </summary>
		/// <param name="force">是否直接添加到播放列表</param>
		public void addToPlayQueue(bool force) {
			if (controller == null) return;
			controller.add(this, force);
		}
		public void addToPlayQueue() {
			addToPlayQueue(false);
		}

		#endregion

		#region 播放操作

		/// <summary>
		/// 播放
		/// </summary>
		public void play() {
			var ani = curAni();
			Debug.Log(name + " play: " + ani?.getName());
			ani?.setupAnimation(animation);
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void stop() {
            animation.Stop();
        }

        /// <summary>
        /// 切换到下一个动画
        /// </summary>
        public void next() {
            animations.RemoveFirst();
        }

        /// <summary>
        /// 播放下一个动画
        /// </summary>
        public void playNext() {
            next(); play();
        }

		/// <summary>
		/// 添加动画
		/// </summary>
		/// <returns></returns>
		public AnimationUtils.TempAnimation addAnimation(string name, bool legacy) {
			return addAnimation(AnimationUtils.createAnimation(name, legacy));
		}
		public AnimationUtils.TempAnimation addAnimation(string name) {
			var clip = animation.GetClip(name);
			if (clip == null) return addAnimation(name, true);
			return addAnimation(clip);
		}
		public AnimationUtils.TempAnimation addAnimation(AnimationClip clip) {
			return addAnimation(AnimationUtils.createAnimation(clip));
		}
		public AnimationUtils.TempAnimation addAnimation(
            AnimationUtils.TempAnimation ani) {
			Debug.Log(name + " addAnimation: " + ani.getName());
			return enqueueAnimation(ani);
        }

		/// <summary>
		/// 加入队列
		/// </summary>
		/// <param name="ani"></param>
		AnimationUtils.TempAnimation enqueueAnimation(AnimationUtils.TempAnimation ani) {
			foreach (var ani_ in animations)
				if (ani_.isClipEquals(ani)) {
					Debug.LogWarning("Adding animation: " + ani.getName() + " failed.");
					return ani_;
				}
			animations.AddLast(ani); return ani;
		}
		
		#endregion

		#region 预设动画

		/// <summary>
		/// 移动到指定位置
		/// </summary>
		/// <param name="target">目标量</param>
		/// <param name="duration">时间</param>
		/// <param name="name">动画名</param>
		/// <param name="play">立即播放</param>
		public void moveTo(Vector2 target, string name = "Move", 
			float duration = AnimationUtils.AniDuration, bool play = false) {
			if (rectTransform == null) return;

			var ani = addAnimation(name, true);
			var ori = rectTransform.anchoredPosition;

			ani.addCurve(typeof(RectTransform),
				"m_AnchoredPosition.x", ori.x, target.x, duration);
			ani.addCurve(typeof(RectTransform),
				"m_AnchoredPosition.y", ori.y, target.y, duration);

			if (play) this.play();
		}

		/// <summary>
		/// 位置偏移指定量
		/// </summary>
		/// <param name="delta">偏移量</param>
		/// <param name="duration">时间</param>
		/// <param name="name">动画名</param>
		/// <param name="play">立即播放</param>
		public void moveDelta(Vector2 delta, string name = "Move",
			float duration = AnimationUtils.AniDuration, bool play = false) {
			if (rectTransform == null) return;

			var ori = rectTransform.anchoredPosition;
			moveTo(ori + delta, name, duration, play);
		}

		/// <summary>
		/// 缩放到指定大小
		/// </summary>
		/// <param name="target">目标量</param>
		/// <param name="duration">时间</param>
		/// <param name="name">动画名</param>
		/// <param name="play">立即播放</param>
		public void scaleTo(Vector3 target, string name = "Scale",
			float duration = AnimationUtils.AniDuration, bool play = false) {
			if (rectTransform == null) return;

			var ani = addAnimation(name, true);
			var ori = rectTransform.localScale;

			ani.addCurve(typeof(RectTransform),
				"m_LocalScale.x", ori.x, target.x, duration);
			ani.addCurve(typeof(RectTransform),
				"m_LocalScale.y", ori.y, target.y, duration);
			ani.addCurve(typeof(RectTransform),
				"m_LocalScale.z", ori.z, target.z, duration);

			if (play) this.play();
		}

		/// <summary>
		/// 缩放指定量
		/// </summary>
		/// <param name="delta">偏移量</param>
		/// <param name="duration">时间</param>
		/// <param name="name">动画名</param>
		/// <param name="play">立即播放</param>
		public void scaleDelta(Vector3 delta, string name = "Scale",
			float duration = AnimationUtils.AniDuration, bool play = false) {
			if (rectTransform == null) return;

			var ori = rectTransform.localScale;
			scaleTo(ori + delta, name, duration, play);
		}

		/// <summary>
		/// 旋转到指定角度
		/// </summary>
		/// <param name="target">目标量</param>
		/// <param name="duration">时间</param>
		/// <param name="name">动画名</param>
		/// <param name="play">立即播放</param>
		public void rotateTo(Vector3 target, string name = "Rotate",
			float duration = AnimationUtils.AniDuration, bool play = false) {
			if (rectTransform == null) return;

			var ani = addAnimation(name, true);
			var ori = rectTransform.localEulerAngles;

			ani.addCurve(typeof(RectTransform),
				"m_LocalEulerAngles.x", ori.x, target.x, duration);
			ani.addCurve(typeof(RectTransform),
				"m_LocalEulerAngles.y", ori.y, target.y, duration);
			ani.addCurve(typeof(RectTransform),
				"m_LocalEulerAngles.z", ori.z, target.z, duration);

			if (play) this.play();
		}

		/// <summary>
		/// 旋转指定量
		/// </summary>
		/// <param name="delta">偏移量</param>
		/// <param name="duration">时间</param>
		/// <param name="name">动画名</param>
		/// <param name="play">立即播放</param>
		public void rotateDelta(Vector3 delta, string name = "Rotate",
			float duration = AnimationUtils.AniDuration, bool play = false) {
			if (rectTransform == null) return;

			var ori = rectTransform.localEulerAngles;
			rotateTo(ori + delta, name, duration, play);
		}

		/// <summary>
		/// 旋转到指定角度
		/// </summary>
		/// <param name="target">目标量</param>
		/// <param name="duration">时间</param>
		/// <param name="name">动画名</param>
		/// <param name="play">立即播放</param>
		public void colorTo(Color target, string name = "Color",
			float duration = AnimationUtils.AniDuration, bool play = false) {
			if (graphic == null) return;

			var ani = addAnimation(name, true);
			var ori = graphic.color;

			ani.addCurve(typeof(Graphic),
				"m_Color.a", ori.a, target.a, duration);
			ani.addCurve(typeof(Graphic),
				"m_Color.r", ori.r, target.r, duration);
			ani.addCurve(typeof(Graphic),
				"m_Color.g", ori.g, target.g, duration);
			ani.addCurve(typeof(Graphic),
				"m_Color.b", ori.b, target.b, duration);

			if (play) this.play();
		}

		/// <summary>
		/// 淡入淡出到指定量
		/// </summary>
		/// <param name="alpha">目标量</param>
		/// <param name="duration">时间</param>
		/// <param name="name">动画名</param>
		/// <param name="play">立即播放</param>
		public void fadeTo(float alpha, string name = "Fade",
			float duration = AnimationUtils.AniDuration, bool play = false) {
			if (canvasGroup == null) return;

			var ani = addAnimation(name, true);
			var ori = canvasGroup.alpha;

			ani.addCurve(typeof(CanvasGroup), 
				"m_Alpha", ori, alpha, duration);

			if (play) this.play();
		}

		/// <summary>
		/// 淡入
		/// </summary>
		/// <param name="duration">时间</param>
		/// <param name="name">动画名</param>
		/// <param name="play">立即播放</param>
		public void fadeIn(string name = "FadeIn", 
			float duration = AnimationUtils.AniDuration, bool play = false) {
			fadeTo(1, name, duration, play);
		}

		/// <summary>
		/// 淡出
		/// </summary>
		/// <param name="duration">时间</param>
		/// <param name="name">动画名</param>
		/// <param name="play">立即播放</param>
		public void fadeOut(string name = "FadeOut",
			float duration = AnimationUtils.AniDuration, bool play = false) {
			fadeTo(0, name, duration, play);
		}

		#endregion

		#endregion

	}
}
