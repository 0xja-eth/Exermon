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
            public class ExerPackEquipDisplay : SelectableItemDisplay<ExerPackEquip> {

                /// <summary>
                /// 常量定义
                /// </summary>

                /// <summary>
                /// 外部组件设置
                /// </summary>
                public Image icon; // 图片
                public Text count;

                public GameObject equipedFlag;

                /// <summary>
                /// 内部变量声明
                /// </summary>

                #region 数据控制

                /// <summary>
                /// 获取容器
                /// </summary>
                /// <returns></returns>
                public new PackContainerDisplay getContainer() {
                    return base.getContainer() as PackContainerDisplay;
                }

                #endregion

                #region 界面控制

                /// <summary>
                /// 绘制物品
                /// </summary>
                protected override void drawExactlyItem(ExerPackEquip packEquip) {
                    if (packEquip.isNullItem()) clearItem();
                    else {
                        var equip = packEquip.equip();
                        var icon = equip.icon;
                        var rect = new Rect(0, 0, icon.width, icon.height);
                        this.icon.gameObject.SetActive(true);
                        this.icon.overrideSprite = Sprite.Create(
                            icon, rect, new Vector2(0.5f, 0.5f));
                        this.icon.overrideSprite.name = icon.name;

                        if (count) count.text = packEquip.count > 1 ?
                             packEquip.count.ToString() : "";
                        equipedFlag?.SetActive(packEquip.equiped);
                    }
                }

                /// <summary>
                /// 清除物品
                /// </summary>
                protected override void clearItem() {
                    if (count) count.text = "";
                    icon.overrideSprite = null;
                    icon.gameObject.SetActive(false);
                    equipedFlag?.SetActive(false);
                }

                #endregion

                #region 事件控制

                #endregion
            }
        }
    }
}