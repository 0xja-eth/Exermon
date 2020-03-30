using PlayerModule.Data;

using UI.Common.Controls.ItemDisplays;

/// <summary>
/// 右窗口主内容页
/// </summary>
namespace UI.BattleStartScene.Controls.Right.MainContent {

    /// <summary>
    /// 右窗口主页面
    /// </summary>
    public class MainContent : ItemDetailDisplay<Player> {

        /// <summary>
        /// 外部变量定义
        /// </summary>
        public RankDisplay rankDisplay;
        
        #region 画面绘制

        /// <summary>
        /// 刷新视窗
        /// </summary>
        protected override void drawExactlyItem(Player player) {
            base.drawExactlyItem(player);

            var rec = player.seasonRecord;
            rankDisplay.setItem(rec);
        }

        #endregion

    }
}