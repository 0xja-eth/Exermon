
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using LitJson;
using UnityEditor;
using System.Linq;

#region 物品

/// <summary>
/// 艾瑟萌数据
/// </summary>
public class Exermon : BaseItem, ParamDisplay.DisplayDataArrayConvertable {

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public string animal { get; protected set; }
    [AutoConvert]
    public int starId { get; protected set; }
    [AutoConvert]
    public int subjectId { get; protected set; }
    [AutoConvert]
    public int eType { get; protected set; }

    [AutoConvert]
    public ParamData[] baseParams { get; protected set; }
    [AutoConvert]
    public ParamData[] rateParams { get; protected set; }

    public Texture2D full { get; protected set; }
    public Texture2D icon { get; protected set; }
    public Texture2D battle { get; protected set; }

    /// <summary>
    /// 后续增加属性
    /// </summary>
    public List<ExerSkill> exerSkills { get; protected set; } = new List<ExerSkill>();
    public ExerFrag exerFrag { get; protected set; } = null;

    #region 属性显示数据生成

    /// <summary>
    /// 转化为属性信息集
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>属性信息集</returns>
    public JsonData[] convertToDisplayDataArray(string type = "") {
        var count = baseParams.Length;
        var data = new JsonData[count];
        for (int i = 0; i < count; i++) 
            data[i] = convertParamToDisplayData(i, type);
        return data;
    }

    /// <summary>
    /// 转化一个属性为属性信息
    /// </summary>
    /// <param name="index">索引</param>
    /// <param name="type">类型</param>
    /// <returns>属性信息</returns>
    JsonData convertParamToDisplayData(int index, string type = "") {
        switch (type.ToLower()) {
            case "growth": return convertGrowth(index);
            default: return convertBasic(index);
        }
    }

    /// <summary>
    /// 转化基础属性
    /// </summary>
    /// <param name="index">索引</param>
    /// <returns>属性信息集</returns>
    JsonData convertBasic(int index) {
        var json = new JsonData();

        var param = baseParams[index].param();
        var percent = param.isPercent();

        var value = baseParams[index].value;
        var growth = rateParams[index].value;

        var max = star().baseRanges[index].maxValue;

        json["max"] = DataLoader.convertDouble(max, !percent, percent ? 4 : 2);
        json["value"] = DataLoader.convertDouble(value, !percent, percent ? 4 : 2);
        json["growth"] = DataLoader.convertDouble(growth);
        json["rate"] = value / max;

        return json;
    }

    /// <summary>
    /// 转化成长率属性
    /// </summary>
    /// <param name="index">索引</param>
    /// <returns>属性信息集</returns>
    JsonData convertGrowth(int index) {
        var json = new JsonData();
        var rate = rateParams[index];
        var max = star().rateRanges[index].maxValue;
        var value = rate.value;

        json["max"] = max;
        json["value"] = value;
        json["rate"] = value / max;

        return json;
    }

    #endregion

    #region 数据操作

    /// <summary>
    /// 获取艾瑟萌星级
    /// </summary>
    /// <returns>艾瑟萌星级</returns>
    public ExerStar star() {
        return DataService.get().exerStar(starId);
    }

    /// <summary>
    /// 获取科目
    /// </summary>
    /// <returns>科目</returns>
    public Subject subject() {
        return DataService.get().subject(subjectId);
    }

    /// <summary>
    /// 获取类型文本
    /// </summary>
    /// <returns>科目</returns>
    public string typeText() {
        return DataService.get().exermonType(eType).Item2;
    }

    /// <summary>
    /// 设置艾瑟萌碎片
    /// </summary>
    /// <param name="frag">碎片</param>
    public void setExerFrag(ExerFrag frag) {
        if (exerFrag == null) exerFrag = frag;
    }

    /// <summary>
    /// 增加艾瑟萌技能
    /// </summary>
    /// <param name="skill">技能</param>
    public void addExerSkill(ExerSkill skill) {
        if (exerSkills.Contains(skill)) return;
        if (exerSkills.Count < 3) exerSkills.Add(skill);
    }

    #region 属性操作

    /// <summary>
    /// 获取属性基础值
    /// </summary>
    /// <param name="paramId">属性ID</param>
    /// <returns>属性数据</returns>
    public ParamData paramBase(int paramId) {
        foreach (var param in baseParams)
            if (param.paramId == paramId) return param;
        return new ParamData(paramId);
    }

    /// <summary>
    /// 获取属性成长率
    /// </summary>
    /// <param name="paramId">属性ID</param>
    /// <returns>属性数据</returns>
    public ParamData paramRate(int paramId) {
        foreach (var param in rateParams)
            if (param.paramId == paramId) return param;
        return new ParamData(paramId);
    }

    #endregion

