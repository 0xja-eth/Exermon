using PlayerModule.Data;

using UI.Common.Controls.ItemDisplays;

/// <summary>
/// 右窗口控件
/// </summary>
namespace UI.BattleStartScene.Controls.Right { }

/// <summary>
/// 右窗口物品页控件
/// </summary>
namespace UI.BattleStartScene.Controls.Right.ItemContent {

        /// <summary>
        /// 右窗口物品页面
        /// </summary>
        public class ItemContent : ItemDetailDisplay<Player> {

        /// <summary>
        /// 外部变量定义
        /// </summary>
        public ItemPackDisplay itemPackDisplay;
        public PackItemDetail packItemDetail;

        #region 画面绘制

        /// <summary>
        /// 刷新视窗
        /// </summary>
        protected override void drawExactlyItem(Player player) {
            base.drawExactlyItem(player);

            var humanPack = player.packContainers.humanPack;
            itemPackDisplay.setPackData(humanPack);
        }

        #endregion

    }
}