
using UnityEngine;
using UnityEngine.EventSystems;

using HedgehogTeam.EasyTouch;

using Core.UI;

namespace UI.MainScene.Controls {

    /// <summary>
    /// 地图显示组件（可拖拽，已弃用）
    /// </summary>
    class MapDisplay : BaseView,
    IBeginDragHandler, IDragHandler, IEndDragHandler {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public RectTransform mask; // 遮罩

        /// <summary>
        /// 外部变量设置
        /// </summary>
        public float gestureSens = 0.0002f;
        public float scrollSens = 0.7f;
        public float minScale = 0.4f, maxScale = 1f;

        /// <summary>
        /// 内部变量声明
        /// </summary>
        RectTransform rTransform; // 自己的RectTransform

        Vector2 lastDragPos;
        Vector2 maxDelta;

        bool gesturing = false; // 手势事件标志
        bool lastGesturing = false;

        bool dragging = false;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            rTransform = transform as RectTransform;
            setupMinScale();
            updateMaxPositionDelta();
        }

        /// <summary>
        /// 配置最小缩放
        /// </summary>
        void setupMinScale() {
            var size = rTransform.rect.size;
            var maskSize = mask.rect.size;
            var min = maskSize / size;
            minScale = Mathf.Max(min.x, min.y, minScale);
        }

        #endregion

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            updateGesture();
            updateScroll();
        }

        /// <summary>
        /// 更新手势
        /// </summary>
        void updateGesture() {
            if (!lastGesturing) gesturing = false;
            lastGesturing = false;
        }

        /// <summary>
        /// 更新滚轮事件
        /// </summary>
        void updateScroll() {
            var delta = Input.GetAxis("Mouse ScrollWheel");
            if (delta != 0)
                scaleDelta(delta * scrollSens, Input.mousePosition);
        }

        #endregion

        #region 坐标控制

        /// <summary>
        /// 移动到
        /// </summary>
        /// <param name="pos"></param>
        public void moveTo(Vector2 pos) {
            var ori = rTransform.anchoredPosition;
            rTransform.anchoredPosition = adjustPosition(pos);
            if (ori != rTransform.anchoredPosition)
                onPositionChanged(ori, rTransform.anchoredPosition);
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="delta">Delta 坐标</param>
        public void moveDetla(Vector2 delta) {
            moveTo(rTransform.anchoredPosition + delta);
        }

        /// <summary>
        /// 调整位置
        /// </summary>
        /// <param name="pos">新位置</param>
        /// <returns>最终位置是否有改变</returns>
        Vector2 adjustPosition(Vector2 pos) {
            // var oriPos = pos;
            if (pos.x > maxDelta.x) pos.x = maxDelta.x;
            if (pos.x < -maxDelta.x) pos.x = -maxDelta.x;
            if (pos.y > maxDelta.y) pos.y = maxDelta.y;
            if (pos.y < -maxDelta.y) pos.y = -maxDelta.y;
            return pos;
        }

        /// <summary>
        /// 位置改变回调
        /// </summary>
        void onPositionChanged(Vector2 ori, Vector2 cur) { }

        /// <summary>
        /// 缩放到
        /// </summary>
        /// <param name="scale">目标缩放</param>
        /// <param name="position">原点</param>
        public void scaleTo(Vector3 scale, Vector2 position) {
            var ori = rTransform.localScale;
            rTransform.localScale = adjustScale(scale);
            if (ori != rTransform.localScale)
                onScaleChanged(ori, rTransform.localScale, position);
        }

        /// <summary>
        /// 缩放
        /// </summary>
        /// <param name="delta">缩放量</param>
        /// <param name="position">原点</param>
        public void scaleDelta(Vector2 delta, Vector2 position) {
            var delta3 = new Vector3(delta.x, delta.y, 0);
            scaleTo(rTransform.localScale + delta3, position);
        }
        public void scaleDelta(float delta, Vector2 position) {
            scaleDelta(new Vector2(delta, delta), position);
        }

        /// <summary>
        /// 缩放改变回调
        /// </summary>
        void onScaleChanged(Vector3 ori, Vector3 cur, Vector2 position) {
            updateMaxPositionDelta();
            adjustScaledPosition(cur - ori, position);
        }

        /// <summary>
        /// 调整缩放
        /// </summary>
        Vector3 adjustScale(Vector3 scale) {
            scale.x = Mathf.Clamp(scale.x, minScale, maxScale);
            scale.y = Mathf.Clamp(scale.y, minScale, maxScale);
            return scale;
        }

        /// <summary>
        /// 调整缩放后的位置
        /// </summary>
        void adjustScaledPosition(Vector2 delta, Vector2 position) {
            var size = rTransform.rect.size * rTransform.localScale;
            var maskSize = mask.rect.size;
            var targetPos = size / 2 - rTransform.anchoredPosition +
                (position - mask.anchoredPosition - maskSize / 2);
            var deltaPos = size / 2 - targetPos;
            var deltaMove = deltaPos * delta;

            moveDetla(deltaMove);
        }

        /// <summary>
        /// 更新最大位置偏移量
        /// </summary>
        void updateMaxPositionDelta() {
            var size = rTransform.rect.size * rTransform.localScale;
            maxDelta = (size - mask.rect.size) / 2;
        }

        #endregion

        #region 事件控制

        /// <summary>
        /// 开始拖拽事件
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public void OnBeginDrag(PointerEventData eventData) {
            if (gesturing) return;
            dragging = true;
            lastDragPos = eventData.position;
        }

        /// <summary>
        /// 拖拽中回调事件
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public void OnDrag(PointerEventData eventData) {
            if (!dragging || gesturing) return;
            moveDetla(eventData.position - lastDragPos);
            lastDragPos = eventData.position;
        }

        /// <summary>
        /// 拖拽结束事件
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public void OnEndDrag(PointerEventData eventData) {
            dragging = false;
        }

        /// <summary>
        /// 手势时间回调
        /// </summary>
        /// <param name="gesture">手势数据</param>
        public void onGesture(Gesture gesture) {
            lastGesturing = gesturing = true;
            dragging = false;

            scaleDelta(gesture.deltaPinch * gestureSens, gesture.position / 2);
        }

        #endregion

    }
}
