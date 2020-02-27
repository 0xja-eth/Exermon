using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using LitJson;
using UnityEditor;

/// <summary>
/// Exermon控制类
/// </summary>
/// <remarks>
/// 控制整个游戏进程（游戏内流程）
/// </remarks>
public class ExermonGameSystem : BaseSystem<ExermonGameSystem> {

    /// <summary>
    /// 游戏配置（设置）
    /// </summary>
    public ConfigureData configure { get; protected set; } = new ConfigureData();

    /// <summary>
    /// 外部系统
    /// </summary>
    GameSystem gameSys;
    StorageSystem storageSys;
    PlayerService playerSer;

    /// <summary>
    /// 初始化外部系统
    /// </summary>
    protected override void initializeSystems() {
        base.initializeSystems();
        gameSys = GameSystem.get();
        storageSys = StorageSystem.get();
        playerSer = PlayerService.get();
        gameSys.reconnectedCallback = onReconnected;
    }

    #region 流程控制

    /// <summary>
    /// 开始游戏（根据用户是否创建角色自动分配实际执行的操作）
    /// </summary>
    public void startGame() {
        storageSys.save();
        var player = playerSer.player;
        if (player.isCreated()) loadGame();
        else newGame();
    }

    /// <summary>
    /// 新游戏（未有角色的玩家）
    /// </summary>
    public void newGame() {
        gameSys.requestChangeScene(SceneUtils.GameScene.StartScene, 
            GameSystem.ChangeSceneRequest.Type.Push);
    }

    /// <summary>
    /// 继续游戏（已经有角色的玩家）
    /// </summary>
    public void loadGame() {
        gameSys.requestChangeScene(SceneUtils.GameScene.MainScene, 
            GameSystem.ChangeSceneRequest.Type.Push);
    }

    /// <summary>
    /// 游戏登出
    /// </summary>
    public void logoutGame() {
        playerSer.logout();
    }

    /// <summary>
    /// 返回主菜单
    /// </summary>
    public void backToMenu() {
        if (playerSer.isLogined()) logoutGame();
        gameSys.requestChangeScene(SceneUtils.GameScene.TitleScene, 
            GameSystem.ChangeSceneRequest.Type.Goto);
    }

    /// <summary>
    /// 结束游戏
    /// </summary>
    public void exitGame() {
        gameSys.terminate(beforeQuit);
    }

    /// <summary>
    /// 结束前操作
    /// </summary>
    void beforeQuit() {
        if (playerSer.isLogined()) logoutGame();
    }

    #endregion

    #region 回调控制

    /// <summary>
    /// 重连回调
    /// </summary>
    void onReconnected() {
        if (playerSer.isLogined()) playerSer.reconnect();
    }

    #endregion
}

