using System;

using UnityEngine;

using Core.Data;

using ItemModule.Data;
using ItemModule.Services;

using GameModule.Services;

using UI.Common.Controls.InputFields;
using UI.Common.Controls.ItemDisplays;

namespace UI.ShopScene.Controls {

    /// <summary>
    /// 艾瑟萌装备商店显示
    /// </summary>
    public abstract class ShopDisplay<T> : 
        SelectableContainerDisplay<ItemService.ShopItem<T>> 
        where T: BaseItem, new() {

        /// <summary>
        /// 常量设置
        /// </summary>
        protected const string UnlimitedText = "不限";

        protected const int UnlimitedIndex = -1;

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public DropdownField typeSelector, starSelector;

        /// <summary>
        /// 物品详情控件
        /// </summary>
        public abstract ShopItemDetail<T> itemDetail { get; set; }

        /// <summary>
        /// 商品缓存
        /// </summary>
        public ItemService.ShopItem<T>[] shopItems { get; set; } = null;

        /// <summary>
        /// 外部系统设置
        /// </summary>
        protected DataService dataSer;
        protected ItemService itemSer;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeEvery() {
            base.initializeEvery();
            configureSelectors();
        }

        /// <summary>
        /// 配置筛选器
        /// </summary>
        void configureSelectors() {
            typeSelector?.configure(generateTypesData());
            starSelector?.configure(generateStarsData());
			if (typeSelector) typeSelector.onChanged = onSelectorChanged;
			if (starSelector) starSelector.onChanged = onSelectorChanged;
            //typeSelector.setValue(UnlimitedIndex);
            //starSelector.setValue(UnlimitedIndex);
        }

        /// <summary>
        /// 生成类型下拉框数据
        /// </summary>
        /// <returns></returns>
        protected virtual Tuple<int, string>[] generateTypesData() {

            var data = typeData();
            var res = new Tuple<int, string>[data.Length + 1];

            res[0] = new Tuple<int, string>(UnlimitedIndex, UnlimitedText);
            for (int i = 1; i < res.Length; ++i) {
                var _data = data[i-1];
                res[i] = new Tuple<int, string>(
                    _data.id, _data.name);
            }

            return res;
            //var base_ = new Tuple<int, string>[1] {
            //    new Tuple<int, string>(UnlimitedIndex, UnlimitedText),
            //};
            //var baseLen = base_.Length;

            //var types = typeData(); // dataSer.staticData.configure.exerEquipTypes;
            //var res = new Tuple<int, string>[types.Length + baseLen];

            //for (int i = 0; i < res.Length; ++i)
            //    if (i < baseLen) res[i] = base_[i];
            //    else {
            //        var type = types[i - baseLen];
            //        res[i] = new Tuple<int, string>(type.id, type.name);
            //    }

            //return res;
        }

        /// <summary>
        /// 类型数据
        /// </summary>
        /// <returns></returns>
        protected abstract TypeData[] typeData();

        /// <summary>
        /// 生成类型下拉框数据
        /// </summary>
        /// <returns></returns>
        Tuple<int, string>[] generateStarsData() {

			var data = starData();
            var res = new Tuple<int, string>[data.Length + 1];

            res[0] = new Tuple<int, string>(UnlimitedIndex, UnlimitedText);
            for (int i = 1; i < res.Length; ++i) {
                var _data = data[i-1];
                res[i] = new Tuple<int, string>(
                    _data.id, _data.name);
            }

            return res;
		}

		/// <summary>
		/// 星级数据
		/// </summary>
		/// <returns></returns>
		protected virtual TypeData[] starData() {
			return dataSer.staticData.configure.itemStars;
		}

		/// <summary>
		/// 初始化外部系统
		/// </summary>
		protected override void initializeSystems() {
            base.initializeSystems();
            dataSer = DataService.get();
            itemSer = ItemService.get();
        }

        #endregion

        #region 启动/结束视窗

        /// <summary>
        /// 启动视窗
        /// </summary>
        /// <param name="items"></param>
        public override void startView() {
            if (shopItems == null || shopItems.Length == 0) {
                base.startView(); generateShop();
			} else startView(shopItems);
        }

        /// <summary>
        /// 启动视窗
        /// </summary>
        /// <param name="items"></param>
        void startView(ItemService.ShopItem<T>[] items) {
            base.startView(); itemDetail.startView();
            setItems(shopItems = items);
        }

        /// <summary>
        /// 结束视窗
        /// </summary>
        public override void terminateView() {
            base.terminateView();
            itemDetail?.terminateView();
        }

		#endregion

		#region 数据控制

		/// <summary>
		/// 生成商品
		/// </summary>
		protected virtual void generateShop() {
			itemSer.getShop<T>(setItems);
		}

		/// <summary>
		/// 设置商品集
		/// </summary>
		/// <param name="items"></param>
		public override void setItems(ItemService.ShopItem<T>[] items) {
            shopItems = items; base.setItems(items);
        }

        /// <summary>
        /// 获取物品帮助组件
        /// </summary>
        /// <returns>帮助组件</returns>
        public override IItemDetailDisplay<ItemService.ShopItem<T>>
           getItemDetail() { return itemDetail; }

        /// <summary>
        /// 是否包含物品
        /// </summary>
        /// <param name="shopItem">物品</param>
        /// <returns>返回指定物品能否包含在容器中</returns>
        protected override bool isIncluded(ItemService.ShopItem<T> shopItem) {
            if (!base.isIncluded(shopItem)) return false;

            // 判断星级
            var item = shopItem.item() as LimitedItem;

            if (item != null) {
                var starIndex = starSelector.getValueId();
                Debug.Log("starIndex: " + starIndex);
                return starIndex == UnlimitedIndex || 
                    starIndex == item.starId;
            } else return true;
        }

        /// <summary>
        /// 筛选器变化回调
        /// </summary>
        /// <param name="index"></param>
        void onSelectorChanged(Tuple<int, string> data) {
            if (data!=null)
                Debug.Log("onSelectorChanged: " + name + ": " + data.Item1);
            else
                Debug.Log("onSelectorChanged: " + name + ": null");
            setItems(shopItems);
        }

        #endregion
    }
}