    #endregion
    
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    protected override void loadCustomAttributes(JsonData json) {
        base.loadCustomAttributes(json);
        full = AssetLoader.loadExermonFull(getID());
        icon = AssetLoader.loadExermonIcon(getID());
        battle = AssetLoader.loadExermonBattle(getID());
    }
}

/// <summary>
/// 艾瑟萌碎片数据
/// </summary>
public class ExerGift : BaseItem, ParamDisplay.DisplayDataArrayConvertable {

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public int starId { get; protected set; }
    [AutoConvert]
    public int gType { get; protected set; }
    [AutoConvert]
    public Color color { get; protected set; }
    [AutoConvert("params")]
    public ParamData[] params_ { get; protected set; }

    public Texture2D icon { get; protected set; }
    public Texture2D bigIcon { get; protected set; }

    #region 属性显示数据生成

    /// <summary>
    /// 转化为属性信息集
    /// </summary>
    /// <returns>属性信息集</returns>
    public JsonData[] convertToDisplayDataArray(string type = "") {
        var count = params_.Length;
        var data = new JsonData[count];
        for (int i = 0; i < count; i++) {
            var json = new JsonData();
            var base_ = params_[i];
            var max = (float)star().paramRanges[i].maxValue;
            var value = (float)base_.value;
            json["max"] = max;
            json["value"] = value;
            json["rate"] = value / max;
            json["color"] = DataLoader.convert(color);
            data[i] = json;
        }
        return data;
    }

    #endregion

    #region 数据操作

    /// <summary>
    /// 获取艾瑟萌天赋星级
    /// </summary>
    /// <returns>艾瑟萌天赋星级</returns>
    public ExerGiftStar star() {
        return DataService.get().exerGiftStar(starId);
    }

    /// <summary>
    /// 获取属性成长加成率
    /// </summary>
    /// <param name="paramId">属性ID</param>
    /// <returns>属性数据</returns>
    public ParamData paramRate(int paramId) {
        foreach (var param in params_)
            if (param.paramId == paramId) return param;
        return new ParamData(paramId);
    }

    #endregion

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    protected override void loadCustomAttributes(JsonData json) {
        base.loadCustomAttributes(json);
        icon = AssetLoader.loadExerGift(getID());
        bigIcon = AssetLoader.loadBigExerGift(getID());
    }
}

/// <summary>
/// 艾瑟萌碎片数据
/// </summary>
public class ExerFrag : BaseItem {

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public int exermonId { get; protected set; }
    [AutoConvert]
    public int sellPrice { get; protected set; }
    [AutoConvert]
    public int count { get; protected set; }

    /// <summary>
    /// 获取艾瑟萌
    /// </summary>
    /// <returns>艾瑟萌</returns>
    public Exermon exermon() {
        return DataService.get().exermon(exermonId);
    }

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    protected override void loadCustomAttributes(JsonData json) {
        base.loadCustomAttributes(json);

        var exer = exermon();
        if (exer != null) exer.setExerFrag(this);
    }

    /// <summary>
    /// 最大叠加数量
    /// </summary>
    /// <returns></returns>
    public override int maxCount() { return 0; }

}

/// <summary>
/// 艾瑟萌碎片数据
/// </summary>
public class ExerSkill : BaseItem {

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public int exermonId { get; protected set; }
    [AutoConvert]
    public bool passive { get; protected set; }
    [AutoConvert]
    public int nextSkillId { get; protected set; }
    [AutoConvert]
    public int needCount { get; protected set; }
    [AutoConvert]
    public int mpCost { get; protected set; }
    [AutoConvert]
    public int rate { get; protected set; }
    [AutoConvert]
    public int freeze { get; protected set; }
    [AutoConvert]
    public int maxUseCount { get; protected set; }
    [AutoConvert]
    public int target { get; protected set; }
    [AutoConvert]
    public int hitType { get; protected set; }
    [AutoConvert]
    public int atkRate { get; protected set; }
    [AutoConvert]
    public int defRate { get; protected set; }

    public Texture2D icon { get; protected set; }
    public Texture2D ani { get; protected set; }
    public Texture2D targetAni { get; protected set; }

    #region 数据操作

    /// <summary>
    /// 使用效果
    /// </summary>
    public List<EffectData> effects { get; protected set; }

    /// <summary>
    /// 获取艾瑟萌
    /// </summary>
    /// <returns>艾瑟萌</returns>
    public Exermon exermon() {
        return DataService.get().exermon(exermonId);
    }

    /// <summary>
    /// 获取下一级技能
    /// </summary>
    /// <returns>下一级技能</returns>
    public ExerSkill nextSkill() {
        return DataService.get().exerSkill(nextSkillId);
    }

    /// <summary>
    /// 获取目标类型文本
    /// </summary>
    /// <returns>目标类型文本</returns>
    public string targetText() {
        return DataService.get().exerSkillTargetType(target).Item2;
    }

