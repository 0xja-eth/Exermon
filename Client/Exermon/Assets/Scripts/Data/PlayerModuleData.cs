
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using LitJson;
using UnityEditor;

/// <summary>
/// 形象数据
/// </summary>
public class Character : BaseData {

    /// <summary>
    /// 属性
    /// </summary>
    public string name { get; private set; }
    public int gender { get; private set; }
    public string description { get; private set; }

    public Texture2D bust { get; private set; }
    public Texture2D face { get; private set; }
    public Texture2D battle { get; private set; }

    /// <summary>
    /// 获取性别文本
    /// </summary>
    /// <returns>性别文本</returns>
    public string genderText() {
        return DataService.get().characterGender(gender).Item2;
    }

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        name = DataLoader.loadString(json, "name");
        gender = DataLoader.loadInt(json, "gender");
        description = DataLoader.loadString(json, "description");

        bust = AssetLoader.loadCharacterBust(getID());
        face = AssetLoader.loadCharacterFace(getID());
        battle = AssetLoader.loadCharacterBattle(getID());
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["name"] = name;
        json["gender"] = gender;
        json["description"] = description;

        return json;
    }
}

/// <summary>
/// 玩家数据
/// </summary>
public class Player : BaseData, ParamDisplay.DisplayDataConvertable {

    /// <summary>
    /// 背包类容器数据		
    /// </summary>
    public class PackContainerInfo : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        /*
        public int humanPackId { get; private set; }
        public int exerPackId { get; private set; }
        public int exerFragPackId { get; private set; }
        public int exerGiftPoolId { get; private set; }
        public int exerHubId { get; private set; }
        public int quesSugarPackId { get; private set; }
        */

        /// <summary>
        /// 缓存属性
        /// </summary>
        public PackContainer<PackContItem> humanPack { get; private set; }
        public PackContainer<PackContItem> exerPack { get; private set; }
        public PackContainer<PackContItem> exerFragPack { get; private set; }
        public PackContainer<PackContItem> exerGiftPool { get; private set; }
        public PackContainer<PlayerExermon> exerHub { get; private set; }
        public PackContainer<PackContItem> quesSugarPack { get; private set; }

        /*
        /// <summary>
        /// 读取人类背包
        /// </summary>
        /// <param name="json">数据</param>
        public void loadHumanPack(JsonData json) {
            humanPack = DataLoader.loadData<PackContainer<PackContItem>>(json);
        }

        /// <summary>
        /// 读取艾瑟萌背包
        /// </summary>
        /// <param name="json">数据</param>
        public void loadExerPack(JsonData json) {
            exerPack = DataLoader.loadData<PackContainer<PackContItem>>(json);
        }

        /// <summary>
        /// 读取艾瑟萌碎片背包
        /// </summary>
        /// <param name="json">数据</param>
        public void loadExerFragPack(JsonData json) {
            exerFragPack = DataLoader.loadData<PackContainer<PackContItem>>(json);
        }

        /// <summary>
        /// 读取艾瑟萌天赋池
        /// </summary>
        /// <param name="json">数据</param>
        public void loadExerGiftPool(JsonData json) {
            exerGiftPool = DataLoader.loadData<PackContainer<PackContItem>>(json);
        }

        /// <summary>
        /// 读取艾瑟萌仓库
        /// </summary>
        /// <param name="json">数据</param>
        public void loadExerHub(JsonData json) {
            exerHub = DataLoader.loadData<PackContainer<PlayerExermon>>(json);
        }
        */

        /// <summary>
        /// 数据加载
        /// </summary>
        /// <param name="json">数据</param>
        public override void load(JsonData json) {
            base.load(json);

            humanPack = DataLoader.loadData<PackContainer<PackContItem>>(json, "humanpack");
            exerPack = DataLoader.loadData<PackContainer<PackContItem>>(json, "exerpack");
            exerFragPack = DataLoader.loadData<PackContainer<PackContItem>>(json, "exerfragpack");
            exerGiftPool = DataLoader.loadData<PackContainer<PackContItem>>(json, "exergiftpool");
            exerHub = DataLoader.loadData<PackContainer<PlayerExermon>>(json, "exerhub");
            quesSugarPack = DataLoader.loadData<PackContainer<PackContItem>>(json, "quessugarpack");
        }

