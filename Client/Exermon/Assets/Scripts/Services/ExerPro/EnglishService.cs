using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        /// 听力题目类型数值
        /// </summary>
        const int ListeningQuestionType = 1;
        const int ReadingQuestionType = 2;
        const int CorrectionQuestionType = 3;

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
                //    return readingQuestions as List<T>;                if (typeof(T) == typeof(CorrectionQuestion))
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
        /// 记录数据
        /// </summary>
        public List<WordRecord> wordRecords { get; protected set; }

        /// <summary>
        /// 缓存题目数据
        /// </summary>
        public QuestionCache questionCache { get; protected set; } = new QuestionCache();
        
        #region 初始化

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
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
            //    return ReadingQuestionType;
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
    }
}