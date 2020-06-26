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
    public abstract class ParamDisplay<T> : BaseView {

        /// <summary>
        /// 内部变量声明
        /// </summary>
        protected T data; // 数据

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            data = defaultValue();
        }

        #endregion

        #region 启动控制

        protected override void update() {
            base.update();
            //Debug.Log(name + data);
        }

        /// <summary>
        /// 启动窗口
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="refresh">强制刷新</param>
        public virtual void startView(T item) {
            startView(); setValue(item);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 默认值
        /// </summary>
        /// <returns>返回数据默认值</returns>
        protected virtual T defaultValue() {
            return default;
        }

        /// <summary>
        /// 空值
        /// </summary>
        /// <returns>返回数据默认值</returns>
        protected virtual T emptyValue() {
            return defaultValue();
        }

        /// <summary>
        /// 是否为空值
        /// </summary>
        /// <returns></returns>
        public virtual bool isEmptyValue(T data) {
            return Equals(data, emptyValue());
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns>物品</returns>
        public T getData() {
            return data;
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value">值</param>
        public virtual void setValue(T value) {
            Debug.Log("setValue: " + name + ": " + value);
            data = value; onValueChanged();
        }

        /// <summary>
        /// 清除值
        /// </summary>
        public virtual void clearValue() {
            setValue(defaultValue());
        }

        /// <summary>
        /// 值改变回调
        /// </summary>
        /// <param name="force">强制刷新</param>
        protected virtual void onValueChanged(bool force = false) {
            requestRefresh(force);
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制值
        /// </summary>
        /// <param name="data">物品</param>
        void drawValue(T data) {
            if (isEmptyValue(data)) drawEmptyValue();
            else drawExactlyValue(data);
        }

        /// <summary>
        /// 绘制空物品
        /// </summary>
        protected virtual void drawEmptyValue() { }

        /// <summary>
        /// 绘制确切值
        /// </summary>
        /// <param name="data">物品</param>
        protected virtual void drawExactlyValue(T data) { }

        /// <summary>
        /// 更新值
        /// </summary>
        protected virtual void refreshValue() {
            drawValue(data);
        }

        /// <summary>
        /// 刷新视窗
        /// </summary>
        protected override void refresh() {
            base.refresh();
            refreshValue();
        }
        /*
        /// <summary>
        /// 清除视窗
        /// </summary>
        protected override void clear() {
            base.clear(); clearValue(false);
        }
        */
        #endregion
    }

    /// <summary>
    /// 属性展示
    /// </summary>
    public abstract class ParamDisplay : ParamDisplay<JsonData> {

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
        //protected JsonData data = new JsonData(); // 原始数据

        protected bool immediately = false; // 用于控制动画，在子类中实现其功能

        #region 初始化

        /// <summary>
        /// 配置
        /// </summary>
        public override void configure() {
            configure(new JsonData());
        }
        /// <param name="configData">配置数据</param>
        public void configure(JsonData configData) {
            base.configure(); setValue(configData, true);
        }
        /// <param name="obj">配置对象</param>
        public void configure(IDisplayDataConvertable obj, string type = "") {
            configure(obj.convertToDisplayData(type));
        }

        #endregion

        #region 启动控制

        /// <summary>
        /// 启动窗口
        /// </summary>
        /// <param name="obj">值对象</param>
        public void startView(IDisplayDataConvertable obj,
            string type = "", bool immediately = false) {
            startView(); setValue(obj, type, immediately);
        }
        /// <param name="objs">值对象组</param>
        public void startView(IDisplayDataConvertable[] objs,
            string type = "", bool immediately = false) {
            startView(); setValue(objs, type, immediately);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 默认值
        /// </summary>
        /// <returns>返回数据默认值</returns>
        protected override JsonData defaultValue() {
            return new JsonData();
        }

        /// <summary>
        /// 空值
        /// </summary>
        /// <returns>返回数据默认值</returns>
        protected override JsonData emptyValue() {
            return null;
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value">值</param>
        public override void setValue(JsonData value) {
            setValue(value, false);
        }
        /// <param name="immediately">立即设置</param>
        public virtual void setValue(JsonData value, bool immediately) {
            if (value == null) value = new JsonData();
            this.immediately = immediately; base.setValue(value);
            Debug.Log("SetValue: " + name + ":" + data.ToJson());
        }
        /// <param name="obj">值对象</param>
        public void setValue(IDisplayDataConvertable obj, string type = "", bool immediately = false) {
            setValue(obj.convertToDisplayData(type), immediately);
        }
        /// <param name="objs">值对象组</param>
        public void setValue(IDisplayDataConvertable[] objs, string type = "", bool immediately = false) {
            setValue(mergeObjs(objs, type), immediately);
        }

        /// <summary>
        /// 组合数据
        /// </summary>
        /// <param name="objs"></param>
        JsonData mergeObjs(IDisplayDataConvertable[] objs, string type) {
            var json = new JsonData();
            foreach (var obj in objs) {
                var res = obj.convertToDisplayData(type);
                foreach (var key in res.Keys)
                    json[key] = res[key];
            }
            return json;
        }

        /// <summary>
        /// 设置键
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public virtual void setKey(string key,
            JsonData value, bool refresh = false) {
            if (data != null) data[key] = value;
            requestRefresh(refresh);
        }

        /// <summary>
        /// 清除值
        /// </summary>
        public override void clearValue() {
            immediately = true; base.clearValue();
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
        /// 实际刷新（用于绘制数据）
        /// </summary>
        protected abstract void refreshMain();

        /// <summary>
        /// 刷新视窗
        /// </summary>
        protected sealed override void drawExactlyValue(JsonData data) {
            base.drawExactlyValue(data); refreshMain();
        }

        /// <summary>
        /// 刷新值
        /// </summary>
        protected sealed override void refreshValue() {
            base.refreshValue();
            immediately = false;
        }

        #endregion
    }
}