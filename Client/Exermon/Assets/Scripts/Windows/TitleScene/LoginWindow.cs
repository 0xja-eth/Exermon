using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using LitJson;

/// <summary>
/// 登录窗口
/// </summary>
public class LoginWindow : BaseWindow {

    /// <summary>
    /// 界面类型
    /// </summary>
    public enum Type {
        Welcome, // 欢迎界面
        Login, // 登陆界面
        Register, // 注册界面
        Retrieve // 找回密码界面
    }
   


    /// <summary>
    /// 提示框内容
    /// </summary>
    const string IncorrectUsernameOrPassword = "用户名或密码错误";//
  

    const string InvalidInputAlertText = "请检查输入格式正确后再提交！";

    const string RegisterSuccessText = "注册用户成功！你的幸运号码是 {0} ！";
    const string RetrieveSuccessText = "密码已重置！请返回登录！";
    const string SendCodeSuccessText = "验证码发送成功！请留意你的邮箱！";

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public GameObject loginUsername, loginPassword;
    public GameObject registerRetrieveUsername, registerRetrievePassword, registerRetrieveEmail, registerRetrieveCode;
    public Button loginBtn, registerBtn, retrieveBtn, finishBtn;//功能按钮
    public GameObject registerPattern, retrievePattern;//界面花纹
    public GameObject loginWindow, registerRetrieveWindow, chooseWindow, buttons;//界面
    /// <summary>
    /// 内部组件声明
    /// </summary>
    private TextInputField loginUsernameInput, loginPasswordInput, usernameInput, emailInput, passwordInput, codeInput;

    /// <summary>ck
    /// 界面类型
    /// </summary>
    private Type _type = Type.Welcome;
    public Type type {
        get { return _type; }
        set {
            if (_type == value) return;
            _type = value; requestRefresh();
        }
    } 

    /// <summary>
    /// 场景组件引用
    /// </summary>
    TitleScene scene;

    /// <summary>
    /// 外部系统引用
    /// </summary>
    GameSystem gameSys = null;
    ExermonGameSystem exermonSys = null;
    PlayerService playerSer = null;

    #region 初始化

    /// <summary>
    /// 初次初始化
    /// </summary>
    protected override void initializeOnce() {
        base.initializeOnce();
        if (gameSys == null) gameSys = GameSystem.get();
        if (exermonSys == null) exermonSys = ExermonGameSystem.get();
        if (playerSer == null) playerSer = PlayerService.get();
        scene = (TitleScene)SceneUtils.getSceneObject("Scene");
        state = State.Shown;
        _type = Type.Login;
        initializeInputItemFields();
        setupInputItemFields();
    }


    /// <summary>
    /// 初始化输入域
    /// </summary>
    void initializeInputItemFields() {
        //登录输入域
        if (loginUsername) loginUsernameInput = SceneUtils.get<TextInputField>(loginUsername);
        if (loginPassword) loginPasswordInput = SceneUtils.get<TextInputField>(loginPassword);

        //注册和找回输入域
        if (registerRetrieveUsername) usernameInput = SceneUtils.get<TextInputField>(registerRetrieveUsername);
        if (registerRetrievePassword) passwordInput = SceneUtils.get<TextInputField>(registerRetrievePassword);
        if (registerRetrieveEmail) emailInput = SceneUtils.get<TextInputField>(registerRetrieveEmail);
        if (registerRetrieveCode) codeInput = SceneUtils.get<TextInputField>(registerRetrieveCode);
    }

    /// <summary>
    /// 配置输入域
    /// </summary>
    void setupInputItemFields() {
        /*
        if (loginUsernameInput) loginUsernameInput.check = ValidateService.checkUsername;
        if (loginPasswordInput) loginPasswordInput.check = ValidateService.checkPassword;
        */
        if (usernameInput) usernameInput.check = ValidateService.checkUsername;
        if (passwordInput) passwordInput.check = ValidateService.checkPassword;
        if (emailInput) emailInput.check = ValidateService.checkEmail;
        if (codeInput) codeInput.check = ValidateService.checkCode;
    }

