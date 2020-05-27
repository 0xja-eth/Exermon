
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;
using Core.UI;

using ExerPro.EnglishModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.MapScene.Controls {

    /// <summary>
    /// 据点显示控件
    /// </summary
    public class MapNodeDisplay :
        SelectableItemDisplay<ExerProMapNode> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Image icon;

        /// <summary>
        /// 外部变量设置
        /// </summary>
        public Texture2D[] icons; // 每种据点的图标

        #region 界面控制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="item">题目</param>
        protected override void drawExactlyItem(ExerProMapNode item) {
            base.drawExactlyItem(item);
            icon.gameObject.SetActive(true);
            icon.overrideSprite = AssetLoader.
                generateSprite(icons[(int)item.type]);
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem() {
            icon.gameObject.SetActive(false);
        }

        #endregion

    }
}