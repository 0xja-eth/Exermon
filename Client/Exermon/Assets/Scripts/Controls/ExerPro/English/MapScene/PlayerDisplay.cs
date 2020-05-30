
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;
using Core.UI.Utils;

using ExermonModule.Data;
using ExerPro.EnglishModule.Data;

using UI.Common.Controls.ItemDisplays;

/// <summary>
/// 地图场景控件
/// </summary>
namespace UI.ExerPro.EnglishPro.MapScene.Controls {

    /// <summary>
    /// 玩家显示组件
    /// </summary>
    public class PlayerDisplay : ItemDisplay<ExerProActor> {

        /// <summary>
        /// 外部组件定义
        /// </summary>
        public Image full; // 艾瑟萌全身像
        public Text nickname; // 艾瑟萌昵称

        #region 界面绘制

        /// <summary>
        /// 是否空物品
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool isNullItem(ExerProActor item) {
            return base.isNullItem(item) && item.slotItem.isNullItem();
        }

        /// <summary>
        /// 绘制物品
        /// </summary>
        /// <param name="item"></param>
        protected override void drawExactlyItem(ExerProActor item) {
            base.drawExactlyItem(item);
            drawPlayerExer(item.slotItem.playerExer);
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
        }

        #endregion

    }
}
