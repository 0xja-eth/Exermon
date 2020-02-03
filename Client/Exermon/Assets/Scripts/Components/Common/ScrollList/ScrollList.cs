using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 滚动列表组件
/// </summary>
public class ScrollList : BaseView {

    /// <summary>
    /// 项目内容
    /// </summary>
    public interface ScrollListItemContent {

        /// <summary>
        /// 设置项目
        /// </summary>
        /// <param name="index">项目索引</param>
        /// <param name="text">项目文本</param>
        /// <param name="tag">项目标签</param>
        void setItem(int index, string text, string tag);
        /// <param name="item">项目组件实例</param>
        void setItem(ScrollListItem item);

    }

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public string[] initItems; // 初始项
    public string[] initTags; // 初始标签

    public bool multiple = false; // 是否多选
    public bool checkable = true; // 是否允许有check

    public RectTransform listContent; // 列表载体
    
    /// <summary>
    /// 预制件设置
    /// </summary>
    public GameObject listItemPrefab; // 列表项预制件（需包含ScrollListItem）

    /// <summary>
    /// 内部变量声明
    /// </summary>
    List<ScrollListItem> listItems = new List<ScrollListItem>(); // 项目实例列表
    List<int> selection = new List<int>(); // 选择情况
    int curIndex = -1; // 当前选中

    ScrollListItemContent helpContent; // 帮助内容组件

    /// <summary>
    /// 选项是否改变
    /// </summary>
    public bool isDirty { get; private set; }

    #region 初始化

    /// <summary>
    /// 初次打开时初始化（子类中重载）
    /// </summary>
    protected override void initializeOnce() {
        initializeItems();
    }

    /// <summary>
    /// 初始化项目
    /// </summary>
    void initializeItems() {
        clearListItems();
        for (int i = 0; i < Math.Min(initItems.Length, initTags.Length); i++)
            addListItem(initItems[i], initTags[i]);
    }

    #endregion

    #region 启动/结束控制

    /// <summary>
    /// 启动视窗
    /// </summary>
    public virtual void startView(ScrollListItemContent content) {
        helpContent = content;
        base.startView();
    }

    #endregion

    #region 内容控制

    #region 增加操作

    /// <summary>
    /// 新增项
    /// </summary>
    /// <param name="text">文本</param>
    /// <param name="tag">标签</param>
    public void addListItem(string text, string tag) {
        createListItem(text, tag); isDirty = true;
    }

    /// <summary>
    /// 创建项
    /// </summary>
    /// <param name="text">文本</param>
    /// <param name="tag">标签</param>
    void createListItem(string text, string tag) {
        var go = Instantiate(listItemPrefab, listContent);
        var item = SceneUtils.get<ScrollListItem>(go);
        item.startView(this, text, tag, select);
        listItems.Add(item);
    }

    #endregion

    #region 获取操作

    /// <summary>
    /// 获取项
    /// </summary>
    /// <param name="index">索引</param>
    /// <return>项组件实例</return>
    public ScrollListItem getListItem(int index) {
        if (index < 0 || index > listItems.Count) return null;
        return listItems[index];
    }
    /// <summary>
    /// 获取项ID
    /// </summary>
    /// <param name="item">项文本</param>
    /// <param name="tag">项标签</param>
    /// <returns>索引</returns>
    public int getListItemId(string text, string tag) {
        return getListItemId((item) => item.itemText == text && item.itemTag == tag);
    }
    /// <summary>
    /// 获取项
    /// </summary>
    /// <param name="item">项文本</param>
    /// <param name="tag">项标签</param>
    /// <return>项组件实例</return>
    public ScrollListItem getListItem(string text, string tag) {
        return getListItem((item) => item.itemText == text && item.itemTag == tag);
    }
    /// <summary>
    /// 通过项文本获取项ID
    /// </summary>
    /// <param name="text">项文本</param>
    /// <returns>索引</returns>
    public int getListItemIdByText(string text) {
        return getListItemId((item) => item.itemText == text);
    }
    /// <summary>
    /// 通过项文本获取项
    /// </summary>
    /// <param name="text">项文本</param>
    /// <return>项组件实例</return>
    public ScrollListItem getListItemByText(string text) {
        return getListItem((item) => item.itemText == text);
    }
    /// <summary>
    /// 根据项标签获取项ID
    /// </summary>
    /// <param name="tag">项标签</param>
    /// <returns>索引</returns>
    public int getListItemIdByTag(string tag) {
        return getListItemId((item) => item.itemTag == tag);
    }
    /// <summary>
    /// 通过项文本获取项
    /// </summary>
    /// <param name="tag">项标签</param>
    /// <return>项组件实例</return>
    public ScrollListItem getListItemByTag(string text) {
        return getListItem((item) => item.itemTag == tag);
    }
    /// <summary>
    /// 根据自定义条件获取项ID
    /// </summary>
    /// <param name="p">条件</param>
    /// <return>项ID</return>
    public int getListItemId(Predicate<ScrollListItem> p) {
        for (int i = 0; i < listItems.Count; i++) if (p.Invoke(listItems[i])) return i;
        return -1;
    }
    /// <summary>
    /// 根据自定义条件获取项
    /// </summary>
    /// <param name="p">条件</param>
    /// <return>项组件实例</return>
    public ScrollListItem getListItem(Predicate<ScrollListItem> p) {
        foreach (var item in listItems) if (p.Invoke(item)) return item;
        return null;
    }

