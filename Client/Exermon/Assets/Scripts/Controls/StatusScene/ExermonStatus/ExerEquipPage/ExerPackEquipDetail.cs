
using UnityEngine;
using UnityEngine.UI;

using ItemModule.Data;
using ExermonModule.Data;

using UI.Common.Controls.ParamDisplays;

namespace UI.StatusScene.Controls.ExermonStatus.ExerEquipPage {

    /// <summary>
    /// 状态窗口艾瑟萌页属性信息显示
    /// </summary>
    public class ExerPackEquipDetail : ExermonStatusExerSlotDetail<PackContItem> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Image icon;
        public StarsDisplay stars;

        public Text name, description;

        /// <summary>
        /// 艾瑟萌装备槽项
        /// </summary>
        ExerEquipSlotItem equipSlotItem = null;

        /// <summary>
        /// 装备槽索引
        /// </summary>
        int equipIndex = 0;

        #region 数据控制

        /// <summary>
        /// 设置艾瑟萌装备槽项
        /// </summary>
        /// <param name="slotItem"></param>
        public void setEquipSlotItem(int index, ExerEquipSlotItem slotItem) {
            equipSlotItem = slotItem;
            equipIndex = index;
            requestRefresh();
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制主体信息
        /// </summary>
        /// <param name="slotItem">艾瑟萌槽项</param>
        protected override void drawMainInfo(PackContItem packEquip) {
            base.drawMainInfo(packEquip);
            if (packEquip.type == (int)BaseContItem.Type.ExerPackEquip) {
                drawIconImage((ExerPackEquip)packEquip);
                drawBaseInfo((ExerPackEquip)packEquip);
            }
        }

        /// <summary>
        /// 绘制艾瑟萌图像
        /// </summary>
        /// <param name="slotItem">艾瑟萌槽项</param>
        void drawIconImage(ExerPackEquip packEquip) {
            var equip = packEquip.equip();
            icon.color = new Color(1, 1, 1, 1);
            icon.overrideSprite = equip.icon;
            
            stars.setValue(equip.starId);
        }

        /// <summary>
        /// 绘制基本信息
        /// </summary>
        /// <param name="slotItem">艾瑟萌槽项</param>
        void drawBaseInfo(ExerPackEquip packEquip) {
            var equip = packEquip.equip();
            name.text = equip.name;
            description.text = equip.description;
        }

        /// <summary>
        /// 获取当前装备
        /// </summary>
        /// <returns></returns>
        protected override PackContItem currentEquip() {
            return equipSlotItem?.getEquip<ExerPackEquip>();
        }

        /// <summary>
        /// 绘制当前属性数据
        /// </summary>
        /// <param name="contItem"></param>
        protected override void drawCurrentParamsInfo(PackContItem contItem) {
            if (contItem.type == (int)BaseContItem.Type.ExerPackEquip) {
                // var packEquip = (ExerPackEquip)contItem;
                var objs = new ParamDisplay.DisplayDataArrayConvertable[]
                    { slotItem, equipSlotItem };
                paramInfo.setValues(objs, "params");
                battlePoint.setValue(slotItem, "battle_point");
            }
        }

        /// <summary>
        /// 绘制预览属性数据
        /// </summary>
        /// <param name="contItem"></param>
        protected override void drawPreviewParamsInfo(PackContItem contItem) {
            if (contItem.type == (int)BaseContItem.Type.ExerPackEquip) {
                var packEquip = (ExerPackEquip)contItem;

                slotItem.setPackEquipPreview(equipSlotItem.index, packEquip);
                equipSlotItem = slotItem.getPreviewEquipSlotItem(equipSlotItem.index);

                var objs = new ParamDisplay.DisplayDataArrayConvertable[]
                    { slotItem, equipSlotItem };

                paramInfo?.setValues(objs, "preview_params");
                battlePoint?.setValue(slotItem, "preview_battle_point");
                slotItem.clearPreviewObject();
            }
        }

        /// <summary>
        /// 绘制纯物品属性数据
        /// </summary>
        /// <param name="contItem"></param>
        protected override void drawContItemParamsInfo(PackContItem contItem) {
            if (contItem.type == (int)BaseContItem.Type.ExerPackEquip) 
                paramInfo.setValues((ExerPackEquip)contItem);
        }
                
        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void clearMainInfo() {
            name.text = description.text = "";
            icon.overrideSprite = null;
            icon.color = new Color(0, 0, 0, 0);
            stars.clearValue();
        }

        #endregion
    }
}