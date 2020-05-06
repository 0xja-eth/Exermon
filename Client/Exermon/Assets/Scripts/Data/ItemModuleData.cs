
using System;
using System.Collections.Generic;
using UnityEngine;

using LitJson;

using Core.Data;
using Core.Data.Loaders;

using GameModule.Data;
using GameModule.Services;

using UI.Common.Controls.ParamDisplays;

/// <summary>
/// 物品模块
/// </summary>
namespace ItemModule { }

/// <summary>
/// 物品模块数据
/// </summary>
namespace ItemModule.Data {

    /// <summary>
    /// 基本物品数据
    /// </summary>
    public class BaseItem : BaseData,
        ParamDisplay.IDisplayDataConvertable {

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
        
        #region 属性显示数据生成

        /// <summary>
        /// 转化为属性信息集
        /// </summary>
        /// <returns>属性信息集</returns>
        public virtual JsonData convertToDisplayData(string type = "") {
            var res = new JsonData();

            res["name"] = name;
            res["description"] = description;

            return res;
        }

        #endregion

        /// <summary>
        /// 最大叠加数量
        /// </summary>
        /// <returns></returns>
        public virtual int maxCount() { return 1; }
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
    }

    /// <summary>
    /// 有限物品数据
    /// </summary>
    public class LimitedItem : BaseItem {

        /// <summary>
        /// 常量定义
        /// </summary>
        const string UnsellableText = "不可出售";

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int starId { get; protected set; }
        [AutoConvert]
        public int iconIndex { get; protected set; }
        [AutoConvert]
        public ItemPrice buyPrice { get; protected set; }
        [AutoConvert]
        public int sellPrice { get; protected set; }
        [AutoConvert]
        public bool discardable { get; protected set; }
        [AutoConvert]
        public bool tradable { get; protected set; }

        /// <summary>
        /// 物品图标
        /// </summary>
        public Sprite icon { get; protected set; }

        #region 数据转换

        /// <summary>
        /// 转化为属性信息集
        /// </summary>
        /// <returns>属性信息集</returns>
        public override JsonData convertToDisplayData(string type = "") {
            var res = base.convertToDisplayData(type);

            res["sell_price"] = sellPrice;
            res["sellable"] = sellable();
            res["discardable"] = discardable;
            res["tradable"] = tradable;

            return res;
        }

        #endregion

        /// <summary>
        /// 获取星级实例
        /// </summary>
        /// <returns></returns>
        public ItemStar star() {
            return DataService.get().itemStar(starId);
        }

        /// <summary>
        /// 可否出售
        /// </summary>
        /// <returns></returns>
        public bool sellable() {
            return sellPrice > 0;
        }
        
        /// <summary>
        /// 加载自定义属性
        /// </summary>
        /// <param name="json"></param>
        protected override void loadCustomAttributes(JsonData json) {
            base.loadCustomAttributes(json);
            icon = AssetLoader.getItemIconSprite(iconIndex);
        }
    }

    /// <summary>
    /// 可转化为效果数据的接口
    /// </summary>
    public interface IEffectsConvertable {

        /// <summary>
        /// 转化为效果数据数组（用于对战中显示）
        /// </summary>
        /// <returns>返回对应的效果数据数组</returns>
        EffectData[] convertToEffectData();
    }

    /// <summary>
    /// 物品使用场合
    /// </summary>
    public enum ItemUseOccasion {
        Unknown = 0, // 未知
        Battle = 1, // 对战中
	    Menu = 2, // 菜单（背包中）
	    Adventure = 3, // 冒险中
    }

    /// <summary>
    /// 使用效果数据
    /// </summary>
    public class EffectData : BaseData {

        /// <summary>
        /// 效果代码枚举
        /// </summary>
        public enum Code {
            Unset = 0, // 空

            RecoverHP = 10, // 回复体力值
            RecoverMP = 11, // 回复精力值
            AddParam = 20, // 增加能力值
            TempAddParam = 21, // 临时增加能力值
            BattleAddParam = 22, // 战斗中增加能力值

