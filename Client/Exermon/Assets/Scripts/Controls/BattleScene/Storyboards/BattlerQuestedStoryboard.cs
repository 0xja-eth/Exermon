using System;
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

using BattleModule.Data;

/// <summary>
/// 对战匹配场景控件
/// </summary>
namespace UI.BattleScene.Controls.Storyboards {

    /// <summary>
    /// 玩家准备显示
    /// </summary>
    public class  BattlerQuestedStoryboard : BattlerPrepareStoryboard {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Image exermon;

        //public Image statusBackground;
        //public Texture2D correctBackground, wrongBackground;

        public GameObject humanBlackLine, exermonBlackLine;
        public GameObject lightEffect;

        #region 界面绘制

        /// <summary>
        /// 绘制具体物品
        /// </summary>
        /// <param name="battler">对战玩家</param>
        protected override void drawExactlyItem(RuntimeBattlePlayer battler) {
            base.drawExactlyItem(battler);

            var exermon = battler.currentExermon().exermon();
            this.exermon.gameObject.SetActive(true);
            this.exermon.overrideSprite = AssetLoader.generateSprite(exermon.full);

            //var texture = correct ? correctBackground : wrongBackground;
            //statusBackground.overrideSprite = AssetLoader.generateSprite(texture);
            /*
            if (humanBlackLine) humanBlackLine.SetActive(!correct);
            if (exermonBlackLine) exermonBlackLine.SetActive(!correct);
            if (lightEffect) lightEffect.SetActive(correct);
            */
        }

        /// <summary>
        /// 清空物品
        /// </summary>
        protected override void clearItem() {
            base.clearItem();
            exermon.gameObject.SetActive(false);
            
            if (humanBlackLine) humanBlackLine.SetActive(false);
            if (exermonBlackLine) exermonBlackLine.SetActive(false);
            if (lightEffect) lightEffect.SetActive(false);
        }
        
        #endregion
    }
}
