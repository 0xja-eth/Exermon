
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 状态窗口艾瑟萌页属性信息显示
/// </summary>
public class PlayerExerGiftDetail : ItemDetail<PlayerExerGift> {
    
    /// <summary>
    /// 外部组件设置
    /// </summary>
    public Image icon;
    public StarsDisplay stars;

    public Text name, description;

    public ParamDisplaysGroup paramInfo;
    public ParamDisplay battlePoint;

    /// <summary>
    /// 艾瑟萌槽项
    /// </summary>
    public ExerSlotItem slotItem = null;

    #region 初始化

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void initializeOnce() {
        base.initializeOnce();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public override void configure() {
        base.configure();
        var params_ = DataService.get().staticData.configure.baseParams;
        paramInfo.configure(params_);
    }

    #endregion

    #region 数据控制

    /// <summary>
    /// 设置艾瑟萌槽项
    /// </summary>
    /// <param name="slotItem"></param>
    public void setSlotItem(ExerSlotItem slotItem) {
        this.slotItem = slotItem;
        requestRefresh();
    }

    #endregion

    #region 界面绘制

    /// <summary>
    /// 绘制物品
    /// </summary>
    /// <param name="slotItem">艾瑟萌槽项</param>
    protected override void drawExactlyItem(PlayerExerGift playerGift) {
        base.drawExactlyItem(playerGift);
        drawIconImage(playerGift);
        drawBaseInfo(playerGift);
        drawParamsInfo(playerGift);
    }

    /// <summary>
    /// 绘制艾瑟萌图像
    /// </summary>
    /// <param name="slotItem">艾瑟萌槽项</param>
    void drawIconImage(PlayerExerGift playerGift) {
        var gift = playerGift.item();
        var icon = gift.bigIcon;
        var rect = new Rect(0, 0, icon.width, icon.height);
        this.icon.gameObject.SetActive(true);
        this.icon.overrideSprite = Sprite.Create(
            icon, rect, new Vector2(0.5f, 0.5f));
        this.icon.overrideSprite.name = icon.name;

        stars.setValue(gift.starId);
    }

    /// <summary>
    /// 绘制基本信息
    /// </summary>
    /// <param name="slotItem">艾瑟萌槽项</param>
    void drawBaseInfo(PlayerExerGift playerGift) {
        var gift = playerGift.item();
        name.text = gift.name;
        description.text = gift.description;
    }

    /// <summary>
    /// 绘制艾瑟萌图像
    /// </summary>
    /// <param name="slotItem">艾瑟萌槽项</param>
    void drawParamsInfo(PlayerExerGift playerGift) {
        // 如果要显示的艾瑟萌与装备中的一致，直接显示
        if (slotItem != null)
            if(playerGift == slotItem.playerGift) {
                paramInfo.setValues(slotItem, "params");
                battlePoint.setValue(slotItem, "battle_point");
            } else {
                slotItem.setPlayerGiftPreview(playerGift);
                paramInfo.setValues(slotItem, "preview_params");
                battlePoint.setValue(slotItem, "preview_battle_point");
                slotItem.clearPreviewObject();
            }
        else {
            paramInfo.setValues(playerGift.item());
        }
    }

    /// <summary>
    /// 清除物品
    /// </summary>
    protected override void clearItem() {
        name.text = description.text = "";
        icon.gameObject.SetActive(false);
        stars.clearValue();

        paramInfo.clearValues();
        battlePoint.clearValue();
    }

    #endregion
}
