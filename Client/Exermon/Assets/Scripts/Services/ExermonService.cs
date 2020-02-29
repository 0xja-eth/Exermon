using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using LitJson;

/// <summary>
/// 物品服务
/// </summary>
public class ExermonService : BaseService<ExermonService> {

    /// <summary>
    /// 操作文本设定
    /// </summary>
    const string Rename = "修改昵称";

    const string ExerSlotEquip = "装备";
    const string EquipSlotEquip = "装备";

    /// <summary>
    /// 业务操作
    /// </summary>
    public enum Oper {
        Rename,
        ExerSlotEquip, EquipSlotEquip
    }

    /// <summary>
    /// 外部系统
    /// </summary>
    ItemService itemSer;

    #region 初始化

    /// <summary>
    /// 初始化外部系统
    /// </summary>
    protected override void initializeSystems() {
        base.initializeSystems();
        itemSer = ItemService.get();
    }

    /// <summary>
    /// 初始化操作字典
    /// </summary>
    protected override void initializeOperDict() {
        base.initializeOperDict();
        addOperDict(Oper.Rename, Rename, NetworkSystem.Interfaces.ExermonPlayerExerRename);
        addOperDict(Oper.ExerSlotEquip, ExerSlotEquip, NetworkSystem.Interfaces.ExermonExerSlotEquip);
        addOperDict(Oper.EquipSlotEquip, EquipSlotEquip, NetworkSystem.Interfaces.ExermonEquipSlotEquip);
    }

    #endregion

    #region 操作控制

