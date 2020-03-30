
using UnityEngine;
using UnityEngine.UI;

using Core.UI;

namespace UI.Common.Controls.ParamDisplays {

    /// <summary>
    /// 信誉积分显示
    /// </summary>
    public class CreditDisplay : GroupView<Image> {

        /// <summary>
        /// 信誉积分最大值
        /// </summary>
        const int MaxCredit = 100;

        /// <summary>
        /// 外部变量设置
        /// </summary>
        public Texture2D on, off; // 开启/关闭状态下的图片

        /// <summary>
        /// 信誉积分
        /// </summary>
        int credit = 0;

        #region 数据控制

        /// <summary>
        /// 获取信誉值
        /// </summary>
        /// <returns></returns>
        public int getValue() {
            return credit;
        }

        /// <summary>
        /// 设置信誉值
        /// </summary>
        /// <param name="count">数目</param>
        public void setValue(int credit) {
            if (this.credit == credit) return;
            this.credit = credit;
            requestRefresh();
        }

        /// <summary>
        /// 清空值
        /// </summary>
        public void clearValue() {
            setValue(0);
        }

        /// <summary>
        /// 获取显示数目
        /// </summary>
        /// <returns></returns>
        int showCount() {
            var max = subViewsCount();
            return (int)((credit*1.0 / MaxCredit) * max);
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 刷新子视图
        /// </summary>
        /// <param name="sub">子视图</param>
        protected override void refreshSubView(Image sub, int index) {
            var block = index <= showCount() ? on : off;
            var rect = new Rect(0, 0, block.width, block.height);

            sub.overrideSprite = Sprite.Create(
                block, rect, new Vector2(0.5f, 0.5f));
        }

        #endregion

    }
}