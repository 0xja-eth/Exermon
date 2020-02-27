using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LitJson;
using System.Reflection;
using System.Linq;

/// <summary>
/// 数据控制器
/// </summary>
public static class DataLoader {

    /// <summary>
    /// 更新时间格式
    /// </summary>
    // 系统内部存储的日期格式
    public const string SystemDateFormat = "yyyy-MM-dd"; 
    public const string SystemDateTimeFormat = "yyyy-MM-dd HH:mm:ss";

    // 显示的日期格式
    public const string DisplayDateFormat = "yyyy 年 MM 月 dd 日";
    public const string DisplayDateTimeFormat = "yyyy 年 MM 月 dd 日 HH:mm:ss";

    /// <summary>
    /// 判断JsonData是否包含键
    /// </summary>
    /// <param name="json">要判断的JsonData</param>
    /// <param name="key">键</param>
    /// <returns>是否包含</returns>
    public static bool contains(JsonData json, string key) {
        bool res = json.Keys.Contains(key);
        if (!res) Debug.LogWarning("Hasn't Key: " + key + " in " + json.ToJson());
        return res;
    }

    #region 加载JsonData
    
    /// <summary>
    /// 加载数据
    /// </summary>
    /// <param name="json">数据</param>
    /// <param name="key">键</param>
    public static JsonData load(JsonData json, string key) {
        if (contains(json, key)) return json[key];
        var data = new JsonData();
        data.SetJsonType(JsonType.Object);
        return data;
    }

    /// <typeparam name="T">读取数据类型</typeparam>
    /// <param name="val">原始值</param>
    /// <param name="ignoreNull">是否忽略空值</param>
    public static T load<T>(T val, JsonData json, string key, bool ignoreNull = false) {
        if (!contains(json, key)) return val; // 如果不存在键，则返回原样
        return (T)load(typeof(T), val, json[key], ignoreNull);
    }
    /// <param name="data">数据</param>
    public static T load<T>(T val, JsonData data, bool ignoreNull = false) {
        if (data == null)
            // 如果值为空且忽略空值，则返回原样
            return ignoreNull ? val : default;
        return (T)load(typeof(T), val, data);
    }
    /// <param name="type">类型</param>
    public static object load(Type type, object val, JsonData json, string key, bool ignoreNull = false) {
        if (!contains(json, key)) return val; // 如果不存在键，则返回原样
        return load(type, val, json[key], ignoreNull);
    }
    public static object load(Type type, object val, JsonData data, bool ignoreNull = false) {
        if (data == null) 
            // 如果值为空且忽略空值，则返回原样
            return ignoreNull ? val : default;
        // 判断特殊类型
        if (type.IsSubclassOf(typeof(BaseData)) || type == typeof(BaseData)) {
            if (val == default) val = Activator.CreateInstance(type);
            ((BaseData)val).load(data);
            return val;
        } else return load(type, data);
    }

    public static T load<T>(JsonData json, string key) {
        if (!contains(json, key)) return default;
        return (T)load(typeof(T), json[key]);
    }
    public static T load<T>(JsonData data) {
        if (data == null) return default;
        return (T)load(typeof(T), data);
    }
    public static object load(Type type, JsonData json, string key) {
        if (!contains(json, key)) return default;
        return load(type, json[key]);
    }
    public static object load(Type type, JsonData data) {
        if (data == null) return default;

        // 处理数组情况
        if (type.IsArray) {
            if (!data.IsArray) return default;
            var cnt = data.Count;
            var eleType = type.GetElementType();
            var res = Array.CreateInstance(eleType, cnt);
            for (var i = 0; i < cnt; ++i)
                res.SetValue(load(eleType, data[i]), i);
            return res;
        }

        // 处理列表情况
        if (type.Name == typeof(List<>).Name) {
            if (!data.IsArray) return default;
            var cnt = data.Count;
            var eleType = type.GetGenericArguments()[0];
            var res = Activator.CreateInstance(type);
            for (var i = 0; i < cnt; ++i)
                type.GetMethod("Add").Invoke(res,
                    new object[] { load(eleType, data[i]) });
            return res;
        }

