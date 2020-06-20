
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;
using Core.UI.Utils;

using ExerPro.EnglishModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.BattleScene.Controls.Battler {

	/// <summary>
	/// 据点显示控件
	/// </summary
	public class HPResultDisplay : ItemDisplay<RuntimeBattler.DeltaHP> {

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
		/*
		/// <summary>
		/// 配置
		/// </summary>
		public void configure(BattlerDisplay battler) {
			this.battler = battler;
			configure();
		}
		*/
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
			if (!animation.isPlaying) requestDestroy = true;
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 是否为伤害
		/// </summary>
		/// <returns></returns>
		public bool isDamage() {
			return item.value < 0;
		}

		/// <summary>
		/// 是否为恢复
		/// </summary>
		/// <returns></returns>
		public bool isRecover() {
			return item.value > 0;
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
		protected override void drawExactlyItem(RuntimeBattler.DeltaHP item) {
            base.drawExactlyItem(item);

			if (isDamage()) drawDamage(item);
			else if (isRecover()) drawRecover(item);
			else requestDestroy = true;
		}

		/// <summary>
		/// 绘制伤害点数
		/// </summary>		
		/// <param name="item">结果</param>
		void drawDamage(RuntimeBattler.DeltaHP item) {
			Debug.Log(name + ": drawDamage: " + item.value);

			animation.Play(DamageAnimation);
			drawValue(Mathf.Abs(item.value));
		}

		/// <summary>
		/// 绘制恢复点数
		/// </summary>
		/// <param name="item">结果</param>
		void drawRecover(RuntimeBattler.DeltaHP item) {
			Debug.Log(name + ": drawRecover: " + item.value);

			animation.Play(RecoverAnimation);
			drawValue(Mathf.Abs(item.value));
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
		}

        #endregion

    }
}