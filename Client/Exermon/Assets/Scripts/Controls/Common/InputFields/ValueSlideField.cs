using System;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI.Common.Controls.InputFields {

    /// <summary>
    /// 值条输入域
    /// </summary>
    public class ValueSlideField : BaseInputField<float>,
        IPointerEnterHandler, IPointerExitHandler {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Slider slider;
        public Text valueText;
        public string valueFormat = "{0}";

        /// <summary>
        /// 内部变量声明
        /// </summary>
        bool enter = false;
        bool focused_ = false;

        #region 初始化

        /// <summary>
        /// 初次打开时初始化（子类中重载）
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            value = slider.value;
            slider?.onValueChanged.AddListener((value) => {
                this.value = value;
                onValueChanged();
            });
        }

        /// <summary>
        /// 配置组件
        /// </summary>
        public void configure(float minValue, float maxValue) {
            slider.minValue = minValue;
            slider.maxValue = maxValue;
            configure();
        }

        #endregion

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            updateFocus();
        }

        /// <summary>
        /// 更新 Focus 事件
        /// </summary>
        void updateFocus() {
            if (Input.GetMouseButtonDown(0) ||
                Input.GetMouseButtonDown(1) ||
                Input.touchCount > 0) {
                focused_ = enter;
            }
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 空值
        /// </summary>
        /// <returns></returns>
        public override float emptyValue() { return slider.minValue; }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制值
        /// </summary>
        /// <param name="value">值</param>
        protected override void drawValue(float value) {
            slider.value = value;
            valueText.text = string.Format(valueFormat, value);
        }

        #endregion

        #region 事件控制

        /// <summary>
        /// 是否实际有焦点
        /// </summary>
        public override bool isRealFocused() {
            return focused_;
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 指针进入回调
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerEnter(PointerEventData eventData) {
            enter = true;
        }

        /// <summary>
        /// 指针离开回调
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerExit(PointerEventData eventData) {
            enter = false;
        }

        #endregion
    }
}