        // 处理特殊类型
        if (type == typeof(Color)) return loadColor(data);
        if (type == typeof(DateTime)) return loadDateTime(data);
        if (type == typeof(TimeSpan)) return loadTimeSpan(data);
        if (type == typeof(Tuple<int, string>)) return loadTuple(data);
        if (type == typeof(Texture2D)) return loadTexture2D(data);

        if (type.IsSubclassOf(typeof(BaseData))) return loadData(type, data);

        // 处理基本类型
        if (type == typeof(int)) return (int)data;
        if (type == typeof(double)) return (double)data;
        if (type == typeof(float)) return (float)(double)data;
        if (type == typeof(string)) return (string)data;
        if (type == typeof(bool)) return (bool)data;

        // 其他情况下，直接返回即可
        return data;
    }

    #region 具体加载函数

    /// <summary>
    /// 加载颜色
    /// </summary>
    /// <param name="json">数据</param>
    /// <returns>加载的颜色</returns>
    static Color loadColor(JsonData data) {
        return SceneUtils.str2Color((string)data);
    }

    /// <summary>
    /// 加载日期时间
    /// </summary>
    /// <param name="json">数据</param>
    /// <returns>加载的字符串</returns>
    static DateTime loadDateTime(JsonData data) {
        try { return Convert.ToDateTime((string)data); } 
        catch { return default; }
    }

    /// <summary>
    /// 加载TimeSpan
    /// </summary>
    /// <param name="json">数据</param>
    /// <returns>加载的TimeSpan</returns>
    static TimeSpan loadTimeSpan(JsonData data) {
        return new TimeSpan((int)data);
    }

    /// <summary>
    /// 加载(int, string)二元组
    /// </summary>
    /// <param name="json">数据</param>
    /// <returns>加载的二元组</returns>
    static Tuple<int, string> loadTuple(JsonData data) {
        return new Tuple<int, string>((int)data[0], (string)data[1]);
    }

    /// <summary>
    /// 加载纹理
    /// </summary>
    /// <param name="json">数据</param>
    /// <returns>加载的纹理</returns>
    static Texture2D loadTexture2D(JsonData data) {
        byte[] bytes = Convert.FromBase64String((string)data);
        var res = new Texture2D(0, 0);
        res.LoadImage(bytes);
        return res;
    }

    /// <summary>
    /// 加载数据
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="data">数据</param>
    static BaseData loadData(Type type, JsonData data) {
        var res = (BaseData)Activator.CreateInstance(type);
        res.load(data); return res;
    }

    #endregion

