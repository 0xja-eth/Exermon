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
        namespace ExerGiftPage {

            /// <summary>
            /// 玩家艾瑟萌显示
            /// </summary
            public class PlayerExerGiftDisplay : SelectableItemDisplay<PlayerExerGift> {

                /// <summary>
                /// 常量定义
                /// </summary>
                const string LevelTextFormat = "Lv.{0}";

                /// <summary>
                /// 外部组件设置
                /// </summary>
                public Image icon; // 图片
                public StarsDisplay stars;

                public Text name;

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
                protected override void drawExactlyItem(PlayerExerGift playerGift) {
                    var gift = playerGift.item();
                    var icon = gift.bigIcon;
                    var rect = new Rect(0, 0, icon.width, icon.height);
                    this.icon.gameObject.SetActive(true);
                    this.icon.overrideSprite = Sprite.Create(
                        icon, rect, new Vector2(0.5f, 0.5f));
                    this.icon.overrideSprite.name = icon.name;

                    if (name) name.text = gift.name;
                    equipedFlag?.SetActive(playerGift.equiped);
                    stars?.setValue(gift.starId);
                }

                /// <summary>
                /// 清除物品
                /// </summary>
                protected override void clearItem() {
                    stars?.clearValue();
                    if (name) name.text = "";
                    icon.overrideSprite = null;
                    equipedFlag?.SetActive(false);
                    icon.gameObject.SetActive(false);
                }

                #endregion

                #region 事件控制

                #endregion
            }
        }
    }
}