    /// <summary>
    /// 获取命中类型文本
    /// </summary>
    /// <returns>命中类型文本</returns>
    public string hitTypeText() {
        return DataService.get().exerSkillHitType(hitType).Item2;
    }

    #endregion

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    protected override void loadCustomAttributes(JsonData json) {
        base.loadCustomAttributes(json);

        icon = AssetLoader.loadExerSkillIcon(getID());
        ani = AssetLoader.loadExerSkillAni(getID());
        targetAni = AssetLoader.loadExerSkillTarget(getID());

        var exer = exermon();
        if (exer != null) exer.addExerSkill(this);
    }
}

/// <summary>
/// 艾瑟萌物品
/// </summary>
public class ExerItem : UsableItem {

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public int rate { get; protected set; }
}

/// <summary>
/// 艾瑟萌装备
/// </summary>
public class ExerEquip : EquipableItem {

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public int eType { get; protected set; }

    /// <summary>
    /// 获取装备类型
    /// </summary>
    /// <returns>装备类型</returns>
    public TypeData equipType() {
        return DataService.get().exerEquipType(eType);
    }
}

#endregion 

#region 容器项

/// <summary>
/// 艾瑟萌背包物品
/// </summary>
public class ExerPackItem : PackContItem<ExerItem> {

    /// <summary>
    /// 容器项容量（0为无限）
    /// </summary>
    /// <returns></returns>
    public override int capacity() { return item().maxCount(); }

}

/// <summary>
/// 艾瑟萌背包装备
/// </summary>
public class ExerPackEquip : PackContItem<ExerEquip> {
    
    /// <summary>
    /// 获取装备实例
    /// </summary>
    /// <returns>装备</returns>
    public ExerEquip equip() { return item(); }

    /// <summary>
    /// 获取装备的所有属性
    /// </summary>
    /// <returns>属性数据数组</returns>
    public ParamData[] getParams() {
        var equip = this.equip();
        if (equip == null) return new ParamData[0];
        return equip.params_;
    }

    /// <summary>
    /// 获取装备的属性
    /// </summary>
    /// <param name="paramId">属性ID</param>
    /// <returns>属性数据</returns>
    public ParamData getParam(int paramId) {
        var equip = this.equip();
        if (equip == null) return new ParamData(paramId);
        return equip.getParam(paramId);
    }

}

/// <summary>
/// 艾瑟萌碎片背包项
/// </summary>
public class ExerFragPackItem : PackContItem<ExerFrag> { }

