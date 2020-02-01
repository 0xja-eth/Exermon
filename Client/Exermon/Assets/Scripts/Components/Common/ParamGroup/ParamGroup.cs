
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 属性条组
/// </summary>
public class ParamGroup : BaseView {

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public Transform paramView;

    /// <summary>
    /// 外部变量设置
    /// </summary>
    public int count = 6; // 属性值条个数
    public string nameFormat = "ParamBar{0}"; // 属性值条名称格式

    /// <summary>
    /// 内部变量声明
    /// </summary>
    ParamBar[] bars;

    #region 初始化

    /// <summary>
    /// 初次打开时初始化（子类中重载）
    /// </summary>
    protected override void initializeOnce() {
        setupBars();
    }

    /// <summary>
    /// 配置属性值条数组
    /// </summary>
    void setupBars() {
        bars = new ParamBar[count];
        for (int i = 1; i <= count; i++) {
            var name = string.Format(nameFormat, i);
            bars[i-1] = SceneUtils.find<ParamBar>(paramView, name);
        }
    }

    /// <summary>
    /// 配置组件
    /// </summary>
    /// <param name="params_">属性</param>
    public void configure(ParamBar.ParamConfigInfoConvertable[] objs) {
        base.configure();
        configureBars(objs);
    }

    /// <summary>
    /// 配置属性条
    /// </summary>
    /// <param name="obj">对象</param>
    void configureBars(ParamBar.ParamConfigInfoConvertable[] objs) {
        for(int i=0;i<count;i++) {
            var obj = objs[i];
            var info = obj.convertToParamInfo();
            configureBar(i, info);
        }
    }

    /// <summary>
    /// 配置属性条
    /// </summary>
    /// <param name="index"></param>
    /// <param name="info"></param>
    void configureBar(int index, ParamBar.ParamConfigInfo info) {
        bars[index].configure(info);
    }

    #endregion

    #region 数据控制

    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="obj">对象</param>
    public void setValues(ParamBar.ParamValueInfosConvertable obj, bool force = false) {
        if (obj == null) clearValues();
        else {
            var infos = obj.convertToParamInfos();
            for (int i = 0; i < count; i++) setValue(i, infos[i], force);
        }
    }
    /// <param name="values">值</param>
    /// <param name="maxs">最大值</param>
    /// <param name="rates">成长率</param>
    /// <param name="force">强制</param>
    public void setValues(List<float> values, List<float> maxs = null,
        List<float> rates = null, bool force = false) {
        for (int i = 0; i < count; i++) {
            var value = values[i];
            float? max = null, rate = null;
            if (maxs != null) max = maxs[i];
            if (rates != null) rate = rates[i];

            setValue(i, value, max, rate, force);
        }
    }

    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="info">属性信息</param>
    public void setValue(int index, ParamBar.ParamValueInfo info, bool force = false) {
        bars[index].setValue(info, force);
    }
    /// <param name="index">索引</param>
    /// <param name="value">值</param>
    /// <param name="max">最大值</param>
    /// <param name="rate">成长率</param>
    /// <param name="force">强制</param>
    public void setValue(int index, float value, float? max = null,
        float? rate = null, bool force = false) {
        bars[index].setValue(value, max, rate, force);
    }

    /// <summary>
    /// 清除值
    /// </summary>
    public void clearValues() {
        for (int i = 0; i < count; i++) clearValue(i);
    }

    /// <summary>
    /// 清除值
    /// </summary>
    public void clearValue(int index) {
        bars[index].clearValue();
    }

    #endregion
}
