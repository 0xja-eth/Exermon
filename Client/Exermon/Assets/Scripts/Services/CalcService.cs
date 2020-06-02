using System;

using UnityEngine;
using Random = UnityEngine.Random;

using Core.Data;
using Core.Data.Loaders;
using Core.Services;

using GameModule.Data;
using PlayerModule.Data;
using ItemModule.Data;
using ExermonModule.Data;
using SeasonModule.Data;
using BattleModule.Data;

using ExerPro.EnglishModule.Data;

using SeasonModule.Services;
using System.Collections.Generic;

/// <summary>
/// 游戏模块服务
/// </summary>
namespace GameModule.Services {

    /// <summary>
    /// 计算服务
    /// </summary>
    public class CalcService : BaseService<CalcService> {

        /// <summary>
        /// 公用计算函数
        /// </summary>
        public class Common {

            /// <summary>
            /// sigmoid函数
            /// </summary>
            /// <param name="x"></param>
            /// <returns></returns>
            public static double sigmoid(double x) {
                return 1 / (1 + Math.Exp(-x));
            }
        }

        /// <summary>
        /// 艾瑟萌等级计算类
        /// </summary>
        public class ExermonLevelCalc {

            /// <summary>
            /// 星级等级表
            /// </summary>
            public static Dictionary<int, List<double>> StarLevelTable = null;

            /// <summary>
            /// 初始化计算所有星级的等级表
            /// </summary>
            public static void init() {
                StarLevelTable = new Dictionary<int, List<double>>();
                var stars = DataService.get().staticData.configure.exerStars;

                foreach (var star in stars)
                    StarLevelTable[star.id] = generateTalbe(star);
            }

            /// <summary>
            /// 生成单个星级表格
            /// </summary>
            static List<double> generateTalbe(ExerStar star) {
                var res = new List<double>(star.maxLevel);
                var a = star.levelExpFactors[0];
                var b = star.levelExpFactors[1];
                var c = star.levelExpFactors[2];
                for (int l = 0; l < star.maxLevel; ++l)
                    res.Add(_calcTable(l, a, b, c));
                return res;
            }

            /// <summary>
            /// 计算表格项
            /// </summary>
            /// <param name="x">等级</param>
            /// <param name="a">参数A</param>
            /// <param name="b">参数B</param>
            /// <param name="c">参数C</param>
            /// <returns></returns>
            static double _calcTable(int x, double a, double b, double c) {
                return a / 3 * x * x * x + (a + b) / 2 * x * x + (a + b * 3 + c * 6) / 6 * x;
            }

            /// <summary>
            /// 获取当前等级所需经验值
            /// </summary>
            /// <param name="star">星级</param>
            /// <param name="level">等级</param>
            /// <returns></returns>
            public static double getDetlaExp(ExerStar star, int level) {
                if (level >= star.maxLevel) return -1;
                if (StarLevelTable == null) init();

                var data = StarLevelTable[star.id];
                return data[level] - data[level - 1];
            }

            /// <summary>
            /// 获取下一等级所需总经验值
            /// </summary>
            /// <param name="star">星级</param>
            /// <param name="level">等级</param>
            /// <returns></returns>
            public static double getDeltaSumExp(ExerStar star, int level) {
                if (level >= star.maxLevel) return -1;
                if (StarLevelTable == null) init();

                var data = StarLevelTable[star.id];
                return data[level];
            }

            /// <summary>
            /// 获取累计经验值
            /// </summary>
            /// <param name="star">星级</param>
            /// <param name="level">等级</param>
            /// <returns></returns>
            public static double getSumExp(ExerStar star, int level, int exp) {
                if (level >= star.maxLevel) return -1;
                if (StarLevelTable == null) init();

                var data = StarLevelTable[star.id];
                return data[level - 1] + exp;
            }
        }

        /// <summary>
        /// 艾瑟萌属性计算类
        /// </summary>
        public class ExermonParamCalc {

