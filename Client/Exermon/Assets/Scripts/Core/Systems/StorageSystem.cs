
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

using LitJson;

/// <summary>
/// 存储管理器
/// </summary>>
/// <remarks>
/// 名词定义：
///     缓存：缓存的题目数据
///     缓存文件：储存缓存的文件
/// </remarks>
public class StorageSystem : BaseSystem<StorageSystem> {

    /// <summary>
    /// 储存项
    /// </summary>
    public class StorageItem {

        /// <summary>
        /// 属性
        /// </summary>
        public BaseData data;
        public string filename;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="filename">储存文件名</param>
        public StorageItem(BaseData data, string filename) {
            this.data = data; this.filename = filename;
        }

        /// <summary>
        /// 转化为JSON
        /// </summary>
        /// <returns>JSON数据</returns>
        public JsonData toJson() { return data.toJson(); }

        /// <summary>
        /// 从JSON中读取
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="data">读取的数据</param>
        public void load(JsonData json) {
            DataLoader.loadData(ref data, json);
        }

    }

    /// <summary>
    /// 是否需要加密
    /// </summary>
    const bool NeedEncode = true;

    /// <summary>
    /// 路径常量定义
    /// </summary>
    public static readonly string SaveRootPath = Application.persistentDataPath+"/";

    /// <summary>
    /// 缓存文件路径
    /// </summary>
    const string StaticDataFilename = ".static";
    const string CacheDataFilename = ".cache";
    const string ConfigDataFilename = ".config";

    /// <summary>
    /// 加密盐
    /// </summary>
    const string DefaultSalt = "R0cDovMzgwLTE2Lm1pZAl";

    /// <summary>
    /// 储存项
    /// </summary>
    List<StorageItem> storageItems;

    /// <summary>
    /// 外部系统
    /// </summary>
    DataService dataSer;
    GameSystem gameSys;
    ExermonGameSystem exermonSys;
    QuestionService quesSer;

    #region 初始化

    /// <summary>
    /// 初始化外部系统
    /// </summary>
    protected override void initializeSystems() {
        base.initializeSystems();
        gameSys = GameSystem.get();
        exermonSys = ExermonGameSystem.get();
        dataSer = DataService.get();
    }

    /// <summary>
    /// 初始化其他
    /// </summary>
    protected override void initializeOthers() {
        base.initializeOthers();
        storageItems = new List<StorageItem>();
        storageItems.Add(new StorageItem(dataSer.staticData, StaticDataFilename));
        storageItems.Add(new StorageItem(exermonSys.configure, ConfigDataFilename));
        storageItems.Add(new StorageItem(quesSer.questionCache, CacheDataFilename));
    }

    #endregion

    #region 文件操作

    /// <summary>
    /// 储存数据到文件（JSON数据）
    /// </summary>
    /// <param name="json">JSON数据</param>
    /// <param name="path">文件路径</param>
    /// <param name="name">文件名</param>
    public void saveObjectIntoFile(BaseData obj, string path, string name) {
        Debug.Log("Saving " + obj + " into " + path + name);
        saveJsonIntoFile(obj.toJson(), path, name);
    }

    /// <summary>
    /// 储存数据到文件（JSON数据）
    /// </summary>
    /// <param name="json">JSON数据</param>
    /// <param name="path">文件路径</param>
    /// <param name="name">文件名</param>
    public void saveJsonIntoFile(JsonData json, string path, string name) {
        saveDataIntoFile(json.ToJson(), path, name);
    }

    /// <summary>
    /// 存储数据到指定文件
    /// </summary>
    /// <param name="data">数据（任意字符串）</param>
    /// <param name="path">文件路径</param>
    /// <param name="name">文件名</param>
    public void saveDataIntoFile(string data, string path, string name) {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        Debug.Log("saveToFile: " + data);
        StreamWriter streamWriter = new StreamWriter(path + name, false);
        streamWriter.Write(data);
        streamWriter.Close();
        streamWriter.Dispose();
    }

    /// <summary>
    /// 从指定文件读取数据
    /// </summary>
    /// <param name="path">文件路径（包括文件名）</param>
    /// <returns>读取的数据（字符串）</returns>
    public void loadObjectFromFile<T>(ref T data, string path) where T : BaseData, new() {
        Debug.Log("Loading " + data + " from " + path);
        var json = loadJsonFromFile(path);
        DataLoader.loadData(ref data, json);
    }

    /// <summary>
    /// 从指定文件读取数据
    /// </summary>
    /// <param name="path">文件路径（包括文件名）</param>
    /// <returns>读取的数据（字符串）</returns>
    public JsonData loadJsonFromFile(string path) {
        var data = loadDataFromFile(path);
        return JsonMapper.ToObject(data);
    }

    /// <summary>
    /// 从指定文件读取数据
    /// </summary>
    /// <param name="path">文件路径（包括文件名）</param>
    /// <returns>读取的数据（字符串）</returns>
    public string loadDataFromFile(string path) {
        if (!File.Exists(path)) return "";
        StreamReader streamReader = new StreamReader(path);
        string data = streamReader.ReadToEnd();
        Debug.Log("loadFromFile: " + data);
        streamReader.Close();
        streamReader.Dispose();
        return data;
    }

    /// <summary>
    /// 删除指定文件
    /// </summary>
    /// <param name="path">文件路径（包括文件名）</param>
    void deleteFile(string path) {
        if (File.Exists(path)) File.Delete(path);
    }

    #endregion

    #region 储存项

    /// <summary>
    /// 储存储存项
    /// </summary>
    public void save() {
        foreach(var item in storageItems) 
            saveObjectIntoFile(item.data, SaveRootPath, item.filename);
    }

    /// <summary>
    /// 读取储存项
    /// </summary>
    public void load() {
        foreach (var item in storageItems)
            loadObjectFromFile(ref item.data, SaveRootPath+item.filename);
    }

    #endregion

    #region 编码解码

    /// <summary>
    /// base64编码
    /// </summary>
    /// <param name="ori">源字符串</param>
    /// <param name="salt">盐</param>
    /// <returns>编码后字符串</returns>
    public string base64Encode(string ori, string salt = DefaultSalt) {
        byte[] bytes = Encoding.UTF8.GetBytes(ori);
        string code = Convert.ToBase64String(bytes, 0, bytes.Length);
        float pos = UnityEngine.Random.Range(0.1f, 0.9f);
        code = code.Insert((int)(code.Length * pos), salt);
        code = randString(DefaultSalt.Length) + code;
        return code;
    }

    /// <summary>
    /// base64解码
    /// </summary>
    /// <param name="code">编码后字符串</param>
    /// <param name="salt">盐</param>
    /// <returns>源字符串</returns>
    public string base64Decode(string code, string salt = DefaultSalt) {
        code = code.Substring(salt.Length);
        code = code.Replace(salt, "");
        byte[] bytes = Convert.FromBase64String(code);
        return Encoding.UTF8.GetString(bytes);
    }

    /// <summary>
    /// 随机字符串生成
    /// </summary>
    /// <param name="len">长度</param>
    /// <returns>随机字符串</returns>
    string randString(int len) {
        string s = "";
        for (int i = 0; i < len; i++) {
            char c = (char)UnityEngine.Random.Range('A', 'Z');
            s += (UnityEngine.Random.Range(0, 2) >= 1) ? Char.ToLower(c) : c;
        }
        return s;
    }

    #endregion
}
