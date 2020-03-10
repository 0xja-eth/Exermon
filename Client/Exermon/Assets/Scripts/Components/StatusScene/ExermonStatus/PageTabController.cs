
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// 状态场景
/// </summary>
namespace StatusScene {

    /// <summary>
    /// 艾瑟萌状态
    /// </summary>
    namespace ExermonStatus {

        /// <summary>
        /// 艾瑟萌页控制器
        /// </summary>
        public class PageTabController : TabView<ExermonStatusPageDisplay> {


            /// <summary>
            /// 内部变量定义
            /// </summary>
            public ExerSlotItem slotItem { get; private set; }
            public Player player { get; private set; }
            public Subject subject { get; private set; }

            #region 数据控制

            /// <summary>
            /// 设置
            /// </summary>
            /// <param name="player">玩家</param>
            /// <param name="subject">科目</param>
            public void setup(Player player, Subject subject) {
                this.player = player; this.subject = subject;
                if (player == null || subject == null) slotItem = null;
                else slotItem = player.getExerSlotItem(subject);
                requestRefresh();
            }

            #endregion

            #region 界面绘制

            /// <summary>
            /// 显示内容页
            /// </summary>
            /// <param name="content"></param>
            protected override void showContent(ExermonStatusPageDisplay content, int index) {
                UnityAction action = () => content.startView(slotItem, true);
                if (content.initialized) action.Invoke();
                else {
                    var loadFunc = content.getLoadFunction();
                    if (loadFunc == null) action.Invoke();
                    else loadFunc.Invoke(action);
                }
            }

            /// <summary>
            /// 显示内容页
            /// </summary>
            /// <param name="content"></param>
            protected override void hideContent(ExermonStatusPageDisplay content, int index) {
                content.terminateView();
            }

            #endregion

            #region 流程控制

            /// <summary>
            /// 装备回调
            /// </summary>
            public void onEquip() {
                currentContent()?.equipCurrentItem();
            }

            /// <summary>
            /// 卸下回调
            /// </summary>
            public void onDequip() {
                currentContent()?.dequipCurrentItem();
            }

            #endregion

        }

        /// <summary>
        /// Exermon 状态中每页总控制组件的基类
        /// </summary>
        public class ExermonStatusPageDisplay : ItemDisplay<ExerSlotItem> {

            /// <summary>
            /// 外部组件设置
            /// </summary>
            public PageTabController tabController;
            
            public GameObject equip, dequip;

            /// <summary>
            /// 内部组件设置
            /// </summary>
            Button equipBtn, dequipBtn;

            /// <summary>
            /// 外部系统设置
            /// </summary>
            protected PlayerService playerSer;
            protected ExermonService exerSer;

            #region 初始化

            protected override void initializeOnce() {
                base.initializeOnce();
                equipBtn = SceneUtils.button(equip);
                dequipBtn = SceneUtils.button(dequip);
            }

            /// <summary>
            /// 初始化系统/服务
            /// </summary>
            protected override void initializeSystems() {
                base.initializeSystems();
                playerSer = PlayerService.get();
                exerSer = ExermonService.get();
            }

            #endregion

            #region 启动控制

            /// <summary>
            /// 加载函数
            /// </summary>
            /// <returns></returns>
            public virtual UnityAction<UnityAction> getLoadFunction() { return null; }

            #endregion

            #region 数据控制

            /// <summary>
            /// 装备当前装备物品（子类继承）
            /// </summary>
            public virtual void equipCurrentItem() { }

            /// <summary>
            /// 卸下当前装备物品（子类继承）
            /// </summary>
            public virtual void dequipCurrentItem() { }

            #endregion

            #region 流程控制

            /// <summary>
            /// 状态变更回调
            /// </summary>
            public virtual void onEquipChanged() {
                requestRefresh(true);
            }

            #region 请求控制

            /// <summary>
            /// 可否装备
            /// </summary>
            /// <returns></returns>
            public virtual bool equipable() { return equipRequestFunc() != null; }
            public virtual bool dequipable() { return dequipRequestFunc() != null; }

            /// <summary>
            /// 请求更改艾瑟萌，并更新整个页面
            /// </summary>
            protected void requestEquip() {
                equipRequestFunc()?.Invoke(onEquipChanged);
            }

            /// <summary>
            /// 请求更改艾瑟萌，并更新整个页面
            /// </summary>
            protected void requestDequip() {
                dequipRequestFunc()?.Invoke(onEquipChanged);
            }