            /// <summary>
            /// 参数定义
            /// </summary>
            const double S = 1.005;
            const double R = 233;

            /// <summary>
            /// 执行计算
            /// </summary>
            /// <param name="base_">基础值</param>
            /// <param name="rate">成长率</param>
            /// <param name="level">等级</param>
            /// <param name="plusVal">附加值</param>
            /// <param name="plusRate">附加率</param>
            /// <returns>属性实际值</returns>
            public static double calc(double base_, double rate, 
                int level, double plusVal = 0, double plusRate = 1) {
                var value = base_ * Math.Pow((rate / R + 1) * S, level - 1);
                return value * plusRate + plusVal;
            }
        }

        /// <summary>
        /// 艾瑟萌槽项属性成长率计算类
        /// </summary>
        public class ExerSlotItemParamRateCalc {

            /// <summary>
            /// 执行计算
            /// </summary>
            /// <param name="exerSlotItem">艾瑟萌槽项</param>
            /// <param name="paramId">属性ID</param>
            /// <returns>属性成长率</returns>
            public static double calc(ExerSlotItem exerSlotItem, int paramId) {
                if (exerSlotItem == null) return 0;

                var playerExer = exerSlotItem.playerExer;
                if (playerExer == null) return 0;
                var epr = playerExer.paramRate(paramId).value;

                var exerGift = exerSlotItem.exerGift();
                if (exerGift == null) return epr;
                var gprr = exerGift.paramRate(paramId).value;

                return epr * gprr;
            }
        }

        /// <summary>
        /// 艾瑟萌槽项属性计算类
        /// </summary>
        public class ExerSlotItemParamCalc {

            /// <summary>
            /// 参数定义
            /// </summary>
            const double S = 1.005;
            const double R = 233;

            /// <summary>
            /// 暂存变量
            /// </summary>
            ExerSlotItem exerSlotItem;
            int paramId;
            double value;

            PlayerExermon playerExer;
            PlayerExerGift playerGift;
            ExerEquipSlot exerEquipSlot;
            Exermon exermon;

            /// <summary>
            /// 执行计算
            /// </summary>
            /// <param name="exerSlotItem">艾瑟萌槽项</param>
            /// <param name="paramId">属性ID</param>
            /// <returns>属性实际值</returns>
            public static double calc(ExerSlotItem exerSlotItem, int paramId) {
                var calc = new ExerSlotItemParamCalc(exerSlotItem, paramId);
                return calc.value;
            }

            /// <summary>
            /// 初始化计算实例
            /// </summary>
            /// <param name="exerSlotItem">艾瑟萌槽项</param>
            /// <param name="paramId">属性ID</param>
            ExerSlotItemParamCalc(ExerSlotItem exerSlotItem, int paramId) {
                value = 0;

                this.exerSlotItem = exerSlotItem;
                if (exerSlotItem == null) return;

                playerExer = exerSlotItem.playerExer;
                if (playerExer == null) return;

                exermon = playerExer.exermon();
                if (exermon == null) return;

                playerGift = exerSlotItem.playerGift;
                exerEquipSlot = exerSlotItem.exerEquipSlot;

                this.paramId = paramId;

                value = _calc();
            }

            /// <summary>
            /// 计算
            /// </summary>
            /// <returns>计算结果</returns>
            double _calc() {

                var bpv = _calcBaseParamValue();
                var ppv = _calcPlusParamValue();
                var rr = _calcRealRate();
                var apv = _calcAppendParamValue();

                var val = (bpv + ppv) * rr + apv;

                return _adjustParamValue(val);
            }

