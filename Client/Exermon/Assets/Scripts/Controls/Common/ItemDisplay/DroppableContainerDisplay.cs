
using UnityEngine.EventSystems;

using Core.UI.Utils;

namespace UI.Common.Controls.ItemDisplays {

    /// <summary>
    /// 物品容器接口
    /// </summary>
    public interface IDroppableContainerDisplay<T> : IContainerDisplay<T>,
    IDropHandler where T : class { }

    /// <summary>
    /// 物品容器（实现IDropHandler）
    /// </summary>
    public class DroppableContainerDisplay<T> : SelectableContainerDisplay<T>, 
        IDroppableContainerDisplay<T> where T : class {

        #region 事件控制

        /// <summary>
        /// 拖拽物品放下回调
        /// </summary>
        /// <param name="data">事件数据</param>
        public void OnDrop(PointerEventData data) {
            processItemDrop(getDraggingItemDisplay(data));
        }

        /// <summary>
        /// 获取拖拽中的物品显示项
        /// </summary>
        /// <param name="data">事件数据</param>
        /// <returns>物品显示项</returns>
        DraggableItemDisplay<T> getDraggingItemDisplay(PointerEventData data) {
            var obj = data.pointerDrag;
            if (obj == null) return null;
            return SceneUtils.get<DraggableItemDisplay<T>>(obj);
        }

        /// <summary>
        /// 处理物品放下
        /// </summary>
        protected virtual void processItemDrop(DraggableItemDisplay<T> display) {
            if (display == null) return;
            var container = display.getContainer();
            container.transferItem(this, display.getItem());
        }

        #endregion

    }

    /// <summary>
    /// 物品容器（实现IDropHandler）
    /// </summary>
    public class DroppableContainerDisplay<T, E> : SelectableContainerDisplay<T>, 
        IDroppableContainerDisplay<T> where T : class where E : class {

        #region 事件控制

        /// <summary>
        /// 拖拽物品放下回调
        /// </summary>
        /// <param name="data">事件数据</param>
        public void OnDrop(PointerEventData data) {
            processItemDrop(getDraggingItemDisplay(data), data);
        }

        /// <summary>
        /// 接受转移
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="item">物品</param>
        public virtual void acceptTransfer(SelectableContainerDisplay<E> container, E item) {
        }

        /// <summary>
        /// 获取拖拽中的物品显示项
        /// </summary>
        /// <param name="data">事件数据</param>
        /// <returns>物品显示项</returns>
        DraggableItemDisplay<E> getDraggingItemDisplay(PointerEventData data) {
            var obj = data.pointerDrag;
            if (obj == null) return null;
            return SceneUtils.get<DraggableItemDisplay<E>>(obj);
        }

        /// <summary>
        /// 处理物品放下
        /// </summary>
        protected virtual void processItemDrop(
            DraggableItemDisplay<E> display, PointerEventData data) {
            if (display == null && !display.isDraggable()) return;
            var container = display.getContainer();
            container.transferItem(this, display.getItem());
            display.OnEndDrag(data);
        }

        #endregion

    }
}