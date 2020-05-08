using UnityEngine;
using UnityEngine.UI;

using Core.Systems;
using Core.UI;
using Core.UI.Utils;

using GameModule.Services;

using ItemModule.Data;

using PlayerModule.Services;
using ItemModule.Services;

using UI.Common.Controls.InputFields;
using UI.Common.Controls.ItemDisplays;

using UI.ShopScene.Controls;

/// <summary>
/// 状态场景窗口
/// </summary>
namespace UI.ShopScene.Windows {

    /// <summary>
    /// 数量输入窗口
    /// </summary>
    public class NumberInputWindow : BaseWindow {

        /// <summary>
        /// 文本常量定义
        /// </summary>
        const string BuyTipsText = "选择购买数量";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public ShopWindow shopWindow;

        public BuySelector buySelector;
        public ValueInputField numberInput;
        
        /// <summary>
        /// 场景组件引用
        /// </summary>
        ShopScene scene;

        /// <summary>
        /// 外部系统引用
        /// </summary>
        protected DataService dataSer = null;
        protected PlayerService playerSer = null;
        protected ItemService itemSer = null;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            numberInput.onChanged = onValueChanged;
        }

        /// <summary>
        /// 每次初始化
        /// </summary>
        protected override void initializeEvery() {
            base.initializeEvery();
            setupNumberInput();
        }

        /// <summary>
        /// 配置输入
        /// </summary>
        void setupNumberInput() {
            numberInput.configure(minCount(), maxCount());
        }

        /// <summary>
        /// 初始化场景
        /// </summary>
        protected override void initializeScene() {
            scene = (ShopScene)SceneUtils.getSceneObject("Scene");
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            dataSer = DataService.get();
            playerSer = PlayerService.get();
            itemSer = ItemService.get();
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 操作容器
        /// </summary>
        /// <returns></returns>
        public PackContainer<PackContItem> operContainer() {
            return shopWindow.operContainer();
        }

        /// <summary>
        /// 操作物品
        /// </summary>
        /// <returns>返回操作物品</returns>
        public BaseItem operItem() {
            return shopWindow.operItem();
        }

        /// <summary>
        /// 操作物品
        /// </summary>
        /// <returns>返回操作物品</returns>
        public ItemService.ShopItem operShopItem() {
            return shopWindow.operShopItem();
        }

        /// <summary>
        /// 最大值
        /// </summary>
        /// <returns></returns>
        public virtual int maxCount() {
            var money = currentMoney();
            var price = singlePrice();
            if (price == 0) return 0;
            return money / price;
        }

        /// <summary>
        /// 最小值
        /// </summary>
        /// <returns></returns>
        public virtual int minCount() {
            return 0;
        }

        /// <summary>
        /// 当前数量
        /// </summary>
        /// <returns></returns>
        public virtual int currentCount() {
            return numberInput.getValue();
        }

        /// <summary>
        /// 单价
        /// </summary>
        /// <returns></returns>
        public virtual int singlePrice() {
            var buyType = buySelector.getBuyType();
            var price = operShopItem().price;

            if (buyType == 0) return price.gold;
            if (buyType == 1) return price.ticket;
            if (buyType == 2) return price.boundTicket;

            return 0;
        }

        /// <summary>
        /// 当前指定金钱
        /// </summary>
        /// <returns></returns>
        public int currentMoney() {
            var buyType = buySelector.getBuyType();
            var money = playerSer.player.money;

            if (buyType == 0) return money.gold;
            if (buyType == 1) return money.ticket;
            if (buyType == 2) return money.boundTicket;

            return 0;
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 值变化回调
        /// </summary>
        protected virtual void onValueChanged(int value) {
            drawPrice();
        }

        /// <summary>
        /// 绘制出售状态
        /// </summary>
        void drawPrice() {
            buySelector.setCount(currentCount());
        }

        /// <summary>
        /// 刷新
        /// </summary>
        protected override void refresh() {
            base.refresh(); drawPrice();
        }

        /// <summary>
        /// 清除窗口
        /// </summary>
        protected override void clear() {
            base.clear();
            buySelector.clearItem();
            numberInput.requestClear(true);
        }

        #endregion

        #region 流程控制
        
        /// <summary>
        /// 购买
        /// </summary>
        public virtual void onBuy() {
            var count = currentCount();
            if (count <= 0) terminateWindow();
            else shopWindow.buyItem(count);
        }
        
        #endregion
    }
}