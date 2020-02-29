
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 状态窗口艾瑟萌页信息显示
/// </summary>
public class ExermonBaseInfoDisplay : SlotItemDisplay<ExerSlotItem, PlayerExermon> {

    /// <summary>
    /// 操作文本常量设置
    /// </summary>
    const string RenameOperText = "修改艾瑟萌昵称";
    const string EquipOperText = "更换艾瑟萌";

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public Image full;
    public StarsDisplay stars;

    public Text animal, type, nickname, description;
    public TextInputField nicknameInput;

    public ParamDisplay expBar;

    public GameObject editButton;

    /// <summary>
    /// 页显示组件
    /// </summary>
    public ExermonPageDisplay pageDisplay { get; set; }

    /// <summary>
    /// 场景组件引用
    /// </summary>
    StatusScene scene;

    /// <summary>
    /// 外部系统设置
    /// </summary>
    ExermonService exerSer;

    #region 初始化

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void initializeOnce() {
        nicknameInput.onChanged = onNicknameChanged;
        exerSer = ExermonService.get();
        scene = (StatusScene)SceneUtils.getSceneObject("Scene");
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
    protected override void onEquipChanged() {
        base.onEquipChanged();
        pageDisplay?.requestRefresh();
        item.setPlayerExer(equip);
        requestEquip();
    }

    /// <summary>
    /// 预览变更回调
    /// </summary>
    protected override void onPreviewChanged() {
        base.onPreviewChanged();
        pageDisplay?.requestRefresh();
        item.setPlayerExerView(equip);
    }

    #endregion

    #region 界面绘制

    /// <summary>
    /// 绘制物品
    /// </summary>
    /// <param name="slotItem">艾瑟萌槽项</param>
    protected override void drawExactlyEquip(PlayerExermon playerExer) {
        base.drawExactlyEquip(playerExer);
        drawFullImage(playerExer);
        drawBaseInfo(playerExer);
        drawExpInfo(playerExer);
    }

    /// <summary>
    /// 绘制艾瑟萌图像
    /// </summary>
    /// <param name="slotItem">艾瑟萌槽项</param>
    void drawFullImage(PlayerExermon playerExer) {
        var exermon = playerExer.exermon();
        var full = exermon.full;
        var rect = new Rect(0, 0, full.width, full.height);
        this.full.gameObject.SetActive(true);
        this.full.overrideSprite = Sprite.Create(
            full, rect, new Vector2(0.5f, 0.5f));
        this.full.overrideSprite.name = full.name;

        stars.setValue(exermon.starId);
    }

    /// <summary>
    /// 绘制基本信息
    /// </summary>
    /// <param name="slotItem">艾瑟萌槽项</param>
    void drawBaseInfo(PlayerExermon playerExer) {
        var exermon = playerExer.exermon();
        animal.text = exermon.animal;
        type.text = exermon.typeText();
        nickname.text = playerExer.name();
        description.text = exermon.description;
    }

    /// <summary>
    /// 绘制经验信息
    /// </summary>
    /// <param name="slotItem">艾瑟萌槽项</param>
    void drawExpInfo(PlayerExermon playerExer) {
        expBar.setValue(playerExer, "exp");
    }

    /// <summary>
    /// 清除物品
    /// </summary>
    protected override void clearItem() {
        animal.text = type.text = "";
        nickname.text = description.text = "";
        full.gameObject.SetActive(false);
        expBar.clearValue();
        stars.clearValue();
    }

    #endregion

    #region 流程控制

    /// <summary>
    /// 修改昵称点击回调
    /// </summary>
    public void onEditClick() {
        nickname.enabled = false;
        editButton.SetActive(false);
        nicknameInput.startView(item.playerExer.name());
        nicknameInput.activate();
    }

    /// <summary>
    /// 结束昵称输入
    /// </summary>
    void terminateNicknameInput() {
        nickname.enabled = true;
        editButton.SetActive(true);
        nicknameInput.terminateView();
        requestRefresh();
    }

    /// <summary>
    /// 昵称改变回调事件
    /// </summary>
    public void onNicknameChanged(string value) {
        var name = item.playerExer.name();
        requestRename(value == "" ? name : value);
    }

    #endregion

    #region 请求控制

    /// <summary>
    /// 请求更改昵称
    /// </summary>
    void requestRename(string nickname) {
        scene.pushRequestItem(RenameOperText, () => {
            exerSer.rename(equip, nickname, terminateNicknameInput);
        }, true);
    }

    /// <summary>
    /// 请求更改艾瑟萌
    /// </summary>
    void requestEquip() {
        scene.pushRequestItem(EquipOperText, () => {
            exerSer.exerSlotEquip(item, equip, item.playerGift);
        }, true);
    }

    #endregion
}
