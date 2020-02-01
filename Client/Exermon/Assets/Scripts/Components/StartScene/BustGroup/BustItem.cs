using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 半身像项
/// </summary>
public class BustItem : BaseView {

    /// <summary>
    /// 常量定义
    /// </summary>
    const float MoveDuration = 0.5f; // 移动用时

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public Image image; // 图片

    /// <summary>
    /// 内部变量声明
    /// </summary>
    BustGroup group;
    Character character;
    RectTransform rectTransform;

    int index, posIndex;
    float realIndex; 

    #region 初始化

    /// <summary>
    /// 初次打开时初始化（子类中重载）
    /// </summary>
    protected override void initializeOnce() {
        rectTransform = transform as RectTransform;
    }

    /// <summary>
    /// 配置组件
    /// </summary>
    public void configure(BustGroup group, int index) {
        this.index = index;
        this.group = group;
        base.configure();
    }

    #endregion

    #region 更新控制

    /// <summary>
    /// 更新
    /// </summary>
    protected override void update() {
        updateAnimation();
    }

    /// <summary>
    /// 更新动画
    /// </summary>
    void updateAnimation() {
        if (!isMoving()) return;
        updateRealIndex();
        updateSwitching();
    }

    /// <summary>
    /// 更新实际位置索引
    /// </summary>
    void updateRealIndex() {
        var dt = Time.deltaTime;
        var sign1 = Mathf.Sign(posIndex - realIndex);
        realIndex += dt / MoveDuration * sign1;
        var sign2 = Mathf.Sign(posIndex - realIndex);
        if (sign1 != sign2) realIndex = posIndex;
    }

    /// <summary>
    /// 更新切换效果
    /// </summary>
    void updateSwitching() {
        updateMoving();
        updateScaling();
        updateFading();
        updateSiblingIndex();
    }

    /// <summary>
    /// 分别更新三种动画
    /// </summary>
    void updateMoving() {
        var pos = rectTransform.anchoredPosition;
        pos.x = group.calcX(realIndex);
        rectTransform.anchoredPosition = pos;
    }
    void updateScaling() {
        var s = group.calcScale(realIndex);
        rectTransform.localScale = new Vector3(s, s, 1);
    }
    void updateFading() {
        var f = group.calcFade(realIndex);
        image.color = new Color(f, f, f, 1);
    }
    void updateSiblingIndex() {
        var cnt = group.characterCount();
        int index = (int)Mathf.Round(realIndex);
        index = (index % cnt + cnt) % cnt;
        transform.SetSiblingIndex(cnt - index - 1);
    }

    /// <summary>
    /// 是否正在移动中
    /// </summary>
    /// <returns>移动</returns>
    public bool isMoving() {
        return realIndex != posIndex;
    }

    #endregion

    /*
    #region 启动/结束控制

    /// <summary>
    /// 启动视窗
    /// </summary>
    public void startView(Character character) {
        this.character = character;
        base.startView();
    }

    #endregion
    */

    #region 界面控制

    /// <summary>
    /// 绘制半身像
    /// </summary>
    void drawBust() {
        if (character == null) return;
        var bust = character.bust;
        var rect = new Rect(0, 0, bust.width, bust.height);
        image.overrideSprite = Sprite.Create(
            bust, rect, new Vector2(0.5f, 0.5f));
        image.overrideSprite.name = bust.name;
        SceneUtils.setRectWidth(rectTransform, bust.width);
        SceneUtils.setRectHeight(rectTransform, bust.height);
    }

    /// <summary>
    /// 刷新视窗（clear后重绘）
    /// </summary>
    public override void refresh() {
        base.refresh();
        drawBust();
    }
    
    #endregion

    #region 数据控制

    /// <summary>
    /// 设置人物
    /// </summary>
    /// <param name="character">人物</param>
    public void setCharacter(Character character) {
        if(this.character != character) {
            this.character = character;
            refresh();
        }
    }

    /// <summary>
    /// 获取人物
    /// </summary>
    /// <returns>人物</returns>
    public Character getCharacter() {
        return character;
    }

    /// <summary>
    /// 设置位置索引
    /// </summary>
    /// <param name="posIndex">位置索引</param>
    public void setPosIndex(int posIndex, bool force=false) {
        this.posIndex = posIndex;
        if (force) {
            realIndex = posIndex;
            updateSwitching();
        }
    }

    /// <summary>
    /// 是否选中
    /// </summary>
    /// <returns>是否选中</returns>
    public bool isSelected() {
        var cnt = group.characterCount();
        return posIndex % cnt == 0;
    }

    #endregion

}