            GainItem = 30, // 获得物品
            GainGold = 31, // 获得金币
            GainBoundTicket = 32, // 获得绑定点券
            GainExermonExp = 40, // 指定艾瑟萌获得经验
            GainExerSlotItemExp = 41, // 指定艾瑟萌槽项获得经验
            GainPlayerExp = 42, // 玩家获得经验

            Eval = 99, // 执行程序        
        }

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int code { get; protected set; }
        [AutoConvert("params")]
        public JsonData params_ { get; protected set; } // 参数（数组）
        [AutoConvert]
        public string description { get; protected set; }
        [AutoConvert]
        public string shortDescription { get; protected set; }

        /// <summary>
        /// 是否为恢复效果
        /// </summary>
        /// <returns>返回当前效果是否为恢复效果</returns>
        public bool isRecovery() {
            return code == (int)Code.RecoverHP || code == (int)Code.RecoverMP;
        }

        /// <summary>
        /// 是否为提升效果
        /// </summary>
        /// <returns>返回当前效果是否为提升效果</returns>
        public bool isPromotion() {
            return code == (int)Code.AddParam || 
                code == (int)Code.TempAddParam || 
                code == (int)Code.BattleAddParam;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public EffectData() { }
        public EffectData(Code code, JsonData params_, 
            string description, string shortDescription) {
            this.code = (int)code;
            this.params_ = params_;
            this.description = description;
            this.shortDescription = shortDescription;
        }
    }
    
    /// <summary>
    /// 物品使用目标接口
    /// </summary>
    public interface IItemUseTarget {

        /// <summary>
        /// 修改HP
        /// </summary>
        /// <param name="value">HP</param>
        void changeHP(int value);

        /// <summary>
        /// 修改HP（百分比）
        /// </summary>
        /// <param name="rate">HP百分比</param>
        void changePercentHP(double rate);

        /// <summary>
        /// 修改MP
        /// </summary>
        /// <param name="value">MP</param>
        void changeMP(int value);

        /// <summary>
        /// 修改MP（百分比）
        /// </summary>
        /// <param name="rate">MP百分比</param>
        void changePercentMP(double rate);

        /// <summary>
        /// 添加属性值
        /// </summary>
        /// <param name="paramId">属性ID</param>
        /// <param name="value">属性值</param>
        void changeParam(int paramId, double value);

        /// <summary>
        /// 添加属性值（百分比）
        /// </summary>
        /// <param name="paramId">属性ID</param>
        /// <param name="rate">属性值（百分比）</param>
        void changePercentParam(int paramId, double rate);

    }

    /// <summary>
    /// 可用物品数据
    /// </summary>
    public class UsableItem : LimitedItem, IEffectsConvertable {

        /// <summary>
        /// 使用目标类型
        /// </summary>
        public enum UseTargetType {
            Empty = 0, Human = 1, Exermon = 2
        }

        /// <summary>
        /// 常量定义
        /// </summary>
        const string UnusableText = "不可使用";
        const string BattleUseText = "对战";
        const string MenuUseText = "背包";
        const string AdventureUseText = "冒险";

        const string EmptyEffectText = "无";

        const string Spliter = "|";

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert("max_count")]
        public int maxCount_ { get; protected set; }
        [AutoConvert]
        public bool battleUse { get; protected set; }
        [AutoConvert]
        public bool menuUse { get; protected set; }
        [AutoConvert]
        public bool adventureUse { get; protected set; }
        [AutoConvert]
        public bool consumable { get; protected set; }
        [AutoConvert]
        public int target { get; protected set; }
        [AutoConvert]
        public int freeze { get; protected set; }
        [AutoConvert]
        public int iType { get; protected set; }

        /// <summary>
        /// 使用效果
        /// </summary>
        [AutoConvert]
        public List<EffectData> effects { get; protected set; }

        #region 数据转换

        /// <summary>
        /// 转化为属性信息集
        /// </summary>
        /// <returns>属性信息集</returns>
        public override JsonData convertToDisplayData(string type = "") {
            var res = base.convertToDisplayData(type);
            var occasion = generateTimingText();

            res["type"] = itemType().name;

            res["max_count"] = maxCount_;

            res["battle_use"] = battleUse;
            res["menu_use"] = menuUse;
            res["adventure_use"] = adventureUse;

            res["occasion"] = occasion;
            res["usable"] = occasion == UnusableText;

            res["target"] = targetText();
            res["freeze"] = freeze;

            res["effects"] = generateEffectsText();

            return res;
        }

