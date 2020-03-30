
using UnityEngine;
using UnityEngine.UI;

using Core.UI;

using SeasonModule.Data;
using SeasonModule.Services;

namespace UI.Common.Controls.ParamDisplays {

    /// <summary>
    /// 段位星星显示
    /// </summary>
    public class RankStarsDisplay : GroupView<Image> {

        /// <summary>
        /// 常量定义
        /// </summary>
        const string DeltaDescriptionFormat = "距离下一段位还差 {0} 颗星";
        const string MaxRankDescription = "恭喜您已达到最大段位，可无限累计段位星星";

        /// <summary>
        /// 外部组件定义
        /// </summary>
        public Text description;

        public Texture2D on, off; // 星星点亮/熄灭时图片

        public GameObject restStars; // 超出星星
        public Text countText; // 超出星星文本

        /// <summary>
        /// 星星数（传入的是计算后星星的值）
        /// </summary>
        int count = -1;

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
        /// 获取星星数目
        /// </summary>
        /// <returns></returns>
        public int getValue() {
            return count;
        }

        /// <summary>
        /// 设置星星数目
        /// </summary>
        /// <param name="count">数目</param>
        public void setValue(int count) {
            if (this.count == count) return;
            this.count = count;
            requestRefresh();
        }

        /// <summary>
        /// 清空值
        /// </summary>
        public void clearValue() {
            setValue(0);
        }
        
        /// <summary>
        /// 返回是否最大等级
        /// </summary>
        /// <returns></returns>
        public bool isMaxRank() {
            return count > CompRank.StarsPerSubRank;
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 刷新子视图
        /// </summary>
        /// <param name="sub">子视图</param>
        protected override void refreshSubView(Image sub, int index) {
            if (!isMaxRank()) drawGeneralStar(sub, index);
            else sub.gameObject.SetActive(false);
        }

        /// <summary>
        /// 绘制一般情况下的星星
        /// </summary>
        /// <param name="sub"></param>
        /// <param name="index"></param>
        void drawGeneralStar(Image sub, int index) {
            var block = index < count ? on : off;
            var rect = new Rect(0, 0, block.width, block.height);

            sub.gameObject.SetActive(true);
            sub.overrideSprite = Sprite.Create(
                block, rect, new Vector2(0.5f, 0.5f));
        }

        /// <summary>
        /// 绘制更多星星
        /// </summary>
        void drawMoreStars() {
            restStars.SetActive(true);
            countText.text = "×" + count;
        }

        /// <summary>
        /// 绘制描述文本
        /// </summary>
        void drawDescription() {
            if(description) description.text = generateDescription();
        }

        /// <summary>
        /// 生成描述文本
        /// </summary>
        /// <returns></returns>
        string generateDescription() {
            if (isMaxRank()) return MaxRankDescription;
            var delta = CompRank.StarsPerSubRank - count + 1;
            return string.Format(DeltaDescriptionFormat, delta);
        }

        /// <summary>
        /// 刷新视窗
        /// </summary>
        protected override void refresh() {
            base.refresh();
            drawDescription();
            if (isMaxRank()) drawMoreStars();
            else restStars.SetActive(false);
        }

        /// <summary>
        /// 清除视窗
        /// </summary>
        protected override void clear() {
            base.clear();
            if (description) description.text = "";
            restStars.SetActive(false);
        }

        #endregion

    }
}