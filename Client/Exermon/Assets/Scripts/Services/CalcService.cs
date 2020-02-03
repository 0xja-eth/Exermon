using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

using LitJson;

/// <summary>
/// 计算服务
/// </summary>
public class CalcService : BaseService<CalcService> {

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
        /// <returns>属性实际值</returns>
        public static double calc(double base_, double rate, int level) {
            return base_ * Math.Pow((rate / R + 1) * S, level - 1);
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
        PackContItem playerGift;
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

            return ExermonParamCalc.calc(epb, rpr, playerExer.level);
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
}
