﻿
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;
using Core.UI.Utils;

using GameModule.Services;

using ExerPro.EnglishModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.BattleScene.Controls.Battler {

	/// <summary>
	/// 战场控件
	/// </summary
	public class BattleGround : SelectableContainerDisplay<RuntimeBattler> {

		/// <summary>
		/// 位置偏移量设置
		/// </summary>
		const int MaxEnemyCols = CalcService.BattleEnemiesGenerator.MaxEnemyCols; // 最大敌人列数
		const int MaxEnemyRows = CalcService.BattleEnemiesGenerator.MaxEnemyRows; // 最大敌人行数

		/// <summary>
		/// 位置定义
		/// </summary>
		static readonly Vector2 ActorPos = new Vector2(-0.35f, 0);
		static readonly Vector2[] EnemiesPos = new Vector2[MaxEnemyCols * MaxEnemyRows] {
			new Vector2(0.3f, 0.25f), new Vector2(0.2f, 0.25f), new Vector2(0.1f, 0.25f), // 4, 5, 6
			new Vector2(0.35f, 0), new Vector2(0.25f, 0), new Vector2(0.15f, 0), // 1, 2, 3
			new Vector2(0.4f, -0.25f), new Vector2(0.3f, -0.25f), new Vector2(0.2f, -0.25f), // 7, 8, 9
		};

		/// <summary>
		/// 预制件设置
		/// </summary>
		public GameObject actorPerfab, enemyPerfab;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		List<BattlerDisplay> sortedDisplays = new List<BattlerDisplay>();

		#region 更新控制

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			updateBattlerLayers();
		}

		/// <summary>
		/// 更新战斗者层级
		/// </summary>
		void updateBattlerLayers() {
			foreach (var display in sortedDisplays)
				if (display.isAnimationPlaying()) {
					resetBattlerSiblings(); break;
				}
		}

		/// <summary>
		/// 重置战斗者层级
		/// </summary>
		void resetBattlerSiblings() {
			sortedDisplays.Sort();
			foreach (var display in sortedDisplays)
				display.transform.SetAsFirstSibling();
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 根据运行时对象获取战斗者显示控件
		/// </summary>
		/// <param name="battler">战斗者对象</param>
		/// <returns>返回对应的显示控件</returns>
		public BattlerDisplay getBattlerDisplay(RuntimeBattler battler) {
			return subViews.Find(display => display.getItem() == battler) as BattlerDisplay;
		}

		#endregion

		#region 界面控制

		/// <summary>
		/// 指定索引战斗者是否玩家
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public bool isActor(int index) {
			var battler = items[index];
			return battler != null && battler.isActor();
		}

		/// <summary>
		/// 指定索引战斗者是否敌人
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public bool isEnemy(int index) {
			var battler = items[index];
			return battler != null && battler.isEnemy();
		}

		/// <summary>
		/// 获取子视图预制件
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		protected override GameObject getSubViewPerfab(int index) {
			if (isActor(index)) return actorPerfab;
			if (isEnemy(index)) return enemyPerfab;
			return base.getSubViewPerfab(index);
		}

		/// <summary>
		/// 子视图创建回调
		/// </summary>
		/// <param name="sub">子视图</param>
		/// <param name="index">索引</param>
		protected override void onSubViewCreated(SelectableItemDisplay<RuntimeBattler> sub, int index) {
			base.onSubViewCreated(sub, index);
			var display = sub as BattlerDisplay;
			if (display == null) return;

			sortedDisplays.Add(display);

			if (isActor(index)) setupActorPos(display); // index = 0
			if (isEnemy(index)) setupEnemyPos(display, 
				items[index] as RuntimeEnemy);
		}

		/// <summary>
		/// 配置角色位置
		/// </summary>
		void setupActorPos(BattlerDisplay display) {
			display.setupPosition(ActorPos);
		}

		/// <summary>
		/// 设置敌人位置
		/// </summary>
		/// <param name="display"></param>
		/// <param name="index"></param>
		void setupEnemyPos(BattlerDisplay display, RuntimeEnemy battler) {
			display.setupPosition(EnemiesPos[battler.pos]);
		}

		#endregion
	}
}