            /// <summary>
            /// 计算 BPV = EPB*((实际属性成长率/R+1)*S)^(L-1)
            /// </summary>
            /// <returns>BPV</returns>
            double _calcBaseParamValue() {
                var epb = playerExer.paramBase(paramId).value;
                var rpr = _calcRealParamRate();

                var plusVal = playerExer.plusParamValue(paramId).value;
                var plusRate = playerExer.plusParamRate(paramId).value;

                return ExermonParamCalc.calc(epb, rpr, 
                    playerExer.level, plusVal, plusRate);
            }
            /// <summary>
            /// 计算 RPR
            /// </summary>
            /// <returns>RPR</returns>
            double _calcRealParamRate() {
                return ExerSlotItemParamRateCalc.calc(exerSlotItem, paramId);
            }
            /// <summary>
            /// 计算 PPV = EPPV+SPPV
            /// </summary>
            /// <returns>PPV</returns>
            double _calcPlusParamValue() {
                return _calcEquipPlusParamValue() + _calcStatusPlusParamValue();
            }
            /// <summary>
            /// 计算 EPPV 装备附加值
            /// </summary>
            /// <returns>EPPV</returns>
            double _calcEquipPlusParamValue() {
                if (exerEquipSlot == null) return 0;
                return exerEquipSlot.getParam(paramId).value;
            }
            /// <summary>
            /// 计算 SPPV 状态附加值
            /// </summary>
            /// <returns>SPPV</returns>
            double _calcStatusPlusParamValue() { return 0; }
            /// <summary>
            /// 计算 RR = 基础加成率*附加加成率
            /// </summary>
            /// <returns>RR</returns>
            double _calcRealRate() {
                return _calcBaseRate() * _calcPlusRate();
            }
            /// <summary>
            /// 计算 RR = 基础加成率*附加加成率
            /// </summary>
            /// <returns>BR</returns>
            double _calcBaseRate() { return 1; }
            /// <summary>
            /// 计算 PR = GPR*SPR（对战时还包括题目糖属性加成率）
            /// </summary>
            /// <returns>PR</returns>
            double _calcPlusRate() { return 1; }
            /// <summary>
            /// 计算 APV
            /// </summary>
            /// <returns>APV</returns>
            double _calcAppendParamValue() { return 0; }

            /// <summary>
            /// 调整值
            /// </summary>
            /// <param name="val">原始值</param>
            /// <returns>调整值</returns>
            double _adjustParamValue(double val) {
                var param = _getParam();
                if (param == null) return val;
                return param.clamp(val);
            }

            // 获取属性实例
            BaseParam _getParam() {
                return DataService.get().baseParam(paramId);
            }
        }

        /// <summary>
        /// 装备能力值计算
        /// </summary>
        public class EquipParamCalc {

            /// <summary>
            /// 属性
            /// </summary>
            BaseParam param;

            ExerEquipSlotItem slotItem;
            ExerPackEquip packEquip;
            ExerEquip equip;

            PlayerExermon playerExer;

            double value = 0;

            /// <summary>
            /// 计算
            /// </summary>
            /// <param name="slotItem">容器项</param>
            /// <param name="paramId">属性ID</param>
            /// <returns>返回属性值</returns>
            public static double calc(ExerEquipSlotItem slotItem, int paramId) {
                var calc = new EquipParamCalc(slotItem, paramId);
                return calc.value;
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="slotItem">容器项</param>
            /// <param name="paramId">属性ID</param>
            EquipParamCalc(ExerEquipSlotItem slotItem, int paramId) {
                param = DataService.get().baseParam(paramId);
                if (param == null) return;

                if (slotItem.isNullItem()) return;
                packEquip = slotItem.packEquip;
                equip = slotItem.equip();

                var exerSlotItem = slotItem.equipSlot.exerSlotItem;
                playerExer = exerSlotItem.playerExer;
                _calc();
            }

            /// <summary>
            /// 计算
            /// </summary>
            void _calc() {
                value = packEquip.getBaseParam(param.id).value;
                // var rate = packEquip.getLevelParam(param.getID()).value;
                // value += rate * playerExer.level;
            }
        }

        /// <summary>
        /// 战斗力计算类
        /// </summary>
        public class BattlePointCalc {

            /// <summary>
            /// 参数定义
            /// </summary>
            const double C = 2;

            public delegate ParamData paramFunc(int index);