    /*
    /// <summary>
    /// 加载整数
    /// </summary>
    /// <param name="json">数据</param>
    /// <returns>加载的整数</returns>
    public static int loadInt(JsonData json) {
        return json == null ? default : (int)json;
    }
    /// <param name="key">键</param>
    public static int loadInt(JsonData json, string key) {
        if (contains(json, key)) return loadInt(json[key]);
        return default;
    }
    /// <param name="key">键</param>
    public static void loadInt(ref int val, JsonData json, string key) {
        if (contains(json, key)) val = loadInt(json[key]);
    }

    /// <summary>
    /// 加载浮点数
    /// </summary>
    /// <param name="json">数据</param>
    /// <returns>加载的浮点数</returns>
    public static float loadFloat(JsonData json) {
        return json == null ? default : (float)(double)json;
    }
    /// <param name="key">键</param>
    public static float loadFloat(JsonData json, string key) {
        if (contains(json, key)) return loadFloat(json[key]);
        return default;
    }
    public static void loadFloat(ref float val, JsonData json, string key) {
        if (contains(json, key)) val = loadFloat(json[key]);
    }

    /// <summary>
    /// 加载浮点数
    /// </summary>
    /// <param name="json">数据</param>
    /// <returns>加载的浮点数</returns>
    public static double loadDouble(JsonData json) {
        return json == null ? default : (double)json;
    }
    /// <param name="key">键</param>
    public static double loadDouble(JsonData json, string key) {
        if (contains(json, key)) return loadDouble(json[key]);
        return default;
    }

    /// <summary>
    /// 加载布尔值
    /// </summary>
    /// <param name="json">数据</param>
    /// <returns>加载的布尔值</returns>
    public static bool loadBool(JsonData json) {
        return json == null ? false : (bool)json;
    }
    /// <param name="key">键</param>
    public static bool loadBool(JsonData json, string key) {
        if (contains(json, key)) return loadBool(json[key]);
        return default;
    }

    /// <summary>
    /// 加载字符串
    /// </summary>
    /// <param name="json">数据</param>
    /// <returns>加载的字符串</returns>
    public static string loadString(JsonData json) {
        return json == null ? "" : (string)json;
    }
    /// <param name="key">键</param>
    public static string loadString(JsonData json, string key) {
        if (contains(json, key)) return loadString(json[key]);
        return default;
    }
    
    /// <summary>
    /// 加载纹理
    /// </summary>
    /// <param name="json">数据</param>
    /// <param name="name">纹理命名</param>
    /// <returns>加载的纹理</returns>
    public static Texture2D loadTexture2D(JsonData json, string name = "") {
        var str = loadString(json);
        byte[] bytes = Convert.FromBase64String(str);
        var res = new Texture2D(0, 0);
        res.LoadImage(bytes);
        res.name = name;
        return res;
    }
    /// <param name="key">键</param>
    public static Texture2D loadTexture2D(JsonData json, string key, string name = "") {
        if (contains(json, key)) return loadTexture2D(json[key], name);
        return default;
    }

    /// <param name="key">键</param>
    public static Color loadColor(JsonData json, string key) {
        if (contains(json, key)) return loadColor(json[key]);
        return default;
    }
    /// <param name="key">键</param>
    public static DateTime loadDateTime(JsonData json, string key) {
        if (contains(json, key)) return loadDateTime(json[key]);
        return default;
    }
    /// <param name="key">键</param>
    public static TimeSpan loadTimeSpan(JsonData json, string key) {
        if (contains(json, key)) return loadTimeSpan(json[key]);
        return default;
    }
    /// <param name="key">键</param>
    public static Tuple<int, string> loadTuple(JsonData json, string key) {
        if (contains(json, key)) return loadTuple(json[key]);
        return default;
    }
    /// <param name="key">键</param>
    public static int[] loadIntArray(JsonData json, string key) {
        if (contains(json, key)) return loadIntArray(json[key]);
        return default; // null
    }
    /// <param name="key">键</param>
    public static int[][] loadInt2DArray(JsonData json, string key) {
        if (contains(json, key)) return loadInt2DArray(json[key]);
        return default; // null
    }
    /// <param name="key">键</param>
    public static string[] loadStringArray(JsonData json, string key) {
        if (contains(json, key)) return loadStringArray(json[key]);
        return default; // null
    }

    /// <summary>
    /// 加载纹理数组
    /// </summary>
    /// <param name="json">数据</param>
    /// <param name="name">纹理命名</param>
    /// <returns>加载的纹理</returns>
    public static Texture2D[] loadTexture2DArray(JsonData json, string name = "") {
        var cnt = json.Count; Texture2D[] res = new Texture2D[cnt];
        for (int i = 0; i < cnt; i++) res[i] = loadTexture2D(json[i], name + "-" + i);
        return res;
    }
    /// <param name="key">键</param>
    public static Texture2D[] loadTexture2DArray(JsonData json, string key, string name = "") {
        if (contains(json, key)) return loadTexture2DArray(json[key], name);
        return default;
    }

    /// <summary>
    /// 加载(int, string)二元组数组
    /// </summary>
    /// <param name="json">数据</param>
    /// <returns>加载的二元组数组</returns>
    public static Tuple<int, string>[] loadTupleArray(JsonData json) {
        var cnt = json.Count; Tuple<int, string>[] res = new Tuple<int, string>[cnt];
        for (int i = 0; i < cnt; i++) res[i] = loadTuple(json[i]);
        return res;
    }
    /// <param name="key">键</param>
    public static Tuple<int, string>[] loadTupleArray(JsonData json, string key) {
        if (contains(json, key)) return loadTupleArray(json[key]);
        return default; // null
    }
    
    /// <summary>
    /// 加载数据
    /// </summary>
    /// <typeparam name="T">要加载的类型</typeparam>
    /// <param name="data">数据容器</param>
    /// <param name="json">数据</param>
    public static T loadData<T>(JsonData json) where T : BaseData, new() {
        if (json == null) return null;
        T data = new T(); data.load(json); return data;
    }
    /// <param name="key">键</param>
    public static T loadData<T>(JsonData json, string key) where T : BaseData, new() {
        if (contains(json, key)) return loadData<T>(json[key]);
        else return default;
    }
    /// <remarks>传引用模式</remarks>
    public static void loadData<T>(ref T data, JsonData json) where T : BaseData, new() {
        if (json == null) { data = null; return; }
        if (data == null) data = new T();
        data.load(json);
    }
    public static void loadData<T>(ref T data, JsonData json, string key) where T : BaseData, new() {
        if (contains(json, key)) loadData(ref data, json[key]);
        else data = default;
    }
    

    /// <summary>
    /// 加载数据数组
    /// </summary>
    /// <typeparam name="T">要加载的类型</typeparam>
    /// <param name="data">数据容器</param>
    /// <param name="json">数据</param>
    public static T[] loadDataArray<T>(JsonData json) where T : BaseData, new() {
        var cnt = json.Count;
        T[] data = new T[cnt];
        for (int i = 0; i < cnt; i++) {
            data[i] = new T(); data[i].load(json[i]);
        }
        return data;
    }
    /// <param name="key">键</param>
    public static T[] loadDataArray<T>(JsonData json, string key) where T : BaseData, new() {
        if (contains(json, key)) return loadDataArray<T>(json[key]);
        else return new T[0];
    }
    /// <remarks>传引用模式</remarks>
    public static void loadDataArray<T>(ref T[] data, JsonData json) where T : BaseData, new() {
        var cnt = json.Count; data = new T[cnt];
        for (int i = 0; i < cnt; i++) {
            data[i] = new T(); data[i].load(json[i]);
        }
    }
    public static void loadDataArray<T>(ref T[] data, JsonData json, string key) where T : BaseData, new() {
        if (contains(json, key)) loadDataArray(ref data, json[key]);
        else data = new T[0];
    }

    /// <summary>
    /// 加载数据数组
    /// </summary>
    /// <typeparam name="T">要加载的类型</typeparam>
    /// <param name="data">数据容器（列表）</param>
    /// <param name="json">数据</param>
    public static List<T> loadDataList<T>(JsonData json) where T : BaseData, new() {
        var cnt = json.Count;
        List<T> data = new List<T>();
        for (int i = 0; i < cnt; i++) {
            T item = new T(); item.load(json[i]);
            data.Add(item);
        }
        return data;
    }
    /// <param name="key">键</param>
    public static List<T> loadDataList<T>(JsonData json, string key) where T : BaseData, new() {
        if (contains(json, key)) return loadDataList<T>(json[key]);
        else return new List<T>();
    }
    /// <remarks>传引用模式</remarks>
    public static void loadDataList<T>(ref List<T> data, JsonData json) where T : BaseData, new() {
        data = new List<T>();
        for (int i = 0; i < json.Count; i++) {
            T item = new T(); item.load(json[i]);
            data.Add(item);
        }
    }
    public static void loadDataList<T>(ref List<T> data, JsonData json, string key) where T : BaseData, new() {
        if (contains(json, key)) loadDataList(ref data, json[key]);
        else data = new List<T>();
    }
    */