/// <summary>
/// 艾瑟萌
/// </summary>
public class PlayerExermon : PackContItem<Exermon>,
    ParamDisplay.DisplayDataConvertable, 
    ParamDisplay.DisplayDataArrayConvertable {

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public string nickname { get; protected set; }
    [AutoConvert]
    public int exp { get; protected set; }
    [AutoConvert]
    public int next { get; protected set; }
    [AutoConvert]
    public int level { get; protected set; }

    [AutoConvert]
    public ExerSkillSlot exerSkillSlot { get; protected set; } = new ExerSkillSlot();

    [AutoConvert]
    public ParamData[] paramValues { get; protected set; }
    [AutoConvert]
    public ParamData[] rateParams { get; protected set; }
    
    #region 属性显示数据生成

    /// <summary>
    /// 转化为属性信息
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>属性信息</returns>
    public JsonData convertToDisplayData(string type = "") {
        switch (type.ToLower()) {
            case "exp": return convertExp();
            case "battle_point": return covnertBattlePoint();
            default: return toJson();
        }
    }

    /// <summary>
    /// 转化经验信息
    /// </summary>
    /// <returns></returns>
    JsonData convertExp() {
        var json = new JsonData();
        json["exp"] = exp;
        json["next"] = next;
        json["level"] = level;
        json["rate"] = exp*1.0 / next;
        return json;
    }

    /// <summary>
    /// 转化战斗力信息
    /// </summary>
    /// <returns></returns>
    JsonData covnertBattlePoint() {
        var json = new JsonData();
        json["battle_point"] = battlePoint();
        return json;
    }

    /// <summary>
    /// 转化为属性信息集
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>属性信息集</returns>
    public JsonData[] convertToDisplayDataArray(string type = "") {
        var count = paramValues.Length;
        var data = new JsonData[count];
        for (int i = 0; i < count; i++)
            data[i] = convertParamToDisplayData(i, type);
        return data;
    }

    /// <summary>
    /// 转化一个属性为属性信息
    /// </summary>
    /// <param name="index">索引</param>
    /// <param name="type">类型</param>
    /// <returns>属性信息</returns>
    JsonData convertParamToDisplayData(int index, string type = "") {
        switch (type.ToLower()) {
            case "params": return convertParams(index);
            case "growth": return convertGrowth(index);
            default: return convertBasic(index);
        }
    }

    /// <summary>
    /// 转化基础属性
    /// </summary>
    /// <param name="index">索引</param>
    /// <returns>属性信息集</returns>
    JsonData convertBasic(int index) {
        var json = new JsonData();
        var param = paramValues[index].param();
        var percent = param.isPercent();

        var value = paramValues[index].value;
        var growth = rateParams[index].value;

        json["value"] = DataLoader.convertDouble(value, !percent, percent ? 4 : 2);
        json["growth"] = DataLoader.convertDouble(growth);

        return json;
    }

    /// <summary>
    /// 转化成长率属性
    /// </summary>
    /// <param name="index">索引</param>
    /// <returns>属性信息集</returns>
    JsonData convertGrowth(int index) {
        var json = new JsonData();
        var rate = rateParams[index];
        var max = exermon().star().rateRanges[index].maxValue;
        var value = rate.value;
        json["max"] = max;
        json["value"] = value;
        json["rate"] = value / max;
        return json;
    }

    /// <summary>
    /// 转化属性信息
    /// </summary>
    /// <returns></returns>
    JsonData convertParams(int index) {
        var json = new JsonData();
        var exermon = this.exermon();
        var param = paramValues[index].param();
        var percent = param.isPercent();

        var value = paramValues[index].value;
        var growth = rateParams[index].value;

        var baseValue = exermon.baseParams[index].value;
        var baseGrowth = exermon.rateParams[index].value;

        var levelValue = CalcService.ExermonParamCalc.
            calc(baseValue, baseGrowth, level) - baseValue;

        json["value"] = DataLoader.convertDouble(value, !percent, percent ? 4 : 2);
        json["growth"] = DataLoader.convertDouble(growth);

        json["base_value"] = DataLoader.convertDouble(baseValue, !percent, percent ? 4 : 2);
        json["level_value"] = DataLoader.convertDouble(levelValue, !percent, percent ? 4 : 2);
        json["equip_value"] = 0; json["gift_value"] = 0;

        return json;
    }

    #endregion

    #region 数据操作

    /// <summary>
    /// 获取艾瑟萌
    /// </summary>
    /// <returns></returns>
    public Exermon exermon() { return item(); }

    /// <summary>
    /// 获取名称
    /// </summary>
    /// <returns></returns>
    public string name() {
        return nickname.Length > 0 ? nickname : exermon().name;
    }

    /// <summary>
    /// 更改昵称
    /// </summary>
    /// <param name="name"></param>
    public void rename(string name) {
        nickname = name;
    }

    #region 属性操作

    /// <summary>
    /// 获取属性基础值
    /// </summary>
    /// <param name="paramId">属性ID</param>
    /// <returns>属性数据</returns>
    public ParamData paramBase(int paramId) {
        return exermon().paramBase(paramId);
    }

    /// <summary>
    /// 获取属性成长率
    /// </summary>
    /// <param name="paramId">属性ID</param>
    /// <returns>属性数据</returns>
    public ParamData paramRate(int paramId) {
        return exermon().paramRate(paramId);
    }

    /// <summary>
    /// 获取实际属性值
    /// </summary>
    /// <param name="paramId">属性ID</param>
    /// <returns>属性数据</returns>
    public ParamData paramValue(int paramId) {
        var base_ = paramBase(paramId).value;
        var rate = paramRate(paramId).value;
        var value = CalcService.ExermonParamCalc.calc(base_, rate, level);
        value = Math.Round(value);
        return new ParamData(paramId, value);
    }

    /// <summary>
    /// 重新计算属性
    /// </summary>
    public void recomputeParams() {
        foreach (var param in paramValues)
            param.setValue(paramValue(param.paramId).value);
    }

    /// <summary>
    /// 战斗力
    /// </summary>
    /// <returns></returns>
    public int battlePoint() {
        return CalcService.BattlePointCalc.calc(paramValue);
    }

    #endregion

    #endregion

    /// <summary>
    /// 构造函数
    /// </summary>
    public PlayerExermon() { }
    /// <param name="itemId">物品ID</param>
    public PlayerExermon(int itemId) : base(itemId) { }

}

/// <summary>
/// 艾瑟萌天赋
/// </summary>
public class PlayerExerGift : PackContItem<ExerGift> {

    /// <summary>
    /// 获取艾瑟萌
    /// </summary>
    /// <returns></returns>
    public ExerGift exerGift() { return item(); }

    /// <summary>
    /// 构造函数
    /// </summary>
    public PlayerExerGift() { }
    /// <param name="itemId">物品ID</param>
    public PlayerExerGift(int itemId) : base(itemId) { }

}

