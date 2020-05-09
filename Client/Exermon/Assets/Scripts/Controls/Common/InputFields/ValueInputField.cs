
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace UI.Common.Controls.InputFields {

    /// <summary>
    /// 文本输入域
    /// </summary>
    public class ValueInputField : BaseInputField<int> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public InputField inputField;

        public EventTrigger decrease, increase;

        /// <summary>
        /// 外部变量设置
        /// </summary>
        public int minValue = 0, maxValue = 99;
        public int step = 1, longStep = 10;

        public int stepTime = 10; // 速率（帧）
        public int longStepTime = 60; // 长按速率（秒）

        public bool editable = true;

        /// <summary>
        /// 内部变量定义
        /// </summary>
        float increaseCnt = 0, decreaseCnt = 0;
        bool increasing = false, decreasing = false;

        //public Text content, placeholder;
        //调用InputField的属性就可以，不需要额外声明

        #region 初始化

        /// <summary>
        /// 初次打开时初始化（子类中重载）
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            setupEventTrigger();
            value = int.Parse(inputField.text);
            inputField?.onEndEdit.AddListener((text) => {
                setValue(int.Parse(text));
            });
        }

        /// <summary>
        /// 配置触发器
        /// </summary>
        void setupEventTrigger() {
            decrease.triggers.Clear();
            increase.triggers.Clear();

            addEventTrigger(decrease,
                EventTriggerType.PointerDown, onDecreaseDown);
            addEventTrigger(decrease,
                EventTriggerType.PointerUp, onDecreaseUp);

            addEventTrigger(increase,
                EventTriggerType.PointerDown, onIncreaseDown);
            addEventTrigger(increase,
                EventTriggerType.PointerUp, onIncreaseUp);
        }

        /// <summary>
        /// 添加事件触发器
        /// </summary>
        /// <param name="trigger">触发器</param>
        /// <param name="eventID">事件类型</param>
        /// <param name="func">回调函数</param>
        void addEventTrigger(EventTrigger trigger,
            EventTriggerType eventID, UnityAction<BaseEventData> func) {
            var entry = new EventTrigger.Entry();
            entry.callback = new EventTrigger.TriggerEvent();
            entry.callback.AddListener(func);
            entry.eventID = eventID;
            trigger.triggers.Add(entry);
        }

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        public void configure(int minValue, int maxValue) {
            this.minValue = minValue;
            this.maxValue = maxValue;
            requestRefresh();
            configure();
        }

        #endregion

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            updateIncreasing();
            updateDecreasing();
            updateEditable();
        }

        /// <summary>
        /// 更新可输入性
        /// </summary>
        void updateEditable() {
            inputField.readOnly = !editable;
        }

        /// <summary>
        /// 更新增加
        /// </summary>
        void updateIncreasing() {
            if (!increasing) return;
            if (++increaseCnt % stepTime == 0)
                increaseValue(increaseCnt > longStepTime);
        }

        /// <summary>
        /// 更新减少
        /// </summary>
        void updateDecreasing() {
            if (!decreasing) return;
            if (++decreaseCnt % stepTime == 0)
                decreaseValue(decreaseCnt > longStepTime);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 空值
        /// </summary>
        /// <returns></returns>
        public override int emptyValue() { return 0; }

        /// <summary>
        /// 赋值
        /// </summary>
        /// <param name="value"></param>
        protected override bool assignValue(int value) {
            var oldValue = this.value;
            Debug.Log("min, max: " + minValue + ", " + maxValue);

            this.value = Mathf.Clamp(value, minValue, maxValue);
            return oldValue != this.value;
        }

        /// <summary>
        /// 增加值
        /// </summary>
        public void increaseValue(bool longStep = false) {
            setValue(value + (longStep ? this.longStep : step));
        }

        /// <summary>
        /// 减少值
        /// </summary>
        public void decreaseValue(bool longStep = false) {
            setValue(value - (longStep ? this.longStep : step));
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制值
        /// </summary>
        /// <param name="text">值</param>
        protected override void drawValue(int value) {
            inputField.text = value.ToString();
        }

        #endregion

        #region 事件控制

        /// <summary>
        /// 是否实际有焦点
        /// </summary>
        public override bool isRealFocused() {
            return inputField && inputField.isFocused;
        }

        /// <summary>
        /// 减少按钮点击按下回调
        /// </summary>
        void onDecreaseDown(BaseEventData e) {
            Debug.Log("onDecreaseDown: " + decreasing);
            decreasing = true; decreaseCnt = 0;
        }

        /// <summary>
        /// 减少按钮点击释放回调
        /// </summary>
        void onDecreaseUp(BaseEventData e) {
            Debug.Log("onDecreaseUp: " + decreasing);
            if (!decreasing) return; decreasing = false;
            decreaseValue(decreaseCnt > longStepTime);
            decreaseCnt = 0;
        }

        /// <summary>
        /// 增加按钮点击按下回调
        /// </summary>
        void onIncreaseDown(BaseEventData e) {
            Debug.Log("onIncreaseDown: " + increasing);
            increasing = true; increaseCnt = 0;
        }

        /// <summary>
        /// 增加按钮点击释放回调
        /// </summary>
        void onIncreaseUp(BaseEventData e) {
            Debug.Log("onIncreaseUp: " + increasing);
            if (!increasing) return; increasing = false;
            increaseValue(increaseCnt > longStepTime);
            increaseCnt = 0;
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 激活
        /// </summary>
        public override void activate() {
            base.activate();
            inputField.ActivateInputField();
        }

        #endregion
    }
}
