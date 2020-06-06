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

using GameModule.Services;

using ExerPro.EnglishModule.Data;

/// <summary>
/// 艾瑟萌特训-英语模块服务
/// </summary>
namespace ExerPro.EnglishModule.Services {

    /// <summary>
    /// 战斗服务
    /// </summary>
    public class BattleService : BaseService<BattleService> {

        /// <summary>
        /// 常量定义
        /// </summary>
        public const int NormalWordCount = 8;
        public const int BonusWordCount = 2;

        /// <summary>
        /// 特训状态
        /// </summary>
        public enum State {

            NotInBattle = 0, // 不在战斗中
            Answering = 1, // 回答单词阶段
            Drawing = 2, // 抽牌阶段
            Playing = 3, // 出牌阶段
            Discarding = 4, // 弃牌阶段
            Enemy = 5, // 敌人行动阶段
            RoundEnd = 6, // 回合结束阶段
            Result = 7, // 战斗结算阶段

        }

        /// <summary>
        /// 外部系统设置
        /// </summary>
        EnglishService engSer;

        /// <summary>
        /// 内部变量定义
        /// </summary>
        ExerProRecord record;
        ExerProCardGroup cardGroup;

        List<RuntimeEnemy> enemies; // 敌人

        int corrCnt = 0, bonusCnt = 0;
        bool bonus = false;

        #region 初始化

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            engSer = EnglishService.get();
            record = engSer.record;
            cardGroup = record.actor.cardGroup;
        }

        /// <summary>
        /// 初始化状态字典
        /// </summary>
        protected override void initializeStateDict() {
            base.initializeStateDict();
            addStateDict(State.NotInBattle);
            addStateDict(State.Answering);
            addStateDict(State.Drawing, updateDrawing);
            addStateDict(State.Playing);
            addStateDict(State.Discarding);
            addStateDict(State.Enemy);
            addStateDict(State.RoundEnd, updateRoundEnd);
            addStateDict(State.Result);
        }

        #endregion

        #region 更新控制

        /// <summary>
        /// 更新抽牌
        /// </summary>
        void updateDrawing() {
            // 抽牌答对次数 + 额外次数
            for (int i = 0; i < corrCnt + bonusCnt; ++i)
                cardGroup.drawCard();
            changeState(State.Playing);
        }

        /// <summary>
        /// 更新出牌
        /// </summary>
        void updatePlaying() {

        }

        /// <summary>
        /// 更新弃牌
        /// </summary>
        void updateDiscarding() {

        }

        /// <summary>
        /// 更新敌人
        /// </summary>
        void updateEnemy() {

        }

        /// <summary>
        /// 更新回合结束
        /// </summary>
        void updateRoundEnd() {
            bonus = false;
            corrCnt = bonusCnt = 0;
            changeState(State.Answering);
        }

        /// <summary>
        /// 更新结果
        /// </summary>
        void updateResult() {

        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 开始战斗
        /// </summary>
        /// <param name="node"></param>
        public void start(ExerProMapNode.Type type) {
            generateEnemies(type);
            cardGroup.onBattleStart();
            changeState(State.Drawing);
        }

        /// <summary>
        /// 生成敌人
        /// </summary>
        /// <param name="type"></param>
        void generateEnemies(ExerProMapNode.Type type) {
            var actor = record.actor;
            var stage = record.stage();
            enemies = CalcService.BattleEnemiesGenerator.
                generate(actor, stage, type);
        }

        /// <summary>
        /// 结束
        /// </summary>
        public void terminate() {
            cardGroup.onBattleEnd();
        }

        #region 答题/抽卡控制

        /// <summary>
        /// 当前题目
        /// </summary>
        /// <returns></returns>
        public Word currentWord() {
            return record.nextWord();
        }

        /// <summary>
        /// 当前抽卡
        /// </summary>
        /// <returns></returns>
        public ExerProPackCard[] drawnCards() {
            return cardGroup.handGroup.getDrawnCards();
        }

        /// <summary>
        /// 回答
        /// </summary>
        /// <param name="chinese"></param>
        public void answer(string chinese, UnityAction<bool> callback) {
            var word = currentWord();
            engSer.answerWord(word, chinese, (res) => 
                onAnswerSuccess(res, callback));
        }

        /// <summary>
        /// 回答成功回调
        /// </summary>
        /// <param name="correct">回答是否错误</param>
        void onAnswerSuccess(bool correct, UnityAction<bool> callback) {
            if (correct) onAnswerCorrect();
            else onAnswerWrong();

            callback?.Invoke(correct);
        }

        /// <summary>
        /// 回答正确回调
        /// </summary>
        void onAnswerCorrect() {
            if (bonus) {
                if (++bonusCnt >= BonusWordCount)
                    changeState(State.Drawing);
            } else if (++corrCnt >= NormalWordCount)
                bonus = true;
        }
        
        /// <summary>
        /// 回答错误回调
        /// </summary>
        void onAnswerWrong() {
            bonusCnt = 0;
            if (bonus) changeState(State.Drawing);
            else bonus = true;
        }

        #endregion

        #region 出牌控制

        /// <summary>
        /// 
        /// </summary>
        /// <param name="card"></param>
        /// <param name="targets"></param>
        public void play(ExerProCard card, List<RuntimeEnemy> targets) {

        }

        #endregion

        #endregion

    }
}