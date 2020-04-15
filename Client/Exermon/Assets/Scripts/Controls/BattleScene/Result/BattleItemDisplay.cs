
using UnityEngine.UI;

using ItemModule.Data;

using PlayerModule.Data;
using QuestionModule.Data;

using BattleModule.Data;
using BattleModule.Services;

using UI.Common.Controls.ItemDisplays;

namespace UI.BattleScene.Controls.Result {

    /// <summary>
    /// 对战物资槽物品显示
    /// </summary
    public class BattleItemDisplay : ItemDisplay<BaseItem> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Text name;
        public Image icon; // 图片

        #region 界面控制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="item">物品</param>
        protected override void drawExactlyItem(BaseItem item) {
            base.drawExactlyItem(item);
            switch (item.type) {
                case (int)BaseItem.Type.HumanItem:
                    drawHumanItem((HumanItem)item); break;
                case (int)BaseItem.Type.QuesSugar:
                    drawQuesSugar((QuesSugar)item); break;
                default: drawEmptyItem(); break;
            }
        }

        /// <summary>
        /// 绘制人类物品
        /// </summary>
        /// <param name="item"></param>
        void drawHumanItem(HumanItem item) {
            if (name) name.text = item.name;
            if (icon) {
                icon.gameObject.SetActive(true);
                icon.overrideSprite = item.icon;
            }
        }

        /// <summary>
        /// 绘制题目糖
        /// </summary>
        /// <param name="item"></param>
        void drawQuesSugar(QuesSugar item) { }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void clearItem() {
            icon.gameObject.SetActive(false);
            name.text = "";
        }

        #endregion
    }
}