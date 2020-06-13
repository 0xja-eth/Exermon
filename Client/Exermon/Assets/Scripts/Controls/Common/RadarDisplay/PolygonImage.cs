using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using Core.UI.Utils;

/// <summary>
/// 雷达图控件
/// </summary>
namespace UI.Common.Controls.RadarDisplay {

    /// <summary>
    /// 多边形绘制
    /// </summary>
    public class PolygonImage : MaskableGraphic,
    ISerializationCallbackReceiver, ICanvasRaycastFilter {

        /// <summary>
        /// 纹理
        /// </summary>
        [SerializeField]
        Texture _texture;

        /// <summary>
        /// 边权重
        /// </summary>
        //public PolygonImageEdge edgeWeights;
        [SerializeField]
        List<float> _weights;// = new List<float>();
        public List<float> weights {
            get { return _weights; }
        }

        /// <summary>
        /// 默认权重值
        /// </summary>
        public float defaultValue = 0;

        /// <summary>
        /// 设置所有权重
        /// </summary>
        /// <param name="weights"></param>
        public void setWeights(List<float> weights) {
            _weights = weights;
            SetAllDirty();
        }

        /// <summary>
        /// 设置单个权重
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="value">值</param>
        public void setWeight(int index, float value) {
            if (index >= getEdgeCount()) return;
            weights[index] = value;
            SetAllDirty();
        }

        /// <summary>
        /// 增加权重
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="value">值</param>
        public void addWeight(int index, float value) {
            setWeight(index, getWeight(index) + value);
        }

        /// <summary>
        /// 获取单个权重数据
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>权重值</returns>
        public float getWeight(int index) {
            if (index < getEdgeCount())
                return weights[index];
            return 0;
        }

        /// <summary>
        /// 获取边数
        /// </summary>
        /// <returns>权重数据</returns>
        public int getEdgeCount() {
            return weights != null ? weights.Count : 0;
        }

        /// <summary>
        /// 设置权重边数
        /// </summary>
        /// <param name="count">数量</param>
        public void setWeightCount(int count) {
            if (weights == null) _weights = new List<float>();

            int cnt = getEdgeCount();
            if (count > cnt) for (int i = cnt; i < count; i++) weights.Add(defaultValue);
            if (count < cnt) weights.RemoveRange(count, cnt - count);
            if (count != cnt) SetAllDirty();
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void clear() {
            setWeightCount(0);
        }

        /// <summary>
        /// mainTexture
        /// </summary>
        public override Texture mainTexture {
            get {
                if (_texture == null)
                    if (material != null && material.mainTexture != null)
                        return material.mainTexture;
                    else return s_WhiteTexture;
                return _texture;
            }
        }

        public new void SetVerticesDirty() {
        }

        /// <summary>
        /// Texture to be used.
        /// </summary>
        public Texture texture {
            get { return _texture; }
            set {
                if (_texture == value) return;
                _texture = value;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// 击中判断
        /// </summary>
        /// <param name="sp">屏幕位置点</param>
        /// <param name="eventCamera">摄像机</param>
        /// <returns>是否击中</returns>
        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera) {
            if (raycastTarget) {
                // sp 转换为当前 RT 内的点的位置
				Vector2 local = SceneUtils.screen2Local(
					sp, rectTransform, eventCamera);

                int cnt = getEdgeCount();
                float deltaAngle = 360f / cnt;

                for (int i = 0; i < cnt; i++)
                    if (isInPolygon(i, deltaAngle, local)) return true;
            }
            return false;
        }

        public virtual void OnAfterDeserialize() {

        }
        public virtual void OnBeforeSerialize() {

        }

        /// <summary>
        /// 顶点更新
        /// </summary>
        /// <param name="vh">VertexHelper</param>
        protected override void OnPopulateMesh(VertexHelper vh) {
            // 如果顶点不存在或者顶点数 <= 2 则无法绘制，按默认的来绘制
            int cnt = getEdgeCount();
            if (cnt <= 2) base.OnPopulateMesh(vh);
            else {
                vh.Clear(); // 清除默认顶点
                            // 创建新顶点
                float deltaAngle = 360f / cnt;
                for (int i = 0; i < cnt; i++)
                    getTriangle(vh, i, deltaAngle);
            }
        }

        /// <summary>
        /// 创建三角形（创建顶点）
        /// </summary>
        /// <param name="vh">VertexHelper</param>
        /// <param name="index">索引</param>
        /// <param name="deltaAngle">角增量</param>
        private void getTriangle(VertexHelper vh, int index, float deltaAngle) {
            var color32 = color;
            Vector3 cent = new Vector3(0, 0);
            Vector2 p1, p2;

            calcTriangleVertexs(index, deltaAngle, out p1, out p2);

            vh.AddVert(cent, color32, Vector2.zero);
            vh.AddVert(p1, color32, new Vector2(0, 1));
            vh.AddVert(p2, color32, new Vector2(1, 0));

            vh.AddTriangle(index * 3, index * 3 + 1, index * 3 + 2);
        }

        /// <summary>
        /// 判断点是否在多边形内
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="deltaAngle">角增量</param>
        /// <param name="point">点位置</param>
        /// <returns></returns>
        private bool isInPolygon(int index, float deltaAngle, Vector2 point) {
            Vector2 cent = new Vector2(0, 0), p1, p2;

            calcTriangleVertexs(index, deltaAngle, out p1, out p2);

            return isInTriangle(cent, p1, p2, point);
        }

        /// <summary>
        /// 计算三角形顶点
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="deltaAngle">角增量</param>
        /// <param name="p1">顶点1</param>
        /// <param name="p2">顶点2</param>
        void calcTriangleVertexs(int index, float deltaAngle, out Vector2 p1, out Vector2 p2) {
            int cnt = getEdgeCount();
            float edgeLength = Mathf.Min(rectTransform.rect.width, rectTransform.rect.height) * 0.5f;
            float angle1 = 90 + (index + 1) * deltaAngle;
            float angle2 = 90 + (index) * deltaAngle;
            float radius1 = (index == cnt - 1 ? weights[0] : weights[index + 1]) * edgeLength;
            float radius2 = weights[index] * edgeLength;

            p1 = new Vector2(radius1 * Mathf.Cos(angle1 * Mathf.Deg2Rad), radius1 * Mathf.Sin(angle1 * Mathf.Deg2Rad));
            p2 = new Vector2(radius2 * Mathf.Cos(angle2 * Mathf.Deg2Rad), radius2 * Mathf.Sin(angle2 * Mathf.Deg2Rad));
        }

        /// <summary>
        /// 判断点是否在某个三角形内
        /// </summary>
        /// <param name="vertex1">顶点1</param>
        /// <param name="vertex2">顶点2</param>
        /// <param name="vertex3">顶点3</param>
        /// <param name="point">点位置</param>
        /// <returns></returns>
        private bool isInTriangle(Vector2 vertex1, Vector2 vertex2, Vector2 vertex3, Vector2 point) {
            Vector2 v0 = vertex3 - vertex1;
            Vector2 v1 = vertex2 - vertex1;
            Vector2 v2 = point - vertex1;

            float dot00 = Vector2.Dot(v0, v0);
            float dot01 = Vector2.Dot(v0, v1);
            float dot02 = Vector2.Dot(v0, v2);
            float dot11 = Vector2.Dot(v1, v1);
            float dot12 = Vector2.Dot(v1, v2);

            float inverDeno = 1 / (dot00 * dot11 - dot01 * dot01);

            float u = (dot11 * dot02 - dot01 * dot12) * inverDeno;
            if (u < 0 || u > 1) return false;

            float v = (dot00 * dot12 - dot01 * dot02) * inverDeno;
            if (v < 0 || v > 1) return false;

            return u + v <= 1;
        }
    }
}