using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// 状态场景
/// </summary>
namespace StatusScene {

    /// <summary>
    /// 艾瑟萌状态
    /// </summary>
    namespace ExermonStatus {

        /// <summary>
        /// 艾瑟萌装备页
        /// </summary>
        namespace ExerEquipPage {

            /// <summary>
            /// 状态窗口艾瑟萌页信息显示
            /// </summary>
            public class PageDisplay : ExermonStatusPageDisplay<ExerPackEquip> {

                /// <summary>
                /// 外部组件设置
                /// </summary>
                public ExerSlotItemDisplay slotItemDisplay;

                public PackContainerDisplay packDisplay;

                public ExerEquipSlotDisplay equipSlotDisplay;
                
                #region 启动控制

                /// <summary>
                /// 加载函数
                /// </summary>
                /// <returns></returns>
                public override UnityAction<UnityAction> getLoadFunction() {
                    return action => ExermonService.get().loadExerPack(action);
                }

                #endregion

                #region 数据控制

                /// <summary>
                /// 获取容器组件
                /// </summary>
                protected override ItemContainer<ExerPackEquip>
                    getPackDisplay() { return packDisplay; }

                /// <summary>
                /// 获取艾瑟萌槽项显示组件
                /// </summary>
                protected override ExermonStatusSlotItemDisplay<ExerPackEquip>
                    getSlotItemDisplay() { return slotItemDisplay; }

                /// <summary>
                /// 获取容器物品数据
                /// </summary>
                /// <returns></returns>
                protected override ExerPackEquip getFirstItem() {
                    var equipSlotItem = slotItemDisplay.getEquipSlotItem();
                    if (equipSlotItem == null || equipSlotItem.isNullItem()) return null;
                    return equipSlotItem.packEquip;
                }

                /// <summary>
                /// 获取容器物品数据
                /// </summary>
                /// <returns></returns>
                protected override List<ExerPackEquip> getContainerItems() {
                    var equipSlotItem = slotItemDisplay.getEquipSlotItem();
                    if (equipSlotItem == null) return new List<ExerPackEquip>();

                    var player = playerSer.player;
                    var exerPack = player.packContainers.exerPack;
                    return exerPack.exerEquips().FindAll(e => !e.isNullItem() && 
                        (equipSlotItem != null && e.item().eType == equipSlotItem.eType));
                }

                #endregion

                #region 界面绘制

                /// <summary>
                /// 刷新装备槽
                /// </summary>
                void refreshExerEquipSlot() {
                    var items = item.exerEquipSlot.items;
                    equipSlotDisplay.configure(items);
                    equipSlotDisplay.selectLast();
                }

                /// <summary>
                /// 刷新
                /// </summary>
                protected override void refresh() {
                    if (item != null) refreshExerEquipSlot();
                    base.refresh();
                    packDisplay.deselect();
                }

                #endregion

                #region 流程控制

                /// <summary>
                /// 状态变更回调
                /// </summary>
                public override void onEquipChanged() {
                    base.onEquipChanged();
                    equipSlotDisplay.selectLast();
                }

                #region 请求控制

                /// <summary>
                /// 卸下装备
                /// </summary>
                /// <returns></returns>
                public override bool dequipable() {
                    var equipSlotItem = slotItemDisplay.getEquipSlotItem();
                    return base.dequipable() && equipSlotItem != null && !equipSlotItem.isNullItem();
                }

                /// <summary>
                /// 装备请求函数
                /// </summary>
                /// <returns></returns>
                protected override UnityAction<UnityAction> equipRequestFunc() {
                    var equip = slotItemDisplay.getEquip();
                    return action => exerSer.equipExerEquip(item, equip, action); 
                }

                /// <summary>
                /// 装备请求函数
                /// </summary>
                /// <returns></returns>
                protected override UnityAction<UnityAction> dequipRequestFunc() {
                    var equipItem = equipSlotDisplay.selectedItem();
                    return action => exerSer.dequipExerEquip(item, equipItem.eType, action);
                }

                #endregion

                #endregion
            }
        }
    }
}