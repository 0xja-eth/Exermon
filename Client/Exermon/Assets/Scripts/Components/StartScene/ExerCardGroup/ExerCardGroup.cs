using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 艾瑟萌卡片图片组
/// </summary>
public class ExerCardGroup : BaseView {

    /// <summary>
    /// 常量设置
    /// </summary>

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public Transform exerCardView;
    public ExermonDetail detail;
    
    /// <summary>
    /// 预制件设置
    /// </summary>
    public GameObject exerCardPerfab; // 半身像项预制件

    /// <summary>
    /// 内部变量声明
    /// </summary>
    List<ExerCard> exerCards = new List<ExerCard>();
    List<int> checkedIndices = new List<int>();

    Exermon[] exermons;
    string[] enames; // 每一个艾瑟萌的昵称

    // ExermonsWindow window;

    int index, maxChecked;
    
    #region 初始化

    /// <summary>
    /// 初次打开时初始化（子类中重载）
    /// </summary>
    protected override void initializeOnce() {
        var max = DataService.get().staticData.configure.maxSubject;
        var cnt = Subject.ForceCount;
        maxChecked = max - cnt;
    }
    
    /// <summary>
    /// 配置组件
    /// </summary>
    public void configure(Exermon[] exermons) {
        base.configure();
        detail.configure(this);
        setExermons(exermons);
    }

    #endregion

    #region 启动/结束控制

    /// <summary>
    /// 启动视窗
    /// </summary>
    public void startView(int index=0) {
        this.index = index;
        base.startView();
    }

    #endregion

    #region 数据控制

    /// <summary>
    /// 更改昵称
    /// </summary>
    /// <param name="index">索引</param>
    /// <param name="name">名称</param>
    public void changeNickname(int index, string name) {
        enames[index] = name;
    }

    /// <summary>
    /// 获取昵称
    /// </summary>
    /// <param name="index">索引</param>
    /// <returns>昵称</returns>
    public string getNickname(int index) {
        return enames[index];
    }

    /// <summary>
    /// 获取角色数量
    /// </summary>
    /// <returns>数量</returns>
    public int exermonCount() {
        return exermons.Length;
    }

    /// <summary>
    /// 获取当前选中人物
    /// </summary>
    /// <returns>人物</returns>
    public Exermon currentExermon() {
        if (index >= exerCards.Count) return null;
        return exerCards[index].getExermon();
    }

    /// <summary>
    /// 设置选中索引
    /// </summary>
    /// <param name="index">索引</param>
    public void setExermons(Exermon[] exermons) {
        if(this.exermons != exermons) {
            this.exermons = exermons;
            setupEnames();
            createExerCards();
            refresh();
        }
    }

    /// <summary>
    /// 配置艾瑟萌
    /// </summary>
    void setupEnames() {
        var cnt = exermonCount();
        enames = new string[cnt];
        for (int i = 0; i < cnt; i++)
            enames[i] = "";
    }

    /// <summary>
    /// 设置选中索引
    /// </summary>
    /// <param name="index">索引</param>
    public void setIndex(int index) {
        if(this.index != index) {
            this.index = index;
            refresh();
        }
    }

    /// <summary>
    /// 获取选择索引
    /// </summary>
    /// <returns>索引</returns>
    public int getIndex() {
        return index;
    }

    /// <summary>
    /// 增加选择项
    /// </summary>
    /// <param name="index">索引</param>
    public void addCheck(int index) {
        var cnt = checkedIndices.Count;
        if (cnt >= maxChecked) {
            var i = checkedIndices[0];
            exerCards[i].uncheck();
        }
        checkedIndices.Add(index);
    }

    /// <summary>
    /// 移除选择项
    /// </summary>
    /// <param name="index">索引</param>
    public void removeCheck(int index) {
        checkedIndices.Remove(index);
    }

    /// <summary>
    /// 校验选择数目
    /// </summary>
    /// <returns>选择数目是否正确</returns>
    public bool check() {
        return checkedIndices.Count == maxChecked;
    }

    /// <summary>
    /// 获取选择结果
    /// </summary>
    /// <param name="eids">选择的艾瑟萌ID数组</param>
    /// <param name="enames">选择的艾瑟萌昵称数组</param>
    public void getResult(out int[] eids, out string[] enames) {
        var eidList = new List<int>();
        var enameList = new List<string>();
        for (int i = 0; i < exermonCount(); i++) {
            var card = exerCards[i];
            if (card.isChecked()) {
                eidList.Add(exermons[i].getID());
                enameList.Add(this.enames[i]);
            }
        }
        eids = eidList.ToArray();
        enames = enameList.ToArray();
    }

    /// <summary>
    /// 获取选择结果
    /// </summary>
    /// <returns>选择的所有艾瑟萌</returns>
    public Exermon[] getResult() {
        var exermons = new List<Exermon>();
        for (int i = 0; i < exermonCount(); i++) 
            if (exerCards[i].isChecked()) 
                exermons.Add(this.exermons[i]);
        return exermons.ToArray();
    }

    #endregion

    #region 界面控制

    /// <summary>
    /// 创建艾瑟萌卡片
    /// </summary>
    public void createExerCards() {
        for (int i = 0; i < exermons.Length; i++)
            createExerCard(exermons[i], i);
    }

    /// <summary>
    /// 创建艾瑟萌卡片
    /// </summary>
    void createExerCard(Exermon exermon, int index) {
        var card = getOrCreateExerCard(index);
        card.setExermon(exermon);
    }

    /// <summary>
    /// 获取或者创建一个 ExerCard
    /// </summary>
    /// <param name="index">索引</param>
    /// <returns>ExerCard</returns>
    ExerCard getOrCreateExerCard(int index) {
        if (index < exerCards.Count) return exerCards[index];
        var obj = Instantiate(exerCardPerfab, exerCardView);
        var item = SceneUtils.get<ExerCard>(obj);
        item.configure(this, index);
        exerCards.Add(item);
        return item;
    }

    /// <summary>
    /// 绘制详细
    /// </summary>
    public void drawDetail() {
        var exermon = currentExermon();
        if (exermon == null) return;
        detail.setExermon(exermon, index);
    }

    /// <summary>
    /// 刷新视窗（clear后重绘）
    /// </summary>
    public override void refresh() {
        base.refresh();
        drawDetail();
    }

    /// <summary>
    /// 清除描述
    /// </summary>
    public override void clear() {
        base.clear();
        clearDetail();
    }

    /// <summary>
    /// 清除描述
    /// </summary>
    public void clearDetail() {
        detail.clear();
    }

    #endregion
    
}
