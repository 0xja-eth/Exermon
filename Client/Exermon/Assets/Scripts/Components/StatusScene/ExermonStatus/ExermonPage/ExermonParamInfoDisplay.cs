
using UnityEngine;
/// <summary>
/// 状态窗口艾瑟萌页属性信息显示
/// </summary>
public class ExermonParamInfoDisplay : ItemDisplay<ExerSlotItem> {

    /// <summary>
    /// 外部组件设置
    /// </summary>
    public ParamDisplaysGroup paramInfo;
    public ParamDisplay battlePoint;

    /// <summary>
    /// 外部系统设置
    /// </summary>

    #region 初始化

    /// <summary>
    /// 初始化
    /// </summary>
    public override void configure() {
        base.configure();
        var params_ = DataService.get().staticData.configure.baseParams;
        paramInfo.configure(params_);
    }

    #endregion

    #region 界面绘制

    /// <summary>
    /// 绘制物品
    /// </summary>
    /// <param name="slotItem">艾瑟萌槽项</param>
    protected override void drawExactlyItem(ExerSlotItem slotItem) {
        base.drawExactlyItem(slotItem);

        Debug.Log("drawExactlyItem:" + slotItem);

        paramInfo.setValues(slotItem, "params");
        battlePoint.setValue(slotItem, "battle_point");

        paramInfo.setIgnoreTrigger();
    }

    /// <summary>
    /// 清除物品
    /// </summary>
    protected override void clearItem() {
        paramInfo.clearValues();
        battlePoint.clearValue();

        paramInfo.setIgnoreTrigger();
    }

    #endregion
    
}
