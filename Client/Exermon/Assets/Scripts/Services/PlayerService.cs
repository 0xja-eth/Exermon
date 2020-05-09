using System;
using UnityEngine.Events;

using LitJson;

using Core.Data.Loaders;
using Core.Systems;
using Core.Services;

using PlayerModule.Data;

using ExermonModule.Services;
using ItemModule.Services;

/// <summary>
/// 玩家模块服务
/// </summary>
namespace PlayerModule.Services {

    /// <summary>
    /// 玩家服务
    /// </summary>
    public class PlayerService : BaseService<PlayerService> {

        /// <summary>
        /// 操作文本设定
        /// </summary>
        const string Register = "注册";
        const string Login = "登陆";
        const string Retrieve = "找回密码";
        const string Code = "发送验证码";
        const string Logout = "登出";

        const string GetBasic = "拉取玩家基本信息";
        const string GetStatus = "拉取玩家状态信息";
        const string GetBattle = "拉取玩家对战信息";
        const string GetPack = "拉取玩家背包信息";

        const string CreateCharacter = "创建角色";
        const string CreateExermons = "装备艾瑟萌";
        const string CreateGifts = "装备天赋";
        const string CreateInfo = "提交信息";

        const string EditName = "修改昵称";
        const string EditInfo = "修改信息";

        const string EquipSlotEquip = "装备";
        const string EquipSlotDequip = "卸下";

        /// <summary>
        /// 业务操作
        /// </summary>
        public enum Oper {
            Register, Login, Retrieve, Code, Logout,
            GetBasic, GetStatus, GetBattle, GetPack,
            CreateCharacter, CreateExermons, CreateGifts, CreateInfo,
            EditName, EditInfo,
            EquipSlotEquip, EquipSlotDequip,
        }

        /// <summary>
        /// 状态
        /// </summary>
        public enum State {
            Unlogin,
            Logined,
        }

        public bool isLogined() {
            return state == (int)State.Logined;
        }

        /// <summary>
        /// 玩家
        /// </summary>
        public Player player { get; protected set; } = null;

        /// <summary>
        /// 外部系统
        /// </summary>
        ItemService itemSer;
        ExermonService exerSer;

        #region 初始化

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            itemSer = ItemService.get();
            exerSer = ExermonService.get();
        }

        /// <summary>
        /// 初始化状态字典
        /// </summary>
        protected override void initializeStateDict() {
            base.initializeStateDict();
            addStateDict(State.Unlogin);
            addStateDict(State.Logined);
        }

        /// <summary>
        /// 初始化操作字典
        /// </summary>
        protected override void initializeOperDict() {
            base.initializeOperDict();
            addOperDict(Oper.Register, Register, NetworkSystem.Interfaces.PlayerRegister);
            addOperDict(Oper.Login, Login, NetworkSystem.Interfaces.PlayerLogin);
            addOperDict(Oper.Retrieve, Retrieve, NetworkSystem.Interfaces.PlayerRetrieve);
            addOperDict(Oper.Code, Code, NetworkSystem.Interfaces.PlayerCode);
            addOperDict(Oper.Logout, Logout, NetworkSystem.Interfaces.PlayerLogout);

            addOperDict(Oper.GetBasic, GetBasic, NetworkSystem.Interfaces.PlayerGetBasic);
            addOperDict(Oper.GetStatus, GetStatus, NetworkSystem.Interfaces.PlayerGetStatus);
            addOperDict(Oper.GetBattle, GetBattle, NetworkSystem.Interfaces.PlayerGetBattle);
            addOperDict(Oper.GetPack, GetPack, NetworkSystem.Interfaces.PlayerGetPack);

            addOperDict(Oper.CreateCharacter, CreateCharacter,
                NetworkSystem.Interfaces.PlayerCreateCharacter);
            addOperDict(Oper.CreateExermons, CreateExermons,
                NetworkSystem.Interfaces.PlayerCreateExermons);
            addOperDict(Oper.CreateGifts, CreateGifts,
                NetworkSystem.Interfaces.PlayerCreateGifts);
            addOperDict(Oper.CreateInfo, CreateInfo,
                NetworkSystem.Interfaces.PlayerCreateInfo);

            addOperDict(Oper.EditName, EditName, NetworkSystem.Interfaces.PlayerEditName);
            addOperDict(Oper.EditInfo, EditInfo, NetworkSystem.Interfaces.PlayerEditInfo);

            addOperDict(Oper.EquipSlotEquip, EquipSlotEquip,
                NetworkSystem.Interfaces.PlayerEquipSlotEquip);
            addOperDict(Oper.EquipSlotDequip, EquipSlotDequip,
                NetworkSystem.Interfaces.PlayerEquipSlotDequip);
        }

