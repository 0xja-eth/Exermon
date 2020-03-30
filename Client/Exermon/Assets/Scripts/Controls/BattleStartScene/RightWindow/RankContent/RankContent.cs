using PlayerModule.Data;

using SeasonModule.Services;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.ParamDisplays;

/// <summary>
/// 右窗口排行页面
/// </summary>
namespace UI.BattleStartScene.Controls.Right.RankContent {

    /// <summary>
    /// 右窗口排行页面
    /// </summary>
    public class RankContent : ItemDetailDisplay<Player> {

        /// <summary>
        /// 外部变量定义
        /// </summary>
        public ParamDisplaysGroup rankList;
        public ParamDisplay selfRank;
        public ParamDisplay rankInfo;

        /// <summary>
        /// 外部系统定义
        /// </summary>
        SeasonService seasonSer;

        #region 初始化

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            seasonSer = SeasonService.get();
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制物品
        /// </summary>
        /// <param name="player">物品</param>
        protected override void drawExactlyItem(Player player) {
            base.drawExactlyItem(player);
            rankList.setValues(seasonSer.seasonRank);
            selfRank.setValue(seasonSer.seasonRank.rank);
            rankInfo.setValue(seasonSer.seasonRank);
        }

        #endregion
    }
}