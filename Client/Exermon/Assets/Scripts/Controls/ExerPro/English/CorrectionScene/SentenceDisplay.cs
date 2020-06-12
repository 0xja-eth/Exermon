using Core.UI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Common.Controls.ItemDisplays;
using UnityEngine;

namespace UI.ExerPro.EnglishPro.CorrectionScene.Controls
{

    /// <summary>
    /// 句子
    /// </summary
    public class SentenceDisplay : ItemDisplay<string>
    {
        

        /// <summary>
        /// 开启视图
        /// </summary>
        /// <param name="item"></param>
        public override void startView(string item)
        {
            base.startView(item);
        }
        

        /// <summary>
        /// 绘制物品
        /// </summary>
        /// <param name="item"></param>
        protected override void drawExactlyItem(string item)
        {
            base.drawExactlyItem(item);
            SceneUtils.get<SentenceContainer>(gameObject).setItem(item);
        }
    }
}
