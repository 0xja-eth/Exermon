
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
    /// 半身像高度
    /// </summary>
    public const int BustHeight = 512;

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public string name { get; protected set; }
    [AutoConvert]
    public int gender { get; protected set; }
    [AutoConvert]
    public string description { get; protected set; }

    public Texture2D bust { get; protected set; }
    public Texture2D face { get; protected set; }
    public Texture2D battle { get; protected set; }

    /// <summary>
    /// 获取性别文本
    /// </summary>
    /// <returns>性别文本</returns>
    public string genderText() {
        return DataService.get().characterGender(gender).Item2;
    }

    /// <summary>
    /// 加载自定义数据
    /// </summary>
    /// <param name="json">数据</param>
    protected override void loadCustomAttributes(JsonData json) {
        base.loadCustomAttributes(json);
        bust = AssetLoader.loadCharacterBust(getID());
        face = AssetLoader.loadCharacterFace(getID());
        battle = AssetLoader.loadCharacterBattle(getID());
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
        [AutoConvert]
        public HumanPack humanPack { get; protected set; } = new HumanPack();
        [AutoConvert]
        public ExerPack exerPack { get; protected set; } = new ExerPack();
        [AutoConvert]
        public PackContainer<ExerFragPackItem> exerFragPack { get; protected set; }
            = new PackContainer<ExerFragPackItem>();
        [AutoConvert]
        public ExerGiftPool exerGiftPool { get; protected set; } = new ExerGiftPool();
        [AutoConvert]
        public ExerHub exerHub { get; protected set; } = new ExerHub();
        [AutoConvert]
        public PackContainer<QuesSugarPackItem> quesSugarPack { get; protected set; }
            = new PackContainer<QuesSugarPackItem>();

        /// <summary>
        /// 获取容器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public PackContainer<T> getContainer<T>() where T: PackContItem, new() {
            var type = typeof(T);
            /* 会出错，需要另外获取
            if (type == typeof(HumanPackItem) || type == typeof(HumanPackEquip))
                return (PackContainer<T>)(object)humanPack;
            if (type == typeof(ExerPackItem) || type == typeof(ExerPackEquip))
                return (PackContainer<T>)(object)exerPack;
            */
            if (type == typeof(ExerFragPackItem))
                return (PackContainer<T>)(object)exerFragPack;
            if (type == typeof(PlayerExerGift))
                return (PackContainer<T>)(object)exerGiftPool;
            if (type == typeof(PlayerExermon))
                return (PackContainer<T>)(object)exerHub;
            if (type == typeof(QuesSugarPackItem))
                return (PackContainer<T>)(object)quesSugarPack;
            return null;
        }
    }

    /// <summary>
    /// 槽类容器索引数据		
    /// </summary>
    public class SlotContainerInfo : BaseData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public ExerSlot exerSlot { get; protected set; } = new ExerSlot();
        [AutoConvert]
        public HumanEquipSlot humanEquipSlot { get; protected set; } = new HumanEquipSlot();
        
    }

    /// <summary>
    /// 对战信息
    /// </summary>
    public class BattleInfo : BaseData, ParamDisplay.DisplayDataConvertable {

        /// <summary>
        /// 子段位标志
        /// </summary>
        const string SubRankSign = "I";

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int rankId { get; protected set; }
        [AutoConvert]
        public int subRank { get; protected set; }
        [AutoConvert]
        public int starNum { get; protected set; }
        [AutoConvert]
        public int score { get; protected set; }
        [AutoConvert]
        public int credit { get; protected set; }
        [AutoConvert]
        public int count { get; protected set; }
        [AutoConvert]
        public double winRate { get; protected set; }
        [AutoConvert]
        public double corrRate { get; protected set; }
        [AutoConvert]
        public double avgHurt { get; protected set; }
        [AutoConvert]
        public double avgDamage { get; protected set; }
        [AutoConvert]
        public double avgScore { get; protected set; }
        [AutoConvert]
        public int maxHurt { get; protected set; }
        [AutoConvert]
        public int maxDamage { get; protected set; }
        [AutoConvert]
        public double maxScore { get; protected set; }

        /// <summary>
        /// 转换成显示数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public JsonData convertToDisplayData(string type = "") {
            var res = toJson();
            res["rank_text"] = rankText();
            return res;
        }

        /// <summary>
        /// 获取段位实例
        /// </summary>
        /// <returns></returns>
        public CompRank rank() {
            return DataService.get().compRank(rankId);
        }

        /// <summary>
        /// 完整段位文本
        /// </summary>
        /// <returns>段位文本</returns>
        public string rankText() {
            var rank = this.rank();
            if (rank == null) return "无";
            string res = rank.name + SubRankSign;
            for (int i = 0; i < subRank; ++i)
                res += SubRankSign;
            return res;
        }
    }

    /// <summary>
    /// 做题信息
    /// </summary>
    public class QuestionInfo : BaseData, ParamDisplay.DisplayDataConvertable {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int count { get; protected set; }
        [AutoConvert]
        public int corrCnt { get; protected set; }
        [AutoConvert]
        public double corrRate { get; protected set; }
        [AutoConvert]
        public int sumTimespan { get; protected set; }
        [AutoConvert]
        public double avgTimespan { get; protected set; }
        [AutoConvert]
        public double corrTimespan { get; protected set; }
        [AutoConvert]
        public int sumExp { get; protected set; }
        [AutoConvert]
        public int sumGold { get; protected set; }

        /// <summary>
        /// 转换成显示数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public JsonData convertToDisplayData(string type = "") {
            var res = toJson();
            res["sum_timespan"] = sumTimespan / 1000;
            res["avg_timespan"] = (int)avgTimespan / 1000;
            res["corr_timespan"] = (int)corrTimespan / 1000;
            return res;
        }
    }

    /// <summary>
    /// 状态枚举
    /// </summary>
    public enum Status {
        // 已注册，未创建角色
        Uncreated = 1,  // 未创建
        CharacterCreated = 2,  // 已创建人物
        ExermonsCreated = 3,  // 已选择艾瑟萌
        GiftsCreated = 4,  // 已选择天赋

        // 已完全创建角色
        Normal = 10,  // 正常
        Banned = 20,  // 封禁
        Frozen = 30,  // 冻结
        Other = 0  // 其他
    }

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public string username { get; protected set; }
    [AutoConvert]
    public string password { get; protected set; }
    [AutoConvert]
    public string phone { get; protected set; }
    [AutoConvert]
    public string email { get; protected set; }
    [AutoConvert]
    public string name { get; protected set; }
    [AutoConvert]
    public int characterId { get; protected set; }
    [AutoConvert]
    public int grade { get; protected set; }
    [AutoConvert]
    public int status { get; protected set; }
    [AutoConvert]
    public int type { get; protected set; }
    [AutoConvert]
    public bool online { get; protected set; }
    [AutoConvert]
    public int exp { get; protected set; }
    [AutoConvert]
    public int level { get; protected set; }
    [AutoConvert]
    public int next { get; protected set; }
    [AutoConvert]
    public DateTime createTime { get; protected set; }
    [AutoConvert(format: "date")]
    public DateTime birth { get; protected set; }
    [AutoConvert]
    public string school { get; protected set; }
    [AutoConvert]
    public string city { get; protected set; }
    [AutoConvert]
    public string contact { get; protected set; }
    [AutoConvert]
    public string description { get; protected set; }

    [AutoConvert]
    public ItemPrice money { get; protected set; }

    [AutoConvert]
    public PackContainerInfo packContainers { get; protected set; }
    [AutoConvert]
    public SlotContainerInfo slotContainers { get; protected set; }

    [AutoConvert]
    public BattleInfo battleInfo { get; protected set; }
    [AutoConvert]
    public QuestionInfo questionInfo { get; protected set; }

    #region 信息转换

    /// <summary>
    /// 转化为属性信息集
    /// </summary>
    /// <returns>属性信息集</returns>
    public JsonData convertToDisplayData(string type = "") {
        switch (type.ToLower()) {
            case "exp": return convertExp();
            case "params_info": return covnertParamsInfo();
            case "battle_info": return battleInfo.convertToDisplayData();
            case "question_info": return questionInfo.convertToDisplayData();
            case "personal_info": return covnertPersonalInfo();
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
        json["rate"] = next == 0 ? 0 : exp / next;
        return json;
    }

    /// <summary>
    /// 转化为属性信息
    /// </summary>
    /// <returns></returns>
    JsonData covnertParamsInfo() {
        var exerSlot = slotContainers.exerSlot;
        var json = new JsonData();
        var params_ = DataService.get().staticData.configure.baseParams;

        json["sum_mhp"] = (int)exerSlot.sumParam(params_[0].getID()).value;
        json["sum_mmp"] = (int)exerSlot.sumParam(params_[1].getID()).value;
        json["avg_atk"] = exerSlot.avgParam(params_[2].getID()).value;
        json["avg_def"] = exerSlot.avgParam(params_[3].getID()).value;
        json["avg_eva"] = exerSlot.avgParam(params_[4].getID()).value;
        json["avg_cri"] = exerSlot.avgParam(params_[5].getID()).value;
        json["sum_bp"] = exerSlot.sumBattlePoint();

        return json;
    }

    /// <summary>
    /// 转化为属性信息
    /// </summary>
    /// <returns></returns>
    JsonData covnertPersonalInfo() {
        var json = new JsonData();
        json["grade"] = gradeText();
        json["school"] = school;
        json["birth"] = DataLoader.convert(birth);
        json["city"] = city;
        json["contact"] = contact;
        json["description"] = description;
        return json;
    }

    #endregion

    #region 数据操作

    #region 登陆/创建/修改

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
    /// 修改昵称
    /// </summary>
    /// <param name="name"></param>
    public void editNmae(string name) {
        this.name = name;
    }

    /// <summary>
    /// 修改个人信息
    /// </summary>
    /// <param name="grade">年级</param>
    /// <param name="birth">生日</param>
    /// <param name="school">学校</param>
    /// <param name="city">城市</param>
    /// <param name="contact">联系方式</param>
    /// <param name="description">个人描述</param>
    public void editInfo(int grade, DateTime birth, string school,
        string city, string contact, string description) {
        this.grade = grade;
        this.birth = birth;
        this.school = school;
        this.city = city;
        this.contact = contact;
        this.description = description;
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

    #endregion

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
    /// 获取所选科目
    /// </summary>
    /// <returns></returns>
    public Subject[] subjects() {
        return slotContainers.exerSlot.subjects();
    }

    #region 状态判断

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

    #endregion

    /// <summary>
    /// 获取艾瑟萌槽项
    /// </summary>
    /// <param name="subject"></param>
    /// <returns></returns>
    public ExerSlotItem getExerSlotItem(Subject subject) {
        var exerSlot = slotContainers.exerSlot;
        return exerSlot.getSlotItem(subject.getID());
    }

    #endregion
}

#region 物品

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
    [AutoConvert]
    public int eType { get; protected set; }

    /// <summary>
    /// 获取装备类型
    /// </summary>
    /// <returns>装备类型</returns>
    public TypeData equipType() {
        return DataService.get().humanEquipType(eType);
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
    }
    */
}

