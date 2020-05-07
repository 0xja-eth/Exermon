
using UnityEngine;
using UnityEngine.UI;

using ExermonModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.BattleStartScene.Controls.Main {

    /// <summary>
    /// 状态窗口艾瑟萌页信息显示
    /// </summary>
    public class SmallExerSlotItemDisplay : SelectableItemDisplay<ExerSlotItem> {

        /// <summary>
        /// 常量定义
        /// </summary>
        const string LevelFormat = "Lv. {0}";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Image icon; // 图片
        public Text level; // 等级

        /// <summary>
        /// 内部变量声明
        /// </summary>

        #region 数据控制

        /// <summary>
        /// 获取容器
        /// </summary>
        /// <returns></returns>
        public new SmallExerSlotDisplay getContainer() {
            return base.getContainer() as SmallExerSlotDisplay;
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 绘制物品
        /// </summary>
        protected override void drawExactlyItem(ExerSlotItem slotItem) {
            if (slotItem.isNullItem()) drawEmptyItem();
            else {
                drawIcon(slotItem);
                drawLevel(slotItem);
            }
        }

        /// <summary>
        /// 绘制缩略图
        /// </summary>
        void drawIcon(ExerSlotItem slotItem) {
            var item = slotItem.playerExer.item();
            var icon = item.icon;
            var rect = new Rect(0, 0, icon.width, icon.height);
            this.icon.gameObject.SetActive(true);
            this.icon.overrideSprite = Sprite.Create(
                icon, rect, new Vector2(0.5f, 0.5f));
            this.icon.overrideSprite.name = icon.name;
        }

        void drawLevel(ExerSlotItem slotItem) {
            level.text = string.Format(LevelFormat, slotItem.playerExer.level);
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem() {
            icon.overrideSprite = null;
            icon.gameObject.SetActive(false);
        }

        #endregion

        #region 事件控制

        #endregion
    }
}