
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 状态窗口艾瑟萌页信息显示
/// </summary>
public class ExerSlotExerGiftDisplay : SlotItemDisplay<ExerSlotItem, PlayerExerGift> {

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public PlayerExerGiftDetail playerGiftDetail;

    /// <summary>
    /// 页显示组件
    /// </summary>
    public ExerGiftPageDisplay pageDisplay { get; set; }

    /// <summary>
    /// 外部系统设置
    /// </summary>
    ExermonService exerSer;

    #region 初始化

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void initializeOnce() {
        base.initializeOnce();
        exerSer = ExermonService.get();
    }

    #endregion

    #region 数据控制
    
    /// <summary>
    /// 获取艾瑟萌
    /// </summary>
    /// <returns></returns>
    public Exermon getExermon(ExerSlotItem item = null) {
        if (item == null) item = this.item;
        if (item == null) return null;
        return item.playerExer.exermon();
    }

    /// <summary>
    /// 配置装备
    /// </summary>
    protected override void setupEquip() {
        equip = item.playerGift;
    }

    /// <summary>
    /// 装备变更回调
    /// </summary>
    protected override void onItemChanged() {
        base.onItemChanged();
        playerGiftDetail.slotItem = item;
    }

    /// <summary>
    /// 装备变更回调
    /// </summary>
    protected override void onEquipChanged() {
        base.onEquipChanged();
        requestEquip();
    }

    /// <summary>
    /// 预览变更回调
    /// </summary>
    protected override void onPreviewChanged() {
        base.onPreviewChanged();
        playerGiftDetail.setItem(previewingEquip);
    }

    #endregion

    #region 界面绘制

    /// <summary>
    /// 绘制物品
    /// </summary>
    /// <param name="slotItem">艾瑟萌槽项</param>
    protected override void drawExactlyEquip(PlayerExerGift playerGift) {
        base.drawExactlyEquip(playerGift);
        playerGiftDetail.setItem(playerGift);
    }

    #endregion

    #region 请求控制
    
    /// <summary>
    /// 请求更改艾瑟萌
    /// </summary>
    void requestEquip() {
        exerSer.equipPlayerGift(item, equip, pageDisplay.onStatusChanged);
    }

    #endregion
}
