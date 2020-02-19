using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using LitJson;

/// <summary>
/// 玩家服务
/// </summary>
public class PlayerService : BaseService<PlayerService> {

    /// <summary>
    /// 操作文本设定
    /// </summary>
    const string Register = "注册";
    const string Login = "登陆";
    const string Forget = "重置密码";
    const string Code = "发送验证码";
    const string Logout = "登出";

    const string GetBasic = "拉取玩家基本信息";

    const string CreateCharacter = "创建角色";
    const string CreateExermons = "装备艾瑟萌";
    const string CreateGifts = "装备天赋";
    const string CreateInfo = "提交信息";

    const string EquipSlotEquip = "装备";

    /// <summary>
    /// 业务操作
    /// </summary>
    public enum Oper {
        Register, Login, Forget, Code, Logout,
        GetBasic,
        CreateCharacter, CreateExermons, CreateGifts, CreateInfo,
        EquipSlotEquip,
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
    public Player player { get; private set; } = null;

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
        addOperDict(Oper.Forget, Forget, NetworkSystem.Interfaces.PlayerForget);
        addOperDict(Oper.Code, Code, NetworkSystem.Interfaces.PlayerCode);
        addOperDict(Oper.Logout, Logout, NetworkSystem.Interfaces.PlayerLogout);

        addOperDict(Oper.GetBasic, GetBasic, NetworkSystem.Interfaces.PlayerGetBasic);

        addOperDict(Oper.CreateCharacter, CreateCharacter, 
            NetworkSystem.Interfaces.PlayerCreateCharacter);
        addOperDict(Oper.CreateExermons, CreateExermons, 
            NetworkSystem.Interfaces.PlayerCreateExermons);
        addOperDict(Oper.CreateGifts, CreateGifts,
            NetworkSystem.Interfaces.PlayerCreateGifts);
        addOperDict(Oper.CreateInfo, CreateInfo,
            NetworkSystem.Interfaces.PlayerCreateInfo);

        addOperDict(Oper.EquipSlotEquip, EquipSlotEquip,
            NetworkSystem.Interfaces.PlayerEquipSlotEquip);
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

        NetworkSystem.RequestObject.SuccessAction _onSuccess = generateOnLoginSuccessFunc(onSuccess);

        JsonData data = new JsonData();
        data["un"] = un; data["pw"] = pw;
        sendRequest(Oper.Login, data, _onSuccess, onError);
    }

    /// <summary>
    /// 生成登陆成功回调函数
    /// </summary>
    /// <param name="onSuccess">成功回调</param>
    /// <returns></returns>
    NetworkSystem.RequestObject.SuccessAction generateOnLoginSuccessFunc(UnityAction onSuccess) {
        return (res) => {
            changeState(State.Logined);
            player = DataLoader.loadData<Player>(res, "player");
            onSuccess?.Invoke();
        };
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
    public void forget(string un, string pw, string email, string code,
        NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {

        JsonData data = new JsonData();
        data["un"] = un; data["pw"] = pw;
        data["email"] = email; data["code"] = code;

        sendRequest(Oper.Forget, data, onSuccess, onError);
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

        NetworkSystem.RequestObject.SuccessAction _onSuccess = generateOnLogoutSuccessFunc(onSuccess);

        JsonData data = new JsonData();
        sendRequest(Oper.Logout, data, _onSuccess, onError, uid: true);
    }

    /// <summary>
    /// 生成登出成功回调函数
    /// </summary>
    /// <param name="onSuccess">成功回调</param>
    /// <returns></returns>
    NetworkSystem.RequestObject.SuccessAction generateOnLogoutSuccessFunc(UnityAction onSuccess) {
        return (res) => {
            player = null;
            changeState(State.Unlogin);
            onSuccess?.Invoke();
        };
    }

    /// <summary>
    /// 重连
    /// </summary>
    public void reconnect() {

    }

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
    /// 创建角色
    /// </summary>
    /// <param name="name">昵称</param>
    /// <param name="grade">年级ID</param>
    /// <param name="cid">人物ID</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void createCharacter(string name, int grade, int cid,
        UnityAction onSuccess, UnityAction onError = null) {

        NetworkSystem.RequestObject.SuccessAction _onSuccess = 
            generateCreateCharacterSuccessFunc(name, grade, cid, onSuccess);

        JsonData data = new JsonData();
        data["name"] = name; data["grade"] = grade; data["cid"] = cid;
        sendRequest(Oper.CreateCharacter, data, _onSuccess, onError, uid: true);
    }

    /// <summary>
    /// 生成创建角色成功回调函数
    /// </summary>
    /// <param name="onSuccess">成功回调</param>
    NetworkSystem.RequestObject.SuccessAction 
        generateCreateCharacterSuccessFunc(string name, 
        int grade, int cid, UnityAction onSuccess) {
        return (res) => {
            player.createCharacter(name, grade, cid);
            onSuccess?.Invoke();
        };
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

        NetworkSystem.RequestObject.SuccessAction _onSuccess =
            generateCreateExermonsSuccessFunc(onSuccess);

        JsonData data = new JsonData();
        data["eids"] = DataLoader.convertArray(eids);
        data["enames"] = DataLoader.convertArray(enames);
        sendRequest(Oper.CreateExermons, data, _onSuccess, onError, uid: true);
    }

    /// <summary>
    /// 生成选择艾瑟萌成功回调函数
    /// </summary>
    /// <param name="onSuccess">成功回调</param>
    NetworkSystem.RequestObject.SuccessAction
        generateCreateExermonsSuccessFunc(UnityAction onSuccess) {
        return (res) => {
            player.createExermons(res);
            onSuccess?.Invoke();
        };
    }

    /// <summary>
    /// 选择天赋
    /// </summary>
    /// <param name="gids">艾瑟萌天赋ID</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void createGifts(int[] gids,
        UnityAction onSuccess, UnityAction onError = null) {

        NetworkSystem.RequestObject.SuccessAction _onSuccess =
            generateCreateGiftsSuccessFunc(gids, onSuccess);

        JsonData data = new JsonData();
        data["gids"] = DataLoader.convertArray(gids);
        sendRequest(Oper.CreateGifts, data, _onSuccess, onError, uid: true);
    }

