using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Core.Systems;
using Core.Services;

using GameModule.Services;

using ItemModule.Data;
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
            Result = 6, // 战斗结算阶段

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
		SceneSystem sceneSys;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		ExerProRecord record;
        ExerProCardGroup _cardGroup;

        List<RuntimeEnemy> _enemies; // 敌人
        int curEnemyIndex = 0; // 当前敌人索引

		int corrCnt = 0, bonusCnt = 0;
        bool bonus = false;
		
		int round = 1;
		
		Result result = Result.None;

        #region 初始化

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            engSer = EnglishService.get();
			sceneSys = SceneSystem.get();
        }

        /// <summary>
        /// 初始化状态字典
        /// </summary>
        protected override void initializeStateDict() {
            base.initializeStateDict();
            addStateDict(State.NotInBattle);
			addStateDict(State.Answering); //, updateAnswering);
            addStateDict(State.Drawing, updateDrawing);
            addStateDict(State.Playing);
            addStateDict(State.Discarding, updateDiscarding);
            addStateDict(State.Enemy, updateEnemy);
            addStateDict(State.Result);
        }

        #endregion

        #region 更新控制

		/// <summary>
		/// 更新回答（回合开始）
		/// </summary>
		//void updateAnswering() {
		//	if (isStateChanged()) onRoundStart();
		//}

		/// <summary>
		/// 更新抽牌
		/// </summary>
		void updateDrawing() {
			// 抽牌答对次数 + 额外次数
			if (isStateChanged()) onDraw();
		}
		/*
        /// <summary>
        /// 更新出牌
        /// </summary>
        void updatePlaying() {
        }
		*/
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
			if (processEnemiesAction()) onRoundEnd();
		}

        
        /// <summary>
        /// 更新结果
        /// </summary>
        void updateResult() {
            changeState(State.NotInBattle);
        }
        
        #endregion

        #region 流程控制

        /// <summary>
        /// 开始战斗
        /// </summary>
        /// <param name="node"></param>
        public void start(ExerProMapNode.Type type) {
            setup(type); onBattleStart();
			sceneSys.pushScene(SceneSystem.Scene.EnglishProBattleScene);
			changeState(State.Answering);
        }

        /// <summary>
        /// 开始配置
        /// </summary>
        void setup(ExerProMapNode.Type type) {
			round = 1; result = Result.None;
			record = engSer.record;

			_cardGroup = actor().cardGroup;

			generateEnemies(type);
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
		/// 进入回合
		/// </summary>
		public void play() {
			changeState(State.Playing);
		}

		/// <summary>
		/// 结束出牌阶段
		/// </summary>
		public void jump() {
			changeState(State.Discarding);
		}

		#region 回合控制

		/// <summary>
		/// 当前回合
		/// </summary>
		/// <returns></returns>
		public int currentRound() {
			return round;
		}

		/// <summary>
		/// 角色是否死亡
		/// </summary>
		/// <returns></returns>
		bool isActorLost() {
			return actor().isLost();
		}

		/// <summary>
		/// 是否敌人死亡
		/// </summary>
		/// <returns></returns>
		bool isEnemiesLost() {
			foreach (var enemy in _enemies)
				if (!enemy.isLost()) return false;
			return true;
		}

		/// <summary>
		/// 重置回合状态
		/// </summary>
		void resetRoundStates() {
			bonus = false;
			corrCnt = bonusCnt = 0;
			curEnemyIndex = 0;
		}

		/// <summary>
		/// 判断结果
		/// </summary>
		void processRoundResult() {
			if (isActorLost()) result = Result.Lose;
			else if (isEnemiesLost()) result = Result.Win;
			if (result != Result.None) onBattleEnd();
			else nextRound();
		}

		/// <summary>
		/// 切换到下一回合
		/// </summary>
		void nextRound() {
			round++;
			changeState(State.Answering);
		}

		#endregion

		/// <summary>
		/// 结束
		/// </summary>
		public void terminate() {
			engSer.exitNode();
            //onBattleEnd();
        }

		#endregion

		#region 数据获取

		/// <summary>
		/// 获取玩家
		/// </summary>
		/// <returns></returns>
		public RuntimeActor actor() {
			return record?.actor;
		}

		/// <summary>
		/// 卡组
		/// </summary>
		/// <returns></returns>
		public ExerProCardGroup cardGroup() {
			return _cardGroup;
		}

		/// <summary>
		/// 手牌
		/// </summary>
		/// <returns></returns>
		public ExerProCardHandGroup handGroup() {
			return _cardGroup?.handGroup;
		}

		/// <summary>
		/// 抽牌堆
		/// </summary>
		/// <returns></returns>
		public ExerProCardDrawGroup drawGroup() {
			return _cardGroup?.drawGroup;
		}

		/// <summary>
		/// 弃牌堆
		/// </summary>
		/// <returns></returns>
		public ExerProCardDiscardGroup discardGroup() {
			return _cardGroup?.discardGroup;
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
		/// 是否Bonus
		/// </summary>
		/// <returns></returns>
		public bool isBonus() {
			return bonus;
		}

		/// <summary>
		/// 单词量
		/// </summary>
		/// <returns></returns>
		public int wordCount() {
			return bonus ? BonusWordCount : NormalWordCount;
		}

		/// <summary>
		/// 单词索引
		/// </summary>
		/// <returns></returns>
		public int wordIndex() {
			return bonus ? bonusCnt : corrCnt;
		}

		/// <summary>
		/// 当前抽牌数量
		/// </summary>
		/// <returns></returns>
		public int drawCount() {
			return Math.Min(maxDrawCount(), 
				corrCnt + bonusCnt / BonusWordCount * BonusWordCount);
		}

		/// <summary>
		/// 最大抽牌数量
		/// </summary>
		/// <returns></returns>
		public int maxDrawCount() {
			var hand = _cardGroup.handGroup;
			return hand.capacity - hand.items.Count;
		}

		/// <summary>
		/// 当前题目
		/// </summary>
		/// <returns></returns>
		public Word currentWord() {
            return record.nextWord();
        }

		/// <summary>
		/// 当前抽卡（自动清空）
		/// </summary>
		/// <returns></returns>
		public ExerProPackCard[] drawnCards() {
            return _cardGroup.handGroup.getDrawnCards();
        }

        /// <summary>
        /// 回答
        /// </summary>
        /// <param name="chinese"></param>
        public void answer(string chinese, 
			UnityAction<bool> onSuccess, UnityAction onError = null) {
            engSer.answerWord(currentWord(), chinese, (res) => 
                onAnswerSuccess(res, onSuccess), onError);
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

		#region 行动控制

		#region 玩家行动
		
		/// <summary>
		/// 使用
		/// </summary>
		/// <param name="effect">效果</param>
		/// <param name="targets">目标</param>
		public void use<T>(T item, List<RuntimeBattler> targets) where T: BaseExerProItem {
			Debug.Log("use: " + item.toJson().ToJson() + " -> " + targets);

			var actor = this.actor();
			var repeats = item.repeats();

			for (int i = 0; i < repeats; ++i)
				actor.addAction(new RuntimeAction(actor, targets.ToArray(), item));
			//foreach (var target in targets)
			//	actor.addAction(new RuntimeAction(actor, target, effects));
		}

		#endregion

		#region 敌人行动
		
		/// <summary>
		/// 处理敌人行动
		/// </summary>
		/// <returns>敌人是否全部行动完毕</returns>
		bool processEnemiesAction() {
			Debug.Log("curEnemyIndex = " + curEnemyIndex);
			var enemy = _enemies[curEnemyIndex];
			if (enemy.updateAction()) curEnemyIndex++;
			return curEnemyIndex >= _enemies.Count;
		}

		#endregion

		#endregion

		#region 回调控制

		/// <summary>
		/// 战斗开始回调
		/// </summary>
		void onBattleStart() {
			Debug.Log("onBattleStart: " + round);
			actor().onBattleStart();
			foreach (var enemy in _enemies)
				enemy.onBattleStart();
		}

		/// <summary>
		/// 抽牌回调
		/// </summary>
		void onDraw() {
			onRoundStart();
			var cnt = drawCount();
			for (int i = 0; i < cnt; ++i)
				_cardGroup.drawCard();
		}

		/// <summary>
		/// 回合开始回调
		/// </summary>
		void onRoundStart() {
			Debug.Log("onRoundStart: " + round);
			actor().onRoundStart(round);
			foreach (var enemy in _enemies)
				enemy.onRoundStart(round);
		}

		/// <summary>
		/// 回合结束回调
		/// </summary>
		void onRoundEnd() {
			Debug.Log("onRoundEnd: " + round);
			resetRoundStates();
			battlersRoundEnd();
			processRoundResult();
		}

		/// <summary>
		/// 处理战斗者回合结束
		/// </summary>
		void battlersRoundEnd() {
			Debug.Log("battlersRoundEnd: " + round);
			actor().onRoundEnd(round);
			foreach (var enemy in _enemies)
				enemy.onRoundEnd(round);
		}

		/// <summary>
		/// 战斗结束回调
		/// </summary>
		void onBattleEnd() {
			Debug.Log("onBattleEnd: " + round);
            engSer.processReward(enemyNumber: 10, bossNumber: 0);
            battlersBattleEnd();
			changeState(State.Result);
		}

        /// <summary>
        /// 战斗结束回调
        /// </summary>
        void battlersBattleEnd() {
			actor().onBattleEnd();
			foreach (var enemy in _enemies)
				enemy.onBattleEnd();
		}

		#endregion

	}
}