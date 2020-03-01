using System.Collections.Generic;

/// <summary>
/// 状态窗口艾瑟萌页信息显示
/// </summary>
public class ExerGiftPageDisplay : ExermonStatusPageInfo<PlayerExerGift> {

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public ExermonStatusTabController tabController;

    public ExerSlotExerGiftDisplay exerGiftDisplay;

    public ExerGiftPoolDisplay exerGiftPoolDisplay;

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
        exerGiftDisplay.pageDisplay = this;
        exerSer.loadExerGiftPool();
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
    /// 获取容器组件
    /// </summary>
    protected override ItemContainer<PlayerExerGift>
        getContainer() { return exerGiftPoolDisplay; }

    /// <summary>
    /// 获取艾瑟萌槽项显示组件
    /// </summary>
    protected override SlotItemDisplay<ExerSlotItem, PlayerExerGift>
        getSlotItemDisplay() { return exerGiftDisplay; }

    /// <summary>
    /// 物品改变回调
    /// </summary>
    protected override void onItemChanged() {
        base.onItemChanged();
        updateGiftPoolItems();
        exerGiftDisplay.setItem(item);
    }
    
    /// <summary>
    /// 更新艾瑟萌天赋池物品
    /// </summary>
    void updateGiftPoolItems() {
        var player = playerSer.player;
        var items = new List<PlayerExerGift>();
        var giftPool = player.packContainers.exerGiftPool;
        items.Add(item.playerGift);
        items.AddRange(giftPool.items);
        exerGiftPoolDisplay.setItems(items);
        exerGiftPoolDisplay.startView(0);
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
