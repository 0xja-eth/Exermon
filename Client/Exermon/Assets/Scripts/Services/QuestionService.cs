using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using LitJson;

/// <summary>
/// 题目服务
/// </summary>
public class QuestionService : BaseService<QuestionService> {

    /// <summary>
    /// 操作文本设定
    /// </summary>
    const string Get = "查询题目";

    const string GetReports = "查询反馈记录";
    const string PushReport = "提交反馈";
    
    /// <summary>
    /// 题目缓存
    /// </summary>
    public class QuestionCache : BaseData {

        /// <summary>
        /// 缓存的题目
        /// </summary>
        [AutoConvert]
        public List<Question> questions { get; protected set; } = new List<Question>();

        /// <summary>
        /// 添加题目
        /// </summary>
        /// <param name="json">题目数据</param>
        public void addQuestions(Question[] questions) {
            this.questions.AddRange(questions);
        }
    }

    /// <summary>
    /// 业务操作
    /// </summary>
    public enum Oper {
        Get,
        GetReports, PushReport,
    }

    /// <summary>
    /// 缓存题目数据
    /// </summary>
    public QuestionCache questionCache { get; protected set; } = new QuestionCache();

    /// <summary>
    /// 反馈记录
    /// </summary>
    public QuesReport[] quesReports { get; protected set; }

    /// <summary>
    /// 外部系统
    /// </summary>
    StorageSystem storageSys;

    #region 初始化

    /// <summary>
    /// 初始化操作字典
    /// </summary>
    protected override void initializeOperDict() {
        base.initializeOperDict();
        addOperDict(Oper.Get, Get, NetworkSystem.Interfaces.RecordGet);

        addOperDict(Oper.GetReports, GetReports, 
            NetworkSystem.Interfaces.RecordQuestionCollect);
        addOperDict(Oper.PushReport, PushReport, 
            NetworkSystem.Interfaces.RecordQeestionUnwrong);
    }
    
    #endregion

    #region 操作控制

    /// <summary>
    /// 获取记录数据
    /// </summary>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void get(int[] qids, UnityAction onSuccess = null, UnityAction onError = null) {

        NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
            var questions = DataLoader.load<Question[]>(res, "questions");
            questionCache.addQuestions(questions);
            onSuccess?.Invoke();
        };
        JsonData data = new JsonData();
        data["qids"] = DataLoader.convert(qids);
        sendRequest(Oper.Get, data, _onSuccess, onError, uid: true);
    }

    /// <summary>
    /// 获取反馈记录
    /// </summary>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void getReports(UnityAction onSuccess, UnityAction onError = null) {

        NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
            quesReports = DataLoader.load(quesReports, res, "reports");
            onSuccess?.Invoke();
        };

        JsonData data = new JsonData();

        sendRequest(Oper.GetReports, data, _onSuccess, onError, uid: true);
    }

    /// <summary>
    /// 提交反馈
    /// </summary>
    /// <param name="qid">题目ID</param>
    /// <param name="type">类型</param>
    /// <param name="description">描述</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void pushReport(int qid, int type, string description, UnityAction onSuccess, UnityAction onError = null) {

        NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
            onSuccess?.Invoke();
        };

        JsonData data = new JsonData();
        data["qid"] = qid; data["type"] = type;
        data["description"] = description;

        sendRequest(Oper.PushReport, data, _onSuccess, onError, uid: true);
    }

    #endregion

    #region 题目操作

    /// <summary>
    /// 读取题目（先判断缓存）
    /// </summary>
    /// <param name="qids">题目ID集</param>
    /// <param name="onSuccess">成功回调</param>
    /// <param name="onError">失败回调</param>
    public void loadQuestions(int[] qids, UnityAction onSuccess = null, UnityAction onError = null) {
        var cnt = qids.Length;
        var reqIds = new List<int>(); // 需要请求的题目ID数组
        for (int i = 0; i < cnt; ++i)
            // 如果没有缓存
            if (!isQuestionCached(qids[i])) reqIds.Add(qids[i]);

        if (reqIds.Count > 0) // 如果需要请求
            get(reqIds.ToArray(), onSuccess, onError);
        else onSuccess?.Invoke();
    }

    /// <summary>
    /// 题目是否已缓存
    /// </summary>
    /// <param name="qid">题目ID</param>
    /// <returns>是否缓存</returns>
    public bool isQuestionCached(int qid) {
        return questionCache.questions.Exists(q => q.getID() == qid);
    }

    /// <summary>
    /// 获取单个题目（需确保已经缓存）
    /// </summary>
    /// <param name="qid">题目ID</param>
    /// <returns>题目实例</returns>
    public Question getQuestion(int qid) {
        return questionCache.questions.Find(q => q.getID() == qid);
    }

    #endregion

}