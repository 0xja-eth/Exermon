
using System.Collections.Generic;

using UnityEngine;

using Core.Systems;
using Core.UI;

using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;

using UI.Common.Controls.ParamDisplays;

using UI.ExerPro.EnglishPro.MapScene.Controls;

/// <summary>
/// 地图场景
/// </summary>
namespace UI.ExerPro.EnglishPro.BattleScene {

	using Windows;
    using Controls.Battler;

    /// <summary>
    /// 地特训战斗场景
    /// </summary>
    public class BattleScene : BaseScene {

		/// <summary>
		/// 文本常量定义
		/// </summary>
		const string InvalidTargetAlertText = "无效的使用目标！";
		const string NotEngoughEnergyAlertText = "没有足够的能量！";

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public BattleGround battleGround;

		public MenuWindow menuWindow;
		public WordWindow wordWindow;
		public DrawWindow drawWindow;

		public PlayerStatus playerStatue;
		public MultParamsDisplay wordProgress;

		/// <summary>
		/// 外部系统设置
		/// </summary>
		EnglishService engSer;
		BattleService battleSer;

        #region 初始化

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            engSer = EnglishService.get();
			battleSer = BattleService.get();
			battleSer.onStateChanged = onStateChanged;
		}

        /// <summary>
        /// 场景名称
        /// </summary>
        /// <returns></returns>
        public override SceneSystem.Scene sceneIndex() {
            return SceneSystem.Scene.EnglishProBattleScene;
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected override void start() {
			base.start();
			battleGround.setItems(battleSer.battlers());
			refreshStatus();
		}

		/// <summary>
		/// 销毁回调
		/// </summary>
		private void OnDestroy() {
			battleSer.onStateChanged = null;
		}

		#endregion

		#region 更新控制

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
            base.update();
			battleSer?.update();
			/*
			if (battleSer.isStateChanged())
				onStateChanged();*/
		}

		#endregion

		#region 回调控制

		/// <summary>
		/// 状态改变回调
		/// </summary>
		void onStateChanged() {
			Debug.Log("BattleScene.onStateChanged: " + (BattleService.State)battleSer.state);
			switch ((BattleService.State)battleSer.state) {
				case BattleService.State.Answering: onAnswer(); break;
				//case BattleService.State.Drawing: onDraw(); break;
				case BattleService.State.Playing: onPlay(); break;
				case BattleService.State.Discarding: onDiscard(); break;
				case BattleService.State.Enemy: onEnemy(); break;
			}
		}

		/// <summary>
		/// 答题
		/// </summary>
		void onAnswer() {
			wordWindow.startWindow();
		}

		/// <summary>
		/// 抽卡
		/// </summary>
		void onDraw() {
			//wordWindow.terminateWindow();
			//drawWindow.startWindow();
		}

		/// <summary>
		/// 开始出牌
		/// </summary>
		void onPlay() {
			menuWindow.startWindow();
		}

		/// <summary>
		/// 弃牌
		/// </summary>
		void onDiscard() {
			menuWindow.terminateView();
		}

		/// <summary>
		/// 敌人
		/// </summary>
		void onEnemy() {

		}

		#endregion

		#region 流程控制

		#region 使用控制

		///// <summary>
		///// 生成使用目标
		///// </summary>
		///// <typeparam name="T">物品类型</typeparam>
		///// <param name="item">物品</param>
		///// <returns>返回使用目标</returns>
		//List<RuntimeBattler> makeTargets<T>(T item) where T : ExerProItem {
		//	if (typeof(T) == typeof(ExerProPotion))
		//		var res = new List<RuntimeBattler>();
		//	res.Add(battleSer.actor());
		//	else if (typeof(T) == typeof(ExerProCard))
		//		res = makeCardTargets(item as ExerProCard);
		//	return res;
		//}

		#region 可用性判断

		/// <summary>
		/// 生成单个目标
		/// </summary>
		/// <param name="battler">对战者</param>
		/// <returns>返回目标数组</returns>
		List<RuntimeBattler> makeSingleTarget(RuntimeBattler battler) {
			var res = new List<RuntimeBattler>();
			if (battler != null) res.Add(battler);
			return res;
		}

		/// <summary>
		/// 生成药水目标
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="item"></param>
		/// <returns></returns>
		List<RuntimeBattler> makePotionTargets(ExerProPotion potion) {
			return makeSingleTarget(battleSer.actor());
		}

