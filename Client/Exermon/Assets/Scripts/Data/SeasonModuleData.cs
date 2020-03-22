
using System;
using UnityEngine;

using Core.Data;

/// <summary>
/// 赛季模块
/// </summary>
namespace SeasonModule { }

/// <summary>
/// 赛季模块数据
/// </summary>
namespace SeasonModule.Data {

    /// <summary>
    /// 赛季数据
    /// </summary>
    public class CompSeason : TypeData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public DateTime startTime { get; protected set; }
        [AutoConvert]
        public DateTime endTime { get; protected set; }
    }

    /// <summary>
    /// 段位数据
    /// </summary>
    public class CompRank : TypeData {

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public Color color { get; protected set; }
        [AutoConvert]
        public int subRankNum { get; protected set; }
        [AutoConvert]
        public int scoreFactor { get; protected set; }
        [AutoConvert]
        public int offsetFactor { get; protected set; }
    }
}