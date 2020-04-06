
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

using Core.UI;

using GameModule.Services;

using SeasonModule.Data;
using SeasonModule.Services;

namespace UI.Common.Controls.ParamDisplays {

    /// <summary>
    /// 段位显示（小图标）
    /// </summary>
    public class SmallRankDisplay : BaseView {
        
        /// <summary>
        /// 外部组件定义
        /// </summary>
        public Image icon;
        public Text name;

        /// <summary>
        /// 内部变量定义
        /// </summary>
        CompRank rank; // 段位
        int subRank; // 子段位编号

        /// <summary>
        /// 内部系统定义
        /// </summary>
        SeasonService seasonSer;
        DataService dataSer;

        #region 初始化

        /// <summary>
        /// 初始化内部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            dataSer = DataService.get();
            seasonSer = SeasonService.get();
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 设置段位数据
        /// </summary>
        /// <param name="rank">段位对象</param>
        /// <param name="subRank">子段位数</param>
        public void setRank(CompRank rank, int subRank) {
            this.rank = rank;
            this.subRank = subRank;
            requestRefresh();
        }
        /// <param name="rankId">段位ID</param>
        public void setRank(int rankId, int subRank) {
            setRank(dataSer.compRank(rankId), subRank);
        }

        /// <summary>
        /// 设置星星数
        /// </summary>
        public void setStarNum(int starNum) {
            CalcService.RankCalc.calc(
                starNum, out rank, out subRank);
            requestRefresh();
        }

        /// <summary>
        /// 设置赛季记录
        /// </summary>
        /// <param name="rec"></param>
        public void setSeasonRecord(SeasonRecord rec) {
            setStarNum(rec.starNum);
        }

        #endregion

        #region 画面绘制

        /// <summary>
        /// 绘制段位
        /// </summary>
        protected void drawRank() {
            icon.gameObject.SetActive(true);
            icon.overrideSprite = AssetLoader.
                getRankIconSprite(rank.getID(), subRank, true);
            name.text = generateRankName();
        }

        /// <summary>
        /// 生成段位名称
        /// </summary>
        /// <returns>返回段位名称</returns>
        string generateRankName() {
            return CalcService.RankCalc.getText(rank, subRank);
        }

        /// <summary>
        /// 刷新视窗
        /// </summary>
        protected override void refresh() {
            base.refresh();
            if (rank != null) drawRank();
            else clear();
        }

        /// <summary>
        /// 清除视窗
        /// </summary>
        protected override void clear() {
            base.clear();
            name.text = "";
            icon.overrideSprite = null;
            icon.gameObject.SetActive(false);
        }

        #endregion

    }
}