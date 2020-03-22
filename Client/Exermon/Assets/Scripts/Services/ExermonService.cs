
using UnityEngine.Events;

using LitJson;

using Core.Systems;
using Core.Services;

using PlayerModule.Data;
using ExermonModule.Data;

using ItemModule.Services;

/// <summary>
/// 艾瑟萌模块服务
/// </summary>
namespace ExermonModule.Services {

    /// <summary>
    /// 艾瑟萌服务
    /// </summary>
    public class ExermonService : BaseService<ExermonService> {

        /// <summary>
        /// 操作文本设定
        /// </summary>
        const string EditNickname = "修改昵称";

        const string EquipPlayerExer = "装备";
        const string EquipPlayerGift = "装备";
        const string EquipExerEquip = "装备";
        const string DequipExerEquip = "卸下装备";

        /// <summary>
        /// 业务操作
        /// </summary>
        public enum Oper {
            EditNickname,
            EquipPlayerExer, EquipPlayerGift,
            EquipExerEquip, DequipExerEquip
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
            addOperDict(Oper.EditNickname, EditNickname, NetworkSystem.Interfaces.ExermonEditNickname);
            addOperDict(Oper.EquipPlayerExer, EquipPlayerExer, NetworkSystem.Interfaces.ExermonEquipPlayerExer);
            addOperDict(Oper.EquipPlayerGift, EquipPlayerGift, NetworkSystem.Interfaces.ExermonEquipPlayerGift);
            addOperDict(Oper.EquipExerEquip, EquipExerEquip, NetworkSystem.Interfaces.ExermonEquipExerEquip);
            addOperDict(Oper.DequipExerEquip, DequipExerEquip, NetworkSystem.Interfaces.ExermonDequipExerEquip);
        }

        #endregion

        #region 操作控制

        /// <summary>
        /// 艾瑟萌槽装备
        /// </summary>
        /// <param name="playerExer">艾瑟萌仓库项</param>
        /// <param name="name">新名字</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void editNickname(PlayerExermon playerExer, string name,
            UnityAction onSuccess = null, UnityAction onError = null) {

            if (playerExer.nickname == name) { onSuccess?.Invoke(); return; }

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                playerExer.rename(name);
                onSuccess?.Invoke();
            };

