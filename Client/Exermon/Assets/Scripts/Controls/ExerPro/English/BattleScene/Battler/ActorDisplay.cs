
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

		/// <summary>
		/// 是否抽卡
		/// </summary>
		bool _isDrawCards = false;
		public bool isDrawCards {
			get {
				var res = _isDrawCards;
				_isDrawCards = false; return res;
			}
		}

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			scene = SceneUtils.getCurrentScene<BattleScene>();
		}

		#endregion

		#region 更新控制

		/// <summary>
		/// 处理结果
		/// </summary>
		/// <param name="result"></param>
		protected override void processResult(RuntimeActionResult result) {
			if (result.drawCardCnt > 0) processDrawCards();
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

		#region 结果控制

		/// <summary>
		/// 处理抽卡显示
		/// </summary>
		void processDrawCards() {
			Debug.Log("processDrawCards");
			_isDrawCards = true;
		}

		#endregion
	}
}