            /// <summary>
            /// 执行计算
            /// </summary>
            /// <returns>战斗力</returns>
            public static int calc(paramFunc func) {
                var params_ = DataService.get().staticData.configure.baseParams;
                return calc(func(1).value, func(2).value, func(3).value,
                    func(4).value, func(5).value, func(6).value);
            }

            /// <summary>
            /// 执行计算
            /// </summary>
            /// <returns>战斗力</returns>
            public static int calc(double mhp, double mmp, double atk,
                double def, double eva, double cri) {
                return (int)Math.Round((mhp + mmp * 2 + atk * 6 * C *
                    (1 + cri / 100) + def * 4) * (1 + eva / 50));
            }
        }

        /// <summary>
        /// 最大星级计算类
        /// </summary>
        public class MaxStarCalc {

            /// <summary>
            /// 计算
            /// </summary>
            /// <param name="player">玩家</param>
            /// <param name="sid">科目ID</param>
            /// <returns></returns>
            static public QuesStar calc(Player player, int sid) {
                var stars = DataService.get().staticData.configure.quesStars;
                var exerSlot = player.slotContainers.exerSlot;
                var exerSlotItem = exerSlot.getSlotItem(sid);

                if (exerSlotItem == null) return null;

                var lastStar = stars[0];
                foreach(var star in stars) {
                    if (exerSlotItem.level < star.level)
                        return lastStar;
                    lastStar = star;
                }

                return lastStar;
            }
        }

        /// <summary>
        /// 段位星星与段位计算类
        /// </summary>
        public class RankCalc {

            /// <summary>
            /// 对应的罗马数字
            /// </summary>
            public static readonly string[] RomanNums = new string[] {
                "Ⅰ", "Ⅱ", "Ⅲ", "Ⅳ", "Ⅴ", "Ⅵ", "Ⅶ", "Ⅷ", "Ⅸ", "Ⅹ"
            };

            /// <summary>
            /// 执行计算
            /// </summary>
            /// <param name="starNum">段位星星数目</param>
            /// <param name="rank">所在段位对象</param>
            /// <param name="subRank">所在子段位数目</param>
            /// <returns>剩余星星数目</returns>
            /// <example>
            /// 0 = > 学渣I(1, 0, 0)
            /// 1 = > 学渣I(1, 0, 1)
            /// 2 = > 学渣I(1, 0, 2)
            /// 3 = > 学渣I(1, 0, 3)
            /// 4 = > 学渣II(1, 1, 1)
            /// 5 = > 学渣II(1, 1, 2)
            /// 6 = > 学渣II(1, 1, 3)
            /// 7 = > 学渣III(1, 2, 1)
            /// 10 = > 学酥I(2, 1, 1)
            /// </example>
            public static int calc(int starNum, out CompRank rank, out int subRank) {
                var ranks = SeasonService.get().compRanks();
                var subNum = CompRank.StarsPerSubRank;
                rank = ranks[0]; subRank = 0;

                // 需要事先保证 ranks 有序
                foreach (var rank_ in ranks) {
                    rank = rank_; subRank = 0;
                    var starCnt = rank_.starCount();

                    // 最后一个段位，直接返回（out 变量已经赋值）
                    if (starCnt == 0) return starNum;

                    if (starNum > starCnt) 
                        // 还没找到段位，继续找下一个
                        starNum -= starCnt;
                    else { // 找到所属的段位了
                        var tmpStar = starNum - 1;
                        // 如果此时星星数 -1 小于零，直接返回不需计算
                        if (tmpStar < 0) return 0;

                        // 否则正常计算
                        subRank = tmpStar / subNum;
                        starNum = (tmpStar % subNum) + 1;
                        break;
                    }
                }
                return starNum;
            }

