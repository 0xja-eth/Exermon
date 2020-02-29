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
    /// 请求项，上层需要发起的请求通过该项储存，统一执行
    /// </summary>
    struct RequestItem {

        /// <summary>
        /// 属性
        /// </summary>
        public string name;
        public UnityAction action;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">请求名称</param>
        /// <param name="action">请求函数动作</param>
        public RequestItem(string name, UnityAction action) {
            this.name = name; this.action = action;
        }
    }

    /// <summary>
    /// 文本定义
    /// </summary>
    const string RequestAlertTextFormat = "你有 {0} 等 {1} 个操作仍未保存，确定放弃修改返回吗？";

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public StatusWindow statusWindow;

    public InfoEditWindow infoEditWindow;

    public Button confirm;

    /// <summary>
    /// 请求项列表
    /// </summary>
    List<RequestItem> requestItems = new List<RequestItem>();

    /// <summary>
    /// 内部系统声明
    /// </summary>
    GameSystem gameSys;
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
        gameSys = GameSystem.get();
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

    #region 请求项控制

    /// <summary>
    /// 添加请求项
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="action">动作</param>
    /// <param name="immediately">立刻发送</param>
    public void pushRequestItem(string name, UnityAction action, bool immediately = false) {
        if (immediately) action.Invoke();
        else requestItems.Add(new RequestItem(name, action));
        confirm.interactable = true;
    }

    /// <summary>
    /// 清除请求项
    /// </summary>
    public void clearRequestItems() {
        requestItems.Clear();
        confirm.interactable = false;
    }

    /// <summary>
    /// 执行请求项
    /// </summary>
    void doRequestItem(RequestItem item) {
        item.action.Invoke();
    }

    /// <summary>
    /// 检查请求项
    /// </summary>
    bool checkRequestItems() {
        return requestItems.Count <= 0;
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
        foreach (var item in requestItems) doRequestItem(item);
        clearRequestItems();
    }

    /// <summary>
    /// 返回
    /// </summary>
    public void onBack() {
        if (checkRequestItems()) doBack();
        else confirmBack();
    }

    /// <summary>
    /// 进行返回操作
    /// </summary>
    void doBack() {
        clearRequestItems();
        if (infoEditWindow.shown) infoEditWindow.terminateView();
        else gameSys.requestChangeScene(GameSystem.ChangeSceneRequest.Type.Pop);
    }

    /// <summary>
    /// 提示确认返回
    /// </summary>
    void confirmBack() {
        gameSys.requestAlert(generateRequestItemAlertText(),
            AlertWindow.Type.YesOrNo, doBack);
    }

    /// <summary>
    /// 生成请求项提示文本
    /// </summary>
    /// <returns></returns>
    string generateRequestItemAlertText() {
        var cnt = requestItems.Count;
        var first = requestItems[0].name;
        return string.Format(RequestAlertTextFormat, first, cnt);
    }

    #endregion
}
