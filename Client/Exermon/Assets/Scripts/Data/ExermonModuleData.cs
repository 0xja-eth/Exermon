
using System;
using System.Collections.Generic;
using UnityEngine;

using LitJson;
using System.Linq;

using Core.Data;
using Core.Data.Loaders;

using GameModule.Data;
using GameModule.Services;

using ItemModule.Data;
using PlayerModule.Data;

using UI.Common.Controls.ParamDisplays;

/// <summary>
/// 艾瑟萌模块
/// </summary>
namespace ExermonModule { }

/// <summary>
/// 艾瑟萌模块数据
/// </summary>
namespace ExermonModule.Data {

    #region 物品

    /// <summary>
    /// 艾瑟萌数据
    /// </summary>
    public class Exermon : BaseItem, 
        ParamDisplay.IDisplayDataArrayConvertable {

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
        public ParamRateData[] rateParams { get; protected set; }

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
        public ParamRateData paramRate(int paramId) {
            foreach (var param in rateParams)
                if (param.paramId == paramId) return param;
            return new ParamRateData(paramId);
        }

        #endregion

        #endregion

        /// <summary>
        /// 数据加载
        /// </summary>
        /// <param name="json">数据</param>
        protected override void loadCustomAttributes(JsonData json) {
            base.loadCustomAttributes(json);
            full = AssetLoader.loadExermonFull(id);
            icon = AssetLoader.loadExermonIcon(id);
            battle = AssetLoader.loadExermonBattle(id);
        }
    }

