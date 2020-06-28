
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

namespace UI.ExerPro.EnglishPro.BattleScene.Controls.Battler {

	/// <summary>
	/// 战斗者显示控件
	/// </summary
	public class BattlerDisplay :
		SelectableItemDisplay<RuntimeBattler>, IComparable<BattlerDisplay> {

		/// <summary>
		/// 最大偏移量
		/// </summary>
		const int MaxXOffset = 24;
		const int MaxYOffset = 16;

		/// <summary>
		/// 动画名称定义
		/// </summary>
		public const string IdleAnimation = "Idle";
		public const string HurtAnimation = "Hurt";
		public const string AttackAnimation = "Attack";
		public const string ForwardAnimation = "Forward";
		public const string BackAnimation = "Back";
		public const string EscapeAnimation = "Escape";
		public const string DieAnimation = "Die";
		//public const string PowerUpAnimation = "PowerUp";
		//public const string PowerDownAnimation = "PowerDown";

		public const string MovingAttr = "moving";
		public const string AttackAttr = "attack";
		public const string HurtAttr = "hurt";
		public const string DieAttr = "died";

		public const string DefaultAttackAnimation = "Animation_1";
		public const string DefaultHurtAnimation = "Animation_3";

		//public const string SkillAttr = "skill";
		//public const string PowerUpAttr = "power_up";
		//public const string PowerDownAttr = "power_down";

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Image battle;

		public Text power, defense;
		public GameObject powerObj, defenseObj;

		public StatesContainer states;
		public HPResultsContainer hpResults;

		public AnimationView animation; // 控制战斗的动态动画
		public AnimatorView animator; // 控制静态动画

		public MultParamsDisplay hpBar;
		public GameObject hpBarObj;

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
		//protected RuntimeAction currentAction;

		protected RectTransform rectTransform;

		protected bool hitFlag = false, resultFlag = false;

		float xOffset, yOffset;
		
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
			animation.addEndEvent(ForwardAnimation, attack);
			animation.addEndEvent(BackAnimation, onMoveEnd);
			animation.addEndEvent(EscapeAnimation, onEscapeEnd);

			//animator.addEndEvent(AttackAnimation, resetPosition);

			//animator.addEndEvent(HurtAnimation, onActionEnd);
			//animator.addEndEvent(AttackAnimation, onActionEnd);
			//animator.addEndEvent(PowerUpAnimation, onActionEnd);
			//animator.addEndEvent(PowerDownAnimation, onActionEnd);
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
			updateDead();
		}

		/// <summary>
		/// 更新行动
		/// </summary>
		protected virtual void updateAction() {
			var action = item?.currentAction();
			if (action == null) return;
			processAction(action);
			//if (currentAction != null)
			//	item.processAction();
		}

		/// <summary>
		/// 处理行动
		/// </summary>
		/// <param name="action"></param>
		protected virtual void processAction(RuntimeAction action) {
			Debug.Log(name + ": processAction + " + action.toJson().ToJson());

			actionManager()?.add(action);
		}

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
		protected virtual void processResult(RuntimeActionResult result) {
			if (result.drawCardCnt > 0) processDrawCards();
			requestRefresh();
		}

		/// <summary>
		/// 更新HP变化
		/// </summary>
		void updateHPDelta() {
			var delta = item.deltaHP;
			if (delta != null) {
				Debug.Log(name + ": updateHPDelta: " + delta.value);
				hpResults.addItem(delta);
				requestRefresh();
			}
		}

		/// <summary>
		/// 更新HP变化
		/// </summary>
		void updateStates() {
			if (!item.isStateChanged) return;
			states.setItems(item.allRuntimeStates());
		}