    #endregion

    #region 转化为JsonData

    /// <summary>
    /// 转化为JsonData
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="data">数据</param>
    /// <param name="format">转化格式</param>
    /// <returns>JsonData</returns>
    public static JsonData convert<T>(T data, string format = "") {
        return convert(typeof(T), data, format);
    }
    /// <param name="type">类型</param>
    public static JsonData convert(Type type, object data, string format = "") {
        format = format.ToLower();
        bool isArray = false;
        Type eleType = null;

        // 处理数组情况
        if (isArray = type.IsArray) 
            eleType = type.GetElementType();
        if (type.Name == typeof(List<>).Name) 
            eleType = type.GetGenericArguments()[0];
        if (isArray) {
            var array = (IEnumerable)data;
            var json = new JsonData();
            json.SetJsonType(JsonType.Array);
            foreach (var d in array)
                json.Add(convert(eleType, d, format));
            return json;
        }

        // 处理特殊类型
        if (type == typeof(Color)) return convertColor((Color)data);
        if (type == typeof(DateTime)) return convertDateTime((DateTime)data);
        if (type == typeof(DateTime) && format == "date") return convertDate((DateTime)data);
        if (type == typeof(Tuple<int, string>)) return convertTuple((Tuple<int, string>)data);
        if (type == typeof(Texture2D)) return convertTexture2D(data as Texture2D);

        if (type.IsSubclassOf(typeof(BaseData))) return convertData(data as BaseData);

        // 处理基本类型
        if (type == typeof(int)) return (int)data;
        if (type == typeof(double)) return (double)data;
        if (type == typeof(float)) return (double)data;
        if (type == typeof(string)) return (string)data;
        if (type == typeof(bool)) return (bool)data;

        // 其他情况下，直接返回即可
        return data as JsonData;
    }

