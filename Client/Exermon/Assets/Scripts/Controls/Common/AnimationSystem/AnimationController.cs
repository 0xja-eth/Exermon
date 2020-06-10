using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Core.UI;

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
    /// 动画控制器
    /// </summary>
    public class AnimationController : BaseView {

        /// <summary>
        /// 动画队列
        /// </summary>
        public Queue<AnimationItem> animations = new Queue<AnimationItem>();

        /// <summary>
        /// 当前播放列表
        /// </summary>
        public List<AnimationItem> playingAnimations = new List<AnimationItem>();

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            updateAnimations();
        }

        /// <summary>
        /// 更新动画
        /// </summary>
        void updateAnimations() {
            var tmp = playingAnimations.ToArray();
            foreach (var ani in tmp)
                updateAnimationItem(ani);
			if (playingAnimations.Count <= 0)
				playNext();
		}

        /// <summary>
        /// 更新动画项
        /// </summary>
        /// <param name="ani"></param>
        void updateAnimationItem(AnimationItem ani) {
            if (!ani.isPlaying()) ani.play();
			if (ani.isPlayed()) playingAnimations.Remove(ani);
       }

        #endregion

        #region 动画控制

		/// <summary>
		/// 添加动画项
		/// </summary>
		/// <param name="ani">动画项</param>
		/// <param name="force">是否直接添加到播放列表</param>
		public void addAnimationItem(AnimationItem ani, bool force = false) {
			if (force) playingAnimations.Add(ani);
			else animations.Enqueue(ani);
		}

        /// <summary>
        /// 播放下一个动画
        /// </summary>
        public void playNext() {
            if (animations.Count <= 0) return;
            playingAnimations.Add(animations.Dequeue());
        }

        #endregion

    }
}
