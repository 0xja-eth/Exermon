using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 半身像图片组（拥有切换等功能）
/// </summary>
public class BustGroup : BaseView {

    /// <summary>
    /// 常量设置
    /// </summary>
    const float WidthRate = 1; // 移动宽度比率
    const float ScaleRate = 0.8f; // 缩放比率
    const float FadeRate = 0.25f; // 淡出比率

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public Text name, description;
    public Transform bustItemView;

    /// <summary>
    /// 预制件设置
    /// </summary>
    public GameObject bustItemPerfab; // 半身像项预制件

    /// <summary>
    /// 内部变量声明
    /// </summary>
    List<BustItem> bustItems = new List<BustItem>();

    Character[] characters;

    int index, posIndex;

    #region 初始化

    /// <summary>
    /// 初次打开时初始化（子类中重载）
    /// </summary>
    protected override void initializeOnce() {
        //bustItems = new List<BustItem>();
    }
    
    /// <summary>
    /// 配置组件
    /// </summary>
    public void configure(Character[] characters) {
        base.configure();
        setCharacters(characters);
    }

    #endregion

    #region 启动/结束控制

    /// <summary>
    /// 启动视窗
    /// </summary>
    public void startView(int index=0) {
        setIndex(index, index, true);
        base.startView();
    }

    #endregion

    #region 数据控制

    /// <summary>
    /// 获取角色数量
    /// </summary>
    /// <returns>数量</returns>
    public int characterCount() {
        return characters.Length;
    }

    /// <summary>
    /// 获取当前选中人物
    /// </summary>
    /// <returns>人物</returns>
    public Character currentCharacter() {
        if (index >= bustItems.Count) return null;
        return bustItems[index].getCharacter();
    }

    /// <summary>
    /// 设置选中索引
    /// </summary>
    /// <param name="index">索引</param>
    public void setCharacters(Character[] characters) {
        if(this.characters != characters) {
            this.characters = characters;
            createBustItems();
            setIndex(0, 0, true);
            refresh();
        }
    }

    /// <summary>
    /// 设置选中索引
    /// </summary>
    /// <param name="index">索引</param>
    public void setIndex(int index, int posIndex, bool force = false) {
        var cnt = bustItems.Count;
        this.posIndex = posIndex;
        this.index = (index+cnt)%cnt;
        refreshPosition(force);
        refresh();
    }

    #endregion

    #region 动画计算控制

    /// <summary>
    /// 获取动画的最大宽度
    /// </summary>
    /// <returns>宽度</returns>
    float maxWidth() {
        var rt = transform as RectTransform;
        return rt.rect.width/2 * WidthRate;
    }

    /// <summary>
    /// 最大/最小缩放
    /// </summary>
    /// <returns>缩放</returns>
    float maxScale() { return 1; }
    float minScale() { return maxScale() * ScaleRate; }

    /// <summary>
    /// 最大/最小淡入
    /// </summary>
    /// <returns>淡入程度（明亮度）</returns>
    float maxFade() { return 1; }
    float minFade() { return maxFade() * FadeRate; }

    /// <summary>
    /// 计算X坐标
    /// </summary>
    /// <param name="i">索引</param>
    /// <returns>X坐标</returns>
    public float calcX(float i) {
        float n = characterCount();
        float w = maxWidth();
        float k = Mathf.Floor((i + n / 4) / n);
        i -= k * n;
        if (i < n / 4) return 4 * w / n * i;
        if (i < 3 * n / 4) return 2 * w - 4 * w / n * i;
        return 0;
    }

    /// <summary>
    /// 计算缩放
    /// </summary>
    /// <param name="i">索引</param>
    /// <returns>缩放</returns>
    public float calcScale(float i) {
        float n = characterCount();
        float maxS = maxScale();
        float minS = minScale();
        float k = Mathf.Floor(i / n);
        i -= k * n;
        if (i < n / 2) return 2 * (minS - maxS) / n * i + maxS;
        if (i <= n) return 2 * (maxS - minS) / n * i + 2 * minS - maxS;
        return 1;
    }

    /// <summary>
    /// 计算淡入
    /// </summary>
    /// <param name="i">索引</param>
    /// <returns>淡入</returns>
    public float calcFade(float i) {
        float n = characterCount();
        float maxF = maxFade();
        float minF = minFade();
        float k = Mathf.Floor(i / n);
        i -= k * n;
        if (i < n / 2) return 2 * (minF - maxF) / n * i + maxF;
        if (i <= n) return 2 * (maxF - minF) / n * i + 2 * minF - maxF;
        return 1;
    }

    #endregion

    #region 界面控制

    /// <summary>
    /// 刷新人物集
    /// </summary>
    public void createBustItems() {
        for (int i = 0; i < characters.Length; i++)
            createBustItem(characters[i], i);
    }

    /// <summary>
    /// 创建半身像项
    /// </summary>
    void createBustItem(Character character, int index) {
        var bustItem = getOrCreateBustItem(index);
        bustItem.setCharacter(character);
    }

    /// <summary>
    /// 获取或者创建一个BustItem
    /// </summary>
    /// <param name="index">索引</param>
    /// <returns>BustItem</returns>
    BustItem getOrCreateBustItem(int index) {
        if (index < bustItems.Count) return bustItems[index];
        var obj = Instantiate(bustItemPerfab, bustItemView);
        var item = SceneUtils.get<BustItem>(obj);
        item.configure(this, index);
        bustItems.Add(item);
        return item;
    }

    /// <summary>
    /// 绘制详细
    /// </summary>
    public void drawDetail() {
        var character = currentCharacter();
        if (character == null) return;
        name.text = character.name;
        description.text = character.description;
    }

    /// <summary>
    /// 刷新位置
    /// </summary>
    public void refreshPosition(bool force = false) {
        var cnt = bustItems.Count;
        for (int i = 0; i < cnt; i++) {
            var posIndex = i - this.posIndex;
            // if (posIndex > 3 * cnt / 4.0f) posIndex = cnt;
            // posIndex = (posIndex + cnt) % cnt;
            bustItems[i].setPosIndex(posIndex, force);
        }
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
        name.text = description.text = "";
    }

    #endregion

    #region 流程控制

    /// <summary>
    /// 是否有BustItem在移动
    /// </summary>
    /// <returns>是否移动</returns>
    public bool isMoving() {
        foreach (var bustItem in bustItems)
            if (bustItem.isMoving()) return true;
        return false;
    }

    /// <summary>
    /// 当“下一个”按钮按下时回调事件
    /// </summary>
    public void onNext() {
        if (isMoving()) return;
        setIndex(index + 1, posIndex + 1);
    }

    /// <summary>
    /// 当“上一个”按钮按下时回调事件
    /// </summary>
    public void onPrev() {
        if (isMoving()) return;
        setIndex(index - 1, posIndex - 1);
    }

    #endregion
}