/// <summary>
/// 艾瑟萌装备槽项
/// </summary>
public class ExerSlotItem : SlotContItem<PlayerExermon, PlayerExerGift>,
    ParamDisplay.DisplayDataConvertable,
    ParamDisplay.DisplayDataArrayConvertable {

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public PlayerExermon playerExer { get; protected set; }
    [AutoConvert]
    public PlayerExerGift playerGift { get; protected set; }
    [AutoConvert]
    public int subjectId { get; protected set; }
    [AutoConvert]
    public int exp { get; protected set; }
    [AutoConvert]
    public int next { get; protected set; }
    [AutoConvert]
    public int level { get; protected set; }

    [AutoConvert]
    public ExerEquipSlot exerEquipSlot { get; protected set; }

    [AutoConvert]
    public ParamData[] paramValues { get; protected set; }
    [AutoConvert]
    public ParamData[] rateParams { get; protected set; }

    /// <summary>
    /// 装备
    /// </summary>
    public override PlayerExermon equip1 {
        get { return playerExer; }
        protected set { playerExer = value; }
    }
    public override PlayerExerGift equip2 {
        get { return playerGift; }
        protected set { playerGift = value; }
    }

    /// <summary>
    /// 复制的对象，用于生成装备预览
    /// </summary>
    public ExerSlotItem previewObj { get; set; } = null;

    #region 属性显示数据生成

    #region 单信息数据生成

    /// <summary>
    /// 转化为属性信息
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>属性信息</returns>
    public JsonData convertToDisplayData(string type = "") {
        switch (type.ToLower()) {
            case "exp": return convertExp();
            case "slot_exp": return convertSlotExp();
            case "battle_point": return covnertBattlePoint();
            case "preview_battle_point": return covnertBattlePoint(true);
            default: return toJson();
        }
    }

    /// <summary>
    /// 转化经验信息
    /// </summary>
    /// <returns></returns>
    JsonData convertExp() {
        var json = new JsonData();
        json["exp"] = playerExer.exp;
        json["next"] = playerExer.next;
        json["level"] = playerExer.level;
        json["rate"] = playerExer.exp / playerExer.next;
        return json;
    }

    /// <summary>
    /// 转化槽经验信息
    /// </summary>
    /// <returns></returns>
    JsonData convertSlotExp() {
        var json = new JsonData();
        json["exp"] = exp;
        json["next"] = next;
        json["level"] = level;
        json["rate"] = exp / next;
        return json;
    }

    /// <summary>
    /// 转化战斗力信息
    /// </summary>
    /// <returns></returns>
    JsonData covnertBattlePoint(bool preview = false) {
        var json = new JsonData();
        var bp = battlePoint();
        if (preview && previewObj != null) {
            json = previewObj.covnertBattlePoint();
            var bp2 = DataLoader.load<int>(json, "battle_point");
            json["delta_battle_point"] = bp2 - bp;
        } else
            json["battle_point"] = bp;
        
        return json;
    }

    #endregion

    #region 多信息数据生成

    /// <summary>
    /// 转化为属性信息集
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>属性信息集</returns>
    public JsonData[] convertToDisplayDataArray(string type = "") {
        var count = paramValues.Length;
        var data = new JsonData[count];
        for (int i = 0; i < count; i++)
            data[i] = convertParamToDisplayData(i, type);
        return data;
    }

    /// <summary>
    /// 转化一个属性为属性信息
    /// </summary>
    /// <param name="index">索引</param>
    /// <param name="type">类型</param>
    /// <returns>属性信息</returns>
    JsonData convertParamToDisplayData(int index, string type = "") {
        switch (type.ToLower()) {
            case "params": return convertParams(index);
            case "preview_params": return convertParams(index, true);
            case "growth": return convertGrowth(index);
            default: return convertBasic(index);
        }
    }

    /// <summary>
    /// 转化基础属性
    /// </summary>
    /// <param name="index">索引</param>
    /// <returns>属性信息集</returns>
    JsonData convertBasic(int index) {
        var json = new JsonData();
        var base_ = paramValues[index];
        var rate = rateParams[index];
        var value = base_.value;
        json["value"] = value;
        json["growth"] = rate.value;
        return json;
    }

    /// <summary>
    /// 转化属性信息
    /// </summary>
    /// <returns></returns>
    JsonData convertParams(int index, bool preview = false) {
        var json = new JsonData();
        var param = paramValues[index].param();
        var percent = param.isPercent();

        var value = paramValues[index].value;
        var growth = rateParams[index].value;

        if(preview && previewObj != null) {

            json = previewObj.convertParams(index);

            var value2 = DataLoader.load<double>(json, "value");
            var growth2 = DataLoader.load<double>(json, "growth");

            var dtValue = value2 - value;
            var dtGrowth = growth2 - growth;

            json["delta_value"] = DataLoader.convertDouble(dtValue, !percent, percent ? 4 : 2);
            json["delta_growth"] = DataLoader.convertDouble(dtGrowth);

            json["delta_value_rate"] = dtValue / value;
            json["delta_growth_rate"] = dtGrowth / growth;

        } else {

            var exermon = this.exermon();
            var level = playerExer.level;

            var mGrowth = exermon.star().rateRanges[index].maxValue;

            var baseValue = exermon.baseParams[index].value;
            var baseGrowth = exermon.rateParams[index].value;

            var levelValue = CalcService.ExermonParamCalc.
                calc(baseValue, baseGrowth, level) - baseValue;
            var equipValue = exerEquipSlot.getParam(param.getID()).value;
            var giftValue = CalcService.ExermonParamCalc.
                calc(baseValue, growth, level) - levelValue - baseValue;

            json["value"] = DataLoader.convertDouble(value, !percent, percent ? 4 : 2);
            json["growth"] = DataLoader.convertDouble(growth);

            json["max_growth"] = mGrowth;
            json["growth_rate"] = growth / mGrowth;

            json["base_value"] = DataLoader.convertDouble(baseValue, !percent, percent ? 4 : 2);
            json["level_value"] = DataLoader.convertDouble(levelValue, !percent, percent ? 4 : 2);
            json["equip_value"] = DataLoader.convertDouble(equipValue, !percent, percent ? 4 : 2);
            json["gift_value"] = DataLoader.convertDouble(giftValue, !percent, percent ? 4 : 2);
        }
        
        return json;
    }

    /// <summary>
    /// 转化成长率属性
    /// </summary>
    /// <param name="index">索引</param>
    /// <returns>属性信息集</returns>
    JsonData convertGrowth(int index) {
        var json = new JsonData();
        var value = rateParams[index].value;
        var ori = playerExer.rateParams[index].value;
        var max = exermon().star().rateRanges[index].maxValue;
        var delta = value - ori;
        var deltaRate = delta / value;
        var gift = exerGift();
        var color = new Color(1, 1, 1);
        if (gift != null) color = gift.color;

        json["max"] = max;
        json["value"] = value;
        json["rate"] = value / max;
        json["delta"] = delta;
        json["delta_rate"] = deltaRate;

        json[ParamDisplay.TrueColorKey] = DataLoader.convert(color);

        return json;
    }

    #endregion

    #endregion

    #region 数据操作

    /// <summary>
    /// 获取科目
    /// </summary>
    /// <returns>科目</returns>
    public Subject subject() {
        return DataService.get().subject(subjectId);
    }

    /// <summary>
    /// 获取艾瑟萌
    /// </summary>
    /// <returns></returns>
    public Exermon exermon() {
        if (playerExer == null) return null;
        return playerExer.exermon();
    }

    /// <summary>
    /// 获取艾瑟萌天赋
    /// </summary>
    /// <returns>艾瑟萌天赋</returns>
    public ExerGift exerGift() {
        if (playerGift == null) return null;
        return playerGift.exerGift();
    }

    /// <summary>
    /// 设置艾瑟萌
    /// </summary>
    /// <param name="exermon">艾瑟萌</param>
    public void setExermon(Exermon exermon) {
        setEquip(new PlayerExermon(exermon.getID()));
    }

    /// <summary>
    /// 设置艾瑟萌天赋
    /// </summary>
    /// <param name="exerGift">艾瑟萌天赋</param>
    public void setExerGift(ExerGift exerGift) {
        setEquip(new PlayerExerGift(exerGift.getID()));
    }
    
    /// <summary>
    /// 装备改变回调
    /// </summary>
    protected override void onEquipChanged() {
        base.onEquipChanged();
        recomputeParams();
    }
    
    #region 属性操作

    /// <summary>
    /// 获取属性基础值
    /// </summary>
    /// <param name="paramId">属性ID</param>
    /// <returns>属性数据</returns>
    public ParamData paramBase(int paramId) {
        return playerExer.paramBase(paramId);
    }

    /// <summary>
    /// 获取属性成长率
    /// </summary>
    /// <param name="paramId">属性ID</param>
    /// <returns>属性数据</returns>
    public ParamData paramRate(int paramId) {
        var value = CalcService.ExerSlotItemParamRateCalc.calc(this, paramId);
        return new ParamData(paramId, value);
    }

    /// <summary>
    /// 获取实际属性值
    /// </summary>
    /// <param name="paramId">属性ID</param>
    /// <returns>属性数据</returns>
    public ParamData paramValue(int paramId) {
        var value = CalcService.ExerSlotItemParamCalc.calc(this, paramId);
        return new ParamData(paramId, value);
    }

    /// <summary>
    /// 重新计算属性
    /// </summary>
    public void recomputeParams() {
        foreach (var param in paramValues)
            param.setValue(paramValue(param.paramId).value);
        foreach (var param in rateParams)
            param.setValue(paramRate(param.paramId).value);
    }

    /// <summary>
    /// 战斗力
    /// </summary>
    /// <returns></returns>
    public int battlePoint() {
        return CalcService.BattlePointCalc.calc(paramValue);
    }

    #endregion

    #region 预览操作

    /// <summary>
    /// 创建预览对象
    /// </summary>
    /// <returns></returns>
    public ExerSlotItem createPreviewObject() {
        return previewObj = (ExerSlotItem)copy();
    }

    /// <summary>
    /// 清除预览对象
    /// </summary>
    public void clearPreviewObject() {
        previewObj = null;
    }

    /// <summary>
    /// 设置预览艾瑟萌
    /// </summary>
    /// <param name="playerExer"></param>
    public void setPlayerExerPreview(PlayerExermon playerExer) {
        if (previewObj == null) createPreviewObject();
        previewObj.setEquip(playerExer);
    }

    /// <summary>
    /// 设置预览艾瑟萌天赋
    /// </summary>
    /// <param name="playerExer"></param>
    public void setPlayerGiftPreview(PlayerExerGift playerGift) {
        if (previewObj == null) createPreviewObject();
        previewObj.setEquip(playerGift);
    }

    #endregion

    #endregion

    /// <summary>
    /// 构造函数
    /// </summary>
    public ExerSlotItem() {
        exerEquipSlot = new ExerEquipSlot(this);
    }

    /// <summary>
    /// 读取自定义属性
    /// </summary>
    /// <param name="json"></param>
    protected override void loadCustomAttributes(JsonData json) {
        base.loadCustomAttributes(json);

        if (playerExer != null) playerExer.equiped = true;
        if (playerGift != null) playerGift.equiped = true;
    }
}

