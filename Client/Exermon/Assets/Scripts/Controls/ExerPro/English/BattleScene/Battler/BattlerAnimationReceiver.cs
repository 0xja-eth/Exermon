
using System;

using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;
using Core.UI;

namespace UI.ExerPro.EnglishPro.BattleScene.Controls.Battler {

	/// <summary>
	/// 动画事件接收控件
	/// </summary
	public class BattlerAnimationReceiver : BaseView {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public BattlerDisplay battler;

		/// <summary>
		/// 击中回调
		/// </summary>
		public void onHit() {
			Debug.Log(name + ": onHit");
			battler.onHit();
		}
		
		/// <summary>
		/// 产生结果
		/// </summary>
		public void onResult() {
			Debug.Log(name + ": onResult");
			battler.onResult();
		}

		/// <summary>
		/// 死亡回调
		/// </summary>
		public void onDead() {
			battler.terminateView();
		}

	}
}