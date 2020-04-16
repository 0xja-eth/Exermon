using System;
using System.Collections.Generic;

using UnityEngine.UI;
using UnityEngine.Events;

using ItemModule.Data;

using PlayerModule.Data;
using QuestionModule.Data;

using BattleModule.Data;
using BattleModule.Services;

using UI.Common.Controls.ItemDisplays;

namespace UI.BattleScene.Controls.ItemDisplays {

    /// <summary>
    /// 对战物资槽物品显示
    /// </summary
    public class UsedItemDisplay : BaseItemDisplay {

        #region 初始化

        /// <summary>
        /// 初始化绘制函数
        /// </summary>
        protected override void initializeDrawFuncs() {
            registerItemType<HumanItem>(drawHumanItem);
            registerItemType<QuesSugar>(drawQuesSugar);
        }

        #endregion

        #region 界面控制
        
        /// <summary>
        /// 绘制人类物品
        /// </summary>
        /// <param name="item"></param>
        protected virtual void drawHumanItem(HumanItem item) {
            name.text = item.name;
            icon.gameObject.SetActive(true);
            icon.overrideSprite = item.icon;
        }

        /// <summary>
        /// 绘制题目糖
        /// </summary>
        /// <param name="item"></param>
        protected virtual void drawQuesSugar(QuesSugar item) { }

        #endregion
    }
}