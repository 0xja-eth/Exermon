
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using LitJson;
using Random = UnityEngine.Random;

/// <summary>
/// 状态窗口
/// </summary>
public class StatusWindow : BaseWindow {

    /// <summary>
    /// 文本常量定义
    /// </summary>
    const string InvalidInputAlertText = "请检查输入格式正确后再提交！";

    const string CreateSuccessText = "创建人物成功！";

    /// <summary>
    /// 视图枚举
    /// </summary>
    public enum View {
        HumanView, ExermonView,
    }

    /// <summary>
    /// 外部组件设置
    /// </summary>
    HumanView humanView;
    ExermonView exermonView;

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
    PlayerService playerSer = null;

    #region 初始化

    /// <summary>
    /// 初次初始化
    /// </summary>
    protected override void initializeOnce() {
        base.initializeOnce();
        if (playerSer == null) playerSer = PlayerService.get();
        scene = (StartScene)SceneUtils.getSceneObject("Scene");
        configureSubViews();
    }

    /// <summary>
    /// 配置子组件
    /// </summary>
    void configureSubViews() {
    }

    #endregion

    #region 开启控制

    /// <summary>
    /// 开始窗口
    /// </summary>
    public override void startWindow() {
        startWindow(View.HumanView);
    }

    /// <summary>
    /// 开始窗口
    /// </summary>
    public void startWindow(View view) {
        base.startWindow();
        switchView(view);
    }

    #endregion

    #region 流程控制

    /// <summary>
    /// 切换视图
    /// </summary>
    public void switchView(View view) {
        clearView();
        switch(view) {
            case View.HumanView: humanView.startView(playerSer.player); break;
            case View.ExermonView: exermonView.startView(playerSer.player); break;
        }
    }

    #region 数据校验
    
    #endregion

    #endregion

    #region 界面控制

    /// <summary>
    /// 清除视图
    /// </summary>
    public void clearView() {
        humanView.terminateView();
        exermonView.terminateView();
    }

    /// <summary>
    /// 清除窗口
    /// </summary>
    protected override void clear() {
        base.clear();
        clearView();
    }

    #endregion
}
