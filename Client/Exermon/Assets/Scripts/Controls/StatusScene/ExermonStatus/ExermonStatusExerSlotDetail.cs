
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using Core.UI;
using Core.UI.Utils;

using GameModule.Data;
using ItemModule.Data;
using PlayerModule.Data;
using ExermonModule.Data;

using GameModule.Services;
using PlayerModule.Services;
using ExermonModule.Services;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.ParamDisplays;

namespace UI.StatusScene.Controls.ExermonStatus {
    
    /// <summary>
    /// 状态窗口艾瑟萌槽详细信息显示
    /// </summary>
    public class ExermonStatusExerSlotDetail<T> : ItemDetailDisplay<T> where T : PackContItem, new() {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public ParamDisplaysGroup paramInfo;
        public MultParamsDisplay battlePoint;

        /// <summary>
        /// 艾瑟萌槽项
        /// </summary>
        protected ExerSlotItem slotItem = null;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        public override void configure() {
            base.configure();
            if (paramInfo != null) {
                var params_ = DataService.get().staticData.configure.baseParams;
                paramInfo.configure(params_);
            }
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 设置艾瑟萌槽项
        /// </summary>
        /// <param name="slotItem"></param>
        public void setSlotItem(ExerSlotItem slotItem) {
            this.slotItem = slotItem;
            requestRefresh();
        }

        #endregion

        /*
        #region 数据控制

        /// <summary>
        /// 是否为空物品
        /// </summary>
        /// <param name="item">物品</param>
        /// <returns></returns>
        public override bool isNullItem(T item) {
            return base.isNullItem(item) || ;
        }

        #endregion
        */

        #region 界面绘制

        /// <summary>
        /// 绘制物品
        /// </summary>
        /// <param name="slotItem">艾瑟萌槽项</param>
        protected override void drawExactlyItem(T contItem) {
            if (!contItem.isNullItem())
                drawMainInfo(contItem);
            else clearMainInfo();
            drawParamsInfo(contItem);
            drawSlotInfo();
        }

        /// <summary>
        /// 绘制主体信息
        /// </summary>
        /// <param name="item"></param>
        protected virtual void drawMainInfo(T item) { }

        /// <summary>
        /// 绘制槽信息
        /// </summary>
        /// <param name="item"></param>
        protected virtual void drawSlotInfo() { }

        /// <summary>
        /// 绘制艾瑟萌图像
        /// </summary>
        /// <param name="contItem">容器项</param>
        protected virtual void drawParamsInfo(T contItem) {
            // 如果要显示的艾瑟萌与装备中的一致，直接显示
            if (slotItem != null)
                if (contItem == currentEquip())
                    drawCurrentParamsInfo(contItem);
                else drawPreviewParamsInfo(contItem);
            else if (!contItem.isNullItem())
                drawContItemParamsInfo(contItem);
            else clearParamsInfo();
            paramInfo?.setIgnoreTrigger();
        }

        /// <summary>
        /// 获取当前装备
        /// </summary>
        /// <returns></returns>
        protected virtual T currentEquip() { return slotItem.getEquip<T>(); }

        /// <summary>
        /// 绘制当前属性数据
        /// </summary>
        /// <param name="contItem"></param>
        protected virtual void drawSlotParamsInfo() {
            paramInfo?.setValues(slotItem, "params");
            battlePoint?.setValue(slotItem, "battle_point");
        }

        /// <summary>
        /// 绘制当前属性数据
        /// </summary>
        /// <param name="contItem"></param>
        protected virtual void drawCurrentParamsInfo(T contItem) {
            drawSlotParamsInfo();
        }

        /// <summary>
        /// 绘制预览属性数据
        /// </summary>
        /// <param name="contItem"></param>
        protected virtual void drawPreviewParamsInfo(T contItem) {
            slotItem.setPreview(contItem);
            paramInfo?.setValues(slotItem, "preview_params");
            battlePoint?.setValue(slotItem, "preview_battle_point");
            slotItem.clearPreviewObject();
        }

        /// <summary>
        /// 绘制纯物品属性数据
        /// </summary>
        /// <param name="contItem"></param>
        protected virtual void drawContItemParamsInfo(T contItem) { }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem() {
            /*
            if (slotItem != null) {
                drawSlotParamsInfo();
                paramInfo?.setIgnoreTrigger();
            }
            */
            clearMainInfo();
            clearSlotInfo();
            clearParamsInfo();
        }

        /// <summary>
        /// 清除主体信息
        /// </summary>
        protected virtual void clearMainInfo() { }

        /// <summary>
        /// 清除槽信息
        /// </summary>
        protected virtual void clearSlotInfo() { }

        /// <summary>
        /// 清除属性信息
        /// </summary>
        void clearParamsInfo() {
            if (slotItem != null) drawSlotParamsInfo();
            else paramInfo?.clearValues();
            paramInfo?.setIgnoreTrigger();

            battlePoint?.clearValue();
        }

        #endregion
    }
}