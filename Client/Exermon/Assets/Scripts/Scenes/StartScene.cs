using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 开始场景
/// </summary>
public class StartScene : BaseScene {

    /// <summary>
    /// 文本定义
    /// </summary>
    const string AbnormalAlertText = "您的账号异常，请返回登陆重试！";

    /// <summary>
    /// 步骤枚举
    /// </summary>
    public enum Step {
        Character, Exermons, Gifts, Info, Finished, Abnormal
    }

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public CharacterWindow characterWindow;
    public ExermonsWindow exermonsWindow;
    public GiftsWindow giftsWindow;
    public InfoWindow infoWindow;

    /// <summary>
    /// 能否跟随旋转
    /// </summary>
    public bool rotatable { get; set; } = true;

    /// <summary>
    /// 内部系统声明
    /// </summary>
    GameSystem gameSys;
    PlayerService playerSer;
    ExermonGameSystem exermonSys;

    Step step;

    #region 初始化

    /// <summary>
    /// 场景名
    /// </summary>
    /// <returns>场景名</returns>
    public override string sceneName() {
        return SceneUtils.GameScene.StartScene;
    }

    /// <summary>
    /// 初始化外部系统
    /// </summary>
    protected override void initializeSystems() {
        base.initializeSystems();
        gameSys = GameSystem.get();
        exermonSys = ExermonGameSystem.get();
        playerSer = PlayerService.get();
    }

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
        refreshStep();
        refreshWindows();
    }

    /// <summary>
    /// 刷新当前步骤
    /// </summary>
    void refreshStep() {
        var player = playerSer.player;
        switch (player.statusEnum()) {
            case Player.Status.Uncreated:
                step = Step.Character; break;
            case Player.Status.CharacterCreated:
                step = Step.Exermons; break;
            case Player.Status.ExermonsCreated:
                step = Step.Gifts; break;
            case Player.Status.GiftsCreated:
                step = Step.Info; break;
            case Player.Status.Normal:
                step = Step.Finished; break;
            default:
                step = Step.Abnormal; break;
        }
    }

    /// <summary>
    /// 刷新显示的窗口
    /// </summary>
    void refreshWindows() {
        closeWindows();
        switch (step) {
            case Step.Character:
                startCharacterWindow(); break;
            case Step.Exermons:
                startExermonsWindow(); break;
            case Step.Gifts:
                startGiftsWindow(); break;
            case Step.Info:
                startInfoWindow(); break;
            case Step.Finished:
                processFinished(); break;
            case Step.Abnormal:
                processAbnormal(); break;
        }
    }

    /// <summary>
    /// 开始人物创建窗口
    /// </summary>
    void startCharacterWindow() {
        characterWindow.startWindow();
        //exermonsWindow.startWindow();
    }

    /// <summary>
    /// 开始艾瑟萌选择窗口
    /// </summary>
    void startExermonsWindow() {
        exermonsWindow.startWindow();
    }

    /// <summary>
    /// 开始天赋选择窗口
    /// </summary>
    void startGiftsWindow() {
        giftsWindow.startWindow();
    }

    /// <summary>
    /// 开始完善资料窗口
    /// </summary>
    void startInfoWindow() {
        infoWindow.startWindow();
    }

    /// <summary>
    /// 处理创建完成
    /// </summary>
    void processFinished() {
        exermonSys.startGame();
    }

    /// <summary>
    /// 处理异常
    /// </summary>
    void processAbnormal() {
        gameSys.requestAlert(AbnormalAlertText);
    }

    /// <summary>
    /// 关闭所有窗口
    /// </summary>
    public void closeWindows() {
        characterWindow.terminateWindow();
        exermonsWindow.terminateWindow();
        //giftsWindow.terminateWindow();
        //infoWindow.terminateWindow();
    }

    #endregion
}