		/// <summary>
		/// 更新死亡
		/// </summary>
		void updateDead() {
			if (item.isDead()) die();
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 是否空物品
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		//public override bool isNullItem(RuntimeBattler item) {
		//	return base.isNullItem(item) || item.isDead() || item.isEscaped;
		//}

		#region 行动数据

		/// <summary>
		/// 是否击中
		/// </summary>
		/// <returns></returns>
		public bool isHit() {
			var res = hitFlag;
			hitFlag = false;
			return res;
		}
		
		/// <summary>
		/// 是否结果
		/// </summary>
		/// <returns></returns>
		public bool isResult() {
			var res = resultFlag;
			resultFlag = false;
			return res;
		}

		/// <summary>
		/// 配置起手动画
		/// </summary>
		/// <param name="ani"></param>
		public void setupStartAni(AnimationClip ani) {
			animator.changeAni(DefaultAttackAnimation, ani);
			// AttackAnimation
		}

		/// <summary>
		/// 配置目标动画
		/// </summary>
		/// <param name="ani"></param>
		public void setupTargetAni(AnimationClip ani) {
			animator.changeAni(DefaultHurtAnimation, ani);
			// HurtAnimation
		}

		/*
		/// <summary>
		/// 获取起手动画
		/// </summary>
		/// <returns></returns>
		AnimationClip startAni() {
			return currentAction?.startAni ??
				actionManager()?.defaultStartAni;
		}

		/// <summary>
		/// 获取起手动画
		/// </summary>
		/// <returns></returns>
		AnimationClip targetAni() {
			return currentAction?.targetAni ??
				actionManager()?.defaultTargetAni;
		}
		*/
		/// <summary>
		/// 获取行动目标
		/// </summary>
		/// <returns></returns>
		BattlerDisplay targetDisplay() {
			return actionManager()?.getTarget(this);
		}

		#endregion

		#region 控件数据

		/// <summary>
		/// 获取战场现实控件
		/// </summary>
		/// <returns></returns>
		public BattleGround battleGround() {
			return container as BattleGround;
		}

		/// <summary>
		/// 获取行动管理控件
		/// </summary>
		/// <returns></returns>
		public ActionManager actionManager() {
			return battleGround()?.actionManager;
		}

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
		
		#endregion

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
		/// 获取原始位置
		/// </summary>
		/// <returns></returns>
		public Vector2 getOriPosition() {
			return oriPosition;
		}

		/// <summary>
		/// 重置位置
		/// </summary>
		public void resetPosition() {
			Debug.Log("resetPosition: " + oriPosition);
			moveTo(oriPosition, BackAnimation);
		}

		/// <summary>
		/// 重置位置
		/// </summary>
		public void moveToTarget() {
			Debug.Log("moveToTarget");
			moveTo(actionManager().getAttackPosition(this));
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
		public void moveTo(Vector2 target, string name = ForwardAnimation) {
			animation.moveTo(target, name);
			animator.setVar(MovingAttr, true);
		}

		/// <summary>
		/// 移动指定量
		/// </summary>
		public void moveDelta(Vector2 delta, string name = ForwardAnimation) {
			animation.moveDelta(delta, name);
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
			item.onEscape();
			onMoveEnd();
		}

		#endregion

		#region 技能动画

		/// <summary>
		/// 处理攻击
		/// </summary>
		public void moveToAttack() {
			moveToTarget();
			//attack();
			//resetPosition();
		}

		/// <summary>
		/// 攻击
		/// </summary>
		public void attack() {
			animator.setVar(AttackAttr);
		}

		/*
		/// <summary>
		/// 发动技能
		/// </summary>
		public void skill() {
			animator.setVar(SkillAttr);
		}
		
		/// <summary>
		/// 技能结束回调
		/// </summary>
		public void onSkillEnd() {
			//animator.setVar(SkillAttr, false);
		}
		*/
		#endregion

		#region 目标动画

		///// <summary>
		///// 提升
		///// </summary>
		//public void powerUp() {
		//	animator.setVar(PowerUpAttr);
		//}

		///// <summary>
		///// 削弱
		///// </summary>
		//public void powerDown() {
		//	animator.setVar(PowerDownAttr);
		//}

		/// <summary>
		/// 削弱
		/// </summary>
		public void hurt() {
			animator.setVar(HurtAttr);
		}

		/// <summary>
		/// 死亡
		/// </summary>
		public void die() {
			animator.setVar(DieAttr, true);
		}

		#endregion

		#region 动画行动控制

		/// <summary>
		/// 击中回调
		/// </summary>
		public void onHit() {
			hitFlag = true;
		}

		/// <summary>
		/// 产生结果
		/// </summary>
		public void onResult() {
			resultFlag = true;
		}

		///// <summary>
		///// 结束行动 
		///// </summary>
		//public void onActionEnd() {
		//	animator.setVar(HurtAttr, false);
		//	animator.setVar(PowerUpAttr, false);
		//	animator.setVar(PowerDownAttr, false);
		//}

		#endregion

		#region 其他动画

		#endregion

		#endregion

		#region 界面控制

		/// <summary>
		/// 配置位置
		/// </summary>
		/// <param name="pos"></param>
		public void setupPosition(Vector2 pos) {
			var xOffset = UnityEngine.Random.Range(-MaxXOffset, MaxXOffset);
			var yOffset = UnityEngine.Random.Range(-MaxYOffset, MaxYOffset);

			pos += new Vector2(xOffset, yOffset);
			rectTransform.anchoredPosition = oriPosition = pos;
		}

		/// <summary>
		/// 绘制确切物品
		/// </summary>
		/// <param name="item">题目</param>
		protected override void drawExactlyItem(RuntimeBattler item) {
            base.drawExactlyItem(item);
			drawBattle(item);
			drawHP(item);
			drawParams(item);
			drawStates(item);
		}

		/// <summary>
		/// 绘制战斗图
		/// </summary>
		/// <param name="item"></param>
		void drawBattle(RuntimeBattler item) {
			var battle = item.getBattlePicture();
			this.battle.gameObject.SetActive(true);
			this.battle.overrideSprite = AssetLoader.generateSprite(battle);
			this.battle.SetNativeSize();
		}

		/// <summary>
		/// 绘制体力值
		/// </summary>
		/// <param name="item"></param>
		void drawHP(RuntimeBattler item) {
			if (hpBar) {
				hpBar.setValue(item, "hp");
				hpBarObj.SetActive(!item.isDead());
			}
		}

		/// <summary>
		/// 绘制属性
		/// </summary>
		/// <param name="battler"></param>
		void drawParams(RuntimeBattler battler) {
			var pow = battler.power();
			var def = battler.defense();

			powerObj.SetActive(!item.isDead() && pow > 0);
			defenseObj.SetActive(!item.isDead() && def > 0);

			power.text = pow.ToString();
			defense.text = def.ToString();
		}

		/// <summary>
		/// 绘制状态
		/// </summary>
		/// <param name="item"></param>
		void drawStates(RuntimeBattler item) {
			states.gameObject.SetActive(!item.isDead());
			states.setItems(item.allRuntimeStates());
		}

		/// <summary>
		/// 清除物品
		/// </summary>
		protected override void drawEmptyItem() {
            battle.gameObject.SetActive(false);
			if (hpBar) {
				hpBar.clearValue();
				hpBarObj.SetActive(false);
			}
			states.clearItems();

			powerObj.SetActive(false);
			defenseObj.SetActive(false);

			power.text = defense.text = "";
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

		#region 结果控制

		/// <summary>
		/// 处理抽卡显示
		/// </summary>
		void processDrawCards() {
			battleGround()?.processDrawCards();
		}

		#endregion

	}
}