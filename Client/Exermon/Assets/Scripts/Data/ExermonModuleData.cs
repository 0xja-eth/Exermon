
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using LitJson;
using UnityEditor;

/// <summary>
/// 艾瑟萌数据
/// </summary>
public class Exermon : BaseItem, ParamBar.ParamValueInfosConvertable {

    /// <summary>
    /// 属性
    /// </summary>
    public string animal { get; private set; }
    public int starId { get; private set; }
    public int subjectId { get; private set; }
    public int eType { get; private set; }

    public ParamData[] baseParams { get; private set; }
    public ParamData[] rateParams { get; private set; }

    public Texture2D full { get; private set; }
    public Texture2D icon { get; private set; }
    public Texture2D battle { get; private set; }

    /// <summary>
    /// 后续增加属性
    /// </summary>
    public List<ExerSkill> exerSkills { get; private set; } = new List<ExerSkill>();
    public ExerFrag exerFrag { get; private set; } = null;

    /// <summary>
    /// 转化为属性信息集
    /// </summary>
    /// <returns>属性信息集</returns>
    public ParamBar.ParamValueInfo[] convertToParamInfos() {
        var count = baseParams.Length;
        var infos = new ParamBar.ParamValueInfo[count];
        for (int i = 0; i < count; i++) {
            var info = new ParamBar.ParamValueInfo();
            var base_ = baseParams[i];
            var rate = rateParams[i];
            info.max = (float)star().baseRanges[i].maxValue;
            info.value = (float)base_.value;
            if(i >= 4) {
                info.max *= 100; info.value *= 100;
            }
            info.rate = (float)rate.value;
            infos[i] = info;
        }
        return infos;
    }

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
    }
}

/// <summary>
/// 艾瑟萌碎片数据
/// </summary>
public class ExerGift : BaseItem {

    /// <summary>
    /// 属性
    /// </summary>
    public int starId { get; private set; }
    public int gType { get; private set; }
    public ParamData[] params_ { get; private set; }

    /// <summary>
    /// 获取艾瑟萌天赋星级
    /// </summary>
    /// <returns>艾瑟萌天赋星级</returns>
    public ExerGiftStar star() {
        return DataService.get().exerGiftStar(starId);
    }

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        starId = DataLoader.loadInt(json, "star_id");
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
        json["g_type"] = gType;
        json["params"] = DataLoader.convertDataArray(params_);

        return json;
    }
}

/// <summary>
/// 艾瑟萌碎片数据
/// </summary>
public class ExerFrag : BaseItem {

    /// <summary>
    /// 属性
    /// </summary>
    public int exermonId { get; private set; }
    public int sellPrice { get; private set; }
    public int count { get; private set; }

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
    public override void load(JsonData json) {
        base.load(json);

        exermonId = DataLoader.loadInt(json, "eid");
        sellPrice = DataLoader.loadInt(json, "sell_price");
        count = DataLoader.loadInt(json, "count");

        var exer = exermon();
        if(exer != null) exer.setExerFrag(this);
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
    }
}

/// <summary>
/// 艾瑟萌碎片数据
/// </summary>
public class ExerSkill : BaseItem {

    /// <summary>
    /// 属性
    /// </summary>
    public int exermonId { get; private set; }
    public bool passive { get; private set; }
    public int nextSkillId { get; private set; }
    public int needCount { get; private set; }
    public int mpCost { get; private set; }
    public int rate { get; private set; }
    public int freeze { get; private set; }
    public int maxUseCount { get; private set; }
    public int target { get; private set; }
    public int hitType { get; private set; }
    public int atkRate { get; private set; }
    public int defRate { get; private set; }
    public Texture2D icon { get; private set; }
    public Texture2D ani { get; private set; }
    public Texture2D targetAni { get; private set; }

    /// <summary>
    /// 使用效果
    /// </summary>
    public List<EffectData> effects { get; private set; }

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

        icon = AssetLoader.loadExerSkillIcon(getID());
        ani = AssetLoader.loadExerSkillAni(getID());
        targetAni = AssetLoader.loadExerSkillTarget(getID());

