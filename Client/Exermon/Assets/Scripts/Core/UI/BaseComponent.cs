
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

/// <summary>
/// 所有组件父类
/// </summary>
public class BaseComponent : MonoBehaviour {

    /// <summary>
    /// 开始标志
    /// </summary>
    public bool awaked { get; protected set; } = false;
    public bool started { get; protected set; } = false;
    public bool updating { get; protected set; } = false;

    #region 初始化

    /// <summary>
    /// 初始化（唤醒）
    /// </summary>
    private void Awake() {
        awaked = true; awake();
    }

    /// <summary>
    /// 初始化（同Awake）
    /// </summary>
    protected virtual void awake() { }

    /// <summary>
    /// 初始化（开始）
    /// </summary>
    private void Start() {
        started = true; start();
    }

    /// <summary>
    /// 初始化（同Start）
    /// </summary>
    protected virtual void start() { }

    #endregion

    #region 更新

    /// <summary>
    /// 更新
    /// </summary>
    private void Update() {
        updating = true;
        update();
        updating = false;
    }

    /// <summary>
    /// 更新（同Update）
    /// </summary>
    protected virtual void update() { }

    #endregion
}