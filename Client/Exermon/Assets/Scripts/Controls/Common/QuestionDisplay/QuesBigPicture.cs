
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

using UI.Common.Controls.ItemDisplays;

namespace UI.Common.Controls.QuestionDisplay {

    /// <summary>
    /// 题目大图片显示
    /// </summary>
    public class QuesBigPicture : ItemDetailDisplay<Texture2D> {

        /// <summary>
        /// 常量定义
        /// </summary>
        const string ImageTextFormat = "图片{0}";

        /// <summary>
        /// 外部变量定义
        /// </summary>
        public Text alph;
        public Image picture; // 图片

        /// <summary>
        /// 内部变量定义
        /// </summary>

        #region 界面控制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="texture">图片</param>
        protected override void drawExactlyItem(Texture2D texture) {
            base.drawExactlyItem(texture);
            picture.gameObject.SetActive(true);
            picture.preserveAspect = true;
            picture.overrideSprite = AssetLoader.generateSprite(texture);

            alph.text = string.Format(ImageTextFormat, index + 1);
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void clearItem() {
            picture.gameObject.SetActive(false);
            alph.text = "";
        }

        #endregion

    }
}