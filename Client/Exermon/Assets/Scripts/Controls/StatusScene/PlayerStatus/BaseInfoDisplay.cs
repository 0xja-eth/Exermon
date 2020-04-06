
using UnityEngine;
using UnityEngine.UI;

using PlayerModule.Data;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.ParamDisplays;

/// <summary>
/// 状态场景中人类状态页控件
/// </summary>
namespace UI.StatusScene.Controls.PlayerStatus {

    /// <summary>
    /// 人物基本信息视图
    /// </summary>
    public class BaseInfoDisplay : ItemDisplay<Player> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Image bust;
        public Text name;
        public ParamDisplay expBar;

        /// <summary>
        /// 内部变量设置
        /// </summary>

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
            drawPlayerBust(player);
            drawPlayerInfo(player);
        }

        /// <summary>
        /// 绘制半身像
        /// </summary>
        /// <param name="player">玩家</param>
        void drawPlayerBust(Player player) {
            bust.gameObject.SetActive(true);
            bust.overrideSprite = player.character().bust;
        }

        /// <summary>
        /// 绘制信息
        /// </summary>
        void drawPlayerInfo(Player player) {
            name.text = player.name;
            expBar.setValue(player, "exp");
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void clearItem() {
            name.text = "";
            bust.gameObject.SetActive(false);
            expBar.clearValue();
        }

        #endregion

    }
}