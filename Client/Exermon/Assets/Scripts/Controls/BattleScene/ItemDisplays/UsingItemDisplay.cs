
using UnityEngine.UI;

using PlayerModule.Data;
using QuestionModule.Data;

using BattleModule.Data;
using BattleModule.Services;

namespace UI.BattleScene.Controls.ItemDisplays {

    /// <summary>
    /// 对战物资槽物品显示
    /// </summary
    public class UsingItemDisplay : UsedItemDisplay {

        /// <summary>
        /// 常量定义
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public ItemEffectList effects;
        
        #region 界面控制

        /// <summary>
        /// 绘制人类物品
        /// </summary>
        /// <param name="item"></param>
        protected override void drawHumanItem(HumanItem item) {
            base.drawHumanItem(item);
            effects.setItem(item);
        }

        /// <summary>
        /// 绘制题目糖
        /// </summary>
        /// <param name="item"></param>
        protected override void drawQuesSugar(QuesSugar item) {
            base.drawQuesSugar(item);
            effects.setItem(item);
        }

        /// <summary>
        /// 清空物品
        /// </summary>
        protected override void clearItem() {
            base.clearItem();
            effects.clearItems();
        }

        #endregion
    }
}