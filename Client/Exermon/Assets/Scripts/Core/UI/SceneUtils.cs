using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景工具类
/// </summary>
public static class SceneUtils {

    /// <summary>
    /// 游戏场景数据
    /// </summary>
    public class GameScene {

        /// <summary>
        /// 常量设定
        /// </summary>
        public const string TitleScene = "TitleScene";
        public const string StartScene = "StartScene";
        public const string MainScene = "MainScene";
        public const string StatusScene = "StatusScene";
        public const string HelpScene = "HelpScene";
    }

    /// <summary>
    /// 常量设定
    /// </summary>
    public const string AlertWindowKey = "AlertWindow";
    public const string LoadingWindowKey = "LoadingWindow";

    /// <summary>
    /// 提示窗口（脚本）
    /// </summary>
    public static AlertWindow alertWindow {
        get {
            return (AlertWindow)getSceneObject(AlertWindowKey);
        }
        set {
            depositSceneObject(AlertWindowKey, value);
        }
    }

    /// <summary>
    /// 加载窗口（脚本）
    /// </summary>
    public static LoadingWindow loadingWindow {
        get {
            return (LoadingWindow)getSceneObject(LoadingWindowKey);
        }
        set {
            depositSceneObject(LoadingWindowKey, value);
        }
    }

    /// <summary>
    /// 提示文本缓存
    /// </summary>
    public static string alertText { get; set; } = "";

    /// <summary>
    /// 场景栈
    /// </summary>
    static Stack<string> sceneStack = new Stack<string>();

    /// <summary>
    /// 场景物体托管
    /// </summary>
    static Dictionary<string,UnityEngine.Object> sceneObjectDeposit = 
        new Dictionary<string, UnityEngine.Object>();

    /// <summary>
    /// 外部系统
    /// </summary>
    static GameSystem gameSys = null;
    static ExermonGameSystem exermonSys = null;

    /// <summary>
    /// 初始化界面工具
    /// </summary>
    public static void initialize(string scene, 
        AlertWindow alertWindow = null, LoadingWindow loadingWindow = null) {
        Debug.Log("Initializing SceneUtils...");
        if (gameSys == null) gameSys = GameSystem.get();
        if (exermonSys == null) exermonSys = ExermonGameSystem.get();
        Debug.Log("Systems loaded.");
        if (currentScene() != scene) gotoScene(scene);
        SceneUtils.alertWindow = alertWindow;
        SceneUtils.loadingWindow = loadingWindow;
    }

    #region 场景管理

    /// <summary>
    /// 托管场景物体（键每个单词首字母必须为大写）
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="obj">场景物体</param>
    /// <return>场景物体</return>
    public static UnityEngine.Object depositSceneObject(string key, UnityEngine.Object obj) {
        //if (key == AlertWindowKey) alertWindow = (AlertWindow)obj;
        //if (key == LoadingWindowKey) loadingWindow = (LoadingWindow)obj;
        sceneObjectDeposit.Add(key, obj); return obj;
    }
    /*
    /// <summary>
    /// 托管提示窗口（快捷方法）
    /// </summary>
    /// <param name="obj">提示窗口</param>
    /// <return>提示窗口</return>
    public static AlertWindow depositAlertWindow(AlertWindow obj) {
        return (AlertWindow)depositSceneObject(AlertWindowKey, obj);
    }

    /// <summary>
    /// 托管加载窗口（快捷方法）
    /// </summary>
    /// <param name="obj">加载窗口</param>
    /// <return>加载窗口</return>
    public static LoadingWindow depositLoadingWindow(LoadingWindow obj) {
        return (LoadingWindow)depositSceneObject(LoadingWindowKey, obj);
    }
    */
    /// <summary>
    /// 获取托管的场景物体
    /// </summary>
    /// <param name="key">键</param>
    /// <return>场景物体</return>
    public static UnityEngine.Object getSceneObject(string key) {
        return hasSceneObject(key) ? sceneObjectDeposit[key] : null;
    }

    /// <summary>
    /// 是否存在托管的场景物体
    /// </summary>
    /// <param name="key">键</param>
    /// <return>场是否存在</return>
    public static bool hasSceneObject(string key) {
        return sceneObjectDeposit.ContainsKey(key) && sceneObjectDeposit[key] != null;
    }

    /// <summary>
    /// 清空场景物体托管
    /// </summary>
    public static void clearSceneObjects() {
        sceneObjectDeposit.Clear();
    }

    /// <summary>
    /// 当前场景名称
    /// </summary>
    /// <returns>场景名称</returns>
    public static string currentScene() {
        return sceneStack.Count > 0 ? sceneStack.Peek() : "";
    }

