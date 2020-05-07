
using UnityEngine.UI;

using BattleModule.Data;

using UI.Common.Controls.ItemDisplays;

/// <summary>
/// 对战界面准备窗口控件
/// </summary>
namespace UI.BattleScene.Controls.Prepare {

    /// <summary>
    /// 物品详情
    /// </summary>
    public class BattleItemDetail : ItemDetailDisplay<RuntimeBattleItem> {

        /// <summary>
        /// 常量定义
        /// </summary>
        const string InfoFormat = "名称：{0}\n描述：{1}";

        /// <summary>
        /// 外部变量定义
        /// </summary>
        public Text info;

        #region 数据控制

        /// <summary>
        /// 是否为空物品
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool isNullItem(RuntimeBattleItem item) {
            return base.isNullItem(item) || item.item() == null;
        }

        #endregion

        #region 画面绘制

        /// <summary>
        /// 绘制记录
        /// </summary>
        /// <param name="runtimeItem">运行时物品</param>
        protected override void drawExactlyItem(RuntimeBattleItem runtimeItem) {
            base.drawExactlyItem(runtimeItem);
            var item = runtimeItem.item();
            info.text = string.Format(InfoFormat,
                item.name, item.description);
        }
        
        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem() {
            base.drawEmptyItem();
            info.text = "";
        }

        #endregion

    }
}