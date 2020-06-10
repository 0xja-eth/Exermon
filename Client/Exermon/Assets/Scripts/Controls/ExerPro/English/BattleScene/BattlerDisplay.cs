
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
	/// 据点显示控件
	/// </summary
	public class BattlerDisplay :
		SelectableItemDisplay<RuntimeBattler> {

		/// <summary>
		/// 受击动画
		/// </summary>
		const string IdleAnimation = "Idle";
		const string HurtedAnimation = "Hurt";
		const string AttackAnimation = "Attack";
		const string MovingAnimation = "Move";

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Image full;

		public HPResultsContainer hpResults;

		public AnimationItem animation;

		public MultParamsDisplay expBar;

		/// <summary>
		/// 外部系统定义
		/// </summary>
		BattleService battleSer;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		RuntimeActionResult currentResult;

		#region 初始化

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
		}

		/// <summary>
		/// 更新行动
		/// </summary>
		void updateAction() {
		}

		/// <summary>
		/// 更新结果
		/// </summary>
		void updateResult() {
			if ((currentResult = item.getResult()) == null) return;

			hpResults.addItem(currentResult);
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 是否为敌人
		/// </summary>
		/// <returns></returns>
		public virtual bool isEnemy() {
			return false;
		}

		/// <summary>
		/// 是否激活
		/// </summary>
		/// <returns></returns>
		public override bool isActived() {
			return isEnemy() && base.isActived();
		}

		#endregion

		#region 界面控制

		#region 状态控制



		#endregion

		/// <summary>
		/// 绘制确切物品
		/// </summary>
		/// <param name="item">题目</param>
		protected override void drawExactlyItem(RuntimeBattler item) {
            base.drawExactlyItem(item);
			expBar?.setValue(item, "hp");
		}

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem() {
            full.gameObject.SetActive(false);
        }

        #endregion

    }
}