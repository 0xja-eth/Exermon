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

    static readonly int[] NameLen = new int[] { 1, 8 };

    /*
    static readonly int[] ExerNameLen = new int[] { 0, 4 };

    static readonly int[] SchoolLen = new int[] { 0, 24 };
    static readonly int[] CityLen = new int[] { 0, 24 };
    static readonly int[] ContactLen = new int[] { 0, 24 };
    static readonly int[] DescLen = new int[] { 0, 128 };
    */

    /// <summary>
    /// 提示文本设定
    /// </summary>
    public const string UsernameTips = "用户名由6~16个英文字母或数字组成";
    public const string PasswordTips = "密码由8~24个非中文字符组成";
    public const string PhoneTips = "请按正确的电话号码格式输入";
    public const string EmailTips = "请按正确的邮箱格式输入";

    public const string CodeTips = "请输入收到的6位数字验证码";

    public const string NameTips = "昵称由1~8个任意字符组成";
    public const string ExerNameTips = "艾瑟萌昵称由1~8个任意字符组成";

    public const string SchoolTips = "学校名称不得超过24个字符";
    public const string CityTips = "居住地名称不得超过24个字符";
    public const string ContactTips = "联系方式不得超过24个字符";
    public const string DescTips = "个人介绍不得超过128个字符";

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
    /// 根据长度检查
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="maxLen">最大长度</param>
    /// <param name="minLen">最小长度</param>
    /// <returns>是否正确</returns>
    public static bool check(string value, int maxLen, int minLen = 0) {
        int len = value.Length;
        if (len > maxLen) return false;
        if (len < minLen) return false;
        return true;
    }

    /// <summary>
    /// 根据长度检查
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="len">长度[最小值, 最大值]</param>
    /// <returns>是否正确</returns>
    public static bool check(string value, int[] len) {
        return check(value, len[1], len[0]);
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

    /// <summary>
    /// 检查昵称
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>校验结果文本</returns>
    public static string checkName(string value) {
        return check(value, NameLen) ? "" : NameTips;
    }

    /*
    /// <summary>
    /// 检查艾瑟萌昵称
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>校验结果文本</returns>
    public static string checkExerName(string value) {
        return check(value, ExerNameLen) ? "" : ExerNameTips;
    }

    /// <summary>
    /// 检查学校名称
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>校验结果文本</returns>
    public static string checkSchool(string value) {
        return check(value, SchoolLen) ? "" : SchoolTips;
    }

    /// <summary>
    /// 检查居住地
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>校验结果文本</returns>
    public static string checkCity(string value) {
        return check(value, CityLen) ? "" : CityTips;
    }

    /// <summary>
    /// 检查联系方式
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>校验结果文本</returns>
    public static string checkContact(string value) {
        return check(value, ContactLen) ? "" : ContactTips;
    }

    /// <summary>
    /// 检查个人介绍
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>校验结果文本</returns>
    public static string checkDescription(string value) {
        return check(value, DescLen) ? "" : DescTips;
    }
    */

    #endregion

}