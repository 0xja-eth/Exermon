using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Core.Systems;
using Core.UI;
using Core.UI.Utils;

public class ColumnDivider : BaseView, IDragHandler
{
    RectTransform left;
    RectTransform right;
    public RectTransform canvas;
    RectTransform parent;
    int width;
    int height;
    int leftPadding;
    public void Awake()
    {
        left = this.transform.parent.GetChild(0).GetComponent<RectTransform>();
        right = this.transform.parent.GetChild(1).GetComponent<RectTransform>();
        var rt = gameObject.GetComponent<RectTransform>();
        parent = this.transform.parent.GetComponent<RectTransform>();
        width = (int)parent.sizeDelta.x;
        height = (int)parent.sizeDelta.y;
        leftPadding = (int)parent.position.x-width/2;
        left.sizeDelta = new Vector2(rt.position.x - leftPadding - rt.sizeDelta.x/2, left.sizeDelta.y);
        right.sizeDelta = new Vector2(width - left.sizeDelta.x - rt.sizeDelta.x, right.sizeDelta.y);
    }
    //当鼠标拖动时调用   对应接口 IDragHandler
    public void OnDrag(PointerEventData eventData)
    {
        SetDraggedPosition(eventData);
    }

    private void SetDraggedPosition(PointerEventData eventData)
    {
        var rt = gameObject.GetComponent<RectTransform>();
        // transform the screen point to world point int rectangle
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            globalMousePos.y = rt.position.y;
            rt.position = globalMousePos;
            float rectWidth = rt.position.x - leftPadding - rt.sizeDelta.x / 2;
            if (rectWidth <= 0) rectWidth = 0;
            if (rectWidth >= width) rectWidth = width;
            left.sizeDelta = new Vector2(rectWidth, left.sizeDelta.y);
            right.sizeDelta = new Vector2(width - left.sizeDelta.x - rt.sizeDelta.x, right.sizeDelta.y);
        }
    }
}
