

using System.Collections.Generic;
/// <summary>
/// 状态窗口艾瑟萌页信息显示
/// </summary>
public class ExermonPageDisplay : ExermonPageInfoBase {

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public ExermonStatusTabController tabController;

    public ExerSlotExermonDisplay exermonDisplay;

    public ExerHubDisplay exerHubDisplay;

    /// <summary>
    /// 外部系统设置
    /// </summary>
    PlayerService playerSer;
    ExermonService exerSer;

    #region 初始化

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void initializeOnce() {
        base.initializeOnce();
        playerSer = PlayerService.get();
        exerSer = ExermonService.get();
        exermonDisplay.pageDisplay = this;
        exerSer.loadExerHub();
    }

    #endregion

    #region 启动控制

    /// <summary>
    /// 开始视窗
    /// </summary>
    public override void startView() {
        base.startView();
    }

    #endregion

    #region 数据控制

    /// <summary>
    /// 物品改变回调
    /// </summary>
    protected override void onItemChanged() {
        base.onItemChanged();
        updateExerHubItems(item.subjectId);
        exermonDisplay.setItem(item);
    }
    
    /// <summary>
    /// 更新艾瑟萌仓库物品
    /// </summary>
    void updateExerHubItems(int subjectId) {
        var player = playerSer.player;
        var items = new List<PlayerExermon>();
        var exerHub = player.packContainers.exerHub;
        items.Add(item.playerExer);
        items.AddRange(exerHub.getItems(item =>
            item.exermon().subjectId == subjectId));
        exerHubDisplay.setItems(items);
        exerHubDisplay.startView(0);
    }

    #endregion

    #region 界面绘制

    #endregion

    #region 流程控制

    /// <summary>
    /// 状态变更回调
    /// </summary>
    public void onStatusChanged() {
        setItem(item, true);
    }

    #endregion
}
