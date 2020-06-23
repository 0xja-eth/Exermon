using System;

using UnityEngine;

using Core.Data;

using ItemModule.Data;
using ItemModule.Services;

using GameModule.Services;

using UI.Common.Controls.InputFields;
using UI.Common.Controls.ItemDisplays;

using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;

namespace UI.ExerPro.EnglishPro.BusinessManScene.Controls {

    /// <summary>
    /// 商人据点显示
    /// </summary>
    public abstract class ShopDisplay<T> :
        SelectableContainerDisplay<T> where T : BaseExerProItem {

        /// <summary>
        /// 常量设置
        /// </summary>
        protected const string UnlimitedText = "不限";

        protected const int UnlimitedIndex = -1;

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public DropdownField starSelector;

        /// <summary>
        /// 商品缓存
        /// </summary>
        public T[] shopItems { get; set; } = null;

        /// <summary>
        /// 外部系统设置
        /// </summary>
        protected DataService dataSer;
        protected EnglishService engSer;

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
            starSelector.configure(generateStarsData());
            starSelector.onChanged = onSelectorChanged;
        }

        /// <summary>
        /// 生成类型下拉框数据
        /// </summary>
        /// <returns></returns>
        Tuple<int, string>[] generateStarsData() {
            var data = dataSer.staticData.configure.exerProItemStars;
            var res = new Tuple<int, string>[data.Length + 1];

            res[0] = new Tuple<int, string>(UnlimitedIndex, UnlimitedText);
            for (int i = 1; i < res.Length; ++i) {
                var _data = data[i - 1];
                res[i] = new Tuple<int, string>(
                    _data.id, _data.name);
            }

            return res;
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            dataSer = DataService.get();
            engSer = EnglishService.get();
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 设置商品集
        /// </summary>
        /// <param name="items"></param>
        public override void setItems(T[] items) {
            shopItems = items;
            base.setItems(items);
        }

        /// <summary>
        /// 是否包含物品
        /// </summary>
        /// <param name="shopItem">物品</param>
        /// <returns>返回指定物品能否包含在容器中</returns>
        protected override bool isIncluded(T shopItem) {
            if (!base.isIncluded(shopItem)) return false;

            // 判断星级
            var starIndex = starSelector.getValueId();
            Debug.Log("starIndex: " + starIndex);
            return starIndex == UnlimitedIndex ||
                starIndex == shopItem.starId;
        }

        /// <summary>
        /// 筛选器变化回调
        /// </summary>
        /// <param name="index"></param>
        void onSelectorChanged(Tuple<int, string> data) {
            if (data != null)
                Debug.Log("onSelectorChanged: " + name + ": " + data.Item1);
            else
                Debug.Log("onSelectorChanged: " + name + ": null");
            setItems(shopItems);
        }

        #endregion

        /// <summary>
        /// 结束视图，重置价格
        /// </summary>
        public override void terminateView() {
            base.terminateView();
            Debug.Log("Shop Display Terminate!");
            if (shopItems != null) {
                foreach (var item in shopItems) {
                    item.gold = 0;
                }
            }
        }
    }
}