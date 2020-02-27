
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

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public string name { get; protected set; }
    [AutoConvert]
    public string description { get; protected set; }
    [AutoConvert]
    public int type { get; protected set; } // 物品类型
    /*
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
    }*/
}

/// <summary>
/// 物品价格数据
/// </summary>
public class ItemPrice : BaseData {

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public int gold { get; protected set; }
    [AutoConvert]
    public int ticket { get; protected set; }
    [AutoConvert]
    public int boundTicket { get; protected set; } // 物品类型

    /// <summary>
    /// 是否需要ID
    /// </summary>
    protected override bool idEnable() { return false; }
    /*
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
    */
}

/// <summary>
/// 有限物品数据
/// </summary>
public class LimitedItem : BaseItem {

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public ItemPrice buyPrice { get; protected set; }
    [AutoConvert]
    public int sellPrice { get; protected set; }
    [AutoConvert]
    public bool discardable { get; protected set; }
    [AutoConvert]
    public bool tradable { get; protected set; }

    public Texture2D icon { get; protected set; }
    /*
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
    }*/
}

/// <summary>
/// 使用效果数据
/// </summary>
public class EffectData : BaseData {

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public int code { get; protected set; }
    [AutoConvert("params")]
    public JsonData params_ { get; protected set; } // 参数（数组）
    [AutoConvert]
    public string description { get; protected set; }
    /*
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
    }*/
}

/// <summary>
/// 可用物品数据
/// </summary>
public class UsableItem : LimitedItem {

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public int maxCount { get; protected set; }
    [AutoConvert]
    public bool battleUse { get; protected set; }
    [AutoConvert]
    public bool menuUse { get; protected set; }
    [AutoConvert]
    public bool adventureUse { get; protected set; }
    [AutoConvert]
    public bool consumable { get; protected set; }
    [AutoConvert]
    public int freeze { get; protected set; }
    [AutoConvert]
    public int iType { get; protected set; }

    /// <summary>
    /// 使用效果
    /// </summary>
    [AutoConvert]
    public List<EffectData> effects { get; protected set; }

    /// <summary>
    /// 获取物品类型
    /// </summary>
    /// <returns>物品类型</returns>
    public UsableItemType itemType() {
        return DataService.get().usableItemType(iType);
    }
    /*
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
    }*/
}

/// <summary>
/// 可装备物品数据
/// </summary>
public class EquipableItem : LimitedItem {

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert("params")]
    public ParamData[] params_ { get; protected set; }

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
    /*
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
    }*/
}

/// <summary>
/// 基本容器项
/// </summary>
public abstract class BaseContItem : BaseData {

    /// <summary>
    /// 容器项类型
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
    [AutoConvert]
    public int type { get; protected set; }

    /// <summary>
    /// 默认类型
    /// </summary>
    /// <returns></returns>
    public abstract Type defaultType();

    /// <summary>
    /// 构造函数
    /// </summary>
    public BaseContItem() { type = (int)defaultType(); }
    /*
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
    }*/
}

/// <summary>
/// 背包类容器项
/// </summary>
public abstract class PackContItem : BaseContItem {

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public int itemId { get; protected set; }
    [AutoConvert]
    public int count { get; protected set; }

    /**
    /// <summary>
    /// 查找物品函数类型
    /// </summary>
    protected delegate BaseItem GetFunc(int id);

    /// <summary>
    /// 获取物品类型
    /// </summary>
    /// <returns></returns>
    protected abstract virtual System.Type getItemType() {
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
    */

    /// <summary>
    /// 构造函数
    /// </summary>
    public PackContItem() { }
    /// <param name="type">类型</param>
    /// <param name="itemId">物品ID</param>
    /// <param name="count">数量</param>
    public PackContItem(int itemId, int count=1) {
        this.itemId = itemId;
        this.count = count;
    }
    /*
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
    }*/
}

/// <summary>
/// 槽类容器项
/// </summary>
public abstract class SlotContItem : BaseContItem {

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public int index { get; protected set; }
    /*
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
    }*/
}

/// <summary>
/// 基本容器数据
/// </summary>
public class BaseContainer<T> : BaseData where T: BaseContItem, new() {

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public int type { get; protected set; }
    [AutoConvert]
    public int capacity { get; protected set; }

    [AutoConvert]
    public List<T> items { get; protected set; }

    /// <summary>
    /// 数据是否需要实时同步（在背包界面时需要实时同步）
    /// </summary>
    public bool realTime = false;

    /// <summary>
    /// 获得一个物品
    /// </summary>
    /// <param name="p">条件</param>
    /// <returns>物品</returns>
    public T getItem(Predicate<T> p) {
        return items.Find(p);
    }

    /// <summary>
    /// 获得多个物品
    /// </summary>
    /// <param name="p">条件</param>
    /// <returns>物品列表</returns>
    public List<T> getItems(Predicate<T> p) {
        return items.FindAll(p);
    }
    /*
    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        type = DataLoader.loadInt(json, "type");
        capacity = DataLoader.loadInt(json, "capacity");

        items = DataLoader.loadDataList<T>(json, "items");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["type"] = type;
        json["capacity"] = capacity;

        json["items"] = DataLoader.convertDataArray(items);

        return json;
    }*/
}

/// <summary>
/// 背包类容器数据
/// </summary>
public class PackContainer<T> : BaseContainer<T> where T : PackContItem, new() {
    
}

/// <summary>
/// 背包类容器数据
/// </summary>
public class SlotContainer<T> : BaseContainer<T> where T : SlotContItem, new() {
    
}