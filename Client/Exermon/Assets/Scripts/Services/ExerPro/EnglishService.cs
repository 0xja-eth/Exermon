using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Random = UnityEngine.Random;

using LitJson;

using Core.Data;
using Core.Data.Loaders;
using Core.Systems;
using Core.Services;

using ExerPro.EnglishModule.Data;

/// <summary>
/// 记录模块服务
/// </summary>
namespace ExerPro.EnglishModule.Services {

    /// <summary>
    /// 记录服务
    /// </summary>
    public class EnglishService : BaseService<EnglishService> {

        /// <summary>
        /// 题目类型数值
        /// </summary>
        const int ListeningQuestionType = 1;
        const int InfinitiveQuestionType = 2;
        const int CorrectionQuestionType = 3;

        /// <summary>
        /// 随机据点几率比例（剧情:休息:藏宝:商人:敌人）
        /// </summary>
        static readonly int[] RandomNodeRates = { 3, 2, 1, 2, 2 };
        static readonly State[] RandomNodeStates = {
            State.StoryNode, State.RestNode, State.TreasureNode,
            State.ShopNode, State.EnemyNode
        };

        /// <summary>
        /// 操作文本设定
        /// </summary>
        const string QuestionGenerate = "生成题目";
        const string QuestionGet = "获取题目";

        const string WordGenerate = "生成单词";
        const string WordAnswer = "提交答案";
        const string WordGet = "获取单词";

        const string WordRecordGet = "查询单词记录";

        /// <summary>
        /// 题目缓存
        /// </summary>
        public class QuestionCache : BaseData {

            /// <summary>
            /// 缓存的题目
            /// </summary>
            [AutoConvert]
            public List<ListeningQuestion> listeningQuestions { get; protected set; } = new List<ListeningQuestion>();
            //[AutoConvert]
            //public List<ReadingQuestion> readingQuestions { get; protected set; } = new List<ReadingQuestion>();
            [AutoConvert]
            public List<CorrectionQuestion> correctionQuestions { get; protected set; } = new List<CorrectionQuestion>();
            [AutoConvert]
            public List<Word> words { get; protected set; } = new List<Word>();

            /// <summary>
            /// 获取缓存列表
            /// </summary>
            /// <typeparam name="T">类型</typeparam>
            /// <returns>返回的缓存列表</returns>
            public List<T> getCacheList<T>() {
                if (typeof(T) == typeof(ListeningQuestion))
                    return listeningQuestions as List<T>;
                //if (typeof(T) == typeof(ReadingQuestion))
                //    return readingQuestions as List<T>;
                if (typeof(T) == typeof(CorrectionQuestion))
                    return correctionQuestions as List<T>;
                if (typeof(T) == typeof(Word))
                    return words as List<T>;
                return null;
            }

            /// <summary>
            /// 添加题目
            /// </summary>
            /// <param name="json">题目数据</param>
            public void addQuestions<T>(T[] questions) {
                getCacheList<T>().AddRange(questions);
            }

            /// <summary>
            /// 读取自定义属性
            /// </summary>
            /// <param name="json">数据</param>
            protected override void loadCustomAttributes(JsonData json) {
                base.loadCustomAttributes(json);

                listeningQuestions = listeningQuestions ?? new List<ListeningQuestion>();
                //readingQuestions = readingQuestions ?? new List<ReadingQuestion>();
                correctionQuestions = correctionQuestions ?? new List<CorrectionQuestion>();
                words = words ?? new List<Word>();
            }
        }

        /// <summary>
        /// 业务操作
        /// </summary>
        public enum Oper {
            QuestionGenerate, QuestionGet,
            WordGenerate, WordAnswer, WordGet,
            WordRecordGet,
        }

        /// <summary>
        /// 特训状态
        /// </summary>
        public enum State {

            NotInExerPro = 0, // 不在特训中
            Unstarted = 1, // 未开始（需要选择一个起始据点）
            Moving = 2, // 角色移动中
            AfterMoved = 3, // 角色移动结束，准备进入据点事件
            AfterEvent = 4, // 据点事件结束，保存中
            Idle = 5, // 待机状态（选择下一个据点）

            RestNode = 10, // 休息据点
            TreasureNode = 11, // 宝藏据点
            ShopNode = 12, // 宝藏据点
            EnemyNode = 13, // 敌人据点
            EliteNode = 14, // 精英据点
            UnknownNode = 15, // 未知据点
            BossNode = 16, // BOSS据点
            StoryNode = 20, // 剧情据点
        }