#endregion

#region 容器

/// <summary>
/// 人类背包
/// </summary>
public class HumanPack : PackContainer<PackContItem> {

    /// <summary>
    /// 读取单个物品
    /// </summary>
    /// <param name="json"></param>
    protected override PackContItem loadItem(JsonData json) {
        var type = DataLoader.load<int>(json, "type");
        if (type == (int)BaseContItem.Type.HumanPackItem)
            return DataLoader.load<HumanPackItem>(json);
        if (type == (int)BaseContItem.Type.HumanPackEquip)
            return DataLoader.load<HumanPackEquip>(json);
        return null;
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
    public override HumanEquipSlotItem getSlotItem(int eType) {
        return getItem((item) => item.eType == eType);
    }

    /// <summary>
    /// 通过装备物品获取槽ID
    /// </summary>
    /// <typeparam name="E">装备物品类型</typeparam>
    /// <param name="equipItem">装备物品</param>
    /// <returns>槽ID</returns>
    public override HumanEquipSlotItem getSlotItemByEquipItem<E>(E equipItem) {
        if (typeof(E) == typeof(HumanPackEquip)) {
            var eType = ((HumanPackEquip)(object)equipItem).equip().eType;
            return getSlotItem(eType);
        }
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

#endregion

#region 容器项

/// <summary>
/// 人类背包物品
/// </summary>
public class HumanPackItem : PackContItem<HumanItem> {
}

/// <summary>
/// 人类背包装备
/// </summary>
public class HumanPackEquip : PackContItem<HumanEquip> {
    /// <summary>
    /// 获取装备实例
    /// </summary>
    /// <returns>装备</returns>
    public HumanEquip equip() { return item(); }

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
public class HumanEquipSlotItem : SlotContItem<HumanPackEquip> {

    /// <summary>
    /// 属性
    /// </summary>
    [AutoConvert]
    public HumanPackEquip packEquip { get; protected set; }
    [AutoConvert]
    public int eType { get; protected set; }

    /// <summary>
    /// 装备
    /// </summary>
    public override HumanPackEquip equip1 {
        get { return packEquip; }
        protected set { packEquip = value; }
    }

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

#endregion