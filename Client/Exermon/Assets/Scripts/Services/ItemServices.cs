
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
    /// <param name="cid">容器ID</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void getPack(int cid, 
        NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
        JsonData data = new JsonData(); data["cid"] = cid;
        sendRequest(Oper.GetPack, data, onSuccess, onError, uid: true);
    }

    /// <summary>
    /// 获取槽容器
    /// </summary>
    /// <param name="cid">容器ID</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void getSlot(int cid,
        NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
        JsonData data = new JsonData(); data["cid"] = cid;
        sendRequest(Oper.GetSlot, data, onSuccess, onError, uid: true);
    }

    /// <summary>
    /// 获得物品
    /// </summary>
    /// <param name="cid">容器ID</param>
    /// <param name="itemId">物品ID</param>
    /// <param name="count">数量</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void gainItem(int cid, int itemId, int count,
        NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
        JsonData data = new JsonData();
        data["cid"] = cid; data["item_id"] = itemId; data["count"] = count;
        sendRequest(Oper.GainItem, data, onSuccess, onError, uid: true);
    }

    /// <summary>
    /// 失去物品
    /// </summary>
    /// <param name="cid">容器ID</param>
    /// <param name="itemId">物品ID</param>
    /// <param name="count">数量</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void lostItem(int cid, int itemId, int count,
        NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
        JsonData data = new JsonData();
        data["cid"] = cid; data["item_id"] = itemId; data["count"] = -count;
        sendRequest(Oper.LostItem, data, onSuccess, onError, uid: true);
    }

    /// <summary>
    /// 转移物品
    /// </summary>
    /// <param name="cid">容器ID</param>
    /// <param name="targetCid">目标容器ID</param>
    /// <param name="contItemIds">容器项ID集</param>
    /// <param name="counts">每个容器项转移的数量</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void transferItem(int cid, int targetCid, int[] contItemIds, int[] counts,
        NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
        JsonData data = new JsonData();
        data["cid"] = cid; data["target_cid"] = targetCid;
        data["contitem_ids"] = DataLoader.convertArray(contItemIds);
        data["counts"] = DataLoader.convertArray(counts);
        sendRequest(Oper.TransferItem, data, onSuccess, onError, uid: true);
    }

    /// <summary>
    /// 拆分物品
    /// </summary>
    /// <param name="cid">容器ID</param>
    /// <param name="contItemId">容器项ID</param>
    /// <param name="count">拆分数量</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void splitItem(int cid, int contItemId, int count,
        NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
        JsonData data = new JsonData();
        data["cid"] = cid; data["count"] = count;
        data["contitem_id"] = contItemId;
        sendRequest(Oper.SplitItem, data, onSuccess, onError, uid: true);
    }

    /// <summary>
    /// 合并物品
    /// </summary>
    /// <param name="cid">容器ID</param>
    /// <param name="contItemIds">容器项ID集</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void mergeItem(int cid, int[] contItemIds,
        NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
        JsonData data = new JsonData();
        data["cid"] = cid; 
        data["contitem_ids"] = DataLoader.convertArray(contItemIds);
        sendRequest(Oper.MergeItem, data, onSuccess, onError, uid: true);
    }

    #endregion
}