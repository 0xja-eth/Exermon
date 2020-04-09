using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using LitJson;

using Core.Data;
using Core.Data.Loaders;
using Core.Systems;
using Core.Services;

using ItemModule.Data;
using PlayerModule.Data;
using BattleModule.Data;

using ItemModule.Services;
using PlayerModule.Services;
using QuestionModule.Services;

/// <summary>
/// 对战模块服务
/// </summary>
namespace BattleModule.Services {

    /// <summary>
    /// 对战服务
    /// </summary>
    public class BattleService : BaseService<BattleService> {

        /// <summary>
        /// 操作文本设定
        /// </summary>
        const string EquipItem = "装备物品";
        const string DequipItem = "卸下物品";

        const string MatchStart = "开始匹配";
        const string MatchCancel = "取消匹配";
        const string MatchProgress = "匹配进度";
        const string PrepareComplete = "准备完成";
        const string QuestionAnswer = "题目作答";
        const string ActionComplete = "行动完成";
        const string ResultComplete = "结算完成";

        /// <summary>
        /// 业务操作
        /// </summary>
        public enum Oper {
            EquipItem, DequipItem,
            MatchStart, MatchCancel, MatchProgress,
            PrepareComplete, QuestionAnswer, ActionComplete, ResultComplete
        }

        /// <summary>
        /// 对战状态
        /// </summary>
        public enum State {
            NotInBattle = 0, // 不在对战中
            Matching = 1, // 匹配中
            Matched = 10, // 匹配完毕
            Preparing = 2, // 准备中
            Questing = 3, // 作答中
            Quested = 11, // 作答完毕
            Acting = 4, // 行动中
            Resulting = 5, // 结算中
            Terminating = 6, // 结束中
            Terminated = 7, // 已结束
        }

        /// <summary>
        /// 游戏模式
        /// </summary>
        public enum Mode {
            Normal = 0
        }

        /// <summary>
        /// 当前对战
        /// </summary>
        public RuntimeBattle battle { get; protected set; }

        /// <summary>
        /// 外部系统
        /// </summary>
        ItemService itemSer;
        QuestionService quesSer;
        NetworkSystem networkSys;

        #region 初始化

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            itemSer = ItemService.get();
            quesSer = QuestionService.get();
            networkSys = NetworkSystem.get();

            initializeEmitHandlers();
        }

        /// <summary>
        /// 初始化状态字典
        /// </summary>
        protected override void initializeStateDict() {
            base.initializeStateDict();
            addStateDict(State.NotInBattle);
            addStateDict(State.Matching);
            addStateDict(State.Matched);
            addStateDict(State.Preparing);
            addStateDict(State.Questing);
            addStateDict(State.Quested);
            addStateDict(State.Acting);
            addStateDict(State.Resulting);
            addStateDict(State.Terminating);
            addStateDict(State.Terminated);
        }

        /// <summary>
        /// 初始化发射数据处理器
        /// </summary>
        void initializeEmitHandlers() {
            networkSys.addEmitHandler("matched", onMatched);
            networkSys.addEmitHandler("match_progress", onMatchProgress);
            networkSys.addEmitHandler("new_round", onNewRound);
            networkSys.addEmitHandler("prepare_completed", onPrepareCompleted);
            networkSys.addEmitHandler("ques_result", onQuesResult);
            networkSys.addEmitHandler("action_start", onActionStart);
            networkSys.addEmitHandler("round_result", onRoundResult);
            networkSys.addEmitHandler("battle_result", onBattleResult);
        }

