
using UnityEngine;
using UnityEngine.EventSystems;

using Core.UI;

namespace UI.Common.Windows {

	/// <summary>
	/// 反转窗口，点击窗口外触发onCancel
	/// </summary>
	public class ToggleWindow : BaseWindow,
    IPointerEnterHandler, IPointerExitHandler {

        /// <summary>
        /// 外部组件设置
        /// </summary>

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
			if (!enter && isPointerDown()) onCancel();
		}

		/// <summary>
		/// 指针是否按下
		/// </summary>
		/// <returns></returns>
		bool isPointerDown() {
			return Input.GetMouseButtonDown(0) ||
				Input.GetMouseButtonDown(1) ||
				Input.touchCount > 0;
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

		/// <summary>
		/// 取消回调
		/// </summary>
		protected virtual void onCancel() {
			terminateWindow();
		}

        #endregion
    }
}