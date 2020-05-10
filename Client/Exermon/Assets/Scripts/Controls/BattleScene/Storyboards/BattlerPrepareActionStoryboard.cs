using System;
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

using GameModule.Services;

using ItemModule.Data;
using PlayerModule.Data;
using QuestionModule.Data;
using BattleModule.Data;

using UI.BattleScene.Controls.ItemDisplays;

/// <summary>
/// 对战匹配场景控件
/// </summary>
namespace UI.BattleScene.Controls.Storyboards {

    /// <summary>
    /// 玩家准备动作
    /// </summary>
    public class BattlerPrepareActionStoryboard : 
        BattlerQuestedStoryboard {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public UsingItemDisplay itemDisplay;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            //selfWindow.addChangeEvent(
            //    selfWindow.shownState, onShown);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 获取效果转化对象
        /// </summary>
        /// <returns></returns>
        IEffectsConvertable getEffectConvertableItem() {
            var item = this.item.roundItem();
            if (item == null) return null;

            switch ((BaseItem.Type)item.type) {
                case BaseItem.Type.HumanItem:
                    return (HumanItem)item;
                case BaseItem.Type.QuesSugar:
                    return (QuesSugar)item;
                default: return null;
            }
        }

        #endregion

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
        protected override void drawEmptyItem() {
            base.drawEmptyItem();
            itemDisplay.requestClear(true);
        }

        #endregion

        #region 动画事件

        /// <summary>
        /// 物品使用效果
        /// </summary>
        public void onItemUse() {
            var effectItem = getEffectConvertableItem();
            if (effectItem == null) return;

            itemDisplay.showEffect = true;

            CalcService.BattleItemEffectProcessor.process(item, effectItem);

            requestRefresh(true);
        }

        /// <summary>
        /// 完全显示
        /// </summary>
        public void onShown() {
            selfWindow.terminateWindow();
        }

        #endregion
    }
}
