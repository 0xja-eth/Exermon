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
        static readonly ExerProMapNode.Type[] RandomNodeTypes = {
            ExerProMapNode.Type.Story, ExerProMapNode.Type.Rest,
            ExerProMapNode.Type.Treasure, ExerProMapNode.Type.Shop,
            ExerProMapNode.Type.Enemy
        };

        /// <summary>
        /// 操作文本设定
        /// </summary>
        const string QuestionGenerate = "生成题目";
        const string QuestionGet = "获取题目";

        const string WordGenerate = "生成单词";
        const string WordAnswer = "提交答案";
        const string WordGet = "获取单词";
        const string WordQuery = "查询状态";

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
            WordGenerate, WordAnswer, WordGet, WordQuery,
            WordRecordGet,
        }

        /// <summary>
        /// 特训状态
        /// </summary>
        public enum State {

            NotInExerPro = 0, // 不在特训中
            Unstarted = 1, // 未开始（需要选择一个起始据点）
            Moving = 2, // 角色移动中
            //AfterMoved = 3, // 角色移动结束，准备进入据点事件
            InNode = 3, // 在据点内
            AfterNode = 4, // 据点事件结束，保存中
            Idle = 5, // 待机状态（选择下一个据点）

            /*
            RestNode = 10, // 休息据点
            TreasureNode = 11, // 宝藏据点
            ShopNode = 12, // 宝藏据点
            EnemyNode = 13, // 敌人据点
            EliteNode = 14, // 精英据点
            UnknownNode = 15, // 未知据点
            BossNode = 16, // BOSS据点
            StoryNode = 20, // 剧情据点
            */
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
            //addStateDict(State.AfterMoved, updateAfterMoved);
            addStateDict(State.InNode, updateNode);
            addStateDict(State.AfterNode, updateAfterNode);
            addStateDict(State.Idle);

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
            addOperDict(Oper.WordQuery, WordQuery,
                NetworkSystem.Interfaces.EngProWordQuery);

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
        public void generateQuestions<T>(UnityAction<T[]> onSuccess, 
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
        public void getQuestions<T>(int[] qids, UnityAction<T[]> onSuccess, 
            UnityAction onError = null) where T : BaseData, new() {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                var questions = DataLoader.load<T[]>(res, "questions");
                questionCache.addQuestions(questions);
                onSuccess?.Invoke(questions);
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
        public void generateWords(UnityAction<Word[]> onSuccess, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                var words = DataLoader.load<Word[]>(res, "words");
                questionCache.addQuestions(words);
                onSuccess?.Invoke(words);
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
        public void getWords(int[] wids, UnityAction<Word[]> onSuccess, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                var words = DataLoader.load<Word[]>(res, "words");
                questionCache.addQuestions(words);
                onSuccess?.Invoke(words);
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
        /// <param name="onSuccess">成功回调</param>
        /// <param name="onError">失败回调</param>
        public void queryWords(UnityAction onSuccess, UnityAction onError = null) {

            NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
                record.load(res); onSuccess?.Invoke();
            };

            queryWords(_onSuccess, onError);
        }
        public void queryWords(NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {

            JsonData data = new JsonData();

            sendRequest(Oper.WordQuery, data, onSuccess, onError, uid: true);
        }

        /// <summary>
        /// 获取单词记录
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
        public void loadQuestion<T>(int qid, UnityAction<T> onSuccess = null, 
            UnityAction onError = null) where T : BaseData, new() {
            UnityAction<T[]> _onSuccess = (res) => onSuccess.Invoke(res[0]);
            loadQuestions<T>(new int[] { qid }, _onSuccess, onError);
        }
        /// <param name="qids">题目ID集</param>
        public void loadQuestions<T>(int[] qids, UnityAction<T[]> onSuccess = null, 
            UnityAction onError = null) where T : BaseData, new() {
            var cnt = qids.Length;
            var reqIds = new List<int>(); // 需要请求的题目ID数组
            var questions = new List<T>(); // 已缓存的题目数组

            for (int i = 0; i < cnt; ++i) {
                var question = getQuestion<T>(qids[i]);
                if (question == null) reqIds.Add(qids[i]);
                else questions.Add(question);
            }

            if (reqIds.Count > 0) // 如果需要请求
                getQuestions<T>(reqIds.ToArray(), onSuccess, onError);
            else onSuccess?.Invoke(questions.ToArray());
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
        /*
        /// <summary>
        /// 更新移动结束
        /// </summary>
        void updateAfterMoved() {
            changeState(State.InNode);
        }
        */
        /// <summary>
        /// 更新事件结束
        /// </summary>
        void updateAfterNode() {
            save(); changeState(State.Idle);
        }
        
        /// <summary>
        /// 更新据点
        /// </summary>
        void updateNode() {
            Debug.Log("updateNode");
            if (isStateChanged()) {
                Debug.Log("isStateChanged");
                var node = record.currentNode();
                if (node == null) exitNode(false);
                else switchNode(node.typeId);
            }
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
            return state == (int)State.InNode && isBattleNode();
        }

        /// <summary>
        /// 是否空闲
        /// </summary>
        /// <returns></returns>
        public bool isIdle() {
            return state == (int)State.Unstarted || state == (int)State.Idle;
        }

        /// <summary>
        /// 当前据点类型
        /// </summary>
        /// <returns></returns>
        public ExerProMapNode.Type currentNodeType() {
            return record.currentNode().typeEnum();
        }

        /// <summary>
        /// 是否为战斗据点
        /// </summary>
        /// <returns></returns>
        public bool isBattleNode() {
            var type = currentNodeType();
            return type == ExerProMapNode.Type.Enemy ||
                type == ExerProMapNode.Type.Elite ||
                type == ExerProMapNode.Type.Boss;
        }

        #endregion

        #region 据点入口

        /// <summary>
        /// 切换据点
        /// </summary>
        /// <param name="type">类型</param>
        void switchNode(int type) {
            switchNode((ExerProMapNode.Type)type);
        }
        void switchNode(ExerProMapNode.Type type) {
            Debug.Log("switchNode: " + type);
            switch (type) {
                case ExerProMapNode.Type.Rest: onRestNode(); break;
                case ExerProMapNode.Type.Treasure: onTreasureNode(); break;
                case ExerProMapNode.Type.Shop: onShopNode(); break;
                case ExerProMapNode.Type.Story: onStoryNode(); break;
                case ExerProMapNode.Type.Enemy: onEnemyNode(); break;
                case ExerProMapNode.Type.Elite: onEliteNode(); break;
                case ExerProMapNode.Type.Unknown: onUnknownNode(); break;
                case ExerProMapNode.Type.Boss: onBossNode(); break;
            }
            // 将下面这句删掉来测试具体的据点场景
            exitNode(false);
        }

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
        /// 剧情据点
        /// </summary>
        void onStoryNode() { }

        /// <summary>
        /// 敌人据点
        /// </summary>
        void onEnemyNode() { }

        /// <summary>
        /// 精英据点
        /// </summary>
        void onEliteNode() { }

        /// <summary>
        /// Boss据点
        /// </summary>
        void onBossNode() { }

        /// <summary>
        /// 未知据点
        /// </summary>
        void onUnknownNode() {
            switchNode(randomNode());
        }

        /// <summary>
        /// 随机据点生成
        /// </summary>
        /// <returns></returns>
        public ExerProMapNode.Type randomNode() {
            var rates = RandomNodeRates;
            var states = RandomNodeTypes;
            var rateList = new List<int>();

            // 将比率填充为列表，然后从中抽取一个
            for (int i = 0; i < rates.Length; ++i)
                for (int j = 0; j < rates[i]; ++j) rateList.Add(i);

            var index = Random.Range(0, rateList.Count);
            return states[rateList[index]];
        }

        /// <summary>
        /// 退出据点（据点退出时候需要调用此函数！）
        /// </summary>
        public void exitNode(bool pop = true) {
            if (pop) sceneSys.popScene();
            changeState(State.Idle);
        }

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
                changeState(State.InNode);
        }
        /*
        /// <summary>
        /// 处理当前据点
        /// </summary>
        void processCurrentNode() {
            changeState(State.InNode);
        }
        */
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