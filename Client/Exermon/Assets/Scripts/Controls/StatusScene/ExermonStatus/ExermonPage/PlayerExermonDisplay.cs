
using UnityEngine;
using UnityEngine.UI;

using ExermonModule.Data;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.ParamDisplays;

namespace UI.StatusScene.Controls.ExermonStatus.ExermonPage {

    /// <summary>
    /// 玩家艾瑟萌显示
    /// </summary
    public class PlayerExermonDisplay : SelectableItemDisplay<PlayerExermon> {

        /// <summary>
        /// 常量定义
        /// </summary>
        const string LevelTextFormat = "Lv.{0}";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Image icon; // 图片
        public StarsDisplay stars;
        public Text name, level;

        public GameObject equipedFlag;

        /// <summary>
        /// 内部变量声明
        /// </summary>

        #region 数据控制

        /// <summary>
        /// 获取容器
        /// </summary>
        /// <returns></returns>
        public new PackContainerDisplay getContainer() {
            return base.getContainer() as PackContainerDisplay;
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 绘制物品
        /// </summary>
        protected override void drawExactlyItem(PlayerExermon playerExer) {
            var exermon = playerExer.exermon();
            var icon = exermon.icon;
            var rect = new Rect(0, 0, icon.width, icon.height);
            this.icon.gameObject.SetActive(true);
            this.icon.overrideSprite = Sprite.Create(
                icon, rect, new Vector2(0.5f, 0.5f));
            this.icon.overrideSprite.name = icon.name;

            if (name) name.text = playerExer.name();
            if (level) level.text = string.Format(LevelTextFormat, playerExer.level);
            equipedFlag?.SetActive(playerExer.equiped);
            stars?.setValue(exermon.starId);
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem() {
            stars?.clearValue();
            if (name) name.text = "";
            if (level) level.text = "";
            icon.overrideSprite = null;
            equipedFlag?.SetActive(false);
            icon.gameObject.SetActive(false);
        }

        #endregion

        #region 事件控制

        #endregion
    }
    }