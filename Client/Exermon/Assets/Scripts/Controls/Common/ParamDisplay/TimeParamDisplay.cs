
using System;

using GameModule.Services;

/// <summary>
/// 属性显示类控件
/// </summary>
namespace UI.Common.Controls.ParamDisplays {

    /// <summary>
    /// 时间属性展示
    /// </summary>
    public class TimeParamDisplay : BarParamDisplay {

        #region 初始化

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="stdTime"></param>
        public void configure(TimeSpan stdTime) {
            configure(stdTime.TotalMilliseconds);
        }
        public void configure(double stdTime) {
            configure(); setMax((float)stdTime);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 计算比率
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>返回值对应的比率</returns>
        public override double calcRate(double value) {
            var tmp = ((value / param.max) - 0.5f) * 10;
            return CalcService.Common.sigmoid(tmp);
        }

        #endregion

        #region 界面绘制
        
        #endregion
    }
}