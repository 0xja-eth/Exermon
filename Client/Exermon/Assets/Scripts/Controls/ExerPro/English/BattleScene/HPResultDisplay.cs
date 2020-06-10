
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;
using Core.UI.Utils;

using ExerPro.EnglishModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.BattleScene.Controls {

	/// <summary>
	/// 据点显示控件
	/// </summary
	public class HPResultDisplay : ItemDisplay<RuntimeActionResult> {

		/// <summary>
		/// 受击动画
		/// </summary>
		const string DamageAnimation = "Damage";
		const string RecoverAnimation = "Recover";

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Text value;

		public Animation animation;

		/// <summary>
		/// 内部组件定义
		/// </summary>
		BattlerDisplay battler;

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			animation = animation ?? SceneUtils.ani(gameObject);
		}
		
		/// <summary>
		/// 配置
		/// </summary>
		public void configure(BattlerDisplay battler) {
			this.battler = battler;
			configure();
		}

		#endregion

		#region 更新控制

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			uodateAnimation();
		}

		/// <summary>
		/// 更新动画
		/// </summary>
		void uodateAnimation() {
			if (!animation.isPlaying)
				drawEmptyItem();
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 是否为目标
		/// </summary>
		/// <returns></returns>
		public bool isObject() {
			return item.action.object_ == getBattler();
		}

		/// <summary>
		/// 是否为物品
		/// </summary>
		/// <returns></returns>
		public bool isSubject() {
			return item.action.subject == getBattler();
		}

		/// <summary>
		/// 是否为伤害
		/// </summary>
		/// <returns></returns>
		public bool isDamage() {
			return item.hpDamage > 0 && isObject() ||
				item.hpDrain > 0 && isObject();
		}

		/// <summary>
		/// 是否为恢复
		/// </summary>
		/// <returns></returns>
		public bool isRecover() {
			return item.hpRecover > 0 && isObject() ||
				item.hpDrain > 0 && isSubject();
		}

		/// <summary>
		/// 获取伤害值
		/// </summary>
		/// <returns></returns>
		public int getDamageValue() {
			if (item.hpDamage > 0 && isObject()) return item.hpDamage;
			if (item.hpDrain > 0 && isObject()) return item.hpDrain;
			return 0;
		}

		/// <summary>
		/// 获取恢复值
		/// </summary>
		/// <returns></returns>
		public int getRecoverValue() {
			if (item.hpRecover > 0 && isObject()) return item.hpRecover;
			if (item.hpDrain > 0 && isSubject()) return item.hpDrain;
			return 0;
		}

		/// <summary>
		/// 获取战斗者
		/// </summary>
		/// <returns></returns>
		public RuntimeBattler getBattler() {
			return battler.getItem();
		}

		#endregion

		#region 界面控制
		
		/// <summary>
		/// 绘制确切物品
		/// </summary>
		/// <param name="item">结果</param>
		protected override void drawExactlyItem(RuntimeActionResult item) {
            base.drawExactlyItem(item);
			if (isDamage()) drawDamage(item);
			else if (isRecover()) drawRecover(item);
			else drawEmptyItem();
		}

		/// <summary>
		/// 绘制伤害点数
		/// </summary>		
		/// <param name="item">结果</param>
		void drawDamage(RuntimeActionResult item) {
			animation.Play(DamageAnimation);
			drawValue(getDamageValue());
		}

		/// <summary>
		/// 绘制恢复点数
		/// </summary>
		/// <param name="item">结果</param>
		void drawRecover(RuntimeActionResult item) {
			animation.Play(RecoverAnimation);
			drawValue(getRecoverValue());
		}

		/// <summary>
		/// 绘制值
		/// </summary>
		/// <param name="value"></param>
		void drawValue(int value) {
			if (value <= 0) this.value.text = "";
			else this.value.text = value.ToString();
		}

		/// <summary>
		/// 清除物品
		/// </summary>
		protected override void drawEmptyItem() {
			animation.Stop();
			value.text = "";
			requestDestroy = true;
		}

        #endregion

    }
}