using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using Core.Systems;

using GameModule.Services;

using UI.Common.Windows;
using UI.Common.Controls.SystemExtend.QuestionText;

namespace Core.UI.Utils {

    /// <summary>
    /// 场景工具类
    /// </summary>
    public static class SceneUtils {

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
        /// 场景物体托管
        /// </summary>
        static Dictionary<string, UnityEngine.Object> sceneObjectDeposit =
            new Dictionary<string, UnityEngine.Object>();

        /// <summary>
        /// 外部系统
        /// </summary>
        static GameSystem gameSys = null;
        static SceneSystem sceneSys = null;
        static GameService gameSer = null;

        /// <summary>
        /// 初始化界面工具
        /// </summary>
        /// <param name="scene">当前场景</param>
        /// <param name="alertWindow">当前场景的提示弹窗</param>
        /// <param name="loadingWindow">当前场景的加载窗口</param>
        public static void initialize(string scene,
            AlertWindow alertWindow = null, LoadingWindow loadingWindow = null) {
            Debug.Log("initialize Scene: " + scene);
            initializeSystems();
            initializeScene(scene, alertWindow, loadingWindow);
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        static void initializeSystems() {
            if (gameSys == null) gameSys = GameSystem.get();
            if (sceneSys == null) sceneSys = SceneSystem.get();
            if (gameSer == null) gameSer = GameService.get();
        }

        /// <summary>
        /// 初始化场景
        /// </summary>
        /// <param name="scene">当前场景</param>
        /// <param name="alertWindow">当前场景的提示弹窗</param>
        /// <param name="loadingWindow">当前场景的加载窗口</param>
        static void initializeScene(string scene,
            AlertWindow alertWindow = null, 
            LoadingWindow loadingWindow = null) {
            if (sceneSys.currentScene() != scene)
                sceneSys.gotoScene(scene);
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
            Debug.Log("depositSceneObject: " + key + ", " + obj);
            if (sceneObjectDeposit.ContainsKey(key))
                sceneObjectDeposit[key] = obj;
            else
                sceneObjectDeposit.Add(key, obj);
            return obj;
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
        /// 设置加载进度
        /// </summary>
        /// <param name="tips">加载界面文本</param>
        static void setupLoadingProgress(double progress = -1) {
            if (loadingWindow) loadingWindow.setProgress(progress);
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
            if (req.start)
                if (req.setProgress)
                    setupLoadingProgress(req.progress);
                else startLoadingWindow(req.text);
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
            updateAlert();
            updateLoading();
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

        #endregion

        #region Find, GetComponent的封装

        /// <summary>
        /// 获取组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="t">变换对象</param>
        /// <remarks>
        /// 取代 t.GetComponent<T> 的写法：SceneUtils.get<T>(obj)
        /// </remarks>
        /// <returns>返回获取的组件</returns>
        public static T get<T>(Transform t) {
            return t == null ? default : t.GetComponent<T>();
        }
        /// <summary>
        /// 获取组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="obj">物体对象</param>
        /// <remarks>
        /// 取代 obj.GetComponent<T> 的写法：SceneUtils.get<T>(obj)
        /// </remarks>
        /// <returns>返回获取的组件</returns>
        public static T get<T>(GameObject obj) {
            return obj == null ? default : obj.GetComponent<T>();
        }
        /// <summary>
        /// 获取子物体下的组件
        /// </summary>
        /// <remarks>
        /// 取代 parent.Find(obj).GetComponent<T> 的写法：
        /// SceneUtils.find<T>(obj)
        /// </remarks>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="parent">父物体变换对象</param>
        /// <param name="path">寻找路径</param>
        /// <returns>返回查找到的组件</returns>
        public static T find<T>(Transform parent, string path) {
            return get<T>(parent.Find(path));
        }
        /// <summary>
        /// 获取子物体下的组件
        /// </summary>
        /// <remarks>
        /// 取代 parent.Find(obj).GetComponent<T> 的写法：
        /// SceneUtils.find<T>(obj)
        /// </remarks>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="parent">父物体对象</param>
        /// <param name="path">寻找路径</param>
        /// <returns>返回查找到的组件</returns>
        public static T find<T>(GameObject parent, string obj) {
            return get<T>(parent.transform.Find(obj));
        }
        /// <summary>
        /// 获取子物体下的 GameObject
        /// </summary>
        /// <remarks>
        /// 取代 parent.Find(obj).gameObject 的写法：
        /// SceneUtils.find(obj)
        /// </remarks>
        /// <param name="parent">父物体对象</param>
        /// <param name="path">寻找路径</param>
        /// <returns>返回查找到的物体</returns>
        public static GameObject find(Transform parent, string obj) {
            return parent.Find(obj).gameObject;
        }
        /// <summary>
        /// 获取子物体下的 GameObject
        /// </summary>
        /// <remarks>
        /// 取代 parent.transform.Find(obj).gameObject 的写法：
        /// SceneUtils.find(obj)
        /// </remarks>
        /// <param name="parent">父物体对象</param>
        /// <param name="path">寻找路径</param>
        /// <returns>返回查找到的物体</returns>
        public static GameObject find(GameObject parent, string obj) {
            return parent.transform.Find(obj).gameObject;
        }

        /// <summary>
        /// 快速获取 Text 组件
        /// </summary>
        /// <param name="t">物体变换对象</param>
        /// <returns>返回该对象的 Text 组件</returns>
        public static Text text(Transform t) {
            return get<Text>(t);
        }
        /// <summary>
        /// 快速获取 Text 组件
        /// </summary>
        /// <param name="obj">物体对象</param>
        /// <returns>返回该对象的 Text 组件</returns>
        public static Text text(GameObject obj) {
            return get<Text>(obj);
        }
        /// <summary>
        /// 快速获取 Button 组件
        /// </summary>
        /// <param name="t">物体变换对象</param>
        /// <returns>返回该对象的 Button 组件</returns>
        public static Button button(Transform t) {
            return get<Button>(t);
        }
        /// <summary>
        /// 快速获取 Button 组件
        /// </summary>
        /// <param name="obj">物体对象</param>
        /// <returns>返回该对象的 Button 组件</returns>
        public static Button button(GameObject obj) {
            return get<Button>(obj);
        }
        /// <summary>
        /// 快速获取 Image 组件
        /// </summary>
        /// <param name="t">物体变换对象</param>
        /// <returns>返回该对象的 Image 组件</returns>
        public static Image image(Transform t) {
            return get<Image>(t);
        }
        /// <summary>
        /// 快速获取 Image 组件
        /// </summary>
        /// <param name="obj">物体对象</param>
        /// <returns>返回该对象的 Image 组件</returns>
        public static Image image(GameObject obj) {
            return get<Image>(obj);
        }
        /// <summary>
        /// 快速获取 Animation 组件
        /// </summary>
        /// <param name="t">物体变换对象</param>
        /// <returns>返回该对象的 Animation 组件</returns>
        public static Animation ani(Transform t) {
            return get<Animation>(t);
        }
        /// <summary>
        /// 快速获取 Animation 组件
        /// </summary>
        /// <param name="obj">物体对象</param>
        /// <returns>返回该对象的 Animation 组件</returns>
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
            return string.Format("{0}天{1:00}时{2:00}分{3:00}秒", span.Days, span.Hours, span.Minutes, span.Seconds);
        }

        /// <summary>
        /// TimeSpan转字符串（00:00 格式）
        /// </summary>
        /// <param name="span">TimeSpan实例</param>
        /// <returns>转换后字符串</returns>
        public static string time2Str(TimeSpan span) {
            return string.Format("{0:00}:{1:00}", Math.Floor(span.TotalMinutes), span.Seconds);
        }
        /// <summary>
        /// 秒数转字符串（00:00 格式）
        /// </summary>
        /// <param name="span">TimeSpan实例</param>
        /// <returns>转换后字符串</returns>
        public static string time2Str(int sec) {
            return string.Format("{0:00}:{1:00}", sec / 60, sec % 60);
        }
        public static string time2Str(double sec) {
            sec = Math.Round(sec, 2);
            return string.Format("{0:00}:{1:00.00}", (int)sec / 60, sec % 60);
        }

        /// <summary>
        /// TimeSpan转字符串（00:00:00 格式）
        /// </summary>
        /// <param name="span">TimeSpan实例</param>
        /// <returns>转换后字符串</returns>
        public static string time2StrWithHour(TimeSpan span) {
            return string.Format("{0}:{1:00}:{2:00}", Math.Floor(span.TotalHours), span.Minutes, span.Seconds);
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
        public static string double2Str(double value, bool intAdj = false) {
            if (intAdj && value == (int)value)
                return string.Format("{0}", (int)value);
            return string.Format("{0:0.00}", value);
        }

        /// <summary>
        /// 浮点数转化为百分数
        /// </summary>
        /// <param name="value">浮点数</param>
        /// <returns>百分数字符串</returns>
        public static string double2Perc(double value, bool intAdj = false) {
            if (intAdj && value == (int)value)
                return string.Format("{0}%", (int)value * 100);
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
        public static string decodedJsonText(string oriText, int maxLength, bool adjuest = true) {
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
}