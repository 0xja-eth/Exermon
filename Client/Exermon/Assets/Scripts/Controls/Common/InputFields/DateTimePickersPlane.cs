
using UnityEngine;
using UnityEngine.EventSystems;

using Core.UI;

namespace UI.Common.Controls.InputFields {

    /// <summary>
    /// 日期选择器
    /// </summary>
    public class DateTimePickersPlane : BaseWindow,
    IPointerEnterHandler, IPointerExitHandler {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public DateTimeField field;

        /// <summary>
        /// 内部变量设置
        /// </summary>
        bool enter = false;

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            updateCancel();
        }

        /// <summary>
        /// 更新取消事件
        /// </summary>
        void updateCancel() {
            if (!enter && (
                Input.GetMouseButtonDown(0) ||
                Input.GetMouseButtonDown(1) ||
                Input.touchCount > 0))
                field.endSelect();
        }

        #endregion

        #region 事件控制

        /// <summary>
        /// 指针进入事件
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public void OnPointerEnter(PointerEventData eventData) {
            enter = true;
        }

        /// <summary>
        /// 指针离开事件
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public void OnPointerExit(PointerEventData eventData) {
            enter = false;
        }

        #endregion
    }
}