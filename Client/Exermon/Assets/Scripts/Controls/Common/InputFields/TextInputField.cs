
using System.Text.RegularExpressions;

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

        /// <summary>
        /// 外部变量设置
        /// </summary>
        public string[] filters = new string[] {
            @"\p{Cs}", @"[\u2702-\u27B0]"
        };

        //public Text content, placeholder;
        //调用InputField的属性就可以，不需要额外声明

        #region 初始化

        /// <summary>
        /// 初次打开时初始化（子类中重载）
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            value = inputField.text;
            inputField.onValidateInput = onValidateInput;
            inputField.onEndEdit.AddListener((text) => {
                setValue(text);
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
            return inputField.isFocused;
        }

        /// <summary>
        /// 数据验证
        /// </summary>
        /// <returns></returns>
        char onValidateInput(string text, int charIndex, char addedChar) {
            if (filters.Length > 0) 
                if (filter(addedChar)) return '\0';
            return addedChar;
        }

        /// <summary>
        /// 过滤非法字符
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        bool filter(char s) {
            var str = s.ToString();
            foreach(var f in filters)
                if (Regex.IsMatch(str, f))
                    return true;
            return false;
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
