
using System;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Core.Data.Loaders;
using Core.UI.Utils;

using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.ParamDisplays;
using UI.Common.Controls.AnimationSystem;

namespace UI.ExerPro.EnglishPro.BattleScene.Controls.Battler {

	/// <summary>
	/// 敌人显示控件
	/// </summary
	public class EnemyDisplay : BattlerDisplay { //, IDropHandler {

		/// <summary>
		/// 动画名称定义
		/// </summary>

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
		/// 内部变量定义
		/// </summary>
		ExerProEnemy.Action currentEnemyAction = null;

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
			currentEnemyAction = enemy().currentEnemyAction;
			if (currentEnemyAction != null)
				processEnemyAction(currentEnemyAction);
		}

		#endregion

		#region 行动处理

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
			skill();
		}

		/// <summary>
		/// 处理削弱
		/// </summary>
		void processPowerDown(ExerProEnemy.Action action) {
			skill();
		}

		/// <summary>
		/// 处理附加状态
		/// </summary>
		void processAddStates(ExerProEnemy.Action action) {
			skill();
		}

		/// <summary>
		/// 处理逃走
		/// </summary>
		void processEscape(ExerProEnemy.Action action) {
			escape();
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 是否空物品
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public override bool isNullItem(RuntimeBattler item) {
			return base.isNullItem(item) || !isEnemy();
		}

		#endregion

		#region 动画控制

		/// <summary>
		/// 击中回调
		/// </summary>
		protected override void playHitAnimation() {
			switch(currentEnemyAction.typeEnum()) {
				case ExerProEnemy.Action.Type.Attack:
					base.playHitAnimation(); break;
				case ExerProEnemy.Action.Type.PowerUp:
					powerUp(); onSkillEnd(); break;
				case ExerProEnemy.Action.Type.PowerDown:
				case ExerProEnemy.Action.Type.AddStates:
					target()?.powerDown();
					onSkillEnd(); break;
			}
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

		#region 事件控制
		/*
		/// <summary>
		/// 拖拽释放回调
		/// </summary>
		public void OnDrop(PointerEventData eventData){

		}

		/// <summary>
		/// 获取拖拽中的物品显示项
		/// </summary>
		/// <param name="data">事件数据</param>
		/// <returns>物品显示项</returns>
		DraggableItemDisplay<ExerProPackCard> getDraggingItemDisplay(
			PointerEventData data) {
			var obj = data.pointerDrag; if (obj == null) return null;
			return SceneUtils.get<DraggableItemDisplay<ExerProPackCard>>(obj);
		}
		*/
		#endregion

	}
}