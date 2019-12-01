using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

using LitJson;

/// <summary>
/// 玩家服务
/// </summary>
public class ValidateService : BaseService<ValidateService> {

    /// <summary>
    /// 正则设定
    /// </summary>
    static readonly Regex UsernameReg = new Regex(@"^[a-z0-9A-Z_]{6,16}$");
    static readonly Regex PasswordReg = new Regex(@"^[^\u4e00-\u9fa5].{8,24}$");
    static readonly Regex PhoneReg = new Regex(@"^1[0-9]{10}$");
    static readonly Regex EmailReg = new Regex(@"^([\w]+\.*)([\w]+)\@[\w]+\.\w{3}(\.\w{2}|)$");
    
    static readonly Regex CodeReg = new Regex(@"^\d{6}$");

    /// <summary>
    /// 提示文本设定
    /// </summary>
    public const string UsernameTips = "用户名由6~16个英文字母或数字组成";
    public const string PasswordTips = "密码由8~24个非中文字符组成";
    public const string PhoneTips = "请按正确的电话号码格式输入";
    public const string EmailTips = "请按正确的邮箱格式输入";

    public const string CodeTips = "请输入收到的6位数字验证码";

    // 本地数据校验
    #region 数据校验

    /// <summary>
    /// 数据校验
    /// </summary>
    /// <returns></returns>
    public static string checkData(string username = null, string password = null, string phone = null,
        string email = null, string name = null, string school = null, string code = null) {
        string res = "";

        res = (username != null ? checkUsername(username) : res);
        if (res != "") return res;
        res = (password != null ? checkPassword(password) : res);
        if (res != "") return res;
        res = (phone != null ? checkPhone(phone) : res);
        if (res != "") return res;
        res = (email != null ? checkEmail(email) : res);
        if (res != "") return res;
        res = (code != null ? checkCode(code) : res);
        if (res != "") return res;

        return res;
    }

    /// <summary>
    /// 根据正则表达式检查
    /// </summary>
    /// <param name="reg">检查规则</param>
    /// <param name="value">值</param>
    /// <returns>是否正确</returns>
    public static bool check(Regex reg, string value) {
        return reg.IsMatch(value);
    }

    /// <summary>
    /// 检查用户名
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>校验结果文本</returns>
    public static string checkUsername(string value) {
        return check(UsernameReg, value) ? "" : UsernameTips;
    }

    /// <summary>
    /// 检查密码
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>校验结果文本</returns>
    public static string checkPassword(string value) {
        return check(PasswordReg, value) ? "" : PasswordTips;
    }

    /// <summary>
    /// 检查手机号
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>校验结果文本</returns>
    public static string checkPhone(string value) {
        return check(PhoneReg, value) ? "" : PhoneTips;
    }

    /// <summary>
    /// 检查邮箱
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>校验结果文本</returns>
    public static string checkEmail(string value) {
        return check(EmailReg, value) ? "" : EmailTips;
    }

    /// <summary>
    /// 检查验证码
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>校验结果文本</returns>
    public static string checkCode(string value) {
        return check(CodeReg, value) ? "" : CodeTips;
    }

    #endregion

}