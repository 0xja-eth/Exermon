using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 艾瑟萌详细信息
/// </summary>
public class ExermonDetail : BaseView {

    /// <summary>
    /// 常量设置
    /// </summary>
    const string StarTextFormat = "星级：{0}";
    const string TypeTextFormat = "类型：{0}";
    const string AnimalTextFormat = "品种：{0}";

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public Image full;
    public Text name, subject, description, star, type, animal;
    public ParamGroup paramsView;
    public TextInputField nicknameInput;

    /// <summary>
    /// 内部变量声明
    /// </summary>
    ExerCardGroup group;

    Exermon exermon;
    int index;

    #region 初始化

    /// <summary>
    /// 初次打开时初始化（子类中重载）
    /// </summary>
    protected override void initializeOnce() {
        nicknameInput.check = ValidateService.checkExerName;
        setupParamsView();
    }

    /// <summary>
    /// 配置属性组
    /// </summary>
    void setupParamsView() {
        var params_ = DataService.get().staticData.configure.baseParams;
        paramsView.configure(params_);
    }

    /// <summary>
    /// 配置组件
    /// </summary>
    /// <param name="window">父窗口</param>
    public void configure(ExerCardGroup group) {
        this.group = group;
        base.configure();
    }

    #endregion
    
    #region 数据控制
    
    /// <summary>
    /// 设置艾瑟萌
    /// </summary>
    /// <param name="exermon">艾瑟萌实例</param>
    public void setExermon(Exermon exermon, int index) {
        if(this.exermon != exermon) {
            this.exermon = exermon;
            this.index = index;
            refresh();
        }
    }

    #endregion

    #region 界面控制

    /// <summary>
    /// 绘制全身像卡片
    /// </summary>
    void drawFullView() {
        if (exermon == null) return;
        var full = exermon.full;
        var rect = new Rect(0, 0, full.width, full.height);
        this.full.overrideSprite = Sprite.Create(
            full, rect, new Vector2(0.5f, 0.5f));
        this.full.overrideSprite.name = full.name;
        name.text = exermon.name;
        subject.text = exermon.subject().name;
    }

    /// <summary>
    /// 清除全身像卡片
    /// </summary>
    void clearFullView() {
        full.overrideSprite = null;
        name.text = subject.text = "";
    }

    /// <summary>
    /// 绘制基本信息
    /// </summary>
    void drawInfoView() {
        if (exermon == null) return;
        var starText = exermon.star().name;
        var typeText = exermon.typeText();

        description.text = exermon.description;
        star.text = string.Format(StarTextFormat, starText);
        type.text = string.Format(TypeTextFormat, typeText);
        animal.text = string.Format(AnimalTextFormat, exermon.animal);
    }

    /// <summary>
    /// 清除基本信息
    /// </summary>
    void clearInfoView() {
        description.text = star.text = type.text = animal.text = "";
    }

    /// <summary>
    /// 绘制属性信息
    /// </summary>
    void drawParamsView() {
        paramsView.setValues(exermon);
    }

    /// <summary>
    /// 清除属性信息
    /// </summary>
    void clearParamsView() {
        paramsView.clearValues();
    }

    /// <summary>
    /// 自动读取设定好的昵称
    /// </summary>
    void completeNicknameText() {
        Debug.Log("completeNicknameText" + group.getNickname(index));
        nicknameInput.setText(group.getNickname(index));
    }

    /// <summary>
    /// 清空昵称输入
    /// </summary>
    void clearNicknameText() {
        nicknameInput.setText("");
    }

    /// <summary>
    /// 刷新视窗（clear后重绘）
    /// </summary>
    public override void refresh() {
        base.refresh();
        drawFullView();
        drawInfoView();
        drawParamsView();
        completeNicknameText();
    }

    /// <summary>
    /// 清除描述
    /// </summary>
    public override void clear() {
        base.clear();
        clearFullView();
        clearInfoView();
        // clearParamsView();
        // clearNicknameText();
    }

    #endregion

    #region 流程控制

    /// <summary>
    /// 昵称改变回调事件
    /// </summary>
    public void onNicknameChanged() {
        Debug.Log("onNicknameChanged: " + nicknameInput.getText());
        group.changeNickname(index, nicknameInput.getText());
    }

    #endregion
}
