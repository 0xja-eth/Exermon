using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

using GameModule.Services;

using ItemModule.Data;
using PlayerModule.Data;
using ExermonModule.Data;

using UI.Common.Controls.ParamDisplays;
using UI.Common.Controls.ItemDisplays;

using UI.PackScene.Windows;
using UI.ExerPro.EnglishPro.ExerProPackScene.Windows;
using UI.ExerPro.EnglishPro.BattleScene.Controls.Menu;
using ExerPro.EnglishModule.Data;

namespace UI.ExerPro.EnglishPro.ExerProPackScene.CardPackItemDetail {

    /// <summary>
    /// 物品详情
    /// </summary>
    public class ExerProPackItemDetail : PackContItemDetail {

        /// <summary>
        /// 常量定义
        /// </summary>

        /// <summary>
        /// 外部变量定义
        /// </summary>
        public ExerProPackWindow exerProPackWindow;

        public StarsDisplay starsDisplay;
        public Text name, description;
        public Image icon;

        public MultParamsDisplay itemDetail, equipDetail;
        public ParamDisplaysGroup paramsGroup;

        public GameObject equip, use, discard, sell;

        /// <summary>
        /// 外部系统设置
        /// </summary>
        DataService dataSer;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            //configureParamsGroup();
        }
        #endregion

        ///// <summary>
        ///// 配置属性组
        ///// </summary>
        //void configureParamsGroup() {
        //    var params_ = dataSer.staticData.configure.baseParams;
        //    paramsGroup.configure(params_);
        //}

        ///// <summary>
        ///// 初始化外部系统
        ///// </summary>
        //protected override void initializeSystems() {
        //    base.initializeSystems();
        //    dataSer = DataService.get();
        //}

        /// <summary>
        /// 初始化绘制函数
        /// </summary>
        protected override void initializeDrawFuncs() {
            registerItemType<ExerProPackItem>(drawExerProPackItem);
            registerItemType<ExerProPackPotion>(drawExerProPackPotion);
            registerItemType<ExerProPackCard>(drawExerProPackCard);
            //registerItemType<ExerPackEquip>(drawExerPackEquip);
        }

        //#endregion

        //#region 数据控制

        //#region 物品控制

        ///// <summary>
        ///// 获取物品容器
        ///// </summary>
        ///// <returns>容器</returns>
        //public override IContainerDisplay<PackContItem> getContainer() {
        //    return exerProPackWindow.currentPackContainer();
        //}

        //#endregion

        ///// <summary>
        ///// 获取包含的道具
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <returns></returns>
        //public override T getContainedItem<T>() {
        //    if (typeof(T) == typeof(LimitedItem))
        //        return getContainedItem() as T;
        //    if (typeof(T) == typeof(UsableItem))
        //        return getContainedUsableItem() as T;
        //    return base.getContainedItem<T>();
        //}

        ///// <summary>
        ///// 获取包含的道具
        ///// </summary>
        ///// <returns></returns>
        //public LimitedItem getContainedItem() {
        //    LimitedItem item = getContainedItem<HumanItem>();
        //    item = item ?? getContainedItem<HumanEquip>();
        //    item = item ?? getContainedItem<ExerItem>();
        //    item = item ?? getContainedItem<ExerEquip>();
        //    return item;
        //}

        ///// <summary>
        ///// 获取包含的道具
        ///// </summary>
        ///// <returns></returns>
        //public UsableItem getContainedUsableItem() {
        //    UsableItem item = getContainedItem<HumanItem>();
        //    item = item ?? getContainedItem<ExerItem>();
        //    return item;
        //}

        ///// <summary>
        ///// 是否为装备
        ///// </summary>
        ///// <returns></returns>
        //public bool isEquip(LimitedItem item = null) {
        //    if (item == null) item = getContainedItem();
        //    return item.type == (int)BaseItem.Type.HumanEquip ||
        //        item.type == (int)BaseItem.Type.ExerEquip;
        //}

        ///// <summary>
        ///// 物品是否可用
        ///// </summary>
        ///// <returns></returns>
        //public bool isUsable(LimitedItem item = null) {
        //    if (isEquip(item)) return false;
        //    if (item == null) item = getContainedItem();
        //    return ((UsableItem)item).menuUse;
        //}

        //#endregion

        //#region 画面绘制

        /// <summary>
        /// 绘制基本信息
        /// </summary>
        /// <param name="item">物品1</param>
        void drawBaseInfo(BaseExerProItem item) {
            name.text = item.name;
            description.text = item.description;
            starsDisplay.setValue(item.starId);

            icon.gameObject.SetActive(true);
            icon.overrideSprite = item.icon;

            //setupButtons(item);
        }

        /// <summary>
        /// 绘制特训背包物品
        /// </summary>
        /// <param name="cardItem">特训背包物品</param>
        void drawExerProPackItem(ExerProPackItem Item) {
            drawBaseInfo(Item.item());
            drawItemDetail(Item);
        }

        /// <summary>
        /// 绘制特训背包药水
        /// </summary>
        /// <param name="cardItem">特训背包药水</param>
        void drawExerProPackPotion(ExerProPackPotion potionItem) {
            drawBaseInfo(potionItem.item());
            drawItemDetail(potionItem);
        }

        /// <summary>
        /// 绘制特训背包卡片
        /// </summary>
        /// <param name="cardItem">特训背包卡片</param>
        void drawExerProPackCard(ExerProPackCard cardItem) {
            drawBaseInfo(cardItem.item());
            drawItemDetail(cardItem);
        }



        ///// <summary>
        ///// 绘制艾瑟萌背包装备
        ///// </summary>
        ///// <param name="packEquip">艾瑟萌背包装备</param>
        //void drawExerPackEquip(ExerPackEquip packEquip) {
        //    drawBaseInfo(packEquip.item());
        //    drawEquipDetail(packEquip);
        //}

        /// <summary>
        /// 绘制物品详情
        /// </summary>
        /// <param name="obj"></param>
        void drawItemDetail(ParamDisplay.IDisplayDataConvertable obj) {
            equipDetail.gameObject.SetActive(false);
            itemDetail.gameObject.SetActive(true);
            itemDetail.setValue(obj, "detail");
        }

        ///// <summary>
        ///// 绘制物品详情
        ///// </summary>
        ///// <param name="obj"></param>
        //void drawEquipDetail(ParamDisplay.IDisplayDataConvertable obj) {
        //    itemDetail.gameObject.SetActive(false);
        //    equipDetail.gameObject.SetActive(true);
        //    equipDetail.setValue(obj, "detail");
        //}

        ///// <summary>
        ///// 配置控制按钮
        ///// </summary>
        ///// <param name="item">物品</param>
        //void setupButtons(LimitedItem item) {
        //    equip.SetActive(isEquip());
        //    use.SetActive(isUsable());
        //    discard.SetActive(item.discardable);
        //    sell.SetActive(item.sellable());
        //}

        ///// <summary>
        ///// 清除物品
        ///// </summary>
        //protected override void drawEmptyItem() {
        //    base.drawEmptyItem();
        //    starsDisplay.clearValue();
        //    icon.gameObject.SetActive(false);
        //    name.text = description.text = "";

        //    itemDetail.gameObject.SetActive(false);
        //    equipDetail.gameObject.SetActive(false);

        //    equip.SetActive(false);
        //    use.SetActive(false);
        //    sell.SetActive(false);
        //    discard.SetActive(false);
        //}

        //#endregion

    }
}
