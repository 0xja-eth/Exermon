
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
        for (int i = 0; i < count; i++) {
            data[i] = convertParamToDisplayData(i, type);
            Debug.Log("Exermon.convertToDisplayDataArray["+i+"]: " + data[i].ToJson());
        }
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
        var base_ = baseParams[index];
        var rate = rateParams[index];
        var max = star().baseRanges[index].maxValue;
        var value = base_.value;
        if (base_.param().isPercent()) { // 如果是百分数属性
            max *= 100; value *= 100;
        }
        json["max"] = max;
        json["value"] = value;
        json["rate"] = value / max;
        json["growth"] = rate.value;
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
        /*
        name = DataLoader.loadString(json, "name");
        gender = DataLoader.loadInt(json, "gender");
        description = DataLoader.loadString(json, "description");
        */
        full = AssetLoader.loadExermonFull(getID());
        icon = AssetLoader.loadExermonIcon(getID());
        battle = AssetLoader.loadExermonBattle(getID());
    }

    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        animal = DataLoader.loadString(json, "animal");
        starId = DataLoader.loadInt(json, "star_id");
        subjectId = DataLoader.loadInt(json, "subject_id");
        eType = DataLoader.loadInt(json, "e_type");

        var paramRanges = DataLoader.loadJsonData(json, "params");

        baseParams = DataLoader.loadDataArray<ParamData>(paramRanges, "bases");
        rateParams = DataLoader.loadDataArray<ParamData>(paramRanges, "rates");

        full = AssetLoader.loadExermonFull(getID());
        icon = AssetLoader.loadExermonIcon(getID());
        battle = AssetLoader.loadExermonBattle(getID());
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["animal"] = animal;
        json["star_id"] = starId;
        json["subject_id"] = subjectId;
        json["e_type"] = eType;

        json["params"] = new JsonData();
        json["params"]["bases"] = DataLoader.convertDataArray(baseParams);
        json["params"]["rates"] = DataLoader.convertDataArray(rateParams);

        return json;
    }*/
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
        /*
        name = DataLoader.loadString(json, "name");
        gender = DataLoader.loadInt(json, "gender");
        description = DataLoader.loadString(json, "description");
        */
        icon = AssetLoader.loadExerGift(getID());
        bigIcon = AssetLoader.loadBigExerGift(getID());
    }

    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        starId = DataLoader.loadInt(json, "star_id");
        color = DataLoader.loadColor(json, "color");
        gType = DataLoader.loadInt(json, "g_type");
        params_ = DataLoader.loadDataArray<ParamData>(json, "params");

    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["star_id"] = starId;
        json["color"] = DataLoader.convertColor(color);
        json["g_type"] = gType;
        json["params"] = DataLoader.convertDataArray(params_);

        return json;
    }*/
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
    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        exermonId = DataLoader.loadInt(json, "eid");
        sellPrice = DataLoader.loadInt(json, "sell_price");
        count = DataLoader.loadInt(json, "count");

    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["eid"] = exermonId;
        json["sell_price"] = sellPrice;
        json["count"] = count;

        return json;
    }*/
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
    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        exermonId = DataLoader.loadInt(json, "eid");
        passive = DataLoader.loadBool(json, "passive");
        nextSkillId = DataLoader.loadInt(json, "next_skill_id");
        needCount = DataLoader.loadInt(json, "need_count");
        mpCost = DataLoader.loadInt(json, "mp_cost");
        rate = DataLoader.loadInt(json, "rate");
        freeze = DataLoader.loadInt(json, "freeze");
        maxUseCount = DataLoader.loadInt(json, "max_use_count");
        target = DataLoader.loadInt(json, "target");
        hitType = DataLoader.loadInt(json, "hit_type");
        atkRate = DataLoader.loadInt(json, "atk_rate");
        defRate = DataLoader.loadInt(json, "def_rate");

        effects = DataLoader.loadDataList<EffectData>(json, "effects");

    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["eid"] = exermonId;
        json["passive"] = passive;
        json["next_skill_id"] = nextSkillId;
        json["need_count"] = needCount;
        json["mp_cost"] = mpCost;
        json["rate"] = rate;
        json["freeze"] = freeze;
        json["max_use_count"] = maxUseCount;
        json["target"] = target;
        json["hit_type"] = hitType;
        json["atk_rate"] = atkRate;
        json["def_rate"] = defRate;

        json["effects"] = DataLoader.convertDataArray(effects);

        return json;
    }*/
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
    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        rate = DataLoader.loadInt(json, "rate");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["rate"] = rate;

        return json;
    }
    */
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
    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        eType = DataLoader.loadInt(json, "e_type");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["e_type"] = eType;

        return json;
    }*/
}

