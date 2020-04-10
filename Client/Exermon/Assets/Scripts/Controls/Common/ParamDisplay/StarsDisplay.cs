
using UnityEngine;
using UnityEngine.UI;

using Core.UI;

namespace UI.Common.Controls.ParamDisplays {

    /// <summary>
    /// 普通星星显示
    /// </summary>
    public class StarsDisplay : GroupView<Image> {

        /// <summary>
        /// 星星数
        /// </summary>
        int count = -1;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            registerUpdateLayout(container);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 获取星星数目
        /// </summary>
        /// <returns></returns>
        public int getValue() {
            return count;
        }

        /// <summary>
        /// 设置星星数目
        /// </summary>
        /// <param name="count">数目</param>
        public void setValue(int count) {
            if (this.count == count) return;
            this.count = count;
            requestRefresh();
        }

        /// <summary>
        /// 清空值
        /// </summary>
        public void clearValue() {
            setValue(0);
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 刷新子视图
        /// </summary>
        /// <param name="sub">子视图</param>
        protected override void refreshSubView(Image sub, int index) {
            sub.gameObject.SetActive(index < count);
        }

        #endregion

    }
}