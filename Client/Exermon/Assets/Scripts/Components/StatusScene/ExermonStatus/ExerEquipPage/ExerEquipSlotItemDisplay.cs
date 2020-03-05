using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            /// 装备背包显示
            /// </summary
            public class ExerEquipSlotItemDisplay : SelectableItemDisplay<ExerEquipSlotItem> {

                /// <summary>
                /// 常量定义
                /// </summary>

                /// <summary>
                /// 外部组件设置
                /// </summary>
                public Image icon; // 图片
                public Text name, type;

                /// <summary>
                /// 内部变量声明
                /// </summary>

                #region 数据控制

                /// <summary>
                /// 获取容器
                /// </summary>
                /// <returns></returns>
                public new ExerEquipSlotDisplay getContainer() {
                    return base.getContainer() as ExerEquipSlotDisplay;
                }

                #endregion

                #region 界面控制

                /// <summary>
                /// 绘制物品
                /// </summary>
                protected override void drawExactlyItem(ExerEquipSlotItem slotItem) {
                    if (type) type.text = slotItem.equipType().name;
                    if (slotItem.isNullItem()) clearPackEquip();
                    else {
                        var equip = slotItem.equip();
                        var icon = equip.icon;
                        var rect = new Rect(0, 0, icon.width, icon.height);
                        this.icon.gameObject.SetActive(true);
                        this.icon.overrideSprite = Sprite.Create(
                            icon, rect, new Vector2(0.5f, 0.5f));
                        this.icon.overrideSprite.name = icon.name;

                        if (name) name.text = equip.name;
                    }
                }

                /// <summary>
                /// 清除背包装备绘制信息
                /// </summary>
                void clearPackEquip() {
                    if (name) name.text = "";
                    icon.overrideSprite = null;
                    icon.gameObject.SetActive(false);
                }

                /// <summary>
                /// 清除物品
                /// </summary>
                protected override void clearItem() {
                    if (type) type.text = "";
                    clearPackEquip();
                }

                #endregion

                #region 事件控制

                #endregion
            }
        }
    }
}