        /// <summary>
        /// 其他初始化工作
        /// </summary>
        protected override void initializeOthers() {
            base.initializeOthers();
            changeState(State.Unlogin);
        }

        #endregion

        #region 操作控制

        #region 登录注册

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="un">用户名</param>
        /// <param name="pw">密码</param>
        /// <param name="email">邮箱</param>
        /// <param name="code">验证码</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void register(string un, string pw, string email, string code,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {

            JsonData data = new JsonData();
            data["un"] = un; data["pw"] = pw;
            data["email"] = email; data["code"] = code;
            sendRequest(Oper.Register, data, onSuccess, onError);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="un">用户名</param>
        /// <param name="pw">密码</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void login(string un, string pw,
            UnityAction onSuccess, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                changeState(State.Logined);
                player = DataLoader.load(player, res, "player");
                onSuccess?.Invoke();
            };

            JsonData data = new JsonData();
            data["un"] = un; data["pw"] = pw;
            sendRequest(Oper.Login, data, _onSuccess, onError);
        }

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <param name="un">用户名</param>
        /// <param name="pw">密码</param>
        /// <param name="email">邮箱</param>
        /// <param name="code">验证码</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void retrieve(string un, string pw, string email, string code,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {

            JsonData data = new JsonData();
            data["un"] = un; data["pw"] = pw;
            data["email"] = email; data["code"] = code;

            sendRequest(Oper.Retrieve, data, onSuccess, onError);
        }

        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="un">用户名</param>
        /// <param name="email">邮箱</param>
        /// <param name="type">验证码类型</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void sendCode(string un, string email, string type,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {

            JsonData data = new JsonData();
            data["un"] = un; data["email"] = email;
            data["type"] = type;
            sendRequest(Oper.Code, data, onSuccess, onError);
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void logout(UnityAction onSuccess = null, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                player = null;
                changeState(State.Unlogin);
                onSuccess?.Invoke();
            };

            JsonData data = new JsonData();
            sendRequest(Oper.Logout, data, _onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 重连
        /// </summary>
        public void reconnect() {

        }

        #endregion

        #region 信息获取

        /// <summary>
        /// 获取玩家基本信息
        /// </summary>
        /// <param name="uid">要获取信息的玩家ID</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void getBasic(int uid,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
            JsonData data = new JsonData();
            data["get_uid"] = uid;
            sendRequest(Oper.GetBasic, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 获取玩家状态信息
        /// </summary>
        /// <param name="uid">要获取信息的玩家ID</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void getStatus(int uid,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
            JsonData data = new JsonData();
            data["get_uid"] = uid;
            sendRequest(Oper.GetStatus, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 获取玩家对战信息
        /// </summary>
        /// <param name="uid">要获取信息的玩家ID</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void getBattle(int uid,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
            JsonData data = new JsonData();
            data["get_uid"] = uid;
            sendRequest(Oper.GetBattle, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 获取玩家背包信息
        /// </summary>
        /// <param name="uid">要获取信息的玩家ID</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void getPack(int uid,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
            JsonData data = new JsonData();
            data["get_uid"] = uid;
            sendRequest(Oper.GetPack, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 获取玩家自身基础信息数据
        /// </summary>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void getPlayerBasic(UnityAction onSuccess = null, UnityAction onError = null) {
            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                player = DataLoader.load(player, res, "player");
                onSuccess?.Invoke();
            };

            getBasic(player.getID(), _onSuccess, onError);
        }

        /// <summary>
        /// 获取玩家自身状态信息数据
        /// </summary>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void getPlayerStatus(UnityAction onSuccess = null, UnityAction onError = null) {
            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                player = DataLoader.load(player, res, "player");
                onSuccess?.Invoke();
            };

            getStatus(player.getID(), _onSuccess, onError);
        }

        /// <summary>
        /// 获取玩家自身对战信息数据
        /// </summary>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void getPlayerBattle(UnityAction onSuccess = null, UnityAction onError = null) {
            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                player = DataLoader.load(player, res, "player");
                onSuccess?.Invoke();
            };

            getBattle(player.getID(), _onSuccess, onError);
        }

        /// <summary>
        /// 获取玩家自身背包信息数据
        /// </summary>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void getPlayerPack(UnityAction onSuccess = null, UnityAction onError = null) {
            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                player = DataLoader.load(player, res, "player");
                onSuccess?.Invoke();
            };

            getPack(player.getID(), _onSuccess, onError);
        }

        #endregion

        #region 创建/修改操作

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="name">昵称</param>
        /// <param name="grade">年级ID</param>
        /// <param name="cid">人物ID</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void createCharacter(string name, int grade, int cid,
            UnityAction onSuccess, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                player.createCharacter(name, grade, cid);
                onSuccess?.Invoke();
            };

            JsonData data = new JsonData();
            data["name"] = name; data["grade"] = grade; data["cid"] = cid;
            sendRequest(Oper.CreateCharacter, data, _onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 选择艾瑟萌
        /// </summary>
        /// <param name="eids">艾瑟萌ID</param>
        /// <param name="enames">艾瑟萌昵称</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void createExermons(int[] eids, string[] enames,
            UnityAction onSuccess, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                player.createExermons(res);
                onSuccess?.Invoke();
            };

            JsonData data = new JsonData();
            data["eids"] = DataLoader.convert(eids);
            data["enames"] = DataLoader.convert(enames);
            sendRequest(Oper.CreateExermons, data, _onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 选择天赋
        /// </summary>
        /// <param name="gids">艾瑟萌天赋ID</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void createGifts(int[] gids,
            UnityAction onSuccess, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                player.createGifts(gids);
                onSuccess?.Invoke();
            };

            JsonData data = new JsonData();
            data["gids"] = DataLoader.convert(gids);
            sendRequest(Oper.CreateGifts, data, _onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 补全信息
        /// </summary>
        /// <param name="birth">出生日期</param>
        /// <param name="school">学校名称</param>
        /// <param name="city">居住地</param>
        /// <param name="contact">联系方式</param>
        /// <param name="description">个人介绍</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void createInfo(DateTime birth, string school,
            string city, string contact, string description,
            UnityAction onSuccess, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                player.createInfo(birth, school, city, contact, description);
                onSuccess?.Invoke();
            };

            JsonData data = new JsonData();
            data["birth"] = DataLoader.convert(birth, "date");
            data["school"] = school; data["city"] = city;
            data["contact"] = contact; data["description"] = description;
            sendRequest(Oper.CreateInfo, data, _onSuccess, onError, uid: true);
        }
        public void createInfo(UnityAction onSuccess, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                player.createInfo(); onSuccess?.Invoke();
            };

            JsonData data = new JsonData();
            sendRequest(Oper.CreateInfo, data, _onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 修改昵称
        /// </summary>
        /// <param name="name">新昵称</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void editName(string name, UnityAction onSuccess, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                player.editNmae(name);
                onSuccess?.Invoke();
            };

            JsonData data = new JsonData(); data["name"] = name;
            sendRequest(Oper.EditName, data, _onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 修改信息
        /// </summary>
        /// <param name="grade">年级ID</param>
        /// <param name="birth">出生日期</param>
        /// <param name="school">学校名称</param>
        /// <param name="city">居住地</param>
        /// <param name="contact">联系方式</param>
        /// <param name="description">个人介绍</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void editInfo(int grade, DateTime birth, string school,
            string city, string contact, string description,
            UnityAction onSuccess, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                player.editInfo(grade, birth, school, city, contact, description);
                onSuccess?.Invoke();
            };

            JsonData data = new JsonData();
            data["grade"] = grade;
            data["birth"] = DataLoader.convert(birth, "date");
            data["school"] = school; data["city"] = city;
            data["contact"] = contact; data["description"] = description;
            sendRequest(Oper.EditInfo, data, _onSuccess, onError, uid: true);
        }

        #endregion

        #region 装备操作

        /// <summary>
        /// 装备人物装备
        /// </summary>
        /// <param name="packEquip">人类背包装备</param>
        /*
        public void equipSlotEquip(HumanPackEquip packEquip) {
            var humanPack = player.packContainers.humanPack;
            var equipSlot = player.slotContainers.humanEquipSlot;
            equipSlot.setEquip(humanPack, packEquip);
        }
        */
        /// <param name="packEquip">人类背包装备</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void equipSlotEquip(HumanPackEquip packEquip,
            UnityAction onSuccess, UnityAction onError = null, bool localChange = true) {

            var humanPack = player.packContainers.humanPack;
            var equipSlot = player.slotContainers.humanEquipSlot;
            var _onSuccess = itemSer.slotOperationSuccess(humanPack, equipSlot, onSuccess);

            equipSlotEquip(packEquip.getID(), _onSuccess, onError);
        }
        /// <param name="heid">人类背包装备项ID</param>
        public void equipSlotEquip(int heid,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
            JsonData data = new JsonData(); data["heid"] = heid;
            sendRequest(Oper.EquipSlotEquip, data, onSuccess, onError, uid: true);
        }
        
        /// <summary>
        /// 装备人物装备
        /// </summary>
        /// <param name="type">装备类型</param>
        /*
        public void equipSlotDequip(int type) {
            var humanPack = player.packContainers.humanPack;
            var equipSlot = player.slotContainers.humanEquipSlot;
            equipSlot.setEquip(type, humanPack, null);
        }
        */
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void equipSlotDequip(int type,
            UnityAction onSuccess, UnityAction onError = null, bool localChange = true) {

            var humanPack = player.packContainers.humanPack;
            var equipSlot = player.slotContainers.humanEquipSlot;
            var _onSuccess = itemSer.slotOperationSuccess(humanPack, equipSlot, onSuccess);

            equipSlotDequip(type, _onSuccess, onError);
        }
        public void equipSlotDequip(int type,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
            JsonData data = new JsonData(); data["type"] = type;
            sendRequest(Oper.EquipSlotDequip, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 读取人物背包
        /// </summary>
        /// <param name="cid">容器ID</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void loadHumanPack(UnityAction onSuccess = null, UnityAction onError = null) {
            itemSer.getPack(player.packContainers.humanPack, onSuccess, onError);
        }

        /// <summary>
        /// 读取人物装备槽
        /// </summary>
        /// <param name="cid">容器ID</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void loadHumanEquipSlot(UnityAction onSuccess = null, UnityAction onError = null) {
            itemSer.getSlot(player.slotContainers.humanEquipSlot, onSuccess, onError);
        }

        #endregion

        /// <summary>
        /// 更新读取玩家信息
        /// </summary>
        /// <param name="json"></param>
        public void loadPlayer(JsonData json) {
            player = DataLoader.load(player, json);
        }

        #endregion
    }
}