            /// <summary>
            /// 计算段位的文本
            /// </summary>
            /// <param name="starNum"></param>
            /// <returns></returns>
            public static string getText(int starNum) {
                CompRank rank; int subRank;
                calc(starNum, out rank, out subRank);
                return getText(rank, subRank);
            }
            public static string getText(CompRank rank, int subRank) {
                if (rank.subRankNum <= 0) return rank.name;
                return rank.name + RomanNums[subRank];
            }
        }

        /// <summary>
        /// 通用物品处理器
        /// </summary>
        public class GeneralItemEffectProcessor {

            /// <summary>
            /// 处理
            /// </summary>
            /// <param name="effects">效果</param>
            /// <param name="count">次数</param>>
            /// <param name="target">目标</param>
            public static void process(IEffectsConvertable item, int count = 1, PlayerExermon target = null) {
                process(item.convertToEffectData(), count, target);
                target?.refresh();
            }
            public static void process(EffectData[] effects, int count = 1, PlayerExermon target = null) {
                foreach (var effect in effects)
                    processEffect(effect, count, target);
            }

            /// <summary>
            /// 处理单个效果
            /// </summary>
            /// <param name="target">目标</param>
            /// <param name="effect">效果</param>
            static void processEffect(EffectData effect, int count = 1, PlayerExermon target = null) {
                int p; double a, b;
                int min, max, val;
                var params_ = effect.params_;
                if (params_ == null && params_.Count <= 0) return;

                switch ((EffectData.Code)effect.code) {
                    case EffectData.Code.AddParam:
                        p = DataLoader.load<int>(params_[0]);
                        a = DataLoader.load<double>(params_[1]);
                        a *= count;

                        target.addPlusParamValues(p, a);

                        if (params_.Count > 2) { // 处理百分比
                            b = DataLoader.load<double>(params_[2]) / 100 + 1;
                            b = Math.Pow(b, count);
                            target.addPlusParamRates(p, b);
                        }
                        break;
                    case EffectData.Code.TempAddParam:
                        p = DataLoader.load<int>(params_[0]);
                        a = DataLoader.load<int>(params_[2]);
                        a *= count;

                        target.addPlusParamValues(p, a);

                        if (params_.Count > 3) { // 处理百分比
                            b = DataLoader.load<double>(params_[3]) / 100 + 1;
                            b = Math.Pow(b, count);
                            target.addPlusParamRates(p, b);
                        }
                        break;
                    case EffectData.Code.GainExermonExp:
                        min = max = DataLoader.load<int>(params_[0]);
                        if (params_.Count > 1)
                            max = DataLoader.load<int>(params_[1]);
                        val = UnityEngine.Random.Range(min, max + 1);
                        target.gainExp(val);
                        break;
                }
            }
        }

        /// <summary>
        /// 物品效果处理类
        /// </summary>
        public class BattleItemEffectProcessor {

            /// <summary>
            /// 处理
            /// </summary>
            /// <param name="target">目标</param>
            /// <param name="effects">效果</param>
            public static void process(IBattleItemUseTarget target, IEffectsConvertable item) {
                process(target, item.convertToEffectData());
            }
            public static void process(IBattleItemUseTarget target, EffectData[] effects) {
                foreach (var effect in effects)
                    processEffect(target, effect);
            }

