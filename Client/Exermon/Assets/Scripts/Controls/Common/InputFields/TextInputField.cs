
using UnityEngine.UI;

namespace UI.Common.Controls.InputFields {

    /// <summary>
    /// 文本输入域
    /// </summary>
    public class TextInputField : BaseInputField<string> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public InputField inputField;

        //public Text content, placeholder;
        //调用InputField的属性就可以，不需要额外声明

        #region 初始化

        /// <summary>
        /// 初次打开时初始化（子类中重载）
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            value = inputField.text;
            inputField?.onEndEdit.AddListener((text) => {
                value = text;
                onValueChanged();
            });
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 空值
        /// </summary>
        /// <returns></returns>
        public override string emptyValue() { return ""; }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制值
        /// </summary>
        /// <param name="text">值</param>
        protected override void drawValue(string text) {
            inputField.text = text;
        }

        #endregion

        #region 事件控制

        /// <summary>
        /// 是否实际有焦点
        /// </summary>
        public override bool isRealFocused() {
            return inputField && inputField.isFocused;
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