    #region 具体转化函数

    /// <summary>
    /// 加载颜色
    /// </summary>
    /// <param name="json">数据</param>
    /// <returns>加载的颜色</returns>
    static JsonData convertColor(Color c) {
        return SceneUtils.color2Str(c);
    }

    /// <summary>
    /// 转化日期时间
    /// </summary>
    /// <param name="data">日期时间</param>
    /// <returns>转化后的JsonData</returns>
    static JsonData convertDateTime(DateTime data) {
        if (data == null) return "";
        return data.ToString(SystemDateTimeFormat);
    }

    /// <summary>
    /// 转化日期时间
    /// </summary>
    /// <param name="data">日期时间</param>
    /// <returns>转化后的JsonData</returns>
    static JsonData convertDate(DateTime data) {
        if (data == null) return "";
        return data.ToString(SystemDateFormat);
    }

    /// <summary>
    /// 转化(int, string)二元组
    /// </summary>
    /// <param name="data">二元组数据</param>
    /// <returns>转化后的JsonData</returns>
    static JsonData convertTuple(Tuple<int, string> data) {
        var json = new JsonData();
        json.SetJsonType(JsonType.Array);
        if (data == null) return json;
        json.Add(data.Item1); json.Add(data.Item2);
        return json;
    }

    /// <summary>
    /// 转化纹理
    /// </summary>
    /// <param name="data">纹理</param>
    /// <returns>转化后的JsonData</returns>
    static JsonData convertTexture2D(Texture2D data) {
        if (data == null) return "";
        byte[] bytes = data.EncodeToPNG();
        return Convert.ToBase64String(bytes, 0, bytes.Length);
    }

    /// <summary>
    /// 转化数据
    /// </summary>
    /// <typeparam name="T">要转化的类型</typeparam>
    /// <param name="data">数据</param>
    public static JsonData convertData(BaseData data) {
        if (data != null) return data.toJson();
        var json = new JsonData();
        json.SetJsonType(JsonType.Object);
        return json;
    }

    #endregion

    ///// <summary>
    ///// 转化泛型数组
    ///// </summary>
    ///// <param name="data">整型数组</param>
    ///// <returns>转化后的JsonData</returns>
    //public static JsonData convert<T>(T[] data) {
    //    var json = new JsonData();
    //    json.SetJsonType(JsonType.Array);
    //    foreach (var d in data) json.Add(d);
    //    return json;
    //}

    ///// <summary>
    ///// 转化泛型2D数组
    ///// </summary>
    ///// <param name="data">2D整型数组</param>
    ///// <returns>转化后的JsonData</returns>
    //public static JsonData convert2DArray<T>(T[][] data) {
    //    var json = new JsonData();
    //    json.SetJsonType(JsonType.Array);
    //    foreach (var d in data) json.Add(convert(d));
    //    return json;
    //}

