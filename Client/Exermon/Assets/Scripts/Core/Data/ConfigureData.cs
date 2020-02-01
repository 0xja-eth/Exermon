using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using LitJson;
using UnityEditor;

/// <summary>
/// 配置数据
/// </summary>
public class ConfigureData : BaseData {

    /// <summary>
    /// 设置项
    /// </summary>
    public string rememberPassword { get; set; } = null; // 记住密码
    public string rememberUsername { get; set; } = null; // 记住账号
    public bool autoLogin { get; set; } = false; // 自动登录

    /// <summary>
    /// 是否需要ID
    /// </summary>
    protected override bool idEnable() { return false; }

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json"></param>
    public override void load(JsonData json) {
        base.load(json);
        rememberPassword = DataLoader.loadString(json, "remember_password");
        rememberUsername = DataLoader.loadString(json, "remember_username");
        autoLogin = DataLoader.loadBool(json, "auto_login");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();
        json["remember_password"] = rememberPassword;
        json["remember_username"] = rememberUsername;
        json["auto_login"] = autoLogin;
        return json;
    }
}