    #endregion

    #region 删除操作

    /// <summary>
    /// 删除项
    /// </summary>
    /// <param name="index">下标</param>
    public void deleteListItem(int index) {
        DestroyImmediate(listItems[index].gameObject);
        listItems.RemoveAt(index); isDirty = true;
    }
    /// <param name="text">文本</param>
    /// <param name="tag">标签</param>
    public void deleteListItem(string text, string tag) {
        deleteListItem((item) => item.itemText == text && item.itemTag == tag);
    }
    /// <summary>
    /// 通过文本删除项
    /// </summary>
    /// <param name="text">文本</param>
    public void deleteListItemByText(string text) {
        deleteListItem((item) => item.itemText == text);
    }
    /// <summary>
    /// 通过标签删除项
    /// </summary>
    /// <param name="tag">标签</param>
    public void deleteListItemByTag(string tag) {
        deleteListItem((item) => item.itemTag == tag);
    }
    /// <summary>
    /// 根据自定义条件删除项
    /// </summary>
    /// <param name="p">条件</param>
    public void deleteListItem(Predicate<ScrollListItem> p) {
        var list = new List<ScrollListItem>();
        foreach (var item in listItems) if (p.Invoke(item)) list.Add(item);
        for (int i = 0; i < list.Count; i++) {
            DestroyImmediate(list[i].gameObject);
            listItems.Remove(list[i]);
        }
        isDirty = true;
    }

    #endregion

    /// <summary>
    /// 清空项目
    /// </summary>
    public void clearListItems() {
        deleteListItem((_) => true);
    }

    /// <summary>
    /// 清空
    /// </summary>
    protected override void clear() {
        deselect();
        clearSelection();
        clearListItems();
    }

    #endregion

    #region 选择控制

    /// <summary>
    /// 当前选择ID
    /// </summary>
    /// <returns>当前ID</returns>
    public int currentIndex() { return curIndex; }

    /// <summary>
    /// 当前选择项目
    /// </summary>
    /// <returns>当前项目</returns>
    public ScrollListItem currentItem() { return getListItem(curIndex); }

    /// <summary>
    /// 所选ID数组
    /// </summary>
    /// <returns>所选ID数组</returns>
    public int[] currentSelection() { return selection.ToArray(); }

    /// <summary>
    /// 所选项目数组
    /// </summary>
    /// <returns>所选项目数组</returns>
    public ScrollListItem[] currentSelectedItem() {
        return selection.ConvertAll((id) => getListItem(id)).ToArray();
    }

    /// <summary>
    /// 指定索引项目是否选择
    /// </summary>
    /// <param name="index">索引</param>
    /// <returns>是否选择</returns>
    public bool isChecked(int index) {
        return selection.Contains(index);
    }

    /// <summary>
    /// 指定索引项目是否当前选中
    /// </summary>
    /// <param name="index">索引</param>
    /// <returns>是否选中</returns>
    public bool isSelected(int index) {
        return curIndex == index;
    }

    /// <summary>
    /// 设置当前选中项
    /// </summary>
    /// <param name="index">索引</param>
    public void select(int index) {
        var last = getListItem(curIndex);
        var _new = getListItem(curIndex = index);
        if (last) last.setSelected(false);
        if (_new) _new.setSelected(true);
        if (checkable) toggle(index);
        else check(index);
        if (helpContent != null)
            helpContent.setItem(_new);
    }

    /// <summary>
    /// 清空选择
    /// </summary>
    public void clearSelection() {
        selection.Clear();
    }

    /// <summary>
    /// 取消选择
    /// </summary>
    public void deselect() {
        select(-1);
    }

    /// <summary>
    /// 反转项
    /// </summary>
    /// <param name="index">索引</param>
    public void toggle(int index) {
        if (isChecked(index)) uncheck(index);
        else check(index);
    }

    /// <summary>
    /// 选择项
    /// </summary>
    /// <param name="index">索引</param>
    public void check(int index) {
        if (!multiple && selection.Count>0) uncheck(selection[0]);
        var item = getListItem(index);
        if(item) {
            item.setChecked(true);
            selection.Add(index);
        }
    }

    /// <summary>
    /// 取消选择项
    /// </summary>
    /// <param name="index">索引</param>
    public void uncheck(int index) {
        var item = getListItem(index);
        item.setChecked(false);
        selection.Remove(index);
    }

    #endregion
}
