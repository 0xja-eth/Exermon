
using System.Collections.Generic;

using Core.Systems;
using Core.UI;

using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;

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
		/// 外部组件设置
		/// </summary>
		public BattleGround battleGround;

		public MenuWindow menu;

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
		}

        #endregion

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
			if (battleSer.isStateChanged())
				onStateChanged();
			battleSer?.update();
		}

		#endregion

		#region 回调控制

		/// <summary>
		/// 状态改变回调
		/// </summary>
		void onStateChanged() {
			switch ((BattleService.State)battleSer.state) {
				case BattleService.State.Playing:
					onPlay(); break;
			}
		}

		/// <summary>
		/// 开始出牌
		/// </summary>
		void onPlay() {
			menu.startWindow();
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

		/// <summary>
		/// 生成单个目标
		/// </summary>
		/// <param name="battler">对战者</param>
		/// <returns>返回目标数组</returns>
		List<RuntimeBattler> makeSingleTarget(RuntimeBattler battler) {
			var res = new List<RuntimeBattler>();
			res.Add(battleSer.actor());
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
		/// 使用药水
		/// </summary>
		/// <param name="potion"></param>
		public void usePotion(ExerProPackPotion packPotion) {
			if (packPotion == null ||
				packPotion.isNullItem()) return;

			var targets = makePotionTargets(packPotion.item());

			battleSer.use(packPotion.effects(), targets);
			battleSer.actor().usePotion(packPotion);
		}

		/// <summary>
		/// 使用卡牌
		/// </summary>
		/// <param name="packCard">卡牌</param>
		/// <param name="enemy">敌人</param>
		public void useCard(ExerProPackCard packCard, RuntimeEnemy enemy) {
			if (packCard == null ||
				packCard.isNullItem()) return;

			var targets = makeCardTargets(packCard.item(), enemy);

			battleSer.use(packCard.effects(), targets);
			battleSer.actor().useCard(packCard);
		}

		/// <summary>
		/// 刷新状态
		/// </summary>
		public void refreshStatus() {

		}

		#endregion

		/// <summary>
		/// 跳过
		/// </summary>
		public void jump() {
			battleSer.jump();
			menu.terminateWindow();
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