    ///// <summary>
    ///// 转化泛型3D数组
    ///// </summary>
    ///// <param name="data">3D整型数组</param>
    ///// <returns>转化后的JsonData</returns>
    //static JsonData convert3DArray<T>(T[][][] data) {
    //    var json = new JsonData();
    //    json.SetJsonType(JsonType.Array);
    //    foreach (var d in data) json.Add(convert2DArray(d));
    //    return json;
    //}

    ///// <summary>
    ///// 转化纹理数组
    ///// </summary>
    ///// <param name="data">纹理数组</param>
    ///// <returns>转化后的JsonData</returns>
    //public static JsonData convertTexture2DArray(Texture2D[] data) {
    //    var json = new JsonData();
    //    json.SetJsonType(JsonType.Array);
    //    if (data == null) return json;
    //    foreach (var d in data) json.Add(convertTexture2D(d));
    //    return json;
    //}

    ///// <summary>
    ///// 转化(int, string)二元组数组
    ///// </summary>
    ///// <param name="data">二元组数组</param>
    ///// <returns>转化后的JsonData</returns>
    //public static JsonData convertTupleArray(Tuple<int, string>[] data) {
    //    var json = new JsonData();
    //    json.SetJsonType(JsonType.Array);
    //    if (data == null) return json;
    //    foreach (var d in data) json.Add(convertTuple(d));
    //    return json;
    //}

    ///// <summary>
    ///// 转化数据数组
    ///// </summary>
    ///// <typeparam name="T">要转化的类型</typeparam>
    ///// <param name="data">数据数组</param>
    //public static JsonData convertDataArray<T>(T[] data) where T : BaseData, new() {
    //    var json = new JsonData();
    //    json.SetJsonType(JsonType.Array);
    //    if (data == null) return json;
    //    foreach (var d in data) json.Add(convertData(d));
    //    return json;
    //}

    ///// <summary>
    ///// 转化数据数组
    ///// </summary>
    ///// <typeparam name="T">要转化的类型</typeparam>
    ///// <param name="data">数据列表</param>
    //public static JsonData convertDataArray<T>(List<T> data) where T : BaseData, new() {
    //    var json = new JsonData();
    //    json.SetJsonType(JsonType.Array);
    //    if (data == null) return json;
    //    foreach (var d in data) json.Add(convertData(d));
    //    return json;
    //}

    #endregion

    #region 其他工具

    /// <summary>
    /// 下划线命名法转化为小驼峰命名法
    /// </summary>
    /// <param name="str">待转化字符串</param>
    /// <param name="spliter">下划线字符</param>
    /// <returns>转化结果</returns>
    public static string underline2LowerHump(string str, char spliter = '_') {
        string res = ""; bool flag = false;
        for (int i = 0; i < str.Length; i++) {
            if (str[i] == spliter) flag = true;
            else if (flag) {
                res += char.ToUpper(str[i]);
                flag = false;
            } else res += str[i];
        }
        return res;
    }

    /// <summary>
    /// 下划线命名法转化为大驼峰命名法
    /// </summary>
    /// <param name="str">待转化字符串</param>
    /// <param name="spliter">下划线字符</param>
    /// <returns>转化结果</returns>
    public static string underline2UpperHump(string str, char spliter = '_') {
        string res = ""; bool flag = true;
        for (int i = 0; i < str.Length; i++) {
            if (str[i] == spliter) flag = true;
            else if (flag) {
                res += char.ToUpper(str[i]);
                flag = false;
            } else res += str[i];
        }
        return res;
    }

    /// <summary>
    /// 驼峰命名法转化为下划线命名法
    /// </summary>
    /// <param name="str">待转化字符串</param>
    /// <param name="spliter">下划线字符</param>
    /// <returns>转化结果</returns>
    public static string hump2Underline(string str, char spliter = '_') {
        string res = ""; 
        for (int i = 0; i < str.Length; i++) {
            if (i > 0 && char.IsUpper(str[i])) {
                res += spliter;
                res += char.ToLower(str[i]);
            } else res += str[i];
        }
        return res;
    }

    #endregion
}


