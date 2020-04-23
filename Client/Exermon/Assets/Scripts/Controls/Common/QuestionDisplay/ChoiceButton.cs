
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

/// <summary>
/// 对战界面刷题窗口控件
/// </summary>
namespace UI.Common.Controls.QuestionDisplay {

    using Question = QuestionModule.Data.Question;

    /// <summary>
    /// 选项按钮显示
    /// </summary
    public class ChoiceButton : QuesChoiceDisplay {

        /// <summary>
        /// 常量定义
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Texture2D[] textures; // 每个选项的按钮图片

        public Image image, override_;

        #region 界面控制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="choice">选项</param>
        protected override void drawExactlyItem(Question.Choice choice) {
            base.drawExactlyItem(choice);
            image.gameObject.SetActive(true);
            override_.gameObject.SetActive(true);
            override_.overrideSprite = image.overrideSprite = 
                AssetLoader.generateSprite(textures[index]);
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void clearItem() {
            base.clearItem();
            image.gameObject.SetActive(false);
            override_.gameObject.SetActive(false);
        }

        #endregion

    }
}