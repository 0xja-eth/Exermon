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
namespace UI.BattleMatchingScene.Controls {

    /// <summary>
    /// 对战玩家显示
    /// </summary>
    public class BattlerDisplay : ItemDisplay<RuntimeBattlePlayer>{

        /// <summary>
        /// 常量定义
        /// </summary>
        public const string UnknownName = "旗鼓相当的对手";
        public const string LevelFormat = "Lv. {0}";
        public const string ProgressFormat = "{0}%";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Image bust;
        public GameObject unknown;
        public Text name, level, progress;
        public SmallRankDisplay rankDisplay;

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
            drawProgress(battler);
        }

        /// <summary>
        /// 绘制半身像
        /// </summary>
        /// <param name="battler">对战者</param>
        void drawBust(RuntimeBattlePlayer battler) {
            if (unknown) unknown.SetActive(false);
            bust.overrideSprite = battler.character().bust;
        }

        /// <summary>
        /// 绘制基础信息
        /// </summary>
        /// <param name="battler">对战者</param>
        void drawBaseInfo(RuntimeBattlePlayer battler) {
            rankDisplay.setStarNum(battler.starNum);
            name.text = battler.name;
            level.text = string.Format(LevelFormat, battler.level);
        }

        /// <summary>
        /// 绘制进度
        /// </summary>
        /// <param name="battler">对战者</param>
        void drawProgress(RuntimeBattlePlayer battler) {
            if (battler.progress >= 0)
                progress.text = string.Format(ProgressFormat, battler.progress);
            else
                progress.text = "";
        }

        /// <summary>
        /// 绘制空物品
        /// </summary>
        protected override void drawEmptyItem() {
            if(unknown) unknown.SetActive(true);
            bust.gameObject.SetActive(false);
            rankDisplay.requestClear(true);
            name.text = UnknownName;
            progress.text = level.text = "";
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void clearItem() {
            if (unknown) unknown.SetActive(false);
            bust.gameObject.SetActive(false);
            rankDisplay.requestClear(true);
            name.text = progress.text = level.text = "";
        }

        #endregion
    }
}