    /// <summary>
    /// 艾瑟萌槽装备
    /// </summary>
    /// <param name="sid">科目ID</param>
    /// <param name="peid">艾瑟萌仓库项ID</param>
    /// <param name="pgid">艾瑟萌天赋池项ID</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void rename(PlayerExermon playerExer, string name,
        UnityAction onSuccess = null, UnityAction onError = null) {

        if(playerExer.nickname == name) {
            onSuccess?.Invoke(); return;
        }

        NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
            playerExer.rename(name);
            onSuccess?.Invoke();
        };

        rename(playerExer.getID(), name, _onSuccess, onError);
    }

    /// <summary>
    /// 艾瑟萌槽装备
    /// </summary>
    /// <param name="sid">科目ID</param>
    /// <param name="peid">艾瑟萌仓库项ID</param>
    /// <param name="name">新名字</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void rename(int peid, string name, 
        NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
        JsonData data = new JsonData();
        data["peid"] = peid; data["name"] = name;
        sendRequest(Oper.Rename, data, onSuccess, onError, uid: true);
    }

    /// <summary>
    /// 艾瑟萌槽装备
    /// </summary>
    /// <param name="slotItem">艾瑟萌槽项</param>
    /// <param name="playerExer">艾瑟萌仓库项</param>
    /// <param name="playerGift">艾瑟萌天赋池项</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void exerSlotEquip(ExerSlotItem slotItem, PlayerExermon playerExer, PlayerExerGift playerGift,
        UnityAction onSuccess = null, UnityAction onError = null) {

        NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
            slotItem.setPlayerExer(playerExer);
            slotItem.setPlayerGift(playerGift);
            onSuccess?.Invoke();
        };

        exerSlotEquip(slotItem.subjectId, playerExer.getID(), playerGift.getID(), _onSuccess, onError);
    }
    /// <param name="sid">科目ID</param>
    public void exerSlotEquip(int sid, PlayerExermon playerExer, PlayerExerGift playerGift,
        UnityAction onSuccess = null, UnityAction onError = null) {

        NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
            var player = getPlayer();
            var exerSlot = player.slotContainers.exerSlot;
            exerSlot.setPlayerExer(playerExer);
            exerSlot.setPlayerGift(sid, playerGift);
            onSuccess?.Invoke();
        };

        exerSlotEquip(sid, playerExer.getID(), playerGift.getID(), _onSuccess, onError);
    }
    /// <param name="peid">艾瑟萌仓库项ID</param>
    /// <param name="pgid">艾瑟萌天赋池项ID</param>
    public void exerSlotEquip(int sid, int peid, int pgid,
        NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
        JsonData data = new JsonData();
        data["sid"] = sid; data["peid"] = peid; data["pgid"] = pgid;
        sendRequest(Oper.ExerSlotEquip, data, onSuccess, onError, uid: true);
    }

    /// <summary>
    /// 艾瑟萌装备槽装备
    /// </summary>
    /// <param name="equipSlot">艾瑟萌装备槽</param>
    /// <param name="packEquip">背包装备</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void equipSlotEquip(ExerEquipSlot equipSlot, ExerPackEquip packEquip,
        UnityAction onSuccess = null, UnityAction onError = null) {

        NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
            equipSlot.setEquip(packEquip);
            onSuccess?.Invoke();
        };

        equipSlotEquip(equipSlot.exerSlotItem.subjectId,
            packEquip.getID(), _onSuccess, onError);
    }
    /// <param name="slotItem">艾瑟萌槽项</param>
    public void equipSlotEquip(ExerSlotItem slotItem, ExerPackEquip packEquip,
        UnityAction onSuccess, UnityAction onError = null) {
        equipSlotEquip(slotItem.exerEquipSlot, packEquip, onSuccess, onError);
    }
    /// <param name="sid">科目ID</param>
    public void equipSlotEquip(int sid, ExerPackEquip packEquip, 
        UnityAction onSuccess = null, UnityAction onError = null) {

        NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
            var player = getPlayer();
            var exerSlot = player.slotContainers.exerSlot;
            var equipSlot = exerSlot.getExerSlotItem(sid).exerEquipSlot;
            equipSlot.setEquip(packEquip);
            onSuccess?.Invoke();
        };

        equipSlotEquip(sid, packEquip.getID(), _onSuccess, onError);
    }
    /// <param name="eid">装备位置ID</param>
    /// <param name="eeid">艾瑟萌背包装备项ID</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void equipSlotEquip(int sid, int eeid, 
        NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
        JsonData data = new JsonData();
        data["eeid"] = eeid; data["sid"] = sid;
        sendRequest(Oper.EquipSlotEquip, data, onSuccess, onError, uid: true);
    }

    /// <summary>
    /// 读取艾瑟萌背包
    /// </summary>
    /// <param name="cid">容器ID</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void loadExerPack(UnityAction onSuccess = null, UnityAction onError = null) {
        Player player = getPlayer();
        itemSer.getPack(player.packContainers.exerPack, onSuccess, onError);
    }

    /// <summary>
    /// 读取艾瑟萌碎片背包
    /// </summary>
    /// <param name="cid">容器ID</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void loadExerFragPack(UnityAction onSuccess = null, UnityAction onError = null) {
        Player player = getPlayer();
        itemSer.getPack(player.packContainers.exerFragPack, onSuccess, onError);
    }

    /// <summary>
    /// 读取艾瑟萌天赋池
    /// </summary>
    /// <param name="cid">容器ID</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void loadExerGiftPool(UnityAction onSuccess = null, UnityAction onError = null) {
        Player player = getPlayer();
        itemSer.getPack(player.packContainers.exerGiftPool, onSuccess, onError);
    }

    /// <summary>
    /// 读取艾瑟萌仓库
    /// </summary>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void loadExerHub(UnityAction onSuccess = null, UnityAction onError = null) {
        Player player = getPlayer();
        itemSer.getPack(player.packContainers.exerHub, onSuccess, onError);
    }

    /// <summary>
    /// 读取艾瑟萌槽
    /// </summary>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void loadExerSlot(UnityAction onSuccess = null, UnityAction onError = null) {
        Player player = getPlayer();
        itemSer.getSlot(player.slotContainers.exerSlot, onSuccess, onError);
    }
    /*
    /// <summary>
    /// 获取艾瑟萌
    /// </summary>
    /// <param name="cid">容器ID</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void loadExerHub(int cid = 0, UnityAction onSuccess = null, UnityAction onError = null) {
        Player player = getPlayer();
        itemSer.getPack(player.packContainers.exerHub, onSuccess, onError);
    }
    */
    #endregion
}