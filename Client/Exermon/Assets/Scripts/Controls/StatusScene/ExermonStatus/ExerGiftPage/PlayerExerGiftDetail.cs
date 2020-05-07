
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

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
            icon.gameObject.SetActive(true);
            icon.overrideSprite = AssetLoader.generateSprite(gift.bigIcon);

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