
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using LitJson;

/// <summary>
/// 物品服务
/// </summary>
public class ItemService : BaseService<ItemService> {

    /// <summary>
    /// 操作文本设定
    /// </summary>
    const string GetPack = "获取数据";
    const string GetSlot = "获取数据";

    const string GainItem = "获得物品";
    const string LostItem = "失去物品";
    const string TransferItem = "转移物品";
    const string SplitItem = "分解物品";
    const string MergeItem = "组合物品";

    /// <summary>
    /// 业务操作
    /// </summary>
    public enum Oper {
        GetPack, GetSlot, GainItem, LostItem, TransferItem, SplitItem, MergeItem
    }
    
    #region 初始化
    
    /// <summary>
    /// 初始化操作字典
    /// </summary>
    protected override void initializeOperDict() {
        base.initializeOperDict();
        addOperDict(Oper.GetPack, GetPack, NetworkSystem.Interfaces.ItemGetPack);
        addOperDict(Oper.GetSlot, GetSlot, NetworkSystem.Interfaces.ItemGetSlot);
        addOperDict(Oper.GainItem, GainItem, NetworkSystem.Interfaces.ItemGainItem);
        addOperDict(Oper.LostItem, LostItem, NetworkSystem.Interfaces.ItemGainItem);
        addOperDict(Oper.TransferItem, TransferItem, NetworkSystem.Interfaces.ItemTransferItem);
        addOperDict(Oper.SplitItem, SplitItem, NetworkSystem.Interfaces.ItemSplitItem);
        addOperDict(Oper.MergeItem, MergeItem, NetworkSystem.Interfaces.ItemMergeItem);
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

        getPack(container.type, container.getID(), _onSuccess, onError);
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

        getSlot(container.type, container.getID(), _onSuccess, onError);
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
    /// 获得物品
    /// </summary>
    /// <param name="container">容器</param>
    /// <param name="item">物品</param>
    /// <param name="count">数量</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void gainItem<T>(PackContainer<T> container, BaseItem item, int count,
        UnityAction onSuccess, UnityAction onError = null) where T : PackContItem, new() {

        NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
            if(container.realTime)
                container = DataLoader.load(container, res, "container");
            onSuccess?.Invoke();
        };

        gainItem(container.type, container.getID(), item.type, item.getID(), count, 
            container.realTime, _onSuccess, onError);
    }
    /// <param name="type">容器类型</param>
    /// <param name="cid">容器ID</param>
    /// <param name="iType">物品类型</param>
    /// <param name="itemId">物品ID</param>
    /// <param name="realTime">实时刷新</param>
    public void gainItem(int type, int cid, int iType, int itemId, int count, bool realTime,
        NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
        JsonData data = new JsonData();
        data["type"] = type; data["cid"] = cid;
        data["i_type"] = iType; data["item_id"] = itemId;
        data["count"] = count; data["refresh"] = realTime;
        sendRequest(Oper.GainItem, data, onSuccess, onError, uid: true);
    }

    /// <summary>
    /// 获得物品
    /// </summary>
    /// <param name="container">容器</param>
    /// <param name="item">物品</param>
    /// <param name="count">数量</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void lostItem<T>(PackContainer<T> container, BaseItem item, int count,
        UnityAction onSuccess, UnityAction onError = null) where T : PackContItem, new() {
        gainItem(container, item, -count, onSuccess, onError);
    }
    /// <param name="type">容器类型</param>
    /// <param name="cid">容器ID</param>
    /// <param name="iType">物品类型</param>
    /// <param name="itemId">物品ID</param>
    /// <param name="realTime">实时刷新</param>
    public void lostItem(int type, int cid, int iType, int itemId, int count, bool realTime,
        NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
        gainItem(type, cid, iType, itemId, -count, realTime, onSuccess, onError);
    }

    /// <summary>
    /// 转移物品
    /// </summary>
    /// <param name="container">容器</param>
    /// <param name="targetCid">目标容器ID</param>
    /// <param name="contItems">容器项集</param>
    /// <param name="counts">每个容器项转移的数量</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void transferItem<T>(PackContainer<T> container, int targetCid, T[] contItems, int[] counts,
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
            contItemIds[i] = contItems[i].getID();
        }

        transferItem(container.type, container.getID(), targetCid, ciTypes, 
            contItemIds, counts, _onSuccess, onError);
    }
    /// <param name="type">容器类型</param>
    /// <param name="cid">容器ID</param>
    /// <param name="ciTypes">容器项类型集</param>
    /// <param name="contItemIds">容器项ID集</param>
    public void transferItem(int type, int cid, int targetCid, 
        int[] ciTypes, int[] contItemIds, int[] counts,
        NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
        JsonData data = new JsonData();
        data["type"] = type; data["cid"] = cid;
        data["target_cid"] = targetCid;
        data["contitem_ids"] = DataLoader.convert(contItemIds);
        data["counts"] = DataLoader.convert(counts);
        sendRequest(Oper.TransferItem, data, onSuccess, onError, uid: true);
    }

    /// <summary>
    /// 拆分物品
    /// </summary>
    /// <param name="container">容器</param>
    /// <param name="targetCid">目标容器ID</param>
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

        splitItem(container.type, container.getID(), contItem.type,
            contItem.getID(), count, _onSuccess, onError);
    }
    /// <param name="type">容器类型</param>
    /// <param name="cid">容器ID</param>
    /// <param name="ciType">容器项类</param>
    /// <param name="contItemId">容器项ID</param>
    public void splitItem(int type, int cid, int ciType, int contItemId, int count,
        NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
        JsonData data = new JsonData();
        data["type"] = type; data["cid"] = cid;
        data["count"] = count; data["contitem_id"] = contItemId;
        sendRequest(Oper.SplitItem, data, onSuccess, onError, uid: true);
    }

    /// <summary>
    /// 合并物品
    /// </summary>
    /// <param name="container">容器</param>
    /// <param name="contItems">容器项</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void mergeItem<T>(PackContainer<T> container, T[] contItems,
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
            contItemIds[i] = contItems[i].getID();
        }

        mergeItem(container.type, container.getID(), 
            ciTypes, contItemIds, _onSuccess, onError);
    }
    /// <param name="type">容器类型</param>
    /// <param name="cid">容器ID</param>
    /// <param name="ciTypes">容器项类型集</param>
    /// <param name="contItemIds">容器项ID集</param>
    public void mergeItem(int type, int cid, int[] ciTypes, int[] contItemIds,
        NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
        JsonData data = new JsonData();
        data["type"] = type; data["cid"] = cid; 
        data["contitem_ids"] = DataLoader.convert(contItemIds);
        sendRequest(Oper.MergeItem, data, onSuccess, onError, uid: true);
    }

    #endregion
}