
using UnityEngine;
using UnityEngine.UI;

using PlayerModule.Data;
using QuestionModule.Data;

using BattleModule.Data;
using BattleModule.Services;
using ItemModule.Data;

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

        /// <summary>
        /// 显示效果
        /// </summary>
        bool _showEffect = false;
        public bool showEffect {
            get { return _showEffect; }
            set {
                _showEffect = value;
                requestRefresh();
            }
        }

        #region 界面控制

        /*
        /// <summary>
        /// 绘制物品
        /// </summary>
        /// <param name="item"></param>
        protected override void drawExactlyItem(BaseItem item) {
            base.drawExactlyItem(item);
        }
        */
        /// <summary>
        /// 绘制人类物品
        /// </summary>
        /// <param name="item"></param>
        protected override void drawHumanItem(HumanItem item) {
            base.drawHumanItem(item);
            if (showEffect) effects.setItem(item);
            else effects.clearItems();
        }

        /// <summary>
        /// 绘制题目糖
        /// </summary>
        /// <param name="item"></param>
        protected override void drawQuesSugar(QuesSugar item) {
            base.drawQuesSugar(item);
            if (showEffect) effects.setItem(item);
            else effects.clearItems();
        }

        /// <summary>
        /// 清空物品
        /// </summary>
        protected override void drawEmptyItem() {
            base.drawEmptyItem();
            effects.clearItems();
        }

        /// <summary>
        /// 清除
        /// </summary>
        protected override void clear() {
            _showEffect = false;
            base.clear();
        }

        #endregion
    }
}