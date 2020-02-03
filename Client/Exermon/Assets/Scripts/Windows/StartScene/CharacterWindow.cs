
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using LitJson;

/// <summary>
/// 创建人物窗口
/// </summary>
public class CharacterWindow : BaseWindow {

    /// <summary>
    /// 文本常量定义
    /// </summary>
    const string InvalidInputAlertText = "请检查输入格式正确后再提交！";

    const string CreateSuccessText = "创建人物成功！";

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public CharacterContainer bustGroup; // 人物集
    public TextInputField nameInput; // 名称
    public DropdownField gradeInput; // 年级

    /// <summary>
    /// 内部组件声明
    /// </summary>

    /// <summary>
    /// 场景组件引用
    /// </summary>
    StartScene scene;

    /// <summary>
    /// 外部系统引用
    /// </summary>
    GameSystem gameSys = null;
    DataService dataSer = null;
    PlayerService playerSer = null;

    #region 初始化

    /// <summary>
    /// 初次初始化
    /// </summary>
    protected override void initializeOnce() {
        base.initializeOnce();
        if (gameSys == null) gameSys = GameSystem.get();
        if (playerSer == null) playerSer = PlayerService.get();
        if (dataSer == null) dataSer = DataService.get();
        scene = (StartScene)SceneUtils.getSceneObject("Scene");
        configureSubViews();
    }

    /// <summary>
    /// 配置子组件
    /// </summary>
    void configureSubViews() {
        var characters = dataSer.staticData.data.characters;
        var grades = dataSer.staticData.configure.playerGrades;
        nameInput.check = ValidateService.checkName;
        bustGroup.configure(characters);
        gradeInput.configure(grades);
    }

    #endregion

    #region 流程控制

    /// <summary>
    /// 创建角色
    /// </summary>
    public void create() {
        if (check()) doCreate();
        else onCheckFailed();
    }

    /// <summary>
    /// 不正确的格式
    /// </summary>
    void onCheckFailed() {
        gameSys.requestAlert(InvalidInputAlertText, null, null);
    }

    /// <summary>
    /// 执行创建
    /// </summary>
    void doCreate() {
        var name = nameInput.getText();
        var grade = gradeInput.getValue().Item1;
        var cid = bustGroup.selectedItem().getID();

        playerSer.createCharacter(name, grade, cid, onCreateSuccess);
    }

    /// <summary>
    /// 创建人物成功回调
    /// </summary>
    void onCreateSuccess() {
        var req = gameSys.requestAlert(CreateSuccessText);
        req.addButton(AlertWindow.OKText, scene.refresh);
    }

    #region 数据校验

    /// <summary>
    /// 检查是否可以登陆
    /// </summary>
    bool check() {
        return nameInput.isCorrect();
    }

    #endregion

    #endregion

    #region 界面控制

    #endregion
}
