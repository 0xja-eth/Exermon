﻿
using UnityEngine.UI;

using BattleModule.Data;
using BattleModule.Services;

using UI.Common.Controls.ItemDisplays;

namespace UI.BattleScene.Controls.Prepare {

    /// <summary>
    /// 对战物资槽物品显示
    /// </summary
    public class BattleItemDisplay : 
        SelectableItemDisplay<RuntimeBattleItem> {

        /// <summary>
        /// 常量定义
        /// </summary>
        const string FreezeFormat = "冻结回合：{0}";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Text name, freeze;
        public Image icon; // 图片

        /// <summary>
        /// 外部系统声明
        /// </summary>
        BattleService battleSer;

        #region 初始化

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            battleSer = BattleService.get();
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 是否可以选中
        /// </summary>
        /// <returns>可否选中</returns>
        public override bool isEnabled() {
            return base.isEnabled() && item != null && 
                item.item() != null && item.freezeRound == 0;
        }

        #endregion

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

        #region 界面控制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="runtimeItem">运行时物品</param>
        protected override void drawExactlyItem(RuntimeBattleItem runtimeItem) {
            base.drawExactlyItem(runtimeItem);
            var item = runtimeItem.item();

            icon.gameObject.SetActive(true);
            icon.overrideSprite = item.icon;

            name.text = item.name;
            freeze.text = string.Format(
                FreezeFormat, runtimeItem.freezeRound);
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem() {
            icon.gameObject.SetActive(false);
            name.text = freeze.text = "";
        }

        #endregion
    }
}