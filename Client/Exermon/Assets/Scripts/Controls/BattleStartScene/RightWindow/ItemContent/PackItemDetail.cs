
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

using ItemModule.Data;
using PlayerModule.Data;
using BattleModule.Data;

using UI.Common.Controls.ParamDisplays;
using UI.Common.Controls.ItemDisplays;

namespace UI.BattleStartScene.Controls.Right.ItemContent {

    /// <summary>
    /// 物品详情
    /// </summary>
    public class PackItemDetail : ItemDetailDisplay<PackContItem>,
        IItemDetailDisplay<BattleItemSlotItem> {

        /// <summary>
        /// 常量定义
        /// </summary>

        /// <summary>
        /// 外部变量定义
        /// </summary>
        public StarsDisplay starsDisplay;
        public Text name, description, type, freeze, effects;
        public Image icon;

        /// <summary>
        /// 内部变量定义
        /// </summary>

        #region 接口实现

        /// <summary>
        /// 内部变量定义
        /// </summary>
        BattleItemSlotItem slotItem;

        /// <summary>
        /// 配置（非必须）
        /// </summary>
        /// <param name="container"></param>
        public void configure(IContainerDisplay<BattleItemSlotItem> container) { }

        /// <summary>
        /// 开启视窗
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="index">索引</param>
        /// <param name="refresh">是否刷新</param>
        public void startView(BattleItemSlotItem item, int index = -1, bool refresh = false) {
            slotItem = item;
            PackContItem packItem;
            if (item == null || item.isNullItem()) packItem = null;
            else packItem = item.packItem;
            startView(packItem, index, refresh);
        }

        /// <summary>
        /// 设置物品
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="index">索引</param>
        /// <param name="refresh">是否刷新</param>
        public void setItem(BattleItemSlotItem item, int index = -1, bool refresh = false) {
            slotItem = item;
            setItem(item.packItem, index, refresh);
        }

        /// <summary>
        /// 开启视窗
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="refresh">是否刷新</param>
        public void startView(BattleItemSlotItem item, bool refresh = false) {
            slotItem = item;
            startView(item.packItem, refresh);
        }

        /// <summary>
        /// 设置物品
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="refresh">是否刷新</param>
        public void setItem(BattleItemSlotItem item, bool refresh = false) {
            slotItem = item;
            setItem(item.packItem, refresh);
        }

        /// <summary>
        /// 获取物品
        /// </summary>
        /// <returns>返回设置的装备项</returns>
        BattleItemSlotItem IItemDisplay<BattleItemSlotItem>.getItem() {
            return slotItem;
        }

        #endregion

        #region 初始化
        
        #endregion

        #region 数据控制
        
        #endregion

        #region 画面绘制

        /// <summary>
        /// 绘制记录
        /// </summary>
        /// <param name="packItem">物品</param>
        protected override void drawExactlyItem(PackContItem packItem) {
            base.drawExactlyItem(packItem);

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
            drawItemIcon(item);
            drawBaseInfo(item);
            drawEffects(item);
        }

        /// <summary>
        /// 绘制物品图标
        /// </summary>
        /// <param name="item">物品1</param>
        void drawItemIcon(HumanItem item) {
            icon.gameObject.SetActive(true);
            icon.overrideSprite = item.icon;
        }

        /// <summary>
        /// 绘制基本信息
        /// </summary>
        /// <param name="item">物品1</param>
        void drawBaseInfo(HumanItem item) {
            name.text = item.name;
            description.text = item.description;
            type.text = item.itemType().name;
            freeze.text = item.freeze.ToString();
        }

        /// <summary>
        /// 绘制物品效果
        /// </summary>
        /// <param name="item">物品1</param>
        void drawEffects(HumanItem item) {
            effects.text = "";
            foreach(var effect in item.effects) 
                effects.text += effect.description + "\n";
        }
        
        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void clearItem() {
            base.clearItem();
            icon.gameObject.SetActive(false);
            name.text = description.text = type.text =
                freeze.text = effects.text = "";
        }

        #endregion

    }
}