            /// <summary>
            /// 装备请求函数
            /// </summary>
            /// <returns></returns>
            protected virtual UnityAction<UnityAction> equipRequestFunc() {
                return null;
            }

            /// <summary>
            /// 装备请求函数
            /// </summary>
            /// <returns></returns>
            protected virtual UnityAction<UnityAction> dequipRequestFunc() {
                return null;
            }

            #endregion

            #endregion

            #region 界面绘制

            /// <summary>
            /// 切换按钮
            /// </summary>
            public virtual void refreshButtons() {
                equip.SetActive(equipable());
                dequip.SetActive(!equip.activeSelf && dequipable());
                equipBtn.interactable = equip.activeSelf;
                dequipBtn.interactable = dequip.activeSelf;
                if (!equip.activeSelf && !dequip.activeSelf) {
                    var obj = defaultShownObject();
                    if (obj == null) return;
                    var btn = SceneUtils.button(obj);
                    obj.SetActive(true);
                }
            }

            /// <summary>
            /// 默认显示按钮
            /// </summary>
            public virtual GameObject defaultShownObject() {
                return equip;
            }
            /*
            /// <summary>
            /// 刷新
            /// </summary>
            protected override void refresh() {
                base.refresh();
                refreshButtons();
            }
            */
            #endregion
        }

        /// <summary>
        /// Exermon 状态中每页总控制组件的基类
        /// </summary>
        public class ExermonStatusPageDisplay<T> : ExermonStatusPageDisplay where T : PackContItem, new() {

            #region 初始化

            /// <summary>
            /// 初始化
            /// </summary>
            protected override void initializeOnce() {
                base.initializeOnce();
                var slotDisplay = getSlotItemDisplay();
                var packDisplay = getPackDisplay();
                if (slotDisplay != null) slotDisplay.pageDisplay = this;
                packDisplay?.addCallback(refreshButtons, 0);
                packDisplay?.addCallback(refreshButtons, 1);
            }

            #endregion

            #region 数据控制

            /// <summary>
            /// 获取容器组件
            /// </summary>
            protected virtual ItemContainer<T> getPackDisplay() { return null; }

            /// <summary>
            /// 获取艾瑟萌槽项显示组件
            /// </summary>
            protected virtual ExermonStatusSlotItemDisplay<T> getSlotItemDisplay() { return null; }

            /// <summary>
            /// 装备当前装备物品
            /// </summary>
            public override void equipCurrentItem() {
                var container = getPackDisplay();
                var slotDisplay = getSlotItemDisplay();
                if (container == null || slotDisplay == null) return;
                slotDisplay.setEquip(container.selectedItem());
                requestEquip();
            }

            /// <summary>
            /// 卸下当前装备物品
            /// </summary>
            public override void dequipCurrentItem() {
                var container = getPackDisplay();
                var slotDisplay = getSlotItemDisplay();
                if (container == null || slotDisplay == null) return;
                slotDisplay.setEquip(null);
                requestDequip();
            }

            /// <summary>
            /// 获取容器物品数据
            /// </summary>
            /// <returns></returns>
            protected virtual T getFirstItem() {
                return item.getEquip<T>();
            }

            /// <summary>
            /// 获取容器物品数据
            /// </summary>
            /// <returns></returns>
            protected virtual List<T> getContainerItems() {
                var containerData = getPackContainerData();
                if (containerData == null) return new List<T>();
                return containerData.getItems(packCondition());
            }

            /// <summary>
            /// 获取容器物品数据
            /// </summary>
            /// <returns></returns>
            protected virtual PackContainer<T> getPackContainerData() {
                return playerSer.player.packContainers.getContainer<T>();
            }

            /// <summary>
            /// 背包条件
            /// </summary>
            /// <returns></returns>
            protected virtual Predicate<T> packCondition() { return (item) => true; }

            #endregion

            #region 画面绘制

            /// <summary>
            /// 刷新槽显示
            /// </summary>
            void refreshSlotDisplay() {
                getSlotItemDisplay()?.setItem(item);
            }

            /// <summary>
            /// 刷新背包容器
            /// </summary>
            void refreshPackContainer() {
                var packDisplay = getPackDisplay();
                if (packDisplay == null) return;
                var items = new List<T>();
                var first = getFirstItem();
                if (first != null && !first.isNullItem())
                    items.Add(first);
                items.AddRange(getContainerItems());
                packDisplay.configure(items);
                packDisplay.select(0);
            }