    /// <summary>
    /// 真实当前场景名称
    /// </summary>
    /// <returns>场景名称</returns>
    public static string realCurrentScene() {
        return SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// 是否出现场景分歧
    /// </summary>
    /// <returns>是否场景分歧</returns>
    public static bool differentScene() {
        return currentScene() != realCurrentScene();
    }

    /// <summary>
    /// 返回上一场景（如果上一场景为空则退出游戏）
    /// </summary>
    /// <returns>当前场景名称</returns>
    public static void popScene() {
        sceneStack.Pop(); loadScene();
    }

    /// <summary>
    /// 添加场景（往当前追加场景）
    /// </summary>
    /// <param name="scene">场景名称</param>
    public static void pushScene(string scene) {
        sceneStack.Push(scene); loadScene();
    }

    /// <summary>
    /// 切换场景（替换掉当前场景）
    /// </summary>
    /// <param name="scene">场景名称</param>
    public static void changeScene(string scene) {
        if (sceneStack.Count > 0) sceneStack.Pop();
        pushScene(scene);
    }

    /// <summary>
    /// 转到场景（前面的场景将被清空）
    /// </summary>
    /// <param name="scene">场景名称</param>
    public static void gotoScene(string scene) {
        clearScene(); pushScene(scene);
    }

    /// <summary>
    /// 清除场景
    /// </summary>
    public static void clearScene() {
        sceneStack.Clear();
    }

    /// <summary>
    /// 读取/重新读取当前场景（如果当前场景为空则退出游戏）
    /// </summary>
    /// <param name="reload">是否重载</param>
    public static void loadScene(bool reload=false) {
        string scene = currentScene();
        if (scene == "") exermonSys.exitGame();
        // 如果需要重载（强制LoadScene）或者场景分歧
        else if(reload || differentScene()) {
            clearSceneObjects();
            SceneManager.LoadScene(scene);
        }
    }

    #endregion

    #region 公用组件管理

    /// <summary>
    /// 显示提示
    /// </summary>
    /// <param name="msg">提示信息</param>
    /// <param name="btns">按键文本</param>
    /// <param name="actions">按键回调</param>
    static void alert(string text,
        AlertWindow.Type type = AlertWindow.Type.Notice,
        UnityAction onOK = null, UnityAction onCancel = null,
        float duration = AlertWindow.DefaultDuration) {
        Debug.Log("alert: " + text + ":" + alertWindow);
        if (alertWindow) alertWindow.startWindow(text, type, onOK, onCancel, duration);
        // 若未设置提示窗口，储存这个信息
        else {
            alertText = text;
            onOK?.Invoke();
        }
    }
    /// <param name="req">弹窗请求</param>
    static void alert(GameSystem.AlertRequest req) {
        alert(req.text, req.type, req.onOK, req.onCancel, req.duration);
    }

    /// <summary>
    /// 开始加载窗口
    /// </summary>
    /// <param name="tips">加载界面文本</param>
    static void startLoadingWindow(string tips = "") {
        if (loadingWindow) loadingWindow.startWindow(tips);
    }

    /// <summary>
    /// 结束加载窗口
    /// </summary>
    static void endLoadingWindow() {
        if (loadingWindow) loadingWindow.terminateWindow();
    }

    /// <summary>
    /// 设置加载窗口进度
    /// </summary>
    /// <param name="rate">进度</param>
    static void setLoadingProgress(float rate) {
        if (loadingWindow) loadingWindow.setProgress(rate);
    }

    /// <summary>
    /// 处理加载窗口
    /// </summary>
    /// <param name="tips">加载界面文本</param>
    public static void processLoadingRequest(GameSystem.LoadingRequest req) {
        if (req.start) startLoadingWindow(req.text);
        else endLoadingWindow();
    }

    #endregion

    #region 更新管理

    /// <summary>
    /// 更新
    /// </summary>
    public static void update() {
        updateGameSystem();
    }

    /// <summary>
    /// 更新GameSystem
    /// </summary>
    public static void updateGameSystem() {
        updateAlert(); updateLoading();
        updateSceneChanging();
        gameSys.update();
    }

    /// <summary>
    /// 更新Alert
    /// </summary>
    static void updateAlert() {
        if (gameSys.alertRequest != null) {
            alert(gameSys.alertRequest);
            gameSys.clearRequestAlert();
        }
    }

    /// <summary>
    /// 更新Loading
    /// </summary>
    static void updateLoading() {
        if (gameSys.loadingRequest != null) {
            processLoadingRequest(gameSys.loadingRequest);
            gameSys.clearRequestLoad();
        }
    }

    /// <summary>
    /// 更新场景变换（GameSystem的）
    /// </summary>
    static void updateSceneChanging() {
        var req = gameSys.changeScene;
        if (req != null) {
            switch(req.type) {
                case GameSystem.ChangeSceneRequest.Type.Push: pushScene(req.scene); break;
                case GameSystem.ChangeSceneRequest.Type.Goto: gotoScene(req.scene); break;
                case GameSystem.ChangeSceneRequest.Type.Change: changeScene(req.scene); break;
                case GameSystem.ChangeSceneRequest.Type.Pop: popScene(); break;
                case GameSystem.ChangeSceneRequest.Type.Clear: clearScene(); break;
            }
            gameSys.clearChangeScene();
        }
    }


    #endregion

    #region Find, GetComponent的封装

    // 取代 t.GetComponent<T> 的写法：
    // GameUtils.get<T>(obj)
    public static T get<T> (Transform t){
		return t == null ? default : t.GetComponent<T>();
    }
    // 取代 obj.GetComponent<T> 的写法：
    // GameUtils.get<T>(obj)
    public static T get<T> (GameObject obj){
		return obj == null ? default : obj.GetComponent<T>();
    }
    // 取代 parent.Find(obj).GetComponent<T> 的写法：
    // GameUtils.find<T>(obj)
    public static T find<T>(Transform parent, string obj) {
        return get<T>(parent.Find(obj));
    }
    // 取代 parent.Find(obj).GetComponent<T> 的写法：
    // GameUtils.find<T>(obj)
    public static T find<T>(GameObject parent, string obj) {
        return get<T>(parent.transform.Find(obj));
    }
    // 取代 parent.Find(obj).gameObject 的写法：
    // GameUtils.find(obj)
    public static GameObject find(Transform parent, string obj) {
        return parent.Find(obj).gameObject;
    }
    // 取代 parent.transform.Find(obj).gameObject 的写法：
    // GameUtils.find(obj)
    public static GameObject find(GameObject parent, string obj) {
        return parent.transform.Find(obj).gameObject;
    }

    // 取代 GameUtils.get<Text>(t) 的写法：
    // GameUtils.text(t)
    public static Text text(Transform t) {
        return get<Text>(t);
    }
    // 取代 GameUtils.get<Text>(obj) 的写法：
    // GameUtils.text(obj)
    public static Text text(GameObject obj) {
        return get<Text>(obj);
    }
    // 取代 GameUtils.get<Button>(t) 的写法：
    // GameUtils.button(t)
    public static Button button(Transform t) {
        return get<Button>(t);
    }
    // 取代 GameUtils.get<Button>(obj) 的写法：
    // GameUtils.button(obj)
    public static Button button(GameObject obj) {
        return get<Button>(obj);
    }
    // 取代 GameUtils.get<Image>(t) 的写法：
    // GameUtils.image(t)
    public static Image image(Transform t) {
        return get<Image>(t);
    }
    // 取代 GameUtils.get<Image>(obj) 的写法：
    // GameUtils.image(obj)
    public static Image image(GameObject obj) {
        return get<Image>(obj);
    }
    // 取代 GameUtils.get<Animation>(t) 的写法：
    // GameUtils.image(t)
    public static Animation ani(Transform t) {
        return get<Animation>(t);
    }
    // 取代 GameUtils.get<Animation>(obj) 的写法：
    // GameUtils.image(obj)
    public static Animation ani(GameObject obj) {
        return get<Animation>(obj);
    }

    #endregion

    #region RectTransform操作封装
    /// <summary>
    /// 设置RectTransform的宽度
    /// </summary>
    /// <param name="rt">RectTransform实例</param>
    /// <param name="w">要设置的宽度</param>
    public static void setRectWidth(RectTransform rt, float w) {
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
    }
    /// <summary>
    /// 设置RectTransform的高度
    /// </summary>
    /// <param name="rt">RectTransform实例</param>
    /// <param name="h">要设置的高度</param>
    public static void setRectHeight(RectTransform rt, float h) {
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
    }
    #endregion

    #region 时间文本转换封装

    /// <summary>
    /// TimeSpan转字符串（00:00 格式）
    /// </summary>
    /// <param name="span">TimeSpan实例</param>
    /// <returns>转换后字符串</returns>
    public static string time2StrWithDay(TimeSpan span) {
        return String.Format("{0}天{1:00}时{2:00}分{3:00}秒", span.Days, span.Hours, span.Minutes, span.Seconds);
    }

    /// <summary>
    /// TimeSpan转字符串（00:00 格式）
    /// </summary>
    /// <param name="span">TimeSpan实例</param>
    /// <returns>转换后字符串</returns>
    public static string time2Str(TimeSpan span) {
        return String.Format("{0:00}:{1:00}", Math.Floor(span.TotalMinutes), span.Seconds);
    }
    /// <summary>
    /// 秒数转字符串（00:00 格式）
    /// </summary>
    /// <param name="span">TimeSpan实例</param>
    /// <returns>转换后字符串</returns>
    public static string time2Str(int sec) {
        return String.Format("{0:00}:{1:00}", sec / 60, sec % 60);
    }
    public static string time2Str(double sec) {
        sec = Math.Round(sec, 2);
        return String.Format("{0:00}:{1:00.00}", (int)sec / 60, sec % 60);
    }

    /// <summary>
    /// TimeSpan转字符串（00:00:00 格式）
    /// </summary>
    /// <param name="span">TimeSpan实例</param>
    /// <returns>转换后字符串</returns>
    public static string time2StrWithHour(TimeSpan span) {
        return String.Format("{0}:{1:00}:{2:00}", Math.Floor(span.TotalHours), span.Minutes, span.Seconds);
    }
    /// <summary>
    /// 秒数转字符串（00:00:00 格式）
    /// </summary>
    /// <param name="span">TimeSpan实例</param>
    /// <returns>转换后字符串</returns>
    public static string time2StrWithHour(int sec) {
        return string.Format("{0}:{1:00}:{2:00}", sec / 60 / 60, sec / 60 % 60, sec % 60);
    }
    public static string time2StrWithHour(double sec) {
        sec = Math.Round(sec, 2);
        return string.Format("{0}:{1:00}:{2:00}", (int)sec / 60 / 60, 
            (int)sec / 60 % 60, sec % 60);
    }

    #endregion

    #region 浮点数转换操作

    /// <summary>
    /// 浮点数转化为字符串
    /// </summary>
    /// <param name="value">浮点数</param>
    /// <returns>字符串</returns>
    public static string double2Str(double value) {
        return string.Format("{0:0.00}", value);
    }

    /// <summary>
    /// 浮点数转化为百分数
    /// </summary>
    /// <param name="value">浮点数</param>
    /// <returns>百分数字符串</returns>
    public static string double2Perc(double value) {
        return string.Format("{0:0.00}%", value * 100);
    }

    /// <summary>
    /// 浮点数转化为百分数（四舍五入）
    /// </summary>
    /// <param name="value">浮点数</param>
    /// <returns>百分数（四舍五入）字符串</returns>
    public static string double2RoundedPerc(double value) {
        return string.Format("{0:0}%", value * 100);
    }

    #endregion

    #region 颜色转化

    /// <summary>
    /// 十六进制字符串转颜色
    /// </summary>
    /// <param name="str">十六进制字符串，形如“#ABCDEF”</param>
    /// <returns>颜色</returns>
    public static Color str2Color(string str) {
        int index = 0;
        float[] c = { 0, 0, 0, 1 };
        str = str.ToUpperInvariant();
        string reg = @"[0-9A-F][0-9A-F]";
        foreach (Match match in Regex.Matches(str, reg))
            c[index++] = Convert.ToInt32(match.Value, 16) / 255.0f;
        return new Color(c[0], c[1], c[2], c[3]);
    }

    /// <summary>
    /// 十六进制字符串转颜色
    /// </summary>
    /// <param name="str">十六进制字符串，形如“#ABCDEF”</param>
    /// <returns>颜色</returns>
    public static string color2Str(Color c) {
        var r = (int)(c.r * 255);
        var g = (int)(c.g * 255);
        var b = (int)(c.b * 255);
        return string.Format("#{0:X}{1:X}{2:X}", r, g, b);
    }

    #endregion

    #region 其他文本操作

    /// <summary>
    /// 获取JSON解码后文本
    /// </summary>
    /// <param name="oriText">原文本</param>
    /// <returns>解码后文本</returns>
    public static string decodedJsonText(string oriText, int maxLength, bool adjuest=true) {
        var decoded = Regex.Unescape(oriText);
        if (decoded.Length > maxLength)
            decoded = decoded.Substring(0, maxLength) + "...";
        return adjuest ? adjustText(decoded) : decoded;
    }

    /// <summary>
    /// 数值转化为增量数值
    /// </summary>
    /// <param name="value">数值</param>
    /// <returns>增量数值字符串</returns>
    public static string int2Incr(int value) {
        if (value >= 0) return "+" + value;
        return value.ToString();
    }

    /// <summary>
    /// 数值转化为增量数值
    /// </summary>
    /// <param name="value">数值</param>
    /// <returns>增量数值字符串</returns>
    public static string double2Incr(double value) {
        if (value >= 0) return "+" + double2Str(value);
        return double2Str(value);
    }

    /// <summary>
    /// 文本调整（替换掉自动换行的空格符）
    /// </summary>
    /// <param name="text">文本</param>
    /// <returns>调整后的文本</returns>
    public static string adjustText(string text) {
        text = Regex.Replace(text, @"(?<=<.*?) (?=.*?>)", QuestionText.SpaceIdentifier);
        text = text.Replace(" ", QuestionText.SpaceEncode);
        text = text.Replace(QuestionText.SpaceIdentifier, " ");
        return text.ToString();
    }

    #endregion
}