        /// <summary>
        /// 生成使用场合文本
        /// </summary>
        /// <returns></returns>
        string generateTimingText() {
            List<string> texts = new List<string>();

            if (battleUse) texts.Add(BattleUseText);
            if (menuUse) texts.Add(MenuUseText);
            if (adventureUse) texts.Add(AdventureUseText);

            string res = string.Join(Spliter, texts);

            return res == "" ? UnusableText : res;
        }

        /// <summary>
        /// 生成效果文本
        /// </summary>
        /// <returns></returns>
        string generateEffectsText() {
            var res = "";
            foreach (var effect in effects)
                res += effect.description + "\n";
            return res == "" ? EmptyEffectText : res;
        }

        #endregion

        /// <summary>
        /// 获取物品类型
        /// </summary>
        /// <returns>物品类型</returns>
        public UsableItemType itemType() {
            return DataService.get().usableItemType(iType);
        }

        /// <summary>
        /// 目标类型文本
        /// </summary>
        /// <returns>目标类型文本</returns>
        public string targetText() {
            return DataService.get().itemUseTargetType(target).Item2;
        }

        /// <summary>
        /// 最大叠加数量
        /// </summary>
        /// <returns></returns>
        public override int maxCount() { return maxCount_; }
        
        /// <summary>
        /// 转化为效果数据数组（用于对战中显示）
        /// </summary>
        /// <returns>返回对应的效果数据数组</returns>
        public EffectData[] convertToEffectData() {
            return effects.ToArray();
        }
    }

