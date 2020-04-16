using System;
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

using BattleModule.Data;

using UI.BattleScene.Controls.ItemDisplays;

/// <summary>
/// 对战匹配场景控件
/// </summary>
namespace UI.BattleScene.Controls.Animators {

    /// <summary>
    /// 玩家准备动作
    /// </summary>
    public class BattlerPrepareAction : BattlerQuestedStatus {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public UsingItemDisplay itemDisplay;
        
        #region 界面绘制

        /// <summary>
        /// 绘制具体物品
        /// </summary>
        /// <param name="battler">对战玩家</param>
        protected override void drawExactlyItem(RuntimeBattlePlayer battler) {
            base.drawExactlyItem(battler);
            itemDisplay.setItem(battler.roundItem());
        }

        /// <summary>
        /// 清空物品
        /// </summary>
        protected override void clearItem() {
            base.clearItem();
            itemDisplay.requestClear(true);
        }

        #endregion
    }
}