    #endregion

    #region 更新控制

    /// <summary>
    /// 更新
    /// </summary>
    protected override void update() {
        base.update();
        updateType();
    }

    /// <summary>
    /// 更新类型变换
    /// </summary>
    void updateType() {
        
    }

    #endregion

    #region 流程控制
    
    /// <summary>
    /// 进入登陆界面
    /// </summary>
    public void gotoLogin() {
        type = Type.Login;
        scene.rotatable = false;
    }

    /// <summary>
    /// 进入注册界面
    /// </summary>
    public void gotoRegister() {
        type = Type.Register;
    }

    /// <summary>
    /// 进入找回密码界面
    /// </summary>
    public void gotoRetrieve() {
        type = Type.Retrieve;
    }

    /// <summary>
    /// 登陆
    /// </summary>
    public void login() {
        if (type != Type.Login) gotoLogin();
        else if (checkLogin()) doLogin();
        else onCheckFailed();
    }

    /// <summary>
    /// 注册
    /// </summary>
    public void register() {
        if (checkRegisterOrRetrieve()) doRegister();
        else onCheckFailed();
    }

    /// <summary>
    /// 找回密码
    /// </summary>
    public void retrieve() {
        if (checkRegisterOrRetrieve()) doRetrieve();
        else onCheckFailed();
    }

    /// <summary>
    /// 获取验证码
    /// </summary>
    public void send() {
        if (checkSend()) doSend();
        else onCheckFailed();
    }

    /// <summary>
    /// 不正确的格式
    /// </summary>
    void onCheckFailed() {
        gameSys.requestAlert(InvalidInputAlertText);
    }

    /// <summary>
    /// 执行登陆
    /// </summary>
    void doLogin() {
        var un = loginUsernameInput.getValue();
        var pw = loginPasswordInput.getValue();

        playerSer.login(un, pw, onLoginSuccess);
    }

    /// <summary>
    /// 执行注册
    /// </summary>
    void doRegister() {
        var un = usernameInput.getValue();
        var pw = passwordInput.getValue();
        var email = emailInput.getValue();
        var code = codeInput.getValue();

        playerSer.register(un, pw, email, code, onRegisterSuccess);
    }

    /// <summary>
    /// 执行找回密码
    /// </summary>
    void doRetrieve() {
        var un = usernameInput.getValue();
        var pw = passwordInput.getValue();
        var email = emailInput.getValue();
        var code = codeInput.getValue();

        playerSer.retrieve(un, pw, email, code, onRegisterSuccess);
    }

    /// <summary>
    /// 执行验证码发送
    /// </summary>
    void doSend() {
        var un = usernameInput.getValue();
        var email = emailInput.getValue();
        var type = "";
        if (this.type == Type.Register) type = "register";
        if (this.type == Type.Retrieve) type = "retrieve";
        playerSer.sendCode(un, email, type, onSendSuccess);
    }

    /// <summary>
    /// 注册成功回调
    /// </summary>
    /// <param name="res">返回结果</param>
    void onRegisterSuccess(JsonData res) {
        var id = DataLoader.loadInt(res, "id");
        gameSys.requestAlert(string.Format(RegisterSuccessText, id));
        gotoLogin();
    }

    /// <summary>
    /// 登录成功回调
    /// </summary>
    /// <param name="res">返回结果</param>
    void onLoginSuccess() {
        var config = exermonSys.configure;
        config.rememberUsername = usernameInput.getValue();
        config.rememberPassword = passwordInput.getValue();
        scene.startGame();
    }

    /// <summary>
    /// 重置密码成功回调
    /// </summary>
    /// <param name="res">返回结果</param>
    void onRetrieveSuccess(JsonData res) {
        gameSys.requestAlert(RetrieveSuccessText);
        gotoLogin();
    }

    /// <summary>
    /// 验证码发送成功回调
    /// </summary>
    /// <param name="res">返回结果</param>
    void onSendSuccess(JsonData res) {
        gameSys.requestAlert(SendCodeSuccessText);
    }

