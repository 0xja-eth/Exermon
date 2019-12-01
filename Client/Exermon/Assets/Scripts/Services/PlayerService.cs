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

    /// <summary>
    /// 业务操作
    /// </summary>
    public enum Oper {
        Register, Login, Forget, Code
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

    #region 初始化

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
        addOperDict(Oper.Register, Register, NetworkSystem.Interfaces.PlayerRegisterRoute);
        addOperDict(Oper.Login, Login, NetworkSystem.Interfaces.PlayerLoginRoute);
        addOperDict(Oper.Forget, Forget, NetworkSystem.Interfaces.PlayerForgetRoute);
        addOperDict(Oper.Code, Code, NetworkSystem.Interfaces.PlayerCodeRoute);
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
            player = DataLoader.loadData<Player>(res);
            if (onSuccess != null) onSuccess.Invoke();
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
    /// 重连
    /// </summary>
    public void reconnect() {

    }
    
    #endregion
}