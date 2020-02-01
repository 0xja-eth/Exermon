
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景父类
/// </summary>
public class BaseScene : BaseComponent {
    
    /// <summary>
    /// 外部组件设置
    /// </summary>
    public AlertWindow alertWindow;
    public LoadingWindow loadingWindow;

    /// <summary>
    /// 初始化标志
    /// </summary>
    public bool initialized { get; protected set; } = false;

    #region 初始化

    /// <summary>
    /// 场景名
    /// </summary>
    /// <returns>场景名</returns>
    public virtual string sceneName() { return ""; }

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void awake() {
        base.awake();
        initialized = true;
        initializeSceneUtils();
        initializeSystems();
        initializeOthers();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    void initializeSceneUtils() {
        SceneUtils.initialize(sceneName(), alertWindow, loadingWindow);
    }

    /// <summary>
    /// 初始化外部系统
    /// </summary>
    protected virtual void initializeSystems() {

    }

    /// <summary>
    /// 初始化其他项
    /// </summary>
    protected virtual void initializeOthers() {

    }

    #endregion

    #region 更新控制

    protected override void update() {
        base.update(); SceneUtils.update();
    }

    /// <summary>
    /// 创建协程
    /// </summary>
    /// <param name="func">协程函数</param>
    public void createCoroutine(IEnumerator func) {
        StartCoroutine(func);
    }

    #endregion

}
