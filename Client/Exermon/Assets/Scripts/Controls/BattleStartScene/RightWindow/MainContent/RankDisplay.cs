
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

using PlayerModule.Data;
using SeasonModule.Data;

using GameModule.Services;
using SeasonModule.Services;

using UI.Common.Controls.ParamDisplays;
using UI.Common.Controls.ItemDisplays;

namespace UI.BattleStartScene.Controls.Right.MainContent {

    /// <summary>
    /// 段位显示
    /// </summary>
    public class RankDisplay : ItemDisplay<SeasonRecord> {

        /// <summary>
        /// 常量定义
        /// </summary>
        const string ScoreDescFormat = "当前段位下，{0}积分可抵消一颗星星的减少";

        /// <summary>
        /// 外部变量定义
        /// </summary>
        public RankStarsDisplay starsDisplay;
        public Text rankName, score, scoreDesc; // 排位名, 赛季积分, 积分说明
        public Image rankIcon;

        /// <summary>
        /// 内部变量定义
        /// </summary>
        CompRank rank; // 段位
        int subRank, restStar; // 子段位编号，剩余星星数

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

        #region 数据控制

        /// <summary>
        /// 更新段位数据
        /// </summary>
        void updateRankData(SeasonRecord record) {
            restStar = CalcService.RankCalc.calc(
                record.starNum, out rank, out subRank);
        }

        #endregion

        #region 画面绘制

        /// <summary>
        /// 绘制记录
        /// </summary>
        /// <param name="record">记录</param>
        protected override void drawExactlyItem(SeasonRecord record) {
            base.drawExactlyItem(record);
            updateRankData(record);
            drawRankStars();
            drawRankIcon();
            drawScore(record);
        }

        /// <summary>
        /// 绘制赛季星级
        /// </summary>
        void drawRankStars() {
            starsDisplay.setValue(restStar);
        }

        /// <summary>
        /// 绘制段位图标
        /// </summary>
        void drawRankIcon() {
            rankName.text = generateRankName();
            rankIcon.gameObject.SetActive(true);
            rankIcon.overrideSprite = rank.icon;
        }

        /// <summary>
        /// 绘制赛季积分
        /// </summary>
        /// <param name="record">记录</param>
        void drawScore(SeasonRecord record) {
            score.text = record.point.ToString();
            scoreDesc.text = string.Format(ScoreDescFormat, rank.offsetFactor);
        }

        /// <summary>
        /// 生成段位名称
        /// </summary>
        /// <returns>返回段位名称</returns>
        string generateRankName() {
            return CalcService.RankCalc.getText(rank, subRank);
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem() {
            base.drawEmptyItem();
            starsDisplay.clearValue();
            rankIcon.gameObject.SetActive(false);
            rankName.text = score.text = scoreDesc.text = "";
        }

        #endregion

    }
}