/// <summary>
/// 艾瑟萌装备槽项
/// </summary>
public class ExerEquipSlotItem : SlotContItem<ExerPackEquip> {

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public ExerPackEquip packEquip { get; protected set; }
    [AutoConvert]
    public int eType { get; protected set; }
    /// <summary>
    /// 装备
    /// </summary>
    public override ExerPackEquip equip1 {
        get { return packEquip; }
        protected set { packEquip = value; }
    }

    /// <summary>
    /// 获取装备类型
    /// </summary>
    /// <returns>装备类型</returns>
    public TypeData equipType() {
        return DataService.get().exerEquipType(eType);
    }

    /// <summary>
    /// 获取装备类型
    /// </summary>
    /// <returns>装备类型</returns>
    public ExerEquip equip() {
        return packEquip.equip();
    }

    /// <summary>
    /// 获取装备的所有属性
    /// </summary>
    /// <returns>属性数据数组</returns>
    public ParamData[] getParams() {
        if (packEquip == null) return new ParamData[0];
        return packEquip.getParams();
    }

    /// <summary>
    /// 获取装备的属性
    /// </summary>
    /// <param name="paramId">属性ID</param>
    /// <returns>属性数据</returns>
    public ParamData getParam(int paramId) {
        if (packEquip == null) return new ParamData(paramId);
        return packEquip.getParam(paramId);
    }
}

