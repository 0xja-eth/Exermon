﻿using System;
using System.Collections.Generic;

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

using LitJson;

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
            public static void process(BattleModule.Data.RuntimeAction action) {
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
                BattleModule.Data.RuntimeAction action) {
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
            ExerProRecord stageRecord;
            ExerProMapStage stage;

            List<ExerProMapNode>[] nodes;

            /// <summary>
            /// 生成关卡地图
            /// </summary>
            /// <param name="stage">关卡</param>
            public static bool generate(ExerProRecord stage) {
                var generator = new NodeGenerator(stage);
                return generator.generateNodes() && 
                    generator.generateForkNodes() && 
                    generator.generateLinks();
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            NodeGenerator(ExerProRecord stageRecord) {
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

        /// <summary>
        /// 战斗敌人生成器
        /// </summary>
        public class BattleEnemiesGenerator {

            /// <summary>
            /// 常量定义
            /// </summary>
            public const int MaxEnemyCols = 3; // 最大敌人列数
			public const int MaxEnemyRows = 3; // 最大敌人行数

            /// <summary>
            /// 内部变量定义
            /// </summary>
            ExerProMapStage stage;
            RuntimeActor actor;

            List<RuntimeEnemy> enemies = new List<RuntimeEnemy>();

            /// <summary>
            /// 生成
            /// </summary>
            /// <param name="actor">角色对象</param>
            /// <param name="stage">关卡对象</param>
            /// <param name="type">类型</param>
            public static List<RuntimeEnemy> generate(RuntimeActor actor, 
                ExerProMapStage stage, ExerProMapNode.Type type) {
                var generator = new BattleEnemiesGenerator(actor, stage, type);

                return generator.enemies;
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            BattleEnemiesGenerator(RuntimeActor actor,
                ExerProMapStage stage, ExerProMapNode.Type type) {
                this.actor = actor; this.stage = stage;
                switch (type) {
                    case ExerProMapNode.Type.Enemy:
                        generateEnemies(); break;
                    case ExerProMapNode.Type.Elite:
                        generateElites(); break;
                    case ExerProMapNode.Type.Boss:
                        generateBoss(); break;
                }
            }

            /// <summary>
            /// 生成常规敌人
            /// </summary>
            void generateEnemies() {
                _generateRandomEnemies(stage.normalEnemies());
            }

            /// <summary>
            /// 生成精英敌人
            /// </summary>
            void generateElites() {
                _generateRandomEnemies(stage.eliteEnemies());
				if (enemies.Count <= 0)
					_generateRandomEnemies(stage.normalEnemies());
			}

			/// <summary>
			/// 随机生成敌人
			/// </summary>
			void _generateRandomEnemies(List<ExerProEnemy> enemies) {
                var posCnt = MaxEnemyCols * MaxEnemyRows;
                var enemyCnt = Math.Min(posCnt, enemies.Count);
                var posVis = new bool[posCnt];
                var cnt = Random.Range(0, stage.maxBattleEnemies) + 1;

				Debug.Log("enemies.Count: " + enemies.Count + ", " + enemyCnt);

                for (int i = 0; i < cnt; ++i) {
                    var enemy = enemies[Random.Range(0, enemyCnt)];
                    var pos = Random.Range(0, posCnt);
                    while (posVis[pos]) pos = Random.Range(0, posCnt);
                    posVis[pos] = true;

                    this.enemies.Add(new RuntimeEnemy(pos, enemy));
                }
            }

			/// <summary>
			/// 生成BOSS
			/// </summary>
			void generateBoss() {
				var enemies = stage.bosses();
				if (enemies.Count <= 0) {
					generateElites(); return;
				}

				var posCnt = MaxEnemyCols * MaxEnemyRows;
				var posVis = new bool[posCnt];

				var cnt = Random.Range(0, stage.maxBattleEnemies) + 1;
				/*
                foreach(var enemy in enemies) {
                    var pos = Random.Range(0, posCnt);
                    while (posVis[pos]) pos = Random.Range(0, posCnt);
                    posVis[pos] = true;

                    this.enemies.Add(new RuntimeEnemy(pos, enemy));
                }
				*/

				// 非攻击牌
				var cardRecord = actor.cardGroup.cardRecord();
				int unAttackCount = 0;
				int cardCount = cardRecord.Count;
				foreach (var card in cardRecord) 
					if (!card.cardType.Equals(ExerProCard.Type.Attack)) 
						unAttackCount++;

				int enemyId = ExerProEnemy.BOSS1;
				if (cardCount > 0 && unAttackCount / cardCount >= 0.7f)
					enemyId = ExerProEnemy.BOSS2; //乌龟大师
				else if (cardCount >= 100) // 出牌数量大于100
					enemyId = ExerProEnemy.BOSS3; // 龙王

				foreach (var enemy in enemies) 
					if (enemy.id == enemyId) {
						this.enemies.Add(new RuntimeEnemy(4, enemy));
						return;
					}
			}
        }

        /// <summary>
        /// 特训行动结果生成器
        /// </summary>
        public class ExerProActionResultGenerator {

            /// <summary>
            /// 类型代号设置
            /// </summary>
            public const int HPDamageType = 1;
            public const int HPDrainType = 2;
            public const int HPRecoverType = 3;

            /// <summary>
            /// 行动
            /// </summary>
            ExerPro.EnglishModule.Data.RuntimeAction action;
            RuntimeActionResult[] results;

			RuntimeBattler subject;
			RuntimeBattler[] objects;

            List<RuntimeActionResult.StateChange> stateChanges = 
				new List<RuntimeActionResult.StateChange>();
            List<RuntimeBuff> addBuffs = new List<RuntimeBuff>();

			RuntimeBattler object_;
			RuntimeActionResult result;

			/// <summary>
			/// 生成
			/// </summary>
			/// <param name="action">行动</param>
			/// <returns>返回结果</returns>
			public static RuntimeActionResult[] generate(
                ExerPro.EnglishModule.Data.RuntimeAction action) {
                var generator = new ExerProActionResultGenerator(action);

                var enemy = action.subject as RuntimeEnemy;
                if (enemy != null) {
                    Debug.Log("Enemy Next Result Name:" + enemy.enemy().id);
                    if(generator.results.Length > 0)
                        Debug.Log("Enemy Next Result Action:" + generator.results[0].hpDamage);
                }
                return generator.results;
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="action">行动</param>
            ExerProActionResultGenerator(ExerPro.EnglishModule.Data.RuntimeAction action) {
                this.action = action;
				subject = action.subject;
				objects = action.objects;

				var cnt = objects.Length;
				results = new RuntimeActionResult[cnt];

				//Debug.Log("ExerProActionResultGenerator: " + action.toJson().ToJson());

				for(int i=0;i< cnt; ++i) {
					_generate(objects[i], out results[i]);
					_setup(results[i]);
				}
			}

            /// <summary>
            /// 生成
            /// </summary>
            void _generate(RuntimeBattler object_, out RuntimeActionResult result) {
				result = new RuntimeActionResult(object_, action);
				this.object_ = object_; this.result = result;
                foreach (var effect in action.effects) processEffect(effect);
            }

            /// <summary>
            /// 配置结果
            /// </summary>
            void _setup(RuntimeActionResult result) {
                result.stateChanges = stateChanges.ToArray();
                result.addBuffs = addBuffs.ToArray();
				stateChanges.Clear();
				addBuffs.Clear();
			}

            /// <summary>
            /// 处理效果
            /// </summary>
            /// <param name="effect">效果</param>
            void processEffect(ExerProEffectData effect) {

				Debug.Log("processEffect: " + effect.toJson().ToJson());

				int a, b, h, p, n, s, r;
                bool select = false;

                switch (effect.codeEnum()) {
                    case ExerProEffectData.Code.Attack:
                    case ExerProEffectData.Code.AttackBlack:
						a = effect.get(0, 0); h = effect.get(2, HPDamageType);
                        processDamage(a, h); break;

                    case ExerProEffectData.Code.AttackSlash:
						a = effect.get(0, 0); b = effect.get(1, 2);
                        _processSlashAttack(a, b); break;

                    case ExerProEffectData.Code.Recover:
						a = effect.get(0, 0); b = effect.get(1, 0);
						var val = Math.Round(a + object_.hp * 1.0 * b / 100);
						processDamage((int)val, HPRecoverType); break;

                    case ExerProEffectData.Code.AddParam:
						p = effect.get(0, 0);
						a = effect.get(1, 0);
						b = 100 + effect.get(2, 0);
						processBuff(p, a, b, -1); break;

					case ExerProEffectData.Code.AddMHP:
						p = RuntimeBattler.MHPParamId;
						a = effect.get(0, 0);
						b = 100 + effect.get(1, 0);
						processBuff(p, a, b, -1); break;

					case ExerProEffectData.Code.AddPower:
						p = RuntimeBattler.PowerParamId;
						a = effect.get(0, 0);
						b = 100 + effect.get(1, 0);
						processBuff(p, a, b, -1); break;

					case ExerProEffectData.Code.AddDefense:
						p = RuntimeBattler.DefenseParamId;
						a = effect.get(0, 0);
						b = 100 + effect.get(1, 0);
						processBuff(p, a, b, -1); break;

					case ExerProEffectData.Code.AddAgile:
						p = RuntimeBattler.AgileParamId;
						a = effect.get(0, 0);
						b = 100 + effect.get(1, 0);
						processBuff(p, a, b, -1); break;

					case ExerProEffectData.Code.TempAddParam:
						p = effect.get(0, 0);
						a = effect.get(1, 0);
						b = 100 + effect.get(2, 0);
						n = effect.get(3, 1);
						processBuff(p, a, b, n); break;

					case ExerProEffectData.Code.TempAddMHP:
						p = RuntimeBattler.MHPParamId;
						a = effect.get(0, 0);
						b = 100 + effect.get(1, 0);
						n = effect.get(2, 1);
						processBuff(p, a, b, n); break;

					case ExerProEffectData.Code.TempAddPower:
						p = RuntimeBattler.PowerParamId;
						a = effect.get(0, 0);
						b = 100 + effect.get(1, 0);
						n = effect.get(2, 1);
						processBuff(p, a, b, n); break;

					case ExerProEffectData.Code.TempAddDefense:
						p = RuntimeBattler.DefenseParamId;
						a = effect.get(0, 0);
						b = 100 + effect.get(1, 0);
						n = effect.get(2, 1);
						processBuff(p, a, b, n); break;

					case ExerProEffectData.Code.TempAddAgile:
						p = RuntimeBattler.AgileParamId;
						a = effect.get(0, 0);
						b = 100 + effect.get(1, 0);
						n = effect.get(2, 1);
						processBuff(p, a, b, n); break;

					case ExerProEffectData.Code.AddEnergy:
						a = effect.get(0, 0);
						processEnergy(a); break;

					case ExerProEffectData.Code.AddState:
						s = effect.get(0, 0); r = effect.get(1, 0);
						p = effect.get(2, 100);
                        processAddState(s, r, p); break;

                    case ExerProEffectData.Code.RemoveState:
						s = effect.get(0, 0); r = effect.get(1, 0);
						p = effect.get(2, 100);
						processRemoveState(s, r, p); break;

                    case ExerProEffectData.Code.RemoveNegaState:
                        _processRemoveNegaStates(); break;

                    case ExerProEffectData.Code.DrawCards:
						n = effect.get(0, 0); drawCards(n); break;

                    case ExerProEffectData.Code.ConsumeCards:
						n = effect.get(0, 0);
						select = effect.get(1, false);
						consumeCards(n, select); break;

					case ExerProEffectData.Code.GainGold:
						a = effect.get(0, 0);
						b = effect.get(1, a);
						gainGold(a, b); break;

					case ExerProEffectData.Code.GainCard:
						a = effect.get(0, -1); gainCard(a); break;

					case ExerProEffectData.Code.DropCard:
						a = effect.get(0, 0); dropCard(a); break;

					case ExerProEffectData.Code.CopyCard:
						a = effect.get(0, 0); copyCard(a); break;

					case ExerProEffectData.Code.GainItem:
						a = effect.get(0, -1); gainItem(a); break;

					case ExerProEffectData.Code.LoseItem:
						a = effect.get(0, -1); loseItem(a); break;

					case ExerProEffectData.Code.GainPotion:
						a = effect.get(0, -1); gainPotion(a); break;

					case ExerProEffectData.Code.LosePotion:
						a = effect.get(0, -1); losePotion(a); break;
				}
			}

			#region 处理函数

			/// <summary>
			/// 处理完美斩击
			/// </summary>
			void _processSlashAttack(int a, int b = 2) {
                const string SlashStr = "斩击";
                var actor = subject as RuntimeActor;
                if (actor == null) return;

                var hand = actor.cardGroup.handGroup;
                var cnt = hand.countItems(card => card.item().name.Contains(SlashStr));

                processDamage(a + b * cnt);
            }

            /// <summary>
            /// 处理消除消极状态
            /// </summary>
            void _processRemoveNegaStates() {
                var states = DataService.get().staticData.data.exerProStates;
                foreach (var state in states) removeState(state.id);
            }

            /// <summary>
            /// 计算伤害值
            /// </summary>
            /// <param name="a">伤害点数</param>
            /// <param name="h">伤害类型</param>
            void processDamage(int a, int h = HPDamageType) {
                if (h == HPRecoverType)
					result.hpRecover = calcRecover(a);
                else {
                    var val = calcDamage(a);
                    if (h == HPDamageType) result.hpDamage = val;
                    if (h == HPDrainType) result.hpDrain = val;
                }
            }

			/// <summary>
			/// 计算性质加成
			/// </summary>
			/// <param name="c1"></param>
			/// <param name="c2"></param>
			double calcCharacterRate(string c1, string c2) {
				var antonyms = DataService.get().staticData.configure.antonyms;
				foreach (var ant in antonyms)
					if (ant.cardWord == c1 && ant.enemyWord == c2) return ant.hurtRate;
				return 1;
			}

			/// <summary>
			/// 计算伤害
			/// </summary>
			/// <param name="a"></param>
			/// <returns></returns>
			int calcDamage(int a) {
				double res = a + subject.power() - object_.defense();

				if (subject.isActor()) res *= calcCharacterRate(
					subject.currentCharacter(), object_.currentCharacter());

				res += subject.damagePlusVal();
				res += object_.hurtPlusVal();

				res *= subject.damagePlusRate();
				res *= object_.hurtPlusRate();

				return (int)Math.Round(Math.Max(res, 1));
			}

			/// <summary>
			/// 计算回复
			/// </summary>
			/// <param name="a"></param>
			/// <returns></returns>
			int calcRecover(int a) {
				double res = a;

				res += object_.recoverPlusVal();
				res *= object_.recoverPlusRate();

				return (int)Math.Round(Math.Max(res, 1));
			}

			/// <summary>
			/// 处理能量获得
			/// </summary>
			/// <param name="a"></param>
			void processEnergy(int a) {
				result.energyGain = a;
			}

            /// <summary>
            /// 处理Buff
            /// </summary>
            /// <param name="p">属性ID</param>
            /// <param name="a">点数</param>
            /// <param name="b">比率</param>
            /// <param name="n">回合数（为-1则为永久）</param>
            void processBuff(int p, int a = 0, int b = 100, int n = 0) {
                addBuff(p, a, b / 100.0, n);
            }

            /// <summary>
            /// 处理添加状态
            /// </summary>
            /// <param name="s">状态ID</param>
            /// <param name="r">回合数</param>
            /// <param name="p">几率</param>
            void processAddState(int s, int r = 0, int p = 100) {
                var rand = Random.Range(0, 100);
                if (rand < p) addState(s, r);
            }

            /// <summary>
            /// 处理移除状态
            /// </summary>
            /// <param name="s">状态ID</param>
            /// <param name="r">回合数</param>
            /// <param name="p">几率</param>
            void processRemoveState(int s, int r = 0, int p = 100) {
                var rand = Random.Range(0, 100);
                if (rand < p) removeState(s, r);
            }

            /// <summary>
            /// 添加状态
            /// </summary>
            /// <param name="stateId">状态ID</param>
            /// <param name="turns">持续回合</param>
            void addState(int stateId, int turns) {
                stateChanges.Add(new RuntimeActionResult.StateChange(stateId, turns));
            }

            /// <summary>
            /// 移除状态
            /// </summary>
            /// <param name="stateId">状态ID</param>
            /// <param name="turns">持续回合</param>
            void removeState(int stateId, int turns = 0) {
                stateChanges.Add(new RuntimeActionResult.StateChange(stateId, turns, true));
            }

            /// <summary>
            /// 添加Buff
            /// </summary>
            /// <param name="paramId">状态ID</param>
            /// <param name="value">状态ID</param>
            /// <param name="rate">状态ID</param>
            /// <param name="turns">持续回合</param>
            void addBuff(int paramId, int value = 0, double rate = 1, int turns = 0) {
                addBuffs.Add(new RuntimeBuff(paramId, value, rate, turns));
            }

            /// <summary>
            /// 抽取卡牌
            /// </summary>
            /// <param name="count">数量</param>
            void drawCards(int count) {
				Debug.Log("drawCards");
				result.drawCardCnt = count;
            }

            /// <summary>
            /// 消耗卡牌
            /// </summary>
            /// <param name="count">数量</param>
            /// <param name="show">是否显示并选择</param>
            void consumeCards(int count, bool select = true) {
                result.consumeCardCnt = count;
				result.consumeSelect = select;
            }

			/// <summary>
			/// 获得金钱
			/// </summary>
			void gainGold(int a, int b) {
				result.gainCard = Random.Range(a, b + 1);
			}

			/// <summary>
			/// 获得卡牌
			/// </summary>
			void gainCard(int i) {
				// TODO: 处理随机卡牌
				result.gainCard = i;
			}

			/// <summary>
			/// 获得道具
			/// </summary>
			void gainItem(int i) {
				// TODO: 处理随机道具
				result.gainItem = i;
			}

			/// <summary>
			/// 获得药水
			/// </summary>
			void gainPotion(int i) {
				// TODO: 处理随机药水
				result.gainPotion = i;
			}

			/// <summary>
			/// 去除卡牌
			/// </summary>
			void dropCard(int i) {
				// TODO: 处理随机卡牌
				result.dropCard = i;
			}

			/// <summary>
			/// 复制卡牌
			/// </summary>
			void copyCard(int i) {
				// TODO: 处理随机卡牌
				result.copyCard = i;
			}

			/// <summary>
			/// 失去道具
			/// </summary>
			void loseItem(int i) {
				// TODO: 处理随机道具
				result.loseItem = i;
			}

			/// <summary>
			/// 失去药水
			/// </summary>
			void losePotion(int i) {
				// TODO: 处理随机药水
				result.losePotion = i;
			}

			#endregion
		}

		/// <summary>
		/// 结果应用计算类
		/// </summary>
		public class ResultApplyCalc {

            /// <summary>
            /// 属性
            /// </summary>
            RuntimeBattler battler;
            RuntimeActionResult result;

            RuntimeBattler subject; // 发起者
			RuntimeActor actor; // 关联的角色

			/// <summary>
			/// 应用
			/// </summary>
			/// <param name="battler">对战者</param>
			/// <param name="result">结果</param>
			public static void apply(RuntimeBattler battler, RuntimeActionResult result) {
                var calc = new ResultApplyCalc(battler, result);
                calc.processHP();
                calc.processAddBuffs();
                calc.processAddStates();
                calc.processConsume();
                calc.processDraw();
				calc.processEnergy();
				calc.processGain();
			}

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="battler">对战者</param>
            /// <param name="result">结果</param>
            ResultApplyCalc(RuntimeBattler battler, RuntimeActionResult result) {
                this.battler = battler; this.result = result;
                subject = result.action.subject;
				actor = (battler as RuntimeActor) 
					?? (subject as RuntimeActor);
			}

            /// <summary>
            /// 处理HP
            /// </summary>
            void processHP() {
                battler.addHP(result.hpRecover);
                battler.addHP(-result.hpDamage);
                battler.addHP(-result.hpDrain);

                subject.addHP(result.hpDrain);
            }

            /// <summary>
            /// 处理Buff增加
            /// </summary>
            void processAddBuffs() {
                foreach (var buff in result.addBuffs)
                    battler.addBuff(buff);
            }

            /// <summary>
            /// 处理状态增加
            /// </summary>
            void processAddStates() {
                foreach (var state in result.stateChanges) 
                    if (state.remove)
                        battler.removeState(state.stateId, state.turns);
                    else
                        battler.addState(state.stateId, state.turns);
            }

            /// <summary>
            /// 处理抽牌
            /// </summary>
            void processDraw() {
                if (actor == null) return;

                for (int i = 0; i < result.drawCardCnt; ++i)
                    actor.cardGroup.drawCard();
            }

            /// <summary>
            /// 处理消耗
            /// </summary>
            void processConsume() {
                if (result.consumeSelect) return;
                if (actor == null) return;

                for (int i = 0; i < result.consumeCardCnt; ++i)
                    actor.cardGroup.consumeCard();
            }

			/// <summary>
			/// 处理能量变化
			/// </summary>
			void processEnergy() {
				if (actor == null) return;

				actor.addEnergy(result.energyGain);
			}

			/// <summary>
			/// 处理物品获得
			/// </summary>
			void processGain() {
				if (actor == null) return;

				actor.gainGold(result.gainGold);
				actor.gainCard(result.gainCard);
				actor.gainItem(result.gainItem);
				actor.gainPotion(result.gainPotion);
			}
		}

        /// <summary>
        /// 敌人下一步行动计算类
        /// </summary>
        public class EnemyNextCalc {

			/// <summary>
			/// 动画常量控制
			/// </summary>
			public const int AttackAniIndex = 1; // 开始攻击动画
			public const int SkillAniIndex = 2; // 开始技能动画
			public const int HurtAniIndex = 3; // 受伤动画
			public const int PowerUpAniIndex = 4; // 提升动画
			public const int PowerDownAniIndex = 5; // 削弱动画

			/// <summary>
			/// 属性
			/// </summary>
			int round;
            RuntimeEnemy enemy;

            ExerProEnemy enemyData;
            ExerProEnemy.Action[] actions;

            /// <summary>
            /// 计算
            /// </summary>
            public static void calc(int round,
                RuntimeEnemy enemy, RuntimeActor actor,
                out ExerProEnemy.Action action, 
                out ExerPro.EnglishModule.Data.RuntimeAction runtimeAction) {

                var calc = new EnemyNextCalc(round, enemy);

                action = calc.generateAction();
                runtimeAction = calc.generateRuntimeAction(actor);
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            EnemyNextCalc(int round, RuntimeEnemy enemy) {
                this.round = round; this.enemy = enemy;
                enemyData = enemy.enemy();

				Debug.Log("EnemyNextCalc: " + enemyData.name + ": " + enemyData.toJson().ToJson());

				actions = filterActions(enemyData.actions);
            }

			/// <summary>
			/// 过滤行动
			/// </summary>
			/// <returns>过滤后的行动列表</returns>
			ExerProEnemy.Action[] filterActions(ExerProEnemy.Action[] actions) {
                var res = new List<ExerProEnemy.Action>();
				foreach (var action in actions) {
					Debug.Log(enemyData.name + ": filterActions (" + action.testRound(round) + ") " + action.toJson().ToJson());
					if (action.testRound(round)) res.Add(action);
				}
				return res.ToArray();
            }

            /// <summary>
            /// 生成行动
            /// </summary>
            ExerProEnemy.Action generateAction() {
				var list = new List<int>();
                for (var i = 0; i < actions.Length; ++i) {
                    var action = actions[i];
                    for (var j = 0; j < action.rate; ++j) list.Add(i);
                }
				Debug.Log(enemyData.name + ": generateAction: " + string.Join(",", list));
				if (list.Count <= 0) return null; 
                var index = Random.Range(0, list.Count);
                return actions[list[index]];
            }

            /// <summary>
            /// 生成运行时行动
            /// </summary>
            ExerPro.EnglishModule.Data.RuntimeAction 
                generateRuntimeAction(RuntimeActor actor) {

                RuntimeBattler object_;
				int startAniIndex = 0, targetAniIndex = 0;
                var params_ = enemy.currentActionParams();
                var effects = new List<ExerProEffectData>();
				bool move = false;

                switch (enemy.currentActionTypeEnum()) {
                    case ExerProEnemy.Action.Type.Attack:
                        _processAttack(effects, params_);
						startAniIndex = AttackAniIndex;
						targetAniIndex = HurtAniIndex;
						move = true; object_ = actor; break;

					case ExerProEnemy.Action.Type.NegStates:
						_processAddStates(effects, params_);
						startAniIndex = SkillAniIndex;
						targetAniIndex = PowerDownAniIndex;
						object_ = actor; break;

					case ExerProEnemy.Action.Type.PowerDown:
                        _processPowerDown(effects, params_);
						startAniIndex = SkillAniIndex;
						targetAniIndex = PowerDownAniIndex;
						object_ = actor; break;

                    case ExerProEnemy.Action.Type.PosStates:
                        _processAddStates(effects, params_);
						startAniIndex = SkillAniIndex;
						targetAniIndex = PowerUpAniIndex;
						object_ = enemy; break;

                    case ExerProEnemy.Action.Type.PowerUp:
                        _processPowerUp(effects, params_);
						startAniIndex = SkillAniIndex;
						targetAniIndex = PowerUpAniIndex;
						object_ = enemy; break;

                    default: return null;
                }

                effects.AddRange(enemyData.effects);

				var startAni = AssetLoader.loadAnimation(startAniIndex);
				var targetAni = AssetLoader.loadAnimation(targetAniIndex);

				return new ExerPro.EnglishModule.Data.RuntimeAction(
                    enemy, object_, effects.ToArray(), startAni, targetAni, move);
            }

            /// <summary>
            /// 处理攻击行动
            /// </summary>
            void _processAttack(
                List<ExerProEffectData> effects, JsonData actionParams) {

				JsonData params_ = new JsonData();
				params_.SetJsonType(JsonType.Array);
				if (actionParams.Count > 0)
					params_.Add(actionParams[0]);

				effects.Add(new ExerProEffectData(
                    ExerProEffectData.Code.Attack, params_));
            }

            /// <summary>
            /// 处理削弱
            /// </summary>
            void _processPowerDown(
                List<ExerProEffectData> effects, JsonData actionParams) {
                var t = actionParams.Count == 5 ?
                    actionParams[4] : 1;

                for (int i = 0; i < 4; ++i) {
                    var params_ = new JsonData();
					var p = DataLoader.load<int>(actionParams[i]);

					params_.SetJsonType(JsonType.Array);
					params_.Add(i); params_.Add(-p);
					params_.Add(0); params_.Add(t);

					Debug.Log("_processPowerDown: " + params_.ToJson());

					effects.Add(new ExerProEffectData(
                        ExerProEffectData.Code.AddParam, params_));
                }
            }

            /// <summary>
            /// 处理状态
            /// </summary>
            void _processAddStates(
                List<ExerProEffectData> effects, JsonData actionParams) {
                for (int i = 0; i < actionParams.Count; ++i) 
                    effects.Add(new ExerProEffectData(
                        ExerProEffectData.Code.AddParam,
                        actionParams[i]));
            }

            /// <summary>
            /// 处理削弱
            /// </summary>
            void _processPowerUp(
                List<ExerProEffectData> effects, JsonData actionParams) {
                var t = actionParams.Count == 5 ?
                    actionParams[4] : 1;

                for (int i = 0; i < 4; ++i) {
                    var params_ = new JsonData();
					var p = DataLoader.load<int>(actionParams[i]);

					params_.SetJsonType(JsonType.Array);
                    params_.Add(i); params_.Add(p);
                    params_.Add(0); params_.Add(t);

					Debug.Log("_processPowerUp: " + params_.ToJson());

					effects.Add(new ExerProEffectData(
                        ExerProEffectData.Code.AddParam, params_));
                }
            }
        }

		/// <summary>
		/// 单词选项生成器
		/// </summary>
		public class WordChoicesGenerator {

			/// <summary>
			/// 最大选项数
			/// </summary>
			public const int MaxChoices = 4;
			
			/// <summary>
			/// 属性
			/// </summary>
			Word word;
			ICollection<Word> words;

			List<string> result;

			/// <summary>
			/// 生成
			/// </summary>
			/// <param name="word">单词</param>
			/// <param name="words">单词集</param>
			/// <returns></returns>
			public static List<string> generate(Word word, ICollection<Word> words) {
				var generator = new WordChoicesGenerator(word, words);
				return generator.result;
			}

			/// <summary>
			/// 构造函数
			/// </summary>
			WordChoicesGenerator(Word word, ICollection<Word> words) {
				this.word = word; this.words = words;
				result = new List<string>(MaxChoices);

				// 正确答案位置
				var corrIndex = Random.Range(0, MaxChoices);

				for (var i = 0; i < MaxChoices; ++i)
					if (corrIndex == i)
						result.Add(word.chinese);
					else 
						result.Add(generateWordChoice(
							Random.Range(0, 2) == 1).chinese);
			}

			/// <summary>
			/// 生成距离字典
			/// </summary>
			/// <param name="chi">中文距离</param>
			/// <param name="eng">英文距离</param>
			Word generateWordChoice(bool english) {
				var minVal = 999.0;
				Word minWord = null;
				foreach (var word_ in words) {
					if (word_.chinese == word.chinese) continue;
					var val = calcDistance(word, word_, english);
					if (val < minVal && word_ != word &&
						!result.Contains(word_.chinese))
						minWord = word_; minVal = val;
				}
				return minWord;
			}
			
			/// <summary>
			/// 以s1为基准，计算s2到s1的距离
			/// </summary>
			/// <returns>返回距离</returns>
			static double calcDistance(Word w1, Word w2, bool english = false) {
				if (english) return calcDistance(w1.english, w2.english);
				return calcDistance(w1.chinese, w2.chinese);
			}
			static double calcDistance(string s1, string s2) {
				int sum = 0, j = 0, k = 0;
				int len1 = s1.Length, len2 = s2.Length;
				for (var i = 0; i < len1; ++i) {
					j = k; k = 0;
					while (j < len2) {
						if (s2[j] == s1[i]) {
							if (k == 0) k = j;
							j++; break;
						}
						if (k == 0) k = j;
						sum++; j++;
					}
				}
				var res = sum * 1.0 / len2;
				//Debug.Log("Dist: " + s1 + " -> " + s2 + ": " + res);
				return res;
			}

		}

		/// <summary>
		/// 特训特性计算类
		/// </summary>
		public class ExerProTraitsCalc {

			/// <summary>
			/// 计算战斗开始时的特性效果
			/// </summary>
			/// <param name="battler"></param>
			public static void calcOnBattleStart(RuntimeBattler battler) {
				processBattleStartRecover(battler);
				processParamBattleAdd(battler);
				processBattleDrawCards(battler);
			}
			static void processBattleStartRecover(RuntimeBattler battler) {
				var traits = battler.filterTraits(ExerProTraitData.Code.BattleStartRecover);

				int val = battler.sumTraits(traits);
				double rate = battler.sumTraits(traits, 1) / 100.0;

				battler.addHP(val); battler.addHP(rate);
			}
			static void processParamBattleAdd(RuntimeBattler battler) {
				for (var i = 0; i < RuntimeBattler.MaxParamCount; ++i) {
					var traits = battler.filterTraits(ExerProTraitData.Code.ParamBattleAdd, i);

					var val = battler.sumTraits(traits, 1);
					var rate = battler.multTraits(traits, 2);

					battler.addParam(i, val, rate);
				}
			}
			static void processBattleDrawCards(RuntimeBattler battler) {
				var actor = battler as RuntimeActor;
				if (actor == null) return;

				var val = actor.sumTraits(ExerProTraitData.Code.BattleDrawCards);

				for (int i = 0; i < val; ++i)
					actor.cardGroup.drawCard();
			}

			/// <summary>
			/// 计算回合开始时的特性效果
			/// </summary>
			/// <param name="battler"></param>
			public static void calcOnRoundStart(RuntimeBattler battler, int rest = 0) {
				processRoundStartRecover(battler);
				if (rest > 0) processParamRoundAdd(battler, rest);
				processRoundDrawCards(battler);
			}
			static void processRoundStartRecover(RuntimeBattler battler) {
				var traits = battler.filterTraits(ExerProTraitData.Code.RoundStartRecover);

				int val = battler.sumTraits(traits);
				double rate = battler.sumTraits(traits, 1) / 100.0;

				battler.addHP(val); battler.addHP(rate);
			}
			static void processParamRoundAdd(RuntimeBattler battler, int rest) {
				for (var i = 0; i < RuntimeBattler.MaxParamCount; ++i) {
					var traits = battler.filterTraits(ExerProTraitData.Code.ParamRoundAdd, i);

					var val = battler.sumTraits(traits, 1);
					var rate = battler.multTraits(traits, 2);

					battler.addBuff(i, val, rate, rest);
				}
			}
			static void processRoundDrawCards(RuntimeBattler battler) {
				var actor = battler as RuntimeActor;
				if (actor == null) return;

				var val = actor.sumTraits(ExerProTraitData.Code.RoundDrawCards);

				for (int i = 0; i < val; ++i)
					actor.cardGroup.drawCard();
			}

			/// <summary>
			/// 计算回合开始时的特性效果
			/// </summary>
			/// <param name="battler"></param>
			public static void calcOnRoundEnd(RuntimeBattler battler) {
				processRoundEndRecover(battler);
			}
			static void processRoundEndRecover(RuntimeBattler battler) {
				var traits = battler.filterTraits(ExerProTraitData.Code.RoundEndRecover);

				int val = battler.sumTraits(traits);
				double rate = battler.sumTraits(traits, 1) / 100.0;

				battler.addHP(val); battler.addHP(rate);
			}

			/// <summary>
			/// 计算回合开始时的特性效果
			/// </summary>
			/// <param name="battler"></param>
			public static void calcOnBattleEnd(RuntimeBattler battler, int rest = 0) {
				processBattleEndRecover(battler);
			}
			static void processBattleEndRecover(RuntimeBattler battler) {
				var traits = battler.filterTraits(ExerProTraitData.Code.BattleEndRecover);

				int val = battler.sumTraits(traits);
				double rate = battler.sumTraits(traits, 1) / 100.0;

				battler.addHP(val); battler.addHP(rate);
			}
		}

        /// <summary>
        /// 奖励生成器
        /// </summary>
        public class RewardGenerator {

            /// <summary>
            /// 奖励卡牌选择数量
            /// </summary>
            const int RewardCardNumber = 3;

            /// <summary>
            /// 获取奖励金币
            /// 战斗据点：层数+敌人；精英：敌人；听力/藏宝：层数+题目
            /// </summary>
            /// <param name="type">据点类型</param>
            /// <param name="layer">层数，即当前节点的xOrder</param>
            /// <param name="enemy">杀死敌人数</param>
            /// <param name="question">答对题目数量</param>
            /// <returns>奖励金币</returns>
            public static int getGoldReward(ExerProMapNode node, 
				ExerProRecord record, int enemy = 0, int question = 0) {
                record.scoreRecord.killEnemyAccmu += enemy;

                var type = node.typeEnum(); var layer = node.xOrder + 1;

                int randBase = Random.Range(0, 5);
                switch (type) {
                    case ExerProMapNode.Type.Enemy:
                        return layer * randBase + enemy * (randBase + 15);
                    case ExerProMapNode.Type.Elite:
                        return enemy * (randBase + 35);
                    case ExerProMapNode.Type.Treasure:
                        return layer * randBase + question * 15;
                    case ExerProMapNode.Type.Story:
                        return layer * randBase + question * 50;
                }
                return -1;
            }

            /// <summary>
            /// boss奖励
            /// </summary>
            /// <param name="stageOrder"></param>
            /// <returns></returns>
            public static int getBossGoldReward(ExerProRecord record) {
                int randBase = Random.Range(35, 40);
                return record.stageOrder * randBase;
            }

            /// <summary>
            /// 获取生成的奖励卡牌组（三挑一）
            /// </summary>
            /// <param name="type">据点类型</param>
            /// <returns></returns>
            public static List<ExerProCard> getCardRewards(ExerProMapNode.Type type) {
                List<ExerProCard> exerProCards = new List<ExerProCard>();

                switch (type) {
                    case ExerProMapNode.Type.Enemy:
                    case ExerProMapNode.Type.Elite:
                        exerProCards = ExerProItemGenerator.
							generateCards(RewardCardNumber, 0.75, 0.2);
                        break;

                    case ExerProMapNode.Type.Treasure:
                    case ExerProMapNode.Type.Story:
                        exerProCards = ExerProItemGenerator.
							generateCards(RewardCardNumber, 0.35, 0.4);
                        break;

                    case ExerProMapNode.Type.Boss:
                        exerProCards = ExerProItemGenerator.
							generateCards(RewardCardNumber, 0, 0);
                        break;
                }

                return exerProCards;
            }

            /// <summary>
            /// 获取当前玩家的积分
            /// 该函数使用内部杀死敌人记录进行计算，需在获取通关金币奖励后使用
            /// </summary>
            /// <param name="layer">层数，即当前节点的xOrder</param>
            /// <param name="gold">玩家的金币数</param>
            /// <param name="cards">玩家的卡牌数</param>
            /// <param name="boss">杀死的boss数</param>
            /// <returns>当前玩家总的积分</returns>
            public static int generateScore(ExerProRecord record, int boss = 0, bool isPerfect = false) {
                record.scoreRecord.killBossAccmu += boss;

                var gold = record.gold;
                var cards = record.actor.cardGroup.getCardNumber();

                var score = record.scoreRecord.killEnemyAccmu * 2 + record.scoreRecord.killBossAccmu * 50;
                score += gold / 100 * 25 + (cards > 30 ? (cards - 30) / 5 * 10 : 0);
                if (isPerfect)
                    record.scoreRecord.perfectNumber += 1;
                score += record.scoreRecord.perfectNumber * 50;

                var layer = record.currentNode().xOrder + 1;
                score += (layer + record.scoreRecord.stageOrderAccumu) * 5;

				if (boss > 0) 
                    record.scoreRecord.stageOrderAccumu += layer;

                return score;
            }

        }

        /// <summary>
        /// 随机生成特训物品，如卡牌、药水等
        /// </summary>
        public class ExerProItemGenerator {

            /// <summary>
            /// 指定生成物品类型
            /// </summary>
            public enum Type {
                Card, Potion,
            }

            /// <summary>
            /// 内部变量
            /// </summary>
            const int NormalStarID = 1;
            const int RareStarID = 2;
			const int EpicStarID = 3;
            double DefaultNoramlRatio = 0.8;
            double DefaultRareRatio = 0.15;

            int[] cardTypePrice = new int[3]{ 20, 50, 100 };

			/*
            /// <summary>
            /// 用于生成商店刷新的物品
            /// </summary>
            /// <param name="count">数量</param>
            /// <param name="stageOrder">大层数</param>
            /// <param name="type">物品类型</param>
            /// <returns></returns>
            public static List<BaseExerProItem> generateBusinessItem(int count, int stageOrder, Type type = Type.Card) {
                List<BaseExerProItem> items = new List<BaseExerProItem>();
                var normalRatio = itemGenerator.DefaultNoramlRatio - 0.15 * stageOrder;
                var rareRatio = itemGenerator.DefaultRareRatio + 0.1 * stageOrder;
                while (items.Count != count) {
                    var result = itemGenerator.generateRandomItem(normalRatio, rareRatio, type);
                    if (result == null)
                        continue;
                    items.Add(result);
                }
                return items;
            }

            /// <summary>
            /// 商店出售卡牌定价
            /// </summary>
            /// <param name="card"></param>
            /// <returns></returns>
            public static int generateCardPrice(ExerProCard card) {
                var price = card.cost * Random.Range(5, 10) +
                    itemGenerator.cardTypePrice[card.starId - 1] * Random.Range(0.7f, 1) +
                    ((card.target == (int)ExerProCard.Target.All) ? 10 : 0);   
                return (int)price;
            }

            /// <summary>
            /// 商店出售药水定价
            /// </summary>
            /// <param name="potion"></param>
            /// <returns></returns>
            public static int generatePotionPrice(ExerProPotion potion) {
                switch (potion.starId) {
                    case NormalStarID:
                        return Random.Range(30, 45);
                    case RareStarID:
                        return Random.Range(55, 75);
                }
                return 0;
            }
			*/

            /// <summary>
            /// 随机生成指定数量的卡牌，需提供生成概率
            /// </summary>
            /// <param name="count">数量</param>
            /// <param name="normalRatio">普通概率</param>
            /// <param name="rareRatio">稀有概率</param>
            /// <returns></returns>
            public static List<ExerProCard> generateCards(int count, double normalRatio, double rareRatio){
                List<ExerProCard> exerProCards = new List<ExerProCard>();

				Debug.Log("generateCards: " + count + ", " + normalRatio + ", " + rareRatio);

				int cnt = 0;
				while (exerProCards.Count != count && cnt++ <= 100000) {
                    var result = itemGenerator.generateRandomItem(normalRatio, rareRatio) as ExerProCard;
					if (result == null || exerProCards.Contains(result)) continue;

					Debug.Log("exerProCards.Add(" + result.name + ")");

                    exerProCards.Add(result);
                }

                return exerProCards;
            }

            /// <summary>
            /// 随机生成指定数量的药水，需提供生成概率
            /// </summary>
            /// <param name="count">数量</param>
            /// <param name="normalRatio">普通概率</param>
            /// <param name="rareRatio">稀有概率</param>
            /// <returns></returns>
            public static List<ExerProPotion> generatePotions(int count, double normalRatio, double rareRatio) {
                List<ExerProPotion> exerProPotions = new List<ExerProPotion>();

				int cnt = 0;
                while (exerProPotions.Count != count && cnt++ <= 100000) {
                    var result = itemGenerator.generateRandomItem(normalRatio, rareRatio, Type.Potion) as ExerProPotion;
                    if (result == null || exerProPotions.Contains(result)) continue;

					Debug.Log("exerProCards.Add(" + result.name + ")");

                    exerProPotions.Add(result);
                }

                return exerProPotions;
            }

            /// <summary>
            /// 当前类对象
            /// </summary>
            static ExerProItemGenerator _itemGenerator = null;
            static ExerProItemGenerator itemGenerator {
                get {
                    if (_itemGenerator == null)
                        _itemGenerator = new ExerProItemGenerator();
                    return _itemGenerator;
                }
            }

            /// <summary>
            /// 生成一个指定物品
            /// </summary>
            /// <param name="normalRatio"></param>
            /// <param name="rareRatio"></param>
            /// <param name="type"></param>
            /// <returns></returns>
            BaseExerProItem generateRandomItem(double normalRatio, double rareRatio, Type type = Type.Card) {
                var dataSer = DataService.get();
                List<BaseExerProItem> items;
                switch (type) {
                    case Type.Card:
                        items = new List<BaseExerProItem>(dataSer.staticData.data.exerProCards);
                        break;
                    case Type.Potion:
                        items = new List<BaseExerProItem>(dataSer.staticData.data.exerProPotions);
                        break;
                    default:
                        return null;
                }

                shuffleItems(items);
				float randomValue = Random.Range(0.0f, 1.0f);
                if (randomValue < normalRatio)
                    return items.Find(e => e.starId == NormalStarID);
                else if (randomValue >= normalRatio && randomValue <= normalRatio + rareRatio)
                    return items.Find(e => e.starId == RareStarID);
                else
                    return items.Find(e => e.starId == EpicStarID);
            }

            /// <summary>
            /// 打乱数组
            /// </summary>
            /// <param name="cards"></param>
            /// <returns></returns>
            void shuffleItems(List<BaseExerProItem> items) {
                items.Sort(delegate (BaseExerProItem a, BaseExerProItem b) { return Random.Range(-1, 1); });
            }
        }

	}
}
