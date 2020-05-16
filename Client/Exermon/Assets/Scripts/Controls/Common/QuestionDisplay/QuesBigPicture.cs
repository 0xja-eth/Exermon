
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

using QuestionModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.Common.Controls.QuestionDisplay {

    /// <summary>
    /// 题目大图片显示
    /// </summary>
    public class QuesBigPicture : ItemDetailDisplay<Question.Picture> {

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
        /// <param name="picture">图片</param>
        protected override void drawExactlyItem(Question.Picture picture) {
            base.drawExactlyItem(picture);

            this.picture.gameObject.SetActive(true);
            this.picture.overrideSprite = AssetLoader.generateSprite(picture.data);
            this.picture.SetNativeSize();

            alph.text = string.Format(ImageTextFormat, index + 1);
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem() {
            picture.gameObject.SetActive(false);
            alph.text = "";
        }

        #endregion

    }
}