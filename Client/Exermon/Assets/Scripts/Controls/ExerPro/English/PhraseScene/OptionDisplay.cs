using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Common.Controls.ItemDisplays;
using UnityEngine;

namespace Assets.Scripts.Controls.ExerPro.English.PhraseScene {
    class OptionDisplay :
        SelectableItemDisplay<string> {

        #region 界面绘制

        /// <summary>
        /// 绘制物品
        /// </summary>
        /// <param name="item"></param>
        protected override void drawExactlyItem(string item) {
            base.drawExactlyItem(item);
        }
        #endregion
    }
}
