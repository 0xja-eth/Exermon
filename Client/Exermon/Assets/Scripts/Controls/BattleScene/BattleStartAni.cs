using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

using PlayerModule.Data;
using BattleModule.Data;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.ParamDisplays;

using UI.BattleMatchingScene.Controls;

/// <summary>
/// 对战匹配场景控件
/// </summary>
namespace UI.BattleScene.Controls {

    /// <summary>
    /// 对战开始动画
    /// </summary>
    public class BattleStartAni : ItemDisplay<RuntimeBattle>{

        /// <summary>
        /// 常量定义
        /// </summary>
        
        /// <summary>
        /// 外部组件设置
        /// </summary>
        public BattlerDisplay selfPlayer, oppoPlayer;

        public new Animation animation;

        #region 数据控制
        
        /// <summary>
        /// 是否正确
        /// </summary>
        /// <returns>返回是否正确</returns>
        public bool isAnimationEnd() {
            return shown && !animation.isPlaying;
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="battle">对战者</param>
        protected override void drawExactlyItem(RuntimeBattle battle) {
            base.drawExactlyItem(battle);
            selfPlayer.setItem(battle.self());
            oppoPlayer.setItem(battle.oppo());
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem() {
            selfPlayer.requestClear(true);
            oppoPlayer.requestClear(true);
        }

        #endregion
    }
}