/// <summary>
/// 艾瑟萌技能槽项
/// </summary>
public class ExerSkillSlotItem : SlotContItem {

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public int skillId { get; protected set; }
    [AutoConvert]
    public int useCount { get; protected set; }

    /// <summary>
    /// 获取科目
    /// </summary>
    /// <returns>科目</returns>
    public ExerSkill skill() {
        return DataService.get().exerSkill(skillId);
    }
}

#endregion

#region 容器

/// <summary>
/// 艾瑟萌背包
/// </summary>
public class ExerPack : PackContainer<PackContItem> {

    /// <summary>
    /// 读取单个物品
    /// </summary>
    /// <param name="json"></param>
    protected override PackContItem loadItem(JsonData json) {
        var type = DataLoader.load<int>(json, "type");
        if (type == (int)BaseContItem.Type.ExerPackItem)
            return DataLoader.load<ExerPackItem>(json);
        if (type == (int)BaseContItem.Type.ExerPackEquip)
            return DataLoader.load<ExerPackEquip>(json);
        return null;
    }

}

/// <summary>
/// 艾瑟萌仓库
/// </summary>
public class ExerHub : PackContainer<PlayerExermon> { }

/// <summary>
/// 艾瑟萌仓库
/// </summary>
public class ExerGiftPool : PackContainer<PlayerExerGift> { }

/// <summary>
/// 人类装备槽
/// </summary>
public class ExerSlot : SlotContainer<ExerSlotItem> {

    #region 数据操作

    #region 属性操作

    /// <summary>
    /// 获取属性总和
    /// </summary>
    /// <param name="index">属性索引</param>
    /// <returns></returns>
    public ParamData sumParam(int paramId) {
        ParamData sum = new ParamData(paramId, 0);
        foreach (var item in items)
            sum += item.paramValue(paramId);
        return sum;
    }

    /// <summary>
    /// 获取属性平均值
    /// </summary>
    /// <param name="index">属性索引</param>
    /// <returns></returns>
    public ParamData avgParam(int paramId) {
        return sumParam(paramId) / items.Count;
    }

