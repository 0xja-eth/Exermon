
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using LitJson;
using UnityEditor;


/// <summary>
/// 玩家数据
/// </summary>
public class Player : BaseData {

    /// <summary>
    /// 属性
    /// </summary>
    public string username { get; private set; }
    public string password { get; private set; }
    public string name { get; private set; }
    public int genderId { get; private set; }
    public int gradeId { get; private set; }
    public int typeId { get; private set; }
    public DateTime createTime { get; private set; }

    /// <summary>
    /// 设置密码
    /// </summary>
    /// <param name="pw">密码</param>
    public void setPassword(string pw) {
        password = pw;
    }

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    /// <param name="idEnable">是否可用ID字段</param>
    public override void load(JsonData json) {
        base.load(json);

        username = DataLoader.loadString(json, "username");
        name = DataLoader.loadString(json, "name");
        genderId = DataLoader.loadInt(json, "gender_id");
        gradeId = DataLoader.loadInt(json, "grade_id");
        typeId = DataLoader.loadInt(json, "type_id");

        createTime = DataLoader.loadDateTime(json, "create_time");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["username"] = username;
        json["password"] = password;
        json["name"] = name;
        json["gender_id"] = genderId;
        json["grade_id"] = gradeId;
        json["type_id"] = typeId;
        json["create_time"] = DataLoader.convertDateTime(createTime);

        return json;
    }
}
