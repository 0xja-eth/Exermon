using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 主场景
/// </summary>
public class MainScene : BaseScene {

    /// <summary>
    /// 文本定义
    /// </summary>

    /// <summary>
    /// 外部组件设置
    /// </summary>

    /// <summary>
    /// 能否跟随旋转
    /// </summary>
    public bool rotatable { get; set; } = true;

    /// <summary>
    /// 内部系统声明
    /// </summary>
    GameSystem gameSys;
    PlayerService playerSer;
    ExermonService exermonSer;
    ExermonGameSystem exermonSys;

    #region 初始化

    /// <summary>
    /// 场景名
    /// </summary>
    /// <returns>场景名</returns>
    public override string sceneName() {
        return SceneUtils.GameScene.MainScene;
    }

    /// <summary>
    /// 初始化外部系统
    /// </summary>
    protected override void initializeSystems() {
        base.initializeSystems();
        gameSys = GameSystem.get();
        exermonSys = ExermonGameSystem.get();
        playerSer = PlayerService.get();
        exermonSer = ExermonService.get();
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

    }
    
    #endregion
}