#endregion 

#region 容器项

/// <summary>
/// 艾瑟萌背包物品
/// </summary>
public class ExerPackContItem : PackContItem {

    /// <summary>
    /// 默认类型
    /// </summary>
    /// <returns></returns>
    public override Type defaultType() { return Type.Unset; }

    /// <summary>
    /// 物品
    /// </summary>
    /// <returns></returns>
    public LimitedItem item() {
        if (type == (int)Type.ExerPackItem)
            return DataService.get().exerItem(itemId);
        if (type == (int)Type.ExerPackEquip)
            return DataService.get().exerEquip(itemId);
        return null;
    }
}

/// <summary>
/// 艾瑟萌背包物品
/// </summary>
public class ExerPackItem : ExerPackContItem {

    /// <summary>
    /// 默认类型
    /// </summary>
    /// <returns></returns>
    public override Type defaultType() { return Type.ExerPackItem; }

    /// <summary>
    /// 物品
    /// </summary>
    /// <returns></returns>
    public new LimitedItem item() {
        return DataService.get().exerItem(itemId);
    }
}

/// <summary>
/// 艾瑟萌背包装备
/// </summary>
public class ExerPackEquip : ExerPackContItem {

    /// <summary>
    /// 默认类型
    /// </summary>
    /// <returns></returns>
    public override Type defaultType() { return Type.ExerPackEquip; }

    /// <summary>
    /// 获取装备实例
    /// </summary>
    /// <returns>装备</returns>
    public ExerEquip equip() {
        return DataService.get().exerEquip(itemId);
    }

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
public class ExerFragPackItem : PackContItem {

    /// <summary>
    /// 默认类型
    /// </summary>
    /// <returns></returns>
    public override Type defaultType() { return Type.ExerFragPackItem; }

    /// <summary>
    /// 物品
    /// </summary>
    /// <returns></returns>
    public ExerFrag frag() {
        return DataService.get().exerFrag(itemId);
    }
}

/// <summary>
/// 艾瑟萌
/// </summary>
public class PlayerExermon : PackContItem, ParamDisplay.DisplayDataArrayConvertable {

    /// <summary>
    /// 默认类型
    /// </summary>
    /// <returns></returns>
    public override Type defaultType() { return Type.PlayerExermon; }

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public string nickname { get; protected set; }
    [AutoConvert]
    public int exp { get; protected set; }
    [AutoConvert]
    public int level { get; protected set; }

    [AutoConvert]
    public SlotContainer<ExerSkillSlotItem> exerSkillSlot { get; protected set; }

    [AutoConvert]
    public ParamData[] paramValues { get; protected set; }
    [AutoConvert]
    public ParamData[] rateParams { get; protected set; }

    #region 属性显示数据生成

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
        if (base_.param().isPercent()) // 如果是百分数属性
            value *= 100;
        json["value"] = value;
        json["growth"] = rate.value;
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

    #endregion

    #region 数据操作

    /// <summary>
    /// 获取艾瑟萌
    /// </summary>
    /// <returns></returns>
    public Exermon exermon() {
        return DataService.get().exermon(itemId);
    }

    /// <summary>
    /// 获取名称
    /// </summary>
    /// <returns></returns>
    public string name() {
        return nickname.Length > 0 ? nickname : exermon().name;
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
            param.setValue(paramBase(param.paramId).value);
    }

    #endregion

    #endregion

    /// <summary>
    /// 构造函数
    /// </summary>
    public PlayerExermon() { }
    /// <param name="itemId">物品ID</param>
    public PlayerExermon(int itemId) : base(itemId) { }
    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        nickname = DataLoader.loadString(json, "nickname");
        exp = DataLoader.loadInt(json, "exp");
        level = DataLoader.loadInt(json, "level");

