
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
	public class OnHitReceiver : BaseView {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public BattlerDisplay battler;

		/// <summary>
		/// 击中回调
		/// </summary>
		public void onHit() {
			battler.onHit();
		}
		
		/// <summary>
		/// 产生结果
		/// </summary>
		public void onResult() {
			battler.onResult();
		}

	}
}