using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

using LitJson;

using Core.Data.Loaders;

using Core.UI;
using Core.UI.Utils;

/// <summary>
/// 属性显示类控件
/// </summary>
namespace UI.Common.Controls.ParamDisplays {

    /// <summary>
    /// 属性展示
    /// </summary>
    public abstract class ParamDisplay : BaseView {
        
        /// <summary>
        /// 能转化为属性显示数据的接口
        /// </summary>
        public interface IDisplayDataConvertable {

            /// <summary>
            /// 转化为属性信息集
            /// </summary>
            /// <returns>属性信息集</returns>
            JsonData convertToDisplayData(string type = "");
        }

        /// <summary>
        /// 能转化为属性显示数据的接口
        /// </summary>
        public interface IDisplayDataArrayConvertable {

            /// <summary>
            /// 转化为属性信息集
            /// </summary>
            /// <returns>属性信息集</returns>
            JsonData[] convertToDisplayDataArray(string type = "");
        }

        /// <summary>
        /// 内部变量声明
        /// </summary>
        protected JsonData rawData = new JsonData(); // 原始数据

        protected bool force = false; // 用于控制动画，在子类中实现其功能

        #region 初始化

        /// <summary>
        /// 配置组件
        /// </summary>
        public override void configure() {
            configure(new JsonData());
        }
        /// <param name="initData">初始数据</param>
        public void configure(JsonData initData) {
            base.configure();
            setValue(initData, true);
        }
        /// <param name="obj">对象</param>
        public void configure(IDisplayDataConvertable obj, string type = "") {
            configure(obj.convertToDisplayData(type));
        }

        #endregion

        #region 数据控制
        
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value">值</param>
        public virtual void setValue(JsonData value, bool force = false) {
            this.force = force; rawData = value;
            if (value == null) rawData = new JsonData();
            Debug.Log("SetValue: " + name + ":" + rawData.ToJson());
            requestRefresh();
        }
        /// <param name="obj">值对象</param>
        public void setValue(IDisplayDataConvertable obj, string type = "", bool force = false) {
            setValue(obj.convertToDisplayData(type), force);
        }
        /// <param name="objs">值对象组</param>
        public void setValue(IDisplayDataConvertable[] objs, string type = "", bool force = false) {
            var json = new JsonData();
            foreach (var obj in objs) {
                var res = obj.convertToDisplayData(type);
                foreach (var key in res.Keys)
                    json[key] = res[key];
            }
            setValue(json, force);
        }

        /// <summary>
        /// 设置键
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public virtual void setKey(string key, JsonData value, bool refresh = false) {
            rawData[key] = value;
            requestRefresh(refresh);
        }

        /// <summary>
        /// 清除值
        /// </summary>
        public virtual void clearValue() {
            force = true; rawData = new JsonData();
            requestRefresh(true);
        }

        /// <summary>
        /// 清除键
        /// </summary>
        /// <param name="key">键</param>
        public virtual void clearKey(string key, bool refresh = false) {
            setKey(key, null, refresh);
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 实际刷新
        /// </summary>
        protected abstract void refreshMain();

        /// <summary>
        /// 刷新视窗
        /// </summary>
        protected override void refresh() {
            base.refresh();
            refreshMain();
            force = false;
        }

        /// <summary>
        /// 清除视窗
        /// </summary>
        protected override void clear() {
            base.clear(); clearValue();
        }

        #endregion
    }
}