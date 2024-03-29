﻿using System;
using System.Collections.Generic;

using UnityEngine;

using LitJson;
using System.Reflection;

using Core.Data.Loaders;

using GameModule.Data;
using GameModule.Services;

namespace Core.Data {

    /// <summary>
    /// 可自动转化的属性特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoConvertAttribute : Attribute {

        /// <summary>
        /// 键名
        /// </summary>
        public string keyName;

        /// <summary>
        /// 防止覆盖
        /// </summary>
        public bool preventCover;

        /// <summary>
        /// 忽略空值
        /// </summary>
        public bool ignoreNull;

        /// <summary>
        /// 自动读取
        /// </summary>
        public bool autoLoad;

        /// <summary>
        /// 自动转化
        /// </summary>
        public bool autoConvert;

        /// <summary>
        /// 转换格式
        /// </summary>
        public string format;

        /// <summary>
        /// 构造函数
        /// </summary>
        public AutoConvertAttribute(string keyName = null,
            bool autoLoad = true, bool autoConvert = true,
            bool preventCover = true, bool ignoreNull = false, string format = "") {
            this.keyName = keyName;
            this.preventCover = preventCover;
            this.ignoreNull = ignoreNull;
            this.format = format;
            this.autoLoad = autoLoad;
            this.autoConvert = autoConvert;
        }
    }

    /// <summary>
    /// 游戏数据父类
    /// </summary>
    public class BaseData {

		#region 缓存池操作

		/// <summary>
		/// 对象缓存池
		/// </summary>
		public static Dictionary<Type, List<BaseData>> objects = new Dictionary<Type, List<BaseData>>();

		/// <summary>
		/// 添加对象到缓存池
		/// </summary>
		/// <param name="type"></param>
		/// <param name="data"></param>
		public static void poolAdd(Type type, BaseData data) {
			if (!poolContains(type))
				objects[type] = new List<BaseData>();
			objects[type].Add(data);
		}

		/// <summary>
		/// 缓存池是否存在指定类型
		/// </summary>
		static bool poolContains(Type type) {
			return objects.ContainsKey(type);
		}
		static bool poolContains<T>() where T: BaseData {
			return poolContains(typeof(T));
		}

		/// <summary>
		/// 缓存池对象数量
		/// </summary>
		static int poolCount(Type type) {
			if (!poolContains(type)) return 0;
			return objects[type].Count;
		}
		static int poolCount<T>() where T : BaseData {
			return poolCount(typeof(T));
		}

		/// <summary>
		/// 获取缓存池指定条件对象
		/// </summary>
		public static List<BaseData> poolGet(Type type) {
			if (!poolContains(type)) return new List<BaseData>();
			return objects[type];
		}
		public static BaseData poolGet(Type type, int id) {
			if (!poolContains(type)) return null;
			return objects[type].Find(data => data.id == id);
		}
		public static List<T> poolGet<T>() where T : BaseData {
			var type = typeof(T);
			var list = new List<T>();
			var objects = poolGet(type);

			foreach (var object_ in objects)
				list.Add(object_ as T);

			return list;
		}
		public static T poolGet<T>(int id) where T : BaseData {
			return poolGet(typeof(T), id) as T;
		}
		public static T poolGet<T>(Predicate<T> p) where T : BaseData {
			return poolGet<T>().Find(p);
		}

		/// <summary>
		/// 添加自己到缓存池
		/// </summary>
		protected void addToPool() {
			poolAdd(GetType(), this);
		}

		/// <summary>
		/// 当前ID
		/// </summary>
		protected int currentId() {
			return poolCount(GetType()) + 1;
		}

		/// <summary>
		/// 是否使用缓存池
		/// </summary>
		protected virtual bool useObjectsPool() { return false; }

		/// <summary>
		/// 是否自动分配ID
		/// </summary>
		protected virtual bool autoId() { return false; }

		#endregion

		/// <summary>
		/// 属性
		/// </summary>
		//int id; // ID（只读）

		public int id { get; protected set; }

        //public int id { return id; }

        /// <summary>
        /// 是否为复制对象（复制对象无法进行复制）
        /// </summary>
        bool copied = false;

        /// <summary>
        /// 原始数据
        /// </summary>
        JsonData rawData;

        /// <summary>
        /// 是否需要ID
        /// </summary>
        protected virtual bool idEnable() { return true; }

        /// <summary>
        /// 获取JSON数据
        /// </summary>
        /// <returns>JsonData</returns>
        public virtual BaseData copy(bool flag = true) {
            if (copied) return null; // 如果为复制对象，无法继续复制
            var res = (BaseData)DataLoader.load(GetType(), toJson());
            if (flag) res.copied = true;
            return res;
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        /// <param name="json">数据</param>
        public void load(JsonData json) {
            rawData = json;
            //Debug.Log("load: " + json.ToJson());
            loadAutoAttributes(json);
            loadCustomAttributes(json);
        }

        /// <summary>
        /// 读取自定义属性
        /// </summary>
        /// <param name="json"></param>
        protected virtual void loadCustomAttributes(JsonData json) {
            id = idEnable() ? DataLoader.load(id, json, "id") : -1;
        }

        /// <summary>
        /// 读取自动转换属性
        /// </summary>
        void loadAutoAttributes(JsonData json) {
            var type = GetType();

            foreach (var p in type.GetProperties())
                foreach (Attribute a in p.GetCustomAttributes(false))
                    if (a.GetType() == typeof(AutoConvertAttribute)) {
                        var attr = (AutoConvertAttribute)a;
                        if (!attr.autoLoad) continue;

                        var pType = p.PropertyType; var pName = p.Name;
                        var key = attr.keyName ?? DataLoader.hump2Underline(pName);
                        var val = p.GetValue(this);
                        /*
                        var debug = string.Format("Loading {0} {1} {2} in {3} " +
                            "(ori:{4})", p, pType, pName, type, val);
                        Debug.Log(debug);
                        */
                        val = attr.preventCover ? DataLoader.load(
                            pType, val, json, key, attr.ignoreNull) :
                            DataLoader.load(pType, json, key);
                        p.SetValue(this, val, BindingFlags.Public | BindingFlags.NonPublic, null, null, null);
                    }
        }

        /// <summary>
        /// 获取JSON数据
        /// </summary>
        /// <returns>JsonData</returns>
        public JsonData toJson() {
            var json = new JsonData();
            json.SetJsonType(JsonType.Object);
            convertAutoAttributes(ref json);
            convertCustomAttributes(ref json);
            return json;
        }

        /// <summary>
        /// 转换自定义属性
        /// </summary>
        /// <param name="json"></param>
        protected virtual void convertCustomAttributes(ref JsonData json) {
            if (idEnable()) json["id"] = id;
        }

        /// <summary>
        /// 转换自动转换属性
        /// </summary>
        void convertAutoAttributes(ref JsonData json) {
            var type = GetType();

            foreach (var p in type.GetProperties())
                foreach (Attribute a in p.GetCustomAttributes(false))
                    if (a.GetType() == typeof(AutoConvertAttribute)) {
                        var attr = (AutoConvertAttribute)a;
                        if (!attr.autoConvert) continue;

                        var pType = p.PropertyType; var pName = p.Name;
                        var key = attr.keyName ?? DataLoader.hump2Underline(pName);
                        var val = p.GetValue(this);

                        json[key] = DataLoader.convert(pType, val, attr.format);
                        /*
                        var debug = string.Format("Converting {0} {1} in {2} (val:{3}) " +
                            "to key: {4}, res: {5}", pType, pName, type, val, key, json[key]);
                        Debug.Log(debug);
                        */
                    }
        }

        /// <summary>
        /// 获取原始数据
        /// </summary>
        /// <returns>JSON字符串</returns>
        public string rawJson() { return rawData.ToJson(); }

        /// <summary>
        /// 类型转化（需要读取时候执行，因为 rawData 不会同步改变）
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>目标类型对象</returns>
        public T convert<T>() where T : BaseData, new() {
            return DataLoader.load<T>(rawData);
        }

		/// <summary>
		/// 构造函数
		/// </summary>
		public BaseData() {
			if (useObjectsPool()) {
				if (autoId()) id = currentId();
				addToPool();
			}
		}

    }

    /// <summary>
    /// 配置数据组父类
    /// </summary>
    public class TypeData : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public string name { get; protected set; }
        [AutoConvert]
        public string description { get; protected set; }
    }

    /// <summary>
    /// 属性数据
    /// </summary>
    public class ParamData : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int paramId { get; protected set; }
        [AutoConvert]
        public double value { get; protected set; } // 真实值

        /// <summary>
        /// 是否需要ID
        /// </summary>
        protected override bool idEnable() { return false; }

        #region 数据操作

        /// <summary>
        /// 获取基本属性
        /// </summary>
        /// <returns>基本属性</returns>
        public BaseParam param() {
            return DataService.get().baseParam(paramId);
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="val">值</param>
        public void setValue(double val) {
            value = isDouble() ? val : Math.Round(val);
        }
        public void setValue(ParamData val) {
            setValue(val.value);
        }

        /// <summary>
        /// 是否小数
        /// </summary>
        /// <returns></returns>
        public virtual bool isDouble() {
            return param().isPercent();
        }

        /// <summary>
        /// 增加值
        /// </summary>
        /// <param name="val">值</param>
        public void addValue(double val) {
            value += val;
        }
        public static ParamData operator +(ParamData a, ParamData b) {
            var res = new ParamData(a.paramId, a.value);
            res.addValue(b.value);
            return res;
        }
        public static ParamData operator +(ParamData a, double b) {
            var res = new ParamData(a.paramId, a.value);
            res.addValue(b);
            return res;
        }
        public static ParamData operator -(ParamData a, ParamData b) {
            var res = new ParamData(a.paramId, a.value);
            res.addValue(-b.value);
            return res;
        }
        public static ParamData operator -(ParamData a, double b) {
            var res = new ParamData(a.paramId, a.value);
            res.addValue(-b);
            return res;
        }

        /// <summary>
        /// 乘值
        /// </summary>
        /// <param name="val">值</param>
        public void timesValue(double val) {
            value *= val;
        }

        public static ParamData operator *(ParamData a, ParamData b) {
            var res = new ParamData(a.paramId, a.value);
            res.timesValue(b.value);
            return res;
        }
        public static ParamData operator *(ParamData a, double b) {
            var res = new ParamData(a.paramId, a.value);
            res.timesValue(b);
            return res;
        }
        public static ParamData operator /(ParamData a, ParamData b) {
            var res = new ParamData(a.paramId, a.value);
            res.timesValue(1 / b.value);
            return res;
        }
        public static ParamData operator /(ParamData a, double b) {
            var res = new ParamData(a.paramId, a.value);
            res.timesValue(1 / b);
            return res;
        }

        /// <summary>
        /// 比较
        /// </summary>
        public static bool operator ==(ParamData a, ParamData b) {
            return a.paramId == b.paramId && a.value == b.value;
        }
        public static bool operator ==(ParamData a, double b) {
            return a.value == b;
        }
        public static bool operator !=(ParamData a, ParamData b) {
            return a.paramId != b.paramId || a.value != b.value;
        }
        public static bool operator !=(ParamData a, double b) {
            return a.value != b;
        }
        public static bool operator >(ParamData a, ParamData b) {
            return a.paramId == b.paramId && a.value > b.value;
        }
        public static bool operator >(ParamData a, double b) {
            return a.value > b;
        }
        public static bool operator >=(ParamData a, ParamData b) {
            return a.paramId == b.paramId && a.value >= b.value;
        }
        public static bool operator >=(ParamData a, double b) {
            return a.value >= b;
        }
        public static bool operator <(ParamData a, ParamData b) {
            return a.paramId == b.paramId && a.value < b.value;
        }
        public static bool operator <(ParamData a, double b) {
            return a.value < b;
        }
        public static bool operator <=(ParamData a, ParamData b) {
            return a.paramId == b.paramId && a.value <= b.value;
        }
        public static bool operator <=(ParamData a, double b) {
            return a.value <= b;
        }

        /// <summary>
        /// 判断相等
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>返回是否相等</returns>
        public override bool Equals(object obj) {
            var data = obj as ParamData;
            return data != null &&
                   paramId == data.paramId &&
                   value == data.value;
        }

        /// <summary>
        /// 生成哈希码
        /// </summary>
        /// <returns>返回哈希码</returns>
        public override int GetHashCode() {
            var hashCode = 574597825;
            hashCode = hashCode * -1521134295 + paramId.GetHashCode();
            hashCode = hashCode * -1521134295 + value.GetHashCode();
            return hashCode;
        }

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public ParamData() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ParamData(int paramId, double value = 0) {
            this.paramId = paramId;
            this.value = value;
        }
    }

    /// <summary>
    /// 属性（比率）数据
    /// </summary>
    public class ParamRateData : ParamData {

        #region 数据操作

        /// <summary>
        /// 是否小数
        /// </summary>
        /// <returns></returns>
        public override bool isDouble() {
            return true;
        }

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public ParamRateData() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ParamRateData(int paramId, double value = 0) : base(paramId, value) { }
    }

    /// <summary>
    /// 属性范围数据
    /// </summary>
    public class ParamRangeData : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int paramId { get; protected set; }
        [AutoConvert]
        public double minValue { get; protected set; } // 真实值
        [AutoConvert]
        public double maxValue { get; protected set; } // 真实值

        /// <summary>
        /// 是否需要ID
        /// </summary>
        protected override bool idEnable() { return false; }

        /// <summary>
        /// 获取基本属性
        /// </summary>
        /// <returns>基本属性</returns>
        public BaseParam param() {
            return DataService.get().baseParam(paramId);
        }
    }
}