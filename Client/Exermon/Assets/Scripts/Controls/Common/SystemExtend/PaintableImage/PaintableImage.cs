using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using Core.UI.Utils;

namespace UnityEngine.UI {
    #region 源码部分

    /// <summary>
    /// Unity 源码
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ObjectPool<T> where T : new() {
        private readonly Stack<T> m_Stack = new Stack<T>();
        private readonly UnityAction<T> m_ActionOnGet;
        private readonly UnityAction<T> m_ActionOnRelease;

        public int countAll { get; private set; }
        public int countActive { get { return countAll - countInactive; } }
        public int countInactive { get { return m_Stack.Count; } }

        public ObjectPool(UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease) {
            m_ActionOnGet = actionOnGet;
            m_ActionOnRelease = actionOnRelease;
        }

        public T Get() {
            T element;
            if (m_Stack.Count == 0) {
                element = new T();
                countAll++;
            } else {
                element = m_Stack.Pop();
            }
            if (m_ActionOnGet != null)
                m_ActionOnGet(element);
            return element;
        }

        public void Release(T element) {
            if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
                Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
            if (m_ActionOnRelease != null)
                m_ActionOnRelease(element);
            m_Stack.Push(element);
        }
    }
    internal static class ListPool<T> {
        // Object pool to avoid allocations.
        private static readonly ObjectPool<List<T>> s_ListPool = new ObjectPool<List<T>>(null, Clear);
        static void Clear(List<T> l) { l.Clear(); }

        public static List<T> Get() {
            return s_ListPool.Get();
        }

        public static void Release(List<T> toRelease) {
            s_ListPool.Release(toRelease);
        }
    }

