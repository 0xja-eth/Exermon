using UnityEngine;
using UnityEngine.UI;

using Core.UI;

namespace UI.Common.Controls.ItemDisplays {

    /// <summary>
    /// 物品显示接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IItemDetailDisplay<T> : IItemDisplay<T> where T : class {

        /// <summary>
        /// 配置窗口
        /// </summary>
        void configure(IContainerDisplay<T> container);

        /// <summary>
        /// 启动窗口
        /// </summary>
        void startView(T item, int index = -1);

        /// <summary>
        /// 设置物品
        /// </summary>
        void setItem(T item, int index = -1, bool force = false);

        /// <summary>
        /// 清除物品
        /// </summary>
        void clearItem();

    }

    /// <summary>
    /// 物品详细信息，带有容器和索引功能
    /// </summary>
    public abstract class ItemDetailDisplay<T> : ItemDisplay<T>, IItemDetailDisplay<T> where T : class {

        /// <summary>
        /// 内部变量声明
        /// </summary>
        protected IContainerDisplay<T> container = null;

        protected int index = -1;

        #region 初始化

        /// <summary>
        /// 配置组件
        /// </summary>
        /// <param name="window">父窗口</param>
        public void configure(IContainerDisplay<T> container) {
            this.container = container;
            configure();
        }

        #endregion

        #region 启动控制

        /// <summary>
        /// 启动窗口
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="index">所在索引</param>
        /// <param name="refresh">强制刷新</param>
        public void startView(T item, int index = -1) {
            startView();
            setItem(item, index, true);
        }

        #endregion

        #region 数据控制

        #region 物品控制

        /// <summary>
        /// 获取物品容器
        /// </summary>
        /// <returns>容器</returns>
        public virtual IContainerDisplay<T> getContainer() {
            return container;
        }

        /// <summary>
        /// 获取当前对应的物品显示控件
        /// </summary>
        /// <returns></returns>
        public virtual ItemDisplay<T> getItemDisplay() {
            var displays = container.getItemDisplays();
            if (index >= 0 && index < displays.Length)
                return displays[index];
            return null;
        }

        #endregion

        /// <summary>
        /// 设置物品
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="index">所在索引</param>
        /// <param name="force">强制刷新</param>
        public void setItem(T item, int index = -1, bool force = false) {
            if (!force && this.item == item && this.index == index) return;
            this.index = index; setValue(item);
        }

		#endregion

		#region 画面控制

		/// <summary>
		/// 更新位置
		/// </summary>
		void updatePosition() {
			if (!needUpdatePosition()) return;

			var display = getItemDisplay();
			if (display == null) return;

			var displayRt = display.transform as RectTransform;
			var rt = transform as RectTransform;

			rt.anchoredPosition = calcPosition(displayRt);
		}

		/// <summary>
		/// 是否需要更新位置
		/// </summary>
		/// <returns></returns>
		protected virtual bool needUpdatePosition() {
			return false;
		}

		/// <summary>
		/// 根据ItemDisplay计算一个位置
		/// </summary>
		/// <param name="rt"></param>
		/// <returns></returns>
		protected virtual Vector2 calcPosition(RectTransform rt) {
			return new Vector2(0, 0);
		}

		/// <summary>
		/// 刷新
		/// </summary>
		protected override void refresh() {
			base.refresh();
			updatePosition();
		}

		#endregion

	}
}