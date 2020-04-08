
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

using ItemModule.Data;
using PlayerModule.Data;
using BattleModule.Data;

using UI.Common.Controls.ParamDisplays;
using UI.Common.Controls.ItemDisplays;

namespace UI.BattleScene.Controls.Prepare {

    /// <summary>
    /// 物品详情
    /// </summary>
    public class BattleItemDetail : ItemDetailDisplay<RuntimeBattleItem> {

        /// <summary>
        /// 常量定义
        /// </summary>
        const string NameFormat = "名称：{0}";
        const string DescFormat = "描述：{0}";

        /// <summary>
        /// 外部变量定义
        /// </summary>
        public Text name, description;

        /// <summary>
        /// 内部变量定义
        /// </summary>

        #region 初始化

        #endregion

        #region 数据控制

        #endregion

        #region 画面绘制

        /// <summary>
        /// 绘制记录
        /// </summary>
        /// <param name="runtimeItem">运行时物品</param>
        protected override void drawExactlyItem(RuntimeBattleItem runtimeItem) {
            base.drawExactlyItem(runtimeItem);
            var item = runtimeItem.battleItemSlotItem().item();

            if (item != null) {
                name.text = string.Format(NameFormat, item.name);
                description.text = string.Format(DescFormat, item.description);
            }
        }
        
        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void clearItem() {
            base.clearItem();
            name.text = description.text = "";
        }

        #endregion

    }
}