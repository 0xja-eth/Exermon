
using System;
using System.Collections.Generic;

using UnityEngine.Events;

using LitJson;

using Core.Data;
using Core.Data.Loaders;
using Core.Systems;
using Core.Services;

using GameModule.Services;
using PlayerModule.Services;

using ItemModule.Data;
using ExermonModule.Data;
using UnityEngine;

/// <summary>
/// 物品模块服务
/// </summary>
namespace ItemModule.Services {

    /// <summary>
    /// 物品服务
    /// </summary>
    public class ItemService : BaseService<ItemService> {

        /// <summary>
        /// 商品物品
        /// </summary>
        public class ShopItem : BaseData {

            /// <summary>
            /// 属性
            /// </summary>
            [AutoConvert]
            public int type { get; protected set; }
            [AutoConvert]
            public ItemPrice price { get; protected set; }

        }
        public class ShopItem<T> : ShopItem where T : BaseItem, new() {
            
            /// <summary>
            /// 获取对应的物品
            /// </summary>
            /// <returns></returns>
            public T item() {
                return DataService.get().get<T>(id);
            }

        }

        /// <summary>
        /// 操作文本设定
        /// </summary>
        const string GetPack = "获取数据";
        const string GetSlot = "获取数据";

        const string GainItem = "获得物品";
        const string LostItem = "失去物品";
        const string TransferItems = "转移物品";
        const string SplitItem = "分解物品";
        const string MergeItems = "组合物品";

        const string UseItem = "使用物品";

        const string DiscardItem = "丢弃物品";
        const string SellItem = "出售物品";

        const string BuyItem = "购买物品";
        const string GetShop = "获取商品";

        /// <summary>
        /// 业务操作
        /// </summary>
        public enum Oper {
            GetPack, GetSlot, GainItem, LostItem,
            GainContItems, LostContItems,
            TransferItems, SplitItem, MergeItems,
            UseItem,
            DiscardItem, SellItem, 
            BuyItem, GetShop
        }

        /// <summary>
        /// 外部系统设置
        /// </summary>
        PlayerService playerSer;

        #region 初始化

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            playerSer = PlayerService.get();
        }

        /// <summary>
        /// 初始化操作字典
        /// </summary>
        protected override void initializeOperDict() {
            base.initializeOperDict();
            addOperDict(Oper.GetPack, GetPack, NetworkSystem.Interfaces.ItemGetPack);
            addOperDict(Oper.GetSlot, GetSlot, NetworkSystem.Interfaces.ItemGetSlot);
            addOperDict(Oper.GainItem, GainItem, NetworkSystem.Interfaces.ItemGainItem);
            addOperDict(Oper.LostItem, LostItem, NetworkSystem.Interfaces.ItemGainItem);
            addOperDict(Oper.GainContItems, GainItem, NetworkSystem.Interfaces.ItemGainContItems);
            addOperDict(Oper.LostContItems, LostItem, NetworkSystem.Interfaces.ItemLostContItems);
            addOperDict(Oper.TransferItems, TransferItems, NetworkSystem.Interfaces.ItemTransferItems);
            addOperDict(Oper.SplitItem, SplitItem, NetworkSystem.Interfaces.ItemSplitItem);
            addOperDict(Oper.MergeItems, MergeItems, NetworkSystem.Interfaces.ItemMergeItems);
            addOperDict(Oper.UseItem, UseItem, NetworkSystem.Interfaces.ItemUseItem);
            addOperDict(Oper.DiscardItem, DiscardItem, NetworkSystem.Interfaces.ItemDiscardItem);
            addOperDict(Oper.SellItem, SellItem, NetworkSystem.Interfaces.ItemSellItem);
            addOperDict(Oper.BuyItem, BuyItem, NetworkSystem.Interfaces.ItemBuyItem);
            addOperDict(Oper.GetShop, GetShop, NetworkSystem.Interfaces.ItemGetShop);
        }

        #endregion
        
        #region 操作控制

        /// <summary>
        /// 获取背包容器
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void getPack<T>(PackContainer<T> container,
            UnityAction onSuccess, UnityAction onError = null) where T : PackContItem, new() {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                container = DataLoader.load(container, res, "container");
                onSuccess?.Invoke();
            };

