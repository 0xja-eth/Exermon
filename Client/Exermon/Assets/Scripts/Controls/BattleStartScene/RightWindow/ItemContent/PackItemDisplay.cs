
using UnityEngine;
using UnityEngine.UI;

using PlayerModule.Data;
using ItemModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.BattleStartScene.Controls.Right.ItemContent {

    /// <summary>
    /// 装备背包显示
    /// </summary
    public class PackItemDisplay : DraggableItemDisplay<PackContItem> {

        /// <summary>
        /// 常量定义
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Image icon; // 图片
        public Text count;

        /// <summary>
        /// 内部变量声明
        /// </summary>

        #region 数据控制

        /// <summary>
        /// 获取容器
        /// </summary>
        /// <returns></returns>
        public new ItemPackDisplay getContainer() {
            return base.getContainer() as ItemPackDisplay;
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 绘制物品
        /// </summary>
        protected override void drawExactlyItem(PackContItem packItem) {
            base.drawExactlyItem(packItem);
            
            if (packItem.isNullItem()) drawEmptyItem();
            else if (packItem.type == (int)BaseContItem.Type.HumanPackItem)
                drawHumanPackItem((HumanPackItem)packItem);
        }

        /// <summary>
        /// 绘制人类背包物品
        /// </summary>
        /// <param name="packItem">人类背包物品</param>
        void drawHumanPackItem(HumanPackItem packItem) {
            var item = packItem.item();
            icon.gameObject.SetActive(true);
            icon.overrideSprite = item.icon;

            if (count) count.text = packItem.count > 1 ?
                    packItem.count.ToString() : "";
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void clearItem() {
            if (count) count.text = "";
            icon.overrideSprite = null;
            icon.gameObject.SetActive(false);
        }

        #endregion

        #region 事件控制

        #endregion
    }
}