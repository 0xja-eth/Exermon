﻿
using PlayerModule.Data;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.ParamDisplays;

namespace UI.StatusScene.Controls.PlayerStatus {

    /// <summary>
    /// 人物详细信息视图
    /// </summary>
    public class DetailInfoDisplay : ItemDisplay<Player> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public MultParamsDisplay paramsInfo, battleInfo, questionInfo, personalInfo;

        /// <summary>
        /// 内部变量设置
        /// </summary>
        Player player;

        #region 初始化

        protected override void initializeOnce() {
            base.initializeOnce();
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制玩家状态
        /// </summary>
        /// <param name="player">玩家</param>
        protected override void drawExactlyItem(Player player) {
            paramsInfo.setValue(player, "params_info");
            battleInfo.setValue(player, "battle_info");
            questionInfo.setValue(player, "question_info");
            personalInfo.setValue(player, "personal_info");
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem() {
            paramsInfo.clearValue();
            battleInfo.clearValue();
            questionInfo.clearValue();
            personalInfo.clearValue();
        }

        #endregion

    }
}