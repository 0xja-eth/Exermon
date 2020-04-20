using UnityEngine;

using LitJson;

using Core.UI;

namespace UI.Common.Controls.ParamDisplays {

    /// <summary>
    /// 属性条组
    /// </summary>
    /// 
    public class ParamDisplaysGroup : GroupView<ParamDisplay> {

        #region 初始化

        /// <summary>
        /// 配置组件
        /// </summary>
        /// <param name="objs">对象数组</param>
        public void configure(ParamDisplay.IDisplayDataConvertable[] objs) {
            base.configure();
            configureParams(objs);
        }
        /// <param name="obj">对象</param>
        public void configure(ParamDisplay.IDisplayDataArrayConvertable obj) {
            base.configure();
            configureParams(obj);
        }

        /// <summary>
        /// 配置属性集
        /// </summary>
        /// <param name="objs">对象数组</param>
        void configureParams(ParamDisplay.IDisplayDataConvertable[] objs) {
            for (int i = 0; i < subViewsCount(); i++)
                configureParam(i, objs[i]);
        }
        /// <param name="obj">对象</param>
        void configureParams(ParamDisplay.IDisplayDataArrayConvertable obj, string type = "") {
            var data = obj.convertToDisplayDataArray(type);
            for (int i = 0; i < subViewsCount(); i++)
                configureParam(i, data[i]);
        }

        /// <summary>
        /// 配置属性
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="obj">对象/数据</param>
        void configureParam(int index, ParamDisplay.IDisplayDataConvertable obj) {
            subViews[index].configure(obj);
        }
        void configureParam(int index, JsonData obj) {
            subViews[index].configure(obj);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 设置忽略触发器
        /// </summary>
        public void setIgnoreTrigger(int index = 0) {
            if (index >= subViewsCount()) return;
            subViews[index].setIgnoreTrigger();
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="objs">对象数组</param>
        public void setValues(ParamDisplay.IDisplayDataConvertable[] objs,
            string type = "", bool force = false) {
            for (int i = 0; i < objs.Length; i++)
                setValue(i, objs[i], type, force);
        }
        /// <param name="obj">对象</param>
        public void setValues(ParamDisplay.IDisplayDataArrayConvertable obj,
            string type = "", bool force = false) {
            if (obj == null) clearValues();
            else {
                var infos = obj.convertToDisplayDataArray(type);
                for (int i = 0; i < infos.Length; i++)
                    setValue(i, infos[i], force);
            }
        }
        /// <param name="objs">多个对象</param>
        public void setValues(ParamDisplay.IDisplayDataArrayConvertable[] objs,
            string type = "", bool force = false) {
            if (objs == null || objs.Length <= 0) clearValues();
            else {
                int objLen = objs.Length, count = 0;
                var res = new JsonData[objLen][]; // 缓存所有 JsonData
                for (int i = 0; i < objLen; i++) {
                    res[i] = objs[i].convertToDisplayDataArray(type);
                    count = Mathf.Max(res[i].Length, count);
                }

                // 合并两个 DisplayDataArrayConvertable 生成的 JsonData
                // 对每个对象进行 setValue
                for (int i = 0; i < count; ++i) {
                    var json = new JsonData();
                    // 遍历每个物体的每个键并赋值到 JsonData
                    for (int j = 0; j < objLen; ++j)
                        foreach (var key in res[j][i].Keys)
                            json[key] = res[j][i][key];
                    // Debug.Log("jsons[" + i + "] = " + json.ToJson());
                    setValue(i, json, force);
                }
            }
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="obj">对象/数据</param>
        public void setValue(int index, JsonData obj, bool force = false) {
            createSubView(index).setValue(obj, force);
        }
        public void setValue(int index, ParamDisplay.IDisplayDataConvertable obj,
            string type = "", bool force = false) {
            createSubView(index).setValue(obj, type, force);
        }
        public void setValue(int index, ParamDisplay.IDisplayDataConvertable[] objs,
            string type = "", bool force = false) {
            createSubView(index).setValue(objs, type, force);
        }

        /// <summary>
        /// 清除值
        /// </summary>
        public void clearValues() {
            for (int i = 0; i < subViewsCount(); i++)
                clearValue(i);
        }

        /// <summary>
        /// 清除值
        /// </summary>
        public void clearValue(int index) {
            if (index >= subViewsCount()) return;
            subViews[index].clearValue();
        }

        #endregion
    }
}