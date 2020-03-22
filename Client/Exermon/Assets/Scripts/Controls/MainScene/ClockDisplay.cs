using System;
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;
using Core.UI;

/// <summary>
/// 主场景命名空间
/// </summary>
namespace UI.MainScene { }

/// <summary>
/// 主场景控件
/// </summary>
namespace UI.MainScene.Controls {

    /// <summary>
    /// 时钟显示组件
    /// </summary>
    class ClockDisplay : BaseView {

        /// <summary>
        /// 常量设置
        /// </summary>
        const int MaxHour = 12;
        const int MaxMinute = 60;
        const int MaxSecond = 60;

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public RectTransform second, minute, hour;
        public Text dateTime;

        /// <summary>
        /// 外部变量设置
        /// </summary>
        public string dateTimeFormat = DataLoader.DisplayDateTimeFormat;

        /// <summary>
        /// 内部变量声明
        /// </summary>

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            refresh();
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制时钟
        /// </summary>
        /// <param name="dateTime">时间</param>
        void drawClock(DateTime dateTime) {
            float hour = dateTime.Hour;
            float minute = dateTime.Minute;
            float second = dateTime.Second;
            float hourRate = hour / MaxHour;
            float minRate = minute / MaxMinute;
            float secRate = second / MaxSecond;
            setClockArrowRot(this.hour, hourRate);
            setClockArrowRot(this.minute, minRate);
            setClockArrowRot(this.second, secRate);
            this.dateTime.text = dateTime.ToString(dateTimeFormat);
        }

        /// <summary>
        /// 设置时针旋转
        /// </summary>
        /// <param name="arrow">针</param>
        /// <param name="rate">比率</param>
        void setClockArrowRot(RectTransform arrow, float rate) {
            if (arrow == null) return;
            var rot = new Vector3(0, 0, -rate * 360);
            arrow.transform.localEulerAngles = rot;
        }

        /// <summary>
        /// 刷新时钟
        /// </summary>
        void refreshClock() {
            drawClock(DateTime.Now);
        }

        /// <summary>
        /// 刷新视窗
        /// </summary>
        protected override void refresh() {
            base.refresh();
            refreshClock();
        }

        #endregion

    }

}

