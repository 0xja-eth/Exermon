

/// <summary>
/// 状态窗口艾瑟萌页信息显示
/// </summary>
public class ExermonPageDisplay : ExermonPageInfoBase {

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public ExermonStatusTabController tabController;

    public ExermonBaseInfoDisplay baseInfo;
    public ExermonParamInfoDisplay paramInfo;

    #region 初始化

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void initializeOnce() {
        base.initializeOnce();
        baseInfo.pageDisplay = this;
    }

    #endregion

    #region 数据控制

    /// <summary>
    /// 物品改变回调
    /// </summary>
    protected override void onItemChanged() {
        base.onItemChanged();

        baseInfo.setItem(item);
        paramInfo.setItem(item);
    }

    #endregion

    #region 界面绘制

    #endregion

    #region 流程控制

    #endregion
}
