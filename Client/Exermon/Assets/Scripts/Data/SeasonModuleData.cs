
using System;
using System.Collections.Generic;

using UnityEngine;

using LitJson;

using Core.Data;
using Core.Data.Loaders;

using GameModule.Services;

/// <summary>
/// 赛季模块
/// </summary>
namespace SeasonModule { }

/// <summary>
/// 赛季模块数据
/// </summary>
namespace SeasonModule.Data {

    /// <summary>
    /// 段位数据
    /// </summary>
    public class CompRank : TypeData {

        /// <summary>
        /// 每个子段位星星数
        /// </summary>
        public const int StarsPerSubRank = 3;

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

        /// <summary>
        /// 段位图标
        /// </summary>
        public Sprite icon { get; protected set; }

        /// <summary>
        /// 段位星星总数
        /// </summary>
        /// <returns>返回当前段位的星星总数</returns>
        public int starCount() {
            return StarsPerSubRank * subRankNum;
        }

        /// <summary>
        /// 加载自定义属性
        /// </summary>
        /// <param name="json"></param>
        protected override void loadCustomAttributes(JsonData json) {
            base.loadCustomAttributes(json);
            icon = AssetLoader.getRankIconSprite(getID());
        }
    }

    /// <summary>
    /// 赛季记录数据
    /// </summary>
    public class SeasonRecord : BaseData {

        /// <summary>
        /// 禁赛纪录
        /// </summary>
        public class SuspensionRecord : BaseData {

            /// <summary>
            /// 属性
            /// </summary>
            [AutoConvert]
            public DateTime startTime { get; protected set; }
            [AutoConvert]
            public DateTime endTime { get; protected set; }
        }

        /// <summary>
        /// 属性
        /// </summary>
        [AutoConvert]
        public int seasonId { get; protected set; }
        [AutoConvert]
        public int starNum { get; protected set; }
        [AutoConvert]
        public int point { get; protected set; }
        [AutoConvert]
        public List<SuspensionRecord> suspensions { get; protected set; }

        /// <summary>
        /// 获取赛季实例
        /// </summary>
        /// <returns>返回赛季实例</returns>
        public CompSeason season() {
            return DataService.get().season(seasonId);
        }

        /// <summary>
        /// 当前禁赛纪录
        /// </summary>
        /// <returns>返回当前的禁赛纪录（如果没有则返回 null）</returns>
        public SuspensionRecord currentSuspension() {
            var now = DateTime.Now;
            return suspensions.Find(sus => 
                sus.startTime <= now && now < sus.endTime);
        }

    }

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

}