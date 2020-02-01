using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 艾瑟萌卡片
/// </summary>
public class ExerCard : BaseView {

    /// <summary>
    /// 常量定义
    /// </summary>

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public Image icon; // 图片
    public Text name, subject;
    public GameObject checkFlag;

    /// <summary>
    /// 内部变量声明
    /// </summary>
    ExerCardGroup group;

    Exermon exermon;

    int index;
    bool _checked;

    #region 初始化

    /// <summary>
    /// 配置组件
    /// </summary>
    public void configure(ExerCardGroup group, int index) {
        this.index = index;
        this.group = group;
        base.configure();
    }

    #endregion

    #region 界面控制

    /// <summary>
    /// 绘制艾瑟萌
    /// </summary>
    void drawExermon() {
        if (exermon == null) return;
        var icon = exermon.icon;
        var rect = new Rect(0, 0, icon.width, icon.height);
        this.icon.overrideSprite = Sprite.Create(
            icon, rect, new Vector2(0.5f, 0.5f));
        this.icon.overrideSprite.name = icon.name;
        name.text = exermon.name;
        subject.text = exermon.subject().name;
    }

    /// <summary>
    /// 更新选择状态
    /// </summary>
    void refreshSelected() {
        checkFlag.SetActive(_checked);
    }

    /// <summary>
    /// 清除艾瑟萌
    /// </summary>
    void clearExermon() {
        name.text = subject.text = "";
        icon.overrideSprite = null;
    }

    /// <summary>
    /// 刷新视窗（clear后重绘）
    /// </summary>
    public override void refresh() {
        base.refresh();
        drawExermon();
        refreshSelected();
    }

    public override void clear() {
        base.clear();
        clearExermon();
    }

    #endregion

    #region 数据控制

    /// <summary>
    /// 设置人物
    /// </summary>
    /// <param name="character">人物</param>
    public void setExermon(Exermon exermon) {
        if (this.exermon != exermon) {
            this.exermon = exermon;
            if (isForce()) check();
            refresh();
        }
    }

    /// <summary>
    /// 必选项
    /// </summary>
    /// <returns>是否必选</returns>
    public bool isForce() {
        if (exermon == null) return false;
        return exermon.subject().force;
    }

    /// <summary>
    /// 反转
    /// </summary>
    public void toggle() {
        if (isForce()) return;
        if (_checked) uncheck();
        else check();
        refreshSelected();
    }

    /// <summary>
    /// 选择
    /// </summary>
    public void check() {
        _checked = true;
        refreshSelected();
        if (isForce()) return;
        group.addCheck(index);
    }

    /// <summary>
    /// 取消选择
    /// </summary>
    public void uncheck() {
        if (isForce()) return;
        _checked = false;
        refreshSelected();
        group.removeCheck(index);
    }

    /// <summary>
    /// 获取人物
    /// </summary>
    /// <returns>人物</returns>
    public Exermon getExermon() {
        return exermon;
    }

    /// <summary>
    /// 是否选择
    /// </summary>
    /// <returns>是否选择</returns>
    public bool isChecked() {
        return _checked;
    }

    /// <summary>
    /// 是否选中
    /// </summary>
    /// <returns>是否选中</returns>
    public bool isSelected() {
        return group.getIndex() == index;
    }

    #endregion

    #region 事件控制

    /// <summary>
    /// 选中回调
    /// </summary>
    public void onSelected() {
        if(isSelected()) toggle();
        group.setIndex(index);
    }
    #endregion
}