        /// <summary>
        /// 获取JSON数据
        /// </summary>
        /// <returns>JsonData</returns>
        public override JsonData toJson() {
            var json = base.toJson();

            json["humanpack"] = DataLoader.convertData(humanPack);
            json["exerpack"] = DataLoader.convertData(exerPack);
            json["exerfragpack"] = DataLoader.convertData(exerFragPack);
            json["exergiftpool"] = DataLoader.convertData(exerGiftPool);
            json["exerhub"] = DataLoader.convertData(exerHub);
            json["quessugarpack"] = DataLoader.convertData(quesSugarPack);

            return json;
        }
    }

    /// <summary>
    /// 槽类容器索引数据		
    /// </summary>
    public class SlotContainerInfo : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        /*
        public int exerSlotId { get; set; }
        public int humanEquipSlotId { get; private set; }
        */

        /// <summary>
        /// 缓存属性
        /// </summary>
        public SlotContainer<ExerSlotItem> exerSlot { get; private set; }
        public HumanEquipSlot humanEquipSlot { get; private set; }

        /*
        /// <summary>
        /// 读取艾瑟萌槽
        /// </summary>
        /// <param name="json">数据</param>
        public void loadExerSlot(JsonData json) {
            exerSlot = DataLoader.loadData
                <SlotContainer<ExerSlotItem>>(json, "container");
        }

        /// <summary>
        /// 读取人类装备槽
        /// </summary>
        /// <param name="json">数据</param>
        public void loadHumanEquipSlot(JsonData json) {
            humanEquipSlot = DataLoader.loadData
                <HumanEquipSlot>(json, "container");
        }
        */

        /// <summary>
        /// 数据加载
        /// </summary>
        /// <param name="json">数据</param>
        public override void load(JsonData json) {
            base.load(json);

            exerSlot = DataLoader.loadData<SlotContainer<ExerSlotItem>>(json, "exerslot");
            humanEquipSlot = DataLoader.loadData<HumanEquipSlot>(json, "humanequipslot");
        }

        /// <summary>
        /// 获取JSON数据
        /// </summary>
        /// <returns>JsonData</returns>
        public override JsonData toJson() {
            var json = base.toJson();

            json["exerslot"] = DataLoader.convertData(exerSlot); ;
            json["humanequipslot"] = DataLoader.convertData(humanEquipSlot); ;

            return json;
        }
    }

    /// <summary>
    /// 状态枚举
    /// </summary>
    public enum Status {
        // 已注册，未创建角色
        Uncreated = 0,  // 未创建
        CharacterCreated = 1,  // 已创建人物
        ExermonsCreated = 2,  // 已选择艾瑟萌
        GiftsCreated = 3,  // 已选择天赋

        // 已完全创建角色
        Normal = 10,  // 正常
        Banned = 20,  // 封禁
        Frozen = 30,  // 冻结
        Other = -1  // 其他
    }

    /// <summary>
    /// 属性
    /// </summary>
    public string username { get; private set; }
    public string password { get; private set; }
    public string phone { get; private set; }
    public string email { get; private set; }
    public string name { get; private set; }
    public int characterId { get; private set; }
    public int grade { get; private set; }
    public int status { get; private set; }
    public int type { get; private set; }
    public bool online { get; private set; }
    public int exp { get; private set; }
    public int level { get; private set; }
    public int next { get; private set; }
    public DateTime createTime { get; private set; }
    public DateTime birth { get; private set; }
    public string school { get; private set; }
    public string city { get; private set; }
    public string contact { get; private set; }
    public string description { get; private set; }

    public PackContainerInfo packContainers { get; private set; }
    public SlotContainerInfo slotContainers { get; private set; }

    /// <summary>
    /// 转化为属性信息集
    /// </summary>
    /// <returns>属性信息集</returns>
    public JsonData convertToDisplayData(string type = "") {
        switch (type.ToLower()) {
            case "exp": return convertExp();
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
        json["rate"] = exp / next;
        return json;
    }

    /// <summary>
    /// 设置密码
    /// </summary>
    /// <param name="pw">密码</param>
    public void setPassword(string pw) {
        password = pw;
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    /// <param name="name">昵称</param>
    /// <param name="grade">年级ID</param>
    /// <param name="cid">人物ID</param>
    public void createCharacter(string name, int grade, int cid) {
        this.name = name;
        this.grade = grade;
        characterId = cid;
        status = (int)Status.CharacterCreated;
    }

    /// <summary>
    /// 选择艾瑟萌
    /// </summary>
    /// <param name="cid">艾瑟萌槽容器ID</param>
    public void createExermons(JsonData id) {
        slotContainers.exerSlot.load(id);
        status = (int)Status.ExermonsCreated;
    }

    /// <summary>
    /// 选择天赋
    /// </summary>
    /// <param name="gids">艾瑟萌天赋ID</param>
    public void createGifts(int[] gids) {
        status = (int)Status.GiftsCreated;
    }

    /// <summary>
    /// 补全信息
    /// </summary>
    /// <param name="birth">出生日期</param>
    /// <param name="school">学校名称</param>
    /// <param name="city">居住地</param>
    /// <param name="contact">联系方式</param>
    /// <param name="description">个人介绍</param>
    public void createInfo(DateTime birth, string school,
        string city, string contact, string description) {
        this.birth = birth;
        this.school = school;
        this.city = city;
        this.contact = contact;
        this.description = description;
        status = (int)Status.Normal;
    }

    /// <summary>
    /// 补全信息
    /// </summary>
    /// <param name="birth">出生日期</param>
    /// <param name="school">学校名称</param>
    /// <param name="city">居住地</param>
    /// <param name="contact">联系方式</param>
    /// <param name="description">个人介绍</param>
    public void createInfo() {
        status = (int)Status.Normal;
    }

    /// <summary>
    /// 获取人物
    /// </summary>
    /// <returns>人物</returns>
    public Character character() {
        return DataService.get().character(characterId);
    }

    /// <summary>
    /// 获取年级文本
    /// </summary>
    /// <returns>年级文本</returns>
    public string gradeText() {
        return DataService.get().playerGrade(grade).Item2;
    }

    /// <summary>
    /// 获取状态文本
    /// </summary>
    /// <returns>状态文本</returns>
    public string statusText() {
        return DataService.get().playerStatus(status).Item2;
    }

    /// <summary>
    /// 获取状态枚举
    /// </summary>
    /// <returns>状态枚举</returns>
    public Status statusEnum() {
        return (Status)status;
    }

    /// <summary>
    /// 获取类型文本
    /// </summary>
    /// <returns>类型文本</returns>
    public string typeText() {
        return DataService.get().playerType(type).Item2;
    }

    /// <summary>
    /// 玩家是否已完全创建
    /// </summary>
    /// <returns>是否已创建</returns>
    public bool isCreated() {
        return status >= (int)Status.Normal;
    }

    /// <summary>
    /// 玩家人物是否已创建
    /// </summary>
    /// <returns>是否已创建</returns>
    public bool isCharacterCreated() {
        return status >= (int)Status.CharacterCreated;
    }

    /// <summary>
    /// 玩家艾瑟萌是否已选择
    /// </summary>
    /// <returns>是否已选择</returns>
    public bool isExermonsCreated() {
        return status >= (int)Status.ExermonsCreated;
    }

    /// <summary>
    /// 玩家天赋是否已选择
    /// </summary>
    /// <returns>是否已选择</returns>
    public bool isGiftsCreated() {
        return status >= (int)Status.GiftsCreated;
    }

    /// <summary>
    /// 读取基础信息
    /// </summary>
    /// <param name="json">数据</param>
    public void loadBasic(JsonData json) {
        load(json);
    }

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        username = DataLoader.loadString(json, "username");
        phone = DataLoader.loadString(json, "phone");
        email = DataLoader.loadString(json, "email");
        name = DataLoader.loadString(json, "name");
        characterId = DataLoader.loadInt(json, "character_id");
        grade = DataLoader.loadInt(json, "grade");
        status = DataLoader.loadInt(json, "status");
        type = DataLoader.loadInt(json, "type");
        online = DataLoader.loadBool(json, "online");

        exp = DataLoader.loadInt(json, "exp");
        level = DataLoader.loadInt(json, "level");
        next = DataLoader.loadInt(json, "next");

        createTime = DataLoader.loadDateTime(json, "create_time");
        birth = DataLoader.loadDateTime(json, "birth");

        school = DataLoader.loadString(json, "school");
        city = DataLoader.loadString(json, "city");
        contact = DataLoader.loadString(json, "contact");
        description = DataLoader.loadString(json, "description");

        packContainers = DataLoader.loadData
            <PackContainerInfo>(json, "pack_containers");
        slotContainers = DataLoader.loadData
            <SlotContainerInfo>(json, "slot_containers");
    }

    /// <summary>
    /// 获取JSON数据
    /// </summary>
    /// <returns>JsonData</returns>
    public override JsonData toJson() {
        var json = base.toJson();

        json["username"] = username;
        json["password"] = password;
        json["phone"] = phone;
        json["email"] = email;
        json["name"] = name;
        json["character_id"] = characterId;
        json["grade"] = grade;
        json["status"] = status;
        json["type"] = type;
        json["online"] = online;

        json["exp"] = exp;
        json["level"] = level;
        json["next"] = next;

        json["create_time"] = DataLoader.convertDateTime(createTime);
        json["birth"] = DataLoader.convertDate(birth);

        json["school"] = school;
        json["city"] = city;
        json["contact"] = contact;
        json["description"] = description;

        json["pack_containers"] = DataLoader.convertData(packContainers);
        json["slot_containers"] = DataLoader.convertData(slotContainers);

        return json;
    }
}