		/// <summary>
		/// 生成卡牌目标
		/// </summary>
		/// <param name="card"></param>
		/// <returns></returns>
		List<RuntimeBattler> makeCardTargets(ExerProCard card, RuntimeBattler battler) {
			switch ((ExerProCard.Target)card.target) {
				case ExerProCard.Target.One:
					return makeSingleTarget(battler);
				case ExerProCard.Target.All:
					return battleSer.enemies();
				default:
					return new List<RuntimeBattler>();
			}
		}

		/// <summary>
		/// 目标判断
		/// </summary>
		bool judgeTarget(ExerProPackCard packCard, RuntimeEnemy enemy) {
			var card = packCard.item();
			switch ((ExerProCard.Target)card.target) {
				case ExerProCard.Target.One:
					return enemy != null; // 必须指定一名敌人
				default:
					return true;
			}
		}

		/// <summary>
		/// 能否使用卡牌
		/// </summary>
		/// <param name="packCard">卡牌</param>
		/// <param name="enemy">敌人</param>
		/// <returns></returns>
		bool isCardUsable(ExerProPackCard packCard, RuntimeEnemy enemy) {
			if (packCard == null || packCard.isNullItem()) return false;
			if (battleSer.actor().energy < packCard.cost())
				return requestInvalidAlert(NotEngoughEnergyAlertText);
			if (!judgeTarget(packCard, enemy))
				return requestInvalidAlert(InvalidTargetAlertText);
			return true;
		}

		/// <summary>
		/// 能否使用药水
		/// </summary>
		/// <param name="packCard">卡牌</param>
		/// <param name="enemy">敌人</param>
		/// <returns></returns>
		bool isPotionUsable(ExerProPackPotion packCard) {
			if (packCard == null || packCard.isNullItem()) return false;
			return true;
		}

		/// <summary>
		/// 请求无效提示
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		bool requestInvalidAlert(string text) {
			if (text != "") gameSys.requestAlert(text);
			return false; // 需要返回 false 表示无效
		}

		#endregion

		/// <summary>
		/// 使用药水
		/// </summary>
		/// <param name="potion"></param>
		public void usePotion(ExerProPackPotion packPotion) {
			if (!isPotionUsable(packPotion)) return;

			var targets = makePotionTargets(packPotion.item());

			battleSer.use(packPotion.item(), targets);
			battleSer.actor().usePotion(packPotion);

			refreshStatus();
		}

		/// <summary>
		/// 使用卡牌
		/// </summary>
		/// <param name="packCard">卡牌</param>
		/// <param name="enemy">敌人</param>
		public void useCard(ExerProPackCard packCard, RuntimeEnemy enemy) {
			Debug.Log("useCard: " + packCard + ", " + enemy);

			if (!isCardUsable(packCard, enemy)) return;
			Debug.Log("Use enable!");

			var targets = makeCardTargets(packCard.item(), enemy);

			battleSer.use(packCard.item(), targets);
			battleSer.actor().useCard(packCard);

			refreshStatus();
		}

		/// <summary>
		/// 刷新状态
		/// </summary>
		public void refreshStatus() {
			battleGround.requestRefresh();
			menuWindow.requestRefresh();

			drawPlayerDisplay();
			drawProgresses();
		}

		/// <summary>
		/// 绘制玩家信息
		/// </summary>
		void drawPlayerDisplay() {
			playerStatue.setItem(battleSer.actor());
		}

		/// <summary>
		/// 绘制进度
		/// </summary>
		void drawProgresses() {
			wordProgress.setValue(engSer.record, "word_progress");
		}

		/// <summary>
		/// 刷新
		/// </summary>
		protected override void awake() {
			base.awake();
			refreshStatus();
		}

		#endregion

		/// <summary>
		/// 抽牌
		/// </summary>
		public void draw() {
			wordWindow.terminateWindow();
			drawWindow.startWindow();
		}

		/// <summary>
		/// 进入回合
		/// </summary>
		public void play() {
			drawWindow.terminateWindow();
			battleSer.play();
		}

		/// <summary>
		/// 跳过
		/// </summary>
		public void jump() {
			battleSer.jump();
			menuWindow.terminateWindow();
		}

		/// <summary>
		/// 退出场景
		/// </summary>
		public override void popScene() {
			battleSer.terminate();
        }

        #endregion

    }
}
