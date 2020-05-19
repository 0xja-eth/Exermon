
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

using QuestionModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.Common.Controls.QuestionDisplay {

    /// <summary>
    /// 题目小图片显示
    /// </summary
    public class QuesPictureDisplay :
        SelectableItemDisplay<Question.Picture> {

        /// <summary>
        /// 常量定义
        /// </summary>
        const string FreezeFormat = "冻结回合：{0}";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Text alph;
        public Image picture; // 图片

        #region 界面控制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="picture">图片</param>
        protected override void drawExactlyItem(Question.Picture picture) {
            base.drawExactlyItem(picture);
            this.picture.gameObject.SetActive(true);
            this.picture.overrideSprite = AssetLoader.generateSprite(picture.data);
            this.picture.preserveAspect = true;

            alph.text = (index + 1).ToString();
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