            getPack(container.type, container.id, _onSuccess, onError);
        }
        /// <param name="type">容器类型</param>
        /// <param name="cid">容器ID</param>
        public void getPack(int type, int cid,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
            JsonData data = new JsonData();
            data["type"] = type; data["cid"] = cid;
            sendRequest(Oper.GetPack, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 获取自身背包容器
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void getSelfPack<T>(PackContainer<T> container,
            UnityAction onSuccess, UnityAction onError = null) where T : PackContItem, new() {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                container = DataLoader.load(container, res, "container");
                onSuccess?.Invoke();
            };

            getSelfPack(container.type, _onSuccess, onError);
        }
        /// <param name="type">容器类型</param>
        public void getSelfPack(int type,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
            JsonData data = new JsonData(); data["type"] = type;
            sendRequest(Oper.GetPack, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 获取槽容器
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void getSlot<T>(SlotContainer<T> container,
            UnityAction onSuccess, UnityAction onError = null) where T : SlotContItem, new() {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                container = DataLoader.load(container, res, "container");
                onSuccess?.Invoke();
            };

            getSlot(container.type, container.id, _onSuccess, onError);
        }
        /// <param name="type">容器类型</param>
        /// <param name="cid">容器ID</param>
        public void getSlot(int type, int cid,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
            JsonData data = new JsonData();
            data["type"] = type; data["cid"] = cid;
            sendRequest(Oper.GetSlot, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 获取自身槽容器
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void getSelfSlot<T>(SlotContainer<T> container,
            UnityAction onSuccess, UnityAction onError = null) where T : SlotContItem, new() {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                container = DataLoader.load(container, res, "container");
                onSuccess?.Invoke();
            };

            getSelfSlot(container.type, _onSuccess, onError);
        }
        /// <param name="type">容器类型</param>
        public void getSelfSlot(int type, 
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
            JsonData data = new JsonData(); data["type"] = type; 
            sendRequest(Oper.GetSlot, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 获得物品
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="item">物品</param>
        /// <param name="count">数量</param>
        /// <param name="realTime">实时刷新</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void gainItem<T>(PackContainer<T> container, BaseItem item, int count, bool realTime,
            UnityAction onSuccess, UnityAction onError = null) where T : PackContItem, new() {

            if (count == 0) { onSuccess?.Invoke(); return; }

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                if (realTime) container = DataLoader.load(container, res, "container");
                onSuccess?.Invoke();
            };

            gainItem(container.type, container.id, item.type, item.id, 
                count, realTime, _onSuccess, onError);
        }
        /// <param name="type">容器类型</param>
        /// <param name="cid">容器ID</param>
        /// <param name="iType">物品类型</param>
        /// <param name="itemId">物品ID</param>
        public void gainItem(int type, int cid, int iType, int itemId, int count, bool realTime,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {

            JsonData data = new JsonData();
            data["type"] = type; data["cid"] = cid;
            data["i_type"] = iType; data["item_id"] = itemId;
            data["count"] = count; data["refresh"] = realTime;
            var oper = count >= 0 ? Oper.GainItem : Oper.LostItem;
            sendRequest(oper, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 失去物品
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="item">物品</param>
        /// <param name="count">数量</param>
        /// <param name="realTime">实时刷新</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void lostItem<T>(PackContainer<T> container, BaseItem item, int count, bool realTime,
            UnityAction onSuccess, UnityAction onError = null) where T : PackContItem, new() {
            gainItem(container, item, -count, realTime, onSuccess, onError);
        }
        /// <param name="type">容器类型</param>
        /// <param name="cid">容器ID</param>
        /// <param name="iType">物品类型</param>
        /// <param name="itemId">物品ID</param>
        public void lostItem(int type, int cid, int iType, int itemId, int count, bool realTime,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
            gainItem(type, cid, iType, itemId, -count, realTime, onSuccess, onError);
        }

        /// <summary>
        /// 获得容器项
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="contItem">容器项</param>
        /// <param name="realTime">实时刷新</param>
        /// <param name="fixed_">整体操作</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void gainContItem<T>(PackContainer<T> container, T contItem,
            bool realTime = true, bool fixed_ = false, UnityAction onSuccess = null,
            UnityAction onError = null) where T : PackContItem, new() {
            var contItems = new T[] { contItem };
            gainContItems(container, contItems, realTime, fixed_, onSuccess, onError);
        }
        /// <param name="contItems">容器项集</param>
        public void gainContItems<T>(PackContainer<T> container, T[] contItems,
            bool realTime = true, bool fixed_ = false, UnityAction onSuccess = null,
            UnityAction onError = null) where T : PackContItem, new() {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                if (realTime) container = DataLoader.load(container, res, "container");
                onSuccess?.Invoke();
            };

            var count = contItems.Length;
            var ciTypes = new int[count];
            var contItemIds = new int[count];
            for (int i = 0; i < count; i++) {
                ciTypes[i] = contItems[i].type;
                contItemIds[i] = contItems[i].id;
            }

            gainContItems(container.type, ciTypes, contItemIds,
                realTime, fixed_, _onSuccess, onError);
        }
        /// <param name="type">容器类型</param>
        /// <param name="ciTypes">容器项类型集</param>
        /// <param name="contItemIds">容器项ID集</param>
        public void gainContItems(int type, int[] ciTypes, int[] contItemIds, bool realTime, bool fixed_,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {

            JsonData data = new JsonData();
            data["type"] = type; 
            data["ci_types"] = DataLoader.convert(ciTypes);
            data["contitem_ids"] = DataLoader.convert(contItemIds);
            data["fixed"] = fixed_; data["refresh"] = realTime;

            sendRequest(Oper.GainContItems, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 失去容器项
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="contItem">容器项</param>
        /// <param name="count">失去数目</param>
        /// <param name="realTime">实时刷新</param>
        /// <param name="fixed_">整体操作</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void lostContItem<T>(PackContainer<T> container, T contItem, int count = -1,
            bool realTime = true, bool fixed_ = false, UnityAction onSuccess = null,
            UnityAction onError = null) where T : PackContItem, new() {
            var contItems = new T[] { contItem };
            var counts = count >= 0 ? new int[] { count } : null;
            lostContItems(container, contItems, counts, realTime, fixed_, onSuccess, onError);
        }
        /// <param name="contItems">容器项集</param>
        /// <param name="counts">每个容器项失去的数目</param>
        public void lostContItems<T>(PackContainer<T> container, T[] contItems, 
            int[] counts = null, bool realTime = true, bool fixed_ = false, 
            UnityAction onSuccess = null, UnityAction onError = null) where T : PackContItem, new() {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                if (realTime) container = DataLoader.load(container, res, "container");
                onSuccess?.Invoke();
            };

            var count = contItems.Length;
            var ciTypes = new int[count];
            var contItemIds = new int[count];
            for (int i = 0; i < count; i++) {
                ciTypes[i] = contItems[i].type;
                contItemIds[i] = contItems[i].id;
            }

            lostContItems(container.type, ciTypes, contItemIds, counts,
                realTime, fixed_, _onSuccess, onError);
        }
        /// <param name="type">容器类型</param>
        /// <param name="ciTypes">容器项类型集</param>
        /// <param name="contItemIds">容器项ID集</param>
        public void lostContItems(int type, int[] ciTypes, int[] contItemIds, int[] counts, 
            bool realTime, bool fixed_, NetworkSystem.RequestObject.SuccessAction onSuccess, 
            UnityAction onError = null) {

            JsonData data = new JsonData();
            data["type"] = type;
            data["ci_types"] = DataLoader.convert(ciTypes);
            data["contitem_ids"] = DataLoader.convert(contItemIds);
            data["fixed"] = fixed_; data["refresh"] = realTime;

            if (counts != null) data["counts"] = DataLoader.convert(counts);

            sendRequest(Oper.LostContItems, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 转移物品
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="target">目标容器</param>
        /// <param name="contItem">容器项</param>
        /// <param name="count">转移数量</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void transferItem<T>(PackContainer<T> container, PackContainer<T> target, T contItem, int count = 1,
            UnityAction onSuccess = null, UnityAction onError = null) where T : PackContItem, new() {
            var contItems = new T[] { contItem };
            var counts = count >= 0 ? new int[] { count } : null;
            transferItems(container, target, contItems, counts, onSuccess, onError);
        }
        /// <param name="targetCid">目标容器ID</param>
        public void transferItem<T>(PackContainer<T> container, int targetCid, T contItem, int count = -1,
            UnityAction onSuccess = null, UnityAction onError = null) where T : PackContItem, new() {
            var contItems = new T[] { contItem };
            var counts = count >= 0 ? new int[] { count } : null;
            transferItems(container, targetCid, contItems, counts, onSuccess, onError);
        }
        /// <param name="contItems">容器项集</param>
        /// <param name="counts">每个容器项转移的数量</param>
        public void transferItems<T>(PackContainer<T> container, 
            PackContainer<T> target, T[] contItems, int[] counts = null, 
            UnityAction onSuccess = null, UnityAction onError = null) where T : PackContItem, new() {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                container = DataLoader.load(container, res, "container");
                target = DataLoader.load(target, res, "target");
                onSuccess?.Invoke();
            };

            var count = contItems.Length;
            var ciTypes = new int[count];
            var contItemIds = new int[count];
            for (int i = 0; i < count; i++) {
                ciTypes[i] = contItems[i].type;
                contItemIds[i] = contItems[i].id;
            }

            transferItems(container.type, target.id, ciTypes,
                contItemIds, counts, _onSuccess, onError);
        }
        public void transferItems<T>(PackContainer<T> container, 
            int targetCid, T[] contItems, int[] counts = null,
            UnityAction onSuccess = null, UnityAction onError = null) where T : PackContItem, new() {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                container = DataLoader.load(container, res, "container");
                onSuccess?.Invoke();
            };

            var count = contItems.Length;
            var ciTypes = new int[count];
            var contItemIds = new int[count];
            for (int i = 0; i < count; i++) {
                ciTypes[i] = contItems[i].type;
                contItemIds[i] = contItems[i].id;
            }

            transferItems(container.type, targetCid, ciTypes,
                contItemIds, counts, _onSuccess, onError);
        }
        /// <param name="type">容器类型</param>
        /// <param name="ciTypes">容器项类型集</param>
        /// <param name="contItemIds">容器项ID集</param>
        public void transferItems(int type, int targetCid,
            int[] ciTypes, int[] contItemIds, int[] counts,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
            JsonData data = new JsonData();
            data["type"] = type;
            data["target_cid"] = targetCid;
            data["ci_types"] = DataLoader.convert(ciTypes);
            data["contitem_ids"] = DataLoader.convert(contItemIds);

            if (counts != null) data["counts"] = DataLoader.convert(counts);

            sendRequest(Oper.TransferItems, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 拆分物品
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="contItem">容器项</param>
        /// <param name="count">拆分数量</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void splitItem<T>(PackContainer<T> container, T contItem, int count,
            UnityAction onSuccess, UnityAction onError = null) where T : PackContItem, new() {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                container = DataLoader.load(container, res, "container");
                onSuccess?.Invoke();
            };

            splitItem(container.type, contItem.type,
                contItem.id, count, _onSuccess, onError);
        }
        /// <param name="type">容器类型</param>
        /// <param name="ciType">容器项类</param>
        /// <param name="contItemId">容器项ID</param>
        public void splitItem(int type, int ciType, int contItemId, int count,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
            JsonData data = new JsonData();
            data["type"] = type; data["count"] = count;
            data["contitem_id"] = contItemId;
            sendRequest(Oper.SplitItem, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 合并物品
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="contItems">容器项</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void mergeItems<T>(PackContainer<T> container, T[] contItems,
            UnityAction onSuccess, UnityAction onError = null) where T : PackContItem, new() {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                container = DataLoader.load(container, res, "container");
                onSuccess?.Invoke();
            };

            var count = contItems.Length;
            var ciTypes = new int[count];
            var contItemIds = new int[count];
            for (int i = 0; i < count; i++) {
                ciTypes[i] = contItems[i].type;
                contItemIds[i] = contItems[i].id;
            }

            mergeItems(container.type, ciTypes, contItemIds, _onSuccess, onError);
        }
        /// <param name="type">容器类型</param>
        /// <param name="ciTypes">容器项类型集</param>
        /// <param name="contItemIds">容器项ID集</param>
        public void mergeItems(int type, int[] ciTypes, int[] contItemIds,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
            JsonData data = new JsonData();
            data["type"] = type;
            data["ci_types"] = DataLoader.convert(ciTypes);
            data["contitem_ids"] = DataLoader.convert(contItemIds);
            sendRequest(Oper.MergeItems, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 丢弃物品
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="contItem">容器项</param>
        /// <param name="count">丢弃数量</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void discardItem<T>(PackContainer<T> container, T contItem, int count,
            UnityAction onSuccess, UnityAction onError = null) where T : PackContItem, new() {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                container = DataLoader.load(container, res, "container");
                onSuccess?.Invoke();
            };

            discardItem(container.type, contItem.type,
                contItem.id, count, _onSuccess, onError);
        }
        /// <param name="type">容器类型</param>
        /// <param name="ciType">容器项类型</param>
        /// <param name="contItemId">容器项ID</param>
        public void discardItem(int type, int ciType, int contItemId, int count,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
            JsonData data = new JsonData();
            data["type"] = type;
            data["count"] = count;
            data["ci_type"] = ciType;
            data["contitem_id"] = contItemId;
            sendRequest(Oper.DiscardItem, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 出售物品
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="contItem">容器项</param>
        /// <param name="count">丢弃数量</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void sellItem<T>(PackContainer<T> container, T contItem, int count,
            UnityAction onSuccess, UnityAction onError = null) where T : PackContItem, new() {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                container = DataLoader.load(container, res, "container");
                getPlayer().loadMoney(DataLoader.load(res, "money"));
                onSuccess?.Invoke();
            };

            sellItem(container.type, contItem.type,
                contItem.id, count, _onSuccess, onError);
        }
        /// <param name="type">容器类型</param>
        /// <param name="ciType">容器项类型</param>
        /// <param name="contItemId">容器项ID</param>
        public void sellItem(int type, int ciType, int contItemId, int count,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
            JsonData data = new JsonData();
            data["type"] = type;
            data["count"] = count;
            data["ci_type"] = ciType;
            data["contitem_id"] = contItemId;
            sendRequest(Oper.SellItem, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 使用物品
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="contItem">容器项</param>
        /// <param name="count">丢弃数量</param>
        /// <param name="occasion">使用场合</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void useItem<T>(PackContainer<T> container, T contItem,
            int count, ItemUseOccasion occasion, PlayerExermon playerExer,
            UnityAction onSuccess = null, UnityAction onError = null) where T : PackContItem, new() {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                playerSer.loadPlayer(DataLoader.load(res, "player"));
                onSuccess?.Invoke();
            };

            useItem(container.type, contItem.type, contItem.id,
                count, (int)occasion, playerExer.id, _onSuccess, onError);
        }
        /// <param name="target">目标</param>
        public void useItem<T>(PackContainer<T> container, T contItem,
            int count, ItemUseOccasion occasion, UnityAction onSuccess = null, 
            UnityAction onError = null) where T : PackContItem, new() {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                playerSer.loadPlayer(DataLoader.load(res, "player"));
                onSuccess?.Invoke();
            };

            useItem(container.type, contItem.type, contItem.id,
                count, (int)occasion, 0, _onSuccess, onError);
        }
        /// <param name="type">容器类型</param>
        /// <param name="ciType">容器项类型</param>
        /// <param name="contItemId">容器项ID</param>
        public void useItem(int type, int ciType, int contItemId, int count, int occasion, int target,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
            JsonData data = new JsonData();
            data["type"] = type; data["ci_type"] = ciType;
            data["contitem_id"] = contItemId; data["count"] = count;
            data["occasion"] = occasion; data["target"] = target;
            sendRequest(Oper.UseItem, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 购买物品
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="item">物品</param>
        /// <param name="count">丢弃数量</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void buyItem<T>(PackContainer<PackContItem> container, T item, int count, int buyType,
            UnityAction onSuccess, UnityAction onError = null) where T : BaseItem, new() {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                container = DataLoader.load(container, res, "container");
                getPlayer().loadMoney(DataLoader.load(res, "money"));
                onSuccess?.Invoke();
            };

            buyItem(item.type, item.id, count, buyType, _onSuccess, onError);
        }
        /// <param name="type">物品类型</param>
        /// <param name="itemId">物品ID</param>
        public void buyItem(int type, int itemId, int count, int buyType,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
            JsonData data = new JsonData(); data["type"] = type;
            data["item_id"] = itemId; data["count"] = count; data["buy_type"] = buyType;
            sendRequest(Oper.BuyItem, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 获取商品
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="item">物品</param>
        /// <param name="count">丢弃数量</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void getShop<T>(UnityAction<ShopItem<T>[]> onSuccess, 
            UnityAction onError = null) where T : BaseItem, new() {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                var items = DataLoader.load<ShopItem<T>[]>(res, "items");
                Debug.Log("GetShop: " + string.Join(",", (object[])items));
                onSuccess?.Invoke(items);
            };

            var typeName = typeof(T).Name;
            var type = (int)Enum.Parse(typeof(BaseItem.Type), typeName);

            getShop(type, _onSuccess, onError);
        }
        /// <param name="type">物品类型</param>
        /// <param name="itemId">物品ID</param>
        public void getShop(int type,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
            JsonData data = new JsonData(); data["type"] = type;
            sendRequest(Oper.GetShop, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 槽操作
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="slot">槽</param>
        /// <param name="onSuccess">成功回调</param>
        public NetworkSystem.RequestObject.SuccessAction slotOperationSuccess<T, E>(
            PackContainer<T> container, SlotContainer<E> slot, UnityAction onSuccess) 
            where T: PackContItem, new() where E: SlotContItem, new() {
            return (res) => {
                container = DataLoader.load(container, res, "container");
                slot = DataLoader.load(slot, res, "slot");
                onSuccess?.Invoke();
            };
        }

        #endregion
    }
}