            /// <summary>
            /// 刷新
            /// </summary>
            protected override void refresh() {
                base.refresh();
                if (item != null) {
                    refreshSlotDisplay();
                    refreshPackContainer();
                }
            }

            #endregion

            #region 请求控制

            /// <summary>
            /// 可否装备
            /// </summary>
            /// <returns></returns>
            public override bool equipable() {
                if (!base.equipable()) return false;
                var packDisplay = getPackDisplay();
                var slotItemDisplay = getSlotItemDisplay();
                var selectedItem = packDisplay.selectedItem();
                var equipedItem = slotItemDisplay.getEquip();
                if (selectedItem == null || selectedItem == equipedItem) return false;
                return !selectedItem.isNullItem();
            }
            public override bool dequipable() {
                if (!base.dequipable()) return false;
                var slotItemDisplay = getSlotItemDisplay();
                var equipedItem = slotItemDisplay.getEquip();
                return equipedItem != null && !equipedItem.isNullItem();
            }

            #endregion
        }

        /// <summary>
        /// 状态窗口艾瑟萌槽信息显示
        /// </summary>
        public class ExermonStatusSlotItemDisplay<T> : SlotItemDisplay<ExerSlotItem, T>
            where T : PackContItem, new() {

            /// <summary>
            /// 详情显示组件
            /// </summary>
            protected virtual ExermonStatusExerSlotDetail<T> detail { get; set; }

            /// <summary>
            /// 页显示组件
            /// </summary>
            public ExermonStatusPageDisplay<T> pageDisplay { get; set; }

            #region 数据控制
            
            /// <summary>
            /// 配置装备
            /// </summary>
            protected override void setupEquip() {
                equip = item.getEquip<T>();
            }

            /// <summary>
            /// 装备变更回调
            /// </summary>
            protected override void onItemChanged() {
                base.onItemChanged();
                detail.setSlotItem(item);
            }
            
            /// <summary>
            /// 预览变更回调
            /// </summary>
            protected override void onPreviewChanged() {
                base.onPreviewChanged();
                detail.setItem(previewingEquip);
            }

            #endregion

            #region 界面绘制

            /// <summary>
            /// 绘制物品
            /// </summary>
            /// <param name="slotItem">艾瑟萌槽项</param>
            protected override void drawExactlyEquip(T equipItem) {
                base.drawExactlyEquip(equipItem);
                detail.setItem(equipItem);
            }

            #endregion

        }


        /// <summary>
        /// 状态窗口艾瑟萌槽详细信息显示
        /// </summary>
        public class ExermonStatusExerSlotDetail<T> : ItemDetail<T> where T : PackContItem, new() {
            
            /// <summary>
            /// 外部组件设置
            /// </summary>
            public ParamDisplaysGroup paramInfo;
            public ParamDisplay battlePoint;

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
                if(paramInfo!=null) {
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

            #region 界面绘制

            /// <summary>
            /// 绘制物品
            /// </summary>
            /// <param name="slotItem">艾瑟萌槽项</param>
            protected override void drawExactlyItem(T contItem) {
                base.drawExactlyItem(contItem);
                if (!contItem.isNullItem())
                    drawMainInfo(contItem);
                else clearMainInfo();
                drawSlotInfo();
                drawParamsInfo(contItem);
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
            /// 绘制空白项
            /// </summary>
            protected override void drawEmptyItem() {
                base.drawEmptyItem();
                if (slotItem != null) {
                    drawSlotParamsInfo();
                    paramInfo?.setIgnoreTrigger();
                } 
            }

            /// <summary>
            /// 清除物品
            /// </summary>
            protected override void clearItem() {
                clearMainInfo();
                clearSlotInfo();
                clearParamsInfo();
            }

            /// <summary>
            /// 清除主体信息
            /// </summary>
            protected virtual void clearMainInfo() { }

            /// <summary>
            /// 清除属性信息
            /// </summary>
            protected virtual void clearSlotInfo() { }

            /// <summary>
            /// 清除属性信息
            /// </summary>
            void clearParamsInfo() {
                paramInfo?.clearValues();
                battlePoint?.clearValue();

                paramInfo?.setIgnoreTrigger();
            }

            #endregion

        }
    }
}