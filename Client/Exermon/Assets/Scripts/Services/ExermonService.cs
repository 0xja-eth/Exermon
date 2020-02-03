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
    const string ExerSlotEquip = "装备";
    const string EquipSlotEquip = "装备";

    /// <summary>
    /// 业务操作
    /// </summary>
    public enum Oper {
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
    public void exerSlotEquip(int sid, int peid, int pgid,
        NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
        JsonData data = new JsonData();
        data["sid"] = sid; data["peid"] = peid; data["pgid"] = pgid;
        data["cid"] = getPlayer().slotContainers.exerSlotId;
        sendRequest(Oper.ExerSlotEquip, data, onSuccess, onError, uid: true);
    }

    /// <summary>
    /// 艾瑟萌槽装备
    /// </summary>
    /// <param name="cid">艾瑟萌装备槽ID</param>
    /// <param name="eid">装备位置ID</param>
    /// <param name="heid">人类背包装备项ID</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void equipSlotEquip(int cid, int eid, int heid, 
        NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
        JsonData data = new JsonData();
        data["eid"] = eid; data["heid"] = heid; data["cid"] = cid;
        sendRequest(Oper.EquipSlotEquip, data, onSuccess, onError, uid: true);
    }

    /// <summary>
    /// 读取艾瑟萌槽
    /// </summary>
    /// <param name="cid">容器ID</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void loadExerSlot(int cid = 0, UnityAction onSuccess = null, UnityAction onError = null) {
        Player player = getPlayer();

        NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
            player.slotContainers.loadExerSlot(res);
            if (onSuccess != null) onSuccess.Invoke();
        };

        if (cid == 0) cid = player.slotContainers.exerSlotId;
        itemSer.getSlot(cid, _onSuccess, onError);
    }

    /// <summary>
    /// 读取人物装备槽
    /// </summary>
    /// <param name="cid">容器ID</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void loadHumanEquipSlot(int cid = 0, UnityAction onSuccess = null, UnityAction onError = null) {
        Player player = getPlayer();

        NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
            player.slotContainers.loadHumanEquipSlot(res);
            if (onSuccess != null) onSuccess.Invoke();
        };

        if (cid == 0) cid = player.slotContainers.humanEquipSlotId;
        itemSer.getSlot(cid, _onSuccess, onError);
    }

    #endregion
}