        /// <summary>
        /// 初始化操作字典
        /// </summary>
        protected override void initializeOperDict() {
            base.initializeOperDict();
            addOperDict(Oper.EquipItem, EquipItem, NetworkSystem.Interfaces.BattleEquipItem);
            addOperDict(Oper.DequipItem, DequipItem, NetworkSystem.Interfaces.BattleDequipItem);

            addOperDict(Oper.MatchStart, MatchStart, NetworkSystem.Interfaces.BattleMatchStart);
            addOperDict(Oper.MatchCancel, MatchCancel, NetworkSystem.Interfaces.BattleMatchCancel);
            addOperDict(Oper.MatchProgress, MatchProgress, NetworkSystem.Interfaces.BattleMatchProgress, true);
            addOperDict(Oper.PrepareComplete, PrepareComplete, NetworkSystem.Interfaces.BattlePrepareComplete);
            addOperDict(Oper.QuestionAnswer, QuestionAnswer, NetworkSystem.Interfaces.BattleQuestionAnswer);
            addOperDict(Oper.ActionComplete, ActionComplete, NetworkSystem.Interfaces.BattleActionComplete, true);
            addOperDict(Oper.ResultComplete, ResultComplete, NetworkSystem.Interfaces.BattleResultComplete, true);
        }

        /// <summary>
        /// 其他初始化工作
        /// </summary>
        protected override void initializeOthers() {
            base.initializeOthers();
            changeState(State.NotInBattle);
        }

        #endregion

        #region 操作控制

