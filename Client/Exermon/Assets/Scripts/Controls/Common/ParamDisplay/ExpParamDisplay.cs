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
    /// 经验属性展示
    /// </summary>
    public class ExpParamDisplay : BarParamDisplay {
        
        /// <summary>
        /// 可以转化为经验值条的接口
        /// </summary>
        public interface IExpConvertable {

            /// <summary>
            /// 当前经验值
            /// </summary>
            /// <returns></returns>
            int exp();

            /// <summary>
            /// 下一级经验值
            /// </summary>
            /// <returns></returns>
            int maxExp();
        }

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public GameObject levelUp;

        #region 初始化

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="obj"></param>
        public void configure(IExpConvertable obj) {
            configure(generateJsonFromExpConvertable(obj));
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 从经验转换对象中生成JsonData
        /// </summary>
        /// <param name="obj">转化对象</param>
        /// <returns></returns>
        JsonData generateJsonFromExpConvertable(IExpConvertable obj) {
            var res = new JsonData();
            if (obj == null) return res;

            res["value"] = res["ori_value"] = obj.exp();
            res["rate"] = res["ori_rate"] = obj.exp() * 1.0 / obj.maxExp();
            res["max"] = obj.maxExp();

            return res;
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="obj"></param>
        public void setValue(IExpConvertable obj, bool force = true) {
            setValue(generateJsonFromExpConvertable(obj), force);
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 实际刷新函数s
        /// </summary>
        protected override void refreshMain() {
            base.refreshMain();
            if (levelUp) levelUp.SetActive(param.rate >= 1);
        }

        /// <summary>
        /// 清除视窗
        /// </summary>
        protected override void clear() {
            base.clear();
            if (levelUp) levelUp.SetActive(false);
        }

        #endregion
    }
}