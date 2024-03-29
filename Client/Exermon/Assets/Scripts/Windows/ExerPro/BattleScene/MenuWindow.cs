﻿using UnityEngine;
using UnityEngine.UI;

using Core.UI;
using Core.UI.Utils;

using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;

namespace UI.ExerPro.EnglishPro.BattleScene.Windows {

	using Controls.Menu;
	using Controls.Battler;

	/// <summary>
	/// 菜单窗口
	/// </summary>
	public class MenuWindow : BaseWindow {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Text energy; 
		public HandCardGroupDisplay handCards;
		public PotionSlotDisplay potionSlot;

		/// <summary>
		/// 场景组件引用
		/// </summary>
		BattleScene scene;

		/// <summary>
		/// 外部系统设置
		/// </summary>
		BattleService battleSer;

		#region 初始化

		/// <summary>
		/// 初始化场景
		/// </summary>
		protected override void initializeScene() {
			base.initializeScene();
			scene = SceneUtils.getCurrentScene<BattleScene>();
		}

		/// <summary>
		/// 初始化外部系统
		/// </summary>
		protected override void initializeSystems() {
			base.initializeSystems();
			battleSer = BattleService.get();
		}

		#endregion

		#region 启动/关闭控制

		/// <summary>
		/// 关闭窗口
		/// </summary>
		public override void terminateWindow() {
			base.terminateWindow();
			handCards.terminateView();
			potionSlot.terminateView();
		}

		#endregion

		#region 画面绘制

		/// <summary>
		/// 刷新
		/// </summary>
		protected override void refresh() {
			base.refresh();
			var actor = battleSer.actor();

			energy.text = actor.energy.ToString();
			setupHandCards(actor);
			setupPotionSlot(actor);
		}

		/// <summary>
		/// 配置手牌显示
		/// </summary>
		/// <param name="actor"></param>
		void setupHandCards(RuntimeActor actor) {
			handCards.setItems(actor.cardGroup.handGroup.items);
			handCards.startView();
		}

		/// <summary>
		/// 配置药品槽
		/// </summary>
		/// <param name="actor"></param>
		void setupPotionSlot(RuntimeActor actor) {
			potionSlot.setItems(actor.potionSlot.items);
			potionSlot.startView();
		}

		#endregion

		#region 流程控制

		/// <summary>
		/// 使用药水
		/// </summary>
		/// <param name="potion"></param>
		public bool usePotion(PotionSlotItemDisplay slotDisplay) {
			return scene.usePotion(slotDisplay?.getItem()?.packPotion);
		}

		/// <summary>
		/// 使用卡牌
		/// </summary>
		/// <param name="packCard">卡牌</param>
		/// <param name="enemy">敌人</param>
		public bool useCard(PackCardDisplay cardDisplay, EnemyDisplay enemyDisplay) {
			return scene.useCard(cardDisplay?.getItem(), enemyDisplay?.enemy());
		}

		#endregion

	}
}