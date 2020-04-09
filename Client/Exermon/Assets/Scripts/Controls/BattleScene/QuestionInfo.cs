
using UnityEngine.UI;

using BattleModule.Data;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.ParamDisplays;

/// <summary>
/// 准备状态控件
/// </summary>
namespace UI.BattleScene.Controls {

    /// <summary>
    /// 题目信息控件
    /// </summary>
    public class QuestionInfo : ItemDisplay<BattleRound> {

        /// <summary>
        /// 常量定义
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Text num, subject, type;
        public StarsDisplay star;

        /// <summary>
        /// 内部变量定义
        /// </summary>
        bool isLoaded = false;
        
        #region 初始化
        
        #endregion

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>        
        protected override void update() {
            base.update();
            updateRoundQuestion();
        }

        /// <summary>
        /// 更新回合题目
        /// </summary>
        void updateRoundQuestion() {
            if (isLoaded || item == null) return;
            if (isLoaded = item.loaded)
                requestRefresh(true);
        }

        #endregion

        #region 数据操作

        /// <summary>
        /// 物品变更
        /// </summary>
        protected override void onItemChanged() {
            base.onItemChanged();
            isLoaded = item.loaded;
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="round">对战回合</param>
        protected override void drawExactlyItem(BattleRound round) {
            base.drawExactlyItem(round);
            drawType(round);
            drawBaseInfo(round);
        }

        /// <summary>
        /// 绘制题目基本信息
        /// </summary>
        /// <param name="round">对战回合</param>
        void drawBaseInfo(BattleRound round) {
            num.text = round.order.ToString();
            subject.text = round.subject().name;
            star.setValue(round.starId);
        }

        /// <summary>
        /// 绘制题目类型
        /// </summary>
        /// <param name="round">对战回合</param>
        void drawType(BattleRound round) {
            var ques = round.question();
            if (ques == null) return;
            type.text = ques.typeText();
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void clearItem() {
            num.text = subject.text = "";
            star.clearValue();
        }

        #endregion
    }
}
