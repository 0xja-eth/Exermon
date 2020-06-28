using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using Core.UI;
using ExerPro.EnglishModule.Data;

namespace UI.Common.Controls.ItemDisplays {
    /*
    /// <summary>
    /// 物品容器接口
    /// </summary>
    public interface IContainerDisplay : IBaseView {

        /// <summary>
        /// 启动视窗
        /// </summary>
        //void startView(int index = 0);
    }
    */
    /// <summary>
    /// 物品容器接口
    /// </summary>
    public interface IContainerDisplay<T> : IBaseView where T : class {

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="items">物品集</param>
        void configure(T[] items);
        void configure(List<T> items);

        /// <summary>
        /// 设置物品集
        /// </summary>
        /// <param name="items">物品集</param>
        void setItems(T[] items);
        void setItems(List<T> items);

        /// <summary>
        /// 是否包含物品
        /// </summary>
        /// <param name="item">物品</param>
        /// <returns>是否包含</returns>
        bool containsItem(T item);

        /// <summary>
        /// 获取物品集
        /// </summary>
        /// <returns>物品集</returns>
        T[] getItems();
        
        /// <summary>
        /// 获取物品对应的物品显示项
        /// </summary>
        /// <param name="item">物品</param>
        /// <returns>物品显示项</returns>
        ItemDisplay<T>[] getItemDisplays();
    }

    /// <summary>
    /// 物品容器显示
    /// </summary>
    public class ContainerDisplay<T> : GroupView<ItemDisplay<T>>, IContainerDisplay<T> where T : class {

        /// <summary>
        /// 常量设置
        /// </summary>
        // public ItemInfo<T> detail; // 帮助界面
        
        /// <summary>
        /// 外部变量设置
        /// </summary>
        public int defaultCapacity = 0; // 默认容量

        /// <summary>
        /// 回调函数集
        /// </summary>
        public List<UnityAction> onItemsChangedCallbacks = new List<UnityAction>();

        /// <summary>
        /// 内部变量声明
        /// </summary>
        protected List<T> items = new List<T>(); // 物品列表

        #region 初始化

        /// <summary>
        /// 配置
        /// </summary>
        public override void configure() {
            base.configure();
            configureItemDisplays();
            configureDetail();
        }
        /// <param name="items">物品集</param>
        public void configure(T[] items) {
            configure(); setItems(items);
        }
        public void configure(List<T> items) {
            configure(); setItems(items);
        }

        /// <summary>
        /// 配置物品帮助组件
        /// </summary>
        void configureDetail() {
            var detail = getItemDetail();
            if (detail != null) detail.configure(this);
        }

        /// <summary>
        /// 配置初始的物品显示项
        /// </summary>
        void configureItemDisplays() {
            for (int i = 0; i < itemDisplaysCount(); ++i)
                createSubView(null, i);
        }

        #endregion

        #region 启动控制

        /// <summary>
        /// 启动视窗
        /// </summary>
        /// <param name="index"></param>
        public void startView(int index = 0) {
            base.startView();
        }

		#endregion

		#region 更新控制

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			updateDisplays();
		}

		/// <summary>
		/// 更新显示
		/// </summary>
		void updateDisplays() {
			for(int i = 0; i < itemDisplaysCount(); ++i) {
				var itemDisplay = subViews[i];
				if (itemDisplay.isRequestDestroy())
					removeItem(itemDisplay.getItem());
			}
		}

		#endregion

		#region 回调控制

		/// <summary>
		/// 添加回调函数
		/// </summary>
		/// <param name="cb">回调函数</param>
		/// <param name="type">回调类型（0：物品变更）</param>
		public void addCallback(UnityAction cb, int type = 0) {
            if (cb == null) return;
            switch (type) {
                case 0: onItemsChangedCallbacks.Add(cb); break;
            }
        }

        #endregion

        #region 数据控制

        #region 物品控制

        /// <summary>
        /// 获取物品帮助组件
        /// </summary>
        /// <returns>帮助组件</returns>
        protected virtual IItemDetailDisplay<T> getItemDetail() {
            return null;
        }

        /// <summary>
        /// 获取容量
        /// </summary>
        /// <returns>容量</returns>
        public virtual int capacity() {
            return defaultCapacity;
        }

        /// <summary>
        /// 获取物品数量
        /// </summary>
        /// <returns>数量</returns>
        public int itemsCount() {
            return items.Count;
        }

        /// <summary>
        /// 物品显示项数量
        /// </summary>
        /// <returns></returns>
        public int itemDisplaysCount() {
            return Math.Max(maxItemDisplaysCount(), subViews.Count);
        }