    #region 数据校验

    /// <summary>
    /// 检查是否可以登陆
    /// </summary>
    bool checkLogin() {
        return usernameInput.isCorrect() && passwordInput.isCorrect();
    }

    /// <summary>
    /// 检查是否可以发送验证码
    /// </summary>
    bool checkSend() {
        return usernameInput.isCorrect() && emailInput.isCorrect();
    }

    /// <summary>
    /// 检查是否可以注册/忘记密码
    /// </summary>
    bool checkRegisterOrRetrieve() {
        return usernameInput.isCorrect() && passwordInput.isCorrect() &&
            emailInput.isCorrect() && codeInput.isCorrect();
    }

    #endregion

    #endregion

    #region 界面控制

    /// <summary>
    /// 刷新视窗
    /// </summary>
    protected override void refresh() {
        base.refresh();
        clearAllInputFields();
        switch (type) {
            case Type.Welcome: refreshWelcome(); break;
            case Type.Login: refreshLogin(); break;
            case Type.Register: refreshRegister(); break;
            case Type.Retrieve: refreshRetrieve(); break;
        }
    }

    /// <summary>
    /// 欢迎界面
    /// </summary>
    void refreshWelcome() {
    }

    /// <summary>
    /// 登陆界面
    /// </summary>
    void refreshLogin() {
        loginWindow.SetActive(true);
        showLoginInputFields();
        showAllButtons();
        autoComplete();
    }

    /// <summary>
    /// 注册界面
    /// </summary>
    void refreshRegister() {
        registerRetrieveWindow.SetActive(true);
        registerPattern.SetActive(true);

        showRegisterRetrieveInputFields();

        finishBtn.onClick.RemoveAllListeners();
        finishBtn.onClick.AddListener(register);

    }

    /// <summary>
    /// 找回界面
    /// </summary>
    void refreshRetrieve() {
        registerRetrieveWindow.SetActive(true);
        retrievePattern.SetActive(true);

        showRegisterRetrieveInputFields();

        finishBtn.onClick.RemoveAllListeners();
        finishBtn.onClick.AddListener(retrieve);

    }

    #region 输入域控制

    /// <summary>
    /// 显示登陆输入域
    /// </summary>
    void showLoginInputFields() {
        loginUsernameInput.startView();
        loginPasswordInput.startView();
    }

    /// <summary>
    /// 显示注册和找回的输入域
    /// </summary>
    void showRegisterRetrieveInputFields() {
        usernameInput.startView();
        passwordInput.startView();
        emailInput.startView();
        codeInput.startView();
    }

    /// <summary>
    /// 清除所有输入域
    /// </summary>
    void clearAllInputFields() {
        loginUsernameInput.terminateView();
        loginPasswordInput.terminateView();

        usernameInput.terminateView();
        passwordInput.terminateView();
        emailInput.terminateView();
        codeInput.terminateView();

        retrievePattern.SetActive(false);
        registerPattern.SetActive(false);

        loginWindow.SetActive(false);
        registerRetrieveWindow.SetActive(false);
    }

    #endregion

    #region 按钮控制

    /// <summary>
    /// 显示登陆按钮
    /// </summary>
    void showAllButtons() {
        buttons.SetActive(true);
       
    }

    /// <summary>
    /// 清除所有按钮
    /// </summary>
    void clearAllButtons() {
        buttons.SetActive(false);
    }

    #endregion

    /// <summary>
    /// 清除视窗
    /// </summary>
    protected override void clear() {
        clearAllInputFields();
        clearAllButtons();
    }

    /// <summary>
    /// 处理自动补全项（记住密码）
    /// </summary>
    void autoComplete() {
        var config = exermonSys.configure;
        if(config.rememberUsername.Length > 0 &&
            config.rememberPassword.Length > 0) {
            usernameInput.setValue(config.rememberUsername);
            passwordInput.setValue(config.rememberPassword);
        }
    }

    #endregion
}
