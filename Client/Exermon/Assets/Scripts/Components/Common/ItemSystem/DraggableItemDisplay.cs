using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 可拖拽物品接口
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IDraggableItemDisplay<T> : ISelectableItemDisplay<T>,
    IBeginDragHandler, IDragHandler, IEndDragHandler where T : class {}

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

    public Color draggingColor = new Color(0.8f, 0.8f, 0.8f); // 拖动时背景颜色

    /// <summary>
    /// 内部变量声明
    /// </summary>
    GameObject dragObj; // 拖拽对象（可以是新创建的，也可以是自己）

    Transform originalParent; // 原有父变换

    bool dragging; // 是否拖拽中

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
    /// <param name="eventData">事件数据</param>
    public void OnBeginDrag(PointerEventData eventData) {
        if (!isDraggable()) return;
        dragging = true;
        onBeforeDrag();
        if (createDragObj) dragObj = createDraggingObject();
        else dragObj = convertToDraggingObject();
        updateDraggingObjectPosition(eventData);
        refreshStatus();
    }

    /// <summary>
    /// 拖拽中
    /// </summary>
    /// <param name="eventData">事件数据</param>
    public void OnDrag(PointerEventData eventData) {
        if (!isDraggable()) return;
        dragging = true;
        updateDraggingObjectPosition(eventData);
    }

    /// <summary>
    /// 结束拖拽
    /// </summary>
    /// <param name="eventData">事件数据</param>
    public void OnEndDrag(PointerEventData eventData) {
        dragging = false;
        if (!isDraggable()) return;

        if (createDragObj) Destroy(dragObj);
        else resetFromDraggingObject();
        dragObj = null;
        refreshStatus();
    }

    #endregion

    #region 拖拽控制

    /// <summary>
    /// 开始拖拽
    /// </summary>
    protected virtual void onBeforeDrag() { }

    /// <summary>
    /// 生成拖拽对象
    /// </summary>
    GameObject createDraggingObject() {
        var go = Instantiate(gameObject, transform.parent);
        Destroy(SceneUtils.get<DraggableItemDisplay<T>>(go));
        adjustDraggingObjectTransform(go);
        createDraggingObjectComponents(go);
        return go;
    }

    /// <summary>
    /// 将本对象转化成拖拽对象
    /// </summary>
    GameObject convertToDraggingObject() {
        originalParent = transform.parent;
        adjustDraggingObjectTransform(gameObject);
        createDraggingObjectComponents(gameObject);
        return gameObject;
    }

    /// <summary>
    /// 从拖拽对象中还原
    /// </summary>
    void resetFromDraggingObject() {
        transform.SetParent(originalParent);
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
    }

    /// <summary>
    /// 防止推拽对象的尺寸改变
    /// </summary>
    /// <param name="go">变换</param>
    void preventDraggingObjectSizeChange(RectTransform rt) {
        var size = rt.rect.size;
        rt.anchorMin = draggingParent.anchorMin;
        rt.anchorMax = draggingParent.anchorMax;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;
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
        Vector2 realPos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            draggingParent, eventData.position, eventData.pressEventCamera, out realPos))
            rt.localPosition = realPos; // eventData.position;
        else rt.localPosition = eventData.position;
    }

    #endregion

}
