using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    #region 更新控制


    #endregion

    #region 场景控制

    /// <summary>
    /// 刷新场景
    /// </summary>
    public void refresh() {
        playerSer.getPlayerStatus(statusWindow.startWindow);
    }
    
    #endregion
}