        /// <summary>
        /// 物品显示项最大数量
        /// </summary>
        /// <returns></returns>
        public int maxItemDisplaysCount() {
            var capacity = this.capacity();
            return capacity > 0 ? capacity : itemsCount();
        }

        /// <summary>
        /// 是否包含空物品
        /// </summary>
        /// <returns>返回容器是否包含空物品</returns>
        protected virtual bool includeEmpty() {
            return false;
        }

        /// <summary>
        /// 是否包含物品
        /// </summary>
        /// <param name="item">物品</param>
        /// <returns>返回指定物品能否包含在容器中</returns>
        protected virtual bool isIncluded(T item) {
            if (item == null) return includeEmpty();
            return true;
        }

        /// <summary>
        /// 设置物品集
        /// </summary>
        /// <param name="items">物品集</param>
        public void setItems(T[] items) {
            clearItems();
            var tmpItems = new List<T>(items);
            this.items = tmpItems.FindAll(isIncluded);
            onItemsChanged();
        }
        public void setItems(List<T> items) {
            setItems(items.ToArray());
        }

        /// <summary>
        /// 是否包含物品
        /// </summary>
        /// <param name="item">物品</param>
        /// <returns>是否包含</returns>
        public bool containsItem(T item) {
            return items.Contains(item);
        }

        /// <summary>
        /// 获取物品对应的物品显示项
        /// </summary>
        /// <param name="item">物品</param>
        /// <returns>物品显示项</returns>
        public ItemDisplay<T> getItemDisplay(T item) {
            return subViews.Find((item_) => item_.getItem() == item);
        }

        /// <summary>
        /// 增加物品
        /// </summary>
        /// <param name="item">物品</param>
        public virtual void addItem(T item) {
            items.Add(item);
            onItemsChanged();
        }

        /// <summary>
        /// 移除物品
        /// </summary>
        /// <param name="item">物品</param>
        public virtual void removeItem(T item) {
            items.Remove(item);
            onItemsChanged();
        }

        /// <summary>
        /// 清空物品
        /// </summary>
        public void clearItems() {
            items.Clear();
            requestRefresh();
        }

        /// <summary>
        /// 获取物品集
        /// </summary>
        /// <returns>物品集</returns>
        public T[] getItems() {
            return items.ToArray();
        }

        /// <summary>
        /// 获取物品显示项数组
        /// </summary>
        /// <returns>物品显示项数组</returns>
        public ItemDisplay<T>[] getItemDisplays() {
            return subViews.ToArray();
        }

        /// <summary>
        /// 物品变更回调
        /// </summary>
        protected virtual void onItemsChanged() {
            refreshItemDisplays();
            requestRefresh();
            callbackItemsChange();
        }

        /// <summary>
        /// 处理物品改变回调
        /// </summary>
        void callbackItemsChange() {
            foreach (var cb in onItemsChangedCallbacks) cb?.Invoke();
        }

        /// <summary>
        /// 刷新物品
        /// </summary>
        public virtual void refreshItems() {
            setItems(items);
        }

        #endregion

        #endregion

        #region 界面控制
        
        #region 物品显示项绘制

        /// <summary>
        /// 创建物品显示组件
        /// </summary>
        void refreshItemDisplays() {
            createSubViews();
            destroyRedundantSubViews();
        }

        /// <summary>
        /// 创建子视图
        /// </summary>
        protected virtual void createSubViews() {
            for (int i = 0; i < maxItemDisplaysCount(); ++i) {
                T item = (i < itemsCount() ? items[i] : null);
                createSubView(item, i);
            }
        }

        /// <summary>
        /// 移除冗余子视图
        /// </summary>
        protected virtual void destroyRedundantSubViews() {
            for (int i = itemDisplaysCount() - 1; i >= maxItemDisplaysCount(); --i)
                destroySubView(i);
        }

        /// <summary>
        /// 创建物品显示组件
        /// </summary>
        /// <param name="item">物品</param>
        protected virtual void createSubView(T item, int index) {
            createSubView(index).startView(item);
        }

        /// <summary>
        /// ItemDisplay 创建回调
        /// </summary>
        /// <param name="item">ItemDisplay</param>
        protected override void onSubViewCreated(ItemDisplay<T> sub, int index) {
            sub.configure();
            base.onSubViewCreated(sub, index);
        }

        #endregion

        /// <summary>
        /// 清除视窗
        /// </summary>
        protected override void clear() {
            base.clear();
            clearItems();
        }

		#endregion

	}
}