        exerSkillSlot = DataLoader.loadData<SlotContainer
            <ExerSkillSlotItem>>(json, "exerskillslot");

        var paramRanges = DataLoader.loadJsonData(json, "params");

        paramValues = DataLoader.loadDataArray<ParamData>(paramRanges, "values");
        rateParams = DataLoader.loadDataArray<ParamData>(paramRanges, "rates");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["nickname"] = nickname;
        json["exp"] = exp;
        json["level"] = level;
        json["exerskillslot"] = DataLoader.convertData(exerSkillSlot);

        json["params"] = new JsonData();
        json["params"]["values"] = DataLoader.convertDataArray(paramValues);
        json["params"]["rates"] = DataLoader.convertDataArray(rateParams);

        return json;
    }*/
}

/// <summary>
/// 艾瑟萌天赋
/// </summary>
public class PlayerExerGift : PackContItem {

    /// <summary>
    /// 默认类型
    /// </summary>
    /// <returns></returns>
    public override Type defaultType() { return Type.PlayerExerGift; }

    /// <summary>
    /// 获取艾瑟萌
    /// </summary>
    /// <returns></returns>
    public ExerGift exerGift() {
        return DataService.get().exerGift(itemId);
    }

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
public class ExerSlotItem : SlotContItem,
    // ParamDisplay.DisplayDataConvertable,
    ParamDisplay.DisplayDataArrayConvertable {

    /// <summary>
    /// 默认类型
    /// </summary>
    /// <returns></returns>
    public override Type defaultType() { return Type.ExerSlotItem; }

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
    public int level { get; protected set; }

    [AutoConvert]
    public ExerEquipSlot exerEquipSlot { get; protected set; }

    [AutoConvert]
    public ParamData[] paramValues { get; protected set; }
    [AutoConvert]
    public ParamData[] rateParams { get; protected set; }

    #region 属性显示数据生成
    /*
    /// <summary>
    /// 转化为属性信息
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns>属性信息</returns>
    public JsonData convertToDisplayData(string type = "") {
        return toJson();
    }
    */
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
        if (base_.param().isPercent()) // 如果是百分数属性
            value *= 100;
        json["value"] = value;
        json["growth"] = rate.value;
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

        Debug.Log("Growth: " + json.ToJson());

        return json;
    }

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
        playerExer = new PlayerExermon(exermon.getID());
        recomputeParams();
    }

    /// <summary>
    /// 设置艾瑟萌天赋
    /// </summary>
    /// <param name="exerGift">艾瑟萌天赋</param>
    public void setExerGift(ExerGift exerGift) {
        playerGift = new PlayerExerGift(exerGift.getID());
        recomputeParams();
    }

    /// <summary>
    /// 设置艾瑟萌
    /// </summary>
    /// <param name="playerExer">玩家艾瑟萌</param>
    public void setPlayerExer(PlayerExermon playerExer) {
        this.playerExer = playerExer;
        recomputeParams();
    }

    /// <summary>
    /// 设置艾瑟萌天赋
    /// </summary>
    /// <param name="playerGift">玩家艾瑟萌天赋</param>
    public void setPlayerGift(PlayerExerGift playerGift) {
        this.playerGift = playerGift;
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
            param.setValue(paramBase(param.paramId).value);
        foreach (var param in rateParams)
            param.setValue(paramRate(param.paramId).value);
    }

    #endregion

    #endregion
    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        playerExer = DataLoader.loadData<PlayerExermon>(json, "player_exer");
        playerGift = DataLoader.loadData<PlayerExerGift>(json, "player_gift");

        subjectId = DataLoader.loadInt(json, "subject_id");
        exp = DataLoader.loadInt(json, "exp");
        level = DataLoader.loadInt(json, "level");

        exerEquipSlot = DataLoader.loadData<ExerEquipSlot>(json, "exerequipslot");
        if (exerEquipSlot != default)
            exerEquipSlot.exerSlotItem = this;

        var paramRanges = DataLoader.loadJsonData(json, "params");

        paramValues = DataLoader.loadDataArray<ParamData>(paramRanges, "values");
        rateParams = DataLoader.loadDataArray<ParamData>(paramRanges, "rates");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["player_exer"] = DataLoader.convertData(playerExer);
        json["player_gift"] = DataLoader.convertData(playerGift);

        json["subject_id"] = subjectId;
        json["exp"] = exp;
        json["level"] = level;
        json["exerequipslot"] = DataLoader.convertData(exerEquipSlot);

        json["params"] = new JsonData();
        json["params"]["values"] = DataLoader.convertDataArray(paramValues);
        json["params"]["rates"] = DataLoader.convertDataArray(rateParams);

        return json;
    }*/
}