        /// <summary>
        /// 开始匹配
        /// </summary>
        /// <param name="mode">对战模式</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void startMatch(Mode mode,
            UnityAction onSuccess = null, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                changeState(State.Matching);
                onSuccess?.Invoke();
            };

            startMatch((int)mode, _onSuccess, onError);
        }
        public void startMatch(int mode,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
            JsonData data = new JsonData(); data["mode"] = mode;
            sendRequest(Oper.MatchStart, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 取消匹配
        /// </summary>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void cancelMatch(UnityAction onSuccess = null, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                changeState(State.NotInBattle);
                onSuccess?.Invoke();
            };

            cancelMatch(_onSuccess, onError);
        }
        public void cancelMatch(NetworkSystem.RequestObject.SuccessAction onSuccess = null,
            UnityAction onError = null) {
            sendRequest(Oper.MatchCancel, uid: true);
        }

        /// <summary>
        /// 匹配进度
        /// </summary>
        /// <param name="progress">进度</param>
        public void matchProgress(int progress) {
            JsonData data = new JsonData(); data["progress"] = progress;

            battle.self().loadProgress(progress);

            sendRequest(Oper.MatchProgress, data, uid: true);
        }

        /// <summary>
        /// 准备完成
        /// </summary>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void prepareComplete(UnityAction onSuccess = null, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                onSuccess?.Invoke();
            };
            prepareComplete(0, 0, _onSuccess, onError);
        }
        /// <param name="contItem">物品</param>
        public void prepareComplete(BaseContItem contItem,
            UnityAction onSuccess = null, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                onSuccess?.Invoke();
            };
            prepareComplete(contItem.type, contItem.getID(), _onSuccess, onError);
        }
        /// <param name="type">使用物品类型</param>
        /// <param name="contItemId">容器项ID</param>
        public void prepareComplete(int type, int contItemId,
            NetworkSystem.RequestObject.SuccessAction onSuccess = null, UnityAction onError = null) {
            JsonData data = new JsonData();
            // 有设定物品时才传参数
            if (type > 0 && contItemId > 0) {
                data["type"] = type;
                data["contitem_id"] = contItemId;
            }
            sendRequest(Oper.MatchCancel, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 回答问题
        /// </summary>
        /// <param name="selection">选择情况</param>
        /// <param name="timespan">用时</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void questionAnswer(int[] selection, TimeSpan timespan,
            UnityAction onSuccess = null, UnityAction onError = null) {
            questionAnswer(selection, timespan.Seconds, onSuccess, onError);
        }
        /// <summary>
        /// 回答问题
        /// </summary>
        /// <param name="selection">选择情况</param>
        /// <param name="timespan">用时</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void questionAnswer(int[] selection, int timespan,
            UnityAction onSuccess = null, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                onSuccess?.Invoke();
            };

            questionAnswer(selection, timespan, _onSuccess, onError);
        }
        public void questionAnswer(int[] selection, int timespan,
            NetworkSystem.RequestObject.SuccessAction onSuccess = null, UnityAction onError = null) {
            JsonData data = new JsonData();
            data["selection"] = DataLoader.convert(selection);
            data["timespan"] = timespan;
            sendRequest(Oper.QuestionAnswer, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 行动完成
        /// </summary>
        public void actionComplete() {
            sendRequest(Oper.ActionComplete, uid: true);
        }

        /// <summary>
        /// 结算完成
        /// </summary>
        public void resultComplete(int progress) {
            sendRequest(Oper.ResultComplete, uid: true);
        }

        #region 容器操作

        /// <summary>
        /// 艾瑟萌槽装备艾瑟萌
        /// </summary>
        /// <param name="slotItem">对战物资槽项</param>
        /// <param name="packItem">人类背包物品</param>
        /*
        public void equipBattleItem(BattleItemSlotItem slotItem, HumanPackItem packItem) {
            var player = getPlayer();
            var itemSlot = player.slotContainers.battleItemSlot;
            var humanPack = player.packContainers.humanPack;
            itemSlot.setEquip(slotItem, humanPack, packItem);
        }
        /// <param name="sid">科目ID</param>
        public void equipBattleItem(int index, HumanPackItem packItem) {
            var player = getPlayer();
            var itemSlot = player.slotContainers.battleItemSlot;
            var humanPack = player.packContainers.humanPack;
            itemSlot.setEquip(humanPack, packItem);
        }
        */
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        /// <param name="localChange">本地数据是否改变</param>
        public void equipBattleItem(BattleItemSlotItem slotItem, HumanPackItem packItem,
            UnityAction onSuccess = null, UnityAction onError = null, bool localChange = true) {

            var player = getPlayer();
            var itemSlot = player.slotContainers.battleItemSlot;
            var humanPack = player.packContainers.humanPack;
            var _onSuccess = itemSer.slotOperationSuccess(
                humanPack, itemSlot, onSuccess);

            var id = packItem == null ? 0 : packItem.getID();

            equipBattleItem(slotItem.index, id, _onSuccess, onError);
        }
        public void equipBattleItem(int index, HumanPackItem packItem,
            UnityAction onSuccess = null, UnityAction onError = null, bool localChange = true) {

            var player = getPlayer();
            var itemSlot = player.slotContainers.battleItemSlot;
            var humanPack = player.packContainers.humanPack;
            var _onSuccess = itemSer.slotOperationSuccess(
                humanPack, itemSlot, onSuccess);

            var id = packItem == null ? 0 : packItem.getID();

            equipBattleItem(index, id, _onSuccess, onError);
        }
        /// <param name="index">槽索引</param>
        /// <param name="contItemId">容器项ID</param>
        public void equipBattleItem(int index, int contItemId,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
            JsonData data = new JsonData();
            data["index"] = index; data["contitem_id"] = contItemId;
            sendRequest(Oper.EquipItem, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 艾瑟萌装备槽装备
        /// </summary>
        /// <param name="slotItem">对战物资槽项</param>
        /// <param name="index">槽索引</param>
        /*
        public void dequipBattleItem(BattleItemSlotItem slotItem) {
            var player = getPlayer();
            var itemSlot = player.slotContainers.battleItemSlot;
            var humanPack = player.packContainers.humanPack;
            itemSlot.setEquip(slotItem, humanPack, null);
        }
        /// <param name="sid">科目ID</param>
        public void dequipBattleItem(int sid, int index) {
            var player = getPlayer();
            var itemSlot = player.slotContainers.battleItemSlot;
            var humanPack = player.packContainers.humanPack;
            itemSlot.setEquip(index, humanPack, null);
        }
        */
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        /// <param name="localChange">本地数据是否改变</param>
        public void dequipBattleItem(BattleItemSlotItem slotItem,
            UnityAction onSuccess = null, UnityAction onError = null, bool localChange = true) {

            var player = getPlayer();
            var itemSlot = player.slotContainers.battleItemSlot;
            var humanPack = player.packContainers.humanPack;
            var _onSuccess = itemSer.slotOperationSuccess(
                humanPack, itemSlot, onSuccess);

            dequipBattleItem(slotItem.index, _onSuccess, onError);
        }
        public void dequipBattleItem(int index,
            UnityAction onSuccess = null, UnityAction onError = null, bool localChange = true) {

            var player = getPlayer();
            var itemSlot = player.slotContainers.battleItemSlot;
            var humanPack = player.packContainers.humanPack;
            var _onSuccess = itemSer.slotOperationSuccess(
                humanPack, itemSlot, onSuccess);

            dequipBattleItem(index, _onSuccess, onError);
        }
        /// <param name="eid">装备位置ID</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void dequipBattleItem(int index,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
            JsonData data = new JsonData();
            data["index"] = index;
            sendRequest(Oper.DequipItem, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 读取对战物资槽
        /// </summary>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void loadBattleItemSlot(UnityAction onSuccess = null, UnityAction onError = null) {
            Player player = getPlayer();
            itemSer.getSlot(player.slotContainers.battleItemSlot, onSuccess, onError);
        }

        #endregion

        #endregion

        #region 接收发射信息

        #region 状态控制

        /// <summary>
        /// 匹配成功回调
        /// </summary>
        /// <param name="data"></param>
        void onMatched(JsonData data) {
            battle = DataLoader.load<RuntimeBattle>(data);
            changeState(State.Matched);
        }

        /// <summary>
        /// 匹配进度回调
        /// </summary>
        /// <param name="data"></param>
        void onMatchProgress(JsonData data) {
            var pid = DataLoader.load<int>(data, "pid");
            var progress = DataLoader.load<int>(data, "progress");
            battle.loadProgress(pid, progress);
        }

        /// <summary>
        /// 新回合回调
        /// </summary>
        /// <param name="data"></param>
        void onNewRound(JsonData data) {
            battle = DataLoader.load<RuntimeBattle>(data);
            changeState(State.Preparing);
        }

        /// <summary>
        /// 准备完成回调
        /// </summary>
        /// <param name="data"></param>
        void onPrepareCompleted(JsonData data) {
            changeState(State.Questing);
        }

        /// <summary>
        /// 题目结果回调
        /// </summary>
        /// <param name="data"></param>
        void onQuesResult(JsonData data) {
            var pid = DataLoader.load<int>(data, "pid");
            var correct = DataLoader.load<bool>(data, "correct");
            var timespan = DataLoader.load<int>(data, "timespan");
            battle.loadAnswer(pid, correct, timespan);
            changeState(State.Quested);
        }

        /// <summary>
        /// 开始行动回调
        /// </summary>
        /// <param name="data"></param>
        void onActionStart(JsonData data) {
            battle.loadActions(data);
            changeState(State.Acting);
        }

        /// <summary>
        /// 回合结果回调
        /// </summary>
        /// <param name="data"></param>
        void onRoundResult(JsonData data) {
            battle = DataLoader.load<RuntimeBattle>(data);
            changeState(State.Resulting);
        }

        /// <summary>
        /// 对战结果回调
        /// </summary>
        /// <param name="data"></param>
        void onBattleResult(JsonData data) {
            battle.loadResults(data);
            changeState(State.Terminating);
        }

        #endregion

        #region 状态判断

        /// <summary>
        /// 是否匹配完成
        /// </summary>
        /// <returns>返回当前是否匹配完成</returns>
        public bool isMatchingCompleted() {
            return battle.isMatchingCompleted();
        }

        #endregion

        #endregion
    }
}