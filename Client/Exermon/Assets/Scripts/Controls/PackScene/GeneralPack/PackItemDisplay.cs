
using UnityEngine;
using UnityEngine.UI;

using ItemModule.Data;
using PlayerModule.Data;
using ExermonModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.PackScene.Controls.GeneralPack {

    /// <summary>
    /// 装备背包显示
    /// </summary
    public class PackItemDisplay : PackContItemDisplay {

        /// <summary>
        /// 常量定义
        /// </summary>
        
        /// <summary>
        /// 内部变量声明
        /// </summary>

        #region 初始化

        /// <summary>
        /// 初始化绘制函数
        /// </summary>
        protected override void initializeDrawFuncs() {
            registerItemType<HumanPackItem>(drawHumanPackItem);
            registerItemType<HumanPackEquip>(drawHumanPackEquip);
            registerItemType<ExerPackItem>(drawExerPackItem);
            registerItemType<ExerPackEquip>(drawExerPackEquip);
        }

        #endregion

        #region 界面控制
        /// <summary>
        /// 绘制基本信息
        /// </summary>
        /// <param name="item">物品1</param>
        void drawBaseInfo(LimitedItem item) {
            if (name) name.text = item.name;

            icon.gameObject.SetActive(true);
            icon.overrideSprite = item.icon;
        }

        /// <summary>
        /// 绘制人类背包物品
        /// </summary>
        /// <param name="packItem">人类背包物品</param>
        void drawHumanPackItem(HumanPackItem packItem) {
            drawBaseInfo(packItem.item());
            if (count) count.text = packItem.count.ToString();
        }

        /// <summary>
        /// 绘制人类背包装备
        /// </summary>
        /// <param name="packEquip">人类背包装备</param>
        void drawHumanPackEquip(HumanPackEquip packEquip) {
            drawBaseInfo(packEquip.item());
        }

        /// <summary>
        /// 绘制艾瑟萌背包物品
        /// </summary>
        /// <param name="packItem">艾瑟萌背包物品</param>
        void drawExerPackItem(ExerPackItem packItem) {
            drawBaseInfo(packItem.item());
            if (count) count.text = packItem.count.ToString();
        }

        /// <summary>
        /// 绘制艾瑟萌背包装备
        /// </summary>
        /// <param name="packEquip">艾瑟萌背包装备</param>
        void drawExerPackEquip(ExerPackEquip packEquip) {
            drawBaseInfo(packEquip.item());
        }

        #endregion
    }
}