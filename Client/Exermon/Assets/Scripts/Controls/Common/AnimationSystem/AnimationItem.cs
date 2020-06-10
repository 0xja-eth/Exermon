using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

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
    public class AnimationItem : BaseView {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Animation animation;
        public AnimationController controller;

        /// <summary>
        /// 动画栈/缓冲队列
        /// </summary>
        Stack<AnimationUtils.TempAnimation> animations = 
            new Stack<AnimationUtils.TempAnimation>();
        Queue<AnimationUtils.TempAnimation> tmpAnimations = 
            new Queue<AnimationUtils.TempAnimation>();

		/// <summary>
		/// RectTransform
		/// </summary>
		RectTransform rectTransform;
		CanvasGroup canvasGroup;
		Graphic graphic;

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
			rectTransform = transform as RectTransform;
			canvasGroup = SceneUtils.get<CanvasGroup>(gameObject);
			graphic = SceneUtils.get<Graphic>(gameObject);
		}

		#endregion

		#region 更新控制

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
            base.update();
            updateAnimation();
        }

        /// <summary>
        /// 更新动画
        /// </summary>
        void updateAnimation() {
            if (curAni().isPlayed())
                onAnimationPlayed();
        }

        /// <summary>
        /// 动画播放完毕回调
        /// </summary>
        void onAnimationPlayed() {
            next(); updateTempAnimations();
        }

        /// <summary>
        /// 更新临时动画
        /// </summary>
        void updateTempAnimations() {
            while(tmpAnimations.Count > 0) 
				animations.Push(tmpAnimations.Dequeue());
        }

        #endregion

        #region 动画控制

        /// <summary>
        /// 当前动画
        /// </summary>
        /// <returns></returns>
        public AnimationUtils.TempAnimation curAni() {
            return animations.Peek();
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

		#region 播放队列操作

		/// <summary>
		/// 添加到播放队列
		/// </summary>
		/// <param name="force">是否直接添加到播放列表</param>
		public void addToPlayQueue(bool force) {
			if (controller == null) return;
			controller.addAnimationItem(this, force);
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
            curAni().setupAnimation(animation);
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
            animations.Pop();
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
            if (isPlaying()) tmpAnimations.Enqueue(ani);
            else animations.Push(ani); return ani;
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
		public void moveTo(Vector2 target, 
			float duration = AnimationUtils.AniDuration, 
			string name = "Move", bool play = false) {
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
		public void moveDelta(Vector2 delta, 
			float duration = AnimationUtils.AniDuration, 
			string name = "Move", bool play = false) {
			if (rectTransform == null) return;

			var ori = rectTransform.anchoredPosition;
			moveTo(ori + delta, duration, name, play);
		}

		/// <summary>
		/// 缩放到指定大小
		/// </summary>
		/// <param name="target">目标量</param>
		/// <param name="duration">时间</param>
		/// <param name="name">动画名</param>
		/// <param name="play">立即播放</param>
		public void scaleTo(Vector3 target, 
			float duration = AnimationUtils.AniDuration, 
			string name = "Scale", bool play = false) {
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
		public void scaleDelta(Vector3 delta, 
			float duration = AnimationUtils.AniDuration, 
			string name = "Scale", bool play = false) {
			if (rectTransform == null) return;

			var ori = rectTransform.localScale;
			scaleTo(ori + delta, duration, name, play);
		}

		/// <summary>
		/// 旋转到指定角度
		/// </summary>
		/// <param name="target">目标量</param>
		/// <param name="duration">时间</param>
		/// <param name="name">动画名</param>
		/// <param name="play">立即播放</param>
		public void rotateTo(Vector3 target, 
			float duration = AnimationUtils.AniDuration, 
			string name = "Rotate", bool play = false) {
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
		public void rotateDelta(Vector3 delta, 
			float duration = AnimationUtils.AniDuration, 
			string name = "Rotate", bool play = false) {
			if (rectTransform == null) return;

			var ori = rectTransform.localEulerAngles;
			rotateTo(ori + delta, duration, name, play);
		}

		/// <summary>
		/// 旋转到指定角度
		/// </summary>
		/// <param name="target">目标量</param>
		/// <param name="duration">时间</param>
		/// <param name="name">动画名</param>
		/// <param name="play">立即播放</param>
		public void colorTo(Color target, 
			float duration = AnimationUtils.AniDuration, 
			string name = "Color", bool play = false) {
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
		public void fadeTo(float alpha, 
			float duration = AnimationUtils.AniDuration, 
			string name = "Fade", bool play = false) {
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
		public void fadeIn(float duration = AnimationUtils.AniDuration, 
			string name = "FadeIn", bool play = false) {
			fadeTo(1, duration, name, play);
		}

		/// <summary>
		/// 淡出
		/// </summary>
		/// <param name="duration">时间</param>
		/// <param name="name">动画名</param>
		/// <param name="play">立即播放</param>
		public void fadeOut(float duration = AnimationUtils.AniDuration, 
			string name = "FadeOut", bool play = false) {
			fadeTo(0, duration, name, play);
		}

		#endregion

		#endregion

	}
}
