
using System;

using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;
using Core.UI;

using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.ParamDisplays;
using UI.Common.Controls.AnimationSystem;

namespace UI.ExerPro.EnglishPro.BattleScene.Controls {

	/// <summary>
	/// 敌人显示控件
	/// </summary
	public class EnemyDisplay : BattlerDisplay {

		/// <summary>
		/// 动画名称定义
		/// </summary>
		public const string PowerUpAnimation = "PowerUp";
		public const string PowerDownAnimation = "PowerDown";
		public const string AddStatesAnimation = "AddStates";

		/// <summary>
		/// 字符串常量定义
		/// </summary>
		const string NameFormat = "{0}[{1}]";

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Text name;

		public Image think;

		/// <summary>
		/// 外部系统定义
		/// </summary>
		BattleService battleSer;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		Vector2 oriPosition;
		RuntimeAction currentAction;

		RectTransform rectTransform;
		
		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			rectTransform = transform as RectTransform;

		}

		/// <summary>
		/// 初始化外部系统
		/// </summary>
		protected override void initializeSystems() {
			base.initializeSystems();
			battleSer = BattleService.get();
		}

		#endregion

		#region 更新控制

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			updateEnemyAction();
		}

		/// <summary>
		/// 更新敌人行动
		/// </summary>
		void updateEnemyAction() {
			var action = enemy().currentEnemyAction;
			if (action != null) processEnemyAction(action);
		}

		/// <summary>
		/// 处理敌人行动
		/// </summary>
		void processEnemyAction(ExerProEnemy.Action action) {
			switch (action.typeEnum()) {
				case ExerProEnemy.Action.Type.Attack:
					processAttack(action); break;
				case ExerProEnemy.Action.Type.PowerUp:
					processPowerUp(action); break;
				case ExerProEnemy.Action.Type.PowerDown:
					processPowerDown(action); break;
				case ExerProEnemy.Action.Type.AddStates:
					processAddStates(action); break;
				case ExerProEnemy.Action.Type.Escape:
					processEscape(action); break;
			}
		}

		/// <summary>
		/// 处理攻击
		/// </summary>
		void processAttack(ExerProEnemy.Action action) {
			moveToTarget(); attack(); resetPosition();
		}

		/// <summary>
		/// 处理提升
		/// </summary>
		void processPowerUp(ExerProEnemy.Action action) {

		}

		/// <summary>
		/// 处理削弱
		/// </summary>
		void processPowerDown(ExerProEnemy.Action action) {

		}

		/// <summary>
		/// 处理附加状态
		/// </summary>
		void processAddStates(ExerProEnemy.Action action) {

		}

		/// <summary>
		/// 处理逃走
		/// </summary>
		void processEscape(ExerProEnemy.Action action) {

		}

		#endregion

		#region 界面控制

		/// <summary>
		/// 配置位置
		/// </summary>
		/// <param name="pos"></param>
		public override void setupPosition(Vector2 pos) {
			var enemy = this.enemy();
			if (enemy != null)
				pos += new Vector2(enemy.xOffset, enemy.yOffset);

			base.setupPosition(pos);
		}

		/// <summary>
		/// 绘制确切物品
		/// </summary>
		/// <param name="item">题目</param>
		protected override void drawExactlyItem(RuntimeBattler item) {
            base.drawExactlyItem(item);
			var enemy = this.enemy(item);
			var enemyData = enemy.enemy();

			name.text = string.Format(NameFormat,
				enemyData.name, enemyData.character);

			var think = AssetLoader.loadEnemyThink(
				enemy.currentActionType());
			this.think.gameObject.SetActive(true);
			this.think.overrideSprite = AssetLoader.
				generateSprite(think);
		}

		/// <summary>
		/// 绘制空物品
		/// </summary>
		protected override void drawEmptyItem() {
			base.drawEmptyItem();

			name.text = "";
			think.gameObject.SetActive(false);
		}

		#endregion

	}
}