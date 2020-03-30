
using UnityEngine;
using UnityEngine.UI;

using ExermonModule.Data;

using UI.Common.Controls.ParamDisplays;

namespace UI.StatusScene.Controls.ExermonStatus.ExerGiftPage {

    /// <summary>
    /// 状态窗口艾瑟萌页属性信息显示
    /// </summary>
    public class PlayerExerGiftDetail : ExermonStatusExerSlotDetail<PlayerExerGift> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Image icon;
        public StarsDisplay stars;

        public Text name, description;

        #region 界面绘制

        /// <summary>
        /// 绘制主体信息
        /// </summary>
        /// <param name="slotItem">艾瑟萌槽项</param>
        protected override void drawMainInfo(PlayerExerGift playerGift) {
            base.drawMainInfo(playerGift);
            drawIconImage(playerGift);
            drawBaseInfo(playerGift);
        }

        /// <summary>
        /// 绘制艾瑟萌图像
        /// </summary>
        /// <param name="slotItem">艾瑟萌槽项</param>
        void drawIconImage(PlayerExerGift playerGift) {
            var gift = playerGift.item();
            var icon = gift.bigIcon;
            var rect = new Rect(0, 0, icon.width, icon.height);
            this.icon.gameObject.SetActive(true);
            this.icon.overrideSprite = Sprite.Create(
                icon, rect, new Vector2(0.5f, 0.5f));
            this.icon.overrideSprite.name = icon.name;

            stars.setValue(gift.starId);
        }

        /// <summary>
        /// 绘制基本信息
        /// </summary>
        /// <param name="slotItem">艾瑟萌槽项</param>
        void drawBaseInfo(PlayerExerGift playerGift) {
            var gift = playerGift.item();
            name.text = gift.name;
            description.text = gift.description;
        }

        /// <summary>
        /// 绘制纯物品属性数据
        /// </summary>
        /// <param name="contItem"></param>
        protected override void drawContItemParamsInfo(PlayerExerGift playerGift) {
            paramInfo.setValues(playerGift.item());
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void clearMainInfo() {
            base.clearMainInfo();
            name.text = description.text = "";
            icon.gameObject.SetActive(false);
            stars.clearValue();
        }
                
        #endregion
    }
}