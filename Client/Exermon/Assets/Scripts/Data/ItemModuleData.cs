
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using LitJson;
using UnityEditor;

/// <summary>
/// 基本物品数据
/// </summary>
public class BaseItem : BaseData {
    /*
    /// <summary>
    /// 物品类型
    /// </summary>
    public enum Type {
        Unset = 0,  // 未设置

        // ===！！！！不能轻易修改序号！！！！===
        // LimitedItem 1~100
        // UsableItem
        HumanItem = 1,  // 人类物品
        ExerItem = 2,  // 艾瑟萌物品

        // EquipableItem
        HumanEquip = 11,  // 人类装备
        ExerEquip = 12,  // 艾瑟萌装备

        // InfiniteItem 101+
        QuesSugar = 101,  // 题目糖

        Exermon = 201,  // 艾瑟萌
        ExerSkill = 202,  // 艾瑟萌技能
        ExerGift = 203,  // 艾瑟萌天赋
        ExerFrag = 204,  // 艾瑟萌碎片
    }
    */
    /// <summary>
    /// 属性
    /// </summary>
    public string name { get; private set; }
    public string description { get; private set; }
    public int type { get; private set; } // 物品类型

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        name = DataLoader.loadString(json, "name");
        description = DataLoader.loadString(json, "description");
        type = DataLoader.loadInt(json, "type");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["name"] = name;
        json["description"] = description;
        json["type"] = type;

        return json;
    }
}

/// <summary>
/// 物品价格数据
/// </summary>
public class ItemPrice : BaseData {

    /// <summary>
    /// 属性
    /// </summary>
    public int gold { get; private set; }
    public int ticket { get; private set; }
    public int boundTicket { get; private set; } // 物品类型

    /// <summary>
    /// 是否需要ID
    /// </summary>
    protected override bool idEnable() { return false; }

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        gold = DataLoader.loadInt(json, "gold");
        ticket = DataLoader.loadInt(json, "ticket");
        boundTicket = DataLoader.loadInt(json, "bound_ticket");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["gold"] = gold;
        json["ticket"] = ticket;
        json["bound_ticket"] = boundTicket;

        return json;
    }

}

/// <summary>
/// 有限物品数据
/// </summary>
public class LimitedItem : BaseItem {

    /// <summary>
    /// 属性
    /// </summary>
    public ItemPrice buyPrice { get; private set; }
    public int sellPrice { get; private set; }
    public bool discardable { get; private set; }
    public bool tradable { get; private set; }
    public Texture2D icon { get; private set; }

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        buyPrice = DataLoader.loadData<ItemPrice>(json, "buy_price");
        sellPrice = DataLoader.loadInt(json, "sell_price");
        discardable = DataLoader.loadBool(json, "discardable");
        tradable = DataLoader.loadBool(json, "tradable");

        icon = AssetLoader.loadItemIcon(getID());
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["buy_price"] = DataLoader.convertData(buyPrice);
        json["sell_price"] = sellPrice;
        json["discardable"] = discardable;
        json["tradable"] = tradable;

        return json;
    }
}

/// <summary>
/// 使用效果数据
/// </summary>
public class EffectData : BaseData {

    /// <summary>
    /// 属性
    /// </summary>
    public int code { get; private set; }
    public JsonData params_ { get; private set; } // 参数（数组）
    public string description { get; private set; }

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        code = DataLoader.loadInt(json, "code");
        params_ = DataLoader.loadJsonData(json, "params");
        description = DataLoader.loadString(json, "description");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["code"] = code;
        json["params"] = params_;
        json["description"] = description;

        return json;
    }
}

/// <summary>
/// 可用物品数据
/// </summary>
public class UsableItem : LimitedItem {

    /// <summary>
    /// 属性
    /// </summary>
    public int maxCount { get; private set; }
    public bool battleUse { get; private set; }
    public bool menuUse { get; private set; }
    public bool adventureUse { get; private set; }
    public bool consumable { get; private set; }
    public int freeze { get; private set; }
    public int iType { get; private set; }

    /// <summary>
    /// 使用效果
    /// </summary>
    public List<EffectData> effects { get; private set; }

    /// <summary>
    /// 获取物品类型
    /// </summary>
    /// <returns>物品类型</returns>
    public UsableItemType itemType() {
        return DataService.get().usableItemType(iType);
    }

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        maxCount = DataLoader.loadInt(json, "max_count");
        consumable = DataLoader.loadBool(json, "consumable");
        battleUse = DataLoader.loadBool(json, "battle_use");
        menuUse = DataLoader.loadBool(json, "menu_use");
        adventureUse = DataLoader.loadBool(json, "adventure_use");
        freeze = DataLoader.loadInt(json, "freeze");
        iType = DataLoader.loadInt(json, "i_type");

        effects = DataLoader.loadDataList<EffectData>(json, "effects");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["max_count"] = maxCount;
        json["consumable"] = consumable;
        json["battle_use"] = battleUse;
        json["menu_use"] = menuUse;
        json["adventure_use"] = adventureUse;
        json["freeze"] = freeze;
        json["i_type"] = iType;

        json["effects"] = DataLoader.convertDataArray(effects);

        return json;
    }
}

/// <summary>
/// 可装备物品数据
/// </summary>
public class EquipableItem : LimitedItem {

    /// <summary>
    /// 属性
    /// </summary>
    public ParamData[] params_ { get; private set; }

