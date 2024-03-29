﻿
using System;

using UnityEngine;
using UnityEngine.EventSystems;

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

	using Menu;

	/// <summary>
	/// 角色显示控件
	/// </summary
	public class ActorDisplay : BattlerDisplay, IDropHandler {

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

		#region 事件控制

		/// <summary>
		/// 拖拽释放回调
		/// </summary>
		public void OnDrop(PointerEventData eventData) {
			var display = getPotionDisplay(eventData);
			Debug.Log("Dragger: " + display);
			display?.use();
		}

		/// <summary>
		/// 获取拖拽中的物品显示项
		/// </summary>
		/// <param name="data">事件数据</param>
		/// <returns>物品显示项</returns>
		PotionSlotItemDisplay getPotionDisplay(PointerEventData data) {
			Debug.Log("data.pointerDrag: " + data.pointerDrag);
			var obj = data.pointerDrag; if (obj == null) return null;
			return SceneUtils.get<PotionSlotItemDisplay>(obj);
		}

		#endregion
	}
}