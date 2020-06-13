
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Core.UI {

	using Utils;

    /// <summary>
    /// 分隔栏
    /// </summary>
    public class SplitView : BaseView, IDragHandler {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public RectTransform left; // 左部分
        public RectTransform right; // 右部分
        public RectTransform container; // 父元素（默认为自身父元素）

        /// <summary>
        /// 外部变量设置
        /// </summary>
        public float defaultRate = 0.5f; // 默认比例
        public float minRate = 0.1f; // 最小比例（左部分）
        public float maxRate = 0.9f; // 最大比例（左部分）

        /// <summary>
        /// 当前比例
        /// </summary>
        float rate;
        RectTransform rect;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            rate = defaultRate;
            rect = transform as RectTransform;
            container = container ?? transform.parent as RectTransform;
        }

        /*
        public void Awake() {
            left = this.transform.parent.GetChild(0).GetComponent<RectTransform>();
            right = this.transform.parent.GetChild(1).GetComponent<RectTransform>();
            var rt = gameObject.GetComponent<RectTransform>();
            parent = this.transform.parent.GetComponent<RectTransform>();
            width = (int)parent.sizeDelta.x;
            height = (int)parent.sizeDelta.y;
            leftPadding = (int)parent.position.x - width / 2;
            left.sizeDelta = new Vector2(rt.position.x - leftPadding - rt.sizeDelta.x / 2, left.sizeDelta.y);
            right.sizeDelta = new Vector2(width - left.sizeDelta.x - rt.sizeDelta.x, right.sizeDelta.y);
        }
        */

        #endregion

        #region 数据处理

        /// <summary>
        /// 设置比率
        /// </summary>
        /// <param name="rate">比率</param>
        public void setRate(float rate) {
            Debug.Log("setRate: " + rate);
            this.rate = Mathf.Clamp(rate, minRate, maxRate);
            requestRefresh();
        }

        #endregion

        #region 画面处理

        /// <summary>
        /// 刷新视窗
        /// </summary>
        protected override void refresh() {
            base.refresh();
            refreshSelfPosition();
            refreshLeftViewSize();
            refreshRightViewSize();
        }

        /// <summary>
        /// 刷新自身位置
        /// </summary>
        void refreshSelfPosition() {
            var anchor = rect.anchorMin;
            anchor.x = rate; rect.anchorMin = anchor;
            anchor = rect.anchorMax;
            anchor.x = rate; rect.anchorMax = anchor;
        }

        /// <summary>
        /// 刷新左视图尺寸
        /// </summary>
        void refreshLeftViewSize() {
            var anchor = left.anchorMax;
            anchor.x = rate; left.anchorMax = anchor;
            left.sizeDelta = Vector2.zero;
        }

        /// <summary>
        /// 刷新右视图尺寸
        /// </summary>
        void refreshRightViewSize() {
            var anchor = right.anchorMin;
            anchor.x = rate; right.anchorMin = anchor;
            right.sizeDelta = Vector2.zero;
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 拖拽回调
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData) {
            setRate(calcRate(eventData));
        }

        /// <summary>
        /// 计算比率
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns></returns>
        float calcRate(PointerEventData eventData) {
            // transform the screen point to world point int rectangle
            Vector2 realPos = SceneUtils.screen2Local(
				eventData.position, container, eventData.pressEventCamera);
            // 成功输出的 realPos 为鼠标在 container 内的相对于自己的 RectTransform 的 pivot 点的坐标差

            // rect.position 为 自身 RectTransform 左下角坐标相对于 pivot 坐标的坐标差
            // realPos - rect.position 即可得出鼠标位置在 container 下相对于其左下角的坐标
            realPos -= container.rect.position;

            // 返回比率
            return realPos.x / container.rect.width;
        }
        /*
        private void SetDraggedPosition(PointerEventData eventData) {
            var rt = gameObject.GetComponent<RectTransform>();
            // transform the screen point to world point int rectangle
            Vector3 globalMousePos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, eventData.position, eventData.pressEventCamera, out globalMousePos)) {
                globalMousePos.y = rt.position.y;
                rt.position = globalMousePos;
                float rectWidth = rt.position.x - leftPadding - rt.sizeDelta.x / 2;
                if (rectWidth <= 0) rectWidth = 0;
                if (rectWidth >= width) rectWidth = width;
                left.sizeDelta = new Vector2(rectWidth, left.sizeDelta.y);
                right.sizeDelta = new Vector2(width - left.sizeDelta.x - rt.sizeDelta.x, right.sizeDelta.y);
            }
        }
        */

        #endregion
    }

}
