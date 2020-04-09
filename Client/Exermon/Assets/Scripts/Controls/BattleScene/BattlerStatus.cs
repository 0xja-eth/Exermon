using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using PlayerModule.Data;
using BattleModule.Data;
using BattleModule.Services;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.ParamDisplays;

/// <summary>
/// 对战匹配场景控件
/// </summary>
namespace UI.BattleScene.Controls {

    /// <summary>
    /// 对战玩家显示
    /// </summary>
    public class BattlerStatus : ItemDisplay<RuntimeBattlePlayer>{

        /// <summary>
        /// 常量定义
        /// </summary>
        public const string HPFormat = "HP {0}/{1}";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Image face;
        public Text name, hpText;
        public Image hpBar;

        #region 数据控制

        /// <summary>
        /// 设置人物
        /// </summary>
        /// <param name="player">人物对象</param>
        public void setItem(Player player) {
            setItem(new RuntimeBattlePlayer(player));
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="battler">对战者</param>
        protected override void drawExactlyItem(RuntimeBattlePlayer battler) {
            base.drawExactlyItem(battler);
            drawBust(battler);
            drawBaseInfo(battler);
        }

        /// <summary>
        /// 绘制半身像
        /// </summary>
        /// <param name="battler">对战者</param>
        void drawBust(RuntimeBattlePlayer battler) {
            face.gameObject.SetActive(true);
            face.overrideSprite = battler.character().face;
        }

        /// <summary>
        /// 绘制基础信息
        /// </summary>
        /// <param name="battler">对战者</param>
        void drawBaseInfo(RuntimeBattlePlayer battler) {
            int max = battler.mhp, val = battler.hp;
            float rate = val * 1.0f / max;

            name.text = battler.name;
            hpText.text = string.Format(HPFormat, val, max);
            hpBar.fillAmount = rate;
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void clearItem() {
            face.gameObject.SetActive(false);
            name.text = hpText.text = "";
            hpBar.fillAmount = 0;
        }

        #endregion
    }
}