    /// <summary>
    /// 可装备物品数据
    /// </summary>
    public class EquipableItem : LimitedItem,
        ParamDisplay.IDisplayDataArrayConvertable {

        /// <summary>
        /// 属性类型
        /// </summary>
        //public enum ParamType {
        //    NoType = 0, // 无类型
        //    Attack = 1, // 攻击型
        //    Defense = 2 // 防御型
        //}

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public ParamRateData[] levelParams { get; protected set; }
        [AutoConvert]
        public ParamData[] baseParams { get; protected set; }
        [AutoConvert]
        public int minLevel { get; protected set; }
        
        #region 数据转换

        /// <summary>
        /// 转化为属性信息集
        /// </summary>
        /// <returns>属性信息集</returns>
        public override JsonData convertToDisplayData(string type = "") {
            var res = base.convertToDisplayData(type);

            res["min_level"] = minLevel;
            res["params"] = DataLoader.convert(
                convertToDisplayDataArray(type));

            return res;
        }

        /// <summary>
        /// 转化为属性信息集
        /// </summary>
        /// <returns>属性信息集</returns>
        public JsonData[] convertToDisplayDataArray(string type = "") {
            var params_ = DataService.get().staticData.configure.baseParams;
            var count = params_.Length;
            var data = new JsonData[count];
            for (int i = 0; i < count; ++i) {
                var json = new JsonData();
                var paramId = params_[i].getID();

                var levelParam = getLevelParam(paramId).value;
                var baseParam = getBaseParam(paramId).value;

                json["level_param"] = levelParam;
                json["equip_param"] = baseParam;

                data[i] = json;
            }
            return data;
        }

        #endregion

        /// <summary>
        /// 获取装备的属性
        /// </summary>
        /// <param name="paramId">属性ID</param>
        /// <returns>属性数据</returns>
        public ParamRateData getLevelParam(int paramId) {
            foreach (var param in levelParams)
                if (param.paramId == paramId) return param;
            return new ParamRateData(paramId);
        }

        /// <summary>
        /// 获取装备的属性
        /// </summary>
        /// <param name="paramId">属性ID</param>
        /// <returns>属性数据</returns>
        public ParamData getBaseParam(int paramId) {
            foreach (var param in baseParams)
                if (param.paramId == paramId) return param;
            return new ParamData(paramId);
        }
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

            // 对战物资槽 
            BattleItemSlotItem = 301,  // 对战物资槽
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
        //public abstract Type defaultType();

        /// <summary>
        /// 是否为空
        /// </summary>
        /// <returns></returns>
        public virtual bool isNullItem() { return false; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseContItem() {
            var typeName = GetType().Name;
            type = (int)Enum.Parse(typeof(Type), typeName);
        }
    }

    /// <summary>
    /// 背包类容器项
    /// </summary>
    public class PackContItem : BaseContItem {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int itemId { get; protected set; }
        [AutoConvert]
        public int count { get; protected set; }
        [AutoConvert]
        public bool equiped { get; protected set; } = false;

        /// <summary>
        /// 容器项容量（0为无限）
        /// </summary>
        /// <returns></returns>
        public virtual int capacity() { return 1; }

        /// <summary>
        /// 是否已满
        /// </summary>
        /// <returns></returns>
        public bool isFull() { return capacity() > 0 && count >= capacity(); }

        /// <summary>
        /// 是否为空
        /// </summary>
        /// <returns></returns>
        public override bool isNullItem() {
            return itemId == 0;
        }

        /// <summary>
        /// 移入
        /// </summary>
        /// <param name="count"></param>
        public int enter(int count) {
            this.count += count;
            if (capacity() <= 0) return 0; // 如果可以无限叠加

            var res = this.count - capacity();
            if (this.count > capacity())
                this.count = capacity();
            return Math.Max(0, res);
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="count"></param>
        public int leave(int count) {
            this.count -= count;
            var res = -this.count;
            if (this.count < 0) this.count = 0;
            return Math.Max(0, res);
        }

        /// <summary>
        /// 装备
        /// </summary>
        public void doEquip() { equiped = true; }

        /// <summary>
        /// 卸下
        /// </summary>
        public void doDequip() { equiped = false; }

        /// <summary>
        /// 设置个数
        /// </summary>
        /// <param name="count">个数</param>
        /// <returns></returns>
        public void setCount(int count) {
            this.count = count;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public PackContItem() { }
        /// <param name="itemId">物品ID</param>
        /// <param name="count">数量</param>
        public PackContItem(int itemId, int count = 1) {
            this.itemId = itemId;
            this.count = count;
        }

    }

    /// <summary>
    /// 背包类容器项
    /// </summary>
    public abstract class PackContItem<T> : PackContItem where T : BaseItem {

        /// <summary>
        /// 物品
        /// </summary>
        /// <returns></returns>
        public T item() {
            return DataService.get().get<T>(itemId);
        }

        /// <summary>
        /// 是否为空
        /// </summary>
        /// <returns></returns>
        public override bool isNullItem() {
            return base.isNullItem() || item() == null;
        }

        /// <summary>
        /// 容器项容量（0为无限）
        /// </summary>
        /// <returns></returns>
        public override int capacity() { return item().maxCount(); }

        /// <summary>
        /// 构造函数
        /// </summary>
        public PackContItem() : base() { }
        /// <param name="itemId">物品ID</param>
        /// <param name="count">数量</param>
        public PackContItem(int itemId, int count = 1) : base(itemId, count) { }
        /// <param name="item">物品</param>
        public PackContItem(T item, int count = 1) : base(item.getID(), count) { }

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

        /// <summary>
        /// 槽索引
        /// </summary>
        public virtual int slotIndex {
            get { return index; }
            set { index = value; }
        }

        /// <summary>
        /// 获取装备
        /// </summary>
        /// <typeparam name="E">装备类型</typeparam>
        /// <returns>装备</returns>
        public virtual E getEquip<E>() where E : PackContItem, new() { return null; }

        /// <summary>
        /// 设置装备
        /// </summary>
        /// <typeparam name="E">装备类型</typeparam>
        /// <param name="equipItem">装备物品</param>
        public virtual void setEquip<E>(E equipItem) where E : PackContItem, new() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SlotContItem() : base() {}
    }

    /// <summary>
    /// 槽类容器项（单装备）
    /// </summary>
    public abstract class SlotContItem<T> : SlotContItem where T : PackContItem, new() {

        /// <summary>
        /// 装备
        /// </summary>
        public abstract T equip1 { get; protected set; }

        /// <summary>
        /// 是否为空
        /// </summary>
        /// <returns></returns>
        public override bool isNullItem() {
            return equip1 == null || equip1.isNullItem();
        }

        /// <summary>
        /// 获取装备
        /// </summary>
        /// <typeparam name="E">装备类型</typeparam>
        /// <returns>装备</returns>
        public override E getEquip<E>() {
            var et = typeof(E); var tt = typeof(T);
            if (et == tt || tt.IsSubclassOf(et)) return (E)(object)equip1;
            return null;
        }

        /// <summary>
        /// 获取装备
        /// </summary>
        /// <typeparam name="E">装备类型</typeparam>
        /// <param name="equipItem">装备物品</param>
        public override void setEquip<E>(E equipItem) {
            var et = typeof(E); var tt = typeof(T);
            if (et == tt || tt.IsSubclassOf(et)) {
                var lastEquip = equip1;
                equip1 = (T)(object)equipItem;
                if (lastEquip != equip1) onEquipChanged();
            }
        }

        /// <summary>
        /// 装备改变回调
        /// </summary>
        protected virtual void onEquipChanged() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SlotContItem() : base() { }
    }

    /// <summary>
    /// 槽类容器项（双装备）
    /// </summary>
    public abstract class SlotContItem<T1, T2> : SlotContItem<T1>
        where T1 : PackContItem, new() where T2 : PackContItem, new() {

        /// <summary>
        /// 第二装备
        /// </summary>
        public abstract T2 equip2 { get; protected set; }

        /// <summary>
        /// 是否为空
        /// </summary>
        /// <returns></returns>
        public override bool isNullItem() {
            return base.isNullItem() && (equip2 == null || equip2.isNullItem());
        }

        /// <summary>
        /// 获取装备
        /// </summary>
        /// <typeparam name="E">装备类型</typeparam>
        /// <returns>装备</returns>
        public override E getEquip<E>() {
            var et = typeof(E); var t2t = typeof(T2);
            if (et == t2t || t2t.IsSubclassOf(et)) return (E)(object)equip2;
            return base.getEquip<E>();
        }

        /// <summary>
        /// 设置装备
        /// </summary>
        /// <typeparam name="E">装备类型</typeparam>
        /// <param name="equipItem">装备物品</param>
        public override void setEquip<E>(E equipItem) {
            var et = typeof(E); var t2t = typeof(T2);
            if (et == t2t || t2t.IsSubclassOf(et)) {
                var lastEquip = equip2;
                equip2 = (T2)(object)equipItem;
                if (lastEquip != equip2) onEquipChanged();
            } else base.setEquip(equipItem);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SlotContItem() : base() { }

    }

    /// <summary>
    /// 基本容器数据
    /// </summary>
    public class BaseContainer<T> : BaseData where T : BaseContItem, new() {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int type { get; protected set; }
        [AutoConvert]
        public int capacity { get; protected set; }

        //[AutoConvert]
        public List<T> items { get; protected set; } = new List<T>();

        #region 数据操作

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

        #endregion

        #region 数据读取

        /// <summary>
        /// 读取自定义属性
        /// </summary>
        /// <param name="json"></param>
        protected override void loadCustomAttributes(JsonData json) {
            base.loadCustomAttributes(json);

            var data = DataLoader.load(json, "items");
            if (data != null && data.IsArray) {
                items.Clear();
                for (int i = 0; i < data.Count; ++i) {
                    var item = loadItem(data[i]);
                    if (item != null) items.Add(item);
                }
            }
        }

        /// <summary>
        /// 读取单个物品
        /// </summary>
        /// <param name="json">数据</param>
        protected virtual T loadItem(JsonData json) {
            return DataLoader.load<T>(json);
        }

        /// <summary>
        /// 转化自定义数据
        /// </summary>
        /// <param name="json">数据</param>
        protected override void convertCustomAttributes(ref JsonData json) {
            base.convertCustomAttributes(ref json);
            json["items"] = DataLoader.convert(items);
        }

        #endregion
    }

    /// <summary>
    /// 背包类容器数据
    /// </summary>
    public class PackContainer<T> : BaseContainer<T> where T : PackContItem, new() {

        /// <summary>
        /// 添加物品
        /// </summary>
        /// <param name="item">物品</param>
        public void pushItem(T item) {
            if (item == null) return;
            items.Add(item);
        }

        /// <summary>
        /// 移除物品
        /// </summary>
        /// <param name="item">物品</param>
        public void removeItem(T item) {
            if (item == null) return;
            items.Remove(item);
        }

        /// <summary>
        /// 拆分物品
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="count">数目</param>
        public void splitItem(T item, int count) {
            if (count >= item.count) return;
            var copyItem = (T)item.copy(false);
            copyItem.setCount(count);
            pushItem(copyItem);
            item.leave(count);
        }

        /// <summary>
        /// 拆分物品
        /// </summary>
        /// <param name="items">物品数组</param>
        public void mergeItems(T[] items) {
            var leftIndex = 0;
            var leftItem = items[leftIndex];
            for (int i = 1; i < items.Length; i++) {
                var item = items[i];
                item.setCount(leftItem.enter(item.count));
                if (leftItem.isFull()) // 如果最左物品已满，切换之
                    leftItem = items[++leftIndex];
            }
            for (int i = items.Length - 1; i > leftIndex; --i)
                removeItem(items[i]);
        }

    }

    /// <summary>
    /// 背包类容器数据
    /// </summary>
    public abstract class SlotContainer<T> : BaseContainer<T> where T : SlotContItem, new() {

        /// <summary>
        /// 获取装备
        /// </summary>
        /// <typeparam name="E">装备类型</typeparam>
        /// <returns>装备</returns>
        public E getEquip<E>(T slotItem) where E : PackContItem, new() {
            return slotItem.getEquip<E>();
        }

        /// <summary>
        /// 设置装备
        /// </summary>
        /// <typeparam name="E">装备类型</typeparam>
        /// <param name="container">装备容器</param>
        /// <param name="equipItem">装备物品</param>
        public void setEquip<E>(PackContainer<E> container, E equipItem = null)
            where E : PackContItem, new() {
            setEquip(getSlotItemByEquipItem(equipItem), container, equipItem);
        }
        public void setEquip<E>(T slotItem, PackContainer<E> container, E equipItem = null) where E : PackContItem, new() {
            if (slotItem == null) return;
            var oriEquip = getEquip<E>(slotItem);
            container.removeItem(equipItem); // 移出装备
            container.pushItem(oriEquip); // 卸下原装备
            setEquip(slotItem, equipItem); // 设置新装备
            if (oriEquip != null) oriEquip.doDequip();
            if (equipItem != null) equipItem.doEquip();
        }
        public void setEquip<E>(int slotIndex, PackContainer<E> container, E equipItem = null) where E : PackContItem, new() {
            setEquip(getSlotItem(slotIndex), container, equipItem);
        }
        public void setEquip<E>(E equipItem = null) where E : PackContItem, new() {
            setEquip(getSlotItemByEquipItem(equipItem), equipItem);
        }
        public void setEquip<E>(T slotItem, E equipItem = null) where E : PackContItem, new() {
            slotItem?.setEquip(equipItem);
        }
        public void setEquip<E>(int slotIndex, E equipItem = null) where E : PackContItem, new() {
            setEquip(getSlotItem(slotIndex), equipItem);
        }

        /// <summary>
        /// 通过索引获取槽
        /// </summary>
        /// <param name="index"></param>
        /// <returns>返回对应索引的装备槽项</returns>
        public virtual T getSlotItem(int slotIndex) {
            return getItem(item => item.slotIndex == slotIndex);
        }

        /// <summary>
        /// 通过索引获取槽
        /// </summary>
        /// <param name="index">下标</param>
        /// <returns>返回对应索引的<装备槽项/returns>
        public virtual T getSlotItemByIndex(int index) {
            return getItem(item => item.index == index);
        }

        /// <summary>
        /// 通过装备物品获取槽
        /// </summary>
        /// <typeparam name="E">装备物品类型</typeparam>
        /// <param name="equipItem">装备物品</param>
        /// <returns>槽ID</returns>
        public abstract T getSlotItemByEquipItem<E>(E equipItem) where E : PackContItem, new();

        /// <summary>
        /// 查找一个空的容器项
        /// </summary>
        /// <returns>返回第一个空的容器项</returns>
        public T emptySlotItem() {
            foreach (var item in items)
                if (item.isNullItem()) return item;
            return items[0];
        }

    }
}