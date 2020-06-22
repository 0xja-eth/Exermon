
using System;

using UnityEngine;

using Core.UI.Utils;

using ExerPro.EnglishModule.Data;

/// <summary>
/// 特训战斗场景控件
/// </summary>
namespace UI.ExerPro.EnglishPro.BattleScene.Controls { }

/// <summary>
/// 特训战斗场景战斗者控件
/// </summary>
namespace UI.ExerPro.EnglishPro.BattleScene.Controls.Battler {

	/// <summary>
	/// 角色显示控件
	/// </summary
	public class ActorDisplay : BattlerDisplay {

		/// <summary>
		/// 场景组件引用
		/// </summary>
		BattleScene scene;

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			scene = SceneUtils.getCurrentScene<BattleScene>();
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 是否空物品
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public override bool isNullItem(RuntimeBattler item) {
			return base.isNullItem(item) || !isActor();
		}

		#endregion

		#region 画面绘制

		/// <summary>
		/// 绘制物品
		/// </summary>
		/// <param name="item"></param>
		protected override void drawExactlyItem(RuntimeBattler item) {
			base.drawExactlyItem(item);
			scene.refreshStatus();
		}

		#endregion
	}
}