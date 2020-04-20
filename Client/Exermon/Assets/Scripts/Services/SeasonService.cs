using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using LitJson;

using Core.Data;
using Core.Data.Loaders;
using Core.Systems;
using Core.Services;

using SeasonModule.Data;

using GameModule.Services;

using UI.Common.Controls.ParamDisplays;

/// <summary>
/// 记录模块服务
/// </summary>
namespace SeasonModule.Services {

    /// <summary>
    /// 记录服务
    /// </summary>
    public class SeasonService : BaseService<SeasonService> {

        /// <summary>
        /// 操作文本设定
        /// </summary>
        const string GetRecord = "获取记录";
        const string GetRank = "获取排行";

        /// <summary>
        /// 排行颜色
        /// </summary>
        static readonly Color[] RankColors = new Color[] {
            new Color(0.8392f, 0.6431f, 0.2156f),
            new Color(0.8392f, 0.7254f, 0.2392f),
            new Color(0.8392f, 0.8039f, 0.3882f),
            new Color(1, 1, 1),
        };
        static readonly Color[] FontColors = new Color[] {
            new Color(1, 1, 1),
            new Color(1, 1, 1),
            new Color(1, 1, 1),
            new Color(0.3f, 0.3f, 0.3f),
        };

        /// <summary>
        /// 默认排行榜数量
        /// </summary>
        public const int DefaultRankCount = 10;

        /// <summary>
        /// 赛季排行信息
        /// </summary>
        public class SeasonRank : BaseData,
            ParamDisplay.IDisplayDataConvertable,
            ParamDisplay.IDisplayDataArrayConvertable {

            /// <summary>
            /// 最大排行
            /// </summary>
            public const int MaxRank = 9999;

            /// <summary>
            /// 排行项
            /// </summary>
            public class RankItem : BaseData, 
                ParamDisplay.IDisplayDataConvertable {

                /// <summary>
                /// 属性
                /// </summary>
                [AutoConvert]
                public int order { get; protected set; }
                [AutoConvert]
                public string name { get; protected set; }
                [AutoConvert]
                public int battlePoint { get; protected set; }
                [AutoConvert]
                public int starNum { get; protected set; }

                /// <summary>
                /// 转化为显示数据
                /// </summary>
                /// <param name="type">类型</param>
                /// <returns>返回显示数据</returns>
                public JsonData convertToDisplayData(string type = "") {
                    var res = toJson();
                    var cIndex = Mathf.Clamp(
                        order - 1, 0, RankColors.Length);
                    var color = RankColors[cIndex];
                    var fontColor = FontColors[cIndex];

                    if (order == 0)
                        res["order"] = MaxRank + "+";

                    res["rank"] = rankText();
                    res["color"] = DataLoader.convert(color);
                    res["font_color"] = DataLoader.convert(fontColor);

                    return res;
                }

                /// <summary>
                /// 获取段位文本
                /// </summary>
                /// <returns></returns>
                string rankText() {
                    return CalcService.RankCalc.getText(starNum);
                }
            }

            /// <summary>
            /// 属性
            /// </summary>
            [AutoConvert]
            public RankItem[] ranks { get; protected set; }
            [AutoConvert]
            public RankItem rank { get; protected set; }
            [AutoConvert]
            public DateTime updateTime { get; protected set; }

            /// <summary>
            /// 转化为显示数据
            /// </summary>
            /// <param name="type">类型</param>
            /// <returns>返回显示数据</returns>
            public JsonData convertToDisplayData(string type = "") {
                var res = new JsonData();
                res["update_time"] = DataLoader.convert(updateTime);
                return res;
            }

            /// <summary>
            /// 转化为显示数据
            /// </summary>
            /// <param name="type">类型</param>
            /// <returns>返回显示数据</returns>
            public JsonData[] convertToDisplayDataArray(string type = "") {
                var count = ranks.Length;
                var res = new JsonData[count];
                for (int i = 0; i < count; ++i)
                    res[i] = ranks[i].convertToDisplayData(type);
                return res;
            }


        }

        /// <summary>
        /// 业务操作
        /// </summary>
        public enum Oper {
            GetRecord, GetRank
        }

        /// <summary>
        /// 赛季排行榜
        /// </summary>
        public SeasonRank seasonRank { get; protected set; }

        /// <summary>
        /// 外部系统定义
        /// </summary>
        DataService dataSer;
        NetworkSystem networkSys;