/// <summary>
/// 艾瑟萌技能槽项
/// </summary>
public class ExerSkillSlotItem : SlotContItem {

    /// <summary>
    /// 默认类型
    /// </summary>
    /// <returns></returns>
    public override Type defaultType() { return Type.ExerSkillSlotItem; }

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
    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        skillId = DataLoader.loadInt(json, "skill_id");
        useCount = DataLoader.loadInt(json, "use_count");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["skill_id"] = skillId;
        json["use_count"] = useCount;

        return json;
    }*/
}

/// <summary>
/// 艾瑟萌装备槽项
/// </summary>
public class ExerEquipSlotItem : SlotContItem {

    /// <summary>
    /// 默认类型
    /// </summary>
    /// <returns></returns>
    public override Type defaultType() { return Type.ExerEquipSlotItem; }

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public ExerPackEquip packEquip { get; protected set; }
    [AutoConvert]
    public int eType { get; protected set; }

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
    /// 设置装备
    /// </summary>
    /// <param name="packEquip">装备</param>
    public void setEquip(ExerPackEquip packEquip) {
        this.packEquip = packEquip;
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
    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        packEquip = DataLoader.loadData<ExerPackEquip>(json, "pack_equip");
        eType = DataLoader.loadInt(json, "e_type");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["pack_equip"] = DataLoader.convertData(packEquip);
        json["e_type"] = eType;

        return json;
    }*/
}

#endregion

#region 容器

/// <summary>
/// 人类装备槽
/// </summary>
public class ExerSlot : SlotContainer<ExerSlotItem> {

    /// <summary>
    /// 获取艾瑟萌槽项
    /// </summary>
    /// <param name="sid">科目ID</param>
    /// <returns>艾瑟萌槽项数据</returns>
    public ExerSlotItem getExerSlotItem(int sid) {
        return getItem((item) => item.subjectId == sid);
    }

    /// <summary>
    /// 装备艾瑟萌
    /// </summary>
    /// <param name="playerExer">玩家艾瑟萌</param>
    public void setPlayerExer(PlayerExermon playerExer) {
        var exermon = playerExer.exermon();
        var slotItem = getExerSlotItem(exermon.subjectId);
        slotItem.setPlayerExer(playerExer);
    }

    /// <summary>
    /// 装备艾瑟萌
    /// </summary>
    /// <param name="sid">科目ID</param>
    /// <param name="playerGift">玩家艾瑟萌天赋</param>
    public void setPlayerGift(int sid, PlayerExerGift playerGift) {
        var slotItem = getExerSlotItem(sid);
        slotItem.setPlayerGift(playerGift);
    }
}

/// <summary>
/// 艾瑟萌装备槽
/// </summary>
public class ExerEquipSlot : SlotContainer<ExerEquipSlotItem> {

    /// <summary>
    /// 对应的艾瑟萌槽项
    /// </summary>
    public ExerSlotItem exerSlotItem;

    /// <summary>
    /// 获取装备项
    /// </summary>
    /// <param name="eType">装备类型</param>
    /// <returns>装备项数据</returns>
    public ExerEquipSlotItem getEquipSlotItem(int eType) {
        return getItem((item) => item.eType == eType);
    }

    /// <summary>
    /// 装备
    /// </summary>
    /// <param name="packEquip">背包装备</param>
    public void setEquip(ExerPackEquip packEquip) {
        var equip = packEquip.equip();
        var slotItem = getEquipSlotItem(equip.eType);
        slotItem.setEquip(packEquip);
    }

    /// <summary>
    /// 获取装备的所有属性
    /// </summary>
    /// <returns>属性数据数组</returns>
    public ParamData[] getParams() {
        var params_ = new Dictionary<int, ParamData>();

        foreach (var item in items) {
            var itemParams = item.getParams();
            foreach(var param in itemParams) {
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

}

#endregion