            /// <summary>
            /// 处理单个效果
            /// </summary>
            /// <param name="target">目标</param>
            /// <param name="effect">效果</param>
            static void processEffect(IBattleItemUseTarget target, EffectData effect) {
                int p; double a, b;
                var params_ = effect.params_;
                if (params_ == null && params_.Count <= 0) return;

                switch ((EffectData.Code)effect.code) {
                    case EffectData.Code.RecoverHP:
                        a = DataLoader.load<int>(params_[0]);
                        target.changeHP((int)a);

                        if (params_.Count > 1) { // 处理百分比
                            b = DataLoader.load<double>(params_[1]) / 100;
                            target.changePercentMP(b);
                        }
                        break;
                    case EffectData.Code.RecoverMP:
                        a = DataLoader.load<int>(params_[0]);
                        target.changeMP((int)a);

                        if (params_.Count > 1) { // 处理百分比
                            b = DataLoader.load<double>(params_[1]) / 100;
                            target.changePercentMP(b);
                        }
                        break;
                    case EffectData.Code.AddParam:
                        p = DataLoader.load<int>(params_[0]);
                        a = DataLoader.load<double>(params_[1]);
                        target.changeParam(p, a);

                        if (params_.Count > 2) { // 处理百分比
                            b = DataLoader.load<double>(params_[2]) / 100 + 1;
                            target.changePercentParam(p, b);
                        }
                        break;
                    case EffectData.Code.TempAddParam:
                    case EffectData.Code.BattleAddParam:
                        p = DataLoader.load<int>(params_[0]);
                        a = DataLoader.load<double>(params_[2]);
                        target.changeParam(p, a);

                        if (params_.Count > 3) { // 处理百分比
                            b = DataLoader.load<double>(params_[3]) / 100 + 1;
                            target.changePercentParam(p, b);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 对战伤害处理器
        /// </summary>
        public class AttackActionProceessor {

            /// <summary>
            /// 处理
            /// </summary>
            /// <param name="self">发动者</param>
            /// <param name="target">目标</param>
            /// <param name="action">行动</param>
            public static void process(RuntimeAction action) {
                var self = action.player;
                var target = generateTargets(action);

                var hurt = action.hurt;
                var skill = action.skill();
                var rate = skill == null ? 1 : (skill.drainRate / 100.0);

                switch ((ExerSkill.HitType)action.hitType()) {
                    case ExerSkill.HitType.HPDamage:
                        target?.changeHP(-hurt); break;
                    case ExerSkill.HitType.MPDamage:
                        target?.changeMP(-hurt); break;
                    case ExerSkill.HitType.HPRecover:
                        target?.changeHP(hurt); break;
                    case ExerSkill.HitType.MPRecover:
                        target?.changeMP(hurt); break;
                    case ExerSkill.HitType.HPDrain:
                        target?.changeHP(-hurt);
                        self.changeHP((int)(hurt * rate));
                        break;
                    case ExerSkill.HitType.MPDrain:
                        target?.changeHP(-hurt);
                        self.changeHP((int)(hurt * rate));
                        break;
                }
            }

            /// <summary>
            /// 生成目标（仅单个目标0）
            /// </summary>
            /// <param name="action">攻击行动</param>
            /// <returns>返回目标对战玩家数组</returns>
            public static RuntimeBattlePlayer generateTargets(
                RuntimeAction action) {
                var self = action.player;
                var oppo = self.getOppo();
                switch ((ExerSkill.TargetType)action.targetType) {
                    case ExerSkill.TargetType.Self: return self;
                    case ExerSkill.TargetType.Enemy: return oppo;
                }
                return null;
            }

        }

        /// <summary>
        /// 节点生成器
        /// </summary>
        public class NodeGenerator {

            /// <summary>
            /// 关卡
            /// </summary>
            MapStageRecord stageRecord;
            ExerProMapStage stage;

            List<ExerProMapNode>[] nodes;

            /// <summary>
            /// 生成关卡地图
            /// </summary>
            /// <param name="stage">关卡</param>
            public static bool generate(MapStageRecord stage) {
                var generator = new NodeGenerator(stage);
                return generator.generateNodes() && 
                    generator.generateForkNodes() && 
                    generator.generateLinks();
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            NodeGenerator(MapStageRecord stageRecord) {
                this.stageRecord = stageRecord;
                stage = stageRecord.stage();
            }

            /// <summary>
            /// 生成结点数组
            /// </summary>
            /// <param name="stage"></param>
            bool generateNodes() {
                if (stage == null) return false;

                var steps = stage.steps;
                var xCnt = steps.Length;
                nodes = new List<ExerProMapNode>[xCnt];
                for (int x = 0; x < xCnt; ++x) {
                    nodes[x] = new List<ExerProMapNode>();
                    for (int y = 0; y < steps[x]; ++y)
                        nodes[x].Add(stageRecord.createNode(
                            x, y, generateNodeType(x, xCnt)));
                }

                return true;
            }

            /// <summary>
            /// 生成据点类型
            /// </summary>
            /// <returns></returns>
            ExerProMapNode.Type generateNodeType(int x, int xCnt) {

                // 最后一个为 BOSS
                if (x == xCnt - 1) return ExerProMapNode.Type.Boss;

                var rates = stage.nodeRate;
                var rateList = new List<int>();

                // 将比率填充为列表，然后从中抽取一个
                for (int i = 1; i <= rates.Length; ++i)
                    for (int j = 0; j < rates[i - 1]; ++j) rateList.Add(i);

                var index = Random.Range(0, rateList.Count);
                return (ExerProMapNode.Type)rateList[index];
            }

            /// <summary>
            /// 生成分叉据点
            /// </summary>
            bool generateForkNodes() {
                if (stage == null) return false;

                int cnt = 0, counter = 0;
                int maxCnt = stage.maxForkNode;
                int maxX = nodes.Length;

                while(cnt < maxCnt && counter <= 10000) {
                    if (maxX <= 2) break; counter++;

                    // 随机生成一个点
                    // 最后两步是无法进行分叉的
                    int x = Random.Range(0, maxX - 2);
                    var colNodes = nodes[x];
                    int maxY = colNodes.Count;
                    int y = Random.Range(0, maxY);

                    // 若已经标记为 fork，跳过
                    var node = colNodes[y];
                    if (node.fork) continue;

                    // 若该步的分支据点数目 > 下一步总数 - 1，则跳过
                    int nextCnt = nodes[x + 1].Count;
                    var forkCnt = _calcForkCount(colNodes);
                    if (forkCnt >= nextCnt - 1) continue;

                    node.fork = true; cnt++;
                }
                return true;
            }

            /// <summary>
            /// 计算分支结点数量
            /// </summary>
            /// <param name="nodes"></param>
            /// <returns></returns>
            int _calcForkCount(List<ExerProMapNode> nodes) {
                int cnt = 0;
                foreach (var node in nodes) if (node.fork) cnt++;
                return cnt; 
            }

            /// <summary>
            /// 生成线条连接
            /// </summary>
            bool generateLinks() {
                if (stage == null) return false;

                // 对每一步进行遍历
                for (int x = 0; x < nodes.Length - 1; ++x) {

                    int ny = 0; // 下一步的Y坐标
                    ExerProMapNode node = null;

                    // 对每个据点进行遍历
                    for (int y = 0; y < nodes[x].Count; ++y) {
                        node = nodes[x][y];

                        // 如果不是分叉，添加第一个
                        if (!node.fork) _addNext(node, ref ny, true);

                        // 因为 random 为 true，所以这里直接进行 stage.maxFork 次循环
                        else for (int n = 0; n < stage.maxFork; ++n)
                            _addNext(node, ref ny, true);
                    }

                    // 剩余未分配的下一个据点则全部作为本步最后一个据点的下一步
                    for(; ny < nodes[x + 1].Count; ++ny)
                        _addNext(node, ref ny, incr: false);
                }
                return true;
            }

            /// <summary>
            /// 添加下一步
            /// </summary>
            /// <param name="node">当前结点</param>
            /// <param name="x">当前X坐标</param>
            /// <param name="ny">下一步Y坐标</param>
            /// <param name="random">是否随机增加下一步Y坐标</param>
            /// <param name="incr">是否增加下一步Y坐标</param>
            /// <returns>返回新的 ny 值</returns>
            void _addNext(ExerProMapNode node, ref int ny, 
                bool random = false, bool incr = true) {

                var x = node.xOrder;
                node?.addNext(nodes[x + 1][ny]);
                if (random) incr = Random.Range(0, 2) > 0;
                if (incr && ny < nodes[x + 1].Count - 1) ny++;
            }
        }
    }


}
