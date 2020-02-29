
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using LitJson;

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
    public void configure(ParamDisplay.DisplayDataConvertable[] objs) {
        base.configure();
        configureParams(objs);
    }
    /// <param name="obj">对象</param>
    public void configure(ParamDisplay.DisplayDataArrayConvertable obj) {
        base.configure();
        configureParams(obj);
    }

    /// <summary>
    /// 配置属性集
    /// </summary>
    /// <param name="objs">对象数组</param>
    void configureParams(ParamDisplay.DisplayDataConvertable[] objs) {
        for (int i = 0; i < subViewsCount(); i++)
            configureParam(i, objs[i]);
    }
    /// <param name="obj">对象</param>
    void configureParams(ParamDisplay.DisplayDataArrayConvertable obj, string type = "") {
        var data = obj.convertToDisplayDataArray(type);
        for (int i = 0; i < subViewsCount(); i++)
            configureParam(i, data[i]);
    }

    /// <summary>
    /// 配置属性
    /// </summary>
    /// <param name="index">索引</param>
    /// <param name="obj">对象/数据</param>
    void configureParam(int index, ParamDisplay.DisplayDataConvertable obj) {
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
    public void setIgnoreTrigger(int index=0) {
        if (index >= subViewsCount()) return;
        subViews[index].setIgnoreTrigger();
    }

    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="objs">对象数组</param>
    public void setValues(ParamDisplay.DisplayDataConvertable[] objs,
        string type = "", bool force = false) {
        for (int i = 0; i < subViewsCount(); i++)
            setValue(i, objs[i], type, force);
    }
    /// <param name="obj">对象</param>
    public void setValues(ParamDisplay.DisplayDataArrayConvertable obj, 
        string type = "", bool force = false) {
        if (obj == null) clearValues();
        else {
            var infos = obj.convertToDisplayDataArray(type);
            for (int i = 0; i < subViewsCount(); i++)
                setValue(i, infos[i], force);
        }
    }

    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="obj">对象/数据</param>
    public void setValue(int index, JsonData obj, bool force = false) {
        if (index >= subViewsCount()) return;
        subViews[index].setValue(obj, force);
    }
    public void setValue(int index, ParamDisplay.DisplayDataConvertable obj, 
        string type = "", bool force = false) {
        if (index >= subViewsCount()) return;
        subViews[index].setValue(obj, type, force);
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