            editNickname(playerExer.getID(), name, _onSuccess, onError);
        }
        /// <param name="peid">艾瑟萌仓库项ID</param>
        public void editNickname(int peid, string name,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
            JsonData data = new JsonData();
            data["peid"] = peid; data["name"] = name;
            sendRequest(Oper.EditNickname, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 艾瑟萌槽装备艾瑟萌
        /// </summary>
        /// <param name="slotItem">艾瑟萌槽项</param>
        /// <param name="playerExer">艾瑟萌仓库项</param>
        public void equipPlayerExer(ExerSlotItem slotItem, PlayerExermon playerExer) {
            var player = getPlayer();
            var exerSlot = player.slotContainers.exerSlot;
            var exerHub = player.packContainers.exerHub;
            exerSlot.setEquip(slotItem, exerHub, playerExer);
        }
        /// <param name="sid">科目ID</param>
        public void equipPlayerExer(int sid, PlayerExermon playerExer) {
            var player = getPlayer();
            var exerSlot = player.slotContainers.exerSlot;
            var exerHub = player.packContainers.exerHub;
            exerSlot.setEquip(exerHub, playerExer);
        }
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        /// <param name="localChange">本地数据是否改变</param>
        public void equipPlayerExer(ExerSlotItem slotItem, PlayerExermon playerExer,
            UnityAction onSuccess = null, UnityAction onError = null, bool localChange = true) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                if (localChange) equipPlayerExer(slotItem, playerExer);
                onSuccess?.Invoke();
            };

            var peid = playerExer == null ? 0 : playerExer.getID();

            equipPlayerExer(slotItem.subjectId, peid, _onSuccess, onError);
        }
        public void equipPlayerExer(int sid, PlayerExermon playerExer,
            UnityAction onSuccess = null, UnityAction onError = null, bool localChange = true) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                if (localChange) equipPlayerExer(sid, playerExer);
                onSuccess?.Invoke();
            };

            var peid = playerExer == null ? 0 : playerExer.getID();

            equipPlayerExer(sid, peid, _onSuccess, onError);
        }
        /// <param name="peid">艾瑟萌仓库项ID</param>
        /// <param name="pgid">艾瑟萌天赋池项ID</param>
        public void equipPlayerExer(int sid, int peid,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
            JsonData data = new JsonData();
            data["sid"] = sid; data["peid"] = peid;
            sendRequest(Oper.EquipPlayerExer, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 艾瑟萌槽装备
        /// </summary>
        /// <param name="slotItem">艾瑟萌槽项</param>
        /// <param name="playerGift">艾瑟萌天赋池项</param>
        public void equipPlayerGift(ExerSlotItem slotItem, PlayerExerGift playerGift) {
            var player = getPlayer();
            var exerSlot = player.slotContainers.exerSlot;
            var exerGiftPool = player.packContainers.exerGiftPool;
            exerSlot.setEquip(slotItem, exerGiftPool, playerGift);
        }
        /// <param name="sid">科目ID</param>
        public void equipPlayerGift(int sid, PlayerExerGift playerGift) {
            var player = getPlayer();
            var exerSlot = player.slotContainers.exerSlot;
            var exerGiftPool = player.packContainers.exerGiftPool;
            exerSlot.setEquip(sid, exerGiftPool, playerGift);
        }
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        /// <param name="localChange">本地数据是否改变</param>
        public void equipPlayerGift(ExerSlotItem slotItem, PlayerExerGift playerGift,
            UnityAction onSuccess = null, UnityAction onError = null, bool localChange = true) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                if (localChange) equipPlayerGift(slotItem, playerGift);
                onSuccess?.Invoke();
            };

            var pgid = playerGift == null ? 0 : playerGift.getID();

            equipPlayerGift(slotItem.subjectId, pgid, _onSuccess, onError);
        }
        public void equipPlayerGift(int sid, PlayerExerGift playerGift,
            UnityAction onSuccess = null, UnityAction onError = null, bool localChange = true) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                if (localChange) equipPlayerGift(sid, playerGift);
                onSuccess?.Invoke();
            };

            var pgid = playerGift == null ? 0 : playerGift.getID();

            equipPlayerGift(sid, pgid, _onSuccess, onError);
        }
        /// <param name="pgid">艾瑟萌天赋池项ID</param>
        public void equipPlayerGift(int sid, int pgid,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
            JsonData data = new JsonData();
            data["sid"] = sid; data["pgid"] = pgid;
            sendRequest(Oper.EquipPlayerGift, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 艾瑟萌装备槽装备
        /// </summary>
        /// <param name="equipSlot">艾瑟萌装备槽</param>
        /// <param name="packEquip">背包装备</param>
        public void equipExerEquip(ExerEquipSlot equipSlot, ExerPackEquip packEquip) {
            var player = getPlayer();
            var exerPack = player.packContainers.exerPack;
            equipSlot.setEquip(exerPack, packEquip);
        }
        /// <param name="sid">科目ID</param>
        public void equipExerEquip(int sid, ExerPackEquip packEquip) {
            var player = getPlayer();
            var exerSlot = player.slotContainers.exerSlot;
            var equipSlot = exerSlot.getSlotItem(sid).exerEquipSlot;
            equipExerEquip(equipSlot, packEquip);
        }
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        /// <param name="localChange">本地数据是否改变</param>
        public void equipExerEquip(ExerEquipSlot equipSlot, ExerPackEquip packEquip,
            UnityAction onSuccess = null, UnityAction onError = null, bool localChange = true) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                if (localChange) equipExerEquip(equipSlot, packEquip);
                onSuccess?.Invoke();
            };

            equipExerEquip(equipSlot.exerSlotItem.subjectId, packEquip.getID(), _onSuccess, onError);
        }
        /// <param name="slotItem">艾瑟萌槽项</param>
        public void equipExerEquip(ExerSlotItem slotItem, ExerPackEquip packEquip,
            UnityAction onSuccess, UnityAction onError = null, bool localChange = true) {
            equipExerEquip(slotItem.exerEquipSlot, packEquip, onSuccess, onError, localChange);
        }
        public void equipExerEquip(int sid, ExerPackEquip packEquip,
            UnityAction onSuccess = null, UnityAction onError = null, bool localChange = true) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                if (localChange) equipExerEquip(sid, packEquip);
                onSuccess?.Invoke();
            };

            equipExerEquip(sid, packEquip.getID(), _onSuccess, onError);
        }
        /// <param name="eid">装备位置ID</param>
        /// <param name="eeid">艾瑟萌背包装备项ID</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void equipExerEquip(int sid, int eeid,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
            JsonData data = new JsonData();
            data["eeid"] = eeid; data["sid"] = sid;
            sendRequest(Oper.EquipExerEquip, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 艾瑟萌装备槽装备
        /// </summary>
        /// <param name="equipSlot">艾瑟萌装备槽</param>
        /// <param name="type">装备类型</param>
        public void dequipExerEquip(ExerEquipSlot equipSlot, int type) {
            var player = getPlayer();
            var exerPack = player.packContainers.exerPack;
            equipSlot.setEquip(type, exerPack, null);
        }
        /// <param name="sid">科目ID</param>
        public void dequipExerEquip(int sid, int type) {
            var player = getPlayer();
            var exerSlot = player.slotContainers.exerSlot;
            var equipSlot = exerSlot.getSlotItem(sid).exerEquipSlot;
            dequipExerEquip(equipSlot, type);
        }
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        /// <param name="localChange">本地数据是否改变</param>
        public void dequipExerEquip(ExerEquipSlot equipSlot, int type,
            UnityAction onSuccess = null, UnityAction onError = null, bool localChange = true) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                if (localChange) dequipExerEquip(equipSlot, type);
                onSuccess?.Invoke();
            };

            dequipExerEquip(equipSlot.exerSlotItem.subjectId, type, _onSuccess, onError);
        }
        /// <param name="slotItem">艾瑟萌槽项</param>
        public void dequipExerEquip(ExerSlotItem slotItem, int type,
            UnityAction onSuccess, UnityAction onError = null, bool localChange = true) {
            dequipExerEquip(slotItem.exerEquipSlot, type, onSuccess, onError, localChange);
        }
        public void dequipExerEquip(int sid, int type,
            UnityAction onSuccess = null, UnityAction onError = null, bool localChange = true) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                if (localChange) dequipExerEquip(sid, type);
                onSuccess?.Invoke();
            };

            dequipExerEquip(sid, type, _onSuccess, onError);
        }
        /// <param name="eid">装备位置ID</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void dequipExerEquip(int sid, int type,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
            JsonData data = new JsonData();
            data["type"] = type; data["sid"] = sid;
            sendRequest(Oper.DequipExerEquip, data, onSuccess, onError, uid: true);
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
}