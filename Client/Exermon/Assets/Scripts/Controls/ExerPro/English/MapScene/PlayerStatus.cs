
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;
using Core.UI.Utils;

using ExermonModule.Data;

using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.ParamDisplays;

/// <summary>
/// 地图场景控件
/// </summary>
namespace UI.ExerPro.EnglishPro.MapScene.Controls {

    /// <summary>
    /// 玩家显示组件
    /// </summary>
    public class PlayerStatus : ItemDisplay<RuntimeActor> {

        /// <summary>
        /// HP文本格式
        /// </summary>
        const string HPTextFormat = "HP：{0}/{1}";

        /// <summary>
        /// 外部组件定义
        /// </summary>
        public Image full; // 艾瑟萌全身像
        public Text nickname, hp; // 艾瑟萌昵称
        public MultParamsDisplay hpBar;
        
        #region 界面绘制

        /// <summary>
        /// 是否空物品
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool isNullItem(RuntimeActor item) {
            return base.isNullItem(item) || item.slotItem == null || item.slotItem.isNullItem();
        }

        /// <summary>
        /// 绘制物品
        /// </summary>
        /// <param name="item"></param>
        protected override void drawExactlyItem(RuntimeActor item) {
            base.drawExactlyItem(item);

            drawStatus(item);
            drawPlayerExer(item.slotItem.playerExer);
        }

        /// <summary>
        /// 绘制状态
        /// </summary>
        void drawStatus(RuntimeActor item) {
            hp.text = string.Format(HPTextFormat, item.hp, item.mhp());
            hpBar.setValue(item, "hp");
        }

        /// <summary>
        /// 绘制玩家艾瑟萌
        /// </summary>
        /// <param name="playerExer"></param>
        void drawPlayerExer(PlayerExermon playerExer) {
            nickname.text = playerExer.nickname;
            drawExermon(playerExer.exermon());
        }

        /// <summary>
        /// 绘制玩家信息
        /// </summary>
        void drawExermon(Exermon exermon) {
            full.gameObject.SetActive(true);
            full.overrideSprite = AssetLoader.generateSprite(exermon.full);
        }

        /// <summary>
        /// 绘制空物品
        /// </summary>
        protected override void drawEmptyItem() {
            base.drawEmptyItem();
            full.gameObject.SetActive(false);
            nickname.text = "";
        }

        #endregion

    }
}