    #endregion
}
namespace UI.Common.Controls.SystemExtend.PaintableImage {

    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// 可绘制的画布
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("UI/Paintable Image", 10)]
    public class PaintableImage : Image, IPointerDownHandler, IPointerUpHandler {

        /// <summary>
        /// 线段
        /// </summary>
        public struct Line {

            /// <summary>
            /// 颜色
            /// </summary>
            public Color color;

            /// <summary>
            /// 颜色
            /// </summary>
            public float thickness;

            /// <summary>
            /// 点
            /// </summary>
            public List<Vector2> points;

            /// <summary>
            /// 添加点
            /// </summary>
            /// <param name="point"></param>
            public void addPoint(Vector2 point) {
                points.Add(point);
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="color">线条颜色</param>
            public Line(Color color, float thickness) {
                this.color = color;
                this.thickness = thickness;
                points = new List<Vector2>();
            }
        }

        /// <summary>
        /// 线段
        /// </summary>
        public List<Line> lines = new List<Line>();

        /// <summary>
        /// 绘制颜色
        /// </summary>
        [SerializeField]
        public Color lineColor = new Color(0, 0, 0, 1);

        /// <summary>
        /// 厚度
        /// </summary>
        [SerializeField]
        public float thickness = 2.0f;

        /// <summary>
        /// 正在绘图中
        /// </summary>
        int headPointCount = 7; // 头部顶点数量

        bool isDrawing = false;
        float drawingThickness = 2.0f;
        Color drawingColor = new Color(0, 0, 0, 1);

        Camera eventCamera;
        
        #region 数据控制

        /// <summary>
        /// 添加点
        /// </summary>
        /// <param name="pos"></param>
        void addPoint(Vector2 pos) {
            Debug.Log("addPoint: " + pos);
            lines[lines.Count - 1].addPoint(pos);
            SetVerticesDirty();
        }

        /// <summary>
        /// 撤销
        /// </summary>
        public void undo() {
            if (lines.Count <= 0) return;
            lines.RemoveAt(lines.Count - 1);
            SetVerticesDirty();
        }

        /// <summary>
        /// 撤销
        /// </summary>
        public void clear() {
            lines.Clear();
            SetVerticesDirty();
        }

        #endregion

        #region 更新处理

        private void Update() {
            if (isDrawing) addPoint(mousePosToLocalPos(Input.mousePosition));
        }

        /// <summary>
        /// 鼠标位置转化为相对位置
        /// </summary>
        /// <param name="mouse"></param>
        /// <returns></returns>
        Vector2 mousePosToLocalPos(Vector2 mouse) {
			return SceneUtils.screen2Local(mouse, rectTransform, eventCamera);
        }

        #endregion

        #region 绘制处理

        /// <summary>
        /// 顶点生成操作
        /// </summary>
        /// <param name="toFill"></param>
        protected override void OnPopulateMesh(VertexHelper toFill) {
            toFill.Clear();
            foreach(var line in lines) {
                drawingColor = line.color;
                drawingThickness = line.thickness;
                if (line.points.Count > 1)
                    for (int i = 1; i < line.points.Count; i++)
                        drawLine(toFill, line.points[i - 1], line.points[i]);
            }
        }

        /// <summary>
        /// 绘制线条
        /// </summary>
        /// <param name="vh"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        void drawLine(VertexHelper vh, Vector2 start, Vector2 end) {
            Vector2 to = end - start;
            Vector2 nor_to = to.normalized * drawingThickness + start;
            Vector2 up = rotateVector2(90, nor_to, start);
            Vector2 down = rotateVector2(-90, nor_to, start);
            Vector2 up_end = up + to;
            Vector2 down_end = down + to;
            //添加直线
            addQuad(vh, up, down, up_end, down_end);
            List<Vector2> tempPointer = ListPool<Vector2>.Get();
            //添加左边头部
            float angel = 180 / (headPointCount + 1);
            tempPointer.Add(down);
            for (float i = -angel; i > -180; i -= angel) {
                tempPointer.Add(rotateVector2(i, down, start));
            }
            tempPointer.Add(up);

            for (int i = 1; i < tempPointer.Count; i++) {
                addVert(vh, tempPointer[i - 1], tempPointer[i], start);
            }
            //添加右边头部
            tempPointer.Clear();
            tempPointer.Add(up_end);
            for (float i = -angel; i > -180; i -= angel) {
                tempPointer.Add(rotateVector2(i, up_end, end));
            }
            tempPointer.Add(down_end);
            for (int i = 1; i < tempPointer.Count; i++) {
                addVert(vh, tempPointer[i - 1], tempPointer[i], end);
            }
            ListPool<Vector2>.Release(tempPointer);
        }

        /// <summary>
        /// 旋转向量
        /// </summary>
        /// <param name="rotation">角度</param>
        /// <param name="point">旋转点</param>
        /// <param name="origin">原点</param>
        /// <returns></returns>
        private Vector2 rotateVector2(float rotation, Vector2 point, Vector2 origin) {
            var u = point - origin; //point relative to origin  

            if (u == Vector2.zero) return point;

            var a = Mathf.Atan2(u.y, u.x); //angle relative to origin  
            a += rotation; //rotate  

            //u is now the new point relative to origin  
            u = u.magnitude * new Vector2(Mathf.Cos(a), Mathf.Sin(a));
            return u + origin;
        }

        #region 顶点操作

        /// <summary>
        /// 添加顶点
        /// </summary>
        void addVert(VertexHelper vh, Vector2 pos1, Vector2 pos2, Vector2 pos3) {
            addVert(vh, CreateEmptyVertex(pos1), CreateEmptyVertex(pos2), CreateEmptyVertex(pos3));
        }
        void addVert(VertexHelper vh, UIVertex v1, UIVertex v2, UIVertex v3) {
            int index = vh.currentVertCount;
            vh.AddVert(v1); vh.AddVert(v2);
            vh.AddVert(v3); vh.AddTriangle(index, index + 1, index + 2);
        }

        /// <summary>
        /// 添加多边形
        /// </summary>
        /// <param name="vh"></param>
        void addQuad(VertexHelper vh, Vector2 pos1, Vector2 pos2, Vector2 pos3, Vector2 pos4) {
            addQuad(vh, CreateEmptyVertex(pos1), CreateEmptyVertex(pos2), CreateEmptyVertex(pos3), CreateEmptyVertex(pos4));
        }
        void addQuad(VertexHelper vh, UIVertex v1, UIVertex v2, UIVertex v3, UIVertex v4) {
            int index = vh.currentVertCount;
            vh.AddVert(v1); vh.AddVert(v2);
            vh.AddVert(v3); vh.AddVert(v4);
            vh.AddTriangle(index, index + 1, index + 2);
            vh.AddTriangle(index + 2, index + 3, index + 1);
        }

        /// <summary>
        /// 创建空顶点
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        UIVertex CreateEmptyVertex(Vector2 pos) {
            UIVertex v = new UIVertex();
            v.position = pos;
            v.color = drawingColor;
            v.uv0 = Vector2.zero;
            return v;
        }

        #endregion
        
        #endregion

        #region 事件处理

        /// <summary>
        /// 指针按下回调
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(PointerEventData eventData) {
            eventCamera = eventData.pressEventCamera;
            lines.Add(new Line(lineColor, thickness));
            isDrawing = true;
        }

        /// <summary>
        /// 指针释放回调
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerUp(PointerEventData eventData) {
            isDrawing = false;
        }

        #endregion

    }
}