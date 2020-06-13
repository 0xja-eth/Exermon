﻿using System;
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
        /// 战斗结果
        /// </summary>
        public enum Result {
            None = 0, Win = 1, Lose = 2
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

        List<RuntimeEnemy> _enemies; // 敌人
        int curEnemyIndex = 0; // 当前敌人索引

        int corrCnt = 0, bonusCnt = 0;
        bool bonus = false;

        int round = 0;

        Result result = Result.None;

        #region 初始化

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            engSer = EnglishService.get();
            record = engSer.record;
            cardGroup = actor().cardGroup;
        }

        /// <summary>
        /// 初始化状态字典
        /// </summary>
        protected override void initializeStateDict() {
            base.initializeStateDict();
            addStateDict(State.NotInBattle);
            addStateDict(State.Answering);
            addStateDict(State.Drawing, updateDrawing);
            addStateDict(State.Playing, updatePlaying);
            addStateDict(State.Discarding, updateDiscarding);
            addStateDict(State.Enemy, updateEnemy);
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
            if (isStateChanged())
                foreach (var enemy in _enemies)
                    calcEnemyNext(enemy);
        }

        /// <summary>
        /// 更新弃牌
        /// </summary>
        void updateDiscarding() {
            actor().cardGroup.onRoundEnd();
            changeState(State.Enemy);
        }

        /// <summary>
        /// 更新敌人
        /// </summary>
        void updateEnemy() {
            if (processEnemiesAction())
                changeState(State.RoundEnd);
        }

        /// <summary>
        /// 更新回合结束
        /// </summary>
        void updateRoundEnd() {
            onRoundEnd();
            changeState(State.Answering);
        }
        /*
        /// <summary>
        /// 更新结果
        /// </summary>
        void updateResult() {
            actor().onBattleEnd();
            changeState(State.NotInBattle);
        }
        */
        #endregion

        #region 流程控制

        /// <summary>
        /// 开始战斗
        /// </summary>
        /// <param name="node"></param>
        public void start(ExerProMapNode.Type type) {
            setup(type);
            changeState(State.Drawing);
        }

        /// <summary>
        /// 开始配置
        /// </summary>
        void setup(ExerProMapNode.Type type) {
            result = Result.None;
            generateEnemies(type);
            cardGroup.onBattleStart();
        }

        /// <summary>
        /// 生成敌人
        /// </summary>
        /// <param name="type"></param>
        void generateEnemies(ExerProMapNode.Type type) {
            var stage = record.stage();
            _enemies = CalcService.BattleEnemiesGenerator.
                generate(actor(), stage, type);
        }

		/// <summary>
		/// 结束出牌阶段
		/// </summary>
		public void pass() {
			changeState(State.Discarding);
		}

        /// <summary>
        /// 结束
        /// </summary>
        public void terminate() {
            cardGroup.onBattleEnd();
        }

		#region 数据获取

		/// <summary>
		/// 获取玩家
		/// </summary>
		/// <returns></returns>
		public RuntimeActor actor() {
			return record.actor;
		}

		/// <summary>
		/// 获取敌人
		/// </summary>
		/// <returns></returns>
		public List<RuntimeBattler> enemies() {
			var res = new List<RuntimeBattler>();
			foreach (var enemy in _enemies) res.Add(enemy);
			return res;
		}

		/// <summary>
		/// 获取所有战斗者
		/// </summary>
		/// <returns></returns>
		public List<RuntimeBattler> battlers() {
			var res = new List<RuntimeBattler>();

			res.Add(actor());
			foreach (var enemy in _enemies) res.Add(enemy);
			return res;
		}

		#endregion

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
		/// 使用
		/// </summary>
		/// <param name="effect">效果</param>
		/// <param name="targets">目标</param>
		public void use(ExerProEffectData[] effects, List<RuntimeBattler> targets) {
			var actor = this.actor();
			foreach (var target in targets) {
				actor.addAction(new RuntimeAction(
					actor, target, effects));
			}
		}

		#endregion

		#region 敌人控制

		/// <summary>
		/// 计算敌人下一步行动
		/// </summary>
		void calcEnemyNext(RuntimeEnemy enemy) {
            enemy.calcNext(round, actor());
        }

        /// <summary>
        /// 处理敌人行动
        /// </summary>
        /// <returns>敌人是否全部行动完毕</returns>
        bool processEnemiesAction() {
            var enemy = _enemies[curEnemyIndex];
            if (enemy.updateAction()) curEnemyIndex++;
            return curEnemyIndex >= _enemies.Count;
        }

        #endregion

        #region 回合控制

        /// <summary>
        /// 回合结束回调
        /// </summary>
        void onRoundEnd() {
            resetStates();
            battlersRoundEnd();
            judgeResult();
        }

        /// <summary>
        /// 重置部分状态
        /// </summary>
        void resetStates() {
            bonus = false;
            corrCnt = bonusCnt = 0;
            curEnemyIndex = 0;
        }

        /// <summary>
        /// 处理战斗者回合结束
        /// </summary>
        void battlersRoundEnd() {
            actor().onRoundEnd();
            foreach (var enemy in _enemies)
                enemy.onRoundEnd();
        }

        /// <summary>
        /// 判断结果
        /// </summary>
        void judgeResult() {
            if (isActorDeath()) result = Result.Lose;
            else if (isEnemiesDeath()) result = Result.Win;
            if (result != Result.None) onBattleEnd();
        }

        /// <summary>
        /// 角色是否死亡
        /// </summary>
        /// <returns></returns>
        bool isActorDeath() {
            return actor().isDeath();
        }

        /// <summary>
        /// 是否敌人死亡
        /// </summary>
        /// <returns></returns>
        bool isEnemiesDeath() {
            foreach (var enemy in _enemies)
                if (!enemy.isDeath()) return false;
            return true;
        }

        /// <summary>
        /// 战斗结束回调
        /// </summary>
        void onBattleEnd() {
			// TODO: 生成奖励
			actor().onBattleEnd();
            changeState(State.Result);
        }

        #endregion

        #endregion

    }
}