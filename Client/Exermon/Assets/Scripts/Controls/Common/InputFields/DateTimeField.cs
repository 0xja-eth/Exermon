
using System;
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

namespace UI.Common.Controls.InputFields {

    /// <summary>
    /// 日期选择域
    /// </summary>
    public class DateTimeField : BaseInputField<DateTime> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Text valueText;
        public Transform pickersParent;
        public DateTimePickersPlane pickersPlane;
        public DateTimePicker[] pickers;

        /// <summary>
        /// 外部变量设置
        /// </summary>
        public DateTime defaultDateTime = new DateTime(2000, 1, 1);
        public DateTime minDateTime = new DateTime(1900, 1, 1);
        public DateTime maxDateTime = DateTime.Now;

        public string dateFormat = DataLoader.DisplayDateFormat;

        /// <summary>
        /// 内部变量声明
        /// </summary>
        DateTimePicker year, month, day, hour, minute, second;

        Transform oriParent;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            setupPickers();
        }

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="min">最小日期</param>
        /// <param name="max">最大日期</param>
        /// <param name="default_">默认日期</param>
        public void configure(DateTime min) {
            configure(min, DateTime.Now);
        }
        public void configure(DateTime min, DateTime max) {
            var default_ = defaultDateTime;
            if (default_ > max) default_ = max;
            if (default_ < min) default_ = min;
            configure(min, max, default_);
        }
        public void configure(DateTime min, DateTime max, DateTime default_) {
            defaultDateTime = default_;
            minDateTime = min;
            maxDateTime = max;
            configure();
        }

        /// <summary>
        /// 配置日期选择项
        /// </summary>
        void setupPickers() {
            year = getPicker(DateTimePicker.Type.Year);
            month = getPicker(DateTimePicker.Type.Month);
            day = getPicker(DateTimePicker.Type.Day);
            hour = getPicker(DateTimePicker.Type.Hour);
            minute = getPicker(DateTimePicker.Type.Minute);
            second = getPicker(DateTimePicker.Type.Second);

            value = defaultDateTime;
            updatePickersRange();

            foreach (var picker in pickers)
                picker.configure(this);
        }

        #endregion

        #region 回调控制

        /// <summary>
        /// 值变更回调
        /// </summary>
        public override void onValueChanged(bool check = true, bool emit = true) {
            base.onValueChanged(check, emit);
            if (emit) updatePickersRange();
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 获取日期选择器
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>日期选择器</returns>
        DateTimePicker getPicker(DateTimePicker.Type type) {
            foreach (var picker in pickers)
                if (picker.type == type) return picker;
            return null;
        }

        /// <summary>
        /// 是否选择中
        /// </summary>
        /// <returns>选择中</returns>
        bool isSelecting() {
            return pickersPlane && pickersPlane.shown;
        }

        /// <summary>
        /// 赋值
        /// </summary>
        /// <param name="value">值</param>
        protected override bool assignValue(DateTime dateTime) {
            if (value == dateTime) return false;
            value = dateTime;
            if (value > maxDateTime) value = maxDateTime;
            if (value < minDateTime) value = minDateTime;
            return true;
        }

        /// <summary>
        /// 更新值
        /// </summary>
        public void updateValue(int delta, DateTimePicker.Type type) {
            Debug.Log(type + " updateValue: " + delta);
            switch (type) {
                case DateTimePicker.Type.Year:
                    setValue(value.AddYears(delta), false, false); break;
                case DateTimePicker.Type.Month:
                    setValue(value.AddMonths(delta), false, false); break;
                case DateTimePicker.Type.Day:
                    setValue(value.AddDays(delta), false, false); break;
                case DateTimePicker.Type.Hour:
                    setValue(value.AddHours(delta), false, false); break;
                case DateTimePicker.Type.Minute:
                    setValue(value.AddMinutes(delta), false, false); break;
                case DateTimePicker.Type.Second:
                    setValue(value.AddSeconds(delta), false, false); break;
            }
        }

        /// <summary>
        /// 更新日期选择器的范围
        /// </summary>
        void updatePickersRange() {
            var min = minDateTime;
            var max = maxDateTime;
            var year = value.Year;
            var month = value.Month;
            var minMonth = (year == min.Year) ? min.Month : 1;
            var maxMonth = (year == max.Year) ? max.Month : 12;
            var minDay = (year == min.Year && month == min.Month) ? min.Day : 1;
            var maxDay = (year == max.Year && month == max.Month) ?
                max.Day : DateTime.DaysInMonth(year, month);

            this.year?.setMinMaxValues(min.Year, max.Year);
            this.month?.setMinMaxValues(minMonth, maxMonth);
            day?.setMinMaxValues(minDay, maxDay);
            hour?.setMinMaxValues(0, 23);
            minute?.setMinMaxValues(0, 59);
            second?.setMinMaxValues(0, 59);
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 刷新日期选择器
        /// </summary>
        void refreshPickers() {
            foreach (var picker in pickers)
                picker.setValue(value, true);
        }

        /// <summary>
        /// 绘制值
        /// </summary>
        /// <param name="text">值</param>
        protected override void drawValue(DateTime dateTime) {
            valueText.text = dateTime.ToString(dateFormat);
        }

        /// <summary>
        /// 清除值
        /// </summary>
        protected override void clearValue() {
            value = default;
            valueText.text = "";
        }

        /// <summary>
        /// 刷新视窗
        /// </summary>
        protected override void refresh() {
            base.refresh();
            if (pickersPlane.shown)
                refreshPickers();
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 开始选择
        /// </summary>
        public void startSelect() {
            if (!pickersPlane) return;
            pickersPlane.startWindow();
            oriParent = pickersPlane.transform.parent;
            if (pickersParent) pickersPlane.transform.SetParent(pickersParent);
            refreshPickers();
        }

        /// <summary>
        /// 结束选择
        /// </summary>
        public void endSelect() {
            if (!pickersPlane) return;
            pickersPlane.terminateWindow();
            pickersPlane.transform.SetParent(oriParent);
            onValueChanged();
        }

        /// <summary>
        /// 反转选择
        /// </summary>
        public void toggleSelect() {
            if (isSelecting()) endSelect();
            else startSelect();
        }

        #endregion

        #region 事件控制

        /// <summary>
        /// 是否实际有焦点
        /// </summary>
        public override bool isRealFocused() {
            return isSelecting();
        }

        #endregion

    }
}