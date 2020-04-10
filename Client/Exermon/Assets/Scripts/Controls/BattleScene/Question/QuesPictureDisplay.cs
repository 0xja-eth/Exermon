
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

using UI.Common.Controls.ItemDisplays;

namespace UI.BattleScene.Controls.Question {

    /// <summary>
    /// 题目小图片显示
    /// </summary
    public class QuesPictureDisplay :
        SelectableItemDisplay<Texture2D> {

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
        /// <param name="texture">图片</param>
        protected override void drawExactlyItem(Texture2D texture) {
            base.drawExactlyItem(texture);
            picture.gameObject.SetActive(true);
            picture.preserveAspect = true;
            picture.overrideSprite = AssetLoader.generateSprite(texture);

            alph.text = ((char)('A' + index)).ToString();
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