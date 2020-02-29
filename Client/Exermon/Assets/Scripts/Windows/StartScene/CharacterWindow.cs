
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using LitJson;
using Random = UnityEngine.Random;

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
    /// 名字库
    /// </summary>
    const string NameBase = "阿贝呗思斯坦爱蓉梦怡海山河光耀纽特盖百世白可乐胡虎威猛鲜花华为小米安俺菌俊李狗鸡蛋然冉苒秒妙内外" +
        "色杀金沙林之若者也琳韬丁马博邹生死吴王君利零虚爷老文章黄红泓道鼹彦闪孙儿杰任静俪康灵玲女男无痕攻击力名人博仁忍者目木水雷";

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
        if (dataSer == null) dataSer = DataService.get();
        if (playerSer == null) playerSer = PlayerService.get();
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
    /// 随机名称
    /// </summary>
    public void random() {
        nameInput.setValue(generateRandomName());
    }

    /// <summary>
    /// 生成随机名字
    /// </summary>
    public string generateRandomName() {
        string res = "";
        var nameLen = ValidateService.NameLen;
        var partNum = Random.Range(0, 3); // 片段数量
        while (res.Length < nameLen[1] && partNum >= 0) {
            var partLen = Random.Range(nameLen[0], nameLen[1]-1);
            for (int i = 0; i < partLen; i++) {
                var index = Random.Range(0, NameBase.Length);
                if (res.Length >= nameLen[1]) return res;
                res += NameBase[index];
            }
            if (res.Length >= nameLen[1]-1) return res;
            if (partNum > 0) res += "·";
            partNum--;
        }
        return res;
    }

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
        gameSys.requestAlert(InvalidInputAlertText);
    }

    /// <summary>
    /// 执行创建
    /// </summary>
    void doCreate() {
        var name = nameInput.getValue();
        var grade = gradeInput.getValueId();
        var cid = bustGroup.selectedItem().getID();

        playerSer.createCharacter(name, grade, cid, onCreateSuccess);
    }

    /// <summary>
    /// 创建人物成功回调
    /// </summary>
    void onCreateSuccess() {
        gameSys.requestAlert(CreateSuccessText);
        scene.refresh();
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