        var exer = exermon();
        if (exer != null) exer.addExerSkill(this);
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
    }
}

/// <summary>
/// 艾瑟萌物品
/// </summary>
public class ExerItem : UsableItem {

    /// <summary>
    /// 属性
    /// </summary>
    public int rate { get; private set; }

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

}

/// <summary>
/// 艾瑟萌装备
/// </summary>
public class ExerEquip : EquipableItem {

    /// <summary>
    /// 属性
    /// </summary>
    public int eType { get; private set; }

    /// <summary>
    /// 获取装备类型
    /// </summary>
    /// <returns>装备类型</returns>
    public TypeData equipType() {
        return DataService.get().exerEquipType(eType);
    }

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
    }
}

/*
/// <summary>
/// 艾瑟萌背包物品
/// </summary>
public class ExerPackItem : PackContItem {}
*/

/// <summary>
/// 艾瑟萌背包装备
/// </summary>
public class ExerPackEquip : PackContItem {}

/// <summary>
/// 艾瑟萌
/// </summary>
public class PlayerExermon : PackContItem {

    /// <summary>
    /// 属性
    /// </summary>
    public string nickname { get; private set; }
    public int exp { get; private set; }
    public int level { get; private set; }
    public int exerSkillSlotId { get; private set; }

    /// <summary>
    /// 获取名称
    /// </summary>
    /// <returns></returns>
    public string name() {
        return nickname.Length > 0 ? nickname : item().name;
    }

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        nickname = DataLoader.loadString(json, "nickname");
        exp = DataLoader.loadInt(json, "exp");
        level = DataLoader.loadInt(json, "level");
        exerSkillSlotId = DataLoader.loadInt(json, "exerskillslot_id");
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
        json["exerskillslot_id"] = exerSkillSlotId;

        return json;
    }
}

/// <summary>
/// 艾瑟萌装备槽项
/// </summary>
public class ExerEquipSlotItem : SlotContItem {

    /// <summary>
    /// 属性
    /// </summary>
    public ExerPackEquip packEquip { get; private set; }
    public int eType { get; private set; }

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
        return (ExerEquip)packEquip.item();
    }

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
    }
}

/// <summary>
/// 艾瑟萌装备槽项
/// </summary>
public class ExerSlotItem : SlotContItem {

    /// <summary>
    /// 属性
    /// </summary>
    public PlayerExermon playerExer { get; private set; }
    public PackContItem playerGift { get; private set; }
    public int subjectId { get; private set; }
    public int exp { get; private set; }
    public int level { get; private set; }
    public int exerEquipSlotId { get; private set; }

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
    /// <returns>艾瑟萌</returns>
    public Exermon exermon() {
        return (Exermon)playerExer.item();
    }

    /// <summary>
    /// 获取艾瑟萌天赋
    /// </summary>
    /// <returns>艾瑟萌天赋</returns>
    public ExerGift exerGift() {
        return (ExerGift)playerGift.item();
    }

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        playerExer = DataLoader.loadData<PlayerExermon>(json, "player_exer");
        playerGift = DataLoader.loadData<PackContItem>(json, "player_gift");
        subjectId = DataLoader.loadInt(json, "subject_id");
        exp = DataLoader.loadInt(json, "exp");
        level = DataLoader.loadInt(json, "level");
        exerEquipSlotId = DataLoader.loadInt(json, "exerequipslot_id");
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
        json["exerequipslot_id"] = exerEquipSlotId;

        return json;
    }
}

/// <summary>
/// 艾瑟萌技能槽项
/// </summary>
public class ExerSkillSlotItem : SlotContItem {

    /// <summary>
    /// 属性
    /// </summary>
    public int skillId { get; private set; }
    public int useCount { get; private set; }

    /// <summary>
    /// 获取科目
    /// </summary>
    /// <returns>科目</returns>
    public ExerSkill skill() {
        return DataService.get().exerSkill(skillId);
    }

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
    }
}