    /// <summary>
    /// 总战斗力
    /// </summary>
    /// <returns></returns>
    public int sumBattlePoint() {
        int sum = 0;
        foreach (var item in items)
            sum += item.battlePoint();
        return sum;
    }

    #endregion

    /// <summary>
    /// 获取所选科目
    /// </summary>
    /// <returns></returns>
    public Subject[] subjects() {
        var cnt = items.Count;
        var sbjs = new Subject[cnt];
        for (int i = 0; i < cnt; i++)
            sbjs[i] = items[i].subject();
        return sbjs;
    }

    /// <summary>
    /// 获取艾瑟萌槽项
    /// </summary>
    /// <param name="sid">科目ID</param>
    /// <returns>艾瑟萌槽项数据</returns>
    public ExerSlotItem getExerSlotItem(int sid) {
        return getItem((item) => item.subjectId == sid);
    }
    
    /// <summary>
    /// 装备艾瑟萌天赋
    /// </summary>
    /// <param name="sid">科目ID</param>
    /// <param name="exerGiftPool">艾瑟萌天赋池</param>
    /// <param name="playerGift">玩家艾瑟萌天赋</param>
    public void setEquip(int sid, ExerGiftPool exerGiftPool, PlayerExerGift playerGift) {
        setEquip(getExerSlotItem(sid), exerGiftPool, playerGift);
    }
    public void setEquip(int sid, PlayerExerGift playerGift) {
        setEquip(getExerSlotItem(sid), playerGift);
    }

    /// <summary>
    /// 通过装备物品获取槽ID
    /// </summary>
    /// <typeparam name="E">装备物品类型</typeparam>
    /// <param name="equipItem">装备物品</param>
    /// <returns>槽ID</returns>
    protected override ExerSlotItem getSlotItemByEquipItem<E>(E equipItem) {
        if(typeof(E) == typeof(PlayerExermon)) {
            var sid = ((PlayerExermon)(object)equipItem).exermon().subjectId;
            return getExerSlotItem(sid);
        }
        return null;
    }

    #endregion
}

/// <summary>
/// 艾瑟萌装备槽
/// </summary>
public class ExerEquipSlot : SlotContainer<ExerEquipSlotItem> {

    /// <summary>
    /// 对应的艾瑟萌槽项
    /// </summary>
    public ExerSlotItem exerSlotItem { get; protected set; } = null;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="exerSlotItem">所属的艾瑟萌槽项</param>
    public ExerEquipSlot(ExerSlotItem exerSlotItem) {
        this.exerSlotItem = exerSlotItem;
    }
    public ExerEquipSlot() {}

    #region 数据操作

    /// <summary>
    /// 获取装备项
    /// </summary>
    /// <param name="eType">装备类型</param>
    /// <returns>装备项数据</returns>
    public ExerEquipSlotItem getEquipSlotItem(int eType) {
        return getItem((item) => item.eType == eType);
    }

    /// <summary>
    /// 通过装备物品获取槽ID
    /// </summary>
    /// <typeparam name="E">装备物品类型</typeparam>
    /// <param name="equipItem">装备物品</param>
    /// <returns>槽ID</returns>
    protected override ExerEquipSlotItem getSlotItemByEquipItem<E>(E equipItem) {
        if (typeof(E) == typeof(ExerPackEquip)) {
            var eType = ((ExerPackEquip)(object)equipItem).equip().eType;
            return getEquipSlotItem(eType);
        }
        return null;
    }

    /// <summary>
    /// 获取装备的所有属性
    /// </summary>
    /// <returns>属性数据数组</returns>
    public ParamData[] getParams() {
        var params_ = new Dictionary<int, ParamData>();

        foreach (var item in items) {
            var itemParams = item.getParams();
            foreach (var param in itemParams) {
                var pid = param.paramId;
                if (params_.ContainsKey(pid))
                    params_[pid].addValue(param.value);
                else
                    params_[pid] = new ParamData(pid, param.value);
            }
        }

        return params_.Values.ToArray();
    }

    /// <summary>
    /// 获取装备的属性
    /// </summary>
    /// <param name="paramId">属性ID</param>
    /// <returns>属性数据</returns>
    public ParamData getParam(int paramId) {
        var param = new ParamData(paramId, 0);

        foreach (var item in items)
            param.addValue(item.getParam(paramId).value);

        return param;
    }

    #endregion
}

/// <summary>
/// 艾瑟萌装备槽
/// </summary>
public class ExerSkillSlot : SlotContainer<ExerSkillSlotItem> {
    
    /// <summary>
    /// 通过装备物品获取槽ID
    /// </summary>
    /// <typeparam name="E">装备物品类型</typeparam>
    /// <param name="equipItem">装备物品</param>
    /// <returns>槽ID</returns>
    protected override ExerSkillSlotItem getSlotItemByEquipItem<E>(E equipItem) {
        return null;
    }
}

#endregion