    /// <summary>
    /// 艾瑟萌碎片数据
    /// </summary>
    public class ExerGift : BaseItem,
        ParamDisplay.IDisplayDataArrayConvertable {

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
        public ParamRateData[] params_ { get; protected set; }

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
            icon = AssetLoader.loadExerGift(id);
            bigIcon = AssetLoader.loadBigExerGift(id);
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
    public class ExerSkill : BaseItem,
        ParamDisplay.IDisplayDataConvertable {

        /// <summary>
        /// 目标类型
        /// </summary>
        public enum TargetType {
            Empty = 0, // 无
            Self = 1, // 己方
            Enemy = 2, // 敌方
            BothRandom = 3, // 双方随机
            Both = 4 // 双方全部
        }

        /// <summary>
        /// 命中类型枚举
        /// </summary>
        public enum HitType {
            Empty = 0, // 无
            HPDamage = 1, // 体力值伤害
	        HPRecover = 2, // 体力值回复
	        HPDrain = 3, // 体力值吸收
	        MPDamage = 4, // 精力值伤害
	        MPRecover = 5, // 精力值回复
	        MPDrain = 6, // 精力值吸收
        }

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
        public int drainRate { get; protected set; }
        [AutoConvert]
        public int atkRate { get; protected set; }
        [AutoConvert]
        public int defRate { get; protected set; }

        public Texture2D icon { get; protected set; }
        public Texture2D ani { get; protected set; }
        public Texture2D targetAni { get; protected set; }

        #region 转换为显示数据

        /// <summary>
        /// 转换为显示数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public JsonData convertToDisplayData(string type = "") {
            var res = toJson();
            res["target"] = targetText();
            res["hit_type"] = hitTypeText();
            res["effects"] = "";
            return res;
        }

        #endregion

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

            icon = AssetLoader.loadExerSkillIcon(id);
            ani = AssetLoader.loadExerSkillAni(id);
            targetAni = AssetLoader.loadExerSkillTarget(id);

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

        #region 数据转换

        /// <summary>
        /// 转化为属性信息集
        /// </summary>
        /// <returns>属性信息集</returns>
        public override JsonData convertToDisplayData(string type = "") {
            var res = base.convertToDisplayData(type);

            res["type"] = equipType().name;

            return res;
        }

        #endregion

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
    public class ExerPackItem : PackContItem<ExerItem>,
        ParamDisplay.IDisplayDataConvertable {

        #region 属性显示数据生成

        /// <summary>
        /// 转化为属性信息集
        /// </summary>
        /// <returns>属性信息集</returns>
        public JsonData convertToDisplayData(string type = "") {
            var res = new JsonData();

            return res;
        }

        #endregion

        /// <summary>
        /// 容器项容量（0为无限）
        /// </summary>
        /// <returns></returns>
        public override int capacity() { return item().maxCount(); }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerPackItem() : base() { }
    }

    /// <summary>
    /// 艾瑟萌背包装备
    /// </summary>
    public class ExerPackEquip : PackContItem<ExerEquip>,
        ParamDisplay.IDisplayDataConvertable,
        ParamDisplay.IDisplayDataArrayConvertable {

        #region 属性显示数据生成

        /// <summary>
        /// 转化为属性信息集
        /// </summary>
        /// <returns>属性信息集</returns>
        public JsonData convertToDisplayData(string type = "") {
            var res = item().convertToDisplayData(type);
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
                var paramId = params_[i].id;

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
        /// 获取装备实例
        /// </summary>
        /// <returns>装备</returns>
        public ExerEquip equip() { return item(); }

        /// <summary>
        /// 获取装备的等级属性
        /// </summary>
        /// <returns>属性数据数组</returns>
        public ParamRateData[] getLevelParams() {
            var equip = this.equip();
            if (equip == null) return new ParamRateData[0];
            return equip.levelParams;
        }

        /// <summary>
        /// 获取装备的基础属性
        /// </summary>
        /// <returns>属性数据数组</returns>
        public ParamData[] getBaseParams() {
            var equip = this.equip();
            if (equip == null) return new ParamData[0];
            return equip.baseParams;
        }

        /// <summary>
        /// 获取装备的等级属性
        /// </summary>
        /// <param name="paramId">属性ID</param>
        /// <returns>属性数据</returns>
        public ParamRateData getLevelParam(int paramId) {
            var equip = this.equip();
            if (equip == null) return new ParamRateData(paramId);
            return equip.getLevelParam(paramId);
        }

        /// <summary>
        /// 获取装备的基础属性
        /// </summary>
        /// <param name="paramId">属性ID</param>
        /// <returns>属性数据</returns>
        public ParamData getBaseParam(int paramId) {
            var equip = this.equip();
            if (equip == null) return new ParamData(paramId);
            return equip.getBaseParam(paramId);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerPackEquip() : base() { }

    }

    /// <summary>
    /// 艾瑟萌碎片背包项
    /// </summary>
    public class ExerFragPackItem : PackContItem<ExerFrag> { }

    /// <summary>
    /// 艾瑟萌
    /// </summary>
    public class PlayerExermon : PackContItem<Exermon>,
        ParamDisplay.IDisplayDataConvertable,
        ParamDisplay.IDisplayDataArrayConvertable,
        ExpParamDisplay.IExpConvertable {

        /// <summary>
        /// 附加属性值
        /// </summary>
        public class PlusParamData : ParamData {

            /// <summary>
            /// 属性
            /// </summary>
            [AutoConvert]
            public DateTime expiresTime { get; protected set; }

            /// <summary>
            /// 是否过期
            /// </summary>
            /// <returns></returns>
            public bool isOutOfDate() {
                if (expiresTime == default) return false;
                return DateTime.Now > expiresTime;
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            public PlusParamData() { }

            /// <summary>
            /// 构造函数
            /// </summary>
            public PlusParamData(int paramId, double rate = 0) :
                base(paramId, rate) { }
        }
    
        /// <summary>
        /// 附加属性率
        /// </summary>
        public class PlusParamRateData : ParamRateData {

            /// <summary>
            /// 属性
            /// </summary>
            [AutoConvert]
            public DateTime expiresTime { get; protected set; }

            /// <summary>
            /// 是否过期
            /// </summary>
            /// <returns></returns>
            public bool isOutOfDate() {
                if (expiresTime == default) return false;
                return DateTime.Now > expiresTime;
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            public PlusParamRateData() { }

            /// <summary>
            /// 构造函数
            /// </summary>
            public PlusParamRateData(int paramId, double rate = 1) :
                base(paramId, rate) { }
        }

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
        public ParamRateData[] rateParams { get; protected set; }

        [AutoConvert]
        public List<PlusParamData> plusParamValues { get; protected set; }
        [AutoConvert]
        public List<PlusParamRateData> plusParamRates { get; protected set; }

        /// <summary>
        /// 复制的对象，用于生成装备预览
        /// </summary>
        public PlayerExermon previewObj { get; set; } = null;

        #region 属性显示数据生成

        #region 单数据生成
        
        /// <summary>
        /// 转化为属性信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>属性信息</returns>
        public JsonData convertToDisplayData(string type = "") {
            switch (type.ToLower()) {
                case "exp": return convertExp();
                case "preview_exp": return convertExp(true);
                case "battle_point":
                    return covnertBattlePoint();
                case "preview_battle_point":
                    return covnertBattlePoint(true);
                default: return toJson();
            }
        }

        /// <summary>
        /// 转化经验信息
        /// </summary>
        /// <returns></returns>
        JsonData convertExp(bool preview = false) {
            var json = new JsonData();
            if (preview && previewObj != null) {
                json = previewObj.convertExp();
                var newLevel = previewObj.level;

                json["old_exp"] = exp;
                json["old_level"] = level;
                json["level_up"] = newLevel > level;

            } else {
                json["exp"] = exp;
                json["next"] = next;
                json["level"] = level;
                json["rate"] = exp * 1.0 / next;
            }
            return json;
        }

        /// <summary>
        /// 转化战斗力信息
        /// </summary>
        /// <returns></returns>
        //JsonData covnertBattlePoint(bool preview = false) {
        //    var json = new JsonData();
        //    json["battle_point"] = battlePoint();
        //    return json;
        //}
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

        #region 多数据生成

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
        JsonData convertParams(int index, bool preview = false) {
            var json = new JsonData();
            var exermon = this.exermon();
            var param = paramValues[index].param();
            var percent = param.isPercent();

            var value = paramValues[index].value;
            var growth = rateParams[index].value;
            
            // 获取预览数据
            if (preview && previewObj != null) {

                json = previewObj.convertParams(index);

                var value2 = DataLoader.load<double>(json, "value");
                var growth2 = DataLoader.load<double>(json, "growth");

                var dtValue = value2 - value;
                var dtGrowth = growth2 - growth;

                // 原始数据
                json["old_value"] = DataLoader.convertDouble(value, !percent, percent ? 4 : 2);
                json["old_growth"] = DataLoader.convertDouble(growth);

                json["delta_value"] = DataLoader.convertDouble(dtValue, !percent, percent ? 4 : 2);
                json["delta_growth"] = DataLoader.convertDouble(dtGrowth);

                json["delta_value_rate"] = dtValue / value;
                json["delta_growth_rate"] = dtGrowth / growth;

            } else {

                var baseValue = exermon.baseParams[index].value;
                var baseGrowth = exermon.rateParams[index].value;

                var levelValue = CalcService.ExermonParamCalc.
                    calc(baseValue, baseGrowth, level) - baseValue;

                json["value"] = DataLoader.convertDouble(value, !percent, percent ? 4 : 2);
                json["growth"] = DataLoader.convertDouble(growth);

                json["base_value"] = DataLoader.convertDouble(baseValue, !percent, percent ? 4 : 2);
                json["level_value"] = DataLoader.convertDouble(levelValue, !percent, percent ? 4 : 2);
                json["equip_value"] = 0; json["gift_value"] = 0;
            }
            
            return json;
        }

        #endregion

        /// <summary>
        /// 获取经验值
        /// </summary>
        /// <returns></returns>
        int ExpParamDisplay.IExpConvertable.exp() {
            return exp;
        }

        /// <summary>
        /// 获取下一级经验
        /// </summary>
        /// <returns></returns>
        public int maxExp() {
            return next;
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
        public ParamRateData paramRate(int paramId) {
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
            var plusVal = plusParamValue(paramId).value;
            var plusRate = plusParamRate(paramId).value;
            var value = CalcService.ExermonParamCalc.calc(
                base_, rate, level, plusVal, plusRate);
            // value = Math.Round(value);
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

        #region 附加属性

        /// <summary>
        /// 获取实际属性值
        /// </summary>
        /// <param name="paramId">属性ID</param>
        /// <returns>属性数据</returns>
        public PlusParamData plusParamValue(int paramId) {
            var prarm = new PlusParamData(paramId);
            foreach (var _param in plusParamValues)
                if (_param.paramId == paramId)
                    prarm.setValue((prarm + _param));
            return prarm;
        }

        /// <summary>
        /// 获取实际属性值
        /// </summary>
        /// <param name="paramId">属性ID</param>
        /// <returns>属性数据</returns>
        public PlusParamRateData plusParamRate(int paramId) {
            var prarm = new PlusParamRateData(paramId);
            foreach (var _param in plusParamRates)
                if (_param.paramId == paramId)
                    prarm.setValue(prarm * _param);
            return prarm;
        }

        /// <summary>
        /// 添加临时能力值变量
        /// </summary>
        /// <param name="paramId">能力值ID</param>
        /// <param name="value">值</param>
        /// <param name="second">持续时间（为0则无限）</param>
        public void addPlusParamValues(
            int paramId, double value, int second = 0) {
            var param = new PlusParamData(paramId, value);
            plusParamValues.Add(param);
        }

        /// <summary>
        /// 添加临时能力值变量
        /// </summary>
        /// <param name="paramId">能力值ID</param>
        /// <param name="value">值</param>
        /// <param name="second">持续时间（为0则无限）</param>
        public void addPlusParamRates(
            int paramId, double rate, int second = 0) {
            var param = new PlusParamRateData(paramId, rate);
            plusParamRates.Add(param);
        }

        #endregion

        /// <summary>
        /// 获得经验
        /// </summary>
        /// <param name="exp"></param>
        public void gainExp(int exp) {
            this.exp += exp; refresh();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void refresh() {
            refreshLevel();
            recomputeParams();
        }

        /// <summary>
        /// 刷新等级
        /// </summary>
        void refreshLevel() {
            var exp = this.exp;
            var level = this.level;
            var star = item().star();

            var delta = CalcService.ExermonLevelCalc.
                getDetlaExp(star, level);
            if (delta < 0) return; // 已经满级

            while(exp > delta) {
                level++; exp -= (int)delta;
                if (level >= star.maxLevel) break;

                delta = CalcService.ExermonLevelCalc.
                    getDetlaExp(star, level);
            }

            if (level > this.level) onUpgrade();

            this.exp = exp;
            this.level = level;
        }

        /// <summary>
        /// 升级回调
        /// </summary>
        void onUpgrade() { }

        #endregion

        #region 预览操作

        /// <summary>
        /// 创建预览对象
        /// </summary>
        /// <returns></returns>
        public PlayerExermon createPreviewObject() {
            return previewObj = (PlayerExermon)copy();
        }

        /// <summary>
        /// 清除预览对象
        /// </summary>
        public void clearPreviewObject() {
            previewObj = null;
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
        ParamDisplay.IDisplayDataConvertable,
        ParamDisplay.IDisplayDataArrayConvertable,
        ExpParamDisplay.IExpConvertable {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int playerExerId { get; protected set; }
        [AutoConvert]
        public int playerGiftId { get; protected set; }
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
        public ParamRateData[] rateParams { get; protected set; }

		/// <summary>
		/// 玩家艾瑟萌实例
		/// </summary>
		PlayerExermon _playerExer = null;
		public PlayerExermon playerExer {
            get {
				if (_playerExer == null) {
					var exerHub = exerSlot.player.packContainers.exerHub;
					_playerExer = exerHub.findItem(item => item.id == playerExerId);
				}
				return _playerExer;
			}
            set {
                playerExerId = value.id;
				_playerExer = null;

			}
        }

		/// <summary>
		/// 玩家艾瑟萌天赋实例
		/// </summary>
		PlayerExerGift _playerGift = null;
		public PlayerExerGift playerGift {
            get {
				if (_playerGift == null) {
					var exerGiftPool = exerSlot.player.packContainers.exerGiftPool;
					var playerGift = exerGiftPool.findItem(item => item.id == playerGiftId);
					_playerGift = playerGift ?? tmpPlayerGift;
				}
				return _playerGift;
			}
            set {
                var exerGiftPool = exerSlot.player.packContainers.exerGiftPool;
                var playerGift = exerGiftPool.findItem(item => item.id == playerGiftId);
                if (playerGift == null) {
                    tmpPlayerGift = value;
                    playerGiftId = tmpPlayerGift.id;
                } else {
                    tmpPlayerGift = null;
                    playerGiftId = value.id;
                }
				_playerGift = null;
			}
        }

        /// <summary>
        /// 临时玩家艾瑟萌天赋
        /// </summary>
        PlayerExerGift tmpPlayerGift;

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

        /// <summary>
        /// 艾瑟萌槽
        /// </summary>
        public ExerSlot exerSlot { get; set; }

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
            json["rate"] = playerExer.exp * 1.0 / playerExer.next;
            return json;
        }

        /// <summary>
        /// 转化槽经验信息
        /// </summary>
        /// <returns></returns>
        JsonData convertSlotExp() {
            var json = new JsonData();
            json["subject"] = subject().name;
            json["exp"] = exp;
            json["next"] = next;
            json["level"] = level;
            json["rate"] = exp * 1.0 / next;
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

            // 获取预览数据
            if (preview && previewObj != null) {

                json = previewObj.convertParams(index);

                var value2 = DataLoader.load<double>(json, "value");
                var growth2 = DataLoader.load<double>(json, "growth");

                var dtValue = value2 - value;
                var dtGrowth = growth2 - growth;

                // 原始数据
                json["old_value"] = DataLoader.convertDouble(value, !percent, percent ? 4 : 2);
                json["old_growth"] = DataLoader.convertDouble(growth);

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
                var equipValue = exerEquipSlot.getParam(param.id).value;
                var giftValue = CalcService.ExermonParamCalc.
                    calc(baseValue, growth, level) - levelValue - baseValue;

                json["value"] = DataLoader.convertDouble(value, !percent, percent ? 4 : 2);
                json["growth"] = DataLoader.convertDouble(growth);

                json["max_growth"] = mGrowth;
                json["growth_rate"] = growth / mGrowth;

                json["old_value"] = json["value"];
                json["old_growth"] = json["growth"];

                json["delta_value"] = 0;
                json["delta_growth"] = 0;

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
            json["delta"] = DataLoader.convertDouble(delta);
            json["delta_rate"] = deltaRate;

            json[MultParamsDisplay.TrueColorKey] = DataLoader.convert(color);

            return json;
        }

        #endregion

        /// <summary>
        /// 获取经验值
        /// </summary>
        /// <returns></returns>
        int ExpParamDisplay.IExpConvertable.exp() {
            return exp;
        }

        /// <summary>
        /// 获取下一级经验
        /// </summary>
        /// <returns></returns>
        public int maxExp() {
            return next;
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
        /*
        /// <summary>
        /// 设置艾瑟萌
        /// </summary>
        /// <param name="exermon">艾瑟萌</param>
        public void setExermon(Exermon exermon) {
            setEquip(new PlayerExermon(exermon.id));
        }
        */
        /// <summary>
        /// 设置艾瑟萌天赋
        /// </summary>
        /// <param name="exerGift">艾瑟萌天赋</param>
        public void setExerGift(ExerGift exerGift) {
            //var playerExer = new PlayerExerGift(exerGift.id);
            //var exerGiftPool = exerSlot.player.packContainers.exerGiftPool;
            //exerGiftPool.pushItem(playerExer);
            setEquip(new PlayerExerGift(exerGift.id));
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
        public ParamRateData paramRate(int paramId) {
            var value = CalcService.ExerSlotItemParamRateCalc.calc(this, paramId);
            return new ParamRateData(paramId, value);
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
            previewObj = (ExerSlotItem)copy();
            previewObj.exerSlot = exerSlot;
            return previewObj;
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

        /// <summary>
        /// 设置预览艾瑟萌天赋
        /// </summary>
        /// <param name="packEquip"></param>
        public void setPackEquipPreview(ExerPackEquip packEquip) {
            if (previewObj == null) createPreviewObject();
            previewObj.exerEquipSlot.setEquip(packEquip);
            previewObj.recomputeParams();
        }

        /// <summary>
        /// 设置预览
        /// </summary>
        /// <param name="contItem"></param>
        public void setPreview<T>(T contItem) where T : PackContItem, new() {
            if (previewObj == null) createPreviewObject();
            previewObj.setEquip(contItem);
        }

        /// <summary>
        /// 设置装备预览
        /// </summary>
        /// <param name="contItem"></param>
        public void setPackEquipPreview(int index, ExerPackEquip packEquip) {
            if (previewObj == null) createPreviewObject();
            previewObj.exerEquipSlot.setEquip(index, packEquip);
            previewObj.recomputeParams();
        }

        /// <summary>
        /// 获取预览用的装备槽项
        /// </summary>
        /// <param name="index"></param>
        public ExerEquipSlotItem getPreviewEquipSlotItem(int index) {
            if (previewObj == null) return null;
            return previewObj.exerEquipSlot.getSlotItem(index);
        }

        #endregion

        #endregion

        /// <summary>
        /// 读取自定义属性
        /// </summary>
        /// <param name="json">数据</param>
        protected override void loadCustomAttributes(JsonData json) {
            base.loadCustomAttributes(json);
            exerEquipSlot.exerSlotItem = this;
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerSlotItem() : base() {
            // exerEquipSlot = new ExerEquipSlot(this);
        }
    }

    /// <summary>
    /// 艾瑟萌装备槽项
    /// </summary>
    public class ExerEquipSlotItem : SlotContItem<ExerPackEquip>,
        ParamDisplay.IDisplayDataArrayConvertable {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int packEquipId { get; protected set; }
        [AutoConvert]
        public int eType { get; protected set; }

        /// <summary>
        /// 槽索引
        /// </summary>
        public override int slotIndex {
            get { return eType; }
            set { eType = value; }
        }

        /// <summary>
        /// 艾瑟萌背包装备实例
        /// </summary>
        public ExerPackEquip packEquip {
            get {
                var exerPack = equipSlot.exerSlotItem.exerSlot.
                    player.packContainers.exerPack;
                return exerPack.getItem<ExerPackEquip>(
                    item => item.id == packEquipId);
            }
            set {
                packEquipId = value.id;
            }
        }

        /// <summary>
        /// 装备
        /// </summary>
        public override ExerPackEquip equip1 {
            get { return packEquip; }
            protected set { packEquip = value; }
        }

        /// <summary>
        /// 装备槽
        /// </summary>
        public ExerEquipSlot equipSlot { get; set; }

        #region 属性显示数据生成

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
                var paramId = params_[i].id;
                var param = getParam(paramId);
                var percent = param.param().isPercent();
                json["equip_param"] = DataLoader.convertDouble(
                    param.value, !percent, percent ? 4 : 2); 
                data[i] = json;
            }
            return data;
        }

        #endregion

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
            return packEquip == null ? null : packEquip.equip();
        }

        /// <summary>
        /// 获取装备的所有属性
        /// </summary>
        /// <returns>属性数据数组</returns>
        public ParamData[] getParams() {
            if (packEquip == null) return new ParamData[0];
            return packEquip.getBaseParams();
        }

        /// <summary>
        /// 获取装备的属性
        /// </summary>
        /// <param name="paramId">属性ID</param>
        /// <returns>属性数据</returns>
        public ParamData getParam(int paramId) {
            var val = CalcService.EquipParamCalc.calc(this, paramId);
            return new ParamData(paramId, val);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerEquipSlotItem() : base() { }
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
        /// 是否为空物品
        /// </summary>
        /// <returns></returns>
        public override bool isNullItem() {
            return skillId == 0 || skill() == null;
        }

        /// <summary>
        /// 获取科目
        /// </summary>
        /// <returns>科目</returns>
        public ExerSkill skill() {
            return DataService.get().exerSkill(skillId);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerSkillSlotItem() : base() { }
    }

    #endregion

    #region 容器

    /// <summary>
    /// 艾瑟萌背包
    /// </summary>
    public class ExerPack : PackContainer<PackContItem> {

        /// <summary>
        /// 获得一个物品
        /// </summary>
        /// <param name="p">条件</param>
        /// <returns>物品</returns>
        public T getItem<T>(Predicate<T> p) where T : PackContItem {
            if (typeof(T) == typeof(ExerPackItem))
                return (T)findItem(item => item.type ==
                    (int)BaseContItem.Type.ExerPackItem && p((T)item));
            if (typeof(T) == typeof(ExerPackEquip))
                return (T)findItem(item => item.type ==
                    (int)BaseContItem.Type.ExerPackEquip && p((T)item));
            return null;
        }

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
        /*
        /// <summary>
        /// 获取所有装备
        /// </summary>
        /// <returns>装备</returns>
        public List<ExerPackEquip> exerEquips() {
            var res = new List<ExerPackEquip>();
            foreach (var item in items)
                if (item.type == (int)BaseContItem.Type.ExerPackEquip)
                    res.Add((ExerPackEquip)item);
            return res;
        }

        /// <summary>
        /// 获取所有物品
        /// </summary>
        /// <returns>物品</returns>
        public List<ExerPackItem> exerItems() {
            var res = new List<ExerPackItem>();
            foreach (var item in items)
                if (item.type == (int)BaseContItem.Type.ExerPackItem)
                    res.Add((ExerPackItem)item);
            return res;
        }
        */
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExerPack() : base() { }
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

        /// <summary>
        /// 所属玩家
        /// </summary>
        public Player player { get; set; } 

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
        public override ExerSlotItem getSlotItem(int sid) {
            return findItem((item) => item.subjectId == sid);
        }

        /// <summary>
        /// 通过装备物品获取槽ID
        /// </summary>
        /// <typeparam name="E">装备物品类型</typeparam>
        /// <param name="equipItem">装备物品</param>
        /// <returns>槽ID</returns>
        public override ExerSlotItem getSlotItemByEquipItem<E>(E equipItem) {
            if (typeof(E) == typeof(PlayerExermon)) {
				var playerExer = equipItem as PlayerExermon;
				var sid = playerExer.exermon().subjectId;
                return getSlotItem(sid);
            }
            return null;
        }

        /// <summary>
        /// 读取物品
        /// </summary>
        /// <param name="json">数据</param>
        /// <returns>物品对象</returns>
        protected override ExerSlotItem loadItem(JsonData json) {
            var slotItem = base.loadItem(json);
            slotItem.exerSlot = this;
            return slotItem;
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
        public ExerSlotItem exerSlotItem { get; set; } = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /*
        /// <param name="exerSlotItem">所属的艾瑟萌槽项</param>
        public ExerEquipSlot(ExerSlotItem exerSlotItem) {
            this.exerSlotItem = exerSlotItem;
        }
        */
        public ExerEquipSlot() : base() { }

        #region 数据操作
        /*
        /// <summary>
        /// 获取装备项
        /// </summary>
        /// <param name="eType">装备类型</param>
        /// <returns>装备项数据</returns>
        public override ExerEquipSlotItem getSlotItem(int eType) {
            return getItem((item) => item.eType == eType);
        }
        */
        /// <summary>
        /// 通过装备物品获取槽项
        /// </summary>
        /// <typeparam name="E">装备物品类型</typeparam>
        /// <param name="equipItem">装备物品</param>
        /// <returns>槽项</returns>
        public override ExerEquipSlotItem getSlotItemByEquipItem<E>(E equipItem) {
            var et = typeof(E); var pet = typeof(ExerPackEquip);
            if (et == pet || pet.IsSubclassOf(et)) {
                var eType = ((ExerPackEquip)(object)equipItem).equip().eType;
                return getSlotItem(eType);
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

        /// <summary>
        /// 读取单个物品
        /// </summary>
        /// <param name="json">数据</param>
        protected override ExerEquipSlotItem loadItem(JsonData json) {
            var slotItem = base.loadItem(json);
            slotItem.equipSlot = this;
            return slotItem;
        }

    }

    /// <summary>
    /// 艾瑟萌装备槽
    /// </summary>
    public class ExerSkillSlot : SlotContainer<ExerSkillSlotItem> {

        /// <summary>
        /// 通过装备物品获取槽项
        /// </summary>
        /// <typeparam name="E">装备物品类型</typeparam>
        /// <param name="equipItem">装备物品</param>
        /// <returns>槽项</returns>
        public override ExerSkillSlotItem getSlotItemByEquipItem<E>(E equipItem) {
            return null;
        }
    }

    #endregion

}