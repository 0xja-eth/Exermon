using UnityEngine;
using UnityEngine.UI;

using Core.UI;

/// <summary>
/// 系统拓展控件
/// </summary>
namespace UI.Common.Controls.SystemExtend.ToggleExtend {
    public class ToggleFlag : BaseView {

        /// <summary>
        /// 所属的 Toggle
        /// </summary>
        public Toggle toggle;
        public GameObject flagObject;
        public bool isOnFlag = true; // 是否 isOn 时候出现

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            if (!flagObject) flagObject = gameObject;
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            if (!toggle) return;
            if (isOnFlag) flagObject.SetActive(toggle.isOn);
            else flagObject.SetActive(!toggle.isOn);
        }
    }
}
