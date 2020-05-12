
using UnityEngine;

using Core.UI;

using UI.Common.Controls.ParamDisplays;

namespace UI.Common.Controls.ItemDisplays {

    /// <summary>
    /// 物品显示接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IItemDisplay<T> : IBaseView where T : class {

        /// <summary>
        /// 启动窗口
        /// </summary>
        void startView(T item);

        /// <summary>
        /// 设置物品
        /// </summary>
        void setItem(T item, bool force = false);

        /// <summary>
        /// 获取物品
        /// </summary>
        /// <returns>物品</returns>
        T getItem();

    }

    /// <summary>
    /// 物品显示组件，用于显示物品的信息
    /// </summary>
    public abstract class ItemDisplay<T> : ParamDisplay<T>, IItemDisplay<T> where T : class {

        /// <summary>
        /// 内部变量声明
        /// </summary>
        protected T item {
            get { return data; }
            set {
                Debug.Log("setItem: " + name + ": " + value);
                data = value;
            }
        }
        /*
        #region 启动控制

        /// <summary>
        /// 启动窗口
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="refresh">强制刷新</param>
        public virtual void startView(T item, bool refresh = false) {
            startView();
            setItem(item, refresh);
        }

        #endregion
        */
        #region 数据控制

        /// <summary>
        /// 默认值
        /// </summary>
        /// <returns>返回数据默认值</returns>
        protected override T defaultValue() {
            return null;
        }

        /// <summary>
        /// 是否为空值
        /// </summary>
        /// <returns></returns>
        public sealed override bool isEmptyValue(T data) {
            return isNullItem(data);
        }

        /// <summary>
        /// 是否为空物品
        /// </summary>
        /// <returns></returns>
        public virtual bool isNullItem(T item) {
            return base.isEmptyValue(item);
        }

        /// <summary>
        /// 设置值（物品）
        /// </summary>
        /// <param name="item">物品</param>
        public sealed override void setValue(T item) {
            setItem(item, true);
        }

        /// <summary>
        /// 设置物品
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="force">强制刷新</param>
        public void setItem(T item, bool force = false) {
            if (!force && this.item == item) return;
            base.setValue(item);
        }

        /// <summary>
        /// 清除值
        /// </summary>
        /// <param name="refresh">刷新</param>
        public sealed override void clearValue() {
            clearItem();
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        public virtual void clearItem() {
            base.clearValue();
        }

        /// <summary>
        /// 获取物品
        /// </summary>
        /// <returns>物品</returns>
        public T getItem() { return item; }

        /// <summary>
        /// 值改变回调
        /// </summary>
        /// <param name="force">强制刷新</param>
        protected sealed override void onValueChanged(bool force = false) {
            onItemChanged(); base.onValueChanged(force);
        }

        /// <summary>
        /// 物品变更回调
        /// </summary>
        protected virtual void onItemChanged() { }

        #endregion

        #region 界面控制

        /// <summary>
        /// 刷新值
        /// </summary>
        protected sealed override void refreshValue() {
            base.refreshValue(); refreshItem();
        }

        /// <summary>
        /// 刷新物品
        /// </summary>
        protected virtual void refreshItem() { }
        
        /// <summary>
        /// 绘制空值
        /// </summary>
        protected sealed override void drawEmptyValue() {
            base.drawEmptyValue(); drawEmptyItem();
        }

        /// <summary>
        /// 绘制空物品
        /// </summary>
        protected virtual void drawEmptyItem() { }

        /// <summary>
        /// 绘制确切值
        /// </summary>
        /// <param name="data"></param>
        protected sealed override void drawExactlyValue(T data) {
            base.drawExactlyValue(data);
            drawExactlyItem(data);
        }

        /// <summary>
        /// 绘制确切的物品
        /// </summary>
        /// <param name="item">物品</param>
        protected virtual void drawExactlyItem(T item) { }
        
        /// <summary>
        /// 清除视窗
        /// </summary>
        protected override void clear() {
            base.clear(); drawEmptyItem();
        }

        #endregion
    }
}