    /// <summary>
    /// 生成选择天赋成功回调函数
    /// </summary>
    /// <param name="onSuccess">成功回调</param>
    NetworkSystem.RequestObject.SuccessAction
        generateCreateGiftsSuccessFunc(int[] gids, UnityAction onSuccess) {
        return (res) => {
            player.createGifts(gids);
            onSuccess?.Invoke();
        };
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

        NetworkSystem.RequestObject.SuccessAction _onSuccess =
            generateCreateInfoSuccessFunc(birth, school, city, 
            contact, description, onSuccess);

        JsonData data = new JsonData();
        data["birth"] = DataLoader.convertDate(birth);
        data["school"] = school; data["city"] = city;
        data["contact"] = contact; data["description"] = description;
        sendRequest(Oper.CreateInfo, data, _onSuccess, onError, uid: true);
    }

    /// <summary>
    /// 补全信息成功回调函数
    /// </summary>
    /// <param name="birth">出生日期</param>
    /// <param name="school">学校名称</param>
    /// <param name="city">居住地</param>
    /// <param name="contact">联系方式</param>
    /// <param name="description">个人介绍</param>
    /// <param name="onSuccess">成功回调</param>
    NetworkSystem.RequestObject.SuccessAction
        generateCreateInfoSuccessFunc(DateTime birth, 
        string school, string city, string contact, 
        string description, UnityAction onSuccess) {
        return (res) => {
            player.createInfo(birth, school, city, contact, description);
            onSuccess?.Invoke();
        };
    }

    /// <summary>
    /// 装备人物装备
    /// </summary>
    /// <param name="eid">装备位置ID</param>
    /// <param name="heid">人类背包装备项ID</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void equipSlotEquip(int eid, int heid, 
        NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
        JsonData data = new JsonData();
        data["eid"] = eid; data["heid"] = heid;
        sendRequest(Oper.EquipSlotEquip, data, onSuccess, onError, uid: true);
    }

    /// <summary>
    /// 获取玩家自身基础信息数据
    /// </summary>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void getPlayerBasic(UnityAction onSuccess = null, UnityAction onError = null) {
        NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
            player.loadBasic(DataLoader.loadJsonData(res, "player"));
            onSuccess?.Invoke();
        };

        getBasic(player.getID(), _onSuccess, onError);
    }

    /// <summary>
    /// 读取人物背包
    /// </summary>
    /// <param name="cid">容器ID</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void loadHumanPack(int cid = 0, UnityAction onSuccess = null, UnityAction onError = null) {
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
}