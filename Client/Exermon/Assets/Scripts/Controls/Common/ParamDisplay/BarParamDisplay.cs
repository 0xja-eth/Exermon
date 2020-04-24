using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

using LitJson;

using Core.Data;
using Core.Data.Loaders;

using Core.UI;

using Core.UI.Utils;

/// <summary>
/// 属性显示类控件
/// </summary>
namespace UI.Common.Controls.ParamDisplays {

    /// <summary>
    /// 条属性展示
    /// </summary>
    public class BarParamDisplay : ParamDisplay {

        /// <summary>
        /// 参数类型
        /// </summary>
        public class ParamData : BaseData {

            [AutoConvert]
            public string title { get; set; }
            [AutoConvert]
            public float max { get; set; }
            [AutoConvert]
            public float value { get; set; }
            [AutoConvert]
            public float rate { get; set; }
            [AutoConvert]
            public float oriValue { get; set; }
            [AutoConvert]
            public float oriRate { get; set; }
        }

        /// <summary>
        /// 动画类型设置
        /// </summary>
        public enum BarType {
            ScaleX, ScaleY, Fill
        }

        /// <summary>
        /// 值类型
        /// </summary>
        public enum ValueType {
            Number, Double, Percent,
            Sign, SignDouble, SignPercent,
            TimeSpan
        }

        /// <summary>
        /// 设置值类型
        /// </summary>
        public enum SetValueType {
            Value, ValueIncr, Rate, RateIncr
        }

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Text titleText, valueText;
        public Image bar;
        public Animation ani;

        /// <summary>
        /// 外部变量设置
        /// </summary>
        public float duration = AnimationUtils.AniDuration;
        public BarType barType = BarType.Fill;

        public ValueType valueType = ValueType.Number;
        public ValueType deltaValueType = ValueType.Number;
        public string valueFormat = "{0}/{1}";

        public SetValueType setValueType = SetValueType.Value;

        /// <summary>
        /// 内部变量
        /// </summary>
        protected ParamData param = new ParamData();

        #region 初始化

        #endregion

        #region 数据控制

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="force"></param>
        public override void setValue(JsonData value, bool force = false) {
            base.setValue(value, force); loadData();
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        void loadData() {
            if (rawData == null) return;
            if (rawData.IsObject)
                param = DataLoader.load(param, rawData);
            else if (rawData.IsInt || rawData.IsDouble || rawData.IsLong)
                setValue(DataLoader.load<float>(rawData), force);
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="force">强制</param>
        public virtual void setValue(float value, bool force = false) {
            this.force = force;
            Debug.Log("setValue: " + value);
            switch (setValueType) {
                case SetValueType.Value:
                    param.rate = calcRate(param.value = value); break;
                case SetValueType.ValueIncr:
                    param.rate = calcRate(param.value += value); break;
                case SetValueType.Rate: param.rate = value; break;
                case SetValueType.RateIncr: param.rate += value; break;
            }
            requestRefresh();
        }

        /// <summary>
        /// 计算比率
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual float calcRate(float value) {
            return param.max > 0 ? value / param.max : 0;
        }

        /// <summary>
        /// 设置最大值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="force">强制</param>
        public virtual void setMax(float value, bool force = false) {
            this.force = force; param.max = value;
            param.rate = param.max > 0 ? param.value / param.max : 0;
            requestRefresh();
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制条
        /// </summary>
        /// <param name="rate">比率</param>
        /// <param name="oriRate">原始比率</param>
        /// <param name="animated">是否动画</param>
        void drawBar(float rate, float oriRate, bool animated = true) {
            if (!bar) return;
            rate = Mathf.Clamp01(rate);
            oriRate = Mathf.Clamp01(oriRate);
            if (animated) drawBarWithAnimation(rate, oriRate);
            else drawBarNoAnimation(rate);
        }

        /// <summary>
        /// 绘制无动画的条
        /// </summary>
        /// <param name="rate"></param>
        void drawBarNoAnimation(float rate) {
            var oriScale = bar.rectTransform.localScale;
            switch (barType) {
                case BarType.ScaleX:
                    oriScale.x = rate; transform.localScale = oriScale; break;
                case BarType.ScaleY:
                    oriScale.y = rate; transform.localScale = oriScale; break;
                case BarType.Fill:
                    bar.fillAmount = rate; break;
                default: return;
            }
        }

        /// <summary>
        /// 绘制带动画的条
        /// </summary>
        /// <param name="rate"></param>
        void drawBarWithAnimation(float rate, float oriRate) {
            Type type; string key;
            switch (barType) {
                case BarType.ScaleX:
                    type = typeof(Transform); key = "m_LocalScale.x"; break;
                case BarType.ScaleY:
                    type = typeof(Transform); key = "m_LocalScale.y"; break;
                case BarType.Fill:
                    type = typeof(Image); key = "m_FillAmount"; break;
                default: return;
            }
            var tmpAni = AnimationUtils.createAnimation();
            tmpAni.addCurve(type, key, oriRate, rate, duration);
            tmpAni.setupAnimation(ani);
        }

        /// <summary>
        /// 绘制值
        /// </summary>
        /// <param name="value">值</param>
        void drawValue(float value, float oriValue, float max) {
            if (!valueText) return;
            var delta = value - oriValue;
            value = Mathf.Clamp(value, 0, max);
            oriValue = Mathf.Clamp(oriValue, 0, max);
            valueText.text = string.Format(valueFormat, 
                value2Str(value), value2Str(max), 
                value2Str(delta, deltaValueType) 
            );
        }

        /// <summary>
        /// 值转化为字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string value2Str(float value) {
            return value2Str(value, valueType);
        }
        string value2Str(float value, ValueType valueType) {
            var res = "";
            switch (valueType) {
                case ValueType.Number: return value.ToString();
                case ValueType.Double: return SceneUtils.double2Str(value); 
                case ValueType.Percent: return SceneUtils.double2Perc(value);
                case ValueType.Sign:
                    res = value.ToString();
                    if (value > 0) res = "+" + res; break;
                case ValueType.SignDouble:
                    res = SceneUtils.double2Str(value);
                    if (value > 0) res = "+" + res; break;
                case ValueType.SignPercent:
                    res = SceneUtils.double2Perc(value);
                    if (value > 0) res = "+" + res; break;
                case ValueType.TimeSpan: return SceneUtils.time2Str(value);
            }
            return res;
        }

        /// <summary>
        /// 绘制标题
        /// </summary>
        /// <param name="title"></param>
        void drawTitle(string title) {
            if (titleText) titleText.text = title;
        }

        /// <summary>
        /// 实际刷新函数
        /// </summary>
        protected override void refreshMain() {
            drawTitle(param.title);
            drawValue(param.value, param.oriRate, param.max);
            drawBar(param.rate, param.oriRate, !force);
        }

        #endregion
    }
}