        /// <summary>
        /// 记录数据
        /// </summary>
        public List<WordRecord> wordRecords { get; protected set; }

        /// <summary>
        /// 缓存题目数据
        /// </summary>
        public QuestionCache questionCache { get; protected set; } = new QuestionCache();

        /// <summary>
        /// 关卡记录
        /// </summary>
        public MapStageRecord record { get; protected set; } = new MapStageRecord();

        /// <summary>
        /// 外部系统设置
        /// </summary>
        StorageSystem storageSys;
        SceneSystem sceneSys;

        #region 初始化

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            storageSys = StorageSystem.get();
            sceneSys = SceneSystem.get();
        }

        /// <summary>
        /// 初始化状态字典
        /// </summary>
        protected override void initializeStateDict() {
            base.initializeStateDict();
            addStateDict(State.NotInExerPro);
            addStateDict(State.Unstarted);

            addStateDict(State.Moving);
            addStateDict(State.AfterMoved, updateAfterMoved);
            addStateDict(State.AfterEvent, updateAfterEvent);
            addStateDict(State.Idle);

            addStateDict(State.RestNode, updateNode);
            addStateDict(State.TreasureNode, updateNode);
            addStateDict(State.ShopNode, updateNode);
            addStateDict(State.EnemyNode, updateNode);
            addStateDict(State.EliteNode, updateNode);
            addStateDict(State.UnknownNode, updateUnknownNode);
            addStateDict(State.BossNode, updateNode);
            addStateDict(State.StoryNode, updateNode);
        }

        /// <summary>
        /// 初始化操作字典
        /// </summary>
        protected override void initializeOperDict() {
            base.initializeOperDict();

            addOperDict(Oper.QuestionGenerate, QuestionGenerate,
                NetworkSystem.Interfaces.EngProQuestionGenerate);
            addOperDict(Oper.QuestionGet, QuestionGet,
                NetworkSystem.Interfaces.EngProQuestionGet);
            
            addOperDict(Oper.WordGenerate, WordGenerate,
                NetworkSystem.Interfaces.EngProWordGenerate);
            addOperDict(Oper.WordAnswer, WordAnswer,
                NetworkSystem.Interfaces.EngProWordAnswer);
            addOperDict(Oper.WordGet, WordGet,
                NetworkSystem.Interfaces.EngProWordGet);
            addOperDict(Oper.WordRecordGet, WordRecordGet,
                NetworkSystem.Interfaces.EngProWordRecordGet);

        }

        #endregion

        #region 操作控制

        #region 题目记录操作

