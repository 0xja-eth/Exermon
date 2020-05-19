

using UnityEngine;
using UnityEngine.UI;

using UI.Common.Controls.ItemDisplays;

using UI.Common.Controls.SystemExtend.PaintableImage;

namespace UI.Common.Controls.QuestionDisplay.DrawView {

    /// <summary>
    /// 颜色类封装
    /// </summary>
    public class ColorRef {

        /// <summary>
        /// 颜色
        /// </summary>
        public Color color;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="color"></param>
        public ColorRef(Color color) { this.color = color; }
    }

    /// <summary>
    /// 选项按钮容器
    /// </summary>
    public class DrawView : SelectableContainerDisplay<ColorRef> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public PaintableImage image;

        /// <summary>
        /// 外部变量定义
        /// </summary>
        public Color[] colors;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            setItems(generateColorRefs());
            select(0);
        }

        /// <summary>
        /// 清空物品
        /// </summary>
        public override void clearItems() { }

        /// <summary>
        /// 生成封装的颜色数据
        /// </summary>
        /// <returns></returns>
        ColorRef[] generateColorRefs() {
            var len = this.colors.Length;
            var colors = new ColorRef[len];
            for (int i = 0; i < len; ++i)
                colors[i] = new ColorRef(this.colors[i]);
            return colors;
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 颜色改变回调
        /// </summary>
        protected override void onSelectChanged() {
            base.onSelectChanged();
            var color = selectedItem();
            image.lineColor = color == null ?
                new Color(0, 0, 0, 1) : color.color;
        }

        #endregion

        #region 画面绘制
        /*
        /// <summary>
        /// 清除
        /// </summary>
        protected override void clear() {
            base.clear(); image.clear();
        }
        */
        #endregion
    }
}