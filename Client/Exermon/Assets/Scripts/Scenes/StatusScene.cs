using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// 状态场景
/// </summary>
public class StatusScene : BaseScene {

    /// <summary>
    /// 文本定义
    /// </summary>

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public StatusWindow statusWindow;

    public InfoEditWindow infoEditWindow;

    public Button confirm;

    /// <summary>
    /// 按钮回调
    /// </summary>
    List<UnityAction> confirmCallbacks = new List<UnityAction>(); // 确认按钮回调
    List<UnityAction> backCallbacks = new List<UnityAction>(); // 返回按钮回调

    /// <summary>
    /// 内部系统声明
    /// </summary>
    PlayerService playerSer;

    #region 初始化

    /// <summary>
    /// 场景名
    /// </summary>
    /// <returns>场景名</returns>
    public override string sceneName() {
        return SceneUtils.GameScene.StatusScene;
    }

    /// <summary>
    /// 初始化外部系统
    /// </summary>
    protected override void initializeSystems() {
        base.initializeSystems();
        playerSer = PlayerService.get();
    }

    /// <summary>
    /// 初始化其他
    /// </summary>
    protected override void initializeOthers() {
        base.initializeOthers();
        SceneUtils.depositSceneObject("Scene", this);
    }

    /// <summary>
    /// 开始
    /// </summary>
    protected override void start() {
        base.start();
        refresh();
    }

    #endregion

    #region 请求项控制

    /// <summary>
    /// 添加确认回调
    /// </summary>
    /// <param name="action">动作</param>
    public void pushConfirmCallback(UnityAction action) {
        confirmCallbacks.Add(action);
        confirm.interactable = true;
    }

    /// <summary>
    /// 添加返回回调
    /// </summary>
    /// <param name="action">动作</param>
    public void pushBackCallback(UnityAction action) {
        backCallbacks.Add(action);
    }

    /// <summary>
    /// 清除回调项
    /// </summary>
    public void clearCallbacks() {
        confirmCallbacks.Clear();
        backCallbacks.Clear();
        confirm.interactable = false;
    }

    #endregion

    #region 场景控制

    /// <summary>
    /// 刷新场景
    /// </summary>
    public void refresh() {
        playerSer.getPlayerStatus(statusWindow.startWindow);
    }

    #endregion

    #region 流程控制

    /// <summary>
    /// 确认
    /// </summary>
    public void onConfirm() {
        foreach (var cb in confirmCallbacks) cb.Invoke();
        clearCallbacks();
    }

    /// <summary>
    /// 返回
    /// </summary>
    public void onBack() {
        if(backCallbacks.Count > 0)
            foreach (var cb in backCallbacks) cb.Invoke();
        else popScene();
        clearCallbacks();
    }

    #endregion
}
