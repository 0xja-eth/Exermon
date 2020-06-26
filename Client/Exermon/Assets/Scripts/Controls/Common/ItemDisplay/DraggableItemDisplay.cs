
using UnityEngine;
using UnityEngine.EventSystems;

using Core.UI.Utils;

/// <summary>
/// 物品显示类控件
/// </summary>
namespace UI.Common.Controls.ItemDisplays {

    /// <summary>
    /// 可拖拽物品接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDraggableItemDisplay<T> : ISelectableItemDisplay<T>,
    IBeginDragHandler, IDragHandler, IEndDragHandler where T : class { }

    /// <summary>
    /// 可拖动物品展示组件
    /// </summary>
    public class DraggableItemDisplay<T> : SelectableItemDisplay<T>,
        IDraggableItemDisplay<T> where T : class {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public GameObject draggingFlag; // 拖拽时显示的 GameObject
        public RectTransform draggingParent; // 拖拽时的父变换

        /// <summary>
        /// 外部变量设置
        /// </summary>
        public bool draggable = true; // 能否拖拽
        public bool createDragObj = true; // 拖拽时是否创建一个新的 GameObject

        public float draggingAlpha = 0.75f; // 拖拽时的不透明度

        public Color draggingColor = new Color(0.8f, 0.8f, 0.8f, 0); // 拖动时背景颜色

        /// <summary>
        /// 内部变量声明
        /// </summary>
        GameObject dragObj; // 拖拽对象（可以是新创建的，也可以是自己）

        bool dragging; // 是否拖拽中

        Transform oriParent; // 原有父变换
        Vector2 oriPosition;

        RectTransform rectTransform;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            rectTransform = transform as RectTransform;
        }

		#endregion

		#region 关闭
		
		/// <summary>
		/// 关闭窗口
		/// </summary>
		public override void terminateView() {
			base.terminateView();
			if (isDragging()) releaseDrag();
		}
		
		#endregion

		#region 数据控制

		/// <summary>
		/// 是否可以拖拽
		/// </summary>
		/// <returns>可否拖拽</returns>
		public virtual bool isDraggable() {
            return isActived() && draggable;
        }

        /// <summary>
        /// 是否正在拖拽
        /// </summary>
        /// <returns>是否正在拖拽</returns>
        public virtual bool isDragging() {
            return isDraggable() && dragging;
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 刷新状态
        /// </summary>
        protected override void refreshStatus() {
            base.refreshStatus();
            refreshDragStatus();
        }

        /// <summary>
        /// 刷新拖拽状态
        /// </summary>
        void refreshDragStatus() {
            var dragging = isDragging();
            if (dragging) changeBackgroundColor(draggingColor);
            if (draggingFlag) draggingFlag.SetActive(dragging);
        }

        #endregion

        #region 事件控制

        /// <summary>
        /// 开始拖拽
        /// </summary>
        /// <param name="data">事件数据</param>
        public void OnBeginDrag(PointerEventData data) {
            if (!isDraggable()) return;
            dragging = true;
            onBeforeDrag();

            if (createDragObj) dragObj = createDraggingObject();
            else dragObj = convertToDraggingObject();

            updateDraggingObjectPosition(data);
            refreshStatus();
        }

        /// <summary>
        /// 拖拽中
        /// </summary>
        /// <param name="data">事件数据</param>
        public void OnDrag(PointerEventData data) {
            Debug.Log("OnDrag: " + data.pointerDrag);
            if (!isDraggable()) return;
            dragging = true;
            updateDraggingObjectPosition(data);
        }

        /// <summary>
        /// 结束拖拽
        /// </summary>
        /// <param name="data">事件数据</param>
        public void OnEndDrag(PointerEventData data) {
            Debug.Log("OnEndDrag: " + data.pointerDrag);
            if (isDragging()) releaseDrag();
        }

        #endregion

        #region 拖拽控制

		/// <summary>
		/// 释放拖拽
		/// </summary>
		void releaseDrag() {
			dragging = false;

			onAfterDrag();

			if (createDragObj) Destroy(dragObj);
			else resetFromDraggingObject();
			dragObj = null;

			refreshStatus();
		}

		/// <summary>
		/// 开始拖拽
		/// </summary>
		protected virtual void onBeforeDrag() { }

        /// <summary>
        /// 拖拽结束
        /// </summary>
        protected virtual void onAfterDrag() { }

        /// <summary>
        /// 生成拖拽对象
        /// </summary>
        GameObject createDraggingObject() {
            var go = Instantiate(gameObject, transform.parent);
            var cp = SceneUtils.get<DraggableItemDisplay<T>>(go);
            cp.setItem(item, true); Destroy(cp);
            adjustDraggingObjectTransform(go);
            createDraggingObjectComponents(go);
            return go;
        }

        /// <summary>
        /// 将本对象转化成拖拽对象
        /// </summary>
        GameObject convertToDraggingObject() {
            oriParent = transform.parent;
            oriPosition = rectTransform.anchoredPosition;
            adjustDraggingObjectTransform(gameObject);
            createDraggingObjectComponents(gameObject);
            return gameObject;
        }

        /// <summary>
        /// 从拖拽对象中还原
        /// </summary>
        void resetFromDraggingObject() {
            transform.SetParent(oriParent);
            rectTransform.anchoredPosition = oriPosition;

            Destroy(SceneUtils.get<CanvasGroup>(transform));
        }

        /// <summary>
        /// 调整拖拽对象的变换
        /// </summary>
        /// <param name="go">对象</param>
        protected virtual void adjustDraggingObjectTransform(GameObject go) {
            var rt = go.transform as RectTransform;

            rt.SetParent(draggingParent);
            rt.SetAsLastSibling();

            preventDraggingObjectSizeChange(rt);
        }

        /// <summary>
        /// 防止推拽对象的尺寸改变
        /// </summary>
        /// <param name="go">变换</param>
        void preventDraggingObjectSizeChange(RectTransform rt) {
            var srt = transform as RectTransform;
            //rt.anchorMin = draggingParent.anchorMin;
            //rt.anchorMax = draggingParent.anchorMax;
            //rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = srt.sizeDelta;
        }

        /// <summary>
        /// 创建拖拽对象组件
        /// </summary>
        /// <param name="go">对象</param>
        protected virtual void createDraggingObjectComponents(GameObject go) {
            var cg = go.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = false;
            cg.alpha = draggingAlpha;
        }

        /// <summary>
        /// 更新拖拽对象位置
        /// </summary>
        /// <param name="eventData">事件数据</param>
        void updateDraggingObjectPosition(PointerEventData eventData) {
            var rt = dragObj.transform as RectTransform;
            Debug.Log("screen:" + eventData.position);
            rt.localPosition = SceneUtils.screen2Local(
                eventData.position, draggingParent,
                eventData.pressEventCamera);
            Debug.Log("local" + rt.localPosition);
        }

        #endregion

    }
}