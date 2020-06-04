
using UnityEngine;
using UnityEngine.UI;

using ItemModule.Data;
using UI.Common.Controls.ParamDisplays;
using Core.UI;
using ExerPro.EnglishModule.Data;

namespace UI.ExerPro.BusinessManScene.ParamDisplays
{

    /// <summary>
    /// 信誉积分显示
    /// </summary>
    public class CurrencyDisplay : ParamDisplay<int>
    {
        /// <summary>
        /// 外部变量设置
        /// </summary>
        public Text gold; // 货币

        /// <summary>
        /// 外部变量定义
        /// </summary>
        public string goldFormat = "{0}";

        #region 界面绘制

        /// <summary>
        /// 绘制值
        /// </summary>
        protected override void drawExactlyValue(int data)
        {
            base.drawExactlyValue(base.data);
            gold.text = string.Format(goldFormat, data);
        }

        /// <summary>
        /// 绘制空值
        /// </summary>
        protected override void drawEmptyValue()
        {
            base.drawEmptyValue();
            gold.text = string.Format(goldFormat, 0);
        }

        #endregion

    }
}