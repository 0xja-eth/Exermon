using System;
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

using GameModule.Services;

using ItemModule.Data;
using PlayerModule.Data;
using ExermonModule.Data;
using QuestionModule.Data;
using BattleModule.Data;

using UI.BattleScene.Controls.ItemDisplays;

namespace UI.BattleScene.Controls.Storyboards {

    /// <summary>
    /// 玩家攻击动作
    /// </summary>
    public class BattlerAttackActionStoryboard : 
        BattlerQuestedStoryboard {

        /// <summary>
        /// 常量定义
        /// </summary>
        static readonly Color[] HitTypeColors =
            new Color[] { default,
            new Color(1, 0.1647059f, 0.3921569f), // HPDamage
            new Color(0, 0.2526757f, 0.787f), // HPRecover
            new Color(0.5380687f, 0.099f, 0.6792453f), // HPDrain
            new Color(1, 0.1647059f, 0.3921569f), // MPDamage
            new Color(0, 0.2526757f, 0.787f), // MPRecover
            new Color(0.5380687f, 0.099f, 0.6792453f), // MPDrain
        };

        static readonly string[] HitTypeFormats =
            new string[] { "",
                "-{0}HP", "+{0}HP", "{0}HP",
                "-{0}MP", "+{0}MP", "{0}MP",
        };

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Text hurtText;

        #region 数据控制
        
        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制具体物品
        /// </summary>
        /// <param name="battler">对战玩家</param>
        protected override void drawExactlyItem(RuntimeBattlePlayer battler) {
            base.drawExactlyItem(battler);

            var oppo = battler.getOppo();
            if (oppo == null) clearAction();
            else drawAction(battler);
        }

        /// <summary>
        /// 绘制半身像
        /// </summary>
        /// <param name="battler">对战者</param>
        protected override void drawFace(RuntimeBattlePlayer battler) {

            var bust = AssetLoader.getCharacterBustSprite(battler.characterId, 0);
            face.gameObject.SetActive(true);
            face.overrideSprite = bust;
        }

        /// <summary>
        /// 绘制行动
        /// </summary>
        /// <param name="action">行动</param>
        /// <param name="battler">对战者</param>
        void drawAction(RuntimeBattlePlayer battler) {

            var oppo = battler.getOppo();
            var action = battler.attackAction() ?? oppo.attackAction();

            var type = action.hitType();
            var color = HitTypeColors[type];
            var format = getHurtFormat(action, battler);
            var hurt = action.hurt.ToString();
            // hurt = (action.hurt > 0 ? ("+" + hurt) : hurt);

            hurtText.color = color;
            hurtText.text = string.Format(format, hurt);
        }

        /// <summary>
        /// 获取伤害显示格式
        /// </summary>
        /// <param name="action">行动</param>
        /// <param name="battler">对战者</param>
        /// <returns>返回伤害显示格式</returns>
        string getHurtFormat(RuntimeAction action, RuntimeBattlePlayer battler = null) {
            if (action == null) return "";
            if (battler == null) battler = item;

            var oppo = battler.getOppo();

            var type = action.hitType();
            var format = HitTypeFormats[type];

            switch ((ExerSkill.HitType)type) {
                case ExerSkill.HitType.HPDamage:
                case ExerSkill.HitType.MPDamage:
                case ExerSkill.HitType.HPRecover:
                case ExerSkill.HitType.MPRecover:
                    // 对方是发动者，即己方为目标方时显示
                    return action.player == oppo ? format : "";
                case ExerSkill.HitType.HPDrain:
                    // 对方是发动者，即己方为目标方时显示伤害，对方显示回复
                    return action.player == oppo ?
                        HitTypeFormats[1] : HitTypeFormats[4];
                case ExerSkill.HitType.MPDrain:
                    // 对方是发动者，即己方为目标方时显示伤害，对方显示回复
                    return action.player == oppo ?
                        HitTypeFormats[2] : HitTypeFormats[5];
            }

            return format;
        }

        /// <summary>
        /// 清除行动显示
        /// </summary>
        void clearAction() {
            hurtText.text = "";
        }

        /// <summary>
        /// 清空物品
        /// </summary>
        protected override void drawEmptyItem() {
            base.drawEmptyItem();
            clearAction();
        }

        #endregion
    }
}
