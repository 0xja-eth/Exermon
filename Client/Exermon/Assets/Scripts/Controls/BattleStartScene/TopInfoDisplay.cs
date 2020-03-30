using UnityEngine.UI;

using PlayerModule.Data;
using SeasonModule.Data;

using SeasonModule.Services;

using UI.Common.Controls.ParamDisplays;
using UI.Common.Controls.ItemDisplays;

namespace UI.BattleStartScene.Controls {

    /// <summary>
    /// 顶部信息显示
    /// </summary>
    public class TopInfoDisplay : ItemDetailDisplay<Player> {

        /// <summary>
        /// 赛季时间格式
        /// </summary>
        const string DateFormat = "yyyy.MM";
        const string SeasonTimeFormat = "({0} - {1})";

        /// <summary>
        /// 外部变量定义
        /// </summary>
        public CreditDisplay credit;
        public Text name, time;

        /// <summary>
        /// 内部系统定义
        /// </summary>
        SeasonService seasonSer;

        #region 初始化

        /// <summary>
        /// 初始化内部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            seasonSer = SeasonService.get();
        }

        #endregion

        #region 画面绘制

        /// <summary>
        /// 绘制玩家
        /// </summary>
        /// <param name="player">玩家</param>
        protected override void drawExactlyItem(Player player) {
            base.drawExactlyItem(player);
            var season = seasonSer.currentSeason();

            credit.setValue(player.battleInfo.credit);

            if (season != null) {
                name.text = season.name;
                time.text = generateSeasonTimeText(season);
            }
        }

        /// <summary>
        /// 生成赛季时间文本
        /// </summary>
        /// <param name="season">赛季实例</param>
        /// <returns>返回赛季时间文本</returns>
        string generateSeasonTimeText(CompSeason season) {
            var start = season.startTime.ToString(DateFormat);
            var end = season.endTime.ToString(DateFormat);
            return string.Format(SeasonTimeFormat, start, end);
        }

        #endregion

    }
}