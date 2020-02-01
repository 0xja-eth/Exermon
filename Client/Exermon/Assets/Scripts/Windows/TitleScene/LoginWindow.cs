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
        Forget // 忘记密码界面
    }

    /// <summary>
    /// 文本常量定义
    /// </summary>
    const string WelcomeTitle = "点击进入";
    const string LoginTitle = "登陆";
    const string RegisterTitle = "注册";
    const string ForgetTitle = "重置密码";

    const string DefaultPasswordPlaceholder = "请输入密码"; // 默认密码输入占位符
    const string ForgetPasswordPlaceholder = "请输入新密码"; // 忘记密码时密码输入占位符

    const string ForgetEntryBtnText = "忘记密码"; // 登陆界面中忘记密码按钮文本
    const string ForgetConfirmBtnText = "重置密码"; // 重置密码界面中的确认按钮文本

    const string InvalidInputAlertText = "请检查输入格式正确后再提交！";

    const string RegisterSuccessText = "注册用户成功！你的幸运号码是 {0} ！";
    const string ForgetSuccessText = "密码已重置！请返回登录！";
    const string SendCodeSuccessText = "验证码发送成功！请留意你的邮箱！";

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public Text typeText; // 类型名称显示
    public Button menuBtn; // 中间按钮
    public GameObject username, email, password, code;
    public GameObject loginBtn, registerBtn, forgetBtn, backBtn;

    /// <summary>
    /// 内部组件声明
    /// </summary>
    TextInputField usernameInput, emailInput, passwordInput, codeInput;
    Text forgetBtnText;

    /// <summary>
    /// 界面类型
    /// </summary>
    private Type _type = Type.Welcome;
    public Type type {
        get { return _type; }
        set {
            if (_type == value) return;
            _type = value; refresh();
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
        initializeInputItemFields();
        initializeForgetBtnText();
        setupInputItemFields();
    }

    /// <summary>
    /// 初始化忘记密码按钮文本
    /// </summary>
    void initializeForgetBtnText() {
        if (forgetBtn) forgetBtnText = SceneUtils.find<Text>(forgetBtn, "Text");
    }

    /// <summary>
    /// 初始化输入域
    /// </summary>
    void initializeInputItemFields() {
        if (username) usernameInput = SceneUtils.find<TextInputField>(username, "InputField");
        if (password) passwordInput = SceneUtils.find<TextInputField>(password, "InputField");
        if (email) emailInput = SceneUtils.find<TextInputField>(email, "InputField");
        if (code) codeInput = SceneUtils.find<TextInputField>(code, "InputField");
    }

    /// <summary>
    /// 配置输入域
    /// </summary>
    void setupInputItemFields() {
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
        menuBtn.interactable = false;
        scene.rotatable = false;
    }

    /// <summary>
    /// 进入注册界面
    /// </summary>
    public void gotoRegister() {
        type = Type.Register;
    }

    /// <summary>
    /// 进入忘记密码界面
    /// </summary>
    public void gotoForget() {
        type = Type.Forget;
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
        if (type != Type.Register) gotoRegister();
        else if (checkRegisterOrForget()) doRegister();
        else onCheckFailed();
    }

    /// <summary>
    /// 忘记密码
    /// </summary>
    public void forget() {
        if (type != Type.Forget) gotoForget();
        else if (checkRegisterOrForget()) doForget();
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
        gameSys.requestAlert(InvalidInputAlertText, null, null);
    }

    /// <summary>
    /// 执行登陆
    /// </summary>
    void doLogin() {
        var un = usernameInput.getText();
        var pw = passwordInput.getText();

        playerSer.login(un, pw, onLoginSuccess);
    }

    /// <summary>
    /// 执行注册
    /// </summary>
    void doRegister() {
        var un = usernameInput.getText();
        var pw = passwordInput.getText();
        var email = emailInput.getText();
        var code = codeInput.getText();

        playerSer.register(un, pw, email, code, onRegisterSuccess);
    }

    /// <summary>
    /// 执行重置密码
    /// </summary>
    void doForget() {
        var un = usernameInput.getText();
        var pw = passwordInput.getText();
        var email = emailInput.getText();
        var code = codeInput.getText();

        playerSer.forget(un, pw, email, code, onRegisterSuccess);
    }

    /// <summary>
    /// 执行验证码发送
    /// </summary>
    void doSend() {
        var un = usernameInput.getText();
        var email = emailInput.getText();
        var type = "";
        if (this.type == Type.Register) type = "register";
        if (this.type == Type.Forget) type = "forget";
        playerSer.sendCode(un, email, type, onSendSuccess);
    }

    /// <summary>
    /// 注册成功回调
    /// </summary>
    /// <param name="res">返回结果</param>
    void onRegisterSuccess(JsonData res) {
        var id = DataLoader.loadInt(res, "id");
        gameSys.requestAlert(string.Format(RegisterSuccessText, id),
            AlertWindow.OK, new UnityAction[] { null, gotoLogin });
    }

    /// <summary>
    /// 登录成功回调
    /// </summary>
    /// <param name="res">返回结果</param>
    void onLoginSuccess() {
        var config = exermonSys.configure;
        config.rememberUsername = usernameInput.getText();
        config.rememberPassword = passwordInput.getText();
        scene.startGame();
    }

    /// <summary>
    /// 重置密码成功回调
    /// </summary>
    /// <param name="res">返回结果</param>
    void onForgetSuccess(JsonData res) {
        gameSys.requestAlert(ForgetSuccessText,
            AlertWindow.OK, new UnityAction[] { null, gotoLogin });
    }

    /// <summary>
    /// 验证码发送成功回调
    /// </summary>
    /// <param name="res">返回结果</param>
    void onSendSuccess(JsonData res) {
        gameSys.requestAlert(SendCodeSuccessText, null);
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
    bool checkRegisterOrForget() {
        return usernameInput.isCorrect() && passwordInput.isCorrect() &&
            emailInput.isCorrect() && codeInput.isCorrect();
    }

    #endregion

    #endregion

    #region 界面控制

    /// <summary>
    /// 刷新视窗（clear后重绘）
    /// </summary>
    public override void refresh() {
        base.refresh();
        switch(type) {
            case Type.Welcome: refreshWelcome(); break;
            case Type.Login: refreshLogin(); break;
            case Type.Register: refreshRegister(); break;
            case Type.Forget: refreshForget(); break;
        }
    }

    /// <summary>
    /// 欢迎界面
    /// </summary>
    void refreshWelcome() {
        typeText.text = WelcomeTitle;
    }

    /// <summary>
    /// 登陆界面
    /// </summary>
    void refreshLogin() {
        typeText.text = LoginTitle;
        passwordInput.placeholder.text = DefaultPasswordPlaceholder;
        showLoginInputFields();
        showLoginButtons();
        autoComplete();
    }

    /// <summary>
    /// 注册界面
    /// </summary>
    void refreshRegister() {
        typeText.text = RegisterTitle;
        passwordInput.placeholder.text = DefaultPasswordPlaceholder;
        showAllInputFields();
        showRegisterButtons();
    }

    /// <summary>
    /// 忘记密码界面
    /// </summary>
    void refreshForget() {
        typeText.text = ForgetTitle;
        passwordInput.placeholder.text = ForgetPasswordPlaceholder;
        showAllInputFields();
        showForgetButtons();
    }

    #region 输入域控制

    /// <summary>
    /// 显示登陆输入域
    /// </summary>
    void showLoginInputFields() {
        username.SetActive(true);
        password.SetActive(true);

        usernameInput.startView();
        passwordInput.startView();
    }

    /// <summary>
    /// 显示所有的输入域
    /// </summary>
    void showAllInputFields() {
        showLoginInputFields();

        email.SetActive(true);
        code.SetActive(true);

        emailInput.startView();
        codeInput.startView();
    }

    /// <summary>
    /// 清除所有输入域
    /// </summary>
    void clearAllInputFields() {
        usernameInput.terminateView();
        passwordInput.terminateView();
        emailInput.terminateView();
        codeInput.terminateView();

        username.SetActive(false);
        password.SetActive(false);
        email.SetActive(false);
        code.SetActive(false);
    }

    #endregion

    #region 按钮控制

    /// <summary>
    /// 显示登陆按钮
    /// </summary>
    void showLoginButtons() {
        loginBtn.SetActive(true);
        registerBtn.SetActive(true);
        forgetBtn.SetActive(true);
        forgetBtnText.text = ForgetEntryBtnText;
    }

    /// <summary>
    /// 显示注册按钮
    /// </summary>
    void showRegisterButtons() {
        registerBtn.SetActive(true);
        backBtn.SetActive(true);
    }

    /// <summary>
    /// 显示忘记密码按钮
    /// </summary>
    void showForgetButtons() {
        forgetBtn.SetActive(true);
        backBtn.SetActive(true);
        forgetBtnText.text = ForgetConfirmBtnText;
    }

    /// <summary>
    /// 清除所有按钮
    /// </summary>
    void clearAllButtons() {
        loginBtn.SetActive(false);
        registerBtn.SetActive(false);
        forgetBtn.SetActive(false);
        backBtn.SetActive(false);
    }

    #endregion

    /// <summary>
    /// 清除视窗
    /// </summary>
    public override void clear() {
        typeText.text = "";
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
            usernameInput.setText(config.rememberUsername);
            passwordInput.setText(config.rememberPassword);
        }
    }

    #endregion
}
