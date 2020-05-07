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

using UI.PackScene.Controls.GeneralPack;

/// <summary>
/// 状态场景窗口
/// </summary>
namespace UI.PackScene.Windows {

    /// <summary>
    /// 数量输入窗口
    /// </summary>
    public class NumberInputWindow : BaseWindow {

        /// <summary>
        /// 文本常量定义
        /// </summary>
        const string UseTipsText = "选择使用数量";
        const string SellTipsText = "选择出售数量";
        const string DiscardTipsText = "选择丢弃数量";

        /// <summary>
        /// 类型枚举
        /// </summary>
        public enum Mode {
            Use, Sell, Discard,
        }

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public PackWindow packWindow;

        public ValueInputField numberInput;
        //public PackItemDetail itemDetail;

        public GameObject sellDisplay;
        public Text tips, sellGold;

        /// <summary>
        /// 内部变量声明
        /// </summary>
        public Mode mode { get; protected set; }

        /// <summary>
        /// 场景组件引用
        /// </summary>
        PackScene scene;

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
            scene = (PackScene)SceneUtils.getSceneObject("Scene");
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

        #region 更新控制

        #endregion

        #region 开启控制

        /// <summary>
        /// 开始窗口
        /// </summary>
        public override void startWindow() {
            startWindow(Mode.Sell);
        }

        /// <summary>
        /// 开始窗口
        /// </summary>
        public void startWindow(int mode) {
            startWindow((Mode)mode);
        }
        public void startWindow(Mode mode) {
            base.startWindow(); setMode(mode);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 操作容器
        /// </summary>
        /// <returns></returns>
        //public PackContainerDisplay<PackContItem> operContainerDisplay() {
        //    return (PackContainerDisplay<PackContItem>)itemDetail.getContainer();
        //}

        /// <summary>
        /// 操作容器
        /// </summary>
        /// <returns></returns>
        public PackContainer<PackContItem> operContainer() {
            return packWindow.operContainer();
            //return operContainerDisplay()?.getPackData();
        }

        /// <summary>
        /// 操作物品
        /// </summary>
        /// <returns>返回操作物品</returns>
        public LimitedItem operItem() {
            return packWindow.operItem<LimitedItem>();
            //return itemDetail.getContainedItem();
        }
        
        /// <summary>
        /// 操作物品
        /// </summary>
        /// <returns>返回操作物品</returns>
        public PackContItem operPackItem() {
            return packWindow.operPackItem();
            //return itemDetail.getItem();
        }

        /// <summary>
        /// 切换视图
        /// </summary>
        public void setMode(Mode mode) {
            this.mode = mode;
            requestRefresh();
        }

        /// <summary>
        /// 最大值
        /// </summary>
        /// <returns></returns>
        public virtual int maxCount() {
            var maxCnt = operPackItem().count;
            var item = operItem() as UsableItem;
            if (mode == Mode.Use && item != null && 
                item.batchCount > 0) 
                maxCnt = Mathf.Min(maxCnt, item.batchCount);
            return maxCnt;
        }

        /// <summary>
        /// 最小值
        /// </summary>
        /// <returns></returns>
        public virtual int minCount() {
            return 1;
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
            return operItem().sellPrice;
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 值变化回调
        /// </summary>
        protected virtual void onValueChanged(int value) {
            drawSellDisplay();
        }

        /// <summary>
        /// 绘制出售状态
        /// </summary>
        void drawSellDisplay() {
            if (mode == Mode.Sell) {
                var price = currentCount() * singlePrice();
                sellGold.text = price.ToString();
                sellDisplay.SetActive(true);
            } else
                sellDisplay.SetActive(false);
        }

        /// <summary>
        /// 刷新
        /// </summary>
        protected override void refresh() {
            base.refresh();
            drawSellDisplay();
        }

        /// <summary>
        /// 清除窗口
        /// </summary>
        protected override void clear() {
            base.clear();
            sellDisplay.SetActive(false);
            numberInput.requestClear(true);
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 确认
        /// </summary>
        public void onConfirm() {
            switch (mode) {
                case Mode.Use: onUse(); break;
                case Mode.Sell: onSell(); break;
                case Mode.Discard: onDiscard(); break;
            }
        }

        /// <summary>
        /// 出售
        /// </summary>
        protected virtual void onUse() {
            packWindow.useItem(currentCount());
        }

        /// <summary>
        /// 出售
        /// </summary>
        void onSell() {
            packWindow.sellItem(currentCount());
        }

        /// <summary>
        /// 丢弃
        /// </summary>
        void onDiscard() {
            packWindow.discardItem(currentCount());
        }

        #endregion
    }
}