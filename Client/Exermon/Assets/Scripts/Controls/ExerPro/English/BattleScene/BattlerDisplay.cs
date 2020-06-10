
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;
using Core.UI;

using ExerPro.EnglishModule.Data;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.ParamDisplays;

namespace UI.ExerPro.EnglishPro.BattleScene.Controls {

    /// <summary>
    /// 据点显示控件
    /// </summary
    public class BattlerDisplay :
        SelectableItemDisplay<RuntimeBattler> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Image full;

        public Animation animation;

        //public Text name;
        //public MultParamsDisplay expBar;

        #region 数据控制

        #endregion

        #region 界面控制

        #region 状态控制
        
        #endregion

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="item">题目</param>
        protected override void drawExactlyItem(RuntimeBattler item) {
            base.drawExactlyItem(item);

        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem() {
            full.gameObject.SetActive(false);
        }

        #endregion

    }
}