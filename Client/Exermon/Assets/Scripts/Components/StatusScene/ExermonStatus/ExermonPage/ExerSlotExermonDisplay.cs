
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 状态窗口艾瑟萌页信息显示
/// </summary>
public class ExerSlotExermonDisplay : SlotItemDisplay<ExerSlotItem, PlayerExermon> {

    /// <summary>
    /// 操作文本常量设置
    /// </summary>
    const string RenameOperText = "修改艾瑟萌昵称";
    const string EquipOperText = "更换艾瑟萌";

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public PlayerExermonDetail playerExerDetail;

    /// <summary>
    /// 页显示组件
    /// </summary>
    public ExermonPageDisplay pageDisplay { get; set; }

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
        equip = item.playerExer;
    }

    /// <summary>
    /// 装备变更回调
    /// </summary>
    protected override void onItemChanged() {
        base.onItemChanged();
        playerExerDetail.slotItem = item;
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
        playerExerDetail.setItem(previewingEquip);
    }

    #endregion

    #region 界面绘制

    /// <summary>
    /// 绘制物品
    /// </summary>
    /// <param name="slotItem">艾瑟萌槽项</param>
    protected override void drawExactlyEquip(PlayerExermon playerExer) {
        base.drawExactlyEquip(playerExer);
        playerExerDetail.setItem(playerExer);
    }

    #endregion

    #region 请求控制
    
    /// <summary>
    /// 请求更改艾瑟萌
    /// </summary>
    void requestEquip() {
        exerSer.equipPlayerExer(item, equip, pageDisplay.onStatusChanged);
    }

    #endregion
}
