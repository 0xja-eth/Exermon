
using UnityEngine;
using UnityEngine.UI;

using ItemModule.Data;
using PlayerModule.Data;

using BattleModule.Data;
using BattleModule.Services;

using UI.Common.Controls.ItemDisplays;
using UnityEngine.Events;

namespace UI.BattleStartScene.Controls.Main {

    /// <summary>
    /// 对战物资槽物品显示
    /// </summary
    public class BattleItemSlotItemDisplay : SlotContItemDisplay
        <BattleItemSlotItem, PackContItem> {

        /// <summary>
        /// 常量定义
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
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
        /// 设置装备
        /// </summary>
        protected override void setupExactlyEquip() {
            base.setupExactlyEquip();
            equip = item.packItem;
            // base.setEquip(item.packItem, true);
        }

        /// <summary>
        /// 获取容器
        /// </summary>
        /// <returns></returns>
        public new BattleItemSlotDisplay getContainer() {
            return base.getContainer() as BattleItemSlotDisplay;
        }
        
        /// <summary>
        /// 装备请求函数
        /// </summary>
        /// <param name="item">装备项</param>
        /// <returns>返回装备时进行的请求函数</returns>
        protected override UnityAction<UnityAction> equipRequestFunc(PackContItem item) {
            if (item.type != (int)BaseContItem.Type.HumanPackItem) return null;
            return action => battleSer.equipBattleItem(
                this.item, (HumanPackItem)item, action);
        }

        /// <summary>
        /// 卸下装备请求函数
        /// </summary>
        /// <returns>返回卸下时进行的请求函数</returns>
        protected override UnityAction<UnityAction> dequipRequestFunc() {
            return action => battleSer.dequipBattleItem(item, action);
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 绘制装备
        /// </summary>
        /// <param name="equip">装备</param>
        protected override void drawExactlyEquip(PackContItem packItem) {
            base.drawExactlyEquip(packItem);

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
        }

        /// <summary>
        /// 清除装备
        /// </summary>
        protected override void clearEquip() {
            icon.overrideSprite = null;
            icon.gameObject.SetActive(false);
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void clearItem() {
            clearEquip();
        }

        #endregion

        #region 事件控制

        #endregion
    }
}