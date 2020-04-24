
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI.Common.Controls.ItemDisplays {

    /// <summary>
    /// 可选择物品接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISelectableItemDisplay<T> : IItemDisplay<T>,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler where T : class {

        /// <summary>
        /// 配置窗口
        /// </summary>
        void configure(SelectableContainerDisplay<T> container, int index);

        /// <summary>
        /// 获取容器
        /// </summary>
        /// <returns></returns>
        SelectableContainerDisplay<T> getContainer();

    }

    /// <summary>
    /// 可选择物品展示组件
    /// </summary>
    public class SelectableItemDisplay<T> : ItemDisplay<T>, ISelectableItemDisplay<T> where T : class {

        /// <summary>
        /// 常量定义
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Image background;
        public GameObject selectedFlag; // 选择时显示的 GameObject
        public GameObject checkedFlag; // 选中时显示的 GameObject
        public GameObject highlightFlag; // 高亮时显示的 GameObject

        /// <summary>
        /// 外部变量设置
        /// </summary>
        public bool actived = true; // 是否可用

        public bool selectable = true; // 能否选择
        public bool checkable = false; // 能否选中
        public bool highlightable = true; // 能否高亮

        public bool forceChecked = false; // 强制选中

        public bool deselectable = true; // 能否取消选中

        public Color normalColor = new Color(1, 1, 1, 0); // 默认背景颜色
        public Color emptyColor = new Color(1, 1, 1, 0); // 物品为空背景颜色
        public Color selectedColor = new Color(1, 1, 0.75f, 0); // 选择时背景颜色
        public Color checkedColor = new Color(0.8f, 0.8f, 0.8f, 0); // 选中时背景颜色
        public Color highlightColor = new Color(0.8f, 0.8f, 0.8f, 0); // 高亮颜色
        public Color disableColor = new Color(0.5f, 0.5f, 0.5f, 0); // 无效时颜色

        /// <summary>
        /// 内部变量声明
        /// </summary>
        bool highlighting = false; // 是否高亮中

        protected SelectableContainerDisplay<T> container = null;

        protected int index = -1;

        #region 初始化

        /// <summary>
        /// 配置组件
        /// </summary>
        public virtual void configure(SelectableContainerDisplay<T> container, int index) {
            this.container = container;
            this.index = index;
            configure();
        }

        #endregion

        #region 数据控制

        #region 物品控制

        /// <summary>
        /// 获取物品容器
        /// </summary>
        /// <returns>容器</returns>
        public SelectableContainerDisplay<T> getContainer() {
            return container;
        }

        #endregion

        #region 状态控制

        /// <summary>
        /// 是否激活
        /// </summary>
        /// <returns>是否激活</returns>
        public virtual bool isActived() {
            if (container != null && !container.actived) return false;
            return actived;
        }

        /// <summary>
        /// 是否可以高光
        /// </summary>
        /// <returns>可否高光</returns>
        public virtual bool isHighlightable() {
            if (container != null && !container.highlightable) return false;
            return isActived() && highlightable;
        }

        /// <summary>
        /// 是否正在高光
        /// </summary>
        /// <returns>是否正在高光</returns>
        public virtual bool isHighlighting() {
            return isHighlightable() && highlighting;
        }

        /// <summary>
        /// 是否选择
        /// </summary>
        /// <returns>是否选择</returns>
        public virtual bool isSelected() {
            if (!container) return false;
            if (!isSelectable()) return false;
            return container.getSelectedIndex() == index;
        }

        /// <summary>
        /// 是否可以选择
        /// </summary>
        /// <returns>可否选择</returns>
        public virtual bool isSelectable() {
            if (container != null && !container.selectable) return false;
            return isActived() && selectable;
        }

        /// <summary>
        /// 是否可以取消选择
        /// </summary>
        /// <returns></returns>
        public virtual bool isDeselectable() {
            if (container != null && !container.deselectable) return false;
            return isActived() && deselectable;
        }

        /// <summary>
        /// 是否选中
        /// </summary>
        /// <returns>是否选中</returns>
        public virtual bool isChecked() {
            if (!container) return false;
            if (!isCheckable()) return false;
            if (isForceChecked()) return true;
            return container.isChecked(index);
        }

        /// <summary>
        /// 是否强制选中
        /// </summary>
        /// <returns>强制选中</returns>
        public virtual bool isForceChecked() {
            return isCheckable() && forceChecked;
        }

        /// <summary>
        /// 是否可以选中
        /// </summary>
        /// <returns>可否选中</returns>
        public virtual bool isCheckable() {
            if (container != null && !container.checkable) return false;
            return isActived() && checkable;
        }

        /// <summary>
        /// 是否可以取消选中
        /// </summary>
        /// <returns>可否取消选中</returns>
        public virtual bool isUncheckable() {
            if (container != null && !container.checkable) return false;
            return isActived() && checkable && !isForceChecked();
        }

        /// <summary>
        /// 选择
        /// </summary>
        public virtual void select() {
            if (container == null) return;
            if (!isSelectable()) return;
            container.select(index);
        }

        /// <summary>
        /// 取消选择
        /// </summary>
        public virtual void deselect() {
            if (container == null) return;
            container.deselect();
        }

        /// <summary>
        /// 选中
        /// </summary>
        public virtual void check() {
            if (container == null) return;
            if (!isCheckable()) return;
            container.check(index);
        }

        /// <summary>
        /// 取消选中
        /// </summary>
        public virtual void uncheck() {
            if (container == null) return;
            if (!isUncheckable()) return;
            container.uncheck(index);
        }

        /// <summary>
        /// 反转
        /// </summary>
        public virtual void toggle() {
            if (container == null) return;
            if (!isActived()) return;
            container.toggle(index);
        }

        #endregion

        #endregion

        #region 界面控制

        #region 状态刷新

        /// <summary>
        /// 改变背景颜色
        /// </summary>
        protected virtual void changeBackgroundColor(Color color) {
            if (!background) return;
            if (color.a <= 0) return;
            background.color = color;
        }

        /// <summary>
        /// 刷新状态
        /// </summary>
        protected virtual void refreshStatus() {
            changeBackgroundColor(item == null ?
                emptyColor : normalColor);
            refreshActivedStatus();
            refreshHightlightStatus();
            refreshSelectStatus();
            refreshCheckStatus();
        }

        /// <summary>
        /// 刷新激活状态
        /// </summary>
        void refreshActivedStatus() {
            if (!isActived()) changeBackgroundColor(disableColor);
        }

        /// <summary>
        /// 刷新高光状态
        /// </summary>
        void refreshHightlightStatus() {
            if (highlighting) changeBackgroundColor(highlightColor);
            if (highlightFlag) highlightFlag.SetActive(highlighting);
        }

        /// <summary>
        /// 刷新选择状态
        /// </summary>
        void refreshSelectStatus() {
            var selected = isSelected();
            if (selected) changeBackgroundColor(selectedColor);
            if (selectedFlag) selectedFlag.SetActive(selected);
        }

        /// <summary>
        /// 刷新选中状态
        /// </summary>
        void refreshCheckStatus() {
            var checked_ = isChecked();
            if (checked_) changeBackgroundColor(checkedColor);
            if (checkedFlag) checkedFlag.SetActive(checked_);
        }

        #endregion

        /// <summary>
        /// 刷新视窗
        /// </summary>
        protected override void refresh() {
            base.refresh();
            refreshStatus();
        }

        /// <summary>
        /// 清空视窗
        /// </summary>
        protected override void clear() {
            base.clear();
            refreshStatus();
        }

        #endregion

        #region 事件控制

        /// <summary>
        /// 指针进入回调
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public virtual void OnPointerEnter(PointerEventData eventData) {
            if (isHighlightable()) highlighting = true;
            refreshStatus();
        }

        /// <summary>
        /// 指针离开回调
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public virtual void OnPointerExit(PointerEventData eventData) {
            highlighting = false;
            refreshStatus();
        }

        /// <summary>
        /// 处理点击事件回调
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public virtual void OnPointerClick(PointerEventData eventData) {
            // 当已选择且不可选中时单击，如果可以取消选择就取消选择
            if (isSelected() && !isCheckable() && 
                !isUncheckable() && isDeselectable()) deselect();
            // 如果已选择或者不可选择时单击，执行反转操作
            else if (isSelected() || !isSelectable()) toggle();
            // 否则，选择
            else select();
            refreshStatus();
        }

        #endregion
    }
}