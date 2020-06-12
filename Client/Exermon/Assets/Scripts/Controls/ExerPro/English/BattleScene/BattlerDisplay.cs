
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
	/// 战斗者显示控件
	/// </summary
	public class BattlerDisplay :
		SelectableItemDisplay<RuntimeBattler>, IComparable<BattlerDisplay> {

		/// <summary>
		/// 动画名称定义
		/// </summary>
		public const string IdleAnimation = "Idle";
		public const string HurtAnimation = "Hurt";
		public const string AttackAnimation = "Attack";
		public const string MovingAnimation = "Move";
		public const string EscapeAnimation = "Escape";
		public const string PowerUpAnimation = "PowerUp";
		public const string PowerDownAnimation = "PowerDown";

		public const string MovingAttr = "moving";
		public const string AttackAttr = "attack";
		public const string HurtAttr = "hurt";
		public const string SkillAttr = "skill";
		public const string PowerUpAttr = "power_up";
		public const string PowerDownAttr = "power_down";

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Image battle;

		public StatesContainer states;
		public HPResultsContainer hpResults;

		public AnimationView animation; // 控制战斗的动态动画
		public AnimatorView animator; // 控制静态动画

		public MultParamsDisplay hpBar;

		/// <summary>
		/// 外部变量定义
		/// </summary>
		public Vector2 attackOffset; // 受击位置偏移量
		public Vector2 escapeOffset; // 逃离战斗的偏移量

		/// <summary>
		/// 外部系统定义
		/// </summary>
		protected BattleService battleSer;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		protected Vector2 oriPosition;
		protected RuntimeAction currentAction;

		protected RectTransform rectTransform;
		
		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			rectTransform = transform as RectTransform;
			initializeAnimationEvents();
		}

		/// <summary>
		/// 初始化动画事件
		/// </summary>
		void initializeAnimationEvents() {
			animation.addEndEvent(MovingAnimation, onMoveEnd);
			animation.addEndEvent(EscapeAnimation, onMoveEnd);

			animator.addEndEvent(HurtAnimation, onActionEnd);
			animator.addEndEvent(AttackAnimation, onActionEnd);
			animator.addEndEvent(PowerUpAnimation, onActionEnd);
			animator.addEndEvent(PowerDownAnimation, onActionEnd);
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
			updateAction();
			updateResult();
			updateHPDelta();
		}

		/// <summary>
		/// 更新行动
		/// </summary>
		protected virtual void updateAction() {
			currentAction = item.currentAction();
			// if (item.currentAction() != null) item.processAction();
			// processAction(currentAction);
		}

		/*
		/// <summary>
		/// 处理行动
		/// </summary>
		/// <param name="action"></param>
		protected virtual void processAction(RuntimeAction action) {
			item.processAction();
		}
		*/

		/// <summary>
		/// 更新结果
		/// </summary>
		void updateResult() {
			var result = item.getResult();
			if (result != null) processResult(result);
		}

		/// <summary>
		/// 处理结果
		/// </summary>
		/// <param name="result"></param>
		protected virtual void processResult(RuntimeActionResult result) { }

		/// <summary>
		/// 更新HP变化
		/// </summary>
		void updateHPDelta() {
			var delta = item.deltaHP;
			if (delta != null) hpResults.addItem(delta);
		}

		/// <summary>
		/// 更新HP变化
		/// </summary>
		void updateStates() {
			if (!item.isStateChanged) return;
			states.setItems(item.allRuntimeStates());
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 是否为敌人
		/// </summary>
		/// <returns></returns>
		public bool isEnemy() {
			return item != null && item.isEnemy();
		}

		/// <summary>
		/// 是否为角色
		/// </summary>
		/// <returns></returns>
		public bool isActor() {
			return item != null && item.isActor();
		}

		/// <summary>
		/// 是否激活
		/// </summary>
		/// <returns></returns>
		public override bool isActived() {
			return isEnemy() && base.isActived();
		}

		/// <summary>
		/// 动画是否播放
		/// </summary>
		/// <returns></returns>
		public bool isAnimationPlaying() {
			return animation.isPlaying();
		}

		/// <summary>
		/// 获取战斗玩家
		/// </summary>
		/// <param name="battler">战斗者</param>
		public RuntimeActor actor(RuntimeBattler battler = null) {
			if (battler == null) battler = item;
			return battler as RuntimeActor;
		}

		/// <summary>
		/// 获取战斗敌人
		/// </summary>
		/// <param name="battler">战斗者</param>
		public RuntimeEnemy enemy(RuntimeBattler battler = null) {
			if (battler == null) battler = item;
			return battler as RuntimeEnemy;
		}

		/// <summary>
		/// 行动目标显示控件
		/// </summary>
		/// <returns></returns>
		public BattlerDisplay target() {
			if (currentAction == null) return null;

			var battleGround = container as BattleGround;
			if (battleGround == null) return null;

			return battleGround.getBattlerDisplay(currentAction.object_);
		}

		#endregion

		#region 动画控制

		#region 移动动画

		/// <summary>
		/// 受击位置
		/// </summary>
		/// <returns></returns>
		public Vector2 beAttackedPosition() {
			return rectTransform.anchoredPosition + attackOffset;
		}

		/// <summary>
		/// 重置位置
		/// </summary>
		public void resetPosition() {
			moveTo(oriPosition);
		}

		/// <summary>
		/// 重置位置
		/// </summary>
		public void moveToTarget() {
			var target = this.target();
			if (target == null) return;

			moveTo(target.beAttackedPosition());
		}

		/// <summary>
		/// 逃离
		/// </summary>
		public void escape() {
			moveDelta(escapeOffset, EscapeAnimation);
		}

		/// <summary>
		/// 移动到指定位置
		/// </summary>
		public void moveTo(Vector2 target, string name = MovingAnimation) {
			animation.moveTo(oriPosition, MovingAnimation);
			animator.setVar(MovingAttr, true);
		}

		/// <summary>
		/// 移动指定量
		/// </summary>
		public void moveDelta(Vector2 delta, string name = MovingAnimation) {
			animation.moveDelta(oriPosition, MovingAnimation);
			animator.setVar(MovingAttr, true);
		}

		/// <summary>
		/// 移动结束回调
		/// </summary>
		public void onMoveEnd() {
			animator.setVar(MovingAttr, false);
		}

		/// <summary>
		/// 移动结束回调
		/// </summary>
		public void onEscapeEnd() {
			item.isEscaped = true;
			onMoveEnd();
		}

		#endregion

		#region 攻击动画

		/// <summary>
		/// 攻击
		/// </summary>
		public void attack() {
			animator.setVar(AttackAttr, true);
		}

		/// <summary>
		/// 攻击结束回调
		/// </summary>
		public void onAttackEnd() {
			animator.setVar(AttackAttr, false);
		}

		/// <summary>
		/// 发动技能
		/// </summary>
		public void skill() {
			animator.setVar(SkillAttr, true);
		}

		/// <summary>
		/// 技能结束回调
		/// </summary>
		public void onSkillEnd() {
			animator.setVar(SkillAttr, false);
		}

		#endregion

		#region 目标动画

		/// <summary>
		/// 提升
		/// </summary>
		public void powerUp() {
			animator.setVar(PowerUpAttr, true);
		}

		/// <summary>
		/// 削弱
		/// </summary>
		public void powerDown() {
			animator.setVar(PowerDownAttr, true);
		}

		/// <summary>
		/// 削弱
		/// </summary>
		public void hurt() {
			animator.setVar(HurtAttr, true);
		}

		#endregion

		#region 动画行动控制

		/// <summary>
		/// 击中回调
		/// </summary>
		public void onHit() {
			item.processAction();
			playHitAnimation();
			item.endAction();
		}

		/// <summary>
		/// 播放击中动画
		/// </summary>
		protected virtual void playHitAnimation() {
			target().hurt();
			onAttackEnd();
		}

		/// <summary>
		/// 结束行动 
		/// </summary>
		public void onActionEnd() {
			animator.setVar(HurtAttr, false);
			animator.setVar(PowerUpAttr, false);
			animator.setVar(PowerDownAttr, false);
		}

		#endregion

		#region 其他动画

		#endregion

		#endregion

		#region 界面控制

		/// <summary>
		/// 配置位置
		/// </summary>
		/// <param name="pos"></param>
		public virtual void setupPosition(Vector2 pos) {
			rectTransform.anchoredPosition = oriPosition = pos;
		}

		/// <summary>
		/// 绘制确切物品
		/// </summary>
		/// <param name="item">题目</param>
		protected override void drawExactlyItem(RuntimeBattler item) {
            base.drawExactlyItem(item);
			hpBar?.setValue(item, "hp");

			var battle = item.getBattlePicture();
			this.battle.gameObject.SetActive(true);
			this.battle.overrideSprite = AssetLoader.generateSprite(battle);
		}

		/// <summary>
		/// 清除物品
		/// </summary>
		protected override void drawEmptyItem() {
            battle.gameObject.SetActive(false);
			hpBar.clearValue();
		}

		/// <summary>
		/// 比较大小
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int CompareTo(BattlerDisplay other) {
			return rectTransform.anchoredPosition.y.CompareTo(
				other.rectTransform.anchoredPosition.y);
		}

		#endregion

	}
}