/// <summary>
/// 人类物品
/// </summary>
public class HumanItem : UsableItem { }

/// <summary>
/// 人类装备
/// </summary>
public class HumanEquip : EquipableItem {

    /// <summary>
    /// 属性
    /// </summary>
    public int eType { get; private set; }

    /// <summary>
    /// 获取装备类型
    /// </summary>
    /// <returns>装备类型</returns>
    public TypeData equipType() {
        return DataService.get().humanEquipType(eType);
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

/// <summary>
/// 人类背包物品
/// </summary>
//public class HumanPackItem : PackContItem { }

/// <summary>
/// 人类背包装备
/// </summary>
public class HumanPackEquip : PackContItem {

    /// <summary>
    /// 获取装备实例
    /// </summary>
    /// <returns>装备</returns>
    public HumanEquip equip() {
        return item() as HumanEquip;
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
/// 人类装备槽项
/// </summary>
public class HumanEquipSlotItem : SlotContItem {

    /// <summary>
    /// 属性
    /// </summary>
    public HumanPackEquip packEquip { get; private set; }
    public int eType { get; private set; }

    /// <summary>
    /// 获取装备类型
    /// </summary>
    /// <returns>装备类型</returns>
    public TypeData equipType() {
        return DataService.get().humanEquipType(eType);
    }

    /// <summary>
    /// 获取装备类型
    /// </summary>
    /// <returns>装备类型</returns>
    public HumanEquip equip() {
        return (HumanEquip)packEquip.item();
    }

    /// <summary>
    /// 数据加载
    /// </summary>
    /// <param name="json">数据</param>
    public override void load(JsonData json) {
        base.load(json);

        packEquip = DataLoader.loadData<HumanPackEquip>(json, "pack_equip");
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
/// 人类装备槽
/// </summary>
public class HumanEquipSlot : SlotContainer<HumanEquipSlotItem> {

    /// <summary>
    /// 获取装备项
    /// </summary>
    /// <param name="eType">装备类型</param>
    /// <returns>装备项数据</returns>
    public HumanEquipSlotItem getEquipSlotItem(int eType) {
        foreach (var item in items)
            if (item.eType == eType) return item;
        return null;
    }
    /*
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
    */
}