        #region 初始化

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            dataSer = DataService.get();
            networkSys = NetworkSystem.get();

            initializeEmitHandlers();
        }

        /// <summary>
        /// 初始化发射数据处理器
        /// </summary>
        void initializeEmitHandlers() {
            networkSys.addEmitHandler("season_switch", onSeasonSwitch);
        }

        /// <summary>
        /// 初始化操作字典
        /// </summary>
        protected override void initializeOperDict() {
            base.initializeOperDict();
            addOperDict(Oper.GetRecord, GetRecord, NetworkSystem.Interfaces.SeasonRecordGet);
            addOperDict(Oper.GetRank, GetRank, NetworkSystem.Interfaces.SeasonRankGet);
        }

        #endregion

        #region 操作控制

        /// <summary>
        /// 获取记录数据
        /// </summary>
        /// <param name="sid">赛季ID</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void getSeasonRecord(int sid,
            UnityAction<SeasonRecord> onSuccess, UnityAction onError = null) {
            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                var season = DataLoader.load<SeasonRecord>(res, "record");
                onSuccess?.Invoke(season);
            };
            getSeasonRecord(sid, _onSuccess, onError);
        }
        public void getSeasonRecord(int sid,
            NetworkSystem.RequestObject.SuccessAction onSuccess,
            UnityAction onError = null) {
            JsonData data = new JsonData(); data["sid"] = sid;
            sendRequest(Oper.GetRecord, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 获取排行数据
        /// </summary>
        /// <param name="sid">赛季ID</param>
        /// <param name="count">排行数量</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void getSeasonRank(int sid, int count = DefaultRankCount,
            UnityAction onSuccess = null, UnityAction onError = null) {
            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                seasonRank = DataLoader.load(seasonRank, res, "ranks");
                onSuccess?.Invoke();
            };
            getSeasonRank(sid, count, _onSuccess, onError);
        }
        public void getSeasonRank(int sid, int count,
            NetworkSystem.RequestObject.SuccessAction onSuccess,
            UnityAction onError = null) {
            JsonData data = new JsonData();
            data["sid"] = sid; data["count"] = count;
            sendRequest(Oper.GetRank, data, onSuccess, onError, uid: true);
        }
        
        /// <summary>
        /// 获取当前赛季记录数据
        /// </summary>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        public void getCurrentSeasonRecord(
            UnityAction onSuccess = null, UnityAction onError = null) {
            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                var player = getPlayer();
                var data = DataLoader.load(res, "record");
                player.loadCurrentSeasonRecord(data);
                onSuccess?.Invoke();
            };
            getSeasonRecord(dataSer.dynamicData.curSeasonId, _onSuccess, onError);
        }
        public void getCurrentSeasonRecord(
            UnityAction<SeasonRecord> onSuccess, UnityAction onError = null) {
            getSeasonRecord(dataSer.dynamicData.curSeasonId, onSuccess, onError);
        }

        /// <summary>
        /// 获取当前赛季排行数据
        /// </summary>
        /// <param name="count">排行数量</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void getCurrentSeasonRank(int count = DefaultRankCount,
            UnityAction onSuccess = null, UnityAction onError = null) {
            getSeasonRank(dataSer.dynamicData.curSeasonId, count, onSuccess, onError);
        }
        public void getCurrentSeasonRank(int count,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
            getSeasonRank(dataSer.dynamicData.curSeasonId, count, onSuccess, onError);
        }

        #endregion

        #region 赛季操作

        /// <summary>
        /// 获取赛季列表
        /// </summary>
        /// <returns>返回所有赛季</returns>
        public List<CompSeason> seasons() {
            return dataSer.dynamicData.seasons;
        }

        /// <summary>
        /// 获取赛季段位数组
        /// </summary>
        /// <returns>返回所有赛季段位</returns>
        public CompRank[] compRanks() {
            return dataSer.staticData.configure.compRanks;
        }

        /// <summary>
        /// 获取当前赛季
        /// </summary>
        /// <returns></returns>
        public CompSeason currentSeason() {
            return dataSer.season(dataSer.dynamicData.curSeasonId);
        }

        /// <summary>
        /// 赛季切换回调
        /// </summary>
        /// <param name="json">数据</param>
        void onSeasonSwitch(JsonData json) {
            dataSer.dynamicData.curSeasonId = DataLoader.load<int>(json, "season_id");
        }

        #endregion

    }
}