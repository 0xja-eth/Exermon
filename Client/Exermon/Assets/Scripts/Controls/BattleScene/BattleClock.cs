using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Core.UI;
using Core.UI.Utils;

namespace UI.BattleScene.Controls {

    /// <summary>
    /// 对战时间槽
    /// </summary>
    public class BattleClock : BaseView {

        /// <summary>
        /// 常量定义
        /// </summary>
        const int CriticalSecond = 15;

        static readonly Color NormalColor =
            new Color(0.3921569f, 0.8156863f, 0.8470588f);
        static readonly Color CriticalColor =
            new Color(0.9294118f, 0.3098039f, 0.1411765f);

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Text time;
        public Image bar;

        /// <summary>
        /// 内部变量定义
        /// </summary>
        bool timing = false;
        bool timeUp = false;
        DateTime endTime;
        TimeSpan delta;

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            var now = DateTime.Now;
            if (timing && now <= endTime) refresh();
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
        public void startTimer(TimeSpan timespan) {

            timing = true;
            timeUp = false;
            delta = timespan;
            endTime = DateTime.Now + timespan;
        }
        public void startTimer(int seconds) {
            startTimer(new TimeSpan(0, 0, seconds));
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void stopTimer(bool timeUp = false) {
            timing = false;
            this.timeUp = timeUp;
            refresh();
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制时间
        /// </summary>
        void drawTimer() {
            var now = DateTime.Now;
            var delta = endTime - now;
            var seconds = delta.TotalSeconds;

            time.color = (seconds < CriticalSecond) ?
                CriticalColor : NormalColor;
            time.text = SceneUtils.time2Str(delta);

            bar.fillAmount = (this.delta.Ticks == 0 ? 0 :
                delta.Ticks * 1.0f / this.delta.Ticks);
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
