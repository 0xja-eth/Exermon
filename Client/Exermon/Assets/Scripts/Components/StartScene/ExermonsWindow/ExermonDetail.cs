using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 艾瑟萌详细信息
/// </summary>
public class ExermonDetail : ItemInfo<Exermon> {

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
    public ParamBarsGroup paramsView;
    public TextInputField nicknameInput;

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

    #endregion

    #region 数据控制

    /// <summary>
    /// 获取容器
    /// </summary>
    /// <returns></returns>
    public ExermonsContainer getContainer() {
        return container as ExermonsContainer;
    }

    #endregion

    #region 界面控制

    /// <summary>
    /// 绘制确切的物品
    /// </summary>
    /// <param name="exermon">物品</param>
    protected override void drawExactlyItem(Exermon exermon) {
        drawFullView(exermon);
        drawInfoView(exermon);
        drawParamsView(exermon);
        completeNicknameText();
    }

    /// <summary>
    /// 绘制全身像卡片
    /// </summary>
    /// <param name="exermon">物品</param>
    void drawFullView(Exermon exermon) {
        var full = exermon.full;
        var rect = new Rect(0, 0, full.width, full.height);
        this.full.overrideSprite = Sprite.Create(
            full, rect, new Vector2(0.5f, 0.5f));
        this.full.overrideSprite.name = full.name;
        name.text = exermon.name;
        subject.text = exermon.subject().name;
    }

    /// <summary>
    /// 绘制基本信息
    /// </summary>
    /// <param name="exermon">物品</param>
    void drawInfoView(Exermon exermon) {
        var starText = exermon.star().name;
        var typeText = exermon.typeText();

        description.text = exermon.description;
        star.text = string.Format(StarTextFormat, starText);
        type.text = string.Format(TypeTextFormat, typeText);
        animal.text = string.Format(AnimalTextFormat, exermon.animal);
    }

    /// <summary>
    /// 绘制属性信息
    /// </summary>
    /// <param name="exermon">物品</param>
    void drawParamsView(Exermon exermon) {
        paramsView.setValues(exermon);
    }

    /// <summary>
    /// 自动读取设定好的昵称
    /// </summary>
    void completeNicknameText() {
        var container = getContainer();
        nicknameInput.setText(container.getNickname(index));
    }

    /// <summary>
    /// 清除全身像卡片
    /// </summary>
    void clearFullView() {
        full.overrideSprite = null;
        name.text = subject.text = "";
    }

    /// <summary>
    /// 清除基本信息
    /// </summary>
    void clearInfoView() {
        description.text = star.text = type.text = animal.text = "";
    }

    /// <summary>
    /// 清除属性信息
    /// </summary>
    void clearParamsView() {
        paramsView.clearValues();
    }

    /// <summary>
    /// 清空昵称输入
    /// </summary>
    void clearNicknameText() {
        nicknameInput.setText("", false);
    }

    /// <summary>
    /// 清除物品
    /// </summary>
    protected override void clearItem() {
        clearFullView();
        clearInfoView();
        clearParamsView();
        clearNicknameText();
    }

    #endregion

    #region 流程控制

    /// <summary>
    /// 昵称改变回调事件
    /// </summary>
    public void onNicknameChanged() {
        getContainer().changeNickname(index, nicknameInput.getText());
    }

    #endregion
}