        /// <summary>
        /// 获取题目类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        int getQuestionType<T>() where T : BaseData, new() {
            if (typeof(T) == typeof(ListeningQuestion))
                return ListeningQuestionType;
            //if (typeof(T) == typeof(ReadingQuestion))
            //    return ReadingQuestionType;
            if (typeof(T) == typeof(CorrectionQuestion))
                return CorrectionQuestionType;
            return 0;
        }

        /// <summary>
        /// 获取记录数据
        /// </summary>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void generateQuestions<T>(UnityAction onSuccess, 
            UnityAction onError = null) where T : BaseData, new() {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                var ids = DataLoader.load<int[]>(res, "qids");
                loadQuestions<T>(ids, onSuccess, onError);
            };

            generateQuestion(getQuestionType<T>(), _onSuccess, onError);
        }
        /// <param name="qids">题目类型</param>
        public void generateQuestion(int type, 
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError) {

            JsonData data = new JsonData(); data["type"] = type;
            sendRequest(Oper.QuestionGenerate, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 获取记录数据
        /// </summary>
        /// <param name="qids">题目ID集</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void getQuestions<T>(int[] qids, UnityAction onSuccess, 
            UnityAction onError = null) where T : BaseData, new() {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                var questions = DataLoader.load<T[]>(res, "questions");
                questionCache.addQuestions(questions);
                onSuccess?.Invoke();
            };

            getQuestions(qids, getQuestionType<T>(), _onSuccess, onError);
        }
        public void getQuestions(int[] qids, int type,
            NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {

            JsonData data = new JsonData();
            data["qids"] = DataLoader.convert(qids); data["type"] = type;

            sendRequest(Oper.QuestionGet, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 生成单词数据
        /// </summary>
        /// <param name="wids">单词ID集</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void generateWords(UnityAction onSuccess, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                var words = DataLoader.load<Word[]>(res, "words");
                questionCache.addQuestions(words);
                onSuccess?.Invoke();
            };

            generateWords(_onSuccess, onError);
        }
        public void generateWords(NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {

            JsonData data = new JsonData();
            sendRequest(Oper.WordGenerate, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 获取单词数据
        /// </summary>
        /// <param name="wids">单词ID集</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void getWords(int[] wids, UnityAction onSuccess, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                var words = DataLoader.load<Word[]>(res, "words");
                questionCache.addQuestions(words);
                onSuccess?.Invoke();
            };

            getWords(wids, _onSuccess, onError);
        }
        public void getWords(int[] wids, NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {

            JsonData data = new JsonData();
            data["wids"] = DataLoader.convert(wids);

            sendRequest(Oper.WordGet, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 回答单词
        /// </summary>
        /// <param name="word">单词对象</param>
        /// <param name="res">结果是否正确</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void answerWord(Word word, bool res, UnityAction onSuccess, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (_) => {
                onSuccess?.Invoke();
            };

            answerWord(word.id, res, _onSuccess, onError);
        }
        /// <param name="wid">题目ID</param>
        public void answerWord(int wid, bool res, NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {

            JsonData data = new JsonData();
            data["wid"] = wid; data["result"] = res;

            sendRequest(Oper.WordAnswer, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 获取单词数据
        /// </summary>
        /// <param name="wids">单词ID集</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void getWordRecords(int[] wids, UnityAction onSuccess, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                wordRecords = DataLoader.load(wordRecords, res, "records");
                onSuccess?.Invoke();
            };

            getWords(wids, _onSuccess, onError);
        }
        public void getWordRecords(int[] wids, NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {

            JsonData data = new JsonData();
            sendRequest(Oper.WordRecordGet, data, onSuccess, onError, uid: true);
        }

        #endregion

        #region 题目操作

        /// <summary>
        /// 读取题目（先判断缓存）
        /// </summary>
        /// <param name="qid">题目ID</param>
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void loadQuestion<T>(int qid, UnityAction onSuccess = null, 
            UnityAction onError = null) where T : BaseData, new() {
            loadQuestions<T>(new int[] { qid }, onSuccess, onError);
        }
        /// <param name="qids">题目ID集</param>
        public void loadQuestions<T>(int[] qids, UnityAction onSuccess = null, 
            UnityAction onError = null) where T : BaseData, new() {
            var cnt = qids.Length;
            var reqIds = new List<int>(); // 需要请求的题目ID数组
            for (int i = 0; i < cnt; ++i)
                // 如果没有缓存
                if (!isQuestionCached<T>(qids[i])) reqIds.Add(qids[i]);

            if (reqIds.Count > 0) // 如果需要请求
                getQuestions<T>(reqIds.ToArray(), onSuccess, onError);
            else onSuccess?.Invoke();
        }

        /// <summary>
        /// 题目是否已缓存
        /// </summary>
        /// <param name="qid">题目ID</param>
        /// <returns>是否缓存</returns>
        public bool isQuestionCached<T>(int qid) where T: BaseData, new() {
            return questionCache.getCacheList<T>().Exists(q => q.id == qid);
        }
        
        /// <summary>
        /// 获取单个题目（需确保已经缓存）
        /// </summary>
        /// <param name="qid">题目ID</param>
        /// <returns>题目实例</returns>
        public T getQuestion<T>(int qid) where T : BaseData, new() {
            return questionCache.getCacheList<T>().Find(q => q.id == qid);
        }

        #endregion

        #endregion

        #region 更新控制

        /// <summary>
        /// 更新移动结束
        /// </summary>
        void updateAfterMoved() {
            processCurrentNode();
        }

        /// <summary>
        /// 更新事件结束
        /// </summary>
        void updateAfterEvent() {
            save(); changeState(State.Idle);
        }

        /// <summary>
        /// 更新据点
        /// </summary>
        void updateNode() {
            if (isStateChanged()) {
                var node = record.currentNode();
                if (node == null) return;
                Debug.Log("updateNode: isStateChanged: " + node.type);

                switch ((ExerProMapNode.Type)node.type) {
                    case ExerProMapNode.Type.Rest: onRestNode(); break;
                    case ExerProMapNode.Type.Treasure: onTreasureNode(); break;
                    case ExerProMapNode.Type.Shop: onShopNode(); break;
                    case ExerProMapNode.Type.Enemy: onEnemyNode(); break;
                    case ExerProMapNode.Type.Elite: onEliteNode(); break;
                    case ExerProMapNode.Type.Unknown: onUnknownNode(); break;
                    case ExerProMapNode.Type.Boss: onBossNode(); break;
                }
            }
        }

        /// <summary>
        /// 更新未知据点
        /// </summary>
        void updateUnknownNode() {
            changeState(randomNode());
        }

        #endregion

        #region 游戏控制

        #region 状态判断

        /// <summary>
        /// 是否已进入
        /// </summary>
        /// <returns></returns>
        public bool isEntered() {
            return state != (int)State.NotInExerPro;
        }

        /// <summary>
        /// 是否已开始
        /// </summary>
        /// <returns></returns>
        public bool isStarted() {
            return state != (int)State.NotInExerPro && state != (int)State.Unstarted;
        }

        /// <summary>
        /// 是否处于战斗中
        /// </summary>
        /// <returns></returns>
        public bool isInBattle() {
            return state == (int)State.EnemyNode || 
                state == (int)State.EliteNode || state == (int)State.BossNode;
        }

        /// <summary>
        /// 是否空闲
        /// </summary>
        /// <returns></returns>
        public bool isIdle() {
            return state == (int)State.Unstarted || state == (int)State.Idle;
        }

        /// <summary>
        /// 随机据点生成
        /// </summary>
        /// <returns></returns>
        public State randomNode() {
            var rates = RandomNodeRates;
            var states = RandomNodeStates;
            var rateList = new List<int>();

            // 将比率填充为列表，然后从中抽取一个
            for (int i = 0; i < rates.Length; ++i)
                for (int j = 0; j < rates[i]; ++j) rateList.Add(i);

            var index = Random.Range(0, rateList.Count);
            return states[rateList[index]];
        }

        #endregion

        #region 据点入口

        /// <summary>
        /// 休息据点
        /// </summary>
        void onRestNode() { }

        /// <summary>
        /// 藏宝据点
        /// </summary>
        void onTreasureNode() { }

        /// <summary>
        /// 商人据点
        /// </summary>
        void onShopNode() { }

        /// <summary>
        /// 敌人据点
        /// </summary>
        void onEnemyNode() { }

        /// <summary>
        /// 精英据点
        /// </summary>
        void onEliteNode() { }

        /// <summary>
        /// 未知据点
        /// </summary>
        void onUnknownNode() { }

        /// <summary>
        /// Boss据点
        /// </summary>
        void onBossNode() { }

        #endregion

        #region 游戏进程

        /// <summary>
        /// 开始游戏
        /// </summary>
        public void start(int mapId) {
            // 如果未开始或者开启新地图，覆盖原有的记录
            if (record.started == false || record.mapId != mapId) {
                changeState(State.Unstarted);
                record.setup(mapId, 1, true);
            } else changeState(State.Idle);

            sceneSys.pushScene(SceneSystem.Scene.EnglishProMapScene);
        }

        /// <summary>
        /// 开始移动
        /// </summary>
        public void startMove(int nid, bool force = false) {
            if (!isIdle()) return;
            changeState(State.Moving);
            record.moveNext(nid, force);
        }

        /// <summary>
        /// 结束移动
        /// </summary>
        public void terminateMove() {
            if (state == (int)State.Moving)
                changeState(State.AfterMoved);
        }

        /// <summary>
        /// 处理当前据点
        /// </summary>
        void processCurrentNode() {
            var node = record.currentNode();
            if (node == null) return;

            // 自动根据序号来切换状态
            changeState((int)State.RestNode + node.type);
        }

        /// <summary>
        /// 保存进度
        /// </summary>
        public void save() {
            storageSys.saveItem(StorageSystem.EngCacheDataFilename);
            storageSys.saveItem(StorageSystem.EngRecordFilename);
        }

        /// <summary>
        /// 结束
        /// </summary>
        public void terminate() {
            changeState(State.NotInExerPro);
            save(); sceneSys.popScene();
        }

        #endregion

        #endregion
    }
}