    /// <summary>
    /// 获取装备的属性
    /// </summary>
    /// <param name="paramId">属性ID</param>
    /// <returns>属性数据</returns>
    public ParamData getParam(int paramId) {
        foreach (var param in params_)
            if (param.paramId == paramId) return param;
        return new ParamData(paramId);
    }

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        params_ = DataLoader.loadDataArray<ParamData>(json, "params");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["params"] = DataLoader.convertDataArray(params_);

        return json;
    }
}

/// <summary>
/// 基本容器项
/// </summary>
public class BaseContItem : BaseData {

    /// <summary>
    /// 物品类型
    /// </summary>
    public enum Type {
        Unset = 0,  // 未设置

        // ===！！！！不能轻易修改序号！！！！===
        // Pack 1~100
        HumanPackItem = 1,  // 人类背包物品
        ExerPackItem = 2,  // 艾瑟萌背包物品
        HumanPackEquip = 3,  // 人类背包装备
        ExerPackEquip = 4,  // 艾瑟萌背包装备

        // 可叠加
        ExerFragPackItem = 11,  // 艾瑟萌碎片背包物品

        // 可叠加
        QuesSugarPackItem = 21,  // 题目糖背包物品

        // Slot 101~200
        HumanEquipSlotItem = 101,  // 人类装备槽项
        ExerEquipSlotItem = 102,  // 艾瑟萌装备槽项

        ExerSlotItem = 111,  // 艾瑟萌槽项

        ExerSkillSlotItem = 121,  // 艾瑟萌技能槽物品

        // Others 201+
        PlayerExerGift = 201,  // 玩家艾瑟萌天赋关系
        PlayerExermon = 202,  // 玩家艾瑟萌关系
    }

    /// <summary>
    /// 属性
    /// </summary>
    public int type { get; private set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    public BaseContItem() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="type">类型</param>
    public BaseContItem(Type type) { this.type = (int)type; }

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        type = DataLoader.loadInt(json, "type");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["type"] = type;

        return json;
    }
}

/// <summary>
/// 背包类容器项
/// </summary>
public class PackContItem : BaseContItem {

    /// <summary>
    /// 属性
    /// </summary>
    public int itemId { get; private set; }
    public int count { get; private set; }

    /// <summary>
    /// 查找物品函数类型
    /// </summary>
    protected delegate BaseItem GetFunc(int id);

    /// <summary>
    /// 获取物品类型
    /// </summary>
    /// <returns></returns>
    protected virtual System.Type getItemType() {
        var type = (Type)this.type;
        var data = DataService.get();

        switch (type) {
            case Type.HumanPackItem: return typeof(HumanItem);
            case Type.ExerPackItem: return typeof(ExerItem);
            case Type.HumanPackEquip: return typeof(HumanEquip);
            case Type.ExerPackEquip: return typeof(ExerEquip);
            case Type.PlayerExermon: return typeof(Exermon);
            case Type.ExerFragPackItem: return typeof(ExerFrag);
            case Type.PlayerExerGift: return typeof(ExerGift);
            //case Type.QuesSugarPackItem: return typeof(ExerGift);
            default: return null;
        }
    }

    /// <summary>
    /// 获取对应物品
    /// </summary>
    /// <returns></returns>
    public virtual BaseItem item() {
        var type = getItemType();
        if (type == null) return null;
        return (BaseItem)DataService.get().get(type, itemId);
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public PackContItem() { }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="itemId">物品ID</param>
    /// <param name="count">数量</param>
    public PackContItem(Type type, int itemId=0, int count=1) : base(type) {
        this.itemId = itemId;
        this.count = count;
    }

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        itemId = DataLoader.loadInt(json, "item_id");
        count = DataLoader.loadInt(json, "count");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["item_id"] = itemId;
        json["count"] = count;

        return json;
    }
}

/// <summary>
/// 槽类容器项
/// </summary>
public class SlotContItem : BaseContItem {

    /// <summary>
    /// 属性
    /// </summary>
    public int index { get; private set; }

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        index = DataLoader.loadInt(json, "index");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["index"] = index;

        return json;
    }
}

/// <summary>
/// 基本容器数据
/// </summary>
public class BaseContainer : BaseData {

    /// <summary>
    /// 属性
    /// </summary>
    public int type { get; private set; }
    public int capacity { get; private set; }

    /// <summary>
    /// 数据是否需要实时同步（在背包界面时需要实时同步）
    /// </summary>
    public bool realTime = false;

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        type = DataLoader.loadInt(json, "type");
        capacity = DataLoader.loadInt(json, "capacity");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["type"] = type;

        return json;
    }
}

/// <summary>
/// 背包类容器数据
/// </summary>
public class PackContainer<T> : BaseContainer where T : PackContItem, new() {

    /// <summary>
    /// 属性
    /// </summary>
    public List<T> items { get; private set; }
    
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        items = DataLoader.loadDataList<T>(json, "items");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["capacity"] = capacity;
        json["items"] = DataLoader.convertDataArray(items);

        return json;
    }
}

/// <summary>
/// 背包类容器数据
/// </summary>
public class SlotContainer<T> : BaseContainer where T : SlotContItem, new() {

    /// <summary>
    /// 属性
    /// </summary>
    public List<T> items { get; private set; }

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);
        
        items = DataLoader.loadDataList<T>(json, "items");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();
        
        json["items"] = DataLoader.convertDataArray(items);

        return json;
    }
}