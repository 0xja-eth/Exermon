using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Core.UI;
using Core.UI.Utils;

namespace UI.Common.Controls.QuestionDisplay {

    /// <summary>
    /// 做题时间槽
    /// </summary>
    public class QuestionTimer : BaseView {

        /// <summary>
        /// 常量定义
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Text time;
        public Image bar;

		/// <summary>
		/// 外部变量定义
		/// </summary>
        public bool countdown = true; // 倒计时模式

		public int criticalSecond = 15;

		public Color normalColor =
			new Color(0.3921569f, 0.8156863f, 0.8470588f);
		public Color criticalColor =
			new Color(0.9294118f, 0.3098039f, 0.1411765f);

		/// <summary>
		/// 内部变量定义
		/// </summary>
		bool timing = false;
        bool timeUp = false;

        DateTime endTime;
        TimeSpan duration;

        /// <summary>
        /// 时间显示是否显示已过去的时间
        /// </summary>
        [SerializeField]
        bool _reverse = false;
        public bool reverse {
            get { return _reverse; }
            set {
                _reverse = value;
                requestRefresh();
            }
        }

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update(); updateTimer();
        }

        /// <summary>
        /// 更新准备时间
        /// </summary>
        void updateTimer() {
            if (!timing) return;
            var now = DateTime.Now;
            if (now < endTime || !countdown) refresh();
            else if (countdown) stopTimer(true);
        }

        #endregion

        #region 开启/关闭视窗

        /// <summary>
        /// 启动视窗
        /// </summary>
        public void startView(TimeSpan timespan) {
            base.startView();
            startTimer(timespan);
        }
        public void startView(int seconds) {
            base.startView();
            startTimer(seconds);
        }

        /// <summary>
        /// 结束视窗
        /// </summary>
        public override void terminateView() {
            base.terminateView();
            stopTimer();
        }
        public void terminateView(bool timeUp) {
            base.terminateView();
            stopTimer(timeUp);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 是否计时中
        /// </summary>
        /// <returns>返回当前是否计时中</returns>
        public bool isTiming() { return timing; }

        /// <summary>
        /// 是否计时完毕
        /// </summary>
        /// <returns>返回当前是否计时完毕</returns>
        public bool isTimeUp() { return timeUp; }

        /// <summary>
        /// 开始
        /// </summary>
        public void startTimer() {
            startTimer(0);
        }
        /// <param name="timespan">时间</param>
        public void startTimer(TimeSpan timespan) {
            timing = true; timeUp = false;
            setDuration(timespan);
            setTimer(timespan);
        }
        /// <param name="seconds">秒数</param>
        public void startTimer(int seconds) {
            startTimer(new TimeSpan(0, 0, seconds));
        }

        /// <summary>
        /// 设置总时长
        /// </summary>
        /// <param name="timespan">时间</param>
        public void setDuration(TimeSpan timespan) {
            duration = timespan;
            requestRefresh();
        }
        /// <param name="seconds">秒数</param>
        public void setDuration(int seconds) {
            setDuration(new TimeSpan(0, 0, seconds));
        }

        /// <summary>
        /// 设置剩余/过去时间
        /// </summary>
        /// <param name="timespan">时间</param>
        /// <param name="reverse">为false则设置剩余时间，否则设置已过时间</param>
        public void setTimer(TimeSpan timespan, bool reverse = false) {
            if (!reverse) endTime = DateTime.Now + timespan;
            else endTime = DateTime.Now - duration + timespan;
            requestRefresh();
        }
        /// <param name="seconds">秒数</param>
        public void setTimer(int seconds, bool reverse = false) {
            setTimer(new TimeSpan(0, 0, seconds), reverse);
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="timeUp">是否设置timeUp标志</param>
        public void stopTimer(bool timeUp = false) {
            timing = false; this.timeUp = timeUp;
            requestRefresh(true);
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制时间
        /// </summary>
        void drawTimer() {
            var now = DateTime.Now;
            var delta = endTime - now;
            var rate = (duration.Ticks == 0 ? 0 :
                delta.Ticks * 1.0f / duration.Ticks);
            var seconds = delta.TotalSeconds;

            if (reverse) delta = duration - delta;

            time.color = (seconds < criticalSecond) ?
                criticalColor : normalColor;
            time.text = SceneUtils.time2Str(delta);

            rate = Mathf.Clamp01(rate);

            bar.fillAmount = rate;
        }

        /// <summary>
        /// 刷新
        /// </summary>
        protected override void refresh() {
            base.refresh();
            if (timing) drawTimer();
            else clear();
        }

        /// <summary>
        /// 清除
        /// </summary>
        protected override void clear() {
            base.clear();
            time.text = "";
            bar.fillAmount = 0;
        }

        #endregion
    }
}
